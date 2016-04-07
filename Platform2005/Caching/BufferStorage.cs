namespace Platform.Caching
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public sealed class BufferStorage
    {
        private ArrayList allAr;
        private static ArrayList allStorage = new ArrayList();
        private ArrayList freeAr;
        private object[] m_Args;
        private bool m_FinalReleased;
        private Type m_Type;

        public event BufferStorageAfterFinalReleaseEventHandler AfterFinallyRelease;

        public event BufferStorageBeforeFinalReleaseEventHandler BeforeFinallyRelease;

        public event BufferStorageCreateItemEventHandler ItemCreated;

        public BufferStorage(Type type)
        {
            this.allAr = new ArrayList();
            this.freeAr = new ArrayList();
            if ((type == null) || (type.GetInterface("Platform.Caching.IBufferItem") == null))
            {
                throw new Exception("无效缓存类型：" + type.FullName);
            }
            allStorage.Add(this);
            this.m_Type = type;
        }

        public BufferStorage(Type type, object[] args)
        {
            this.allAr = new ArrayList();
            this.freeAr = new ArrayList();
            if ((type == null) || (type.GetInterface("Platform.Caching.IBufferItem") == null))
            {
                throw new Exception("无效缓存类型：" + type.FullName);
            }
            allStorage.Add(this);
            this.m_Type = type;
            this.m_Args = args;
        }

        public void FinalRelease()
        {
            this.m_FinalReleased = true;
            if (this.BeforeFinallyRelease != null)
            {
                this.BeforeFinallyRelease(this.allAr);
            }
            lock (this.allAr.SyncRoot)
            {
                lock (this.freeAr.SyncRoot)
                {
                    foreach (IBufferItem item in this.allAr)
                    {
                        item.FinalRelease();
                    }
                    this.freeAr.Clear();
                    this.allAr.Clear();
                }
            }
            if (this.AfterFinallyRelease != null)
            {
                this.AfterFinallyRelease();
            }
        }

        public static void FinalReleaseAll()
        {
            foreach (BufferStorage storage in allStorage)
            {
                storage.FinalRelease();
            }
        }

        public void FinalReset()
        {
            this.m_FinalReleased = true;
            if (this.BeforeFinallyRelease != null)
            {
                this.BeforeFinallyRelease(this.allAr);
            }
            lock (this.allAr.SyncRoot)
            {
                lock (this.freeAr.SyncRoot)
                {
                    foreach (IBufferItem item in this.allAr)
                    {
                        item.FinalRelease();
                    }
                    this.freeAr.Clear();
                    this.allAr.Clear();
                }
            }
            if (this.AfterFinallyRelease != null)
            {
                this.AfterFinallyRelease();
            }
            this.m_FinalReleased = false;
        }

        public void FreeBuffer(IBufferItem item)
        {
            if (item.GetType() == this.m_Type)
            {
                lock (this.freeAr.SyncRoot)
                {
                    this.freeAr.Add(item);
                }
            }
        }

        public IBufferItem GetBuffer()
        {
            IBufferItem item;
            if (this.m_FinalReleased)
            {
                return null;
            }
            if (this.freeAr.Count > 0)
            {
                lock (this.freeAr.SyncRoot)
                {
                    if (this.freeAr.Count > 0)
                    {
                        item = this.freeAr[this.freeAr.Count - 1] as IBufferItem;
                        this.freeAr.RemoveAt(this.freeAr.Count - 1);
                        return item;
                    }
                }
            }
            item = Activator.CreateInstance(this.m_Type, this.m_Args) as IBufferItem;
            lock (this.allAr.SyncRoot)
            {
                this.allAr.Add(item);
            }
            if (this.ItemCreated != null)
            {
                this.ItemCreated(item);
            }
            return item;
        }

        public void PreStoreItem(int count)
        {
            if (!this.m_FinalReleased && (count >= 1))
            {
                IBufferItem[] c = new IBufferItem[count];
                for (int i = 0; i < count; i++)
                {
                    c[i] = Activator.CreateInstance(this.m_Type, this.m_Args) as IBufferItem;
                    if (this.ItemCreated != null)
                    {
                        this.ItemCreated(c[i]);
                    }
                }
                this.freeAr.AddRange(c);
                this.allAr.AddRange(c);
            }
        }

        public void Remove(IBufferItem item)
        {
            if ((item.GetType() == this.m_Type) && this.freeAr.Contains(item))
            {
                lock (this.freeAr.SyncRoot)
                {
                    this.freeAr.Remove(item);
                }
            }
        }

        public ArrayList AllItems
        {
            get
            {
                return this.allAr;
            }
        }

        public object[] Args
        {
            get
            {
                return this.m_Args;
            }
            set
            {
                this.m_Args = value;
            }
        }

        public bool FinalReleased
        {
            get
            {
                return this.m_FinalReleased;
            }
            set
            {
                this.m_FinalReleased = value;
            }
        }

        public ArrayList FreeItems
        {
            get
            {
                return this.freeAr;
            }
        }
    }
}

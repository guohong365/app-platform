namespace Platform.Caching
{
    using System;
    using System.Collections;

    public sealed class MultiBufferStorage
    {
        private Type m_DefaultType;
        private Hashtable m_StarageTable = new Hashtable();

        public void FinalRelease()
        {
            lock (this.m_StarageTable.SyncRoot)
            {
                foreach (BufferStorage storage in this.m_StarageTable.Values)
                {
                    storage.FinalRelease();
                }
                this.m_StarageTable.Clear();
            }
        }

        public void FreeBuffer(IBufferItem item)
        {
            Type type = item.GetType();
            this.GetBufferStorage(type).FreeBuffer(item);
        }

        public object GetBuffer()
        {
            return this.GetBuffer(this.m_DefaultType);
        }

        public object GetBuffer(Type type)
        {
            return this.GetBufferStorage(type).GetBuffer();
        }

        public BufferStorage GetBufferStorage(Type type)
        {
            return this.GetBufferStorage(type, null);
        }

        public BufferStorage GetBufferStorage(Type type, object[] args)
        {
            BufferStorage storage;
            lock (this.m_StarageTable.SyncRoot)
            {
                storage = this.m_StarageTable[type] as BufferStorage;
                if (storage != null)
                {
                    return storage;
                }
            }
            if ((type == null) || (type.GetInterface("Platform.Caching.IBufferItem") == null))
            {
                throw new Exception("无效缓存类型：" + type.FullName);
            }
            storage = new BufferStorage(type, args);
            lock (this.m_StarageTable.SyncRoot)
            {
                this.m_StarageTable[type] = storage;
            }
            return storage;
        }

        public void SetArguments(Type type, object[] args)
        {
            if ((type == null) || (type.GetInterface("Platform.Caching.IBufferItem") == null))
            {
                throw new Exception("无效缓存类型：" + type.FullName);
            }
            this.GetBufferStorage(type).Args = args;
        }

        public void SetDefaultType(Type type)
        {
            if ((type == null) || (type.GetInterface("Platform.Caching.IBufferItem") == null))
            {
                throw new Exception("无效缓存类型：" + type.FullName);
            }
            this.m_DefaultType = type;
        }
    }
}

namespace Platform.AutoDispose
{
    using Platform;
    using Platform.Threading;
    using System;
    using System.Collections;
    using System.Threading;

    public sealed class AutoDisposeManager
    {
        private static Hashtable m_ItemList = new Hashtable();

        static AutoDisposeManager()
        {
            Platform.Threading.ThreadPool.Run(new WaitCallback(AutoDisposeManager.CleanTimeoutItem));
        }

        public static void AddItem(IAutoDispose item)
        {
            lock (m_ItemList.SyncRoot)
            {
                m_ItemList[item.Index] = item;
            }
        }

        public static void CheckInItem(IAutoDispose item)
        {
            lock (m_ItemList.SyncRoot)
            {
                if (!m_ItemList.ContainsKey(item.Index))
                {
                    m_ItemList[item.Index] = item;
                }
                item.IsCheckOut = false;
            }
        }

        public static IAutoDispose CheckOutItem(Guid index)
        {
            lock (m_ItemList.SyncRoot)
            {
                IAutoDispose dispose = m_ItemList[index] as IAutoDispose;
                if (dispose == null)
                {
                    return null;
                }
                dispose.IsCheckOut = true;
                return dispose;
            }
        }

        private static void CleanTimeoutItem(object state)
        {
            while (PlatformConfig.AppRuning)
            {
                Thread.Sleep(0x3e8);
                lock (m_ItemList.SyncRoot)
                {
                    if (m_ItemList.Count < 1)
                    {
                        continue;
                    }
                    ArrayList list = new ArrayList();
                    foreach (IAutoDispose dispose in m_ItemList.Values)
                    {
                        try
                        {
                            if (dispose.IsTimeout && !dispose.IsCheckOut)
                            {
                                dispose.Dispose();
                                list.Add(dispose);
                            }
                            continue;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    foreach (IAutoDispose dispose2 in list)
                    {
                        m_ItemList.Remove(dispose2.Index);
                    }
                    continue;
                }
            }
        }

        public static void RemoveItem(IAutoDispose item)
        {
            lock (m_ItemList.SyncRoot)
            {
                m_ItemList.Remove(item.Index);
            }
        }
    }
}

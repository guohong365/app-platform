namespace Platform.Threading
{
    using System;
    using System.Collections;
    using System.Threading;

    public sealed class ThreadSlotManager
    {
        private static Hashtable m_Slots = new Hashtable();

        public static void ClearSlotData(string slotName)
        {
            if ((slotName != null) && (slotName != ""))
            {
                LocalDataStoreSlot slot = m_Slots[slotName] as LocalDataStoreSlot;
                if (slot != null)
                {
                    Thread.SetData(slot, null);
                }
            }
        }

        public static bool FreeThreadSlot(string slotName)
        {
            if ((slotName == null) || (slotName == ""))
            {
                return false;
            }
            lock (m_Slots.SyncRoot)
            {
                if (!m_Slots.ContainsKey(slotName))
                {
                    return false;
                }
                Thread.FreeNamedDataSlot(slotName);
                m_Slots.Remove(slotName);
                return true;
            }
        }

        public static object GetSlotData(string slotName)
        {
            if ((slotName == null) || (slotName == ""))
            {
                return null;
            }
            LocalDataStoreSlot slot = m_Slots[slotName] as LocalDataStoreSlot;
            if (slot == null)
            {
                return null;
            }
            return Thread.GetData(slot);
        }

        public static bool SetSlotData(string slotName, object data)
        {
            LocalDataStoreSlot slot = SetupThreadSlot(slotName);
            if (slot == null)
            {
                return false;
            }
            Thread.SetData(slot, data);
            return true;
        }

        public static LocalDataStoreSlot SetupThreadSlot(string slotName)
        {
            if ((slotName == null) || (slotName == ""))
            {
                return null;
            }
            LocalDataStoreSlot namedDataSlot = m_Slots[slotName] as LocalDataStoreSlot;
            if (namedDataSlot != null)
            {
                return namedDataSlot;
            }
            lock (m_Slots.SyncRoot)
            {
                namedDataSlot = m_Slots[slotName] as LocalDataStoreSlot;
                if (namedDataSlot == null)
                {
                    namedDataSlot = Thread.GetNamedDataSlot(slotName);
                    m_Slots[slotName] = namedDataSlot;
                }
                return namedDataSlot;
            }
        }
    }
}

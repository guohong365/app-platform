namespace Platform.Hotkey
{
    using System;
    using System.Collections;
    using System.Threading;
    using System.Windows.Forms;

    public sealed class HotkeyManager : IMessageFilter
    {
        private static Hashtable m_KeyMaps = new Hashtable();

        static HotkeyManager()
        {
            Application.AddMessageFilter(new HotkeyManager());
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != 0x100)
            {
                return false;
            }
            Keys wParam = (Keys) ((int) m.WParam);
            if (!m_KeyMaps.ContainsKey(wParam))
            {
                return false;
            }
            ArrayList list = m_KeyMaps[wParam] as ArrayList;
            if (list == null)
            {
                return false;
            }
            foreach (KeyMap map in list)
            {
                if ((map == null) || (map.Handler == null))
                {
                    continue;
                }
                try
                {
                    map.Handler(map.State);
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            return true;
        }

        public static bool RegisterHotkey(Keys key, WaitCallback handler, object state)
        {
            if (handler == null)
            {
                return false;
            }
            ArrayList list = m_KeyMaps[key] as ArrayList;
            if (list == null)
            {
                list = new ArrayList();
                m_KeyMaps[key] = list;
            }
            list.Add(new KeyMap(handler, state));
            return true;
        }

        public static void RemoveHotkey(Keys key, WaitCallback handler)
        {
            if (handler == null)
            {
                m_KeyMaps.Remove(key);
            }
            else
            {
                ArrayList list = m_KeyMaps[key] as ArrayList;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        KeyMap map = list[i] as KeyMap;
                        if (map.Handler == handler)
                        {
                            list.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        private class KeyMap
        {
            public WaitCallback Handler;
            public object State;

            public KeyMap(WaitCallback handler, object state)
            {
                this.Handler = handler;
                this.State = state;
            }
        }
    }
}

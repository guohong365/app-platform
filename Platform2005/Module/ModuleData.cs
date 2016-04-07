namespace Platform.Module
{
    using System;

    internal sealed class ModuleData
    {
        public string Name;
        public int Order;
        public string Path;

        private ModuleData(string name, string configText)
        {
            this.Name = name;
            string[] textArray = configText.Trim().Split(new char[] { ';' });
            this.Order = int.Parse(textArray[0]);
            this.Path = textArray[1];
        }

        public static ModuleData GetModuleData(string name, string configText)
        {
            try
            {
                return new ModuleData(name, configText);
            }
            catch
            {
                return null;
            }
        }
    }
}

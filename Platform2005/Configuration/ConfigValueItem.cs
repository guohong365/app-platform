namespace Platform.Configuration
{
    using System;

    public sealed class ConfigValueItem
    {
        public string Description;
        public string Value;

        public ConfigValueItem(string value)
        {
            this.Value = value;
            this.Description = "";
        }

        public ConfigValueItem(string value, string description)
        {
            this.Value = value;
            this.Description = description;
        }
    }
}

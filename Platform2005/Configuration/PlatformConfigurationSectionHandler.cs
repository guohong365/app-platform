namespace Platform.Configuration
{
    using System;
    using System.Configuration;
    using System.Xml;

    public sealed class PlatformConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            if (section.Attributes.GetNamedItem("description") != null)
            {
                return new NameValueSectionHandler().Create(parent, configContext, section.Attributes.RemoveNamedItem("description"));
            }
            return new NameValueSectionHandler().Create(parent, configContext, section);
        }
    }
}

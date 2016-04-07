namespace Platform.Module
{
    using System;
    using System.Configuration;
    using System.Xml;

    public sealed class PlatformModuleSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return null;
        }
    }
}

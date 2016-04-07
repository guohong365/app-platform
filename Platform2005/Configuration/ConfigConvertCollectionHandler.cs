namespace Platform.Configuration
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public delegate object ConfigConvertCollectionHandler(Hashtable values, ConfigCollectionItemAttribute attr);
}

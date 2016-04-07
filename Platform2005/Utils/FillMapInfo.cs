namespace Platform.Utils
{
    using System;

    public sealed class FillMapInfo
    {
        private FillConvertHandler converter;
        private string destField;

        public FillMapInfo(FillConvertHandler converter)
        {
            this.destField = null;
            this.converter = converter;
        }

        public FillMapInfo(string destField)
        {
            this.destField = destField;
            this.converter = null;
        }

        public FillMapInfo(string destField, FillConvertHandler converter)
        {
            this.destField = destField;
            this.converter = converter;
        }

        public FillConvertHandler Converter
        {
            get
            {
                return this.converter;
            }
        }

        public string DestField
        {
            get
            {
                return this.destField;
            }
        }
    }
}

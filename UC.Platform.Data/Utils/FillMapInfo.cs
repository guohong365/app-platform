namespace UC.Platform.Data.Utils
{
    public sealed class FillMapInfo
    {
        private readonly FillConvertHandler _converter;
        private readonly string _destField;

        public FillMapInfo(FillConvertHandler converter)
        {
            _destField = null;
            _converter = converter;
        }

        public FillMapInfo(string destField)
        {
            _destField = destField;
            _converter = null;
        }

        public FillMapInfo(string destField, FillConvertHandler converter)
        {
            _destField = destField;
            _converter = converter;
        }

        public FillConvertHandler Converter
        {
            get
            {
                return _converter;
            }
        }

        public string DestField
        {
            get
            {
                return _destField;
            }
        }
    }
}

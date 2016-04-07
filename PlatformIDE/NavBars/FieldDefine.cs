namespace PlatformIDE.NavBars
{
    using Platform.CSS.Packet;
    using System;

    public class FieldDefine
    {
        private static string[] fixtemplate = new string[] { "{ get{ $GetHead BitConverter.ToUInt64( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt64( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToUInt32( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 4 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt32( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 4 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToUInt16( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 2 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt16( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 2 ); $SetEnd }}", "{ get{ $GetHead m_Fields[$Index][0]; $GetEnd } set{ $SetHead m_Fields[$Index][0] = value; $SetEnd }}", "{ get{ $GetHead Platform.PlatformConfig.TextEncoding.GetString( m_Fields[$Index] ); $GetEnd } set{ $SetHead Buffer.BlockCopy( Platform.PlatformConfig.TextEncoding.GetBytes( value ) , 0 , m_Fields[$Index] , _Fields[$Index].Length ); $SetEnd }}", "{ get{ $GetHead m_Fields[$Index]; $GetEnd } set{ $SetHead Buffer.BlockCopy( value , 0 , m_Fields[$Index] , 0 , m_Fields[$Index].Length ); $SetEnd }}", "{ get{ $GetHead new DateTime( BitConverter.ToInt64( m_Fields[$Index] , 0 ) ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value.Ticks ) , 0 , m_Fields[$Index] , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead new Guid( m_Fields[$Index] ); $GetEnd } set{ $SetHead m_Fields[$Index] = value.ToByteArray(); $SetEnd }}" };
        private static int[] lens = new int[] { 8, 8, 4, 4, 2, 2, 1, 0, 0, 8, 0x10 };
        public int m_DataType;
        public bool m_Enable;
        public bool m_Fixed;
        public int m_Index;
        public int m_Length;
        public string m_Name;
        private static string[] names = new string[] { "ulong", "long", "uint", "int", "ushort", "short", "byte", "string", "byte[]", "DateTime", "Guid" };
        private static string[] unfixtemplate = new string[] { "{ get{ $GetHead BitConverter.ToUInt64( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt64( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToUInt32( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 4 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt32( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 4 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToUInt16( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 2 ); $SetEnd }}", "{ get{ $GetHead BitConverter.ToInt16( m_Fields[$Index] , 0 ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value ) , 0 , m_Fields[$Index]  , 0 , 2 ); $SetEnd }}", "{ get{ $GetHead m_Fields[$Index][0]; $GetEnd } set{ $SetHead m_Fields[$Index][0] = value; $SetEnd }}", "{ get{ $GetHead Platform.PlatformConfig.TextEncoding.GetString( m_Fields[$Index] ); $GetEnd } set{ $SetHead m_Fields[$Index] = Platform.PlatformConfig.TextEncoding.GetBytes( value ); $SetEnd }}", "{ get{ $GetHead m_Fields[$Index]; $GetEnd } set{ $SetHead m_Fields[$Index] = value; $SetEnd }}", "{ get{ $GetHead new DateTime( BitConverter.ToInt64( m_Fields[$Index] , 0 ) ); $GetEnd } set{ $SetHead Buffer.BlockCopy( BitConverter.GetBytes( value.Ticks ) , 0 , m_Fields[$Index] , 0 , 8 ); $SetEnd }}", "{ get{ $GetHead new Guid( m_Fields[$Index] ); $GetEnd } set{ $SetHead m_Fields[$Index] = value.ToByteArray(); $SetEnd }}" };

        private FieldDefine()
        {
            this.m_Enable = false;
        }

        public FieldDefine(int index)
        {
            this.m_Enable = false;
            this.Index = index;
        }

        public void Clear()
        {
            this.m_Enable = false;
            this.m_Name = "";
            this.m_DataType = 0;
            this.m_Length = 0;
            this.m_Fixed = true;
        }

        public static FieldDefine Deserial(PacketFieldAttribute attr)
        {
            FieldDefine define = new FieldDefine();
            define.Index = attr.Index;
            define.Enable = true;
            define.Name = attr.Name;
            define.DataType = attr.Type;
            define.Length = attr.Length;
            define.Fixed = attr.Fixed;
            return define;
        }

        public static FieldDefine Deserial(string s)
        {
            string text = s.Trim();
            try
            {
                string[] textArray = text.Split(new char[] { ' ' });
                if (textArray.Length != 6)
                {
                    return null;
                }
                FieldDefine define = new FieldDefine();
                define.Index = int.Parse(textArray[0]);
                define.Enable = bool.Parse(textArray[1]);
                define.Name = textArray[2].Trim();
                define.DataType = int.Parse(textArray[3]);
                define.Length = int.Parse(textArray[4]);
                define.Fixed = bool.Parse(textArray[5]);
                return define;
            }
            catch
            {
                return null;
            }
        }

        public int GetDefaultLen()
        {
            return lens[this.DataType];
        }

        public string GetDefine(string head, int count)
        {
            string text = this.ToString();
            string text4 = "";
            text4 = text4 + head + "#region " + text + "\r\n";
            text4 = text4 + head + "private bool\tIs" + this.Name + "UnOK\t\t= true;\r\n";
            object obj2 = text4 + head + "private " + names[this.DataType] + " m_" + this.Name + ";\r\n";
            object[] objArray = new object[] { obj2, head, "[PacketField( ", this.Index, " , 0X", (((ulong)1) << this.Index).ToString("X16"), "UL , \"", this.Name, "\" , ", this.DataType, " , ", this.Length, " , ", this.Fixed.ToString().ToLower(), " )]\r\n" };
            text4 = string.Concat(objArray) + head + "[Category(\"域定义数据\"),Browsable(true)]\r\n";
            text4 = text4 + head + "[Description(\"" + text + "\")]\r\n";
            text4 = ((text4 + head + "public " + names[this.DataType] + " " + this.Name + "\r\n") + head + this.GetPropertyDefine(count) + "\r\n") + head + "[Category(\"域字节数据\"),Browsable(false)]\r\n";
            text4 = text4 + head + "[Description(\"" + text + "数据\")]\r\n";
            text4 = text4 + head + "public byte[] " + this.Name + "_数据\r\n";
            text4 = (text4 + head + "{ get{ return m_Fields[" + count.ToString() + "]; }}\r\n") + head + "[Category(\"域存在判断\"),Browsable(false)]\r\n";
            text4 = text4 + head + "[Description(\"" + text + "是否存在\")]\r\n";
            text4 = text4 + head + "public bool\tIs" + this.Name + "Enable\r\n";
            string[] textArray = new string[9];
            textArray[0] = text4;
            textArray[1] = head;
            textArray[2] = "{ get{ return ( m_Bitmap & 0X";
            textArray[3] = this.Bitmap.ToString("X16");
            textArray[4] = "UL ) != 0 ; } set{ if( value ) m_Bitmap |= 0X";
            textArray[5] = this.Bitmap.ToString("X16");
            textArray[6] = "UL; else m_Bitmap &= 0X";
            textArray[7] = this.Bitmap.ToString("X16");
            textArray[8] = "UL; }}\r\n";
            return (string.Concat(textArray) + head + "#endregion\r\n\r\n");
        }

        private string GetPropertyDefine(int count)
        {
            string text;
            if (this.Fixed)
            {
                text = fixtemplate[this.DataType];
            }
            else
            {
                text = unfixtemplate[this.DataType];
            }
            string newValue = "if( Is" + this.Name + "UnOK ){ Is" + this.Name + "UnOK = false;  m_" + this.Name + " = ";
            string text3 = "} return m_" + this.Name + ";";
            string text4 = "";
            string text5 = "Is" + this.Name + "UnOK = true;";
            return text.Replace("$Index", count.ToString()).Replace("$GetHead", newValue).Replace("$GetEnd", text3).Replace("$SetHead", text4).Replace("$SetEnd", text5);
        }

        public string GetRelease(string head)
        {
            return (head + "Is" + this.Name + "UnOK\t\t= true;");
        }

        public void Initialize(PacketFieldAttribute attr)
        {
            this.Index = attr.Index;
            this.Enable = true;
            this.Name = attr.Name;
            this.DataType = attr.Type;
            this.Length = attr.Length;
            this.Fixed = attr.Fixed;
        }

        public void Initialize(string s)
        {
            string text = s.Trim();
            try
            {
                string[] textArray = text.Split(new char[] { ' ' });
                if (textArray.Length == 6)
                {
                    int num = int.Parse(textArray[0]);
                    bool flag = bool.Parse(textArray[1]);
                    string text2 = textArray[2].Trim();
                    int num2 = int.Parse(textArray[3]);
                    int num3 = int.Parse(textArray[4]);
                    bool flag2 = bool.Parse(textArray[5]);
                    this.Index = num;
                    this.Enable = flag;
                    this.Name = text2;
                    this.DataType = num2;
                    this.Length = num3;
                    this.Fixed = flag2;
                }
            }
            catch
            {
            }
        }

        public bool IsOK()
        {
            if (this.Enable && ((this.Name == null) || (this.Name.Length < 1)))
            {
                return false;
            }
            return true;
        }

        public string Serial()
        {
            string text = "";
            return ((((((text + this.Index.ToString("D2") + " ") + this.Enable.ToString() + " ") + this.Name + " ") + this.DataType.ToString() + " ") + this.Length.ToString() + " ") + this.Fixed.ToString());
        }

        public override string ToString()
        {
            int num = this.Index + 1;
            return ("第 " + num.ToString("D2") + " 域 " + this.Name);
        }

        public ulong Bitmap
        {
            get
            {
                return (((ulong)1) << this.Index);
            }
        }

        public int DataType
        {
            get
            {
                return this.m_DataType;
            }
            set
            {
                this.m_DataType = value;
            }
        }

        public static string[] DataTypeNames
        {
            get
            {
                return names;
            }
        }

        public bool Enable
        {
            get
            {
                return this.m_Enable;
            }
            set
            {
                this.m_Enable = value;
            }
        }

        public bool Fixed
        {
            get
            {
                return this.m_Fixed;
            }
            set
            {
                this.m_Fixed = value;
            }
        }

        public int Index
        {
            get
            {
                return this.m_Index;
            }
            set
            {
                this.m_Index = value;
            }
        }

        public int Length
        {
            get
            {
                return this.m_Length;
            }
            set
            {
                this.m_Length = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
    }
}

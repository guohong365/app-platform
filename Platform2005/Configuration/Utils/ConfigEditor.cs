namespace Platform.Configuration.Utils
{
    using Platform.IO;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public class ConfigEditor : UserControl
    {
        private Container components;
        private ArrayList m_ConfigItems = new ArrayList();
        private string m_FileContent;
        private string m_Title;
        private PropertyGrid propertyGrid1;

        public ConfigEditor()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            this.propertyGrid1 = new PropertyGrid();
            base.SuspendLayout();
            this.propertyGrid1.CommandsVisibleIfAvailable = true;
            this.propertyGrid1.Dock = DockStyle.Fill;
            this.propertyGrid1.LargeButtons = false;
            this.propertyGrid1.LineColor = SystemColors.ScrollBar;
            this.propertyGrid1.Location = new Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new Size(0x1a8, 0x150);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.Text = "propertyGrid1";
            this.propertyGrid1.ViewBackColor = SystemColors.Window;
            this.propertyGrid1.ViewForeColor = SystemColors.WindowText;
            this.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            this.propertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            base.Controls.Add(this.propertyGrid1);
            base.Name = "ConfigEditor";
            base.Size = new Size(0x1a8, 0x150);
            base.ResumeLayout(false);
        }

        public bool LoadConfigModule(string fileName, Encoding encoding)
        {
            return this.LoadConfigModule(fileName, false, null, encoding);
        }

        public bool LoadConfigModule(string configName, Assembly asm, Encoding encoding)
        {
            return this.LoadConfigModule(configName, true, asm, encoding);
        }

        private bool LoadConfigModule(string configName, bool isResource, Assembly asm, Encoding encoding)
        {
            Regex regex;
            if (isResource)
            {
                if (asm == null)
                {
                    return false;
                }
                Stream manifestResourceStream = ResourceUtility.GetManifestResourceStream(configName, asm);
                if (manifestResourceStream == null)
                {
                    return false;
                }
                try
                {
                    using (StreamReader reader = new StreamReader(manifestResourceStream, encoding))
                    {
                        this.m_FileContent = reader.ReadToEnd();
                    }
                    goto Label_0077;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if (!File.Exists(configName))
            {
                return false;
            }
            try
            {
                using (StreamReader reader2 = new StreamReader(configName, encoding))
                {
                    this.m_FileContent = reader2.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return false;
            }
        Label_0077:
            regex = new Regex(@"CONFIG--\s{0,}(?<ConfigTitle>.*?)\s{0,}--CONFIG", RegexOptions.Multiline);
            this.m_Title = regex.Match(this.m_FileContent).Result("${ConfigTitle}");
            MatchCollection matchs = new Regex("\\s{0,}<\\s{0,}add\\s+key\\s{0,}=\\s{0,}\\\"(?<ConfigNativeName>.*?)\\\"\\s+value\\s{0,}=\\s{0,}\\\".*CONFIG--\\s{0,}(?<ConfigGroup>.*?)\\s{0,}:\\s{0,}(?<ConfigName>.*?)\\s{0,}:\\s{0,}(?<ConfigType>.*?)\\s{0,}:\\s{0,}(?<ConfigUIType>.*?)\\s{0,}:\\s{0,}(?<ConfigDefaultValue>.*?)\\s{0,}--CONFIG.*\\\"\\s+description\\s{0,}=\\s{0,}\\\"(?<ConfigDescription>.*?)\\\"\\s{0,}/>", RegexOptions.Multiline).Matches(this.m_FileContent);
            this.m_ConfigItems.Clear();
            foreach (Match match2 in matchs)
            {
                if (match2.Success)
                {
                    ConfigItem item = new ConfigItem(match2.Value, match2.Result("${ConfigNativeName}"), match2.Result("${ConfigName}"), match2.Result("${ConfigGroup}"), match2.Result("${ConfigType}"), match2.Result("${ConfigUIType}"), match2.Result("${ConfigDefaultValue}"), match2.Result("${ConfigDescription}"));
                    this.m_ConfigItems.Add(item);
                }
            }
            this.propertyGrid1.SelectedObject = new CustomProperties(this.m_ConfigItems);
            return true;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
        }

        public void Save(string path, Encoding encoding)
        {
            string fileContent = this.m_FileContent;
            fileContent = new Regex(@"CONFIG--\s{0,}" + this.m_Title + @"\s{0,}--CONFIG", RegexOptions.Multiline).Replace(fileContent, "");
            foreach (ConfigItem item in this.m_ConfigItems)
            {
                string newValue = new Regex(@"CONFIG--\s{0,}" + item.ConfigGroup + @"\s{0,}:\s{0,}" + item.ConfigName + @"\s{0,}:\s{0,}" + item.ConfigType + @"\s{0,}:\s{0,}" + item.ConfigUIType + @"\s{0,}:\s{0,}" + item.ConfigDefaultValue + @"\s{0,}--CONFIG", RegexOptions.Multiline).Replace(item.MatchValue, item.ConfigValue);
                fileContent = fileContent.Replace(item.MatchValue, newValue);
            }
            PathUtility.MakeFileDirectory(path);
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
            {
                writer.Write(fileContent);
                writer.Flush();
            }
        }

        public string ConfigName
        {
            get
            {
                return this.m_Title;
            }
        }
    }
}

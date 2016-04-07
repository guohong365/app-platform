namespace Platform.UI
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class ImageDiaplayForm : Form
    {
        private Button button_Close;
        private Button button_Next;
        private Button button_Pre;
        private PictureBoxControl cc;
        private Container components;
        private GroupBox groupBox1;
        private ArrayList m_ImageList;
        private int m_SelectIndex;
        private Panel panel1;
        private PictureBox pictureBox_Image;
        private Label t_page;

        public ImageDiaplayForm(string caption, ArrayList imageList)
        {
            this.cc = new PictureBoxControl();
            this.m_ImageList = new ArrayList();
            this.InitializeComponent();
            this.Text = caption;
            if (imageList != null)
            {
                this.m_ImageList = imageList;
            }
        }

        public ImageDiaplayForm(string caption, DataTable dt, string imageDataColumnName)
        {
            this.cc = new PictureBoxControl();
            this.m_ImageList = new ArrayList();
            this.InitializeComponent();
            this.Text = caption;
            if ((dt != null) && dt.Columns.Contains(imageDataColumnName))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (!row.IsNull(imageDataColumnName))
                    {
                        this.m_ImageList.Add(row[imageDataColumnName]);
                    }
                }
            }
        }

        public ImageDiaplayForm(string caption, DataSet ds, string tableName, string imageDataColumnName)
            : this(caption, ds.Tables.Contains(tableName) ? ds.Tables[tableName] : null, imageDataColumnName)
        {
        }

        private void btnXyy_Click(object sender, EventArgs e)
        {
            if (this.m_SelectIndex < (this.m_ImageList.Count - 1))
            {
                this.m_SelectIndex++;
            }
            this.ShowImage(this.m_SelectIndex);
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button_Pre_Click(object sender, EventArgs e)
        {
            if (this.m_SelectIndex > 0)
            {
                this.m_SelectIndex--;
            }
            this.ShowImage(this.m_SelectIndex);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ImageDiaplayForm_Load(object sender, EventArgs e)
        {
            this.cc.PictureBox = this.pictureBox_Image;
            if (this.m_ImageList.Count > 0)
            {
                this.ShowImage(this.m_SelectIndex);
            }
            else
            {
                this.t_page.Text = "无图象数据";
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
            this.panel1 = new Panel();
            this.pictureBox_Image = new PictureBox();
            this.button_Pre = new Button();
            this.groupBox1 = new GroupBox();
            this.button_Next = new Button();
            this.button_Close = new Button();
            this.t_page = new Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.pictureBox_Image);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x27a, 0x182);
            this.panel1.TabIndex = 1;
            this.pictureBox_Image.Dock = DockStyle.Fill;
            this.pictureBox_Image.Location = new Point(0, 0);
            this.pictureBox_Image.Name = "pictureBox_Image";
            this.pictureBox_Image.Size = new Size(0x27a, 0x182);
            this.pictureBox_Image.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox_Image.TabIndex = 0;
            this.pictureBox_Image.TabStop = false;
            this.button_Pre.Location = new Point(0x7e, 0x10);
            this.button_Pre.Name = "button_Pre";
            this.button_Pre.TabIndex = 2;
            this.button_Pre.Text = "上一页";
            this.button_Pre.Click += new EventHandler(this.button_Pre_Click);
            this.groupBox1.Controls.Add(this.button_Next);
            this.groupBox1.Controls.Add(this.button_Close);
            this.groupBox1.Controls.Add(this.button_Pre);
            this.groupBox1.Dock = DockStyle.Bottom;
            this.groupBox1.Location = new Point(0, 410);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x27a, 0x30);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.button_Next.AllowDrop = true;
            this.button_Next.Location = new Point(0x1b2, 0x10);
            this.button_Next.Name = "button_Next";
            this.button_Next.TabIndex = 4;
            this.button_Next.Text = "下一页";
            this.button_Next.Click += new EventHandler(this.btnXyy_Click);
            this.button_Close.Location = new Point(0x115, 0x10);
            this.button_Close.Name = "button_Close";
            this.button_Close.TabIndex = 3;
            this.button_Close.Text = "关闭";
            this.button_Close.Click += new EventHandler(this.button_Close_Click);
            this.t_page.BackColor = SystemColors.ControlLightLight;
            this.t_page.Dock = DockStyle.Bottom;
            this.t_page.Location = new Point(0, 0x182);
            this.t_page.Name = "t_page";
            this.t_page.Size = new Size(0x27a, 0x18);
            this.t_page.TabIndex = 4;
            this.t_page.TextAlign = ContentAlignment.MiddleCenter;
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(0x27a, 0x1ca);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.t_page);
            base.Controls.Add(this.groupBox1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ImageDiaplayForm";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "显示图像";
            base.Load += new EventHandler(this.ImageDiaplayForm_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void ShowImage(int index)
        {
            if ((index >= 0) && (index < this.m_ImageList.Count))
            {
                object obj2 = this.m_ImageList[index];
                try
                {
                    System.Type type = obj2.GetType();
                    this.pictureBox_Image.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (type.IsSubclassOf(typeof(Stream)))
                    {
                        this.cc.Image = Image.FromStream((Stream)obj2);
                    }
                    else if ((type == typeof(Image)) || type.IsSubclassOf(typeof(Image)))
                    {
                        this.cc.Image = (Image)obj2;
                    }
                    else if (type == typeof(byte[]))
                    {
                        MemoryStream stream = new MemoryStream((byte[])obj2);
                        this.cc.Image = Image.FromStream(stream);
                    }
                    this.m_SelectIndex = index;
                    if ((this.m_SelectIndex < 0) || (this.m_SelectIndex >= this.m_ImageList.Count))
                    {
                        this.button_Pre.Enabled = false;
                        this.button_Next.Enabled = false;
                    }
                    if (this.m_SelectIndex == 0)
                    {
                        this.button_Pre.Enabled = false;
                    }
                    else
                    {
                        this.button_Pre.Enabled = true;
                    }
                    if (this.m_SelectIndex == (this.m_ImageList.Count - 1))
                    {
                        this.button_Next.Enabled = false;
                    }
                    else
                    {
                        this.button_Next.Enabled = true;
                    }
                    this.t_page.Text = ((this.m_SelectIndex + 1)).ToString() + " / " + this.m_ImageList.Count;
                }
                catch
                {
                }
            }
        }

        public ArrayList ImageList
        {
            get
            {
                return this.m_ImageList;
            }
            set
            {
                this.m_ImageList = value;
            }
        }
    }
}

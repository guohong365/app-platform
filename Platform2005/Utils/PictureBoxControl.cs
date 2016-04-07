namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;

    public class PictureBoxControl
    {
        private System.Windows.Forms.PictureBox m_Box;
        private PictureBoxContexMenu m_ContexMenu;
        private bool m_DbclkEnale;
        private bool m_DragEnable;
        private System.Drawing.Image m_Image;
        private MenuItem[] m_MenuItems;
        private Point m_MouseDownPosition;
        private Panel m_Panel;
        private static Hashtable m_SaveFormats = new Hashtable();
        private const string PANNELNAME = "__PIC_CTRL__PANEL__";

        static PictureBoxControl()
        {
            m_SaveFormats["JPG"] = ImageFormat.Jpeg;
            m_SaveFormats["JPEG"] = ImageFormat.Jpeg;
            m_SaveFormats["JPE"] = ImageFormat.Jpeg;
            m_SaveFormats["JFIF"] = ImageFormat.Jpeg;
            m_SaveFormats["GIF"] = ImageFormat.Gif;
            m_SaveFormats["TIF"] = ImageFormat.Tiff;
            m_SaveFormats["TIFF"] = ImageFormat.Tiff;
            m_SaveFormats["PNG"] = ImageFormat.Png;
            m_SaveFormats["ICO"] = ImageFormat.Icon;
            m_SaveFormats["BMP"] = ImageFormat.Bmp;
        }

        public PictureBoxControl()
        {
            this.m_MenuItems = new MenuItem[3];
            this.m_DragEnable = true;
            this.m_DbclkEnale = true;
        }

        public PictureBoxControl(PictureBoxContexMenu contexMenu)
        {
            this.m_MenuItems = new MenuItem[3];
            this.m_DragEnable = true;
            this.m_DbclkEnale = true;
            this.m_ContexMenu = contexMenu;
        }

        public PictureBoxControl(System.Windows.Forms.PictureBox box)
        {
            this.m_MenuItems = new MenuItem[3];
            this.m_DragEnable = true;
            this.m_DbclkEnale = true;
            this.PictureBox = box;
        }

        public PictureBoxControl(System.Windows.Forms.PictureBox box, PictureBoxContexMenu contexMenu)
        {
            this.m_MenuItems = new MenuItem[3];
            this.m_DragEnable = true;
            this.m_DbclkEnale = true;
            this.m_ContexMenu = contexMenu;
            this.PictureBox = box;
        }

        private void Add()
        {
            if ((this.m_Box != null) && (this.m_Box.Parent != null))
            {
                Control parent = this.m_Box.Parent;
                parent.SuspendLayout();
                this.m_Box.DoubleClick += new EventHandler(this.Box_DoubleClick);
                this.m_Box.MouseDown += new MouseEventHandler(this.Box_MouseDown);
                this.m_Box.MouseMove += new MouseEventHandler(this.Box_MouseMove);
                this.m_Box.MouseUp += new MouseEventHandler(this.Box_MouseUp);
                this.m_Box.Paint += new PaintEventHandler(this.OnPaint);
                this.m_Panel = new Panel();
                this.m_Panel.Dock = this.m_Box.Dock;
                this.m_Panel.Width = this.m_Box.Width;
                this.m_Panel.Height = this.m_Box.Height;
                this.m_Panel.Location = this.m_Box.Location;
                this.m_Panel.BorderStyle = this.m_Box.BorderStyle;
                this.m_Panel.BackgroundImage = this.m_Box.BackgroundImage;
                this.m_Panel.BackColor = this.m_Box.BackColor;
                this.m_Panel.Name = "__PIC_CTRL__PANEL__";
                this.m_Panel.Tag = this;
                this.m_Box.BorderStyle = BorderStyle.None;
                this.m_Box.Location = new Point(this.m_Panel.Width + 1, this.m_Panel.Width + 1);
                int index = parent.Controls.IndexOf(this.m_Box);
                parent.Controls.Remove(this.m_Box);
                this.m_Box.Dock = DockStyle.None;
                parent.Controls.Add(this.m_Panel);
                parent.Controls.SetChildIndex(this.m_Panel, index);
                this.m_Panel.Controls.Add(this.m_Box);
                Application.Idle += new EventHandler(this.Application_Idle);
                this.m_Panel.SizeChanged += new EventHandler(this.Parent_SizeChanged);
                this.m_Panel.Paint += new PaintEventHandler(this.OnPaint);
                this.m_Panel.ControlRemoved += new ControlEventHandler(this.Panel_ControlRemoved);
                this.SetupMenu();
                parent.ResumeLayout(true);
                this.Image = this.m_Box.Image;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (((this.m_Panel == null) || (this.m_Box == null)) || (this.m_Panel.IsDisposed || this.m_Box.IsDisposed))
            {
                Application.Idle -= new EventHandler(this.Application_Idle);
            }
            else if (this.m_Image != this.m_Box.Image)
            {
                this.m_Image = this.m_Box.Image;
                if (this.m_Box.SizeMode == PictureBoxSizeMode.StretchImage)
                {
                    this.FixImage();
                }
                else
                {
                    this.m_Box.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.CenterImage();
                }
            }
        }

        private void Box_DoubleClick(object sender, EventArgs e)
        {
            if (this.m_DbclkEnale)
            {
                if (this.m_Box.SizeMode == PictureBoxSizeMode.StretchImage)
                {
                    this.m_Box.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.CenterImage();
                }
                else
                {
                    this.m_Box.SizeMode = PictureBoxSizeMode.StretchImage;
                    this.FixImage();
                }
            }
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.m_DragEnable)
            {
                this.m_MouseDownPosition = this.m_Box.PointToClient(Cursor.Position);
                this.m_Panel.Cursor = Cursors.Hand;
            }
        }

        private void Box_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_DragEnable && (e.Button == MouseButtons.Left))
            {
                Point point = this.m_Box.PointToClient(Cursor.Position);
                int num = point.X - this.m_MouseDownPosition.X;
                int num2 = point.Y - this.m_MouseDownPosition.Y;
                Point location = this.m_Box.Location;
                location.X += num;
                location.Y += num2;
                if (location.X < 0)
                {
                    if (this.m_Box.Width < this.m_Panel.ClientRectangle.Width)
                    {
                        location.X = 0;
                    }
                    else if ((location.X + this.m_Box.Width) < this.m_Panel.ClientRectangle.Width)
                    {
                        location.X = this.m_Panel.ClientRectangle.Width - this.m_Box.Width;
                    }
                }
                if ((location.X + this.m_Box.Width) > this.m_Panel.ClientRectangle.Width)
                {
                    if (this.m_Box.Width < this.m_Panel.ClientRectangle.Width)
                    {
                        location.X = this.m_Panel.ClientRectangle.Width - this.m_Box.Width;
                    }
                    else if (location.X > 0)
                    {
                        location.X = 0;
                    }
                }
                if (location.Y < 0)
                {
                    if (this.m_Box.Height < this.m_Panel.ClientRectangle.Height)
                    {
                        location.Y = 0;
                    }
                    else if ((location.Y + this.m_Box.Height) < this.m_Panel.ClientRectangle.Height)
                    {
                        location.Y = this.m_Panel.ClientRectangle.Height - this.m_Box.Height;
                    }
                }
                if ((location.Y + this.m_Box.Height) > this.m_Panel.ClientRectangle.Height)
                {
                    if (this.m_Box.Height < this.m_Panel.ClientRectangle.Height)
                    {
                        location.Y = this.m_Panel.ClientRectangle.Height - this.m_Box.Height;
                    }
                    else if (location.Y > 0)
                    {
                        location.Y = 0;
                    }
                }
                point = this.m_Box.Location;
                this.m_MouseDownPosition.X -= (location.X - num) - point.X;
                this.m_MouseDownPosition.Y -= (location.Y - num2) - point.Y;
                this.m_Box.Location = location;
            }
        }

        private void Box_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.m_DragEnable)
            {
                this.m_Panel.Cursor = Cursors.Default;
            }
        }

        private void CenterImage()
        {
            if (this.m_Box.Image == null)
            {
                this.HidePictureBox();
            }
            else
            {
                this.m_Box.Left = (this.m_Panel.ClientRectangle.Width - this.m_Box.Width) / 2;
                this.m_Box.Top = (this.m_Panel.ClientRectangle.Height - this.m_Box.Height) / 2;
            }
        }

        private void FixImage()
        {
            if (this.m_Box.Image == null)
            {
                this.HidePictureBox();
            }
            else
            {
                int height = this.m_Box.Image.Height;
                int width = this.m_Box.Image.Width;
                int num3 = this.m_Panel.ClientRectangle.Height;
                int num4 = this.m_Panel.ClientRectangle.Width;
                if (((num3 < 1) && (num4 < 1)) && ((height < 1) && (width < 1)))
                {
                    this.HidePictureBox();
                }
                else
                {
                    int num5 = num3;
                    int num6 = num4;
                    int num7 = 0;
                    int num8 = 0;
                    if ((height > num3) || (width > num4))
                    {
                        if ((((float)height) / ((float)width)) > (((float)num3) / ((float)num4)))
                        {
                            num6 = (int)((width * num3) / ((float)height));
                        }
                        else
                        {
                            num5 = (int)((height * num4) / ((float)width));
                        }
                        if (num6 == num4)
                        {
                            num7 = (num3 - num5) / 2;
                        }
                        else
                        {
                            num8 = (num4 - num6) / 2;
                        }
                    }
                    else
                    {
                        num5 = height;
                        num6 = width;
                        num7 = (num3 - height) / 2;
                        num8 = (num4 - width) / 2;
                    }
                    this.m_Box.Height = num5;
                    this.m_Box.Width = num6;
                    this.m_Box.Top = num7;
                    this.m_Box.Left = num8;
                }
            }
        }

        public static PictureBoxControl GetPictureBoxControl(System.Windows.Forms.PictureBox box)
        {
            if (box == null)
            {
                return null;
            }
            if (box.Parent == null)
            {
                return null;
            }
            if (box.Parent.GetType() != typeof(Panel))
            {
                return null;
            }
            if (box.Parent.Name != "__PIC_CTRL__PANEL__")
            {
                return null;
            }
            return (box.Parent.Tag as PictureBoxControl);
        }

        private void HidePictureBox()
        {
            this.m_Box.Location = new Point(this.m_Panel.Width + 1, this.m_Panel.Width + 1);
        }

        private void OnLoadImageFromFile(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "打开图像文件";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "JPG文件\t(*.JPG;*.JPEG;*.JPE;*.JFIF)|*.JPG;*.JPEG;*.JPE;*.JFIF|GIF文件\t(*.GIF)|*.GIF|TIFF文件\t(*.TIF;*.TIFF)|*.TIF;*.TIFF|PNG文件\t(*.PNG)|*.PNG|PNG文件\t(*.PNG)|*.PNG|ICO文件\t(*.ICO)|*.ICO|位图文件\t(*.BMP)|*.BMP|所有图像文件|*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG;*.ICO;*.BMP|所有文件 (*.*)|*.*";
            if ((dialog.ShowDialog() == DialogResult.OK) && File.Exists(dialog.FileName))
            {
                try
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(dialog.FileName);
                    this.Image = image;
                }
                catch
                {
                }
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (this.m_Image != this.m_Box.Image)
            {
                this.m_Image = this.m_Box.Image;
                if (this.m_Box.SizeMode == PictureBoxSizeMode.StretchImage)
                {
                    this.FixImage();
                }
                else
                {
                    this.m_Box.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.CenterImage();
                }
            }
        }

        private void OnSaveImageToFile(object sender, EventArgs e)
        {
            if ((this.m_Box != null) && (this.m_Box.Image != null))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "保存图像文件";
                dialog.Filter = "JPG文件\t(*.JPG;*.JPEG;*.JPE;*.JFIF)|*.JPG;*.JPEG;*.JPE;*.JFIF|GIF文件\t(*.GIF)|*.GIF|TIFF文件\t(*.TIF;*.TIFF)|*.TIF;*.TIFF|PNG文件\t(*.PNG)|*.PNG|PNG文件\t(*.PNG)|*.PNG|ICO文件\t(*.ICO)|*.ICO|位图文件\t(*.BMP)|*.BMP|所有图像文件|*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG;*.ICO;*.BMP|所有文件 (*.*)|*.*";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string text = Path.GetExtension(dialog.FileName).Trim(new char[] { '.' }).ToUpper();
                        ImageFormat format = m_SaveFormats[text] as ImageFormat;
                        if (format == null)
                        {
                            this.m_Box.Image.Save(dialog.FileName);
                        }
                        else
                        {
                            this.m_Box.Image.Save(dialog.FileName, format);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }

        private void OnViewDetails(object sender, EventArgs e)
        {
            new PictureBoxControlDetails(this.m_Box.Image).ShowDialog();
        }

        private void Panel_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control == this.m_Box)
            {
                this.PictureBox = null;
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            if (this.m_Box.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                this.FixImage();
            }
            else
            {
                this.m_Box.SizeMode = PictureBoxSizeMode.AutoSize;
                this.CenterImage();
            }
        }

        private void Remove()
        {
            if (this.m_Box != null)
            {
                Control parent = this.m_Panel.Parent;
                this.m_Box.DoubleClick -= new EventHandler(this.Box_DoubleClick);
                this.m_Box.MouseDown -= new MouseEventHandler(this.Box_MouseDown);
                this.m_Box.MouseMove -= new MouseEventHandler(this.Box_MouseMove);
                this.m_Box.MouseUp -= new MouseEventHandler(this.Box_MouseUp);
                this.m_Box.Paint -= new PaintEventHandler(this.OnPaint);
                Application.Idle -= new EventHandler(this.Application_Idle);
                parent.SuspendLayout();
                int index = parent.Controls.IndexOf(this.m_Panel);
                this.m_Panel.Controls.Remove(this.m_Box);
                this.m_Box.Dock = this.m_Panel.Dock;
                this.m_Box.Width = this.m_Panel.Width;
                this.m_Box.Height = this.m_Panel.Height;
                this.m_Box.Location = this.m_Panel.Location;
                this.m_Box.BorderStyle = this.m_Panel.BorderStyle;
                parent.Controls.Remove(this.m_Panel);
                parent.Controls.Add(this.m_Box);
                parent.Controls.SetChildIndex(this.m_Box, index);
                this.m_Panel.ContextMenu = null;
                this.m_Panel.Dispose();
                this.m_Panel = null;
                this.m_Image = null;
                for (int i = 0; i < this.m_MenuItems.Length; i++)
                {
                    if (this.m_MenuItems[i] != null)
                    {
                        if (this.m_Box.ContextMenu != null)
                        {
                            this.m_Box.ContextMenu.MenuItems.Remove(this.m_MenuItems[i]);
                        }
                        this.m_MenuItems[i].Dispose();
                        this.m_MenuItems[i] = null;
                    }
                }
                parent.ResumeLayout(true);
                this.m_Box = null;
            }
        }

        public static void RemovePictureBoxControl(System.Windows.Forms.PictureBox box)
        {
            if ((box != null) && (box.Parent != null))
            {
                PictureBoxControl tag = box.Parent.Tag as PictureBoxControl;
                if (tag != null)
                {
                    tag.PictureBox = null;
                }
            }
        }

        public static PictureBoxControl SetPictureBoxControl(System.Windows.Forms.PictureBox box, PictureBoxContexMenu contexMenu)
        {
            if (box == null)
            {
                return null;
            }
            if (box.Parent == null)
            {
                return null;
            }
            PictureBoxControl tag = box.Parent.Tag as PictureBoxControl;
            if (tag != null)
            {
                tag.ContexMenu = contexMenu;
                return tag;
            }
            return new PictureBoxControl(box, contexMenu);
        }

        private void SetupMenu()
        {
            if ((this.m_ContexMenu & PictureBoxContexMenu.Details) == PictureBoxContexMenu.Details)
            {
                if (this.m_MenuItems[0] == null)
                {
                    this.m_MenuItems[0] = new MenuItem("查看图像", new EventHandler(this.OnViewDetails));
                }
                if (this.m_Box.ContextMenu == null)
                {
                    this.m_Box.ContextMenu = new ContextMenu();
                }
                this.m_Box.ContextMenu.MenuItems.Add(this.m_MenuItems[0]);
            }
            else if (this.m_MenuItems[0] != null)
            {
                if ((this.m_Box != null) && (this.m_Box.ContextMenu != null))
                {
                    this.m_Box.ContextMenu.MenuItems.Remove(this.m_MenuItems[0]);
                }
                this.m_MenuItems[0].Dispose();
                this.m_MenuItems[0] = null;
            }
            if ((this.m_ContexMenu & PictureBoxContexMenu.LoadFromFile) == PictureBoxContexMenu.LoadFromFile)
            {
                if (this.m_MenuItems[1] == null)
                {
                    this.m_MenuItems[1] = new MenuItem("导入图像", new EventHandler(this.OnLoadImageFromFile));
                }
                if (this.m_Box.ContextMenu == null)
                {
                    this.m_Box.ContextMenu = new ContextMenu();
                }
                this.m_Box.ContextMenu.MenuItems.Add(this.m_MenuItems[1]);
            }
            else if (this.m_MenuItems[1] != null)
            {
                if ((this.m_Box != null) && (this.m_Box.ContextMenu != null))
                {
                    this.m_Box.ContextMenu.MenuItems.Remove(this.m_MenuItems[1]);
                }
                this.m_MenuItems[1].Dispose();
                this.m_MenuItems[1] = null;
            }
            if ((this.m_ContexMenu & PictureBoxContexMenu.SaveToFile) == PictureBoxContexMenu.SaveToFile)
            {
                if (this.m_MenuItems[2] == null)
                {
                    this.m_MenuItems[2] = new MenuItem("保存图像", new EventHandler(this.OnSaveImageToFile));
                }
                if (this.m_Box.ContextMenu == null)
                {
                    this.m_Box.ContextMenu = new ContextMenu();
                }
                this.m_Box.ContextMenu.MenuItems.Add(this.m_MenuItems[2]);
            }
            else if (this.m_MenuItems[2] != null)
            {
                if ((this.m_Box != null) && (this.m_Box.ContextMenu != null))
                {
                    this.m_Box.ContextMenu.MenuItems.Remove(this.m_MenuItems[2]);
                }
                this.m_MenuItems[2].Dispose();
                this.m_MenuItems[2] = null;
            }
            this.m_Panel.ContextMenu = this.m_Box.ContextMenu;
        }

        public Control Container
        {
            get
            {
                return this.m_Panel;
            }
        }

        public PictureBoxContexMenu ContexMenu
        {
            get
            {
                return this.m_ContexMenu;
            }
            set
            {
                if (this.m_ContexMenu != value)
                {
                    if (this.m_Box == null)
                    {
                        this.m_ContexMenu = value;
                    }
                    else
                    {
                        this.m_ContexMenu = value;
                        this.SetupMenu();
                    }
                }
            }
        }

        public bool DoubleClickEnable
        {
            get
            {
                return this.m_DbclkEnale;
            }
            set
            {
                this.m_DbclkEnale = value;
            }
        }

        public bool DragEnable
        {
            get
            {
                return this.m_DragEnable;
            }
            set
            {
                this.m_DragEnable = value;
            }
        }

        public System.Drawing.Image Image
        {
            get
            {
                return this.m_Box.Image;
            }
            set
            {
                this.m_Box.Image = value;
                this.m_Image = this.m_Box.Image;
                if (this.m_Box.SizeMode == PictureBoxSizeMode.StretchImage)
                {
                    this.FixImage();
                }
                else
                {
                    this.m_Box.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.CenterImage();
                }
            }
        }

        public System.Windows.Forms.PictureBox PictureBox
        {
            get
            {
                return this.m_Box;
            }
            set
            {
                if ((this.m_Box != value) && (GetPictureBoxControl(value) == null))
                {
                    this.Remove();
                    this.m_Box = value;
                    this.Add();
                }
            }
        }
    }
}

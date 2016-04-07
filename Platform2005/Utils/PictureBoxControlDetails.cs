namespace Platform.Utils
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class PictureBoxControlDetails : Form
    {
        private Container components;
        private PictureBoxControl ctrl = new PictureBoxControl();
        private Panel panel1;
        private PictureBox pictureBox1;

        public PictureBoxControlDetails(Image img)
        {
            this.InitializeComponent();
            this.ctrl.PictureBox = this.pictureBox1;
            this.ctrl.Image = img;
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
            this.panel1 = new Panel();
            this.pictureBox1 = new PictureBox();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x27a, 0x1ca);
            this.panel1.TabIndex = 0;
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x27a, 0x1ca);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(0x27a, 0x1ca);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "PictureBoxControlDetails";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "�鿴ͼƬ";
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}

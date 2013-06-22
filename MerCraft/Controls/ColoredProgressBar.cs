using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MerCraft.Controls
{
    public partial class ColoredProgressBar : UserControl
    {
        public int Value
        {
            get
            {
                return this.progressBar1.Value;
            }
            set
            {
                this.progressBar1.Value = value;
            }
        }

        public int Maximum
        {
            get
            {
                return this.progressBar1.Maximum;
            }
            set
            {
                this.progressBar1.Maximum = value;
            }
        }

        public enum ProgressBarTextDisplayFormat
        {
            Percentage,
            CustomText
        }

        public ProgressBarTextDisplayFormat DisplayStyle
        {
            get;
            set;
        }

        public override string Text
        {
            get;
            set;
        }

        public ColoredProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // None... Helps control the flicker.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            const int inset = 2; // A single inset value to control teh sizing of the inner rect.

            using (Image offscreenImage = new Bitmap(this.Width, this.Height))
            {
                using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                {
                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

                    if (ProgressBarRenderer.IsSupported)
                        ProgressBarRenderer.DrawHorizontalBar(offscreen, rect);

                    rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                    rect.Width = (int)(rect.Width * ((double)this.Value / this.Maximum));
                    if (rect.Width == 0)
                        rect.Width = 1; // Can't draw rec with width of 0.

                    LinearGradientBrush brush = new LinearGradientBrush(rect, this.BackColor, this.ForeColor, LinearGradientMode.Vertical);
                    offscreen.FillRectangle(brush, inset, inset, rect.Width, rect.Height);

                    string text = DisplayStyle == ProgressBarTextDisplayFormat.Percentage ? Value.ToString() + '%' : this.Text;

                    using (Font f = new Font(FontFamily.GenericSerif, 10))
                    {

                        SizeF len = offscreen.MeasureString(text, f);
                        // Calculate the location of the text (the middle of progress bar)
                        Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
                        // Draw the custom text in inverted color
                        offscreen.DrawString(text, f, new SolidBrush(Color.FromArgb(this.ForeColor.ToArgb() ^ 0xffffff)), location);
                    }

                    e.Graphics.DrawImage(offscreenImage, 0, 0);
                    offscreenImage.Dispose();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
//using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Math3;

namespace Words
{
    public partial class form_Busy : PerPixelForm.PerPixelAlphaForm
    {
        public List<Bitmap> lstBMP = new List<Bitmap>();
        public int intPicPerQuadrant = 8;

        System.Windows.Forms.Timer tmrAnimate = new System.Windows.Forms.Timer();

        int intCounter = 0;
        public form_Busy()
        {
            TopMost = true;
            this.ShowInTaskbar = false;
            ControlBox = false;
            ShowInTaskbar = false;
            if (lstBMP.Count == 0)
            {
                Bitmap bmpBase = Properties.Resources.formBusy;
                Color clrTransparent = bmpBase.GetPixel(0, 0);
                lstBMP.AddRange(new Bitmap[4 * intPicPerQuadrant]);
                double dblDeltaRad = Math.PI / (2 * intPicPerQuadrant);
                Size szMin = new Size();
                for (int intCounter = 0; intCounter < intPicPerQuadrant; intCounter++)
                { 
                    lstBMP[intCounter] = classRotateImage.rotateImage(bmpBase, (double)intCounter * dblDeltaRad);
                    lstBMP[intCounter].MakeTransparent(clrTransparent);

                    if (lstBMP[intCounter].Width > szMin.Width) szMin.Width = lstBMP[intCounter].Width;
                    if (lstBMP[intCounter].Height > szMin.Height) szMin.Height = lstBMP[intCounter].Height;

                    lstBMP[intCounter + intPicPerQuadrant] = new Bitmap(lstBMP[intCounter]);
                    lstBMP[intCounter + intPicPerQuadrant].RotateFlip(RotateFlipType.Rotate270FlipNone);
                    lstBMP[intCounter + intPicPerQuadrant].MakeTransparent(clrTransparent);

                    lstBMP[intCounter + 2 * intPicPerQuadrant] = new Bitmap(lstBMP[intCounter]);
                    lstBMP[intCounter + 2 * intPicPerQuadrant].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    lstBMP[intCounter + 2 * intPicPerQuadrant].MakeTransparent(clrTransparent);

                    lstBMP[intCounter + 3 * intPicPerQuadrant] = new Bitmap(lstBMP[intCounter]);
                    lstBMP[intCounter + 3 * intPicPerQuadrant].RotateFlip(RotateFlipType.Rotate90FlipNone);
                    lstBMP[intCounter + 3 * intPicPerQuadrant].MakeTransparent(clrTransparent);
                }


                for (int intCounter = 0; intCounter < lstBMP.Count; intCounter++)
                {
                    Bitmap bmpNew = new Bitmap(szMin.Width, szMin.Height);
                    using (Graphics g = Graphics.FromImage(bmpNew))
                    {
                        Bitmap bmpSource = lstBMP[intCounter];
                        Rectangle reource = new Rectangle(new Point(), bmpSource.Size);
                        Rectangle recDest = new Rectangle((bmpNew.Width - bmpSource.Width) / 2,
                                                          (bmpNew.Height - bmpSource.Height) / 2,
                                                          bmpSource.Width,
                                                          bmpSource.Height);
                        g.DrawImage(bmpSource, recDest, reource, GraphicsUnit.Pixel);
                        lstBMP[intCounter] = bmpNew;
                    }
                }
            }

            tmrAnimate.Interval = 250;
            tmrAnimate.Tick += TmrAnimate_Tick;
            
            Size = new Size(lstBMP[intCounter].Width, lstBMP[intCounter].Height);

            FormBorderStyle = FormBorderStyle.None;
            Activated += form_Busy_Activated;
            MouseMove += form_Busy_MouseMove;

        }

        private void TmrAnimate_Tick(object sender, EventArgs e)
        {
            if (!IsDisposed)
                Animate();
        }

        public void Start()
        {
            tmrAnimate.Enabled = true;
            if (!IsDisposed)
                Show();
        }

        public void Stop()
        {
            tmrAnimate.Enabled = false;
            if (!IsDisposed)
                Hide();
        }

        private void form_Busy_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                Left = MousePosition.X - Width / 2;
                Top = MousePosition.Y - Height / 2;
            }
        }

        private void form_Busy_Activated(object sender, EventArgs e)
        {
            Hide();
        }

        string _strText = "";
        public override string Text
        { 
        get { return _strText; }
            set { _strText = value; }
        }

        public void Animate()
        {
            intCounter = (intCounter + 1) % lstBMP.Count;

            if (!IsDisposed)
            {
                if (Text.Length > 0)
                {
                    Size szText = TextRenderer.MeasureText(Text, Font);
                    Bitmap bmpText = new Bitmap(szText.Width , szText.Height);
                    SolidBrush brBackground = new SolidBrush(Color.FromArgb(1, 1, 1));

                    using (Graphics g = Graphics.FromImage(bmpText))
                    {
                        g.FillRectangle(brBackground, 0, 0, bmpText.Width, bmpText.Height);
                        g.DrawString(Text, Font, new SolidBrush(ForeColor), new Point(0, 0));
                    }
                    bmpText.MakeTransparent(brBackground.Color);

                    Bitmap bmpSource = (Bitmap)new Bitmap(lstBMP[intCounter]);
                    using (Graphics g = Graphics.FromImage(bmpSource))
                    {
                        Rectangle recSource = new Rectangle(0, 0, bmpText.Width, bmpText.Height);
                        double dblAR_Text = (double)bmpText.Height / (double)bmpText.Width;
                        Size szDestination = new Size(bmpSource.Width, (int)(dblAR_Text * bmpSource.Width));
                        Rectangle recDest = new Rectangle(new Point(0, (int)(bmpSource.Height - szDestination.Height) / 2), szDestination);
                        g.DrawImage(bmpText, recDest, recSource, GraphicsUnit.Pixel);
                    }

                    SetBitmap(bmpSource);
                    Show();
                }
                else
                {
                    SetBitmap(lstBMP[intCounter]);
                    Show();
                }
            }
        }

    }
}

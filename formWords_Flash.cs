using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Words
{
    public class formWords_Flash : PerPixelForm.PerPixelAlphaForm 
    {
        public static formWords_Flash instance  = null;
        public formWords_Flash()
        {
            instance = this;

            TopMost = true;
            this.ShowInTaskbar = false;
            ControlBox = false;
            ShowInTaskbar = false;

            Bitmap bmpQuill = new Bitmap(Properties.Resources.Words);
            bmpQuill.MakeTransparent(bmpQuill.GetPixel(0, 0));
            Size = bmpQuill.Size;
            Location = new Point((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Width) / 2,
                                 (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Height) / 2);
            SetBitmap(bmpQuill);
            Show();
            Refresh();
        }
    }
}

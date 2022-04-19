using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Words
{
    public class classKeyboard
    {
        static System.Windows.Forms.PictureBox pic = new System.Windows.Forms.PictureBox();

        static List<classKeyBoard_Marker> _lstMarkers = new List<classKeyBoard_Marker>();
        public static List<classKeyBoard_Marker> lstMarkers
        {
            get 
            {
                return _lstMarkers; 
            }
            set 
            { 
                _lstMarkers = value;
            }
        }

        public classKeyboard()
        {
            classKeyBoard_Marker cMarker = new classKeyBoard_Marker("\\", '\\');

            ///                                                         A
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'q', 'à'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'Q', 'À'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'a', 'â'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'A', 'Â'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'z', 'á'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'Z', 'Á'));
            
            ///                                                         E
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '3', 'è'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '#', 'È'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'e', 'ê'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'E', 'Ê'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'd', 'é'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'D', 'É'));

            ///                                                         I
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '8', 'ì'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '*', 'Ì'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'i', 'î'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'I', 'Î'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'k', 'í'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'K', 'Í'));

            ///                                                         O
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '9', 'ò'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '(', 'Ò'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'o', 'ô'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'O', 'Ô'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'l', 'ó'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'L', 'Ó'));

            ///                                                         U
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '7', 'ù'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, '&', 'Ù'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'u', 'û'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'U', 'Û'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'j', 'ú'));
            cMarker.lstPairs.Add(new classKeyBoard_Marker.classKeyBoard_Pair(ref cMarker, 'J', 'Ú'));

        }


        public bool HandleKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            System.Windows.Forms.RichTextBox rtx = (System.Windows.Forms.RichTextBox)sender;
            if (pic.Parent != null)
                pic.Parent.Controls.Remove(pic);

            pic.Hide();  /// debug remove

            rtx.Controls.Add(pic);

            if (rtx.SelectionStart > 1)
            {
                char chrPreceding = rtx.Text[rtx.SelectionStart - 2];
                char chrKeyPressed = rtx.Text[rtx.SelectionStart - 1];
                for (int intMarkerCounter = 0; intMarkerCounter < lstMarkers.Count; intMarkerCounter++)
                {
                    // test chrPreceding to Input of each Marker
                    classKeyBoard_Marker cMarker = lstMarkers[intMarkerCounter];
                    if (cMarker.Input == chrPreceding)
                    {
                        // Marker detected before cursor - test if keypressed is an Input for this marker
                        for (int intPairCounter = 0; intPairCounter < cMarker.lstPairs.Count; intPairCounter++)
                        {
                            classKeyBoard_Marker.classKeyBoard_Pair cPair = cMarker.lstPairs[intPairCounter];
                            if (chrKeyPressed == cPair.Input)
                            {
                                rtx.Select(rtx.SelectionStart - 2, 2);
                                rtx.SelectedText = cPair.Output.ToString();
                                pic.Hide();
                                return true;
                            }
                        }
                        pic.Hide();
                        return false;
                    }
                    if (cMarker.Input == chrKeyPressed)
                    {
                        // display possible keys on pic
                        pic.Image = new Bitmap(class_Image.bmp);
                        pic.Size = new Size(rtx.Width, 
                                            (int)((float)rtx.Width * (float)pic.Image.Height / (float)pic.Image.Width));
                        pic.Location = new Point((rtx.Width - pic.Width) / 2,
                                                 (rtx.Height - pic.Height) / 2);
                        pic.BringToFront();
                        //pic.Show();
                    }
                }
            }

            pic.Hide();  /// debug remove
            return false;
        }

        public class classKeyBoard_Marker
        {
            public List<classKeyBoard_Pair> lstPairs = new List<classKeyBoard_Pair>();
            
            Char _chrInput = ' ';
            public Char Input
            {
                get { return _chrInput; }
                set
                {
                    strMarkerCharacters = MarkerCharacters.Replace(Input, value);
                    _chrInput = value;
                }
            }

            string strName = "";
            public string Name
            {
                get { return strName; }
                set { strName = value; }
            }

            string strMarkerCharacters = "";
            public string MarkerCharacters
            {
                get { return strMarkerCharacters; }
                    
            }

            public classKeyBoard_Marker (string Name, char Input)
            {
                this.Name = Name;
                this.Input = Input;
                classKeyboard.lstMarkers.Add(this);
            }

            public void Dispose()
            {
                classKeyboard.lstMarkers.Remove(this);
                Input = ' ';
            }

            public char TestChar(char chrKeyPressed)
            {
                for (int intPairCounter = 0; intPairCounter < lstPairs.Count; intPairCounter++)
                {
                    classKeyBoard_Pair cPair = lstPairs[intPairCounter];
                    if (chrKeyPressed == cPair.Input)
                        return cPair.Output;
                }
                return chrKeyPressed;
            }

            public class classKeyBoard_Pair
            {
                classKeyBoard_Marker _Marker = null;
                public classKeyBoard_Marker Marker
                {
                    get { return _Marker; }
                    set { _Marker = value; }
                }

                Char _chrInput = ' ';
                public Char Input
                {
                    get { return _chrInput; }
                    set { _chrInput = value; }
                }
                Char _chrOutput = ' ';
                public Char Output
                {
                    get { return _chrOutput; }
                    set { _chrOutput = value; }
                }

                public classKeyBoard_Pair(ref classKeyBoard_Marker Marker,  char Input, char Output)
                {
                    this.Marker = Marker;
                    this.Input = Input;
                    this.Output = Output;
                    Marker.lstPairs.Add(this);
                }

                public void Dispose()
                {
                    Marker.lstPairs.Remove(this);
                }
            }




        }

        public class class_Image
        {
            public static class_Image instance = null;
            public static Bitmap bmp = new Bitmap(Properties.Resources.Keyboard_BLANK);

            public static List<class_Image_Key> lstKeys = new List<class_Image_Key>();

            public class_Image()
            {
                if (instance != null) return;
                instance = this;
            }

            public class class_Image_Key
            {
                public Rectangle rec = new Rectangle();
                public char chrKey = ' ';

                Bitmap _bmpImage = null;
                public Bitmap bmpImage
                {
                    get { return _bmpImage; }
                    set { _bmpImage = value; }
                }

                public class_Image_Key(Rectangle rec, char chrKey)
                {
                    this.rec = rec;
                    this.chrKey = chrKey;
                    lstKeys.Add(this);
                }

                public void Draw(ref Graphics g)
                {
                    Rectangle recSource = new Rectangle(0, 0, bmpImage.Width, bmpImage.Height);
                    Rectangle recDest = rec;
                    g.DrawImage(bmpImage, recDest, recSource, GraphicsUnit.Pixel);
                }
            }

        }

        public class formKeyBoard_MapEditor : System.Windows.Forms.Form
        {
            System.Windows.Forms.PictureBox pic = new System.Windows.Forms.PictureBox();
            System.Windows.Forms.TextBox txt = new System.Windows.Forms.TextBox();

            Ck_Objects.classLabelButton btnQuit = new Ck_Objects.classLabelButton();

            public formKeyBoard_MapEditor()
            {
                Controls.Add(pic);
                pic.MouseMove += Pic_MouseMove;
                pic.MouseDown += Pic_MouseDown;
                pic.MouseUp += Pic_MouseUp;

                Controls.Add(btnQuit);
                btnQuit.AutoSize = true;
                btnQuit.Text = "Quit";
                btnQuit.Click += BtnQuit_Click;
            }

            Point _ptMouseDown = new Point();
            public Point ptMouseDown
            {
                get { return _ptMouseDown; }
                set 
                {
                    _ptMouseDown = value;
                }
            }
            Point _ptMouse = new Point();
            public Point ptMouse
            {
                get { return _ptMouse; }
                set 
                {
                    _ptMouse = value;
                }
            }

            bool bolSelectMouseDown = false;
            public bool SelectMouseDown
            {
                get { return bolSelectMouseDown; }
                set 
                {
                    if (bolSelectMouseDown != value)
                    {
                        if (bolSelectMouseDown)
                        {
                            // mouse button has been released - set PtEnd - calculate
                            setRegion();
                        }
                        else
                        {
                            // mouse button has been pressed - set ptStart
                            ptMouseDown = ptMouse;
                        }
                        bolSelectMouseDown = value;
                    }
                }
            }


            private void BtnQuit_Click(object sender, EventArgs e)
            {
                Save();
            }

            private void Pic_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                if (!txt.Visible)
                    bolSelectMouseDown = false;
            }

            private void Pic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                if (!txt.Visible)
                    bolSelectMouseDown = true;
            }

            private void Pic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                ptMouse = new Point(e.X, e.Y);
            }

            void Save()
            {

            }

            void setRegion()
            {
                if (classKeyboard.class_Image.lstKeys.Count > 0)
                {
                    class_Image.class_Image_Key cKey = classKeyboard.class_Image.lstKeys[classKeyboard.class_Image.lstKeys.Count - 1];
                    cKey.rec = new Rectangle(ptMouse, new Size(ptMouse.X - ptMouseDown.X, ptMouse.Y - ptMouseDown.Y));
                    Point ptKey_Center = new Point(cKey.rec.Left + cKey.rec.Width / 2, cKey.rec.Top + cKey.rec.Height / 2);
                    txt.Location = new Point(ptKey_Center.X - cKey.rec.Width / 2, ptKey_Center.Y - cKey.rec.Height / 2);
                    txt.Size = cKey.rec.Size;
                    txt.BringToFront();
                    txt.Show();
                    txt.Focus();
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Words
{
    public class groupboxDictionaryHeader_Info : GroupBox
    {
        RichTextBox rtx = new RichTextBox();
        Ck_Objects.classLabelButton btnOk = new Ck_Objects.classLabelButton();
        static groupboxDictionaryHeader_Info _instance = null;
        public static groupboxDictionaryHeader_Info instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new groupboxDictionaryHeader_Info();
                    formDictionarySelection.instance.Controls.Add(_instance);
                    _instance.BringToFront();
                }
                return _instance;
            }
        }
        public groupboxDictionaryHeader_Info ()
        {
            Controls.Add(rtx);
            rtx.Dock = DockStyle.Fill;
            rtx.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            
            
            Controls.Add(btnOk);
            btnOk.Text = "Ok";
            btnOk.AutoSize = true;
            btnOk.Click += BtnOk_Click;

            Text = "Dictionary Source Information";

            Size = new Size(590, 610);

        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void placeObjects()
        {
            if (Visible)
            {
                Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2,
                                     (Screen.PrimaryScreen.Bounds.Height - Height) / 2);

                btnOk.Location = new Point(Width - btnOk.Width-20, Height - btnOk.Height-5);
                btnOk.BringToFront();

                BringToFront();
            }
        }


        static classDictionary _cDictionary = null;
        static public classDictionary cDictionary
        {
            get { return _cDictionary; }
            set
            {
                _cDictionary = value;
                Filename = cDictionary.strSourceDirectory + "Header.rtf";
            }
        }


        static string strFilename = "";
        static string Filename
        {
            get { return strFilename; }
            set
            {
                strFilename = value;
                if (System.IO.File.Exists(strFilename))
                {
                    instance.rtx.LoadFile(strFilename);
                    instance.Show();
                    instance.placeObjects();
                }
            }
        }


    }
}

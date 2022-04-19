using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using StringLibrary;
//using Ck_Objects;

namespace Words
{

    public class formPopUp : Form
    {
        public static formPopUp instance;
        public RichTextBox rtx = new RichTextBox();

        const string strFontFamily = "sans serif";

        classDictionary _cDictionary = null;
        public classDictionary cDictionary
        {
            get { return _cDictionary; }
            set { _cDictionary = value; }
        }

        public formPopUp()
        {
            instance = this;
            Controls.Add(rtx);
            rtx.Dock = DockStyle.Fill;
            //rtx.MouseDoubleClick += panelDefinitionPort.RtxOutput_MouseDoubleClick;
            rtx.MouseMove += panelDefinitionPort.RtxOutput_MouseMove;
            rtx.MouseDown += panelDefinitionPort.RtxOutput_MouseDown;
            rtx.LostFocus += Rtx_LostFocus;

            rtx.MouseLeave += Rtx_MouseLeave;
            rtx.MouseUp += Rtx_MouseUp;
            Hide();

            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(400, 300);

            TopMost = true;
            VisibleChanged += FormPopUp_VisibleChanged;
            
            Activated += FormPopUp_Activated;
        }

        private void Rtx_LostFocus(object sender, EventArgs e)
        {
            Hide();
        }

        private void Rtx_MouseUp(object sender, MouseEventArgs e)
        {
            Hide();
        }

        public void LoadDefinition(ref classDictionary cDictionary, string strFilename)
        {
            this.cDictionary = cDictionary;
            strFilename += cDictionary != null
                                        ? "." +cDictionary.eFileExtension.ToString()
                                        : "";

            if (System.IO.File.Exists(strFilename))
            {
                switch (cDictionary.eFileExtension)
                {
                    case enuFileExtensions.rtf:
                        {
                            rtx.LoadFile(strFilename);
                        }
                        break;

                    case enuFileExtensions.txt:
                        {
                            rtx.Hide();
                            classFileContent cFileContent = new classFileContent(cDictionary.strSourceDirectory, strFilename.Substring(strFilename.Length - 12, 8));
                            if (cFileContent.Heading != null && cFileContent.Definition != null)
                            {
                                RichTextBox rtxRef = rtx;
                                rtxRef.Clear();
                                classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.Heading.Trim(), panelDefinitionPort.fntWordHeading, Color.Blue, 0);
                                classStringLibrary.RTX_AppendNL(ref rtxRef);
                                if (cFileContent.alt_Heading != null)
                                {
                                    classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.alt_Heading.Trim(), panelDefinitionPort.fntWordHeading, Color.LightBlue, 0);
                                    classStringLibrary.RTX_AppendNL(ref rtxRef);
                                }

                                classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.Definition, panelDefinitionPort.fntWordDefinition, Color.Black, 0);
                                rtxRef.Select(0, 0);
                                rtxRef.ScrollToCaret();
                            }
                            rtx.Show();
                        }
                        break;
                }
            }
            BringToFront();
        }


        private void Rtx_MouseLeave(object sender, EventArgs e)
        {
            Hide();
        }

        private void FormPopUp_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                rtx.Focus();
                TopMost = true;
                BringToFront();
            }
        }

        public static Point ptLocation = new Point();
        bool bolActivated = false;

        private void FormPopUp_Activated(object sender, EventArgs e)
        {
            if (bolActivated) return;
            bolActivated = true;
            Location = ptLocation;
            TopMost = true;
            BringToFront();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words
{
    public class groupboxChapters : groupboxNotes
    {
        static groupboxChapters instance = null;
        ListBox lbx = new ListBox();
        TextBox txtEditHeading = new TextBox();
        ContextMenu cmnu = new ContextMenu();
        RichTextBox rtx = new RichTextBox();

        public groupboxChapters()
        {
            instance = this;

            Controls.Add(lbx);
            lbx.KeyDown += Lbx_KeyDown;
            lbx.SelectedIndexChanged += Lbx_SelectedIndexChanged;

            Controls.Add(txtEditHeading);
            txtEditHeading.KeyDown += TxtEditHeading_KeyDown;

            MenuItem mnuNew = new MenuItem("New");
            {
                mnuNew.MenuItems.Add(new MenuItem("Insert Before", mnuNew_InsertBefore_Click));
                mnuNew.MenuItems.Add(new MenuItem("Insert After", mnuNew_InsertAfter_Click));
                mnuNew.MenuItems.Add(new MenuItem("Append", mnuNew_Append_Click));
                mnuNew.MenuItems.Add(new MenuItem("Delete", mnuNew_Delete_Click));
            }

            cmnu.MenuItems.Add(mnuNew);
        }

        #region Properties

        static List<string> _lstChapterFilenames = new List<string>();
        public static List<string> lstChapterFilenames
        {
            get { return _lstChapterFilenames; }
            set
            {
                _lstChapterFilenames = value;
                instance.lbx_Build();
            }
        }

        #endregion

        #region Methods
        void lbx_Build()
        {
            lbx.Items.Clear();
            
            for (int intChapterCounter = 0; intChapterCounter < lstChapterFilenames.Count; intChapterCounter++)
            {
                string strFilename = formWords.instance.Path + lstChapterFilenames[intChapterCounter] + ".rtf";
                if (System.IO.File.Exists(strFilename))
                {
                    rtx.LoadFile(strFilename);

                }
            }
        }

        #endregion

        #region Events

        void mnuNew_InsertBefore_Click(object sender, EventArgs e)
        {

        }

        void mnuNew_InsertAfter_Click(object sender, EventArgs e)
        {

        }

        void mnuNew_Append_Click(object sender, EventArgs e)
        {

        }

        void mnuNew_Delete_Click(object sender, EventArgs e)
        {

        }

        private void TxtEditHeading_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Lbx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Lbx_KeyDown(object sender, KeyEventArgs e)
        {

        } 
        #endregion


        public class classChapterInfo
        {
            public static List<classChapterInfo> lstChapters = new List<classChapterInfo>();
            const string strChapter = "Chapter ";
            public static void Load()
            {
                RichTextBox rtx = new RichTextBox();
                lstChapterFilenames.Clear();
                char[] chrInfoSplit = { '\n' };
                char[] chrDateSplit = { '\\' };
                
                for (int intChapterCounter = 0; intChapterCounter < groupboxChapters.lstChapterFilenames.Count; intChapterCounter++)
                {
                    string strFilename = formWords.instance.Path + groupboxChapters.lstChapterFilenames[intChapterCounter] + ".rtf";
                    if (System.IO.File.Exists(strFilename))
                    {
                        rtx.LoadFile(strFilename);
                        List<string> lstInfo = rtx.Text.Split(chrInfoSplit).ToList<string>();
                        classChapterInfo chpNew = new classChapterInfo();
                        lstChapters.Add(chpNew);

                        if (lstInfo.Count > 2)
                        {
                            string strDate = lstInfo[0];
                            List<string> lstDate = strDate.Split(chrDateSplit).ToList<string>();
                            if (lstDate.Count == 3)
                            {
                                try
                                {
                                    chpNew.Year = Convert.ToInt32(lstDate[0]);
                                    chpNew.Month = Convert.ToInt32(lstDate[1]);
                                    chpNew.Day = Convert.ToInt32(lstDate[2]);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            chpNew.Heading = lstDate[1];
                        }
                    }
                }
            }

            public int ChapterNumber
            {
                get { return lstChapters.IndexOf(this); }
            }

            string strHeading = "";
            public string Heading
            {
                get { return strHeading; }
                set { strHeading = value; }
            }

            int _intYear = -1;
            public int Year
            {
                get { return _intYear; }
                set { _intYear = value; }
            }

            int _intMonth = -1;
            public int Month
            {
                get { return _intMonth; }
                set { _intMonth = value; }
            }
            
            int _intDay = -1;
            public int Day
            {
                get { return _intDay; }
                set { _intDay = value; }
            }

            public string Date { get { return Year.ToString("0000") + "//" + Month.ToString("00") + "//" + Day.ToString("00"); } }

        }
    }
}

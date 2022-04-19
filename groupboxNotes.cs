using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Words
{
    enum enuFontDetails { FamilyName, Height, Bold, Underline, Italic, StrikeOut, _num };

    public partial class groupboxNotes : GroupBox
    {
        public static groupboxNotes instance = null;

        System.Threading.Semaphore semScroll = new System.Threading.Semaphore(1, 1);

        string strWorkingDirectory = "";
        const char conOpenBrace = '►';
        const char conCloseBrace = '◄';
        System.Windows.Forms.Timer tmrAutoSave = new Timer();

        public SplitContainer splMain = new SplitContainer();
        public SplitContainer splSub = new SplitContainer();
        public ck_RichTextBox rtxNotes = new ck_RichTextBox();
        public Panel pnlRTX = new Panel();

        PictureBox picMainEditor = new PictureBox();

        public panelNotesDataTree pnlNotes = new panelNotesDataTree("Notes");

        public groupboxWordAnalyzer grbWordAnalyzer = new groupboxWordAnalyzer();
        public Ck_Objects.classLabelButton btnMainScreen_Toggle = new Ck_Objects.classLabelButton();

        public bool bolDie = false;

        static int IDCounter = 0;
        int ID = IDCounter++;

        double dblMainEditor_AspectRatio = 1;
        double MainEditor_AspectRatio { get { return dblMainEditor_AspectRatio; } }        
        public groupboxNotes()
        {
            InitializeComponent();
            instance = this;

            Controls.Add(splMain);
            splMain.Dock = DockStyle.Fill;
            splMain.Orientation = Orientation.Horizontal;
            splMain.Panel1.Controls.Add(pnlNotes);
            pnlNotes.ToolTip_Enabled = false;

            splMain.Panel2.Controls.Add(splSub);
            splSub.Dock = DockStyle.Fill;
            splSub.Orientation = Orientation.Horizontal;

            splSub.Panel1.Controls.Add(grbWordAnalyzer);
            grbWordAnalyzer.Dock = DockStyle.Fill;

            splSub.Panel2.Controls.Add(pnlRTX);

            pnlRTX.Dock = DockStyle.Fill;

            pnlRTX.Controls.Add(rtxNotes);
            rtxNotes.Dock = DockStyle.Fill;
            rtxNotes.Name = "rtxNotes";
            rtxNotes.rtx.Font = new Font("Arial", 10);
            rtxNotes.rtx.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            rtxNotes.rtx.WordWrap = false;
            rtxNotes.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_Load].Visible = false;
            rtxNotes.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_SaveAs].Visible = false;
            rtxNotes.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_New].Visible = false;
            rtxNotes.rtx.Tag = (object)enuSearchRequestor.main;
            rtxNotes.rtx.TextChanged += rtx_TextChanged;
            rtxNotes.rtx.KeyDown += formWords.instance.rtxCK_KeyDown;
            rtxNotes.rtx.KeyUp += formWords.instance.rtxCK_KeyUp;
            rtxNotes.rtx.MouseDown += formWords.instance.rtxCK_MouseDown;
            rtxNotes.rtx.GotFocus += RtxNotes_GotFocus;
            rtxNotes.rtx.LostFocus += RtxNotes_LostFocus;
            rtxNotes.rtx.VScroll += Rtx_Scroll;
            rtxNotes.rtx.HScroll += Rtx_Scroll;
            rtxNotes.rtx.MouseDoubleClick += formWords.instance.Rtx_MouseDoubleClick;
            rtxNotes.File_Save = mnuFile_Save_Click;
            rtxNotes.SizeChanged += RtxNotes_SizeChanged;
            rtxNotes.rtx.SelectionChanged += formWords.instance.Rtx_SelectionChanged;
            rtxNotes.MouseDoubleClick = rtxNotes_MouseDoubleClick;
            rtxNotes.Heading_BackColor = Color.LightGreen;
            rtxNotes.Heading_ForeColor = Color.Black;
            
            rtxNotes.rtx.Controls.Add(picMainEditor);
            Bitmap bmpMainEditor = new Bitmap(Properties.Resources.Main_Editor);
            dblMainEditor_AspectRatio = (double)bmpMainEditor.Height / (double)bmpMainEditor.Width;

            picMainEditor.Image = bmpMainEditor;
            picMainEditor.SizeMode = PictureBoxSizeMode.StretchImage;

            pnlRTX.Controls.Add(btnMainScreen_Toggle);
            btnMainScreen_Toggle.Text = "";
            btnMainScreen_Toggle.Size = new Size(19, 19);
            btnMainScreen_Toggle.img_Idle = new Bitmap(Properties.Resources.btnNotes_MainScreen,
                                                       new Size(btnMainScreen_Toggle.Size.Width - 2,
                                                                btnMainScreen_Toggle.Size.Height - 2));
            btnMainScreen_Toggle.img_Highlight = new Bitmap(Properties.Resources.btnNotes_MainScreen_Highlight,
                                                            new Size(btnMainScreen_Toggle.Size.Width - 2,
                                                                     btnMainScreen_Toggle.Size.Height - 2));
            btnMainScreen_Toggle.Toggled = true;
            btnMainScreen_Toggle.Click += BtnMainScreen_Toggle_Click;
            btnMainScreen_Toggle.Image = btnMainScreen_Toggle.img_Idle;
            btnMainScreen_Toggle.Refresh();

            splMain.SplitterMoved += splMain_SplitterMoved1;
            formWords.cProject.Font_Load();

            pnlNotes.Name = "pnlNotes";

            tmrAutoSave.Interval = 120000; // every two minutes
            tmrAutoSave.Tick += TmrAutoSave_Tick;

            SizeChanged += groupboxNotes_SizeChanged;
            MouseClick += GroupboxNotes_MouseClick;
            TextChanged += GroupboxNotes_TextChanged;
        }

        private void GroupboxNotes_TextChanged(object sender, EventArgs e)
        {
            if (Text.Trim().Length == 0)
                ;
        }

        private void Rtx_Scroll(object sender, EventArgs e)
        {
            semScroll.WaitOne();

            if (grbWordAnalyzer.WordOutline_Toggle)
                grbWordAnalyzer.tmrUnderlineSelection_Reset();

            if (pictureboxWordAnalyzer.instance != null && pictureboxWordAnalyzer.instance.Visible)
                pictureboxWordAnalyzer.instance.Draw();

            semScroll.Release();
        }

        public bool Locked
        {
            get { return rtxNotes.ToolBar.btnLock.Toggled; }
        }
        private void GroupboxNotes_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Y < 15)
                if (pnlNotes.cEdit_Alt != null)
                    Parent_Back();
        }

        public classNotesInfo cParent
        {
            get { return panelNotesDataTree.cParent; }
            set { panelNotesDataTree.cParent=value ; }
        }
        public void Parent_Back()
        {
            if (this.cParent != null)
            {
                classNotesInfo cTempParent = cParent;
                panelNotesDataTree.cParent = this.cParent.cParent;
                cProject.cEdit_Alt = cTempParent;
            }
            else
            {
                panelNotesDataTree.cParent = null;
            }
        }


        int _intckrtx_ExpandedHeight = -1;
        public int ckrtx_ExpandedHeight
        {
            get { return _intckrtx_ExpandedHeight; }
            set { _intckrtx_ExpandedHeight = value; }
        }

        bool bolrtxCK_Collapsed = false;
        public bool rtxCK_Collapsed
        {
            get { return bolrtxCK_Collapsed; }
            set
            {
                if (!bolrtxCK_Collapsed)
                    ckrtx_ExpandedHeight = rtxNotes.Height;

                bolrtxCK_Collapsed = value;
                rtxNotes.Height = bolrtxCK_Collapsed
                                        ? 0
                                        : ckrtx_ExpandedHeight;
                placeObjects();
            }
        }

        private void rtxNotes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            rtxCK_Collapsed = !rtxCK_Collapsed;
        }

        public static classProject cProject
        {
            get { return formWords.cProject; }
        }
        public static classNotesInfo cEdit_Alt
        {
            get { return cProject.cEdit_Alt; }
        }

        void mnuFile_Save_Click(object sender, EventArgs e)
        {
            Note_Save();
            formWords.Message("Note Saved : " + cEdit_Alt.Heading, formWords.classMessage.enuTarget.Options);
        }

        public void Init()
        {
            rtxNotes.Text = "";
            
            NotesNeedToBeSaved = false;
        }


        bool bolMainScreen = false;
        public bool MainScreen
        {
            get { return bolMainScreen; }
            set
            {
                if (bolMainScreen != value)
                {
                    bolMainScreen = value;
                    if (MainScreen)
                    {
                        rtxNotes.Hide();
                        {
                            Controls.Remove(pnlRTX);
                            formWords.instance.splMain.Panel2.Controls.Add(pnlRTX);
                            pnlRTX.BringToFront();
                            formWords.instance.placeObjects();
                        }
                        rtxNotes.Show();
                    }
                    else
                    {
                        formWords.instance.splMain.Panel2.Controls.Remove(pnlRTX);
                        splSub.Panel2.Controls.Add(pnlRTX);
                        rtxNotes.Dock = DockStyle.Fill;
                    }
                    placeObjects();
                    formWords.instance.Title_Draw();
                }
            }
        }

        private void RtxNotes_SizeChanged(object sender, EventArgs e)
        {
            // position Bottom-Left corner
            placeObjects();
        }

        private void BtnMainScreen_Toggle_Click(object sender, EventArgs e)
        {
            MainScreen = !MainScreen;
        }

        private void splMain_SplitterMoved1(object sender, SplitterEventArgs e)
        {
            intSpc_distance = splMain.SplitterDistance;
            placeObjects();
            formWords.instance.grbNotePilot.LocationSet();
        }


        static int _intSpc_distance = 90;
        public static int intSpc_distance
        {
            get { return _intSpc_distance; }
            set { _intSpc_distance = value; }
        }
        void RtxNotes_GotFocus(object sender, EventArgs e)
        {
            if (formWords.instance.frmFindReplace == null || formWords.instance.frmFindReplace.IsDisposed)
                formWords.instance.frmFindReplace = new formFindReplace();
            formWords.instance.frmFindReplace.setTextBox(ref sender);

            try
            {
                tmrAutoSave.Enabled = true;
            }
            catch (Exception)
            {

            }
        }

        private void RtxNotes_LostFocus(object sender, EventArgs e)
        {
            if (NotesNeedToBeSaved)
            {
                Note_Save();
                formWords.Message("Lost Focus Note Saved: "
                                        + (cEdit_Alt != null
                                                ? cEdit_Alt.Heading
                                                : "NULL")); 
            }
            tmrAutoSave.Enabled = false;
        }

        static bool _bolFirstLoad = true;
        static public bool bolFirstLoad
        {
            get { return _bolFirstLoad; }
            set { _bolFirstLoad = value; }
        }

        static Single _ZoomFactor = -1;
        static public Single ZoomFactor
        {
            get { return _ZoomFactor; }
            set
            {
                if (_ZoomFactor != value)
                {
                    _ZoomFactor = value;
                }
            }
        }


        public bool EditingCurrentSelection
        {
            get { return cProject.cEdit_Alt == cProject.cEdit_Main; }
        }

        public void Note_Load()
        {
            ZoomFactor = rtxNotes.rtx.ZoomFactor;
            rtxNotes.Enabled = (cEdit_Alt != null);

            if (cEdit_Alt == null)
            {
                rtxNotes.rtx.Clear();

                rtxNotes_SetEnable();
                return;
            }
            
            bolFirstLoad = false;
            string strFilename_Load = cEdit_Alt.Filename;
            if (cEdit_Alt.File_Toggle)
            {
                rtxNotes.rtx.Enabled = true;
                rtxNotes.rtx.Clear();
                rtxNotes.rtx.ZoomFactor = 1;
                {
                    rtxNotes.Heading = cEdit_Alt.Heading.ToString();
                    
                    if (System.IO.File.Exists(strFilename_Load))
                    {
                        rtxNotes.PathAndFilename = strFilename_Load;
                        rtxNotes.LoadFile(rtxNotes.PathAndFilename);
                    }
                    else
                    {
                        rtxNotes.PathAndFilename = System.IO.Directory.GetCurrentDirectory();
                        rtxNotes.rtx.Clear();
                    }
                    rtxNotes.rtx.Font = new Font("Arial", 10);
                }
                rtxNotes.rtx.ZoomFactor = ZoomFactor;

                if (cEdit_Alt.Caret >= 0 && cEdit_Alt.Caret < rtxNotes.rtx.Text.Length)
                {
                    rtxNotes.rtx.SelectionStart = cEdit_Alt.Caret;
                    rtxNotes.rtx.ScrollToCaret();
                }
                NotesNeedToBeSaved = false;
                rtxNotes_SetEnable();
                grbWordAnalyzer.Analyze();
            }
            else
            {
                rtxNotes.rtx.Text = "";
                rtxNotes.rtx.Enabled = false;
                picMainEditor.Visible = false;
            }
        }


        public void rtxNotes_SetEnable()
        {
            rtxNotes.Enabled = ((cEdit_Alt != null && cProject.cEdit_Main != null) && string.Compare(cProject.cEdit_Main.Heading, cEdit_Alt.Heading) != 0);
            picMainEditor.Visible = !rtxNotes.Enabled;
            picMainEditor.BringToFront();
            placeObjects();
            formWords.instance.Title_Draw();
        }


        public void Note_Save()
        {
            if (!bolFirstLoad && NotesNeedToBeSaved && !EditingCurrentSelection)
            {
                if (cEdit_Alt != null &&  !cEdit_Alt.bolBackUpFileNote)
                {
                    
                    string strFilename_Save = cEdit_Alt.Filename;
                    cEdit_Alt.Caret = rtxNotes.rtx.GetCharIndexFromPosition(new Point(0, 0));

                    try
                    {
                        rtxNotes.rtx.SaveFile(strFilename_Save);
                    }
                    catch (Exception)
                    {

                    }

                    NotesNeedToBeSaved = false;
                    formWords.Message("Note Saved : " + cEdit_Alt.Heading);


                    formWords.cProject.cEdit_Alt.HeadingColor_Set(rtxNotes.rtx.Rtf);
                }
            }
        }

        private void TmrAutoSave_Tick(object sender, EventArgs e)
        {
            if (NotesNeedToBeSaved)
            {
                Note_Save();
                formWords.Message("Note Auto Saved: " 
                                        + (cEdit_Alt != null 
                                                ? cEdit_Alt.Heading 
                                                : "NULL"));
            }

        }

        void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(groupboxNotes));
            this.SuspendLayout();
            // 
            // groupboxNotes
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            Bitmap bmp = new Bitmap(Properties.Resources.Notes);
            //Icon = Icon.FromHandle(bmp.GetHicon());
            this.Name = "groupboxNotes";
            this.ResumeLayout(false);

            this.SuspendLayout();
            // 
            // rtxNotes
            // 
            this.rtxNotes.Location = new System.Drawing.Point(-2, 212);
            this.rtxNotes.Name = "rtxNotes";
            this.rtxNotes.Size = new System.Drawing.Size(338, 295);
            this.rtxNotes.TabIndex = 0;

            // 
            // pnlNotes
            // 
            this.pnlNotes.Location = new System.Drawing.Point(0, 59);
            this.pnlNotes.Name = "pnlNotes";
            this.pnlNotes.Size = new System.Drawing.Size(336, 147);
            this.pnlNotes.TabIndex = 3;
            // 
            // groupboxNotes
            // 

            this.ClientSize = new System.Drawing.Size(337, 505);
            this.Controls.Add(this.pnlNotes);
            this.Controls.Add(this.rtxNotes);
            this.Name = "groupboxNotes";
            this.Text = "Notes";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        static bool bolNotes_NeedToBeSaved = false;
        static public bool NotesNeedToBeSaved
        {
            get { return bolNotes_NeedToBeSaved; }
            set
            {
                bolNotes_NeedToBeSaved = value;
            }
        }

        public static void rtx_TextChanged(object sender, EventArgs e)
        {
            NotesNeedToBeSaved = true;
            if (!formWords.grbOptions.Toggle_Numeral_Insert) return;
            RichTextBox rtxSender = (RichTextBox)sender;
            if (rtxSender.SelectionStart > 0)
            {
                if (rtxSender.Text[rtxSender.SelectionStart - 1] == '#')
                {
                    rtxSender.Hide();
                    int intPreviousBraceValue = FindPreviousBraceValue(ref rtxSender, rtxSender.SelectionStart);
                    int intRememberSelectionStart = rtxSender.SelectionStart;
                    if (cursorIsInsideBrackets(ref rtxSender))
                    {
                        setThisBrace(ref rtxSender, intPreviousBraceValue + 1);
                        rtxSender.SelectionStart = intRememberSelectionStart + (intPreviousBraceValue + 1).ToString().Length;
                    }
                    else
                    {
                        insertNewBrace(ref rtxSender, intPreviousBraceValue + 1);
                        rtxSender.SelectionStart = intRememberSelectionStart + (intPreviousBraceValue + 1).ToString().Length + 1;
                    }
                    rtxSender.Show();
                    rtxSender.Focus();
                }
            }
        }

        static void setThisBrace(ref RichTextBox rtxSender, int intNewValue)
        {
            string strText = rtxSender.Text;
            int intIndexNextClose = rtxSender.Text.IndexOf(conCloseBrace, rtxSender.SelectionStart);
            int intIndexPreviousOpen = IndexOfBefore(ref strText, rtxSender.SelectionStart, conOpenBrace);

            rtxSender.SelectionStart = intIndexPreviousOpen + 1;
            int intLength = intIndexNextClose - intIndexPreviousOpen - 1;
            if (intLength > 0)
            {
                rtxSender.SelectionLength = intLength;
                rtxSender.SelectedText = intNewValue.ToString();

                rtxSender.SelectionStart = intIndexNextClose + 1;
                rtxSender.SelectionLength = 0;
                setNextBrace(ref rtxSender, intNewValue + 1);
            }
        }

        static void insertNewBrace(ref RichTextBox rtxSender, int intBraceValue)
        {
            int intRememberSelectionStart = rtxSender.SelectionStart;

            rtxSender.SelectionStart = ((intRememberSelectionStart > 0 && rtxSender.Text[intRememberSelectionStart - 1] == '#') ? intRememberSelectionStart - 1 : intRememberSelectionStart);
            rtxSender.SelectionLength = 1;

            string strNew = conOpenBrace + intBraceValue.ToString() + conCloseBrace;
            rtxSender.SelectedText = strNew;

            rtxSender.SelectionLength = 0;
            setNextBrace(ref rtxSender, intBraceValue + 1);

            rtxSender.SelectionStart = intRememberSelectionStart + intBraceValue.ToString().Length + 1;
        }

        static void setNextBrace(ref RichTextBox rtxSender, int intNextValue)
        {
            int intIndexNextOpen = rtxSender.Text.IndexOf(conOpenBrace, rtxSender.SelectionStart);
            while (intIndexNextOpen == rtxSender.SelectionStart && rtxSender.SelectionStart < rtxSender.Text.Length - 1)
                rtxSender.SelectionStart += 1;

            int intIndexNextClose = rtxSender.Text.IndexOf(conCloseBrace, rtxSender.SelectionStart);
            while (intIndexNextClose == rtxSender.SelectionStart && rtxSender.SelectionStart < rtxSender.Text.Length - 1)
            {
                rtxSender.SelectionStart += 1;
                intIndexNextClose = rtxSender.Text.IndexOf(conCloseBrace, rtxSender.SelectionStart);
            }

            if (intIndexNextClose >= 0 && intIndexNextOpen >= 0 && intIndexNextClose > intIndexNextOpen)
            {
                rtxSender.SelectionStart = intIndexNextOpen + 1;
                setThisBrace(ref rtxSender, intNextValue);
            }

        }

        static int FindPreviousBraceValue(ref RichTextBox rtxSender, int intIndex)
        {
            string strText = rtxSender.Text;
            int intIndexPreviousClose = IndexOfBefore(ref strText, intIndex, conCloseBrace);
            if (intIndexPreviousClose > 0)
            {
                int intIndexPreviousOpen = IndexOfBefore(ref strText, intIndexPreviousClose, conOpenBrace);
                if (intIndexPreviousOpen >= 0)
                {
                    string strBraceValue = strText.Substring(intIndexPreviousOpen + 1, intIndexPreviousClose - intIndexPreviousOpen - 1);
                    try
                    {
                        int intBraceValue = Convert.ToInt16(strBraceValue);
                        return intBraceValue;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
                else
                    return -1;
            }
            else
                return -1;
        }


        static int IndexOfBefore(ref string strText, int intStart, char chaSearch)
        {
            int intIndexOf = strText.IndexOf(chaSearch);
            if (intIndexOf >= 0 && intIndexOf <= intStart)
            {
                int intNextIndexOf = strText.IndexOf(chaSearch, intIndexOf + 1);
                while (intNextIndexOf < intStart && intNextIndexOf > 0)
                {
                    intIndexOf = intNextIndexOf;
                    intNextIndexOf = strText.IndexOf(chaSearch, intIndexOf + 1);
                }
                return intIndexOf;
            }
            else
                return -1;
        }

        #region "numbering"

        static bool cursorIsInsideBrackets(ref RichTextBox rtxSender)
        {
            string strText = rtxSender.Text;
            int intIndexPreviousOpen = IndexOfBefore(ref strText, rtxSender.SelectionStart, conOpenBrace);
            int intIndexPreviousClose = IndexOfBefore(ref strText, rtxSender.SelectionStart, conCloseBrace);
            int intIndexNextOpen = strText.IndexOf(conOpenBrace, rtxSender.SelectionStart);
            int intIndexNextClose = strText.IndexOf(conCloseBrace, rtxSender.SelectionStart);

            return (intIndexPreviousOpen >= 0 && intIndexNextClose >= 0
                    && intIndexPreviousOpen > intIndexPreviousClose
                    && intIndexNextOpen > intIndexNextClose);

        }

        #endregion


        void groupboxNotes_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }
        public void placeObjects()
        {
            pnlNotes.Top = 5;
            pnlNotes.Left = 5;
            pnlNotes.Width = Width - 10;
            pnlNotes.Height = splMain.Panel1.Height - pnlNotes.Top;

            if (!MainScreen) pnlRTX.Dock = DockStyle.Fill;

            picMainEditor.Width = (int)(rtxNotes.rtx.Width * 0.75);
            picMainEditor.Height = (int)(picMainEditor.Width * MainEditor_AspectRatio);

            picMainEditor.Location = new Point((rtxNotes.rtx.Width - picMainEditor.Width) / 2,
                                               (rtxNotes.rtx.Height - picMainEditor.Height) / 2);

            int intGap = 04;
            btnMainScreen_Toggle.Left = intGap;
            btnMainScreen_Toggle.Top = rtxNotes.Height - btnMainScreen_Toggle.Height - intGap;
            btnMainScreen_Toggle.BringToFront();
        }
        
      
        public string WorkingDirectory
        {
            get { return strWorkingDirectory; }
            set
            {
                strWorkingDirectory = value;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using CSRegisterHotkey;

namespace Words
{
    public class ck_RichTextBox : Panel
    {
        public enum enuLanguages { English, _numLanguages };

        #region ObjectsAndVariableDeclarations
        const float fltSubScriptSizeFactor = .7f;
        public static ck_RichTextBox instance = null;

        SPObjects.SPContainer SPContainer_WordSuggestions = null;
        SPObjects.ListBox lbxWordSuggestions = null;

        bool bolWordSuggestion_TempDisable = false;
        public bool WordSuggestion_TempDisable
        {
            get { return bolWordSuggestion_TempDisable; }
            set { bolWordSuggestion_TempDisable = value; }
        }

        static public bool WordSuggestion_Toggle
        {
            get { return formWords.grbOptions.Toggle_Word_Suggestion; }
            set { formWords.grbOptions.Toggle_Word_Suggestion = value; }
        }


        #region PrefixexAndSuffixex
        static List<string> _lstPrefixes = new List<string>();
        static public List<string> lstPrefixes
        {
            get
            {
                if (_lstPrefixes.Count == 0)
                {
                    string[] strTemp = {
                                        "",
                                        "anti",
                                        "auto",
                                        "con",
                                        "de",
                                        "dis",
                                        "down",
                                        "extra",
                                        "hyper",
                                        "il",
                                        "im",
                                        "ir",
                                        "inter",
                                        "in",
                                        "mega",
                                        "mid",
                                        "mis",
                                        "non",
                                        "over",
                                        "out",
                                        "post",
                                        "pre",
                                        "pro",
                                        "re",
                                        "semi",
                                        "sub",
                                        "super",
                                        "tele",
                                        "trans",
                                        "ultra",
                                        "under",
                                        "un",
                                        "up"
                                        };
                    _lstPrefixes.Clear();
                    _lstPrefixes.AddRange(strTemp.ToArray<string>());
                }
                return _lstPrefixes;
            }
        }

        static List<string> _lstSuffixes = new List<string>();
        public static List<string> lstSuffixes
        {
            get
            {
                if (_lstSuffixes.Count == 0)
                {
                    string[] strTemp =
                                  {
                                "",
                                "able",
                                "age",
                                "al",
                                "ance",
                                "ate",
                                "dom",
                                "ied",
                                "ed",
                                "d",
                                "ee",
                                "en",
                                "ence",
                                "er",
                                "e",
                                "ful",
                                "hood",
                                "ian",
                                "ible",
                                "ic",
                                "istic",
                                "ier",
                                "ify",
                                "ing",
                                "ise",
                                "ish",
                                "ism",
                                "ist",
                                "ity",
                                "ive",
                                "ize",
                                "i",
                                "less",
                                "ly",
                                "ment",
                                "ness",
                                "n",
                                "or",
                                "ous",
                                "ry",
                                "ese",
                                "es",
                                "s",
                                "est",
                                "ship",
                                "sion",
                                "tion",
                                "ty",
                                "ward",
                                "wards",
                                "wise",
                                "xion",
                                "y"
                            };
                    _lstSuffixes.Clear();
                    _lstSuffixes.AddRange(strTemp.ToList<string>());
                }
                return _lstSuffixes;
            }
        }
        #endregion 

        public string Heading
        {
            get {return lblHeading.Text; }
            set { lblHeading.Text = value; }
        }

        public Color Heading_BackColor
        {
            get { return lblHeading.BackColor;}
            set { lblHeading.BackColor = value; }
        }
        public Color Heading_ForeColor
        {
            get { return lblHeading.ForeColor;}
            set { lblHeading.ForeColor = value; }
        }
        
        public Font Heading_Font
        {
            get { return lblHeading.Font; }
            set { lblHeading.Font = value; }
        }

        Label lblHeading = new Label();
        public RichTextBox rtx = new RichTextBox();
        
        public panelToolBar ToolBar = null;
        panelRuler Ruler = null;

        MenuItem mnuFontBold = new MenuItem();
        MenuItem mnuFontUnderline = new MenuItem();
        MenuItem mnuFontStrikeOut = new MenuItem();
        MenuItem mnuFontItalic = new MenuItem();
        MenuItem mnuShowRuler = new MenuItem();
        MenuItem mnuShowToolBar = new MenuItem();
        MenuItem mnuHighlight = new MenuItem();

        MenuItem mnuAlignment_Left = new MenuItem();
        MenuItem mnuAlignment_Center = new MenuItem();
        MenuItem mnuAlignment_Right = new MenuItem();

        public ContextMenu cMnu = new ContextMenu();
        #endregion
        
        Label lblSelectionBackColor = new Label();

        RichTextBox rtxHelper = new RichTextBox();
        public const string strAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖÙÚÛÜÝßàáâãäåæçèéêëìíîïðñòóôõöùúûüýÿ";
        public static classKeyboard cKeyboard = new classKeyboard();

        const string conGetFirstWordKeepCharIgnoreCode = "keep no char";
        const int conWidth = 700;

        System.Windows.Forms.Timer tmrLblHighlighter_Hide = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer tmrSubclause_Select = new System.Windows.Forms.Timer();

        classGrammarCutter _cGrammarCutter = null;
        public classGrammarCutter cGrammarCutter
        {
            get
            {
                if (_cGrammarCutter == null)
                {
                    ck_RichTextBox cMyRef = this;
                    _cGrammarCutter = new classGrammarCutter(ref cMyRef);
                }
                return _cGrammarCutter;
            }
        }

        classCopyCutter _cCopyCutter = null;
        public classCopyCutter cCopyCutter { get { return _cCopyCutter; } }

        public class classCopyCutter
        {
            public enum enuAutoShutOffDelay { Minutes_One, Minutes_Five, Minutes_Fifteen, Minutes_Thirty, Minutes_Sixty, Never };
            enuAutoShutOffDelay _eAutoShutOffDelay = enuAutoShutOffDelay.Minutes_Five;
            public enuAutoShutOffDelay eAutoShutOffDelay
            {
                get { return _eAutoShutOffDelay; }
                set { _eAutoShutOffDelay = value; }
            }

            DateTime dtMostRecentCopy = DateTime.Now;

            public RichTextBox rtx
            {
                get
                {
                    if (ckRTX != null)
                        return ckRTX.rtx;
                    else
                        return null;
                }
            }

            ck_RichTextBox _ckRTX = null;
            ck_RichTextBox ckRTX
            {
                get { return _ckRTX; }
            }


            static string strPrefix = "\r\n-";
            static public string Prefix
            {
                get { return strPrefix; }
                set { strPrefix = value; }
            }

            static string strPostfix = "";
            static public string Postfix
            {
                get { return strPostfix; }
                set { strPostfix = value; }
            }


            public bool Enabled
            {
                get
                {
                    return tmrCopyCutter.Enabled;
                }
                set
                {
                    if (tmrCopyCutter.Enabled != value)
                    {
                        System.Windows.Forms.Clipboard.Clear();
                        tmrCopyCutter.Enabled = value;
                        if (tmrCopyCutter.Enabled)
                            dtMostRecentCopy = DateTime.Now;
                    }
                }
            }


            System.Windows.Forms.Timer tmrCopyCutter = new System.Windows.Forms.Timer();

            public classCopyCutter(ref ck_RichTextBox ckRTX)
            {
                this._ckRTX = ckRTX;

                tmrCopyCutter.Interval = 1000;
                tmrCopyCutter.Tick += TmrCopyCutter_Tick;
                tmrCopyCutter.Enabled = false;
            }

            string strTextInClipBoard_Previous = "";
            public string TextInClipBoard_Previous
            {
                get { return strTextInClipBoard_Previous; }
                set { strTextInClipBoard_Previous = value; }
            }

            private void TmrCopyCutter_Tick(object sender, EventArgs e)
            {
                DateTime dtNow = DateTime.Now;
                if (eAutoShutOffDelay != enuAutoShutOffDelay.Never)
                {
                    TimeSpan tsSinceLastCopy = dtNow.Subtract(dtMostRecentCopy);

                    double dblMaxSeconds = 0;
                    double dblSecondsPerMinute = 60;
                    switch (eAutoShutOffDelay)
                    {
                        case enuAutoShutOffDelay.Minutes_One:
                            dblMaxSeconds = 1 * dblSecondsPerMinute;
                            break;

                        case enuAutoShutOffDelay.Minutes_Five:
                            dblMaxSeconds = 5 * dblSecondsPerMinute;
                            break;

                        case enuAutoShutOffDelay.Minutes_Fifteen:
                            dblMaxSeconds = 15 * dblSecondsPerMinute;
                            break;

                        case enuAutoShutOffDelay.Minutes_Thirty:
                            dblMaxSeconds = 30 * dblSecondsPerMinute;
                            break;

                        case enuAutoShutOffDelay.Minutes_Sixty:
                            dblMaxSeconds = 60 * dblSecondsPerMinute;
                            break;
                    }
                    /*
                    if (tsSinceLastCopy.TotalSeconds > 10)
                    /*/
                    if (tsSinceLastCopy.TotalSeconds > dblMaxSeconds)
                    // */
                    {
                        // auto-shut off delay expired
                        ckRTX.ToolBar.btnCopyCutter_Toggle.Toggle_Off();

                        return;
                    }
                }


                if (System.Windows.Forms.Clipboard.ContainsText())
                {
                    if (ckRTX.CopyCutter_DisableOnce)
                    {
                        // this is the same ckRTX that changed the contents of the ClipBoard - Do Not Paste
                        ckRTX.CopyCutter_DisableOnce = false;
                        return;
                    }

                    string strClipboardText = System.Windows.Forms.Clipboard.GetText();
                    if (string.Compare(strClipboardText, TextInClipBoard_Previous) != 0)
                    {
                        if (rtx != null)
                        {
                            rtx.SelectedText = Prefix
                                             + System.Windows.Forms.Clipboard.GetText()
                                             + Postfix;
                            rtx.ScrollToCaret();
                        }
                        TextInClipBoard_Previous = strClipboardText;
                        dtMostRecentCopy = dtNow;
                    }
                }
                else if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    rtx.SelectedRtf = Clipboard.GetText(TextDataFormat.Rtf);
                    rtx.ScrollToCaret();
                    Clipboard.Clear();
                    dtMostRecentCopy = dtNow;
                }
                else if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    Bitmap bmpClipBoard = (Bitmap)System.Windows.Forms.Clipboard.GetImage();
                    ckRTX.InsertImage(bmpClipBoard);
                    rtx.ScrollToCaret();
                    try
                    {
                        System.Windows.Forms.Clipboard.Clear();
                    }
                    catch (Exception err)
                    {
                        System.Diagnostics.Debug.Print("ck_RichTextBox (" + ckRTX.Name + ") classCopyCutter.tmrCopyCutter_Tick() " + err.Message);
                    }
                    dtMostRecentCopy = dtNow;
                }
            }

            public class groupboxPrefixPostfix_Set : System.Windows.Forms.GroupBox
            {
                public static groupboxPrefixPostfix_Set instance = null;      

                Label lblPrefix = new Label();
                TextBox txtPrefix = new TextBox();

                Label lblPostFix = new Label();
                TextBox txtPostfix = new TextBox();

                Label lblSample = new Label();
                TextBox txtSample = new TextBox();

                Ck_Objects.classLabelButton btnOk = new Ck_Objects.classLabelButton();
                Ck_Objects.classLabelButton btnCancel = new Ck_Objects.classLabelButton();

                public groupboxPrefixPostfix_Set()
                {
                    Hide();

                    Controls.Add(lblPrefix);
                    lblPrefix.AutoSize = true;
                    lblPrefix.Text = "Prefix";

                    Controls.Add(txtPrefix);
                    txtPrefix.Multiline = true;
                    txtPrefix.TextChanged += Txt_TextChanged;

                    Controls.Add(lblPostFix);
                    lblPostFix.AutoSize = true;
                    lblPostFix.Text = "Postfix";

                    Controls.Add(txtPostfix);
                    txtPostfix.Multiline = true;
                    txtPostfix.TextChanged += Txt_TextChanged;

                    Controls.Add(lblSample);
                    lblSample.AutoSize = true;
                    lblSample.Text = "Sample text";

                    Controls.Add(txtSample);
                    txtSample.Multiline = true;
                    txtSample.Enabled = false;

                    btnOk.Text = "Ok";
                    btnOk.AutoSize = true;
                    btnOk.Font = new Font("arial", 8);
                    btnOk.Click += BtnOk_Click;
                    Controls.Add(btnOk);

                    btnCancel.Text = "Cancel";
                    btnCancel.AutoSize = true;
                    btnCancel.Font = new Font("arial", 8);
                    btnCancel.Click += BtnCancel_Click;
                    Controls.Add(btnCancel);

                    SizeChanged += GroupboxPrefixPostfix_Set_SizeChanged;
                }

                private void GroupboxPrefixPostfix_Set_SizeChanged(object sender, EventArgs e)
                {
                    placeObjects();
                }

                public void placeObjects()
                {
                    btnOk.Location = new Point(Width - 5 - btnOk.Width, Height - 5 - btnOk.Height);
                    btnCancel.Location = new Point(btnOk.Left - btnCancel.Width, btnOk.Top);

                    lblPrefix.Location = new Point(5, 15);
                    lblSample.Location = new Point(Width / 2, lblPrefix.Top);
                    lblPostFix.Location = new Point(lblPrefix.Left, Height / 2);

                    txtSample.Location = new Point(lblSample.Left, lblSample.Bottom);
                    txtSample.Width = btnOk.Right - txtSample.Left;
                    txtSample.Height = btnOk.Top - txtSample.Top;

                    txtPrefix.Location = new Point(lblPrefix.Left, lblPrefix.Bottom);
                    txtPrefix.Width = lblSample.Left - txtPrefix.Left;
                    txtPrefix.Height = lblPostFix.Top - txtPrefix.Top;

                    txtPostfix.Location = new Point(lblPostFix.Left, lblPostFix.Bottom);
                    txtPostfix.Width = txtPrefix.Width;
                    txtPostfix.Height = txtSample.Bottom - txtPostfix.Top;

              
                    btnOk.BringToFront();
                    btnCancel.BringToFront();
                }

                public void ShowOptions()
                {
                    placeObjects();
                    txtPrefix.Text = classCopyCutter.Prefix;
                    txtPostfix.Text = classCopyCutter.Postfix;

                    Show();
                    BringToFront();
                }

                private void Txt_TextChanged(object sender, EventArgs e)
                {
                    txtSample.Text = txtPrefix.Text + "<Sample Copy Cutter Input>" + txtPostfix.Text;

                    placeObjects();

                }

                private void BtnOk_Click(object sender, EventArgs e)
                {
                    classCopyCutter.Prefix = txtPrefix.Text;
                    classCopyCutter.Postfix = txtPostfix.Text;
                    Hide();
                }

                private void BtnCancel_Click(object sender, EventArgs e)
                {
                    Hide();
                }
            }
        }

        public static groupboxHighlighterColor_UI _grbHighlighterColor_UI = null;
        public static groupboxHighlighterColor_UI grbHighlighterColor_UI
        {
            get
            {
                if (_grbHighlighterColor_UI == null)
                {
                    ck_RichTextBox cMyRef = instance;
                    _grbHighlighterColor_UI = new groupboxHighlighterColor_UI(ref cMyRef);
                    formWords.ckRTX.rtx.Controls.Add(_grbHighlighterColor_UI);
                    _grbHighlighterColor_UI.Hide();
                    _grbHighlighterColor_UI.VisibleChanged += GrbHighlighterColor_UI_VisibleChanged;
                }
                return _grbHighlighterColor_UI;
            }
        }

        public static classCopyCutter.groupboxPrefixPostfix_Set grbCopyCutterOptions = null;
        public ck_RichTextBox()
        {
            ck_RichTextBox cMyRef 
                = instance
                = this;
            _cCopyCutter = new classCopyCutter(ref cMyRef);

            lstHighlighterItems_Init();

            Controls.Add(lblHeading);

            if (grbCopyCutterOptions == null)
                grbCopyCutterOptions = new classCopyCutter.groupboxPrefixPostfix_Set();            

            ToolBar = new panelToolBar(ref cMyRef);
            Controls.Add(ToolBar);
            Controls.Add(lblSelectionBackColor);
            lblSelectionBackColor.Font = new Font("arial", 12);
            lblSelectionBackColor.Text = "Highlighter";
            lblSelectionBackColor.Size = new Size(400, 20);
            lblSelectionBackColor.TextAlign = ContentAlignment.MiddleRight;
            lblSelectionBackColor.MouseWheel += LblSelectionBackColor_MouseWheel;
            lblSelectionBackColor.MouseClick += LblSelectionBackColor_MouseClick;
            lblSelectionBackColor.MouseEnter += LblSelectionBackColor_MouseEnter;

            tmrHighlightSelectedWord.Interval = 200;
            tmrHighlightSelectedWord.Tick += tmrHighlightSelectedWord_Tick;

            tmrLblHighlighter_Hide.Interval = 5000;
            tmrLblHighlighter_Hide.Tick += TmrLblHighlighter_Hide_Tick;

            Ruler = new panelRuler(ref cMyRef);
            Controls.Add(Ruler);

            Controls.Add(rtx);
            rtx.AcceptsTab = true;
            rtx.RightMargin = 1000;
            rtx.ScrollBars = RichTextBoxScrollBars.ForcedBoth;

            rtx.SelectionChanged += Rtx_SelectionChanged;
            rtx.KeyDown += rtx_KeyDown;
            rtx.KeyUp += Rtx_KeyUp;
            rtx.KeyPress += Rtx_KeyPress;
            rtx.MouseDown += Rtx_MouseDown;
            rtx.MouseUp += Rtx_MouseUp;
            rtx.MouseClick += Rtx_MouseClick;
            rtx.MouseDoubleClick += Rtx_MouseDoubleClick;
            rtx.MouseEnter += Rtx_MouseEnter;
            rtx.MouseLeave += Rtx_MouseLeave;
            rtx.MouseMove += Rtx_MouseMove;
            rtx.MouseWheel += Rtx_MouseWheel;
            rtx.TextChanged += Rtx_TextChanged;
            rtx.GotFocus += Rtx_GotFocus;
            rtx.LostFocus += Rtx_LostFocus;
            rtx.KeyUp += rtxCK_KeyUp;
            rtx.KeyDown += rtxCK_KeyDown;
            rtx.VScroll += Rtx_VScroll;

            tmrSubclause_Select.Interval = 1000;
            tmrSubclause_Select.Tick += tmrSubclause_Select_Tick;

            cMnu.Popup += CMnu_Popup;
            rtx.ContextMenu = cMnu;

            cMnu_Build();

            SPContainer_WordSuggestions = new SPObjects.SPContainer("WordSuggestions");

            rtx.Controls.Add(SPContainer_WordSuggestions);
            SPContainer_WordSuggestions.Hide();
            SPContainer_WordSuggestions.Size = new Size(100, 120);
            SPContainer_WordSuggestions.VisibleChanged += SPContainer_WordSuggestions_VisibleChanged;

            bool bolBuilding = SPContainer_WordSuggestions.BuildingInProgress;
            SPContainer_WordSuggestions.Building_Start();
            {
                lbxWordSuggestions = new SPObjects.ListBox(ref SPContainer_WordSuggestions);
                lbxWordSuggestions.Size = SPContainer_WordSuggestions.Size;
                lbxWordSuggestions.Location = new Point(0, 0);
                lbxWordSuggestions.Font = new Font("Arial", 10);
            }

            SPContainer_WordSuggestions.Building_Complete();


            SizeChanged += event_SizeChanged;
            Disposed += Ck_RichTextBox_Disposed;
        }

   
        private void SPContainer_WordSuggestions_VisibleChanged(object sender, EventArgs e)
        {
            if (!SPContainer_WordSuggestions.Visible)
                lbxWordSuggestions.SelectedIndex = 0;
        }


        public static bool CharValid(char chrTest) { return CharValid(chrTest, "-'"); }
        public static bool CharValid(char chrTest, string strValidChar)
        {   
            return char.IsLetter(chrTest) || strValidChar.Contains(chrTest);
        }


        public bool FindNext(string strTextFind)
        {
            strTextFind = strTextFind.Trim().ToUpper();
            
            int intCurrent = rtx.SelectionStart;
            if (rtx.Text.Length == 0) return false;

            string strTextCopy = rtx.Text.ToUpper();

            if (intCurrent < 0) intCurrent = 0;
            bool bolLoop = false;

            while (strTextCopy.Contains(strTextFind)) 
            {
                if (intCurrent < 0)
                {
                    if (bolLoop) return false;
                    bolLoop = true;
                    intCurrent = 0;
                }
                intCurrent = strTextCopy.IndexOf(strTextFind, intCurrent < strTextCopy.Length && intCurrent >= 0 ? intCurrent + 1 : 0);
                if (intCurrent >= 0)
                {
                    if (intCurrent ==0 ||(!char.IsLetter(strTextCopy[intCurrent - 1])))
                    {
                        if ((intCurrent + strTextFind.Length == strTextCopy.Length)  || !char.IsLetter( strTextCopy[intCurrent + strTextFind.Length]))
                        {
                            rtx.Select(intCurrent, strTextFind.Length);
                            rtx.Focus();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Font_Set(string strTextSearch, FontStyle fntStyle)
        {
            int intSelectionStart = rtx.SelectionStart;
            bool bolLoop = true;
            while (FindNext(strTextSearch))
            {
                rtx.SelectionFont = new Font(rtx.SelectionFont, fntStyle);
                if (rtx.SelectionStart < intSelectionStart)
                    bolLoop = false;
                else if (!bolLoop)
                    return;
            }
        }

        

        public string WordUnderCursor() { return WordUnderCursor("-'"); }
        public string WordUnderCursor(string strValidChar)
        {
            int intStart = 0;            
            int intEnd = 0;
            return WordUnderCursor(ref intStart, ref intEnd, strValidChar);
        }
        public string WordUnderCursor(ref int intSelectStart, ref int intSelectEnd) { return WordUnderCursor(ref intSelectStart, ref intSelectEnd, "-'"); }
        public string WordUnderCursor(ref int intSelectStart, ref int intSelectEnd, string strValidChar)
        {
            if (rtx.Text.Length == 0) return "";
            int intIndex = rtx.SelectionStart;

            if (intIndex >= rtx.Text.Length)
                intIndex = rtx.Text.Length - 1;

            intSelectStart
                = intSelectEnd
                = intIndex;

            char chrTest = rtx.Text[intSelectStart > 0 && intSelectStart < rtx.Text.Length ? intSelectStart - 1 : 0];

            if (CharValid(chrTest, strValidChar))
            {
                while (intSelectStart > 0 && CharValid(chrTest, strValidChar))
                    chrTest = rtx.Text[--intSelectStart];
                intSelectStart++;
            }

            chrTest = rtx.Text[intSelectEnd];
            if (CharValid(chrTest, strValidChar))
            {
                while (intSelectEnd < rtx.Text.Length -1  && CharValid(chrTest, strValidChar))
                    chrTest = rtx.Text[++intSelectEnd];
            }
            if (intSelectStart >= 0 && intSelectEnd > intSelectStart && intSelectEnd < rtx.Text.Length)
            {
                string strRetVal = rtx.Text.Substring(intSelectStart, intSelectEnd - intSelectStart);
                return strRetVal;
            }
            else
                return "";

        }
        public string PunctuationUnderCursor() { return PunctuationUnderCursor("-'"); }
        public string PunctuationUnderCursor(string strValidChar)
        {
            int intStart = 0;            
            int intEnd = 0;
            return PunctuationUnderCursor(ref intStart, ref intEnd, strValidChar);
        }
        public string PunctuationUnderCursor(ref int intSelectStart, ref int intSelectEnd) { return PunctuationUnderCursor(ref intSelectStart, ref intSelectEnd, "-'"); }
        public string PunctuationUnderCursor(ref int intSelectStart, ref int intSelectEnd, string strValidChar)
        {
            if (rtx.Text.Length == 0) return "";
            int intIndex = rtx.SelectionStart;

            if (intIndex >= rtx.Text.Length)
                intIndex = rtx.Text.Length - 1;

            intSelectStart
                = intSelectEnd
                = intIndex;

            char chrTest = rtx.Text[intSelectStart > 0 && intSelectStart < rtx.Text.Length ? intSelectStart - 1 : 0];

            if (!CharValid(chrTest, strValidChar))
            {
                while (intSelectStart > 0 && !CharValid(chrTest, strValidChar))
                    chrTest = rtx.Text[--intSelectStart];
                intSelectStart++;
            }

            chrTest = rtx.Text[intSelectEnd];
            if (!CharValid(chrTest, strValidChar))
            {
                while (intSelectEnd < rtx.Text.Length -1  && !CharValid(chrTest, strValidChar))
                    chrTest = rtx.Text[++intSelectEnd];
            }
            if (intSelectStart >= 0 && intSelectEnd > intSelectStart && intSelectEnd < rtx.Text.Length)
            {
                string strRetVal = rtx.Text.Substring(intSelectStart, intSelectEnd - intSelectStart);
                return strRetVal;
            }
            else
                return "";

        }

        public void DeleteWordUnderCursor()
        {
            int intSelectEnd = 0, intSelectStart = 0;

            if (rtx.SelectionLength >0)
            {
                intSelectStart = rtx.SelectionStart;
                rtx.SelectedText = "";
                rtx.Select(intSelectStart, 0);
                rtx.Focus();
                return;
            }

            string strWordUnderCursor = WordUnderCursor(ref intSelectStart, ref intSelectEnd);

            while (intSelectEnd < rtx.Text.Length-1 && rtx.Text[intSelectEnd] == ' ')
                intSelectEnd++;

            if (intSelectEnd > intSelectStart)
            {
                rtx.Select(intSelectStart, intSelectEnd - intSelectStart);
                rtx.SelectedText = "";
                rtx.Select(intSelectStart, 0);
            }
            else
            {
                char chrBefore = rtx.SelectionStart > 0
                                                    ? rtx.Text[rtx.SelectionStart - 1]
                                                    : '!';
                char chrAfter = rtx.SelectionStart < rtx.Text.Length - 2
                                                   ? rtx.Text[rtx.SelectionStart + 1]
                                                   : '!';
                string strTestEmptySpace = "\r\n\t ";
                //if (strTestEmptySpace.Contains(chrBefore) && strTestEmptySpace.Contains(chrAfter))
                //{
                int intStart = rtx.SelectionStart;
                while (intStart >= 0 && intStart < rtx.Text.Length  && strTestEmptySpace.Contains(rtx.Text[intStart]))
                    intStart--;

                int intEnd = rtx.SelectionStart;
                while (intEnd < rtx.Text.Length - 1 && strTestEmptySpace.Contains(rtx.Text[intEnd]))
                    intEnd++;

                if (intStart < intEnd)
                {
                    rtx.Select(intStart +1, intEnd - intStart-1);
                    //string strInsert = rtx.SelectedText.Contains("\n")
                    //                                           ? "\n\r"
                    //                                           : " ";
                    string strInsert = " ";

                    rtx.SelectedText = strInsert;
                    return;
                }

                //}



                //    string strPunctuationUnderCursor = PunctuationUnderCursor(ref intSelectStart, ref intSelectEnd);

                //while (intSelectEnd < rtx.Text.Length - 1 && rtx.Text[intSelectEnd] == ' ')
                //    intSelectEnd++;

                //if (intSelectEnd > intSelectStart)
                //{
                //    rtx.Select(intSelectStart, intSelectEnd - intSelectStart);
                //    string strTextInsert = " ";
                //    rtx.SelectedText = strTextInsert;
                //    rtx.Select(intSelectStart+strTextInsert.Length, 0);
                //}
            }

            rtx.Focus();
        }

        public void Text_Insert(string strText)
        {
            Font fnt = rtx.SelectionFont;
            Text_Insert(strText, fnt);
        }

        public void Text_Insert(string strText, Font fnt)
        {
            Color clrFore = rtx.SelectionColor;
            Color clrBack = rtx.SelectionBackColor;
            Text_Insert(strText, fnt, clrFore, clrBack);
        }

        public void Text_Insert(string strText, Color clrFore, Color clrBack)
        {
            Font fnt = rtx.SelectionFont;
            Text_Insert(strText, fnt, clrFore, clrBack);
        }
        public void Text_Insert(string strText, Font fnt, Color clrFore, Color clrBack)
        {
            int intIndex_Start = rtx.SelectionStart;
            int intIndex_Length = strText.Length;

            rtx.SelectedText = strText;
            rtx.Select(intIndex_Start, intIndex_Length);
            {
                rtx.SelectionFont = fnt;
                rtx.SelectionColor = clrFore;
                rtx.SelectionBackColor = clrBack;
            }
            rtx.Select(intIndex_Start + intIndex_Length, 0);
            rtx.Focus();
        }
        public void Text_ReplaceWord(string strText) { Text_ReplaceWord(strText, false); }
        public void Text_ReplaceWord(string strText, bool bolKeepNewWordCapitalization)
        {
            Font fnt = rtx.SelectionFont;
            Text_ReplaceWord(strText, fnt, bolKeepNewWordCapitalization);
        }

        public void Text_ReplaceWord(string strText, Font fnt, bool bolKeepNewWordCapitalization)
        {
            Color clrFore = rtx.SelectionColor;
            Color clrBack = rtx.SelectionBackColor;
            Text_ReplaceWord(strText, fnt, clrFore, clrBack, bolKeepNewWordCapitalization);
        }
        static string strValidChar = "-";
        public static bool Char_IsValid(char chr)
        {
            return char.IsLetter(chr) || strValidChar.Contains(chr);
        }
        
        public void Text_ReplaceWord(string strText, Color clrFore, Color clrBack, bool bolKeepNewWordCapitalization)
        {
            Font fnt = rtx.SelectionFont;
            Text_ReplaceWord(strText, fnt, clrFore, clrBack, bolKeepNewWordCapitalization);
        }
        public void Text_ReplaceWord(string strText, Font fnt, Color clrFore, Color clrBack, bool bolKeepNewWordCapitalization)
        {
            int intIndex_Length = strText.Length;
            int intStart = 0, intEnd = 0;
            string strText_Old = WordUnderCursor(ref intStart, ref intEnd);

            rtx.Select(intStart, intEnd - intStart);
            //strText_Old = rtx.SelectedText;

            if (!bolKeepNewWordCapitalization)
            {
                // test for First Capital Letters
                int intNumCap = 0;
                while (intNumCap < strText_Old.Length
                        && intNumCap < strText.Length
                        && char.IsUpper(strText_Old[intNumCap++])) ;
                intNumCap--;
                char[] chrText = strText.ToArray<char>();
                if (intNumCap > 0)
                    for (int intCharCounter = 0; intCharCounter < intNumCap; intCharCounter++)
                        chrText[intCharCounter] = char.ToUpper(chrText[intCharCounter]);
                strText = new String(chrText);
            }

            if (rtx.SelectionLength == 0)
            {

                if (rtx.Text.Length > rtx.SelectionStart)
                {
                    char chrTest = rtx.Text[rtx.SelectionStart];
                    if (chrTest == '\n')
                    {
                        strText += " ";
                        intIndex_Length++;
                    }
                }
            }

            rtx.SelectedText = strText;
            rtx.Select(intStart, intIndex_Length);
            {
                rtx.SelectionFont = fnt;
                rtx.SelectionColor = clrFore;
                rtx.SelectionBackColor = clrBack;
            }
            rtx.Select(intStart + intIndex_Length, 0);
        }

        private void LblSelectionBackColor_MouseEnter(object sender, EventArgs e)
        {
            lblSelectionBackColor.Focus();
        }

        void tmrLblHighlighter_Hide_Reset()
        {
            tmrLblHighlighter_Hide.Enabled = false;
            tmrLblHighlighter_Hide.Enabled = true;
        }

        bool bolHighlighterLabel_Hide_Automatically = false;
        public bool HighlighterLabel_Hide_Automatically
        {
            get { return bolHighlighterLabel_Hide_Automatically; }
            set { bolHighlighterLabel_Hide_Automatically = value; }
        }

        private void TmrLblHighlighter_Hide_Tick(object sender, EventArgs e)
        {
            tmrLblHighlighter_Hide.Enabled = false;
            if (HighlighterLabel_Hide_Automatically)
                lblSelectionBackColor.Hide();
        }

        private void Ck_RichTextBox_Disposed(object sender, EventArgs e)
        {
            if (event_Disposed != null)
                event_Disposed((object)this, new EventArgs());
        }


        #region Properties


        EventHandler _event_Disposed = null;
        public EventHandler event_Disposed
        {
            get { return _event_Disposed; }
            set { _event_Disposed = value; }
        }


        EventHandler _eventFile_Load = null;
        public EventHandler File_Load
        {
            get { return _eventFile_Load; }
            set { _eventFile_Load = value; }
        }
        EventHandler _eventFile_Save = null;
        public EventHandler File_Save
        {
            get { return _eventFile_Save; }
            set { _eventFile_Save = value; }
        }
        EventHandler _eventFile_SaveAs = null;
        public EventHandler File_SaveAs
        {
            get { return _eventFile_SaveAs; }
            set { _eventFile_SaveAs = value; }
        }
        EventHandler _eventFile_New = null;
        public EventHandler File_New
        {
            get { return _eventFile_New; }
            set { _eventFile_New = value; }
        }

        public string Language
        {
            get { return ToolBar.Language; }
            set { ToolBar.Language = value; }
        }


        bool bolShowToolBar = true;
        public bool ShowToolBar
        {
            get { return bolShowToolBar; }
            set
            {
                bolShowToolBar = value;
                placeObjects();
            }
        }

        bool bolToolTip_Enabled = true;
        public bool ToolTip_Enabled
        {
            get { return bolToolTip_Enabled; }
            set
            {
                bolToolTip_Enabled
                    = ToolBar.ToolTip_Enabled
                    = value;
            }
        }

        bool bolShowRuler = true;
        public bool ShowRuler
        {
            get { return bolShowRuler; }
            set
            {
                bolShowRuler = value;
                placeObjects();
            }
        }

        #region RTX_Property_Reflections

        string strPathAndFilename = "";
        public string PathAndFilename
        {
            get { return strPathAndFilename; }
            set 
            {
                strPathAndFilename = value;
                int intCut_LastDecimal = strPathAndFilename.LastIndexOf('.');
                string strFilename = strPathAndFilename.Substring(0, intCut_LastDecimal > 0 ? intCut_LastDecimal : strPathAndFilename.Length);
                int intCut_LastBackSpace = strPathAndFilename.LastIndexOf('\\');
                strFilename = strFilename.Length > 0
                                                 ? (1+ strFilename.Substring(intCut_LastBackSpace > 0 ? intCut_LastBackSpace : 0))
                                                 : strFilename;
                
             
            }
        }

        public override string Text
        {
            get { return rtx.Text; }
            set { rtx.Text = value; }
        }

        public int SelectionStart
        {
            get { return rtx.SelectionStart; }
            set { rtx.SelectionStart = value; }
        }

        public int SelectionLength
        {
            get { return rtx.SelectionLength; }
            set { rtx.SelectionLength = value; }
        }

        public string SelectedText
        {
            get { return rtx.SelectedText; }
            set { rtx.SelectedText = value; }
        }

        public Font SelectionFont
        {
            get { return rtx.SelectionFont; }
            set { rtx.SelectionFont = value; }
        }

        public int SelectionCharOffset
        {
            get { return rtx.SelectionCharOffset; }
            set { rtx.SelectionCharOffset = value; }
        }

        public Color SelectionBackColor
        {
            get { return rtx.SelectionBackColor; }
            set { rtx.SelectionBackColor = value; }
        }

        public Color SelectionForeColor
        {
            get { return rtx.SelectionColor; }
            set { rtx.SelectionColor = value; }
        }

        public int RightMargin
        {
            get { return rtx.RightMargin; }
            set { rtx.RightMargin = value; }
        }

        public int SelectionRightIndent
        {
            get { return rtx.SelectionRightIndent; }
            set { rtx.SelectionRightIndent = value; }
        }

        public int SelectionIndent
        {
            get { return rtx.SelectionIndent; }
            set { rtx.SelectionIndent = value; }
        }

        public int SelectionHangingIndent
        {
            get { return rtx.SelectionHangingIndent; }
            set { rtx.SelectionHangingIndent = value; }
        }



        MouseEventHandler _MouseDown = null;
        new public MouseEventHandler MouseDown
        {
            get { return _MouseDown; }
            set { _MouseDown = value; }
        }

        MouseEventHandler _MouseWheel = null;
        new public MouseEventHandler MouseWheel
        {
            get { return _MouseWheel; }
            set { _MouseWheel = value; }
        }

        EventHandler _SelectionChanged = null;
        public EventHandler SelectionChanged
        {
            get { return _SelectionChanged; }
            set { _SelectionChanged = value; }
        }


        MouseEventHandler _MouseUp = null;
        new public MouseEventHandler MouseUp
        {
            get { return _MouseUp; }
            set { _MouseUp = value; }
        }


        MouseEventHandler _MouseClick = null;
        new public MouseEventHandler MouseClick
        {
            get { return _MouseClick; }
            set { _MouseClick = value; }
        }


        MouseEventHandler _MouseDoubleClick = null;
        new public MouseEventHandler MouseDoubleClick
        {
            get { return _MouseDoubleClick; }
            set { _MouseDoubleClick = value; }
        }


        MouseEventHandler _MouseMove = null;
        new public MouseEventHandler MouseMove
        {
            get { return _MouseMove; }
            set { _MouseMove = value; }
        }



        EventHandler _MouseEnter = null;
        new public EventHandler MouseEnter
        {
            get { return _MouseEnter; }
            set { _MouseEnter = value; }
        }


        EventHandler _MouseLeave = null;
        new public EventHandler MouseLeave
        {
            get { return _MouseLeave; }
            set { _MouseLeave = value; }
        }


        KeyEventHandler _KeyDown = null;
        new public KeyEventHandler KeyDown
        {
            get { return _KeyDown; }
            set { _KeyDown = value; }
        }


        KeyEventHandler _KeyUp = null;
        new public KeyEventHandler KeyUp
        {
            get { return _KeyUp; }
            set { _KeyUp = value; }
        }


        List<MenuItem> _lstMnuParent = new List<MenuItem>();
        public List<MenuItem> lstMnuParent
        {
            get { return _lstMnuParent; }
            set { _lstMnuParent = value; }
        }

        #endregion


        #endregion


        #region Graphics
        public enum enuGraphics_Lines { Straight, Squiggle, Circle };

        bool bolHighlightSelectedWord = false;
        public bool HighlightSelectedWord
        {
            get { return bolHighlightSelectedWord; }
            set
            {
                bolHighlightSelectedWord = value;
                rtx.Refresh();
                if (HighlightSelectedWord)
                    tmrHighlightSelectedWord_Tick((object)this, new EventArgs());
            }
        }

        
        bool bolLocked = false;
        public bool Locked
        {
            get { return bolLocked; }
            set
            {
                bolLocked = value;
            }
        }




        System.Windows.Forms.Timer tmrHighlightSelectedWord = new System.Windows.Forms.Timer();


        bool bolGr_Reset = false;
        public bool Gr_Reset
        {
            get { return bolGr_Reset; }
            set
            {
                if (bolGr_Reset != value)
                {
                    bolGr_Reset = value;
                    if (bolGr_Reset)
                        rtx.Refresh();
                }
                tmrHighlightSelectedWord_Reset();
            }
        }

    
        public void tmrHighlightSelectedWord_Reset()
        {
            tmrHighlightSelectedWord.Enabled = false;
            tmrHighlightSelectedWord.Enabled = true;
        }

        private void Rtx_VScroll(object sender, EventArgs e)
        {
            tmrHighlightSelectedWord_Reset();
        }

        public void tmrHighlightSelectedWord_Tick(object sender, EventArgs e)
        {
            tmrHighlightSelectedWord.Enabled = false;
            if (HighlightSelectedWord)
            {
                int intSelectStart = 0, intSelectEnd = 0;
                WordUnderCursor(ref intSelectStart, ref intSelectEnd);

                if (intSelectEnd > intSelectStart)
                {
                    string strWord = rtx.Text.Substring(intSelectStart, intSelectEnd - intSelectStart);
                    if (strWord.Length > 1)
                    {
                        Pen p = new Pen(Color.Red, 2);
                        Underline(p, strWord, enuGraphics_Lines.Squiggle);
                    }
                }
            }
        }

        public void Underline(Pen p, string strText, enuGraphics_Lines eGLine)
        {
            List<string> lstText = new List<string>();
            lstText.Add(strText);
            Underline(ref rtx, p, lstText, eGLine);
        }
        static public void Underline(ref Graphics g, ref RichTextBox rtx, Pen p,Point ptWord_Start,Point ptWord_End, int intY_NextLine, enuGraphics_Lines eGLine)
        {
     
            switch (eGLine)
            {
                case enuGraphics_Lines.Squiggle:
                    {
                        ptWord_End.Y
                             = ptWord_Start.Y
                             = intY_NextLine - 2;
                        int intStepWidth = 3;
                        Point[] pts = new Point[2];
                        int intPtIndex = 0;
                        int intPtAlt = 1;
                        pts[intPtIndex] = ptWord_Start;
                        pts[intPtAlt] = new Point(ptWord_Start.X + intStepWidth, ptWord_Start.Y + intStepWidth);
                        do
                        {
                            g.DrawLine(p, pts[intPtAlt], pts[intPtIndex]);

                            intPtIndex = (intPtIndex + 1) % 2;
                            intPtAlt = (intPtAlt + 1) % 2;

                            pts[intPtAlt].X = pts[intPtIndex].X + intStepWidth;
                            pts[intPtAlt].Y = pts[intPtIndex].Y + 2 * (intPtAlt % 2 != 0 ? 1 : -1) * intStepWidth;

                        } while (pts[intPtIndex].X < ptWord_End.X);
                    }
                    break;

                case enuGraphics_Lines.Straight:
                    {
                        ptWord_End.Y
                             = ptWord_Start.Y
                             = intY_NextLine - 2;
                        g.DrawLine(p, ptWord_Start, ptWord_End);
                    }
                    break;

                case enuGraphics_Lines.Circle:
                    {
                        int intGap = 3;
                        Point ptTL = new Point(ptWord_Start.X - intGap, ptWord_Start.Y - intGap);
                        Point ptBR = new Point(ptWord_End.X + intGap, intY_NextLine + intGap);
                        g.DrawEllipse(p, ptTL.X, ptTL.Y, (ptBR.X - ptTL.X), ptBR.Y - ptTL.Y);
                    }
                    break;
            }
        }

        static public void Underline(ref RichTextBox rtx, Pen p, List<string> lstText, enuGraphics_Lines eGLine)
        {
            rtx.Refresh();
            if (lstText.Count == 0) return;

            int int_IndexTL = rtx.GetCharIndexFromPosition(new Point(0, 0));
            int int_IndexBR = rtx.GetCharIndexFromPosition(new Point(rtx.Width, rtx.Height));

            Graphics g = rtx.CreateGraphics();
            {
                for (int intSearchWord = 0; intSearchWord < lstText.Count; intSearchWord++)
                {
                    string strText = lstText[intSearchWord].ToUpper().Trim();
                    if (strText.Length > 0)
                    {
                        int intIndexSearch = int_IndexTL;
                        if (intIndexSearch < 0)
                            intIndexSearch = 0;

                        int intIndex_Result = rtx.Text.ToUpper().IndexOf(strText, intIndexSearch);

                        while (intIndex_Result >= 0 && intIndex_Result < int_IndexBR)
                        {
                            int intIndexStart = intIndex_Result;
                            int intIndexEnd = intIndexStart + strText.Length;

                            char chrBefore = intIndexStart > 0
                                                           ? rtx.Text[intIndexStart - 1]
                                                           : ' ';
                            int intCharAfter = intIndexStart + strText.Length;
                            char chrAfter = intCharAfter < rtx.Text.Length && intCharAfter >=0
                                                         ? rtx.Text[intCharAfter]
                                                         : ' ';

                            string strTemp = rtx.Text.Substring(intIndexStart, 25);

                            if (!char.IsLetter(chrBefore))
                                if (!char.IsLetter(chrAfter))
                            {
                                do
                                {
                                    Point ptWord_Start = rtx.GetPositionFromCharIndex(intIndexStart);
                                    Point ptWord_End = rtx.GetPositionFromCharIndex(intIndexEnd);
                                    Point ptNextLine = ptWord_End;

                                    int intCharNextLine = intIndexEnd + 1;

                                    if (ptWord_Start.Y == ptWord_End.Y)
                                    { // word is contained on a single-line
                                        while (intCharNextLine < rtx.Text.Length - 1 && ptNextLine.Y == ptWord_End.Y)
                                            ptNextLine = rtx.GetPositionFromCharIndex(intCharNextLine++);

                                        Underline(ref g, ref rtx, p, ptWord_Start, ptWord_End, ptNextLine.Y, eGLine);
                                        break;
                                    }
                                    
                                    intCharNextLine = intIndexStart;
                                    Point ptRightMost = rtx.GetPositionFromCharIndex(intCharNextLine);
                                    ptNextLine = ptWord_Start;
                                    while (intCharNextLine < rtx.Text.Length - 1 && ptNextLine.Y == ptWord_Start.Y)
                                    {
                                        ptRightMost = ptNextLine;
                                        ptNextLine = rtx.GetPositionFromCharIndex(intCharNextLine++);
                                    }

                                    int intRightMost = rtx.GetCharIndexFromPosition(ptRightMost);

                                    Underline(ref g, ref rtx, p, ptWord_Start, ptRightMost, ptNextLine.Y, eGLine);

                                    intIndexStart = intRightMost + 1;
                                    ptWord_Start = rtx.GetPositionFromCharIndex(intIndexStart);
                                } while (true);
                            }
                            intIndex_Result = rtx.Text.ToUpper().IndexOf(strText, intIndex_Result + 1);
                        }
                    }
                }
            }
            g.Dispose();



        }
        #endregion


        #region Methods

        void cMnu_Build()
        {
            cMnu.MenuItems.Clear();
            if (MouseButtons == MouseButtons.Left)
            {
                string strWordAtCursor = WordUnderCursor();
                
                MenuItem mnuPrefix = new MenuItem(strWordAtCursor +  " : Substitute Prefix");
                {
                    for (int intPrefixCounter = 0; intPrefixCounter < lstPrefixes.Count; intPrefixCounter++)
                    {
                        MenuItem mnuPrefix_Item = new MenuItem(lstPrefixes[intPrefixCounter], mnuPrefix_Swap_Click);
                        mnuPrefix_Item.Tag = (object)lstPrefixes[intPrefixCounter];

                        mnuPrefix.MenuItems.Add(mnuPrefix_Item);
                    }
                }
                cMnu.MenuItems.Add(mnuPrefix);

                MenuItem mnuSuffix = new MenuItem(strWordAtCursor + " : Substitute Suffix");
                {
                    for (int intSuffixCounter = 0; intSuffixCounter < lstSuffixes.Count; intSuffixCounter++)
                    {
                        MenuItem mnuSuffix_Item = new MenuItem(lstSuffixes[intSuffixCounter], mnuSuffix_Swap_Click);
                        mnuSuffix_Item.Tag = (object)lstSuffixes[intSuffixCounter];

                        mnuSuffix.MenuItems.Add(mnuSuffix_Item);
                    }
                }
                cMnu.MenuItems.Add(mnuSuffix);
            }
            else
            {

                //                                                      Edit
                MenuItem mnuEdit = new MenuItem("Edit");
                {
                    MenuItem mnuEdit_SelectAll = new MenuItem("Select All", mnuEdit_SelectAll_Click, Shortcut.CtrlA);
                    mnuEdit.MenuItems.Add(mnuEdit_SelectAll);

                    MenuItem mnuEdit_Copy = new MenuItem("Copy", mnuEdit_Copy_Click, Shortcut.CtrlC);
                    mnuEdit.MenuItems.Add(mnuEdit_Copy);

                    MenuItem mnuEdit_Cut = new MenuItem("Cut", mnuEdit_Cut_Click, Shortcut.CtrlX);
                    mnuEdit.MenuItems.Add(mnuEdit_Cut);

                    MenuItem mnuEdit_Paste = new MenuItem("Paste", mnuEdit_Paste_Click, Shortcut.CtrlV);
                    mnuEdit.MenuItems.Add(mnuEdit_Paste);

                    mnuEdit.MenuItems.Add(new MenuItem("insert image", mnuInsertImage_Click));
                }
                cMnu.MenuItems.Add(mnuEdit);

                //                                                      Font
                MenuItem mnuFont = new MenuItem("Font");
                {
                    mnuFontBold = new MenuItem("Bold", mnuFont_Bold_click, Shortcut.CtrlB);
                    mnuFont.MenuItems.Add(mnuFontBold);
                    mnuFontBold.Checked = rtx.SelectionFont != null && (rtx.SelectionFont.Style & FontStyle.Bold) != 0;

                    mnuFontUnderline = new MenuItem("Underline", mnuFont_Underline_click, Shortcut.CtrlU);
                    mnuFont.MenuItems.Add(mnuFontUnderline);
                    mnuFontUnderline.Checked = rtx.SelectionFont != null && (rtx.SelectionFont.Style & FontStyle.Underline) != 0;

                    mnuFontStrikeOut = new MenuItem("StrikeOut", mnuFont_StrikeOut_click, Shortcut.CtrlK);
                    mnuFont.MenuItems.Add(mnuFontStrikeOut);
                    mnuFontStrikeOut.Checked = rtx.SelectionFont != null && (rtx.SelectionFont.Style & FontStyle.Strikeout) != 0;

                    mnuFontItalic = new MenuItem("Italic", mnuFont_Italic_click, Shortcut.CtrlI);
                    mnuFont.MenuItems.Add(mnuFontItalic);
                    mnuFontItalic.Checked = rtx.SelectionFont != null && (rtx.SelectionFont.Style & FontStyle.Italic) != 0;

                    mnuFont.MenuItems.Add("Regular", mnuFont_Regular_click);
                    mnuFont.MenuItems.Add(new MenuItem("Font Settings", mnuFont_Click, Shortcut.CtrlShiftF));

                    //                                                      Colors
                    MenuItem mnuColor = new MenuItem("Color");
                    {
                        mnuColor.MenuItems.Add(new MenuItem("ForeColor", mnuForeColor_Click));
                        mnuColor.MenuItems.Add(new MenuItem("BackColor", mnuBackColor_Click));
                    }
                    mnuFont.MenuItems.Add(mnuColor);
                }
                cMnu.MenuItems.Add(mnuFont);



                //                                                      Vertical Position
                MenuItem mnuVerticalPosition = new MenuItem("Vertical Position");
                {
                    mnuVerticalPosition.MenuItems.Add(new MenuItem("SuperScript", mnuSuperScript_Click));
                    mnuVerticalPosition.MenuItems.Add(new MenuItem("SubScript", mnuSubScript_Click));
                    mnuVerticalPosition.MenuItems.Add(new MenuItem("normal", mnuNormalScript_Click));
                }
                cMnu.MenuItems.Add(mnuVerticalPosition);

                //                                                      Alignment
                MenuItem mnuAlignment = new MenuItem("Alignment");
                {
                    mnuAlignment_Left = new MenuItem("Left", mnuAlignment_Left_Click, Shortcut.CtrlL);
                    mnuAlignment.MenuItems.Add(mnuAlignment_Left);
                    mnuAlignment_Left.Checked = (rtx.SelectionAlignment == HorizontalAlignment.Left);

                    mnuAlignment_Center = new MenuItem("Center", mnuAlignment_Center_Click, Shortcut.CtrlE);
                    mnuAlignment.MenuItems.Add(mnuAlignment_Center);
                    mnuAlignment_Center.Checked = (rtx.SelectionAlignment == HorizontalAlignment.Center);

                    mnuAlignment_Right = new MenuItem("Right", mnuAlignment_Right_Click, Shortcut.CtrlR);
                    mnuAlignment.MenuItems.Add(mnuAlignment_Right);
                    mnuAlignment_Right.Checked = (rtx.SelectionAlignment == HorizontalAlignment.Right);
                }
                cMnu.MenuItems.Add(mnuAlignment);

                //                                                      Find
                MenuItem mnuFind = new MenuItem("Find", mnuFind_Click, Shortcut.CtrlF);
                cMnu.MenuItems.Add(mnuFind);

                ////                                                      SpellCheck
                //MenuItem mnuSpellCheck = new MenuItem("Spell Check", mnuSpellCheck_Click, Shortcut.F7);
                //cMnu.MenuItems.Add(mnuSpellCheck);

                //                                                      Replace
                MenuItem mnuReplace = new MenuItem("Replace", mnuReplace_Click, Shortcut.CtrlH);
                cMnu.MenuItems.Add(mnuReplace);


                //                                                      highlight
                mnuHighlight = new MenuItem("Highlight");
                {
                    mnuHighlight.MenuItems.Add(new MenuItem("Scroll Down", mnuHighlighter_ScrollDown_Click));
                    mnuHighlight.MenuItems.Add(new MenuItem("Scroll Up", mnuHighlighter_ScrollUp_Click));
                    mnuHighlight.MenuItems.Add(new MenuItem("set Background Color", mnuHighlight_SetColorSequence_Click));
                    MenuItem mnuHighlight_ToggleAutoHideLabel = new MenuItem("Toggle Auto Hide Label", mnuHighlight_AutoHideToolBarLabel_Click);
                    mnuHighlight_ToggleAutoHideLabel.Checked = HighlighterLabel_Hide_Automatically;
                    mnuHighlight.MenuItems.Add(mnuHighlight_ToggleAutoHideLabel);
                }
                cMnu.MenuItems.Add(mnuHighlight);

                //                                                      Textbox Color
                MenuItem mnuTextBoxColor = new MenuItem("Textbox Background Color", mnuTextBoxColor_Click);
                cMnu.MenuItems.Add(mnuTextBoxColor);


                cMnu.MenuItems.Add(new MenuItem("Word Count", mnuOptions_NumberOfWords_Click));
                MenuItem mnuCopyCutter = new MenuItem("Copy Cutter", mnuCopyCutter_Click);
                mnuCopyCutter.Checked = cCopyCutter.Enabled;
                cMnu.MenuItems.Add(mnuCopyCutter);

                MenuItem mnuReader = new MenuItem("Reader", mnuReader_Click);
                mnuReader.Checked = Reader_AutoHighlight_Toggle;
                cMnu.MenuItems.Add(mnuReader);

                for (int intMnuParentCounter = 0; intMnuParentCounter < lstMnuParent.Count; intMnuParentCounter++)
                    cMnu.MenuItems.Add(lstMnuParent[intMnuParentCounter]);

                //                                                      ShowRuler
                mnuShowRuler = new MenuItem("Show Ruler", mnuShowRuler_Click);
                mnuShowRuler.Checked = ShowRuler;
                cMnu.MenuItems.Add(mnuShowRuler);

                //                                                      Show ToolBar
                mnuShowToolBar = new MenuItem("Show ToolBar", mnuShowToolBar_Click);
                mnuShowToolBar.Checked = ShowToolBar;
                cMnu.MenuItems.Add(mnuShowToolBar);
            }

        }

        void placeObjects()
        {
            lblHeading.Location = new Point(0, 0);
            lblHeading.Width = Width;
            Size szHeading = TextRenderer.MeasureText(lblHeading.Text, lblHeading.Font);
            lblHeading.Height = szHeading.Height + 3;

            Point ptTL = new Point(0, lblHeading.Bottom);
            if (ShowToolBar)
            {
                ToolBar.Width = Width;
                ToolBar.placeObjects();
                ToolBar.Location = ptTL;
                ptTL.Y = ToolBar.Bottom;
                ToolBar.Visible = true;
            }
            else
                ToolBar.Visible = false;

            if (ShowRuler)
            {
                Ruler.placeObjects();
                Ruler.Location = ptTL;
                ptTL.Y = Ruler.Bottom;
                Ruler.Visible = true;
            }
            else
                Ruler.Visible = false;

            rtx.Location = ptTL;
            rtx.Height = Height - rtx.Top;
            rtx.Width = Width;
            //BackColor = Color.Orange;
            lblColor_Draw();
        }

        #region RTX_Method_Reflections
        public void Select(int Start, int Length)
        {
            rtx.Select(Start, Length);
        }

        new public void Select() { rtx.Select(); }

        public Point GetPositionFromCharIndex(int index)
        {
            return rtx.GetPositionFromCharIndex(index);
        }
        public void ScrollToCaret()
        {
            rtx.ScrollToCaret();
        }

        public void LoadFile(string path)
        {
            if (System.IO.File.Exists(path))
            rtx.LoadFile(path);
        }

        public void SaveFile(string path)
        {
            rtx.SaveFile(path);
        }
        #endregion

        #endregion

        #region Events
        private void CMnu_Popup(object sender, EventArgs e)
        {
            cMnu_Build();
        }

        private void event_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        void mnuShowRuler_Click(object sender, EventArgs e)
        {
            ShowRuler = !ShowRuler;
        }

        void mnuShowToolBar_Click(object sender, EventArgs e)
        {
            ShowToolBar = !ShowToolBar;
        }

        private void Rtx_GotFocus(object sender, EventArgs e)
        {
            if (formFindReplace.instance != null && !formFindReplace.instance.IsDisposed)
                formFindReplace.instance.ckRTX = this;
        }

        
        private void Rtx_TextChanged(object sender, EventArgs e)
        {
            if (formFindReplace.instance != null && !formFindReplace.instance.IsDisposed)
                formFindReplace.instance.Search_Reset();
        }


        bool bolSelectionChangedWorking = false;
        bool SelectionChangedWorking
        {
            get { return bolSelectionChangedWorking; }
            set { bolSelectionChangedWorking = value; }
        }

        private void Rtx_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChangedWorking) return;

            SelectionChangedWorking = true;
            {
                ToolBar.SetToSelectedText();
                Ruler.SetToSelectedText();
                if (SelectionChanged != null)
                    SelectionChanged(sender, e);

                lblColor_Draw();
                Reader_AutoHighlight();
            }
            SelectionChangedWorking = false;
        }


        bool bolReader_AutoHighlight_Toggle = false;
        public bool Reader_AutoHighlight_Toggle
        {
            get { return bolReader_AutoHighlight_Toggle; }
            set
            {
                bolReader_AutoHighlight_Toggle = value;
                Reader_AutoHighlight();
            }
        }

        void Reader_AutoHighlight()
        {
            if (Reader_AutoHighlight_Toggle)
            {
                if (rtx.SelectionLength == 0)
                {
                    Point ptLeft = rtx.GetPositionFromCharIndex(rtx.SelectionStart);
                    ptLeft.X = 1;
                    int intSelectionStart = rtx.GetCharIndexFromPosition(ptLeft);

                    Point ptRight = new Point(rtx.Width, ptLeft.Y);
                    int intSelectionEnd = rtx.GetCharIndexFromPosition(ptRight);

                    int intSelection_Length = intSelectionEnd - intSelectionStart;
                    if (intSelectionStart >= 0 && intSelection_Length > 0)
                    {
                        rtx.Select(intSelectionStart, intSelectionEnd - intSelectionStart);
                    }
                }
            }
        }

        public void lblColor_Draw()
        {

            Color clrSelectionBack = SelectionBackColor;
            Color clrSelectionFore = SelectionForeColor;

            int intColorItemIndex = Highlighter_ColorSelected(ref rtx);

            if (intColorItemIndex >= 0)
            {
                lblSelectionBackColor.BackColor = lstHighlighterItems[intColorItemIndex].clrBack;
                lblSelectionBackColor.ForeColor = lstHighlighterItems[intColorItemIndex].clrFore;
                lblSelectionBackColor.Text = lstHighlighterItems[intColorItemIndex].Text;
            }
            else
            {
                lblSelectionBackColor.BackColor = clrSelectionBack;
                lblSelectionBackColor.ForeColor = clrSelectionFore;
                lblSelectionBackColor.Text = "-unrecognized-";
            }
            lblSelectionBackColor.AutoSize = true;
            lblSelectionBackColor.Location = new Point(rtx.Width - lblSelectionBackColor.Width - 4,
                                                       0);
            lblSelectionBackColor.BringToFront();
            lblSelectionBackColor.Show();

            tmrLblHighlighter_Hide_Reset();
            tmrHighlightSelectedWord_Reset();
        }

        private void Rtx_MouseMove(object sender, MouseEventArgs e)
        {
    
        }

        private void Rtx_MouseLeave(object sender, EventArgs e)
        {
            if (MouseLeave != null) MouseLeave(sender, e);
        }

        private void Rtx_MouseEnter(object sender, EventArgs e)
        {
               if (MouseEnter != null) MouseEnter(sender, e);
        }

        private void Rtx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MouseDoubleClick != null) MouseDoubleClick(sender, e);
        }

        private void Rtx_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseClick != null) MouseClick(sender, e);
        }

        private void Rtx_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null) MouseUp(sender, e);
        }

        private void Rtx_MouseDown(object sender, MouseEventArgs e)
        {
            Gr_Reset = true;
            if (MouseDown != null) MouseDown(sender, e);
        }


        Keys _keyDownLast = Keys.Escape;
        Keys keyDownLast
        {
            get { return _keyDownLast; }
            set { _keyDownLast = value; }
        }

        private void Rtx_LostFocus(object sender, EventArgs e)
        {
            keyDownLast = Keys.Escape;
            SPContainer_WordSuggestions.Hide();
        }

        void rtxCK_KeyUp(object sender, KeyEventArgs e)
        {
            keyDownLast = Keys.Escape;
        }
        
        void rtxCK_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownLast = e.KeyCode;
        }

        private void Rtx_MouseWheel(object sender, MouseEventArgs e)
        {
            if (keyDownLast == Keys.Menu)
            {
                if (e.Delta > 0)
                    ScrollDown(e);
                else
                    ScrollUp(e);
            }
            if (MouseWheel != null)
                MouseWheel(sender, e);
        }

        private void tmrSubclause_Select_Tick(object sender, EventArgs e)
        {
            cGrammarCutter.cClause.Index = rtx.SelectionStart;
        }

        void tmrSubclause_Select_Reset()
        {
            tmrSubclause_Select.Enabled = false;
            tmrSubclause_Select.Enabled = true;
        }

        private void Rtx_KeyUp(object sender, KeyEventArgs e)
        {
            cKeyboard.HandleKeyPress(sender, e);


            if (!WordSuggestion_Toggle) return;

            int intIndexPrevious = rtx.SelectionStart - 1;
            char chrBeforeCursor = intIndexPrevious > 0
                                                    ? rtx.Text[intIndexPrevious]
                                                    : ' ';
            char chrAfterCursor = rtx.SelectionStart < rtx.Text.Length
                                                     ? rtx.Text[rtx.SelectionStart]
                                                     : ' ';

            if (Char_IsValid(chrBeforeCursor) || Char_IsValid(chrAfterCursor))
            {
                if (WordSuggestion_TempDisable) return;

                string strWordUnderCursor = WordUnderCursor();

                List<string> lstSuggestions = WordSuggestion(strWordUnderCursor);

                if (lstSuggestions.Count > 0)
                {
                    bool bolBuilding = SPContainer_WordSuggestions.BuildingInProgress;
                    SPContainer_WordSuggestions.Building_Start();
                    {
                        SPContainer_WordSuggestions.BackColor = Color.Purple;
                        lbxWordSuggestions.BackColor = Color.LightGray;
                        lbxWordSuggestions.Items.Clear();
                        for (int intWordCounter = 0; intWordCounter < lstSuggestions.Count; intWordCounter++)
                            lbxWordSuggestions.Items.Add(lstSuggestions[intWordCounter]);


                        SPContainer_WordSuggestions.recSPArea
                            = SPContainer_WordSuggestions.recVisible
                            = new Rectangle(0, 0, lbxWordSuggestions.Width, lbxWordSuggestions.Height);
                    }
                    if (!bolBuilding)
                        SPContainer_WordSuggestions.Building_Complete();

                    Point ptCursorLocation = GetPositionFromCharIndex(SelectionStart);
                    Rectangle recTest = new Rectangle(ptCursorLocation.X, ptCursorLocation.Y + 15, SPContainer_WordSuggestions.Width, SPContainer_WordSuggestions.Height);

                    if (recTest.Bottom > Height)
                        recTest.Location = new Point(recTest.Left, ptCursorLocation.Y - recTest.Height);
                    if (recTest.Right > Width)
                        recTest.Location = new Point(ptCursorLocation.X - recTest.Width, recTest.Top);

                    SPContainer_WordSuggestions.Location = recTest.Location;
                    SPContainer_WordSuggestions.Show();
                }
                else
                    SPContainer_WordSuggestions.Hide();
            }
            else
            {
                SPContainer_WordSuggestions.Hide();
                WordSuggestion_TempDisable = false;
            }
        }

        bool _bolCopyCutter_DisableOnce = false;
        bool CopyCutter_DisableOnce
        {
            get { return _bolCopyCutter_DisableOnce; }
            set { _bolCopyCutter_DisableOnce = value; }
        }

        private void Rtx_KeyPress(object sender, KeyPressEventArgs e)
        {
        }


        private void rtx_KeyDown(object sender, KeyEventArgs e)
        {
            
            Gr_Reset = true;
            if (Reader_AutoHighlight_Toggle)
            {
                switch (e.KeyCode)
                {
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Control:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        return;

                    default:
                        e.SuppressKeyPress = true;
                        return;
                }
            }



            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        mnuFont_Bold_click((object)this, new EventArgs());
                        break;

                    case Keys.I:
                        mnuFont_Italic_click((object)this, new EventArgs());
                        break;

                    case Keys.U:
                        mnuFont_Underline_click((object)this, new EventArgs());
                        break;

                    case Keys.K:
                        mnuFont_StrikeOut_click((object)this, new EventArgs());
                        break;

                    case Keys.H:
                        mnuFind_Click(sender, e);
                        break;

                    case Keys.F:
                        mnuReplace_Click(sender, e);
                        break;

                    case Keys.C:
                        CopyCutter_DisableOnce = cCopyCutter.Enabled;                // tell CopyCutter that this is the rtxCK that changed ClipBoard data content
                        mnuEdit_Copy_Click((object)this, new EventArgs());
                        break;

                    case Keys.X:
                        CopyCutter_DisableOnce = cCopyCutter.Enabled;                // tell CopyCutter that this is the rtxCK that changed ClipBoard data content
                        mnuEdit_Cut_Click((object)this, new EventArgs());
                        break;

                    case Keys.V:
                        //mnuEdit_Paste_Click((object)this, new EventArgs());
                        break;

                    case Keys.S:
                        mnuFile_Save_Click((object)this, new EventArgs());
                        break;

                    case Keys.Delete:
                        {
                            DeleteWordUnderCursor();
                            e.SuppressKeyPress = true;
                        }
                        break;

                    case Keys.Enter:
                        {
                            NewLine_setSameAsPrevious();
                            e.SuppressKeyPress = true;
                            return;
                        }
                        break;
                }
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.F)
            {
                mnuFont_Click((object)this, new EventArgs());
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift | Keys.Alt))
            {  // highlighter scroll keys
                e.SuppressKeyPress = true;
                switch (e.KeyCode)
                {
                    case Keys.U:
                        Font_Set(rtx.SelectedText, FontStyle.Underline);
                        break;

                    case Keys.B:
                        Font_Set(rtx.SelectedText, FontStyle.Bold);
                        break;

                    case Keys.K:
                        Font_Set(rtx.SelectedText, FontStyle.Strikeout);
                        break;

                    case Keys.R:
                        Font_Set(rtx.SelectedText, FontStyle.Regular);
                        break;

                    case Keys.I:
                        Font_Set(rtx.SelectedText, FontStyle.Italic);
                        break;

                    case Keys.A:
                        ScrollUp();
                        break;

                    case Keys.Z:
                        ScrollDown();
                        break;

                    case Keys.Q:
                        Highlighter_Insert();
                        break;

                    case Keys.W:
                        {
                            panelDictionaryOutput pnlDictionaryOutput = panelDictionaryOutput.instance;
                            if (pnlDictionaryOutput.pnlDefPort != null)
                            {
                                // position mouse over the current RichTextBox - to avoid tripping mouseEnter event

                                //Control ctrRef = (Control)rtx;
                                //Point ptMousePosition = MousePosition;
                                //Point ptTL_rtxThis = Ck_Objects.classControlLocation.Location(ref ctrRef);

                                //Cursor.Position = new Point(ptTL_rtxThis.X + rtx.Width / 2, ptTL_rtxThis.Y + rtx.Height / 2);

                                // set focus
                                pnlDictionaryOutput.pnlDefPort.cmbHeadings.Text = WordUnderCursor();
                                pnlDictionaryOutput.pnlDefPort.cmbHeadings.Focus();
                            }
                        }
                        break;
                    
                    case Keys.C:
                        Clipboard.SetText(rtx.SelectedText);
                        break;

                    case Keys.Up:
                        LblSelectionBackColor_MouseWheel((object)lblSelectionBackColor, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 1));
                        break;

                    case Keys.Down:
                        LblSelectionBackColor_MouseWheel((object)lblSelectionBackColor, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, -1));
                        break;

                    case Keys.Insert:
                        ToolBar.btnFindWord_Click();
                        break;

                    case Keys.Left:
                        {
                            WordUnderMouse_MoveLeft();
                        }
                        break;

                    case Keys.Right:
                        {
                            WordUnderMouse_MoveRight();
                        }
                            break;
                }

            }
            else if (formFindReplace.instance != null
                    && !formFindReplace.instance.IsDisposed
                    && formFindReplace.instance.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        formFindReplace.instance.Hide();
                        break;

                    case Keys.Enter:
                        e.SuppressKeyPress = true;
                        formFindReplace.instance.BtnFindNext_Click((object)this, new EventArgs());
                        break;
                }

            }
            else
            {
                if (!WordSuggestion_Toggle) return;

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        {                            
                            if (SPContainer_WordSuggestions.Visible && lbxWordSuggestions.SelectedIndex > 0)
                            {
                                lbxWordSuggestions.SelectedIndex -= 1;
                                e.SuppressKeyPress = true;
                            }
                        }
                        break;

                    case Keys.Down:
                        {
                            if (SPContainer_WordSuggestions.Visible && lbxWordSuggestions.SelectedIndex < lbxWordSuggestions.Items.Count - 1)
                            {
                                lbxWordSuggestions.SelectedIndex += 1;
                                e.SuppressKeyPress = true;
                            }
                        }
                        break;

                    case Keys.Escape:
                        {
                            SPContainer_WordSuggestions.Hide();
                            WordSuggestion_TempDisable = !WordSuggestion_TempDisable;
                            e.SuppressKeyPress = true;
                        }
                        break;

                    case Keys.Tab:
                        {
                            if (SPContainer_WordSuggestions.Visible)
                            {
                                if (lbxWordSuggestions.SelectedIndex >= 0 && lbxWordSuggestions.SelectedIndex < lbxWordSuggestions.Items.Count)
                                {
                                    string strWordNew = (string)lbxWordSuggestions.Items.Get(lbxWordSuggestions.SelectedIndex);
                                    Text_ReplaceWord(strWordNew);
                                }

                                SPContainer_WordSuggestions.Hide();
                                WordSuggestion_TempDisable = false;

                                e.SuppressKeyPress = true;
                            }
                        }
                        break;
                }
            }
        }


        void NewLine_setSameAsPrevious()
        {
            if (rtx.Text.Length == 0) return;
            int intStart = rtx.SelectionStart > 0
                                              ? rtx.SelectionStart - 1
                                              : rtx.SelectionStart;
            char chrTest = rtx.Text[intStart];
            while (chrTest != '\n' && intStart > 0)
                chrTest = rtx.Text[--intStart];

            string strInsert = "\r";
            while (!char.IsLetterOrDigit(chrTest) && intStart < rtx.Text.Length - 2)
            {
                strInsert += chrTest.ToString();
                chrTest = rtx.Text[++intStart];
            }
            rtx.SelectedText = strInsert;
        }

        public void WordUnderMouse_MoveLeft()
        {
            int intIndexLeft = rtx.SelectionStart;
            while (intIndexLeft > 0 && CharValid(rtx.Text[--intIndexLeft])) ;

            while (intIndexLeft > 0 && !CharValid(rtx.Text[--intIndexLeft])) ;

            while (intIndexLeft > 0 && CharValid(rtx.Text[--intIndexLeft])) ;
            intIndexLeft++;

            int intLeft_Start = 0, intLeft_End = 0;
            int intRight_Start = 0, intRight_End = 0;

            FindWord(intIndexLeft, ref intLeft_Start, ref intLeft_End);

            if (rtx.SelectionLength == 0)
                FindWord(rtx.SelectionStart, ref intRight_Start, ref intRight_End);
            else
            {
                intRight_Start = rtx.SelectionStart;
                intRight_End = rtx.SelectionStart + rtx.SelectionLength;
            }

            WordsSwap(intLeft_Start, intLeft_End, intRight_Start, intRight_End);
        }

        public void WordUnderMouse_MoveRight()
        {
            int intIndexRight = rtx.SelectionStart + (rtx.SelectionLength > 0 ? rtx.SelectionLength - 1 : 0);
            while (intIndexRight < rtx.Text.Length - 1 && CharValid(rtx.Text[++intIndexRight])) ;

            while (intIndexRight < rtx.Text.Length - 1 && !CharValid(rtx.Text[++intIndexRight])) ;

            int intLeft_Start = 0, intLeft_End = 0;
            int intRight_Start = 0, intRight_End = 0;

            FindWord(intIndexRight, ref intRight_Start, ref intRight_End);

            if (rtx.SelectionLength == 0)
                FindWord(rtx.SelectionStart, ref intLeft_Start, ref intLeft_End);
            else
            {
                intLeft_Start = rtx.SelectionStart;
                intLeft_End = rtx.SelectionStart + rtx.SelectionLength;
            }
            string strDebug_Left = rtx.Text.Substring(intLeft_Start, intLeft_End - intLeft_Start);
            string strDebug_Right = rtx.Text.Substring(intRight_Start, intRight_End - intRight_Start);



            WordsSwap(intLeft_Start, intLeft_End, intRight_Start, intRight_End);

        }

        char[] _chrSplit = null;
        char[] chrSplit
        {
            get
            {
                if (_chrSplit == null)
                {
                    List<char> lstCharSplit = new List<char>();
                    for (int i = 0; i < 255; i++)
                    {
                        char chrNew = (char)i;
                        if (!char.IsLetter(chrNew))
                            lstCharSplit.Add(chrNew);
                    }

                    _chrSplit = lstCharSplit.ToArray();
                }
                return _chrSplit;
            }
        }

        List<string> WordSuggestion(string strWordStart)
        {
            string[] strWords = rtx.Text.Split(chrSplit, StringSplitOptions.RemoveEmptyEntries);

            List<string> lstWordSuggestion = new List<string>();
            lstWordSuggestion.AddRange(strWords.ToList<string>().Distinct<string>());
            lstWordSuggestion = StringLibrary.classStringLibrary.Alphabetize_Words(lstWordSuggestion);

            if (lstWordSuggestion.Count == 0) return new List<string>();

            strWordStart = strWordStart.Trim().ToLower();

            int intMax = lstWordSuggestion.Count - 1;
            int intStepSize = intMax;
            int intIndex = intStepSize / 2;
            int intDir = 1;

            List<string> lstWordsTried = new List<string>();

            do
            {
                intStepSize /= 2;
                if (intStepSize < 1)
                    intStepSize = 1;

                string strWordInList = lstWordSuggestion[intIndex];
                if (lstWordsTried.Contains(strWordInList))
                    return new List<string>();

                string strWordTest = (strWordInList.Length > strWordStart.Length
                                                          ? strWordInList.Substring(0, strWordStart.Length)
                                                          : strWordInList).ToLower();
                intDir = string.Compare(strWordStart, strWordTest);

                List<string> lstRetVal = new List<string>();

                if (intDir == 0)
                {
                    int intStartIndex = intIndex;
                    while (intStartIndex >= 0)
                    {
                        strWordInList = lstWordSuggestion[intStartIndex].ToLower();
                        strWordTest = strWordInList.Length > strWordStart.Length
                                                           ? strWordInList.Substring(0, strWordStart.Length)
                                                           : "";
                        int intComparison = string.Compare(strWordStart, strWordTest);
                        if (intComparison != 0)
                            break;

                        if (!lstRetVal.Contains(strWordInList))
                            lstRetVal.Add(strWordInList);
                        intStartIndex--;
                    }

                    int intEndIndex = intIndex + 1;
                    while (intEndIndex < lstWordSuggestion.Count)
                    {
                        strWordInList = lstWordSuggestion[intEndIndex].ToLower();
                        strWordTest = strWordInList.Length > strWordStart.Length
                                                           ? strWordInList.Substring(0, strWordStart.Length)
                                                           : "";
                        int intComparison = string.Compare(strWordStart, strWordTest);
                        if (intComparison != 0)
                            break;

                        if (!lstRetVal.Contains(strWordInList))
                            lstRetVal.Add(strWordInList);
                        intEndIndex++;
                    }

                    lstRetVal = StringLibrary.classStringLibrary.Alphabetize_Words(lstRetVal);
                    return lstRetVal;
                }

                intIndex += intStepSize * intDir;

                if (intIndex > intMax)
                    intIndex = intMax;
                if (intIndex < 0)
                    intIndex = 0;

                lstWordsTried.Add(strWordInList);
            } while (true);
        }

        public void WordsSwap(int intStart_0, int intEnd_0, int intStart_1, int intEnd_1)
        {
            if (intStart_0 < 0
             || intEnd_0 < intStart_0
             || intStart_1 < 0
             || intEnd_1 < intStart_1
             || intEnd_0 > rtx.Text.Length
             || intEnd_1 > rtx.Text.Length)
                return;

            int intLeft_Start=0, intLeft_End=0, intRight_Start=0, intRight_End=0;

            if (intStart_0 < intStart_1)
            {
                intLeft_Start = intStart_0;
                intLeft_End = intEnd_0;
                intRight_Start = intStart_1;
                intRight_End = intEnd_1;
            }
            else
            {
                intLeft_Start = intStart_1;
                intLeft_End = intEnd_1;
                intRight_Start = intStart_0;
                intRight_End = intEnd_0;
            }

            _WordsSwap(intLeft_Start, intLeft_End, intRight_Start, intRight_End);
        }

        public static bool WordStartsSentence(ref RichTextBox rtx, int intIndexWordStart)
        {
            int intIndexTest = intIndexWordStart;

            while (intIndexTest >0)
            {
                char chrTest = rtx.Text[--intIndexTest];
                if (char.IsLetter(chrTest)) return false;
                if ("\r\n!?.".Contains(chrTest)) return true;
            }
            return true;
        }

        public static float WordCapitalizedInText(ref RichTextBox rtx, string strWord)
        {
            float fltNumAppearances = 0;
            float fltNumCapitalized = 0;

            int intIndexTest = rtx.Text.IndexOf(strWord);
            while (intIndexTest >0)
            {
                char chrBefore = intIndexTest > 0
                                              ? rtx.Text[intIndexTest - 1]
                                              : ' ';
                int intEndWordIndex = intIndexTest + strWord.Length;
                char chrAfter = intEndWordIndex < rtx.Text.Length - 2
                                                ? rtx.Text[intEndWordIndex]
                                                : ' ';
                if (!char.IsLetter(chrBefore) && !char.IsLetter(chrAfter))
                {
                    if (!WordStartsSentence(ref rtx, intIndexTest))
                    {
                        fltNumAppearances++;
                        char chrFirstLetter = rtx.Text[intIndexTest];
                        if (char.IsUpper(chrFirstLetter))
                            fltNumCapitalized++;
                    }
                }
                intIndexTest = rtx.Text.IndexOf(strWord, intIndexTest + 1);
            }

            return fltNumCapitalized / fltNumAppearances;
        }

        void _WordsSwap(int intStart_0, int intEnd_0, int intStart_1, int intEnd_1)
        {
            if (intStart_0 == intEnd_0 && intStart_1 == intEnd_1) return;

            bool bolTextSelected = rtx.SelectionLength > 0;
            int intSelectLength = rtx.SelectionLength;

            int intSel = rtx.SelectionStart;
            int intSelIndex = 0;
            bool bolSelFirstWord = false;

            if (intSel >= intStart_0 && intSel < intEnd_0)
            {
                bolSelFirstWord = true;
                intSelIndex = intSel - intStart_0;
            }
            else if (intSel >=intStart_1 && intSel < intEnd_1)
            {
                bolSelFirstWord = false;
                intSelIndex = intSel - intStart_1;
            }

            int intStartShift_0 = 0, intStartShift_1 = 0, intEndShift_0 = 0, intEndShift_1 = 0;

            if (intStart_0 == intEnd_0)
            { // left word is empty -> remove one space-char from right word
                if (intEnd_1 < rtx.Text.Length -1)
                {
                    if (rtx.Text[intEnd_1 ] == ' ')
                    {
                        intEndShift_1++;
                    }
                    else
                    { // check for space-char before right word
                        if (intStart_1 >0)
                        {
                            if (rtx.Text[intStart_1 - 1] == ' ')
                                intStartShift_1--;
                        }
                    }
                }
            }
            else if (intStart_1 == intEnd_1)
            { // right word is empty -> remove one space-char from left word
                if (intEnd_0 < rtx.Text.Length - 1)
                {
                    if (rtx.Text[intEnd_0] == ' ')
                    {
                        intEndShift_0++;
                    }
                    else
                    { // check for space-char before right word
                        if (intStart_0 > 0)
                        {
                            if (rtx.Text[intStart_0 - 1] == ' ')
                                intStartShift_0--;
                        }
                    }
                }
            }

            string strWordLeft = rtx.Text.Substring(intStart_0, intEnd_0 - intStart_0);
            string strWordRight = rtx.Text.Substring(intStart_1, intEnd_1 - intStart_1);

            rtx.Select(intStart_0 + intStartShift_0, intEnd_0 + intEndShift_0 - (intStart_0+intStartShift_0));
            string strText0 = rtx.SelectedText.Trim();
            bool bolLeft_Capitalized = strText0.Length > 0
                                                       ? char.IsUpper(strText0[0])
                                                       : false;
            bool bolStartSentence0 = WordStartsSentence(ref rtx, intStart_0);
            float fltPercentageCapitalized0 = WordCapitalizedInText(ref rtx, strText0);

            rtx.Select(intStart_1 + intStartShift_1, intEnd_1 +intEndShift_1  - (intStart_1 + intStartShift_1));
            string strText1 = rtx.SelectedText.Trim();
            bool bolRight_Capitalized = strText1.Length > 0
                                                       ? char.IsUpper(strText1[0])
                                                       : false;
            bool bolStartSentence1 = WordStartsSentence(ref rtx, intStart_1);
            float fltPercentageCapitalized1 = WordCapitalizedInText(ref rtx, strText1);

            if (strText0.Length > 0)
            {
                if (bolStartSentence1 && bolRight_Capitalized)
                    strText0 = (char.ToUpper(strText0[0]) + strText0.Substring(1));
                else if (bolStartSentence0 && fltPercentageCapitalized0 != 1 && !bolRight_Capitalized) 
                    strText0 = (char.ToLower(strText0[0]) + strText0.Substring(1));
            }
            rtx.SelectedText = strText0;

            rtx.Select(intStart_0 + intStartShift_0, intEnd_0 +intEndShift_0  - (intStart_0  + intStartShift_0));

            if (strText1.Length > 0)
            {
                if (bolStartSentence0 && bolLeft_Capitalized)
                    strText1 = (char.ToUpper(strText1[0]) + strText1.Substring(1));
                else if (bolStartSentence1 && fltPercentageCapitalized1 != 1 && !bolLeft_Capitalized)
                    strText1 = (char.ToLower(strText1[0]) + strText1.Substring(1));
            }
            rtx.SelectedText = strText1;

            if (bolSelFirstWord)
            {
                if (bolTextSelected)
                    rtx.Select(intStart_1 - strWordLeft.Length + strWordRight.Length, intSelectLength);
                else
                    rtx.Select(intStart_1 + intSelIndex - strWordLeft.Length + strWordRight.Length, 0);
            }
            else
            {
                if (bolTextSelected)
                    rtx.Select(intStart_0, intSelectLength);
                else
                    rtx.Select(intStart_0 + intSelIndex, 0);
            }
        }


        void FindWord(int intIndex, ref int intStart, ref int intEnd)
        {
            if (intIndex < 0 || intIndex >= rtx.Text.Length) return;

            if (intIndex > 0)
            {
                char chr0 = rtx.Text[intIndex - 1];
                char chr1 = rtx.Text[intIndex];

                if (!char.IsLetter(chr0) && !char.IsLetter(chr1))
                {
                    intStart
                        = intEnd
                        = intIndex;
                    return;
                }
                if (intIndex > 1 && !char.IsLetter(chr1) && char.IsLetter(chr0))
                    intIndex--;
            }

            intStart = intIndex;
            char chrTest = rtx.Text[intStart];
            while (ck_RichTextBox.CharValid(chrTest) && intStart > 0)
                chrTest = rtx.Text[--intStart];
            if (!char.IsLetter(chrTest)) intStart++;

            intEnd = intIndex;
            chrTest = rtx.Text[intEnd];
            while (ck_RichTextBox.CharValid(chrTest) && intEnd < rtx.Text.Length - 1)
                chrTest = rtx.Text[++intEnd];
        }


        void mnuPrefix_Swap_Click(object sender, EventArgs e)
        {
            MenuItem mnuSender = (MenuItem)sender;
            string strPrefix_New = (string)mnuSender.Tag;
            //if (strPrefix_New.Length > 0)
            {
                string strPrefix_Old = "";
                int intSelectStart = 0, intSelectEnd = 0;
                WordUnderCursor(ref intSelectStart, ref intSelectEnd);
                ck_RichTextBox ckMyRef = this;
                string strWordAtSelection = getWordAtSelection(ref ckMyRef).ToLower();
                for (int intPrefixCounter = 1; intPrefixCounter < lstPrefixes.Count; intPrefixCounter++)
                {
                    string strPrefixTest = lstPrefixes[intPrefixCounter];
                    if (strWordAtSelection.Length > strPrefixTest.Length)
                    {
                        if (string.Compare(strWordAtSelection.Substring(0, strPrefixTest.Length), strPrefixTest) == 0)
                        {
                            strPrefix_Old = strPrefixTest;
                            break;
                        }
                    }
                }
                //if (strPrefix_Old.Length > 0)
                {
                    string strWord_New = strPrefix_New + strWordAtSelection.Substring(strPrefix_Old.Length);
                    Text_ReplaceWord(strWord_New);
                }
            }
        }
        void mnuSuffix_Swap_Click(object sender, EventArgs e)
        {
            MenuItem mnuSender = (MenuItem)sender;
            string strSuffix_New = (string)mnuSender.Tag;
            //if (strSuffix_New.Length > 0)
            {
                string strSuffix_Old = "";
                int intSelectStart = 0, intSelectEnd = 0;
                WordUnderCursor(ref intSelectStart, ref intSelectEnd);
                ck_RichTextBox ckMyRef = this;
                string strWordAtSelection = getWordAtSelection(ref ckMyRef).ToLower();
                for (int intSuffixCounter = 1; intSuffixCounter < lstSuffixes.Count; intSuffixCounter++)
                {
                    string strSuffixTest = lstSuffixes[intSuffixCounter];
                    if (strWordAtSelection.Length > strSuffixTest.Length )
                    {
                        if (string.Compare(strWordAtSelection.Substring(strWordAtSelection.Length - strSuffixTest.Length), strSuffixTest) == 0)
                        {
                            strSuffix_Old = strSuffixTest;
                            break;
                        }
                    }
                }
                //if (strSuffix_Old.Length > 0)
                {
                    string strWord_New = strWordAtSelection.Substring(0, strWordAtSelection.Length - strSuffix_Old.Length) + strSuffix_New;
                    Text_ReplaceWord(strWord_New);
                }
            }
        }


        void mnuAlignment_Left_Click(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Left;
        }
        void mnuAlignment_Center_Click(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Center;
        }
        void mnuAlignment_Right_Click(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Right;
        }

        void mnuFind_Click(object sender, EventArgs e)
        {
            ck_RichTextBox cMyRef = this;
            formFindReplace.ShowDialog(formFindReplace.enuMode.Find, ref cMyRef);
        }

        void mnuReplace_Click(object sender, EventArgs e)
        {
            ck_RichTextBox cMyRef = this;
            formFindReplace.ShowDialog(formFindReplace.enuMode.FindReplace, ref cMyRef);
        }

        void mnuNormalScript_Click(object sender, EventArgs e)
        {
            Font fntSelected = rtx.SelectionFont;
            float fltSize = fntSelected.Size / (rtx.SelectionCharOffset == 0 ? 1 : fltSubScriptSizeFactor);
            fntSelected = new Font(fntSelected.Name, fltSize, fntSelected.Style);
            rtx.SelectionCharOffset = 0;
            rtx.SelectionFont = fntSelected;
        }

        public void mnuSuperScript_Click(object sender, EventArgs e)
        {
            if (rtx.SelectionStart >= 0)
            {
                Font fntPrevious = rtx.SelectionFont;
                int intOffSet = 0;
                float fltSize = 1f;

                Font fntSelected = rtx.SelectionFont;

                if (rtx.SelectionCharOffset > 0)
                {  // was superScript => make regular
                    intOffSet = 0;
                    fltSize = fntSelected.Size / fltSubScriptSizeFactor;

                }
                else if (rtx.SelectionCharOffset < 0)
                { // was subscript => make Superscript
                    intOffSet = (int)(fntPrevious.Height * .4);
                    fltSize = fntSelected.Size;
                }
                else
                { // rtx.SelectionCharOffset  == 0
                    // was regular => make Superscript
                    intOffSet = (int)(fntPrevious.Height * .4);
                    fltSize = fntSelected.Size * fltSubScriptSizeFactor;
                }

                fntSelected = new Font(fntSelected.Name, fltSize, fntSelected.Style);
                rtx.SelectionCharOffset = intOffSet;
                rtx.SelectionFont = fntSelected;
            }
            else
                rtx.SelectionCharOffset = 10;
            ToolBar.SetToSelectedText();
        }

        public void mnuFontIncrease_Click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (rtx.SelectionStart >= 0 && fntSelection != null)
            {
                rtx.SelectionFont = FontSize_Change(fntSelection, 1);
            }
        }
        public void mnuFontDecrease_Click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (rtx.SelectionStart >= 0 && fntSelection != null)
            {
                rtx.SelectionFont = FontSize_Change(fntSelection, -1);
            }
        }

        Font FontSize_Change(Font fntStart, int intDelta)
        {
            Font fntRetVal = new Font(fntStart.FontFamily.Name, fntStart.Size + intDelta, fntStart.Style);
            return fntRetVal;
        }

        public void mnuFile_Load_Click(object sender, EventArgs e)
        {
            if (File_Load != null)
            {
                File_Load(sender, e);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "rich text files | *.rtf";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PathAndFilename = ofd.FileName;
                rtx.LoadFile(PathAndFilename);
            }
        }
        public void mnuFile_Save_Click(object sender, EventArgs e)
        {
            if (File_Save != null)
            {
                File_Save(sender, e);
                return;
            }

            if (PathAndFilename.Length == 0)
                mnuFile_SaveAs_Click(sender, e);
            rtx.SaveFile(PathAndFilename);

        }

        public void mnuFile_SaveAs_Click(object sender, EventArgs e)
        {
            if (File_SaveAs != null)
            {
                File_SaveAs(sender, e);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "rich text files | *.rtf";
            sfd.DefaultExt = "rtf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                PathAndFilename = sfd.FileName;

                rtx.SaveFile(sfd.FileName);
            }
        }


        public void mnuFile_New_Click(object sender, EventArgs e)
        {
            if (File_New != null)
            {
                File_New(sender, e);
                return;
            }
             
            rtx.Text = "";
            rtx.SelectionRightIndent = 50;
            rtx.RightMargin = 1100;
            PathAndFilename = System.IO.Directory.GetCurrentDirectory() + "\\default FileName.rtf";
        }

        public void mnuSubScript_Click(object sender, EventArgs e)
        {
            if (rtx.SelectionStart >= 0)
            {
                Font fntPrevious = rtx.SelectionFont;
                int intOffSet = 0;
                float fltSize = 1f;

                Font fntSelected = rtx.SelectionFont;

                if (rtx.SelectionCharOffset > 0)
                {  // was superScript => make subscript
                    intOffSet = -(int)(fntPrevious.Height * .4);
                    fltSize = fntSelected.Size;
                }
                else if (rtx.SelectionCharOffset < 0)
                { // was subscript => make regular
                    intOffSet = 0;
                    fltSize = fntSelected.Size / fltSubScriptSizeFactor;
                }
                else
                { 
                    // was regular => make subscript
                    intOffSet = -(int)(fntPrevious.Height * .4);
                    fltSize = fntSelected.Size * fltSubScriptSizeFactor;
                }

                fntSelected = new Font(fntSelected.Name, fltSize, fntSelected.Style);
                rtx.SelectionCharOffset = intOffSet;
                rtx.SelectionFont = fntSelected;
            }
            else
                rtx.SelectionCharOffset = 10;
            ToolBar.SetToSelectedText();
        }

        void mnuForeColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                rtx.SelectionColor = cd.Color;
        }

        void mnuBackColor_Click(object sender, EventArgs e)
        {
            if (rtx.SelectedText.Length >= 0)
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog() == DialogResult.OK)
                    rtx.SelectionBackColor = cd.Color;
            }
        }

        void mnuEdit_SelectAll_Click(object sender, EventArgs e)
        {
            rtx.SelectAll();
        }

        void mnuEdit_Copy_Click(object sender, EventArgs e)
        {
            if (rtx.SelectedText.Length > 0)
            {
                string strTextFormatting = rtx.SelectedRtf;
                Clipboard.SetText(strTextFormatting, TextDataFormat.Rtf);                
            }
        }

        void mnuEdit_Cut_Click(object sender, EventArgs e)
        {
            if (rtx.SelectedText.Length == 0) return;
            {
                string strTextFormatting = rtx.SelectedRtf;
                Clipboard.SetText(strTextFormatting, TextDataFormat.Rtf);
                rtx.SelectedRtf = "";
            }
        }

        void mnuEdit_Paste_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsData(DataFormats.Rtf))
                    rtx.SelectedRtf = Clipboard.GetText(TextDataFormat.Rtf);
                else
                    rtx.SelectedText = Clipboard.GetText();

            }
            catch (Exception)
            {
                
            }

        }
        #region Highlighter


        #region Grammar_Cutter
        public class classGrammarCutter
        {
            public RichTextBox rtx
            {
                get 
                {
                    return ckRTX != null          
                                  ? ckRTX.rtx
                                  : null; 
                }
            }

            ck_RichTextBox _ckRTX = null;
            public ck_RichTextBox ckRTX { get { return _ckRTX; } }


            const string _strGrammarCutter_SubclauseSplitter_Default = ".?!:;";
            public static string strGrammarCutter_SubclauseSplitter_Default { get { return _strGrammarCutter_SubclauseSplitter_Default; } }


            static public string strGrammarCutter_SubclauseSplitter
            {
                get { return strGrammarCutter_SubclauseSplitter_UI + "\r\n"; }
            }
            
            static string _strGrammarCutter_SubclauseSplitter_UI = _strGrammarCutter_SubclauseSplitter_Default;
            static public string strGrammarCutter_SubclauseSplitter_UI
            {
                get { return _strGrammarCutter_SubclauseSplitter_UI ; }
                set 
                {
                    _strGrammarCutter_SubclauseSplitter_UI = value;
                }
            }

            public classClause cClause = null;

            public classGrammarCutter(ref ck_RichTextBox ckRTX) 
            {
                this._ckRTX = ckRTX;
                classGrammarCutter cMyRef = this;
                cClause = new classClause(ref cMyRef);
            }


            public class classClause
            {

                public classClause(ref classGrammarCutter cGrammarCutter)
                {
                    _cGrammarCutter = cGrammarCutter;
                }
                public classClause(int intIndex, ref classGrammarCutter cGrammarCutter)
                {
                    _cGrammarCutter = cGrammarCutter;
                    this.Index = intIndex;
                }


                #region Properties

                classGrammarCutter _cGrammarCutter = null;
                public classGrammarCutter cGrammarCutter
                {
                    get { return _cGrammarCutter; }
                }

                public ck_RichTextBox ckRTX
                {
                    get
                    {
                        return cGrammarCutter != null
                                               ? cGrammarCutter.ckRTX
                                               : null;
                    }
                }


                public RichTextBox rtx
                {
                    get
                    {
                        return cGrammarCutter != null && cGrammarCutter.ckRTX != null
                                               ? cGrammarCutter.rtx
                                               : null;
                    }
                }


                int _intStart = -1;
                public int Start
                {
                    get { return _intStart; }
                    set { _intStart = value; }
                }

                int _intEnd = -1;
                public int End
                {
                    get { return _intEnd; }
                    set { _intEnd = value; }
                }

                public int Rank
                {
                    get 
                    {
                        if (cHighLighterColor == null)
                            cHighLighterColor = HighlightItem();

                            return ck_RichTextBox.lstHighlighterItems.IndexOf(cHighLighterColor);
                    }
                    set 
                    {
                        if (value >= 0 && value < ck_RichTextBox.lstHighlighterItems.Count)
                        {
                            cHighLighterColor = ck_RichTextBox.lstHighlighterItems[value];

                            int intSel_Start = rtx.SelectionStart;
                            int intSel_Length = rtx.SelectionLength;

                            int intLength = End - Start;
                            if (Start >= 0 && Start + intLength < rtx.Text.Length)
                            {
                                rtx.Select(Start, intLength);
                                rtx.SelectionColor = cHighLighterColor.clrFore;
                                rtx.SelectionBackColor = cHighLighterColor.clrBack;

                                rtx.Select(intSel_Start, intSel_Length);
                            }
                            if (Start == 0)
                            {
                                if (formWords.ckRTX == formWords.instance.rtxCK)
                                {
                                    // user is working in the Main Editor
                                    formWords.cProject.cEdit_Main.HeadingColor_Set(formWords.ckRTX.rtx.Rtf);
                                }
                                else
                                {
                                    // user is in alt editor
                                    formWords.cProject.cEdit_Alt.HeadingColor_Set(formWords.ckRTX.rtx.Rtf);
                                }
                            }

                        }
                    }
                }

                int _intIndex = -1;
                public int Index
                {
                    get { return _intIndex; }
                    set
                    {
                        _intIndex = value;
                        _cHighLighterColor = null;
                        Start = Start_Find(Index);
                        End = End_Find(Index);
                    }
                }

                classHighlighterColorItem _cHighLighterColor = null;
                classHighlighterColorItem cHighLighterColor
                {
                    get 
                    {
                        if (_cHighLighterColor == null)
                            _cHighLighterColor = HighlightItem();
                                
                        return _cHighLighterColor;
                    }
                    set { _cHighLighterColor = value; }
                }

                #endregion

                #region Methods

                //public void AutoHighlight_Previous()
                //{
                //    System.Diagnostics.Debug.Print("cClause.AutoHighLight_Previous()");
                    
                //    if (Start >0)
                //    {
                //        ScrollDown();

                //        int intStart = Start;
                //        int intEnd = End;
                //        Index = Start - 1;
                //        while (intStart == Start 
                //            || intEnd == End 
                //            || (intStart == intEnd))
                //        {
                //            Index = Index - 1;
                //        }

                //        ScrollUp();
                //    }
                //}

                //public void AutoHighlight_Next()
                //{
                //    System.Diagnostics.Debug.Print("cClause.AutoHighLight_Next()");
                //    if (End < cGrammarCutter.rtx.Text.Length)
                //    {
                //        ScrollDown();

                //        int intStart = Start;
                //        int intEnd = End;
                //        Index = End + 1;
                //        while (intStart == Start
                //            || intEnd == End
                //            || (intStart == intEnd))
                //        {
                //            Index = Index + 1;
                //        }

                //        ScrollUp();
                //    }
                //}

                public void ScrollUp()
                {
                    int intRank_New = Rank + 1;
                    do
                    {
                        if (intRank_New >= ck_RichTextBox.lstHighlighterItems.Count)
                            intRank_New = 0;

                        if (ck_RichTextBox.lstHighlighterItems[intRank_New].valid)
                        {
                            Rank = intRank_New;
                            return;
                        }

                        intRank_New++;
                    } while (true);
                }

                public void ScrollDown()
                {

                    int intRank_New = Rank - 1;
                    if (intRank_New < 0) intRank_New = ck_RichTextBox.lstHighlighterItems.Count - 1;

                    do
                    {
                        if (intRank_New <0) return;

                        if (ck_RichTextBox.lstHighlighterItems[intRank_New].valid)
                        {
                            Rank = intRank_New;
                            return;
                        }

                        intRank_New--;
                    } while (true);
                }

                public void draw()
                {
                    if (cHighLighterColor != null)
                    {
                        Point ptStart = (rtx != null && Start >= 0 && Start < rtx.Text.Length)
                                        ? rtx.GetPositionFromCharIndex(Start)
                                        : new Point();

                        Point ptEnd = (rtx != null && End >= 0 && End < rtx.Text.Length)
                                          ? rtx.GetPositionFromCharIndex(End)
                                          : new Point(rtx.Width, rtx.Height);
                        Point ptTemp = new Point(ptStart.X, ptStart.Y);
                        Point ptLast = ptTemp;
                        int intIndex = Start;

                        Pen p = new Pen(cHighLighterColor.clrFore, 2);

                        using (Graphics g = rtx.CreateGraphics())
                        {
                            if (ptTemp.Y < ptEnd.Y)
                            {
                                ptTemp.X = rtx.Width;
                                intIndex = rtx.GetCharIndexFromPosition(ptTemp);
                                ptTemp = rtx.GetPositionFromCharIndex(intIndex);
                                g.DrawLine(p, ptLast, ptTemp);

                                do
                                {
                                    ptLast = ptTemp;
                                    ptTemp.X = rtx.Width;
                                    intIndex = rtx.GetCharIndexFromPosition(ptTemp);
                                    intIndex++;
                                    ptTemp = rtx.GetPositionFromCharIndex(intIndex);
                                    g.DrawLine(p, ptLast, ptTemp);
                                } while (ptTemp.Y < ptEnd.Y);
                            }

                            g.DrawLine(p, ptTemp, ptEnd);
                        }
                    }
                }

                public classHighlighterColorItem HighlightItem()
                {
                    if (Start >= 0 && Start < cGrammarCutter.rtx.Text.Length-1)
                    {
                        int intSelectionStart = cGrammarCutter.rtx.SelectionStart;
                        int intSelectionLength = cGrammarCutter.rtx.SelectionLength;

                        cGrammarCutter.rtx.Select(Start, 1);
                        classHighlighterColorItem cRetVal = ck_RichTextBox.HighLighterItem_Get(cGrammarCutter.rtx.SelectionBackColor, cGrammarCutter.rtx.SelectionColor);
                        cGrammarCutter.rtx.Select(intSelectionStart, intSelectionLength);
                        
                        return cRetVal;
                    }
                    return null;
                }

                #endregion


                int Start_Find(int intIndex)
                {
                    char[] chrNextIndex = classGrammarCutter.strGrammarCutter_SubclauseSplitter.ToArray<char>();
                    int intTemp = -1, intRetVal = 0;
                    while (intTemp < intIndex && intTemp >=-1)
                    {
                        intRetVal = intTemp;
                        intTemp = rtx.Text.IndexOfAny(chrNextIndex, intTemp + 1);
                    }
                    intRetVal++;
                    if (intRetVal < 0)
                        intRetVal = rtx.Text.Length;
                    return intRetVal;
                }

                int End_Find(int intIndex)
                {
                   char[] chrNextIndex = classGrammarCutter.strGrammarCutter_SubclauseSplitter.ToArray<char>();
                   int intRetVal = rtx.Text.IndexOfAny(chrNextIndex, intIndex);
                    intRetVal++;
                    if (intRetVal >= rtx.Text.Length)
                        intRetVal = rtx.Text.Length - 1;
                   return intRetVal;
                }
            }
        }


        #endregion 

        bool Color_Compare(Color clr1, Color clr2) { return clr1.A == clr2.A && clr1.R == clr2.R && clr1.G == clr2.G && clr1.B == clr2.B; }

        public static List<classHighlighterColorItem> lstHighlighterItems = new List<classHighlighterColorItem>();
        static void lstHighlighterItems_Init()
        {
            if (lstHighlighterItems .Count == 0)
            {
                classHighlighterColorItem[] cTemp = {
                                                            new classHighlighterColorItem(Color.White, Color.Black, "Zero"),
                                                            new classHighlighterColorItem(Color.LightYellow, Color.Black, "One"),
                                                            new classHighlighterColorItem(Color.Beige, Color.Black, "Two"),
                                                            new classHighlighterColorItem(Color.Yellow, Color.Black, "Three"),
                                                            new classHighlighterColorItem(Color.Orange, Color.Black, "Four"),
                                                            new classHighlighterColorItem(Color.Pink, Color.Black, "Five"),
                                                            new classHighlighterColorItem(Color.DeepPink, Color.Black, "Six"),
                                                            new classHighlighterColorItem(Color.Red, Color.Black, "Seven"),
                                                            new classHighlighterColorItem(Color.DarkRed, Color.Black, "Eight")
                                                    };
                lstHighlighterItems = cTemp.ToList<classHighlighterColorItem>();
            }
        }
        public static bool ColorComparison(Color clr1, Color clr2)
        {
            return clr1.R == clr2.R
                && clr1.G == clr2.G
                && clr1.B == clr2.B
                && clr1.A == clr2.A;
        }


        public static classHighlighterColorItem HighLighterItem_Get(Color clrBackground, Color clrForeground)
        {
            for (int intItemCounter = 0; intItemCounter < lstHighlighterItems.Count; intItemCounter++)
            {
                classHighlighterColorItem cHLI = lstHighlighterItems[intItemCounter];
                if (ColorComparison(cHLI.clrFore,clrForeground) 
                    &&  
                    ColorComparison(cHLI.clrBack, clrBackground))
                {
                    return cHLI;
                }
            }

            if (Rank_Maximum >=0 && Rank_Maximum < ck_RichTextBox.lstHighlighterItems.Count )
                return ck_RichTextBox.lstHighlighterItems[Rank_Maximum];

            return null; 
        }
        
        static int _intRank_Minimum = 0;
        static public int Rank_Minimum
        {
            get { return _intRank_Minimum; }
            set { _intRank_Minimum = value; }
        }

        static int _intRank_Maximum = 3;
        static public int Rank_Maximum
        {
            get { return _intRank_Maximum; }
            set { _intRank_Maximum = value; }
        }


        static private void GrbHighlighterColor_UI_VisibleChanged(object sender, EventArgs e)
        {
     
        }

        public void groupboxHighlighterColor_UI_Show()
        {
            if (!rtx.Controls.Contains(grbHighlighterColor_UI))
                rtx.Controls.Add(grbHighlighterColor_UI);

            grbHighlighterColor_UI.Visible = !grbHighlighterColor_UI.Visible;
            grbHighlighterColor_UI.Location = new Point((rtx.Width - grbHighlighterColor_UI.Width) / 2,
                                                        (rtx.Height - grbHighlighterColor_UI.Height) / 2);

            grbHighlighterColor_UI.BringToFront();
        }


        private void LblSelectionBackColor_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                groupboxHighlighterColor_UI_Show();
            }
        }

        void mnuHighlight_AutoHideToolBarLabel_Click(object sender, EventArgs e)
        {
            HighlighterLabel_Hide_Automatically = !HighlighterLabel_Hide_Automatically;
        }

        void mnuHighlight_SetColorSequence_Click(object sender, EventArgs e)
        {
            groupboxHighlighterColor_UI_Show();
            //rtx.Controls.Add(grbHighlighterColor_UI);
            //grbHighlighterColor_UI.Show();
        }

        int Highlighter_ColorSelected(ref RichTextBox rtxColor)
        {
            Color clrSelectedBack = rtxColor.SelectionBackColor;
            if (clrSelectedBack.R 
                + clrSelectedBack.G 
                + clrSelectedBack.B 
                + clrSelectedBack.A == 0)
            {

                cGrammarCutter.cClause.Start = rtxColor.SelectionStart;
                cGrammarCutter.cClause.End = rtxColor.SelectionStart + rtxColor.SelectionLength;
                classHighlighterColorItem cHLC = cGrammarCutter.cClause.HighlightItem();
                clrSelectedBack = cHLC != null
                                        ? cHLC.clrBack
                                        : lstHighlighterItems[0].clrBack;
            }

            for (int intColorCounter = 0; intColorCounter < lstHighlighterItems.Count; intColorCounter++)
            {
                classHighlighterColorItem cClrItem = lstHighlighterItems[intColorCounter];
                if (Color_Compare(clrSelectedBack, cClrItem.clrBack))
                    if (cClrItem.valid)
                        return intColorCounter;
            }
            return -1;
        }

        classHighlighterColorItem Highlighter_ReturnHighest(int intIndexStart)
        {
            if (intIndexStart >= lstHighlighterItems.Count)
                intIndexStart = lstHighlighterItems.Count - 1;
            for (int intCount = intIndexStart; intCount > intIndexStart - lstHighlighterItems.Count; intCount--)
            {
                int inTestIndex = ((lstHighlighterItems.Count +intCount) % lstHighlighterItems.Count);
                classHighlighterColorItem clrTest = lstHighlighterItems[inTestIndex];
                if (clrTest.valid)
                    return clrTest;
            }
            return null;
        }

        classHighlighterColorItem Highlighter_ReturnLowest(int intIndexStart)
        {
            if (intIndexStart < 0)
                intIndexStart = 0;
            for (int intCount = intIndexStart; intCount < intIndexStart+lstHighlighterItems.Count; intCount++)
            {
                int inTestIndex = (intCount % lstHighlighterItems.Count);
                classHighlighterColorItem clrTest = lstHighlighterItems[inTestIndex];
                if (clrTest.valid)
                    return clrTest;
            }
            return null;
        }

        classHighlighterColorItem Highlighter_Color_ScrollDown(ref RichTextBox rtxColor)
        {
            int intIndex = Highlighter_ColorSelected(ref rtxColor);

            if (intIndex <= 0)
            {
                // set to Max
                return Highlighter_ReturnHighest(lstHighlighterItems.Count);
            }
            else
            {
                int intIndex_ScrollDown = intIndex - 1;
                return Highlighter_ReturnHighest(intIndex_ScrollDown);
            }
        }

        classHighlighterColorItem Highlighter_Color_ScrollUp(ref RichTextBox rtxColor)
        {
            int intIndex = Highlighter_ColorSelected(ref rtxColor);

            if (intIndex < 0)
            {
                // set to Min
                return Highlighter_ReturnLowest(0);
            }
            else
            {
                int intIndex_ScrollUp = (intIndex + 1) % lstHighlighterItems.Count;
                return Highlighter_ReturnLowest(intIndex_ScrollUp);
            }
        }



        public void Highlighter_Insert()
        {
            string strInsert = "[ ] ";
            int intSelectionStart = rtx.SelectionStart;
            Font fnt = new Font(rtx.SelectionFont.FontFamily, rtx.SelectionFont.Size, FontStyle.Bold);

            rtx.SelectedText = strInsert;
            rtx.SelectionStart = intSelectionStart;
            rtx.SelectionLength = strInsert.Length;
            classHighlighterColorItem cClrMax = Highlighter_ReturnHighest(lstHighlighterItems.Count - 1);
            rtx.SelectionBackColor = cClrMax.clrBack;
            rtx.SelectionColor = cClrMax.clrFore;

            rtx.SelectionStart = intSelectionStart + strInsert.Length - 1;
            rtx.SelectionLength = 1;
            rtx.SelectionBackColor = Color.White;

            rtx.SelectionStart = intSelectionStart + 1;
            rtx.SelectionLength = 1;
            lblColor_Draw();
        }

        public void mnuHighlighter_ScrollUp_Click(object sender, EventArgs e)
        {
            if (rtx.SelectionFont == null)
                rtx.SelectionFont = rtx.Font;

            if (rtx.SelectionLength == 0)
            {
                cGrammarCutter.cClause.ScrollUp();
                //Highlighter_Insert();
                return;
            }

            classHighlighterColorItem cScrollUp = Highlighter_Color_ScrollUp(ref rtx);

            if (cScrollUp == null)
            {
                rtx.SelectionBackColor = Color.White;
                lblColor_Draw();
                return;
            }
            else
            {
                SelectionBackColor = cScrollUp.clrBack;
                SelectionForeColor = cScrollUp.clrFore;
                lblColor_Draw();
                return;
            }
        }
        public void mnuHighlighter_ScrollDown_Click(object sender, EventArgs e)
        {

            if (rtx.SelectionFont == null)
                rtx.SelectionFont = rtx.Font;
            if (rtx.SelectionLength == 0)
            {
                cGrammarCutter.cClause.ScrollDown();
                //Highlighter_Insert();
                return;
            }

            classHighlighterColorItem cScrollDown = Highlighter_Color_ScrollDown(ref rtx);

            if (cScrollDown == null)
            {
                rtx.SelectionBackColor = Color.White;
                rtx.SelectionColor = Color.Black;
                lblColor_Draw();
                return;
            }
            else
            {
                SelectionBackColor = cScrollDown.clrBack;
                SelectionForeColor  = cScrollDown.clrFore;
                lblColor_Draw();
                return;
            }
        }


        public void ScrollUp()
        { 
            ScrollUp(new MouseEventArgs(MouseButtons.None, 0, 0,0,0)); 
        
        }
        public void ScrollUp(MouseEventArgs e)
        {
            if (rtx.SelectionLength > 0)
                mnuHighlighter_ScrollUp_Click((object)null, new MouseEventArgs(MouseButtons.None, 0, 0,0, 1));
            else
            {   
                string strTemp = classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI;
                if (MouseButtons == MouseButtons.Left || e.Clicks == 1)
                    classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = "";

                cGrammarCutter.cClause.Index = rtx.SelectionStart;

                cGrammarCutter.cClause.ScrollUp();
                classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = strTemp;
            }
        }

        public void ScrollDown() { ScrollDown(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)); }
        public void ScrollDown(MouseEventArgs e)
        {
            if (rtx.SelectionLength > 0)
                mnuHighlighter_ScrollDown_Click((object)null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, -1));
            else
            {
                string strTemp = classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI;
                if (MouseButtons == MouseButtons.Left || e.Clicks ==1)
                    classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = "";

                cGrammarCutter.cClause.Index = rtx.SelectionStart;

                cGrammarCutter.cClause.ScrollDown();
                classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = strTemp;
            }
        }
              
        private void LblSelectionBackColor_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                ScrollDown(e);
            }
            else
            {
                ScrollUp(e);
            }
        }

        #endregion

        public void mnuInsertImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image|*.bmp;*.png;*.jpg;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
                InsertImage(ofd.FileName);
        }

        public void InsertImage(string strFilename)
        {
            if (System.IO.File.Exists(strFilename))
            {
                try
                {
                    Bitmap bmp = new Bitmap(strFilename);
                    InsertImage(bmp);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void InsertImage(Bitmap bmp)
        {
            // Copy the bitmap to the clipboard.
            Clipboard.SetDataObject(bmp);
            // Get the format for the object type.
            DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
            // After verifying that the data can be pasted, paste
            if (rtx.CanPaste(myFormat))
            {
                rtx.Paste(myFormat);
            }
            else
            {
                MessageBox.Show("The data format that you attempted site" +
                  " is not supportedby this control.");
            }
        }


        void mnuReader_Click(object sender, EventArgs e)
        {
            Reader_AutoHighlight_Toggle = !Reader_AutoHighlight_Toggle;
        }


        void mnuTextBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = rtx.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
                rtx.BackColor = cd.Color;
        }

        void mnuCopyCutter_Click(object sender, EventArgs e)
        {
            cCopyCutter.Enabled 
                = ToolBar.btnCopyCutter_Toggle.Toggled  
                = !cCopyCutter.Enabled;
            ToolBar.placeObjects();
        }

        public void mnuLock_Click(object sender, EventArgs e)
        {
            Locked
                = ToolBar.btnLock.Toggled
                = !Locked;
            ToolBar.placeObjects();

            

            groupboxNotePilot.instance.btnLock_SetImage();
        }

        public void mnuOptions_NumberOfWords_Click(object sender, EventArgs e)
        {
            List<string> lstWords = getFirstWords(rtx.SelectionLength > 0
                                                                      ? rtx.SelectedText
                                                                      : rtx.Text);
            IEnumerable<string> query = lstWords.Distinct();
            List<string> lstWords_Distinct = (List<string>)query.ToList<string>();

            MessageBox.Show("Words : " + lstWords.Count.ToString()
                                + "\r\nDistinct Words : " + lstWords_Distinct.Count.ToString()
                                + "\r\n\t% " + (100.0*  (double)lstWords_Distinct.Count / (double)lstWords.Count).ToString("00.0"),
                            "Word Count");
        }
        private static string _strNonAlphaCharacters = "";
        private static string strNonAlphaCharacters
        {
            get
            {
                if (_strNonAlphaCharacters.Length == 0)
                {
                    for (int intCounter = 32; intCounter < 256; intCounter++)
                    {
                        char chrNew = (char)intCounter;
                        if (!char.IsLetter(chrNew))
                            _strNonAlphaCharacters += chrNew.ToString();
                    }
                }
                return _strNonAlphaCharacters;
            }
        }
        public static List<string> getFirstWords(string strText)
        {
            List<string> lstRetVal = getFirstWords(strText, -1, conGetFirstWordKeepCharIgnoreCode);
            return lstRetVal;
        }
        private static List<string> getFirstWords(string strText, int intNumberWords, string strKeepChar)

        {
            if (strText == null || strText.Length == 0) return null;

            //strText = " " + strText + " ";
            int intNextIndex = 0;
            int intIndex = 0;
            List<string> lstRetVal = new List<string>();

            string str_Local_NonAlphaCharacters = "";
            if (string.Compare(conGetFirstWordKeepCharIgnoreCode, strKeepChar) == 0)
                str_Local_NonAlphaCharacters = strNonAlphaCharacters;
            else
            {
                char chrRemoveFromRejectedCharList = strKeepChar[0];
                int intRemoveIndex = strNonAlphaCharacters.IndexOf(chrRemoveFromRejectedCharList);
                if (intRemoveIndex >= 0 && intRemoveIndex < strNonAlphaCharacters.Length)
                    str_Local_NonAlphaCharacters = strNonAlphaCharacters.Remove(intRemoveIndex, 1);
            }
            char[] chr_Local_str_Local_NonAlphaCharacters = str_Local_NonAlphaCharacters.ToArray<char>();

            strText = clean_nonAlpha_Ends(strText) + ".";

            intNextIndex = strText.IndexOfAny(chr_Local_str_Local_NonAlphaCharacters, intIndex + 1);
            while (intNextIndex > intIndex
                    && (lstRetVal.Count < intNumberWords || intNumberWords < 0)
                    )
            {
                string strWord = strText.Substring(intIndex, intNextIndex - intIndex).Trim();
                lstRetVal.Add(strWord);
                while (intNextIndex < strText.Length && str_Local_NonAlphaCharacters.Contains(strText[intNextIndex]))
                    intNextIndex++;
                intIndex = intNextIndex;

                intNextIndex = strText.IndexOfAny(chr_Local_str_Local_NonAlphaCharacters, intIndex);
            }
            if (lstRetVal.Count == 0
                && strText.Length > 0)
                lstRetVal.Add(strText);

            return lstRetVal;
        }
        public static string clean_nonAlpha_Ends(string strSource)
        {
            return cleanBack_nonAlpha(cleanFront_nonAlpha(strSource));
        }

        public static string cleanFront_nonAlpha(string strSource)
        {
            while (strSource.Length > 0
                    &&
                 !strAlphabet.Contains(strSource[0]))
            {
                strSource = strSource.Substring(1);
            }
            return strSource;
        }

        public static string cleanBack_nonAlpha(string strSource)
        {
            while (strSource.Length > 0
                    &&
                 !strAlphabet.Contains(strSource[strSource.Length - 1]))
            {
                strSource = strSource.Substring(0, strSource.Length - 1);
            }
            return strSource;
        }

        void mnuFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            if (rtx.SelectionFont != null)
            {
                Font fnt = rtx.SelectionFont;
                fd.Font = fnt;
            }
            if (fd.ShowDialog() == DialogResult.OK)
                rtx.SelectionFont = fd.Font;
        }

        void btnAlignment_Left(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Left;
        }

        void btnAlignment_Center(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Center;
        }

        void btnAlignment_Right(object sender, EventArgs e)
        {
            rtx.SelectionAlignment = HorizontalAlignment.Right;
        }


        Font FontSelected
        {
            get
            {
                Font fntSelection = rtx.SelectionFont;
                if (fntSelection == null)
                {
                    int intSelStart = rtx.SelectionStart;
                    int intSelLength = rtx.SelectionLength;
                    if (rtx.SelectionLength > 1)
                        rtx.Select(intSelStart, 1);
                    fntSelection = rtx.SelectionFont;
                    rtx.Select(intSelStart, intSelLength);
                }
                
                if (fntSelection == null && rtx.Text.Length >0)
                {
                    int intSelStart = rtx.SelectionStart;
                    int intSelLength = rtx.SelectionLength;
                        rtx.Select(0,1);
                    fntSelection = rtx.SelectionFont;
                    rtx.Select(intSelStart, intSelLength);
                }

                return fntSelection;
            }
        }

        public void mnuFont_Bold_click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (fntSelection == null) return;


                FontStyle fntStyle_OLD = fntSelection.Style;
                bool bolStrikeOut = (fntStyle_OLD & FontStyle.Strikeout) != 0;
                bool bolBold = (fntStyle_OLD & FontStyle.Bold) != 0;
                bool bolItalic = (fntStyle_OLD & FontStyle.Italic) != 0;
                bool bolUnderline = (fntStyle_OLD & FontStyle.Underline) != 0;

                bolBold = !bolBold;

                FontStyle fntStyle_New = (bolStrikeOut ? FontStyle.Strikeout : FontStyle.Regular)
                                        | (bolBold ? FontStyle.Bold : FontStyle.Regular)
                                        | (bolItalic ? FontStyle.Italic : FontStyle.Regular)
                                        | (bolUnderline ? FontStyle.Underline : FontStyle.Regular);
                rtx.SelectionFont = new Font(fntSelection.Name, fntSelection.Size, fntStyle_New);
            
        }

        public void mnuFont_Italic_click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (fntSelection == null) return;
            FontStyle fntStyle_OLD = fntSelection.Style;
            bool bolStrikeOut = (fntStyle_OLD & FontStyle.Strikeout) != 0;
            bool bolBold = (fntStyle_OLD & FontStyle.Bold) != 0;
            bool bolItalic = (fntStyle_OLD & FontStyle.Italic) != 0;
            bool bolUnderline = (fntStyle_OLD & FontStyle.Underline) != 0;

            bolItalic = !bolItalic;

            FontStyle fntStyle_New = (bolStrikeOut ? FontStyle.Strikeout : FontStyle.Regular)
                                    | (bolBold ? FontStyle.Bold : FontStyle.Regular)
                                    | (bolItalic ? FontStyle.Italic : FontStyle.Regular)
                                    | (bolUnderline ? FontStyle.Underline : FontStyle.Regular);
            rtx.SelectionFont = new Font(fntSelection.Name, fntSelection.Size, fntStyle_New);
        }

        public void mnuFont_Underline_click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (fntSelection == null) return;
            FontStyle fntStyle_OLD = fntSelection.Style;
            bool bolStrikeOut = (fntStyle_OLD & FontStyle.Strikeout) != 0;
            bool bolBold = (fntStyle_OLD & FontStyle.Bold) != 0;
            bool bolItalic = (fntStyle_OLD & FontStyle.Italic) != 0;
            bool bolUnderline = (fntStyle_OLD & FontStyle.Underline) != 0;

            bolUnderline = !bolUnderline;

            FontStyle fntStyle_New = (bolStrikeOut ? FontStyle.Strikeout : FontStyle.Regular)
                                    | (bolBold ? FontStyle.Bold : FontStyle.Regular)
                                    | (bolItalic ? FontStyle.Italic : FontStyle.Regular)
                                    | (bolUnderline ? FontStyle.Underline : FontStyle.Regular);
            rtx.SelectionFont = new Font(fntSelection.Name, fntSelection.Size, fntStyle_New);
        }

        public void mnuFont_StrikeOut_click(object sender, EventArgs e)
        {
            Font fntSelection = FontSelected;
            if (fntSelection == null) return;
            FontStyle fntStyle_OLD = fntSelection.Style;
            bool bolStrikeOut = (fntStyle_OLD & FontStyle.Strikeout) != 0;
            bool bolBold = (fntStyle_OLD & FontStyle.Bold) != 0;
            bool bolItalic = (fntStyle_OLD & FontStyle.Italic) != 0;
            bool bolUnderline = (fntStyle_OLD & FontStyle.Underline) != 0;

            bolStrikeOut = !bolStrikeOut;

            FontStyle fntStyle_New = (bolStrikeOut ? FontStyle.Strikeout : FontStyle.Regular)
                                    | (bolBold ? FontStyle.Bold : FontStyle.Regular)
                                    | (bolItalic ? FontStyle.Italic : FontStyle.Regular)
                                    | (bolUnderline ? FontStyle.Underline : FontStyle.Regular);
            rtx.SelectionFont = new Font(fntSelection.Name, fntSelection.Size, fntStyle_New);
        }

        void mnuFont_Regular_click(object sender, EventArgs e)
        {
            Font fntSelection = rtx.SelectionFont;
            if (fntSelection == null) return;
            rtx.SelectionFont = new Font(fntSelection.Name, fntSelection.Size, FontStyle.Regular);
        }

        public static string getSentenceAtSelection(ref ck_RichTextBox rtxbox) { return getSentenceAtSelection(ref rtxbox, rtxbox.SelectionStart); }
        public static string getSentenceAtSelection(ref ck_RichTextBox rtxbox, int intSelection)
        {
            string strRetVal = "";

            int intStartSentence = intSelection - 1;
            string strPunctuation = ".!?\r\n\t";
            while (intStartSentence >= 0 && strPunctuation.IndexOf(rtxbox.Text[intStartSentence]) < 0) intStartSentence--;

            int intEndSentence = intSelection;
            while (intEndSentence < rtxbox.Text.Length && strPunctuation.IndexOf(rtxbox.Text[intEndSentence]) < 0) intEndSentence++;

            if (intEndSentence > intStartSentence + 1)
            {
                strRetVal = rtxbox.Text.Substring(intStartSentence + 1, intEndSentence - intStartSentence - 1);
            }

            return strRetVal;
        }

        public static bool isAlpha(char chr) { return strAlphabet.Contains(chr); }
        public static string getWordAtSelection(ref ck_RichTextBox rtxbox) { return getWordAtSelection(ref rtxbox, rtxbox.SelectionStart); }
        public static string getWordAtSelection(ref ck_RichTextBox rtxbox, int intSelection)
        {
            string strRetVal = "";

            if (rtxbox.SelectionLength > 0)
                return rtxbox.SelectedText;

            int intStartWord = intSelection - 1;
            while (intStartWord >= 0 && isAlpha(rtxbox.Text[intStartWord])) intStartWord--;


            int intEndWord = intSelection;
            while (intEndWord < rtxbox.Text.Length && isAlpha(rtxbox.Text[intEndWord])) intEndWord++;

            if (intEndWord > intStartWord + 1)
            {
                strRetVal = rtxbox.Text.Substring(intStartWord + 1, intEndWord - intStartWord - 1);
            }

            return strRetVal;
        }

        #endregion

        public class panelToolBar : Panel
        {
            ck_RichTextBox rtx = null;

            public enum enuButtons
            {
                File_New, File_Load, File_Save, File_SaveAs,
                Bold, Italic, Underline, Strikeout, SuperScript, SubScript,
                Font_Increase, Font_Decrease,
                Align_Left, Align_Center, Align_Right,
                SpellCheck, Highlighter, FindWord,
                Picture_Insert, Undo, Redo,
                ToolTip_Toggle,
                CopyCutter_Toggle,
                Lock,
                _numButtons
            };

            public SPObjects.SPContainer SPContainer = new SPObjects.SPContainer("Tool Bar");
            public List<SPObjects.Button> lstButtons = new List<SPObjects.Button>();
            System.Windows.Forms.Timer tmrMouseLeave = new Timer();

            public string Language
            {
                get { return SPContainer.Language; }
                set { SPContainer.Language = value; }
            }

            public panelToolBar(ref ck_RichTextBox ckRTX)
            {
                this.rtx = ckRTX;
                Height = 22;

                tmrMouseLeave.Interval = 100;
                tmrMouseLeave.Tick += TmrMouseLeave_Tick;

                SPContainer.pic.MouseMove += Pic_MouseMove;
                SPContainer.pic.MouseLeave += Pic_MouseLeave;
                SPContainer.MouseMove += Pic_MouseMove;
                SPContainer.MouseLeave += Pic_MouseLeave;
                Controls.Add(SPContainer);
                SPContainer.Dock = DockStyle.Fill;

                SPContainer.Language = enuLanguages.English.ToString();

                bool bolBuilding = SPContainer.BuildingInProgress;

                SPContainer.Building_Start();
                {
                    for (int intButtonCounter = 0; intButtonCounter < (int)enuButtons._numButtons; intButtonCounter++)
                    {
                        enuButtons eButton = (enuButtons)intButtonCounter;
                        SPObjects.Button btnNew = new SPObjects.Button(ref SPContainer);
                        lstButtons.Add(btnNew);
                        btnNew.Size = szButtons;
                        btnNew.Tag = (object)eButton;
                        btnNew.cEle.Name = eButton.ToString();
                        btnNew.MouseClick = btn_Click;
                        
                        btnNew.cEle.Tip_Set(eButton.ToString(), enuLanguages.English.ToString());
                        switch (eButton)
                        {
                            case enuButtons.File_Load:
                                btnNew.cEle.Tip_Set("File Load", enuLanguages.English.ToString());
                                break;

                            case enuButtons.File_New:
                                btnNew.cEle.Tip_Set("File New", enuLanguages.English.ToString());
                                break;

                            case enuButtons.File_Save:
                                btnNew.cEle.Tip_Set("File Save", enuLanguages.English.ToString());
                                break;

                            case enuButtons.File_SaveAs:
                                btnNew.cEle.Tip_Set("Save As", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Bold:
                                btnNew.cEle.Tip_Set("Bold", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Italic:
                                btnNew.cEle.Tip_Set("Italic", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Underline:
                                btnNew.cEle.Tip_Set("Underline", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Strikeout:
                                btnNew.cEle.Tip_Set("Strikeout", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Font_Decrease:
                                btnNew.cEle.Tip_Set("Font Shrink", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Font_Increase:
                                btnNew.cEle.Tip_Set("Font Grow", enuLanguages.English.ToString());
                                break;

                            case enuButtons.SubScript:
                                btnNew.cEle.Tip_Set("Subscript", enuLanguages.English.ToString());
                                break;

                            case enuButtons.SuperScript:
                                btnNew.cEle.Tip_Set("Superscript", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Align_Left:
                                btnNew.cEle.Tip_Set("Align Left", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Align_Right:
                                btnNew.cEle.Tip_Set("Align Right", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Align_Center:
                                btnNew.cEle.Tip_Set("Align Center", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Highlighter:
                                btnNew.cEle.Tip_Set("Highlighter", enuLanguages.English.ToString());
                                break;

                            case enuButtons.FindWord:
                                btnNew.cEle.Tip_Set("Find Word", enuLanguages.English.ToString());
                                btnNew.CanBeToggled = true;
                                break;

                            case enuButtons.Picture_Insert:
                                btnNew.cEle.Tip_Set("Picture Insert", enuLanguages.English.ToString());
                                break;
                                

                            case enuButtons.Undo:
                                btnNew.cEle.Tip_Set("Undo", enuLanguages.English.ToString());
                                break;
                                
                            case enuButtons.Redo:
                                btnNew.cEle.Tip_Set("Redo", enuLanguages.English.ToString());
                                break;

                            case enuButtons.ToolTip_Toggle:
                                btnNew.cEle.Tip_Set("ToolTip Toggle", enuLanguages.English.ToString());
                                break;

                            case enuButtons.SpellCheck:
                                btnNew.cEle.Tip_Set("Spell Check", enuLanguages.English.ToString());
                                break;

                            case enuButtons.CopyCutter_Toggle:
                                btnNew.cEle.Tip_Set("Copy Cutter toggle", enuLanguages.English.ToString());
                                break;

                            case enuButtons.Lock:
                                btnNew.cEle.Tip_Set("Lock toggle", enuLanguages.English.ToString());
                                break;
                        }
                    }
                    Buttons_Draw();
                }
                SPContainer.Building_Complete();
                SizeChanged += PanelToolBar_SizeChanged;
                BackColorChanged += PanelToolBar_BackColorChanged;
            }

            private void Pic_MouseLeave(object sender, EventArgs e)
            {
                tmrMouseLeave_Reset();
            }

            private void Pic_MouseMove(object sender, MouseEventArgs e)
            {
                tmrMouseLeave_Reset();
            }

            void tmrMouseLeave_Reset()
            {
                tmrMouseLeave.Enabled = false;
                tmrMouseLeave.Enabled = true;
            }

            private void TmrMouseLeave_Tick(object sender, EventArgs e)
            {
                tmrMouseLeave.Enabled = false;
                Point ptMouse = MousePosition;
                Control ctrl = (Control)this;
                Point ptPanelRelToScreen = Ck_Objects.classControlLocation.Location(ref ctrl);
                if (ptMouse.X > ptPanelRelToScreen.X && ptMouse.X < ptPanelRelToScreen.X + Width)
                {
                    if (ptMouse.Y > ptPanelRelToScreen.Y && ptMouse.Y < ptPanelRelToScreen.Y + Height)
                    {
                        return;
                    }
                }
                SPContainer.cEleUnderMouse = null;
                Buttons_Draw();
            }

            private void PanelToolBar_BackColorChanged(object sender, EventArgs e)
            {

            }


            #region Properties

            public bool ToolTip_Enabled
            {
                get { return SPContainer.ToolTip_Enabled; }
                set
                {
                    SPContainer.ToolTip_Enabled = value;
                    btnToolTip_Toggle.Toggled = value;
                }
            }


            Size _szButtons = new Size(18, 18);
            Size szButtons
            {
                get { return _szButtons; }
                set { _szButtons = value; }
            }

            SPObjects.Button btnFile_New
            {
                get { return lstButtons[(int)enuButtons.File_New]; }
                set { lstButtons[(int)enuButtons.File_New] = value; }
            }
            SPObjects.Button btnFile_Load
            {
                get { return lstButtons[(int)enuButtons.File_Load]; }
                set { lstButtons[(int)enuButtons.File_Load] = value; }
            }
            SPObjects.Button btnFile_Save
            {
                get { return lstButtons[(int)enuButtons.File_Save]; }
                set { lstButtons[(int)enuButtons.File_Save] = value; }
            }
            SPObjects.Button btnFile_SaveAs
            {
                get { return lstButtons[(int)enuButtons.File_SaveAs]; }
                set { lstButtons[(int)enuButtons.File_SaveAs] = value; }
            }

            SPObjects.Button btnBold
            {
                get { return lstButtons[(int)enuButtons.Bold]; }
                set { lstButtons[(int)enuButtons.Bold] = value; }
            }
            SPObjects.Button btnItalic
            {
                get { return lstButtons[(int)enuButtons.Italic]; }
                set { lstButtons[(int)enuButtons.Italic] = value; }
            }
            SPObjects.Button btnUnderline
            {
                get { return lstButtons[(int)enuButtons.Underline]; }
                set { lstButtons[(int)enuButtons.Underline] = value; }
            }
            SPObjects.Button btnStrikeout
            {
                get { return lstButtons[(int)enuButtons.Strikeout]; }
                set { lstButtons[(int)enuButtons.Strikeout] = value; }
            }
            SPObjects.Button btnSuperScript
            {
                get { return lstButtons[(int)enuButtons.SuperScript]; }
                set { lstButtons[(int)enuButtons.SuperScript] = value; }
            }
            SPObjects.Button btnSubScript
            {
                get { return lstButtons[(int)enuButtons.SubScript]; }
                set { lstButtons[(int)enuButtons.SubScript] = value; }
            }

            SPObjects.Button btnAlign_Left
            {
                get { return lstButtons[(int)enuButtons.Align_Left]; }
                set { lstButtons[(int)enuButtons.Align_Left] = value; }
            }

            SPObjects.Button btnAlign_Center
            {
                get { return lstButtons[(int)enuButtons.Align_Center]; }
                set { lstButtons[(int)enuButtons.Align_Center] = value; }
            }

            SPObjects.Button btnAlign_Right
            {
                get { return lstButtons[(int)enuButtons.Align_Right]; }
                set { lstButtons[(int)enuButtons.Align_Right] = value; }
            }

            SPObjects.Button btnFontIncrease
            {
                get { return lstButtons[(int)enuButtons.Font_Increase]; }
                set { lstButtons[(int)enuButtons.Font_Increase] = value; }
            }
            SPObjects.Button btnFontDecrease
            {
                get { return lstButtons[(int)enuButtons.Font_Decrease]; }
                set { lstButtons[(int)enuButtons.Font_Decrease] = value; }
            }

            SPObjects.Button btnSpellCheck
            {
                get { return lstButtons[(int)enuButtons.SpellCheck]; }
                set { lstButtons[(int)enuButtons.SpellCheck] = value; }
            }

            SPObjects.Button btnHighlighter
            {
                get { return lstButtons[(int)enuButtons.Highlighter]; }
                set { lstButtons[(int)enuButtons.Highlighter] = value; }
            }
            
            public SPObjects.Button btnFindWord
            {
                get { return lstButtons[(int)enuButtons.FindWord]; }
                set { lstButtons[(int)enuButtons.FindWord] = value; }
            }

            SPObjects.Button btnPicture_Insert
            {
                get { return lstButtons[(int)enuButtons.Picture_Insert]; }
                set { lstButtons[(int)enuButtons.Picture_Insert] = value; }
            }
            SPObjects.Button btnRedo
            {
                get { return lstButtons[(int)enuButtons.Redo]; }
                set { lstButtons[(int)enuButtons.Redo] = value; }
            }
            
            SPObjects.Button btnUndo
            {
                get { return lstButtons[(int)enuButtons.Undo]; }
                set { lstButtons[(int)enuButtons.Undo] = value; }
            }

            public SPObjects.Button btnToolTip_Toggle
            {
                get { return lstButtons[(int)enuButtons.ToolTip_Toggle]; }
                set { lstButtons[(int)enuButtons.ToolTip_Toggle] = value; }
            }
            
            //public SPObjects.Button btnFindWord
            //{
            //    get { return lstButtons[(int)enuButtons.FindWord]; }
            //    set { lstButtons[(int)enuButtons.FindWord] = value; }
            //}            
            
            public SPObjects.Button btnCopyCutter_Toggle
            {
                get { return lstButtons[(int)enuButtons.CopyCutter_Toggle]; }
                set { lstButtons[(int)enuButtons.CopyCutter_Toggle] = value; }
            }
            
            public SPObjects.Button btnLock
            {
                get { return lstButtons[(int)enuButtons.Lock]; }
                set { lstButtons[(int)enuButtons.Lock] = value; }
            }

            System.Drawing.Color _clrButtons_BackColor = System.Drawing.Color.White;
            public System.Drawing.Color clrButtons_BackColor
            {
                get { return _clrButtons_BackColor; }
                set
                {
                    _clrButtons_BackColor = value;
                    Buttons_Draw();
                }
            }

            System.Drawing.Color _clrButtons_ForeColor = System.Drawing.Color.Black;
            public System.Drawing.Color clrButtons_ForeColor
            {
                get { return _clrButtons_ForeColor; }
                set
                {
                    _clrButtons_ForeColor = value;
                    Buttons_Draw();
                }
            }

            bool bolCanUndo = false;
            bool CanUndo
            {
                get {return bolCanUndo; }
                set
                {
                    if (bolCanUndo != value)
                    {
                        bolCanUndo = value;

                        if (bolCanUndo)
                        {
                            btnUndo.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo);
                            btnUndo.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo_Highlight);
                        }
                        else
                        {
                            btnUndo.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo_Disabled);
                            btnUndo.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo_Disabled);
                        }

                        btnUndo.cEle.NeedsToBeRedrawn = true;
                        tmrMouseLeave_Reset();
                    }
                }
            }

            bool bolCanRedo = false;
            bool CanRedo
            {
                get { return bolCanRedo;                }
                set
                {
                    if (bolCanRedo != value)
                    {
                        bolCanRedo = value;
                        
                        if (bolCanRedo)
                        {
                            btnRedo.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo);
                            btnRedo.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo_Highlight);
                        }
                        else
                        {
                            btnRedo.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo_Disabled);
                            btnRedo.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo_Disabled );
                        }
                        btnRedo.cEle.NeedsToBeRedrawn = true;
                        tmrMouseLeave_Reset();
                    }
                }
            }

            #endregion

            #region Methods
            public void SetToSelectedText()
            {
                Font fnt = rtx.SelectionFont;
                if (fnt == null) return;
                bool bolBuilding = SPContainer.BuildingInProgress;
                SPContainer.Building_Start();
                {

                    btnBold.Toggled = (fnt.Style & FontStyle.Bold) != 0;
                    btnItalic.Toggled = (fnt.Style & FontStyle.Italic) != 0;
                    btnUnderline.Toggled = (fnt.Style & FontStyle.Underline) != 0;
                    btnStrikeout.Toggled = (fnt.Style & FontStyle.Strikeout) != 0;
                    btnSuperScript.Toggled = (rtx.SelectionCharOffset > 0);
                    btnSubScript.Toggled = (rtx.SelectionCharOffset < 0);

                    CanUndo = rtx.rtx.CanUndo;
                    CanRedo = rtx.rtx.CanRedo;

                }
                if (!bolBuilding)
                    SPContainer.Building_Complete();
            }

            void Buttons_Draw()
            {
                bool bolBuilding = SPContainer.BuildingInProgress;
                Font fnt = new Font("Segoe UI", 72);
                SPContainer.Building_Start();
                {
                    SPContainer.BackColor_Dull = BackColor;
                    for (int intButtonCounter = 0; intButtonCounter < (int)enuButtons._numButtons; intButtonCounter++)
                    {
                        enuButtons eButton = (enuButtons)intButtonCounter;
                        lstButtons[(int)eButton].Size = szButtons;
                        switch (eButton)
                        {
                            case enuButtons.File_New:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_New);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_New_Highlight);
                                }
                                break;

                            case enuButtons.File_Load:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_Load);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_Load_Highlight);
                                }
                                break;

                            case enuButtons.File_Save:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_Save);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_Save_Highlight);
                                }
                                break;

                            case enuButtons.File_SaveAs:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_SaveAs);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_File_SaveAs_Highlight);
                                }
                                break;


                            case enuButtons.Bold:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Bold);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Bold_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.Italic:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Italic);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Italic_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.Underline:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Underline);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Underline_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.Strikeout:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Strikeout);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Font_Strikeout_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.SuperScript:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SuperScript);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SuperScript_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.SubScript:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SubScript);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SubScript_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;


                            case enuButtons.Align_Left:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Left);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Left_Highlight);
                                }
                                break;

                            case enuButtons.Align_Center:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Center);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Center_Highlight);
                                }
                                break;

                            case enuButtons.Align_Right:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Right);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Align_Right_Highlight);
                                }
                                break;


                            case enuButtons.Font_Decrease:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FontDecrease);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FontDecrease_Highlight);
                                }
                                break;

                            case enuButtons.Font_Increase:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FontIncrease);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FontIncrease_Highlight);
                                }
                                break;

                            case enuButtons.SpellCheck:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SpellChecker);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_SpellChecker_Highlight);
                                }
                                break;

                            case enuButtons.Highlighter:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_HighLighter_Dull);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_HighLighter_Highlight);
                                }
                                break;
                                

                            case enuButtons.FindWord:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FindWord);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_FindWord__Highlight);
                                }
                                break;

                            case enuButtons.Picture_Insert:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Picture_Insert);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Picture_Insert_Highlight);
                                }
                                break;
                                
                            case enuButtons.Undo:
                                {
                                    CanUndo = rtx.rtx.CanUndo;
                                    //SPObjects.Button btn = lstButtons[(int)eButton];
                                    //btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo);
                                    //btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Undo_Highlight);
                                }
                                break;
                                
                            case enuButtons.Redo:
                                {
                                    CanRedo = rtx.rtx.CanRedo;
                                    //SPObjects.Button btn = lstButtons[(int)eButton];
                                    //btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo);
                                    //btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Redo_Highlight);
                                }
                                break;

                            case enuButtons.ToolTip_Toggle:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_ToolTip_Toggle);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_ToolTip_Toggle_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.CopyCutter_Toggle:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_CopyCutter);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_CopyCutter_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;

                            case enuButtons.Lock:
                                {
                                    SPObjects.Button btn = lstButtons[(int)eButton];
                                    btn.bmpDull = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Lock);
                                    btn.bmpHighlight = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Lock_Highlight);
                                    btn.CanBeToggled = true;
                                }
                                break;
                        }
                    }
                }

                if (!bolBuilding)
                    SPContainer.Building_Complete();
            }

      

            public void placeObjects()
            {
                Buttons_Draw();
                bool bolBuilding = SPContainer.BuildingInProgress;
                List<classBtnWidths> lstBtns = new List<classBtnWidths>();

                lstBtns.Add(new classBtnWidths());
                lstBtns[0].lstSPBtns.Add(btnToolTip_Toggle);
                lstBtns[0].lstSPBtns.Add(btnHighlighter);
                lstBtns[0].lstSPBtns.Add(btnFindWord);
                lstBtns[0].lstSPBtns.Add(btnCopyCutter_Toggle);

                lstBtns.Add(new classBtnWidths());
                lstBtns[1].lstSPBtns.Add(btnFile_Load);
                lstBtns[1].lstSPBtns.Add(btnFile_New);
                lstBtns[1].lstSPBtns.Add(btnFile_Save);
                lstBtns[1].lstSPBtns.Add(btnFile_SaveAs);

                lstBtns.Add(new classBtnWidths());
                lstBtns[2].lstSPBtns.Add(btnAlign_Left);
                lstBtns[2].lstSPBtns.Add(btnAlign_Center);
                lstBtns[2].lstSPBtns.Add(btnAlign_Right);

                lstBtns.Add(new classBtnWidths());
                lstBtns[3].lstSPBtns.Add(btnFontDecrease);
                lstBtns[3].lstSPBtns.Add(btnFontIncrease);
                lstBtns[3].lstSPBtns.Add(btnBold);
                lstBtns[3].lstSPBtns.Add(btnItalic);
                lstBtns[3].lstSPBtns.Add(btnUnderline);
                lstBtns[3].lstSPBtns.Add(btnStrikeout);
                lstBtns[3].lstSPBtns.Add(btnSubScript);
                lstBtns[3].lstSPBtns.Add(btnSuperScript);
                lstBtns[3].lstSPBtns.Add(btnPicture_Insert);
                lstBtns[3].lstSPBtns.Add(btnUndo);
                lstBtns[3].lstSPBtns.Add(btnRedo);

                lstBtns.Add(new classBtnWidths());
                lstBtns[4].lstSPBtns.Add(btnLock);


                for (int intLstCounter = 0; intLstCounter < lstBtns.Count; intLstCounter++)
                {
                    Rectangle rec = new Rectangle();
                    for (int intBtnCounter = 0; intBtnCounter < lstBtns[intLstCounter].lstSPBtns.Count; intBtnCounter++)
                    {
                        SPObjects.Button btn = lstBtns[intLstCounter].lstSPBtns[intBtnCounter];
                        if (btn.Visible)
                        {
                            rec.Width += btn.MyImage.Width;
                            if (rec.Height < btn.MyImage.Height)
                                rec.Height = btn.MyImage.Height;
                            lstBtns[intLstCounter].Visible = true;
                        }
                    }
                    lstBtns[intLstCounter].rec = rec;
                }

                SPContainer.Building_Start();
                {
                    Point pt = new Point(0, 0);
                    for (int intLstCounter = 0; intLstCounter < lstBtns.Count; intLstCounter++)
                    {
                        classBtnWidths cBtnLst = lstBtns[intLstCounter];

                        if (pt.X + cBtnLst.rec.Width > SPContainer.pic.Width)
                        {
                            if (intLstCounter > 0)
                            {
                                pt.Y = lstBtns[intLstCounter - 1].rec.Bottom;
                                pt.X = 0;
                            }
                        }

                        cBtnLst.rec.Location = pt;
                        Point ptBtn = pt;
                        for (int intBtnCounter = 0; intBtnCounter < cBtnLst.lstSPBtns.Count; intBtnCounter++)
                        {
                            SPObjects.Button btn = cBtnLst.lstSPBtns[intBtnCounter];
                            if (btn.Visible)
                            {
                                btn.Location = ptBtn;
                                ptBtn.X += btn.Width;
                            }
                        }

                        pt.X += cBtnLst.rec.Width;
                    }

                    if (lstBtns[lstBtns.Count - 1].Visible && SPContainer.pic.Height != lstBtns[lstBtns.Count - 1].rec.Bottom)
                    {
                        SPContainer.pic.Height
                            = Height
                            = lstBtns[lstBtns.Count - 1].rec.Bottom;
                    }

                    SPContainer.recVisible
                        = SPContainer.recSPArea
                        = new Rectangle(0, 0, SPContainer.pic.Width, SPContainer.pic.Height);
                    SPContainer.RecVisible_Changed = true;
                }

                if (!bolBuilding)
                    SPContainer.Building_Complete();
            }

            #endregion

            #region Events

            public class classBtnWidths
            {
                public List<SPObjects.Button> lstSPBtns = new List<SPObjects.Button>();
                public Rectangle rec = new Rectangle();
                public bool Visible = false;
            }

            private void PanelToolBar_SizeChanged(object sender, EventArgs e)
            {
                Buttons_Draw();
                placeObjects();
            }

            void btn_Click(object sender, EventArgs e)
            {
                if (!rtx.Enabled) return;

                SPObjects.Button btnSender = (SPObjects.Button)sender;
                enuButtons eButton = (enuButtons)btnSender.Tag;
                if (btnSender.CanBeToggled) btnSender.Toggle();

                switch (eButton)
                {
                    case enuButtons.File_New:
                        rtx.mnuFile_New_Click((object)this, new EventArgs());
                        break;

                    case enuButtons.File_Load:
                        rtx.mnuFile_Load_Click((object)this, new EventArgs());
                        break;

                    case enuButtons.File_Save:
                        rtx.mnuFile_Save_Click((object)this, new EventArgs());
                        break;

                    case enuButtons.File_SaveAs:
                        rtx.mnuFile_SaveAs_Click((object)this, new EventArgs());
                        break;

                    case enuButtons.Bold:
                        rtx.mnuFont_Bold_click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Italic:
                        rtx.mnuFont_Italic_click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Underline:
                        rtx.mnuFont_Underline_click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Strikeout:
                        rtx.mnuFont_StrikeOut_click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.SuperScript:
                        rtx.mnuSuperScript_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.SubScript:
                        rtx.mnuSubScript_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Font_Decrease:
                        rtx.mnuFontDecrease_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Font_Increase:
                        rtx.mnuFontIncrease_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Align_Left:
                        rtx.btnAlignment_Left((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Align_Center:
                        rtx.btnAlignment_Center((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Align_Right:
                        rtx.btnAlignment_Right((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.SpellCheck:
                        //rtx.mnuSpellCheck_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Highlighter:
                        rtx.mnuHighlighter_ScrollDown_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.FindWord:
                        btnFindWord_Click();
                        break;

                    case enuButtons.Picture_Insert:
                        rtx.mnuInsertImage_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Undo:
                        if (rtx.rtx.CanUndo)
                        { 
                            rtx.rtx.Undo();
                            SetToSelectedText();
                                }
                        break;

                    case enuButtons.Redo:
                        if (rtx.rtx.CanRedo)
                        {
                            rtx.rtx.Redo();
                            SetToSelectedText();
                        }
                        break;

                    case enuButtons.CopyCutter_Toggle:
                        rtx.mnuCopyCutter_Click((object)btnSender, new EventArgs());
                        break;

                    case enuButtons.Lock:
                        rtx.mnuLock_Click((object)btnSender, new EventArgs());
                        break;

                }
                #endregion
            }

           public void btnFindWord_Click()
            {
                rtx.HighlightSelectedWord = !rtx.HighlightSelectedWord;
                //rtx.Gr_PtReset();
                btnFindWord.Toggled = rtx.HighlightSelectedWord;
            }
        }


        public class panelRuler : Panel
        {
            enum enuControls { LeftIndent, RightIndent, FirstLineIndent, HangingIndent, _numControls };
            const int conHeight = 23;

            SPObjects.SPContainer SPContainer = new SPObjects.SPContainer("panelRuler");
            SPObjects.Label[] lblControls = new SPObjects.Label[(int)enuControls._numControls];
            System.Windows.Forms.Timer tmrGrab = new System.Windows.Forms.Timer();

            public panelRuler(ref ck_RichTextBox ckRTX)
            {
                _ckRTX = ckRTX;
                Controls.Add(SPContainer);
                SPContainer.Dock = DockStyle.Fill;
                SPContainer.BackgroundImage = bmpRuler;

                SPContainer.Language = enuLanguages.English.ToString();

                for (int intControlCounter = 0; intControlCounter < (int)enuControls._numControls; intControlCounter++)
                {
                    enuControls eControl = (enuControls)intControlCounter;
                    SPObjects.Label lblNew = new SPObjects.Label(ref SPContainer);
                    lblNew.Text = "";
                    lblNew.MouseDown = lblMouse_Down;
                    lblNew.MouseUp = lblMouse_Up;
                    lblNew.cEle.Name = eControl.ToString();
                    lblNew.BlendIntoSPContainer = true;
                    lblNew.Tag = (object)eControl;

                    switch (eControl)
                    {
                        case enuControls.FirstLineIndent:
                            lblNew.BackgroundImage = bmpFirstLineIndent;
                            lblNew.cEle.Tip_Set("First Line Indent", enuLanguages.English.ToString());
                            break;

                        case enuControls.HangingIndent:
                            lblNew.BackgroundImage = bmpHangingIndent;
                            lblNew.cEle.Tip_Set("Hanging Indent", enuLanguages.English.ToString());
                            break;

                        case enuControls.LeftIndent:
                            lblNew.BackgroundImage = bmpLeftIndent;
                            lblNew.cEle.Tip_Set("Left-Indent", enuLanguages.English.ToString());
                            break;

                        case enuControls.RightIndent:
                            lblNew.BackgroundImage = bmpRightIndent;
                            lblNew.cEle.Tip_Set("Right Indent", enuLanguages.English.ToString());
                            break;
                    }
                    lblNew.Size = lblNew.BackgroundImage.Size;
                    lblNew.cEle.Tip_Set(eControl.ToString(), enuLanguages.English.ToString());

                    lblControls[(int)intControlCounter] = lblNew;
                }
                SPContainer.ToolTip_Set("Ruler");

                SPContainer.Building_Complete();

                tmrGrab.Interval = 100;
                tmrGrab.Tick += TmrGrab_Tick;
            }


            #region Events

            private void TmrGrab_Tick(object sender, EventArgs e)
            {
                if (MouseButtons != MouseButtons.Left)
                    tmrGrab.Enabled = false;
                SPObjects.Label lblGrab = (SPObjects.Label)tmrGrab.Tag;

                Point ptMouseRel = MouseRelTo(this);

                enuControls eControlGrabbed = (enuControls)lblGrab.Tag;
                switch (eControlGrabbed)
                {
                    case enuControls.FirstLineIndent:
                        {
                            // moves just the FirstLineIndent

                            int intFirstLineIndent_Start = RTX.SelectionIndent;
                            int intFirstLineIndent_End = ptMouseRel.X;
                            if (intFirstLineIndent_End != intFirstLineIndent_Start)
                            {
                                // measure difference b/w FirstLine and Hanging before/after moving FirstLine
                                int intLeftIndent_Start = intFirstLineIndent_Start + RTX.SelectionHangingIndent;
                                int intLeftIndent_End = intLeftIndent_Start - intFirstLineIndent_End;
                                RTX.SelectionIndent = intFirstLineIndent_End;
                                RTX.SelectionHangingIndent = intLeftIndent_End;
                                placeObjects();
                            }
                        }
                        break;

                    case enuControls.LeftIndent:
                        {
                            // moves both FirstLineIndent && Hanging Indent
                            //measure difference between FirstLine and Hanging indents
                            RTX.SelectionIndent = ptMouseRel.X - RTX.SelectionHangingIndent;
                            placeObjects();
                        }
                        break;

                    case enuControls.HangingIndent:
                        {
                            RTX.SelectionHangingIndent = ptMouseRel.X - RTX.SelectionIndent;
                            placeObjects();
                        }
                        break;

                    case enuControls.RightIndent:
                        {
                            int intMouseXRelToWidth = RTX.RightMargin - ptMouseRel.X;
                            if (intMouseXRelToWidth < 10)
                                intMouseXRelToWidth = 10;
                            RTX.SelectionRightIndent = intMouseXRelToWidth;
                            placeObjects();
                        }
                        break;
                }


            }

            void lblMouse_Down(object sender, MouseEventArgs e)
            {
                tmrGrab.Tag = (object)SPContainer.cEleUnderMouse.obj;
                tmrGrab.Enabled = true;
            }

            void lblMouse_Up(object sender, MouseEventArgs e)
            {
                tmrGrab.Enabled = false;
            }

            #endregion

            #region Properties

            ck_RichTextBox _ckRTX = null;
            ck_RichTextBox RTX
            {
                get { return _ckRTX; }
            }

            #region bitmaps

            static Bitmap _bmpRuler = null;
            static Bitmap bmpRuler
            {
                get
                {
                    if (_bmpRuler == null)
                    {
                        Font fntNumeral = new Font("Segoe UI", 10);
                        _bmpRuler = new Bitmap(Screen.PrimaryScreen.Bounds.Width, conHeight);
                        int intVMid = (int)(conHeight / 2);
                        using (Graphics g = Graphics.FromImage(_bmpRuler))
                        {
                            g.FillRectangle(Brushes.LightGray, new RectangleF(0, 0, _bmpRuler.Width, _bmpRuler.Height));
                            int intRulerHeight = conHeight;
                            g.FillRectangle(Brushes.White, new RectangleF(0, intVMid - intRulerHeight / 2, _bmpRuler.Width, intRulerHeight));
                            g.DrawRectangle(Pens.Black, new Rectangle(0, 0, _bmpRuler.Width - 1, _bmpRuler.Height - 1));

                            int intStep = 10;
                            for (int intHashCounter = 0; intHashCounter < _bmpRuler.Width; intHashCounter += intStep)
                            {
                                int intHeight = 0;
                                bool bolDrawNumeral = false;
                                if (intHashCounter % (intStep * 10) == 0)
                                    intHeight = (int)(conHeight * .7);
                                else if (intHashCounter % (intStep * 5) == 0)
                                {
                                    intHeight = (int)(conHeight * .4);
                                    bolDrawNumeral = true;
                                }
                                else
                                    intHeight = (int)(conHeight * .2);
                                g.DrawLine(Pens.Black, new Point(intHashCounter, intVMid - intHeight / 2), new Point(intHashCounter, intVMid + intHeight / 2));
                                if (bolDrawNumeral)
                                {
                                    string strNumeral = (intHashCounter / (intStep * 10)).ToString();
                                    Size szNumeral = TextRenderer.MeasureText(strNumeral, fntNumeral);
                                    g.DrawString(strNumeral, fntNumeral, Brushes.DarkGray, new Point(intHashCounter - szNumeral.Width / 2, intVMid - szNumeral.Height / 2));
                                }
                            }
                        }
                    }
                    return _bmpRuler;
                }
            }



            static Bitmap _bmpLeftIndent = null;
            static Bitmap bmpLeftIndent
            {
                get
                {
                    if (_bmpLeftIndent == null)
                    {
                        _bmpLeftIndent = new Bitmap(9, 8);
                        using (Graphics g = Graphics.FromImage(_bmpLeftIndent))
                        {
                            //g.FillRectangle(Brushes.Red, new RectangleF(0, 0, _bmpLeftIndent.Width, _bmpLeftIndent.Height));
                            //int intPointDepth = 4;
                            Point[] pts = {
                                            new Point(0,0),
                                            new Point(_bmpLeftIndent.Width-1, 0),
                                            new Point(_bmpLeftIndent.Width -1, _bmpLeftIndent.Height-1),
                                            new Point(0, _bmpLeftIndent.Height-1),
                                            new Point(0,0)
                                        };
                            g.FillPolygon(Brushes.LightGray, pts);
                            g.DrawPolygon(Pens.DarkGray, pts);
                        }
                    }
                    return _bmpLeftIndent;
                }
            }

            static Bitmap _bmpRightIndent = null;
            static Bitmap bmpRightIndent
            {
                get
                {
                    if (_bmpRightIndent == null)
                    {
                        _bmpRightIndent = new Bitmap(9, 8);
                        Color clrTransparent = Color.Red;
                        using (Graphics g = Graphics.FromImage(_bmpRightIndent))
                        {
                            g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, _bmpRightIndent.Width, _bmpRightIndent.Height));
                            int intPointDepth = 4;
                            Point[] pts = {
                                            new Point(_bmpRightIndent.Width /2, 0),
                                            new Point(_bmpRightIndent.Width-1, intPointDepth),
                                            new Point(_bmpRightIndent.Width -1, _bmpRightIndent.Height-1),
                                            new Point(0, _bmpRightIndent.Height-1),
                                            new Point(0, intPointDepth),
                                            new Point(_bmpRightIndent.Width /2, 0)
                                        };
                            g.FillPolygon(Brushes.LightGray, pts);
                            g.DrawPolygon(Pens.DarkGray, pts);
                        }
                        _bmpRightIndent.MakeTransparent(clrTransparent);
                    }

                    return _bmpRightIndent;
                }
            }

            static Bitmap _bmpFirstLineIndent = null;
            static Bitmap bmpFirstLineIndent
            {
                get
                {
                    if (_bmpFirstLineIndent == null)
                    {
                        _bmpFirstLineIndent = new Bitmap(9, 8);
                        Color clrTransparent = Color.Red;
                        using (Graphics g = Graphics.FromImage(_bmpFirstLineIndent))
                        {
                            g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, _bmpFirstLineIndent.Width, _bmpFirstLineIndent.Height));
                            int intPointDepth = 4;
                            Point[] pts = {
                                            new Point(0,0),
                                            new Point(_bmpFirstLineIndent.Width-1, 0),
                                            new Point(_bmpFirstLineIndent.Width -1, _bmpFirstLineIndent.Height - intPointDepth),
                                            new Point(_bmpFirstLineIndent.Width /2, _bmpFirstLineIndent.Height-1),
                                            new Point(0, _bmpFirstLineIndent.Height - intPointDepth),
                                            new Point(0,0)
                                        };
                            g.FillPolygon(Brushes.LightGray, pts);
                            g.DrawPolygon(Pens.DarkGray, pts);
                        }
                        _bmpFirstLineIndent.MakeTransparent(clrTransparent);
                    }
                    return _bmpFirstLineIndent;
                }
            }

            static Bitmap _bmpHangingIndent = null;
            static Bitmap bmpHangingIndent
            {
                get
                {
                    if (_bmpHangingIndent == null)
                    {
                        _bmpHangingIndent = new Bitmap(9, 8);
                        Color clrTransparent = Color.Red;
                        using (Graphics g = Graphics.FromImage(_bmpHangingIndent))
                        {
                            g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, _bmpHangingIndent.Width, _bmpHangingIndent.Height));
                            int intPointDepth = 4;
                            Point[] pts = {
                                            new Point(_bmpHangingIndent.Width /2,0),
                                            new Point(_bmpHangingIndent.Width -1, intPointDepth),
                                            new Point(_bmpHangingIndent.Width-1, _bmpHangingIndent.Height-1),
                                            new Point(0, _bmpHangingIndent.Height-1),
                                            new Point(0, intPointDepth),
                                            new Point(_bmpHangingIndent.Width /2,0)
                                        };
                            g.FillPolygon(Brushes.LightGray, pts);
                            g.DrawPolygon(Pens.DarkGray, pts);
                        }
                        _bmpHangingIndent.MakeTransparent(clrTransparent);
                    }

                    return _bmpHangingIndent;
                }
            }

            #endregion

            SPObjects.Label lblLeftIndent
            {
                get { return lblControls[(int)enuControls.LeftIndent]; }
            }

            SPObjects.Label lblRightIndent
            {
                get { return lblControls[(int)enuControls.RightIndent]; }
            }

            SPObjects.Label lblFirstLineIndent
            {
                get { return lblControls[(int)enuControls.FirstLineIndent]; }
            }

            SPObjects.Label lblHangingIndent
            {
                get { return lblControls[(int)enuControls.HangingIndent]; }
            }

            #endregion


            #region Methods
            public static Point MouseRelTo(Control ctrl)
            {
                Point ptRetVal = System.Windows.Forms.Control.MousePosition;

                while (ctrl != null)
                {
                    ptRetVal.X -= ctrl.Location.X;
                    ptRetVal.Y -= ctrl.Location.Y;
                    ctrl = ctrl.Parent;
                }

                return ptRetVal;
            }

            public void placeObjects()
            {
                bool bolBuilding = SPContainer.BuildingInProgress;
                SPContainer.Building_Start();
                {
                    if (Parent != null)
                        Width = Parent.Width;
                    Height = conHeight;

                    lblFirstLineIndent.Left = RTX.SelectionIndent - RTX.Margin.Left - lblFirstLineIndent.Width / 2;
                    if (lblFirstLineIndent.Left < 0)
                        lblFirstLineIndent.Left = 0;

                    lblFirstLineIndent.Top = 0;

                    int intLeftIndent = RTX.SelectionIndent + RTX.SelectionHangingIndent - RTX.Margin.Left - lblLeftIndent.Width / 2;
                    if (intLeftIndent < 0)
                        intLeftIndent = 0;
                    lblLeftIndent.Left
                        = lblHangingIndent.Left
                        = intLeftIndent;
                    lblRightIndent.Left = RTX.RightMargin - RTX.SelectionRightIndent - lblRightIndent.Width / 2;

                    lblRightIndent.Top = Height - lblRightIndent.Height;
                    lblLeftIndent.Top = Height - lblLeftIndent.Height;
                    lblHangingIndent.Top = lblLeftIndent.Top - lblHangingIndent.Height + 1;

                    SPContainer.BackgroundImage = bmpRuler;
                    SPContainer.recVisible
                        = SPContainer.recSPArea
                        = new Rectangle(0, 0, Width, Height);
                    SPContainer.RecVisible_Changed = true;
                }
                if (!bolBuilding)
                    SPContainer.Building_Complete();
            }

            public void SetToSelectedText()
            {
                placeObjects();
            }

            #endregion
        }

        public class formFindReplace : Form
        {
            public static formFindReplace instance = null;

            string strAlphabet
            {
                get { return ck_RichTextBox.strAlphabet; }
            }

            public enum enuMode { Find, FindReplace, _numMode };

            List<int> lstSearchResults = new List<int>();
            string strSearchWord = "";

            Label lblSearchText = new Label();
            TextBox txtSearchText = new TextBox();

            Label lblReplacementText = new Label();
            TextBox txtReplacementText = new TextBox();

            GroupBox grbSearchOptions = new GroupBox();
            CheckBox cbxMatchWholeWord = new CheckBox();
            CheckBox cbxMatchCase = new CheckBox();
            RadioButton rbtSearchUp = new RadioButton();
            RadioButton rbtSearchDown = new RadioButton();

            Button btnFindNext = new Button();
            Button btnReplace = new Button();
            Button btnReplaceAll = new Button();
            Button btnCancel = new Button();

            ck_RichTextBox _ckRTX = null;
            public ck_RichTextBox ckRTX
            {
                get { return _ckRTX; }
                set
                {
                    if (_ckRTX != value)
                    {
                        _ckRTX = value;
                        Search_Reset();
                    }
                }
            }

            public static void ShowDialog(enuMode eMode, ref ck_RichTextBox ck_RichText)
            {
                if (instance == null || instance.IsDisposed)
                    new formFindReplace(eMode, ref ck_RichText);
                instance.ckRTX = ck_RichText;
                instance.eMode = eMode;
                instance.Show();
            }


            formFindReplace(enuMode eMode, ref ck_RichTextBox ck_RichText)
            {
                ckRTX = ck_RichText;
                instance = this;

                this.eMode = eMode;
                MinimizeBox = false;
                MaximizeBox = false;

                Icon = Properties.Resources.Search_Text;
                ShowInTaskbar = false;

                TopMost = true;// !Debugging;
                BringToFront();

                Controls.Add(lblSearchText);
                lblSearchText.AutoSize = true;
                lblSearchText.Text = "Search Text:";

                Controls.Add(txtSearchText);
                txtSearchText.KeyDown += _KeyDown_QuitTest;
                txtSearchText.KeyUp += TxtSearchText_KeyUp;
                txtSearchText.GotFocus += TxtSearchText_GotFocus;

                Controls.Add(lblReplacementText);
                lblReplacementText.Text = "Replacement Text";
                lblReplacementText.AutoSize = true;

                Controls.Add(txtReplacementText);
                txtReplacementText.Enabled = false;
                txtReplacementText.KeyDown += _KeyDown_QuitTest;

                Controls.Add(grbSearchOptions);
                grbSearchOptions.Text = "Search Options";
                grbSearchOptions.AutoSize = true;

                grbSearchOptions.Controls.Add(cbxMatchWholeWord);
                cbxMatchWholeWord.Text = "Match Whole Word";
                cbxMatchWholeWord.AutoSize = true;
                cbxMatchWholeWord.Checked = false;
                cbxMatchWholeWord.CheckedChanged += CbxMatchWholeWord_CheckedChanged;

                grbSearchOptions.Controls.Add(cbxMatchCase);
                cbxMatchCase.Text = "Match Case";
                cbxMatchCase.AutoSize = true;
                cbxMatchCase.Checked = false;
                cbxMatchCase.CheckedChanged += CbxMatchCase_CheckedChanged;

                grbSearchOptions.Controls.Add(rbtSearchUp);
                rbtSearchUp.Text = "Search Up";
                rbtSearchUp.AutoSize = true;
                rbtSearchUp.Checked = false;

                grbSearchOptions.Controls.Add(rbtSearchDown);
                rbtSearchDown.Text = "Search Down";
                rbtSearchDown.AutoSize = true;
                rbtSearchDown.Checked = true;
                rbtSearchUp.CheckedChanged += RbtSearchUp_CheckedChanged;

                Controls.Add(btnFindNext);
                btnFindNext.Text = "Find Next";
                btnFindNext.Click += BtnFindNext_Click;

                Controls.Add(btnReplace);
                btnReplace.Text = "Replace";
                btnReplace.Enabled = false;
                btnReplace.Click += BtnReplace_Click;

                Controls.Add(btnReplaceAll);
                btnReplaceAll.Text = "Replace All";
                btnReplaceAll.Enabled = false;
                btnReplaceAll.Click += BtnReplaceAll_Click;

                Controls.Add(btnCancel);
                btnCancel.Text = "Cancel";
                btnCancel.Click += BtnCancel_Click;

                btnFindNext.Size
                    = btnReplace.Size
                    = btnReplaceAll.Size
                    = btnCancel.Size
                    = new Size(73, 21);

                Activated += FormFindReplace_Activated;
                KeyDown += _KeyDown_QuitTest;
            }

            #region Methods
            void placeObjects()
            {
                grbSearchOptions.Size = new Size(225, 78);

                cbxMatchWholeWord.Location = new Point(5, 25);
                cbxMatchCase.Location = new Point(cbxMatchWholeWord.Left, cbxMatchWholeWord.Bottom + 10);

                rbtSearchUp.Location = new Point(cbxMatchWholeWord.Right, cbxMatchWholeWord.Top);
                rbtSearchDown.Location = new Point(rbtSearchUp.Left, cbxMatchCase.Top);

                lblSearchText.Location = new Point(10, 5);
                txtSearchText.Location = new Point(lblSearchText.Left, lblSearchText.Bottom);

                txtSearchText.Width
                    = txtReplacementText.Width = grbSearchOptions.Width - 10;

                grbSearchOptions.Left = lblSearchText.Left;
                btnFindNext.Location = new Point(grbSearchOptions.Right + 7, txtSearchText.Top);

                switch (eMode)
                {
                    case enuMode.Find:
                        {
                            Text = "Search Text";
                            grbSearchOptions.Top = txtSearchText.Bottom;
                            btnCancel.Location = new Point(btnFindNext.Left, btnFindNext.Bottom + 20);

                            btnReplace.Visible
                                = btnReplaceAll.Visible
                                = lblReplacementText.Visible
                                = txtReplacementText.Visible
                                = false;

                        }
                        break;

                    case enuMode.FindReplace:
                        {
                            Text = "Search & Replace Text";
                            lblReplacementText.Location = new Point(lblSearchText.Left, txtSearchText.Bottom);
                            txtReplacementText.Location = new Point(lblReplacementText.Left, lblReplacementText.Bottom);
                            grbSearchOptions.Top = txtReplacementText.Bottom;

                            int intVGap = 5;
                            btnReplace.Location = new Point(btnFindNext.Left, btnFindNext.Bottom + intVGap);
                            btnReplaceAll.Location = new Point(btnFindNext.Left, btnReplace.Bottom + intVGap);
                            btnCancel.Location = new Point(btnFindNext.Left, btnReplaceAll.Bottom + intVGap);

                            btnReplace.Visible
                                = btnReplaceAll.Visible
                                = lblReplacementText.Visible
                                = txtReplacementText.Visible
                                = true;
                        }
                        break;
                }
                Size = new Size(348, grbSearchOptions.Bottom + 45);
            }

            public void Search_Reset()
            {
                strSearchWord = "";
                intSearchListIndex = -1;
                lstSearchResults.Clear();
            }

            void Search_SelectText()
            {
                if (intSearchListIndex < 0 || intSearchListIndex >= lstSearchResults.Count) return;
                rtx.Select(lstSearchResults[intSearchListIndex], txtSearchText.Text.Length);
                rtx.ScrollToCaret();
                rtx.rtx.Focus();
            }

            void Search_Indices_FindBest()
            {
                if (lstSearchResults.Count == 0)
                    return;

                int intListIndexBest = lstSearchResults.Count / 2;
                int intStep = intListIndexBest;
                int intDir = 1;
                int intCursorIndex = rtx.SelectionStart;
                do
                {
                    intStep /= 2;
                    if (intStep < 1)
                        intStep = 1;
                    if (intCursorIndex == lstSearchResults[intListIndexBest])
                    {
                        intSearchListIndex = intListIndexBest;
                        return;
                    }
                    else if (intCursorIndex > lstSearchResults[intListIndexBest])
                    {
                        if (lstSearchResults.Count > intListIndexBest + 1)
                        {
                            if (intCursorIndex < lstSearchResults[intListIndexBest + 1])
                            {
                                intSearchListIndex = intListIndexBest;
                                return;
                            }
                        }
                        else
                        {
                            intSearchListIndex = intListIndexBest;
                            return;
                        }
                        intDir = 1;
                    }
                    else
                    {
                        if (intListIndexBest > 0)
                        {
                            if (intCursorIndex > lstSearchResults[intListIndexBest - 1])
                            {
                                intSearchListIndex = intListIndexBest;
                                return;
                            }
                        }
                        else
                        {
                            intSearchListIndex = intListIndexBest;
                            return;
                        }
                        intDir = -1;
                    }
                    intListIndexBest += intStep * intDir;
                } while (true);
            }

            void Search_Indices_BuildList()
            {
                string strText = cbxMatchCase.Checked
                                            ? rtx.Text
                                            : rtx.Text.ToLower();
                if (strSearchWord.Length == 0 && txtSearchText.Text.Length > 0)
                {
                    strSearchWord = cbxMatchCase.Checked
                                            ? txtSearchText.Text
                                            : txtSearchText.Text.ToLower();
                    lstSearchResults.Clear();

                    int intIndex = -1;
                    do
                    {
                        intIndex = strText.IndexOf(strSearchWord, intIndex + 1);
                        if (intIndex >= 0)
                        {
                            if (cbxMatchWholeWord.Checked)
                            {
                                if ((intIndex > 0 && !strAlphabet.Contains(strText[intIndex - 1]))
                                                || intIndex == 0)
                                {
                                    if (((intIndex < strText.Length - 1 && !strAlphabet.Contains(strText[intIndex + strSearchWord.Length])
                                                || intIndex == strText.Length)))
                                        lstSearchResults.Add(intIndex);
                                }
                            }
                            else
                                lstSearchResults.Add(intIndex);
                        }
                    } while (intIndex >= 0);
                }
            }
            bool replace(string strSearch, string strReplace)
            {
                if (string.Compare(rtx.SelectedText.ToLower(), strSearch.ToLower()) == 0)
                {
                    rtx.SelectedText = strReplace;
                    Search_Reset();
                    return true;
                }
                return false;
            }
            #endregion

            #region Properties
            enuMode _eMode = enuMode.Find;
            public enuMode eMode
            {
                get { return _eMode; }
                set
                {
                    _eMode = value;
                    placeObjects();
                }
            }

            int _intSearchListIndex = -1;
            int intSearchListIndex
            {
                get { return _intSearchListIndex; }
                set
                {
                    if (value >= lstSearchResults.Count)
                        value = -1;
                    _intSearchListIndex = value;
                }
            }

            ck_RichTextBox rtx { get { return ckRTX; } }

            #endregion

            #region Events 
            bool bolActivated = false;
            private void FormFindReplace_Activated(object sender, EventArgs e)
            {
                if (bolActivated) return;

                placeObjects();
                bolActivated = true;
            }

            private void _KeyDown_QuitTest(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {
                            Dispose();
                        }
                        break;

                    case Keys.F:
                        {
                            if (e.Modifiers == Keys.Control)
                                eMode = enuMode.Find;
                        }
                        break;

                    case Keys.H:
                        {
                            if (e.Modifiers == Keys.Control)
                                eMode = enuMode.FindReplace;
                        }
                        break;
                }
            }

            private void CbxMatchCase_CheckedChanged(object sender, EventArgs e)
            {
                //Search_MatchCase = cbxMatchCase.Checked;
                Search_Reset();
            }

            private void CbxMatchWholeWord_CheckedChanged(object sender, EventArgs e)
            {
                //Search_MatchWord = cbxMatchWholeWord.Checked; 
                Search_Reset();
            }

            private void RbtSearchUp_CheckedChanged(object sender, EventArgs e)
            {
                //Search_Up = rbtSearchUp.Checked;
            }

            private void BtnCancel_Click(object sender, EventArgs e)
            {
                Dispose();
            }

            private void BtnReplaceAll_Click(object sender, EventArgs e)
            {
                //MessageBox.Show("BtnReplaceAll_Click(object sender, EventArgs e)");
                //bool bolDebug = false;
                //if (bolDebug)
                //{
                //    TopMost = false;
                //    RichTextBox rtxTemp = rtx.rtx;
                //    {
                //        rtx.rtx = new RichTextBox();
                //        //classProject cProject = cProject;
                //        for (int intNoteCounter = 0; intNoteCounter < cProject.lstChildNotes.Count; intNoteCounter++)
                //        {
                //            classNotesInfo cNote = cProject.lstChildNotes[intNoteCounter];
                //            string strFilename = groupboxNotes.NotesFilename(cNote.Heading);
                //            if (System.IO.File.Exists(strFilename))
                //            {
                //                rtx.LoadFile(strFilename);
                //                int intIndexSearch = rtx.Text.ToLower().IndexOf(txtSearchText.Text.ToLower());
                //                while (intIndexSearch >= 0 && intIndexSearch + txtSearchText.Text.Length < rtx.Text.Length)
                //                {
                //                    rtx.Select(intIndexSearch, txtSearchText.Text.Length);
                //                    replace(txtSearchText.Text, txtReplacementText.Text);

                //                    intIndexSearch = rtx.Text.ToLower().IndexOf(txtSearchText.Text.ToLower());
                //                }

                //                rtx.SaveFile(strFilename);
                //            }
                //        }
                //        //cProject.lstChildNotes[0].
                //    }
                //    rtx.rtx = rtxTemp;

                //    TopMost = true;
                //    //return;
                //}
                do
                {
                    BtnFindNext_Click(sender, e);
                } while (replace(txtSearchText.Text, txtReplacementText.Text));

            }

            private void BtnReplace_Click(object sender, EventArgs e)
            {
                replace(txtSearchText.Text, txtReplacementText.Text);
            }

            public void BtnFindNext_Click(object sender, EventArgs e)
            {
                Search_Indices_BuildList();

                if (intSearchListIndex < 0)
                {
                    Search_Indices_FindBest();
                }

                if (intSearchListIndex < 0 || intSearchListIndex >= lstSearchResults.Count)
                {
                    goto QuitSearch;
                }

                if (rbtSearchDown.Checked)
                {
                    if (rtx.SelectionStart >= lstSearchResults[intSearchListIndex])
                        intSearchListIndex++;
                    if (intSearchListIndex < lstSearchResults.Count && intSearchListIndex >= 0)
                    {
                        Search_SelectText();
                    }
                    else
                        goto QuitSearch;
                }
                else
                {
                    if (rtx.SelectionStart <= lstSearchResults[intSearchListIndex])
                        intSearchListIndex--;
                    if (intSearchListIndex > 0)
                    {
                        Search_SelectText();
                    }
                    else
                        goto QuitSearch;
                }
                return;
            QuitSearch:
                ;
                //MessageBox.Show("Search has reached end of text", "Search " + (rbtSearchDown.Checked ? "Down" : "Up") + " Complete");

            }
            private void TxtSearchText_GotFocus(object sender, EventArgs e)
            {
                txtSearchText.SelectAll();
            }

            private void TxtSearchText_KeyUp(object sender, KeyEventArgs e)
            {
                btnReplace.Enabled
                    = btnReplaceAll.Enabled
                    = txtReplacementText.Enabled
                    = txtSearchText.Text.Length > 0;
                Search_Reset();
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        {
                            BtnFindNext_Click((object)btnFindNext, new EventArgs());
                        }
                        break;
                }
            }

            #endregion 
        }

        public class classHighlighterColorItem
        {
            public bool valid = false;
            public Color clrBack = Color.White;
            public Color clrFore = Color.Black;
            public string Text = "";
            public classHighlighterColorItem(Color clrBack, Color clrFore, string text)
            {
                this.clrBack = clrBack;
                this.clrFore = clrFore;
                this.Text = text;
                this.valid = true;
            }
        }
    }

}
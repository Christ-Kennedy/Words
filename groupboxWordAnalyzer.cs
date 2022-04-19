using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Ck_Objects;
 
namespace Words
{
    public class groupboxWordAnalyzer : GroupBox
    {
        public static groupboxWordAnalyzer instance = null;

        classLabelButton btnWordOutline_Toggle = new classLabelButton();
        classLabelButton btnOrder = new classLabelButton();
        classLabelButton btnWordMap = new classLabelButton();
        classLabelButton btnLock = new classLabelButton();
        pictureboxWordAnalyzer picWordMap = new pictureboxWordAnalyzer();
        
        Label lblFilename = new Label();
        Label lblWordCount = new Label();
        public TextBox txtSearch = new TextBox();
        Timer tmrUnderlineSelection = new Timer();
        Timer tmrDraw = new Timer();
        int _intWords_Count = -1;
        public int Words_Count
        {
            get { return _intWords_Count; }
            set { _intWords_Count = value; }
        }

        int _intWords_Distinct = -1;
        public int Words_Distinct
        {
            get { return _intWords_Distinct; }
            set { _intWords_Distinct = value; }
        }

        static bool bolWordMap = false;
        static public bool WordMap
        {
            get { return instance.picWordMap.Visible; }
            set 
            {
                if (instance == null)
                {
                    bolWordMap = value;
                    return;
                }
                if (bolWordMap != value)
                {
                    instance.picWordMap.SizeMode = PictureBoxSizeMode.Zoom;
                    instance.picWordMap.Visible 
                        = bolWordMap
                        = value;
                    instance.placeObjects();
                    instance.tmrDraw_Reset();
                }
            }
        }

        void lblWordCount_Set()
        {
            lblWordCount.Text = Words_Distinct.ToString() + " / " + Words_Count.ToString() + "   " + ((float)Words_Distinct / (float)Words_Count).ToString("0.00");
        }

        VScrollBar vScrBar = new VScrollBar();

        public MultiButtonPic_WordAnalyzer mbpWordAnalyzer = null;

        public List<classWordAnalyzer_Item> lstWords = new List<classWordAnalyzer_Item>();
        groupboxNotes grbNotes
        {
            get
            {
                if (formWords.instance == null) return null;
                return formWords.instance.grbNotes;
            }
        }

        bool bolAlphabeticalOrder = true;
        bool AlphabeicalOrder
        {
            get { return bolAlphabeticalOrder; }
            set
            {
                if (bolAlphabeticalOrder != value)
                {
                    bolAlphabeticalOrder = value;
                    if (bolAlphabeticalOrder)
                        Order_Alphabetical((object)null, new EventArgs());
                    else
                        Order_Count((object)null, new EventArgs());
                }
            }
        }
        void btnOrder_Click(object sender, EventArgs e)
        {
            AlphabeicalOrder = !AlphabeicalOrder;
            btnOrder_SetText();
        }

        public void btnOrder_SetText()
        {
            btnOrder.Text = AlphabeicalOrder ? "Count" : "Alpha";
        }

        void Order_Count(object sender, EventArgs e)
        {
            string strWordSelected = WordSelected;
            IEnumerable<classWordAnalyzer_Item> query = lstWords.OrderByDescending(item => item.Count);
            lstWords = (List<classWordAnalyzer_Item>)query.ToList<classWordAnalyzer_Item>();
            displayWords();
            DisplayWord(strWordSelected);

        }
        void Order_Alphabetical(object sender, EventArgs e)
        {
            string strWordSelected = WordSelected;
            IEnumerable<classWordAnalyzer_Item> query = lstWords.OrderBy(item => item.Word);
            lstWords = (List<classWordAnalyzer_Item>)query.ToList<classWordAnalyzer_Item>();
            displayWords();
            DisplayWord(strWordSelected);
        }

  
        public groupboxWordAnalyzer()
        {
            instance = this;

            Text = "Word Analyzer";

            BackColor = Color.Beige;

            MouseClick += GroupboxWordAnalyzer_MouseClick;
            MouseEnter += GroupboxWordAnalyzer_MouseEnter;
            MouseLeave += GroupboxWordAnalyzer_MouseLeave;

            groupboxWordAnalyzer cMyRef = this;

            Controls.Add(txtSearch);
            txtSearch.Size = new Size(30, 13);
            txtSearch.KeyDown += TxtSearch_KeyDown;
            txtSearch.TextChanged += SearchTextLoad_MouseClickChanged;
            txtSearch.GotFocus += SearchTextLoad_MouseClickChanged;

            Controls.Add(lblWordCount);
            lblWordCount.AutoSize = true;
            lblWordCount.SizeChanged += LblWordCount_SizeChanged;

            mbpWordAnalyzer = new MultiButtonPic_WordAnalyzer(ref cMyRef);
            Controls.Add(mbpWordAnalyzer);

            Controls.Add(lblFilename);
            lblFilename.Font = new Font("Arial", 12, FontStyle.Bold);
            lblFilename.TextAlign = ContentAlignment.TopRight;

            Controls.Add(vScrBar);
            vScrBar.ValueChanged += VScrBar_ValueChanged;

            Controls.Add(btnWordOutline_Toggle);
            btnWordOutline_Toggle.AutoSize = true;
            btnWordOutline_Toggle.CanBeToggled = true;
            btnWordOutline_Toggle.Text = "!";
            btnWordOutline_Toggle.Font = new Font("Arial", 10);
            btnWordOutline_Toggle.Forecolor_Idle
                       = btnWordOutline_Toggle.ForeColor
                       = btnWordOutline_Toggle.Backcolor_Highlight
                       = Color.Black;
            btnWordOutline_Toggle.Backcolor_Idle
                        = btnWordOutline_Toggle.BackColor
                        = btnWordOutline_Toggle.Forecolor_Highlight
                        = Color.White;
            btnWordOutline_Toggle.Forecolor_Toggled_Idle
                        = btnWordOutline_Toggle.Backcolor_Toggled_Highlight
                        = Color.White;
            btnWordOutline_Toggle.Backcolor_Toggled_Idle
                        = btnWordOutline_Toggle.Forecolor_Toggled_Highlight
                        = Color.Red;
            btnWordOutline_Toggle.Refresh();
            btnWordOutline_Toggle.Toggle_Changed = btnWordOutline_Toggle_Changed;

            Controls.Add(btnOrder);
            btnOrder.AutoSize = true;
            btnOrder.Text = "Alpha";
            btnOrder.Click += btnOrder_Click;

            Controls.Add(btnWordMap);
            btnWordMap.AutoSize = true;
            btnWordMap.Text = "Word Map";
            btnWordMap.Click += btnWordMap_Click;

            Controls.Add(btnLock);
            btnLock.AutoSize = false;
            btnLock.Text = "";
            btnLock.img_Highlight= new Bitmap(Properties.Resources.ck_RichTextBox_Button_Lock);
            btnLock.img_Idle = new Bitmap(Properties.Resources.ck_RichTextBox_Button_Lock_Highlight);
            btnLock.Size = new Size(18, 18);
            btnLock.CanBeToggled = true;
            btnLock.Click += btnLock_Click;

            Controls.Add(picWordMap);
            picWordMap.Hide();

            tmrUnderlineSelection.Interval = 200;
            tmrUnderlineSelection.Tick += TmrUnderlineSelection_Tick;

            tmrDraw.Interval = 250;
            tmrDraw.Tick += TmrDraw_Tick;

            Lock = true;
            Lock = false;

            SizeChanged += GroupboxWordAnalyzer_SizeChanged;
        }

        

        private void TmrDraw_Tick(object sender, EventArgs e)
        {
            tmrDraw.Enabled = false;
            if (!Lock)
                pictureboxWordAnalyzer.instance.Word = mbpWordAnalyzer.SelectedWord;
            else
                pictureboxWordAnalyzer.instance.DrawWordMap();
        }

        public void tmrDraw_Reset()
        {
            tmrDraw.Enabled = false;
            tmrDraw.Enabled = WordMap;
        }

        private void TmrUnderlineSelection_Tick(object sender, EventArgs e)
        {
            tmrUnderlineSelection.Enabled = false;
            mbpWordAnalyzer.UnderlineSelectedWord();
        }

        public void tmrUnderlineSelection_Reset()
        {
            tmrUnderlineSelection.Enabled = false;
            tmrUnderlineSelection.Enabled = true;
            tmrUnderlineSelection.Interval = 500;
        }

        int _intsplSub_SplitDistance = -1;
        public int splSub_SplitDistance
        {
            get { return _intsplSub_SplitDistance; }
            set { _intsplSub_SplitDistance = value; }
        }


      
        bool bolCollapsed = false;
        public bool Collapsed
        {
            get { return bolCollapsed; }
            set
            {
                if (!bolCollapsed)
                    splSub_SplitDistance = grbNotes.splSub.SplitterDistance;

                bolCollapsed = value;
                grbNotes.splSub.Panel1MinSize = 12;
                grbNotes.splSub.SplitterDistance = bolCollapsed
                                                    ? 0
                                                    : splSub_SplitDistance;
                grbNotes.placeObjects();
                cursor_Set();
                Analyze();
            }
        }

        void cursor_Set()
        {
            Cursor = bolCollapsed ? formWords.cCursors[(int)enuCursors.Expand] : formWords.cCursors[(int)enuCursors.Collapse];
        }

        private void GroupboxWordAnalyzer_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void GroupboxWordAnalyzer_MouseEnter(object sender, EventArgs e)
        {
            cursor_Set();
        }

        private void GroupboxWordAnalyzer_MouseClick(object sender, MouseEventArgs e)
        {
            Collapsed = !Collapsed;
        }

        private void SearchTextLoad_MouseClickChanged(object sender, EventArgs e)
        {
            Search(txtSearch.Text);
        }

        private void LblWordCount_SizeChanged(object sender, EventArgs e)
        {
            txtSearch.Location = new Point(lblWordCount.Right, lblWordCount.Top);
            txtSearch.Size = new Size(Width - txtSearch.Left, lblWordCount.Height);
        }

        int Search_GetLength(string str1, string str2)
        {
            str1 = str1.Trim().ToUpper();
            str2 = str2.Trim().ToUpper();
            int intLength = 0;
            string strTest1 = str1.Substring(0, intLength);
            string strTest2 = str2.Substring(0, intLength);
            int intBestLength = 0;
            int intComparison = 0;
            do
            {
                intComparison = string.Compare(strTest1, strTest2);

                if (intComparison == 0)
                    intBestLength = intLength;
                else
                    return intBestLength;

                intLength++;

                if (intLength > str1.Length || intLength > str2.Length)
                    return intBestLength;

                strTest1 = str1.Substring(0, intLength);
                strTest2 = str2.Substring(0, intLength);                
            } while (true);            
        }

        public void Search(string strWord_Search)
        {
            if (Collapsed) return;

            strWord_Search = strWord_Search.Trim().ToUpper();

            int intMax = lstWords.Count;
            int intIndex = intMax /2;
            int intStep = intIndex;
            int intDir = 0;
            int intStepResetCounter = 0;
            int intStepResetCounter_Max = 32;
            int intBestIndex = -1;
            int intBestLength = 0;

            while (intIndex >= 0 && intIndex < intMax)
            {
                string strWord_Test = lstWords[intIndex].Word;
                intDir = string.Compare(strWord_Search, strWord_Test);

                int intLength = Search_GetLength(strWord_Test, strWord_Search);
                if (intLength > intBestLength || intLength == strWord_Search.Length)
                {
                    intBestLength = intLength;
                    intBestIndex = intIndex;
                }
                if (intDir == 0)
                {
                    break;
                }
                else
                {
                    intStep /= 2;
                    if (intStep < 1)
                    {
                        intStep = 1;
                        intStepResetCounter++;
                        if (intStepResetCounter > intStepResetCounter_Max)
                            break;
                    }
                    intIndex += intDir * intStep;
                }
            }
            TopIndex = intBestIndex - mbpWordAnalyzer.NumWords_Above;
        }


        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
           
            switch(e.KeyCode)
            {
                case Keys.Back:
                case Keys.Delete:
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Shift:
                    break;


                case Keys.Enter:
                    {
                        Search(txtSearch.Text);
                    }
                    break;

                default:
                    {
                        char chrKeyPressed = (char)e.KeyData;
                        if (!char.IsLetter(chrKeyPressed))
                        {
                            e.SuppressKeyPress = true;
                            return;
                        }
                    }
                    break;
            }
        }

        private void VScrBar_ValueChanged(object sender, EventArgs e)
        {
            if (TopIndex != vScrBar.Value)
                TopIndex = vScrBar.Value;
        }

        bool bolLock = false;
        public bool Lock
        {
            get { return bolLock; }
            set
            {
                if (bolLock != value)
                {
                    bolLock = value;

                    if (bolLock)
                    {
                        btnLock.img_Highlight = new Bitmap(Properties.Resources.btnNotes_Lock_closed_highlight);
                        btnLock.img_Idle = new Bitmap(Properties.Resources.btnNotes_Lock_closed_idle);
                    }
                    else
                    {
                        btnLock.img_Highlight = new Bitmap(Properties.Resources.btnNotes_Lock_open_highlight);
                        btnLock.img_Idle = new Bitmap(Properties.Resources.btnNotes_Lock_open_idle);
                    }

                    btnLock.Size = new Size(18, 18);
                }
                vScrBar.Enabled = !Lock;
            }
        }

        string _strWordSelected = "";
        public string WordSelected
        {
            get { return _strWordSelected; }
            set
            {
                if (!Lock)
                    _strWordSelected = value;
            }
        }

        int intTopIndex = 0;
        public int TopIndex
        {
            get { return intTopIndex; }
            set
            {
                if (value + mbpWordAnalyzer.NumWords_Below >= mbpWordAnalyzer.lstWordItems_Total.Count)
                    value = mbpWordAnalyzer.lstWordItems_Total.Count - mbpWordAnalyzer.NumWords_Below ;

                if (value < -mbpWordAnalyzer.NumWords_Above)
                    value = -mbpWordAnalyzer.NumWords_Above;

                //if (intTopIndex != value)
                {
                    intTopIndex = value;
                    if (value >= vScrBar.Minimum && value <= vScrBar.Maximum)
                        vScrBar.Value = value;

                    mbpWordAnalyzer.DisplayButtons();
                    if (btnWordOutline_Toggle.Toggled)
                        tmrUnderlineSelection_Reset();

                    if (pictureboxWordAnalyzer.instance != null && pictureboxWordAnalyzer.instance.Visible)
                        if (ckRTX_Focus != null)                        
                            tmrDraw_Reset();
                }  
            }
        }


        private void GroupboxWordAnalyzer_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        void placeObjects()
        {
            int intGap = 4;
            lblWordCount.Location = new Point(intGap, 15); 
            txtSearch.Location = new Point(lblWordCount.Right, lblWordCount.Top);
            txtSearch.Size = new Size(Width - txtSearch.Left - 2*intGap, lblWordCount.Height);

            btnWordOutline_Toggle.Location = new System.Drawing.Point(intGap, txtSearch.Bottom);
            btnOrder.Location = new System.Drawing.Point(btnWordOutline_Toggle.Right, btnWordOutline_Toggle.Top + btnLock.Height - btnOrder.Height);
            btnWordMap.Location = new Point(btnOrder.Right, btnWordOutline_Toggle.Top + btnLock.Height - btnWordMap.Height);
            btnLock.Location = new Point(btnWordMap.Right, btnWordOutline_Toggle.Top);

            lblFilename.Location = new Point(btnLock.Right, btnWordOutline_Toggle.Top);
            lblFilename.Width = Width - lblFilename.Left - intGap;
            lblFilename.Height = btnLock.Height;


            if (WordMap)
            {
                picWordMap.Width = (int)(0.2 * Width);
                picWordMap.Left = Width - picWordMap.Width - intGap;
                picWordMap.Top = lblFilename.Bottom;

                vScrBar.Location = new Point(picWordMap.Left - vScrBar.Width, btnLock.Bottom);
                vScrBar.Height = Height - vScrBar.Top - intGap;
                mbpWordAnalyzer.Location = new System.Drawing.Point(intGap, lblFilename.Bottom);

                mbpWordAnalyzer.Width = vScrBar.Left;
                picWordMap.Height
                    = mbpWordAnalyzer.Height
                    = Height - mbpWordAnalyzer.Top - intGap;
            }
            else
            {
                vScrBar.Location = new Point(Width - vScrBar.Width - intGap, lblFilename.Bottom);
                vScrBar.Height = Height - vScrBar.Top - intGap;
                mbpWordAnalyzer.Location = new System.Drawing.Point(intGap, vScrBar.Top);

                mbpWordAnalyzer.Size = new System.Drawing.Size(vScrBar.Left - intGap, Height - mbpWordAnalyzer.Top - intGap);
            }
            
        }

        void btnWordMap_Click(object sender, EventArgs e)
        {
            if (WordMap)
            {
                WordMap = false;
                return;
            }


            if (ckRTX_Focus != null)
            {
                pictureboxWordAnalyzer.instance.Word = ckRTX_Focus.WordUnderCursor("");
                WordMap = true;
            }
        }

        void btnLock_Click(object sender, EventArgs e)
        {
            if (Lock)
            {
                Lock = false;
                return;
            }


            if (ckRTX_Focus != null)
            {
                pictureboxWordAnalyzer.instance.Word = ckRTX_Focus.WordUnderCursor("");
                Lock = true;
            }
        }


        public void DisplayWord(string strWord)
        {
            classWordAnalyzer_Item cWordItem = classWordAnalyzer_Item.Search(strWord.Trim());
            if (cWordItem != null)
            {
                /*
                int intSelectedIndex = lstWords.IndexOf(cWordItem);
                if (intSelectedIndex > mbpWordAnalyzer.NumWords_Above)
                    TopIndex = intSelectedIndex - mbpWordAnalyzer.NumWords_Above;
                else
                    TopIndex = 0;
                /*/
                TopIndex = lstWords.IndexOf(cWordItem) - mbpWordAnalyzer.NumWords_Above;
                // */
            }
        }
    
        void displayWords()
        {
            TopIndex = 0;
        }


        void btnWordOutline_Toggle_Changed(object sender, EventArgs e)
        {
            if (rtxFocus != null)
            {
                if (btnWordOutline_Toggle.Toggled)
                    mbpWordAnalyzer.UnderlineSelectedWord();
                else
                    rtxFocus.Refresh();
            }
        }
        
        public bool WordOutline_Toggle
        {
            get { return btnWordOutline_Toggle.Toggled; }
        }

        ck_RichTextBox ckRTX_Focus
        {
            get
            {
                if (rtxFocus != null && rtxFocus.Parent != null)
                {
                    ck_RichTextBox cRetVal = (ck_RichTextBox)rtxFocus.Parent;
                    return cRetVal;
                }
                return formWords.instance.rtxCK; ;
            }
        }

        static RichTextBox _rtxFocus = null;
        static public RichTextBox rtxFocus
        {
            get { return _rtxFocus; }
            set
            {
                if (_rtxFocus != value)
                {
                    _rtxFocus = value;
                    if (instance != null)
                    {
                        instance.Analyze();
                        instance.placeObjects();
                        instance.lblFilename_Set();
                    }
                }
            }
        }

        void lblFilename_Set()
        {
            if (rtxFocus == grbNotes.rtxNotes.rtx)
            {

                lblFilename.BackColor = grbNotes.rtxNotes.Heading_BackColor;
                lblFilename.ForeColor = grbNotes.rtxNotes.Heading_ForeColor;
                lblFilename.Text = (grbNotes.pnlNotes != null && grbNotes.pnlNotes.cEdit_Alt != null)
                                                     ? grbNotes.pnlNotes.cEdit_Alt.Title 
                                                     : "";
            }
            else
            {

                lblFilename.BackColor = formWords.instance.rtxCK.Heading_BackColor;
                lblFilename.ForeColor = formWords.instance.rtxCK.Heading_ForeColor;
                lblFilename.Text = groupboxNotes.cProject != null && groupboxNotes.cProject.cEdit_Main!= null
                                                           ? groupboxNotes.cProject.cEdit_Main.Title
                                                           : "";
            }

            BackColor = lblFilename.BackColor;
        }

        char[] _chrSplit = null;
        char[] chrSplit
        {
            get
            {
                if (_chrSplit == null)
                {
                    string strCharKeep = "-";


                    List<char> lstCharSplit = new List<char>();
                    for (int i = 0; i < 255; i++)
                    {
                        char chrNew = (char)i;
                        if (!char.IsLetter(chrNew) && !strCharKeep.Contains(chrNew))
                            lstCharSplit.Add(chrNew);
                    }

                    _chrSplit = lstCharSplit.ToArray();
                }
                return _chrSplit;
            }
        }

        public void Analyze()
        {
            if (rtxFocus == null || Collapsed) return;      
            
            string[] strWords = rtxFocus.Text.ToUpper().Split(chrSplit, StringSplitOptions.RemoveEmptyEntries);
            Words_Count = strWords.Length;

            classWordAnalyzer_Item.Reset();
            for (int i = 0; i < strWords.Length; i++)
                classWordAnalyzer_Item.Insert(strWords[i]);
            lstWords.Clear();
            lstWords.AddRange(classWordAnalyzer_Item.Traverse());
            Words_Distinct = lstWords.Count;
            
            vScrBar.Minimum = 0;
            vScrBar.LargeChange = mbpWordAnalyzer.NumWordsDisplayed;

            int intMax = lstWords.Count - 2;
            if (intMax >= 0)
            {
                if (vScrBar.Value > intMax)
                    vScrBar.Value = intMax;
                vScrBar.Maximum = intMax;
            }
            else
            {
                vScrBar.Value
                    = vScrBar.Maximum
                    = 0;
            }
            TopIndex = 0;
            formWords.instance.tmrWordAnalyzer_Reset();
            lblFilename_Set();
            lblWordCount_Set();
        }

        public class classWordAnalyzer_Item
        {
            public string Word = "";
            public int Count = 1;

            static classWordAnalyzer_Item Root = null;

            public classWordAnalyzer_Item Left = null;
            public classWordAnalyzer_Item Right = null;


            static classWordAnalyzer_Item _cBlank = null;
            public static classWordAnalyzer_Item cBlank
            {
                get
                {
                    if (_cBlank == null)
                    {
                        _cBlank = new classWordAnalyzer_Item("");
                        _cBlank.Count = 0;
                    }
                    return _cBlank;
                }
            }

            public static void Reset()
            {
                Root = null;
            }

            classWordAnalyzer_Item(string Word)
            {
                this.Word = Word;

            }


            static List<classWordAnalyzer_Item> lstTraverseResults = new List<classWordAnalyzer_Item>();
            public static List<classWordAnalyzer_Item> Traverse()
            {
                lstTraverseResults.Clear();
                Traverse(ref Root);
                return lstTraverseResults;
            }
            public static void Traverse(ref classWordAnalyzer_Item cTemp)
            {
                if (cTemp == null) return;

                Traverse(ref cTemp.Left);
                lstTraverseResults.Add(cTemp);
                Traverse(ref cTemp.Right);
            }



            public static void Insert(String Word)
            {
                if (Root == null)
                {
                    Root = new classWordAnalyzer_Item(Word);
                    return;
                }

                Word = Word.Trim().ToUpper();
                classWordAnalyzer_Item cTemp = Root;
                while (cTemp != null)
                {
                    int intComparison = string.Compare(Word, cTemp.Word);
                    if (intComparison < 0)
                    {
                        if (cTemp.Left == null)
                        {
                            cTemp.Left = new classWordAnalyzer_Item(Word);
                            return;
                        }
                        else
                            cTemp = cTemp.Left;
                    }
                    else if (intComparison > 0)
                    {
                        if (cTemp.Right == null)
                        {
                            cTemp.Right = new classWordAnalyzer_Item(Word);
                            return;
                        }
                        else cTemp = cTemp.Right;
                    }
                    else
                    {
                        cTemp.Count++;
                        return;
                    }
                }
            }

            public static classWordAnalyzer_Item Search(string Word)
            {
                Word = Word.Trim().ToUpper();
                classWordAnalyzer_Item cTemp = Root;
                while (cTemp != null)
                {
                    int intComparison = string.Compare(Word, cTemp.Word);
                    if (intComparison < 0)
                    {
                        if (cTemp.Left == null) return null;
                        else
                            cTemp = cTemp.Left;
                    }
                    else if (intComparison > 0)
                    {
                        if (cTemp.Right == null) return null;
                        else cTemp = cTemp.Right;
                    }
                    else
                        return cTemp;
                }
                return null;
            }
        }
    }

    public class MultiButtonPic_WordAnalyzer : classMultiButtonPic
    {
        groupboxWordAnalyzer grbWordAnalyzer = null;

        List<classMultiButtonPic.classButton> lstButtons = new List<classButton>();
        List<classMultiButtonPic.classButton> lstButtons_Reserved = new List<classButton>();
        public MultiButtonPic_WordAnalyzer(ref groupboxWordAnalyzer grbWordAnalyzer)
        {
            this.grbWordAnalyzer = grbWordAnalyzer;

            BackColor = Color.Gray;
            ForeColor = Color.Black;

            Formation = classMultiButtonPic.enuButtonFormation.manual;
            classMultiButtonPic cMyRef = this;

            BorderStyle = BorderStyle.Fixed3D;
            SizeChanged += mbpWordAnalyzer_SizeChanged;
            FontChanged += mbpWordAnalyzer_FontChanged;

            MouseWheel += mbpWordAnalyzer_MouseWheel;
            MouseDown += MultiButtonPic_WordAnalyzer_MouseDown;
            MouseUp += MultiButtonPic_WordAnalyzer_MouseUp;
            MouseDoubleClick += MultiButtonPic_WordAnalyzer_MouseDoubleClick;
            MouseLeave += MultiButtonPic_WordAnalyzer_MouseLeave;
            
            DisplayButtons();
        }

        private void MultiButtonPic_WordAnalyzer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (groupboxWordAnalyzer.instance.Lock) return;

            if (groupboxWordAnalyzer.rtxFocus != null && groupboxWordAnalyzer.rtxFocus.Parent != null
                   && ButtonUnderMouse != null)
            {
                ck_RichTextBox ckRTX = (ck_RichTextBox)groupboxWordAnalyzer.rtxFocus.Parent;


                string strSelectedWord = ButtonUnderMouse.Text.Trim().ToUpper();
                grbWordAnalyzer.Search(strSelectedWord);

                ckRTX.FindNext(strSelectedWord);
            }
        }
               

        public string SelectedWord
        {
            get
            {
                if (ButtonUnderMouse == null && cButtonSelected == null) return "";


                string strSelectedWord = (MouseButtons == MouseButtons.Left && ButtonUnderMouse != null)
                                                           ? ButtonUnderMouse.Text.Trim().ToUpper()
                                                           : cButtonSelected.Text.Trim().ToUpper();

                return strSelectedWord;
            }
        }


        public void UnderlineSelectedWord()
        {
            if (groupboxWordAnalyzer.rtxFocus != null && groupboxWordAnalyzer.rtxFocus.Parent != null 
                    && cButtonSelected != null)
            {
                //string strSelectedWord = (MouseButtons == MouseButtons.Left && ButtonUnderMouse != null) 
                //                                           ? ButtonUnderMouse.Text.Trim().ToUpper()
                //                                           : cButtonSelected.Text.Trim().ToUpper();

                string strSelectedWord = SelectedWord;
                Pen pUnderline = new Pen(Color.Red, 2);

                ck_RichTextBox ckRTX = (ck_RichTextBox)groupboxWordAnalyzer.rtxFocus.Parent;
                ckRTX.Underline(pUnderline, strSelectedWord, ck_RichTextBox.enuGraphics_Lines.Squiggle);
            }
            
        }

        void RefreshRTX()
        {
            if (groupboxWordAnalyzer.rtxFocus != null)
                groupboxWordAnalyzer.rtxFocus.Refresh();
        }

        private void MultiButtonPic_WordAnalyzer_MouseLeave(object sender, EventArgs e)
        {
            //RefreshRTX();
        }

        private void MultiButtonPic_WordAnalyzer_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (grbWordAnalyzer.WordOutline_Toggle)
                UnderlineSelectedWord();
            else
                RefreshRTX();
        }

        private void MultiButtonPic_WordAnalyzer_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {   
                List<string> lstWords = new List<string>();



                string strSelectedWord = (MouseButtons == MouseButtons.Right && ButtonUnderMouse != null)
                                                           ? ButtonUnderMouse.Text.Trim().ToUpper()
                                                           : cButtonSelected.Text.Trim().ToUpper();


                lstWords.Add(strSelectedWord);
                classDictionary cDictionaryPopUp = null;
                if (formDictionarySelection.PopUp_Dictionary != null)
                {
                    cDictionaryPopUp = formDictionarySelection.PopUp_Dictionary;
                }
                else
                {
                    if (formPopUp.instance == null)
                        new formPopUp();
                    cDictionaryPopUp = formPopUp.instance.cDictionary;
                }

                if (cDictionaryPopUp != null)
                {
                    List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                    lstSearchParameters.Add(new classSearchParameters(ref cDictionaryPopUp, ref lstWords, enuSearchType.Heading));
                    formWords.instance.Search(ref lstSearchParameters, enuSearchRequestor.definition_Hover);
                }
                ContextMenu = null;
                return;
            }

            UnderlineSelectedWord();
        }

        private void mbpWordAnalyzer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (groupboxWordAnalyzer.instance.Lock) return;

            if (e.Delta > 0)
                scroll_Up();
            else
                scroll_Down();
        }
   

        void scroll_Up()
        {
            grbWordAnalyzer.TopIndex = grbWordAnalyzer.TopIndex - 1;
        }
        void scroll_Down()
        {
            grbWordAnalyzer.TopIndex = grbWordAnalyzer.TopIndex + 1;
        }

        public void DisplayButtons()
        {
            Buttons_Rebuild();
            Refresh();
        }

        private void mbpWordAnalyzer_FontChanged(object sender, EventArgs e)
        {

        }

        Font _fntHeadingList = new Font("Arial", 8);
        public Font fntHeadingList
        {
            get { return _fntHeadingList; }
            set { _fntHeadingList = value; }
        }

        System.Drawing.Color _clrHeadingList = System.Drawing.Color.Gray;
        public System.Drawing.Color HeadingList
        {
            get { return _clrHeadingList; }
            set { _clrHeadingList = value; }
        }

        Font _fntHeadingList_UnderMouse = new Font("Arial", 8);
        public Font fntHeadingList_UnderMouse
        {
            get { return _fntHeadingList_UnderMouse; }
            set { _fntHeadingList_UnderMouse = value; }
        }
        System.Drawing.Color _clrHeadingList_UnderMouse = System.Drawing.Color.Black;
        public System.Drawing.Color HeadingList_UnderMouse
        {
            get { return _clrHeadingList_UnderMouse; }
            set { _clrHeadingList_UnderMouse = value; }
        }

        Font _fntHeadingList_Selected = new Font("Arial", 8);
        public Font fntHeadingList_Selected
        {
            get { return _fntHeadingList_Selected; }
            set { _fntHeadingList_Selected = value; }
        }
        System.Drawing.Color _clrHeadingList_Selected = System.Drawing.Color.Blue;
        public System.Drawing.Color HeadingList_Selected
        {
            get { return _clrHeadingList_Selected; }
            set { _clrHeadingList_Selected = value; }
        }

        private void mbpWordAnalyzer_SizeChanged(object sender, EventArgs e)
        {
            Size szItemHeight = TextRenderer.MeasureText("Chibougamou", fntHeadingList);

            float numPerHalf = ((float)(Height - szItemHeight.Height) / (float)szItemHeight.Height) / 2.0f;
            intNumWords_Above = (int)Math.Floor(numPerHalf);
            intNumWords_Below = (int)Math.Ceiling(numPerHalf);
            intNumWordsDisplayed = 1 + NumWords_Above + NumWords_Below;

            DisplayButtons();
        }

        int intNumWords_Above = -1;
        public int NumWords_Above { get { return intNumWords_Above; } }

        int intNumWords_Below = -1;
        public int NumWords_Below { get { return intNumWords_Below; } }

        int intNumWordsDisplayed = -1;
        public int NumWordsDisplayed { get { return intNumWordsDisplayed; } }

        classButton cButtonSelected
        {
            get
            {
                int intButtonIndex = NumWords_Above * 2;
                if (intButtonIndex >= 0 && intButtonIndex < lstButtons.Count)
                    return lstButtons[intButtonIndex];
                else
                    return null;
            }
        }
        
        List<groupboxWordAnalyzer.classWordAnalyzer_Item> _lstWords = new List<groupboxWordAnalyzer.classWordAnalyzer_Item>();
        List<groupboxWordAnalyzer.classWordAnalyzer_Item> lstWords
        {
            get { return _lstWords; }
            set
            {
                _lstWords = value;
            }
        }

        //int Index(ref groupboxWordAnalyzer.classWordAnalyzer_Item cWordItem)
        //{
        //    return grbWordAnalyzer != null
        //                            ? grbWordAnalyzer.lstWords.IndexOf(cWordItem)
        //                            : -1;
        //}

        public void Buttons_Rebuild()
        {
            if (grbWordAnalyzer.lstWords == null) return;
            Color clrButtonBck = Color.White;

            if (NumWordsDisplayed <= 0)
            {
                return;
            }
            classMultiButtonPic cMyRef = this;

            // ensure list is the right length
            {
                lstWords.Clear();
                int intNumGet = grbWordAnalyzer.TopIndex + NumWordsDisplayed < lstWordItems_Total.Count
                                                                             ? NumWordsDisplayed
                                                                             : lstWordItems_Total.Count - grbWordAnalyzer.TopIndex;
                if (intNumGet > 0)
                {
                    if (grbWordAnalyzer.TopIndex <0)
                    {
                        for (int i = 0; i < Math.Abs(grbWordAnalyzer.TopIndex); i++)
                            lstWords.Add(groupboxWordAnalyzer.classWordAnalyzer_Item.cBlank);
                        lstWords.AddRange(grbWordAnalyzer.lstWords.GetRange(0, intNumGet + grbWordAnalyzer.TopIndex));
                    }
                    else if (grbWordAnalyzer.TopIndex + NumWordsDisplayed > grbWordAnalyzer.lstWords.Count)
                    {
                        int intWordsExtra = NumWordsDisplayed - intNumGet;
                        lstWords.AddRange(grbWordAnalyzer.lstWords.GetRange(grbWordAnalyzer.TopIndex, intNumGet));
                        for (int i = 0; i < intWordsExtra; i++)
                            lstWords.Add(groupboxWordAnalyzer.classWordAnalyzer_Item.cBlank);
                    }
                    else
                    {
                        lstWords.AddRange(grbWordAnalyzer.lstWords.GetRange(grbWordAnalyzer.TopIndex, intNumGet));
                    }
                    
                }
            }

            if (NumWordsDisplayed <= 0) return;
            // build, draw & position all needed buttons
            Size szFont = TextRenderer.MeasureText("BCD", fntHeadingList);
            for (int intButtonCounter = 0; intButtonCounter < lstWords.Count * 2; intButtonCounter++)
            {
                if (lstButtons.Count <= intButtonCounter)
                {
                    classMultiButtonPic.classButton cButtonNew = null;
                    if (lstButtons_Reserved.Count > 0)
                    {
                        cButtonNew = lstButtons_Reserved[0];
                        lstButtons_Reserved.Remove(cButtonNew);
                    }
                    else
                    {
                        // create new button
                        cButtonNew = new classButton(ref cMyRef);
                    }

                    Button_Add(ref cButtonNew);

                    cButtonNew.Location = new Point(0, 0);
                    cButtonNew.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.normal;

                    lstButtons.Add(cButtonNew);
                }

                // write button - DRAW HEADING 
                int intWordIndex = intButtonCounter / 2;
                groupboxWordAnalyzer.classWordAnalyzer_Item cRec = lstWords[intWordIndex];
                classButton cButton = lstButtons[intButtonCounter];

                if (cRec != null && cRec.Word != null)
                {
                    cButton.AutoSize = false;
                    cButton.Text = intButtonCounter % 2 == 0
                                                         ? cRec.Word.Trim()
                                                         :(cRec.Count > 0
                                                                      ? cRec.Count.ToString()
                                                                      : "");

                    cButton.BackgroundStyle = (cRec.Count == 0 ? classButton.enuBackgroundStyle.AlwaysIdle : classButton.enuBackgroundStyle.normal);

                    if (cButton == cButtonSelected)
                    {
                        cButton.Font = fntHeadingList_Selected;
                        cButton.Backcolor_Highlight
                            = cButton.Backcolor_Idle
                            = Color.Blue;
                        cButton.Forecolor_Highlight
                            = cButton.Forecolor_Idle
                            = Color.White;
                    }
                    else
                    {
                        cButton.Font = fntHeadingList;
                        cButton.Backcolor_Highlight = Color.LightGray;
                        cButton.Backcolor_Idle = Color.White;
                        cButton.Forecolor_Highlight
                            = cButton.Forecolor_Idle
                            = Color.Black;
                    }

                    // position button
                    {
                        int intCountWidth = 25;
                        cButton.Size = intButtonCounter % 2 == 0
                                                            ? new Size(Width - intCountWidth, szFont.Height + 1)
                                                            : new Size(intCountWidth, szFont.Height + 1);
                        switch(intButtonCounter)
                        {
                            case 0:
                                {
                                    cButton.Location = new Point(0, 0);
                                }
                                break;

                            case 1:
                                {
                                    cButton.Location = new Point(Width - intCountWidth, 0);
                                }
                                break;

                            default:
                                {
                                    classButton cBtn_prev = lstButtons[intButtonCounter - 2];
                                    cButton.Location = new Point(cBtn_prev.Area.Left, cBtn_prev.Area.Bottom);
                                }
                                break;
                        }
                        
                        
                    }
                }
            }

            // remove excess buttons
            while (lstButtons.Count > NumWordsDisplayed *2)
            {
                classMultiButtonPic.classButton cButton_Removed = lstButtons[lstButtons.Count - 1];
                lstButtons.Remove(cButton_Removed);
                Button_Sub(ref cButton_Removed);
                lstButtons_Reserved.Add(cButton_Removed);
            }

        }


        public List<groupboxWordAnalyzer.classWordAnalyzer_Item> lstWordItems_Total
        {
            get
            {
                return grbWordAnalyzer != null
                                       ? grbWordAnalyzer.lstWords
                                       : null;
            }
        }
    }


}

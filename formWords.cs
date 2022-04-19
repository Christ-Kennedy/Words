using StringLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Words
{

    public enum enuSearchRequestor { main, definition_Hover, definition_Click, clipboard, DefinitionPort_Heading_Click, DefinitionPort_Click, PopUp, _num };
    public enum enuCursors { Expand, Collapse, Garbage, MoveWord_Left, MoveWord_Right, RecallWord, InsertWord, Capitalize, _numCursors };
    public partial class formWords : Form
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/answers/questions/753327/send-mousescroll-from-picturebox-to-richtextbox.html
        /// this was used to let the mousewheel event on the WordMap picturebox simulate a MouseWheel event on RTX editor in use
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        public static formWords instance;

        static bool bolDebug = true;
        static public bool Debug
        {
            get { return bolDebug; }
            set { bolDebug = value; }
        }

        System.Threading.Semaphore semScroll = new System.Threading.Semaphore(1, 1);
        public static classBackUp cBackUp = null;
        formWords_Flash frmWordsFlash = new formWords_Flash();

        const int intVerticalOffset = 35;

        static Cursor[] _cCursors = null;
        public static Cursor[] cCursors
        {
            get
            {
                if (_cCursors == null)
                    instance.cursors_Build();
                return _cCursors;
            }
        }
        
        
        public ck_RichTextBox rtxCK = new ck_RichTextBox();
        RichTextBox rtb = new RichTextBox();
        public static panelDictionaryOutput pnlDictionaryOutput;
        public const string conStandardToolTip = "write tool tip in ToolTip_SetUp()";
        bool bolDispose = false;
        PictureBox picHelp = new PictureBox();
        public groupboxNotes grbNotes = null;
        public static groupboxOptions grbOptions = new groupboxOptions();
        public groupboxNotePilot grbNotePilot = null;

        public BackgroundWorker bckSearch = new BackgroundWorker();
        public BackgroundWorker bckDictionaryOutput_Init = new BackgroundWorker();
        public SplitContainer splMain = new SplitContainer();
        public SplitContainer splSub_A = new SplitContainer();
        public SplitContainer splSub = new SplitContainer();

        int intDictionaryOutput_spt0_splitterDistance = 0, intDictionaryOutput_spt1_splitterDistance = 0, intPanelDefinitionPort_Height = 0;
        string _strDictionaryOuput_mpbWordList = "";
        string strDictionaryOuput_mpbWordList
        {
            get { return _strDictionaryOuput_mpbWordList; }
            set 
            {
                _strDictionaryOuput_mpbWordList = value; 
            }
        }

        groupboxHeadingList grbHeadingList
        {
            get
            {
                if (groupboxHeadingList.instance == null)
                    new groupboxHeadingList();
                return groupboxHeadingList.instance;
            }
        }

        Timer tmrHeadingList = new Timer();

        Size sz;
        Point loc;
        int intsplMainSplitterDistance = 10;
        int intsplSub_ASplitterDistance = 10;
        int intSplSubSplitterDistance = 10;
        int intSplHeadingListSplitterDistance = 10;
        int intSpl_GrbNotes_Main_SplitterDistance = 10;
        int intSpl_GrbNotes_Sub_SplitterDistance = 10;

        public static bool bolInit = false;
        public formFindReplace frmFindReplace;
        Timer tmrMessage = new Timer();
        Timer tmrPosition = new Timer();
        Timer tmrRtxSelectionStart = new Timer();
        Timer tmrWordAnalyzer = new Timer();
        Timer tmrLaunchDictionaryInit = new Timer();

        string strFirstRunFilename = "FirstRunFilename.txt";
        public formWords()
        {
            instance = this;
            Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Hide();
            string strCurrentDirectory = System.IO.Directory.GetCurrentDirectory();
            string strTest = "C:\\C Sharp\\Projects - Writing\\Words\\bin\\Debug";
            Debug = string.Compare(strCurrentDirectory, strTest ) == 0; // set to true for debug features

            InitializeComponent();
            tmrRtxSelectionStart.Interval = 100;
            tmrRtxSelectionStart.Tick += TmrRtxSelectionStart_Tick;

            Disposed += new EventHandler(formWords_Disposed);
            Move += new EventHandler(formWords_Move);
            SizeChanged += new EventHandler(formWords_SizeChanged);
            Activated += new EventHandler(formWords_Activated);
            VisibleChanged += new EventHandler(formWords_VisibleChanged);
            FormClosing += FormWords_FormClosing;
        }
        void debug_TestInitDictionary_Click(object sender, EventArgs e)
        {
            InitDictionaryOutput();
        }

        MenuItem mnuDictionary = new MenuItem("Dictionary");
        public void cMnu_PopUp(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu mnuSender = (System.Windows.Forms.ContextMenu)sender;
            RichTextBox rtxSender = (RichTextBox)mnuSender.Tag;

            mnuDictionary.MenuItems.Clear();
            MenuItem mnuDictionary_Selection = new MenuItem("Selection", mnuDictionary_Selection_Click);
            mnuDictionary.MenuItems.Add(mnuDictionary_Selection);

            List<string> lstWords = new List<string>();

            lstWords.Add(classStringLibrary.getWordAtSelection(ref rtxSender));
            for (int intPanelCounter = 0; intPanelCounter < formDictionarySelection.instance.lstPnlSelector.Count; intPanelCounter++)
            {
                panelSelector pnlSelector = formDictionarySelection.instance.lstPnlSelector[intPanelCounter];
                for (int intDictionaryCounter = 0; intDictionaryCounter < pnlSelector.lstToggles.Count; intDictionaryCounter++)
                {
                    char chrKey = pnlSelector.FastKey;
                    panelSelector.classButtonArray cBtnArray = pnlSelector.lstToggles[intDictionaryCounter];
                    List<classSearchParameters> lstSP = Search_GetParameters(ref cBtnArray, lstWords);
                    if (lstSP.Count > 0)
                    {
                        classSearchParameters cSearchParameter = lstSP[0];
                        if (cSearchParameter.cDictionary != null)
                        {
                            MenuItem mnuNew = new MenuItem();
                            mnuNew.Text = cSearchParameter.cDictionary.Heading;
                            mnuNew.Click += mnuDictionary_click;
                            mnuNew.Tag = (object)lstSP;
                            mnuDictionary.MenuItems.Add(mnuNew);
                        }
                    }
                }
                rtxCK.rtx.ContextMenu.MenuItems.Add(mnuDictionary);
            }

            MenuItem mnuQuickInsertWord = new MenuItem("Quick Insert Word", mnuQuickInsertWord_click);
            rtxCK.rtx.ContextMenu.MenuItems.Add(mnuQuickInsertWord);

            MenuItem mnuOptions = new MenuItem("Options", mnuOptions_Click);
            rtxCK.rtx.ContextMenu.MenuItems.Add(mnuOptions);

            MenuItem mnuMovitationalQuote = new MenuItem("Motivational Quote", mnuMotivationalQuote_Click);
            rtxCK.rtx.ContextMenu.MenuItems.Add(mnuMovitationalQuote);

            if (false && Debug)
            {
                MenuItem mnuDebug = new MenuItem("trigger BackUp timer event", mnuDebug_Click);
                rtxCK.rtx.ContextMenu.MenuItems.Add(mnuDebug);
            }

        }

        void mnuDebug_Click(object sender, EventArgs e)
        {
            cBackUp.tmrBackup_Tick((object)cBackUp, e);
        }


        void mnuQuickInsertWord_click(object sender, EventArgs e)
        {
            if (rtxCK == null) return;
            string strWordUnderCursor = rtxCK.WordUnderCursor();
            panelDictionaryOutput.instance.mbpDefPort_ButtonList_Insert(strWordUnderCursor, "NULL");
            panelDictionaryOutput.mbpDevNav_ButtonList.placeButtons();
        }

        void mnuMotivationalQuote_Click(object sender, EventArgs e)
        {
            new groupboxMotivationalQuote();
        }


        void mnuOptions_Click(object sender, EventArgs e)
        {
            grbOptions.Visible = !grbOptions.Visible;
            grbOptions.BringToFront();
        }

        void mnuDictionary_click(object sender, EventArgs e)
        {
            MenuItem mnuSender = (MenuItem)sender;
            List<classSearchParameters> lstSearchParameters_New = (List<classSearchParameters>)mnuSender.Tag;
            Search(ref lstSearchParameters_New, enuSearchRequestor.definition_Click);
        }

        private void splSub_A_SplitterMoved(object sender, SplitterEventArgs e)
        {
            placeObjects();
            if (pictureboxWordAnalyzer.instance != null && pictureboxWordAnalyzer.instance.Visible)
                pictureboxWordAnalyzer.instance.Draw();
        }

        public void Selection_Last_Set()
        {
            if (RTX_Focused != null)
            {
                rtxSelectionLast_Start = RTX_Focused.SelectionStart;
                rtxSelectionLast_Length = RTX_Focused.SelectionLength;
            }
            
        }

        int _intrtxSelectionLast_Start = -1;
        public int rtxSelectionLast_Start
        {
            get { return _intrtxSelectionLast_Start; }
            set { _intrtxSelectionLast_Start = value; }
        }

        int _intrtxSelectionLast_Length = -1;
        public int rtxSelectionLast_Length
        {
            get { return _intrtxSelectionLast_Length; }
            set { _intrtxSelectionLast_Length = value; }
        }


        private void TmrRtxSelectionStart_Tick(object sender, EventArgs e)
        {
            tmrRtxSelectionStart.Enabled = false;
            //rtxSelectionLast_Start = RTX_Focused.SelectionStart;
            Selection_Last_Set();
        }

        void tmrRtxSelectionSTart_Reset()
        {
            tmrRtxSelectionStart.Enabled = false;
            tmrRtxSelectionStart.Enabled = true;
        }

        public void Rtx_SelectionChanged(object sender, EventArgs e)
        {
            RTX_Focused = (RichTextBox)sender;
            tmrRtxSelectionSTart_Reset();
            tmrHeadingList_Reset();
            tmrWordAnalyzer_Reset();

        }


        public void tmrHeadingList_Reset()
        {
            tmrHeadingList.Enabled = false;
            tmrHeadingList.Enabled = grbHeadingList.Visible;
        }

        private void TmrHeadingList_Tick(object sender, EventArgs e)
        {
            Timer tmrSender = (Timer)sender;
            tmrSender.Enabled = false;
            if (grbHeadingList.Visible)
            {
                RichTextBox rtxTemp = RTX_Focused;
                string strWordUnderCursor = classStringLibrary.getWordAtSelection(ref rtxTemp).Trim();
                if (strWordUnderCursor.Length > 0)
                {
                    grbHeadingList.pnlDefPort.Reset();
                    grbHeadingList.JumpTo(strWordUnderCursor);
                }
            }

            pnlDictionaryOutput.btnWordUnderMouse_Capitalize_SetText();
        }


        public void tmrWordAnalyzerDraw_Reset()
        {
            groupboxWordAnalyzer.instance.tmrDraw_Reset();
        }

        public void tmrWordAnalyzer_Reset()
        {
            tmrWordAnalyzer.Enabled = false;
            tmrWordAnalyzer.Enabled = true;
        }

        
        string WordAnalyzer_WordSelected
        {
            get { return groupboxWordAnalyzer.instance!= null ? groupboxWordAnalyzer.instance.WordSelected : ""; }
            set 
            {
                if (groupboxWordAnalyzer.instance != null)
                    groupboxWordAnalyzer.instance.WordSelected = value; 
            }
        }


        private void TmrWordAnalyzer_Tick(object sender, EventArgs e)
        {
           Timer tmrSender = (Timer)sender;
            tmrSender.Enabled = false;
            if (grbNotes.grbWordAnalyzer.Visible)
            {
                if (bolTextChanged)
                {
                    bolTextChanged = false;
                    grbNotes.grbWordAnalyzer.Analyze();
                }

                RichTextBox rtxTemp = RTX_Focused;// rtxCK.rtx;
                groupboxWordAnalyzer.instance.WordSelected = classStringLibrary.getWordAtSelection(ref rtxTemp).Trim();

                if (WordAnalyzer_WordSelected.Length > 0)
                {

                    grbNotes.grbWordAnalyzer.DisplayWord(WordAnalyzer_WordSelected);
                    if (grbNotes.grbWordAnalyzer.WordOutline_Toggle)
                        grbNotes.grbWordAnalyzer.mbpWordAnalyzer.UnderlineSelectedWord();
                }
            }
        }

        private void FormWords_FormClosing(object sender, FormClosingEventArgs e)
        {
            TextChangedAndNotSaved(MessageBoxButtons.YesNo);
            if (classHeadingList.instance != null && classHeadingList.instance.bck_BuildHeadingList.IsBusy)
                classHeadingList.instance.bck_BuildHeadingList.CancelAsync();

            panelDictionaryOutput.mbpWordList_Save();
        }

        public static void Message(string strMessageNew) { Message(strMessageNew, classMessage.enuTarget.Main); }
        public static void Message(string strMessageNew, classMessage.enuTarget eTarget)
        {
            lstMessages.Add(new classMessage(strMessageNew, eTarget));
            if (!instance.tmrMessage.Enabled)
                instance.TmrMessage_Tick((object)instance.tmrMessage, new EventArgs());
        }

        public class classMessage
        {
            static enuTarget _eTargetLast = enuTarget.Main;
            public static enuTarget eTargetLast
            {
                get { return _eTargetLast; }
                set { _eTargetLast = value; }
            }
            public enum enuTarget { Main, Options, _num };
            public enuTarget eTarget = enuTarget.Main;
            public string Text = "";
            public classMessage(string strMessage) { Text = strMessage; }
            public classMessage(string strMessage, enuTarget eTarget)
            {
                Text = strMessage;
                this.eTarget = eTarget;
            }
        }

        static List<classMessage> lstMessages = new List<classMessage>();
        private void TmrMessage_Tick(object sender, EventArgs e)
        {
            if (lstMessages.Count > 0)
            {
                classMessage cMsg = lstMessages[0];
                classMessage.eTargetLast = cMsg.eTarget;
                lstMessages.RemoveAt(0);
                switch (cMsg.eTarget)
                {
                    case classMessage.enuTarget.Main:
                        Text = cMsg.Text;
                        break;

                    case classMessage.enuTarget.Options:
                        grbOptions.Text = cMsg.Text;
                        break;
                }
                tmrMessage.Enabled = true;
            }
            else
            {
                switch (classMessage.eTargetLast)
                {
                    case classMessage.enuTarget.Main:
                        Text = Title;
                        break;

                    case classMessage.enuTarget.Options:
                        grbOptions.Text = "";
                        break;
                }
                tmrMessage.Enabled = false;
            }
        }

        private void splMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            placeObjects();
            grbNotePilot.LocationSet();
        }

        static RichTextBox _RTX_Focused = null;
        public static RichTextBox RTX_Focused
        {
            get
            {
                if (_RTX_Focused == null) return instance.rtxCK.rtx;
                return _RTX_Focused;
            }
            set
            {
                if (_RTX_Focused != value)
                {
                    _RTX_Focused = value;

                    if (_RTX_Focused != null && _RTX_Focused.Parent != null)
                        ck_RichTextBox.instance = (ck_RichTextBox)_RTX_Focused.Parent;

                    instance.Title_Draw();
                }
            }
        }

        string strFilename = "";
        public string Filename
        {
            get
            {
                return strFilename;
            }
        }

        public string Path
        {
            get
            {
                string strPath = System.IO.Path.GetFullPath(PathAndFilename);
                strFilename = System.IO.Path.GetFileName(PathAndFilename);
                string strRetVal = strPath.Substring(0, strPath.Length - strFilename.Length);
                return strRetVal;
            }
        }

        public string PathAndFilename
        {
            get
            {
                return cProject.FilePath;
            }
            set
            {
                cProject.FilePath = value;
                strFilename = "";
                if (value.ToUpper().Contains("RTF"))
                    MessageBox.Show("RTF FILEN");
                Text = Title;
            }
        }


        public string ProjectFileName
        {
            get
            {

                string strFilename = System.IO.Path.GetFileName(PathAndFilename);
                string strExtension = System.IO.Path.GetExtension(PathAndFilename);

                return strFilename.Substring(0, strFilename.Length - strExtension.Length);// + ":" + cProject.Title_Current;
            }
        }

        public string Title
        {
            get
            {
                if (grbNotes != null && grbNotes.MainScreen && cProject.cEdit_Main != null)
                    return cProject.cEdit_Main.Heading;
                else
                    return ProjectFileName + " : " + cProject.Title_Current;
            }
        }

        public void Title_Draw()
        {
            Text = Title;
        }

        static classProject _cProject = new classProject();
        public static classProject cProject
        {
            get { return _cProject; }
            set { _cProject = value; }
        }


        form_Busy frmBusy = null;
        List<classSearchResult> lstSearchResults = new List<classSearchResult>();
        public List<classSearchParameters> lstSearchParameters;
        public enuSearchRequestor eSearchRequestor = enuSearchRequestor._num;
        public void Search(ref List<classSearchParameters> _lstSearchParameters, enuSearchRequestor _eSearchRequestor)
        {
            if (bckSearch.IsBusy) return;
            lstSearchParameters = _lstSearchParameters;
            if (lstSearchParameters == null || lstSearchParameters.Count == 0)
                return;

            lstSearchParameters = _lstSearchParameters;
            eSearchRequestor = _eSearchRequestor;

            if (frmBusy == null)
            {
                frmBusy = new form_Busy();
                frmBusy.Location = new Point(10, 10);
                frmBusy.ForeColor = Color.White;
                frmBusy.Font = new Font("roman", 12, FontStyle.Bold);
                ContextMenu cmuBusy = new ContextMenu();
                MenuItem mnuAbortSearch = new MenuItem("Abort Search", frmBusy_AbortSearch_Click);
                cmuBusy.MenuItems.Add(mnuAbortSearch);
                frmBusy.ContextMenu = cmuBusy;
            }

            frmBusy.Show();
            frmBusy.Start();
            bckSearch.RunWorkerAsync();
        }

        void frmBusy_AbortSearch_Click(object sender, EventArgs e)
        {
            bckSearch.CancelAsync();
        }

        private void BckSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setSearchProgress(enuSearchProgress.Display);
            if (panelDictionaryOutput.instance == null)
                new panelDictionaryOutput();
            if (lstSearchResults.Count == 0)
            {
                Message("no results found for :" + (lstSearchParameters.Count > 0 && lstSearchParameters[0].lstWords.Count > 0
                                                                    ? lstSearchParameters[0].lstWords[0]
                                                                    : "NULL"));

                goto quit;
            }

            switch (eSearchRequestor)
            {
                case enuSearchRequestor.PopUp:
                case enuSearchRequestor.definition_Hover:
                    {
                        panelDictionaryOutput.instance.lstHoverSearchResults = lstSearchResults;
                    }
                    break;


                case enuSearchRequestor.DefinitionPort_Heading_Click:
                case enuSearchRequestor.DefinitionPort_Click:
                    {
                        panelDictionaryOutput.instance.lstSearchResults_Main = lstSearchResults;
                    }
                    break;

                case enuSearchRequestor.definition_Click:
                case enuSearchRequestor.main:
                    {
                        string strWordFound = strClickedWord;
                        string strTemp = strClickedWord.ToUpper();

                        if (classBT_Dictionaries.Prefix.Length > 0 && strTemp.Length > classBT_Dictionaries.Prefix.Length)
                        {
                            if (string.Compare(strTemp.Substring(0, classBT_Dictionaries.Prefix.Length), classBT_Dictionaries.Prefix.ToUpper()) == 0)
                            {
                                strWordFound = strWordFound.Substring(classBT_Dictionaries.Prefix.Length);
                            }
                        }

                        if (classBT_Dictionaries.Suffix.Length > 0 && strTemp.Length > classBT_Dictionaries.Suffix.Length)
                        {
                            if (string.Compare(strTemp.Substring(strTemp.Length - classBT_Dictionaries.Suffix.Length), classBT_Dictionaries.Suffix.ToUpper()) == 0)
                            {
                                strWordFound = strWordFound.Substring(0, strWordFound.Length - classBT_Dictionaries.Suffix.Length);
                            }
                        }


                        DictionaryOutput(lstSearchResults,
                               "Search (" + lstSearchResults.Count.ToString() + ") : "
                               + (classBT_Dictionaries.Prefix.Length > 0
                                                              ? (classBT_Dictionaries.Prefix + " + ")
                                                              : "")
                               + strWordFound
                               + (classBT_Dictionaries.Suffix.Length > 0
                                                              ? (" + " + classBT_Dictionaries.Suffix)
                                                              : "")
                               );
                    }
                    break;

                case enuSearchRequestor.clipboard:
                    {
                        formDictionaryOutput.lstSearchResults = lstSearchResults;
                    }
                    break;


            }
            quit:
            panelDictionaryOutput.rtxCalling = null;

            frmBusy.Hide();
            frmBusy.Stop();
        }


        private void BckSearch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > (int)enuSearchProgress.idle && e.ProgressPercentage <= (int)enuSearchProgress.Display)
                setSearchProgress((enuSearchProgress)e.ProgressPercentage);
        }

        void setSearchProgress(enuSearchProgress eSearchProgress)
        {
            frmBusy.Text = eSearchProgress.ToString();
            Color[] clr = { Color.White, Color.Red, Color.Yellow, Color.Green, Color.Purple };
            frmBusy.ForeColor = clr[(int)eSearchProgress];
        }

        public enum enuSearchProgress { idle, Search, Sort, Display, _num };
        public enuSearchProgress eSearchProgress = enuSearchProgress.idle;

        private void BckSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            lstSearchResults = new List<classSearchResult>();
            string strSearchText = "";
            if (lstSearchParameters.Count > 0)
                for (int intCounter = 0; intCounter < lstSearchParameters[0].lstWords.Count; intCounter++)
                    strSearchText += (intCounter > 0 ? " " : "") + lstSearchParameters[0].lstWords[intCounter].Trim();
            panelDictionaryOutput.SearchText = StringLibrary.classStringLibrary.clean_nonAlpha_Ends(strSearchText);

            bckSearch.ReportProgress((int)enuSearchProgress.Search);

            lstSearchResults.Clear();
            List<string> lstFileNames = new List<string>();

            List<classSearchResult> lstBestResults = new List<classSearchResult>();
            List<classSearchResult> lstResults = new List<classSearchResult>();

            for (int intParameterCounter = 0; intParameterCounter < lstSearchParameters.Count; intParameterCounter++)
            {
                List<List<class_LL_Record>> lstLL = new List<List<class_LL_Record>>();
                classSearchParameters cParameters = lstSearchParameters[intParameterCounter];
                List<string> lstWords = cParameters.lstWords;
                classDictionary cDictionary = cParameters.cDictionary;

                int intBoolean = 0, intAltBoolean = 1;

                List<enuFileDataTypes> lstDataType = new List<enuFileDataTypes>();
                switch (cParameters.eSearchType)
                {
                    case enuSearchType.Content:
                        {
                            lstDataType.Add(enuFileDataTypes.content);
                        }
                        break;

                    case enuSearchType.Heading:
                        {
                            lstDataType.Add(enuFileDataTypes.heading);
                        }
                        break;

                    case enuSearchType.Both_Heading_And_Content:
                        {
                            lstDataType.Add(enuFileDataTypes.heading);
                            lstDataType.Add(enuFileDataTypes.content);
                        }
                        break;
                }

                for (int intDataTypeCounter = 0; intDataTypeCounter < lstDataType.Count; intDataTypeCounter++)
                {
                    if (bckSearch.CancellationPending)
                    {
                        lstSearchResults.Clear();
                        return;
                    }
                    for (int intWordCounter = 0; intWordCounter < lstWords.Count; intWordCounter++)
                    {
                        List<class_LL_Record> lstLL_Intermediate = classBT_Dictionaries.BinTree_Search(ref cDictionary, lstWords[intWordCounter], lstDataType[intDataTypeCounter]);
                        if (lstLL_Intermediate != null && lstLL_Intermediate.Count > 0)
                        {
                            lstLL.Add(lstLL_Intermediate);
                        }
                    }

                    if (lstLL.Count == 1)
                    {
                        for (int intResCounter = 0; intResCounter < lstLL[0].Count; intResCounter++)
                        {
                            class_LL_Record cLL = lstLL[0][intResCounter];
                            if (!lstFileNames.Contains(cLL.FileName))
                            {
                                classSearchResult cRes = new classSearchResult(ref cDictionary, ref cLL, classBT_Dictionaries.Prefix, classBT_Dictionaries.Suffix);
                                cRes.WordSearched = strSearchText;
                                lstSearchResults.Add(cRes);
                                lstFileNames.Add(cLL.FileName);
                            }
                        }
                    }

                    //if (lstLL.Count >= 1)
                    if (lstLL.Count > 1)
                    {
                        IEnumerable<List<class_LL_Record>> queryLLResults = lstLL.OrderBy(lst => lst.Count);
                        lstLL = (List<List<class_LL_Record>>)queryLLResults.ToList<List<class_LL_Record>>();

                        AlphaTrees.cAlphaTree[] cAT = new AlphaTrees.cAlphaTree[2];


                        List<class_LL_Record> lstLLRec = lstLL[0];
                        lstLL.Remove(lstLL[0]);

                        cAT[intBoolean] = new AlphaTrees.cAlphaTree();
                        while (lstLLRec.Count > 0)
                        {
                            int intRND = Math3.classRND.Get_Int(0, lstLLRec.Count - 1);
                            class_LL_Record cLL = lstLLRec[intRND];
                            lstLLRec.Remove(cLL);
                            if (!lstFileNames.Contains(cLL.FileName))
                            {

                                object objLL = (object)cLL;
                                cAT[intBoolean].Insert(ref objLL, cLL.FileName);

                                lstFileNames.Add(cLL.FileName);
                            }
                        }

                        for (int intListCounter = 0; intListCounter < lstLL.Count; intListCounter++)
                        {
                            List<class_LL_Record> lst = lstLL[intListCounter];
                            cAT[intAltBoolean] = new AlphaTrees.cAlphaTree();

                            while (lst.Count > 0)
                            {
                                int intRND = Math3.classRND.Get_Int(0, lst.Count - 1);
                                class_LL_Record cLL = lst[intRND];
                                lst.Remove(cLL);
                                if (cAT[intBoolean].Search(cLL.FileName) != null)
                                {
                                    object objLL = (object)cLL;
                                    cAT[intAltBoolean].Insert(ref objLL, cLL.FileName);
                                }
                            }
                            intBoolean = intAltBoolean;
                            intAltBoolean = (intBoolean + 1) % 2;
                        }

                        if (cAT[intAltBoolean] != null)
                        {
                            List<AlphaTrees.cAlphaTree.classTraversalReport_Record> lstObjResults = cAT[intBoolean].TraverseTree_InOrder();
                            for (int intResCounter = 0; intResCounter < lstObjResults.Count; intResCounter++)
                            {
                                object objLL = (object)lstObjResults[intResCounter].data;
                                class_LL_Record cLL = (class_LL_Record)objLL;
                                classSearchResult cRes = new classSearchResult(ref cDictionary, ref cLL, classBT_Dictionaries.Prefix, classBT_Dictionaries.Suffix);
                                cRes.WordSearched = strSearchText;
                                lstSearchResults.Add(cRes);
                            }
                        }
                    }
                }
                if (lstDataType.Count > 0)
                    bckSearch.ReportProgress((int)enuSearchProgress.Sort);

                ///*
                IEnumerable<classSearchResult> query = lstSearchResults.OrderBy(result => result.Find);

                lstBestResults = (List<classSearchResult>)query.ToList<classSearchResult>();
                /*/

                for (int intCounter = 0; intCounter < lstSearchResults.Count; intCounter++)
                {
                    if (bckSearch.CancellationPending)
                    {
                        lstSearchResults.Clear();
                        return; 
                    }
                    classSearchResult cResult = lstSearchResults[intCounter];
                    cResult.cFileContent = new classFileContent(cResult.cDictionary.strSourceDirectory, cResult.strFileName);
                    string strHeading = classStringLibrary.cleanFront_nonAlpha(cResult.cFileContent.Heading.Trim());
                    if (string.Compare(strHeading, strSearchText) == 0)
                        lstBestResults.Insert(0, cResult);
                    else if (string.Compare(strHeading.ToLower(), strSearchText.ToLower()) == 0)
                        lstBestResults.Add(cResult);
                    else
                        lstResults.Add(cResult);
                }
                IEnumerable<classSearchResult> query = lstBestResults.OrderBy(result => result.cFileContent.Heading);
                lstBestResults = (List<classSearchResult>)query.ToList<classSearchResult>();
                //*/

            }
            lstSearchResults.Clear();

            lstSearchResults.AddRange(lstBestResults);
            lstSearchResults.AddRange(lstResults);
            bckSearch.ReportProgress((int)enuSearchProgress.Display);

            return;
        }

        public void DictionaryOutput(List<classSearchResult> lstSearchResults, string strTitle)
        {
            if (pnlDictionaryOutput == null)
                initPanelDictionaryOutput();

            pnlDictionaryOutput.lstSearchResults_Main = lstSearchResults;
            pnlDictionaryOutput.Text = strTitle;
        }


        void initPanelDictionaryOutput()
        {
            pnlDictionaryOutput = new panelDictionaryOutput();
            pnlDictionaryOutput.Disposed += new EventHandler(panelDictionaryOutput_Disposed);
        }

        void panelDictionaryOutput_Disposed(object sender, EventArgs e)
        {
            if (bolDispose)
                return;
            initPanelDictionaryOutput();
        }

        public List<classSearchParameters> Search_GetParameters(ref panelSelector.classButtonArray cBtnArray, List<string> lstWords)
        {
            List<classSearchParameters> lstRetVal = new List<classSearchParameters>();

            bool bolContent = false;
            bool bolHeading = false;

            switch (cBtnArray.eSearchType)
            {
                case enuSearchType.Both_Heading_And_Content:
                    bolContent
                        = bolHeading
                        = true;
                    break;

                case enuSearchType.Content:
                    bolContent = true;
                    break;

                case enuSearchType.Heading:
                    bolHeading = true;
                    break;
            }
            classDictionary cDictionary = cBtnArray.cDictionary;

            int intMaxWords = 4;
            if (lstWords.Count > intMaxWords)
                lstWords.RemoveRange(intMaxWords, lstWords.Count - intMaxWords);

            if (bolHeading && bolContent)
                lstRetVal.Add(new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Both_Heading_And_Content));
            else if (bolHeading)
                lstRetVal.Add(new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Heading));
            else if (bolContent)
                lstRetVal.Add(new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Content));

            return lstRetVal;
        }

        public string strClickedWord = "";

        void LBX_Scroll_Down()
        {
            if (panelDictionaryOutput.instance != null)
                panelDictionaryOutput.instance.lbxResults_ScrollDown();
        }

        void LBX_Scroll_Up()
        {
            if (panelDictionaryOutput.instance != null)
                panelDictionaryOutput.instance.lbxResults_ScrollUp();
        }

        public bool TwoWordSwap
        {
            get { return grbOptions.Toggle_Two_Word_Swap; }
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
        }

        public void rtxCK_KeyUp(object sender, KeyEventArgs e)
        {
            keyDownLast = Keys.Escape;
        }


        public void rtxCK_MouseDown(object sender, MouseEventArgs e)
        {
            RichTextBox rtxSender = (RichTextBox)sender;
      
            
            if (keyDownLast == (Keys.ControlKey | Keys.Control)
                            || e.Clicks < 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (TwoWordSwap)
                    {
                        int intIndexUnderMouse = rtxSender.GetCharIndexFromPosition(new Point(e.X, e.Y));
                        int intIndex_Selected = rtxSelectionLast_Start;

                        if (intIndexUnderMouse != intIndex_Selected)
                        {
                            int intIndex0 = intIndexUnderMouse < intIndex_Selected
                                                               ? intIndexUnderMouse
                                                               : intIndex_Selected;

                            int intIndex1 = intIndexUnderMouse > intIndex_Selected
                                                               ? intIndexUnderMouse
                                                               : intIndex_Selected;

                            int intStart_0 = 0, intEnd_0 = 0;
                            int intStart_1 = 0, intEnd_1 = 0;

                            if (rtxSelectionLast_Length > 0)
                            {
                                if (intIndex0 >= rtxSelectionLast_Start && intIndex0 < rtxSelectionLast_Start + rtxSelectionLast_Length)
                                {
                                    intStart_0 = rtxSelectionLast_Start;
                                    intEnd_0 = rtxSelectionLast_Start + rtxSelectionLast_Length;
                                    TwoWordSwap_FindWord(ref rtxSender, intIndex1, ref intStart_1, ref intEnd_1);
                                }
                                else
                                {
                                    TwoWordSwap_FindWord(ref rtxSender, intIndex0, ref intStart_0, ref intEnd_0);
                                    intStart_1 = rtxSelectionLast_Start;
                                    intEnd_1 = rtxSelectionLast_Start + rtxSelectionLast_Length;
                                }
                            }
                            else
                            {
                                TwoWordSwap_FindWord(ref rtxSender, intIndex0, ref intStart_0, ref intEnd_0);
                                TwoWordSwap_FindWord(ref rtxSender, intIndex1, ref intStart_1, ref intEnd_1);
                            }

                            if (intStart_0 <= intEnd_0 && intStart_0 >= 0
                                &&
                                intStart_1 <= intEnd_1 && intStart_1 >= 0)
                            {
                                ck_RichTextBox ckRTXSender = (ck_RichTextBox)rtxSender.Parent;
                                ckRTXSender.WordsSwap(intStart_0, intEnd_0, intStart_1, intEnd_1);
                            }
                        }
                    }
                }
            }

        }

        void TwoWordSwap_FindWord(ref RichTextBox rtx, int intIndex, ref int intStart, ref int intEnd)
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

        public void rtxCK_KeyDown(object sender, KeyEventArgs e)
        {
            RTX_Focused = (RichTextBox)sender;
            keyDownLast = e.KeyData;

            if (e.Modifiers == (Keys.Control | Keys.Alt))
            { //        Heading List Key controls
                e.SuppressKeyPress = true;
                switch (e.KeyCode)
                {
                    case Keys.A:
                        //WordHeading scroll up
                        grbHeadingList.Scroll_Up();
                        return;

                    case Keys.Z:
                        //WordHeading scroll down
                        grbHeadingList.Scroll_Down();
                        return;

                    case Keys.Q:
                        { // insert the HeadingList selected word into the text
                            if (grbHeadingList != null && grbHeadingList.cSelected != null)
                            {
                                ckRTX.Text_ReplaceWord(grbHeadingList.cSelected.strHeading);
                            }
                            e.SuppressKeyPress = true;
                        }
                        break;
                }
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift))

            {  // search results scroll commands
                //System.Diagnostics.Debug.Print(((char)e.KeyValue).ToString());
                switch (e.KeyCode)
                {
                    case Keys.A:
                        e.SuppressKeyPress = true;
                        LBX_Scroll_Up();
                        return;

                    case Keys.Z:
                        LBX_Scroll_Down();
                        e.SuppressKeyPress = true;
                        return;

                    case Keys.S:
                        e.SuppressKeyPress = true;
                        if (panelDictionaryOutput.instance != null && !panelDictionaryOutput.instance.IsDisposed)
                        {
                            panelDictionaryOutput.instance.DefinitionPortList_DeleteLast();
                            RTX_Focused.Focus();
                        }
                        return;

                    case Keys.X:
                        e.SuppressKeyPress = true;
                        if (panelDictionaryOutput.instance != null && !panelDictionaryOutput.instance.IsDisposed)
                        {
                            panelDictionaryOutput.instance.DefinitionPortList_ClearAll();
                            RTX_Focused.Focus();
                        }
                        return;

                    case Keys.R:
                        {
                            e.SuppressKeyPress = true;
                            RhymeDictionarySearch();
                        }
                        break;

                    default:
                        {
                            RichTextBox rtx = (RichTextBox)sender;
                            
                            strClickedWord = rtx.SelectionLength == 0
                                                                 ? (ckRTX != null ? ckRTX.WordUnderCursor(""): classStringLibrary.getClickedWord(ref rtx))
                                                                 : classStringLibrary.getWordAtSelection(ref rtx);

                            List<string> lstWords = classStringLibrary.getFirstWords(strClickedWord);
                            char chrKey = (char)e.KeyValue;

                            if (lstWords != null && lstWords.Count > 0)
                            {
                                RichTextBox rtxRef = (RichTextBox)sender;
                                Search(lstWords, ref rtxRef, chrKey);
                            }
                            tmrHeadingList_Reset();
                            tmrWordAnalyzer_Reset();
                        }
                        break;

                }
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift | Keys.Alt))
            {
               
            }
        }

        void RhymeDictionarySearch()
        {
            classDictionary cDictionary = null;
            class_LL_Record cLL = null;
            classSearchResult cSearchResult = new classSearchResult(ref cDictionary, ref cLL);
            cSearchResult.Find 
                = cSearchResult.WordSearched 
                = strClickedWord;

            panelDefinitionPort pnlDefPort = new panelDefinitionPort(ref cDictionary, ref cSearchResult);
            RhymeDictionarySearch(ref pnlDefPort, strClickedWord);
        }


        public void RhymeDictionarySearch(ref Words.panelDefinitionPort pnlDefPort)
        {
            if (formWords.RTX_Focused != null)
            {
                RichTextBox rtx = (RichTextBox)RTX_Focused;
                strClickedWord = rtx.SelectionLength == 0
                                                     ? classStringLibrary.getClickedWord(ref rtx)
                                                     : classStringLibrary.getWordAtSelection(ref rtx);
                RhymeDictionarySearch(ref pnlDefPort, strClickedWord);
            }
        }

        public void RhymeDictionarySearch(ref Words.panelDefinitionPort pnlDefPort, string strSearchWord)
        {
            List<List<string>> lstResults = classRhymeDictionary.Search(strSearchWord, (uint)grbOptions.Integer_Rhyme_Dictionary_MaxResults);
            if (lstResults.Count == 0) return;
            string strText = "";
            List<string> lstDropped = new List<string>();

            for (int intResultCounter = 0; intResultCounter < lstResults.Count; intResultCounter++)
            {
                List<string> lstWords = lstResults[intResultCounter];
                if (lstWords.Count < grbOptions.Integer_Rhyme_Dictionary_MaxResults)
                {
                    if (lstWords.Count > 0)
                    {
                        if (strText.Length > 0)
                            strText += "\r\n---------------\r\n";

                        if (lstWords.Count == 1)
                        {
                            if (lstWords[0].Length > 0 && lstWords[0][0] == '(')
                                lstDropped.Add(lstWords[0]);
                        }

                        for (int intWordCounter = lstWords.Count - 1; intWordCounter >= 0; intWordCounter--)
                        {
                            if (intWordCounter < lstWords.Count - 1)
                                strText += ", ";

                            strText += lstWords[intWordCounter];
                        }
                    }
                }
            }
            if (!pnlDefPort.RhymeDictionary)
                panelDictionaryOutput.instance.DefinitionPort_InsertNew(ref pnlDefPort);

            pnlDefPort.RhymeDictionary = true;
            pnlDefPort.Heading_Set();
            pnlDefPort.rtx.Text = strText;
            pnlDefPort.cSearchResult.Find = strSearchWord;            
            pnlDefPort.rtx.SelectAll();
            pnlDefPort.rtx.SelectionFont = new Font("Arial", 10);

            for (int intDroppedCounter = 0; intDroppedCounter < lstDropped.Count; intDroppedCounter++)
            {
                string strDropped = lstDropped[intDroppedCounter];
                int intFind = pnlDefPort.rtx.Text.IndexOf(strDropped);
                while (intFind >= 0)
                {
                    pnlDefPort.rtx.Select(intFind, strDropped.Length);
                    pnlDefPort.rtx.SelectionBackColor = Color.LightGray;
                    intFind = pnlDefPort.rtx.Text.IndexOf(strDropped, intFind + 1);
                }
            }

            pnlDefPort.rtx.Select(0, 0);
            pnlDefPort.rtx.ScrollToCaret();

        }

        public void Search(List<string> lstWords, ref RichTextBox rtxCalling, char chrKey)
        {
            List<classSearchParameters> lstSearchParameters_New = new List<classSearchParameters>();
            for (int intPanelCounter = 0; intPanelCounter < formDictionarySelection.instance.lstPnlSelector.Count; intPanelCounter++)
            {
                panelSelector pnlSelector = formDictionarySelection.instance.lstPnlSelector[intPanelCounter];
                for (int intDictionaryCounter = 0; intDictionaryCounter < pnlSelector.lstToggles.Count; intDictionaryCounter++)
                {
                    if (pnlSelector.FastKey == chrKey)
                    {
                        panelSelector.classButtonArray cBtnArray = pnlSelector.lstToggles[intDictionaryCounter];
                        List<classSearchParameters> lstSP = Search_GetParameters(ref cBtnArray, lstWords);
                        if (lstSP != null && lstSP.Count > 0)
                        {
                            lstSearchParameters_New.AddRange(lstSP);
                        }
                    }
                }
            }

            if (lstSearchParameters_New.Count > 0)
            {
                panelDictionaryOutput.rtxCalling = rtxCalling;
                /*
                enuRTX eRTX = panelDictionaryOutput.RTX_getID(ref rtxCalling);
                switch (eRTX)
                {
                    case enuRTX.Main:
                        Search(ref lstSearchParameters_New, eSearchRequestor);
                        break;


                    default:
                        MessageBox.Show("unhandled search");
                        break;
                }
                Search(ref lstSearchParameters, eSearchRequestor);
                /*/
                enuSearchRequestor eSearchRequestor = (enuSearchRequestor)rtxCalling.Tag;
                switch (eSearchRequestor)
                {
                    case enuSearchRequestor.main:
                    case enuSearchRequestor.definition_Hover:
                        Search(ref lstSearchParameters_New, eSearchRequestor);
                        break;


                    default:
                        MessageBox.Show("unhandled search");
                        break;
                }
                Search(ref lstSearchParameters, eSearchRequestor);
                // */
            }
        }

        /// <summary>
        /// classToolTip allows multiple tips to be assigned to each control
        /// </summary>
        public classToolTip cTTips = new classToolTip();

        /// <summary>
        /// manages all tool tips in SpriteEditor_2017
        /// </summary>
        public ToolTip toolTip = new ToolTip();

        /// <summary>
        /// initializes an instance of classToolTip with all the tips for each control
        /// </summary>
        public void ToolTip_SetUp()
        {
            cTTips.Tip_Clear_All();

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 100;
            toolTip.UseAnimation = true;
            toolTip.Popup += ToolTip_Popup;

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            #region Word
            // 
            string[] strTips = {
                "",
                ""};

            #endregion

            #region Notes
            toolTip.SetToolTip(grbNotes.pnlNotes, formWords.conStandardToolTip);
            toolTip.SetToolTip(grbNotes.rtxNotes, formWords.conStandardToolTip);

            // list - notes
            strTips = new string[] {
                "you can select the note you wish to view or edit here",
                "move any notes up or down in this list by first highlighting them and then pressing the up or down arrows",
                "these notes can be moved using the up/down arrows",
                "you can scroll the mouse to move these notes up or down"};
            for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                cTTips.Tip_set(grbNotes.pnlNotes.Name, strTips[intCounter]);

            // textbox note
            strTips = new string[]{
                "type your notes in this text box",
                "these notes are saved automatically when you save your project",
                "typing the '#' key here will insert a numeric entry for a list in this note",
                "pressing the '#' key inside the numeric bullet will renumber your list to keep it in order",
                "you can erase a numeric-list entry or cut/paste them into a different order then reset them by typing '#' inside a numeric bullet"};
            for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                cTTips.Tip_set(grbNotes.rtxNotes.Name, strTips[intCounter]);
            #endregion

            #region DictionarySelector 
            if (formDictionarySelection.instance != null)
            {
                formDictionarySelection.instance.lbtnAdd.Name = "formDictionarySelection_lbtnAdd";
                formDictionarySelection.instance.lbtnHide.Name = "formDictionarySelection_lbtnHide";
                toolTip.SetToolTip(formDictionarySelection.instance.lbtnAdd, formWords.conStandardToolTip);
                toolTip.SetToolTip(formDictionarySelection.instance.lbtnHide, formWords.conStandardToolTip);

                // button add note
                strTips = new string[]{
                "press this button to create a new dictionary selection",
                "you can program the Words app to search dictionary combinations by clicking here"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("formDictionarySelection_lbtnAdd", strTips[intCounter]);

                // button Hide note
                strTips = new string[]{
                "hide this form by clicking this button",
                "clicking this button will hide this form - use the Options menu in the main form to bring it back "};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("formDictionarySelection_lbtnHide", strTips[intCounter]);

                // btn delete
                strTips = new string[]{
                "press this button to delete this dictionary selection",
                "clicking here will magically make this box disappear"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_btnDelete", strTips[intCounter]);

                // label fastkey note
                strTips = new string[]{
                "pressing the control-key combination described here will summon the results of this dictionary selection's search",
                "you can program your Words app to search dictionary entries by setting this control-key combination"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_lblFastKey", strTips[intCounter]);

                // textbox FastKey note
                strTips = new string[]{
                "enter the letter that will combine with the Control-key to make a quick dictionary search",
                "you can program the Words App to search the dictionaries selected in this list by typing a letter here"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_txtFastKey", strTips[intCounter]);

                // label cursor note
                strTips = new string[]{
                "i have no idea what this label cursor is supposed to do",
                "this is the label cursor figure it out"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_lblCursor", strTips[intCounter]);

                // panel scrolling note
                strTips = new string[]{
                "the scrolling panel lists something something that i forget",
                "scrolling panel note - write it in later"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_pnlScrolling", strTips[intCounter]);

                // checkbox UseClipBoard note
                strTips = new string[]{
                "by selecting this option you tell the Words App to search words you 'copy' using MS Copy/Cut options",
                "you can search your selected dictionaries for words which are copied to the MS clipboard by highlighting text and pressing Ctrl-C or Ctrol-X"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_chkUseClipBoard", strTips[intCounter]);

                // (Dictionary) cSelector note
                strTips = new string[]{
                    "this list of dictionaries allows your to select any combination of dictionaries to search with a single control-key combination",
                    "you can select any combination of dictionaries to help you find what you're looking for" };
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("panelSelector_cSelector", strTips[intCounter]);

                // "classRDBArray_lbl"
                strTips = new string[]{
                "you can select any of the four options for this particular dictionary : do-not-search, search heading only, search-content only, search both heading/content",
                "you can set your search preference for each dictionary independently using the radio buttons on the left of the dictionary names listed here"};
                for (int intCounter = 0; intCounter < strTips.Length; intCounter++)
                    cTTips.Tip_set("classRDBArray_lbl", strTips[intCounter]);

            }
            #endregion 
        }


        /// <summary>
        /// prevents tool tip from giving the same control a tip every mousemove over that control
        /// </summary>
        object objAssociatedControl = null;
        /// <summary>
        /// stops edits tool tip with the multiple tips available for a given control
        /// </summary>
        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (e.AssociatedControl == objAssociatedControl
                || e.AssociatedControl == null
                || e.AssociatedControl.Name.Length == 0) return;
            objAssociatedControl = e.AssociatedControl;
            string strName = e.AssociatedControl.Name;
            string strTip = cTTips.Tip_get(strName);

            if (strTip.Length > 0)
                toolTip.SetToolTip(e.AssociatedControl, strTip);
        }
        void formWords_VisibleChanged(object sender, EventArgs e)
        {
            tmrPosition.Enabled = Visible;
        }

        void tmrPosition_Tick(object sender, EventArgs e)
        {
            tmrPosition.Enabled = false;
            Size = sz;
            Location = loc;
            bolInit = true;
        }

        void formWords_Disposed(object sender, EventArgs e)
        {
            if (classBT_Dictionaries.bck_BuildBinSearch != null
                && classBT_Dictionaries.bck_BuildBinSearch.IsBusy)
            {
                classBT_Dictionaries.bck_BuildBinSearch.CancelAsync();
            }

            if (formDictionarySelection.instance != null
                &&
                !formDictionarySelection.instance.IsDisposed)
                formDictionarySelection.instance.DictionarySelections_Save();

            cProject.Save();
            grbNotes.bolDie = true;
            Forms_Save();
        }




        public void mnuDictionary_Selection_Click(object sender, EventArgs e)
        {
            if (formDictionarySelection.instance == null || formDictionarySelection.instance.IsDisposed)
                new formDictionarySelection();
            formDictionarySelection.instance.Show();
        }


        string GetFormsFileName()
        {
            return System.IO.Directory.GetCurrentDirectory() + "\\forms_20210816.bfs";
        }

        public void Forms_Save()
        {
            string strResourcesDirectoryAndFileName = GetFormsFileName();
            FileStream fs;
            if (System.IO.File.Exists(strResourcesDirectoryAndFileName))
                fs = new FileStream(strResourcesDirectoryAndFileName, FileMode.Open);
            else
                fs = new FileStream(strResourcesDirectoryAndFileName, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            fs.Position = 0;
            if (PathAndFilename == null)
                PathAndFilename = "";
            formatter.Serialize(fs, PathAndFilename);

            formatter.Serialize(fs, sz.Width);
            formatter.Serialize(fs, sz.Height);
            formatter.Serialize(fs, loc.X);
            formatter.Serialize(fs, loc.Y);

            // groupbox_NotePilot Location + collapse-state
            formatter.Serialize(fs, groupboxNotePilot.instance.Collapsed);

            formatter.Serialize(fs, splMain.SplitterDistance);
            formatter.Serialize(fs, splSub_A.SplitterDistance);
            formatter.Serialize(fs, grbHeadingList.splMain_Distance);
            formatter.Serialize(fs, groupboxNotes.intSpc_distance);
            formatter.Serialize(fs, splSub.SplitterDistance);
            formatter.Serialize(fs, grbNotes.splMain.SplitterDistance);
            formatter.Serialize(fs, grbNotes.splSub.SplitterDistance);

            formatter.Serialize(fs, formFindReplace.loc.X);
            formatter.Serialize(fs, formFindReplace.loc.Y);

            formatter.Serialize(fs, (bool)rtxCK.HighlighterLabel_Hide_Automatically);
            formatter.Serialize(fs, (bool)grbNotes.rtxNotes.HighlighterLabel_Hide_Automatically);

            formatter.Serialize(fs, rtxCK.rtx.BackColor.A);
            formatter.Serialize(fs, rtxCK.rtx.BackColor.R);
            formatter.Serialize(fs, rtxCK.rtx.BackColor.G);
            formatter.Serialize(fs, rtxCK.rtx.BackColor.B);

            formatter.Serialize(fs, grbNotes.rtxNotes.rtx.BackColor.A);
            formatter.Serialize(fs, grbNotes.rtxNotes.rtx.BackColor.R);
            formatter.Serialize(fs, grbNotes.rtxNotes.rtx.BackColor.G);
            formatter.Serialize(fs, grbNotes.rtxNotes.rtx.BackColor.B);

            formatter.Serialize(fs, grbNotes.pnlNotes.POV_Highlight);

            formatter.Serialize(fs, grbNotes.Locked);

            formatter.Serialize(fs, grbNotes.grbWordAnalyzer.Collapsed);

            for (int intHighLighterColorCounter = 0; intHighLighterColorCounter < ck_RichTextBox.lstHighlighterItems.Count; intHighLighterColorCounter++)
            {
                ck_RichTextBox.classHighlighterColorItem cHighlighterColorItem = ck_RichTextBox.lstHighlighterItems[intHighLighterColorCounter];
                formatter.Serialize(fs, cHighlighterColorItem.clrBack.A);
                formatter.Serialize(fs, cHighlighterColorItem.clrBack.R);
                formatter.Serialize(fs, cHighlighterColorItem.clrBack.G);
                formatter.Serialize(fs, cHighlighterColorItem.clrBack.B);

                formatter.Serialize(fs, cHighlighterColorItem.clrFore.A);
                formatter.Serialize(fs, cHighlighterColorItem.clrFore.R);
                formatter.Serialize(fs, cHighlighterColorItem.clrFore.G);
                formatter.Serialize(fs, cHighlighterColorItem.clrFore.B);

                formatter.Serialize(fs, cHighlighterColorItem.Text);
                formatter.Serialize(fs, cHighlighterColorItem.valid);
            }

            formatter.Serialize(fs, ck_RichTextBox.classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI);
            formatter.Serialize(fs, ck_RichTextBox.classCopyCutter.Prefix);
            formatter.Serialize(fs, ck_RichTextBox.classCopyCutter.Postfix);

            formatter.Serialize(fs, groupboxHeadingList.DefaultDictionaryIndex);
            formatter.Serialize(fs, groupboxHeadingList.instance.pnlDefPort.rtx.ZoomFactor);

            formatter.Serialize(fs, panelDictionaryOutput.ZoomFactor);
            formatter.Serialize(fs, panelDefinitionPort.Height_Default);

            formatter.Serialize(fs, groupboxWordAnalyzer.WordMap);

            // Dictionary Output
            formatter.Serialize(fs, pnlDictionaryOutput.spt0.SplitterDistance);
            formatter.Serialize(fs, pnlDictionaryOutput.spt1.SplitterDistance);
            formatter.Serialize(fs, panelDefinitionPort.Height_Default);
            //formatter.Serialize(fs, pnlDictionaryOutput.mbpWordList_Save_GetString());

            // motivational quote appear at launch 
            formatter.Serialize(fs, groupboxMotivationalQuote.AppearAtLaunch);

            // backup tabs
            int[] intBackupTabs = classBackUp.arrWidths;
            if (intBackupTabs == null)
            {
                intBackupTabs = new int[5];// { 20, 200, 90, 90, 200 }
                intBackupTabs[0] = 20;
                intBackupTabs[1] = 120;
                intBackupTabs[2] = 90;
                intBackupTabs[3] = 90;
                intBackupTabs[4] = 90;
            }
            for (int intTabCounter = 0; intTabCounter < intBackupTabs.Length; intTabCounter++)
                formatter.Serialize(fs, intBackupTabs[intTabCounter]);

            fs.Close();
        }


        bool bolFormLoad_Notes_Locked = false;
        bool bolNotes_POV_Highlight = false;

        void Forms_Load()
        {
            FileStream fs;
            string PathAndFilenameAndDirectory = GetFormsFileName();
            if (System.IO.File.Exists(PathAndFilenameAndDirectory))
            {
                fs = new FileStream(PathAndFilenameAndDirectory, FileMode.Open);
                fs.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                /*
                //PathAndFilename =
                string strTemp = (string)formatter.Deserialize(fs);
                PathAndFilename = @"C:\Writer\Trading With the Enemy\Trading With the Enemy.WordsProject_1";
                /*/
                                PathAndFilename = (string)formatter.Deserialize(fs);
                // */
                sz.Width = (int)formatter.Deserialize(fs);
                sz.Height = (int)formatter.Deserialize(fs);
                loc.X = (int)formatter.Deserialize(fs);
                if (loc.X < 0) loc.X = 0;
                loc.Y = (int)formatter.Deserialize(fs);
                if (loc.Y < 0) loc.Y = 0;

                // Note Pilot
                groupboxNotePilot.bolCollapsed_Load = (bool)formatter.Deserialize(fs);

                intsplMainSplitterDistance = (int)formatter.Deserialize(fs);
                intsplSub_ASplitterDistance = (int)formatter.Deserialize(fs);
                intSplHeadingListSplitterDistance = (int)formatter.Deserialize(fs);
                groupboxNotes.intSpc_distance = (int)formatter.Deserialize(fs);
                intSplSubSplitterDistance = (int)formatter.Deserialize(fs);

                intSpl_GrbNotes_Main_SplitterDistance = (int)formatter.Deserialize(fs);
                intSpl_GrbNotes_Sub_SplitterDistance = (int)formatter.Deserialize(fs);

                formFindReplace.loc.X = (int)formatter.Deserialize(fs);
                if (formFindReplace.loc.X < 0) formFindReplace.loc.X = 0;
                formFindReplace.loc.Y = (int)formatter.Deserialize(fs);
                if (formFindReplace.loc.Y < 0) formFindReplace.loc.Y = 0;

                rtxCK.HighlighterLabel_Hide_Automatically = (bool)formatter.Deserialize(fs);
                grbNotes.rtxNotes.HighlighterLabel_Hide_Automatically = (bool)formatter.Deserialize(fs);

                byte bytA, bytR, bytG, bytB;

                bytA = (byte)formatter.Deserialize(fs);
                bytR = (byte)formatter.Deserialize(fs);
                bytG = (byte)formatter.Deserialize(fs);
                bytB = (byte)formatter.Deserialize(fs);
                rtxCK.rtx.BackColor = Color.FromArgb(bytA, bytR, bytG, bytB);

                bytA = (byte)formatter.Deserialize(fs);
                bytR = (byte)formatter.Deserialize(fs);
                bytG = (byte)formatter.Deserialize(fs);
                bytB = (byte)formatter.Deserialize(fs);
                grbNotes.rtxNotes.rtx.BackColor = Color.FromArgb(bytA, bytR, bytG, bytB);

                bolNotes_POV_Highlight = (bool)formatter.Deserialize(fs);

                bolFormLoad_Notes_Locked = (bool)formatter.Deserialize(fs);

                grbNotes.grbWordAnalyzer.Collapsed = (bool)formatter.Deserialize(fs);

                for (int intHighlighter_ColorCounter = 0; intHighlighter_ColorCounter < ck_RichTextBox.lstHighlighterItems.Count; intHighlighter_ColorCounter++)
                {
                    byte bytClrBackHighlight_A = (byte)formatter.Deserialize(fs);
                    byte bytClrBackHighlight_R = (byte)formatter.Deserialize(fs);
                    byte bytClrBackHighlight_G = (byte)formatter.Deserialize(fs);
                    byte bytClrBackHighlight_B = (byte)formatter.Deserialize(fs);

                    byte bytClrForeHighlight_A = (byte)formatter.Deserialize(fs);
                    byte bytClrForeHighlight_R = (byte)formatter.Deserialize(fs);
                    byte bytClrForeHighlight_G = (byte)formatter.Deserialize(fs);
                    byte bytClrForeHighlight_B = (byte)formatter.Deserialize(fs);

                    ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].clrBack
                        = ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].clrBack
                        = Color.FromArgb(bytClrBackHighlight_A, bytClrBackHighlight_R, bytClrBackHighlight_G, bytClrBackHighlight_B);

                    ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].clrFore
                        = ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].clrFore
                        = Color.FromArgb(bytClrForeHighlight_A, bytClrForeHighlight_R, bytClrForeHighlight_G, bytClrForeHighlight_B);

                    ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].Text
                        = ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].Text
                        = (string)formatter.Deserialize(fs);

                    ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].valid
                        = ck_RichTextBox.lstHighlighterItems[intHighlighter_ColorCounter].valid
                        = (bool)formatter.Deserialize(fs);
                }

                ck_RichTextBox.classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = (string)formatter.Deserialize(fs);
                ck_RichTextBox.classCopyCutter.Prefix = (string)formatter.Deserialize(fs);
                ck_RichTextBox.classCopyCutter.Postfix = (string)formatter.Deserialize(fs);

                groupboxHeadingList.DefaultDictionaryIndex = (int)formatter.Deserialize(fs);
                groupboxHeadingList.ZoomFactor = (Single)formatter.Deserialize(fs);
                panelDictionaryOutput.ZoomFactor = (Single)formatter.Deserialize(fs);

                panelDefinitionPort.Height_Default = (int)formatter.Deserialize(fs);

                groupboxWordAnalyzer.WordMap = (bool)formatter.Deserialize(fs);

                //  Dictionary Output
                intDictionaryOutput_spt0_splitterDistance = (int)formatter.Deserialize(fs);
                intDictionaryOutput_spt1_splitterDistance = (int)formatter.Deserialize(fs);
                intPanelDefinitionPort_Height = (int)formatter.Deserialize(fs);
                if (intPanelDefinitionPort_Height < 25)
                    intPanelDefinitionPort_Height = 25;

                // motivational quotes appear at launch 
                groupboxMotivationalQuote.AppearAtLaunch = (bool)formatter.Deserialize(fs);

                // backup tabs
                classBackUp.arrWidths = new int[5];

                for (int intTabCounter = 0; intTabCounter < classBackUp.arrWidths.Length; intTabCounter++)
                    classBackUp.arrWidths[intTabCounter] = (int)formatter.Deserialize(fs);

                fs.Close();
            }
            else
            {
            }
            grbNotes.pnlNotes.setTabs_byWidths();
        }



        void formWords_Move(object sender, EventArgs e)
        {
            recordForm();
        }

        void formWords_Activated(object sender, EventArgs e)
        {
            if (bolInit)
                return;

            Hide();
            Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            classNotesInfo.init();

            grbNotePilot = new groupboxNotePilot();

            Controls.Add(splMain);
            splMain.SplitterMoved += splMain_SplitterMoved;
            splMain.Dock = DockStyle.Fill;

            splMain.Panel2.Controls.Add(splSub);
            splSub.Orientation = Orientation.Vertical;
            splSub.Panel1.Controls.Add(splSub_A);
            splSub.Dock = DockStyle.Fill;

            pnlDictionaryOutput = new panelDictionaryOutput();
            splSub.Panel2.Controls.Add(pnlDictionaryOutput);
            pnlDictionaryOutput.Dock = DockStyle.Fill;
            pnlDictionaryOutput.BackColor = Color.Blue;

            splSub_A.Orientation = Orientation.Horizontal;
            splSub_A.SplitterMoved += splSub_A_SplitterMoved;
            splSub_A.Dock = DockStyle.Fill;
            splSub_A.Panel1.Controls.Add(rtxCK);

            splMain.Dock = DockStyle.Fill;

            rtb.AllowDrop = true;

            rtxCK.rtx.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            rtxCK.rtx.KeyDown += rtxCK_KeyDown;
            rtxCK.rtx.KeyUp += rtxCK_KeyUp;
            rtxCK.rtx.TextChanged += Rtx_TextChanged;
            rtxCK.rtx.MouseDown += rtxCK_MouseDown;
            rtxCK.rtx.MouseDoubleClick += Rtx_MouseDoubleClick;
            rtxCK.rtx.MouseLeave += Rtx_MouseLeave;
            rtxCK.rtx.SelectionChanged += Rtx_SelectionChanged;
            rtxCK.rtx.GotFocus += rtx_GotFocus;
            rtxCK.rtx.LostFocus += Rtx_LostFocus;
            rtxCK.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_Load].Visible = false;
            rtxCK.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_SaveAs].Visible = false;
            rtxCK.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.File_New].Visible = false;
            rtxCK.ToolBar.lstButtons[(int)ck_RichTextBox.panelToolBar.enuButtons.Lock].Visible = false;

            rtxCK.File_Save = mnuFile_Save_Click;
            rtxCK.rtx.ContextMenu.MenuItems.Add(mnuDictionary);
            rtxCK.rtx.ContextMenu.Tag = (object)rtxCK.rtx;
            rtxCK.rtx.ContextMenu.Popup += cMnu_PopUp;
            rtxCK.rtx.VScroll += Rtx_VScroll;
            rtxCK.Dock = DockStyle.Fill;
            rtxCK.rtx.Tag = (object)enuSearchRequestor.main;
            rtxCK.Heading_BackColor = Color.LightBlue;
            rtxCK.Heading_ForeColor = Color.Black;
            rtxCK.Heading_Font = new Font("Arial", 14);

            splSub_A.Panel2.Controls.Add(grbHeadingList);
            grbHeadingList.Dock = DockStyle.Fill;
            tmrHeadingList.Interval = 500;
            tmrHeadingList.Tick += TmrHeadingList_Tick;

            tmrWordAnalyzer.Interval = 750;
            tmrWordAnalyzer.Tick += TmrWordAnalyzer_Tick;

            bckSearch.WorkerReportsProgress = true;
            bckSearch.WorkerSupportsCancellation = true;
            bckSearch.DoWork += BckSearch_DoWork;
            bckSearch.ProgressChanged += BckSearch_ProgressChanged;
            bckSearch.RunWorkerCompleted += BckSearch_RunWorkerCompleted;

            bckDictionaryOutput_Init.WorkerReportsProgress = true;
            bckDictionaryOutput_Init.WorkerSupportsCancellation = true;

            grbNotes = new groupboxNotes();
            grbNotes.rtxNotes.Heading_Font = ckRTX.Heading_Font;
            splMain.Panel1.Controls.Add(grbNotes);
            grbNotes.Dock = DockStyle.Fill;


            Forms_Load();

            new formFeedback();
            new classBT_Dictionaries();
            new formDictionarySelection();

            grbHeadingList.DictionarySelection_Init();

            if (!System.IO.File.Exists(strFirstRunFilename))
            {
                string strDir = System.IO.Directory.GetCurrentDirectory();
                string[] strFiles = System.IO.Directory.GetFiles(strDir  + "\\", "Short Stories.*");

                if (strFiles.Length > 0)
                {
                    cProject.FilePath
                      = PathAndFilename
                      = strFiles[0];

                    this.LoadProject(cProject.FilePath);

                    classNotesInfo.cRoot.setBasePath(System.IO.Directory.GetCurrentDirectory() + "\\");
                    cProject.Save();
                    Forms_Save();
                    System.IO.File.WriteAllText(strFirstRunFilename, "Words First run");
                }
            }
            else if (PathAndFilename.Length > 0)
            {
                cProject.FilePath = PathAndFilename;
                LoadProject(cProject.FilePath);
            }

            grbNotes.rtxNotes.rtx.MouseLeave += Rtx_MouseLeave;
            grbNotes.rtxNotes.rtx.GotFocus += rtx_GotFocus;
            grbNotes.rtxNotes.Locked
                    = grbNotes.rtxNotes.ToolBar.btnLock.Toggled
                    = bolFormLoad_Notes_Locked;
            grbNotes.pnlNotes.POV_Highlight = bolNotes_POV_Highlight;

            ToolTip_SetUp();

            tmrMessage.Interval = 2 * 1000;
            tmrMessage.Tick += TmrMessage_Tick;

            tmrPosition.Interval = 5;
            tmrPosition.Tick += new EventHandler(tmrPosition_Tick);

            Size = sz;

            formWords_SizeChanged((object)this, new EventArgs());
            if (classDictionary.lstDictionaries != null && classDictionary.lstDictionaries.Count > 0)
                if (grbHeadingList != null)
                    grbHeadingList.cDictionary = classDictionary.lstDictionaries[groupboxHeadingList.DefaultDictionaryIndex];

            try { splMain.SplitterDistance = intsplMainSplitterDistance; }
            catch (Exception) { }

            try { splSub.SplitterDistance = intSplSubSplitterDistance; }
            catch (Exception) { }

            try { splSub_A.SplitterDistance = intsplSub_ASplitterDistance; }
            catch (Exception) { }

            try { grbHeadingList.splMain_Distance = intSplHeadingListSplitterDistance; }
            catch (Exception) { }

            try { splSub.SplitterMoved += SplSub_SplitterMoved; }
            catch (Exception) { }

            try { splSub.SplitterDistance = intSplSubSplitterDistance; }
            catch (Exception) { }
            
            rtxCK.lblColor_Draw();
            grbNotes.rtxNotes.lblColor_Draw();
            try { grbNotes.splMain.SplitterDistance = intSpl_GrbNotes_Main_SplitterDistance; }
            catch (Exception) { }

            try { grbNotes.splSub.SplitterDistance = intSpl_GrbNotes_Sub_SplitterDistance; }
            catch (Exception) { }
            

            Controls.Add(grbNotePilot);

            bolInit = true;

            Controls.Add(grbOptions);
            grbOptions.Hide();

            Location = loc;
            Show();
            grbNotePilot.Show();
            groupboxNotePilot.instance.Show();
            groupboxNotePilot.instance.Location = groupboxNotePilot.ptLocation;
            groupboxNotePilot.instance.BringToFront();
            grbHeadingList.BringToFront();

            if (intDictionaryOutput_spt0_splitterDistance > 0 && intDictionaryOutput_spt0_splitterDistance < pnlDictionaryOutput.spt0.Height)
                pnlDictionaryOutput.spt0.SplitterDistance = intDictionaryOutput_spt0_splitterDistance;
            if (intDictionaryOutput_spt1_splitterDistance > 0 && intDictionaryOutput_spt1_splitterDistance < pnlDictionaryOutput.spt1.Height)
                pnlDictionaryOutput.spt1.SplitterDistance = intDictionaryOutput_spt1_splitterDistance;

            if (intDictionaryOutput_spt0_splitterDistance > 0 && intDictionaryOutput_spt0_splitterDistance < pnlDictionaryOutput.spt0.Height)
                pnlDictionaryOutput.spt0.SplitterDistance = intDictionaryOutput_spt0_splitterDistance;
            if (intDictionaryOutput_spt1_splitterDistance > 0 && intDictionaryOutput_spt1_splitterDistance < pnlDictionaryOutput.spt1.Height)
                pnlDictionaryOutput.spt1.SplitterDistance = intDictionaryOutput_spt1_splitterDistance;

            panelDefinitionPort.Height_Default = intPanelDefinitionPort_Height;

            frmWordsFlash.Hide();
            frmWordsFlash.Dispose();
            frmWordsFlash = null;

            InitDictionaryOutput();

            grbNotePilot.LocationSet();

            groupboxNotePilot.setToLoad();
            if (groupboxMotivationalQuote.AppearAtLaunch)
                new groupboxMotivationalQuote();
                        
            panelDictionaryOutput.mbpWordList_Load();

            cBackUp = new classBackUp();
            
            classRhymeDictionary.Init();
        }

        public void Rtx_VScroll(object sender, EventArgs e)
        {     
            semScroll.WaitOne();
            
            if (grbNotes.grbWordAnalyzer.WordOutline_Toggle)
                grbNotes.grbWordAnalyzer.tmrUnderlineSelection_Reset();

            if (pictureboxWordAnalyzer.instance != null && pictureboxWordAnalyzer.instance.Visible)
                pictureboxWordAnalyzer.instance.Draw();

            semScroll.Release();
        }

        void cursors_Build()
        {
            _cCursors = new Cursor[(int)enuCursors._numCursors];

            Bitmap bmpExpand = (Bitmap)new Bitmap(Properties.Resources.Cursor_Expand);
            bmpExpand.MakeTransparent(bmpExpand.GetPixel(0, 0));
            cCursors[(int)enuCursors.Expand] = Ck_Objects.classIconMaker.BitmapToCursor(bmpExpand);
            
            Bitmap bmpCollapse = (Bitmap)new Bitmap(Properties.Resources.Cursor_Collapse);
            bmpCollapse.MakeTransparent(bmpCollapse.GetPixel(0, 0));
            cCursors[(int)enuCursors.Collapse] = Ck_Objects.classIconMaker.BitmapToCursor(bmpCollapse);
            
            Bitmap bmpGarbage = (Bitmap)new Bitmap(Properties.Resources.Cursor_Garbage);
            bmpGarbage.MakeTransparent(bmpGarbage.GetPixel(0, 0));
            cCursors[(int)enuCursors.Garbage] = Ck_Objects.classIconMaker.BitmapToCursor(bmpGarbage, 0,0);
            
            Bitmap bmpMoveWord_Left = (Bitmap)new Bitmap(Properties.Resources.cursor_MoveWord_Left);
            bmpMoveWord_Left.MakeTransparent(bmpMoveWord_Left.GetPixel(0, 0));
            cCursors[(int)enuCursors.MoveWord_Left] = Ck_Objects.classIconMaker.BitmapToCursor(bmpMoveWord_Left, 0,0);
            
            Bitmap bmpMoveWord_Right = (Bitmap)new Bitmap(Properties.Resources.cursor_MoveWord_Right);
            bmpMoveWord_Right.MakeTransparent(bmpMoveWord_Right.GetPixel(0, 0));
            cCursors[(int)enuCursors.MoveWord_Right] = Ck_Objects.classIconMaker.BitmapToCursor(bmpMoveWord_Right, 0,0);

            Bitmap bmpRecallWord = (Bitmap)new Bitmap(Properties.Resources.Cursor_RecallWord);
            bmpRecallWord.MakeTransparent(bmpRecallWord.GetPixel(0, 0));
            cCursors[(int)enuCursors.RecallWord] = Ck_Objects.classIconMaker.BitmapToCursor(bmpRecallWord, 0,0);

            Bitmap bmpInsertWord = (Bitmap)new Bitmap(Properties.Resources.Cursor_InsertWord);
            bmpInsertWord.MakeTransparent(bmpInsertWord.GetPixel(0, 0));
            cCursors[(int)enuCursors.InsertWord] = Ck_Objects.classIconMaker.BitmapToCursor(bmpInsertWord, 0,0);

            Bitmap bmpCapitalize = (Bitmap)new Bitmap(Properties.Resources.cursor_Capitalize);
            bmpCapitalize.MakeTransparent(bmpCapitalize.GetPixel(0, 0));
            cCursors[(int)enuCursors.Capitalize] = Ck_Objects.classIconMaker.BitmapToCursor(bmpCapitalize, 0,0);
        }
        
        public void Rtx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RichTextBox rtxSender = (RichTextBox)sender;
            ck_RichTextBox ckSender = (ck_RichTextBox)rtxSender.Parent;
            grbNotes.grbWordAnalyzer.txtSearch.Text = ckSender.WordUnderCursor();
            grbNotes.grbWordAnalyzer.txtSearch.Focus();
            grbNotes.grbWordAnalyzer.txtSearch.Select(grbNotes.grbWordAnalyzer.txtSearch.Text.Length, 0);
        }

        private void rtx_GotFocus(object sender, EventArgs e)
        {
            groupboxWordAnalyzer.rtxFocus = (RichTextBox)sender;
            
        }

        void InitDictionaryOutput()
        {
            if (classDictionary.lstDictionaries.Count == 0) return;

            classDictionary cDictionary= classDictionary.get("Dictionary : English MW");
            string strFilename = "aaaaaaba";


            if (cDictionary == null)
            {
                cDictionary = classDictionary.lstDictionaries[0];
            }
            else
            {
                Random rnd = new Random();
                string[] array_Filenames = { "lmweakix", "emweacer", "amwearym", "bmweageq", "gmweaatl", "pmwebcow", "rmweanrr", "tmweappz", "cmweaelc", "imweakpc", "mmweajot", "ymweaacm", "bmweafyh", "amweatnr", "pmweaslw", "pmweatze", "dmweabsm", "dmweapbw", "cmweavpy", "imweakoa", "lmweajyv", "pmweaqal", "amweatns", "mmweamqz", "lmweamep", "pmwebcnc", "pmweahug", "tmweasgp", "gmweaatk", "cmweawjc", "cmweapkw", "imweakob", "cmweadqs", "pmweaslm", "emweajfi", "rmweadzk", "smwebmay", "amweaeoi", "amweardl", "cmweapkz", "imweaayd", "qmweabjn", "rmweantb", "bmweawgu", "amweajde", "pmweanir", "emweacyv", "omweabim", "bmweamtw", "mmweabxo", "rmweanta", "dmweangr", "pmweaevh", "smweaddl", "hmweamhe", "_mweaawb", "fmweacgz", "fmweamym", "omweaajv", "mmweahti", "amweapxf", "bmweacwv", "amweatnr", "cmweaxip", "rmweagpg", "amweartm", "omweacxh", "emweadkb", "cmweavpy", "emweabbn", "tmweafcd", "mmweangw", "smwebtbj", "pmweaqal", "cmwebdeq", "amweatns", "mmweajow", "pmweaddm", "tmweafhl", "bmweailx", "amweakni", "bmweavut", "pmweamjj", "smweazjo", "fmweacia", "amweaqju", "imweaacu", "mmweauwx", "cmwebjsx", "amweapbs", "pmweajml", "rmweakqu", "cmweafdl", "mmweahoh", "pmweaduh", "vmweacmb", "amweamch", "pmweasmb", "pmweasls", "pmweaiki", "smweasrj", "lmweancy", "mmweajov", "pmweamid", "hmweamhf", "pmwebcpz", "imweaayd", "mmweaqrs", "hmweagpc", "amwealkh", "smweassh", "amweatnu", "tmweabpo", "wmweabdq", "hmweaaly", "pmweazvv", "pmweasmg", "dmweampk", "rmweakqr", "cmweavqa", "cmweaxqe", "cmweanre", "dmweaedr", "dmweahxe", "cmweaxql", "emweafml", "mmweakbc", "emweagod", "amweamar", "pmweadug", "tmweajee", "mmweabvw", "lmweadcc", "amweamam", "amweaqjw", "kmweaboa", "dmweaaws", "gmweadbs", "pmweaqik", "rmweakqm", "mmweampe", "amweamio", "cmweazte", "lmweabcf", "rmweangd", "dmweancs", "dmweaecw", "lmweaazl", "emweahlr", "bmweavuu", "tmweafzo", "pmweaslo", "mmweaawd", "hmweamhd", "smweaupv", "cmweajzr", "hmweagpe", "cmwebdmt", "rmwealpg", "pmweasmk", "dmweangu", "mmweatud", "imweabmt", "jmweacth", "emweagoe", "cmweaasr", "pmweasln", "fmweacgy", "smwealri", "smwebrzo", "nmweajdv", "pmweamkb", "vmweabfl", "lmweandb", "nmweahgn", "bmweafzi", "mmweajui", "pmweaslm", "pmwebcmy", "mmweajok", "_mweaawa", "emweacys", "cmweabpn", "pmweasmd", "vmweaclz", "lmweancu", "tmweasli", "mmweajol", "pmwebaym", "emweacer", "pmweahql", "pmwebcnw", "pmwebcpg", "pmweaslw", };
                int intEleRND = (int)((rnd.NextDouble() * int.MaxValue) % array_Filenames.Length);
                strFilename = array_Filenames[intEleRND];
            }

            List<classSearchResult> lstSearchResults = new List<classSearchResult>();
            classSearchResult cSR = new classSearchResult(ref cDictionary, strFilename);
            lstSearchResults.Add(cSR);

            List<string> lstWords = new List<string>();
            lstWords.Add("Poetry");
            lstWords.Add("Prose");

            lstSearchParameters = new List<classSearchParameters>();
            classSearchParameters cSP = new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Heading);
            lstSearchParameters.Add(cSP);

            eSearchRequestor = enuSearchRequestor.main;
            DictionaryOutput(lstSearchResults, "");
        }

        private void SplSub_SplitterMoved(object sender, SplitterEventArgs e)
        {
            placeObjects();
        }

        // prevents TwoWordSwap when Ctrl pressed in other RichTextBox
        private void Rtx_MouseLeave(object sender, EventArgs e)
        {
            keyDownLast = Keys.Escape;
        }

        void formWords_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        public static ck_RichTextBox ckRTX
        {
            get
            {

                if (formWords.RTX_Focused != null && formWords.RTX_Focused.Parent != null)
                {
                    try
                    {
                        ck_RichTextBox cRetVal = (ck_RichTextBox)formWords.RTX_Focused.Parent;
                        return cRetVal;
                    }
                    catch (Exception)
                    {

                        
                    }
                }

                if (formWords.RTX_Focused != null)
                {
                    if (formWords.RTX_Focused.Tag != null)
                    {
                        enuRTX eRTX = (enuRTX)formWords.RTX_Focused.Tag;
                        switch (eRTX)
                        {
                            case enuRTX.Main:
                                return formWords.instance.rtxCK;

                            case enuRTX.Notes:
                                return formWords.instance.grbNotes.rtxNotes;

                            case enuRTX.Copy:
                            case enuRTX.Dictionary:
                            case enuRTX.HeadingList:
                            default:
                                break;
                        }
                    }
                }

                return null;
            }
        }

        public void placeObjects()
        {
            if (grbNotes != null && grbNotes.MainScreen)
            {
                grbNotes.pnlRTX.Dock = DockStyle.None;
                grbNotes.pnlRTX.Location = rtxCK.Location;
                grbNotes.pnlRTX.Size = rtxCK.Size;
            }
            grbHeadingList.Size = new Size(Width, 250);
            grbHeadingList.Location = new Point(0,
                                                rtxCK.Height - grbHeadingList.Height);
            grbHeadingList.BringToFront();

            grbOptions.Location = new Point((Width - grbOptions.Width) / 2, (Height - grbOptions.Height) / 2);
            if (pnlDictionaryOutput != null)
                pnlDictionaryOutput.placeObjects();
            grbNotePilot.LocationSet();

            recordForm();
        }

        void recordForm()
        {
            if (!bolInit) return;
            if (WindowState != FormWindowState.Minimized)
            {
                sz = Size;
                loc = Location;
            }
        }

        public void mnuFile_Save_Click(object sender, EventArgs e)
        {
            if (cProject.cEdit_Main != null)
            {
                string strFilename = cProject.cEdit_Main.Filename;

                rtxCK.SaveFile(strFilename);
                bolTextChangedAndNotSaved = false;
                Message("File saved : " + strFilename);

                formWords.cProject.cEdit_Main.HeadingColor_Set(ckRTX.rtx.Rtf);
            }
        }

        public void mnuProject_Load_Click(object sender, EventArgs e)
        {
            if (!TextChangedAndNotSaved()) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = classProject.ProjectExtensionFilter;  // "Project Files | *." + classProject.ProjectFileExtension;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadProject(ofd.FileName);
            }
        }

        public void LoadProject(string strFileName)
        {
            groupboxNotes.bolFirstLoad = true;
            PathAndFilename = strFileName;
            cProject.Load();
            if (cProject.eVersion != classProject.eVersion_Latest)
            {
                cProject.eVersion = classProject.eVersion_Latest;
                string strExtension_Old = System.IO.Path.GetExtension(PathAndFilename);
                string strExtension_New ="." + cProject.eVersion.ToString();
                PathAndFilename = PathAndFilename.Replace(strExtension_Old, strExtension_New);
                cProject.Save();
            }
            grbNotes.pnlNotes.setTabs_byWidths();
            setWorkingDirectoryFromFileName(PathAndFilename);
        }

        private void setWorkingDirectoryFromFileName(string FileName)
        {
            int intIndexLastSlash = FileName.LastIndexOf('\\');
            if (intIndexLastSlash > 0)
            {
                string strTemp = FileName.Substring(0, intIndexLastSlash).ToUpper();
                if (grbNotes != null)
                {
                    if (strTemp != grbNotes.WorkingDirectory)
                    {
                        grbNotes.WorkingDirectory = strTemp;
                    }
                }
            }
        }
        public void mnuProject_Save_Click(object sender, EventArgs e)
        {
            cProject.Save();
            bolTextChangedAndNotSaved = false;
        }

        public void mnuProject_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Project Files | *." + classProject.ProjectFileExtension;
            sfd.DefaultExt = classProject.ProjectFileExtension;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                PathAndFilename = sfd.FileName;
                Text = sfd.FileName;
                cProject.Save();
                bolTextChangedAndNotSaved = false;
            }
        }

        bool _bolTextChanged = false;
        public bool bolTextChanged
        {
            get { return _bolTextChanged; }
            set { _bolTextChanged = value; }
        }


        private void Rtx_TextChanged(object sender, EventArgs e)
        {
            bolTextChangedAndNotSaved = true;

            classNotesInfo cNote_User = ckRTX == groupboxNotes.instance.rtxNotes
                                                           ? cProject.cEdit_Alt
                                                           : cProject.cEdit_Main;
            if (cNote_User != null)
                cNote_User.bolBackup_ChangesMade = true;

            groupboxNotes.rtx_TextChanged(sender, e);

            tmrHeadingList_Reset();
            tmrWordAnalyzer_Reset();
            tmrWordAnalyzerDraw_Reset();
            bolTextChanged = true;
        }

        bool _bolTextChangedAndNotSaved = false;
        public bool bolTextChangedAndNotSaved
        {
            get { return _bolTextChangedAndNotSaved; }
            set
            {
                _bolTextChangedAndNotSaved = value;
            }
        }

        bool TextChangedAndNotSaved() { return TextChangedAndNotSaved(MessageBoxButtons.YesNoCancel); }

        bool TextChangedAndNotSaved(MessageBoxButtons msgBtns)
        {
            if (bolTextChangedAndNotSaved)
            {
                DialogResult dr = MessageBox.Show("Your work is not saved.\r\nDo you want to save your work?", "Work not saved", msgBtns);
                switch (dr)
                {
                    case DialogResult.Yes:
                        mnuProject_Save_Click(rtxCK, new EventArgs());
                        mnuFile_Save_Click((object)this, new EventArgs());
                        return true;

                    case DialogResult.No:
                        return true;

                    case DialogResult.Cancel:
                    default:
                        return false;
                }
            }
            return true;
        }

        public void mnuProject_Exit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        public void mnuProject_New_Click(object sender, EventArgs e)
        {
            if (!TextChangedAndNotSaved()) return;
            
            rtxCK.Text = "";
            rtxCK.SelectionRightIndent = 50;
            rtxCK.RightMargin = 1100;
            cProject.New();
            
            PathAndFilename = System.IO.Directory.GetCurrentDirectory() + "\\default FileName.xml";
            Text = PathAndFilename;

            grbNotes.pnlNotes.Buttons_Rebuild();

        }
    }


    public class classRTXInfo
    {
        public string text;
        public Font fnt;
        public Color clr;
        public int start;
        public int stop;

        public classRTXInfo(string _text, Font _fnt, Color _clr, int _start, int _stop)
        {
            text = _text;
            fnt = _fnt;
            clr = _clr;
            start = _start;
            stop = _stop;
        }

    }

    public class classSearchResult
    {

        public string strFileName = "";

        public string Heading
        {
            get
            {
                string strRetVal = "";
                if (Prefix.Length > 0)
                    strRetVal = Prefix + " + ";
                strRetVal += Find;
                if (Suffix.Length > 0)
                    strRetVal += " + " + Suffix;
                return strRetVal;
            }
        }


        string str_Prefix = "";
        public string Prefix
        {
            get { return str_Prefix; }
            set { str_Prefix = value; }
        }

        string str_Suffix = "";
        public string Suffix
        {
            get { return str_Suffix; }
            set { str_Suffix = value; }
        }

        string str_Find = "";
        public string Find
        {
            get { return str_Find; }
            set
            {
                str_Find = value;
                if (_eventSearchChanged != null)
                    _eventSearchChanged((object)this, new EventArgs());
            }
        }


        string strWordSearched = "";
        public string WordSearched
        { 
        get { return strWordSearched; }
            set { strWordSearched = value; }
        }

        public EventHandler _eventSearchChanged = null;

      
        public classDictionary cDictionary;
        public classFileContent cFileContent;
        public classSearchResult(ref classDictionary cDictionary, ref class_LL_Record cLL)
        {
            Init(ref cDictionary, ref cLL, "", "");
        }
        public classSearchResult(ref classDictionary cDictionary, ref class_LL_Record cLL, string strPrefix, string strSuffix)
        {
            Init(ref cDictionary, ref cLL, strPrefix, strSuffix);
        }

        void Init(ref classDictionary dictionary, ref class_LL_Record cLL, string strPrefix, string strSuffix)
        {
            this.cDictionary = dictionary;
            this.strFileName = cLL != null
                                    ? cLL.FileName
                                    : "";

            Prefix = strPrefix;
            Suffix = strSuffix;
            Find = cLL != null 
                        ? cLL.strHeading
                        : "";
        }

        public classSearchResult(ref classDictionary cDictionary, string strFilename)
        {
            Init(ref cDictionary, strFilename, "", "", "");
        }
        public classSearchResult(ref classDictionary cDictionary, string strFilename, string strHeading)
        {
            Init(ref cDictionary, strFilename, strHeading, "", "");
        }
        public classSearchResult(ref classDictionary cDictionary, string strFilename, string strHeading, string strPrefix, string strSuffix)
        {
            Init(ref cDictionary, strFilename, strHeading, strPrefix, strSuffix);
        }
        void Init(ref classDictionary cDictionary, string strFilename, string strHeading, string strPrefix, string strSuffix)
        {
            this.strFileName = strFilename;
            this.cDictionary = cDictionary;

            Prefix = strPrefix;
            Suffix = strSuffix;
            Find = strHeading;
        }

        public classSearchResult Copy()
        {
            classDictionary cDictionary_Ref = cDictionary;
            string strFilename = this.strFileName;
            string strHeading = Find;
            string strPrefix = Prefix;
            string strSuffix = Suffix;

            classSearchResult cRetVal = new classSearchResult(ref cDictionary, strFilename, strHeading, strPrefix, strSuffix);
            return cRetVal;
        }
    }

    public class classSearchParameters
    {
        public classDictionary cDictionary = null;
        public List<string> lstWords = new List<string>();
        public enuSearchType eSearchType = enuSearchType._num;

        public classSearchParameters(ref classDictionary _cDictionary, ref List<string> _lstWords, enuSearchType _eSearchType)
        {
            cDictionary = _cDictionary;
            lstWords = _lstWords;
            eSearchType = _eSearchType;
        }
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StringLibrary;
using Ck_Objects;

namespace Words
{
    public class panelDefinitionPort : Panel
    {
        System.Windows.Forms.Timer tmrDebug = null;

        static System.Windows.Forms.Timer tmrMouseHover = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer tmrClick_WaitForDoubleClick = null;

        public static Font fntDictionaryHeading = new Font("Times Roman", 14, FontStyle.Bold & FontStyle.Italic);
        public static Font fntWordHeading = new Font("Times Roman", 12, FontStyle.Underline);
        public static Font fntWordDefinition = new Font("Times Roman", 10, FontStyle.Regular);

        public classMultiButtonPic.classButton cBtn = null;
        System.Windows.Forms.Timer tmrHighLightSearchedWord = new System.Windows.Forms.Timer();

        public RichTextBox[] rtx_Array = new RichTextBox[2];

        int intRTXIndex_Current = 0;
        void RTX_Swap()
        {
            rtx.SendToBack();
            intRTXIndex_Current = (intRTXIndex_Current + 1) % 2;
        }

        public RichTextBox rtx
        {
            get { return rtx_Array[intRTXIndex_Current]; }
        }
        RichTextBox rtx_Next
        {
            get { return rtx_Array[(intRTXIndex_Current + 1) % 2]; }
        }

        public void tmrHighLightSearchedWord_Reset()
        {
            tmrHighLightSearchedWord.Enabled = false;
            tmrHighLightSearchedWord.Enabled = true;
        }

        private void Rtx_VScroll(object sender, EventArgs e)
        {
            tmrHighLightSearchedWord_Reset();
        }


        List<string> _lstSearchWords = new List<string>();
        public List<string> lstSearchWords
        {
            get { return _lstSearchWords; }
            set { _lstSearchWords = value; }
        }

        public panelDefinitionPort Previous
        {
            get
            {
                if (intMyIndex >0)
                {
                    int intPrevIndex = intMyIndex - 1;
                    panelSP.classSweepAndPrune_Element cEleSP = pnlSP.lstElements[intPrevIndex];

                    Panel pnlContainerPrev = (Panel)cEleSP.obj;
                    panelDefinitionPort pnlDefPort_Prev = (panelDefinitionPort)pnlContainerPrev.Controls[0];
                    return pnlDefPort_Prev;
                }
                return null;
            }
        }

        public panelDefinitionPort Next
        {
            get
            {
                if (intMyIndex < pnlSP.lstElements.Count -1)
                {
                    int intNextIndex = intMyIndex+ 1;
                    panelSP.classSweepAndPrune_Element cEleSP = pnlSP.lstElements[intNextIndex];

                    Panel pnlContainerNext = (Panel)cEleSP.obj;
                    panelDefinitionPort pnlDefPort_Next = (panelDefinitionPort)pnlContainerNext.Controls[0];
                    return pnlDefPort_Next;
                }
                return null;
            }
        }

        int intMyIndex
        {
            get
            {
                Panel pnlContainer = (Panel)Parent;
                panelSP.classSweepAndPrune_Element cEleSP = (panelSP.classSweepAndPrune_Element)pnlContainer.Tag;
                return pnlSP.lstElements.IndexOf(cEleSP); 
            }
        }


        panelSP.panelSP pnlSP
        {
            get { return pnlDictionaryOutput.pnlSP; }
        }

        static public panelDictionaryOutput pnlDictionaryOutput
        {
            get { return formWords.pnlDictionaryOutput; }
        }


        public void tmrHighLightSearchedWord_Tick(object sender, EventArgs e)
        {
            tmrHighLightSearchedWord.Enabled = false;
            if (eSearchType == enuSearchType.Content || eSearchType == enuSearchType.Both_Heading_And_Content)
            {
                Pen p = new Pen(Color.Red, 1);
                RichTextBox rtxRef = rtx;
                ///*
                ck_RichTextBox.Underline(ref rtxRef, p, lstSearchWords, ck_RichTextBox.enuGraphics_Lines.Circle);
                /*/
                ck_RichTextBox.Underline(ref rtxRef, p, lstSearchWords, ck_RichTextBox.enuGraphics_Lines.Straight);
                //*/
            }
        }

        static int intResizeTolerance = 10;
        Label lblDictionaryHeading = new Label();
        public ComboBox cmbHeadings = new ComboBox();
        public List<classQueueItem> lstQueue = new List<classQueueItem>();
        public class classQueueItem
        {
            public List<classSearchResult> lstSearchResults = new List<classSearchResult>();
            public int IndexLast = 0;

            public classQueueItem(List<classSearchResult> lstSearchResults)
            {
                this.lstSearchResults = lstSearchResults;
            }

            public string Filename
            {
                get
                {
                    if (lstSearchResults.Count < 1)
                        return "null";
                    if (IndexLast < 0)
                        IndexLast = 0;
                    if (IndexLast >= lstSearchResults.Count)
                        IndexLast = lstSearchResults.Count - 1;
                    return lstSearchResults[IndexLast].strFileName;
                }
            }
            public string Heading
            {
                get
                {
                    if (lstSearchResults.Count < 1)
                        return "null";
                    if (IndexLast < 0)
                        IndexLast = 0;
                    if (IndexLast >= lstSearchResults.Count)
                        IndexLast = lstSearchResults.Count - 1;
                    return lstSearchResults[IndexLast].Find;
                }
            }
        }

        classDictionary _cDictionary = null;
        public classDictionary cDictionary
        {
            get { return _cDictionary; }
            set
            {
                if (_cDictionary != value)
                {
                    _cDictionary = value;
                    Heading_Set();
                }
            }
        }


        bool bolRhymeDictionary = false;
        public bool RhymeDictionary
        {
            get { return bolRhymeDictionary; }
            set { bolRhymeDictionary = value; }
        }

        public void Search_AltDictionary(object sender, EventArgs e)
        {

        }

        public void Heading_Set()
        {
            Heading
                = lblDictionaryHeading.Text
                = (_cDictionary == null
                                ? (RhymeDictionary ? "Rhyme Dictionary" : "no dictionary selected")
                                : _cDictionary.Heading.Trim());

            Buttons_Visible();
        }
        public enum enuRTXStyle { DictionaryHeading, WordHeading, WordDefinition };
        public static classRTXInfo getRTX(ref RichTextBox rtx, string strText, enuRTXStyle eRTXStyle)
        {
            int intStart = rtx.Text.Length;
            rtx.Text += rtx.Text.Length > 0
                                        ? "\r\n"
                                        : "";
            rtx.Text += strText;
            int intStop = rtx.Text.Length;

            switch (eRTXStyle)
            {
                case enuRTXStyle.DictionaryHeading:
                    return new classRTXInfo(strText, fntDictionaryHeading, Color.Red, intStart, intStop);

                case enuRTXStyle.WordDefinition:
                    return new classRTXInfo(strText, fntWordDefinition, Color.Black, intStart, intStop);

                case enuRTXStyle.WordHeading:
                    return new classRTXInfo(strText, fntWordHeading, Color.Blue, intStart, intStop);
            }
            return null;
        }

        public enum enuButtons { delete, forward, back, reset, WordAnalyzer_Search, WordLists, _numButtons };


        public static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID { get { return intIDCounter; } }
        static System.Windows.Forms.Timer tmrResize = null;

        static Panel _pnlResize = null;
        static Panel pnlResize
        {
            get { return _pnlResize; }
            set
            {
                if (_pnlResize != value)
                {
                    _pnlResize = value;
                    if (_pnlResize != null)
                        tmrResize.Enabled = true;
                }
            }
        }

        static int _intHeight_Default = 125;
        static public int Height_Default
        {
            get { return _intHeight_Default; }
            set
            {
                if (value < 50) value = 50;
                _intHeight_Default = value;

            }
        }

        string strHeading = "";
        public string Heading
        {
            get { return strHeading; }
            set { strHeading = value; }
        }


        Ck_Objects.classLabelButton[] btns = new Ck_Objects.classLabelButton[(int)enuButtons._numButtons];
        public Panel pnlButtons = new Panel();
        static bool bolTmrMouseHover_Init = false;


        classLabelButton btnWordAnalyzer_Search
        {
            get { return btns[(int)enuButtons.WordAnalyzer_Search]; }
        }

        classLabelButton btnWordLists
        {
            get { return btns[(int)enuButtons.WordLists]; }
        }


        static public panelDefinitionPort pnlDebug = null;
        classSearchResult _cSearchResult = null;
        public classSearchResult cSearchResult { get { return _cSearchResult; } }

        public panelDefinitionPort.panelWordLists pnlWordLists = null;

        ContextMenu mnuDictionary = new ContextMenu();
        public panelDefinitionPort(ref classDictionary cDictionary, ref classSearchResult cSearchResult)
        {
            this._cSearchResult = cSearchResult;
            this.cDictionary = cDictionary;

            panelDefinitionPort pnlMyRef = this;
            pnlWordLists = new panelWordLists(ref pnlMyRef);
            Controls.Add(pnlWordLists);

            bool bolDebug = false;
            if (bolDebug)
                pnlDebug = this;

            if (!bolTmrMouseHover_Init)
            {
                tmrMouseHover_Interval_Set();
                bolTmrMouseHover_Init = true;
            }

            tmrHighLightSearchedWord.Interval = 200;
            tmrHighLightSearchedWord.Tick += tmrHighLightSearchedWord_Tick;

            tmrClick_WaitForDoubleClick = new System.Windows.Forms.Timer();
            tmrClick_WaitForDoubleClick.Interval = 500;
            tmrClick_WaitForDoubleClick.Tick += tmrClick_WaitForDoubleClick_Tick;

            Name = "panelDefinitionPort " + ID.ToString();
            BackColor = Color.Blue;
            BorderStyle = BorderStyle.Fixed3D;

            Controls.Add(cmbHeadings);
            cmbHeadings.SelectedIndexChanged += CmbHeadings_SelectedIndexChanged;
            cmbHeadings.KeyDown += CmbHeadings_KeyDown;

            Controls.Add(lblDictionaryHeading);
            lblDictionaryHeading.ForeColor = Color.White;
            lblDictionaryHeading.Text = cDictionary == null
                                                     ? "no dictionary selected"
                                                     : cDictionary.Heading;
            lblDictionaryHeading.Tag = this;
            lblDictionaryHeading.MouseClick += LblDictionaryHeading_Click;
            lblDictionaryHeading.MouseEnter += LblDictionaryHeading_MouseEnter;
            lblDictionaryHeading.MouseLeave += LblDictionaryHeading_MouseLeave;
            lblDictionaryHeading.MouseWheel += LblDictionaryHeading_MouseWheel;

            lblDictionaryHeading.ContextMenu = mnuDictionary;
            mnuDictionary.Popup += CMnu_Popup;

            tmrMouseHover.Enabled = false;
            tmrMouseHover.Tick += TmrMouseHover_Tick;

            rtx_Array[0] = new RichTextBox();
            rtx_Array[1] = new RichTextBox();

            for (int intRTXCounter = 0; intRTXCounter < rtx_Array.Length; intRTXCounter++)
            {
                Controls.Add(rtx);
                rtx.Clear();
                rtx.ZoomFactor = 1;
                rtx.ZoomFactor = ZoomFactor;
                rtx.SizeChanged += rtx_SizeChanged;
                rtx.Tag = (object)this;
                rtx.BorderStyle = BorderStyle.None;
                rtx.KeyDown += RtxOutput_KeyDown;
                rtx.KeyUp += RtxOutput_KeyUp;
                rtx.KeyPress += RtxOutput_KeyPress;
                rtx.MouseEnter += RtxOutput_MouseEnter;
                rtx.MouseEnter += _MouseEnter;
                rtx.MouseLeave += RtxOutput_MouseLeave;
                rtx.MouseMove += RtxOutput_MouseMove;
                rtx.MouseDown += RtxOutput_MouseDown;
                rtx.MouseWheel += RtxOutput_MouseWheel;
                rtx.VScroll += RtxOutput_VScroll;
                rtx.LostFocus += Rtx_LostFocus;
                rtx.GotFocus += Rtx_GotFocus;
                RTX_Swap();
            }

            if (tmrResize == null)
            {
                tmrResize = new System.Windows.Forms.Timer();
                tmrResize.Interval = 200;
                tmrResize.Tick += TmrResize_Tick;
            }

            Controls.Add(pnlButtons);
            pnlButtons.Name = Name + "( pnlButtons )";
            for (int intButtonCounter = 0; intButtonCounter < (int)enuButtons._numButtons; intButtonCounter++)
            {
                enuButtons eButton = (enuButtons)intButtonCounter;
                Ck_Objects.classLabelButton btnNew = new Ck_Objects.classLabelButton();
                {
                    pnlButtons.Controls.Add(btnNew);
                    btnNew.Tag = (object)this;
                    btnNew.Visible = true;
                    btnNew.AutoSize = true;
                    btnNew.BackColor
                        = btnNew.Backcolor_Idle
                        = Color.LightGray;
                    btnNew.Name = Name + "(" + eButton.ToString() + ")";
                    switch (eButton)
                    {
                        case enuButtons.back:
                            btnNew.Text = "<";
                            btnNew.Click += btn_Back_Clicked;
                            break;

                        case enuButtons.forward:
                            btnNew.Text = ">";
                            btnNew.Click += btn_Forward_Clicked;
                            break;

                        case enuButtons.reset:
                            btnNew.Text = "¤";
                            btnNew.Click += btn_Reset_Clicked;
                            break;

                        case enuButtons.delete:
                            btnNew.Text = "X";
                            if (panelDictionaryOutput.instance != null)
                                btnNew.Click += panelDictionaryOutput.instance.btnCopy_Delete_click;
                            else
                                btnNew.Hide();
                            break;

                        case enuButtons.WordAnalyzer_Search:
                            btnNew.Text = "-";
                            btnNew.Click += btnWordAnalyzerSearch_Click;
                            break;

                        case enuButtons.WordLists:
                            btnNew.Text = "?";
                            btnNew.Click += btnWordLists_Click;
                            break;
                    }
                }
                btns[intButtonCounter] = btnNew;
            }

            MouseDown += PanelDefinitionPort_MouseDown;
            MouseMove += PanelDefinitionPort_MouseMove;

            SizeChanged += panelDefinitionPort_SizeChanged;
            placeButtons();
            Buttons_Visible();
        }

   
        private void CmbHeadings_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Back:
                case Keys.Home:
                case Keys.End:
                case Keys.Delete:
                case Keys.Space:
                case Keys.Shift:
                case Keys.OemMinus:
                    break;

                case Keys.Enter:
                    {
                        pnlDefPort_Caller = this;
                        List<string> lstWords = new List<string>();
                        lstWords.Add(cmbHeadings.Text);
                        List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                        lstSearchParameters.Add(new classSearchParameters(ref _cDictionary, ref lstWords, eSearchType));
                        formWords.instance.Search(ref lstSearchParameters, enuSearchRequestor.DefinitionPort_Heading_Click);

                        if (pnlDefPort_Caller.Next != null)
                            pnlDefPort_Caller.Next.cmbHeadings.Focus();
                        
                    }
                    break;

                default:
                    {
                        char chrPressed = (char)e.KeyValue;

                        string strAllowedChar = "!?-_'";
                        if (strAllowedChar.Contains(chrPressed))
                            return;

                        if (!char.IsLetter(chrPressed))
                            e.SuppressKeyPress = true;
                    }
                    break;
            }
        }

        void mnuDictionary_click(object sender, EventArgs e)
        {
            pnlDefPort_Caller = this;
            MenuItem mnuSender = (MenuItem)sender;
            List<classSearchParameters> lstSearchParameters_New = (List<classSearchParameters>)mnuSender.Tag;
            formWords.instance.Search(ref lstSearchParameters_New, enuSearchRequestor.DefinitionPort_Heading_Click);
        }

        void mnuDebug_Click(object sender, EventArgs e)
        {
            MenuItem mnuSender = (MenuItem)sender;
            Clipboard.SetText(mnuSender.Text);

            formWords.cBackUp.tmrBackup_Tick((object)this, new EventArgs());
        }

        private void CMnu_Popup(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu mnuSender = (System.Windows.Forms.ContextMenu)sender;
            RichTextBox rtxSender = (RichTextBox)mnuSender.Tag;

            mnuDictionary.MenuItems.Clear();

            if (formWords.Debug)
                mnuDictionary.MenuItems.Add(new MenuItem("Filename:" + cQueueItem != null
                                                                                ? cQueueItem.Filename
                                                                                : "",
                                                          mnuDebug_Click));


            List<string> lstWords = new List<string>();

            lstWords.Add(cSearchResult.WordSearched);
            for (int intPanelCounter = 0; intPanelCounter < formDictionarySelection.instance.lstPnlSelector.Count; intPanelCounter++)
            {
                panelSelector pnlSelector = formDictionarySelection.instance.lstPnlSelector[intPanelCounter];
                for (int intDictionaryCounter = 0; intDictionaryCounter < pnlSelector.lstToggles.Count; intDictionaryCounter++)
                {
                    char chrKey = pnlSelector.FastKey;
                    panelSelector.classButtonArray cBtnArray = pnlSelector.lstToggles[intDictionaryCounter];
                    List<classSearchParameters> lstSP = formWords.instance.Search_GetParameters(ref cBtnArray, lstWords);
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
            }
        }

        private void Rtx_GotFocus(object sender, EventArgs e)
        {

        }

        public panelDefinitionPort Copy()
        {
            classDictionary cDictionary_Ref = cDictionary;
            classSearchResult cSearchResult_Ref = cSearchResult;
            panelDefinitionPort pnlRetVal = new panelDefinitionPort(ref cDictionary_Ref, ref cSearchResult_Ref);
            pnlRetVal._cSearchResult = cSearchResult.Copy();
            rtx.SelectAll();
            pnlRetVal.rtx.SelectedRtf = rtx.SelectedRtf;
            rtx.Select(0, 0);
            return pnlRetVal;
        }

        private void btn_Minimize_Clicked(object sender, EventArgs e)
        {

        }

        private void TmrMouseHover_Tick(object sender, EventArgs e)
        {
            tmrMouseHover_Enabled = false;
            panelDictionaryOutput.WordUnderMouse_PopUpDefinition();
        }

        public static void tmrMouseHover_Reset()
        {
            tmrMouseHover_Enabled = false;
            tmrMouseHover_Enabled = (formDictionarySelection.PopUp_Dictionary != null && formDictionarySelection.ePopUpDelayTime != enuPopUpDelayTime.RightMouse_Click);
        }


        public static void tmrMouseHover_Interval_Set()
        {
            switch (formDictionarySelection.ePopUpDelayTime)
            {
                case enuPopUpDelayTime.One_Second:
                    tmrMouseHover_Enabled = true;
                    tmrMouseHover.Interval = 1000;
                    break;

                case enuPopUpDelayTime.Three_Seconds:
                    tmrMouseHover_Enabled = true;
                    tmrMouseHover.Interval = 3000;
                    break;

                case enuPopUpDelayTime.Five_Seconds:
                    tmrMouseHover_Enabled = true;
                    tmrMouseHover.Interval = 5000;
                    break;

                case enuPopUpDelayTime.Ten_Seconds:
                    tmrMouseHover_Enabled = true;
                    tmrMouseHover.Interval = 10000;
                    break;

                case enuPopUpDelayTime.RightMouse_Click:
                    tmrMouseHover_Enabled = false;
                    tmrMouseHover.Interval = int.MaxValue;
                    break;

                default:
                    break;
            }
        }

        public static int tmrMouseHover_Interval
        {
            get
            {
                return tmrMouseHover.Interval;
            }
        }

        public static bool tmrMouseHover_Enabled
        {
            get { return tmrMouseHover.Enabled; }
            set
            {
                tmrMouseHover.Enabled = value;
            }
        }


        #region RTX_Events


        static bool bolIgnoreMouseEnter = false;
        static public bool IgnoreMouseEnter
        {
            get { return bolIgnoreMouseEnter; }
            set { bolIgnoreMouseEnter = value; }
        }
        static bool bolIgnoreMouseUp = false;


        static object rtxOutput_MouseClick_Sender = null;
        static MouseEventArgs rtxOutput_MouseClick_MouseEventArgs = null;


        static private void tmrClick_WaitForDoubleClick_Tick(object sender, EventArgs e)
        {
            tmrClick_WaitForDoubleClick.Enabled = false;

            if (bolIgnoreMouseUp)
            {
                bolIgnoreMouseUp = false;
                return;
            }
            panelDictionaryOutput.rtxCalling = (RichTextBox)rtxOutput_MouseClick_Sender;// (RichTextBox)sender;
            object objSender = (object)panelDictionaryOutput.rtxCalling;
            switch (rtxOutput_MouseClick_MouseEventArgs.Button)
            {
                case MouseButtons.Left:
                    {
                        panelDictionaryOutput.WordUnderMouse_Search(objSender, new Point(rtxOutput_MouseClick_MouseEventArgs.X,
                                                                                         rtxOutput_MouseClick_MouseEventArgs.Y));
                    }
                    break;

                case MouseButtons.Right:
                    {
                        panelDictionaryOutput.WordUnderMouse_PopUpDefinition(objSender);
                    }
                    break;

                case MouseButtons.Middle:
                    break;
            }
        }


        static DateTime dtMouseDown_Once = DateTime.Now;

        public bool SearchTextLoad_MouseClick(ref RichTextBox rtx)
        {
            if (!bolWordAnalyzerSearch) return false;

            string strWordSelected = StringLibrary.classStringLibrary.getClickedWord(ref rtx);

            return SearchTextLoad_MouseClick(strWordSelected);
        }

        public bool SearchTextLoad_MouseClick(string strWord)
        {

            if (!bolWordAnalyzerSearch) return false;
            if (string.Compare(groupboxWordAnalyzer.instance.txtSearch.Text.Trim().ToUpper(), strWord.Trim().ToUpper()) != 0)
                groupboxWordAnalyzer.instance.txtSearch.Text = strWord;
            else
                groupboxWordAnalyzer.instance.Search(strWord);

            bolWordAnalyzerSearch = false;
            return true;
        }


        public static void RtxOutput_MouseDown(object sender, MouseEventArgs e)
        {
            rtxOutput_MouseClick_MouseEventArgs = e;
            rtxOutput_MouseClick_Sender = sender;

            RichTextBox rtxSender = (RichTextBox)sender;
            panelDefinitionPort pnlDefPort_Sender = (panelDefinitionPort)rtxSender.Parent;
            if (pnlDefPort_Sender.bolWordAnalyzerSearch)
            if (pnlDefPort_Sender.SearchTextLoad_MouseClick(ref rtxSender))
                return;

            DateTime dtNow = DateTime.Now;
            TimeSpan tsSinceLastClick = dtNow.Subtract(dtMouseDown_Once);
            long lngMillisSinceLastClick = (long)tsSinceLastClick.TotalMilliseconds;
            bool bolDoubleclick = lngMillisSinceLastClick < 600;

            dtMouseDown_Once = dtNow;

            if (bolDoubleclick)
            {
                tmrClick_WaitForDoubleClick.Enabled = false;
                rtxOutput_MouseClick_MouseEventArgs = e;

                if (panelDictionaryOutput.instance != null)
                    panelDictionaryOutput.WordUnderMouse_Insert(sender, new Point(e.X, e.Y));
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    int intIndexSelected = rtxSender.GetCharIndexFromPosition(new Point(e.X, e.Y));
                    rtxSender.Select(intIndexSelected >= 0 && intIndexSelected < rtxSender.Text.Length
                                                    ? intIndexSelected
                                                    : 0,
                                     0);

                    panelDictionaryOutput.WordUnderMouse_PopUpDefinition(sender);
                }
                else
                {
                    tmrClick_WaitForDoubleClick.Interval = 500;
                    tmrClick_WaitForDoubleClick.Enabled = false;
                    tmrClick_WaitForDoubleClick.Enabled = true;
                }
            }
        }
        void btnWordLists_Click(object sender, EventArgs e)
        {
            pnlWordLists.Visible = !pnlWordLists.Visible;
        }

        private void btnWordAnalyzerSearch_Click(object sender, EventArgs e)
        {
            bolWordAnalyzerSearch = !bolWordAnalyzerSearch;
        }


        bool _bolWordAnalyzerSearch = false;
        public bool bolWordAnalyzerSearch
        {
            get { return _bolWordAnalyzerSearch; }
            set
            {
                _bolWordAnalyzerSearch = value;
                btnSearchText_Load_SetText();
            }
        }

        void btnSearchText_Load_SetText()
        {
            btnWordAnalyzer_Search.Text = bolWordAnalyzerSearch ? "!" : "-";
        }

        

        static long lngDTStart = 0;
        public static void RtxOutput_VScroll(object sender, EventArgs e)
        {
            tmrMouseHover_Reset();
            RichTextBox rtxSender = (RichTextBox)sender;
            panelDefinitionPort pnlSender = (panelDefinitionPort)rtxSender.Parent;
            pnlSender.tmrHighLightSearchedWord_Reset();
            lngDTStart = DateTime.Now.Ticks;
        }

        public static void RtxOutput_MouseMove(object sender, MouseEventArgs e)
        {
            tmrMouseHover_Reset();
            lngDTStart = DateTime.Now.Ticks;
            panelDictionaryOutput.ptRtxMouseLocation.X = e.X;
            panelDictionaryOutput.ptRtxMouseLocation.Y = e.Y;
        }

        public static void RtxOutput_MouseLeave(object sender, EventArgs e)
        {
            tmrMouseHover_Enabled = false;
            formWords.RTX_Focused.Focus();
        }

        public static void RtxOutput_MouseWheel(object sender, MouseEventArgs e)
        {
            //RichTextBox rtxSender = (RichTextBox)sender;
            //if (rtxSender.Parent != null)
            //{
            //    try
            //    {
            //        panelDefinitionPort pnlDefPortSender = (panelDefinitionPort)rtxSender.Parent;
            //        pnlDefPortSender.ZoomFactor = rtxSender.ZoomFactor;

            //        formWords.instance.Text = "panelDefinitionPort.rtxOutput_MouseWheel ZoomFactor = " + rtxSender.ZoomFactor.ToString();
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}
        }

        public static void RtxOutput_MouseEnter(object sender, EventArgs e)
        {
            if (bolIgnoreMouseUp) return;
            

            tmrMouseHover_Enabled = (formDictionarySelection.PopUp_Dictionary != null && formDictionarySelection.ePopUpDelayTime != enuPopUpDelayTime.RightMouse_Click);
            RichTextBox rtxSender = (RichTextBox)sender;

            panelDefinitionPort pnlSender = (panelDefinitionPort)rtxSender.Parent;
            if (pnlSender.cmbHeadings.Focused) return;

            panelDictionaryOutput.rtxUnderMouse = rtxSender;
            rtxSender.Focus();
        }


        public static void RtxOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (formWords.RTX_Focused != null)
            {
                RichTextBox rtx = formWords.RTX_Focused;
                if (!rtx.Focused)
                    rtx.Focus();
                SendKeys.Send(e.KeyChar.ToString());
            }
        }
        public static void RtxOutput_KeyUp(object sender, KeyEventArgs e)
        {
            object objSender = (object)formWords.RTX_Focused;
            e.SuppressKeyPress = true;
        }

        void Move_Up()
        {
            IgnoreMouseEnter = true;
            {
                Panel pnlContainer = (Panel)Parent;
                if (panelDictionaryOutput.instance != null)
                    panelDictionaryOutput.instance.pnlSP_MoveUp(ref pnlContainer);
                rtx.Focus();
            }
            IgnoreMouseEnter = false;
        }

        void Move_Down()
        {

            IgnoreMouseEnter = true;
            {
                Panel pnlContainer = (Panel)Parent;
                if (panelDictionaryOutput.instance != null)
                    panelDictionaryOutput.instance.pnlSP_MoveDown(ref pnlContainer);
                rtx.Focus();
            }
            IgnoreMouseEnter = false;
        }


        public static void RtxOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (formWords.RTX_Focused != null)
            {
                RichTextBox rtx = formWords.RTX_Focused;
                if (!rtx.Focused)
                {
                    if (e.Modifiers == Keys.Control)
                    {
                        RichTextBox rtxSender = (RichTextBox)sender;
                        if (rtxSender.Tag != null)
                        {
                            try
                            {
                                panelDefinitionPort pnlDefPort = (panelDefinitionPort)rtxSender.Tag;
                                Panel pnlContainer = (Panel)pnlDefPort.Parent;
                                switch (e.KeyCode)
                                {
                                    case Keys.Up:
                                        {
                                            pnlDefPort.Move_Up();
                                            e.SuppressKeyPress = true;
                                            return;
                                        }

                                    case Keys.Down:
                                        {
                                            pnlDefPort.Move_Down();
                                            e.SuppressKeyPress = true;
                                            return;
                                        }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.ControlKey:
                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            return;
                    }

                    rtx.Focus();
                    formWords.instance.rtxCK_KeyDown(rtx, e);
                    e.SuppressKeyPress = true;
                }
            }
        }
        #endregion
        private void Rtx_LostFocus(object sender, EventArgs e)
        {
            //formDictionaryOutput.ZoomFactor = rtx.ZoomFactor;
        }

        static panelDefinitionPort _pnlDefPort_Caller = null;
        static public panelDefinitionPort pnlDefPort_Caller
        {
            get { return _pnlDefPort_Caller; }
            set { _pnlDefPort_Caller = value; }
        }

        void LblDictionaryHeading_Click(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (this.RhymeDictionary)
                        {
                            panelDefinitionPort pnlMyRef = this;
                            formWords.instance.RhymeDictionarySearch(ref pnlMyRef);
                        }
                        else
                        {
                            pnlDefPort_Caller = this;
                            List<string> lstWords = new List<string>();

                            lstWords.Add(formWords.ckRTX.WordUnderCursor(""));
                            List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                            lstSearchParameters.Add(new classSearchParameters(ref _cDictionary, ref lstWords, eSearchType));
                            formWords.instance.Search(ref lstSearchParameters, enuSearchRequestor.DefinitionPort_Heading_Click);
                        }
                    }
                    break;

                case MouseButtons.Right:
                    {
                        if (formWords.Debug)
                        {
                            if (tmrDebug == null)
                            {
                                tmrDebug = new System.Windows.Forms.Timer();
                                tmrDebug.Interval = 5000;
                                tmrDebug.Tick += TmrDebug_Tick;
                            }
                            if (Filename.Length > 12)
                                lblDictionaryHeading.Text = Filename.Substring(Filename.Length - 12, 8);
                            tmrDebug.Enabled = true;
                        }
                    }
                    break;
            }
        }

        private void LblDictionaryHeading_MouseLeave(object sender, EventArgs e)
        {
            if (cSearchResult == null || cSearchResult.cDictionary == null)
            {
                if (this.RhymeDictionary)
                {
                    lblDictionaryHeading.Text = Heading;
                }
                else
                    return;
            }
            else
            {
                lblDictionaryHeading.Text = cSearchResult.cDictionary.Heading;
            }
        }

        private void LblDictionaryHeading_MouseEnter(object sender, EventArgs e)
        {
            if (cSearchResult == null) return;
            string strPrefix = (cSearchResult.Prefix.Length > 0
                                                    ? "(" + cSearchResult.Prefix + ") "
                                                    : "");

            string strFind = classStringLibrary.clean_nonAlpha_Content(cSearchResult.Find.Trim());

            string strSuffix = (cSearchResult.Suffix.Length > 0
                                                    ? " (" + cSearchResult.Suffix + ")"
                                                    : "");

            lblDictionaryHeading.Text = strPrefix + strFind + strSuffix;
        }

        private void LblDictionaryHeading_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                Move_Up();
            else
                Move_Down();

            Label lblSender = (Label)sender;
            Control ctrRef = (Control)lblSender;
            Point ptMousePosition = MousePosition;
            Point ptTL_lblSender = Ck_Objects.classControlLocation.Location(ref ctrRef);

            Cursor.Position = new Point(ptTL_lblSender.X + lblSender.Width / 2, ptTL_lblSender.Y + lblSender.Height / 2);
            
            //Point ptMouseRelToTL = new Point(ptMousePosition.X - ptTLPanel.X, ptMousePosition.Y - ptTLPanel.Y);

        }


        enuSearchType _eSearchType = enuSearchType.Not_Searched;
        public enuSearchType eSearchType
        {
            get { return _eSearchType; }
            set { _eSearchType = value; }
        }

        void TmrDebug_Tick(object sender, EventArgs e)
        {
            tmrDebug.Enabled = false;
            lblDictionaryHeading.Text = Heading;
        }

        void _MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void CmbHeadings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.Count == 0) return;

            classQueueItem cQueueItem = lstQueue[Queue_Index];
            if (cmbHeadings.SelectedIndex >= 0 && cmbHeadings.SelectedIndex < cQueueItem.lstSearchResults.Count)
            {
                classSearchResult cSearchResult = cQueueItem.lstSearchResults[cmbHeadings.SelectedIndex];
                cQueueItem.IndexLast = cmbHeadings.SelectedIndex;
                if (cDictionary != null)
                {
                    string strFileName = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, cSearchResult.strFileName) + "." + cDictionary.eFileExtension.ToString();
                    Load(strFileName);
                    grbDefinitionPort_WordList_Set();
                }
            }
        }

        private void PanelDefinitionPort_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = (e.Y > Height - intResizeTolerance)
                                ? Cursors.HSplit
                                : Cursors.Arrow;
        }

        private void TmrResize_Tick(object sender, EventArgs e)
        {
            if (pnlResize == null)
                goto quit;
            if (MouseButtons != MouseButtons.Left)
                goto quit;
            Control ctrRef = (Control)pnlResize;
            Point ptMousePosition = MousePosition;
            Point ptTLPanel = Ck_Objects.classControlLocation.Location(ref ctrRef);
            Point ptMouseRelToTL = new Point(ptMousePosition.X - ptTLPanel.X, ptMousePosition.Y - ptTLPanel.Y);

            if (ptMouseRelToTL.Y > 15)
            {
                Panel pnlContainer = (Panel)pnlResize.Parent;
                pnlContainer.Height
                            = Height_Default
                            = ptMouseRelToTL.Y;
                if (panelDictionaryOutput.instance != null)
                    panelDictionaryOutput.instance.DefinitionPortPanel_PlacePanels();
            }
            return;
        quit:
            tmrResize.Enabled = false;
            pnlResize = null;
            formWords.instance.rtxCK.rtx.Focus();
            return;
        }

        private void PanelDefinitionPort_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Y > Height - intResizeTolerance)
            {
                pnlResize = this;
                Cursor = Cursors.HSplit;
            }
        }

        private void panelDefinitionPort_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        void btn_Back_Clicked(object sender, EventArgs e)
        {
            Back();
        }

        void btn_Forward_Clicked(object sender, EventArgs e)
        {
            Forward();
        }

        void btn_Reset_Clicked(object sender, EventArgs e)
        {
            Queue_Index = 0;
        }


        public void Reset()
        {
            lstQueue.Clear();
            rtx.Clear();
            rtx.ZoomFactor = 1;
        }

        public void Reset(List<classSearchResult> lstSearchResults)
        {
            lstQueue.Clear();

            lstQueue.Add(new classQueueItem(lstSearchResults));
            if (lstSearchResults.Count > 0)
            {
                _cSearchResult = lstSearchResults[0];
                cDictionary = _cSearchResult.cDictionary;
            }
            else
            {
                _cSearchResult = null;
            }
            rtx.Clear();
            rtx.ZoomFactor = 1;
            Queue_Index = lstQueue.Count - 1;
        }

        public void ShowDefinition(classSearchResult cSearchResult)
        {
            Load(cSearchResult.strFileName);
        }

        //public void Jump(string strFilename, string strHeading)
        public void Jump(List<classSearchResult> lstSearchResults)
        {
            if (lstSearchResults.Count == 0) return;

            if (lstQueue.Count > 0 && Queue_Index < lstQueue.Count)
            {
                classQueueItem cQueueItem = lstQueue[Queue_Index];
                if (cQueueItem == null) return;
                if (cQueueItem.IndexLast >= 0 && cQueueItem.IndexLast < cQueueItem.lstSearchResults.Count)
                {
                    string strQueueItemFilename = cQueueItem.lstSearchResults[cQueueItem.IndexLast].strFileName;

                    if (lstQueue.Count > 0 && string.Compare(strFilename, strQueueItemFilename) == 0) return;
                    while (Queue_Index < lstQueue.Count - 1)
                        lstQueue.RemoveAt(lstQueue.Count - 1);
                }
            }
            lstQueue.Add(new classQueueItem(lstSearchResults));
            Queue_Index = lstQueue.Count - 1;

            tmrHighLightSearchedWord_Reset();

            grbDefinitionPort_WordList_Set();

        }


        void grbDefinitionPort_WordList_Set()
        {
            pnlWordLists.Text_Set(rtx.Text);
            if (Next != null)
                Next.pnlWordLists.Start_Set();

            if (Previous != null)
                Previous.pnlWordLists.End_Set();
        }



        Single sngZoomFactor = 1;
        public Single ZoomFactor
        {
            get { return sngZoomFactor; }
            set
            {
                sngZoomFactor
                    = rtx.ZoomFactor
                    = rtx_Next.ZoomFactor
                    = value;

                tmrHighLightSearchedWord_Reset();
            }
        }


        void Load(string strFilename)
        {
            if (System.IO.File.Exists(strFilename))
            {
                Filename = strFilename;

                rtx.Clear();
                rtx.ZoomFactor = 1.0f;

                switch (cDictionary.eFileExtension)
                {
                    case enuFileExtensions.rtf:
                        {
                            rtx.LoadFile(strFilename);
                            rtx.Select(0, 0);
                            rtx.ScrollToCaret();
                        }
                        break;

                    case enuFileExtensions.txt:
                        {
                            classFileContent cFileContent = new classFileContent(cDictionary.strSourceDirectory, strFilename.Substring(strFilename.Length - 12, 8));
                            if (cFileContent.Heading != null && cFileContent.Definition != null)
                            {
                                RichTextBox rtxRef = rtx_Next;
                                rtxRef.Clear();
                                rtxRef.ZoomFactor = 1;
                                classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.Heading.Trim(), fntWordHeading, Color.Blue, 0);
                                classStringLibrary.RTX_AppendNL(ref rtxRef);
                                if (cFileContent.alt_Heading != null)
                                {
                                    classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.alt_Heading.Trim(), fntWordHeading, Color.LightBlue, 0);
                                    classStringLibrary.RTX_AppendNL(ref rtxRef);
                                }

                                classStringLibrary.RTX_AppendText(ref rtxRef, cFileContent.Definition, fntWordDefinition, Color.Black, 0);
                                rtxRef.Select(0, 0);
                                rtxRef.ScrollToCaret();
                                rtxRef.ZoomFactor = ZoomFactor;
                                RTX_Swap();
                            }
                        }
                        break;
                }
                rtx.ZoomFactor = ZoomFactor;
                //rtx.Refresh();

                tmrHighLightSearchedWord_Reset();




            }
        }


        string strFilename = "";
        public string Filename
        {
            get { return strFilename; }
            set
            {
                //if (string.Compare(strFilename, value.ToUpper()) != 0)
                {
                    strFilename = value.ToUpper();
                    if (strFilename == null) return;
                    if (strFilename.Length == 8)
                    {
                        strFilename = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, strFilename) + "." + cDictionary.eFileExtension.ToString();
                    }
                }
            }
        }


        int intQueue_Index = 0;
        public int Queue_Index
        {
            get { return intQueue_Index; }
            set
            {
                if (//value != intQueue_Index &&
                    value >= 0 &&
                    value < lstQueue.Count)
                {
                    intQueue_Index = value;
                    cmbHeadings_Build();
                    if (cQueueItem != null)
                    {
                        if (cQueueItem.IndexLast >= 0 && cQueueItem.IndexLast < cmbHeadings.Items.Count)
                        {
                            if (cmbHeadings.SelectedIndex != cQueueItem.IndexLast)
                                cmbHeadings.SelectedIndex = cQueueItem.IndexLast;
                            else
                                CmbHeadings_SelectedIndexChanged((object)cmbHeadings, new EventArgs());
                        }
                    }
                    Buttons_Visible();
                }
            }
        }

        public string SearchFind
        {
            get
            {
                classQueueItem cQ = cQueueItem;

                if (cQ != null)
                {
                    if (cQ.lstSearchResults[cQ.IndexLast] != null)
                        return cQ.lstSearchResults[cQ.IndexLast].Find;
                }

                return "";
            }
        }


        classQueueItem cQueueItem
        {
            get
            {
                if (Queue_Index >= 0 && Queue_Index < lstQueue.Count)
                    return lstQueue[Queue_Index];
                else
                    return null;
            }
        }

        void cmbHeadings_Build()
        {
            cmbHeadings.Items.Clear();

            for (int intFileCounter = 0; intFileCounter < cQueueItem.lstSearchResults.Count; intFileCounter++)
            {
                classSearchResult cSearchResult = cQueueItem.lstSearchResults[intFileCounter];
                cmbHeadings.Items.Add(cSearchResult.Find.Trim());
            }
            //    cmbHeadings.SelectedIndex = 0;
        }

        static classLabelButton _btnDebug = null;
        static public classLabelButton btnDebug
        {
            get { return _btnDebug; }
            set
            {
                if (_btnDebug != value)
                {
                    _btnDebug = value;
                    _btnDebug.VisibleChanged += _btnDebug_VisibleChanged;
                }
            }
        }

        private static void _btnDebug_VisibleChanged(object sender, EventArgs e)
        {
            classLabelButton btnSender = (classLabelButton)sender;
            //System.Diagnostics.Debug.Print("btnDebug_VisibleChange(" + btnSender.Text + ") Visible=" + btnSender.Visible);
        }


        public void Buttons_Visible()
        {
            if (btns[(int)enuButtons.back] != null)
                btns[(int)enuButtons.back].Visible = Queue_Index > 0;
            if (btns[(int)enuButtons.forward] != null)
                btns[(int)enuButtons.forward].Visible = Queue_Index < lstQueue.Count - 1;

        }

        public void Back()
        {
            if (intQueue_Index > 0)
                Queue_Index -= 1;
        }

        public void Forward()
        {
            if (intQueue_Index < lstQueue.Count - 1)
                Queue_Index += 1;
        }


        public void rtx_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }


        public const int intGap = 3;
        void placeButtons()
        {
            Point ptTL = new Point(intGap, intGap);
            for (int intButtonCounter = ((int)enuButtons._numButtons) - 1; intButtonCounter >= 0; intButtonCounter--)
            {
                Ck_Objects.classLabelButton btn = btns[intButtonCounter];
                btn.Location = ptTL;
                ptTL.X += btn.Width;
            }
            pnlButtons.Width = btns[0].Right + btns[0].Width + 2 * intGap;
            pnlButtons.Height = btns[0].Bottom + intGap;
        }

        public void placeObjects()
        {
            placeButtons();
            pnlButtons.Location = new Point(Width - pnlButtons.Width, 0);

            cmbHeadings.Location = new Point(pnlButtons.Left - cmbHeadings.Width, pnlButtons.Top);
            
            lblDictionaryHeading.Location = new Point(0, 0);
            lblDictionaryHeading.Width = cmbHeadings.Left - lblDictionaryHeading.Left;
            lblDictionaryHeading.SendToBack();
            rtx_Array[0].Location
                = rtx_Array[1].Location
                = new Point(0, cmbHeadings.Bottom);

            int intHeight = Height - rtx.Top - intResizeTolerance;

            rtx_Array[0].Size
                = rtx_Array[1].Size
                = new Size(Width, intHeight > 0
                                            ? intHeight
                                            : 5);

            pnlWordLists.Location = rtx.Location;
            pnlWordLists.Size = rtx.Size;
        }


        public class panelWordLists : Panel
        {
            Label lblStart = new Label();
            Label lblEnd = new Label();
            TextBox txtStart = new TextBox();
            TextBox txtEnd = new TextBox();

            CheckBox chkStart = new CheckBox();
            CheckBox chkEnd = new CheckBox();

            TextBox txtWordList_Start = new TextBox();
            TextBox txtWordList_End = new TextBox();
            TextBox txtWordList_Common = new TextBox();

            System.Windows.Forms.Timer tmrDoubleClick = new System.Windows.Forms.Timer();
            bool bolWaitingForDoubleClick = false;
            TextBox txtClick = null;
            string strWordClick = "";
            Point ptMouseClick = new Point();

            public panelWordLists(ref panelDefinitionPort pnlDefPort)
            { 
                Hide();
                              
                _pnlDefPort = pnlDefPort;

                txtWordList_Common.ShortcutsEnabled  // prevents the context menu from appearing when the user right-clicks for a definition pop-up
                    = txtWordList_Start.ShortcutsEnabled
                    = txtWordList_End.ShortcutsEnabled
                    = false;

                txtWordList_Common.Font
                    = txtWordList_End.Font
                    = txtWordList_Start.Font
                    = new Font("Arial", 8);

                Controls.Add(lblStart);
                lblStart.Text = "Start";
                lblStart.AutoSize = true;

                Controls.Add(chkStart);
                chkStart.Size = new Size(18, 18);
                chkStart.Text = "";
                chkStart.CheckedChanged += ChkStart_CheckedChanged;

                Controls.Add(txtStart);
                txtStart.TextChanged += TxtStart_TextChanged;
                txtStart.KeyDown += Txt_KeyDown;

                Controls.Add(lblEnd);
                lblEnd.Text = "End";
                lblEnd.AutoSize = true;

                lblEnd.ForeColor
                    = lblStart.ForeColor
                    = Color.White;

                Controls.Add(txtEnd);
                txtEnd.TextAlign = HorizontalAlignment.Right;
                txtEnd.TextChanged += TxtEnd_TextChanged;
                txtEnd.KeyDown += Txt_KeyDown;

                Controls.Add(chkEnd);
                chkEnd.Size = new Size(18, 18);
                chkEnd.Text = "";
                chkEnd.CheckedChanged += ChkEnd_CheckedChanged;

                Controls.Add(txtWordList_Start);
                txtWordList_Start.Multiline = true;
                txtWordList_Start.ScrollBars = ScrollBars.Vertical;
                txtWordList_Start.MouseDown += TxtWordList_MouseDown;

                Controls.Add(txtWordList_Common);
                txtWordList_Common.Multiline = true;
                txtWordList_Common.ScrollBars = ScrollBars.Vertical;
                txtWordList_Common.MouseDown += TxtWordList_MouseDown;

                Controls.Add(txtWordList_End);
                txtWordList_End.Multiline = true;
                txtWordList_End.ScrollBars = ScrollBars.Vertical;
                txtWordList_End.MouseDown += TxtWordList_MouseDown;

                tmrDoubleClick.Interval = 250;
                tmrDoubleClick.Tick += TmrDoubleClick_Tick;


                SizeChanged += PanelWordLists_SizeChanged;
                VisibleChanged += PanelWordLists_VisibleChanged;
            }


            #region Events

            private void ChkStart_CheckedChanged(object sender, EventArgs e)
            {
                Start_Set();
            }

            public void Start_Set()
            {
      if (chkStart.Checked)
                {
                    if (pnlDefPort_Previous != null)
                    {
                        string strPrevious = classStringLibrary.clean_nonAlpha_Ends( pnlDefPort_Previous.cmbHeadings.Text.Trim());
                        if (strPrevious.Length > 0)
                            txtStart.Text = strPrevious[strPrevious.Length - 1].ToString();
                    }
                }
                else
                {
                    txtStart.Text = "";
                }
            }

            private void ChkEnd_CheckedChanged(object sender, EventArgs e)
            {
                End_Set();
            }

            public void End_Set()
            {
                if (chkEnd.Checked)
                {
                    if (pnlDefPort_Next != null)
                    {
                        string strNext = classStringLibrary.clean_nonAlpha_Ends( pnlDefPort_Next.cmbHeadings.Text.Trim());
                        if (strNext.Length > 0)
                            txtEnd.Text = strNext[0].ToString();

                    }
                }
                else
                {
                    txtEnd.Text = "";
                }
            }


            private void Txt_KeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Back:
                    case Keys.Delete:
                    case Keys.Down:
                    case Keys.Control:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        return;

                    default:
                        char chrKeyPressed = (char)e.KeyValue;
                        if (!char.IsLetter(chrKeyPressed))
                            e.SuppressKeyPress = true;
                        return;
                }
            }

            private void TmrDoubleClick_Tick(object sender, EventArgs e)
            {
                tmrDoubleClick.Enabled = false;
                bolWaitingForDoubleClick = false;
                
                panelDictionaryOutput.WordUnderMouse_Search(ref txtClick, ptMouseClick);

            }

            private void TxtWordList_MouseDown(object sender, MouseEventArgs e)
            {
                TextBox txtSender 
                    = txtClick
                    = (TextBox)sender;
                ptMouseClick= new Point(e.X, e.Y);

                txtWordList_Common.ShortcutsEnabled
                    = txtWordList_Start.ShortcutsEnabled
                    = txtWordList_End.ShortcutsEnabled
                    = false;

                int intIndexUnderMouse = txtSender.GetCharIndexFromPosition(ptMouseClick);

                strWordClick = StringLibrary.classStringLibrary.getWordAtSelection(ref txtSender, intIndexUnderMouse);

                
                if (pnlDefPort.bolWordAnalyzerSearch)
                    if (pnlDefPort.SearchTextLoad_MouseClick(strWordClick))
                        return;

                if (e.Button == MouseButtons.Right)
                {
                    //SearchText = strWordUnderMouse;
                    List<string> lstWords = new List<string>();
                    lstWords.Add(strWordClick);
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

                if (!bolWaitingForDoubleClick)
                {
                    bolWaitingForDoubleClick = true;
                    tmrDoubleClick.Enabled = true;
                    return;
                }

                bolWaitingForDoubleClick = false;
                tmrDoubleClick.Enabled = false; 
                if (formWords.ckRTX != null)
                    formWords.ckRTX.Text_ReplaceWord(strWordClick);
            }

            private void TxtEnd_TextChanged(object sender, EventArgs e)
            {
                WordLists_Build();
            }


            private void TxtStart_TextChanged(object sender, EventArgs e)
            {
                WordLists_Build();
            }
            private void PanelWordLists_VisibleChanged(object sender, EventArgs e)
            {
                WordLists_Build();
                BringToFront();
            }

            private void PanelWordLists_SizeChanged(object sender, EventArgs e)
            {
                placeObjects();
            }

            #endregion

            #region Methods


            void WordLists_Build()
            {
                //placeObjects();
                string strWordList_Start = "";
                string strWordList_Common = "";
                string strWordList_End = "";

                string strStart = txtStart.Text.Trim().ToUpper();
                string strEnd = txtEnd.Text.Trim().ToUpper();

                for (int intWordIndex = 0; intWordIndex < lstWords.Count; intWordIndex++)
                {
                    string strWord_Source = lstWords[intWordIndex].Trim();
                    if (strWord_Source.Length >1)
                    {
                        string strWord = lstWords[intWordIndex].ToUpper().Trim();
                        bool bolStart = false;

                        if (strWord.Length > 0)
                        {
                            if (strStart.Length == 0)
                            {
                                bolStart = true;
                            }
                            else if (strWord.Length > strStart.Length)
                            {
                                if (string.Compare(strWord.Substring(0, strStart.Length), strStart) == 0)
                                    bolStart = true;
                            }
                            if (bolStart)
                                strWordList_Start += (strWordList_Start.Length > 0 ? "\r\n" : "")
                                                       + strWord_Source;

                            bool bolEnd = false;
                            if (strEnd.Length == 0)
                                bolEnd = true;
                            else if (strWord.Length > strEnd.Length)
                            {
                                string strWord_TestEnd = strWord.Substring(strWord.Length - strEnd.Length);
                                if (string.Compare(strWord_TestEnd, strEnd) == 0)
                                    bolEnd = true;
                            }

                            if (bolEnd)
                            {
                                strWordList_End += (strWordList_End.Length > 0 ? "\r\n" : "")
                                                        + strWord_Source;
                                if (bolStart)
                                {
                                    strWordList_Common += (strWordList_Common.Length > 0 ? "\r\n" : "")
                                                        + strWord_Source;
                                }
                            }
                        }
                    }
                }

                txtWordList_Start.Text = strWordList_Start;
                txtWordList_Common.Text = strWordList_Common;
                txtWordList_End.Text = strWordList_End;

                txtWordList_Common.Select(0, 0);
                txtWordList_End.Select(0, 0);
                txtWordList_Start.Select(0, 0);
            }

            public void placeObjects()
            {
                txtWordList_Start.Width
                    = txtWordList_Common.Width
                    = txtWordList_End.Width
                    = (Width - 10) / 3;

                txtStart.Top
                    = lblStart.Top
                    = txtEnd.Top
                    = lblEnd.Top
                    = chkStart.Top 
                    = chkEnd.Top 
                    = 1;

                txtStart.Width
                    = txtEnd.Width
                    = (Width - lblStart.Width - lblEnd.Width - chkStart.Width - chkEnd.Width - 15) / 2;

                chkStart.Left = 5;
                lblStart.Left = chkStart.Right;
                txtStart.Left = lblStart.Right;

                txtEnd.Left = Width - chkStart.Left - txtEnd.Width;
                lblEnd.Left = txtEnd.Left - lblEnd.Width;
                chkEnd.Left = lblEnd.Left - chkEnd.Width;

                txtWordList_Start.Top
                    = txtWordList_Common.Top
                    = txtWordList_End.Top
                    = txtEnd.Bottom;

                txtWordList_Start.Left = 5;
                txtWordList_Common.Left = txtWordList_Start.Right;
                txtWordList_End.Left = txtWordList_Common.Right;

                txtWordList_End.Height
                    = txtWordList_Start.Height
                    = txtWordList_Common.Height
                    = Height - txtWordList_Common.Top;
            }

            #endregion


            #region Properties
            panelDefinitionPort _pnlDefPort = null;
            public panelDefinitionPort pnlDefPort
            {
                get { return _pnlDefPort; }
            }

            public panelDefinitionPort pnlDefPort_Previous
            {
                get { return _pnlDefPort.Previous; }
            }

            public panelDefinitionPort pnlDefPort_Next
            {
                get { return _pnlDefPort.Next; }
            }


            string _strText = "";
            string strText
            {
                get { return _strText; }
                set
                {
                    _strText = value;
                    
                        lstWords = classStringLibrary.getFirstWords(strText);
                }
            }
            public void Text_Set(string strText)
            {
                this.strText = strText;
            }


            List<string> _lstWords = null;
            public List<string> lstWords
            {
                get { return _lstWords; }
                set
                {
                    if (value != null)
                        _lstWords = classStringLibrary.Alphabetize_Words(value);
                    else
                        _lstWords = new List<string>();

                    IEnumerable<string> query = _lstWords.Distinct();
                    _lstWords = (List<string>)query.ToList<string>();
                    if (Visible)
                        WordLists_Build();
                }
            }


            #endregion

        }


    }

}


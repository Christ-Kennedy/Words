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
    public enum enuRTX { Main, Notes, Dictionary, HeadingList, PopUp, Copy, _num};
    
    public class panelDictionaryOutput : Panel
    {
        enum enuCursorSelection { Insert, Recall, Delete, Default };
        enuCursorSelection _eCursorSelection = enuCursorSelection.Default;
        enuCursorSelection eCursorSelection
        {
            get { return _eCursorSelection; }
            set
            {
                if (_eCursorSelection != value)
                {
                    _eCursorSelection = value;
                    switch (_eCursorSelection)
                    {
                        case enuCursorSelection.Default:
                            Cursor = Cursors.Default;
                            break;

                        case enuCursorSelection.Insert:
                            Cursor = formWords.cCursors[(int)enuCursors.InsertWord];
                            break;

                        case enuCursorSelection.Delete:
                            Cursor = formWords.cCursors[(int)enuCursors.Garbage];
                            break;

                        case enuCursorSelection.Recall:
                            Cursor = formWords.cCursors[(int)enuCursors.RecallWord]; ;
                            break;
                    }
                }
            }
        }

        public static panelDictionaryOutput instance;

        static Single _sngZoomFactor = 1;
        static public Single ZoomFactor
        {
            get { return _sngZoomFactor; }
            set { _sngZoomFactor = value; }
        }

        static public classMultiButtonPic mbpDevNav_ButtonList = null;

        public RichTextBox rtxOutput
        {
            get
            {
                try
                {
                    return pnlDefPort != null
                                      ? pnlDefPort.rtx
                                      : null;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        public panelDefinitionPort pnlDefPort
        {
            get
            {
                if (pnlSP.lstElements.Count == 0) return null;
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[0];
                Panel pnlContainer = (Panel)cEle.obj;
                panelDefinitionPort pnlDefPort = (panelDefinitionPort)pnlContainer.Controls[0];
                return pnlDefPort;
            }
        }

        public static ck_RichTextBox ckRTX { get { return formWords.ckRTX; } }

        static public RichTextBox _rtxUnderMouse = null;
        static public RichTextBox rtxUnderMouse
        {
            get { return _rtxUnderMouse; }
            set { _rtxUnderMouse = value; }
        }

        public SplitContainer spt0 = new SplitContainer();
        public SplitContainer spt1 = new SplitContainer();

        public panelSP.panelSP pnlSP = new panelSP.panelSP();
        Ck_Objects.classLabelButton btnWordUnderMouse_Delete = new Ck_Objects.classLabelButton();
        Ck_Objects.classLabelButton btnWordUnderMouse_MoveLeft = new Ck_Objects.classLabelButton();
        Ck_Objects.classLabelButton btnWordUnderMouse_MoveRight = new Ck_Objects.classLabelButton();
        Ck_Objects.classLabelButton btnWordUnderMouse_Capitalize = new Ck_Objects.classLabelButton();


        public static Point ptRtxMouseLocation = new Point();
        formFindReplace _frmFindReplace;
        ListBox lbxResults = new ListBox();

        static Semaphore semHoverWord = new Semaphore(1, 1);

        public static List<string> lstRTFDictionaries = new List<string>();
        string strDirectoryAndFileName = "";

        public panelDictionaryOutput()
        {
            instance = this;
            _frmFindReplace = formWords.instance.frmFindReplace;

            spt0.Orientation = Orientation.Horizontal;
            spt1.Orientation = Orientation.Horizontal;
            spt0.Dock = DockStyle.Fill;
            VisibleChanged += new EventHandler(panelDictionaryOutput_VisibleChanged);

            Controls.Add(spt0);
            //spt0.Dock = DockStyle.Fill;
            {
                spt0.Panel1.Controls.Add(lbxResults);
                lbxResults.ContextMenu = new ContextMenu();
                lbxResults.ContextMenu.Popup += cMnu_PopUp;
                lbxResults.SelectedIndexChanged += LbxResults_SelectedIndexChanged;
                lbxResults.MouseLeave += LbxResults_MouseLeave;
                lbxResults.MouseEnter += LbxResults_MouseEnter;
                lbxResults.MouseMove += LbxResults_MouseMove;
                lbxResults.MouseClick += LbxResults_MouseClick;
                lbxResults.MouseDoubleClick += LbxResults_MouseDoubleClick;
                lbxResults.Dock = DockStyle.Fill;
                lbxResults.Font = new Font("Arial", 10);

                spt0.Panel2.Controls.Add(spt1);
            }

            mbpDefPort_ButtonList_Build();
            spt1.Dock = DockStyle.Fill;
            spt1.Panel1.Controls.Add(mbpDevNav_ButtonList);
            spt1.Panel2.Controls.Add(pnlSP);
            pnlSP.Dock = DockStyle.Fill;

            Controls.Add(btnWordUnderMouse_Delete);
            btnWordUnderMouse_Delete.Text = "X";
            btnWordUnderMouse_Delete.AutoSize = true;
            btnWordUnderMouse_Delete.Forecolor_Highlight
                    = btnWordUnderMouse_Delete.Backcolor_Idle
                    = Color.Red;
            btnWordUnderMouse_Delete.Forecolor_Idle
                    = btnWordUnderMouse_Delete.Backcolor_Highlight
                    = Color.Black;
            btnWordUnderMouse_Delete.Click += btnWordUnderMouse_Delete_Click;
            btnWordUnderMouse_Delete.MouseEnter += btnWordUnderMouse_Delete_MouseEnter;
            btnWordUnderMouse_Delete.MouseLeave += btnWordUnderMouse_MouseLeave;

            Controls.Add(btnWordUnderMouse_MoveLeft);
            btnWordUnderMouse_MoveLeft.Text = "<";
            btnWordUnderMouse_MoveLeft.AutoSize = true;
            btnWordUnderMouse_MoveLeft.Forecolor_Highlight
                    = btnWordUnderMouse_MoveLeft.Backcolor_Idle
                    = Color.Green;
            btnWordUnderMouse_MoveLeft.Forecolor_Idle
                    = btnWordUnderMouse_MoveLeft.Backcolor_Highlight
                    = Color.Black;
            btnWordUnderMouse_MoveLeft.Click += BtnWordUnderMouse_MoveLeft_Click;
            btnWordUnderMouse_MoveLeft.MouseEnter += btnWordUnderMouse_MoveWord_Left_MouseEnter;
            btnWordUnderMouse_MoveLeft.MouseLeave += btnWordUnderMouse_MouseLeave;

            Controls.Add(btnWordUnderMouse_MoveRight);
            btnWordUnderMouse_MoveRight.Text = ">";
            btnWordUnderMouse_MoveRight.AutoSize = true;
            btnWordUnderMouse_MoveRight.Forecolor_Highlight
                    = btnWordUnderMouse_MoveRight.Backcolor_Idle
                    = Color.Green;
            btnWordUnderMouse_MoveRight.Forecolor_Idle
                    = btnWordUnderMouse_MoveRight.Backcolor_Highlight
                    = Color.Black;
            btnWordUnderMouse_MoveRight.Click += BtnWordUnderMouse_MoveRight_Click;
            btnWordUnderMouse_MoveRight.MouseEnter += btnWordUnderMouse_MoveWord_Right_MouseEnter;
            btnWordUnderMouse_MoveRight.MouseLeave += btnWordUnderMouse_MouseLeave;

            Controls.Add(btnWordUnderMouse_Capitalize);
            btnWordUnderMouse_Capitalize.Text = ">";
            btnWordUnderMouse_Capitalize.AutoSize = true;
            btnWordUnderMouse_Capitalize.Forecolor_Highlight
                    = btnWordUnderMouse_Capitalize.Backcolor_Idle
                    = Color.Green;
            btnWordUnderMouse_Capitalize.Forecolor_Idle
                    = btnWordUnderMouse_Capitalize.Backcolor_Highlight
                    = Color.Black;
            btnWordUnderMouse_Capitalize.Click += BtnWordUnderMouse_Capitalize_Click;
            btnWordUnderMouse_Capitalize.MouseEnter += btnWordUnderMouse_Capitalize_MouseEnter;
            btnWordUnderMouse_Capitalize.MouseLeave += btnWordUnderMouse_MouseLeave;

            btnWordUnderMouse_MoveLeft.Font
                = btnWordUnderMouse_MoveRight.Font
                = btnWordUnderMouse_Delete.Font
                = btnWordUnderMouse_Capitalize.Font
                = new Font("arial", 10, FontStyle.Bold);

            mbpDevNav_ButtonList.Show();
            mbpDevNav_ButtonList.placeButtons();
            mbpDevNav_ButtonList.MouseLeave += MbpDevNav_ButtonList_MouseLeave;

            spt0.SplitterDistance = spt0_Distance;
            spt0.SplitterMoved += spt_SplitterMoved;

            spt1.SplitterDistance = spt1_Distance;
            spt1.SplitterMoved += spt_SplitterMoved;

            Disposed += new EventHandler(panelDictionaryOutput_Disposed);
            SizeChanged += new EventHandler(panelDictionaryOutput_SizeChanged);
            GotFocus += new EventHandler(panelDictionaryOutput_GotFocus);
            LostFocus += new EventHandler(panelDictionaryOutput_LostFocus);
            MouseLeave += panelDictionaryOutput_MouseLeave;
            TextChanged += panelDictionaryOutput_TextChanged;
        }

        private void BtnWordUnderMouse_MoveRight_Click(object sender, EventArgs e)
        {
            if (ckRTX != null)
                ckRTX.WordUnderMouse_MoveRight();
        }

        private void BtnWordUnderMouse_MoveLeft_Click(object sender, EventArgs e)
        {
            if (ckRTX != null)
                ckRTX.WordUnderMouse_MoveLeft();
        }

        private void btnWordUnderMouse_Delete_MouseEnter(object sender, EventArgs e)
        {
            Cursor = formWords.cCursors[(int)enuCursors.Garbage];
        }

        private void MbpDevNav_ButtonList_MouseLeave(object sender, EventArgs e)
        {
            Point ptMouse = MousePosition;
            Control ctrlMbpDevNav = (Control)mbpDevNav_ButtonList;
            Point ptTL = Ck_Objects.classControlLocation.Location(ref ctrlMbpDevNav);

            if (ptMouse.X <= ptTL.X || ptMouse.X >= ptTL.X + mbpDevNav_ButtonList.Width || ptMouse.Y <= ptTL.Y || ptMouse.Y >= ptTL.Y + mbpDevNav_ButtonList.Height)
            {
                eCursorSelection = enuCursorSelection.Default;
            }
        }

        private void btnWordUnderMouse_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void btnWordUnderMouse_MoveWord_Left_MouseEnter(object sender, EventArgs e)
        {
            Cursor = formWords.cCursors[(int)enuCursors.MoveWord_Left];
        }


        private void btnWordUnderMouse_MoveWord_Right_MouseEnter(object sender, EventArgs e)
        {
            Cursor = formWords.cCursors[(int)enuCursors.MoveWord_Right];
        }

        private void btnWordUnderMouse_Delete_Click(object sender, EventArgs e)
        {
            if (ckRTX != null)
            {
                ckRTX.DeleteWordUnderCursor();
            }
        }

        public void btnWordUnderMouse_Capitalize_SetText()
        {
            if (ckRTX != null)
            {
                string strWordUnderMouse = ckRTX.WordUnderCursor();
                if (strWordUnderMouse != null && strWordUnderMouse.Length > 0)
                {
                    char chrFront = strWordUnderMouse[0];
                    chrFront = char.IsUpper(chrFront) ? char.ToLower(chrFront) : char.ToUpper(chrFront);
                    string strWordReplace = chrFront.ToString() + strWordUnderMouse.Substring(1);
                    btnWordUnderMouse_Capitalize.Text = strWordReplace;
                }
            }
        }

        void BtnWordUnderMouse_Capitalize_Click(object sender, EventArgs e)
        {
            formWords.ckRTX.Text_ReplaceWord(btnWordUnderMouse_Capitalize.Text, true);
            btnWordUnderMouse_Capitalize_SetText();
        }

        void btnWordUnderMouse_Capitalize_MouseEnter(object sender, EventArgs e)
        {
            Cursor = formWords.cCursors[(int)enuCursors.Capitalize];
        }

        public void DefinitionPortList_DeleteLast()
        {
            if (pnlSP.lstElements.Count > 0)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[0];
                Panel pnlContainer = (Panel)cEle.obj;
                pnlSP.Sub(ref pnlContainer);
            }
            DefinitionPortPanel_PlacePanels();
            formWords.instance.rtxCK.rtx.Focus();
        }


        public void DefinitionPortList_ClearAll()
        {
            while (pnlSP.lstElements.Count > 0)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[0];
                Panel pnlContainer = (Panel)cEle.obj;
                pnlSP.Sub(ref pnlContainer);
            }
            DefinitionPortPanel_PlacePanels();
            formWords.instance.rtxCK.rtx.Focus();
        }

        private void cMnu_PopUp(object sender, EventArgs e)
        {
            //lbxResults.ContextMenu.MenuItems.Clear();
            //lbxResults.ContextMenu.MenuItems.Add(new MenuItem("Top Most", btnTopMost_Click));
        }

        private void panelDictionaryOutput_TextChanged(object sender, EventArgs e)
        {

        }

        static RichTextBox _rtx_Calling = null;
        static public RichTextBox rtxCalling
        {
            get { return _rtx_Calling; }
            set { _rtx_Calling = value; }
        }

        static int _intspt0_Distance = 45;
        static public int spt0_Distance
        {
            get { return _intspt0_Distance; }
            set { _intspt0_Distance = value; }
        }

        static int _intspt1_Distance = 45;
        static public int spt1_Distance
        {
            get { return _intspt1_Distance; }
            set { _intspt1_Distance = value; }
        }


        private void spt_SplitterMoved(object sender, SplitterEventArgs e)
        {
            placeObjects();
            spt0_Distance = spt0.SplitterDistance;
            spt1_Distance = spt1.SplitterDistance;
        }


        private void panelDictionaryOutput_MouseLeave(object sender, EventArgs e)
        {
            bolMouseOverLbxResults = false;
        }



        static public void WordUnderMouse_PopUpDefinition() { WordUnderMouse_PopUpDefinition((object)rtxUnderMouse); }
        static public void WordUnderMouse_PopUpDefinition(object sender)
        {
            if (formPopUp.instance != null && formPopUp.instance.Visible) return;

            semHoverWord.WaitOne();

            RichTextBox rtxSender = (RichTextBox)sender;

            string strWordUnderMouse = classStringLibrary.getWordUnderMouse(ref rtxSender, ptRtxMouseLocation);

            if (strWordUnderMouse != null && strWordUnderMouse.Length > 0)
            {

                SearchText = strWordUnderMouse;
                List<string> lstWords = new List<string>();
                lstWords.Add(SearchText);
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
            }
            else
            {
                if (formPopUp.instance != null
                            &&
                    !formPopUp.instance.IsDisposed)
                    formPopUp.instance.Hide();
            }
            semHoverWord.Release();
        }

        public static void WordUnderMouse_Insert(object sender, Point ptMouse)
        {
            RichTextBox rtxSender = (RichTextBox)sender;
            panelDefinitionPort pnlDefPort = (panelDefinitionPort)rtxSender.Parent;

            int intSelection = rtxSender.GetCharIndexFromPosition(ptMouse);

            int intStartWord = intSelection - 1;

            char chrStart = rtxSender.Text[intStartWord];
            while (intStartWord > 0 && ck_RichTextBox.Char_IsValid(chrStart))
                chrStart = rtxSender.Text[--intStartWord];

            if (!ck_RichTextBox.Char_IsValid(chrStart))
                chrStart = rtxSender.Text[++intStartWord];
            if (intStartWord < 0)
                intStartWord = 0;

            int intEndWord = intSelection;
            char chrEnd = rtxSender.Text[intEndWord];
            while (intEndWord < rtxSender.Text.Length-1  && ck_RichTextBox.Char_IsValid(chrEnd))
            {
                intEndWord++;
                chrEnd = rtxSender.Text[intEndWord];
            }
            bool bolExtraChar = false;
            if (intEndWord >= rtxSender.Text.Length - 1)
            {
                intEndWord = rtxSender.Text.Length - 1;
                bolExtraChar = true;
            }

            if (intEndWord > intStartWord + 1 && intStartWord >= 0)
            {
                string strText = (rtxSender.Text.Substring(intStartWord, intEndWord - intStartWord+(bolExtraChar ? 1:0))).Trim();

                if (pnlDefPort != null)
                {
                    if (formWords.grbOptions.Toggle_Insert_Search_Retains_Prefix_and_Suffix)
                        strText = pnlDefPort.cSearchResult.Prefix + strText + pnlDefPort.cSearchResult.Suffix;
                }

                RichTextBox rtxFocus = formWords.RTX_Focused;
                if (rtxFocus.Parent != null)
                {
                    try
                    {
                        ck_RichTextBox ckFocus = (ck_RichTextBox)rtxFocus.Parent;
                        ckFocus.Text_ReplaceWord(strText);
                    }
                    catch (Exception)
                    {
                        formWords.ckRTX.Text_ReplaceWord(strText);
                    }
                }

                //formWords.ckRTX.Text_ReplaceWord(strText);
            }
        }


        public static enuRTX RTX_getID(ref RichTextBox rtx)
        {
            if (formWords.instance == null || formWords.instance.IsDisposed) return enuRTX._num;

            for (int intCounter = 0; intCounter < (int)enuRTX._num; intCounter++)
            {
                enuRTX eRTX = (enuRTX)intCounter;
                switch (eRTX)
                {
                    case enuRTX.Main:
                        {
                            if (rtx == formWords.instance.rtxCK.rtx)
                                return eRTX;
                        }
                        break;

                    case enuRTX.Notes:
                        {
                            if (rtx == formWords.instance.grbNotes.rtxNotes.rtx)
                                return eRTX;
                        }
                        break;

                    case enuRTX.HeadingList:
                        {
                            if (rtx == groupboxHeadingList.instance.pnlDefPort.rtx)
                                return eRTX;
                        }
                        break;

                    case enuRTX.Copy:
                        {
                            return eRTX;
                        }

                    case enuRTX.PopUp:
                        {
                            if (formPopUp.instance == null)
                                new formPopUp();
                            if (rtx == formPopUp.instance.rtx)
                                return eRTX;
                        }
                        break;
                }
            }

            return enuRTX._num;
        }

        public static void WordUnderMouse_Search(object sender, Point ptMouse)
        {
            RichTextBox rtxSender = (RichTextBox)sender;

            SearchText
                = strHeading
                = classStringLibrary.getWordUnderMouse(ref rtxSender, ptMouse);

            if (SearchText != null
                && SearchText.Length > 0)
            {
                List<string> lstWords = new List<string>();
                lstWords.Add(SearchText);
                classDictionary cDictionary = null;

                enuRTX eRTX = RTX_getID(ref rtxSender);
                enuSearchRequestor eSearchRequester = enuSearchRequestor.DefinitionPort_Click;

                switch (eRTX)
                {
                    case enuRTX.PopUp:
                        {
                            cDictionary = formPopUp.instance.cDictionary;
                            eSearchRequester = enuSearchRequestor.PopUp;
                        }
                        break;

                    case enuRTX.Copy:
                        {
                            panelDefinitionPort pnlDefinitionCopy = (panelDefinitionPort)rtxSender.Tag;
                            cDictionary = pnlDefinitionCopy.cDictionary;
                        }
                        break;

                    case enuRTX.HeadingList:
                        {
                            cDictionary = groupboxHeadingList.instance.cDictionary;
                        }
                        break;

                    default:
                        {
                            cDictionary = formDictionarySelection.PopUp_Dictionary;
                        }
                        break;
                }

                if (cDictionary != null)
                {
                    List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                    lstSearchParameters.Add(new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Heading));
                    formWords.instance.strClickedWord = SearchText;
                    rtxDefinitionPort_Click = rtxSender;
                    formWords.instance.Search(ref lstSearchParameters, eSearchRequester);
                }
            }
        }

        public static void WordUnderMouse_Search(ref TextBox txt, Point ptMouse)
        {
            SearchText
                = strHeading
                = classStringLibrary.getWordUnderMouse(ref txt, ptMouse);

            if (SearchText != null
                && SearchText.Length > 0)
            {
                List<string> lstWords = new List<string>();
                lstWords.Add(SearchText);
                classDictionary cDictionary = null;
                enuSearchRequestor eSearchRequester = enuSearchRequestor.DefinitionPort_Click;

                panelDefinitionPort.panelWordLists pnlWordsList = (panelDefinitionPort.panelWordLists)txt.Parent;
                panelDefinitionPort pnlDefPort = (panelDefinitionPort)pnlWordsList.Parent;
                cDictionary = pnlDefPort.cDictionary;

                if (cDictionary != null)
                {
                    List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                    lstSearchParameters.Add(new classSearchParameters(ref cDictionary, ref lstWords, enuSearchType.Heading));
                    formWords.instance.strClickedWord = SearchText;
                    rtxDefinitionPort_Click = pnlDefPort.rtx;
                    formWords.instance.Search(ref lstSearchParameters, eSearchRequester);
                }
            }
        }

        public void pnlSP_MoveUp(ref Panel pnlContainer)
        {
            int intIndex = pnlSP.IndexOf(ref pnlContainer);
            if (intIndex > 0)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[intIndex];
                pnlSP.lstElements.Remove(cEle);
                pnlSP.lstElements.Insert(intIndex - 1, cEle);
                DefinitionPortPanel_PlacePanels();
            }
        }

        public void pnlSP_MoveDown(ref Panel pnlContainer)
        {
            int intIndex = pnlSP.IndexOf(ref pnlContainer);
            if (intIndex >= 0 && intIndex < pnlSP.lstElements.Count - 1)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[intIndex];
                pnlSP.lstElements.Remove(cEle);
                pnlSP.lstElements.Insert(intIndex + 1, cEle);
                DefinitionPortPanel_PlacePanels();
            }
        }




        List<classSearchResult> _lstHoverSearchResults = null;
        public List<classSearchResult> lstHoverSearchResults
        {
            get { return _lstHoverSearchResults; }
            set
            {
                semHoverWord.WaitOne();
                ReorderList(ref value, formWords.instance.lstSearchParameters[0].lstWords[0]);
                _lstHoverSearchResults = value;

                if (lstHoverSearchResults != null && lstHoverSearchResults.Count > 0)
                {
                    IEnumerable<classSearchResult> query = lstHoverSearchResults.OrderBy(SearchResult => SearchResult.strFileName);
                    _lstHoverSearchResults = (List<classSearchResult>)query.ToList<classSearchResult>();

                    // eliminate all entries that do not start with same letter as searched word
                    if (SearchText.Length > 0)
                    {
                        // find entry whose heading exactly matches searched word
                        string strWordTest = StringLibrary.classStringLibrary.cleanFront_nonAlpha(SearchText.ToLower());
                        bool bolResultFound = false;
                        int intBestResult = -1;
                        int intBestLength = 0;
                        for (int intCounter = 0; intCounter < lstHoverSearchResults.Count; intCounter++)
                        {
                            classSearchResult cSR = lstHoverSearchResults[intCounter];
                            if (cSR.strFileName != null
                                && cSR.strFileName.Length > 0)
                            {
                                string strSR_Find = cSR.Find.ToLower().Trim();
                                if (strSR_Find.Length >= intBestLength)
                                {
                                    int intLengthTest = intBestLength;
                                    while (intLengthTest <= strSR_Find.Length && intLengthTest <= strWordTest.Length)
                                    {
                                        if (string.Compare(strSR_Find.Substring(0, intLengthTest), strWordTest.Substring(0, intLengthTest)) == 0)
                                        {
                                            intBestResult = intCounter;
                                            intBestLength = intLengthTest;
                                            if (strSR_Find.Length == strWordTest.Length)
                                            {
                                                PopUp_Set(intCounter);
                                                bolResultFound = true;
                                                break;
                                            }
                                        }
                                        else
                                            break;

                                        intLengthTest++;
                                    }
                                }
                            }
                            else
                                goto lstHoverSearchResults_Quit;

                        }
                        if (intBestResult >= 0 && !bolResultFound)
                            PopUp_Set(intBestResult);
                    }
                }
            lstHoverSearchResults_Quit:
                semHoverWord.Release();
            }
        }

        void PopUp_Set(int intIndex)
        {
            string strFileName = classFileContent.getFileNameAndDirectory(lstHoverSearchResults[intIndex].cDictionary.strSourceDirectory, lstHoverSearchResults[intIndex].strFileName);

            classDictionary cDictionaryRef = lstHoverSearchResults[intIndex].cDictionary;

            switch (formWords.instance.eSearchRequestor)
            {
                case enuSearchRequestor.PopUp:
                case enuSearchRequestor.definition_Hover:
                    {
                        PopUp(ref cDictionaryRef, strFileName, true);
                    }
                    break;
            }
        }

        //static string _SearchText = "";
        //static string SearchText
        //{
        //    get { return _SearchText; }
        //    set { _SearchText = value; }
        //}



        public void lbxResults_ScrollDown()
        {
            if (lbxResults.SelectedIndex < lbxResults.Items.Count - 1)
            {
                lbxResults.SelectedIndex += 1;
                if (lbxResults.SelectedIndex >= 0 && lbxResults.SelectedIndex < lbxResults.Items.Count)
                    LoadRTX(lbxResults.SelectedIndex);
            }
        }


        public void lbxResults_ScrollUp()
        {
            if (lbxResults.SelectedIndex > 0)
            {
                lbxResults.SelectedIndex -= 1;
                if (lbxResults.SelectedIndex >= 0 && lbxResults.SelectedIndex < lbxResults.Items.Count)
                    LoadRTX(lbxResults.SelectedIndex);
            }
        }

        private void LbxResults_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            int intMouseOverIndex = lbxResults.IndexFromPoint(new Point(e.X, e.Y));
            if (intMouseOverIndex >= 0 && intMouseOverIndex < lbxSender.Items.Count)
            {
                lbxSender.SelectedIndex = intMouseOverIndex;
            }
        }
        bool bolMouseOverLbxResults = false;
        private void LbxResults_MouseEnter(object sender, EventArgs e)
        {
            bolMouseOverLbxResults = true;
        }
        private void LbxResults_MouseLeave(object sender, EventArgs e)
        {
            bolMouseOverLbxResults = false;
            //if (lbxResults.Items.Count > intRtxOutput_Index)
            //    lbxResults.SelectedIndex = intRtxOutput_Index;
            if (formPopUp.instance != null)
                formPopUp.instance.Hide();
        }

        private void LbxResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            if (lbxSender.SelectedIndex >= 0 && lbxSender.SelectedIndex < lbxSender.Items.Count)
            {
                ck_RichTextBox ckRTX = formWords.ckRTX;
                if (ckRTX != null)
                {
                    classSearchResult cSearchResult = lstSearchResults_Main[lbxSender.SelectedIndex];
                    if (cSearchResult.cFileContent == null)
                    {
                        cSearchResult.cFileContent = new classFileContent(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                        ckRTX.Text_Insert(cSearchResult.cFileContent.Heading);
                    }
                }
            }
        }

        private void LbxResults_MouseClick(object sender, MouseEventArgs e)
        {

            if (lbxResults.SelectedIndex >= 0 && lbxResults.SelectedIndex < lbxResults.Items.Count)
                LoadRTX(lbxResults.SelectedIndex);
            return;
        }

        private void LbxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            if (lbxSender.Items.Count != lstSearchResults_Main.Count) return;
            if (bolMouseOverLbxResults && formWords.grbOptions.Toggle_Dictionary_Form_Mouse_Pop_Up_Definition)
            {
                if (lbxSender.SelectedIndex >= 0 && lbxSender.SelectedIndex < lbxSender.Items.Count)
                {
                    if (formPopUp.instance == null || formPopUp.instance.IsDisposed) new formPopUp();
                    string strFileName = classFileContent.getFileNameAndDirectory(lstSearchResults_Main[lbxSender.SelectedIndex].cDictionary.strSourceDirectory, lstSearchResults_Main[lbxSender.SelectedIndex].strFileName);
                    classDictionary cDictionaryRef = lstSearchResults_Main[lbxSender.SelectedIndex].cDictionary;
                    PopUp(ref cDictionaryRef, strFileName);
                }
            }
        }


        static public void PopUp(ref classDictionary cDictionary, string strFileName) { PopUp(ref cDictionary, strFileName, false); }

        static public void PopUp(ref classDictionary cDictionary, string strFileName, bool bolUnderMouse)
        {
            if (formPopUp.instance == null)
                new formPopUp();
            formPopUp.instance.TopMost = true;

            formPopUp.instance.LoadDefinition(ref cDictionary, strFileName);
            if (!bolUnderMouse)
            {
                formPopUp.instance.Location = new Point(MousePosition.X + 15, MousePosition.Y + 15);
                if (formPopUp.instance.Right > Screen.PrimaryScreen.WorkingArea.Width)
                    formPopUp.instance.Left = MousePosition.X - formPopUp.instance.Width - 15;
                if (formPopUp.instance.Bottom > Screen.PrimaryScreen.WorkingArea.Height)
                    formPopUp.instance.Top = MousePosition.Y - formPopUp.instance.Height - 15;
            }
            else
            {
                formPopUp.instance.Location = new Point(MousePosition.X - 15, MousePosition.Y - 15);
                if (formPopUp.instance.Right > Screen.PrimaryScreen.WorkingArea.Width)
                    formPopUp.instance.Left = MousePosition.X - formPopUp.instance.Width + 35;
                if (formPopUp.instance.Bottom > Screen.PrimaryScreen.WorkingArea.Height)
                    formPopUp.instance.Top = MousePosition.Y - formPopUp.instance.Height + 15;
                formPopUp.ptLocation = formPopUp.instance.Location;
            }
            formPopUp.instance.Show();
            formPopUp.instance.BringToFront();
        }
        enuFileExtensions eFileExtension = enuFileExtensions._num;
        int intRtxOutput_Index = -1;


        void LoadRTX(int intLbxResultsIndex)
        {
            List<classRTXInfo> lstNewRTX = new List<classRTXInfo>();
            if (pnlSP.lstElements.Count == 0)
                DefinitionPort_InsertNew();

            RichTextBox rtx = rtxOutput;
            rtx.Clear();
            rtx.ZoomFactor = 1;

            rtx.Text = "";
            intRtxOutput_Index = intLbxResultsIndex;
            if (intLbxResultsIndex >= 0 && intLbxResultsIndex < lstSearchResults_Main.Count)
            {
                classSearchResult cSearchResult = lstSearchResults_Main[intLbxResultsIndex];

                panelDefinitionPort pnlDefPortRef = pnlDefPort;
                if (pnlDefPortRef != null)
                    pnlDefPortRef.cDictionary = cSearchResult.cDictionary;


                pnlDefPortRef.Filename = cSearchResult.strFileName;

                strDirectoryAndFileName = classFileContent.getFileNameAndDirectory(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                eFileExtension = enuFileExtensions._num;

                for (int intExtensionCounter = 0; intExtensionCounter <= (int)enuFileExtensions._num; intExtensionCounter++)
                {
                    eFileExtension = (enuFileExtensions)intExtensionCounter;
                    string strTestFileName = strDirectoryAndFileName + "." + eFileExtension.ToString();
                    if (System.IO.File.Exists(strTestFileName))
                    {
                        strDirectoryAndFileName = strTestFileName;
                        break;
                    }
                }

                switch (eFileExtension)
                {
                    case enuFileExtensions.txt:
                        {
                            classFileContent cFileContent = new classFileContent(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                            try
                            {
                                strHeading = cFileContent.Heading;
                                lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.Heading.Trim(), panelDefinitionPort.enuRTXStyle.WordHeading));
                            }
                            catch (Exception)
                            {

                            }

                            try
                            {
                                if (cFileContent.alt_Heading != null
                                    &&
                                    cFileContent.alt_Heading.Length > 0)
                                    lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.alt_Heading.Trim(), panelDefinitionPort.enuRTXStyle.WordDefinition));
                            }
                            catch (Exception)
                            {

                            }

                            try
                            {
                                lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.Definition.Trim(), panelDefinitionPort.enuRTXStyle.WordDefinition));
                            }
                            catch (Exception)
                            {
                            }
                            lstRTX = lstNewRTX;
                        }
                        break;

                    case enuFileExtensions.rtf:
                        {
                            if (rtxOutput == null || rtxOutput.IsDisposed)
                            {
                                rtxOutput.LoadFile(strDirectoryAndFileName);
                            }
                            else
                            {
                                rtxOutput.LoadFile(strDirectoryAndFileName);
                                int intFirstCR = rtx.Text.IndexOf("\n");
                                if (intFirstCR > 0)
                                    strHeading = rtxOutput.Text.Substring(0, intFirstCR).Trim();

                            }
                        }
                        break;

                    case enuFileExtensions._num:
                    default:
                        { 
                        }
                        break;
                }

                rtxOutput.Select(0, 0);
                rtxOutput.ZoomFactor = ZoomFactor;
                pnlDefPort.tmrHighLightSearchedWord_Reset();
                Show();
            }
        }


        static string _strSearchText = "";
        static public string SearchText
        {
            get { return _strSearchText; }
            set { _strSearchText = value; }
        }
        /// <summary>
        /// reorders list of search results so that exact search finds lead the list
        /// </summary>
        /// <param name="lstResults">list of search results to be reordered</param>
        /// <param name="strWordSearched">word that was searched which should lead the list of results</param>
        void ReorderList(ref List<classSearchResult> lstResults, string strWordSearched)
        {


            string strTest = strWordSearched.Trim().ToUpper();
            int intCounter = lstResults.Count-1;
            List<string> lstFinds = new List<string>();
            List<classSearchResult> lstTemp = new List<classSearchResult>();
            List<classSearchResult> lstBest = new List<classSearchResult>();
            while (intCounter >=0 )
            {
                classSearchResult cResult = lstResults[intCounter];
                string strTemp = classStringLibrary.cleanFront_nonAlpha(cResult.Find.Trim()).ToUpper();
                if (!lstFinds.Contains(cResult.strFileName))
                {
                    if (string.CompareOrdinal(strTemp, strTest) == 0)
                    {
                        lstBest.Add (cResult);
                    }
                    else
                    {
                        lstTemp.Add(cResult);
                    }
                    lstFinds.Add(cResult.strFileName);
                }
                intCounter--;
            }

            lstResults.Clear();

            IEnumerable<classSearchResult> query = lstBest.OrderBy(searchResult => searchResult.Find);
            lstBest = (List<classSearchResult>)query.ToList<classSearchResult>();

            query = lstTemp.OrderBy(searchResult => searchResult.Find);
            lstTemp = (List<classSearchResult>) query.ToList<classSearchResult>();
            lstResults.AddRange(lstBest);
            lstResults.AddRange(lstTemp);
        }



        List<classSearchResult> _lstSearchResults_ = new List<classSearchResult>();
        List<string> lstHeadings = new List<string>();
        public List<classSearchResult> lstSearchResults_Main
        {
            get { return _lstSearchResults_; }
            set
            {
                ReorderList(ref value, formWords.instance.lstSearchParameters[0].lstWords[0]);

                switch (formWords.instance.eSearchRequestor)
                {
                    //case enuSearchRequestor.clipboard:
                    case enuSearchRequestor.main:
                    case enuSearchRequestor.definition_Click:
                        {
                            _lstSearchResults_ = value;

                            if (lstSearchResults_Main.Count > 0)
                            {
                                {
                                    //// set exact result top of the list
                                    //if (formWords.instance.lstSearchParameters.Count > 0 && formWords.instance.lstSearchParameters[0].lstWords.Count > 0)
                                    //{
                                    //    string strTest = formWords.instance.lstSearchParameters[0].lstWords[0].Trim().ToUpper();
                                    //    intCounter = 0;

                                    //    while (intCounter < lstSearchResults_Main.Count )
                                    //    {
                                    //        classSearchResult cResult = lstSearchResults_Main[intCounter];
                                    //        string strTemp = classStringLibrary.cleanFront_nonAlpha(cResult.Find.Trim()).ToUpper();
                                    //        if (!lstFinds.Contains(cResult.strFileName))
                                    //        {
                                    //            if (string.CompareOrdinal(strTemp, strTest) == 0)
                                    //            {
                                    //                lstTopReturns.Insert(0, cResult);
                                    //            }
                                    //            else
                                    //            {
                                    //                lstTopReturns.Add(cResult);
                                    //            }
                                    //            lstFinds.Add(cResult.strFileName);
                                    //        }
                                    //        intCounter++;
                                    //    }
                                    //}

                                    //_lstSearchResults_.Clear();
                                    //_lstSearchResults_.AddRange(lstTopReturns);
                                }
                                lbxResults.Items.Clear();
                                int intCounter = 0;
                                List<string> lstFilenames = new List<string>();
                                while (intCounter < lstSearchResults_Main.Count)
                                {
                                    classSearchResult cResult = lstSearchResults_Main[intCounter];

                                    if (lstFilenames.Contains(cResult.strFileName))
                                    {
                                        lstSearchResults_Main.Remove(cResult);
                                    }
                                    else
                                    {
                                        lstFilenames.Add(cResult.strFileName);
                                        lstHeadings.Add(cResult.Find);
                                        lbxResults.Items.Add(
                                            cResult.cDictionary.Heading.Trim() +
                                            "  :  " + cResult.Find.Trim());


                                        //System.Diagnostics.Debug.Print(cResult.Heading);
                                        intCounter++;
                                    }
                                }
                                lbxResults.SelectedIndex = 0;
                                lbxResults.Refresh();
                                DefinitionPort_InsertNew();
                                pnlDefPort.Reset(lstSearchResults_Main);
                                pnlDefPort.eSearchType = formWords.instance.lstSearchParameters[0].eSearchType;
                                pnlDefPort.lstSearchWords = formWords.instance.lstSearchParameters[0].lstWords;
                                pnlDefPort.ZoomFactor = ZoomFactor;
                                Show();
                            }

                            if (formWords.instance.eSearchRequestor == enuSearchRequestor.main)
                                formWords.RTX_Focused.Focus();
                        }
                        break;

                    case enuSearchRequestor.DefinitionPort_Heading_Click:
                    case enuSearchRequestor.DefinitionPort_Click:
                        {
                            List<classSearchResult> lstSearchResults_Copy = value;
                            List<classSearchResult> lstSR_Temp = new List<classSearchResult>();

                            if (lstSearchResults_Copy.Count > 0)
                            {
                                string strSearchUpperCase = SearchText.ToUpper();

                                for (int intExactCounter = 0; intExactCounter < lstSearchResults_Copy.Count && intExactCounter < 1024; intExactCounter++)
                                {
                                    classSearchResult cResultTest = lstSearchResults_Copy[intExactCounter];
                                    string strTest = classStringLibrary.cleanFront_nonAlpha(cResultTest.Find.Trim().ToUpper());
                                    if (string.Compare(strTest, strSearchUpperCase) == 0)
                                        lstSR_Temp.Add(cResultTest);
                                }

                                if (formWords.instance.eSearchRequestor == enuSearchRequestor.DefinitionPort_Click
                                    && rtxDefinitionPort_Click != null
                                    && rtxDefinitionPort_Click.Tag != null)
                                {

                                    panelDefinitionPort pnlDefinitionCopy = (panelDefinitionPort)rtxDefinitionPort_Click.Tag;
                                    pnlDefinitionCopy.Jump(lstSR_Temp);

                                }
                                else if (formWords.instance.eSearchRequestor == enuSearchRequestor.DefinitionPort_Heading_Click)
                                {
                                    if (panelDefinitionPort.pnlDefPort_Caller != null)
                                        panelDefinitionPort.pnlDefPort_Caller.Reset(lstSearchResults_Copy);
                                }
                            }
                        }
                        break;
                }
            }
        }

        static string _strHeading = "";
        static string strHeading
        {
            get { return _strHeading; }
            set { _strHeading = value; }
        }


        List<classRTXInfo> _lstRTX = new List<classRTXInfo>();
        public List<classRTXInfo> lstRTX
        {
            get { return _lstRTX; }
            set
            {
                _lstRTX = value;
                for (int intRTXCounter = 0; intRTXCounter < lstRTX.Count; intRTXCounter++)
                {
                    classRTXInfo cRTX = lstRTX[intRTXCounter];
                    rtxOutput.SelectionStart = cRTX.start;
                    rtxOutput.SelectionLength = cRTX.stop - cRTX.start;
                    rtxOutput.SelectionColor = cRTX.clr;
                    rtxOutput.SelectionFont = cRTX.fnt;
                }
            }
        }

        public string Title
        {
            get { return Text; }
            set
            {
                Text = value;
            }
        }

        private void BtnHide_Click(object sender, EventArgs e)
        {
            Hide();
        }

        void panelDictionaryOutput_LostFocus(object sender, EventArgs e)
        {

        }


        void panelDictionaryOutput_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        public void placeObjects()
        {
            if (!Visible) return;

            DefinitionPortPanel_PlacePanels();

            btnWordUnderMouse_Delete.Location = new Point(5, spt0.Top + spt0.Panel1.Height - btnWordUnderMouse_Delete.Height);
            btnWordUnderMouse_Delete.BringToFront();

            btnWordUnderMouse_MoveLeft.Location = new Point(btnWordUnderMouse_Delete.Right, btnWordUnderMouse_Delete.Top);
            btnWordUnderMouse_MoveLeft.BringToFront();

            btnWordUnderMouse_MoveRight.Location = new Point(btnWordUnderMouse_MoveLeft.Right, btnWordUnderMouse_Delete.Top);
            btnWordUnderMouse_MoveRight.BringToFront();

            btnWordUnderMouse_Capitalize.Location = new Point(btnWordUnderMouse_MoveRight.Right, btnWordUnderMouse_Delete.Top);
            btnWordUnderMouse_Capitalize.BringToFront();
        }

        public void btnCopy_Delete_click(object sender, EventArgs e)
        {
            Ck_Objects.classLabelButton btnSender = (Ck_Objects.classLabelButton)sender;
            Panel pnlButtons = (Panel)btnSender.Parent;
            panelDefinitionPort pnlDefPort = (panelDefinitionPort)pnlButtons.Parent;
            Panel pnlContainer = (Panel)pnlDefPort.Parent;

            pnlSP.Building = true;
            {
                int intIndex = pnlSP.IndexOf(ref pnlContainer);
                if (intIndex >= 0)
                {
                    for (int intCounter = pnlSP.lstElements.Count - 1; intCounter > intIndex; intCounter--)
                    {
                        panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[intCounter];
                        if (intCounter > 0)
                        {
                            panelSP.classSweepAndPrune_Element cEle_Prev = pnlSP.lstElements[intCounter - 1];
                            cEle.Location = cEle_Prev.Location;
                        }
                        else
                            cEle.Location = new Point();
                    }
                }
                pnlSP.Sub(ref pnlContainer);
            }
            pnlSP.Building = false;
            placeObjects();

            formWords.RTX_Focused.Focus();
        }

        public void DefinitionPortPanel_PlacePanels()
        {
            if (pnlSP.lstElements.Count == 0)
            {
                //btnWordUnderMouse_Delete.Hide();
                return;
            }

            lstPnlPorts.Clear();

            Rectangle recVisible = pnlSP.recVisible;
            int intVBarValue = pnlSP.scrollBar_V.Value;

            pnlSP.Show();
            pnlSP.Building = true;
            {
                Panel pnlContainer_Previous = null;
                panelSP.classSweepAndPrune_Element cEle_Previous = null;

                while (pnlSP.lstElements.Count > formWords.grbOptions.Integer_Definition_Ports_List)
                {
                    panelSP.classSweepAndPrune_Element cSPEleSub = pnlSP.lstElements[pnlSP.lstElements.Count - 1];
                    pnlSP.Sub(ref cSPEleSub);
                }

                for (int intCopyCounter = 0; intCopyCounter < pnlSP.lstElements.Count; intCopyCounter++)
                {
                    panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[intCopyCounter];
                    Panel pnlContainer = (Panel)cEle.obj;

                    pnlContainer.Width = pnlSP.scrollBar_V.Left;
                    pnlContainer.Top = pnlContainer_Previous == null
                                                              ? 0
                                                              : pnlContainer_Previous.Bottom;
                    cEle.recArea = new Rectangle(0,
                                                 cEle_Previous == null
                                                                ? 0
                                                                : cEle_Previous.recArea.Bottom,
                                                 pnlContainer.Width,
                                                 pnlContainer.Height);
                    pnlContainer.Show();

                    try
                    {
                        if (intCopyCounter == 0)
                        { //                        place Clear-All button
                            if (cEle != null)
                            {
                                panelDefinitionPort pnlDefPort = (panelDefinitionPort)pnlContainer.Controls[0];
                                btnWordUnderMouse_Delete.Show();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    cEle_Previous = cEle;


                    //panelDefinitionPort pnlDefPort_Temp = (panelDefinitionPort)pnlContainer.Tag;
                    //lstPnlPorts.Add(pnlDefPort_Temp);
                    pnlContainer_Previous = pnlContainer;
                }

            }


            pnlSP.Building = false;
            if (intVBarValue < pnlSP.scrollBar_V.Minimum)
                intVBarValue = pnlSP.scrollBar_V.Minimum;
            if (intVBarValue > pnlSP.scrollBar_V.Maximum)
                intVBarValue = pnlSP.scrollBar_V.Maximum;
            pnlSP.scrollBar_V.Value = intVBarValue;
        }




        static RichTextBox rtx = null;

        static RichTextBox rtxDefinitionPort_Click
        {
            get { return rtx; }
            set { rtx = value; }
        }


        static public List<panelDefinitionPort> _lstPnlPorts = new List<panelDefinitionPort>();
        static public List<panelDefinitionPort> lstPnlPorts
        {
            get { return _lstPnlPorts; }
            set
            {
                if (_lstPnlPorts != value)
                {
                    _lstPnlPorts = value;
                }
            }
        }

        public void DefinitionPort_InsertNew()
        {
            if (lbxResults.SelectedIndex >= 0 && lbxResults.SelectedIndex < lstSearchResults_Main.Count)
            {
                classSearchResult cSearchResult = lstSearchResults_Main[lbxResults.SelectedIndex];

                panelDefinitionPort pnlDefPort_New = new panelDefinitionPort(ref cSearchResult.cDictionary, ref cSearchResult);
                DefinitionPort_InsertNew(ref pnlDefPort_New);
            }
        }
        public void DefinitionPort_InsertNew(ref panelDefinitionPort pnlDefPort_New)
        {
            pnlSP.Building = true;
            {
                lstPnlPorts.Add(pnlDefPort_New);

                pnlDefPort_New.lstQueue.Add(new panelDefinitionPort.classQueueItem(lstSearchResults_Main));

                Panel pnlContainer = new Panel();
                pnlDefPort_New.ZoomFactor = ZoomFactor;

                pnlDefPort_New.rtx.Tag = (object)pnlDefPort_New;
                pnlDefPort_New.rtx_Array[0].MouseLeave += rtx_MouseLeave;
                pnlDefPort_New.rtx_Array[1].MouseLeave += rtx_MouseLeave;
                pnlDefPort_New.Tag = (object)pnlContainer;

                pnlContainer.Name = "DefinitionPort_InsertNew: pnlContainer:"
                                        + (lstSearchResults_Main.Count > lbxResults.SelectedIndex
                                                                       ? lbxResults.Items[lbxResults.SelectedIndex]
                                                                       : (pnlDefPort_New.cDictionary != null
                                                                                                      ? pnlDefPort_New.cDictionary.Heading.Trim()
                                                                                                      : ""));

                pnlContainer.Size = new Size(pnlSP.scrollBar_V.Left, panelDefinitionPort.Height_Default);
                pnlContainer.Controls.Add(pnlDefPort_New);
                pnlDefPort_New.Tag = (object)pnlContainer;
                pnlDefPort_New.Dock = DockStyle.Fill;
                pnlDefPort_New.Name = "Definition Copy";
                panelSP.classSweepAndPrune_Element cEleNew = pnlSP.Add(ref pnlContainer);
                //cEleNew.Name = "debugging element";
                pnlContainer.Tag = (object)cEleNew; // debughere

                if (pnlSP.lstElements.Count > 1)
                {
                    pnlSP.lstElements.Remove(cEleNew);
                    pnlSP.lstElements.Insert(0, cEleNew);
                    cEleNew.Location = new Point(0, 0);
                }
                pnlDefPort_New.Buttons_Visible();
                DefinitionPortPanel_PlacePanels();

                Button btnMinimize = new Button();
                btnMinimize.Text = "_";
                btnMinimize.AutoSize = false;
                Panel pnlParent = pnlDefPort_New.pnlButtons;
                int intBtnWidth = pnlParent.Height - panelDefinitionPort.intGap * 2;
                btnMinimize.Size = new Size(intBtnWidth, intBtnWidth);
                btnMinimize.Show();
                btnMinimize.Dock = DockStyle.None;
                btnMinimize.Click += btnDefPort_Minimize_Click;
                pnlDefPort_New.pnlButtons.Controls.Add(btnMinimize);
                btnMinimize.Tag = (object)pnlDefPort_New;

                btnMinimize.Location = new Point(pnlParent.Width - panelDefinitionPort.intGap - btnMinimize.Width, panelDefinitionPort.intGap);
            }
            pnlSP.Building = false;
            placeObjects();
            formWords.instance.rtxCK.rtx.Focus();
        }


        void mbpDefPort_ButtonList_Build()
        {
            // build MultiButtonPic 
            {
                if (mbpDevNav_ButtonList == null)
                {
                    mbpDevNav_ButtonList = new classMultiButtonPic();
                    mbpDevNav_ButtonList.Formation = classMultiButtonPic.enuButtonFormation.contiguous;
                    mbpDevNav_ButtonList.Dock = DockStyle.Fill;
                    mbpDevNav_ButtonList.BackColor = Color.LightBlue;
                    mbpDevNav_ButtonList.BorderSize = 3;

                    mbpDevNav_ButtonList.eventHandler_MouseMove = mbpDevNav_MouseMove;
                    mbpDevNav_ButtonList.eventHandler_MouseClick = mbpDevNav_Click;
                }
            }
        }

        void mbpDevNav_Click(object sender, MouseEventArgs e)
        {
            classMultiButtonPic cMBPSender = (classMultiButtonPic)sender;
            classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMBPSender.ButtonUnderMouse;
            if (cBtn != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            switch (eCursorSelection)
                            {
                                case enuCursorSelection.Insert:
                                    formWords.ckRTX.Text_ReplaceWord(cBtn.Text);
                                    break;

                                case enuCursorSelection.Delete:
                                    {
                                        cMBPSender.Button_Sub(ref cBtn);
                                        mbpDevNav_ButtonList.placeButtons();
                                        mbpDefPort_ButtonList_Backup_Record();
                                    }
                                    break;

                                case enuCursorSelection.Recall:
                                    {
                                        classMBPDefPort_Item mbpDefPortItem = (classMBPDefPort_Item)cBtn.Tag;
                                        classDictionary cDictionaryDevNavItem = classDictionary.get(mbpDefPortItem.strDictionary);
                                        if (cDictionaryDevNavItem != null)
                                        {
                                            List<string> lstWords = new List<string>();
                                            lstWords.Add(mbpDefPortItem.strSearchFind);
                                            classSearchParameters cSearchParameters = new classSearchParameters(ref cDictionaryDevNavItem, ref lstWords, enuSearchType.Heading);
                                            List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();
                                            lstSearchParameters.Add(cSearchParameters);
                                            formWords.instance.Search(ref lstSearchParameters, enuSearchRequestor.main);

                                            cMBPSender.Button_Sub(ref cBtn);
                                            mbpDevNav_ButtonList.placeButtons();
                                            mbpDefPort_ButtonList_Backup_Record();
                                        }
                                    }
                                    break;

                                case enuCursorSelection.Default:
                                    break;
                            }
                        }
                        break;

                    case MouseButtons.Right:

                        break;
                }
            }
        }
        void mbpDevNav_MouseMove(object sender, MouseEventArgs e)
        {
            classMultiButtonPic cMBPSender = (classMultiButtonPic)sender;
            classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMBPSender.ButtonUnderMouse;
            if (cBtn != null)
            {
                Point ptMouseOnButton = new Point(e.X - cBtn.Location.X,
                                                  e.Y - cBtn.Location.Y);
                //pnlSP.recVisible = new Rectangle(0, 0, pnlSP.Width, pnlSP.Height);
                int intCursor = (int)(3f * (float)ptMouseOnButton.X / (float)cBtn.Size.Width);
                enuCursorSelection eCursorSelection_Temp = (enuCursorSelection)intCursor;
                if (eCursorSelection_Temp == enuCursorSelection.Recall)
                {
                    classMBPDefPort_Item mbpDefPortItem = (classMBPDefPort_Item)cBtn.Tag;
                    classDictionary cDictionaryDevNavItem = classDictionary.get(mbpDefPortItem.strDictionary);
                    if (cDictionaryDevNavItem == null) // rejects context-menu generated words that are not tied to any dictionary
                    {
                        eCursorSelection = enuCursorSelection.Default;
                        return;
                    }
                }
                eCursorSelection = eCursorSelection_Temp;
            }
            else
                eCursorSelection = enuCursorSelection.Default;
        }

        private void btnDefPort_Minimize_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            panelDefinitionPort pnlDefPortSender = (panelDefinitionPort)btnSender.Tag;

            Panel pnlContainer = (Panel)pnlDefPort.Parent;
            pnlSP.Sub(ref pnlContainer);
            if (pnlSP.lstElements.Count > 0)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[0];
                cEle.Location = new Point(0, 0);
                DefinitionPortPanel_PlacePanels();
            }

            mbpDefPort_ButtonList_Insert(ref pnlDefPortSender);
        }

        public void mbpDefPort_ButtonList_Insert(ref panelDefinitionPort pnlDefPort_Insert)
        {
            string strFind = classStringLibrary.clean_nonAlpha_Ends(pnlDefPort_Insert.cSearchResult.Find.Trim());
            string strDictionary = pnlDefPort_Insert.cDictionary.Heading;
            mbpDefPort_ButtonList_Insert(strFind, strDictionary);
        }

        public void mbpDefPort_ButtonList_Insert(string strFind, string strDictionary)
        {
            // refuse to insert if word is already in the list
            {
                string strTest = strFind.Trim().ToLower();
                List<classSweepAndPrune.classElement> lstSP_Ele = mbpDevNav_ButtonList.cMap.lstElements;
                for (int intEleCounter = 0; intEleCounter < lstSP_Ele.Count; intEleCounter++)
                {
                    classSweepAndPrune.classElement cEle = lstSP_Ele[intEleCounter];
                    classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cEle.obj;
                    if (string.Compare(cBtn.Text.ToLower().Trim(), strTest) == 0) // identical button found - exit
                        return;
                }
            }

            classMultiButtonPic.classButton btnNew = mbpDevNav_ButtonList.Button_New();
            btnNew.AutoSize = true;
            btnNew.Text = strFind;
            btnNew.Backcolor_Highlight
                = btnNew.Forecolor_Idle
                = Color.Black;
            btnNew.Backcolor_Idle
                = btnNew.Forecolor_Highlight
                = Color.White;

            classMBPDefPort_Item cWBL_Item = new classMBPDefPort_Item(strFind, strDictionary);
            btnNew.Tag = (object)cWBL_Item;
            mbpDevNav_ButtonList.Refresh();
            mbpDefPort_ButtonList_Alphabetize();
            Show();
            mbpDefPort_ButtonList_Backup_Record();
        }

        public static List<classMBPDefPort_Item> lstmbpDefPort_ButtonList_Backup = new List<classMBPDefPort_Item>();
        bool bolMBPDefPort_IgnoreNewButton = false;
        void mbpDefPort_ButtonList_Backup_Record()
        {
            if (bolMBPDefPort_IgnoreNewButton) return;
            lstmbpDefPort_ButtonList_Backup.Clear();

            List<Ck_Objects.classSweepAndPrune.classElement> lst = mbpDevNav_ButtonList.cMap.lstElements;
            for (int intWordCounter = 0; intWordCounter < lst.Count; intWordCounter++)
            {
                Ck_Objects.classSweepAndPrune.classElement cEle = lst[intWordCounter];
                classMultiButtonPic.classButton btn = (classMultiButtonPic.classButton)cEle.obj;

                classMBPDefPort_Item mbpRecord = (classMBPDefPort_Item)btn.Tag;

                lstmbpDefPort_ButtonList_Backup.Add(mbpRecord);
            }
        }

        public class classMBPDefPort_Item
        {
            public string strSearchFind = "";
            public string strDictionary = "";
            public classMBPDefPort_Item() { }
            public classMBPDefPort_Item(string strSearchFind, string strDictionary)
            {
                this.strSearchFind = strSearchFind;
                this.strDictionary = strDictionary;
            }
        }

        void mbpDefPort_ButtonList_Alphabetize()
        {
            List<Ck_Objects.classSweepAndPrune.classElement> lst = mbpDevNav_ButtonList.cMap.lstElements;

            Ck_Objects.classSweepAndPrune.classElement cBest = null;
            string strBest = "";

            for (int intOuter = 0; intOuter < lst.Count - 1; intOuter++)
            {
                cBest = lst[intOuter];
                classMultiButtonPic.classButton btnBest = (classMultiButtonPic.classButton)cBest.obj;
                strBest = btnBest.Text;
                for (int intInner = intOuter + 1; intInner < lst.Count; intInner++)
                {
                    classSweepAndPrune.classElement cEleInner = lst[intInner];
                    classMultiButtonPic.classButton btnInner = (classMultiButtonPic.classButton)cEleInner.obj;
                    string strInner = btnInner.Text;
                    if (string.Compare(strInner, strBest) <= 0)
                    {
                        cBest = cEleInner;
                        strBest = btnBest.Text;
                    }
                }

                if (cBest != lst[intOuter])
                {
                    classSweepAndPrune.classElement cTemp = lst[intOuter];
                    int intTemp = lst.IndexOf(cBest);
                    lst[intOuter] = cBest;
                    lst[intTemp] = cTemp;
                }
            }

            mbpDevNav_ButtonList.cMap.Reset();
            mbpDevNav_ButtonList.placeButtons();
        }


        private void rtx_MouseLeave(object sender, EventArgs e)
        {
            RichTextBox rtxSender = (RichTextBox)sender;
            ZoomFactor = rtxSender.ZoomFactor;
        }


        void panelDictionaryOutput_GotFocus(object sender, EventArgs e)
        {
            if (_frmFindReplace != null)
                _frmFindReplace.setTextBox(ref sender);
        }



        void panelDictionaryOutput_VisibleChanged(object sender, EventArgs e)
        {
        }

        void panelDictionaryOutput_Disposed(object sender, EventArgs e)
        {
        }



        const string mbpWordList_ItemDivider = "\r\n";
        const string mbpWordList_ItemSubDivider = " -> ";

        static public void mbpWordList_Save()
        {
            string strSave = mbpWordList_Save_GetString();
            System.IO.File.WriteAllText(mbpWordList_Filename, strSave);
        }

        static string mbpWordList_Save_GetString()
        {
            string strRetVal = "";
            for (int intWordCounter = 0; intWordCounter < lstmbpDefPort_ButtonList_Backup.Count; intWordCounter++)
            {
                classMBPDefPort_Item cMBPItem = lstmbpDefPort_ButtonList_Backup[intWordCounter];
                strRetVal += mbpWordList_ItemDivider
                                + cMBPItem.strSearchFind
                                + mbpWordList_ItemSubDivider
                                + cMBPItem.strDictionary;
            }
            return strRetVal;
        }

        static string mbpWordList_Filename
        {
            get { return System.IO.Directory.GetCurrentDirectory() + "\\mbpWordList.txt"; }
        }

        static public void mbpWordList_Load()
        {
            if (!System.IO.File.Exists(mbpWordList_Filename))
                return;

            string strMbpWordList = System.IO.File.ReadAllText(mbpWordList_Filename);

            strMbpWordList = strMbpWordList.Replace(mbpWordList_ItemDivider, "|");
            strMbpWordList = strMbpWordList.Replace(mbpWordList_ItemSubDivider, "|");

            char[] chrSplitter = { '|' };

            string[] strSplit = strMbpWordList.Split(chrSplitter, StringSplitOptions.RemoveEmptyEntries);
            lstmbpDefPort_ButtonList_Backup.Clear();
            if (strSplit.Length % 2 != 0)
                return;
            for (int intCounter = 0; intCounter < strSplit.Length; intCounter += 2)
            {
                string strWord = strSplit[intCounter];
                string strDictionary = strSplit[intCounter + 1];
                lstmbpDefPort_ButtonList_Backup.Add(new classMBPDefPort_Item(strWord, strDictionary));
            }
          instance.  bolMBPDefPort_IgnoreNewButton = true;
            {
                for (int intButtonCounter = 0; intButtonCounter < lstmbpDefPort_ButtonList_Backup.Count; intButtonCounter++)
                {
                    classMBPDefPort_Item mbpItem = lstmbpDefPort_ButtonList_Backup[intButtonCounter];
                    instance.mbpDefPort_ButtonList_Insert(mbpItem.strSearchFind, mbpItem.strDictionary);
                }
            }
            instance.bolMBPDefPort_IgnoreNewButton = false;
        }


    }


    public class formDictionaryOutput :Form
    {
        public static formDictionaryOutput instance = null;
        public ListBox lbx = null;
        public panelDefinitionPort pnlDefPort = null;
        public SplitContainer spl = null;
        public static Size sz;
        public static Point loc;

        public formDictionaryOutput()
        {
            instance = this;
            TopMost = true;
            
            Bitmap bmp = new Bitmap(Properties.Resources.Dictionary);
            Icon = Icon.FromHandle(bmp.GetHicon());
            lbx = new ListBox();
            spl = new SplitContainer();
            
            Activated += panelDictionaryOutput_Activated;

            LocationChanged += formDictionaryOutput_LocationChanged;
            SizeChanged += FormDictionaryOutput_SizeChanged;

            FormClosing += formDictionaryOutput_FormCLosing;

            Controls.Add(spl);
            spl.Dock = DockStyle.Fill;
            spl.Orientation = Orientation.Horizontal;

            spl.Panel1.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;
            lbx.ContextMenu = new ContextMenu();
            lbx.SelectedIndexChanged += lbx_SelectedIndexChanged;
            lbx.MouseLeave += lbx_MouseLeave;
            lbx.MouseEnter += lbx_MouseEnter;
            lbx.MouseMove += lbx_MouseMove;
            lbx.MouseClick += lbx_MouseClick;
            lbx.MouseDoubleClick += lbx_MouseDoubleClick;

        }

        string strDirectoryAndFileName = "";
        enuFileExtensions eFileExtension = enuFileExtensions._num;
        void LoadRTX(int intLbxResultsIndex)
        {
            List<classRTXInfo> lstNewRTX = new List<classRTXInfo>();

            RichTextBox rtx = pnlDefPort.rtx;
            rtx.Clear();
            rtx.ZoomFactor = 1;

            rtx.Text = "";
            if (intLbxResultsIndex >= 0 && intLbxResultsIndex < lstSearchResults.Count)
            {
                classSearchResult cSearchResult = lstSearchResults[intLbxResultsIndex];

                panelDefinitionPort pnlDefPortRef = pnlDefPort;
                if (pnlDefPortRef != null)
                    pnlDefPortRef.cDictionary = cSearchResult.cDictionary;

                strDirectoryAndFileName = classFileContent.getFileNameAndDirectory(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                eFileExtension = enuFileExtensions._num;

                for (int intExtensionCounter = 0; intExtensionCounter <= (int)enuFileExtensions._num; intExtensionCounter++)
                {
                    eFileExtension = (enuFileExtensions)intExtensionCounter;
                    string strTestFileName = strDirectoryAndFileName + "." + eFileExtension.ToString();
                    if (System.IO.File.Exists(strTestFileName))
                    {
                        strDirectoryAndFileName = strTestFileName;
                        break;
                    }
                }
        
                switch (eFileExtension)
                {
                    case enuFileExtensions.txt:
                        {
                            classFileContent cFileContent = new classFileContent(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                            try
                            {
                                strHeading = cFileContent.Heading;
                                lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.Heading.Trim(), panelDefinitionPort.enuRTXStyle.WordHeading));
                            }
                            catch (Exception)
                            {

                            }

                            try
                            {
                                if (cFileContent.alt_Heading != null
                                    &&
                                    cFileContent.alt_Heading.Length > 0)
                                    lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.alt_Heading.Trim(), panelDefinitionPort.enuRTXStyle.WordDefinition));
                            }
                            catch (Exception)
                            {

                            }

                            try
                            {
                                lstNewRTX.Add(panelDefinitionPort.getRTX(ref rtx, cFileContent.Definition.Trim(), panelDefinitionPort.enuRTXStyle.WordDefinition));
                            }
                            catch (Exception)
                            {
                            }
                        }
                        break;

                    case enuFileExtensions.rtf:
                        {
                            if (pnlDefPort.rtx == null || pnlDefPort.rtx.IsDisposed)
                            {
                                pnlDefPort.rtx.LoadFile(strDirectoryAndFileName);
                            }
                            else
                            {
                                pnlDefPort.rtx.LoadFile(strDirectoryAndFileName);
                                int intFirstCR = rtx.Text.IndexOf("\n");
                                if (intFirstCR > 0)
                                    strHeading = pnlDefPort.rtx.Text.Substring(0, intFirstCR).Trim();

                            }
                        }
                        break;

                    case enuFileExtensions._num:
                    default:
                        { }
                        break;

                }

                pnlDefPort.rtx.Select(0, 0);
                pnlDefPort.tmrHighLightSearchedWord_Reset();
                Show();
            }
        }

        static List<classSearchResult> _lstSearchResults_ = new List<classSearchResult>();
        static List<string> lstHeadings = new List<string>();
        static public List<classSearchResult> lstSearchResults
        {
            get 
            {
                if (instance == null || instance.IsDisposed)
                    instance = new formDictionaryOutput();
                return instance.SearchResults; ; 
            }
            set
            {
                if (instance == null || instance.IsDisposed)
                    instance = new formDictionaryOutput();
                instance.SearchResults = value;
            }
        }

        List<classSearchResult> SearchResults
        {
            get { return _lstSearchResults_; }
            set
            {
                if (instance == null || instance.IsDisposed)
                    instance = new formDictionaryOutput();

                _lstSearchResults_ = value;

                int intCounter = 0;
                if (SearchResults.Count > 0)
                {
                    List<classSearchResult> lstTopReturns = new List<classSearchResult>();
                    classSearchResult cSearchResult = SearchResults[0];
                        //classSearchParameters cSearchParameters = formWords.instance.lstSearchParameters[0];
                    string strPrefix = classBT_Dictionaries.Prefix;
                    string strSuffix = classBT_Dictionaries.Suffix;
                    string strFind = classBT_Dictionaries.WordFound;

                    strHeading  = (strPrefix.Length > 0
                                             ? (strPrefix + " + ")
                                             : "")
                           + strFind
                           + (strSuffix.Length > 0
                                               ? (" + " + strSuffix)
                                               : "");
                    // set exact result top of the list
                    if (formWords.instance.lstSearchParameters.Count > 0 && formWords.instance.lstSearchParameters[0].lstWords.Count > 0)
                    {
                        string strTest = formWords.instance.lstSearchParameters[0].lstWords[0].Trim().ToUpper();
                        intCounter = 0;
                        int intNumSinceFound = 0;

                        while (intCounter < SearchResults.Count)
                        {
                            classSearchResult cResult = SearchResults[intCounter];
                            if (string.CompareOrdinal(classStringLibrary.cleanFront_nonAlpha(cResult.Find.Trim()).ToUpper(), strTest) == 0)
                            {
                                _lstSearchResults_.Remove(cResult);
                                lstTopReturns.Add(cResult);
                            }
                            else
                            {
                                if (lstTopReturns.Count > 0)
                                {
                                    intNumSinceFound++;
                                    if (intNumSinceFound > 64)
                                        goto exit_SetExactResult_TopOfList;
                                }
                                intCounter++;
                            }
                        }
                    }

exit_SetExactResult_TopOfList:
                    _lstSearchResults_.InsertRange(0, lstTopReturns);

                    lbx.Items.Clear();
                    intCounter = 0;
                    List<string> lstFilenames = new List<string>();
                    while (intCounter < SearchResults.Count)
                    {
                        classSearchResult cResult = SearchResults[intCounter];

                        if (lstFilenames.Contains(cResult.strFileName))
                        {
                            SearchResults.Remove(cResult);
                        }
                        else
                        {
                            lstFilenames.Add(cResult.strFileName);
                            lstHeadings.Add(cResult.Find);
                            lbx.Items.Add(
                                cResult.cDictionary.Heading.Trim() +
                                "  :  " + cResult.Find.Trim());
                            intCounter++;
                        }
                    }
                    lbx.SelectedIndex = 0;
                    lbx.Refresh();
                    if (pnlDefPort == null)
                    {
                        if (lbx.SelectedIndex >= 0 && lbx.SelectedIndex < SearchResults.Count)
                        {
                            cSearchResult = SearchResults[lbx.SelectedIndex];

                            pnlDefPort = new panelDefinitionPort(ref cSearchResult.cDictionary, ref cSearchResult);
                            pnlDefPort.Dock = DockStyle.Fill;
                            pnlDefPort.Name = "Definition Copy";
                        }
                        spl.Panel2.Controls.Add(pnlDefPort);
                        pnlDefPort.Dock = DockStyle.Fill;
                    }
                 
                    pnlDefPort.Reset(SearchResults);
                    pnlDefPort.eSearchType = formWords.instance.lstSearchParameters[0].eSearchType;
                    pnlDefPort.lstSearchWords = formWords.instance.lstSearchParameters[0].lstWords;

                    instance.Text = strHeading;
                    instance.Show();
                }
            }
        }



        static string _strHeading = "";
        static string strHeading
        {
            get { return _strHeading; }
            set { _strHeading = value; }
        }


        private void lbx_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            int intMouseOverIndex = lbx.IndexFromPoint(new Point(e.X, e.Y));
            if (intMouseOverIndex >= 0 && intMouseOverIndex < lbxSender.Items.Count)
            {
                lbxSender.SelectedIndex = intMouseOverIndex;
            }
        }
        bool bolMouseOverlbx = false;
        private void lbx_MouseEnter(object sender, EventArgs e)
        {
            bolMouseOverlbx = true;
        }
        private void lbx_MouseLeave(object sender, EventArgs e)
        {
            bolMouseOverlbx = false;
            //if (lbx.Items.Count > intRtxOutput_Index)
            //    lbx.SelectedIndex = intRtxOutput_Index;
            if (formPopUp.instance != null)
                formPopUp.instance.Hide();
        }

        private void lbx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            if (lbxSender.SelectedIndex >= 0 && lbxSender.SelectedIndex < lbxSender.Items.Count)
            {
                ck_RichTextBox ckRTX = formWords.ckRTX;
                if (ckRTX != null)
                {
                    classSearchResult cSearchResult = lstSearchResults[lbxSender.SelectedIndex];
                    if (cSearchResult.cFileContent == null)
                    {
                        cSearchResult.cFileContent = new classFileContent(cSearchResult.cDictionary.strSourceDirectory, cSearchResult.strFileName);
                        ckRTX.Text_Insert(cSearchResult.cFileContent.Heading);
                    }
                }
            }
        }

        private void lbx_MouseClick(object sender, MouseEventArgs e)
        {
            if (lbx.SelectedIndex >= 0 && lbx.SelectedIndex < lbx.Items.Count)
                LoadRTX(lbx.SelectedIndex);
            return;
        }

        private void lbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lbxSender = (ListBox)sender;
            if (lbxSender.Items.Count != lstSearchResults.Count) return;
            if (bolMouseOverlbx && formWords.grbOptions.Toggle_Dictionary_Form_Mouse_Pop_Up_Definition)
            {
                if (lbxSender.SelectedIndex >= 0 && lbxSender.SelectedIndex < lbxSender.Items.Count)
                {
                    if (formPopUp.instance == null || formPopUp.instance.IsDisposed) new formPopUp();
                    string strFileName = classFileContent.getFileNameAndDirectory(lstSearchResults[lbxSender.SelectedIndex].cDictionary.strSourceDirectory, lstSearchResults[lbxSender.SelectedIndex].strFileName);
                    classDictionary cDictionaryRef = lstSearchResults[lbxSender.SelectedIndex].cDictionary;
                panelDictionaryOutput.PopUp(ref cDictionaryRef, strFileName);
                }
            }
        }

        private void FormDictionaryOutput_SizeChanged(object sender, EventArgs e)
        {

            if (WindowState != FormWindowState.Minimized)
                sz = Size;
        }

        private void panelDictionaryOutput_Activated(object sender, EventArgs e)
        {
            if (bolInit) return;

            formInfo_Load();

            bolInit = true;
        }

        private void formDictionaryOutput_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                loc = Location;
        }


        private void formDictionaryOutput_FormCLosing(object sender, FormClosingEventArgs e)
        {
            recordForm();
            pnlDefPort.Dispose();
            pnlDefPort = null;
            lbx.Dispose();
            lbx = null;
        }

        bool bolInit = false;
        void recordForm()
        {
            if (!bolInit)
                return;
            formInfo_Save();
        }

        enum enuFormInfo { Left, Top, Width, Height, DefinitionPort_HeightDefault, spl, _num };
        const string strFormInfoSplit = "|";
        void formInfo_Save()
        {
            string strOutput = "";
            for (int intCounter = 0; intCounter < (int)enuFormInfo._num; intCounter++)
            {
                enuFormInfo eFormInfo = (enuFormInfo)intCounter;
                switch (eFormInfo)
                {
                    case enuFormInfo.Height:
                        {
                            strOutput += sz.Height.ToString() + strFormInfoSplit;
                        }
                        break;

                    case enuFormInfo.Width:
                        {
                            strOutput += sz.Width.ToString() + strFormInfoSplit;
                        }
                        break;

                    case enuFormInfo.Left:
                        {
                            strOutput += loc.X.ToString() + strFormInfoSplit;
                        }
                        break;

                    case enuFormInfo.Top:
                        {
                            strOutput += loc.Y.ToString() + strFormInfoSplit;
                        }
                        break;

                    case enuFormInfo.spl:
                        {
                            strOutput +=spl.SplitterDistance.ToString() + strFormInfoSplit;
                        }
                        break;
                }
            }
            System.IO.File.WriteAllText(FormInfoFileName, strOutput);
        }

        void formInfo_Load()
        {
            if (System.IO.File.Exists(FormInfoFileName))
            {
                string strFormInfo = System.IO.File.ReadAllText(FormInfoFileName);
                char[] chrSplit = strFormInfoSplit.ToArray<char>();
                string[] strInfo = strFormInfo.Split(chrSplit);
                if (strInfo.Length == (int)enuFormInfo._num + 1)
                {
                    for (int intCounter = 0; intCounter < strInfo.Length; intCounter++)
                    {
                        enuFormInfo eFormInfo = (enuFormInfo)intCounter;
                        switch (eFormInfo)
                        {
                            case enuFormInfo.Height:
                                {
                                    try
                                    {
                                        Height = Convert.ToInt32(strInfo[intCounter]);
                                    }
                                    catch (Exception)
                                    {
                                        goto FailLoad;
                                    }
                                }
                                break;

                            case enuFormInfo.Width:
                                {
                                    try
                                    {
                                        Width = Convert.ToInt32(strInfo[intCounter]);
                                    }
                                    catch (Exception)
                                    {
                                        goto FailLoad;
                                    }
                                }
                                break;

                            case enuFormInfo.Left:
                                {
                                    try
                                    {
                                        Left = Convert.ToInt32(strInfo[intCounter]);
                                    }
                                    catch (Exception)
                                    {
                                        goto FailLoad;
                                    }
                                }
                                break;

                            case enuFormInfo.Top:
                                {
                                    try
                                    {
                                        Top = Convert.ToInt32(strInfo[intCounter]);
                                    }
                                    catch (Exception)
                                    {
                                        goto FailLoad;
                                    }
                                }
                                break;


                            case enuFormInfo.spl:
                                {
                                    try
                                    {
                                        spl.SplitterDistance = Convert.ToInt32(strInfo[intCounter]);
                                    }
                                    catch (Exception)
                                    {
                                        goto FailLoad;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            return;
        FailLoad:
            Width = 400;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
            Left = (int)(Screen.PrimaryScreen.WorkingArea.Width * .75);
            Top = 0;
        }

        string FormInfoFileName
        {
            get
            {
                return System.IO.Directory.GetCurrentDirectory() + "\\formDictionarOutput.txt";
            }
        }
    }


}

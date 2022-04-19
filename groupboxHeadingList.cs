using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ck_Objects;
using Math3;
using StringLibrary;

namespace Words
{
    public class groupboxHeadingList : System.Windows.Forms.GroupBox
    {
        public static groupboxHeadingList instance = null;
        MultiButtonPic_HeadingList cHeadingList = null;
        VScrollBar vScrBar = new VScrollBar();

        public panelDefinitionPort pnlDefPort = null;

        GroupBox grbHeadingList_DictionarySelection = new GroupBox();
        ListBox lbxHeadingList_DictionarySelection = new ListBox();
        SplitContainer splMain = new SplitContainer();
        Button btnDefPort_ButtonList_Add = new Button();

        int intSplMain_Distance = 0;
        public int splMain_Distance
        {
            get { return intSplMain_Distance; }
            set 
            {
                intSplMain_Distance = value;
                splMain.SplitterDistance = value;
            }
        }

        static Single _sngZoomFactor = 1;
        static public Single ZoomFactor
        {
            get { return _sngZoomFactor; }
            set
            {
                _sngZoomFactor = value;
            }
        }


        public void Scroll_Up() { TopIndex = TopIndex - 1; }
        public void Scroll_Down() { TopIndex = TopIndex + 1; }
        
        List<classHeadingList.classHeadingList_Record> _lstWords = new List<classHeadingList.classHeadingList_Record>();
        public List<classHeadingList.classHeadingList_Record> lstWords
        {
            get { return _lstWords; }
            set
            {
                if (_lstWords.Count != value.Count 
                    || (_lstWords.Count > 0 && value.Count >0 && string.Compare(lstWords[0].strFilename, value[0].strFilename) != 0))
                {
                    _lstWords = value;
                    InitHeadingList();
                    cSelected_Set();
                }
            }
        }

        private void VScrBar_ValueChanged(object sender, EventArgs e)
        {
            if (TopIndex != vScrBar.Value)
                TopIndex = vScrBar.Value;
        }

        public void JumpTo(string strWord)
        {
            classDictionary cDictionaryReference = pnlDefPort.cDictionary;
            lstWords = classHeadingList.Get(ref cDictionaryReference, strWord, cHeadingList.NumWordsDisplayed);
            if (lstWords.Count > 0)
            {
                cSelected_Set();
                InitHeadingList();
                if (lstWords[0].Index >= vScrBar.Minimum && lstWords[0].Index < vScrBar.Maximum)
                    vScrBar.Value = lstWords[0].Index;
            }
            pnlDefPort.Heading_Set();
        }

        public int TopIndex
        {
            get 
            {
                if (lstWords.Count > 0) return lstWords[0].Index;
                return -1; 
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (lstWords.Count == 0 || lstWords[0].Index != value)
                {
                    if (value >= 0 && value < cDictionary.cHeadingListInfo.NumEntries -cHeadingList.NumWordsDisplayed)
                    {
                        classDictionary cDicRef = cDictionary;
                        lstWords = classHeadingList.Get(ref cDicRef, value, cHeadingList.NumWordsDisplayed);
                        event_ButtonUnderMouse_Changed((object)this, new EventArgs());
                    }

                    if (value >= vScrBar.Minimum && value <= vScrBar.Maximum)
                    vScrBar.Value = value;
                }
            }
        }

        public void cSelected_Set()
        {
            cSelected = cHeadingList.SelectedHeading;
        }


        public Ck_Objects.progressbar proBar = new Ck_Objects.progressbar();

        static System.Windows.Forms.Timer tmrPopUp_Hide = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer tmrMouseHover = new System.Windows.Forms.Timer();

        static int _intDefaultDictionaryIndex = 0;
        static public int DefaultDictionaryIndex
        {
            get { return _intDefaultDictionaryIndex; }
            set 
            {
                _intDefaultDictionaryIndex = value;
            }
        }


        public groupboxHeadingList()
        {
            instance = this;
            groupboxHeadingList cMyRef = this;
            cHeadingList = new MultiButtonPic_HeadingList(ref cMyRef);
            cHeadingList.eventHandler_ButtonUnderMouse_Changed = event_ButtonUnderMouse_Changed;
            cHeadingList.MouseDown += HeadingList_MouseClick;
            cHeadingList.MouseMove += CHeadingList_MouseMove;
            
            Controls.Add(splMain);
            splMain.SplitterMoved += SplMain_SplitterMoved;
            splMain.Dock = DockStyle.Fill;
            
            groupboxHeadingList grbMyRef = this;
            splMain.Panel1.Controls.Add(cHeadingList);
            splMain.Panel1.Controls.Add(vScrBar);
            vScrBar.ValueChanged += VScrBar_ValueChanged;

            cHeadingList.BringToFront();
            classDictionary cDictionary_Ref = cDictionary;
            
            classSearchResult cSR = new classSearchResult(ref cDictionary_Ref, "");
            pnlDefPort = new panelDefinitionPort(ref cDictionary_Ref, ref cSR);
            pnlDefPort.Queue_Index = 0;
            pnlDefPort.cmbHeadings.Hide();
            pnlDefPort.rtx_Array[0].MouseLeave += rtx_MouseLeave;
            pnlDefPort.rtx_Array[1].MouseLeave += rtx_MouseLeave;

            btnDefPort_ButtonList_Add.Text = "_";
            int intDefPort_Button_Width = pnlDefPort.pnlButtons.Height - panelDefinitionPort.intGap * 2;
            btnDefPort_ButtonList_Add.Size = new Size(intDefPort_Button_Width, intDefPort_Button_Width);
            btnDefPort_ButtonList_Add.Location = new Point(pnlDefPort.pnlButtons.Width - panelDefinitionPort.intGap - btnDefPort_ButtonList_Add.Width, panelDefinitionPort.intGap);
            pnlDefPort.pnlButtons.Controls.Add(btnDefPort_ButtonList_Add);
            btnDefPort_ButtonList_Add.Click += BtnDefPort_ButtonList_Add_Click;

            pnlDefPort.Dock = DockStyle.Fill;
            splMain.Panel2.Controls.Add(pnlDefPort);

            Controls.Add(proBar);
            proBar.Minimum = 0;
            proBar.Maximum = 100;
            proBar.Value = 0;
            proBar.Hide();

            tmrMouseHover.Interval = 200;
            tmrMouseHover.Tick += TmrMouseHover_Tick;

            tmrPopUp_Hide.Interval = 1000;
            tmrPopUp_Hide.Tick += TmrPopUp_Hide_Tick;

            splMain.Panel1.Controls.Add(grbHeadingList_DictionarySelection);
            grbHeadingList_DictionarySelection.BackColor = Color.Azure;
            grbHeadingList_DictionarySelection.Controls.Add(lbxHeadingList_DictionarySelection);
            lbxHeadingList_DictionarySelection.Items.Clear();
            grbHeadingList_DictionarySelection.Dock = DockStyle.Fill;
            grbHeadingList_DictionarySelection.Hide();
            grbHeadingList_DictionarySelection.BringToFront();

            lbxHeadingList_DictionarySelection.MouseClick += LbxHeadingList_DictionarySelection_MouseClick;
            lbxHeadingList_DictionarySelection.MouseLeave += LbxHeadingList_DictionarySelection_MouseLeave;
            SizeChanged += GroupboxHeadingList_SizeChanged;
            MouseLeave += GroupboxHeadingList_MouseLeave;
            VisibleChanged += GroupboxHeadingList_VisibleChanged;
        }

        private void BtnDefPort_ButtonList_Add_Click(object sender, EventArgs e)
        {
            panelDefinitionPort pnlDefPort_Copy = pnlDefPort.Copy();
            pnlDefPort_Copy.cSearchResult.Find = cSelected != null 
                                                        ? cSelected.strHeading
                                                        : "NULL";
            pnlDefPort_Copy.cSearchResult.cDictionary = cDictionary;


            if (panelDictionaryOutput.instance == null || panelDictionaryOutput.instance.IsDisposed)
                panelDictionaryOutput.instance = new panelDictionaryOutput();

            

            panelDictionaryOutput.instance.mbpDefPort_ButtonList_Insert(ref pnlDefPort_Copy);

        }

        void rtx_MouseLeave(object sender, EventArgs e)
        {
            RichTextBox rtxSender = (RichTextBox)sender;
            ZoomFactor = rtxSender.ZoomFactor;
        }
        private void GroupboxHeadingList_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void SplMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            intSplMain_Distance = splMain.SplitterDistance;
        }

        void event_ButtonUnderMouse_Changed(object sender, EventArgs e)
        {
           //if (cHeadingList.ButtonUnderMouse != null)
           // {
           //     if (cHeadingList.ButtonUnderMouse_Index >= 0)
           //         cUnderMouse = lstWords[cHeadingList.ButtonUnderMouse_Index];
           //     else
           //         cUnderMouse = null;
           // }
           //else
           // {
           //     ShowDefinition();
           // }
        }

        public void pnl_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        private void GroupboxHeadingList_MouseLeave(object sender, EventArgs e)
        {
        //    PopUpDefinition_Hide();
        }

        public void TmrPopUp_Hide_Tick(object sender, EventArgs e)
        {
            tmrPopUp_Hide.Enabled = false;
            if (instance.grbHeadingList_DictionarySelection.Visible) return;

            ShowDefinition();
        }


        public void PopUpDefinition_Hide()
        {
            tmrPopUp_Hide.Enabled = true;
        }

        public void DictionarySelection_Init()
        {
            for (int intDictionaryCounter = 0; intDictionaryCounter < classDictionary.lstDictionaries.Count; intDictionaryCounter++)
            {
                classDictionary cDictionary = classDictionary.lstDictionaries[intDictionaryCounter];
                lbxHeadingList_DictionarySelection.Items.Add(cDictionary.Heading);
            }
            grbHeadingList_DictionarySelection.Text = "Select Dictionary";
        }

        private void LbxHeadingList_DictionarySelection_MouseLeave(object sender, EventArgs e)
        {
            grbHeadingList_DictionarySelection.Hide();
        }

        private void LbxHeadingList_DictionarySelection_MouseClick(object sender, MouseEventArgs e)
        {
            cDictionary = classDictionary.lstDictionaries[lbxHeadingList_DictionarySelection.SelectedIndex];
            grbHeadingList_DictionarySelection.Hide();
        }

        private void GroupboxHeadingList_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }


        void VScrBar_SetToDictionary()
        {
            vScrBar.Minimum = 0;
            vScrBar.LargeChange = cHeadingList.NumWordsDisplayed;
            int intMax = 0;
            if (cDictionary != null && cDictionary.cHeadingListInfo != null)
                intMax = cDictionary.cHeadingListInfo.NumEntries-2;
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
        }

        void placeObjects()
        {
            proBar.Width = Width;
            proBar.Location = new Point(0, 15);
            proBar.BringToFront();


            vScrBar.Width = 10;
            vScrBar.Height = splMain.Panel1.Height;
            vScrBar.Top = 0;
            vScrBar.Left = splMain.Panel1.Width - vScrBar.Width;

            cHeadingList.Location = new Point(0, 0);
            cHeadingList.Height = vScrBar.Height;
            cHeadingList.Width = vScrBar.Left;

            VScrBar_SetToDictionary();

            grbHeadingList_DictionarySelection.Size = cHeadingList.Size;
            lbxHeadingList_DictionarySelection.Location = new Point (5,17);
            lbxHeadingList_DictionarySelection.Size = new Size(grbHeadingList_DictionarySelection.Width - lbxHeadingList_DictionarySelection.Left * 2,
                                                               grbHeadingList_DictionarySelection.Height - lbxHeadingList_DictionarySelection.Left - lbxHeadingList_DictionarySelection.Top);
        }

        public void tmrMouseHover_Reset()
        {
            tmrMouseHover.Enabled = false;
            tmrMouseHover.Enabled = true;
        }

        public void tmrMouseHover_Disable()
        {
            tmrMouseHover.Enabled = false;
        }

        int _intPopUp_Index = -1;
        public int PopUp_Index
        {
            get { return _intPopUp_Index; }
            set
            {
                if (PopUp_Index != value)
                {
                    _intPopUp_Index = value;
                    if (proBar.Visible) return;

                    if (_intPopUp_Index < 0 || _intPopUp_Index >= lstWords.Count) return;
                    if (formPopUp.instance == null || formPopUp.instance.IsDisposed) new formPopUp();
             
                    string strFileName = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, lstWords[PopUp_Index].strFilename);
                    pnlDefPort.rtx.LoadFile(strFileName);
                }
            }
        }

        classHeadingList.classHeadingList_Record _cSelected = null;
        public classHeadingList.classHeadingList_Record cSelected
        {
            get { return _cSelected; }
            set
            {
                if (cSelected != value)
                {
                    _cSelected = value;
                    if (proBar.Visible) return;
                    
                }

                ShowDefinition();
            }
        }
        
        classHeadingList.classHeadingList_Record _cUnderMouse = null;

        public classHeadingList.classHeadingList_Record cUnderMouse
        {
            get { return _cUnderMouse; }
            set
            {
                if (cUnderMouse != value)
                {
                    _cUnderMouse = value;
                    if (proBar.Visible) return;
                    if (formWords.grbOptions.Toggle_Word_List_Form_Mouse_Pop_Up_Definition)
                    {
                        classHeadingList.classHeadingList_Record cDisplay = cUnderMouse;

                        if (cDisplay == null) return;
                        string strFileName = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, cDisplay.strFilename) + "." + cDictionary.eFileExtension.ToString();

                        if (panelDictionaryOutput.instance == null || panelDictionaryOutput.instance.IsDisposed)
                            new formDictionaryOutput();
                        classDictionary cDictionaryRef = cDictionary;
                        panelDictionaryOutput.PopUp(ref cDictionaryRef, strFileName);

                    }
                }
            }
        }


        void ShowDefinition()
        {
            if (cSelected == null) return;
            classHeadingList.classHeadingList_Record cDisplay = cSelected;

            classDictionary cDictionaryRef = pnlDefPort.cDictionary;

            List<classSearchResult> lstSearchResults = new List<classSearchResult>();
            lstSearchResults.Add(new classSearchResult(ref cDictionaryRef, cSelected.strFilename, cSelected.strHeading, "", ""));

            pnlDefPort.lstQueue.Add(new panelDefinitionPort.classQueueItem(lstSearchResults));

            pnlDefPort.rtx.Clear();
            pnlDefPort.rtx.ZoomFactor = 1;
            pnlDefPort.rtx.ZoomFactor = ZoomFactor;
            if (cDisplay == null) return;
            string strFileName = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, cDisplay.strFilename) +"."+ cDictionary.eFileExtension.ToString();
            classDictionary cDictionary_Ref = cDictionary;
            pnlDefPort.ShowDefinition(new classSearchResult(ref cDictionary_Ref, strFileName, "", "", ""));
            pnlDefPort.rtx.ZoomFactor = 1;
            pnlDefPort.rtx.ZoomFactor = ZoomFactor;
        }


        public void TmrMouseHover_Tick(object sender, EventArgs e)
        {
            tmrMouseHover.Enabled = false;
            if (proBar.Visible) return;
            if (grbHeadingList_DictionarySelection.Visible) return;
            if (intEntryBeneathMouse >= 0 && intEntryBeneathMouse < lstWords.Count)
                PopUp_Index = intEntryBeneathMouse;
        }

        int intEntryBeneathMouse = -1;
        public int EntryBeneathMouse
        {
            get { return intEntryBeneathMouse; }
            set 
            {
                if (intEntryBeneathMouse != value)
                {
                    intEntryBeneathMouse = value;
                    if (intEntryBeneathMouse >= 0 && intEntryBeneathMouse < lstWords.Count)
                    {
                        cUnderMouse = lstWords[intEntryBeneathMouse];
                        
                        if (formWords.grbOptions.Toggle_Word_List_Form_Mouse_Pop_Up_Definition)
                        {
                            string strFileName = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, cUnderMouse.strFilename);
                            classDictionary cDictionaryRef = cDictionary;
                            panelDictionaryOutput.PopUp(ref cDictionaryRef,strFileName);
                        }
                    }
                    else
                    {
                        cUnderMouse = null;
                        return;
                    }
                }
            }
        }

        private void CHeadingList_MouseMove(object sender, MouseEventArgs e)
        {
            EntryBeneathMouse = cHeadingList.ButtonUnderMouse_Index;


            
        }

        public void HeadingList_MouseClick(object sender, MouseEventArgs e)
        {
            if (proBar.Visible) return;
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        grbHeadingList_DictionarySelection.Location = cHeadingList.Location;
                        grbHeadingList_DictionarySelection.Size = cHeadingList.Size ;
                        grbHeadingList_DictionarySelection.Show();
                        grbHeadingList_DictionarySelection.BringToFront();

                        if (formPopUp.instance != null && !formPopUp.instance.IsDisposed)
                            formPopUp.instance.Hide();

                    }
                    break;

                case MouseButtons.Left:
                    {
                        classMultiButtonPic.classButton cBtnUnderMouse = cHeadingList.ButtonUnderMouse;
                        if (cBtnUnderMouse != null)
                        {
                            if (cUnderMouse == cSelected)
                            {
                                formWords.ckRTX.Text_ReplaceWord(cBtnUnderMouse.Text);
                            }
                            else
                            {
                                JumpTo(cBtnUnderMouse.Text);
                            }
                        }
                    }
                    break;
            }
        }

        void mnuDictionarySelection_Click(object sender, EventArgs e)
        {
            MenuItem mnuSender = (MenuItem)sender;
            cDictionary = (classDictionary)mnuSender.Tag;
        }

        public classDictionary cDictionary
        {
            get 
            {
                return pnlDefPort == null 
                                   ? null
                                   : pnlDefPort.cDictionary; 
            }
            set
            {
                if (pnlDefPort.cDictionary != value)
                {
                    string strWordSelected = cSelected != null 
                                                        ? cSelected.strHeading
                                                        : "";

                    pnlDefPort.cDictionary = value;
                    classDictionary cDictionaryReference = value;
                    pnlDefPort.cDictionary.cHeadingListInfo.NumEntries = classHeadingList.NumEntries(ref cDictionaryReference);
                    DefaultDictionaryIndex = classDictionary.lstDictionaries.IndexOf(pnlDefPort.cDictionary);
                    Text = cDictionary.Heading;
                    VScrBar_SetToDictionary();

                    groupboxHeadingList.instance.JumpTo(strWordSelected);
                    //TopIndex = 0; // debughere
                }
            }
        }
     
        public void InitHeadingList()
        {
            cHeadingList.Buttons_Rebuild();
            cHeadingList.Refresh();
        }

    }

    class MultiButtonPic_HeadingList : classMultiButtonPic 
    {
        groupboxHeadingList grbHeadingList = null;

        List<classMultiButtonPic.classButton> lstButtons = new List<classButton>();
        List<classMultiButtonPic.classButton> lstButtons_Reserved = new List<classButton>();
        public MultiButtonPic_HeadingList(ref groupboxHeadingList grbHeadingList)
        {
            this.grbHeadingList = grbHeadingList;
            BackColor = Color.Gray;
            ForeColor = Color.Black;

            Formation = classMultiButtonPic.enuButtonFormation.manual;
            Dock = DockStyle.Fill;
            classMultiButtonPic cMyRef = this;

            BorderStyle = BorderStyle.Fixed3D;
            SizeChanged += cHeadingList_SizeChanged;
            FontChanged += cHeadingList_FontChanged;

            MouseWheel += cHeadingList_MouseWheel;

            placeObjects();
        }

        private void cHeadingList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                scroll_Up();
            else
                scroll_Down();
        }

        void scroll_Up()
        {
         grbHeadingList.   TopIndex = grbHeadingList.TopIndex - 1;
        }
        void scroll_Down()
        {
            grbHeadingList.TopIndex = grbHeadingList.TopIndex + 1;
        }

        void placeObjects()
        {
            Buttons_Rebuild();
            Refresh();
        }

        private void cHeadingList_FontChanged(object sender, EventArgs e)
        {
           
        }

        Font _fntHeadingList = new Font("Arial", 10);
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

        Font _fntHeadingList_UnderMouse = new Font("Arial", 10);
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

        Font _fntHeadingList_Selected = new Font("Arial", 10);
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


        private void cHeadingList_SizeChanged(object sender, EventArgs e)
        {
            Size szItemHeight = TextRenderer.MeasureText("Chibougamou", fntHeadingList);

            float numPerHalf = ((float)(Height - szItemHeight.Height) / (float)szItemHeight.Height) / 2.0f;
            intNumWords_Above = (int)Math.Floor(numPerHalf);
            intNumWords_Below = (int)Math.Ceiling(numPerHalf);
            intNumWordsDisplayed = 1 + NumWords_Above + NumWords_Below;

            placeObjects();
            grbHeadingList.pnl_SizeChanged(sender, e);
        }

        int intNumWords_Above = -1;
        int NumWords_Above { get { return intNumWords_Above; } }

        int intNumWords_Below = -1;
        int NumWords_Below { get { return intNumWords_Below; } }

        int intNumWordsDisplayed = -1;
        public int NumWordsDisplayed { get { return intNumWordsDisplayed; } }

        public classHeadingList.classHeadingList_Record SelectedHeading
        {
            get
            {
                if (lstRecords.Count >= NumWords_Below && NumWords_Below >0)
                    return lstRecords[NumWords_Below-1];
                return null;
            }
        }

        classButton cButtonSelected
        {
            get
            {
                if (lstButtons.Count > NumWords_Above)
                    return lstButtons[NumWords_Above];
                else 
                    return null;
            }
        }

    

        public void Buttons_Rebuild()
        {
            if (grbHeadingList.cDictionary == null) return;
            Color clrButtonBck = Color.White;

            if (NumWordsDisplayed <= 0)
            {
                return;
            }
            classMultiButtonPic cMyRef = this;
            
            // ensure list is the right length
            {
                if (lstRecords.Count < NumWordsDisplayed)
                {
                    int intIndex = 0;

                    if (lstRecords.Count > 0)
                        intIndex = lstRecords[0] != null
                                                  ? lstRecords[0].Index
                                                  : 0;
                    classDictionary cDictionary = grbHeadingList.cDictionary;
                   grbHeadingList.lstWords  = classHeadingList.Get(ref cDictionary, intIndex, NumWordsDisplayed);
                }
                else
                {
                    while (lstRecords.Count > NumWordsDisplayed && lstRecords.Count > 0)
                        lstRecords.RemoveAt(lstRecords.Count - 1);
                }
            }
            if(NumWordsDisplayed <=0) return;
            // build, draw & position all needed buttons
            Size szFont = TextRenderer.MeasureText("BCD", fntHeadingList);
            for (int intButtonCounter = 0; intButtonCounter < lstRecords.Count; intButtonCounter++)
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
                classHeadingList.classHeadingList_Record cRec = lstRecords[intButtonCounter];
                classButton cButton = lstButtons[intButtonCounter];

                if (cRec != null && cRec.strHeading != null)
                {
                    cButton.AutoSize = false;
                    cButton.Text = cRec.strHeading.Trim();
             
                    if (cButton == cButtonSelected)
                    { 
                        cButton.Font = fntHeadingList_Selected;
                        cButton.Backcolor_Highlight 
                            = cButton.Backcolor_Idle 
                            = Color.Blue;
                        cButton.Forecolor_Highlight
                            = cButton.Forecolor_Idle
                            = Color.White ;
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
                        cButton.Size = new Size(Width, szFont.Height+3);
                        if (intButtonCounter > 0)
                        {
                            classButton cBtn_prev = lstButtons[intButtonCounter - 1];
                            cButton.Location = new Point(cBtn_prev.Area.Left, cBtn_prev.Area.Bottom);
                        }
                        else
                        {
                            cButton.Location = new Point(0, 0);
                        }
                    }
                }
            }

            // remove excess buttons
            while (lstButtons.Count >= NumWordsDisplayed)
            {
                classMultiButtonPic.classButton cButton_Removed = lstButtons[lstButtons.Count - 1];
                lstButtons.Remove(cButton_Removed);
                Button_Sub(ref cButton_Removed);
                lstButtons_Reserved.Add(cButton_Removed);
            }

        }


        public List<classHeadingList.classHeadingList_Record> lstRecords
        {
            get
            { 
                return grbHeadingList != null 
                                       ? grbHeadingList.lstWords 
                                       : null;
            }
        }
    }


    public class classHeadingList
    {
        static BinaryFormatter formatter = new BinaryFormatter();
        public enum enuBuildWordList_Stages { Building_TempTree, Traverse_Heading_Tree, _numBuildWordList_Stages };
        enuBuildWordList_Stages _eBuildStage = enuBuildWordList_Stages.Building_TempTree;
        public enuBuildWordList_Stages eBuildStage
        {
            get { return _eBuildStage; }
            set { _eBuildStage = value; }
        }

        const int MaxHeadingSize = 64;
        const int MaxFilenameSize = 8;

        public static string strDictionaries_SourceDirectory = "c:\\Words_dictionaries\\";
        public static string strListFilesLeftFileName = "List_FilesLeft.fil";
        public static string strHeadingList_Bin_FileName = "HeadingList_";
        public static string str_complete = "HeadingListCompleted.txt";

        public static bool bolQuit;
        static classDictionary cBck_BuildHeadingList_CurrentDictionary = null;
        public BackgroundWorker bck_BuildHeadingList = new BackgroundWorker();
        FileStream fsTempHeadingTree = null;

        public static Semaphore semBuild = new Semaphore(1, 1);

        public static classHeadingList instance;
        public classHeadingList()
        {
            instance = this;
            bck_BuildHeadingList_Init();
        }

        static public List<classHeadingList_Record> getRoot(ref classDictionary cDictionary, int intNumReturn)
        {
            if (instance == null)
                new classHeadingList();
            return instance._getRoot(ref cDictionary, intNumReturn);
        }

        List<classHeadingList_Record> _getRoot(ref classDictionary cDictionary, int intNumReturn)
        {
            if (!cDictionary.cHeadingListInfo.bolHeadingListCompleted)
            {
                if (bck_BuildHeadingList.IsBusy)
                    return new List<classHeadingList_Record>();
                Build(ref cDictionary);
                return new List<classHeadingList_Record>();
            }

            cDictionary.HeadingListSize_Set();

            List<classHeadingList_Record> lstRetVal = new List<classHeadingList_Record>();
            for (int intCounter = 0; intCounter < intNumReturn; intCounter++)
            {
                classHeadingList_Record cWR = BuildHeadingList_Load(ref cDictionary, intCounter);
                lstRetVal.Add(cWR);
            }
            return lstRetVal;
        }

        static public List<classHeadingList_Record> Get(ref classDictionary cDictionary, int intStartIndex, int intNumReturn)
        {
            List<classHeadingList_Record> lstRetVal = new List<classHeadingList_Record>();
            if (cDictionary == null) return lstRetVal;

            for (int intIndexCounter = intStartIndex; intIndexCounter < intStartIndex + intNumReturn; intIndexCounter++)
            {
                classHeadingList_Record cRec = BuildHeadingList_Load(ref cDictionary, intIndexCounter);
                if (cRec != null)
                    lstRetVal.Add(cRec);
            }

            return lstRetVal;
        }
        static public List<classHeadingList_Record> Get(ref classDictionary cDictionary, string strWord, int intNumReturn)
        {
            if (instance == null)
                new classHeadingList();
            return instance._Get(ref cDictionary, strWord, intNumReturn);
        }

        List<classHeadingList_Record> _Get(ref classDictionary cDictionary, string strHeading, int intNumReturn)
        {
            strHeading = classStringLibrary.Deaccent(  strHeading.ToLower().Trim());
            if (cDictionary == null) return new List<classHeadingList_Record>();
            if (!cDictionary.cHeadingListInfo.bolHeadingListCompleted)
            {
                if (bck_BuildHeadingList.IsBusy)
                    return new List<classHeadingList_Record>();
                Build(ref cDictionary);
                return new List<classHeadingList_Record>();
            }

            // measure size of list
            int intIndex = cDictionary.cHeadingListInfo.NumEntries / 2;
            int intStepSize = intIndex;
            int intNumErrors = 0;
            int intNumMaxErrors = 32;


            int intBestMatch = 0;
            int intBestIndex = -1;
            string strBest = "";

            classHeadingList_Record cWR = BuildHeadingList_Load(ref cDictionary, intIndex);
            while (cWR != null && intNumErrors < intNumMaxErrors)
            {
                string strTestHeading = classStringLibrary.Deaccent(cWR.strHeading.ToLower().Trim());
                int intComparison = string.Compare(strHeading, strTestHeading);
                if (intComparison == 0)
                {
                    goto ExitResult;
                }
                else
                {
                    int intTest = StringLibrary.classStringLibrary.StringCompare_NumCharMatch(strHeading, strTestHeading);

                    if (intTest > intBestMatch)
                    {
                        intBestMatch = intTest;
                        intBestIndex = intIndex;
                        strBest = cWR.strHeading;
                    }

                    intStepSize /= 2;
                    if (intStepSize < 1)
                    {
                        intStepSize = 1;
                        intNumErrors++;
                    }
                    intIndex += intComparison * intStepSize;

                    if (intIndex < 0)
                        intIndex = 0;
                    else if (intIndex > cDictionary.cHeadingListInfo.NumEntries)
                        intIndex = cDictionary.cHeadingListInfo.NumEntries;

                }
                cWR = BuildHeadingList_Load(ref cDictionary, intIndex);
            }
            // exact search failed - scan for closest spelling
            int intNumTest = 32;
            int intStart = intBestIndex - intNumTest / 2;
            int intEnd = intBestIndex + intNumTest / 2;
            intBestMatch = 0;
            for (int intTestIndex = intStart;
                     intTestIndex <= intEnd;
                     intTestIndex++)
            {
                if (intTestIndex >= 0 && intTestIndex < cDictionary.cHeadingListInfo.NumEntries)
                {
                    cWR = BuildHeadingList_Load(ref cDictionary, intTestIndex);
                    string strTestHeading = classStringLibrary.Deaccent( cWR.strHeading.Trim().ToLower());
                    int intTest = StringLibrary
                                    .classStringLibrary
                                    .StringCompare_NumCharMatch(strHeading, strTestHeading);

                    if (intTest > intBestMatch)
                    {
                        intBestMatch = intTest;
                        intBestIndex = intTestIndex;
                        strBest = cWR.strHeading;
                    }
                }
            }
            intIndex = intBestIndex;

        ExitResult:
            List<classHeadingList_Record> lstRetVal = new List<classHeadingList_Record>();
            int intNumBefore = intNumReturn / 2;
            int intNumAfter = intNumReturn - intNumBefore - 1;

            for (int intIndexCounter = 1; intIndex - intIndexCounter >= 0 && intIndexCounter < intNumBefore; intIndexCounter++)
            {
                classHeadingList_Record cRec = BuildHeadingList_Load(ref cDictionary, intIndex - intIndexCounter);
                lstRetVal.Insert(0, cRec);
            }

            for (int intIndexCounter = 0;
                (intIndexCounter <= intNumAfter || lstRetVal.Count < intNumReturn) && intIndex + intIndexCounter < cDictionary.cHeadingListInfo.NumEntries;
                intIndexCounter++)
            {
                classHeadingList_Record cRec = BuildHeadingList_Load(ref cDictionary, intIndex + intIndexCounter);
                lstRetVal.Add(cRec);
            }

            return lstRetVal;
        }

        public static int NumEntries(ref classDictionary cDictionary)
        {
            int intRetVal = 0;
            cDictionary.cHeadingListInfo.sem.WaitOne();
            if (System.IO.File.Exists(cDictionary.cHeadingListInfo.Filename))
            {
                FileStream fs = new FileStream(cDictionary.cHeadingListInfo.Filename, FileMode.Open);
                intRetVal = (int)(fs.Length / BuildHeadingList_Size);
                fs.Close();
            }
            cDictionary.cHeadingListInfo.sem.Release();
            return intRetVal;
        }

        bool bolHeadingListBuildNeeded = false;
        public bool HeadingListBuildNeeded
        {
            get { return bolHeadingListBuildNeeded; }
            set
            {
                if (bolHeadingListBuildNeeded != value)
                {
                    bolHeadingListBuildNeeded = value;
                }
            }
        }

        void Build(ref classDictionary cDictionary)
        {
            if (!formWords.bolInit) return;
            string strFilename = cDictionary.cHeadingListInfo.Filename;
            if (System.IO.File.Exists(strFilename))
                return;//System.IO.File.Delete(strFilename);

            if (instance == null)
                new classHeadingList();
            eBuildStage = enuBuildWordList_Stages.Building_TempTree;

            cBck_BuildHeadingList_CurrentDictionary = cDictionary;

            proBar.Minimum = 0;
            proBar.Maximum = classBT_Dictionaries.Bin_NumRecords_Get(ref cBck_BuildHeadingList_CurrentDictionary, enuFileDataTypes.heading);
            proBar.Value = 0;
            proBar.Text = "Building Word List";
            proBar.BackColor = Color.White;
            proBar.ForeColor = Color.Blue;
            proBar.Show();

            bck_BuildHeadingList.RunWorkerAsync();
        }

        #region BackgroundWorker_BuildHeadingList

        void bck_BuildHeadingList_Init()
        {
            bck_BuildHeadingList.DoWork += Bck_BuildHeadingList_DoWork;
            bck_BuildHeadingList.ProgressChanged += Bck_BuildHeadingList_ProgressChanged;
            bck_BuildHeadingList.RunWorkerCompleted += Bck_BuildHeadingList_RunWorkerCompleted;
            bck_BuildHeadingList.WorkerReportsProgress = true;
            bck_BuildHeadingList.WorkerSupportsCancellation = true;
        }

        private void Bck_BuildHeadingList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            proBar.Hide();
            cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.NumEntries = NumEntries(ref cBck_BuildHeadingList_CurrentDictionary);
            cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.bolHeadingListCompleted = true;
            groupboxHeadingList.instance.InitHeadingList();
            bolQuit = false;
            semBuild.Release();
        }


        static bool _bolProbar_Reset = false;
        static bool bolProbar_Reset
        {
            get { return _bolProbar_Reset; }
            set { _bolProbar_Reset = value; }
        }


        static int _intprobarMinimum = -1;
        static public int probarMinimum
        {
            get { return _intprobarMinimum; }
            set { _intprobarMinimum = value; }
        }

        static int _intprobarMaximum = -1;
        static public int probarMaximum
        {
            get { return _intprobarMaximum; }
            set { _intprobarMaximum = value; }
        }

        Ck_Objects.progressbar proBar { get { return groupboxHeadingList.instance.proBar; } }
        private void Bck_BuildHeadingList_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (proBar.Minimum != probarMaximum || proBar.Maximum != probarMaximum)
            {
                proBar.Minimum = probarMinimum;
                proBar.Maximum = probarMaximum;
            }
            if (bolProbar_Reset)
            {
                proBar.Minimum
                    = proBar.Value
                    = probarMinimum;
                proBar.Maximum = probarMaximum;
                bolProbar_Reset = false;
            }

            float fltPC = (float)(100.0 * (e.ProgressPercentage - proBar.Minimum) / (float)(proBar.Maximum - proBar.Minimum));

            proBar.Heading = eBuildStage.ToString() + " %" + fltPC.ToString("00.00");
            proBar.ResetText();
            proBar.Value = e.ProgressPercentage;

        }

        static string HeadingListCompleted_Filename(ref classDictionary cDictionary)
        {
            return cDictionary.strSourceDirectory + "HeadingListComplete.txt";
        }

        static void HeadingListCompleted_Write(ref classDictionary cDictionary) { System.IO.File.WriteAllText(HeadingListCompleted_Filename(ref cDictionary), "Complete"); }

        static bool HeadingListCompleted_Read(ref classDictionary cDictionary) { return System.IO.File.Exists(HeadingListCompleted_Filename(ref cDictionary)); }

        void Bck_BuildHeadingList_DoWork(object sender, DoWorkEventArgs e)
        {
            semBuild.WaitOne();

            if (System.IO.File.Exists(cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.Filename))
                System.IO.File.Delete(cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.Filename);

            if (!HeadingListCompleted_Read(ref cBck_BuildHeadingList_CurrentDictionary))
            {
                bck_BuildHeadingList.ReportProgress(0);
                intBuildHeadingList_NumEntries = 0;
                // 1) for every index in BinarySearch tree - insert each LL Heading/filename into temp binTree
                BuildHeadingList_BuildTempFile_BinaryTree();

                // 2) traverse temp Heading binary-tree to build Heading-List
                BuildHeadingList_TraverseTempTree_InOrder();

                if (bolQuit || bck_BuildHeadingList.CancellationPending)
                    return;
            }
        }

        int intBuildHeadingList_NumEntries = 0;
        int intBuildHeadingList_TraverseCounter = 0;

        void BuildHeadingList_TraverseTempTree_InOrder()
        {
            probarMinimum = 0;
            probarMaximum = intBuildHeadingList_NumEntries;
            bolProbar_Reset = true;
            eBuildStage = enuBuildWordList_Stages.Traverse_Heading_Tree;
            intBuildHeadingList_TraverseCounter = 0;

            if (System.IO.File.Exists(cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.Filename))
                System.IO.File.Delete(cBck_BuildHeadingList_CurrentDictionary.cHeadingListInfo.Filename);

            classHeadingBinTree_Record cRec = BuildHeadingList_BuildTempFile_BinaryTree_Load(0);

            BuildHeadingList_TraverseTempTree_InOrder_Recurse(ref cRec);

            fsTempHeadingTree.Close();
        }

        void BuildHeadingList_TraverseTempTree_InOrder_Recurse(ref classHeadingBinTree_Record cRec)
        {
            if (cRec == null) return;
            if (cRec.Left >= 0)
            {
                classHeadingBinTree_Record cRec_Left = BuildHeadingList_BuildTempFile_BinaryTree_Load(cRec.Left);
                BuildHeadingList_TraverseTempTree_InOrder_Recurse(ref cRec_Left);
            }

            classHeadingList_Record cHeadingListRecord = new classHeadingList_Record(cRec.Heading, cRec.filename);
            BuildHeadingList_Append(ref cBck_BuildHeadingList_CurrentDictionary, ref cHeadingListRecord);
            if (intBuildHeadingList_TraverseCounter++ % 32 == 0)
                bck_BuildHeadingList.ReportProgress(intBuildHeadingList_TraverseCounter);

            if (cRec.Right >= 0)
            {
                classHeadingBinTree_Record cRec_Right = BuildHeadingList_BuildTempFile_BinaryTree_Load(cRec.Right);
                BuildHeadingList_TraverseTempTree_InOrder_Recurse(ref cRec_Right);
            }
        }

        string BuildHeadingList_TempFile_BinTree_Name { get { return cBck_BuildHeadingList_CurrentDictionary.strSourceDirectory + "HeadingList_BinaryTree.temp"; } }

        void BuildHeadingList_BuildTempFile_BinaryTree()
        {
            if (System.IO.File.Exists(BuildHeadingList_TempFile_BinTree_Name))
                System.IO.File.Delete(BuildHeadingList_TempFile_BinTree_Name);

            fsTempHeadingTree = new FileStream(BuildHeadingList_TempFile_BinTree_Name, FileMode.Create);

            int intNumSourceEntries = cBck_BuildHeadingList_CurrentDictionary.cFileData[(int)enuFileDataTypes.heading].int_BinRec_Index_NextAvailable;
            bolProbar_Reset = true;
            probarMinimum = 0;
            probarMaximum = intNumSourceEntries;

            for (int intIndexCounter = 0; intIndexCounter < intNumSourceEntries; intIndexCounter++)
            {
                class_BinTree_Record cBTSource = classBT_Dictionaries.BinRec_Load(ref cBck_BuildHeadingList_CurrentDictionary, intIndexCounter, enuFileDataTypes.heading);

                if (cBTSource != null)
                {
                    class_LL_Record cLL = classBT_Dictionaries.LL_Record_Load(ref cBck_BuildHeadingList_CurrentDictionary, cBTSource.LL, enuFileDataTypes.heading);
                    while (cLL != null)
                    {
                        BuildHeadingList_BuildTempFile_BinaryTree_Insert(ref cLL);
                        intBuildHeadingList_NumEntries++;
                        cLL = classBT_Dictionaries.LL_Record_Load(ref cBck_BuildHeadingList_CurrentDictionary, cLL.next, enuFileDataTypes.heading);
                    }
                }
                if (intIndexCounter % 32 == 0)
                    bck_BuildHeadingList.ReportProgress(intIndexCounter);
            }
        }


        void BuildHeadingList_BuildTempFile_BinaryTree_Save(long lngAddr, ref classHeadingBinTree_Record cRec)
        {
            fsTempHeadingTree.Position = lngAddr;
            formatter.Serialize(fsTempHeadingTree, (string)cRec.Heading);
            formatter.Serialize(fsTempHeadingTree, (string)cRec.filename);
            formatter.Serialize(fsTempHeadingTree, (long)cRec.Left);
            formatter.Serialize(fsTempHeadingTree, (long)cRec.Right);
        }

        classHeadingBinTree_Record BuildHeadingList_BuildTempFile_BinaryTree_Load(long lngAddr)
        {
            if (lngAddr < 0 || lngAddr >= fsTempHeadingTree.Length)
                return null;

            classHeadingBinTree_Record cRetVal = new classHeadingBinTree_Record();
            fsTempHeadingTree.Position
                = cRetVal.Addr
                = lngAddr;

            cRetVal.Heading = (string)formatter.Deserialize(fsTempHeadingTree);
            cRetVal.filename = (string)formatter.Deserialize(fsTempHeadingTree);
            cRetVal.Left = (long)formatter.Deserialize(fsTempHeadingTree);
            cRetVal.Right = (long)formatter.Deserialize(fsTempHeadingTree);


            return cRetVal;
        }


        void BuildHeadingList_BuildTempFile_BinaryTree_Search(string strHeading)
        {

        }



        void BuildHeadingList_BuildTempFile_BinaryTree_Insert(ref class_LL_Record cLL)
        {
            if (cLL.strHeading.Trim().Length == 0) return;
            classHeadingBinTree_Record cRoot = BuildHeadingList_BuildTempFile_BinaryTree_Load(0);
            if (cRoot == null)
            {
                cRoot = new classHeadingBinTree_Record(cLL.strHeading, cLL.FileName);
                BuildHeadingList_BuildTempFile_BinaryTree_Save(0, ref cRoot);
                return;
            }

            classHeadingBinTree_Record cRec = cRoot;


            // fix heading to put lead numeral at the end
            string strHeading = cLL.strHeading.Trim();
            if (!char.IsLetter(strHeading[0]))
                strHeading += " ";

            int intCount = 0;


            while (intCount < strHeading.Length && !char.IsLetter(strHeading[0]))
            {
                Char chrTemp = strHeading[0];
                strHeading = strHeading.Substring(1) + chrTemp.ToString();
                intCount++;
            }

            do
            {
                int intComparison = string.Compare(strHeading.Trim(), cRec.Heading.Trim());
                if (intComparison < 0)
                {
                    if (cRec.Left >= 0)
                    {
                        cRec = BuildHeadingList_BuildTempFile_BinaryTree_Load(cRec.Left);
                    }
                    else
                    {
                        cRec.Left = fsTempHeadingTree.Length;
                        BuildHeadingList_BuildTempFile_BinaryTree_Save(cRec.Addr, ref cRec);
                        classHeadingBinTree_Record cRecNew = new classHeadingBinTree_Record(strHeading, cLL.FileName);
                        BuildHeadingList_BuildTempFile_BinaryTree_Save(fsTempHeadingTree.Length, ref cRecNew);
                        return;
                    }
                }
                else if (intComparison > 0)
                {
                    if (cRec.Right >= 0)
                    {
                        cRec = BuildHeadingList_BuildTempFile_BinaryTree_Load(cRec.Right);
                    }
                    else
                    {
                        cRec.Right = fsTempHeadingTree.Length;
                        BuildHeadingList_BuildTempFile_BinaryTree_Save(cRec.Addr, ref cRec);
                        classHeadingBinTree_Record cRecNew = new classHeadingBinTree_Record(strHeading, cLL.FileName);
                        BuildHeadingList_BuildTempFile_BinaryTree_Save(fsTempHeadingTree.Length, ref cRecNew);
                        return;
                    }
                }
                else
                {
                    // this heading has multiple words and has already been processed - ignore and exit
                    return;
                }

            } while (true);

        }




        private void Bck_Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            proBar.Hide();
        }

        private static void Bck_Search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        #endregion

        #region "initfiles"

        public static string Standard_Heading(string strHeadingList)
        {
            strHeadingList = strHeadingList.PadRight(MaxHeadingSize, ' ');
            strHeadingList = strHeadingList.Substring(0, MaxHeadingSize);
            return strHeadingList;
        }

        public static string Standard_Filename(string strHeadingList)
        {
            strHeadingList = strHeadingList.PadRight(MaxFilenameSize, ' ');
            strHeadingList = strHeadingList.Substring(0, MaxFilenameSize);
            return strHeadingList;
        }


        /// <summary>
        /// formats FileName for bin-tree file insertion
        /// </summary>
        /// <param name="strInputFileName"></param>
        /// <returns></returns>
        public static string getFileName(string strInputFileName)
        {
            strInputFileName = strInputFileName.ToLower().Trim();

            if (strInputFileName.Length > 12)
                strInputFileName = strInputFileName.Substring(strInputFileName.Length - 12, 8);

            strInputFileName = strInputFileName.PadLeft(MaxFilenameSize, ' ').Substring(0, MaxFilenameSize);
            return strInputFileName;
        }

        public static enuFileExtensions GetFileExtension(string strFileName)
        {
            int intLastIndex = strFileName.LastIndexOf('.');
            if (intLastIndex >= 0 && intLastIndex < strFileName.Length - 1)
            {
                string strExtension = strFileName.Substring(intLastIndex + 1);
                for (int intCounter = 0; intCounter < (int)enuFileExtensions._num; intCounter++)
                {
                    enuFileExtensions eFileExtension = (enuFileExtensions)intCounter;
                    string strTest = eFileExtension.ToString();
                    if (strTest.Length == strExtension.Length)
                    {
                        if (string.Compare(strTest, strExtension) == 0)
                            return eFileExtension;
                    }
                }
            }
            return enuFileExtensions._num;
        }

        /// <summary>
        /// formats FileName for bin-tree insertion
        /// </summary>
        /// <param name="intFileName"></param>
        /// <returns></returns>
        static string getFileName(int intFileName) { return getFileName(intFileName.ToString().PadLeft(MaxFilenameSize)); }



        #endregion

        #region "Handle files"


        static bool validWord(string strWord)
        {
            string strValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZĀAĒEĪIŌOŪUāaēeīiōoūuÀÁÂÃÄÅàáâãäåÇçÈÉÊËèéêëḥÌÍÎÏ¡ìíîïïíÐÑñÒÓÔÕÖðòóôõöōøØŠšÙÚÛÜùúûüxÝŸýÿŽžÆŒæœ$&?!:;.,[]()-_=+/`'₁₂₅₆₀²³ 0123456789";
            strWord = strWord.Trim();
            for (int intChrIndex = 0; intChrIndex < strWord.Length; intChrIndex++)
            {
                char c = strWord[intChrIndex];
                if (!strValidCharacters.Contains(c))
                    return false;
            }
            return true;
        }

        #endregion


        #region "Bin tree"


        static long lngBinRec_Size = -1;

        public static long BuildHeadingList_Size
        {
            get
            {
                if (lngBinRec_Size < 0)
                {

                    string strTempFileName = System.IO.Directory.GetCurrentDirectory() + "\\tempFileName.flx";
                    if (System.IO.File.Exists(strTempFileName))
                        System.IO.File.Delete(strTempFileName);
                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(strTempFileName, FileMode.Create);
                        classHeadingList_Record cBinRec = new classHeadingList_Record();

                        fs.Position = 0;

                        formatter.Serialize(fs, Standard_Heading("chibougamou"));
                        formatter.Serialize(fs, Standard_Filename("12345768"));

                        // add extra space for accented words that fuck things up
                        formatter.Serialize(fs, (long)0);
                        formatter.Serialize(fs, (long)0);
                        formatter.Serialize(fs, (long)0);
                        formatter.Serialize(fs, (long)0);
                        formatter.Serialize(fs, (long)0);

                        lngBinRec_Size = (long)fs.Position;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Print("error :" + e.Message);
                    }

                    if (fs != null) fs.Close();
                }
                return lngBinRec_Size;
            }
        }


        public static classHeadingList_Record BuildHeadingList_Load(ref classDictionary cDictionary, int index)
        {
            classHeadingList_Record cRetVal = new classHeadingList_Record();
            if (cDictionary == null) return cRetVal;
            if (index < 0 || index > cDictionary.cHeadingListInfo.NumEntries) return new classHeadingList_Record();

            if (!System.IO.File.Exists(cDictionary.cHeadingListInfo.Filename)) return null;

            cDictionary.cHeadingListInfo.sem.WaitOne();
            FileStream fs = new FileStream(cDictionary.cHeadingListInfo.Filename, FileMode.Open);
            fs.Position = (long)(index * BuildHeadingList_Size);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                cRetVal.strHeading = ((string)formatter.Deserialize(fs)).Trim();
                cRetVal.strFilename = ((string)formatter.Deserialize(fs)).Trim();
                cRetVal.Index = index;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }

            fs.Close();
            cDictionary.cHeadingListInfo.sem.Release();
            return cRetVal;
        }


        public static void BuildHeadingList_Append(ref classDictionary cDictionary, ref classHeadingList_Record cBinRec)
        {
            if (cBinRec == null) return;
            cDictionary.cHeadingListInfo.sem.WaitOne();

            FileStream fs = null;
            string strFilename = cDictionary.cHeadingListInfo.Filename;
            if (System.IO.File.Exists(strFilename))
                fs = new FileStream(strFilename, FileMode.Open);
            else
                fs = new FileStream(strFilename, FileMode.Create);

            int intIndex = (int)Math.Ceiling((double)fs.Length / (double)BuildHeadingList_Size);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                fs.Position = (long)(lngBinRec_Size * intIndex);
                formatter.Serialize(fs, Standard_Heading(cBinRec.strHeading));
                formatter.Serialize(fs, Standard_Filename(cBinRec.strFilename));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }

            fs.Close();
            cDictionary.cHeadingListInfo.sem.Release();


        }

        public static void BuildHeadingList_Save(ref classDictionary cDictionary, int index, ref classHeadingList_Record cBinRec)
        {
            cDictionary.cHeadingListInfo.sem.WaitOne();

            string strFilename = cDictionary.cHeadingListInfo.Filename;
            FileStream fs = null;

            if (System.IO.File.Exists(strFilename))
                fs = new FileStream(strFilename, FileMode.Open);
            else
                fs = new FileStream(strFilename, FileMode.Create);

            fs.Position = (long)(index * BuildHeadingList_Size);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, Standard_Heading(cBinRec.strHeading));
                formatter.Serialize(fs, Standard_Filename(cBinRec.strFilename));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }
            fs.Close();
            cDictionary.cHeadingListInfo.sem.Release();
        }

        #endregion

        public class classHeadingBinTree_Record
        {
            public long Left = -1;
            public long Right = -1;

            public string Heading = "";
            public string filename = "";

            public long Addr = -1;
            public classHeadingBinTree_Record(string heading, string filename)
            {
                if (heading.Length == 0)
                {
                    MessageBox.Show("This is a problem: heading length =0");
                    return;
                }

                this.Heading = heading.Trim();
                this.filename = filename;
            }
            public classHeadingBinTree_Record() { }
        }

        public class classHeadingList_Record
        {
            //      public int parent;
            public string strHeading;
            public string strFilename;
            int intIndex = -1;
            public int Index
            {
                get { return intIndex; }
                set { intIndex = value; }
            }

            public classHeadingList_Record() { }
            public classHeadingList_Record(string strHeading, string strFilename)
            {
                this.strHeading = strHeading;
                this.strFilename = strFilename;
            }
        }
    }
}
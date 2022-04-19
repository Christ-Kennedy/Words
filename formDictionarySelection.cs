using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using Ck_Objects;
using Math3;
using StringLibrary;

namespace Words
{
    public enum enuSearchType { Not_Searched, Heading, Content, Both_Heading_And_Content, _num };
    public enum enuPopUpDelayTime { One_Second, Three_Seconds, Five_Seconds, Ten_Seconds, RightMouse_Click, _numPopUpDelayTime};

    public partial class formDictionarySelection : Form
    {
        public static formDictionarySelection instance;
        public classLabelButton lbtnAdd = new classLabelButton();
        public classLabelButton lbtnHide = new classLabelButton();
        public Label lblPopUpDelayTime = new Label();
        ListBox lbxPopUpDelayTime = new ListBox();

        Size szForm = new Size(15, 45);
        public List<panelSelector> lstPnlSelector = new List<panelSelector>();
        panelSP.panelSP pnlSP = new panelSP.panelSP();

        Timer tmrClipBoard = new Timer();

        public static enuSearchType GetSearchTypeFromString(string strSearchType)
        {
            if (strSearchType != null && strSearchType.Length >0)
            {
                string strIndex = "NHCB_";
                char chrTest = strSearchType.ToUpper()[0];
                int intIndex = strIndex.IndexOf(chrTest);
                if (intIndex >= 0) return (enuSearchType)intIndex;
            }
            return enuSearchType._num;
        }

        static enuPopUpDelayTime _ePopUpDelayTime = enuPopUpDelayTime.Three_Seconds;
        public static enuPopUpDelayTime ePopUpDelayTime
        {
            get { return _ePopUpDelayTime; }
            set 
            {
                _ePopUpDelayTime = value;
                panelDefinitionPort.tmrMouseHover_Interval_Set();
            }
        }

       static classDictionary cDictionaryPopUp = null;
        static public classDictionary PopUp_Dictionary
        {
            get { return cDictionaryPopUp; }
            set { cDictionaryPopUp = value; }
        }
        public panelSelector pnlPopUpReference
        {
            set
            {
                cDictionaryPopUp = null;
                panelSelector PnlSelector = value;
                if (PnlSelector != null)
                {
                    for (int intCounter = 0; intCounter < PnlSelector.lstToggles.Count; intCounter++)
                    {
                        if (!PnlSelector.lstToggles[intCounter].lstButtons[(int)enuSearchType.Not_Searched].Toggled)
                        {
                            PopUp_Dictionary = PnlSelector.lstToggles[intCounter].cDictionary;
                            return;
                        }
                    }
                }
            }
        }
                   
       classDictionary cDictionaryUseClipBoard = null;
        public classDictionary UseClipBoard_Dictionary
        {
            get { return cDictionaryUseClipBoard; }
        }
        public panelSelector pnlUseClipBoard
        {
            set
            {
                cDictionaryUseClipBoard = null;
                panelSelector PnlSelector = value;
                if (PnlSelector != null)
                {
                    for (int intCounter = 0; intCounter < PnlSelector.lstToggles.Count; intCounter++)
                    {
                        if (!PnlSelector.lstToggles[intCounter].lstButtons[(int)enuSearchType.Not_Searched].Toggled)
                        {
                            cDictionaryUseClipBoard = PnlSelector.lstToggles[intCounter].cDictionary;
                            return;
                        }
                    }
                }
            }
        }
                     
        public formDictionarySelection()
        {
            instance = this;
            InitializeComponent();

            Controls.Add(pnlSP);


            Controls.Add(lbtnAdd);
            lbtnAdd.AutoSize = true;
            lbtnAdd.Text = "Add";
            lbtnAdd.Click += lbtnAdd_Click;

            Controls.Add(lbtnHide);
            lbtnHide.AutoSize = true;
            lbtnHide.Text = "Hide";
            lbtnHide.Click += lbtnHide_Click;

            Controls.Add(lblPopUpDelayTime);
            lblPopUpDelayTime.AutoSize = true;
            lblPopUpDelayTime.Text = "PopUpDelayTime";
            lblPopUpDelayTime.MouseEnter += LblPopUpDelayTime_MouseEnter;

            Controls.Add(lbxPopUpDelayTime);
            int intMinWidth = 0;
            for (int intPopCounter = 0; intPopCounter < (int)enuPopUpDelayTime._numPopUpDelayTime; intPopCounter++)
            {
                enuPopUpDelayTime ePUDT = (enuPopUpDelayTime)intPopCounter;
                string strPUDT = ePUDT.ToString().Replace('_', ' ');
                lbxPopUpDelayTime.Items.Add(strPUDT);
                Size szText = TextRenderer.MeasureText(strPUDT, lbxPopUpDelayTime.Font);
                if (szText.Width > intMinWidth)
                    intMinWidth = szText.Width;
            }
            lbxPopUpDelayTime.SelectedIndex = (int)ePopUpDelayTime;
            lbxPopUpDelayTime.Width = intMinWidth + 10;
            lbxPopUpDelayTime.Height = (int)enuPopUpDelayTime._numPopUpDelayTime * lbxPopUpDelayTime.ItemHeight + 10;
            lbxPopUpDelayTime.Hide();
            lbxPopUpDelayTime.SelectedIndexChanged += LbxPopUpDelayTime_SelectedIndexChanged;
            lbxPopUpDelayTime.MouseLeave += LbxPopUpDelayTime_MouseLeave;
            MouseLeave += LbxPopUpDelayTime_MouseLeave;

            Bitmap bmp = new Bitmap(Properties.Resources.Selection);
            Icon = Icon.FromHandle(bmp.GetHicon());

            bmp = Properties.Resources.DictionarySelection;

            BackColor = bmp.GetPixel(classRND.Get_Int(0, bmp.Width), classRND.Get_Int(0, bmp.Height));

            tmrClipBoard.Interval = 500;
            tmrClipBoard.Tick += TmrClipBoard_Tick;

            Text = "Dictionary Selection";
            formWords.instance.ToolTip_SetUp();

            DictionarySelections_Load();

            TmrClipboard_Set();

            WindowState = FormWindowState.Maximized;

            SizeChanged += FormDictionarySelection_SizeChanged;
            Disposed += FormDictionarySelection_Disposed;
            Activated += FormDictionarySelection_Activated;
        }

        private void LbxPopUpDelayTime_MouseLeave(object sender, EventArgs e)
        {
            lbxPopUpDelayTime.Hide();
        }

        private void LbxPopUpDelayTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ePopUpDelayTime = (enuPopUpDelayTime)lbxPopUpDelayTime.SelectedIndex;
        }

        private void LblPopUpDelayTime_MouseEnter(object sender, EventArgs e)
        {
            lbxPopUpDelayTime.Location = lblPopUpDelayTime.Location;
            lbxPopUpDelayTime.BringToFront();
            lbxPopUpDelayTime.Visible = true;
        }

        bool bolInit = false;

        private void FormDictionarySelection_Activated(object sender, EventArgs e)
        {
            if (bolInit) return;
            placeObjects();
        }

        private void FormDictionarySelection_Disposed(object sender, EventArgs e)
        {
            DictionarySelections_Save();
        }
        

        void lbtnHide_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public void TmrClipboard_Set()
        {
            tmrClipBoard.Enabled = UseClipBoard_Dictionary != null;
        }

        string strClipboardContent = "";
        private void TmrClipBoard_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string strNewClipboardContent = Clipboard.GetText();
                    if (string.Compare(strClipboardContent, strNewClipboardContent) != 0)
                    {
                        strClipboardContent = strNewClipboardContent;
                        List<string> lstWords = classStringLibrary.getFirstWords(strClipboardContent);
                        List<classSearchParameters> lstSearchParameters = new List<classSearchParameters>();

                        for (int intPanelSelector_Counter = 0; intPanelSelector_Counter < formDictionarySelection.instance.lstPnlSelector.Count; intPanelSelector_Counter++)
                        {
                            panelSelector pnlSelector = formDictionarySelection.instance.lstPnlSelector[intPanelSelector_Counter];
                            if (pnlSelector.cBtnUseClipBoard.Toggled)
                            {
                                for (int intDictionaryCounter = 0; intDictionaryCounter < pnlSelector.lstToggles.Count; intDictionaryCounter++)
                                {
                                    panelSelector.classButtonArray cBtnArray = pnlSelector.lstToggles[intDictionaryCounter];
                                    lstSearchParameters.AddRange(formWords.instance.Search_GetParameters(ref cBtnArray, lstWords));
                                }
                            }
                        }

                        formWords.instance.Search(ref lstSearchParameters, enuSearchRequestor.clipboard);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("clipboard error: " + err.Message);
            }
        }

        List<classSearchResult> _lstSearchResults_Clipboard = new List<classSearchResult>();
        public List<classSearchResult> lstSearchResults_Clipboard
        {
            get { return _lstSearchResults_Clipboard; }
            set
            {
                _lstSearchResults_Clipboard = value;
                IEnumerable<classSearchResult> querySearchResults = _lstSearchResults_Clipboard.OrderBy(result => result.strFileName);
                _lstSearchResults_Clipboard = (List<classSearchResult>)querySearchResults.ToList<classSearchResult>();

                formWords.instance.DictionaryOutput(_lstSearchResults_Clipboard, "Search : " + strClipboardContent);
            }
        }
        private void lbtnAdd_Click(object sender, EventArgs e)
        {
            panelSelector pnlSelector_New = new panelSelector();
            lstPnlSelector.Add(pnlSelector_New);

            Panel pnlContainer = new Panel();
            pnlContainer.Size = pnlSelector_New.Size;

            pnlContainer.Controls.Add(pnlSelector_New);
            pnlSelector_New.Dock = DockStyle.Fill;

            panelSP.classSweepAndPrune_Element cEleNew = pnlSP.Add(ref pnlContainer);
            if (pnlSP.lstElements.Count > 1)
            {
                panelSP.classSweepAndPrune_Element cEleLast = pnlSP.lstElements[pnlSP.lstElements.Count - 2];
                //cEleNew.recArea.Location = new Point(cEleLast.recArea.Right, cEleLast.recArea.Top);
                cEleNew.Location= new Point(cEleLast.recArea.Right, cEleLast.recArea.Top);
            }
            cEleNew.Size = pnlSelector_New.Size;
            pnlSP.setRecArea_Auto();

            placeObjects();
            formWords.instance.ToolTip_SetUp();
        }

        private void FormDictionarySelection_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        public void placeObjects()
        {
            Size szForm = new Size(15, 5);
            lbtnAdd.Left = 0;
            lbtnHide.Left = lbtnAdd.Right + 5;
            lblPopUpDelayTime.Left = lbtnHide.Right + 5;

            lbtnAdd.Top
                = lbtnHide.Top
                = lblPopUpDelayTime.Top 
                = szForm.Height;
            
            if (lstPnlSelector.Count == 0) return;
            
            pnlSP.Location = new Point(0, lbtnAdd.Bottom);
            pnlSP.Size = new Size(Width - szForm.Width, Height - pnlSP.Top- 40 );
            Point ptLocation = new Point(0, 0);
            for (int intPanelCounter = 0; intPanelCounter < pnlSP.lstElements.Count; intPanelCounter++)
            {
                panelSP.classSweepAndPrune_Element cEle = pnlSP.lstElements[intPanelCounter];
                cEle.recArea = new Rectangle(ptLocation, cEle.recArea.Size);
                ptLocation.X += cEle.recArea.Width;
            }
            pnlSP.PlaceObjects();

        }

        public void DeletePanel(ref panelSelector pnlSelector_Delete)
        {
            if (lstPnlSelector.Contains(pnlSelector_Delete))
            {
                lstPnlSelector.Remove(pnlSelector_Delete);

                Panel pnlParent = (Panel)pnlSelector_Delete.Parent;

                pnlSP.Sub(ref pnlParent);
                //pnlSP.setRecArea_Auto();
                //placeObjects();
            }
        }



        #region XML


        string StrXmlDictionarySelectionFileName
        {
            get { return System.IO.Directory.GetCurrentDirectory() + "\\DictionarySelectionInfo.xml"; }
        }

        void DictionarySelections_Load()
        {
            XmlDocument xDoc = new XmlDocument();
            if (System.IO.File.Exists(StrXmlDictionarySelectionFileName))
            {
                lstPnlSelector.Clear();
                xDoc.Load(StrXmlDictionarySelectionFileName);
                XmlNode xRoot = xDoc.FirstChild;
                XmlNode xPopUpDelay = xRoot.FirstChild;

                int intPopUpDelay = Convert.ToInt32(xPopUpDelay.InnerText);
                if (intPopUpDelay >= 0 && intPopUpDelay < lbxPopUpDelayTime.Items.Count)
                {
                    ePopUpDelayTime = (enuPopUpDelayTime)intPopUpDelay;
                    lbxPopUpDelayTime.SelectedIndex = intPopUpDelay;
                }
                cDictionaryUseClipBoard = null;

                XmlNode xPanelsList = xPopUpDelay.NextSibling;
                Point ptLocation = new Point(0, 0);

                for (int intPanelCounter = 0; intPanelCounter < xPanelsList.ChildNodes.Count; intPanelCounter++)
                {
                    Panel pnlContainer = new Panel();
                    pnlContainer.BackColor = Color.Purple;

                    XmlNode xPanel = xPanelsList.ChildNodes[intPanelCounter];
                    panelSelector pnlSel = panelSelector.Get_FromXml(xPanel);
                    pnlContainer.Size = pnlSel.Size;
                    pnlSel.cMBP.Refresh();
                    lstPnlSelector.Add(pnlSel);

                    pnlContainer.Controls.Add(pnlSel);

                    pnlSel.Dock = DockStyle.Fill;

                    panelSP.classSweepAndPrune_Element cEle = pnlSP.Add(ref pnlContainer);
                    cEle.recArea = new Rectangle(ptLocation, pnlContainer.Size);
                    ptLocation.X += pnlContainer.Width;

                    if (pnlSel.cBtnUseClipBoard.Toggled)
                    {
                        tmrClipBoard.Enabled = true;
                        
                    }
                }
            }
            pnlSP.Building = false;
            placeObjects();
        }

        public void DictionarySelections_Save()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement xRoot = xDoc.CreateElement("Root");
            try
            {
                XmlNode xPopUpDelay = xDoc.CreateElement("PopUpDelay");
                xPopUpDelay.InnerText = ((int)ePopUpDelayTime).ToString();
                xRoot.AppendChild(xPopUpDelay);

                XmlNode xPanels = xDoc.CreateElement("Panels");
                {
                    for (int intPanelCounter = 0; intPanelCounter < lstPnlSelector.Count; intPanelCounter++)
                        xPanels.AppendChild(lstPnlSelector[intPanelCounter].Xml_Get(ref xDoc));
                }
                xRoot.AppendChild(xPanels);

                xDoc.AppendChild(xRoot);
                xDoc.Save(StrXmlDictionarySelectionFileName);
            }
            catch (Exception)
            {

            }
        }
        #endregion

    }

    public class panelSelector : Panel
    {
        public classMultiButtonPic cMBP = null;
        public List<classButtonArray> lstToggles = new List<classButtonArray>();
        public enum enuTypeButton { Search, Delete, UseClipBoard, NotClickable, PopUpReference, Heading, _num };

        public classMultiButtonPic.classButton cBtnUseClipBoard = null;
        public classMultiButtonPic.classButton cBtnPopupReference = null;
        public classMultiButtonPic.classButton cBtnCursor = null;
        public classMultiButtonPic.classButton cBtnDelete = null;
        public classMultiButtonPic.classButton cBtnFastKey = null;
        public Panel pnlContainer = null;
        public panelSelector()
        {
            BackColor = formDictionarySelection.instance.BackColor;
            ForeColor = Color.Black;

            Font fnt = new Font("sans serif", 12, FontStyle.Regular);

            Color clrButtonBck = Color.White;
            int intHTab  = 3;
            // build MultiButtonPic 
            {
                cMBP = new classMultiButtonPic();
                cMBP.Formation = classMultiButtonPic.enuButtonFormation.manual;
                cMBP.BackgroundImage= Properties.Resources.DictionarySelection;
                cMBP.BackgroundImageLayout = ImageLayout.Stretch;
                cMBP.Dock = DockStyle.Fill;

                // FastKey button 
                cBtnFastKey = new classMultiButtonPic.classButton(ref cMBP);
                cBtnFastKey.AutoSize = true;
                cBtnFastKey.Text = "Ctrl-Shift-";
                cBtnFastKey.Font = fnt;
                cBtnFastKey.Backcolor_Highlight
                    = cBtnFastKey.Backcolor_Idle
                    = clrButtonBck;
                cBtnFastKey.Forecolor_Highlight
                    = cBtnFastKey.Forecolor_Idle
                    = ForeColor;
                cBtnFastKey.obj = (object)enuTypeButton.NotClickable;
                cBtnFastKey.Location = new Point(10, 10);
                cBtnFastKey.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.Blend;
                cMBP.Button_Add(ref cBtnFastKey);

                // fastkey-textbox
                txtFastKey.Text = "";
                txtFastKey.Location = new Point(cBtnFastKey.Area.Right, cBtnFastKey.Area.Top);
                txtFastKey.Width = 20;
                txtFastKey.KeyDown += TxtFastKey_KeyDown;
                Controls.Add(txtFastKey);

                // Cursor button 
                cBtnCursor = new classMultiButtonPic.classButton(ref cMBP);
                cBtnCursor.AutoSize = true;
                cBtnCursor.Text = "";
                cBtnCursor.Font = new Font(fnt.FontFamily, fnt.Size+2, FontStyle.Bold); ;
                cBtnCursor.Backcolor_Highlight
                    = cBtnCursor.Backcolor_Idle
                    = clrButtonBck;
                cBtnCursor.Forecolor_Highlight
                    = cBtnCursor.Forecolor_Idle
                    = ForeColor;
                cBtnCursor.obj = (object)enuTypeButton.NotClickable;
                cBtnCursor.Location = new Point(txtFastKey.Right+20, txtFastKey.Top);
                cBtnCursor.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.Blend;
                cMBP.Button_Add(ref cBtnCursor);

                int intX = cBtnCursor.Area.Left;
                int intY = txtFastKey.Bottom;

                int intWidth = 0, intHeight = 0;
                Size szHeading = new Size(0, cBtnFastKey.Area.Height);
                for (int intDictionaryCounter = 0; intDictionaryCounter < classDictionary.lstDictionaries.Count; intDictionaryCounter++)
                {
                    classDictionary cDictionary = classDictionary.lstDictionaries[intDictionaryCounter];
                    Size szMeasure = TextRenderer.MeasureText(cDictionary.Heading.Trim(), fnt);
                    if (szMeasure.Width > szHeading.Width)
                        szHeading.Width = szMeasure.Width;
                }
                szHeading.Width += 5;
                szHeading.Height += 8;

                for (int intDictionaryCounter = 0; intDictionaryCounter < classDictionary.lstDictionaries.Count; intDictionaryCounter++)
                {
                    classDictionary cDictionary = classDictionary.lstDictionaries[intDictionaryCounter];
                    List<classMultiButtonPic.classButton> lstButtons = new List<classMultiButtonPic.classButton>();

                    classButtonArray cButtonArray = new classButtonArray(ref cDictionary, ref lstButtons);
                    lstToggles.Add(cButtonArray);

                    intX = 10;
                    for (int intSearchTypeCounter = 0; intSearchTypeCounter <= (int)enuSearchType._num; intSearchTypeCounter++)
                    {
                        classMultiButtonPic.classButton cButton = new classMultiButtonPic.classButton(ref cMBP);

                        enuSearchType eSearchType = (enuSearchType)intSearchTypeCounter;

                        switch (eSearchType)
                        {
                            case enuSearchType._num:
                                {
                                    cMBP.Button_Add(ref cButton);
                                    cButton.CanBeToggled = false;
                                    cButton.AutoSize = false;
                                    cButton.Text = cDictionary.Heading;
                                    cButton.Font = fnt;
                                    cButton.Size = szHeading;
                                    cButton.Backcolor_Highlight
                                        = cButton.Forecolor_Idle
                                        = ForeColor;
                                    cButton.Forecolor_Highlight
                                        = cButton.Backcolor_Idle
                                        = clrButtonBck;
                                    cButton.obj = (object)enuTypeButton.Heading;
                                    cButton.Tag = (object)cButtonArray;
                                    cButton.BackgroundStyle =  classMultiButtonPic.classButton.enuBackgroundStyle.Blend;
                                    lstButtons.Add(cButton);
                                }
                                break;

                            default:
                                {
                                    cButton.AutoSize = true;
                                    cButton.CanBeToggled = true;
                                    cButton.Toggled
                                        = cButton.Highlight
                                        = (intSearchTypeCounter == (int)enuSearchType.Not_Searched);
                                    cButton.Text = "¤";
                                    cButton.Tag = (object)cButtonArray;
                                    cButton.Backcolor_Highlight
                                        = cButton.Forecolor_Idle
                                        = ForeColor;
                                    cButton.Forecolor_Highlight
                                        = cButton.Backcolor_Idle
                                        = clrButtonBck;
                                    cButton.obj = (object)enuTypeButton.Search;
                                    cButton.Font = new Font(fnt.FontFamily, cButton.Font.Size, FontStyle.Bold);
                                    cButton.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.BlendIdle;
                                    lstButtons.Add(cButton);

                                    switch(eSearchType)
                                    {
                                        case enuSearchType.Heading:
                                            if (cDictionary.SearchTreeBuild_Heading)
                                                cMBP.Button_Add(ref cButton);
                                            break;

                                        case enuSearchType.Content:
                                            if (cDictionary.SearchTreeBuild_Content)
                                                cMBP.Button_Add(ref cButton);
                                            break;

                                        case enuSearchType.Both_Heading_And_Content:
                                            if (cDictionary.SearchTreeBuild_Content && cDictionary.SearchTreeBuild_Heading)
                                                cMBP.Button_Add(ref cButton);
                                            break;

                                        default:
                                            cMBP.Button_Add(ref cButton);
                                            break;
                                    }
                                }
                                break;
                        }
                        cButton.Location = new Point(intX, intY);
                        intX += cButton.Area.Width + 3;

                        if (cButton.Area.Right > intWidth)
                            intWidth = cButton.Area.Right;
                        if (cButton.Area.Bottom > intHeight)
                            intHeight = cButton.Area.Bottom;

                    }
                    intY += lstButtons[0].Area.Height + intHTab;

                }

                Width = intWidth;
                
                // UseClipBoard button 
                cBtnUseClipBoard = new classMultiButtonPic.classButton(ref cMBP);
                cBtnUseClipBoard.AutoSize = true;
                cBtnUseClipBoard.Text = "Use Clip Board";
                cBtnUseClipBoard.Font = fnt;
                cBtnUseClipBoard.CanBeToggled = true;
                cBtnUseClipBoard.Toggled = false;
                cBtnUseClipBoard.Backcolor_Highlight
                    = cBtnUseClipBoard.Forecolor_Idle
                    = ForeColor;
                cBtnUseClipBoard.Forecolor_Highlight
                    = cBtnUseClipBoard.Backcolor_Idle
                    = clrButtonBck;
                cBtnUseClipBoard.obj = (object)enuTypeButton.UseClipBoard;
                cBtnUseClipBoard.Location = new Point(0, intHeight+5);
                cBtnUseClipBoard.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.BlendIdle;
                cMBP.Button_Add(ref cBtnUseClipBoard);
                
                
                // PopUp Reference
                cBtnPopupReference = new classMultiButtonPic.classButton(ref cMBP);
                cBtnPopupReference.AutoSize = true;
                cBtnPopupReference.Text = "Pop Up Reference";
                cBtnPopupReference.Font = fnt;
                cBtnPopupReference.CanBeToggled = true;
                cBtnPopupReference.Toggled = false;
                cBtnPopupReference.Backcolor_Highlight
                    = cBtnPopupReference.Forecolor_Idle
                    = ForeColor;
                cBtnPopupReference.Forecolor_Highlight
                    = cBtnPopupReference.Backcolor_Idle
                    = clrButtonBck;
                cBtnPopupReference.obj = (object)enuTypeButton.PopUpReference;
                cBtnPopupReference.Location = new Point(cBtnUseClipBoard.Area.Right, cBtnUseClipBoard.Area.Top);
                cBtnPopupReference.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.BlendIdle;
                cMBP.Button_Add(ref cBtnPopupReference);

                // delete button 
                cBtnDelete = new classMultiButtonPic.classButton(ref cMBP);
                cBtnDelete.AutoSize = true;
                cBtnDelete.Text = "Delete";
                cBtnDelete.Font = fnt;
                cBtnDelete.Backcolor_Highlight
                    = cBtnDelete.Forecolor_Idle
                    = ForeColor;
                cBtnDelete.Forecolor_Highlight
                    = cBtnDelete.Backcolor_Idle
                    = clrButtonBck;
                cBtnDelete.obj = (object)enuTypeButton.Delete;
                cBtnDelete.Location = new Point(cBtnPopupReference.Area.Right, cBtnPopupReference.Area.Top);
                cBtnDelete.BackgroundStyle = classMultiButtonPic.classButton.enuBackgroundStyle.Blend;
                cMBP.Button_Add(ref cBtnDelete);

                Height += cBtnDelete.Area.Bottom;

                Controls.Add(cMBP);

                // events 
                cMBP.eventHandler_MouseMove = CMBP_MouseMove;
                cMBP.eventHandler_MouseClick = cMBP_HTMLTagList_Click;
            }

            BorderStyle = BorderStyle.Fixed3D;
        }

        private void CMBP_MouseMove(object sender, MouseEventArgs e)
        {
            if (cMBP.ButtonUnderMouse != null)
            {
                classButtonArray cBtnArray = (classButtonArray)cMBP.ButtonUnderMouse.Tag;

                if (cBtnArray != null)
                {
                    int intIndex = cBtnArray.lstButtons.IndexOf(cMBP.ButtonUnderMouse);
                    enuSearchType eSType = (enuSearchType)intIndex;
                    switch (eSType)
                    {
                        case enuSearchType.Not_Searched:
                        case enuSearchType.Heading:
                        case enuSearchType.Content:
                        case enuSearchType.Both_Heading_And_Content:
                            {
                                if (intIndex >= 0 && intIndex < (int)enuSearchType._num)
                                {
                                    cBtnCursor.Text = eSType.ToString().Replace('_', ' ');
                                    cMBP.Refresh();
                                
                                    return;
                                }
                            }
                            break;

                        default:
                            {
                                cBtnCursor.Text = cBtnArray.eSearchType.ToString().Replace('_', ' ');
                                cMBP.Refresh();
                                
                                return;
                            }
                    }

                }
            }
            cBtnCursor.Text = "";
            cMBP.Refresh();
        }

        public TextBox txtFastKey = new TextBox();
        char _chrFastKey = 'A';

        public CheckBox chkUseClipBoard = new CheckBox();
        public char FastKey
        {
            get { return _chrFastKey; }
        }
        char _FastKey
        {
            get { return FastKey; }
            set
            {
                _chrFastKey = value;
                cBtnFastKey.Text = "Ctrl-Shift-" + _chrFastKey.ToString();
            }
        }

        static string strAllowableControlKeys = "0123456789ABCDEGHIJKMNOPQSTUVWXY";
        private void TxtFastKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (!strAllowableControlKeys.Contains((Char)e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }
            _FastKey = char.ToUpper((char)e.KeyValue);
            txtFastKey.Text = _FastKey.ToString();
            cMBP.Refresh();
            e.SuppressKeyPress = true;
        }

        public string CursorText
        {
            get { return cBtnCursor.Text; }
            set
            {
                cBtnCursor.Text = value;
            }
        }

        private void cMBP_HTMLTagList_Click(object sender, EventArgs e)
        {
            classMultiButtonPic cMBPSender = (classMultiButtonPic)sender;
            classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMBPSender.ButtonUnderMouse;
            if (cBtn != null)
            {
                if (cBtn.Tag != null)
                {
                    classButtonArray cBtnArray = (classButtonArray)cBtn.Tag;
                    classDictionary cDictionary = cBtnArray.cDictionary;
                    int intIndex = cBtnArray.lstButtons.IndexOf(cBtn);
                    if (intIndex < (int)enuSearchType._num)
                    {
                        cBtnArray.eSearchType = (intIndex >= 0 && intIndex < (int)enuSearchType._num)
                                                          ? (enuSearchType)intIndex
                                                          : enuSearchType._num;
                        for (int intBtnCounter = 0; intBtnCounter < (int)enuSearchType._num; intBtnCounter++)
                        {
                            cBtn = cBtnArray.lstButtons[intBtnCounter];
                            cBtn.Toggled
                                  = cBtn.Highlight
                                  = intBtnCounter == intIndex;
                        }
                        cMBPSender.Refresh();

                        PopUpReference_Handle(ref cBtnPopupReference);
                    }
                    else
                    {
                        panelSelector.enuTypeButton eButtonType = (panelSelector.enuTypeButton)cBtn.obj;
                        switch (eButtonType)
                        {
                            case panelSelector.enuTypeButton.Heading:
                                {
                                    groupboxDictionaryHeader_Info.cDictionary = cBtnArray.cDictionary;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    panelSelector.enuTypeButton eButtonType = (panelSelector.enuTypeButton)cBtn.obj;
                    switch (eButtonType)
                    {
                        case panelSelector.enuTypeButton.UseClipBoard:
                            {
                                cBtn.Toggle();
                                if (cBtn.Toggled)
                                {
                                    for (int intPnlCounter = 0; intPnlCounter < formDictionarySelection.instance.lstPnlSelector.Count; intPnlCounter++)
                                    {
                                        panelSelector pnlSel = formDictionarySelection.instance.lstPnlSelector[intPnlCounter];
                                        if (pnlSel != this && pnlSel.cBtnUseClipBoard.Toggled)
                                        {
                                            pnlSel.cBtnUseClipBoard.Toggled = false;
                                            pnlSel.cBtnUseClipBoard.Highlight = false;
                                            pnlSel.cMBP.Refresh();
                                        }
                                    }
                                    formDictionarySelection.instance.pnlUseClipBoard = this;
                                }
                                else
                                {
                                    formDictionarySelection.instance.pnlUseClipBoard = null;
                                }
                                formDictionarySelection.instance.TmrClipboard_Set();
                            }
                            break;

                        case panelSelector.enuTypeButton.Search:
                            {
                            }
                            break;

                        case enuTypeButton.PopUpReference:
                            {
                                cBtn.Toggle();
                                PopUpReference_Handle(ref cBtn);

                            }
                            break;

                        case panelSelector.enuTypeButton.Delete:
                            {
                                panelSelector pnlRefThis = this;
                                formDictionarySelection.instance.DeletePanel(ref pnlRefThis);
                                pnlRefThis.Hide();
                                pnlRefThis.Dispose();
                                formDictionarySelection.instance.placeObjects();
                            }
                            break;


                        case enuTypeButton.Heading:
                            {
                            }
                            break;

                        default:
                            {
                            }
                            break;
                    }
                }
            }

        }

        void PopUpReference_Handle(ref classMultiButtonPic.classButton cBtn)
        {
            if (cBtn.Toggled)
            {
                for (int intPnlCounter = 0; intPnlCounter < formDictionarySelection.instance.lstPnlSelector.Count; intPnlCounter++)
                {
                    panelSelector pnlSel = formDictionarySelection.instance.lstPnlSelector[intPnlCounter];
                    if (pnlSel != this && pnlSel.cBtnPopupReference.Toggled)
                    {
                        pnlSel.cBtnPopupReference.Toggled = false;
                        pnlSel.cBtnPopupReference.Highlight = false;
                        pnlSel.cMBP.Refresh();
                    }
                }
                formDictionarySelection.instance.pnlPopUpReference = this;
            }
            //else
            //{
            //    formDictionarySelection.instance.pnlPopUpReference = null;
            //}
        }


        #region XML
        const string xmlMarker_Panel = "Panel";
        const string xmlMarker_CtrlChar = "CTRL_Char";
        const string xmlMarker_UseAsClipboard = "UseAsClipboard";
        const string xmlMarker_PopupReference = "PopupReference";
        const string xmlMarker_DictionaryList= "DictionaryList";
        const string xmlMarker_Dictionary= "Dictionary";
        const string xmlMarker_DictionaryHeading = "DictionaryHeading";
        const string xmlMarker_TypeSearch = "TypeSearch";

        public XmlNode Xml_Get(ref XmlDocument xDoc)
        {
            XmlElement xRetVal = xDoc.CreateElement(xmlMarker_Panel);

            XmlNode xAtt_CtrlChar = xDoc.CreateNode(XmlNodeType.Attribute, xmlMarker_CtrlChar, "");
            XmlNode xAtt_UseAsClipboard = xDoc.CreateNode(XmlNodeType.Attribute, xmlMarker_UseAsClipboard, "");
            XmlNode xAtt_PopupReference = xDoc.CreateNode(XmlNodeType.Attribute, xmlMarker_PopupReference, "");
            
            //          fast-key
            {
                xAtt_CtrlChar.Value = _chrFastKey.ToString();
                xRetVal.Attributes.SetNamedItem(xAtt_CtrlChar);
            }

            //          clipboard
            {
                xAtt_UseAsClipboard.Value = cBtnUseClipBoard.Toggled.ToString();
                xRetVal.Attributes.SetNamedItem(xAtt_UseAsClipboard);
            }

            //              
            {
                xAtt_PopupReference.Value = cBtnPopupReference.Toggled.ToString();
                xRetVal.Attributes.SetNamedItem(xAtt_PopupReference);
            }

            XmlNode xDictionaries = xDoc.CreateElement(xmlMarker_DictionaryList);
            {
                for (int intCounter = 0; intCounter < lstToggles.Count; intCounter++)
                {
                    classButtonArray cBtnArray = lstToggles[intCounter];
                    XmlNode xDictionary = xDoc.CreateElement(xmlMarker_Dictionary);

                    //              heading
                    { 
                        XmlNode xDictionaryHeading = xDoc.CreateElement(xmlMarker_DictionaryHeading);
                        xDictionaryHeading.InnerText = cBtnArray.cDictionary.Heading;
                        xDictionary.AppendChild(xDictionaryHeading);
                    }

                    //              TypeSearch
                    { 
                        XmlNode xDictionaryTypeSearch = xDoc.CreateElement(xmlMarker_TypeSearch);
                        xDictionaryTypeSearch.InnerText = cBtnArray.eSearchType.ToString();
                        xDictionary.AppendChild(xDictionaryTypeSearch);
                    }

                    xDictionaries.AppendChild(xDictionary);
                }
            }
            xRetVal.AppendChild(xDictionaries);

            return xRetVal;
        }

        public static panelSelector Get_FromXml(XmlNode xPnlSelector)
        {
            panelSelector pnlRetVal = new panelSelector();
    
            XmlNode xDictionaries = xPnlSelector.FirstChild;
            if (xDictionaries != null)
            {
                for (int intDictionaryCounter = 0; intDictionaryCounter < xDictionaries.ChildNodes.Count; intDictionaryCounter++)
                {
                    XmlNode xDictionary = xDictionaries.ChildNodes[intDictionaryCounter];
                    XmlNode xHeading = xDictionary.FirstChild;
                    XmlNode xTypeSearch = xHeading.NextSibling;

                    for (int intSearcCounter = 0; intSearcCounter < pnlRetVal.lstToggles.Count; intSearcCounter++)
                    {
                        classDictionary cDictionary = pnlRetVal.lstToggles[intSearcCounter].cDictionary;
                        if (string.Compare(xHeading.InnerText, cDictionary.Heading) == 0)
                        {
                            enuSearchType eSearchType = formDictionarySelection.GetSearchTypeFromString(xTypeSearch.InnerText);
                            if (eSearchType != enuSearchType._num)
                            {
                                classButtonArray cBtnArray = pnlRetVal.lstToggles[intSearcCounter];
                                for (int intBtnCounter = 0; intBtnCounter < (int)enuSearchType._num; intBtnCounter++)
                                {
                                    classMultiButtonPic.classButton cBtn = cBtnArray.lstButtons[intBtnCounter];
                                    cBtn.Toggled
                                          = cBtn.Highlight
                                          = intBtnCounter == (int)eSearchType;
                                    if (cBtn.Toggled)
                                        cBtnArray.eSearchType = (enuSearchType)intBtnCounter;
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                pnlRetVal._chrFastKey = xPnlSelector.Attributes[0].InnerText[0];
                pnlRetVal.txtFastKey.Text = xPnlSelector.Attributes[0].InnerText[0].ToString();
            }
            catch (Exception ) { }

            try
            {
                pnlRetVal.cBtnUseClipBoard.Toggled =string.Compare( xPnlSelector.Attributes[1].InnerText, true.ToString()) ==0;
                if (pnlRetVal.cBtnUseClipBoard.Toggled)
                    formDictionarySelection.instance.pnlUseClipBoard = pnlRetVal;

                pnlRetVal.cBtnUseClipBoard.Highlight = pnlRetVal.cBtnUseClipBoard.Toggled;
            }
            catch (Exception ) { }

            try
            {
                pnlRetVal.cBtnPopupReference.Toggled =string.Compare( xPnlSelector.Attributes[2].InnerText, true.ToString()) ==0;
                pnlRetVal.cBtnPopupReference.Highlight = pnlRetVal.cBtnPopupReference.Toggled;
                if (pnlRetVal.cBtnPopupReference.Toggled)
                    formDictionarySelection.instance.pnlPopUpReference = pnlRetVal;
            }
            catch (Exception ) { }

            pnlRetVal.cMBP.Refresh();
            return pnlRetVal;
        }

        #endregion 



        public class classButtonArray
        {
            public classDictionary cDictionary = null;
            public List<classMultiButtonPic.classButton> lstButtons = new List<classMultiButtonPic.classButton>();
            public enuSearchType eSearchType = enuSearchType.Not_Searched;
            public classButtonArray(ref classDictionary dictionary, ref List<classMultiButtonPic.classButton> _lstButtons)
            {
                cDictionary = dictionary;
                lstButtons = _lstButtons;
            }
        }
    }
}
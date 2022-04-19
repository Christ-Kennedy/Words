using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Drawing;
using SPObjects;

namespace Words
{

    public enum enuNotesFieldTypes { Title, Year, Month, Day, Hour, Minute, ChapterNumber, POV, Path, CompletionLevel };
    public enum enuNotesButtons { ChapterNumber, Date, Time, Title, POV };
    public enum enuHighlightStyle { none, underMouse, Edit_Main, Edit_Alt, EditSelect, POV_Highlight };


    public class panelNotesDataTree : SPObjects.SPContainer
    {
        public static panelNotesDataTree instance = null;
        System.Windows.Forms.Timer tmrMouseLeave = new System.Windows.Forms.Timer();
        
        static public classProject cProject { get { return formWords.cProject; } }

        public static classNotesInfo cRoot { get { return classNotesInfo.lst.Count > 0 ? classNotesInfo.lst[0] : null; } }
        public static groupboxNotes grbNotes
        {
            get
            {
                if (formWords.instance != null && formWords.instance.grbNotes != null)
                    return formWords.instance.grbNotes;
                return null;
            }
        }

        public static groupboxNotePilot grbNotePilot { get { return groupboxNotePilot.instance; } }

        static classNotesInfo _cParent = null;
        public static classNotesInfo cParent
        {
            get { return _cParent; }
            set
            {
                if (_cParent != value)
                {
                    if (_cParent != null)
                    {
                        // do what needs done to outgoing Note
                    }

                    _cParent = value;

                    if (_cParent != null)
                    {
                        // do what needs done to incoming Note
                        grbNotes.pnlNotes.RecVisible_Changed = true;
                        instance.setTabs_byWidths();
                        instance.Buttons_Rebuild();
                        grbNotes.Text = cParent.Heading;
                        grbNotePilot.cParent_Changed();
                        instance.cEdit_Alt = cParent.lstChildNotes.Count > 0
                                                                         ? cParent.lstChildNotes[0]
                                                                         : null;

                        if (_cParent == classNotesInfo.cRoot)
                        {
                            classNotesInfo cNote_Backup = classBackUp.NoteInfo;
                            if (!classNotesInfo.cRoot.lstChildNotes.Contains(cNote_Backup))
                                _cParent.ChildNodes_Add(ref cNote_Backup);
                        }

                        instance.Reorder();
                    }
                }
            }
        }
        
        public classNotesInfo cEdit_Alt
        {
            get { return cProject.cEdit_Alt; }
            set { cProject.cEdit_Alt = value; }
        }



        SPObjects.SlideRuler slideRuler = null;

        System.Windows.Forms.ContextMenu cmnu = new System.Windows.Forms.ContextMenu();

        public panelNotesDataTree(string Name) : base(Name)
        {
            instance = this;

            BackColor = Color.Yellow;
            pic.MouseMove += Pic_MouseMove;
            pic.MouseLeave += Pic_MouseLeave;
            pic.MouseClick += Pic_MouseClick;
            
            MouseWheel = _MouseWheel;
            cEleUnderMouseChange = cEleUnderMouse_Change;

            VScrollBar.Visible = true;
            HScrollBar.Visible = true;

            VScrollBar.ValueChanged = scrollBar_ValueChanged;
            HScrollBar.ValueChanged = scrollBar_ValueChanged;


            ContextMenu = cmnu;
            cmnu.Popup += Cmnu_Popup;

            slideRuler_Init();
            SizeChanged += panelNotesDataTree_SizeChanged;
        }

        private void Cmnu_Popup(object sender, EventArgs e)
        {
            cmnuNotes_Build();
        }

        void cmnuNotes_Build()
        {
            cmnu.MenuItems.Clear();

            System.Windows.Forms.MenuItem mnuSetAsMain = new System.Windows.Forms.MenuItem("Edit in Main", mnuEditInMain_Click);
            cmnu.MenuItems.Add(mnuSetAsMain);

            System.Windows.Forms.MenuItem mnuNote = new System.Windows.Forms.MenuItem("Notes");
            {
                System.Windows.Forms.MenuItem mnuCreate = new System.Windows.Forms.MenuItem("Create");
                {
                    mnuCreate.MenuItems.Add(new System.Windows.Forms.MenuItem("New", grbNotePilot.mnuNote_Add_New_Click));
                    mnuCreate.MenuItems.Add(new System.Windows.Forms.MenuItem("File", grbNotePilot.mnuNote_Add_File_Click));
                }
                mnuNote.MenuItems.Add(mnuCreate);
                mnuNote.MenuItems.Add(new System.Windows.Forms.MenuItem("Delete", grbNotePilot.mnuNoteDelete_Click));
            }
            cmnu.MenuItems.Add(mnuNote);

            System.Windows.Forms.MenuItem mnuProject = new System.Windows.Forms.MenuItem("Project");
            {
                mnuProject.MenuItems.Add(new System.Windows.Forms.MenuItem("Load", frmWords.mnuProject_Load_Click));
                mnuProject.MenuItems.Add(new System.Windows.Forms.MenuItem("New", frmWords.mnuProject_New_Click));
                mnuProject.MenuItems.Add(new System.Windows.Forms.MenuItem("Save", frmWords.mnuProject_Save_Click));
                mnuProject.MenuItems.Add(new System.Windows.Forms.MenuItem("Save As", frmWords.mnuProject_SaveAs_Click));
                mnuProject.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", frmWords.mnuProject_Exit_Click));
            }
            cmnu.MenuItems.Add(mnuProject);

            System.Windows.Forms.MenuItem mnuPOVHighlight = new System.Windows.Forms.MenuItem("POV Highlight", mnuPOV_Highlight_Click);
            mnuPOVHighlight.Checked = POV_Highlight;
            cmnu.MenuItems.Add(mnuPOVHighlight);
        }


        void mnuEditInMain_Click(object sender, EventArgs e)
        {
            if (cEdit_Alt == null) return;

 

            cEdit_Alt.Project_SetMainEdit();
        }

        void mnuPOV_Highlight_Click(object sender, EventArgs e)
        {
            bolPOV_Highlight = !bolPOV_Highlight;
            cParent.POV_HighlightChildren();
        }


        bool bolPOV_Highlight = false;
        public bool POV_Highlight
        {
            get { return bolPOV_Highlight; }
            set { bolPOV_Highlight = value; }
        }

        public formWords frmWords { get { return formWords.instance; } }

        public void init()
        {
            lstElements.Clear();
        }

        public class classTabWidth
        {
            int intValue = 0;
            public int Value
            {
                get { return intValue; }
                set { intValue = value; }
            }

            bool bolValid = false;
            public bool Valid
            {
                get { return bolValid; }
                set { bolValid = value; }
            }

            public classTabWidth(int Value)
            {
                this.Value = Value;
                bolValid = true;
            }

            public static List<classTabWidth> getList(int[] intWidths)
            {
                List<int> lstWidths = intWidths.ToList<int>();
                List<classTabWidth> lstRetVal = new List<classTabWidth>();
                while (lstWidths.Count > 0)
                {
                    lstRetVal.Add(new classTabWidth(lstWidths[0]));
                    lstWidths.RemoveAt(0);
                }
                return lstRetVal;
            }
        }

        void event_TabsChanged(object sender, EventArgs e)
        {
            if (bolIgnoreTabChanges) return;
            setWidths_byTabs();
        }

        void event_MyImage_Changed(object sender, EventArgs e)
        {
            pic.Refresh();
        }

        public void setWidths_byTabs()
        {
            if (bolIgnoreTabChanges) return;

            bolIgnoreTabChanges = true;
            bool bolBuilding = BuildingInProgress;
            if (cParent == null) return;
            Building_Start();
            {
                for (int intTabCounter = 0; intTabCounter < slideRuler.lstTabs.Count; intTabCounter++)
                {
                    SlideRuler.Tab tabCounter = slideRuler.lstTabs[intTabCounter];
                    if (cParent.TabWidths[intTabCounter] != tabCounter.Width)
                    {

                        cParent.TabWidths[intTabCounter] = tabCounter.Width;
                        for (int intNoteCounter = 0; intNoteCounter < cParent.lstChildNotes.Count; intNoteCounter++)
                        {
                            classNotesInfo cNote = cParent.lstChildNotes[intNoteCounter];
                            if (intTabCounter >= 0 && intTabCounter < cNote.lstButtons.Count)
                            {
                                NotesButton btn = cNote.lstButtons[intTabCounter];
                                btn.Width = cParent.TabWidths[intTabCounter];
                            }
                        }
                    }
                }

                Buttons_Rebuild();
                bolIgnoreTabChanges = false;
            }
            if (!bolBuilding)
                Building_Complete();

        }

        bool bolIgnoreTabChanges = true;
        public void setTabs_byWidths()
        {
            bolIgnoreTabChanges = true;
            if (cParent == null) return;
            for (int intTabCounter = 0; intTabCounter < slideRuler.lstTabs.Count && intTabCounter < cParent.TabWidths.Length; intTabCounter++)
            {
                SlideRuler.Tab tabCounter = slideRuler.lstTabs[intTabCounter];
                tabCounter.Width = cParent.TabWidths[intTabCounter];
            }
            bolIgnoreTabChanges = false;
        }

        void slideRuler_Init()
        {
            SPObjects.SPContainer cMyRef = this;
            if (slideRuler == null)
            {

                slideRuler = new SlideRuler(ref cMyRef);
                slideRuler.MyImage_Changed = event_MyImage_Changed;

                SlideRuler.Tab tabPrev = null;
                int intNumTabs = Enum.GetNames(typeof(enuNotesButtons)).Length;

                int[] arrDefault = { 20,56,44,110,200,100,110,100,100 };


                for (int intTabCounter = 0; intTabCounter < intNumTabs; intTabCounter++)
                {
                    enuNotesButtons eNoteButton = (enuNotesButtons)intTabCounter;


                    SlideRuler.Tab tabNew = new SlideRuler.Tab(ref slideRuler);
                    int intArrElement = 0;
                    if (cParent != null)
                    {
                        int intArrayIndex = intTabCounter < cParent.TabWidths.Length
                                                       ? intTabCounter
                                                       : 0;
                        intArrElement = cParent.TabWidths[intArrayIndex];
                    }
                    else
                    {
                        int intArrIndex = intTabCounter < arrDefault.Length
                                                       ? intTabCounter
                                                       : arrDefault.Length - 1;
                        intArrElement = arrDefault[intArrIndex];

                    }
                    tabNew.Width = intArrElement;
                    tabNew.Name = eNoteButton.ToString();
                    tabPrev = tabNew;
                }

                slideRuler.TabsChanged = event_TabsChanged;
                slideRuler.MouseClick = slideRuler_MouseClick;
            }
        }


        void _MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        void slideRuler_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            SlideRuler slrSennder = (SlideRuler)sender;
            SPObjects.SlideRuler.Tab tabClicked = slrSennder.tabUnderMouse;
            if (cParent == null || cParent.lstChildNotes.Count == 0) return;
            if (tabClicked != null)
            {
                cParent.eOrderBy = (enuNotesButtons)slrSennder.lstTabs.IndexOf(tabClicked);
                Reorder();
            }
        }

        public void Reorder()
        {
            IEnumerable<classNotesInfo> query = null;
                        
            switch (cParent.eOrderBy)
            {
                case enuNotesButtons.Date:
                case enuNotesButtons.Time:
                    query = cParent.lstChildNotes.OrderBy(note => note.keyDateTime);
                    break;

                case enuNotesButtons.ChapterNumber:
                    query = cParent.lstChildNotes.OrderBy(note => note.Chapter);
                    break;

                case enuNotesButtons.Title:
                    query = cParent.lstChildNotes.OrderBy(note => note.Title);
                    break;

                case enuNotesButtons.POV:
                    query = cParent.lstChildNotes.OrderBy(note => note.POV);
                    break;
            }

            cParent._lstChildNotes = (List<classNotesInfo>)query.ToList<classNotesInfo>();
            Buttons_Rebuild();
        }


        private void Pic_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    {
                        if (cEleUnderMouse != null)
                        {
                            switch (cEleUnderMouse.eSP_ObjectType)
                            {

                                case enuSweepPrune_ObjectType.UserDefined:
                                    {

                                        NotesButton cBtnClicked = cBtnUnderMouse;
                                        if (cBtnClicked != null)
                                        {
                                            classNotesInfo cNote = cBtnClicked.cItem;
                                            bool bolBuilding = BuildingInProgress;
                                            Building_Start();
                                            {
                                                if (cNote != null)
                                                    cEdit_Alt = cNote;
                                            }
                                            if (!bolBuilding)
                                                Building_Complete();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        public static string FilenameGet(string strPath)
        {
            string strTempHeading = "";
            int intLastBackSlash = strPath.LastIndexOf("\\");
            if (intLastBackSlash > 0)
                strTempHeading = strPath.Substring(intLastBackSlash + 1);
            else
                return strPath;

            int intLastDecimal = strTempHeading.LastIndexOf(".");
            if (intLastDecimal >= 0)
                strTempHeading = strTempHeading.Substring(0, intLastDecimal);

            return strTempHeading;
        }



        NotesButton _cBtnUnderMouse = null;
        public NotesButton cBtnUnderMouse
        {
            get { return _cBtnUnderMouse; }
            set
            {
                bool bolBuilding = BuildingInProgress;
                Building_Start();
                {
                    if (_cBtnUnderMouse != null)
                    {
                        if (_cBtnUnderMouse.cItem == cProject.cEdit_Main)
                            _cBtnUnderMouse.cItem.eHighLight = enuHighlightStyle.Edit_Main;
                        else if (_cBtnUnderMouse.cItem == cEdit_Alt)
                            _cBtnUnderMouse.cItem.eHighLight = enuHighlightStyle.Edit_Alt;
                        else
                            _cBtnUnderMouse.cItem.eHighLight = enuHighlightStyle.none;
                    }
                    _cBtnUnderMouse = value;

                    if (_cBtnUnderMouse != null)
                        _cBtnUnderMouse.cItem.eHighLight = enuHighlightStyle.underMouse;
                }
                if (!bolBuilding)
                    Building_Complete();
            }
        }

        void cEleUnderMouse_Change(object sender, EventArgs e)
        {
            classSweepAndPrune_Element cSPEle_Sender = (classSweepAndPrune_Element)sender;
            if (cSPEle_Sender == null)
            {
                cBtnUnderMouse = null;
                return;
            }

            switch (cSPEle_Sender.eSP_ObjectType)
            {
                case enuSweepPrune_ObjectType.Button:
                    {
                    }
                    break;

                case enuSweepPrune_ObjectType.SlideRuler:
                    {

                    }
                    break;

                case enuSweepPrune_ObjectType.UserDefined:
                    {
                        if (cEleUnderMouse != null)
                            cBtnUnderMouse = (NotesButton)cEleUnderMouse.obj;
                        else
                            cBtnUnderMouse = null;

                    }
                    break;

                default:
                    System.Windows.Forms.MessageBox.Show("This should not happen");
                    break;
            }
        }

        private void Pic_MouseLeave(object sender, EventArgs e)
        {
            tmrMouseLeave_Reset();
        }

        private void Pic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
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
            System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)this;
            Point ptPanelRelToScreen = Ck_Objects.classControlLocation.Location(ref ctrl);
            if (ptMouse.X > ptPanelRelToScreen.X && ptMouse.X < ptPanelRelToScreen.X + Width)
            {
                if (ptMouse.Y > ptPanelRelToScreen.Y && ptMouse.Y < ptPanelRelToScreen.Y + Height)
                {
                    return;
                }
            }
            cEleUnderMouse = null;
        }

        private void panelNotesDataTree_SizeChanged(object sender, EventArgs e)
        {
            bool bolBuilding = BuildingInProgress;

            Building_Start();
            {
                if (slideRuler.Width < Width)
                {
                    slideRuler.Width = Width;
                    slideRuler._Draw(true);
                }
                recVisible = new Rectangle(recVisible.Left, recVisible.Top, Width, Height);

                if (cParent != null && cParent.lstChildNotes.Count > 1 && cParent.lstChildNotes[cParent.lstChildNotes.Count - 1].lstButtons.Count >0)
                    recSPArea = new Rectangle(0, 0, Width, cParent.lstChildNotes[cParent.lstChildNotes.Count - 1].lstButtons[0].recSPArea.Bottom);
                else
                    recSPArea = new Rectangle(0, 0, Width, Height);

                recVisible = new Rectangle(recVisible.Left, recVisible.Top, Width, Height);
            }
            if (!bolBuilding)
                Building_Complete();

            NeedsToBeRedrawn = true;
            Refresh();
        }


        static Font fntNotes = new Font("Arial", 10);

        void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (slideRuler != null)
            {
                Building_Start();
                {
                    SPObjects.classScrollBar vsbSender = (SPObjects.classScrollBar)sender;

                    recVisible = new Rectangle(recVisible.Left, vsbSender.Value, recVisible.Width, recVisible.Height);
                    if (recVisible.Top < 0)
                        recVisible = new Rectangle(recVisible.Left, 0, recVisible.Width, recVisible.Height);
                    slideRuler.cEle.rec = new Rectangle(recVisible.Left, recVisible.Top, Width, slideRuler.recDraw.Height);
                    slideRuler.BringToFront();
                }
                Building_Complete();
            }
        }

        public void Buttons_Rebuild()
        {
            Color clrButtonBck = Color.White;
            if (cParent == null) return;
            // build, draw & position all needed buttons
            Size szFont = System.Windows.Forms.TextRenderer.MeasureText("BCD", fntNotes);
            SPContainer cMyRef = this;
            bool bolBuilding = BuildingInProgress;
            int intButtons_Height = 15;
            formWords_Flash.instance.TopMost = false;
            int intWidth = 0;
            Building_Start();
            {
                lstElements.Clear();
                Add(ref slideRuler);
                slideRuler.cEle.rec = new Rectangle(recVisible.Left, recVisible.Top, Width, intButtons_Height);
                intWidth = slideRuler.cEle.rec.Width;
                Point ptTL = new Point(0, slideRuler.recSPArea.Height);
                classSweepAndPrune_Element cEleNew = null;
                for (int intItemCounter = 0; intItemCounter < cParent.lstChildNotes.Count; intItemCounter++)
                {
                    classNotesInfo cItem = cParent.lstChildNotes[intItemCounter];
                    int intNumButtons = Enum.GetNames(typeof(enuNotesButtons)).Length;
                    ptTL.X = 0;
                    NotesButton btnNew = null;
                    for (int intButtonCounter = 0; intButtonCounter < intNumButtons; intButtonCounter++)
                    {
                        if (intButtonCounter >= cItem.lstButtons.Count)
                        {
                            btnNew = new NotesButton(ref cMyRef);
                            cItem.lstButtons.Add(btnNew);
                            btnNew.cItem = cItem;
                            btnNew.eButtonType = (enuNotesButtons)intButtonCounter;
                            btnNew.Font = fntNotes;
                            btnNew.AutoSize = false;

                            btnNew.MouseClick = btn_Click;
                        }
                        else
                        {
                            btnNew = cItem.lstButtons[intButtonCounter];
                        }

                        btnNew.eHighLight = cItem.eHighLight;
                        cEleNew = btnNew.cEle;
                        lstElements.Add(cEleNew);

                        if (cItem != null && cItem.Heading != null)
                        {
                            switch (btnNew.eButtonType)
                            {
                                case enuNotesButtons.Title:
                                    btnNew.Text = cItem.Title + (cItem.lstChildNotes.Count == 0 
                                                                                           ? ""
                                                                                           : "(" + cItem.lstChildNotes.Count.ToString() + ")");
                                    break;

                                case enuNotesButtons.Date:
                                    btnNew.Text = cItem.Year.ToString() + "/" + cItem.Month.ToString("00") + "/" + cItem.Day.ToString("00");
                                    break;

                                case enuNotesButtons.Time:
                                    btnNew.Text = cItem.Hour.ToString("00") + ":" + cItem.Minute.ToString("00");
                                    break;

                                case enuNotesButtons.POV:
                                    btnNew.Text = cItem.POV;
                                    break;

                                case enuNotesButtons.ChapterNumber:
                                    btnNew.Text = cItem.Chapter.ToString();
                                    btnNew.BackColor = cItem.clrBack_Heading;
                                    btnNew.ForeColor
                                        = btnNew.ForeColor_Dull 
                                        = cItem.clrFore_Heading;
                                    break;
                            }
                        }

                        btnNew.cEle.Name = cItem.Title + ":" + btnNew.Text;
                        // position button
                        {
                            btnNew.Size = new Size(cParent.TabWidths[intButtonCounter], szFont.Height + 1);
                            btnNew.Location = ptTL;
                            btnNew.cEle.rec = new Rectangle(ptTL, btnNew.Size);
                            ptTL.X += cParent.TabWidths[(int)btnNew.eButtonType];
                        }
                    }
                    if (ptTL.X > intWidth)
                        intWidth = ptTL.X;
                    ptTL.Y += intButtons_Height;
                }

                recSPArea = new Rectangle(0, 0, intWidth, ptTL.Y + intButtons_Height + 1);
                recVisible = new Rectangle(recVisible.Left, recVisible.Top, Width, Height);

                slideRuler.BringToFront();
            }
            Building_Complete();
        }

        public void ScrollToCaret()
        {
            bool bolBuilding = BuildingInProgress;

            Building_Start();
            {
                if (cEdit_Alt != null)
                {
                    SPObjects.classSweepAndPrune_Element cEle = cEdit_Alt.lstButtons[0].cEle;
                    // /*
                    if (cEle.recSP.Y + recVisible.Height < recSPArea.Bottom)
                    {
                        recVisible = new Rectangle(cEle.recSP.X,
                                                    recSPArea.Bottom - recVisible.Height,
                                                    recVisible.Width,
                                                    recVisible.Height);
                    }
                    else
                    {
                        if (cEle.recSP.Y < recVisible.Height)
                            recVisible = new Rectangle(cEle.recSP.X, 0, recVisible.Width, recVisible.Height);
                        else
                            recVisible = new Rectangle(cEle.recSP.X, cEle.recSP.Y, recVisible.Width, recVisible.Height);
                    }
                    /*/
                    recVisible = new Rectangle(cEle.recSP.Left, cEle.recSP.Top, recVisible.Width, recVisible.Height);
                    // */
                    NeedsToBeRedrawn = true;
                }
            }
            if (!bolBuilding)
                Building_Complete();
        }


        void btn_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch(e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    {
                        NotesButton btnSender = (NotesButton)sender;
                        classNotesInfo cItem = btnSender.cItem;

                        if (btnSender.CanBeToggled) btnSender.Toggle();
                        else
                            cEdit_Alt = cItem;
                    }
                    break;
            }       
        }


        public class NotesButton : SPObjects.Button
        {
            public class classColorDisplay_Item
            {
                public Color BackColor = Color.White;
                public Color ForeColor = Color.Black;
                public classColorDisplay_Item(Color fore, Color back)
                {
                    this.ForeColor = fore;
                    this.BackColor = back;
                }
            }

            
            enuHighlightStyle _eHighlightStyle = enuHighlightStyle.none;
            public enuHighlightStyle eHighLight
            {
                get
                {
                    return _eHighlightStyle;
                }

                set
                {
                    if (_eHighlightStyle != value)
                    {
                        _eHighlightStyle = value;

                        if (eButtonType == enuNotesButtons.ChapterNumber)
                        {
                            bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                            groupboxNotes.instance.pnlNotes.Building_Start();
                            {
                                DrawBorder = false;
                                BackColor = cItem.clrBack_Heading;
                                ForeColor = cItem.clrFore_Heading;
                                cEle.NeedsToBeRedrawn = true;
                            }
                            if (!bolBuilding)
                                groupboxNotes.instance.pnlNotes.Building_Complete();
                        }
                        else
                        {
                            switch (eHighLight)
                            {
                                case enuHighlightStyle.EditSelect:
                                    {
                                        bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                                        groupboxNotes.instance.pnlNotes.Building_Start();
                                        {
                                            DrawBorder = true;
                                            BorderWidth = 2;
                                            cEle.NeedsToBeRedrawn = true;
                                        }
                                        if (!bolBuilding)
                                            groupboxNotes.instance.pnlNotes.Building_Complete();
                                    }
                                    break;

                                case enuHighlightStyle.Edit_Alt:
                                case enuHighlightStyle.POV_Highlight:
                                    {
                                        bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                                        groupboxNotes.instance.pnlNotes.Building_Start();
                                        {
                                            DrawBorder = false;
                                            BackColor = grbNotes.rtxNotes.Heading_BackColor;
                                            ForeColor = grbNotes.rtxNotes.Heading_ForeColor;
                                            cEle.NeedsToBeRedrawn = true;
                                        }
                                        if (!bolBuilding)
                                            groupboxNotes.instance.pnlNotes.Building_Complete();
                                    }
                                    break;

                                case enuHighlightStyle.Edit_Main:
                                    {
                                        bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                                        groupboxNotes.instance.pnlNotes.Building_Start();
                                        {
                                            DrawBorder = false;
                                            BackColor = formWords.instance.rtxCK.Heading_BackColor;
                                            ForeColor = formWords.instance.rtxCK.Heading_ForeColor;
                                            cEle.NeedsToBeRedrawn = true;
                                        }
                                        if (!bolBuilding)
                                            groupboxNotes.instance.pnlNotes.Building_Complete();
                                    }
                                    break;

                                case enuHighlightStyle.underMouse:
                                    {
                                        bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                                        groupboxNotes.instance.pnlNotes.Building_Start();
                                        {
                                            DrawBorder = false;
                                            BackColor = Color.LightGray;
                                            ForeColor = Color.Black;
                                            cEle.NeedsToBeRedrawn = true;
                                        }
                                        if (!bolBuilding)
                                            groupboxNotes.instance.pnlNotes.Building_Complete();
                                    }
                                    break;

                                default:
                                    {   //none, EditSelect , underMouse, Edit_Main, Edit_Alt 
                                        bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                                        groupboxNotes.instance.pnlNotes.Building_Start();
                                        {
                                            DrawBorder = false;                                            
                                            BackColor = Color.White;
                                            ForeColor = Color.Black;                                            
                                            cEle.NeedsToBeRedrawn = true;
                                        }
                                        if (!bolBuilding)
                                            groupboxNotes.instance.pnlNotes.Building_Complete();
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            public NotesButton(ref SPContainer SPContainer) : base(ref SPContainer)
            {
                MouseDoubleClick = MouseDouble_Click;

                this.SPContainer = SPContainer;
                eSP_ObjectType
                    = cEle.eSP_ObjectType
                    = enuSweepPrune_ObjectType.UserDefined;
                cEle.obj = (object)this;
                cEle.Name = "Label";
                cEle.eventDraw = eventDraw;

                BackColor
                    = BackColor_Dull
                    = ForeColor_Highlight
                    = Color.White;

                ForeColor
                    = ForeColor_Dull
                    = BackColor_Highlight
                    = Color.Black;
            }

            
            void MouseDouble_Click(object sender, EventArgs e)
            {
                if (cItem != null)
                    cItem.Project_SetParent();
            }

            void eventDraw(object sender, EventArgs e)
            {
                Draw(true);
                if (MyImage != null)
                    panelNotesDataTree.instance.DrawImage(MyImage, cEle.recDraw, new Rectangle(0, 0, MyImage.Width, MyImage.Height));
            }

            public enuNotesButtons eButtonType = enuNotesButtons.Title;

            public classNotesInfo cItem = null;
        }
    }

}
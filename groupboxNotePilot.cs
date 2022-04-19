using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Words
{
    public class groupboxNotePilot : GroupBox
    {
        public static groupboxNotePilot instance = null;

        Label lblParent_Title = new Label();
        Label lblTitle = new Label();
        List<noteEditor_TextBox> lstTextBoxes = new List<noteEditor_TextBox>();
        
        Label lblYear = new Label();
        Label lblMonth = new Label();
        Label lblDay = new Label();

        Label lblHour = new Label();
        Label lblMinute = new Label();

        Label lblPOV = new Label();

        Ck_Objects.classLabelButton btnPath = new Ck_Objects.classLabelButton();
        
        System.Windows.Forms.Timer tmrGrab = new System.Windows.Forms.Timer();
        System.Windows.Forms.ListBox lbxPOV = new System.Windows.Forms.ListBox();

        enum enuButtons { Parent, Up, Down, Add, Sub, EditMain, Enum, Project, Lock, FileToggle,   Expand };

        
        List<Label> lstLbl = new List<Label>();
        public static List<Ck_Objects.classLabelButton> lstBtn = new List<Ck_Objects.classLabelButton>();
        public classNotesInfo cParent
        {
            get { return panelNotesDataTree.cParent; }
            set { panelNotesDataTree.cParent = value; }
        }

        static public classProject cProject
        {
            get { return formWords.cProject; }
        }

        static public classNotesInfo cEdit_Alt
        {
            get { return cProject.cEdit_Alt; }
            set { cProject.cEdit_Alt = value; }
        }
        public void cParent_Changed()
        {
            lblParent_Title.Text = cParent != null ? cParent.Title : "NULL";            
        }

        Ck_Objects.classLabelButton btnParent
        {
            get { return lstBtn[(int)enuButtons.Parent]; }
        }

        public static Ck_Objects.classLabelButton btnAdd
        {
            get { return lstBtn[(int)enuButtons.Add]; }
        }

        public static Ck_Objects.classLabelButton btnSub
        {
            get { return lstBtn[(int)enuButtons.Sub]; }
        }

        public static Ck_Objects.classLabelButton btnDown
        {
            get { return lstBtn[(int)enuButtons.Down]; }
        }

        public static Ck_Objects.classLabelButton btnUp
        {
            get { return lstBtn[(int)enuButtons.Up]; }
        }

        public static Ck_Objects.classLabelButton btnFileToggle
        {
            get { return lstBtn[(int)enuButtons.FileToggle]; }
        }

        public static Ck_Objects.classLabelButton btnEnum
        {
            get { return lstBtn[(int)enuButtons.Enum]; }
        }

        public static Ck_Objects.classLabelButton btnEditMain
        {
            get { return lstBtn[(int)enuButtons.EditMain]; }
        }

        public static Ck_Objects.classLabelButton btnProject
        {
            get { return lstBtn[(int)enuButtons.Project]; }
        }

        public static Ck_Objects.classLabelButton btnExpand
        {
            get { return lstBtn[(int)enuButtons.Expand]; }
        }
        
        public static Ck_Objects.classLabelButton btnLock
        {
            get { return lstBtn[(int)enuButtons.Lock]; }
        }

        // Title, Year, Month, Day, Hour, Minute, ChapterNumber, POV, 
        public noteEditor_TextBox txtTitle
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Title
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Title]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Title)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Title] = value;
                }                                          
            }
        }
        public noteEditor_TextBox txtYear
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Year
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Year]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Year)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Year] = value;
                }
            }
        }
        public noteEditor_TextBox txtMonth
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Month
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Month]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Month)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Month] = value;
                }
            }
        }


        public noteEditor_TextBox txtDay
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Day
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Day]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Day)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Day] = value;
                }
            }
        }


        public noteEditor_TextBox txtHour
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Hour
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Hour]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Hour)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Hour] = value;
                }
            }
        }
        public noteEditor_TextBox txtMinute
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.Minute
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.Minute]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.Minute)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.Minute] = value;
                }
            }
        }
        public noteEditor_TextBox txtChapterNumber
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.ChapterNumber
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.ChapterNumber]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.ChapterNumber)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.ChapterNumber] = value;
                }
            }
        }
        public noteEditor_TextBox txtPOV
        {
            get
            {
                return lstTextBoxes.Count > (int)enuNotesFieldTypes.POV
                                          ? lstTextBoxes[(int)enuNotesFieldTypes.POV]
                                          : null;
            }
            set
            {
                if (lstTextBoxes.Count > (int)enuNotesFieldTypes.POV)
                {
                    lstTextBoxes[(int)enuNotesFieldTypes.POV] = value;
                }
            }
        }

        
        public groupboxNotePilot()
        {
            Text = "Note Pilot";
            BackColor = Color.DarkBlue;
            ForeColor = Color.LightBlue;

            Font = new Font("arial", 12);
            instance = this;

            //  enuNotesFieldTypes  - Title, Year, Month, Day, Hour, Minute, ChapterNumber, POV
            string[] strEnumNames = Enum.GetNames(typeof(enuNotesFieldTypes));
            int intNumFields = strEnumNames.Length;
            lstTextBoxes.Clear();
            string strFormat = "00";
            for (int intFieldCounter = 0; intFieldCounter < (int)enuNotesFieldTypes.Path; intFieldCounter++)
            {
                enuNotesFieldTypes eField = (enuNotesFieldTypes)intFieldCounter;
                noteEditor_TextBox txtNew = new noteEditor_TextBox(eField, strFormat);
                txtNew.Tag = (object)eField;

                Controls.Add(txtNew);
                lstTextBoxes.Add(txtNew);

                txtNew.GotFocus += TxtPOV_FocusChanged;
                txtNew.LostFocus += TxtPOV_FocusChanged;
            }
            
            lstLbl.Add(lblTitle);
            lstLbl.Add(lblYear);
            lstLbl.Add(lblMonth);
            lstLbl.Add(lblDay);
            lstLbl.Add(lblHour);
            lstLbl.Add(lblMinute);
            lstLbl.Add(lblPOV);
            lstLbl.Add(btnPath);

            Controls.Add(lblParent_Title);
            lblParent_Title.AutoSize = false;

            Controls.Add(btnPath);
            btnPath.Text = "Path";
            btnPath.Click += btnPath_Click;

            // build buttons
            int intNumBtn = Enum.GetNames(typeof(enuButtons)).Length;
            Size sz = new Size();
            for (int intCounter = 0; intCounter < intNumBtn ; intCounter++)
            {
                enuButtons eButton = (enuButtons)intCounter;
                Ck_Objects.classLabelButton btnNew = new Ck_Objects.classLabelButton();
                {
                    Controls.Add(btnNew);
                    btnNew.AutoSize = false;
                    
                    btnNew.Tag = (object)eButton;

                    // set size
                    switch(eButton)
                    {
                        case enuButtons.Add:
                            sz = new Size(25, 25);
                            ContextMenu mnu = new ContextMenu();
                            btnNew.ContextMenu = mnu;
                            mnu.MenuItems.Add("New", mnuNote_Add_New_Click);
                            mnu.MenuItems.Add("From File", mnuNote_Add_File_Click);
                            break;

                        case enuButtons.Sub:
                            sz = new Size(25, 25);
                            break;

                        case enuButtons.Expand:
                        case enuButtons.Parent:
                            sz = new Size(35, 35);
                            break;

                        case enuButtons.Up:
                        case enuButtons.Down:
                            sz = new Size(40, 40);
                                break;


                        case enuButtons.FileToggle:
                            sz = new Size(btnPath.Height, btnPath.Height);
                            break;


                        case enuButtons.Enum:
                        case enuButtons.EditMain:
                            sz = new Size(25, 25);
                            break;

                        case enuButtons.Project:
                            {
                                sz = new Size(25, 25);
                                btnNew.ContextMenu = new ContextMenu();
                                btnNew.ContextMenu.MenuItems.Clear();
                                btnNew.ContextMenu.MenuItems.Add(new MenuItem("New", frmWords.mnuProject_New_Click));
                                btnNew.ContextMenu.MenuItems.Add(new MenuItem("Load", frmWords.mnuProject_Load_Click));
                                btnNew.ContextMenu.MenuItems.Add(new MenuItem("Save", frmWords.mnuProject_Save_Click));
                                btnNew.ContextMenu.MenuItems.Add(new MenuItem("Save As", frmWords.mnuProject_SaveAs_Click));
                                btnNew.ContextMenu.MenuItems.Add(new MenuItem("Exit", frmWords.mnuProject_Exit_Click));
                            }
                            break;

                        case enuButtons.Lock:
                            sz = new Size(25, 25);
                            break;
                    }

                    switch(eButton)
                    {                   
                        case enuButtons.Parent:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Collapse, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Collapse_Highlight,sz);
                            }
                            break;

                        case enuButtons.Add:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Add, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Add_Highlight, sz);
                            }
                            break;

                        case enuButtons.Sub:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Sub, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Sub_Highlight, sz);
                            }
                            break;

                        case enuButtons.Up:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Up, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Up_Highlight, sz);
                            }
                            break;

                        case enuButtons.Down:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Down, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Down_Highlight, sz);
                            }
                            break;

                        case enuButtons.FileToggle:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_True, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_True_Highlight, sz);
                            }
                            break;

                        case enuButtons.Enum:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Enum, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Enum_Highlight, sz);
                            }
                            break;

                        case enuButtons.Expand:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Expand, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Expand_Highlight, sz);
                            }
                            break;

                        case enuButtons.EditMain:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_EditMain, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_EditMain_Highlight, sz);
                            }
                            break;

                        case enuButtons.Project:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Project, sz);
                                btnNew.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_Project_Highlight, sz);
                            }
                            break;

                        case enuButtons.Lock:
                            {
                                btnNew.img_Idle = (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_open_idle, sz);
                                btnNew.img_Highlight= (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_open_highlight, sz);
                            }
                            break;
                    }

                    btnNew.Click += btn_Click;
                }
                lstBtn.Add(btnNew);
            }

            foreach (TextBox txt in lstTextBoxes)
            {
                txt.ForeColor = ForeColor;
                txt.BackColor = BackColor;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }

            foreach(Label lbl in lstLbl)
            {
                lbl.ForeColor = Color.Orange;
                lbl.BackColor = BackColor;
            }

            #region Controls


            Controls.Add(lbxPOV);
            lbxPOV.BackColor = Color.Blue;
            lbxPOV.ForeColor = Color.LightBlue;
            lbxPOV.Hide();
            lbxPOV.Click += LbxPOV_Click;
            lbxPOV.VisibleChanged += LbxPOV_VisibleChanged;

            Controls.Add(lblTitle);
            lblTitle.AutoSize = true;
            lblTitle.Text = "Title";
            lblTitle.AutoSize = true;
            Controls.Add(txtTitle);

            Controls.Add(txtChapterNumber);

            Controls.Add(lblYear);
            lblYear.AutoSize = true;
            lblYear.Text = "Year";
            lblYear.AutoSize = true;
            Controls.Add(txtYear);

            Controls.Add(lblMonth);
            lblMonth.AutoSize = true;
            lblMonth.Text = "/";
            lblMonth.AutoSize = true;
            Controls.Add(txtMonth);

            Controls.Add(lblDay);
            lblDay.AutoSize = true;
            lblDay.Text = "/";
            lblDay.AutoSize = true;
            Controls.Add(txtDay);

            Controls.Add(lblHour);
            lblHour.AutoSize = true;
            lblHour.Text = "Time";
            lblHour.AutoSize = true;
            Controls.Add(txtHour);

            Controls.Add(lblMinute);
            lblMinute.AutoSize = true;
            lblMinute.Text = ":";
            lblMinute.AutoSize = true;
            Controls.Add(txtMinute);

            Controls.Add(lblPOV);
            lblPOV.AutoSize = true;
            lblPOV.Text = "POV";
            lblPOV.AutoSize = true;
            Controls.Add(txtPOV);
            #endregion 

            tmrGrab.Interval = 128;
            tmrGrab.Tick += TmrGrab_Tick;

            ContextMenu = cmnu;
            cmnu.Popup += Cmnu_Popup;

            MouseDown += grbNoteEditor_MouseDown;
            MouseUp += grbNoteEditor_MouseUp;
            MouseDoubleClick += GroupboxNotePilot_MouseDoubleClick;
            MouseMove += groupbox_NotePilot_MouseMove;
            MouseLeave += Groupbox_NotePilot_MouseLeave;

            setToLoad();

            tmrPOV_LostFocus.Interval = 250;
            tmrPOV_LostFocus.Tick += tmrPOV_LostFocus_Tick;
            SizeChanged += groupbox_NotePilot_SizeChanged;
            Move += Groupbox_NotePilot_Move;
            VisibleChanged += groupbox_NotePilot_VisibleChanged;
        }

     

        private void tmrPOV_LostFocus_Tick(object sender, EventArgs e)
        {
            tmrPOV_LostFocus.Enabled = false;
            if (txtPOV.Focused) return;
            lbxPOV.Hide();
        }

        public static enuNotesButtons Note_ButtonFromField(enuNotesFieldTypes eField)
        {
            switch(eField)
            {
                case enuNotesFieldTypes.Year:
                case enuNotesFieldTypes.Month:
                case enuNotesFieldTypes.Day:
                    return enuNotesButtons.Date;

                case enuNotesFieldTypes.Hour:
                case enuNotesFieldTypes.Minute:
                    return enuNotesButtons.Time;

                case enuNotesFieldTypes.ChapterNumber:
                    return enuNotesButtons.ChapterNumber;

                case enuNotesFieldTypes.POV:
                    return enuNotesButtons.POV;

                case enuNotesFieldTypes.Title:
                    return enuNotesButtons.Title;

                default:
                    MessageBox.Show("this should not happen");
                    return enuNotesButtons.Title;
            }
        }        

        Timer tmrPOV_LostFocus = new Timer();
        void tmrPOV_LostFocus_Reset()
        {
            tmrPOV_LostFocus.Enabled = false;
            tmrPOV_LostFocus.Enabled = true;
        }

        private void TxtPOV_FocusChanged(object sender, EventArgs e)
        {
            if (txtPOV.Focused)
            {
                noteEditor_TextBox txtSender = (noteEditor_TextBox)sender;
                enuNotesFieldTypes eField = txtSender.eNoteField;
                enuNotesButtons eButton = Note_ButtonFromField(eField);
                int intButtonIndex = (int)eButton;
                if (cEdit_Alt != null)
                {
                    panelNotesDataTree.NotesButton btn = cEdit_Alt.lstButtons[intButtonIndex];
                    panelNotesDataTree.NotesButton btnAlt = cEdit_Alt.lstButtons[(intButtonIndex + 1) % cEdit_Alt.lstButtons.Count];
                    btn.eHighLight = txtSender.Focused ? enuHighlightStyle.EditSelect : btnAlt.eHighLight;
                }
                lbxPOV_Build();
                lbxPOV.Show();
            }
            else
            {
                ///*
                    tmrPOV_LostFocus_Reset();
                /*/
                lbxPOV.Hide();
                // */

            }
        }


        static public void setToLoad()
        {
            instance.Collapsed = bolCollapsed_Load;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            if (cEdit_Alt.File_Toggle)
            {
                FolderBrowserDialog fd = new FolderBrowserDialog();
                try
                {
                    fd.SelectedPath = cEdit_Alt.Path;
                }
                catch (Exception)
                {
                }
                
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    //classNotesInfo.cRoot.setBasePath(fd.SelectedPath);
                    cEdit_Alt.Path 
                        = btnPath.Text
                        = fd.SelectedPath + "\\";
                }
            }
        }

        private void Groupbox_NotePilot_Move(object sender, EventArgs e)
        {
            ptLocation = Location;
        }

        public static Point ptLocation_Load= new Point();
        public static bool bolCollapsed_Load = false;

        static Point _ptLocation = new Point();
        public static Point ptLocation
        {
            get { return _ptLocation; }
            set 
            {
                _ptLocation = value; 
            }
        }

        static Size _szSize = new Size();
        public static Size szSize
        {
            get { return _szSize; }
            set { _szSize = value; }
        }

        panelNotesDataTree pnlEditor
        {
            get { return panelNotesDataTree.instance; }
        }

        groupboxNotes grbNotes
        {
            get { return groupboxNotes.instance; }
        }

        public void mnuNote_Add_File_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Add File(s)";
            ofd.Multiselect = true;
            string strInitialDirectory = cEdit_Alt != null && cEdit_Alt.Path.Length > 0 ? cEdit_Alt.Path : "";

            int intNoteCounter = 0;
            while (intNoteCounter < classNotesInfo.lst.Count && strInitialDirectory.Length == 0)
            {
                classNotesInfo cNoteTemp = classNotesInfo.lst[intNoteCounter];
                if (cNoteTemp != null && cNoteTemp.Path.Length > 0)
                    strInitialDirectory = cNoteTemp.Path;

                intNoteCounter++;
            }

            if (strInitialDirectory.Length == 0)
                strInitialDirectory = System.IO.Directory.GetCurrentDirectory();

            ofd.InitialDirectory = strInitialDirectory;

            ofd.Filter = "rtx|*.rtf";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                for (int intFileCounter = 0; intFileCounter < ofd.FileNames.Length; intFileCounter++)
                {
                    string strFilenameAndPath = ofd.FileNames[intFileCounter];

                    //System.IO.Path 
                    string strFilename = System.IO.Path.GetFileName(strFilenameAndPath);
                    string strExtension = System.IO.Path.GetExtension(strFilenameAndPath);
                    string strHeading = strFilename.Replace(strExtension, "");
                    string strPath = System.IO.Path.GetFullPath(strFilenameAndPath).Replace(strHeading, "").Replace (strExtension, "");

                    classNotesInfo cNoteAdd = new classNotesInfo(strHeading);
                    cParent.ChildNodes_Add(ref cNoteAdd);
                    cNoteAdd.Path = strPath;
                    cNoteAdd.File_Toggle = true;
                    pnlEditor.Buttons_Rebuild();
                    cEdit_Alt = cParent.lstChildNotes.Count > 0
                                                             ? cParent.lstChildNotes[cParent.lstChildNotes.Count - 1]
                                                             : null;
                }
            }
        }

        
        public void mnuNote_Add_New_Click(object sender, EventArgs e)
        {
            classNotesInfo cNoteAdd = new classNotesInfo("New button");
            cParent.ChildNodes_Add(ref cNoteAdd);
            cNoteAdd.Path_SetNew();
            pnlEditor.Buttons_Rebuild();
            //cEdit_Alt = cParent.lstChildNotes.Count > 0
            //                                         ? cParent.lstChildNotes[cParent.lstChildNotes.Count - 1]
            //                                         : null;
            cEdit_Alt = cNoteAdd;

            pnlEditor.ScrollToCaret();
            Collapsed = false;
        }


        public void mnuNoteDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete " + cEdit_Alt.Title + "?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cEdit_Alt.die();
                pnlEditor.Buttons_Rebuild();
                cEdit_Alt = cParent.lstChildNotes.Count > 0
                                                         ? cParent.lstChildNotes[0]
                                                         : null;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Ck_Objects.classLabelButton btnSender = (Ck_Objects.classLabelButton)sender;
            enuButtons eBtn = (enuButtons)btnSender.Tag;
            lbxPOV.Hide();
            switch(eBtn)
            {
                case enuButtons.Add:
                    {
                    
                    }
                    break;

                case enuButtons.Sub:
                    {
                        if (cEdit_Alt !=null)
                            mnuNoteDelete_Click((object)this, new EventArgs());
                    }
                    break;

                case enuButtons.Down:
                    {
                        if (Move_Down())
                            pnlEditor.Buttons_Rebuild();
                    }
                    break;

                case enuButtons.Up:
                    {
                        if (Move_Up())
                            pnlEditor.Buttons_Rebuild();
                    }
                    break;

                case enuButtons.FileToggle:
                    {
                        if (cEdit_Alt != null)
                        {
                            cEdit_Alt.File_Toggle
                                = btnFileToggle.Toggled
                                = groupboxNotes.instance.rtxNotes.Enabled 
                                = btnEditMain.Enabled 
                                = !cEdit_Alt.File_Toggle;
                            if (cEdit_Alt.File_Toggle)
                                grbNotes.Note_Load();
                            cEdit_Alt.eHighLight = enuHighlightStyle.Edit_Alt;
                            Data_setFromNote();
                        }
                    }
                    break;

                case enuButtons.Enum:
                    {
                        if (cParent != null)
                        {
                            for (int intChildCounter = 0; intChildCounter < cParent.lstChildNotes.Count; intChildCounter++)
                            {
                                classNotesInfo cNote = cParent.lstChildNotes[intChildCounter];
                                cNote.Chapter = intChildCounter;
                            }
                            pnlEditor.Buttons_Rebuild();
                        }
                    }
                    break;

                case enuButtons.Expand:
                    {
                    if (cEdit_Alt != null)
                            cParent = cEdit_Alt;
                    }
                    break;

                case enuButtons.Parent:
                    {
                        grbNotes.Parent_Back();
                    }
                    break;

                case enuButtons.EditMain:
                    {
                        cEdit_Alt.Project_SetMainEdit();
                    }
                    break;

                case enuButtons.Project:
                    {
                        Cmnu_Popup((object)cmnu, new EventArgs());
                    }
                    break;

                case enuButtons.Lock:
                    {
                        grbNotes.rtxNotes.mnuLock_Click((object)this, new EventArgs());
                    }
                    break;
            }
        }

        public void btnLock_SetImage()
        {
            if (grbNotes.Locked)
            {
                btnLock.img_Idle = (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_closed_idle);   
                btnLock.img_Highlight= (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_closed_highlight);   
            }
            else
            {
                btnLock.img_Idle = (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_open_idle);
                btnLock.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.btnNotes_Lock_open_highlight);
            }
            btnLock.Redraw();
        }

    

        public bool Move_Up()
        {
            if (cParent == null) return false;
            if (cEdit_Alt == null) return false;

            int intIndex = cParent.lstChildNotes.IndexOf(cEdit_Alt);
            if (intIndex > 0)
            {
                cParent.lstChildNotes.Remove(cEdit_Alt);
                cParent.lstChildNotes.Insert(intIndex - 1, cEdit_Alt);
            }
            //intIndex = cParent.lstChildNotes.IndexOf(cEdit_Alt);
            return true;
        }

        public bool Move_Down()
        {
            if (cParent == null) return false;
            if (cEdit_Alt == null) return false;

            int intIndex = cParent.lstChildNotes.IndexOf(cEdit_Alt);
            if (intIndex < cParent.lstChildNotes.Count - 1)
            {
                cParent.lstChildNotes.Remove(cEdit_Alt);
                cParent.lstChildNotes.Insert(intIndex + 1, cEdit_Alt);
            }

            //intIndex = cParent.lstChildNotes.IndexOf(cEdit_Alt);
            return true;
        }

        private void Cmnu_Popup(object sender, EventArgs e)
        {
            cmnu.MenuItems.Clear();
        }

        private void groupbox_NotePilot_SizeChanged(object sender, EventArgs e)
        {
            szSize = Size;
            Size szText = TextRenderer.MeasureText(Text, Font);

            recGrab = new Rectangle(0, 0, szText.Width, szText.Height);
        }

        Rectangle recGrab = new Rectangle();

        System.Drawing.Point ptGrab = new System.Drawing.Point();
        static System.Drawing.Point ptMouse = new System.Drawing.Point();
        ContextMenu cmnu = new ContextMenu();

        public static formWords frmWords { get { return formWords.instance; } }

        enum enuMouseRegion { idle, grab, collapse };
        enuMouseRegion _eMouseRegion = enuMouseRegion.idle;
        enuMouseRegion eMouseRegion
        {
            get { return _eMouseRegion; }
            set
            {
                if (_eMouseRegion != value)
                {
                    _eMouseRegion = value;

                    Cursor_Set();
                }
            }
        }


        void Cursor_Set()
        {
            switch (_eMouseRegion)
            {
                case enuMouseRegion.collapse:
                    Cursor = bolCollapsed ? formWords.cCursors[(int)enuCursors.Expand]: formWords.cCursors[(int)enuCursors.Collapse];
                    break;

                case enuMouseRegion.idle:
                    Cursor = Cursors.Arrow;
                    break;

                case enuMouseRegion.grab:
                    Cursor = Cursors.Hand;
                    break;
            }
        }
        private void GroupboxNotePilot_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Anchored = true;
        }

        void grbNoteEditor_MouseDown(object sender, MouseEventArgs e)
        {
            switch(eMouseRegion)
            {
                case enuMouseRegion.grab:
                    {
                        // grab
                        ptGrab = new System.Drawing.Point(e.X, e.Y);
                        tmrGrab.Enabled = true;
                        Anchored = false;
                    }
                    break;

                case enuMouseRegion.collapse:
                    {
                        Collapsed = !Collapsed;
                    }
                    break;

                default:
                    placeObjects();
                    break;
            }
        }

        void grbNoteEditor_MouseUp(object sender, MouseEventArgs e)
        {
            tmrGrab.Enabled = false;
        }
        private void groupbox_NotePilot_MouseMove(object sender, MouseEventArgs e)
        {
            if (tmrGrab.Enabled) return;

            ptMouse = new System.Drawing.Point(e.X, e.Y);
            if (Math3.classMath3.PointIsInsideARectangle(ptMouse, recGrab))
                eMouseRegion = enuMouseRegion.grab;
            else
            {
                Rectangle recTestCollapse = new Rectangle(recGrab.Width, recGrab.Top, Width - recGrab.Width, recGrab.Height);
                if (Math3.classMath3.PointIsInsideARectangle(ptMouse, recTestCollapse))
                    eMouseRegion = enuMouseRegion.collapse;
                else
                    eMouseRegion = enuMouseRegion.idle;
            }
        }

        private void Groupbox_NotePilot_MouseLeave(object sender, EventArgs e)
        {
            eMouseRegion = enuMouseRegion.idle;
        }

        private void TmrGrab_Tick(object sender, EventArgs e)
        {
            Point ptMouse = new Point(MousePosition.X, MousePosition.Y);
            Control ctrlParent = (Control)this.Parent;
            Point ptTL = Ck_Objects.classControlLocation.Location(ref ctrlParent);

            Location = new Point(ptMouse.X-ptTL.X - ptGrab.X,
                                 ptMouse.Y -ptTL.Y - ptGrab.Y);
            tmrGrab.Enabled = (MouseButtons == MouseButtons.Left);
        }

        bool bolCollapsed = false;
        public bool Collapsed
        {
            get { return bolCollapsed; }
            set 
            {
                bolCollapsed = value;
                Size = bolCollapsed
                            ? szCollapsed
                            : szExpanded;
                Cursor_Set();
                LocationSet();
            }
        }
        
        bool bolAnchored = false;
        public bool Anchored
        {
            get { return bolAnchored; }
            set 
            {
                bolAnchored = value;
                if (bolAnchored)
                {
                    Cursor_Set();
                    LocationSet();
                }

                ForeColor = bolAnchored
                                ? System.Drawing.Color.Yellow
                                : System.Drawing.Color.White;
            }
        }

        public void LocationSet()
        {
            if (!Anchored)
                return;

            //Size szCollapsed = 
              //  szCollapsed;

            Rectangle recNotePilot = new Rectangle(new Point(formWords.instance.splMain.SplitterDistance - szCollapsed.Width,
                                                             grbNotes.splMain.SplitterDistance + szCollapsed.Height),
                                                   Size);
            if (recNotePilot.Bottom > formWords.instance.Height)
                recNotePilot.Location = new Point(formWords.instance.splMain.SplitterDistance, formWords.instance.Height - recNotePilot.Height - 45);

            Location = recNotePilot.Location;
        }

        Size szExpanded = new Size();
        public Size szCollapsed = new Size();


        private void LbxPOV_VisibleChanged(object sender, EventArgs e)
        {
            placeObjects();
            lbxPOV_Build();
        }

        List<string> lstPOVs = new List<string>();
        public void lbxPOV_Build()
        {
            lbxPOV.Items.Clear();
            lstPOVs = POV_Gather();

            for (int intBTCounter = 0; intBTCounter < lstPOVs.Count; intBTCounter++)
            {               
                lbxPOV.Items.Add(lstPOVs[intBTCounter]);
            }
        }
        List<string> POV_Gather()
        {
            if (cParent == null) return new List<string>();
            List<string> lstRetVal = new List<string>();
            string strPOVs_AlreadySeen = ".";
            for (int intNoteCounter = 0; intNoteCounter < classNotesInfo.lst.Count; intNoteCounter++)
            {
                classNotesInfo cTemp = classNotesInfo.lst[intNoteCounter];
                string strTest = "." + POV_Standardized(cTemp.POV) + ".";
                if (!strPOVs_AlreadySeen.Contains(strTest))
                {
                    strPOVs_AlreadySeen += strTest;
                    lstRetVal.Add(cTemp.POV);
                }
            }
            lstRetVal = StringLibrary.classStringLibrary.Alphabetize_Words(lstRetVal);

            return lstRetVal;
        }

        string POV_Standardized(string strPOV)
        {
            return strPOV.Trim().ToUpper();
        }

        private void LbxPOV_Click(object sender, EventArgs e)
        {
            if (sender == null) return;
            ListBox lbxSender = (ListBox)sender;
            if (lbxPOV != null)
            {
                int intSelected = lbxPOV.SelectedIndex;
                if (intSelected >= 0 && intSelected < lbxPOV.Items.Count)
                {
                    txtPOV.Text = lbxPOV.SelectedItem.ToString();
                    noteEditor_TextBox txtPOV_Ref = txtPOV;
                    txtPOV.txtAlphaNumeric_Enter(ref txtPOV_Ref);

                    txtPOV.Focus();
                    lbxPOV.Hide();
                }
            }
        }

        private void groupbox_NotePilot_VisibleChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        public void placeObjects()
        {
            if (Visible)
            {
                Width = 500;
                int intGap = 1;

                Size sz = TextRenderer.MeasureText(Text, Font);

                lbxPOV.Width = 150;
                lbxPOV.Top = 17;
                lbxPOV.Left = Width - lbxPOV.Width - 4;
                lbxPOV.BringToFront();
                lbxPOV_Build();
                
                for (int intCounter = 0; intCounter < lstBtn.Count ; intCounter++)
                {
                    Ck_Objects.classLabelButton btn = lstBtn[intCounter];
                    enuButtons eButton = (enuButtons)btn.Tag;                    
                    btn.Size = btn.img_Highlight.Size;
                    // position left column

                    switch (eButton)
                    {
                        case enuButtons.Parent:
                            {
                                // left most
                                btn.Location = new Point(intGap * 2, sz.Height + intCounter* (btn.Height + intGap));
                            }
                            break;

                        case enuButtons.FileToggle:
                            {
                                // left most
                                btn.Location = new Point(intGap*2, btnEditMain.Bottom+ intGap);
                            }
                            break;

                        case enuButtons.Add:
                            {
                                btn.Location = new Point(btnDown.Left, btnDown.Bottom + intGap);
                            }
                            break;

                        case enuButtons.Sub:
                            {
                                btn.Location = new Point(btnAdd.Right +intGap , btnAdd.Top);
                            }
                            break;

                        case enuButtons.Up:
                        case enuButtons.Down:
                            {
                                // left of lbx
                                btn.Location = new Point(btn.Width/4 + intGap, sz.Height + intCounter * (btn.Height + intGap));
                            }
                            break;

                        case enuButtons.Expand:
                            {
                                // br
                                btn.Location = new Point(lbxPOV.Right - btn.Width - intGap, btnParent.Top);
                            }
                            break;

                        case enuButtons.Project:
                            {
                                btn.Location = new Point(btnEnum.Right + intGap, btnEnum.Top);
                            }
                            break;

                        case enuButtons.Lock:
                            {
                                btn.Location = new Point(btnProject.Right, btnProject.Top);
                            }
                            break;

                        case enuButtons.EditMain:
                            {

                                btn.Location = new Point(btnAdd.Left, btnAdd.Bottom + intGap);
                            }
                            break;

                        case enuButtons.Enum:
                            {
                                btn.Location = new Point(btnEditMain.Right + intGap, btnEditMain.Top);
                            }
                            break;
                    }
                }

                lblParent_Title.Location = new Point(btnParent.Right + intGap, btnParent.Top);
                lblParent_Title.Size = new Size(btnExpand.Left - lblParent_Title.Left - intGap, btnParent.Height);
                lblParent_Title.Font = new Font("Arial", 20);

                lblTitle.Location = new Point(btnUp.Right + intGap, btnUp.Top);
                txtTitle.Location = new Point(lblTitle.Right, lblTitle.Top);
                txtTitle.Width = Width - txtTitle.Left - intGap;
                txtTitle.Font = new Font("Arial", 22);

                txtChapterNumber.Location = new Point(btnDown.Right + intGap, btnDown.Top);
                txtChapterNumber.Width = 35;
                txtChapterNumber.Font = new Font("Arial", 18);

                lblPOV.Location = new Point(txtChapterNumber.Right + intGap, txtChapterNumber.Top);
                lblPOV.Font = new Font("Arial", 10);
                
                txtPOV.Location = new Point(lblPOV.Right, lblPOV.Top);
                txtPOV.Width = Width  - txtPOV.Left - intGap;
                txtPOV.Font = new Font("Arial", 20);

                btnPath.Location = new Point(btnFileToggle.Right +intGap, btnFileToggle.Bottom - btnPath.Height);
                btnPath.Width = Width - btnPath.Left - intGap;

                txtYear.Width = TextRenderer.MeasureText("0000", txtYear.Font).Width;
                txtMonth.Width
                    = txtDay.Width
                    = txtHour.Width
                    = txtMinute.Width
                    = TextRenderer.MeasureText("00", txtMonth.Font).Width;

                txtYear.Font
                    = txtMonth.Font
                    = txtDay.Font
                    = txtHour.Font
                    = txtMinute.Font
                    = lblYear .Font
                    = lblMonth .Font 
                    = lblDay.Font 
                    = lblHour.Font 
                    = lblMinute.Font 
                    = new Font("Arial", 12);

                // /* Year and time
                // measure from the right 
                txtMinute.Location = new Point(Width - intGap - txtMinute.Width, txtPOV.Bottom + intGap);
                lblMinute.Location = new Point(txtMinute.Left - lblMinute.Width, txtMinute.Top);
                txtHour.Location = new Point(lblMinute.Left - txtHour.Width, txtMinute.Top);
                lblHour.Location = new Point(txtHour.Left - lblHour.Width, txtMinute.Top);

                txtDay.Location = new Point(lblHour.Left - intGap - txtDay.Width, txtMinute.Top);
                lblDay.Location = new Point(txtDay.Left - lblDay.Width, txtMinute.Top);
                txtMonth.Location = new Point(lblDay.Left - txtMonth.Width, txtMinute.Top);
                lblMonth.Location = new Point(txtMonth.Left - lblMonth.Width, txtMinute.Top);
                txtYear.Location = new Point(lblMonth.Left - txtYear.Width, txtMinute.Top);
                lblYear.Location = new Point(txtYear.Left - lblYear.Width, txtMinute.Top);

                /*/  
                // measure from the left
                lblYear.Location = new Point(btnDown.Right + intGap, btnPath.Top - intGap - txtYear.Height);
                txtYear.Location = new Point(lblYear.Right, lblYear.Top);
                lblMonth.Location = new Point(txtYear.Right, txtYear.Top);
                txtMonth.Location = new Point(lblMonth.Right, lblMonth.Top);
                lblDay.Location = new Point(txtMonth.Right, txtMonth.Top);
                txtDay.Location = new Point(lblDay.Right, lblDay.Top);

                lblHour.Location = new Point(txtDay.Right + intGap, txtDay.Top);
                txtHour.Location = new Point(lblHour.Right, lblHour.Top);
                lblMinute.Location = new Point(txtHour.Right, txtHour.Top);
                txtMinute.Location = new Point(lblMinute.Right, lblMinute.Top); 
                //*/

                Height = btnPath.Bottom + intGap ;
                lbxPOV.Height = btnPath.Bottom - lbxPOV.Top;

                Size szHeading = TextRenderer.MeasureText(Text, Font);

                szCollapsed = new Size(szHeading.Width + 25, sz.Height);
                szExpanded = Size;
            }
        }


        public void Data_setFromNote()
        {
            if (cEdit_Alt != null)
            {
                txtTitle.Text = cEdit_Alt.Title;
                txtPOV.Text = cEdit_Alt.POV;
                btnPath.Text = cEdit_Alt.Path;

                txtChapterNumber.Text = cEdit_Alt.Chapter.ToString(txtChapterNumber.FormatString );
                txtYear.Text= cEdit_Alt.Year.ToString(txtYear.FormatString);
                txtMonth.Text = cEdit_Alt.Month.ToString(txtMonth.FormatString);
                txtDay.Text = cEdit_Alt.Day.ToString(txtDay.FormatString);
                txtHour.Text = cEdit_Alt.Hour.ToString(txtHour.FormatString);
                txtMinute.Text = cEdit_Alt.Minute.ToString(txtMinute.FormatString);
                btnFileToggle.Toggled
                    = btnEditMain.Enabled
                    = cEdit_Alt.File_Toggle;
                if (btnFileToggle.Toggled)
                {
                    btnFileToggle.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_True, btnFileToggle.Size);
                    btnFileToggle.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_True_Highlight, btnFileToggle.Size);
                }
                else
                {
                    btnFileToggle.img_Idle = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_False, btnFileToggle.Size);
                    btnFileToggle.img_Highlight = (Bitmap)new Bitmap(Properties.Resources.NotePilot_Button_FileValid_False_Highlight, btnFileToggle.Size);
                }
                btnPath.Enabled = btnFileToggle.Toggled;
                
                btnFileToggle.Redraw();
            }
        }

        public void Data_setToNote()
        {
            if (cEdit_Alt == null) return;

            if (txtTitle.ValidData)
                cEdit_Alt.Title = txtTitle.Text;
            if (txtPOV.ValidData)
                cEdit_Alt.POV = txtPOV.Text;
            cEdit_Alt.Path = btnPath.Text;
            if (txtChapterNumber.ValidData)
                cEdit_Alt.Chapter = txtChapterNumber.Value;
            if (txtYear.ValidData)
                cEdit_Alt.Year = txtYear.Value;
            if (txtMonth.ValidData)
                cEdit_Alt.Month = txtMonth.Value;
            if (txtDay.ValidData)
                cEdit_Alt.Day = txtDay.Value;
            if (txtHour.ValidData)
                cEdit_Alt.Hour = txtHour.Value;
            if (txtMinute.ValidData)
                cEdit_Alt.Minute = txtMinute.Value;
        }

        public class noteEditor_TextBox : TextBox
        {

            public enum enuTextBoxType { Numeric, AlphaNumeric };

            enuTextBoxType _eTypeBox = enuTextBoxType.Numeric;
            public enuTextBoxType eTypeBox { get { return _eTypeBox; } }

            public enuNotesFieldTypes eNoteField = enuNotesFieldTypes.Title;

            bool bolValidData = false;
            public bool ValidData
            {
                get { return bolValidData; }
            }

            public int Value
            {
                get
                {
                    switch (eNoteField)
                    {
                        case enuNotesFieldTypes.Year:
                        case enuNotesFieldTypes.Month:
                        case enuNotesFieldTypes.Day:
                        case enuNotesFieldTypes.Hour:
                        case enuNotesFieldTypes.Minute:
                        case enuNotesFieldTypes.ChapterNumber:
                            noteEditor_TextBox cMyRef = this;
                            return getIntFromTextBox(ref cMyRef);

                        default:
                            MessageBox.Show("this should not happen");
                            break;
                    }

                    return -1;
                }
            }

            public noteEditor_TextBox(enuNotesFieldTypes eNoteField)
            {
                Init(eNoteField, "");
            }

            public noteEditor_TextBox(enuNotesFieldTypes eNoteField, string strFormat)
            {
                Init(eNoteField, strFormat);
            }

            void Init(enuNotesFieldTypes eNoteField, String strFormat)
            {
                this.eNoteField = eNoteField;
                this.FormatString = strFormat;

                switch (eNoteField)
                {
                    case enuNotesFieldTypes.Year:
                        Min = 0;
                        Max = 4000;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;

                    case enuNotesFieldTypes.Month:
                        Min = 1;
                        Max = 12;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;

                    case enuNotesFieldTypes.Day:
                        Min = 1;
                        Max = 31;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;


                    case enuNotesFieldTypes.Hour:
                        Min = 0;
                        Max = 23;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;


                    case enuNotesFieldTypes.Minute:
                        Min = 0;
                        Max = 59;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;


                    case enuNotesFieldTypes.ChapterNumber:
                        Min = 0;
                        Max = int.MaxValue;
                        _eTypeBox = enuTextBoxType.Numeric;
                        break;


                    case enuNotesFieldTypes.POV:
                    case enuNotesFieldTypes.Title:
                    case enuNotesFieldTypes.Path:
                        _eTypeBox = enuTextBoxType.AlphaNumeric;
                        break;
                }

                switch (eTypeBox)
                {
                    case enuTextBoxType.AlphaNumeric:
                        {
                            KeyDown += txtAlphaNumeric_KeyDown;
                        }
                        break;

                    case enuTextBoxType.Numeric:
                        {
                            KeyDown += txtNumeric_KeyDown;
                            MouseWheel += NoteEditor_TextBox_MouseWheel;
                        }
                        break;

                    default:
                        MessageBox.Show("This should not happen");
                        break;
                }
                GotFocus += NoteEditor_TextBox_GotFocus;
            }

            private void NoteEditor_TextBox_MouseWheel(object sender, MouseEventArgs e)
            {
                int intValue = Value + (e.Delta > 0 ? 1 : -1);

                if (intValue < Min)
                    intValue = Min;
                if (intValue > Max)
                    intValue = Max;

                Text = intValue.ToString(strFormatString);

                noteEditor_TextBox txtSender = (noteEditor_TextBox)sender;
                txtNumeric_Enter(ref txtSender);

            }

            private void NoteEditor_TextBox_GotFocus(object sender, EventArgs e)
            {
                noteEditor_TextBox txtSender = (noteEditor_TextBox)sender;
                txtSender.SelectAll();
            }

            static classNotesInfo cEdit_Alt
            {
                get
                {
                    return groupboxNotePilot.cEdit_Alt;
                }
            }
            public static int getIntFromTextBox(ref noteEditor_TextBox txt)
            {
                try
                {
                    int intRetVal = Convert.ToInt32(txt.Text);
                    return intRetVal;
                }
                catch (Exception)
                {
                }

                return -1;
            }

            string strFormatString = "";
            public string FormatString
            {
                get { return strFormatString; }
                set { strFormatString = value; }
            }


            static void txtNumeric_KeyDown(object sender, KeyEventArgs e)
            {
                noteEditor_TextBox txtSender = (noteEditor_TextBox)sender;
                txtSender.bolValidData = false;
                enuNotesFieldTypes eNote = txtSender.eNoteField;
                txtSender.BackColor = System.Drawing.Color.Blue;
                char chrKeyDown = (char)e.KeyValue;
                Keys keyDown = e.KeyCode;
                switch (keyDown)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.Delete:
                    case Keys.Back:
                    case Keys.NumPad0:
                    case Keys.NumPad1:
                    case Keys.NumPad2:
                    case Keys.NumPad3:
                    case Keys.NumPad4:
                    case Keys.NumPad5:
                    case Keys.NumPad6:
                    case Keys.NumPad7:
                    case Keys.NumPad8:
                    case Keys.NumPad9:
                        break;

                    case Keys.Escape:
                        {
                            switch (eNote)
                            {
                                case enuNotesFieldTypes.ChapterNumber:
                                    txtSender.Text = cEdit_Alt.Chapter.ToString(txtSender.FormatString);
                                    break;

                                case enuNotesFieldTypes.Year:
                                    txtSender.Text = cEdit_Alt.Year.ToString(txtSender.FormatString);
                                    break;

                                case enuNotesFieldTypes.Month:
                                    txtSender.Text = cEdit_Alt.Month.ToString(txtSender.FormatString);
                                    break;
                                case enuNotesFieldTypes.Day:
                                    txtSender.Text = cEdit_Alt.Day.ToString(txtSender.FormatString);
                                    break;
                                case enuNotesFieldTypes.Hour:
                                    txtSender.Text = cEdit_Alt.Hour.ToString(txtSender.FormatString);
                                    break;

                                case enuNotesFieldTypes.Minute:
                                    txtSender.Text = cEdit_Alt.Minute.ToString(txtSender.FormatString);
                                    break;

                                default:
                                    MessageBox.Show("this should not happen");
                                    break;
                            }
                        }
                        break;

                    case Keys.Enter:
                        {
                            e.SuppressKeyPress = true;

                            txtNumeric_Enter(ref txtSender);
                        }
                        break;


                    default:
                        {
                            if (!char.IsDigit(chrKeyDown))
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                        }
                        break;
                }
            }

            static public void txtNumeric_Enter(ref noteEditor_TextBox txtSender)
            {
                enuNotesFieldTypes eNote_Field = txtSender.eNoteField;

                int intValue = getIntFromTextBox(ref txtSender);
                if (intValue < txtSender.Min)
                    intValue = txtSender.Min;
                else if (intValue > txtSender.Max)
                    intValue = txtSender.Max;

                txtSender.Text = intValue.ToString(txtSender.FormatString);
                txtSender.BackColor = System.Drawing.Color.DarkBlue;
                txtSender.bolValidData = true;


                enuNotesButtons eNote_Button = noteButton_fromNoteField(eNote_Field);
                int intNote_Button = (int)eNote_Button;
                if (intNote_Button >= 0 && intNote_Button < cEdit_Alt.lstButtons.Count)
                {
                    SPObjects.Button btnEdit = cEdit_Alt.lstButtons[intNote_Button];
                    panelNotesDataTree.NotesButton btnNotes_Edit = (panelNotesDataTree.NotesButton)btnEdit;
                    btnNotes_Edit.cItem.Data[(int)eNote_Field] = (object)intValue;
                    switch (eNote_Button)
                    {
                        case enuNotesButtons.ChapterNumber:
                            btnEdit.Text = cEdit_Alt.Chapter.ToString(txtSender.FormatString);
                            cEdit_Alt.Chapter = txtSender.Value;
                            break;

                        case enuNotesButtons.Date:
                            btnEdit.Text = btnNotes_Edit.cItem.Date;
                            break;

                        case enuNotesButtons.Time:
                            btnEdit.Text = btnNotes_Edit.cItem.Time;
                            break;

                        default:
                            MessageBox.Show("This should not happen");
                            break;
                    }

                    switch (eNote_Field)
                    {
                        case enuNotesFieldTypes.Year:
                            cEdit_Alt.Year = txtSender.Value;
                            break;

                        case enuNotesFieldTypes.Month:
                            cEdit_Alt.Month = txtSender.Value;
                            break;

                        case enuNotesFieldTypes.Day:
                            cEdit_Alt.Day = txtSender.Value;
                            break;

                        case enuNotesFieldTypes.Hour:
                            cEdit_Alt.Hour = txtSender.Value;
                            break;

                        case enuNotesFieldTypes.Minute:
                            cEdit_Alt.Minute = txtSender.Value;
                            break;

                        case enuNotesFieldTypes.ChapterNumber:
                            cEdit_Alt.Chapter = txtSender.Value;
                            break;
                    }

                    int intBtn_Width = panelNotesDataTree.cParent.TabWidths[intNote_Button];
                    SPObjects.classSweepAndPrune_Element cEleTemp = btnEdit.cEle;
                    cEleTemp.rec = new System.Drawing.Rectangle(cEleTemp.rec.Location, new System.Drawing.Size(intBtn_Width, 15));
                    cEdit_Alt.lstButtons[intNote_Button].Draw(true);
                    cEdit_Alt.lstButtons[intNote_Button].cEle.NeedsToBeRedrawn = true;

                    formWords.instance.grbNotes.rtxNotes.Heading = cEdit_Alt.Heading;
                    if (cProject.cEdit_Alt == cProject.cEdit_Main)
                        formWords.instance.rtxCK.Heading = cProject.cEdit_Main.Heading;

                    groupboxNotePilot.instance.Refresh();
                }
            }

            static enuNotesButtons noteButton_fromNoteField(enuNotesFieldTypes eNoteField)
            {
                switch (eNoteField)
                {
                    case enuNotesFieldTypes.Year:
                    case enuNotesFieldTypes.Month:
                    case enuNotesFieldTypes.Day:
                        return enuNotesButtons.Date;

                    case enuNotesFieldTypes.Hour:
                    case enuNotesFieldTypes.Minute:
                        return enuNotesButtons.Time;

                    case enuNotesFieldTypes.POV:
                        return enuNotesButtons.POV;

                    case enuNotesFieldTypes.ChapterNumber:
                        return enuNotesButtons.ChapterNumber;

                    case enuNotesFieldTypes.Title:
                        return enuNotesButtons.Title;


                    default:
                        MessageBox.Show("This should not happen");
                        return enuNotesButtons.Title;

                }
            }
            static string strValidAlphaNumericCharacters = "-_=+ &()^%$#@!;')(&^%$##@@!@#$%^&~`}{[];.,";
            void txtAlphaNumeric_KeyDown(object sender, KeyEventArgs e)
            {

                noteEditor_TextBox txtSender = (noteEditor_TextBox)sender;
                enuNotesFieldTypes eNote_Field = txtSender.eNoteField;

                if (eNote_Field == enuNotesFieldTypes.Path)
                    return;

                txtSender.bolValidData = false;

                txtSender.BackColor = System.Drawing.Color.Blue;

                char chrKeyDown = (char)e.KeyValue;
                Keys keyDown = e.KeyCode;
                switch (keyDown)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.Delete:
                    case Keys.Back:
                    case Keys.NumPad0:
                    case Keys.NumPad1:
                    case Keys.NumPad2:
                    case Keys.NumPad3:
                    case Keys.NumPad4:
                    case Keys.NumPad5:
                    case Keys.NumPad6:
                    case Keys.NumPad7:
                    case Keys.NumPad8:
                    case Keys.NumPad9:
                        break;

                    case Keys.Escape:
                        {
                            switch (eNote_Field)
                            {
                                case enuNotesFieldTypes.POV:
                                    txtSender.Text = cEdit_Alt.POV;
                                    break;

                                case enuNotesFieldTypes.Title:
                                    txtSender.Text = cEdit_Alt.Title;
                                    break;

                                case enuNotesFieldTypes.Path:
                                    txtSender.Text = cEdit_Alt.Path;
                                    break;

                                default:
                                    MessageBox.Show("this should not happen");
                                    break;
                            }
                        }
                        break;

                    case Keys.Enter:
                        {
                            txtAlphaNumeric_Enter(ref txtSender);

                            e.SuppressKeyPress = true;
                        }
                        break;


                    default:
                        {

                            if (char.IsDigit(chrKeyDown)
                                || char.IsLetter(chrKeyDown)
                                || strValidAlphaNumericCharacters.Contains(chrKeyDown))
                                return;

                            e.SuppressKeyPress = true;
                        }
                        break;
                }
            }

            public void txtAlphaNumeric_Enter(ref noteEditor_TextBox txtSender)
            {
                txtSender.BackColor = System.Drawing.Color.DarkBlue;
                enuNotesFieldTypes eNote_Field = txtSender.eNoteField;
                txtSender.bolValidData = true;
                enuNotesButtons eNote_Button = noteButton_fromNoteField(eNote_Field);
                int intNote_Button = (int)eNote_Button;
                if (intNote_Button >= 0 && intNote_Button < cEdit_Alt.lstButtons.Count)
                {
                    SPObjects.Button btnEdit = cEdit_Alt.lstButtons[intNote_Button];
                    btnEdit.Text = txtSender.Text;
                    btnEdit.Draw(true);
                    btnEdit.cEle.NeedsToBeRedrawn = true;
                    switch (eNote_Button)
                    {
                        case enuNotesButtons.POV:
                            {
                                groupboxNotePilot.instance.lbxPOV_Build();
                            }
                            break;

                        case enuNotesButtons.Title:
                            {
                                cEdit_Alt.Title = txtSender.Text;
                            }
                            break;

                    }
                }
            }

            int intMin = 0;
            public int Min
            {
                get { return intMin; }
                set
                {
                    intMin = value;
                    if (intMin < 0) intMin = 0;
                    if (intMax <= intMin) intMax = intMin + 1;
                }
            }

            int intMax = 100;
            public int Max
            {
                get { return intMax; }
                set
                {
                    intMax = value;
                    if (intMin >= intMax)
                        intMin = intMax - 1;
                }
            }
        }

    }
}

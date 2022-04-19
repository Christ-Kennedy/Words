using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Math3;
namespace Ck_Objects
{

    public class classControlLocation
    {
        public static Point Location(ref Control ctrl) { return Location(ref ctrl, new Point(0, 0)); }
        public static Point Location(ref Control ctrl, Point pt)
        {
            

            pt.X += ctrl.Left;
            pt.Y += ctrl.Top;
            Control ctrlParent = ctrl.Parent;

            if (ctrl.Parent == null)
            {
                // this ctrl is a form
                try
                {
                    Form frmCtrl = (Form)ctrl;
                    switch (frmCtrl.FormBorderStyle)
                    {

                        case FormBorderStyle.None:
                            break;

                        case FormBorderStyle.Sizable:
                        case FormBorderStyle.FixedSingle:
                        case FormBorderStyle.FixedToolWindow:
                        case FormBorderStyle.FixedDialog:
                            pt.Y += 31;
                            pt.X += 8;
                            break;

                    }
                }
                catch (Exception)
                {
                }
                return pt;
            }

            return Location(ref ctrlParent, pt);
        }
    }

    public class classIconMaker   //http://csharphelper.com/blog/2017/01/convert-a-bitmap-into-a-cursor-in-c/
    {
        // Create a cursor from a bitmap.
        [StructLayout(LayoutKind.Sequential)]
        struct ICONINFO
        {
            public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies
            // an icon; FALSE specifies a cursor.
            public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot
            // spot is always in the center of the icon, and this member is ignored.
            public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot
            // spot is always in the center of the icon, and this member is ignored.
            public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon,
            // this bitmask is formatted so that the upper half is the icon AND bitmask and the lower half is
            // the icon XOR bitmask. Under this condition, the height should be an even multiple of two. If
            // this structure defines a color icon, this mask only defines the AND bitmask of the icon.
            public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this
            // structure defines a black and white icon. The AND bitmask of hbmMask is applied with the SRCAND
            // flag to the destination; subsequently, the color bitmap is applied (using XOR) to the
            // destination by using the SRCINVERT flag.
        }

        [DllImport("user32.dll")]
        static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll")]
        static extern IntPtr CreateIconIndirect([In] ref ICONINFO piconinfo);

        public static Cursor BitmapToCursor(Bitmap bmp) { return BitmapToCursor(bmp, bmp.Width / 2, bmp.Height / 2); }

        public static Cursor BitmapToCursor(Bitmap bmp, int hot_x, int hot_y)
        {
            // Initialize the cursor information.
            ICONINFO icon_info = new ICONINFO();
            IntPtr h_icon = bmp.GetHicon();
            GetIconInfo(h_icon, out icon_info);
            icon_info.xHotspot = hot_x;
            icon_info.yHotspot = hot_y;
            icon_info.fIcon = false;    // Cursor, not icon.

            // Create the cursor.
            IntPtr h_cursor = CreateIconIndirect(ref icon_info);
            return new Cursor(h_cursor);
        }
    }
    public class classLabelButton : Label
    {
        public classLabelButton()
        {
            MouseEnter += ClassLabelButton_MouseEnter;
            MouseLeave += ClassLabelButton_MouseLeave;
            MouseClick += ClassLabelButton_MouseClick;

            VisibleChanged += ClassLabelButton_VisibleChanged;
            Show();
        }

        private void ClassLabelButton_VisibleChanged(object sender, EventArgs e)
        {
            classLabelButton cSender = (classLabelButton)sender;
            ClassLabelButton_MouseLeave((object)this, new EventArgs());
            //if (!Visible)
            //    MessageBox.Show("fuckeudkging");
        }

        int intTextCounter = 0;
        /// <summary>
        /// list of text strings to be printed sequentially every time the user clicks the button
        /// </summary>
        List<string> _lstText = new List<string>();
        public List<string> lstText
        {
            get { return _lstText; }
            set
            {
                _lstText = value;
                if (_lstText.Count > 0)
                    Text = _lstText[0];
            }
        }

        Color clrForecolor_Idle = Color.Black;
        public Color Forecolor_Idle
        {
            get { return clrForecolor_Idle; }
            set { clrForecolor_Idle = value; }
        }

        Color clrForecolor_Highlight = Color.White;
        public Color Forecolor_Highlight
        {
            get { return clrForecolor_Highlight; }
            set { clrForecolor_Highlight = value; }
        }

        Color clrBackcolor_Idle = Color.White;
        public Color Backcolor_Idle
        {
            get { return clrBackcolor_Idle; }
            set { clrBackcolor_Idle = value; }
        }

        public Color clrBackcolor_Highlight = Color.Black;
        public Color Backcolor_Highlight
        {
            get { return clrBackcolor_Highlight; }
            set { clrBackcolor_Highlight = value; }
        }

         Color clrForecolor_Toggled_Idle = Color.Black;
        public Color Forecolor_Toggled_Idle
        {
            get { return clrForecolor_Toggled_Idle; }
            set { clrForecolor_Toggled_Idle = value; }
        }

        Color clrForecolor_Toggled_Highlight = Color.White;
        public Color Forecolor_Toggled_Highlight
        {
            get { return clrForecolor_Toggled_Highlight; }
            set { clrForecolor_Toggled_Highlight = value; }
        }

        Color clrBackcolor_Toggled_Idle = Color.White;
        public Color Backcolor_Toggled_Idle
        {
            get { return clrBackcolor_Toggled_Idle; }
            set { clrBackcolor_Toggled_Idle = value; }
        }

        Color clrBackcolor_Toggled_Highlight = Color.Black;
        public Color Backcolor_Toggled_Highlight
        {
            get { return clrBackcolor_Toggled_Highlight; }
            set { clrBackcolor_Toggled_Highlight = value; }
        }

        bool bolCanBeToggled = false;
        public bool CanBeToggled
        {
            get { return bolCanBeToggled; }
            set { bolCanBeToggled = value; }
        }



        EventHandler _Toggle_Changed = null;
        public EventHandler Toggle_Changed
        {
            get { return _Toggle_Changed; }
            set { _Toggle_Changed = value; }
        }

        bool bolToggled = false;
        public bool Toggled
        {
            get { return bolToggled; }
            set
            {
                bolToggled = value;
                ClassLabelButton_MouseLeave((object)this, new EventArgs());
                if (Toggle_Changed != null)
                    Toggle_Changed((object)this, new EventArgs());
            }
        }
        public void Redraw()
        {
            ClassLabelButton_MouseLeave((object)this, new EventArgs());
        }
        public void Toggle()
        {
            Toggled = !Toggled;

        }

        bool _bolHighlight = false;
        bool Highlight
        {
            get { return _bolHighlight; }
            set { _bolHighlight = value; }
        }

        Image _img_Idle = null;
        public Image img_Idle
        {
            get { return _img_Idle; }
            set
            {
                if (_img_Idle != value)
                {
                    _img_Idle = value;
                    AutoSize = (_img_Idle == null);
                }
            }
        }

        Image _img_Highlight = null;
        public Image img_Highlight
        {
            get
            {
                return _img_Highlight;
            }
            set
            {
                if (_img_Highlight != value)
                {
                    _img_Highlight = value;
                    AutoSize = (_img_Highlight == null);
                }
            }
        }



        EventHandler _eventHandler_Click;
        public EventHandler eventHandler_Click
        {
            get { return _eventHandler_Click; }
            set
            {
                if (_eventHandler_Click != value)
                {
                    _eventHandler_Click = value;
                    Click += _eventHandler_Click;
                }
            }
        }

        private void ClassLabelButton_MouseLeave(object sender, EventArgs e)
        {
            Highlight = false;
            if (img_Highlight != null
                &&
                img_Idle != null)
            {
                Image = img_Idle;
                Size = img_Idle.Size;
            }
            else
            {
                setColors();
            }
        }

        public void setColors()
        {
            if (Toggled)
            {
                ForeColor = Highlight ? Forecolor_Toggled_Highlight : Forecolor_Toggled_Idle;
                BackColor = Highlight ? Backcolor_Toggled_Highlight : Backcolor_Toggled_Idle;
            }
            else
            {
                ForeColor = Highlight ? Forecolor_Highlight : Forecolor_Idle;
                BackColor = Highlight ? Backcolor_Highlight : Backcolor_Idle;
            }
        }


        private void ClassLabelButton_MouseEnter(object sender, EventArgs e)
        {
            Highlight = true;
            if (img_Highlight != null
                &&
                img_Idle != null)
            {
                Image = img_Highlight;
                Size = img_Highlight.Size;
            }
            else
            {
                setColors();
            }
        }
        private void ClassLabelButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CanBeToggled) Toggle();
            if (lstText.Count > 0)
            {
                intTextCounter = (intTextCounter + 1) % lstText.Count;
                Text = lstText[intTextCounter];
            }
        }
    }
    public class classMultiButtonPic : PictureBox
    {
        public enum enuButtonFormation { contiguous, column, manual, _num };
        public enum enuTypeSearch { Vertical, Horizontal, _num };

        public classSweepAndPrune cMap = new classSweepAndPrune();
        
        Graphics g = null;
        Bitmap bmp = null;
        public classMultiButtonPic()
        {
            MouseMove += Event_MouseMove;
            MouseEnter += Event_MouseEnter;
            MouseLeave += Event_MouseLeave;
            MouseClick += Event_MouseClick;
            MouseDown += Event_MouseDown;
            MouseUp += Event_MouseUp;
            MouseWheel += Event_MouseWheel;
            SizeChanged += SizeChangeEvent;

        }

        void Selection_GatherSelectedButtons()
        {
            if (lstButtonsSelected.Count < 1)
                return;
            
            IEnumerable<classButton> query = lstButtonsSelected.OrderBy(btn => btn.Index);
            _lstButtonsSelected = (List<classButton>)query.ToList<classButton>();

            cBtn_Move_GrabButton = lstButtonsSelected[0];
            Selection_Move(cBtn_Move_GrabButton.Index, cBtn_Move_GrabButton);
        }

        void Selection_Move(int intGrabButtonNewIndex){ Selection_Move(intGrabButtonNewIndex, ButtonUnderMouse); }
        void Selection_Move(int intGrabButtonNewIndex, classButton cBtn_RelativeTo)
        {
            if (lstButtonsSelected != null
                && lstButtonsSelected.Count > 0
                && cBtn_RelativeTo != null )
            {
                List<classButton> lstButtons_TopList = new List<classButton>();
                List<classButton> lstButtons_BottomList = new List<classButton>();
                bool bolBefore = true;
                int intGrabInSelectedList = lstButtonsSelected.IndexOf(cBtn_Move_GrabButton);
                if (intGrabInSelectedList >= 0)
                {
                    List<classButton> lstButtons_Old = new List<classButton>();
                    for (int intButtonCounter = 0; intButtonCounter < cMap.lstElements.Count; intButtonCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMap.lstElements[intButtonCounter].obj;
                        if (!lstButtonsSelected.Contains(cBtn))
                        {
                            cBtn.Index = lstButtons_Old.Count;
                            lstButtons_Old.Add(cBtn);
                        }
                    }
                    
                    for (int intButtonCounter = 0; intButtonCounter < lstButtons_Old.Count; intButtonCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons_Old[intButtonCounter];
                        if (cBtn_RelativeTo.Index < cBtn_Move_GrabButton.Index)
                        {
                            if (cBtn.Index >= cBtn_RelativeTo.Index - intGrabInSelectedList)
                                bolBefore = false;
                        }
                        else if (cBtn_RelativeTo.Index == cBtn_Move_GrabButton.Index)
                        {
                            if (cBtn.Index >= cBtn_RelativeTo.Index)
                                bolBefore = false;
                        }
                        else 
                        {
                            if (cBtn.Index > cBtn_RelativeTo.Index )
                                bolBefore = false;
                        }

                        if (!lstButtonsSelected.Contains(cBtn))
                        {
                            if (bolBefore)
                                lstButtons_TopList.Add(cBtn);
                            else
                                lstButtons_BottomList.Add(cBtn);
                        }
                    }

                    List<classButton> lstButtons_New = new List<classButton>();
                    lstButtons_New.AddRange(lstButtons_TopList);
                    lstButtons_New.AddRange(lstButtonsSelected);
                    lstButtons_New.AddRange(lstButtons_BottomList);

                    ClearButtons();
                    for (int intButtonCounter = 0; intButtonCounter < lstButtons_New.Count; intButtonCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons_New[intButtonCounter];
                        Button_Add(ref cBtn);
                    }

                    placeButtons();
                    Refresh();
                }
            }
        }


        void setMultiSelection()
        {
            PointF[] ptfPolygon = lstMultiSelect_Polygon.ToArray();
            classLine[] arrLines = new classLine[lstMultiSelect_Polygon.Count];
            List<classLine> lstPolygonLines = arrLines.ToList<classLine>();

            lstButtonsSelected.Clear();
            for (int intButtonCounter = 0; intButtonCounter < cMap.lstElements.Count; intButtonCounter++)
            {
                classMultiButtonPic.classButton cButton = (classMultiButtonPic.classButton)cMap.lstElements[intButtonCounter].obj;
                cButton.Highlight = false;
                List<Point> lstCorners = new List<Point>();

                // test if corners are contained inside the polygon
                for (int intCornerCounter = 0; intCornerCounter < (int)Math3.enuCorner._num; intCornerCounter++)
                {
                    Math3.enuCorner eCorner = (Math3.enuCorner)intCornerCounter;
                    Point ptCorner = new Point();
                    switch (eCorner)
                    {
                        case enuCorner.TL:
                            {
                                ptCorner.X = cButton.Area.Left;
                                ptCorner.Y = cButton.Area.Top;
                            }
                            break;

                        case enuCorner.TR:
                            {
                                ptCorner.X = cButton.Area.Right;
                                ptCorner.Y = cButton.Area.Top;
                            }
                            break;

                        case enuCorner.BR:
                            {
                                ptCorner.X = cButton.Area.Right;
                                ptCorner.Y = cButton.Area.Bottom;
                            }
                            break;

                        case enuCorner.BL:
                            {
                                ptCorner.X = cButton.Area.Left;
                                ptCorner.Y = cButton.Area.Bottom;
                            }
                            break;
                    }
                    lstCorners.Add(ptCorner);
                    if (classMath3.PointFIsInsideAPolygon(ptfPolygon, ptCorner))
                    {
                        lstButtonsSelected.Add(cButton);
                        cButton.Highlight = true;
                        break;
                    }
                }
                if (!lstButtonsSelected.Contains(cButton))
                {
                    // test if lines intersect
                    bool bolLoop = true;
                    for (int intPolygonLineCounter = 0; intPolygonLineCounter < lstMultiSelect_Polygon.Count && bolLoop; intPolygonLineCounter++)
                    {
                        if (lstPolygonLines[intPolygonLineCounter] == null)
                        {
                            classLine cLine = new classLine(lstMultiSelect_Polygon[intPolygonLineCounter], lstMultiSelect_Polygon[(intPolygonLineCounter + 1) % lstMultiSelect_Polygon.Count]);
                            lstPolygonLines[intPolygonLineCounter] = cLine;
                        }
                        for (int intCornerCounter = 0; intCornerCounter < lstCorners.Count && bolLoop; intCornerCounter++)
                        {
                            classLine cLineButton = new classLine(lstCorners[intCornerCounter], lstCorners[(intCornerCounter + 1) % lstCorners.Count]);
                            PointF ptfIntersection = new PointF();
                            if (lstPolygonLines[intPolygonLineCounter].IntersectsLine(ref cLineButton, ref ptfIntersection))
                            {
                                lstButtonsSelected.Add(cButton);
                                cButton.Highlight = true;
                                bolLoop = false;
                            }
                        }
                    }
                }
            }
        }

        bool bolCanMoveSelection = false;
        public bool CanMoveSelection
        {
            get { return bolCanMoveSelection; }
            set { bolCanMoveSelection = value; }
        }

        classMultiButtonPic.classButton cBtn_Move_GrabButton = null;

        enum enuMultiSelectMode { idle, Selecting_WaitingForMouseDown, Selecting, Selected, MoveSelection, _num};
        enuMultiSelectMode _eMultiSelectMode = enuMultiSelectMode.idle;
        enuMultiSelectMode eMultiSelectMode
        {
            get { return _eMultiSelectMode; }
            set
            {
                if (_eMultiSelectMode != value)
                {
                    _eMultiSelectMode = value;
              //     System.Diagnostics.Debug.Print(eMultiSelectMode.ToString() + " lstSelected:" + lstButtonsSelected.Count.ToString());
                    switch (_eMultiSelectMode)
                    {
                        case enuMultiSelectMode.Selecting_WaitingForMouseDown:
                            {
                                _lstButtonsSelected.Clear();
                            }
                            break;

                        case enuMultiSelectMode.Selected:
                            {
                          
                            }
                            break;

                        case enuMultiSelectMode.idle:
                            {
                                if (lstButtonsSelected.Count > 0)
                                {
                                    SelectedButtons_Clear();
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void Buttons_Select(List<int> lstIndices)
        {
            SelectedButtons_Clear();
            for (int intCounter = 0; intCounter < lstIndices.Count; intCounter++)
            {
                if (lstIndices[intCounter] >= 0 && lstIndices[intCounter] < cMap.lstElements.Count)
                {
                    classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMap.lstElements[lstIndices[intCounter]].obj;
                    lstButtonsSelected.Add(cBtn);
                }
            }
            Refresh();
        }

        void mnuMultiSelect_Click(object sender, EventArgs e)
        {
            eMultiSelectMode = enuMultiSelectMode.Selecting_WaitingForMouseDown;
        }

        override public void Refresh()
        {
            initG();
            if (BackgroundImage != null)
                g.DrawImage(BackgroundImage, new PointF());

            for (int intCounter = 0; intCounter < cMap.lstElements.Count; intCounter++)
            {
                classButton cBtn = (classButton)cMap.lstElements[intCounter].obj;
                bool bolHighlight = MultiSelect && lstButtonsSelected.Count > 0
                                            ? lstButtonsSelected.Contains(cBtn)
                                            : cBtn.Highlight && cBtn.Image_Highlight != null;


                if (bolHighlight)
                    g.DrawImage(cBtn.Image_Highlight, cBtn.Location);
                else if (cBtn.Image_Idle != null)
                    g.DrawImage(cBtn.Image_Idle, cBtn.Location);
            }

            if (MultiSelect)
            {
                switch (eMultiSelectMode)
                {
                    case enuMultiSelectMode.Selected:
                    case enuMultiSelectMode.Selecting:
                        {if (lstMultiSelect_Polygon != null && lstMultiSelect_Polygon.Count > 3)
                                g.DrawPolygon(Pens.YellowGreen, lstMultiSelect_Polygon.ToArray());
                        }
                        break;
                }
            }
            Image = bmp;
        }

        void initG()
        {
            if (Width > 10 && Height > 10)
            {
                bmp = new Bitmap(Width, Height);
                g = Graphics.FromImage(bmp);
            }
        }

        EventHandler _eventHandler_SizeChanged = null;
        public EventHandler eventHandler_SizeChanged
        {
            get { return _eventHandler_SizeChanged; }
            set
            {
                if (_eventHandler_SizeChanged != value)
                {
                    if (_eventHandler_SizeChanged != null)
                        SizeChanged -= _eventHandler_SizeChanged;
                    _eventHandler_SizeChanged = value;
                }
            }
        }

        private void SizeChangeEvent(object sender, EventArgs e)
        {
            if (_eventHandler_SizeChanged != null)
            {
                _eventHandler_SizeChanged(sender, e);
            }
            else
            {
                cMap.Reset();
                placeButtons();
                
            }
        }

        MouseEventHandler _eventHandler_MouseDown;
        public MouseEventHandler eventHandler_MouseDown
        { 
        get { return _eventHandler_MouseDown; }
            set
            {
                if (_eventHandler_MouseDown != value)
                {
                    if (_eventHandler_MouseDown != null)
                        MouseDown -= _eventHandler_MouseDown;
                    _eventHandler_MouseDown = value;
                    if (_eventHandler_MouseDown != null)
                        MouseDown += _eventHandler_MouseDown;
                }
            }
        }

        public void Event_MouseDown(object sender, MouseEventArgs e)
        {
            semChangeList.WaitOne();

            if (MultiSelect)
            {
                switch (eMultiSelectMode)
                {
                    case enuMultiSelectMode.Selecting_WaitingForMouseDown:
                        {
                            lstMultiSelect_Polygon = new List<PointF>();
                            lstMultiSelect_Polygon.Add(new Point(e.X, e.Y));
                            eMultiSelectMode = enuMultiSelectMode.Selecting;
                        }
                        break;

                    case enuMultiSelectMode.idle:
                        {
                            if (ButtonUnderMouse != null)
                            {
                                if (ButtonUnderMouse.CanBeToggled)
                                    ButtonUnderMouse.Toggle();
                                else
                                {
                                    if (!lstButtonsSelected.Contains(ButtonUnderMouse))
                                    {
                                        lstButtonsSelected.Clear();
                                        lstButtonsSelected.Add(ButtonUnderMouse);
                                        lstMultiSelect_Polygon = new List<PointF>();
                                        lstMultiSelect_Polygon.Add(new Point(e.X, e.Y));
                                        cBtn_Move_GrabButton = ButtonUnderMouse;
                                        Refresh();
                                    }
                                    eMultiSelectMode = enuMultiSelectMode.Selecting;
                                }
                            }
                        }
                        break;

                    case enuMultiSelectMode.Selected:
                        {
                            if (ButtonUnderMouse != null)
                            {
                                if (ButtonUnderMouse.CanBeToggled)
                                    ButtonUnderMouse.Toggle();
                                else
                                {
                                    if (!lstButtonsSelected.Contains(ButtonUnderMouse))
                                    {
                                        lstButtonsSelected.Clear();
                                        lstButtonsSelected.Add(ButtonUnderMouse);



                                        Refresh();
                                    }
                                }
                                cBtn_Move_GrabButton = ButtonUnderMouse;
                                eMultiSelectMode = enuMultiSelectMode.MoveSelection;
                            }
                            else 
                            {
                                SelectedButtons_Clear();
                                eMultiSelectMode = enuMultiSelectMode.idle;
                            }
                        }
                        break;
                }
            }
            semChangeList.Release();
        }

        void SelectedButtons_Clear()
        {
            if (lstButtonsSelected.Count > 0)
            {
                for (int intButtonCounter = 0; intButtonCounter < lstButtonsSelected.Count; intButtonCounter++)
                {
                    classMultiButtonPic.classButton cBtn = lstButtonsSelected[intButtonCounter];
                    cBtn.Highlight = cBtn.Toggled;
                }
                lstButtonsSelected.Clear();
                Refresh();
            }
            
        }

        MouseEventHandler _eventHandler_MouseUp = null;
        public MouseEventHandler eventHandler_MouseUp
        {
            get { return _eventHandler_MouseUp; }
            set
            {
                if (_eventHandler_MouseUp != value)
                {
                    if (_eventHandler_MouseUp != null)
                        MouseUp -= _eventHandler_MouseUp;
                    _eventHandler_MouseUp = value;
                    if (_eventHandler_MouseUp != null)
                        MouseUp += _eventHandler_MouseUp;
                }
            }
        }

        public void Event_MouseUp(object sender, MouseEventArgs e)
        {
            semChangeList.WaitOne();
            if (MultiSelect)
            {
                switch (eMultiSelectMode)
                {
                    case enuMultiSelectMode.Selecting:
                        {
                            Selection_GatherSelectedButtons();
                            lstMultiSelect_Polygon.Clear();
                            Refresh();
                            cBtn_Move_GrabButton = null;
                            eMultiSelectMode = enuMultiSelectMode.Selected;
                        }
                        break;

                    case enuMultiSelectMode.MoveSelection:
                        {
                            cBtn_Move_GrabButton = null;
                            eMultiSelectMode = enuMultiSelectMode.Selected;
                        }
                        break;
                }
            }
            semChangeList.Release();
        }

        MouseEventHandler _eventHandler_MouseWheel = null;
        public MouseEventHandler eventHandler_MouseWheel
        {
            get { return _eventHandler_MouseWheel; }
            set
            {
                if (_eventHandler_MouseWheel != value)
                {
                    if (_eventHandler_MouseWheel != null)
                        MouseWheel -= _eventHandler_MouseWheel;
                    _eventHandler_MouseWheel = value;
                    if (_eventHandler_MouseWheel != null)
                        MouseWheel += _eventHandler_MouseWheel;
                }
            }
        }

        EventHandler _eventHandler_ButtonListChanged = null;
        public EventHandler eventHandler_ButtonListChanged
        { 
        get { return _eventHandler_ButtonListChanged; }

            set
            {
                if (_eventHandler_ButtonListChanged != value)
                {
                    _eventHandler_ButtonListChanged = value;
                }
            }
        }

        public System.Threading.Semaphore semChangeList = new System.Threading.Semaphore(1, 1);
        public void Event_MouseWheel(object sender, MouseEventArgs e)
        {
            semChangeList.WaitOne();

            if (CanMoveSelection && lstButtonsSelected.Count > 0)
            {
                CanMoveSelection = false;
                int intIndexSelected_High = -1;
                int intIndexSelected_Low = cMap.lstElements.Count;
                List<classMultiButtonPic.classButton> lstButtons = new List<classButton>();
                for (int intCounter = 0; intCounter < cMap.lstElements.Count; intCounter++)
                {
                    classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMap.lstElements[intCounter].obj;
                    lstButtons.Add(cBtn);
                    if (lstButtonsSelected.Contains(cBtn))
                    {
                        if (intCounter < intIndexSelected_Low)
                            intIndexSelected_Low = intCounter;
                        if (intCounter > intIndexSelected_High)
                            intIndexSelected_High = intCounter;
                    }
                }

                intIndexSelected_Low--;
                intIndexSelected_High++;

                ClearButtons();

                if (e.Delta > 0)
                { // move Up
                    // move buttons from front of list that are before the lowest selected button index
                    for (int intCounter = 0; intCounter < intIndexSelected_Low; intCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons[0];
                        lstButtons.Remove(cBtn);
                        Button_Add(ref cBtn);
                    }

                    // move buttons that are selected
                    for (int intCounter = 0; intCounter < lstButtonsSelected.Count; intCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtonsSelected[intCounter];
                        lstButtons.Remove(cBtn);
                        Button_Add(ref cBtn);
                    }

                    // move remaining buttons
                    while (lstButtons.Count > 0)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons[0];
                        lstButtons.Remove(cBtn);
                        Button_Add(ref cBtn);
                    }
                }
                else
                {
                    List<classMultiButtonPic.classButton> lstTemp = new List<classButton>();
                    // remove buttons from the end that are after the highest selected button 
                    for (int intCounter = lstButtons.Count - 1; intCounter > intIndexSelected_High; intCounter--)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons[lstButtons.Count - 1];
                        lstButtons.Remove(cBtn);
                        lstTemp.Insert(0, cBtn);
                    }

                    // move buttons that are selected
                    for (int intCounter = 0; intCounter < lstButtonsSelected.Count; intCounter++)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtonsSelected[intCounter];
                        lstButtons.Remove(cBtn);
                        lstTemp.Insert(0, cBtn);
                    }

                    // move remaining buttons
                    while (lstButtons.Count > 0)
                    {
                        classMultiButtonPic.classButton cBtn = lstButtons[lstButtons.Count - 1];
                        lstButtons.Remove(cBtn);
                        lstTemp.Insert(0, cBtn);
                    }

                    Button_Add_Array(ref lstTemp);
                }
                placeButtons();
                CanMoveSelection = true;
            }
  
            semChangeList.Release();
        }


        List<classMultiButtonPic.classButton> _lstButtonsSelected = new List<classButton>();
        public List<classMultiButtonPic.classButton> lstButtonsSelected
        {
            get { return _lstButtonsSelected; }
        }


        MouseEventHandler _eventHandler_MouseClick;
        public MouseEventHandler eventHandler_MouseClick
        {
            get { return _eventHandler_MouseClick; }
            set
            {
                if (_eventHandler_MouseClick != value)
                {
                    if (_eventHandler_MouseClick != null)
                        MouseClick -= _eventHandler_MouseClick;
                    MouseClick -= Event_MouseClick;

                    _eventHandler_MouseClick = value;

                    if (_eventHandler_MouseClick != null)
                        MouseClick += _eventHandler_MouseClick;
                }
            }
        }

        virtual public void Event_MouseClick(object sender, MouseEventArgs e)
        {
            semChangeList.WaitOne();
      
            semChangeList.Release();
        }

        EventHandler _eventHandler_Enter;
        public EventHandler eventHandler_Enter
        {
            get { return _eventHandler_Enter; }
            set
            {
                if (_eventHandler_Enter != value)
                {
                    if (_eventHandler_Enter != null)
                        MouseEnter -= _eventHandler_Enter;
                    _eventHandler_Enter = value;
                    if (_eventHandler_Enter != null)
                        MouseEnter += _eventHandler_Enter;
                }
            }
        }

        private void Event_MouseEnter(object sender, EventArgs e)
        {

        }

        EventHandler _eventHandler_Leave;
        public EventHandler eventHandler_Leave
        {
            get { return _eventHandler_Leave; }
            set
            {
                if (_eventHandler_Leave != value)
                {
                    if (_eventHandler_Leave != null)
                        MouseLeave -= _eventHandler_Leave;
                    _eventHandler_Leave = value;
                    if (_eventHandler_Leave != null)
                        MouseLeave += _eventHandler_Leave;
                }
            }
        }

        private void Event_MouseLeave(object sender, EventArgs e)
        {
            
            switch (eMultiSelectMode)
            {
                case enuMultiSelectMode.MoveSelection:
                    {
                        eMultiSelectMode = enuMultiSelectMode.Selected;
                    }
                    break;

                default:
                    break;
            }

            cButtonUnderMouse = null;
        }

        MouseEventHandler _eventHandler_MouseMove;
        public MouseEventHandler eventHandler_MouseMove
        {
            get { return _eventHandler_MouseMove; }
            set
            {
                if (_eventHandler_MouseMove != value)
                {
                    if (_eventHandler_MouseMove != null)
                        MouseMove -= _eventHandler_MouseMove;
                    _eventHandler_MouseMove = value;
                    if (_eventHandler_MouseMove != null)
                        MouseMove += _eventHandler_MouseMove;
                }
            }
        }
        List<PointF> lstMultiSelect_Polygon = new List<PointF>();

        public void Event_MouseMove(object sender, MouseEventArgs e)
        {
            semChangeList.WaitOne();
            Point ptMouse = new Point(e.X, e.Y);
            if (MultiSelect)
            {
                
                switch (eMultiSelectMode)
                {
                    case enuMultiSelectMode.Selecting:
                        {
                            if (lstMultiSelect_Polygon != null && lstMultiSelect_Polygon.Count > 0)
                            {
                                int intMinDistance = 20;
                                double dblDistance = (double)Math.Sqrt(Math.Pow(ptMouse.X - lstMultiSelect_Polygon[lstMultiSelect_Polygon.Count - 1].X, 2)
                                                                     + Math.Pow(ptMouse.Y - lstMultiSelect_Polygon[lstMultiSelect_Polygon.Count - 1].Y, 2));
                                if (dblDistance > intMinDistance)
                                {
                                    lstMultiSelect_Polygon.Add(new PointF(e.X, e.Y));
                                    setMultiSelection();
                                }
                                
                            }
                            Refresh();
                        }
                        break;

                    case enuMultiSelectMode.MoveSelection:
                        {
                            if (ButtonUnderMouse != null && ButtonUnderMouse != cBtn_Move_GrabButton)
                            {
                                Selection_Move(ButtonUnderMouse.Index);
                            }
                        }
                        break;
                }
            }
            

            classButton cBtnHighlighted = ButtonUnderMouse;
            if (ButtonUnderMouse != null)
                ButtonUnderMouse.Highlight = ButtonUnderMouse.CanBeToggled && ButtonUnderMouse.Toggled;

            classSweepAndPrune.classElement cEleFind = cMap.Search(ptMouse );

        
               cButtonUnderMouse = cEleFind != null
                                             ? (classButton)cEleFind.obj
                                             : null;

            
            if (ButtonUnderMouse != null)
                ButtonUnderMouse.Highlight = true;

            
            if (ButtonUnderMouse != cBtnHighlighted)
                Refresh();

            
            semChangeList.Release();
        }

        void ButtonListChanged()
        {
            if (_eventHandler_ButtonListChanged != null)
                _eventHandler_ButtonListChanged((object)this, new EventArgs());
        }

        public void Button_Add(ref classButton cBtn)
        {
            classSweepAndPrune.classElement cEle = new classSweepAndPrune.classElement();
            cEle.obj = (object)cBtn;
            cEle.rec = cBtn.Area;
            cBtn.Index = cMap.lstElements.Count;
            cMap.Add(ref cEle);
            cBtn.cEle = cEle;
            ButtonListChanged();
        }

        public void Button_Add_Array(ref List<classButton> lstBtn)
        {
            for (int intCounter = 0; intCounter < lstBtn.Count; intCounter++)
            {
                classButton cButton = lstBtn[intCounter];
                Button_Add(ref cButton);
            }
        }

        public classButton Button_New()
        {
            classMultiButtonPic cMyRef = this;
            classButton cBtn = new classButton(ref cMyRef);
            classSweepAndPrune.classElement cEle = new classSweepAndPrune.classElement();
            cEle.obj = (object)cBtn;
            cEle.rec = cBtn.Area;
            cMap.Add(ref cEle);
            cBtn.cEle = cEle;
            ButtonListChanged();

            return cBtn;
        }
        EventHandler _eventHandler_ButtonUnderMouse_Changed = null;
        public EventHandler eventHandler_ButtonUnderMouse_Changed
        {
            get
            {
                return _eventHandler_ButtonUnderMouse_Changed;
            }
            set
            {
                _eventHandler_ButtonUnderMouse_Changed = value;
            }
        }


        public void Button_Sub(ref classButton cBtn)
        {
            cMap.Sub(ref cBtn.cEle);
            ButtonListChanged();
        }

        public void ClearButtons()
        {
            cMap.lstElements.Clear();
            cMap.Reset();
            ButtonListChanged();
            Refresh();
        
        }


        static int intIDCounter = 0;
        int intID = intIDCounter++;

        public int ID {  get { return intID; } }
       public  classButton ButtonUnderMouse { get { return _cButtonUnderMouse; } }

        classButton _cButtonUnderMouse = null;
        classButton cButtonUnderMouse
        {
            get { return _cButtonUnderMouse; }
            set
            {
                if ( _cButtonUnderMouse != value )
                {
                    if(_cButtonUnderMouse != null)
                        _cButtonUnderMouse.Highlight = _cButtonUnderMouse.Toggled;
                    _cButtonUnderMouse = value;
                    if (eventHandler_ButtonUnderMouse_Changed != null)
                        eventHandler_ButtonUnderMouse_Changed((object)cButtonUnderMouse, new EventArgs());
                    Refresh();
                }
            }
        }

        public int ButtonUnderMouse_Index
        {
            get 
            {
            if (_cButtonUnderMouse != null)
                {
                    classSweepAndPrune.classElement cEle = cButtonUnderMouse.cEle;
                    return cMap.lstElements.IndexOf(cEle);
                }
                return -1;
            
            }
        }

        enuButtonFormation eFormation = enuButtonFormation.contiguous;
        public enuButtonFormation Formation
        {
            get { return eFormation; }
            set
            {
                if (eFormation != value)
                {
                    eFormation = value;
                    placeButtons();
                }
            }
        }

        bool bolMultiSelect = false;
        public bool MultiSelect
        {
            get { return bolMultiSelect; }
            set { bolMultiSelect = value; }
        }


        int intBorder = 5;
        public int BorderSize
        {
            get { return intBorder; }
            set
            {
                if (intBorder != value)
                {
                    intBorder = 5;
                    placeButtons();
                }
            }
        }

        public void placeButtons()
        {
            switch (eFormation)
            {
                case enuButtonFormation.column:
                    placeButtons_Column();
                    break;

                case enuButtonFormation.contiguous:
                    placeButtons_Contiguous();
                    break;

                default:
                    break;
            }

            cMap.Reset();
            Refresh();
        }

        public void placeButtons_Column(ref List<classButton> lstButtons, Rectangle recArea)
        { 
            // columns            
            {
                Point ptCornerColumns = recArea.Location;
                Point ptLocation = new Point(ptCornerColumns.X, ptCornerColumns.Y);

                int intColumnWidtdh = 0;
                for (int intCounter = 0; intCounter < lstButtons.Count; intCounter++)
                {
                    classButton cBtn = lstButtons[intCounter];
                    if (cBtn.Area.Width > intColumnWidtdh)
                        intColumnWidtdh = cBtn.Area.Size.Width;
                }

                for (int intCounter = 0; intCounter < lstButtons.Count; intCounter++)
                {
                    classButton cBtn = lstButtons[intCounter];
                    if (ptLocation.Y + cBtn.Area.Size.Height > recArea.Bottom)
                    {
                        ptLocation.X += intColumnWidtdh + intBorder;
                        ptLocation.Y = ptCornerColumns.Y;
                    }
                    cBtn.Location = ptLocation;
                    ptLocation.Y = cBtn.Area.Location.Y + cBtn.Area.Size.Height + 2;
                }
            }
        }


        void placeButtons_Column()
        {
            Point ptLocation = new Point(intBorder, intBorder);

            int intColumnWidtdh = 0;
            for (int intCounter = 0; intCounter < cMap.lstElements.Count; intCounter++)
            {
                classMultiButtonPic.classButton cBtn = (classMultiButtonPic.classButton)cMap.lstElements[intCounter].obj;
                if (cBtn.Area.Width > intColumnWidtdh)
                    intColumnWidtdh = cBtn.Area.Size.Width;
            }

            for (int intCounter = 0; intCounter < cMap.lstElements.Count; intCounter++)
            {
                classSweepAndPrune.classElement cSPEle = cMap.lstElements[intCounter];
                classButton cBtn = (classButton)cSPEle.obj;
                cBtn.Location = ptLocation;
                ptLocation.Y = cBtn.Area.Location.Y + cBtn.Area.Size.Height+ 2;
                if (ptLocation.Y + cBtn.Area.Size.Height > Height)
                {
                    ptLocation.X += intColumnWidtdh + intBorder;
                    ptLocation.Y = intBorder;
                }
            }
        }

        void placeButtons_Contiguous()
        {
            int intBottomNextLine = 0;
            Point ptLocation = new Point(intBorder, intBorder);
            for (int intCounter = 0; intCounter < cMap.lstElements.Count; intCounter++)
            {
                classButton cBtn = (classButton)cMap.lstElements[intCounter].obj;

                if (ptLocation.X + cBtn.Area.Width + intBorder > Width)
                    ptLocation = new Point(intBorder, intBottomNextLine);
                cBtn.Location = ptLocation;
                ptLocation.X = cBtn.Area.Right + intBorder;
                if (cBtn.Area.Bottom > intBottomNextLine)
                    intBottomNextLine = cBtn.Area.Bottom +2;
            }
        }
        
        public  void placeButtons_Contiguous(ref List<classButton> lstButtons, Rectangle recArea)
        {
            int intBottomNextLine = 0;
            Point ptLocation = recArea.Location; //new Point(intBorder, intBorder);
            for (int intCounter = 0; intCounter < lstButtons.Count; intCounter++)
            {
                classButton cBtn = lstButtons[intCounter];

                if (ptLocation.X + cBtn.Area.Width + intBorder > recArea.Width)
                    ptLocation = new Point(intBorder, intBottomNextLine);
                cBtn.Location = ptLocation;
                ptLocation.X = cBtn.Area.Right + intBorder;
                if (cBtn.Area.Bottom > intBottomNextLine)
                    intBottomNextLine = cBtn.Area.Bottom;
            }
        }

        public class classButton
        {
            classMultiButtonPic cMBP = null;
            public object obj = null;
            public object Tag = null;

            static int intIDCounter = 0;
            int intID = intIDCounter++;
            public int ID
            {
                get { return intID; }
            }
            public enum enuBackgroundStyle { normal, BlendHighlight, BlendIdle, Blend, AlwaysIdle, _num };
            enuBackgroundStyle eBackgroundStyle = enuBackgroundStyle.normal;
            public enuBackgroundStyle BackgroundStyle
            {
                get { return eBackgroundStyle; }
                set
                {
                    if (value != eBackgroundStyle)
                    {
                        eBackgroundStyle = value;
                        bmpHighlight
                            = bmpIdle
                            = null;
                    }
                }
            }
            int intBorderSpace = 3;
            bool bolDrawBorder = false;
            public bool DrawBorder
            {
                get { return bolDrawBorder; }
                set
                {
                    if (bolDrawBorder != value)
                        bolDrawBorder = value;
                }
            }

            Pen pBorder = Pens.Black;
            public Color BorderColor
            {
                get { return pBorder.Color; }
                set
                {
                    if (pBorder.Color != value)
                        pBorder = new Pen(value);
                }
            }


            public classSweepAndPrune.classElement cEle = null;

            public classButton(ref classMultiButtonPic _cMBP)
            {
                cMBP = _cMBP;
            }

            int intIndex = -1;
            public int Index
            {
                get{return intIndex;}
                set { intIndex = value; }
            }

            #region Text
            string strText = "Button";
            public string Text
            {
                get { return strText; }
                set
                {
                    if (strText != value)
                    {
                        strText = value;
                        if (AutoSize)
                            SetSize();
                        ResetBitmaps();
                    }
                }
            }

            Font fnt = new Font("sans serif", 8);
            public Font Font
            {
                get { return fnt; }
                set
                {
                    if (fnt != value)
                    {
                        fnt = value;
                        if (AutoSize)
                            SetSize();
                        ResetBitmaps();
                    }
                }
            }

            Color _foreColor_Idle = Color.Black;
            public Color Forecolor_Idle
            {
                get { return _foreColor_Idle; }
                set
                {
                    if (_foreColor_Idle != value)
                    {
                        _foreColor_Idle = value;
                    }
                }
            }

            Color _BackColor_Idle = Color.Black;
            public Color Backcolor_Idle
            {
                get { return _BackColor_Idle; }
                set
                {
                    if (_BackColor_Idle != value)
                    {
                        _BackColor_Idle = value;
                    }
                }
            }

            Color _foreColor_Highlight = Color.Black;
            public Color Forecolor_Highlight
            {
                get { return _foreColor_Highlight; }
                set
                {
                    if (_foreColor_Highlight != value)
                    {
                        _foreColor_Highlight = value;
                    }
                }
            }

            Color _BackColor_Highlight = Color.Black;
            public Color Backcolor_Highlight
            {
                get { return _BackColor_Highlight; }
                set
                {
                    if (_BackColor_Highlight != value)
                    {
                        _BackColor_Highlight = value;
                    }
                }
            }

            #endregion

            bool bolHighlight = false;
            public bool Highlight
            {
                get { return bolHighlight; }
                set
                {
                    if (bolHighlight != value)
                        bolHighlight = value;
                }
            }

            #region Toggle
            bool bolCanBeToggled = false;
            public bool CanBeToggled
            {
                get { return bolCanBeToggled; }
                set { bolCanBeToggled = value; }
            }

            bool bolToggleValue = false;
            bool ToggleValue
            {
                get { return bolToggleValue; }
                set
                {
                    if (bolToggleValue != value)
                    {
                        bolToggleValue = value;
                        ResetBitmaps();
                    }
                }
            }

            public bool Toggled
            {
                get { return ToggleValue; }
                set { ToggleValue = value; }
            }

            public void Toggle()
            {
                if (CanBeToggled)
                    ToggleValue = !ToggleValue;
                else
                    ToggleValue = false;
                Highlight = ToggleValue;

            }
            #endregion

            bool bolAutoSize = false;
            public bool AutoSize
            {
                get { return bolAutoSize; }
                set
                {
                    if (bolAutoSize != value)
                    {
                        bolAutoSize = value;
                        ResetBitmaps();
                    }
                }
            }

            #region Area
            public Rectangle Area = new Rectangle(0, 0, 10, 10);
       
            public Point Location
            {
                get { return Area.Location; }
                set
                {
                    if (cEle != null)
                        cEle.rec.Location = value;

                    Area.Location = value;
                    if (eBackgroundStyle != enuBackgroundStyle.normal)
                        ResetBitmaps();
                }
            }

            void SetSize()
            {
                if (!AutoSize) return;

                Size szNew = TextRenderer.MeasureText(Text, fnt);
                szNew.Width -= 2;
                if (bolDrawBorder)
                    szNew = new Size(szNew.Width   + 2 * intBorderSpace, szNew.Height + 2 * intBorderSpace);

                Area = new Rectangle(Location, szNew);

                if (Area.Width < 2)
                    Area.Width = 2;
                if (Area.Height < 2)
                    Area.Height = 2;

                if (cEle != null)
                    cEle.rec.Size = szNew;
            }

            public Size Size
            {
                get { return Area.Size; }
                set
                {
                    if (!AutoSize)
                    {
                        if (Area.Size != value)
                        {
                            Area.Size = value;

                            if (cEle != null)
                                cEle.rec.Size = Area.Size;

                            ResetBitmaps();
                        }
                    }
                }
            }
            #endregion

            #region Bitmaps
            Bitmap bmpBackground = null;
            public Bitmap BackgroundImage
            {
                get { return bmpBackground; }
                set { bmpBackground = value; }
            }

            Bitmap bmpHighlight = null;
            public Bitmap Image_Highlight
            {
                get
                {
                    if (bmpHighlight == null) Draw_Image_Highlight();
                    return bmpHighlight;
                }
            }

            void Draw_Image_Highlight()
            {
                SetSize();

                if (Area.Width == 0 || Area.Height == 0) return;

                switch (eBackgroundStyle)
                {
                    case enuBackgroundStyle.AlwaysIdle:
                        {
                            Bitmap bmpTemp = Image_Idle;
                            bmpHighlight = new Bitmap(Image_Idle);
                        }
                        return;

                    case enuBackgroundStyle.normal:
                    case enuBackgroundStyle.BlendIdle:
                        {
                            BackgroundImage = null;
                        }
                        break;

                    case enuBackgroundStyle.BlendHighlight:
                    case enuBackgroundStyle.Blend:
                        {
                            if (cMBP.BackgroundImage != null)
                            {
                                bmpBackground = (Bitmap)(new Bitmap(Area.Width, Area.Height));
                                using (Graphics g = Graphics.FromImage(bmpBackground))
                                {
                                    Rectangle recSource = Area;
                                    Rectangle recDestination = new Rectangle(0, 0, Area.Width, Area.Height);
                                    g.DrawImage(cMBP.BackgroundImage, recDestination, recSource, GraphicsUnit.Pixel);
                                }
                            }
                        }
                        break;
                }
                bmpHighlight = new Bitmap(Area.Width, Area.Height);
                using (Graphics g = Graphics.FromImage(bmpHighlight))
                {

                    if (BackgroundImage != null)
                        g.DrawImage(BackgroundImage, new PointF());
                    else
                        g.FillRectangle(new SolidBrush(Backcolor_Highlight), new Rectangle(0, 0, Area.Size.Width, Area.Size.Height));

                    g.DrawString(Text, fnt, new SolidBrush(Forecolor_Highlight), new Point(bolDrawBorder ? intBorderSpace : 0, bolDrawBorder ? intBorderSpace : 0));
                    if (bolDrawBorder)
                        g.DrawRectangle(pBorder, new Rectangle(0, 0, Area.Width - 1, Area.Height - 1));
                }
            }

            Bitmap bmpIdle = null;
            public Bitmap Image_Idle
            {
                get
                {
                    if (bmpIdle == null) Draw_Image_Idle();
                    return bmpIdle;
                }
            }

            void Draw_Image_Idle()
            {
                /*       SetSize();
                       bmpIdle = new Bitmap(rec.Width, rec.Height);
                       using (Graphics g = Graphics.FromImage(bmpIdle))
                       {
                           if (BackgroundImage != null)
                               g.DrawImage(BackgroundImage, new PointF());
                           else
                               g.FillRectangle(new SolidBrush(Backcolor_Idle), new Rectangle(0, 0, rec.Size.Width, rec.Size.Height));
                           g.DrawString(Text, fnt, new SolidBrush(Forecolor_Idle), new Point());
                       }*/
                if (Text.Length == 0)
                {

                    //return;
                }
                SetSize();

                switch (eBackgroundStyle)
                {
                    case enuBackgroundStyle.normal:
                    case enuBackgroundStyle.BlendHighlight:
                        {
                            BackgroundImage = null;
                        }
                        break;

                    case enuBackgroundStyle.BlendIdle:
                    case enuBackgroundStyle.Blend:
                        {
                            if (cMBP.BackgroundImage != null)
                            {
                                bmpBackground = (Bitmap)(new Bitmap(Area.Width, Area.Height));
                                using (Graphics g = Graphics.FromImage(bmpBackground))
                                {
                                    Rectangle recSource = Area;
                                    Rectangle recDestination = new Rectangle(0, 0, Area.Width, Area.Height);
                                    g.DrawImage(cMBP.BackgroundImage, recDestination, recSource, GraphicsUnit.Pixel);
                                }
                            }
                        }
                        break;
                }
                    
                if (Area.Width >2 && Area.Height >2)
                {
                    bmpIdle = new Bitmap(Area.Width, Area.Height);
                    using (Graphics g = Graphics.FromImage(bmpIdle))
                    {

                        if (BackgroundImage != null)
                            g.DrawImage(BackgroundImage, new PointF());
                        else
                            g.FillRectangle(new SolidBrush(Backcolor_Idle), new Rectangle(0, 0, Area.Size.Width, Area.Size.Height));
                        //g.DrawString(Text, fnt, new SolidBrush(Forecolor_Idle), new Point());

                        g.DrawString(Text, fnt, new SolidBrush(Forecolor_Idle), new Point(bolDrawBorder ? intBorderSpace : 0, bolDrawBorder ? intBorderSpace : 0));
                        if (bolDrawBorder)
                            g.DrawRectangle(pBorder, new Rectangle(0, 0, Area.Width - 1, Area.Height - 1));
                    }
                }
            }

            void ResetBitmaps()
            {
                ResetBitmapHighlight();
                ResetBitmapIdle();
            }

            void ResetBitmapIdle() { bmpIdle = null; }
            void ResetBitmapHighlight() { bmpHighlight = null; }

            #endregion

            public classButton Copy()
            {
                classButton cRetVal = new classButton(ref cMBP);
                cRetVal.Area = Area;
                cRetVal.AutoSize = AutoSize;
                cRetVal.Backcolor_Highlight = Backcolor_Highlight;
                cRetVal.Backcolor_Idle = Backcolor_Idle;
                cRetVal.Forecolor_Highlight = Forecolor_Highlight;
                cRetVal.Forecolor_Idle = Forecolor_Idle;
                cRetVal.Text = Text;
                cRetVal.Font = Font;
                cRetVal.BackgroundImage = BackgroundImage;
                cRetVal.CanBeToggled = CanBeToggled;
                cRetVal.ToggleValue = ToggleValue;
                return cRetVal;
            }
        }
    }

    public class progressbar : PictureBox
    {
        int _intMinimum = 0;
        public int Minimum
        {
            get { return _intMinimum; }
            set { _intMinimum = value; }
        }
        
        int _intMaximum = 100;
        public int Maximum
        {
            get { return _intMaximum; }
            set { _intMaximum = value; }
        }

        int _intValue = 0;
        public int Value
        {
            get { return _intValue; }
            set 
            {
                _intValue = value;
                Draw();
            }
        }

        string strHeading = "";
        public string Heading
        {
            get { return strHeading; }
            set { strHeading = value; }
        }


        public void Increment()
        {
            Value++;
        }

        Bitmap _bmpBackground = null;
        public Bitmap bmpBackground
        {
            get 
            { 
                if (_bmpBackground == null)
                {
                    Size szText = TextRenderer.MeasureText(Heading, Font);
                    _bmpBackground = new Bitmap(Width, Height);
                    using (Graphics g = Graphics.FromImage(_bmpBackground))
                    {
                        g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));
                        g.DrawString(Heading, Font, new SolidBrush(ForeColor), new Point(0, (Height - szText.Height) / 2));
                    }
                }

                return _bmpBackground; 
            }
        }
        Bitmap _bmpForeground = null;
        public Bitmap bmpForeground
        {
            get 
            { 
                if (_bmpForeground == null)
                {
                    Size szText = TextRenderer.MeasureText(Heading, Font);
                    _bmpForeground = new Bitmap(Width, Height);
                    using (Graphics g = Graphics.FromImage(_bmpForeground))
                    {
                        g.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, 0, Width, Height));
                        g.DrawString(Heading, Font, new SolidBrush(BackColor), new Point(0, (Height - szText.Height) / 2));
                    }
                }

                return _bmpForeground; 
            }
        }

        //string strText = "";
        //public string Text
        //{
        //    get { return strText; }
        //    set { strText = value; }
        //}

        public progressbar()
        {
            Height = 17;
            SizeChanged += Progressbar_SizeChanged;
            TextChanged += Progressbar_TextChanged;
        }

        private void Progressbar_TextChanged(object sender, EventArgs e)
        {
            _bmpBackground = null;
            _bmpForeground = null;
        }

        private void Progressbar_SizeChanged(object sender, EventArgs e)
        {
            _bmpBackground = null;
        }

        void Draw()
        {
                _bmpBackground = null;
                _bmpForeground = null;

            float fltPC = (Maximum - Minimum) != 0
                                            ? (float)(Value - Minimum) / (float)(Maximum - Minimum)
                                            : 0;
            Size szForeground = new Size((int)(fltPC * Width), Height);

            Bitmap bmp = new Bitmap(bmpBackground);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle recSource = new Rectangle(0, 0, szForeground.Width, szForeground.Height);
                Rectangle recDest = recSource;
                g.DrawImage(bmpForeground, recDest, recSource, GraphicsUnit.Pixel);
            }
            Image = bmp;
            BringToFront();
            Refresh();
        }
    }

    public class form_GetString : Form
    {
        TextBox txt = new TextBox();
        classLabelButton lbtnOk = new classLabelButton();
        static Font _font = new Font("sans-serif", 10);
        static public Font font
        {
            get { return _font; }
            set
            {
                _font = value;
            }
        }

        public form_GetString(string strHeading, Font fnt, EventHandler EventCompleted)
        {
            Init(strHeading, fnt, EventCompleted);

        }
        public form_GetString(EventHandler EventCompleted) { Init("get String", font, EventCompleted); }

        void Init(string strHeading, Font fnt, EventHandler EventCompleted)
        {
            TopMost = true;
            Text = strHeading;
            Controls.Add(txt);
            txt.Font = fnt;
            txt.Dock = DockStyle.Fill;
            txt.Multiline = true;
            _Event_Completed = EventCompleted;

            txt.Controls.Add(lbtnOk);
            lbtnOk.Text = "Ok";
            lbtnOk.AutoSize = true;
            lbtnOk.BringToFront();
            lbtnOk.Click += LbtnOk_Click;

            SizeChanged += Form_GetString_SizeChanged;
            Activated += Form_GetString_Activated;
        }

        bool bolActivated = false;
        private void Form_GetString_Activated(object sender, EventArgs e)
        {
            if (bolActivated) return;
            bolActivated = true;
            placeObjects();
        }

        private void Form_GetString_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        void placeObjects()
        {
            int intTab = 3;
            lbtnOk.Location = new Point(txt.Width  - lbtnOk.Width - intTab, txt.Height - lbtnOk.Height - intTab);
        }

        EventHandler _Event_Completed = null;

        private void LbtnOk_Click(object sender, EventArgs e)
        {
            Hide();
            _Event_Completed((object)this, new EventArgs());
        }

        public string Text_Input
        {
            get { return txt.Text; }
        }

    }
    public class form_GetInteger : Form
    {
        public int Value
        {
            get { return Convert.ToInt32(txt.Text); }
            set
            {
                txt.Value = value;
                if (_event_ValueChanged != null)
                    _event_ValueChanged((object)this, new EventArgs());
            }
        }

        public EventHandler _event_ValueChanged = null;

        textBox_NumericInput_Integer txt = new textBox_NumericInput_Integer();
        public EventHandler _eventCompleted = null;
        public form_GetInteger(string strText, int _Value)
        {
            Text = strText;
            Value = _Value;

            Controls.Add(txt);
            txt.Text = Value.ToString();
            txt.MouseWheel += Txt_MouseWheel;
            txt.KeyDown += Txt_KeyDown;
            txt.Dock = DockStyle.Fill;

            Size = new Size(200, 50);

            TopMost = true;
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {



            Value = txt.Value;
            if (_eventCompleted != null && e.KeyCode == Keys.Enter)
                _eventCompleted((object)this, new EventArgs());
        }

        private void Txt_MouseWheel(object sender, MouseEventArgs e)
        {
            Value = txt.Value + (e.Delta > 0 ? 1 : -1);
        }
    }

    public class panel_Cartesian_Integer_Input : Panel
    {
        Label lblTitle = new Label();
        Label lblOpenBracket = new Label();
        Label lblComma = new Label();
        Label lblCloseBracket = new Label();

        textBox_NumericInput_Integer txtX = new textBox_NumericInput_Integer();
        textBox_NumericInput_Integer txtY = new textBox_NumericInput_Integer();

        public string Title
        { 
        get { return lblTitle.Text; }
        set 
            {
                lblTitle.Text = value;
                placeObjects();
            }
        }

        public Size Title_Size
        { 
        get { return lblTitle.Size; }
            set 
            {
                lblTitle.Size = value;
                placeObjects();
            }
        }

        public bool Title_AutoSize
        { 
        get { return lblTitle.AutoSize; }
            set 
            {
                lblTitle.AutoSize = value;
                placeObjects();
            }
        }

        int intMinX = int.MinValue;
        public int Min_X
        { 
        get { return intMinX; }
            set { intMinX = value; }
        }
        
        int intMaxX = int.MaxValue;
        public int Max_X
        { 
        get { return intMaxX; }
            set { intMaxX = value; }
        }

        int intX = 0;
        public int X
        {
            get { return intX; }
            set 
            {
                intX = value;
                if (intX < intMinX)
                    intX = intMinX;
                if (intX > intMaxX)
                    intX = intMaxX;
                txtX.Text = intX.ToString();
                txt_FitSize(ref txtX);
                placeObjects();
                valueChanged();
            }
        }


        int intMinY = int.MinValue;
        public int Min_Y
        {
            get { return intMinX; }
            set { intMinY = value; }
        }

        int intMaxY = int.MaxValue;
        public int Max_Y
        {
            get { return intMaxX; }
            set { intMaxY = value; }
        }
        int intY = 0;
        public int Y
        {
            get { return intY; }
            set 
            {
                intY = value;
                if (intY < intMinY)
                    intY = intMinY;
                if (intY > intMaxY)
                    intY = intMaxY;
                txtY.Text = intY.ToString();
                txt_FitSize(ref txtY);
                placeObjects();
                valueChanged();
            }
        }

        public panel_Cartesian_Integer_Input()
        {
            Controls.Add(lblTitle);
            
            Controls.Add(lblOpenBracket);
            lblOpenBracket.Text = "(";
            lblOpenBracket.BackColor = BackColor;
            lblOpenBracket.ForeColor = ForeColor;
            lblOpenBracket.AutoSize = true;

            Controls.Add(lblComma);
            lblComma.Text = ",";
            lblComma.BackColor = BackColor;
            lblComma.ForeColor = ForeColor;
            lblComma.AutoSize = true;

            Controls.Add(lblCloseBracket);
            lblCloseBracket.Text = ")";
            lblCloseBracket.BackColor = BackColor;
            lblCloseBracket.ForeColor = ForeColor;
            lblCloseBracket.AutoSize = true;

            Controls.Add(txtX);
            txtX.Name = "txtX";
            txtX.Text = X.ToString();
            txtX.BorderStyle = BorderStyle.None;
            txtX.BackColor = BackColor;
            txtX.ForeColor = ForeColor;
            txtX.TextChanged += txt_TextChanged;
            txt_FitSize(ref txtX);

            Controls.Add(txtY);
            txtY.Name = "txtY";
            txtY.Text = Y.ToString();
            txtY.BackColor = BackColor;
            txtY.ForeColor = ForeColor;
            txtY.BorderStyle = BorderStyle.None;
            txtY.TextChanged += txt_TextChanged;
            txt_FitSize(ref txtY);

            placeObjects();
        }

        void txt_FitSize(ref textBox_NumericInput_Integer txt)
        {
            Size sz = TextRenderer.MeasureText(txt.Text, txt.Font);
            sz.Width -= 5;
            txt.Size = sz;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txtSender = (TextBox)sender;
            if (txtSender.Text.Length == 0)
                txtSender.Text = "0";
            if (txtSender.Name.Contains('Y'))
                Y = Convert.ToInt32(txtSender.Text);
            else
                X = Convert.ToInt32(txtSender.Text);
        }

        void placeObjects()
        {
            lblTitle.Location = new Point(0, 0);
            lblOpenBracket.Location = new Point(lblTitle.Right + 3, lblTitle.Top);
            txtX.Location = new Point(lblOpenBracket.Right, lblTitle.Top);
            lblComma.Location = new Point(txtX.Right, lblTitle.Top);
            txtY.Location = new Point(lblComma.Right, lblTitle.Top);
            lblCloseBracket.Location = new Point(txtY.Right, lblTitle.Top);
            Size = new Size(lblCloseBracket.Right, lblCloseBracket.Bottom);
        }

        void valueChanged()
        {
            if (_event_ValueChanged != null)
            {
                _event_ValueChanged((object)this, new EventArgs());
            }
        }

        EventHandler _event_ValueChanged = null;
        public EventHandler event_ValueChanged
        { 
        get { return _event_ValueChanged; }
        set { _event_ValueChanged = value; }
        }

    }


    public class textBox_NumericInput_Integer : TextBox
    {
        public textBox_NumericInput_Integer()
        {
            KeyDown += TextBox_NumericInput_KeyDown;
            MouseWheel += TextBox_NumericInput_Integer_MouseWheel;
            MouseDown += TextBox_NumericInput_Integer_MouseDown;
        }

        private void TextBox_NumericInput_Integer_MouseDown(object sender, MouseEventArgs e)
        {
            MouseWheel_Delta_Magnitude = Text.Length - SelectionStart;
        }

        private void TextBox_NumericInput_Integer_MouseWheel(object sender, MouseEventArgs e)
        {
            int intStep = (MouseWheel_Delta_Magnitude >= 0 ? (int)Math.Pow(10, MouseWheel_Delta_Magnitude) : 1);
            Value = Value + (e.Delta > 0 ? intStep : -intStep);
        }
        private void TextBox_NumericInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Right:
                case Keys.Delete:
                case Keys.Home:
                case Keys.End:
                case Keys.OemMinus:
                case Keys.NumLock:
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
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    { }
                    break;

                case Keys.OemPeriod:
                case Keys.Decimal:
                    {
                        e.SuppressKeyPress = !bolAllowDecimal;
                    }
                    break;

                case Keys.Enter:
                    {
                        if (_eventCompleted != null)
                        {
                            _eventCompleted((object)this, new EventArgs());
                            Hide();
                        }
                    }
                    break;

                default:
                    {
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
            MouseWheel_Delta_Magnitude = Text.Length - SelectionStart;
        }

        int intMouseWheel_Delta_Magnitude = 1;
        int MouseWheel_Delta_Magnitude
        {
            get { return intMouseWheel_Delta_Magnitude; }
            set { intMouseWheel_Delta_Magnitude = value; }
        }


        int intMax = int.MaxValue;
        public int Max
        {
            get { return intMax; }
            set 
            {
                intMax = value;
                if (intMax < Value)
                    Text = intMax.ToString();
            }
        }

        int intMin = int.MinValue;
        public int Min
        { 
        get { return intMin; }
        set 
            {
                intMin = value;
                if (intMin > Value)
                    Text = intMin.ToString();
            }
        }

        public EventHandler _eventCompleted = null;
        bool bolAllowDecimal = false;
        public bool AllowDecimal
        {
            get { return bolAllowDecimal; }
            set { bolAllowDecimal = value; }
        }

        public int Value
        {
            get 
            {
                try
                {
                    return Convert.ToInt32(Text);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set 
            {
                int intSelectionStart = SelectionStart;
                Text = value.ToString();
                SelectionStart = intSelectionStart;
            }
        }

     
    }

    public class panelGetSize : Panel
    {
        textBox_NumericInput_Integer txt_Width = new textBox_NumericInput_Integer();
        textBox_NumericInput_Integer txt_Height = new textBox_NumericInput_Integer();
        classLabelButton lbtnOk = new classLabelButton();
        classLabelButton lbtnCancel = new classLabelButton();

        Label lblWidth = new Label();
        Label lblHeight = new Label();

        public panelGetSize()
        {
            Controls.Add(lbtnOk);
            lbtnOk.Text = "Ok";
            lbtnOk.AutoSize = true;

            Controls.Add(lbtnCancel);
            lbtnCancel.Text = "Cancel";
            lbtnCancel.AutoSize = true;

            Controls.Add(txt_Height);
            Controls.Add(txt_Width);

            txt_Height.Min
                = txt_Width.Min
                = 1;

            Controls.Add(lblWidth);
            lblWidth.AutoSize = true;
            lblWidth.Text = "Width";

            Controls.Add(lblHeight);
            lblHeight.AutoSize = true;
            lblHeight.Text = "Height";

            placeObjects();

            lbtnOk.Click += LbtnOk_Click;
            lbtnCancel.Click += LbtnCancel_Click;

            SizeChanged += PanelGetSize_SizeChanged;
        }

        EventHandler _Event_click = null;
        public EventHandler Event_Click
        {
            get { return _Event_click; }
            set { _Event_click = value; }
        }

        private void LbtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void LbtnOk_Click(object sender, EventArgs e)
        {
            if (Event_Click != null)
                Event_Click(sender, e);
            Hide();
        }

        private void PanelGetSize_SizeChanged(object sender, EventArgs e)
        {
            placeObjects();
        }

        void placeObjects()
        {
            int intBorder = 3;
            lblWidth.Location = new Point(intBorder, intBorder);
            txt_Width.Location = new Point(lblWidth.Right, lblWidth.Top);
            txt_Width.Width = Width - txt_Width.Left;

            lblHeight.Location = new Point(intBorder, txt_Width.Bottom + intBorder);
            txt_Height.Location = new Point(lblHeight.Right, lblHeight.Top);
            txt_Height.Width = Width - txt_Height.Width - intBorder;
        }

     
        public int Width_Min
        {
            get { return txt_Width.Min; }
            set{txt_Width.Min = value;}
        }
     
        public int Width_Max
        {
            get { return txt_Width.Max; }
            set{txt_Width.Max = value;}
        }
        
        public int Height_Min
        {
            get { return txt_Height.Min; }
            set{txt_Height.Min = value;}
        }
     
        public int Height_Max
        {
            get { return txt_Height.Max; }
            set{txt_Height.Max = value;}
        }

        public Size size
        { 
        get { return new Size(txt_Width.Value, txt_Height.Value);}
            set 
            {
                txt_Width.Text = value.Width.ToString();
                txt_Height.Text = value.Height.ToString();
            }
        }

    }
    
    
    public partial class formMoveable : Form
    {
        public const string conDefaultMoveableFormName = "defaultMoveableForm";

        /// <summary>
        /// defines the header banner which the user interacts with to control the formMoveable
        /// </summary>
        public class classHeading_Methods
        {
            formMoveable frmMoveable;
            /// <summary>
            /// instantiates the class pointing back to the formMoveable which it controls
            /// </summary>
            /// <param name="MoveablePanel"></param>
            public classHeading_Methods(ref formMoveable MoveablePanel)
            {
                frmMoveable = MoveablePanel;
            }
            /// <summary>
            /// the bottom location of the Heading
            /// </summary>
            public int Bottom
            {
                get { return frmMoveable.picHeading.Bottom; }
            }
            /// <summary>
            ///  sets/gets the back color of the the heading.  
            /// </summary>
            public Color BackColor
            {
                get { return frmMoveable.picHeading.BackColor; }
                set
                {
                    frmMoveable.picHeading.BackColor = value;
                    frmMoveable.picCollapse.BackColor = value;
                    frmMoveable.HeadingImage_Set();
                    frmMoveable.CollapseImage_Set();
                }
            }
            /// <summary>
            /// sets/gets the fore-color of the heading
            /// </summary>
            public Color ForeColor
            {
                get { return frmMoveable.picHeading.ForeColor; }
                set
                {
                    frmMoveable.picHeading.ForeColor = value;
                    frmMoveable.picCollapse.ForeColor = value;
                    frmMoveable.HeadingImage_Set();
                    frmMoveable.CollapseImage_Set();
                }
            }
            /// <summary>
            /// sets/gets the heading font 
            /// </summary>
            public Font Font
            {
                get { return frmMoveable.picHeading.Font; }
                set
                {
                    System.Drawing.Size szOldFont = TextRenderer.MeasureText("Syn.Tax.Error Software", frmMoveable.picCollapse.Font);
                    System.Drawing.Size szNewFont = TextRenderer.MeasureText("Syn.Tax.Error Software", value);
                    int intDifference = (szOldFont.Height - szNewFont.Height);

                    frmMoveable.picHeading.Font = value;
                    frmMoveable.picCollapse.Font = value;
                    if (frmMoveable.Collapsed)
                    {
                        switch (frmMoveable.ExpansionDirection)
                        {
                            case enuExpansionDirection.Up:
                                frmMoveable.Top += intDifference;
                                break;

                            case enuExpansionDirection.Left:
                                frmMoveable.Left += intDifference;
                                break;
                        }
                    }

                    frmMoveable.placeControls();
                    frmMoveable.CollapseImage_Set();
                    frmMoveable.CollapseSize_Set();
                    frmMoveable.HeadingImage_Set();

                    frmMoveable.Size = (frmMoveable.Collapsed)
                                                ? frmMoveable.szCollapsed
                                                : frmMoveable.szExpanded;
                }
            }
            string strText = "";
            /// <summary>
            /// sets/gets the heading text
            /// </summary>
            public string Text
            {
                get { return strText; }
                set
                {
                    strText = value;
                    frmMoveable.HeadingImage_Set();
                }
            }
            /// <summary>
            /// the width of the heading
            /// </summary>
            public int Width
            {
                get { return frmMoveable.picHeading.Width; }
            }
            /// <summary>
            ///  the heading's height
            /// </summary>
            public int Height
            {
                get { return frmMoveable.picHeading.Height; }
            }

            EventHandler _clickEventHandler;
            /// <summary>
            /// calls an eventhandler when the user's mouse clicks the heading
            /// </summary>
            public EventHandler ClickEventHandler
            {
                set { _clickEventHandler = value; }
            }
            /// <summary>
            /// detects a user's mouse click
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void Click(object sender, EventArgs e)
            {
                _clickEventHandler(sender, e);
            }
        }

        #region ObjectDeclarations

        /// <summary>
        ///  holds the heading bitmap and interacts with the user's mouse controls
        /// </summary>
        public PictureBox picHeading = new PictureBox();

        /// <summary>
        /// small picture box which lets the user control the direction of expansion as well as the collapse/expand functions
        /// </summary>
        public PictureBox picCollapse = new PictureBox();

        /// <summary>
        /// docked panel in which all outside objects must be contained to be seen on the screen
        /// </summary>
        public Panel pnlContainer = new Panel();

        #endregion  // objectDeclarations

        #region Global_Variables
        Bitmap bmpHeading = null;
        /// <summary>
        /// subclass used to interact with the user's mouse controls
        /// </summary>
        public static Point ptZero = new Point(0, 0);
        public static Font fntStandard = new Font("ms sans-serif", 18, FontStyle.Regular);
        public classHeading_Methods Heading;
        public static Point ptMouse_Heading = new Point();
        public string strName = "default_MoveablePanelName";
        public classInitSettings cInitSettings;
        ContextMenu cmu_Heading;
        Point ptMouse_Grab_Move = new Point();

        Size szExpanded = new Size(200, 350);
        Size szCollapsed = new Size(200, 30);

        bool bolMouseLeft_Down = false;
        bool bolMouseRight_Down = false;
        #endregion  // global variables

        #region Enumerated_Types
        /// <summary>
        /// direction in which the formMoveable collapses and expands
        /// </summary>
        public enum enuExpansionDirection { Up, Right, Down, Left };
        public static enuExpansionDirection getExpansionDirectionFromString(string strExpansionDirection)
        {
            for (int intCounter = 0; intCounter < 4; intCounter++)
            {
                if (string.Compare(strExpansionDirection, ((enuExpansionDirection)intCounter).ToString()) == 0)
                    return (enuExpansionDirection)intCounter;
            }
            return enuExpansionDirection.Down;
        }


        /// <summary>
        /// identifies list of points used to draw the expand/collapse arrows drawn on the picCollapse as part of the header
        /// </summary>
        enum enuCollapseImagePoints { Top, Right, Bottom, Left };


        #endregion // enumerated types

        #region Methods
        bool bolIgnoreHeadingChanges = true;
        public bool IgnoreHeadingChanges
        {
            get { return bolIgnoreHeadingChanges; }
            set { bolIgnoreHeadingChanges = value; }
        }

        enuExpansionDirection _eExpansionDirection = enuExpansionDirection.Down;
        /// <summary>
        /// method controlling the direction in which the formMoveable expands and collapses
        /// </summary>
        public enuExpansionDirection ExpansionDirection
        {
            get { return _eExpansionDirection; }
            set
            {
                _eExpansionDirection = value;
                CollapseSize_Set();
                placeControls();
                HeadingImage_Set();
                CollapseImage_Set();
                Size = Collapsed ? szCollapsed : szExpanded;
                Collapse();
                MoveOntoForm();
            }
        }

        bool _bolCollapsed = false;
        /// <summary>
        /// gets/sets the collapsed/expanded state of the formMoveable
        /// </summary>
        public bool Collapsed
        {
            get { return _bolCollapsed; }
            set
            {
                if (_bolCollapsed != value)
                {
                    _bolCollapsed = value;
                    if (_bolCollapsed)
                    {
                        Size = szCollapsed;
                        switch (ExpansionDirection)
                        {
                            case enuExpansionDirection.Up:
                                Top += (szExpanded.Height - picHeading.Height);
                                break;

                            case enuExpansionDirection.Down:
                                break;

                            case enuExpansionDirection.Left:
                                Left += (szExpanded.Width - picHeading.Width);
                                break;

                            case enuExpansionDirection.Right:
                                break;
                        }
                        //    placeControls();
                    }
                    else
                    {
                        Size = szExpanded;
                        switch (ExpansionDirection)
                        {
                            case enuExpansionDirection.Down:
                                break;

                            case enuExpansionDirection.Up:
                                Top -= (szExpanded.Height - picHeading.Height);
                                break;

                            case enuExpansionDirection.Left:
                                Left -= (szExpanded.Width - picHeading.Width);
                                break;

                            case enuExpansionDirection.Right:
                                break;
                        }
                        BringToFront();
                    }
                    CollapseImage_Set();
                    // placeControls();
                    MoveOntoForm();
                }
                placeControls();
            }
        }
        #endregion // methods

        public class classInitSettings
        {

            enum enuField { collapsed, expansionDirection, Location_X, Location_Y, Size_Width, Size_Height, _numFields };

            public static classInitSettings cInitSettings_default = null;
            string strNL = "\r\n";

            public Size szExpanded;
            public Point ptLocation;
            public bool bolCollapsed;
            public Ck_Objects.formMoveable.enuExpansionDirection eExpansionDirection;

            formMoveable frmMoveable;

            classInitSettings()
            { }

            public classInitSettings(ref formMoveable frm_Moveable)
            {
                frmMoveable = frm_Moveable;
                if (cInitSettings_default == null)
                {
                    cInitSettings_default = new classInitSettings();
                    cInitSettings_default.bolCollapsed = false;
                    cInitSettings_default.eExpansionDirection = enuExpansionDirection.Down;
                    cInitSettings_default.ptLocation = new Point(100, 100);
                    cInitSettings_default.szExpanded = new Size(350, 600);
                }
            }

            string getFilenameAndPath()
            {
                string strRetVal = System.IO.Directory.GetCurrentDirectory();
                if (frmMoveable.Name.Length > 0)
                    strRetVal += "\\" + frmMoveable.Name;
                else
                    strRetVal += "\\" + conDefaultMoveableFormName;
                strRetVal += ".txt";
                return strRetVal;
            }

            public void Save()
            {
                string strFilenameAndPath = getFilenameAndPath();
                string strOutput = enuField.collapsed.ToString() + ":" + bolCollapsed.ToString() + strNL
                                 + enuField.expansionDirection.ToString() + ":" + eExpansionDirection.ToString() + strNL
                                 + enuField.Location_X.ToString() + ":" + ptLocation.X.ToString() + strNL
                                 + enuField.Location_Y.ToString() + ":" + ptLocation.Y.ToString() + strNL
                                 + enuField.Size_Height.ToString() + ":" + szExpanded.Height.ToString() + strNL
                                 + enuField.Size_Width.ToString() + ":" + szExpanded.Width.ToString() + strNL;
                System.IO.File.WriteAllText(strFilenameAndPath, strOutput);
            }

            public void Load()
            {
                string strFilenameAndPath = getFilenameAndPath();
                if (System.IO.File.Exists(strFilenameAndPath))
                {
                    string strText = System.IO.File.ReadAllText(strFilenameAndPath);
                    string strLine = "";
                    while (strText.Length > 0)
                    {
                        int intNL = strText.IndexOf(strNL);
                        if (intNL > 0)
                        {
                            strLine = strText.Substring(0, intNL);
                            strText = strText.Substring(strLine.Length + strNL.Length);
                        }
                        int intColon = strLine.IndexOf(':');
                        if (intColon > 0)
                        {
                            string strField = strLine.Substring(0, intColon);
                            enuField eField = getFieldFromString(strField);
                            string strValue = strLine.Substring(intColon + 1);
                            switch (eField)
                            {
                                case enuField.collapsed:
                                    bolCollapsed = (string.Compare(strValue, true.ToString()) == 0);
                                    break;

                                case enuField.expansionDirection:
                                    eExpansionDirection = formMoveable.getExpansionDirectionFromString(strValue);
                                    break;

                                case enuField.Location_X:
                                    try
                                    {
                                        ptLocation.X = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        ptLocation.X = cInitSettings_default.ptLocation.X;
                                    }
                                    break;

                                case enuField.Location_Y:
                                    try
                                    {
                                        ptLocation.Y = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        ptLocation.Y = cInitSettings_default.ptLocation.Y;
                                    }
                                    break;

                                case enuField.Size_Height:
                                    try
                                    {
                                        szExpanded.Height = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        szExpanded.Height = cInitSettings_default.szExpanded.Height;
                                    }
                                    break;

                                case enuField.Size_Width:
                                    try
                                    {
                                        szExpanded.Width = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        szExpanded.Width = cInitSettings_default.szExpanded.Width;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    bolCollapsed = cInitSettings_default.bolCollapsed;
                    eExpansionDirection = cInitSettings_default.eExpansionDirection;
                    ptLocation.X = cInitSettings_default.ptLocation.X;
                    ptLocation.Y = cInitSettings_default.ptLocation.Y;
                    szExpanded.Height = cInitSettings_default.szExpanded.Height;
                    szExpanded.Width = cInitSettings_default.szExpanded.Width;
                }
            }

            enuField getFieldFromString(string strField)
            {
                for (int intCounter = 0; intCounter < (int)enuField._numFields; intCounter++)
                {
                    string strTest = ((enuField)intCounter).ToString();
                    if (string.Compare(strField, strTest) == 0)
                        return (enuField)intCounter;
                }
                return enuField._numFields;
            }
        }

        #region Functions
        /// <summary>
        /// creates an instance of the formMoveable object ready to hold outside objects inside its pnlContainer
        /// </summary>
        public formMoveable()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "formMoveable";
            formMoveable frmRef = this;
            cInitSettings = new classInitSettings(ref frmRef);
            cmuHeading_Init();
            SizeChanged += FormMoveable_SizeChanged;
            LocationChanged += FormMoveable_LocationChanged;
            Disposed += FormMoveable_Disposed;
            Activated += FormMoveable_Activated;
            TextChanged += FormMoveable_TextChanged;

            init(fntStandard);
        }

        void cmuHeading_Init()
        {
            cmu_Heading = new ContextMenu();

            MenuItem mnuNormal = new MenuItem("Normal", mnuNormal_click);
            cmu_Heading.MenuItems.Add(mnuNormal);

            MenuItem mnuMinimize = new MenuItem("Minimize", mnuMinimize_click);
            cmu_Heading.MenuItems.Add(mnuMinimize);

            MenuItem mnuMaximize = new MenuItem("Maximize", mnuMaximize_click);
            cmu_Heading.MenuItems.Add(mnuMaximize);

            picHeading.ContextMenu = cmu_Heading;
        }

        void mnuNormal_click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        void mnuMinimize_click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        void mnuMaximize_click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void FormMoveable_TextChanged(object sender, EventArgs e)
        {
            Heading.Text = Text;
        }

        bool bolInit = false;
        private void FormMoveable_Activated(object sender, EventArgs e)
        {
            if (bolInit) return;
            Show();

            Expand();
            Size = cInitSettings.szExpanded;

            ExpansionDirection = cInitSettings.eExpansionDirection;
            if (cInitSettings.bolCollapsed)
                Collapse();
            else
                Expand();

            Location = cInitSettings.ptLocation;



            bolInit = true;
        }

        private void FormMoveable_Disposed(object sender, EventArgs e)
        {
            cInitSettings.bolCollapsed = Collapsed;
            cInitSettings.eExpansionDirection = ExpansionDirection;
            cInitSettings.Save();
        }

        private void FormMoveable_LocationChanged(object sender, EventArgs e)
        {
            if (!bolInit) return;

            cInitSettings.ptLocation = new Point(Left, Top);
        }

        private void FormMoveable_SizeChanged(object sender, EventArgs e)
        {
            if (!bolInit) return;
            if (!Collapsed)
                cInitSettings.szExpanded = Size;
        }

        void init(Font fnt)
        {
            FormBorderStyle = FormBorderStyle.None;
            Controls.Add(picHeading);
            formMoveable pnlMyRef = this;
            Heading = new classHeading_Methods(ref pnlMyRef);
            picHeading.Font = fntStandard;
            picCollapse.Font = fntStandard;

            Heading.ForeColor = Color.White;
            Heading.BackColor = Color.Blue;

            Controls.Add(picCollapse);
            picCollapse.MouseDown += picCollapse_MouseDown;
            picCollapse.MouseUp += PicCollapse_MouseUp;
            picCollapse.MouseWheel += PicCollapse_MouseWheel;

            Controls.Add(pnlContainer);

            picHeading.MouseMove += picHeading_MouseMove;
            picHeading.MouseDown += picHeading_MouseDown;
            picHeading.MouseUp += picHeading_MouseUp;
            picHeading.MouseWheel += picHeading_MouseWheel;
            picHeading.BringToFront();

            formMoveable frmRef = this;
            cInitSettings = new classInitSettings(ref frmRef);
            cInitSettings.Load();

            Size = cInitSettings.szExpanded;
            Location = cInitSettings.ptLocation;
            ExpansionDirection = cInitSettings.eExpansionDirection;
            if (cInitSettings.bolCollapsed)
                Collapse();
            else
                Expand();

            SizeChanged += formMoveable_SizeChanged;
            IgnoreHeadingChanges = false;
            BringToFront();
        }

        /// <summary>
        /// draws the Heading onto bmpHeading and moves it to PicHeading.Image
        /// </summary>
        public void HeadingImage_Set()
        {
            if (IgnoreHeadingChanges) return;

            Size szText = TextRenderer.MeasureText("Testing", Heading.Font);

            Bitmap bmpTemp;

            if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
            { // Heading is written horizontally
                bmpTemp = new Bitmap(szExpanded.Width - szText.Height, szText.Height);
            }
            else
            { // Heading is written vertically
                bmpTemp = new Bitmap(szExpanded.Height - szText.Height, szText.Height);
            }

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(new SolidBrush(Heading.BackColor), new Rectangle(ptZero, bmpTemp.Size));
                g.DrawString(Heading.Text, Heading.Font, new SolidBrush(Heading.ForeColor), ptZero);
            }

            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Left:
                case enuExpansionDirection.Right:
                    bmpTemp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                default:
                    break;

            }
            bmpHeading = (Bitmap)new Bitmap(bmpTemp);
            picHeading.Image = bmpHeading;
        }

        /// <summary>
        /// positions all objects that make up the formMoveable, including the pnlContainer into which all the user's outside objects are located.  
        /// May result in change of size of the pnlContainer which must be detected and handled outside this class
        /// </summary>
        void placeControls()
        {
            Size szText = TextRenderer.MeasureText("Testing", Heading.Font);
            if (Heading == null) return;

            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Down:
                    picCollapse.Height = szText.Height;
                    picCollapse.Width = szText.Height / 2;
                    picHeading.Height = picCollapse.Height;
                    picHeading.Width = szExpanded.Width - picCollapse.Width;

                    picHeading.Left = 0;
                    picHeading.Top = 0;
                    picCollapse.Left = picHeading.Right;
                    picCollapse.Top = picHeading.Top;

                    pnlContainer.Left = 0;
                    pnlContainer.Top = picHeading.Bottom;

                    pnlContainer.Width = szExpanded.Width;
                    pnlContainer.Height = szExpanded.Height - picHeading.Height;
                    break;

                case enuExpansionDirection.Up:
                    picCollapse.Height = szText.Height;
                    picCollapse.Width = szText.Height / 2;
                    picHeading.Height = picCollapse.Height;
                    picHeading.Width = szExpanded.Width - picCollapse.Width;

                    if (Collapsed)
                    {
                        picHeading.Top = 0;
                        picHeading.Left = 0;
                        picCollapse.Left = picHeading.Right;
                        picCollapse.Top = picHeading.Top;

                        pnlContainer.Top = picHeading.Bottom;
                        pnlContainer.Left = 0;
                        pnlContainer.Width = szExpanded.Width;
                        pnlContainer.Height = szExpanded.Height - Heading.Height;
                    }
                    else
                    {
                        pnlContainer.Top = 0;
                        pnlContainer.Left = 0;
                        pnlContainer.Width = szExpanded.Width;
                        pnlContainer.Height = szExpanded.Height - Heading.Height;

                        picHeading.Top = pnlContainer.Bottom;
                        picHeading.Left = 0;
                        picCollapse.Left = picHeading.Right;
                        picCollapse.Top = picHeading.Top;
                    }
                    break;

                case enuExpansionDirection.Left:
                    picCollapse.Width = szText.Height;
                    picCollapse.Height = szText.Height / 2;
                    picHeading.Width = picCollapse.Width;
                    picHeading.Height = szExpanded.Height - picCollapse.Height;

                    pnlContainer.Width = szExpanded.Width - picHeading.Width;
                    pnlContainer.Height = szExpanded.Height;

                    if (Collapsed)
                    {
                        picHeading.Top = 0;
                        picHeading.Left = 0;
                        picCollapse.Top = picHeading.Bottom;
                        picCollapse.Left = picHeading.Left;

                        pnlContainer.Top = 0;
                        pnlContainer.Left = picHeading.Right;
                    }
                    else
                    {
                        pnlContainer.Top = 0;
                        pnlContainer.Left = 0;

                        picHeading.Top = 0;
                        picHeading.Left = pnlContainer.Right;
                        picCollapse.Left = picHeading.Left;
                        picCollapse.Top = picHeading.Bottom;
                    }
                    break;

                case enuExpansionDirection.Right:
                    picCollapse.Width = szText.Height;
                    picCollapse.Height = szText.Height / 2;
                    picHeading.Width = picCollapse.Width;
                    picHeading.Height = szExpanded.Height - picCollapse.Height;

                    pnlContainer.Width = szExpanded.Width - picCollapse.Width;
                    pnlContainer.Height = szExpanded.Height;

                    picHeading.Top = 0;
                    picHeading.Left = 0;
                    picCollapse.Top = picHeading.Bottom;
                    picCollapse.Left = picHeading.Left;

                    pnlContainer.Left = picHeading.Right;
                    pnlContainer.Top = 0;

                    break;

            }
        }

        /// <summary>
        /// collapses a currently expanded formMoveable
        /// </summary>
        public void Collapse() { Collapsed = true; }

        /// <summary>
        /// expands a currently collapsed formMoveable
        /// </summary>
        public void Expand()
        {
            Collapsed = false;
        }

        /// <summary>
        /// draws the Costume of the Collapse/Expand picBox onto a temporary bitmap and stores it into the picCollapse.image
        /// output image displays collapse/expand and expansiondirection information for the user
        /// </summary>
        void CollapseImage_Set()
        {
            Size szText = TextRenderer.MeasureText("testing", Heading.Font);
            Size szArrowPoints = new Size(szText.Height / 4, szText.Height / 2);

            Bitmap bmpTemp = new Bitmap(szText.Height / 2, szText.Height);

            Point[] ptArrow = { new Point(szArrowPoints.Width, 0),
                                new Point(2 * szArrowPoints.Width , szArrowPoints.Height ),
                                new Point(szArrowPoints.Width , 2 * szArrowPoints.Height),
                                new Point(0, szArrowPoints.Height)};

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(new SolidBrush(Heading.BackColor), new Rectangle(ptZero, new Size(bmpTemp.Width, bmpTemp.Height)));
                enuExpansionDirection eDrawArrow = enuExpansionDirection.Up;

                switch (ExpansionDirection)
                {
                    case enuExpansionDirection.Right:
                    case enuExpansionDirection.Up:
                        if (Collapsed)
                            eDrawArrow = enuExpansionDirection.Up;
                        else
                            eDrawArrow = enuExpansionDirection.Down;
                        break;

                    case enuExpansionDirection.Left:
                    case enuExpansionDirection.Down:
                        if (Collapsed)
                            eDrawArrow = enuExpansionDirection.Down;
                        else
                            eDrawArrow = enuExpansionDirection.Up;
                        break;
                }
                Point[] ptDrawArrow = new Point[4];

                switch (eDrawArrow)
                {
                    case enuExpansionDirection.Up:
                        ptDrawArrow[0] = ptArrow[(int)enuCollapseImagePoints.Top];
                        ptDrawArrow[1] = ptArrow[(int)enuCollapseImagePoints.Right];
                        ptDrawArrow[2] = ptArrow[(int)enuCollapseImagePoints.Left];
                        ptDrawArrow[3] = ptArrow[(int)enuCollapseImagePoints.Top];
                        break;

                    case enuExpansionDirection.Down:
                        ptDrawArrow[0] = ptArrow[(int)enuCollapseImagePoints.Bottom];
                        ptDrawArrow[1] = ptArrow[(int)enuCollapseImagePoints.Right];
                        ptDrawArrow[2] = ptArrow[(int)enuCollapseImagePoints.Left];
                        ptDrawArrow[3] = ptArrow[(int)enuCollapseImagePoints.Bottom];
                        break;
                }
                g.DrawPolygon(new Pen(Heading.ForeColor), ptDrawArrow);
            }
            if (ExpansionDirection == enuExpansionDirection.Left || ExpansionDirection == enuExpansionDirection.Right)
                bmpTemp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            picCollapse.Image = bmpTemp;
        }

        /// <summary>
        /// sets the size of the formMoveable when it is collapsed depending on its expansion-direction
        /// </summary>
        void CollapseSize_Set()
        {
            Size szText = TextRenderer.MeasureText("testing", Heading.Font);
            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Down:
                case enuExpansionDirection.Up:
                    szCollapsed.Width = szExpanded.Width;
                    szCollapsed.Height = szText.Height;
                    break;

                case enuExpansionDirection.Left:
                case enuExpansionDirection.Right:
                    szCollapsed.Width = szText.Height;
                    szCollapsed.Height = szExpanded.Height;
                    break;
            }
        }

        /// <summary>
        /// ensures this instance of the formMoveable is not moved outside the frame of its container
        /// </summary>
        void MoveOntoForm()
        {
            bool bolChangesMade = false;
            do
            {
                bolChangesMade = false;
                if (Top < 0)
                {
                    if (Height >= Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        Height = szExpanded.Height = Screen.PrimaryScreen.WorkingArea.Height - 5;
                        Top = 2;
                    }
                    else
                    {
                        Top = 2;
                    }
                    ptMouse_Grab_Move.Y -= 1;
                    bolChangesMade = true;
                }
                else if (Bottom > Screen.PrimaryScreen.WorkingArea.Bottom)
                {
                    if (Height >= Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        Height = szCollapsed.Height = Screen.PrimaryScreen.WorkingArea.Height - 5;
                        Top = 2;
                    }
                    else
                    {
                        Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 2;
                    }
                    ptMouse_Grab_Move.Y += 1;
                    bolChangesMade = true;
                }

                if (Left < 0)
                {
                    if (Width >= Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        Width = szExpanded.Width = Screen.PrimaryScreen.WorkingArea.Width - 5;
                        Left = 2;
                    }
                    else
                    {
                        Left = 2;
                    }
                    ptMouse_Grab_Move.X -= 1;
                    bolChangesMade = true;

                }
                else if (Right > Screen.PrimaryScreen.WorkingArea.Right)
                {
                    if (Width >= Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        Width = szCollapsed.Width = Screen.PrimaryScreen.WorkingArea.Width - 5;
                        Left = 2;
                    }
                    else
                    {
                        Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 2;
                    }
                    ptMouse_Grab_Move.X += 1;
                    bolChangesMade = true;
                }
            } while (bolChangesMade
                    && WindowState != FormWindowState.Maximized
                    && WindowState != FormWindowState.Minimized);
        }
        #endregion

        #region Events

        #region Events_Mouse
        /// <summary>
        /// lets the user rotate the orientation of the ExpansionDirection with the right-mouse button & mouse-wheel controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicCollapse_MouseWheel(object sender, MouseEventArgs e)
        {
            if (bolMouseRight_Down)
            {
                if (e.Delta > 0)
                    ExpansionDirection = (enuExpansionDirection)(((int)ExpansionDirection + 1) % 4);
                else
                    ExpansionDirection = (enuExpansionDirection)(((int)ExpansionDirection + 3) % 4);
            }
        }

        /// <summary>
        /// detects and stores the right & left mouse button states into bolMouseLeft_Down and bolMouseRight_Down variables shared between both picCollapse & picHeading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicCollapse_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bolMouseLeft_Down = false;
            }
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = false;
        }

        /// <summary>
        /// detects and stores the right/left mouse button states in bolMouseLeft_Down & bolMouseRight_Down variables.
        /// left-mouse button causes the formMoveable to expand/collapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picCollapse_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                bolMouseLeft_Down = true;
                bool bolVisible = Visible;
                Hide();
                Collapsed = !Collapsed;
                Visible = bolVisible;
            }
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = true;
        }

        /// <summary>
        /// detects changes in the mouseWheel and allows the user to increase/decrease the expanded panel's height/width using the left or right Mouse-buttons in combination with the mouse-wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseWheel(object sender, MouseEventArgs e)
        {
            int intDeltaSize = 5;
            if (Collapsed) return;

            if (bolMouseLeft_Down)
            {
                if (e.Delta > 0)
                {
                    Width += 2 * intDeltaSize;
                    if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
                        ptMouse_Grab_Move.X += intDeltaSize;
                }
                else
                {
                    if (Width > 50)
                    {
                        Width -= 2 * intDeltaSize;
                        if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
                            ptMouse_Grab_Move.X -= intDeltaSize;
                    }
                }
            }
            if (bolMouseRight_Down)
            {
                if (e.Delta > 0)
                {
                    switch (ExpansionDirection)
                    {
                        case enuExpansionDirection.Left:
                        case enuExpansionDirection.Up:
                            Height += 2 * intDeltaSize;
                            ptMouse_Grab_Move.Y += intDeltaSize;
                            Top -= 2 * intDeltaSize;
                            break;

                        case enuExpansionDirection.Right:
                        case enuExpansionDirection.Down:
                            if (Height > 50)
                            {
                                Height -= 2 * intDeltaSize;
                                ptMouse_Grab_Move.Y += intDeltaSize;
                            }
                            break;
                    }
                }
                else
                {
                    switch (ExpansionDirection)
                    {
                        case enuExpansionDirection.Left:
                        case enuExpansionDirection.Up:
                            if (Height > 50)
                            {
                                Height -= 2 * intDeltaSize;
                                ptMouse_Grab_Move.Y += intDeltaSize;
                                Top += 2 * intDeltaSize;
                            }
                            break;

                        case enuExpansionDirection.Right:
                        case enuExpansionDirection.Down:
                            Height += 2 * intDeltaSize;
                            ptMouse_Grab_Move.Y -= intDeltaSize;
                            break;

                    }
                }
            }
            MoveOntoForm();
        }

        /// <summary>
        /// detects and stores the mouse button conditions shared between picHeading & picCollapse stored in bolMouseLeft_Down & bolMouseRight_Down boolean variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                bolMouseLeft_Down = false;
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = false;
        }

        /// <summary>
        /// detects and stores the mosue button conditions shared between picHeading & picCollapse stored in bolMouseLeft_Down & bolMouseRight_Down boolean variables
        /// sets the ptMouse_Grab_Move coordinate which positions the formMoveable beneath the mouse cursor so long as the left mouse button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                bolMouseLeft_Down = true;
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = true;
            BringToFront();
            ptMouse_Grab_Move.X = ptMouse_Heading.X;
            ptMouse_Grab_Move.Y = ptMouse_Heading.Y;
        }

        /// <summary>
        /// detects motion of the mouse over the Heading.  Moves the formMoveable beneath the cursor so long as the left-mouse button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseMove(object sender, MouseEventArgs e)
        {
            ptMouse_Heading.X = e.X;
            ptMouse_Heading.Y = e.Y;

            if (bolMouseLeft_Down)
            {
                Top += (ptMouse_Heading.Y - ptMouse_Grab_Move.Y);
                Left += (ptMouse_Heading.X - ptMouse_Grab_Move.X);
            }
            MoveOntoForm();
        }
        #endregion // events_Mouse

        #region Events_Objects
        /// <summary>
        /// sets the size changes of szExpanded variable to be used when formMoveable is expanded then draws the Header/collapse images and places objects inside the resized formMoveable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formMoveable_SizeChanged(object sender, EventArgs e)
        {
            if (!Collapsed)
            {
                szExpanded.Width = Width;
                szExpanded.Height = Height;
            }

            CollapseSize_Set();
            HeadingImage_Set();
            placeControls();
        }
        #endregion // events_objects
        #endregion // events
    }
    public class panelMoveable : Panel
    {
        public static Point ptZero = new Point(0, 0);
        public static Point ptOne = new Point(1, 1);
        public const string conDefaultMoveablePanelName = "defaultMoveablePanel";
        /// <summary>
        /// defines the header banner which the user interacts with to control the panelMoveable
        /// </summary>
        public class classHeading_Methods
        {
            panelMoveable pnlMoveable;
            /// <summary>
            /// instantiates the class pointing back to the panelMoveable which it controls
            /// </summary>
            /// <param name="MoveablePanel"></param>
            public classHeading_Methods(ref panelMoveable MoveablePanel)
            {
                pnlMoveable = MoveablePanel;
            }
            /// <summary>
            /// the bottom location of the Heading
            /// </summary>
            public int Bottom
            {
                get { return pnlMoveable.picHeading.Bottom; }
            }
            /// <summary>
            ///  sets/gets the back color of the the heading.  
            /// </summary>
            public Color BackColor
            {
                get { return pnlMoveable.picHeading.BackColor; }
                set
                {
                    pnlMoveable.picHeading.BackColor = value;
                    pnlMoveable.picCollapse.BackColor = value;
                    pnlMoveable.HeadingImage_Set();
                    pnlMoveable.CollapseImage_Set();
                }
            }
            /// <summary>
            /// sets/gets the fore-color of the heading
            /// </summary>
            public Color ForeColor
            {
                get { return pnlMoveable.picHeading.ForeColor; }
                set
                {
                    pnlMoveable.picHeading.ForeColor = value;
                    pnlMoveable.picCollapse.ForeColor = value;
                    pnlMoveable.HeadingImage_Set();
                    pnlMoveable.CollapseImage_Set();
                }
            }
            /// <summary>
            /// sets/gets the heading font 
            /// </summary>
            public Font Font
            {
                get { return pnlMoveable.picHeading.Font; }
                set
                {
                    System.Drawing.Size szOldFont = TextRenderer.MeasureText("Syn.Tax.Error Software", pnlMoveable.picCollapse.Font);
                    System.Drawing.Size szNewFont = TextRenderer.MeasureText("Syn.Tax.Error Software", value);
                    int intDifference = (szOldFont.Height - szNewFont.Height);

                    pnlMoveable.picHeading.Font = value;
                    pnlMoveable.picCollapse.Font = value;
                    if (pnlMoveable.Collapsed)
                    {
                        switch (pnlMoveable.ExpansionDirection)
                        {
                            case enuExpansionDirection.Up:
                                pnlMoveable.Top += intDifference;
                                break;

                            case enuExpansionDirection.Left:
                                pnlMoveable.Left += intDifference;
                                break;
                        }
                    }


                    pnlMoveable.placeControls();
                    pnlMoveable.CollapseImage_Set();
                    pnlMoveable.CollapseSize_Set();
                    pnlMoveable.HeadingImage_Set();

                    pnlMoveable.Size = (pnlMoveable.Collapsed)
                                                ? pnlMoveable.szCollapsed
                                                : pnlMoveable.szExpanded;
                }
            }
            string strText = "";
            /// <summary>
            /// sets/gets the heading text
            /// </summary>
            public string Text
            {
                get { return strText; }
                set
                {
                    strText = value;
                    pnlMoveable.HeadingImage_Set();
                }
            }
            /// <summary>
            /// the width of the heading
            /// </summary>
            public int Width
            {
                get { return pnlMoveable.picHeading.Width; }
            }
            /// <summary>
            ///  the heading's height
            /// </summary>
            public int Height
            {
                get { return pnlMoveable.picHeading.Height; }
            }

            EventHandler _clickEventHandler;
            /// <summary>
            /// calls an eventhandler when the user's mouse clicks the heading
            /// </summary>
            public EventHandler ClickEventHandler
            {
                set { _clickEventHandler = value; }
            }
            /// <summary>
            /// detects a user's mouse click
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void Click(object sender, EventArgs e)
            {
                _clickEventHandler(sender, e);
            }
        }

        #region ObjectDeclarations

        /// <summary>
        ///  holds the heading bitmap and interacts with the user's mouse controls
        /// </summary>
        public PictureBox picHeading = new PictureBox();

        /// <summary>
        /// small picture box which lets the user control the direction of expansion as well as the collapse/expand functions
        /// </summary>
        public PictureBox picCollapse = new PictureBox();

        /// <summary>
        /// docked panel in which all outside objects must be contained to be seen on the screen
        /// </summary>
        public Panel pnlContainer = new Panel();

        #endregion  // objectDeclarations

        #region Global_Variables
        Bitmap bmpHeading = null;
        /// <summary>
        /// subclass used to interact with the user's mouse controls
        /// </summary>

        public static Font fntStandard = new Font("ms sans-serif", 18, FontStyle.Regular);
        public classHeading_Methods Heading;
        public static Point ptMouse_Heading = new Point();
        public classInitSettings cInitSettings;
        Point ptMouse_Grab_Move = new Point();

        Size szExpanded = new Size(200, 350);
        Size szCollapsed = new Size(200, 30);

        bool bolMouseLeft_Down = false;
        bool bolMouseRight_Down = false;
        bool bolInit = false;
        #endregion  // global variables

        #region Enumerated_Types
        /// <summary>
        /// direction in which the panelMoveable collapses and expands
        /// </summary>
        public enum enuExpansionDirection { Up, Right, Down, Left };
        /// <summary>
        /// direction in which the formMoveable collapses and expands
        /// </summary>
        public static enuExpansionDirection getExpansionDirectionFromString(string strExpansionDirection)
        {
            for (int intCounter = 0; intCounter < 4; intCounter++)
            {
                if (string.Compare(strExpansionDirection, ((enuExpansionDirection)intCounter).ToString()) == 0)
                    return (enuExpansionDirection)intCounter;
            }
            return enuExpansionDirection.Down;
        }

        /// <summary>
        /// identifies list of points used to draw the expand/collapse arrows drawn on the picCollapse as part of the header
        /// </summary>
        enum enuCollapseImagePoints { Top, Right, Bottom, Left };


        #endregion // enumerated types

        #region Methods
        bool bolIgnoreHeadingChanges = true;
        public bool IgnoreHeadingChanges
        {
            get { return bolIgnoreHeadingChanges; }
            set { bolIgnoreHeadingChanges = value; }
        }

        enuExpansionDirection _eExpansionDirection = enuExpansionDirection.Down;
        /// <summary>
        /// method controlling the direction in which the panelMoveable expands and collapses
        /// </summary>
        public enuExpansionDirection ExpansionDirection
        {
            get { return _eExpansionDirection; }
            set
            {
                _eExpansionDirection = value;
                CollapseSize_Set();
                placeControls();
                HeadingImage_Set();
                CollapseImage_Set();
                Size = Collapsed ? szCollapsed : szExpanded;
                Collapse();
                MoveOntoForm();
            }
        }

        bool _bolCollapsed = false;
        /// <summary>
        /// gets/sets the collapsed/expanded state of the panelMoveable
        /// </summary>
        public bool Collapsed
        {
            get { return _bolCollapsed; }
            set
            {
                if (_bolCollapsed != value)
                {
                    _bolCollapsed = value;
                    if (_bolCollapsed)
                    {
                        Size = szCollapsed;
                        switch (ExpansionDirection)
                        {
                            case enuExpansionDirection.Up:
                                Top += (szExpanded.Height - picHeading.Height);
                                break;

                            case enuExpansionDirection.Down:
                                break;

                            case enuExpansionDirection.Left:
                                Left += (szExpanded.Width - picHeading.Width);
                                break;

                            case enuExpansionDirection.Right:
                                break;
                        }
                        //    placeControls();
                    }
                    else
                    {
                        Size = szExpanded;
                        switch (ExpansionDirection)
                        {
                            case enuExpansionDirection.Down:
                                break;

                            case enuExpansionDirection.Up:
                                Top -= (szExpanded.Height - picHeading.Height);
                                break;

                            case enuExpansionDirection.Left:
                                Left -= (szExpanded.Width - picHeading.Width);
                                break;

                            case enuExpansionDirection.Right:
                                break;
                        }
                        BringToFront();
                    }
                    CollapseImage_Set();
                    // placeControls();
                    MoveOntoForm();
                }
                placeControls();
            }
        }
        #endregion // methods

        public class classInitSettings
        {

            enum enuField { collapsed, expansionDirection, Location_X, Location_Y, Size_Width, Size_Height, _numFields };

            public static classInitSettings cInitSettings_default = null;
            string strNL = "\r\n";

            public Size szExpanded;
            public Point ptLocation;
            public bool bolCollapsed;
            public Ck_Objects.panelMoveable.enuExpansionDirection eExpansionDirection;

            panelMoveable pnlMoveable;

            classInitSettings()
            {

            }

            public classInitSettings(ref panelMoveable pnl_Moveable)
            {
                pnlMoveable = pnl_Moveable;
                if (cInitSettings_default == null)
                {
                    cInitSettings_default = new classInitSettings();
                    cInitSettings_default.bolCollapsed = false;
                    cInitSettings_default.eExpansionDirection = enuExpansionDirection.Down;
                    cInitSettings_default.ptLocation = new Point(100, 100);
                    cInitSettings_default.szExpanded = new Size(350, 600);
                }
            }

            string getFilenameAndPath()
            {
                string strRetVal = System.IO.Directory.GetCurrentDirectory();
                if (pnlMoveable.Name.Length > 0)
                    strRetVal += "\\" + pnlMoveable.Name;
                else
                    strRetVal += "\\" + conDefaultMoveablePanelName;
                strRetVal += ".txt";
                return strRetVal;
            }

            public void Save()
            {
                string strFilenameAndPath = getFilenameAndPath();
                string strOutput = enuField.collapsed.ToString() + ":" + bolCollapsed.ToString() + strNL
                                 + enuField.expansionDirection.ToString() + ":" + eExpansionDirection.ToString() + strNL
                                 + enuField.Location_X.ToString() + ":" + ptLocation.X.ToString() + strNL
                                 + enuField.Location_Y.ToString() + ":" + ptLocation.Y.ToString() + strNL
                                 + enuField.Size_Height.ToString() + ":" + szExpanded.Height.ToString() + strNL
                                 + enuField.Size_Width.ToString() + ":" + szExpanded.Width.ToString() + strNL;
                System.IO.File.WriteAllText(strFilenameAndPath, strOutput);
            }

            public void Load()
            {
                string strFilenameAndPath = getFilenameAndPath();
                if (System.IO.File.Exists(strFilenameAndPath))
                {
                    string strText = System.IO.File.ReadAllText(strFilenameAndPath);
                    string strLine = "";
                    while (strText.Length > 0)
                    {
                        int intNL = strText.IndexOf(strNL);
                        if (intNL > 0)
                        {
                            strLine = strText.Substring(0, intNL);
                            strText = strText.Substring(strLine.Length + strNL.Length);
                        }
                        int intColon = strLine.IndexOf(':');
                        if (intColon > 0)
                        {
                            string strField = strLine.Substring(0, intColon);
                            enuField eField = getFieldFromString(strField);
                            string strValue = strLine.Substring(intColon + 1);
                            switch (eField)
                            {
                                case enuField.collapsed:
                                    bolCollapsed = (string.Compare(strValue, true.ToString()) == 0);
                                    break;

                                case enuField.expansionDirection:
                                    eExpansionDirection = panelMoveable.getExpansionDirectionFromString(strValue);
                                    break;

                                case enuField.Location_X:
                                    try
                                    {
                                        ptLocation.X = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        ptLocation.X = cInitSettings_default.ptLocation.X;
                                    }
                                    break;

                                case enuField.Location_Y:
                                    try
                                    {
                                        ptLocation.Y = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        ptLocation.Y = cInitSettings_default.ptLocation.Y;
                                    }
                                    break;

                                case enuField.Size_Height:
                                    try
                                    {
                                        szExpanded.Height = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        szExpanded.Height = cInitSettings_default.szExpanded.Height;
                                    }
                                    break;

                                case enuField.Size_Width:
                                    try
                                    {
                                        szExpanded.Width = Convert.ToInt32(strValue);
                                    }
                                    catch (Exception)
                                    {
                                        szExpanded.Width = cInitSettings_default.szExpanded.Width;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    bolCollapsed = cInitSettings_default.bolCollapsed;
                    eExpansionDirection = cInitSettings_default.eExpansionDirection;
                    ptLocation.X = cInitSettings_default.ptLocation.X;
                    ptLocation.Y = cInitSettings_default.ptLocation.Y;
                    szExpanded.Height = cInitSettings_default.szExpanded.Height;
                    szExpanded.Width = cInitSettings_default.szExpanded.Width;
                }
            }


            enuField getFieldFromString(string strField)
            {
                for (int intCounter = 0; intCounter < (int)enuField._numFields; intCounter++)
                {
                    string strTest = ((enuField)intCounter).ToString();
                    if (string.Compare(strField, strTest) == 0)
                        return (enuField)intCounter;
                }
                return enuField._numFields;
            }
        }
        #region Functions
        /// <summary>
        /// creates an instance of the panelMoveable object ready to hold outside objects inside its pnlContainer
        /// </summary>
        public panelMoveable()
        {
            init(fntStandard);
        }

        public panelMoveable(Font fnt)
        {
            init(fnt);
        }

        void init(Font fnt)
        {
            Controls.Add(picHeading);
            panelMoveable pnlMyRef = this;

            cInitSettings = new classInitSettings(ref pnlMyRef);

            Heading = new classHeading_Methods(ref pnlMyRef);
            picHeading.Font = fntStandard;
            picCollapse.Font = fntStandard;

            Heading.ForeColor = Color.White;
            Heading.BackColor = Color.Blue;

            Controls.Add(picCollapse);
            picCollapse.MouseDown += picCollapse_MouseDown;
            picCollapse.MouseUp += PicCollapse_MouseUp;
            picCollapse.MouseWheel += PicCollapse_MouseWheel;

            Controls.Add(pnlContainer);

            picHeading.MouseMove += picHeading_MouseMove;
            picHeading.MouseDown += picHeading_MouseDown;
            picHeading.MouseUp += picHeading_MouseUp;
            picHeading.MouseWheel += picHeading_MouseWheel;
            picHeading.BringToFront();

            SizeChanged += panelMoveable_SizeChanged;
            LocationChanged += PanelMoveable_LocationChanged;
            Disposed += PanelMoveable_Disposed;
            IgnoreHeadingChanges = false;
            BringToFront();
            bolInit = true;
        }




        /// <summary>
        /// draws the Heading onto bmpHeading and moves it to PicHeading.Image
        /// </summary>
        public void HeadingImage_Set()
        {
            if (IgnoreHeadingChanges) return;

            Size szText = TextRenderer.MeasureText("Testing", Heading.Font);

            Bitmap bmpTemp;

            if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
            { // Heading is written horizontally
                bmpTemp = new Bitmap(szExpanded.Width - szText.Height, szText.Height);
            }
            else
            { // Heading is written vertically
                bmpTemp = new Bitmap(szExpanded.Height - szText.Height, szText.Height);
            }

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(new SolidBrush(Heading.BackColor), new Rectangle(ptZero, bmpTemp.Size));
                g.DrawString(Heading.Text, Heading.Font, new SolidBrush(Heading.ForeColor), ptZero);
            }

            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Left:
                case enuExpansionDirection.Right:
                    bmpTemp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                default:
                    break;

            }
            bmpHeading = (Bitmap)new Bitmap(bmpTemp);
            picHeading.Image = bmpHeading;
        }

        /// <summary>
        /// positions all objects that make up the panelMoveable, including the pnlContainer into which all the user's outside objects are located.  
        /// May result in change of size of the pnlContainer which must be detected and handled outside this class
        /// </summary>
        void placeControls()
        {
            Size szText = TextRenderer.MeasureText("Testing", Heading.Font);
            if (Heading == null) return;

            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Down:
                    picCollapse.Height = szText.Height;
                    picCollapse.Width = szText.Height / 2;
                    picHeading.Height = picCollapse.Height;
                    picHeading.Width = szExpanded.Width - picCollapse.Width;

                    picHeading.Left = 0;
                    picHeading.Top = 0;
                    picCollapse.Left = picHeading.Right;
                    picCollapse.Top = picHeading.Top;

                    pnlContainer.Left = 0;
                    pnlContainer.Top = picHeading.Bottom;

                    pnlContainer.Width = szExpanded.Width;
                    pnlContainer.Height = szExpanded.Height - picHeading.Height;
                    break;

                case enuExpansionDirection.Up:
                    picCollapse.Height = szText.Height;
                    picCollapse.Width = szText.Height / 2;
                    picHeading.Height = picCollapse.Height;
                    picHeading.Width = szExpanded.Width - picCollapse.Width;

                    if (Collapsed)
                    {
                        picHeading.Top = 0;
                        picHeading.Left = 0;
                        picCollapse.Left = picHeading.Right;
                        picCollapse.Top = picHeading.Top;

                        pnlContainer.Top = picHeading.Bottom;
                        pnlContainer.Left = 0;
                        pnlContainer.Width = szExpanded.Width;
                        pnlContainer.Height = szExpanded.Height - Heading.Height;
                    }
                    else
                    {
                        pnlContainer.Top = 0;
                        pnlContainer.Left = 0;
                        pnlContainer.Width = szExpanded.Width;
                        pnlContainer.Height = szExpanded.Height - Heading.Height;

                        picHeading.Top = pnlContainer.Bottom;
                        picHeading.Left = 0;
                        picCollapse.Left = picHeading.Right;
                        picCollapse.Top = picHeading.Top;
                    }
                    break;

                case enuExpansionDirection.Left:
                    picCollapse.Width = szText.Height;
                    picCollapse.Height = szText.Height / 2;
                    picHeading.Width = picCollapse.Width;
                    picHeading.Height = szExpanded.Height - picCollapse.Height;

                    pnlContainer.Width = szExpanded.Width - picHeading.Width;
                    pnlContainer.Height = szExpanded.Height;

                    if (Collapsed)
                    {
                        picHeading.Top = 0;
                        picHeading.Left = 0;
                        picCollapse.Top = picHeading.Bottom;
                        picCollapse.Left = picHeading.Left;

                        pnlContainer.Top = 0;
                        pnlContainer.Left = picHeading.Right;
                    }
                    else
                    {
                        pnlContainer.Top = 0;
                        pnlContainer.Left = 0;

                        picHeading.Top = 0;
                        picHeading.Left = pnlContainer.Right;
                        picCollapse.Left = picHeading.Left;
                        picCollapse.Top = picHeading.Bottom;
                    }
                    break;

                case enuExpansionDirection.Right:
                    picCollapse.Width = szText.Height;
                    picCollapse.Height = szText.Height / 2;
                    picHeading.Width = picCollapse.Width;
                    picHeading.Height = szExpanded.Height - picCollapse.Height;

                    pnlContainer.Width = szExpanded.Width - picCollapse.Width;
                    pnlContainer.Height = szExpanded.Height;

                    picHeading.Top = 0;
                    picHeading.Left = 0;
                    picCollapse.Top = picHeading.Bottom;
                    picCollapse.Left = picHeading.Left;

                    pnlContainer.Left = picHeading.Right;
                    pnlContainer.Top = 0;

                    break;

            }
        }

        /// <summary>
        /// collapses a currently expanded panelMoveable
        /// </summary>
        public void Collapse() { Collapsed = true; }

        /// <summary>
        /// expands a currently collapsed panelMoveable
        /// </summary>
        public void Expand()
        {
            Collapsed = false;
        }

        /// <summary>
        /// draws the Costume of the Collapse/Expand picBox onto a temporary bitmap and stores it into the picCollapse.image
        /// output image displays collapse/expand and expansiondirection information for the user
        /// </summary>
        void CollapseImage_Set()
        {
            Size szText = TextRenderer.MeasureText("testing", Heading.Font);
            Size szArrowPoints = new Size(szText.Height / 4, szText.Height / 2);

            Bitmap bmpTemp = new Bitmap(szText.Height / 2, szText.Height);

            Point[] ptArrow = { new Point(szArrowPoints.Width, 0),
                                new Point(2 * szArrowPoints.Width , szArrowPoints.Height ),
                                new Point(szArrowPoints.Width , 2 * szArrowPoints.Height),
                                new Point(0, szArrowPoints.Height)};

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(new SolidBrush(Heading.BackColor), new Rectangle(ptZero, new Size(bmpTemp.Width, bmpTemp.Height)));
                enuExpansionDirection eDrawArrow = enuExpansionDirection.Up;

                switch (ExpansionDirection)
                {
                    case enuExpansionDirection.Right:
                    case enuExpansionDirection.Up:
                        if (Collapsed)
                            eDrawArrow = enuExpansionDirection.Up;
                        else
                            eDrawArrow = enuExpansionDirection.Down;
                        break;

                    case enuExpansionDirection.Left:
                    case enuExpansionDirection.Down:
                        if (Collapsed)
                            eDrawArrow = enuExpansionDirection.Down;
                        else
                            eDrawArrow = enuExpansionDirection.Up;
                        break;
                }
                Point[] ptDrawArrow = new Point[4];

                switch (eDrawArrow)
                {
                    case enuExpansionDirection.Up:
                        ptDrawArrow[0] = ptArrow[(int)enuCollapseImagePoints.Top];
                        ptDrawArrow[1] = ptArrow[(int)enuCollapseImagePoints.Right];
                        ptDrawArrow[2] = ptArrow[(int)enuCollapseImagePoints.Left];
                        ptDrawArrow[3] = ptArrow[(int)enuCollapseImagePoints.Top];
                        break;

                    case enuExpansionDirection.Down:
                        ptDrawArrow[0] = ptArrow[(int)enuCollapseImagePoints.Bottom];
                        ptDrawArrow[1] = ptArrow[(int)enuCollapseImagePoints.Right];
                        ptDrawArrow[2] = ptArrow[(int)enuCollapseImagePoints.Left];
                        ptDrawArrow[3] = ptArrow[(int)enuCollapseImagePoints.Bottom];
                        break;
                }
                g.DrawPolygon(new Pen(Heading.ForeColor), ptDrawArrow);
            }
            if (ExpansionDirection == enuExpansionDirection.Left || ExpansionDirection == enuExpansionDirection.Right)
                bmpTemp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            picCollapse.Image = bmpTemp;
        }

        /// <summary>
        /// sets the size of the panelMoveable when it is collapsed depending on its expansion-direction
        /// </summary>
        void CollapseSize_Set()
        {
            Size szText = TextRenderer.MeasureText("testing", Heading.Font);
            switch (ExpansionDirection)
            {
                case enuExpansionDirection.Down:
                case enuExpansionDirection.Up:
                    szCollapsed.Width = szExpanded.Width;
                    szCollapsed.Height = szText.Height;
                    break;

                case enuExpansionDirection.Left:
                case enuExpansionDirection.Right:
                    szCollapsed.Width = szText.Height;
                    szCollapsed.Height = szExpanded.Height;
                    break;
            }
        }

        /// <summary>
        /// ensures this instance of the panelMoveable is not moved outside the frame of its container
        /// </summary>
        void MoveOntoForm()
        {
            bool bolChangesMade = false;
            do
            {
                bolChangesMade = false;
                if (Top < 0)
                {
                    if (Height > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        Height = szExpanded.Height = Screen.PrimaryScreen.WorkingArea.Height - 5;
                        Top = 2;
                    }
                    else
                    {
                        Top = 2;
                    }
                    ptMouse_Grab_Move.Y -= 1;
                    bolChangesMade = true;
                }
                else if (Bottom > Screen.PrimaryScreen.WorkingArea.Bottom)
                {
                    if (Height > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        Height = szCollapsed.Height = Screen.PrimaryScreen.WorkingArea.Height - 5;
                        Top = 2;
                    }
                    else
                    {
                        Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 2;
                    }
                    ptMouse_Grab_Move.Y += 1;
                    bolChangesMade = true;
                }

                if (Left < 0)
                {
                    if (Width > Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        Width = szExpanded.Width = Screen.PrimaryScreen.WorkingArea.Width - 5;
                        Left = 2;
                    }
                    else
                    {
                        Left = 2;
                    }
                    ptMouse_Grab_Move.X -= 1;
                    bolChangesMade = true;

                }
                else if (Right > Screen.PrimaryScreen.WorkingArea.Right)
                {
                    if (Width > Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        Width = szCollapsed.Width = Screen.PrimaryScreen.WorkingArea.Width - 5;
                        Left = 2;
                    }
                    else
                    {
                        Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 2;
                    }
                    ptMouse_Grab_Move.X += 1;
                    bolChangesMade = true;
                }
            } while (bolChangesMade);
        }
        #endregion

        #region Events

        #region Events_Mouse
        /// <summary>
        /// lets the user rotate the orientation of the ExpansionDirection with the right-mouse button & mouse-wheel controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicCollapse_MouseWheel(object sender, MouseEventArgs e)
        {
            if (bolMouseRight_Down)
            {
                if (e.Delta > 0)
                    ExpansionDirection = (enuExpansionDirection)(((int)ExpansionDirection + 1) % 4);
                else
                    ExpansionDirection = (enuExpansionDirection)(((int)ExpansionDirection + 3) % 4);
            }
        }

        /// <summary>
        /// detects and stores the right & left mouse button states into bolMouseLeft_Down and bolMouseRight_Down variables shared between both picCollapse & picHeading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicCollapse_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bolMouseLeft_Down = false;
            }
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = false;
        }

        /// <summary>
        /// detects and stores the right/left mouse button states in bolMouseLeft_Down & bolMouseRight_Down variables.
        /// left-mouse button causes the panelMoveable to expand/collapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picCollapse_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                bolMouseLeft_Down = true;
                bool bolVisible = Visible;
                Hide();
                Collapsed = !Collapsed;
                Visible = bolVisible;
            }
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = true;
        }

        /// <summary>
        /// detects changes in the mouseWheel and allows the user to increase/decrease the expanded panel's height/width using the left or right Mouse-buttons in combination with the mouse-wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseWheel(object sender, MouseEventArgs e)
        {
            int intDeltaSize = 5;
            if (Collapsed) return;

            if (bolMouseLeft_Down)
            {
                if (e.Delta > 0)
                {
                    Width += 2 * intDeltaSize;
                    if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
                        ptMouse_Grab_Move.X += intDeltaSize;
                }
                else
                {
                    if (Width > 50)
                    {
                        Width -= 2 * intDeltaSize;
                        if (ExpansionDirection == enuExpansionDirection.Up || ExpansionDirection == enuExpansionDirection.Down)
                            ptMouse_Grab_Move.X -= intDeltaSize;
                    }
                }
            }
            if (bolMouseRight_Down)
            {
                if (e.Delta > 0)
                {
                    switch (ExpansionDirection)
                    {
                        case enuExpansionDirection.Left:
                        case enuExpansionDirection.Up:
                            Height += 2 * intDeltaSize;
                            ptMouse_Grab_Move.Y += intDeltaSize;
                            Top -= 2 * intDeltaSize;
                            break;

                        case enuExpansionDirection.Right:
                        case enuExpansionDirection.Down:
                            if (Height > 50)
                            {
                                Height -= 2 * intDeltaSize;
                                ptMouse_Grab_Move.Y += intDeltaSize;
                            }
                            break;
                    }
                }
                else
                {
                    switch (ExpansionDirection)
                    {
                        case enuExpansionDirection.Left:
                        case enuExpansionDirection.Up:
                            if (Height > 50)
                            {
                                Height -= 2 * intDeltaSize;
                                ptMouse_Grab_Move.Y += intDeltaSize;
                                Top += 2 * intDeltaSize;
                            }
                            break;

                        case enuExpansionDirection.Right:
                        case enuExpansionDirection.Down:
                            Height += 2 * intDeltaSize;
                            ptMouse_Grab_Move.Y -= intDeltaSize;
                            break;

                    }
                }
            }
            MoveOntoForm();
        }

        /// <summary>
        /// detects and stores the mouse button conditions shared between picHeading & picCollapse stored in bolMouseLeft_Down & bolMouseRight_Down boolean variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                bolMouseLeft_Down = false;
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = false;
        }

        /// <summary>
        /// detects and stores the mosue button conditions shared between picHeading & picCollapse stored in bolMouseLeft_Down & bolMouseRight_Down boolean variables
        /// sets the ptMouse_Grab_Move coordinate which positions the panelMoveable beneath the mouse cursor so long as the left mouse button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                bolMouseLeft_Down = true;
            else if (e.Button == MouseButtons.Right)
                bolMouseRight_Down = true;
            BringToFront();
            ptMouse_Grab_Move.X = ptMouse_Heading.X;
            ptMouse_Grab_Move.Y = ptMouse_Heading.Y;
        }

        /// <summary>
        /// detects motion of the mouse over the Heading.  Moves the panelMoveable beneath the cursor so long as the left-mouse button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picHeading_MouseMove(object sender, MouseEventArgs e)
        {
            ptMouse_Heading.X = e.X;
            ptMouse_Heading.Y = e.Y;

            if (bolMouseLeft_Down)
            {
                Top += (ptMouse_Heading.Y - ptMouse_Grab_Move.Y);
                Left += (ptMouse_Heading.X - ptMouse_Grab_Move.X);
            }
            MoveOntoForm();
        }
        #endregion // events_Mouse

        #region Events_Objects
        /// <summary>
        /// sets the size changes of szExpanded variable to be used when panelMoveable is expanded then draws the Header/collapse images and places objects inside the resized panelMoveable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelMoveable_SizeChanged(object sender, EventArgs e)
        {
            if (!bolInit) return;

            if (!Collapsed)
            {
                szExpanded.Width = Width;
                szExpanded.Height = Height;
            }

            CollapseSize_Set();
            HeadingImage_Set();
            placeControls();
            if (!Collapsed)
                cInitSettings.szExpanded = Size;
        }

        private void PanelMoveable_LocationChanged(object sender, EventArgs e)
        {
            if (!bolInit) return;

            cInitSettings.ptLocation = new Point(Left, Top);
        }

        private void PanelMoveable_Disposed(object sender, EventArgs e)
        {
            cInitSettings.bolCollapsed = Collapsed;
            cInitSettings.eExpansionDirection = ExpansionDirection;
            cInitSettings.Save();
        }

        #endregion // events_objects
        #endregion // events
    }
    public class panelScrolling_Horizontal : Panel
    {
        Panel pnl = new Panel();
        public HScrollBar Hsb = new HScrollBar();
        public List<Panel> lstPanels = new List<Panel>();

        int intHsb_lastScrollvalue = 0;
        int intPnlWidth_lastScrollvalue = 0;

        public panelScrolling_Horizontal()
        {
            Hsb.Name = "panelScrolling_Horizontal->Hsb";
            Controls.Add(Hsb);
            pnl.Name = "panelScrolling_Horizontal->pnl";
            Controls.Add(pnl);
            SizeChanged += panelScrolling_Horizontal_SizeChanged;

            Hsb.Minimum = 0;
            Hsb.Maximum = 100;
            Hsb.SmallChange = 3;
            Hsb.ValueChanged += Hsb_ValueChanged;
            Hsb.Scroll += Hsb_Scroll;
        }

        bool bolAlphabetize = false;
        /// <summary>
        /// alphabetizes according to the name of the panels
        /// </summary>
        public bool Alphabetize
        {
            get { return bolAlphabetize; }
            set
            {
                if (bolAlphabetize != value)
                {
                    bolAlphabetize = value;
                    if (bolAlphabetize) AlphabeticizePanels();
                }
            }
        }

        /// <summary>
        /// places a new panel at the end of the existing panel list
        /// </summary>
        /// <param name="pnlNew">reference to panel to be added to the list</param>
        public void AddPanel(ref Panel pnlNew)
        {
            if (Alphabetize)
            {
                AddPanel_AlphabeticalOrder(ref pnlNew);
                return;
            }
            if (lstPanels.Count > 0)
            {
                Panel pnlReference = lstPanels[lstPanels.Count - 1];
                AddPanel(ref pnlNew, ref pnlReference);
                pnl.Controls.Add(pnlNew);
            }
            else
            {
                lstPanels.Add(pnlNew);
                pnl.Controls.Add(pnlNew);
            }
            PositionPanels();
        }

        /// <summary>
        /// adds a new panel and places it immediately after pnlPanelPreceding on the screen 
        /// OHERRIDES Alphabeticize setting
        /// </summary>
        /// <param name="pnlNew">reference to the new panel to be added to the list</param>
        /// <param name="pnlPanelPreceding">reference to existing panel beneath which the new panel will be placed</param>
        public void AddPanel(ref Panel pnlNew, ref Panel pnlPanelPreceding)
        {
            //Random rnd = new Random();
            //pnlNew.BackColor = Color.FromArgb((int)(rnd.NextDouble() * 255), (int)(rnd.NextDouble() * 255), (int)(rnd.NextDouble() * 255));
            pnl.Controls.Add(pnlNew);
            List<Panel> lstTemp = new List<Panel>();
            for (int intPanelCounter = 0; intPanelCounter < lstPanels.Count; intPanelCounter++)
            {
                Panel pnlOld = lstPanels[intPanelCounter];
                lstTemp.Add(pnlOld);
                if (pnlOld == pnlPanelPreceding)
                    lstTemp.Add(pnlNew);
            }
            lstPanels = lstTemp;
            pnlNew.Visible = true;
            PositionPanels();
            ScrollPanel();
        }

        void AlphabeticizePanels()
        {
            string strInnerName = "", strOuterName = "";
            for (int intOuterLoop = 0; intOuterLoop < lstPanels.Count - 1; intOuterLoop++)
            {
                // measure shortest
                int intShortest = 1024;
                for (int intShortestCounter = intOuterLoop; intShortestCounter < lstPanels.Count; intShortestCounter++)
                {
                    if (lstPanels[intShortestCounter].Name.Length < intShortest)
                        intShortest = lstPanels[intShortestCounter].Name.Length;
                }
                strOuterName = lstPanels[intOuterLoop].Name.Substring(0, intShortest);
                for (int intInnerLoop = intOuterLoop + 1; intInnerLoop < lstPanels.Count; intInnerLoop++)
                {
                    strInnerName = lstPanels[intInnerLoop].Name.Substring(0, intShortest);

                    if (string.Compare(strInnerName, strOuterName) < 0)
                    {
                        Panel pnlTemp = lstPanels[intOuterLoop];
                        lstPanels[intOuterLoop] = lstPanels[intInnerLoop];
                        lstPanels[intInnerLoop] = pnlTemp;
                    }
                }
            }
            PositionPanels();
            ScrollPanel(true);
        }


        void AddPanel_AlphabeticalOrder(ref Panel pnlNew)
        {
            pnl.Controls.Add(pnlNew);
            if (lstPanels.Count == 0)
            {
                lstPanels.Add(pnlNew);
                goto endAddPanel;
            }
            string strNew_Name = pnlNew.Name.ToLower().Trim();
            for (int intPanelCounter = 0; intPanelCounter < lstPanels.Count; intPanelCounter++)
            {
                Panel pnlThis = lstPanels[intPanelCounter];
                string strThis_Name = pnlThis.Name.ToLower().Trim();
                if (string.Compare(strNew_Name, strThis_Name) < 0)
                {
                    lstPanels.Insert(intPanelCounter, pnlNew);
                    goto endAddPanel;
                }
            }
            lstPanels.Add(pnlNew);
            endAddPanel:
            pnlNew.Visible = true;
            PositionPanels();
        }

        /// <summary>
        /// Removes panel from list
        /// </summary>
        /// <param name="pnlRemove"></param>
        public void SubPanel(ref Panel pnlRemove)
        {
            bool bolTest = lstPanels.Remove(pnlRemove);
            pnl.Controls.Remove(pnlRemove);
            PositionPanels();
            ScrollPanel();
        }
        /// <summary>
        /// Removes all panels from its control
        /// </summary>
        public void clear()
        {
            int intStart = 0;
            int intEnd = pnl.Controls.Count;
            clear(intStart, intEnd);
        }
        /// <summary>
        /// Removes all panels after intStart
        /// </summary>
        /// <param name="intStart"></param>
        public void clear(int intStart)
        {
            int intEnd = pnl.Controls.Count > lstPanels.Count
                                        ? pnl.Controls.Count
                                        : lstPanels.Count;
            clear(intStart, intEnd);
        }
        /// <summary>
        /// Removes all panels between intStart and intEnd
        /// </summary>
        public void clear(int intStart, int intEnd)
        {
            int intFinalLength = pnl.Controls.Count - (intEnd - intStart);
            while (pnl.Controls.Count > intFinalLength
                    && intFinalLength >= 0)
            {
                Panel pnlControlled = (Panel)pnl.Controls[intStart];
                pnl.Controls.Remove(pnlControlled);
                pnlControlled.Visible = false;
            }
            intFinalLength = lstPanels.Count - (intEnd - intStart);
            if (intFinalLength >= 0)
                while (lstPanels.Count > intFinalLength)
                {
                    Panel pnl = lstPanels[intStart];
                    lstPanels.Remove(pnl);
                    pnl.Visible = false;
                }
        }

        /// <summary>
        /// positions user panels Horizontally on pnl then resizes pnl to fit them all
        /// </summary>
        public void PositionPanels()
        {
            Hsb.Location = new Point(0, Height - Hsb.Height);
            Hsb.Width = Width;
            for (int intCounter = 0; intCounter < lstPanels.Count; intCounter++)
            {
                Panel pnlTemp = lstPanels[intCounter];
                pnlTemp.Top = 0;
                pnlTemp.Left = (intCounter > 0)
                                          ? lstPanels[intCounter - 1].Right
                                          : 0;
                pnlTemp.Height = Height - Hsb.Height;
                pnlTemp.BringToFront();
            }
            if (lstPanels.Count > 0)
                pnl.Width = lstPanels[lstPanels.Count - 1].Right;
            if (pnl.Width < Width)
                pnl.Width = Width;
            ScrollPanel();
        }

        /// <summary>
        /// places pnl to position appropriate for Hsb.Value
        /// </summary>
        void ScrollPanel() { ScrollPanel(false); }
        void ScrollPanel(bool bolForce)
        {
            if (bolForce
                || (Hsb.Value == intHsb_lastScrollvalue
                    && pnl.Width == intPnlWidth_lastScrollvalue)) return;
            intHsb_lastScrollvalue = Hsb.Value;
            intPnlWidth_lastScrollvalue = pnl.Width;
            double dblRatio = (double)Width / (double)pnl.Width;
            int intLargeChange = (int)(Hsb.Maximum * dblRatio) + 1;
            if (intLargeChange >= Hsb.Maximum) intLargeChange = Hsb.Maximum - 1;
            Hsb.LargeChange = (intLargeChange > Hsb.Maximum || intLargeChange < 1)
                                                              ? Hsb.Maximum - 1
                                                              : intLargeChange;

            pnl.BackColor = Color.Purple;

            pnl.Height = Height - Hsb.Height;
            Hsb.Width = Width;
            Hsb.Top = Height - Hsb.Height;

            if (pnl.Width <= Width)
            {
                pnl.Left = 0;
                return;
            }
            else
            {
                pnl.Left = -(int)((pnl.Width - Width) * ((double)Hsb.Value / (double)(Hsb.Maximum - Hsb.LargeChange)));
                return;
            }
        }
        public new void Scroll(object sender, ScrollEventArgs e)
        {
            switch (e.Type)
            {
                case ScrollEventType.LargeDecrement:
                    if (Hsb.Value - Hsb.LargeChange >= Hsb.Minimum)
                        Hsb.Value -= Hsb.LargeChange;
                    else
                        Hsb.Value = Hsb.Minimum;
                    break;

                case ScrollEventType.LargeIncrement:
                    if (Hsb.Value + Hsb.LargeChange <= Hsb.Maximum - Hsb.LargeChange)
                        Hsb.Value += Hsb.LargeChange;
                    else
                        Hsb.Value = Hsb.Maximum - Hsb.LargeChange;
                    break;

                case ScrollEventType.SmallDecrement:
                    if (Hsb.Value - Hsb.SmallChange >= Hsb.Minimum)
                        Hsb.Value -= Hsb.SmallChange;
                    else
                        Hsb.Value = Hsb.Minimum;
                    break;

                case ScrollEventType.SmallIncrement:
                    if (Hsb.Value + Hsb.SmallChange <= Hsb.Maximum - Hsb.LargeChange)
                        Hsb.Value += Hsb.SmallChange;
                    else
                        Hsb.Value = Hsb.Maximum - Hsb.LargeChange;
                    break;

            }
            ScrollPanel();

        }
        private void Hsb_Scroll(object sender, ScrollEventArgs e) { ScrollPanel(); }
        private void Hsb_ValueChanged(object sender, EventArgs e) { ScrollPanel(); }
        private void panelScrolling_Horizontal_SizeChanged(object sender, EventArgs e)
        {
            pnl.Height = Height;
            PositionPanels();
            ScrollPanel();
        }
    }
    public class panelScrolling_Vertical : Panel
    {
        public Panel pnl = new Panel();
        public VScrollBar vsb = new VScrollBar();
        public List<Panel> lstPanels = new List<Panel>();

        int intVSB_lastScrollValue = 0;
        int intPnlHeight_lastScrollValue = 0;

        public panelScrolling_Vertical()
        {
            vsb.Name = "panelScrolling_Vertical->vsb";
            Controls.Add(vsb);
            pnl.Name = "panelScrolling_Vertical->pnl";
            Controls.Add(pnl);
            SizeChanged += panelScrolling_Vertical_SizeChanged;

            vsb.Minimum = 0;
            vsb.Maximum = 100;
            vsb.SmallChange = 3;
            vsb.ValueChanged += Vsb_ValueChanged;
            vsb.Scroll += Vsb_Scroll;
        }

        bool bolAlphabetize = false;
        /// <summary>
        /// alphabetizes according to the name of the panels
        /// </summary>
        public bool Alphabetize
        {
            get { return bolAlphabetize; }
            set
            {
                if (bolAlphabetize != value)
                {
                    bolAlphabetize = value;
                    if (bolAlphabetize) AlphabeticizePanels();
                }
            }
        }

        /// <summary>
        /// places a new panel at the end of the existing panel list
        /// </summary>
        /// <param name="pnlNew">reference to panel to be added to the list</param>
        public void AddPanel(ref Panel pnlNew)
        {
            if (Alphabetize)
            {
                AddPanel_AlphabeticalOrder(ref pnlNew);
                return;
            }
            if (lstPanels.Count > 0)
            {
                Panel pnlReference = lstPanels[lstPanels.Count - 1];
                AddPanel(ref pnlNew, ref pnlReference);
                pnl.Controls.Add(pnlNew);
                //pnlNew.Visible = true;
            }
            else
            {
                lstPanels.Add(pnlNew);
                pnl.Controls.Add(pnlNew);

            }
            //PositionPanels();
        }

        /// <summary>
        /// adds a new panel and places it immediately after pnlPanelPreceding on the screen 
        /// OVERRIDES Alphabeticize setting
        /// </summary>
        /// <param name="pnlNew">reference to the new panel to be added to the list</param>
        /// <param name="pnlPanelPreceding">reference to existing panel beneath which the new panel will be placed</param>
        public void AddPanel(ref Panel pnlNew, ref Panel pnlPanelPreceding)
        {
            //Random rnd = new Random();
            //pnlNew.BackColor = Color.FromArgb((int)(rnd.NextDouble() * 255), (int)(rnd.NextDouble() * 255), (int)(rnd.NextDouble() * 255));
            pnl.Controls.Add(pnlNew);
            List<Panel> lstTemp = new List<Panel>();
            for (int intPanelCounter = 0; intPanelCounter < lstPanels.Count; intPanelCounter++)
            {
                Panel pnlOld = lstPanels[intPanelCounter];
                lstTemp.Add(pnlOld);
                if (pnlOld == pnlPanelPreceding)
                    lstTemp.Add(pnlNew);
            }
            lstPanels = lstTemp;
            pnlNew.Visible = true;
            //PositionPanels();
            //ScrollPanel();
        }

        void AlphabeticizePanels()
        {
            string strInnerName = "", strOuterName = "";
            for (int intOuterLoop = 0; intOuterLoop < lstPanels.Count - 1; intOuterLoop++)
            {
                // measure shortest
                int intShortest = 1024;
                for (int intShortestCounter = intOuterLoop; intShortestCounter < lstPanels.Count; intShortestCounter++)
                {
                    if (lstPanels[intShortestCounter].Name.Length < intShortest)
                        intShortest = lstPanels[intShortestCounter].Name.Length;
                }
                strOuterName = lstPanels[intOuterLoop].Name.Substring(0, intShortest);
                for (int intInnerLoop = intOuterLoop + 1; intInnerLoop < lstPanels.Count; intInnerLoop++)
                {
                    strInnerName = lstPanels[intInnerLoop].Name.Substring(0, intShortest);

                    if (string.Compare(strInnerName, strOuterName) < 0)
                    {
                        Panel pnlTemp = lstPanels[intOuterLoop];
                        lstPanels[intOuterLoop] = lstPanels[intInnerLoop];
                        lstPanels[intInnerLoop] = pnlTemp;
                    }
                }
            }
            PositionPanels();
            ScrollPanel(true);
        }


        void AddPanel_AlphabeticalOrder(ref Panel pnlNew)
        {
            pnl.Controls.Add(pnlNew);
            if (lstPanels.Count == 0)
            {
                lstPanels.Add(pnlNew);
                goto endAddPanel;
            }
            string strNew_Name = pnlNew.Name.ToLower().Trim();
            for (int intPanelCounter = 0; intPanelCounter < lstPanels.Count; intPanelCounter++)
            {
                Panel pnlThis = lstPanels[intPanelCounter];
                string strThis_Name = pnlThis.Name.ToLower().Trim();
                if (string.Compare(strNew_Name, strThis_Name) < 0)
                {
                    lstPanels.Insert(intPanelCounter, pnlNew);
                    goto endAddPanel;
                }
            }
            lstPanels.Add(pnlNew);
            endAddPanel:
            pnlNew.Visible = true;
            PositionPanels();
            //ScrollPanel();
        }

        /// <summary>
        /// removes panel from list
        /// </summary>
        /// <param name="pnlRemove"></param>
        public void SubPanel(ref Panel pnlRemove)
        {
            bool bolTest = lstPanels.Remove(pnlRemove);
            pnl.Controls.Remove(pnlRemove);
            //      pnlRemove.Visible = false;
            PositionPanels();
            ScrollPanel();
        }
        /// <summary>
        /// removes all panels from its control
        /// </summary>
        public void clear()
        {
            int intStart = 0;
            int intEnd = pnl.Controls.Count;
            clear(intStart, intEnd);
        }
        /// <summary>
        /// removes all panels after intStart
        /// </summary>
        /// <param name="intStart"></param>
        public void clear(int intStart)
        {
            int intEnd = pnl.Controls.Count > lstPanels.Count
                                        ? pnl.Controls.Count
                                        : lstPanels.Count;
            clear(intStart, intEnd);
        }
        /// <summary>
        /// removes all panels between intStart and intEnd
        /// </summary>
        public void clear(int intStart, int intEnd)
        {
            int intFinalLength = pnl.Controls.Count - (intEnd - intStart);
            while (pnl.Controls.Count > intFinalLength
                    && intFinalLength >= 0)
            {
                Panel pnlControlled = (Panel)pnl.Controls[intStart];
                pnl.Controls.Remove(pnlControlled);
                pnlControlled.Visible = false;
            }
            intFinalLength = lstPanels.Count - (intEnd - intStart);
            if (intFinalLength >= 0)
                while (lstPanels.Count > intFinalLength)
                {
                    Panel pnl = lstPanels[intStart];
                    lstPanels.Remove(pnl);
                    pnl.Visible = false;
                }
        }

        /// <summary>
        /// positions user panels vertically on pnl then resizes pnl to fit them all
        /// </summary>
        public void PositionPanels()
        {
            vsb.Location = new Point(Width - vsb.Width, 0);
            vsb.Height = Height;
            for (int intCounter = 0; intCounter < lstPanels.Count; intCounter++)
            {
                Panel pnlTemp = lstPanels[intCounter];
                pnlTemp.Left = 0;
                pnlTemp.Top = (intCounter > 0)
                                          ? lstPanels[intCounter - 1].Bottom
                                          : 0;
                pnlTemp.Width = Width - vsb.Width;
                pnlTemp.Visible = true;
                pnlTemp.BringToFront();
            }
            if (lstPanels.Count > 0)
                pnl.Height = lstPanels[lstPanels.Count - 1].Bottom;
            if (pnl.Height < Height)
                pnl.Height = Height;
            ScrollPanel();
        }

        /// <summary>
        /// places pnl to position appropriate for vsb.value
        /// </summary>
        void ScrollPanel() { ScrollPanel(false); }
        void ScrollPanel(bool bolForce)
        {
            if (bolForce
                || (vsb.Value == intVSB_lastScrollValue
                    && pnl.Height == intPnlHeight_lastScrollValue)) return;
            intVSB_lastScrollValue = vsb.Value;
            intPnlHeight_lastScrollValue = pnl.Height;
            double dblRatio = (double)Height / (double)pnl.Height;
            int intLargeChange = (int)(vsb.Maximum * dblRatio) + 1;
            if (intLargeChange >= vsb.Maximum) intLargeChange = vsb.Maximum - 1;
            vsb.LargeChange = (intLargeChange > vsb.Maximum || intLargeChange < 1)
                                                              ? vsb.Maximum - 1
                                                              : intLargeChange;
            pnl.Width = Width - vsb.Width;
            vsb.Height = Height;
            vsb.Left = Width - vsb.Width;

            if (pnl.Height <= Height)
            {
                pnl.Top = 0;
                return;
            }
            else
            {
                pnl.Top = -(int)((pnl.Height - Height) * ((double)vsb.Value / (double)(vsb.Maximum - vsb.LargeChange)));
                return;
            }
        }
        public new void Scroll(object sender, ScrollEventArgs e)
        {
            switch (e.Type)
            {
                case ScrollEventType.LargeDecrement:
                    if (vsb.Value - vsb.LargeChange >= vsb.Minimum)
                        vsb.Value -= vsb.LargeChange;
                    else
                        vsb.Value = vsb.Minimum;
                    break;

                case ScrollEventType.LargeIncrement:
                    if (vsb.Value + vsb.LargeChange <= vsb.Maximum - vsb.LargeChange)
                        vsb.Value += vsb.LargeChange;
                    else
                        vsb.Value = vsb.Maximum - vsb.LargeChange;
                    break;

                case ScrollEventType.SmallDecrement:
                    if (vsb.Value - vsb.SmallChange >= vsb.Minimum)
                        vsb.Value -= vsb.SmallChange;
                    else
                        vsb.Value = vsb.Minimum;
                    break;

                case ScrollEventType.SmallIncrement:
                    if (vsb.Value + vsb.SmallChange <= vsb.Maximum - vsb.LargeChange)
                        vsb.Value += vsb.SmallChange;
                    else
                        vsb.Value = vsb.Maximum - vsb.LargeChange;
                    break;

            }
            ScrollPanel();
            /*
                            int intDifference = e.NewValue - e.OldValue ;
                            if (intDifference > 0)
                            {
                                if (vsb.Value < vsb.Maximum - vsb.LargeChange)
                                    vsb.Value += intDifference;
                                else
                                    vsb.Value = vsb.Maximum - vsb.LargeChange;
                            }
                            else
                            {
                                if (vsb.Value > vsb.Minimum)
                                    vsb.Value -= intDifference;
                                else
                                    vsb.Value = 0;
                            }*/
        }
        private void Vsb_Scroll(object sender, ScrollEventArgs e) { ScrollPanel(); }
        private void Vsb_ValueChanged(object sender, EventArgs e) { ScrollPanel(); }
        //    private void Vsb_Scroll(object sender, ScrollEventArgs e) { ScrollPanel(); }
        private void panelScrolling_Vertical_SizeChanged(object sender, EventArgs e)
        {
            pnl.Width = Width;
            PositionPanels();
            ScrollPanel();
        }
    }
    public class classHScroll_Label_Combo : Panel
    {
        public HScrollBar hsb = new HScrollBar();
        public Label lbl = new Label();

        public int Value
        {
            get { return hsb.Value; }
            set
            {
                if (value >= hsb.Minimum
                    && value <= hsb.Maximum)
                    hsb.Value = value;
            }
        }

        public classHScroll_Label_Combo()
        {
            hsb.Name = "classHScroll_Label_Combo->hsb";
            Controls.Add(hsb);
            Controls.Add(lbl);
            hsb.ValueChanged += Hsb_ValueChanged;
            SizeChanged += ClassHScroll_Label_Combo_SizeChanged;
        }

        public System.Drawing.Color backColor
        {
            get { return BackColor; }
            set
            {
                BackColor = value;
            }
        }

        private string strText = "label";
        override public string Text
        {
            get { return strText; }
            set
            {
                strText = value;
                placeControls();
            }
        }

        private string strValueModifier = "";
        public string Label_Value_Modifer
        {
            get { return strValueModifier; }
            set
            {
                strValueModifier = value;
                setLabelText();
            }
        }

        private bool bolLabelOnLeft = true;
        public bool LabelOnLeft
        {
            get { return bolLabelOnLeft; }
            set
            {
                bolLabelOnLeft = value;
                placeControls();
            }
        }

        double dblPercentageWidth_Label = .25;
        public double PercentageWidth_Label
        {
            get { return dblPercentageWidth_Label; }
            set
            {
                dblPercentageWidth_Label = value;
                placeControls();
            }
        }

        void placeControls()
        {
            lbl.Width = (int)((double)Width * dblPercentageWidth_Label);
            hsb.Width = Width - lbl.Width;
            lbl.Height = hsb.Height = Height;
            setLabelText();
            if (bolLabelOnLeft)
            {
                lbl.Left = 0;
                hsb.Left = lbl.Left + lbl.Width;
            }
            else
            {
                hsb.Left = 0;
                lbl.Left = hsb.Left + hsb.Width;
            }
        }



        private void ClassHScroll_Label_Combo_SizeChanged(object sender, EventArgs e)
        {
            placeControls();
        }

        private void Hsb_ValueChanged(object sender, EventArgs e)
        {
            setLabelText();
        }
        private void setLabelText()
        {
            lbl.Text = strText + hsb.Value.ToString(strValueModifier);
        }

    }
    public class classLabel_Textbox_Label_Combo : Panel
    {
        public TextBox txt = new TextBox();
        public Label lbl_Preceding = new Label();
        public Label lbl_Following = new Label();


        void resizeObjects()
        {
            txt.Left = lbl_Preceding.Width + 1;
            txt.Width = Width - lbl_Preceding.Width - lbl_Following.Width;
            lbl_Following.Left = txt.Left + txt.Width;
        }

        public classLabel_Textbox_Label_Combo()
        {
            lbl_Preceding.AutoSize = true;
            lbl_Following.AutoSize = true;
            Controls.Add(txt);
            Controls.Add(lbl_Following);
            Controls.Add(lbl_Preceding);
            SizeChanged += classLabel_Textbox__Label_Combo_SizeChanged;
            lbl_Preceding.SizeChanged += Lbl_SizeChanged;
            lbl_Following.SizeChanged += Lbl_SizeChanged;
        }

        private void Lbl_SizeChanged(object sender, EventArgs e)
        {
            resizeObjects();
        }

        private void classLabel_Textbox__Label_Combo_SizeChanged(object sender, EventArgs e)
        {
            resizeObjects();
        }
    }
    public class classLabel_Textbox_Combo : Panel
    {
        public TextBox txt = new TextBox();
        public Label lbl = new Label();

        #region resize and position objects
        public enum enuLabelPosition_Horizontal { Left, Left_Edge, CLeave, Right_Edge, Right };
        public enum enuLabelPosition_Vertical { Top, Top_Edge, CLeave, Bottom_Edge, Bottom };

        enuLabelPosition_Horizontal eLabelPosition_Horizontal = enuLabelPosition_Horizontal.Left_Edge;
        public enuLabelPosition_Horizontal LabelPosition_Horizontal
        {
            get { return eLabelPosition_Horizontal; }
            set
            {
                if (value != eLabelPosition_Horizontal)
                {
                    eLabelPosition_Horizontal = value;

                    repositionObjects();
                }
            }
        }
        enuLabelPosition_Vertical eLabelPosition_Vertical = enuLabelPosition_Vertical.Top;
        public enuLabelPosition_Vertical LabelPosition_Vertical
        {
            get { return eLabelPosition_Vertical; }
            set
            {
                if (value != eLabelPosition_Vertical)
                {
                    eLabelPosition_Vertical = value;
                    repositionObjects();
                }
            }
        }



        void resizePanel()
        {


        }
        void repositionObjects()
        {
            // resize textbox relative to the label : horizontally
            switch (eLabelPosition_Horizontal)
            {
                case enuLabelPosition_Horizontal.Left_Edge:
                case enuLabelPosition_Horizontal.Right_Edge:
                case enuLabelPosition_Horizontal.CLeave:
                    resize_LabelSide(false);
                    break;

                case enuLabelPosition_Horizontal.Left:
                case enuLabelPosition_Horizontal.Right:
                default:
                    resize_LabelSide(true);
                    break;
            }
            // resize textbox relative to the label _ vertically
            switch (eLabelPosition_Vertical)
            {
                case enuLabelPosition_Vertical.Bottom_Edge:
                case enuLabelPosition_Vertical.Top_Edge:
                case enuLabelPosition_Vertical.CLeave:
                    resize_LabelTopBottom(false);
                    break;
                case enuLabelPosition_Vertical.Top:
                case enuLabelPosition_Vertical.Bottom:
                default:
                    resize_LabelTopBottom(true);
                    break;
            }

            // position both the label and the textbox : horizontally
            switch (eLabelPosition_Horizontal)
            {
                case enuLabelPosition_Horizontal.Left:
                    lbl.Left = 0;
                    txt.Left = lbl.Width;
                    break;

                case enuLabelPosition_Horizontal.Left_Edge:
                    lbl.Left = 0;
                    txt.Left = 0;
                    break;

                case enuLabelPosition_Horizontal.CLeave:
                    lbl.Left = (Width - lbl.Width) / 2;
                    txt.Left = 0;
                    break;

                case enuLabelPosition_Horizontal.Right:
                case enuLabelPosition_Horizontal.Right_Edge:
                    lbl.Left = Width - lbl.Width;
                    txt.Left = 0;
                    break;
            }
            // position both the label and the textbox : vertically
            switch (eLabelPosition_Vertical)
            {
                case enuLabelPosition_Vertical.Top:
                    lbl.Top = 0;
                    txt.Top = lbl.Height;
                    break;
                case enuLabelPosition_Vertical.Top_Edge:
                    lbl.Top = 0;
                    txt.Top = 0;
                    break;
                case enuLabelPosition_Vertical.CLeave:
                    lbl.Top = (Height - lbl.Height) / 2;
                    txt.Top = 0;
                    break;
                case enuLabelPosition_Vertical.Bottom:
                case enuLabelPosition_Vertical.Bottom_Edge:
                    lbl.Top = Height - lbl.Height;
                    txt.Top = 0;
                    break;
            }
        }

        /// <summary>
        /// when true resizes Width of textbox to fit in the container beside the autosized label, otherwise sets width to the size of the container
        /// </summary>
        /// <param name="bolCheck"></param>
        void resize_LabelSide(bool bolCheck)
        {
            if (bolCheck)
                txt.Width = Width - lbl.Width;
            else
                txt.Width = Width;
        }
        /// <summary>
        /// when true resizes Height of textbox to fin in the container below or above the autosized lable, otherwise sets height to the height of the container
        /// </summary>
        /// <param name="bolCheck"></param>
        void resize_LabelTopBottom(bool bolCheck)
        {
            if (bolCheck)
                txt.Height = Height - lbl.Height;
            else
                txt.Height = Height;

        }
        #endregion

        public classLabel_Textbox_Combo()
        {
            lbl.AutoSize = true;
            txt.Multiline = true;
            Controls.Add(txt);
            Controls.Add(lbl);
            SizeChanged += classLabel_Textbox_Combo_SizeChanged;
            lbl.SizeChanged += Lbl_SizeChanged;
        }

        private void Lbl_SizeChanged(object sender, EventArgs e)
        {
            repositionObjects();
        }

        private void classLabel_Textbox_Combo_SizeChanged(object sender, EventArgs e)
        {
            repositionObjects();
        }
    }
    public class classLabel_ComboBox_Combo : Panel
    {
        public ComboBox cmb = new ComboBox();
        public Label lbl = new Label();

        public Bitmap[] bmpCursors;

        #region resize and position objects
        public enum enuLabelPosition_Horizontal { Left, Left_Edge, CLeave, Right_Edge, Right };
        public enum enuLabelPosition_Vertical { Top, Top_Edge, CLeave, Bottom_Edge, Bottom };

        enuLabelPosition_Horizontal eLabelPosition_Horizontal = enuLabelPosition_Horizontal.Left_Edge;
        public enuLabelPosition_Horizontal LabelPosition_Horizontal
        {
            get { return eLabelPosition_Horizontal; }
            set
            {
                if (value != eLabelPosition_Horizontal)
                {
                    eLabelPosition_Horizontal = value;

                    repositionObjects();
                }
            }
        }
        enuLabelPosition_Vertical eLabelPosition_Vertical = enuLabelPosition_Vertical.Top;
        public enuLabelPosition_Vertical LabelPosition_Vertical
        {
            get { return eLabelPosition_Vertical; }
            set
            {
                if (value != eLabelPosition_Vertical)
                {
                    eLabelPosition_Vertical = value;
                    repositionObjects();
                }
            }
        }


        void repositionObjects() { repositionObjects(false); }
        void repositionObjects(bool bolForce)
        {
            // resize textbox relative to the label : horizontally
            switch (eLabelPosition_Horizontal)
            {
                case enuLabelPosition_Horizontal.Left_Edge:
                case enuLabelPosition_Horizontal.Right_Edge:
                case enuLabelPosition_Horizontal.CLeave:
                    resize_LabelSide(false);
                    break;

                case enuLabelPosition_Horizontal.Left:
                case enuLabelPosition_Horizontal.Right:
                default:
                    resize_LabelSide(true);
                    break;
            }
            // resize textbox relative to the label _ vertically
            switch (eLabelPosition_Vertical)
            {
                case enuLabelPosition_Vertical.Bottom_Edge:
                case enuLabelPosition_Vertical.Top_Edge:
                case enuLabelPosition_Vertical.CLeave:
                    resize_LabelTopBottom(false);
                    break;
                case enuLabelPosition_Vertical.Top:
                case enuLabelPosition_Vertical.Bottom:
                default:
                    resize_LabelTopBottom(true);
                    break;
            }

            // position both the label and the textbox : horizontally
            switch (eLabelPosition_Horizontal)
            {
                case enuLabelPosition_Horizontal.Left:
                    lbl.Left = 0;
                    cmb.Left = lbl.Width;
                    break;

                case enuLabelPosition_Horizontal.Left_Edge:
                    lbl.Left = 0;
                    cmb.Left = 0;
                    break;

                case enuLabelPosition_Horizontal.CLeave:
                    lbl.Left = (Width - lbl.Width) / 2;
                    cmb.Left = 0;
                    break;

                case enuLabelPosition_Horizontal.Right:
                case enuLabelPosition_Horizontal.Right_Edge:
                    lbl.Left = Width - lbl.Width;
                    cmb.Left = 0;
                    break;
            }
            // position both the label and the textbox : vertically
            switch (eLabelPosition_Vertical)
            {
                case enuLabelPosition_Vertical.Top:
                    lbl.Top = 0;
                    cmb.Top = lbl.Height;
                    break;
                case enuLabelPosition_Vertical.Top_Edge:
                    lbl.Top = 0;
                    cmb.Top = 0;
                    break;
                case enuLabelPosition_Vertical.CLeave:
                    lbl.Top = (Height - lbl.Height) / 2;
                    cmb.Top = 0;
                    break;
                case enuLabelPosition_Vertical.Bottom:
                case enuLabelPosition_Vertical.Bottom_Edge:
                    lbl.Top = Height - lbl.Height;
                    cmb.Top = 0;
                    break;
            }
        }

        /// <summary>
        /// when true resizes Width of textbox to fit in the container beside the autosized label, otherwise sets width to the size of the container
        /// </summary>
        /// <param name="bolCheck"></param>
        void resize_LabelSide(bool bolCheck)
        {
            if (bolCheck)
                cmb.Width = Width - lbl.Width;
            else
                cmb.Width = Width;
        }
        /// <summary>
        /// when true resizes Height of textbox to fin in the container below or above the autosized lable, otherwise sets height to the height of the container
        /// </summary>
        /// <param name="bolCheck"></param>
        void resize_LabelTopBottom(bool bolCheck)
        {
            if (bolCheck)
                cmb.Height = Height - lbl.Height;
            else
                cmb.Height = Height;
        }


        #endregion

        public classLabel_ComboBox_Combo()
        {
            lbl.AutoSize = true;
            Controls.Add(cmb);
            Controls.Add(lbl);
            SizeChanged += classLabel_Textbox_Combo_SizeChanged;
            lbl.SizeChanged += Lbl_SizeChanged;
        }



        private void Lbl_SizeChanged(object sender, EventArgs e)
        {
            repositionObjects();
        }

        private void classLabel_Textbox_Combo_SizeChanged(object sender, EventArgs e)
        {
            repositionObjects();
        }
    }
    public class classRadioButtonArray : Panel
    {
        static int intCounter = 0;
        public int intMyID = intCounter++;
        public Label lblName = new Label();
        public RadioButton[] rdbArray = null;
        public classRadioButtonArray(string strName, int intNumberButtons)
        {
            if (intNumberButtons < 1)
            {
                MessageBox.Show("must have more than zero number of buttons.");
                Dispose();
                return;
            }
            rdbArray = new RadioButton[intNumberButtons];
            for (int intCounter = 0; intCounter < rdbArray.Length; intCounter++)
            {
                RadioButton rdbNew = new RadioButton();
                rdbNew.Text = "";
                rdbNew.AutoSize = true;
                rdbNew.CheckedChanged += RdbNew_CheckedChanged;
                rdbNew.MouseLeave += RdbNew_MouseLeave;
                rdbNew.MouseLeave += RdbNew_MouseLeave;
                rdbNew.Name = "classRadioButtonArray->rdbNew[" + intCounter.ToString() + "]";
                Controls.Add(rdbNew);
                rdbArray[intCounter] = rdbNew;
            }
            Selected = 0;
            lblName.Name = "classRadioButtonArray->lblName";
            Controls.Add(lblName);
            lblName.Text = strName;
            lblName.AutoSize = true;
            lblName.Click += LblName_Click;
            placeControls();
            Height = rdbArray[0].Bottom + 10;

            SizeChanged += ClassRadioButtonArray_SizeChanged;
        }

        private void LblName_Click(object sender, EventArgs e)
        {
            int intSelected = (Selected + 1) % 3;
            Selected = intSelected;
        }

        private void RdbNew_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdbSender = (RadioButton)sender;
            if (rdbSender.Checked)
            {
                int intSelected = getThisRDBIndex(ref rdbSender);
                Selected = intSelected;
            }
        }

        int getThisRDBIndex(ref RadioButton rdb)
        {
            for (int intCounter = 0; intCounter < rdbArray.Length; intCounter++)
                if (rdb == rdbArray[intCounter]) return intCounter;
            return -1;
        }

        private void RdbNew_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void RdbNew_MouseEnter(object sender, EventArgs e)
        {
            RadioButton rdb = (RadioButton)sender;
            int intMyIndex = getThisRDBIndex(ref rdb);
            Cursor = getCursor(intMyIndex);
        }

        public Cursor getCursor(int intButton)
        {
            Bitmap btMap = new Bitmap(bmpArray[intButton], bmpArray[intButton].Width, bmpArray[intButton].Height);
            IntPtr ptr = btMap.GetHicon();
            return new Cursor(ptr);
        }

        /// <summary>
        /// gets and sets the array of bitmaps used to generate the cursor's appears between mouseLeave and mouseLeave events of the individual radio buttons.  
        /// </summary>
        static Bitmap[] bmpArray = null;
        public static Bitmap[] CursorBitmaps
        {
            get { return bmpArray; }
            set
            {
                bmpArray = value;
            }
        }

        /// <summary>
        /// sets the Font for the text as the checked radiobutton value changes
        /// </summary>
        public Font[] fnt = null;

        /// <summary>
        /// sets the fore color of the label as the checked radio button value changes
        /// </summary>
        Color[] foreColors_ = null;
        public Color[] foreColors
        {
            get { return foreColors_; }
            set
            {
                foreColors_ = value;
                setColors();
            }
        }
        /// <summary>
        /// sets the background color of the label as the checked radio button value changes
        /// </summary>
        Color[] backColors_ = null;
        public Color[] backColors
        {
            get { return backColors_; }
            set
            {
                backColors_ = value;
                setColors();
            }
        }

        private void ClassRadioButtonArray_SizeChanged(object sender, EventArgs e)
        {
            placeControls();
        }

        void placeControls()
        {
            for (int intCounter = 0; intCounter < 3; intCounter++)
            {
                rdbArray[intCounter].AutoSize = true;
                rdbArray[intCounter].Location = new Point((3 + rdbArray[0].Height) * intCounter, 0);
            }
            lblName.Location = new Point(rdbArray[rdbArray.Length - 1].Right + 3, rdbArray[0].Top);
        }

        public string Name_
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }

        int intSelected;
        public int Selected
        {
            get
            {
                for (int intCounter = 0; intCounter < rdbArray.Length; intCounter++)
                    if (rdbArray[intCounter].Checked)
                    {
                        return intCounter;
                    }
                rdbArray[0].Checked = true;
                return 0;
            }
            set
            {
                intSelected = value;
                rdbArray[value].Checked = true;
                if (fnt != null && fnt.Length == rdbArray.Length)
                    Font = fnt[value];
                setColors();
            }
        }

        void setColors()
        {
            if (foreColors != null && foreColors.Length == rdbArray.Length)
            {
                lblName.ForeColor = foreColors[intSelected];
            }
            if (backColors != null && backColors.Length == rdbArray.Length)
            {
                lblName.BackColor = backColors[intSelected];
                BackColor = backColors[intSelected];
            }
        }
    }
    public class classGraphical_HSlider : PictureBox
    {
        #region Global_Variables

        Bitmap bmpCurrent = null;
        int[] intGraph_Height = null;
        double dblAspectRatio = 1;
        bool bolMouseDown = false;
        Point ptMouse = new Point();

        #endregion
        /// <summary>
        /// a graphical Horizonal slider with animated slider that runs along a path
        /// </summary>
        public classGraphical_HSlider()
        {
            SizeChanged += ClassGraphical_HSlider_SizeChanged;
            MouseDown += ClassGraphical_HSlider_MouseDown;
            MouseUp += ClassGraphical_HSlider_MouseUp;
            MouseMove += ClassGraphical_HSlider_MouseMove;
        }

        #region Events
        private void ClassGraphical_HSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (bolMouseDown)
            {
                ptMouse.X = e.X;
                double dblPercentageLeft = (double)ptMouse.X / (double)Width;
                int intValue = (int)(dblPercentageLeft * (double)(Min + (Max - Min)));
                Value = intValue;
            }
        }
        private void ClassGraphical_HSlider_MouseUp(object sender, MouseEventArgs e) { bolMouseDown = false; }
        private void ClassGraphical_HSlider_MouseDown(object sender, MouseEventArgs e) { bolMouseDown = true; }
        private void ClassGraphical_HSlider_SizeChanged(object sender, EventArgs e) { setAspectRatio(); }
        #endregion

        #region Methods

        double dblSlider_Scale_Left = 1.0;
        /// <summary>
        /// the scale is a double factor by which slider's image size will scale when moving from left to right
        /// </summary>
        public double Slider_Scale_Left
        {
            get { return dblSlider_Scale_Left; }
            set { dblSlider_Scale_Left = value; }
        }

        double dblSlider_Scale_Right = 0.15;
        /// <summary>
        /// the scale is a double factor by which slider's image size will scale when moving from left to right
        /// </summary>
        public double Slider_Scale_Right
        {
            get { return dblSlider_Scale_Right; }
            set { dblSlider_Scale_Right = value; }
        }

        bool bolDisplayValue = true;
        /// <summary>
        /// a boolean condition which determines whether the value of the slider will be displayed on the screen
        /// </summary>
        public bool DisplayValue
        {
            get { return bolDisplayValue; }
            set
            {
                bool bolReset = bolDisplayValue != value;
                bolDisplayValue = value;
                if (bolReset) Draw();
            }
        }

        public Size Size_Value
        { get { return szValue; } }

        bool bolDisplayText = true;
        /// <summary>
        /// a boolean condition which determines whether the name of the slider will be displayed on the screen
        /// </summary>
        public bool DisplayText
        {
            get { return bolDisplayText; }
            set
            {
                bool bolReset = bolDisplayText != value;
                bolDisplayText = value;
                if (bolReset) Draw();
            }
        }


        Size szValue;
        public Size sizeValue
        { get { return szValue; } }

        int _intValue = 0;
        int intValue // used by mouseMove event
        {
            get { return _intValue; }
            set { _intValue = value; }
        }
        /// <summary>
        /// the slider's integer value between Min and Max
        /// </summary>
        public int Value
        {
            get { return _intValue; }
            set
            {
                if (value >= Min
                    && value <= Max)
                {
                    bool bolReset = _intValue != value;
                    int intDifference = value - _intValue;
                    _intValue = value;
                    if (bolReset)
                    {
                        Slider_Refresh(intDifference);
                    }
                }
            }
        }

        public void Slider_Refresh() { Slider_Refresh(0); }
        void Slider_Refresh(int intDifference)
        {
            double dblPercentageLeft = (double)(intValue - intMin) / (double)(intMax - intMin);

            int intX = (int)(dblPercentageLeft * (double)Width);
            int intSlider_X_Index = (int)(((double)ptMouse.X / (double)(Width + 1)) * (double)intGraph_Height.Length);
            if (intSlider_X_Index < 0) intSlider_X_Index = 0;
            if (intSlider_X_Index >= intGraph_Height.Length) intSlider_X_Index = intGraph_Height.Length - 1;
            _ptSlider_Screen = new Point(intX, (int)(intGraph_Height[intSlider_X_Index] * (double)Height / (double)bmpBackground.Height));
            if (bolAutoCycleSliderImage)
            {
                if (intDifference > 0)
                    intSlider_BitmapIndex = (intSlider_BitmapIndex + 1) % bmpSlider.Length;
                else
                    intSlider_BitmapIndex = (intSlider_BitmapIndex + bmpSlider.Length - 1) % bmpSlider.Length;
            }
            Draw();
        }

        Size szText;
        public Size sizeText
        {
            get { return szText; }
        }

        string strText;
        /// <summary>
        /// a string value that may be drawn on the HSlider.  not to CONFUSE 'text'(the label printed for the user) with 'Text' of the inherited PictureBox 
        /// </summary>
        public string text
        {
            get { return strText; }
            set
            {
                bool bolReset = string.Compare(value, strText) == 0;
                strText = value;
                if (bolReset)
                    Draw();
            }
        }

        int intMax = 100;
        /// <summary>
        /// Maximum value the HSlider's value can hold.  must be GREATER than Min
        /// </summary>
        public int Max
        {
            get { return intMax; }
            set
            {
                if (value > intMin)
                    intMax = value;
            }
        }

        int intMin = 0;
        /// <summary>
        /// Minimum value the HSlider's value can hold. must be LESS than Max
        /// </summary>
        public int Min
        {
            get { return intMin; }
            set { intMin = value; }
        }

        Point _ptSlider_Screen = new Point();
        public Point ptSlider_Screen
        {
            get { return _ptSlider_Screen; }
        }

        //Point _ptSlider = new Point();
        /// <summary>
        /// point on the HSlider where the HSlider's Slider will be drawn. Given an X coordinate the graphical-path drawn in the bmpBackground_Graph will determine the corresponding Y coordinate
        /// </summary>
        //public Point ptSlider
        // {
        //     get { return ptSlider; }
        //  }

        bool bolAutoSetGraphColor = true;
        /// <summary>
        /// default value is true and will detect the color at pixel(0,0) of the bmpBackground_Graph bitmap.  The Graph Color is the unique color that is used to draw the Slider's path on the bmpBackground_Graph image.
        /// </summary>
        public bool AutoSetGraphColor
        {
            get { return bolAutoSetGraphColor; }
            set
            {
                bool bolReset = (value && value != bolAutoSetGraphColor);
                bolAutoSetGraphColor = value;
                if (bolReset) evaluateNewBackground();
            }
        }

        Color clrGraph = Color.White;
        /// <summary>
        /// unique identifying color that is used on the bmpBackground_Graph bitmap to guide the slider along as the value value shifts and the slider moves from left to right.  see AutoSetGraphColor
        /// </summary>
        public Color colorGraph
        {
            get { return clrGraph; }
            set
            {
                bool bolReset = (clrGraph != value);
                clrGraph = value;
                if (bolReset) evaluateNewBackground();
            }
        }

        Bitmap _bmpForeground = null;
        /// <summary>
        /// Foreground bitmap drawn behind the slider and stretched to cover the entire HSlider
        /// </summary>
        public Bitmap bmpForeground
        {
            get { return _bmpForeground; }
            set
            {
                _bmpForeground = value;
                _bmpForeground.MakeTransparent(_bmpForeground.GetPixel(0, 0));
                SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        Bitmap _bmpBackground = null;
        /// <summary>
        /// background bitmap drawn behind the slider and stretched to cover the entire HSlider
        /// </summary>
        public Bitmap bmpBackground
        {
            get { return _bmpBackground; }
            set
            {
                _bmpBackground = value;
                Image = _bmpBackground;
                SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        Bitmap _bmpBackground_Graph = null;
        /// <summary>
        /// background bitmap on which the slider's path is drawn.  since this is the path along which the slider will move when it is drawn on top of the bmpBackground image it is best for both the bmpBackground and bmpBackground_Graph images to be of equal size
        /// </summary>
        public Bitmap bmpBackground_Graph
        {
            get { return _bmpBackground_Graph; }
            set
            {
                _bmpBackground_Graph = value;
                evaluateNewBackground();
            }
        }

        bool bolAutoCycleSliderImage = true;
        /// <summary>
        /// boolean condition which determines whether the Slider's image is changed whenever the HSlider's value changes
        /// </summary>
        public bool AutoCycleSliderImage
        {
            get { return bolAutoCycleSliderImage; }
            set { bolAutoCycleSliderImage = value; }
        }

        int intSlider_BitmapIndex = 0;
        /// <summary>
        /// index of the bmpSlider[] to be drawn
        /// </summary>
        public int Slider_BitmapIndex
        {
            get { return intSlider_BitmapIndex; }
            set
            {
                if (value < 0 || value >= bmpSlider.Length) return;

                bool bolReset = value != intSlider_BitmapIndex;
                intSlider_BitmapIndex = value;
                if (bolReset) Draw();
            }
        }

        Bitmap[] _bmpSlider = null;
        /// <summary>
        /// images of the slider that are drawn on the screen sequentially to animate the slider as it moves from left to right
        /// </summary>
        public Bitmap[] bmpSlider
        {
            get { return _bmpSlider; }
            set
            {
                _bmpSlider = value;
                foreach (Bitmap bmp in _bmpSlider)
                    if (bmp != null)
                        bmp.MakeTransparent(bmp.GetPixel(0, 0));
            }
        }

        Point _ptDraw_Value = new Point();
        /// <summary>
        /// point on the HSlider where the Value of the HSlider is to be drawn
        /// </summary>
        public Point ptDraw_Value
        {
            get { return _ptDraw_Value; }
            set
            {
                bool bolReset = (_ptDraw_Value.X != value.X || _ptDraw_Value.Y != value.Y);
                _ptDraw_Value = value;
                if (bolReset) Draw();
            }
        }

        Point _ptDraw_Text = new Point();
        /// <summary>
        /// point on the HSlider where the text is to be drawn
        /// </summary>
        public Point ptDraw_Text
        {
            get { return _ptDraw_Text; }
            set
            {
                bool bolReset = (_ptDraw_Text.X != value.X || _ptDraw_Text.Y != value.Y);
                _ptDraw_Text = value;
                if (bolReset) Draw();
            }
        }

        Brush br = Brushes.Black;
        /// <summary>
        /// brush used to draw both the Text and the Value of the HSlider
        /// </summary>
        public Brush brush
        {
            get { return br; }
            set
            {
                bool bolReset = (br != value);
                br = value;
                if (bolReset) Draw();
            }
        }

        Font fnt = new Font("Courier", 12);
        /// <summary>
        /// font used to draw both the name and the text of the HSlider
        /// </summary>
        public Font font
        {
            get { return fnt; }
            set
            {
                bool bolReset = (fnt != value);
                fnt = value;
                if (bolReset) Draw();
            }
        }




        #endregion

        #region Functions
        void evaluateNewBackground()
        {
            if (bmpBackground == null) return;
            intGraph_Height = new int[bmpBackground_Graph.Width];
            int intY;
            Color clrPixel = bmpBackground_Graph.GetPixel(0, 0);// bolTopReference ? 0 : bmpBackground_Graph.Height - 1);

            for (int intX = 1; intX < bmpBackground_Graph.Width; intX++)
            {
                for (intY = 0; intY < bmpBackground_Graph.Height && bmpBackground_Graph.GetPixel(intX, intY) != clrPixel; intY++) ;
                intGraph_Height[intX] = intY;
            }

            intGraph_Height[0] = intGraph_Height[1];
        }

        void setAspectRatio() { dblAspectRatio = Width / Height; }//Draw(); }

        public void refresh()
        {
            Slider_Refresh();
            Draw();
        }

        void Draw()
        {
            bmpCurrent = new Bitmap(Size.Width, Size.Height);
            Bitmap bmpSlider_Screen = new Bitmap(bmpSlider[intSlider_BitmapIndex].Width, bmpSlider[intSlider_BitmapIndex].Height);

            using (Graphics g = Graphics.FromImage(bmpSlider_Screen))
            {
                Rectangle recSource = new Rectangle(new Point(0, 0), bmpSlider[intSlider_BitmapIndex].Size);
                Rectangle recDestination = new Rectangle(new Point(0, 0), bmpSlider_Screen.Size);
                g.DrawImage(bmpSlider[intSlider_BitmapIndex], recDestination, recSource, GraphicsUnit.Pixel);
            }
            using (Graphics g = Graphics.FromImage(bmpCurrent))
            {
                Rectangle recSource = new Rectangle(new Point(0, 0), bmpBackground.Size);
                Rectangle recDestination = new Rectangle(new Point(0, 0), Size);
                g.DrawImage(bmpBackground, recDestination, recSource, GraphicsUnit.Pixel);

                double dblSliderScale = (double)(recDestination.Width + recDestination.Height) / (double)(recSource.Width + recSource.Height)
                                           * (dblSlider_Scale_Right + (dblSlider_Scale_Left - dblSlider_Scale_Right) * ((double)(recDestination.Width - ptSlider_Screen.X) / (double)recDestination.Width));
                recSource = new Rectangle(new Point(0, 0), bmpSlider[intSlider_BitmapIndex].Size);
                Size szDestination = new Size((int)((double)bmpSlider[intSlider_BitmapIndex].Width * dblSliderScale), (int)((double)bmpSlider[intSlider_BitmapIndex].Height * dblSliderScale));
                Point ptDrawSlider = new Point(ptSlider_Screen.X - szDestination.Width / 2, ptSlider_Screen.Y - (int)((double)szDestination.Height * .8));

                recDestination = new Rectangle(ptDrawSlider, szDestination);
                g.DrawImage(bmpSlider_Screen, recDestination, recSource, GraphicsUnit.Pixel);

                if (bmpForeground != null)
                {
                    recSource = new Rectangle(new Point(0, 0), bmpForeground.Size);
                    recDestination = new Rectangle(new Point(0, 0), Size);
                    g.DrawImage(bmpForeground, recDestination, recSource, GraphicsUnit.Pixel);
                }

                if (DisplayText)
                    if (strText != null && strText.Length > 0)
                    {
                        szText = TextRenderer.MeasureText(strText, fnt);
                        g.DrawString(strText, fnt, br, ptDraw_Text);
                    }
                if (DisplayValue)
                {
                    string strValue = Value.ToString();
                    szValue = TextRenderer.MeasureText(strValue, fnt);
                    g.DrawString(strValue, fnt, br, ptDraw_Value);
                }

            }

            Image = bmpCurrent;
            Refresh();
        }
        #endregion
    }
    public class groupBox : Panel
    {
        public enum enuDrawBorder
        {
            Top_Left,
            Top_Center,
            Top_Right,
            Right_Top,
            Right_Center,
            Right_Bottom,
            Bottom_Right,
            Bottom_Center,
            Bottom_Left,
            Left_Bottom,
            Left_Center,
            Left_Top,
            _num
        };
        enuDrawBorder _DrawBorderStyle = enuDrawBorder.Top_Left;
        public enuDrawBorder DrawBorderStyle
        {
            get { return _DrawBorderStyle; }
            set
            {
                if (_DrawBorderStyle != value)
                {
                    _DrawBorderStyle = value;
                    drawImage();
                }
            }
        }

        Font fnt = new Font("sans serif", 8);
        public Font font
        {
            get { return fnt; }
            set
            {
                if (fnt != value)
                {
                    fnt = value;
                    drawImage();
                }
            }
        }

        string strText = "group box";
        override public string Text
        {
            get { return strText; }
            set
            {
                strText = value;
                drawImage();
            }
        }

        Color clrBack = Color.LightGray;
        override public Color BackColor
        {
            get { return clrBack; }
            set
            {
                clrBack = value;
                drawImage();
            }
        }

        Color clrFore = Color.Black;
        override public Color ForeColor
        {
            get { return clrFore; }
            set
            {
                clrFore = value;
                drawImage();
            }
        }

        Bitmap bmpBack = null;
        void drawImage()
        {
            bmpBack = new Bitmap(Width, Height);
            Bitmap bmpText = null;

            Size szText = TextRenderer.MeasureText(Text, fnt);
            bmpText = new Bitmap(szText.Width, szText.Height);

            using (Graphics g = Graphics.FromImage(bmpText))
            {
                g.FillRectangle(new SolidBrush(clrBack), new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                g.DrawString(Text, fnt, new SolidBrush(clrFore), new Point(0, 0));
            }
            int intBorderSpacing = szText.Height / 2;
            int intBorderEdge = 3;
            recArea_Contents = new Rectangle(szText.Height + intBorderEdge,
                                             szText.Height + intBorderEdge,
                                             Width - (szText.Height + intBorderEdge),
                                             Height - (szText.Height + intBorderEdge));

            Pen pBorder = new Pen(clrFore, 2);
            using (Graphics g = Graphics.FromImage(bmpBack))
            {
                // draw Corners 
                {
                    // top-left 
                    {
                        int intAngle_Start = 180;
                        g.DrawArc(pBorder, new Rectangle(intBorderSpacing, intBorderSpacing, 2 * intBorderSpacing, 2 * intBorderSpacing), intAngle_Start, 90);
                    }

                    // top-right 
                    {
                        int intAngle_Start = 270;
                        g.DrawArc(pBorder, new Rectangle(Width - 3 * intBorderSpacing, intBorderSpacing, 2 * intBorderSpacing, 2 * intBorderSpacing), intAngle_Start, 90);

                    }

                    // Bottom-Right
                    {
                        int intAngle_Start = 0;
                        g.DrawArc(pBorder, new Rectangle(Width - 3 * intBorderSpacing, Height - 3 * intBorderSpacing, 2 * intBorderSpacing, 2 * intBorderSpacing), intAngle_Start, 90);
                    }

                    // Bottom-Left
                    {
                        int intAngle_Start = 90;
                        g.DrawArc(pBorder, new Rectangle(intBorderSpacing, Height - 3 * intBorderSpacing, 2 * intBorderSpacing, 2 * intBorderSpacing), intAngle_Start, 90);
                    }
                }


                // draw sides 
                {
                    // draw-top
                    {
                        Point ptTL = new Point(2 * intBorderSpacing, intBorderSpacing);
                        Point ptTR = new Point(Width - 2 * intBorderSpacing, intBorderSpacing);
                        g.DrawLine(pBorder, ptTL, ptTR);
                    }

                    // draw-right
                    {
                        Point ptTR = new Point(Width - intBorderSpacing, 2 * intBorderSpacing);
                        Point ptBR = new Point(Width - intBorderSpacing, Height - 2 * intBorderSpacing);
                        g.DrawLine(pBorder, ptTR, ptBR);
                    }

                    // draw-Bottom
                    {
                        Point ptBR = new Point(Width - 2 * intBorderSpacing, Height - intBorderSpacing);
                        Point ptBL = new Point(2 * intBorderSpacing, Height - intBorderSpacing);
                        g.DrawLine(pBorder, ptBR, ptBL);
                    }

                    // draw-Left
                    {
                        Point ptBL = new Point(intBorderSpacing, Height - 2 * intBorderSpacing);
                        Point ptTL = new Point(intBorderSpacing, 2 * intBorderSpacing);
                        g.DrawLine(pBorder, ptBL, ptTL);
                    }
                }

                Point ptDrawText = new Point();

                switch (DrawBorderStyle)
                {
                    case enuDrawBorder.Bottom_Center:
                        {
                            ptDrawText = new Point((Width - bmpText.Width) / 2, Height - bmpText.Height);
                        }
                        break;
                    case enuDrawBorder.Bottom_Left:
                        {
                            ptDrawText = new Point(3 * intBorderSpacing, Height - bmpText.Height);
                        }
                        break;
                    case enuDrawBorder.Bottom_Right:
                        {
                            ptDrawText = new Point(Width - 3 * intBorderSpacing - bmpText.Width, Height - bmpText.Height);
                        }
                        break;
                    case enuDrawBorder.Top_Left:
                        {
                            ptDrawText = new Point(3 * intBorderSpacing, 0);
                        }
                        break;
                    case enuDrawBorder.Top_Center:
                        {
                            ptDrawText = new Point((Width - bmpText.Width) / 2, 0);
                        }
                        break;
                    case enuDrawBorder.Top_Right:
                        {
                            ptDrawText = new Point(Width - 3 * intBorderSpacing - bmpText.Width, 0);
                        }
                        break;

                    case enuDrawBorder.Left_Top:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            ptDrawText = new Point(0, 3 * intBorderSpacing);
                        }
                        break;
                    case enuDrawBorder.Left_Center:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            ptDrawText = new Point(0, (Height - bmpText.Height) / 2);
                        }
                        break;
                    case enuDrawBorder.Left_Bottom:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            ptDrawText = new Point(0, Height - bmpText.Height - 3 * intBorderSpacing);
                        }
                        break;
                    case enuDrawBorder.Right_Top:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            ptDrawText = new Point(Width - bmpText.Width, 3 * intBorderSpacing);
                        }
                        break;
                    case enuDrawBorder.Right_Center:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            ptDrawText = new Point(Width - bmpText.Width, (Height - bmpText.Height) / 2);
                        }
                        break;
                    case enuDrawBorder.Right_Bottom:
                        {
                            bmpText.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            ptDrawText = new Point(Width - bmpText.Width, Height - bmpText.Height - 3 * intBorderSpacing);
                        }
                        break;
                }
                g.DrawImage(bmpText, ptDrawText);
                recArea_Text = new Rectangle(ptDrawText, bmpText.Size);
            }
            BackgroundImage = bmpBack;
        }

        Rectangle recArea_Contents = new Rectangle();
        public Rectangle Area_Contents
        {
            get { return recArea_Contents; }
        }

        Rectangle recArea_Text = new Rectangle();
        public Rectangle Area_Text
        {
            get { return recArea_Text; }
        }

        public groupBox()
        {
            drawImage();
            SizeChanged += GroupBox_SizeChanged;
        }

        private void GroupBox_SizeChanged(object sender, EventArgs e)
        {
            drawImage();
        }
    }
    public class MyNumericUpDown : NumericUpDown
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var e2 = new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta / SystemInformation.MouseWheelScrollLines);

            base.OnMouseWheel(e2);
        }
    }
    public class classSweepAndPrune
    {
        public List<classElement> lstElements = new List<classElement>();
        
        public class classElement
        {
            public Rectangle rec = new Rectangle();
            public object obj;
            public object tag;
        }

        public void Add(ref classElement cEle)
        {
            if (!lstElements.Contains(cEle))
            {
                lstElements.Add(cEle);
                Reset();
            }
        }

        public void Sub(ref classElement cEle)
        {
            if (lstElements.Contains(cEle))
            {
                lstElements.Remove(cEle);
                Reset();
            }
        }

        public classElement Search(Point pt)
        {
            classElement[] arrTemp = new classElement[lstElements.Count];
            lstElements.CopyTo(arrTemp);

            List<classElement> lstH = Search_H(pt);

            if (lstH != null && lstH.Count > 0)
            {
                List<classElement> lstV = Search_V(pt);
                if (lstV != null && lstV.Count > 0)
                {
                    List<classElement> lstShortest = null;
                    List<classElement> lstLongest = null;
                    if (lstV.Count != lstH.Count)
                    {
                        lstShortest = lstV.Count < lstH.Count
                                                 ? lstV
                                                 : lstH;
                        lstLongest = lstV.Count > lstH.Count
                                                ? lstV
                                                : lstH;
                    }
                    else
                    {
                        lstShortest = lstV;
                        lstLongest = lstH;
                    }
                    for (int intCounter = 0; intCounter < lstShortest.Count; intCounter++)
                    {
                        if (lstLongest.Contains(lstShortest[intCounter]))
                        {
                            lstElements = arrTemp.ToList<classElement>();
                            return lstShortest[intCounter];
                        }
                    }
                }
            }
            lstElements = arrTemp.ToList<classElement>();
            return null;
        }

        public List<classElement> Search(Rectangle rec) { return Search(rec.Location, new Point(rec.Right, rec.Bottom)); }
        public List<classElement> Search(Point ptTL, Point ptBR)
        {
            classElement[] arrTemp = new classElement[lstElements.Count];
            lstElements.CopyTo(arrTemp);

            List<classElementFind> lstH = Search_H(ptTL, ptBR);
            List<classElement> lstRetVal = new List<classElement>();
            if (lstH != null && lstH.Count > 0)
            {
                List<classElementFind> lstV = Search_V(ptTL, ptBR);
                if (lstV != null && lstV.Count > 0)
                {
                    for (int intHCounter = 0; intHCounter < lstH.Count; intHCounter++)
                    {
                        classElementFind cHEF = lstH[intHCounter];
                        for (int intHE = 0; intHE < cHEF.lstElements.Count; intHE++)
                        {
                            classElement cHE = cHEF.lstElements[intHE];
                            if (!lstRetVal.Contains(cHE))
                            {
                                bool bolFound = false;
                                for (int intVCounter = 0; !bolFound && intVCounter < lstV.Count; intVCounter++)
                                {
                                    classElementFind cVEF = lstV[intVCounter];
                                    for (int intVE = 0; !bolFound && intVE < cVEF.lstElements.Count; intVE++)
                                    {
                                        classElement cVE = cVEF.lstElements[intVE];
                                        if (cHE == cVE)
                                        {
                                            bolFound = true;
                                            lstRetVal.Add(cHE);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            lstElements = arrTemp.ToList<classElement>();
            return lstRetVal;
        }

        public void Reset()
        {
            lstHScan
                = lstVScan
                = null;
        }

        List<classElementFind> lstHScan = null;
        List<classElementFind> lstVScan = null;

        List<classElement> Search_H(Point pt)
        {
            Build_Scan_H();
            if (lstHScan == null) Build_Scan_H();

            classElementFind cBFHFind = getBFElementContains(ref lstHScan, pt.X);
            if (cBFHFind != null) return cBFHFind.lstElements;
            return null;
        }
        List<classElementFind> Search_H(Point ptTL, Point ptBR)
        {
            Build_Scan_H();
            if (lstHScan == null) Build_Scan_H();

            List<classElementFind> lstBFHFind = getBFElementContains(ref lstHScan, ptTL.X, ptBR.X);
            if (lstBFHFind != null) return lstBFHFind;
            return null;
        }

        List<classElement> Search_V(Point pt)
        {
            //return lstElements;
            if (lstVScan == null) Build_Scan_V();

            classElementFind cBFVFind = getBFElementContains(ref lstVScan, pt.Y);
            if (cBFVFind != null) return cBFVFind.lstElements;
            return null;
        }

        

        List<classElementFind> Search_V(Point ptTL, Point ptBR)
        {
            //return lstElements;
            if (lstVScan == null) Build_Scan_V();

            List<classElementFind> lstBFVFind = getBFElementContains(ref lstVScan, ptTL.Y, ptBR.Y);
            if (lstBFVFind != null) return lstBFVFind;
            return null;
        }


        void Build_Scan_H()
        {
            // initialize list classElementFind elements
            {
                lstHScan = new List<classElementFind>();
                IEnumerable<classElement> query = lstElements.OrderBy(Element => Element.rec.Left);
                lstElements = (List<classElement>)query.ToList<classElement>();
                for (int intCounter = 0; intCounter < lstElements.Count; intCounter++)
                {
                    classElementFind cBFNew = new classElementFind();
                    classElement cEle = lstElements[intCounter];
                    cBFNew.intStart = cEle.rec.Left;
                    cBFNew.intEnd = cEle.rec.Right;
                    cBFNew.lstElements.Add(cEle);
                    lstHScan.Add(cBFNew);
                }
            }
            BuildScan(ref lstHScan);
        }

        void Build_Scan_V()
        {
            // initialize list classElementFind elements
            {
                lstVScan = new List<classElementFind>();

                IEnumerable <classElement> query = lstElements.OrderBy(Element => Element.rec.Top);
                lstElements = (List<classElement>)query.ToList<classElement>();
                for (int intCounter = 0; intCounter < lstElements.Count; intCounter++)
                {
                    classElementFind cBFNew = new classElementFind();
                    classElement cEle = lstElements[intCounter];
                    cBFNew.intStart = cEle.rec.Top;
                    cBFNew.intEnd = cEle.rec.Bottom;
                    cBFNew.lstElements.Add(cEle);
                    lstVScan.Add(cBFNew);
                }
            }
            BuildScan(ref lstVScan);
        }

        void BuildScan(ref List<classElementFind> lstScan)
        {
            if (lstScan.Count >0)
            // collate/expand list of classElementFind elements
            {
                classElementFind cBFMarker = null;
                int intIndex = 0;
                while (intIndex < lstScan.Count - 1)
                {
                    IEnumerable<classElementFind> queryHScan = lstScan.OrderBy(cBF => cBF.intStart);
                    lstScan = (List<classElementFind>)queryHScan.ToList<classElementFind>();
                    if (cBFMarker == null)
                    {
                        cBFMarker = lstScan[0];
                        intIndex = 0;
                    }
                    else
                        intIndex = lstScan.IndexOf(cBFMarker);

                    // gather elements in list that share the same start value
                    List<classElementFind> lstCommonStart = new List<classElementFind>();
                    do
                    {
                        if (lstScan[intIndex].intStart != lstScan[intIndex].intEnd)
                            lstCommonStart.Add(lstScan[intIndex]);
                        intIndex++;
                    } while (intIndex < lstScan.Count && lstScan[intIndex].intStart == cBFMarker.intStart);

                    // test if more than one element share the same start value
                    if (lstCommonStart.Count > 1)
                    {
                        IEnumerable<classElementFind> queryCommonStart = lstCommonStart.OrderBy(Element => Element.intEnd);
                        lstCommonStart = (List<classElementFind>)queryCommonStart.ToList<classElementFind>();

                        // reorder them in the source list according to their End values
                        int intStartIndex = lstScan.IndexOf(cBFMarker);
                        classElementFind[] arrStart = new classElementFind[intStartIndex];
                        classElementFind[] arrEnd = new classElementFind[lstScan.Count - intStartIndex - lstCommonStart.Count];
                        lstScan.CopyTo(0, arrStart, 0, intStartIndex);
                        lstScan.CopyTo(intStartIndex + lstCommonStart.Count, arrEnd, 0, lstScan.Count - intStartIndex - lstCommonStart.Count);

                        List<classElementFind> lstTest = new List<classElementFind>();
                        lstTest.AddRange(arrStart.ToList<classElementFind>());
                        lstTest.AddRange(lstCommonStart);
                        lstTest.AddRange(arrEnd.ToList<classElementFind>());

                        lstScan = new List<classElementFind>();
                        lstScan.AddRange(arrStart.ToList<classElementFind>());
                        lstScan.AddRange(lstCommonStart);
                        lstScan.AddRange(arrEnd.ToList<classElementFind>());

                        classElementFind cBFShortest = lstCommonStart[0];
                        lstCommonStart.Remove(cBFShortest);

                        // gather all Elements into the shortest element
                        for (int intCounter = 0; intCounter < lstCommonStart.Count; intCounter++)
                            cBFShortest.lstElements.AddRange(lstCommonStart[intCounter].lstElements);

                        // remove all elements that are the same length as the shortest
                        classElementFind cBFNext = lstCommonStart[0];
                        while (lstCommonStart.Count > 0
                                && cBFNext.intEnd == cBFShortest.intEnd)
                        {
                            lstCommonStart.Remove(cBFNext);
                            lstScan.Remove(cBFNext);
                            cBFNext = lstCommonStart.Count > 0 ? lstCommonStart[0] : null;
                        }

                        // adjust start value of all remaining elements in list 
                        for (int intCounter = 0; intCounter < lstCommonStart.Count; intCounter++)
                            lstCommonStart[intCounter].intStart = cBFShortest.intEnd;

                        cBFMarker = cBFShortest;
                        intIndex = lstScan.IndexOf(cBFMarker);
                    }
                    else if (lstCommonStart.Count > 0)
                    { // there is only one element with this start value
                        classElementFind cBFCurrent = lstCommonStart[0];
                        // test if next element in list starts before this one ends
                        intIndex = lstScan.IndexOf(cBFCurrent);
                        if (intIndex < lstScan.Count - 1)
                        {
                            classElementFind cBFNext = lstScan[intIndex + 1]; ;
                            if (cBFCurrent.intEnd > cBFNext.intStart)
                            { // this one overlaps with its neighbour - needs to be cut in two and inserted into the list
                                classElementFind cBFNew = new classElementFind();
                                cBFNew.intStart = cBFNext.intStart;
                                cBFNew.intEnd = cBFCurrent.intEnd;
                                cBFNew.lstElements.AddRange(cBFCurrent.lstElements);

                                cBFCurrent.intEnd = cBFNext.intStart;

                                lstScan.Add(cBFNew);
                            }
                            else
                                cBFMarker = lstScan[intIndex + 1];
                        }
                    }
                    else
                        break;
                }
            }
        }

        classElementFind getBFElementContains(ref List<classElementFind> lst, int intFind)
        {
            if (lst == null) return null;
            if (lst.Count == 0) return null;
            int intIndex = lst.Count / 2;
            int intStep = lst.Count / 2;
            List<classElementFind> lstTried = new List<classElementFind>();
            do
            {
                classElementFind cBF = lst[intIndex];
                if (lstTried.Contains(cBF)) return null;

                if (intFind >= cBF.intStart && intFind <= cBF.intEnd)
                    return cBF;
                lstTried.Add(cBF);
                intStep /= 2;
                if (intStep < 1)
                    intStep = 1;
                intIndex += intStep * (intFind < cBF.intStart ? -1 : 1);

            } while (intIndex >= 0 && intIndex < lst.Count);
            return null;
        }
        
        List<classElementFind> getBFElementContains(ref List<classElementFind> lst, int intFind_Min, int intFind_Max)
        {
            if (lst == null) return null;
            if (lst.Count == 0) return null;
            int intIndex = lst.Count / 2;
            int intStep = lst.Count / 2;
            List<classElementFind> lstTried = new List<classElementFind>();
            int intIndex_Min = (lst.Count > 0 && lst[0].intStart > intFind_Min)
                                            ? 0
                                            : -1;
                int intIndex_Max =( lst.Count >0 && lst[lst.Count -1].intEnd < intFind_Max  )
                                            ? lst.Count -1
                                            : -1;
            bool bolLoop_Min = true;
            do
            {
                classElementFind cBF = lst[intIndex];
                if (lstTried.Contains(cBF)) bolLoop_Min = false;// return null;

                //if (intFind_Min >= cBF.intStart && intFind_Min <= cBF.intEnd)
                if (intFind_Min <= cBF.intEnd)
                {
                    intIndex_Min = intIndex;
                    //bolLoop_Min = false;
                }
                lstTried.Add(cBF);
                intStep /= 2;
                if (intStep < 1)
                    intStep = 1;
                intIndex += intStep * (intFind_Min < cBF.intStart ? -1 : 1);

            } while (intIndex >= 0 && intIndex < lst.Count && bolLoop_Min);

            bool bolLoop_Max = true;
            intIndex = lst.Count / 2;
            lstTried.Clear();
            do
            {
                classElementFind cBF = lst[intIndex];
                if (lstTried.Contains(cBF)) bolLoop_Max = false;// return null;

                if (intFind_Max >= cBF.intStart)// && intFind_Max <= cBF.intEnd)
                {
                    intIndex_Max = intIndex;
                    //bolLoop_Max = false;
                }
                lstTried.Add(cBF);
                intStep /= 2;
                if (intStep < 1)
                    intStep = 1;
                intIndex += intStep * (intFind_Max < cBF.intStart ? -1 : 1);

            } while (intIndex >= 0 && intIndex < lst.Count && bolLoop_Max);

            if (intIndex_Min >= 0 && intIndex_Max >= 0 && intIndex_Min <= intIndex_Max)
            {
                List<classElementFind> lstRetVal = new List<classElementFind>();
                for (int intCounter = intIndex_Min; intCounter <= intIndex_Max; intCounter++)
                    lstRetVal.Add(lst[intCounter]);
                return lstRetVal;
            }

            return null;
        }


        static Random rnd = new Random();

        static int RndInt(int intMin, int intMax)
        {
            if (intMax < intMin) return 0;
            int intDifference = intMax - intMin;
            return (int)((rnd.NextDouble() * (double)intDifference * 4.0) % intDifference);
        }


        public class classElementFind
        {
            public List<classElement> lstElements = new List<classElement>();
            int _intStart = 0;
            public int intStart
            {
                get { return _intStart; }
                set
                {
                    if (intStart != value)
                    {
                        _intStart = value;
                    }
                }
            }

            int _intEnd = 0;
            public int intEnd
            {
                get { return _intEnd; }
                set
                {
                    if (intEnd != value)
                    {
                        _intEnd = value;
                    }
                }
            }
            public classElementFind Copy()
            {
                classElementFind cRetVal = new classElementFind();

                cRetVal.intStart = intStart;
                cRetVal.intEnd = intEnd;
                cRetVal.lstElements.AddRange(lstElements);

                return cRetVal;
            }
        }
    }
    public class classRTX
    {
        public class classRTX_FontInfo
        {
            public Font fnt = new Font("ms sans-serif", 8, FontStyle.Regular);
            public Color clrFore = Color.Black;
            public Color clrBack = Color.White;
            public int intIndent = 0;

            string _strText = "";
            public string strText
            {
                get { return _strText; }
                set
                {
                    _strText = value;
                    _intLength = _strText.Length;
                }
            }

            int _intLength = 0;
            public int intLength
            {
                get { return _intLength; }
            }

            public int intStartIndex;

            public classRTX_FontInfo() { }
            public classRTX_FontInfo(string _strText)
            {
                strText = _strText;
            }
            public classRTX_FontInfo(string _strText, Font _fnt)
            {
                strText = _strText;
                fnt = _fnt;
            }
            public classRTX_FontInfo(string _strText, Font _fnt, Color _clrFore)
            {
                strText = _strText;
                fnt = _fnt;
                clrFore = _clrFore;
            }
            public classRTX_FontInfo(string _strText, Font _fnt, Color _clrFore, Color _clrBack)
            {
                strText = _strText;
                fnt = _fnt;
                clrFore = _clrFore;
                clrBack = _clrBack;
            }

            public classRTX_FontInfo Copy()
            {
                classRTX_FontInfo cRetVal = new classRTX_FontInfo();
                cRetVal.clrBack = clrBack;
                cRetVal.clrFore = clrFore;
                cRetVal.fnt = fnt;
                cRetVal.strText = strText;
                cRetVal.intIndent = intIndent;
                return cRetVal;
            }

        }

        List<classRTX_FontInfo> lstRTXFonts = new List<classRTX_FontInfo>();
        public RichTextBox _rtx = new RichTextBox();
        public void clear()
        {
            lstRTXFonts.Clear();
            //HTML_to_RTF_Writer.classFontDefinitions.InitList();
            _rtx.Text = "";
        }

  
        public void AppendText(string strText, Font fnt, Color clrFore)
        {
            classRTX_FontInfo cRtxFont = new classRTX_FontInfo(strText, fnt, clrFore);
            cRtxFont.intStartIndex = _rtx.Text.Length;

            _rtx.Select(_rtx.Text.Length, 0);
            _rtx.SelectionFont = fnt;
            _rtx.SelectionColor = clrFore;
            _rtx.SelectedText = strText;

            //lstRTXFonts.Add(cRtxFont);
        }

        public void Append_NL()
        {

            _rtx.Select(_rtx.Text.Length, 0);
            _rtx.SelectedText = "\r\n";
        }

        public void Display(ref RichTextBox rtx)
        {
            rtx.Text = "";
            for (int intCounter = 0; intCounter < lstRTXFonts.Count; intCounter++)
            {
                rtx.AppendText(lstRTXFonts[intCounter].strText);
                rtx.Refresh();
            }
            for (int intCounter = 0; intCounter < lstRTXFonts.Count; intCounter++)
            {
                classRTX_FontInfo cRTXFont = lstRTXFonts[intCounter];
                rtx.Select(cRTXFont.intStartIndex, cRTXFont.intLength);
                rtx.SelectionFont = cRTXFont.fnt;
                rtx.SelectionColor = cRTXFont.clrFore;
                rtx.SelectionBackColor = cRTXFont.clrBack;
                rtx.SelectionHangingIndent = cRTXFont.intIndent;
                rtx.Refresh();
            }
        }

        public void SaveToFile(string strFilename)
        {
            Display(ref _rtx);
            _rtx.SaveFile(strFilename);
        }

    }

}
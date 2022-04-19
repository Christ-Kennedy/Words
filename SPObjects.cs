using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;


namespace SPObjects
{
    public enum enuSweepPrune_ObjectType
    {
        Button,
        ButtonControl,
        CheckBox,
        CheckedListBox,
        ComboBox,
        getPoint,
        getRectangle,
        getSize,
        GroupBox,
        Label,
        ListBox,
        numericUpDown,
        Panel,
        PictureBox,
        ProgressBar,
        SlideRuler,
        TextBox,
        TextDisplay,
        TrackBar,
        UserDefined,
        _num
    };


    public class SP_ListElement
    {
        public SP_ListElement(ref classSweepAndPrune_Element Parent)
        {
            this.Parent = Parent;
        }
        classSweepAndPrune_Element Parent = null;

        public List<classSweepAndPrune_Element> lst = new List<classSweepAndPrune_Element>();

        public void Add(ref classSweepAndPrune_Element cEle)
        {
            if (!lst.Contains(cEle))
            {
                lst.Add(cEle);
                cEle.Parent = Parent;
                cEle.Visible = Parent.Visible;
            }
        }

        public void Remove(ref classSweepAndPrune_Element cEle)
        {
            lst.Remove(cEle);
            cEle.Parent = null;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < lst.Count)
            {
                lst[index].Parent = null;
                lst.RemoveAt(index);
            }
        }

        public void Insert(int index, ref classSweepAndPrune_Element cEle)
        {
            if (index >= 0 && index < lst.Count)
                lst.Insert(index, cEle);
        }

        public bool Contains(ref classSweepAndPrune_Element cEle)
        {
            return lst.Contains(cEle);
        }

        public int Count { get { return lst.Count; } }
    }

    #region SweepAndPrune_Engine
    public class classSweepAndPrune_Element
    {
        public SPContainer SPContainer = null;
        classSweepAndPrune_Element cMyRef = null;
        public System.Windows.Forms.ContextMenu ContextMenu = null;
        public string Name = "MyName";
        public object obj = null;

        public object tag;

        static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID
        {
            get { return intID; }
        }

        #region Language

        List<classUILanguage_Element> lstLanguages = new List<classUILanguage_Element>();
        public classUILanguage_Element Language_Get(string strLanguage)
        {
            int intLanguageIndex = SPContainer.LanguageIndex_ByName(strLanguage);
            if (intLanguageIndex >= 0)
            {
                while (intLanguageIndex >= lstLanguages.Count)
                    lstLanguages.Add(new classUILanguage_Element());

                return lstLanguages[intLanguageIndex];
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("SPContainer(" + SPContainer.Name + ").cEle(" + Name + ") language :" + strLanguage + " was not found");
                return null;
            }
        }

        public void Tip_Set(string strTip) { Tip_Set(strTip, SPContainer.Language); }
        public void Tip_Set(string strTip, string strLanguage)
        {
            classUILanguage_Element cLanguage = Language_Get(strLanguage);
            if (cLanguage != null)
                cLanguage.Tip = strTip;
        }
        
        public string Tip_Get() { return Tip_Get(SPContainer.Language); }
        public string Tip_Get(string strLanguage)
        {
            classUILanguage_Element cLanguage = Language_Get(strLanguage);
            if (cLanguage != null)
                return cLanguage.Tip;
            return "";
        }

        public void Text_Set(string strText) { Text_Set(strText, SPContainer.Language); }

        public void Text_Set(string strText, string strLanguage)
        {
            classUILanguage_Element cLanguage = Language_Get(strLanguage);
            if (cLanguage != null)
                cLanguage.Text = strText;
        }

        public string Text_Get(string strLanguage)
        {
            classUILanguage_Element cLanguage = Language_Get(strLanguage);
            if (cLanguage != null)
                return cLanguage.Text;
            return "";
        }

        bool bolTextOverride = true;
        public bool TextOverride
        {
            get { return bolTextOverride; }
            set { bolTextOverride = value; }
        }
        
        public void Language_Set(string strLanguage)
        {
            Language_Set(SPContainer.LanguageIndex_ByName(strLanguage));
        }

        public void Language_Set(int intLanguageIndex)
        {
            if (TextOverride) return;
            while (lstLanguages.Count < intLanguageIndex)
                lstLanguages.Add(new classUILanguage_Element());
            classUILanguage_Element cLanguage = lstLanguages[intLanguageIndex];
            if (cLanguage != null)
            {
                switch (eSP_ObjectType)
                {

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblEle = (Label)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;



                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox lblEle = (PictureBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel lblEle = (Panel)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox lblEle = (CheckBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox lblEle = (ComboBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar lblEle = (ProgressBar)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lblEle = (ListBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox lblEle = (CheckedListBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button lblEle = (Button)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler lblEle = (SlideRuler)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl lblEle = (ButtonControl)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox lblEle = (Textbox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;
                        

                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay lblEle = (TextDisplay)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar lblEle = (TrackBar)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown lblEle = (numericUpDown)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox lblEle = (GroupBox)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint lblEle = (getPoint)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize lblEle = (getSize)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle lblEle = (getRectangle)obj;
                            if (lblEle != null)
                                lblEle.Text = cLanguage.Text;
                        }
                        break;


                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                      
                        }
                        break;


                }
            }
        }

        #endregion


        public classSweepAndPrune_Element Parent = null;
        /// <summary>
        /// list of SweepAndPrune Elements contained in this SPObject
        /// </summary>
        public SP_ListElement lstEle = null;

        bool bolNeedsToBeRedrawn = true;
        public bool NeedsToBeRedrawn
        {
            get { return bolNeedsToBeRedrawn; }
            set
            {

                bolNeedsToBeRedrawn = value;
                if (bolNeedsToBeRedrawn)
                {
                    bool bolBuilding = SPContainer.BuildingInProgress;
                    SPContainer.Building_Start();
                    {

                        if (Parent != null)
                            Parent.NeedsToBeRedrawn = true;

                        SPContainer.NeedsToBeRedrawn = true;
                    }
                    if (!bolBuilding)
                        SPContainer.Building_Complete();
                }
                else
                {

                    for (int intChildCounter = 0; intChildCounter < lstEle.lst.Count; intChildCounter++)
                    {
                        if (lstEle.lst[intChildCounter].NeedsToBeRedrawn)
                            lstEle.lst[intChildCounter].NeedsToBeRedrawn = false;
                    }
                }
            }
        }

        bool bolImageRefreshed = true;
        public bool ImageRefreshed
        {
            get { return bolImageRefreshed; }
            set { bolImageRefreshed = value; }
        }

        public void DoNotDrawChildElements()
        {
            for (int intElementCounter = 0; intElementCounter < lstEle.Count; intElementCounter++)
            {
                classSweepAndPrune_Element cChildElement = lstEle.lst[intElementCounter];
                cChildElement.NeedsToBeRedrawn = false;
                cChildElement.DoNotDrawChildElements();
            }
        }

        public void SendToBack()
        {
            SPContainer.lstElements.Remove(this);
            if (Parent != null)
            {
                SPContainer.lstElements.Insert(Parent.Index, this);
            }
            else
                SPContainer.lstElements.Insert(0, this);
        }

        public void BringToFront()
        {
            SPContainer.lstElements.Remove(this);
            SPContainer.lstElements.Add(this);

            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();
            {
                for (int intChildCounter = 0; intChildCounter < lstEle.Count; intChildCounter++)
                {
                    classSweepAndPrune_Element cEleChild = lstEle.lst[intChildCounter];
                    cEleChild.BringToFront();
                }
            }
            if (!bolBuilding)
                SPContainer.Building_Complete();
        }

        bool bolVisible = true;
        public bool Visible
        {
            get { return bolVisible; }
            set
            {
                if (bolVisible != value)
                {
                    bolVisible = value;
                    for (int intEleCounter = 0; intEleCounter < lstEle.Count; intEleCounter++)
                        lstEle.lst[intEleCounter].Visible = value;

                    if (Parent != null)
                        Parent.NeedsToBeRedrawn = true;

                    SPContainer SPContainer_Ref = SPContainer;
                    List<classSweepAndPrune_Element> lstVisible = classSweepAndPrune_Engine.Search(recSP, ref SPContainer_Ref);

                    for (int intItemCounter = 0; intItemCounter < lstVisible.Count; intItemCounter++)
                    {
                        classSweepAndPrune_Element cEleItem = lstVisible[intItemCounter];
                        cEleItem.NeedsToBeRedrawn = true;
                    }
                    if (!bolVisible)
                        SPContainer.lstElements_Removed = true;
                    NeedsToBeRedrawn = true;
                }
            }
        }

        enuSweepPrune_ObjectType _eSP_ObjectType = enuSweepPrune_ObjectType._num;
        public enuSweepPrune_ObjectType eSP_ObjectType
        {
            get {return _eSP_ObjectType; }
            set
            {
                _eSP_ObjectType = value;
            }
        }
        public classSweepAndPrune_Element(ref SPContainer SPContainer)
        {
            this.SPContainer = SPContainer;
            cMyRef = this;
            lstEle = new SP_ListElement(ref cMyRef);
        }

        public int Index
        {
            get { return SPContainer.lstElements.IndexOf(cMyRef); }
        }

        Rectangle _rec = new Rectangle(new Point(), SweepPrune_Object.szDefault);
        /// <summary>
        /// rectangle of object relative to its parent 
        /// </summary>
        public Rectangle rec
        {
            get
            {
                return _rec;
            }
            set
            {
                if (rec.Left != value.Left
                    || rec.Top != value.Top
                    || rec.Width != value.Width
                    || rec.Height != value.Height)
                {
                    _rec = value;
                    NeedsToBeRedrawn = true;
                    if (Parent != null)
                        Parent.NeedsToBeRedrawn = true;
                    SPContainer.Reset();
                }
            }
        }

        /// <summary>
        /// rectangle of object as seen by the Sweep & Prune algorithm
        /// </summary>
        public Rectangle recSP
        {
            get
            {
                if (Parent != null)
                    return new Rectangle(Parent.recSP.Left + rec.Left,
                                         Parent.recSP.Top + rec.Top,
                                         rec.Width,
                                         rec.Height);
                return _rec;
            }
        }

        /// <summary>
        /// rectangle of object relative to the most recent SPContainer.MyImage
        /// </summary>
        public Rectangle recDraw
        {
            get
            {
                if (SPContainer != null)
                {
                    Rectangle recRetVal = new Rectangle(recSP.Left - SPContainer.recVisible.Left,
                                                        recSP.Top - SPContainer.recVisible.Top,
                                                        recSP.Width,
                                                        recSP.Height);
                    return recRetVal;
                }                
                else
                    return rec;

            }
        }

        public void DrawSequence_Set()
        {
            for (int intSPEleCounter = 0; intSPEleCounter < lstEle.Count; intSPEleCounter++)
                SPContainer.lstElements.Remove(lstEle.lst[intSPEleCounter]);

            int intMyIndex = SPContainer.lstElements.IndexOf(this);
            for (int intSPEleCounter = 0; intSPEleCounter < lstEle.Count; intSPEleCounter++)
                SPContainer.lstElements.Insert(intMyIndex + 1, lstEle.lst[intSPEleCounter]);

            for (int intSPEleCounter = 0; intSPEleCounter < lstEle.Count; intSPEleCounter++)
                lstEle.lst[intSPEleCounter].DrawSequence_Set();
        }

        public Bitmap MyImage
        {
            get
            {
                switch (eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblEle = (Label)obj;
                            if (lblEle != null)
                            {
                                return lblEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picEle = (PictureBox)obj;
                            if (picEle != null)
                            {
                                return picEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel pnlEle = (Panel)obj;
                            if (pnlEle != null)
                            {
                                return pnlEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxEle = (CheckBox)obj;
                            if (chxEle != null)
                            {
                                return chxEle.MyImage;
                            }
                        }
                        break;


                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbEle = (ComboBox)obj;
                            if (cmbEle != null)
                            {
                                return cmbEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbEle = (ProgressBar)obj;
                            if (prbEle != null)
                            {
                                return prbEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxEle = (ListBox)obj;
                            if (lbxEle != null)
                            {
                                return lbxEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxEle = (CheckedListBox)obj;
                            if (cbxEle != null)
                            {
                                return cbxEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnEle = (Button)obj;
                            if (btnEle != null)
                            {
                                return btnEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler btnEle = (SlideRuler)obj;
                            if (btnEle != null)
                            {
                                return btnEle.MyImage;
                            }
                        }
                        break;
                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcEle = (ButtonControl)obj;
                            if (btcEle != null)
                            {
                                return btcEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtEle = (Textbox)obj;
                            if (txtEle != null)
                            {
                                return txtEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtEle = (TextDisplay)obj;
                            if (txtEle != null)
                            {
                                return txtEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbEle = (TrackBar)obj;
                            if (trbEle != null)
                            {
                                return trbEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown nudEle = (numericUpDown)obj;
                            if (nudEle != null)
                            {
                                return nudEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox grbEle = (GroupBox)obj;
                            if (grbEle != null)
                            {
                                return grbEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint gptEle = (getPoint)obj;
                            if (gptEle != null)
                            {
                                return gptEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize gszEle = (getSize)obj;
                            if (gszEle != null)
                            {
                                return gszEle.MyImage;
                            }
                        }
                        break;


                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle gszEle = (getRectangle)obj;
                            if (gszEle != null)
                            {
                                return gszEle.MyImage;
                            }
                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            return null;
                        }

                    default:
                        System.Windows.Forms.MessageBox.Show("SPObjects.classSweepAndPrune.MyImage property) : this should not happen");
                        break;
                }
                return null;
            }
        }

        Color _forecolor = Color.Black;
        public Color ForeColor
        {
            get
            {
                return _forecolor;
            }

            set
            {
                _forecolor = value;

                for (int intEleCounter = 0; intEleCounter < lstEle.Count; intEleCounter++)
                    lstEle.lst[intEleCounter].ForeColor = ForeColor;
            }
        }

        Color _backColor = Color.Gray;
        SolidBrush _sbrBackColor_Dull = new SolidBrush(Color.White);
        public SolidBrush sbrBackColor_Dull
        {
            get { return _sbrBackColor_Dull; }
        }


        Color _clrBackColor_Highlight = Color.Black;
        public Color BackColor_Highlight
        {
            get { return _clrBackColor_Highlight; }
            set
            {
                if (_clrBackColor_Highlight != value)
                {
                    _clrBackColor_Highlight = value;
                    _sbrBackColor_Highlight = new SolidBrush(_clrBackColor_Highlight);
                    NeedsToBeRedrawn = true;
                }
            }
        }

        SolidBrush _sbrBackColor_Highlight = new SolidBrush(Color.Black);
        public SolidBrush sbrBackColor_Highlight
        {
            get { return _sbrBackColor_Highlight; }
        }

        public Color BackColor
        {
            get
            {
                return _backColor;
            }

            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    _sbrBackColor_Dull = new SolidBrush(_backColor);
                    NeedsToBeRedrawn = true;
                    for (int intEleCounter = 0; intEleCounter < lstEle.Count; intEleCounter++)
                        lstEle.lst[intEleCounter].BackColor = BackColor;
                }
            }
        }


        EventHandler _eventDraw = null;
        public EventHandler eventDraw
        {
            get { return _eventDraw; }
            set { _eventDraw = value; }
        }
        

    }

    public class classSweepAndPrune_ElementFind
    {
        public List<classSweepAndPrune_Element> lstElements = new List<classSweepAndPrune_Element>();
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
        public classSweepAndPrune_ElementFind Copy()
        {
            classSweepAndPrune_ElementFind cRetVal = new classSweepAndPrune_ElementFind();

            cRetVal.intStart = intStart;
            cRetVal.intEnd = intEnd;
            cRetVal.lstElements.AddRange(lstElements);

            return cRetVal;
        }
    }

    public class classSweepAndPrune_Engine
    {
        static public void Add(ref classSweepAndPrune_Element cEle, ref SPContainer cSP_Data)
        {
            if (!cSP_Data.lstElements.Contains(cEle))
            {
                cSP_Data.lstElements.Add(cEle);
                Reset(ref cSP_Data);
            }
        }

        static public void Sub(ref classSweepAndPrune_Element cEle, ref SPContainer cSP_Data)
        {
            if (cSP_Data.lstElements.Contains(cEle))
            {
                cSP_Data.lstElements.Remove(cEle);
                Reset(ref cSP_Data);
            }
        }

        static public List<classSweepAndPrune_Element> Search(Point pt, ref SPContainer cSP_Data)
        {
            classSweepAndPrune_Element[] arrTemp = new classSweepAndPrune_Element[cSP_Data.lstElements.Count];
            cSP_Data.lstElements.CopyTo(arrTemp);

            List<classSweepAndPrune_Element> lstH = Search_H(pt, ref cSP_Data);

            if (lstH != null && lstH.Count > 0)
            {
                List<classSweepAndPrune_Element> lstV = Search_V(pt, ref cSP_Data);
                if (lstV != null && lstV.Count > 0)
                {
                    List<classSweepAndPrune_Element> lstShortest = null;
                    List<classSweepAndPrune_Element> lstLongest = null;
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
                    List<classSweepAndPrune_Element> lstRetVal = new List<classSweepAndPrune_Element>();
                    for (int intCounter = 0; intCounter < lstShortest.Count; intCounter++)
                    {
                        if (lstLongest.Contains(lstShortest[intCounter]))
                        {
                            cSP_Data.lstElements = arrTemp.ToList<classSweepAndPrune_Element>();
                            lstRetVal.Add(lstShortest[intCounter]);
                        }
                    }
                    return lstRetVal;
                }
            }
            cSP_Data.lstElements = arrTemp.ToList<classSweepAndPrune_Element>();
            return null;
        }

        static public List<classSweepAndPrune_Element> Search(Rectangle rec, ref SPContainer cSP_Data) { return Search(rec.Location, new Point(rec.Right, rec.Bottom), ref cSP_Data); }
        static public List<classSweepAndPrune_Element> Search(Point ptTL, Point ptBR, ref SPContainer cSP_Data)
        {
            classSweepAndPrune_Element[] arrTemp = new classSweepAndPrune_Element[cSP_Data.lstElements.Count];
            cSP_Data.lstElements.CopyTo(arrTemp);

            List<classSweepAndPrune_ElementFind> lstH = Search_H(ptTL, ptBR, ref cSP_Data);
            List<classSweepAndPrune_Element> lstRetVal = new List<classSweepAndPrune_Element>();
            if (lstH != null && lstH.Count > 0)
            {
                List<classSweepAndPrune_ElementFind> lstV = Search_V(ptTL, ptBR, ref cSP_Data);
                if (lstV != null && lstV.Count > 0)
                {
                    for (int intHCounter = 0; intHCounter < lstH.Count; intHCounter++)
                    {
                        classSweepAndPrune_ElementFind cHEF = lstH[intHCounter];
                        for (int intHE = 0; intHE < cHEF.lstElements.Count; intHE++)
                        {
                            classSweepAndPrune_Element cHE = cHEF.lstElements[intHE];
                            if (!lstRetVal.Contains(cHE))
                            {
                                bool bolFound = false;
                                for (int intVCounter = 0; !bolFound && intVCounter < lstV.Count; intVCounter++)
                                {
                                    classSweepAndPrune_ElementFind cVEF = lstV[intVCounter];
                                    for (int intVE = 0; !bolFound && intVE < cVEF.lstElements.Count; intVE++)
                                    {
                                        classSweepAndPrune_Element cVE = cVEF.lstElements[intVE];
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
            cSP_Data.lstElements = arrTemp.ToList<classSweepAndPrune_Element>();
            return lstRetVal;
        }

        static public void Reset(ref SPContainer cSP_Data)
        {
            cSP_Data.lstHScan
                = cSP_Data.lstVScan
                = null;
             
        }

        static List<classSweepAndPrune_Element> Search_H(Point pt, ref SPContainer cSP_Data)
        {
            if (cSP_Data.lstHScan == null) Build_Scan_H(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstHScan;
            classSweepAndPrune_ElementFind cBFHFind = getBFElementContains(ref lstTemp, pt.X);
            if (cBFHFind != null) return cBFHFind.lstElements;
            return null;
        }
        static List<classSweepAndPrune_ElementFind> Search_H(Point ptTL, Point ptBR, ref SPContainer cSP_Data)
        {
            if (cSP_Data.lstHScan == null) Build_Scan_H(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstHScan;
            List<classSweepAndPrune_ElementFind> lstBFHFind = getBFElementContains(ref lstTemp, ptTL.X, ptBR.X);
            if (lstBFHFind != null) return lstBFHFind;
            return null;
        }
        static List<classSweepAndPrune_Element> Search_V(Point pt, ref SPContainer cSP_Data)
        {
            if (cSP_Data.lstVScan == null) Build_Scan_V(ref cSP_Data);

            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstVScan;
            classSweepAndPrune_ElementFind cBFVFind = getBFElementContains(ref lstTemp, pt.Y);
            if (cBFVFind != null) return cBFVFind.lstElements;
            return null;
        }

        static List<classSweepAndPrune_ElementFind> Search_V(Point ptTL, Point ptBR, ref SPContainer cSP_Data)
        {
            if (cSP_Data.lstVScan == null) Build_Scan_V(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstVScan;
            List<classSweepAndPrune_ElementFind> lstBFVFind = getBFElementContains(ref lstTemp, ptTL.Y, ptBR.Y);
            if (lstBFVFind != null) return lstBFVFind;
            return null;
        }


        static void Build_Scan_H(ref SPContainer cSP_Data)
        {
            // initialize list classSweepAndPrune_ElementFind elements
            cSP_Data.lstHScan = new List<classSweepAndPrune_ElementFind>();
            IEnumerable<classSweepAndPrune_Element> query = cSP_Data.lstElements.OrderBy(Element => Element.recSP.Left);
            cSP_Data.lstElements = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
            for (int intCounter = 0; intCounter < cSP_Data.lstElements.Count; intCounter++)
            {
                classSweepAndPrune_ElementFind cBFNew = new classSweepAndPrune_ElementFind();
                classSweepAndPrune_Element cEle = cSP_Data.lstElements[intCounter];
                if (cEle.recSP.Width > 0)
                {
                    cBFNew.intStart = cEle.recSP.Left;
                    cBFNew.intEnd = cEle.recSP.Right;
                    cBFNew.lstElements.Add(cEle);
                    cSP_Data.lstHScan.Add(cBFNew);
                }
            }

            List<classSweepAndPrune_ElementFind> lstHScan_Ref = cSP_Data.lstHScan;
            //BuildScan(ref lstHScan_Ref);
            BuildScan(ref cSP_Data.lstHScan);
        }

        static void Build_Scan_V(ref SPContainer cSP_Data)
        {
            // initialize list classSweepAndPrune_ElementFind elements
            {
                cSP_Data.lstVScan = new List<classSweepAndPrune_ElementFind>();

                IEnumerable<classSweepAndPrune_Element> query = cSP_Data.lstElements.OrderBy(Element => Element.recSP.Top);
                cSP_Data.lstElements = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
                for (int intCounter = 0; intCounter < cSP_Data.lstElements.Count; intCounter++)
                {
                    classSweepAndPrune_ElementFind cBFNew = new classSweepAndPrune_ElementFind();
                    classSweepAndPrune_Element cEle = cSP_Data.lstElements[intCounter];
                    if (cEle.recSP.Height > 0)
                    {
                        cBFNew.intStart = cEle.recSP.Top;
                        cBFNew.intEnd = cEle.recSP.Bottom;

                        cBFNew.lstElements.Add(cEle);
                        cSP_Data.lstVScan.Add(cBFNew);
                    }
                }
            }

            List<classSweepAndPrune_ElementFind> lstVScan_Ref = cSP_Data.lstVScan;
            //BuildScan(ref lstVScan_Ref);
            BuildScan(ref cSP_Data.lstVScan);
        }

        static void BuildScan(ref List<classSweepAndPrune_ElementFind> lstScan)
        {
            if (lstScan.Count > 0)
            // collate/expand list of classSweepAndPrune_ElementFind elements
            {
                classSweepAndPrune_ElementFind cBFMarker = null;
                int intIndex = 0;
                while (intIndex < lstScan.Count - 1)
                {
                    IEnumerable<classSweepAndPrune_ElementFind> queryHScan = lstScan.OrderBy(cBF => cBF.intStart);
                    lstScan = (List<classSweepAndPrune_ElementFind>)queryHScan.ToList<classSweepAndPrune_ElementFind>();
                    if (cBFMarker == null)
                    {
                        cBFMarker = lstScan[0];
                        intIndex = 0;
                    }
                    else
                        intIndex = lstScan.IndexOf(cBFMarker);

                    // gather elements in list that share the same start value
                    List<classSweepAndPrune_ElementFind> lstCommonStart = new List<classSweepAndPrune_ElementFind>();
                    do
                    {
                        if (lstScan[intIndex].intStart != lstScan[intIndex].intEnd)
                            lstCommonStart.Add(lstScan[intIndex]);
                        intIndex++;
                    } while (intIndex < lstScan.Count && lstScan[intIndex].intStart == cBFMarker.intStart);

                    // test if more than one element share the same start value
                    if (lstCommonStart.Count > 1)
                    {
                        IEnumerable<classSweepAndPrune_ElementFind> queryCommonStart = lstCommonStart.OrderBy(Element => Element.intEnd);
                        lstCommonStart = (List<classSweepAndPrune_ElementFind>)queryCommonStart.ToList<classSweepAndPrune_ElementFind>();

                        // reorder them in the source list according to their End values
                        int intStartIndex = lstScan.IndexOf(cBFMarker);
                        classSweepAndPrune_ElementFind[] arrStart = new classSweepAndPrune_ElementFind[intStartIndex];
                        classSweepAndPrune_ElementFind[] arrEnd = new classSweepAndPrune_ElementFind[lstScan.Count - intStartIndex - lstCommonStart.Count];
                        lstScan.CopyTo(0, arrStart, 0, intStartIndex);
                        lstScan.CopyTo(intStartIndex + lstCommonStart.Count, arrEnd, 0, lstScan.Count - intStartIndex - lstCommonStart.Count);

                        List<classSweepAndPrune_ElementFind> lstTest = new List<classSweepAndPrune_ElementFind>();
                        lstTest.AddRange(arrStart.ToList<classSweepAndPrune_ElementFind>());
                        lstTest.AddRange(lstCommonStart);
                        lstTest.AddRange(arrEnd.ToList<classSweepAndPrune_ElementFind>());

                        lstScan = new List<classSweepAndPrune_ElementFind>();
                        lstScan.AddRange(arrStart.ToList<classSweepAndPrune_ElementFind>());
                        lstScan.AddRange(lstCommonStart);
                        lstScan.AddRange(arrEnd.ToList<classSweepAndPrune_ElementFind>());

                        classSweepAndPrune_ElementFind cBFShortest = lstCommonStart[0];
                        lstCommonStart.Remove(cBFShortest);

                        // gather all Elements into the shortest element
                        for (int intCounter = 0; intCounter < lstCommonStart.Count; intCounter++)
                            cBFShortest.lstElements.AddRange(lstCommonStart[intCounter].lstElements);

                        // remove all elements that are the same length as the shortest
                        classSweepAndPrune_ElementFind cBFNext = lstCommonStart[0];
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
                        classSweepAndPrune_ElementFind cBFCurrent = lstCommonStart[0];
                        // test if next element in list starts before this one ends
                        intIndex = lstScan.IndexOf(cBFCurrent);
                        if (intIndex < lstScan.Count - 1)
                        {
                            classSweepAndPrune_ElementFind cBFNext = lstScan[intIndex + 1]; ;
                            if (cBFCurrent.intEnd > cBFNext.intStart)
                            { // this one overlaps with its neighbour - needs to be cut in two and inserted into the list
                                classSweepAndPrune_ElementFind cBFNew = new classSweepAndPrune_ElementFind();
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

        static classSweepAndPrune_ElementFind getBFElementContains(ref List<classSweepAndPrune_ElementFind> lst, int intFind)
        {
            if (lst == null) return null;
            if (lst.Count == 0) return null;
            int intIndex = lst.Count / 2;
            int intStep = lst.Count / 2;
            List<classSweepAndPrune_ElementFind> lstTried = new List<classSweepAndPrune_ElementFind>();
            do
            {
                classSweepAndPrune_ElementFind cBF = lst[intIndex];
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

        static List<classSweepAndPrune_ElementFind> getBFElementContains(ref List<classSweepAndPrune_ElementFind> lst, int intFind_Min, int intFind_Max)
        {
            if (lst == null) return null;
            if (lst.Count == 0) return null;
            int intIndex = lst.Count / 2;
            int intStep = lst.Count / 2;
            List<classSweepAndPrune_ElementFind> lstTried = new List<classSweepAndPrune_ElementFind>();
            int intIndex_Min = (lst.Count > 0 && lst[0].intStart > intFind_Min)
                                            ? 0
                                            : -1;
            int intIndex_Max = (lst.Count > 0 && lst[lst.Count - 1].intStart < intFind_Max)
                                        ? lst.Count - 1
                                        : -1;
            bool bolLoop_Min = true;
            do
            {
                classSweepAndPrune_ElementFind cBF = lst[intIndex];
                if (lstTried.Contains(cBF)) bolLoop_Min = false;// return null;

                if (intFind_Min <= cBF.intEnd
                    && (intIndex < intIndex_Min || intIndex_Min < 0))
                {
                    intIndex_Min = intIndex;
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
                classSweepAndPrune_ElementFind cBF = lst[intIndex];
                if (lstTried.Contains(cBF)) bolLoop_Max = false;

                if (intFind_Max >= cBF.intStart && intIndex > intIndex_Max)
                {
                    intIndex_Max = intIndex;
                }
                lstTried.Add(cBF);
                intStep /= 2;
                if (intStep < 1)
                    intStep = 1;
                intIndex += intStep * (intFind_Max < cBF.intEnd ? -1 : 1);

            } while (intIndex >= 0 && intIndex < lst.Count && bolLoop_Max);

            if (intIndex_Min >= 0 && intIndex_Max >= 0 && intIndex_Min <= intIndex_Max)
            {
                List<classSweepAndPrune_ElementFind> lstRetVal = new List<classSweepAndPrune_ElementFind>();
                for (int intCounter = intIndex_Min; intCounter <= intIndex_Max; intCounter++)
                    lstRetVal.Add(lst[intCounter]);
                return lstRetVal;
            }

            return null;
        }
    }
    #endregion

    public class SPContainer : System.Windows.Forms.Panel
    {
        SPContainer cMyRef = null;
        public enum enuScrollingStyle { ScrollBars, Edges, none };

        static List<SPContainer> lstSPContainers = new List<SPContainer>();
        static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID { get { return intID; } }

        #region VariablesAndObjectDeclaration

        System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();

        public List<classSweepAndPrune_Element> lstElements = new List<classSweepAndPrune_Element>();
        public List<classSweepAndPrune_ElementFind> lstHScan = null;
        public List<classSweepAndPrune_ElementFind> lstVScan = null;

        System.Windows.Forms.Timer tmrDrawCursor = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer tmrScroll = new System.Windows.Forms.Timer();
        Semaphore semMouseMove = new Semaphore(1, 1);

        bool bolCursorState = false;

        public System.Windows.Forms.TextBox txtFocus = new System.Windows.Forms.TextBox();

        public classScrollBar VScrollBar = null;
        public classScrollBar HScrollBar = null;
        bool[] bolKeyState = new bool[256];

        System.Windows.Forms.ContextMenu cMnu = new System.Windows.Forms.ContextMenu();

        #endregion

        public SPContainer(string Name)
        {
            cMyRef = this;
            this.Name = Name;
            VScrollBar = new classScrollBar(ref cMyRef);
            HScrollBar = new classScrollBar(ref cMyRef);
            lstSPContainers.Add(this);

            picArray[0] = new System.Windows.Forms.PictureBox();
            Controls.Add(picArray[0]);
            picArray[0].Dock = System.Windows.Forms.DockStyle.Fill;

            picArray[1] = new System.Windows.Forms.PictureBox();
            Controls.Add(picArray[1]);
            picArray[1].Dock = System.Windows.Forms.DockStyle.Fill;

            recVisible = new Rectangle(new Point(), pic.Size);
            cMnu.Popup += CMnu_Popup;

            picArray[0].MouseMove += pic_MouseMove;
            picArray[0].MouseDown += pic_MouseDown;
            picArray[0].MouseClick += pic_MouseClick;
            picArray[0].MouseDoubleClick += pic_MouseDoubleClick;
            picArray[0].MouseUp += pic_MouseUP;
            picArray[0].MouseWheel += pic_MouseWheel;
            picArray[0].MouseEnter += pic_MouseEnter;
            picArray[0].MouseLeave += pic_MouseLeave;
            picArray[0].SizeChanged += Pic_SizeChanged;
            picArray[0].GotFocus += Pic_GotFocus;
            picArray[0].BackgroundImageChanged += Pic_BackgroundImageChanged;

            picArray[1].MouseMove += pic_MouseMove;
            picArray[1].MouseDown += pic_MouseDown;
            picArray[1].MouseClick += pic_MouseClick;
            picArray[1].MouseDoubleClick += pic_MouseDoubleClick;
            picArray[1].MouseUp += pic_MouseUP;
            picArray[1].MouseWheel += pic_MouseWheel;
            picArray[1].MouseEnter += pic_MouseEnter;
            picArray[1].MouseLeave += pic_MouseLeave;
            picArray[1].SizeChanged += Pic_SizeChanged;
            picArray[1].GotFocus += Pic_GotFocus;
            picArray[1].BackgroundImageChanged += Pic_BackgroundImageChanged;

            ToolTip_SetUp();

            Controls.Add(txtFocus);
            txtFocus.Location = new Point(-1024, -1024);
            txtFocus.KeyDown += txtFocus_KeyDown;
            txtFocus.KeyUp += txtFocus_KeyUp;
            txtFocus.KeyPress += txtFocus_KeyPress;
            txtFocus.TextChanged += txtFocus_TextChanged;
            txtFocus.GotFocus += TxtFocus_GotFocus;
            txtFocus.LostFocus += TxtFocus_LostFocus;

            VScrollBar.ValueChanged = VScrollBar_ValueChanged;
            HScrollBar.ValueChanged = HScrollBar_ValueChanged;

            tmrDrawCursor.Interval = 400;
            tmrDrawCursor.Tick += TmrDrawCursor_Tick;
            tmrDrawCursor.Enabled = false;

            tmrScroll.Interval = 300;
            tmrScroll.Tick += TmrScroll_Tick;

            BackColorChanged += SPContainer_BackColorChanged;
            picArray[0].BackColorChanged += SPContainer_BackColorChanged1;
            picArray[1].BackColorChanged += SPContainer_BackColorChanged1;


        }

        private void SPContainer_BackColorChanged1(object sender, EventArgs e)
        {
            
        }

        private void SPContainer_BackColorChanged(object sender, EventArgs e)
        {
            
        }

        #region Language

        List<string> _lstLanguages = new List<string>();
        public List<string> lstLanguages
        {
            get { return _lstLanguages; }
        }

        int intLanguageIndex = 0;
        public int LanguageIndex
        {
            get { return intLanguageIndex; }
            set
            {
                if (value > 0 && value < lstLanguages.Count)
                {
                    intLanguageIndex = value;
                    // set lstElements to language here
                    for (int intEleCounter = 0; intEleCounter < lstElements.Count; intEleCounter++)
                    {
                        classSweepAndPrune_Element cEle = lstElements[intEleCounter];
                        cEle.Language_Set(LanguageIndex);
                    }
                }
            }
        }

        public string Language
        {
            get { return lstLanguages[intLanguageIndex]; }
            set
            {
                int intIndex = LanguageIndex_ByName(value);
                if (intIndex >= 0)
                    intLanguageIndex = intIndex;
            }
        }

        public int LanguageIndex_ByName(string strLanguage)
        {
            for (int intLanguageCounter = 0; intLanguageCounter < lstLanguages.Count; intLanguageCounter++)
            {
                if (string.Compare(strLanguage, lstLanguages[intLanguageCounter]) == 0)
                {
                    return intLanguageCounter;
                }
            }
            lstLanguages.Add(strLanguage);
            return lstLanguages.Count - 1;
        }
        #endregion


        #region ToolTip
        static List<string> lstEnuFGB_WordType = new List<string>();
        public List<SPObjects.classSweepAndPrune_Element> lstToolTip_Objects = new List<SPObjects.classSweepAndPrune_Element>();

        static string conDefaultToolTip = "tool tip not set";

        public bool ToolTip_Enabled
        {
            get { return toolTip.Active; }
            set { toolTip.Active = value; }
        }

        /// <summary>
        /// initializes an instance of classToolTip with all the tips for each control
        /// </summary>
        void ToolTip_SetUp()
        {
            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 100;
            toolTip.UseAnimation = true;
            toolTip.Popup += _ToolTip_Popup;
            toolTip.SetToolTip(picArray[0], conDefaultToolTip);
            toolTip.SetToolTip(picArray[1], conDefaultToolTip);

            lstLanguages.Add("default");

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;
            //mnuStrip.ShowItemToolTips = true;

            lstToolTip_Objects.Clear();
        }


        System.Windows.Forms.PopupEventHandler _ToolTip_PopUp = null;
        public System.Windows.Forms.PopupEventHandler ToolTip_PopUp
        {
            get { return _ToolTip_PopUp; }
            set { _ToolTip_PopUp = value; }
        }

        public void ToolTip_Set(string strTip)
        {
            toolTip.SetToolTip((System.Windows.Forms.Control)picArray[0], strTip);
            toolTip.SetToolTip((System.Windows.Forms.Control)picArray[1], strTip);
        }

        classSweepAndPrune_Element cEle_ToolTipLast = null;
        /// <summary>
        /// stops edits tool tip with the multiple tips available for a given control
        /// </summary>
        private void _ToolTip_Popup(object sender, System.Windows.Forms.PopupEventArgs e)
        {
            try
            {
                if (e.AssociatedControl == null
                    || cEleUnderMouse == null
                    || cEleUnderMouse == cEle_ToolTipLast
                    )
                    return;

                if (ToolTip_PopUp != null)
                {
                    ToolTip_PopUp(sender, e);
                    return;
                }

                cEle_ToolTipLast = cEleUnderMouse;
                string strTip = cEleUnderMouse.Tip_Get();

                if (strTip.Length > 0)
                    toolTip.SetToolTip(e.AssociatedControl, strTip);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.Message);
            }
        }

        #endregion


        #region Add_Sub

        public void Add(ref SlideRuler slr_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)slr_Add;
            slr_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.SlideRuler;
            Add(ref cEle);
            cEle.Name = "SlideRuler";
            slr_Add.cEle = cEle;

        }
        public void Add(ref Button btn_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)btn_Add;
            btn_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.Button;
            Add(ref cEle);
            cEle.Name = "button";
            btn_Add.cEle = cEle;
        }

        public void Add(ref CheckBox chx_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)chx_Add;
            chx_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.CheckBox;
            Add(ref cEle);
            cEle.Name = "CheckBox";
            chx_Add.cEle = cEle;
        }
        public void Add(ref ComboBox cmb_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)cmb_Add;
            cmb_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.ComboBox;
            Add(ref cEle);
            cEle.Name = "ComboBox";
            cmb_Add.cEle = cEle;
        }


        public void Add(ref ProgressBar prb_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)prb_Add;
            prb_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.ProgressBar;
            Add(ref cEle);
            cEle.Name = "ProgressBar";
            prb_Add.cEle = cEle;
        }

        public void Add(ref GroupBox grb_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)grb_Add;
            grb_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.GroupBox;
            Add(ref cEle);
            cEle.Name = "groupBox";
            grb_Add.cEle = cEle;
        }

        public void Add(ref Panel pnl_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)pnl_Add;
            pnl_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.Panel;
            Add(ref cEle);
            cEle.Name = "Panel";
            pnl_Add.cEle = cEle;
        }

        public void Add(ref ListBox lbx_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)lbx_Add;
            lbx_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.ListBox;
            Add(ref cEle);
            cEle.BackColor = Color.White;
            cEle.ForeColor = Color.Black;
            cEle.Name = "ListBox";
            lbx_Add.cEle = cEle;
        }

        public void Add(ref CheckedListBox cbx_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)cbx_Add;
            cbx_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.CheckedListBox;
            Add(ref cEle);
            cEle.BackColor = Color.White;
            cEle.ForeColor = Color.Black;
            cEle.Name = "CheckedListBox";
            cbx_Add.cEle = cEle;
        }

        public void Add(ref getPoint gpt_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)gpt_Add;
            gpt_Add.eSP_ObjectType
            = cEle.eSP_ObjectType
            = enuSweepPrune_ObjectType.getPoint;
            Add(ref cEle);
            cEle.Name = "getPoint";
            gpt_Add.cEle = cEle;
        }

        public void Add(ref getSize gsz_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)gsz_Add;
            gsz_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.getSize;
            Add(ref cEle);
            cEle.Name = "getSize";
            gsz_Add.cEle = cEle;
        }

        public void Add(ref getRectangle grc_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)grc_Add;
            grc_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.getRectangle;
            Add(ref cEle);
            cEle.Name = "getRectangle";
            grc_Add.cEle = cEle;
        }

        public void Add(ref Label lbl_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)lbl_Add;
            lbl_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.Label;
            Add(ref cEle);
            cEle.Name = "Label";
            lbl_Add.cEle = cEle;
        }


        public void Add(ref PictureBox pic_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)pic_Add;
            pic_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.PictureBox;
            Add(ref cEle);
            cEle.Name = "PictureBox";
            pic_Add.cEle = cEle;
        }

        public void Add(ref Textbox txt_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)txt_Add;
            txt_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.TextBox;
            Add(ref cEle);
            cEle.Name = "TextBox";
            txt_Add.cEle = cEle;
        }
        

        public void Add(ref TextDisplay txt_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)txt_Add;
            txt_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.TextDisplay;
            Add(ref cEle);
            cEle.Name = "TextDisplay";
            txt_Add.cEle = cEle;
        }

        public void Add(ref TrackBar trb_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)trb_Add;
            trb_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.TrackBar;
            Add(ref cEle);
            cEle.Name = "TrackBar";
            trb_Add.cEle = cEle;
        }

        public void Add(ref numericUpDown nud_Add)
        {
            classSweepAndPrune_Element cEle = new classSweepAndPrune_Element(ref cMyRef);
            cEle.obj = (object)nud_Add;
            nud_Add.eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.numericUpDown;
            Add(ref cEle);
            cEle.Name = "numericUpDown";
            nud_Add.cEle = cEle;
        }

        public void Add(ref classSweepAndPrune_Element cEle) { classSweepAndPrune_Engine.Add(ref cEle, ref cMyRef); }

        public void Sub(ref classSweepAndPrune_Element cEle)
        {
            if (lstElements.Contains(cEle))
            {
                lstElements.Remove(cEle);
                while (cEle.lstEle.Count > 0)
                {
                    classSweepAndPrune_Element cChildEle = cEle.lstEle.lst[0];
                    Sub(ref cChildEle);
                    lstElements.Remove(cEle.lstEle.lst[0]);
                    cEle.lstEle.lst.RemoveAt(0);
                }
                Reset();
                lstElements_Removed = true;
            }
        }


        #endregion


        #region Pic

        System.Windows.Forms.PictureBox[] picArray = new System.Windows.Forms.PictureBox[2];
        int intPicIndex = 0;
        public System.Windows.Forms.PictureBox pic
        {
            get { return picArray[intPicIndex]; }
        }
        void Pic_Cycle()
        {
            intPicIndex = (intPicIndex + 1) % 2;
            pic.ContextMenu = cMnu;
            pic.BringToFront();
            //System.Diagnostics.Debug.Print("SPContainer:" + Name + " Pic_Cycle(" + intPicIndex.ToString() + ")");
        }

        #endregion

        public bool KeyState(int KeyValue)
        {
            return bolKeyState[(byte)KeyValue];
        }

        new public void Focus()
        {
            txtFocus.Focus();
        }


        #region Dimensions


        Rectangle _recSPArea = new Rectangle(0, 0, 10, 10);
        public Rectangle recSPArea
        {
            get { return _recSPArea; }
            set
            {
                if (_recSPArea.Width != value.Width
                    || _recSPArea.Height != value.Height
                    || _recSPArea.X != value.X
                    || _recSPArea.Y != value.Y)
                {
                    _recSPArea = value;
                    ScrollBars_SetSizes();
                    NeedsToBeRedrawn = true;
                }
            }
        }

        public Rectangle recSource
        {
            get { return new Rectangle(0, 0, recSPArea.Width, recSPArea.Height); }
        }

        #endregion

        #region Search
        public List<classSweepAndPrune_Element> Search(Point pt) { return classSweepAndPrune_Engine.Search(pt, ref cMyRef); }

        public List<classSweepAndPrune_Element> Search(Rectangle rec) { return classSweepAndPrune_Engine.Search(rec, ref cMyRef); }

        public List<classSweepAndPrune_Element> Search(Point ptTL, Point ptBR) { return classSweepAndPrune_Engine.Search(ptTL, ptBR, ref cMyRef); }

        public void Reset() { classSweepAndPrune_Engine.Reset(ref cMyRef); }
        #endregion 

        #region ForeColor

        Color clrForeColor_Dull = Color.Black;
        SolidBrush sbrForeColor_Dull = new SolidBrush(Color.Black);
        public Color ForeColor_Dull
        {
            get { return clrForeColor_Dull; }
            set
            {
                //if (ForeColor_Dull != value)
                {
                    clrForeColor_Dull = value;
                    sbrForeColor_Dull = new SolidBrush(clrForeColor_Dull);
                    for (int intCounter = 0; intCounter < lstElements.Count; intCounter++)
                        lstElements[intCounter].ForeColor = ForeColor;

                    NeedsToBeRedrawn = false;
                }
            }
        }

        Color clrForeColor_Highlight = Color.White;
        SolidBrush sbrForeColor_Highlight = new SolidBrush(Color.White);
        public Color ForeColor_Highlight
        {
            get { return clrForeColor_Highlight; }
            set
            {
                if (ForeColor_Highlight != value)
                {
                    clrForeColor_Highlight = value;
                    sbrForeColor_Highlight = new SolidBrush(clrForeColor_Highlight);
                    NeedsToBeRedrawn = false;
                }
            }
        }

        public SolidBrush sbrForeColor
        {
            get { return sbrForeColor_Dull; }
        }

        override public Color ForeColor
        {
            get { return ForeColor_Dull; }
            set
            {
                ForeColor_Dull
                        = BackColor_Highlight
                        = value;
                for (int intCounter = 0; intCounter < lstElements.Count; intCounter++)
                    lstElements[intCounter].ForeColor = value;

                NeedsToBeRedrawn = false;
            }
        }
        #endregion

        #region BackColor
        Color clrBackColor_Dull = Color.White;
        SolidBrush sbrBackColor_Dull = new SolidBrush(Color.White);
        public Color BackColor_Dull
        {
            get { return clrBackColor_Dull; }
            set
            {
                //if (clrBackColor_Dull != value)
                {
                    clrBackColor_Dull = value;
                    sbrBackColor_Dull = new SolidBrush(clrBackColor_Dull);
                    for (int intCounter = 0; intCounter < lstElements.Count; intCounter++)
                        lstElements[intCounter].BackColor = BackColor;

                    NeedsToBeRedrawn = false;
                }
            }
        }

        Color clrBackColor_Highlight = Color.Black;
        SolidBrush sbrBackColor_Highlight = new SolidBrush(Color.Black);
        public Color BackColor_Highlight
        {
            get { return clrBackColor_Highlight; }
            set
            {
                if (BackColor_Highlight != value)
                {
                    clrBackColor_Highlight = value;
                    sbrBackColor_Highlight = new SolidBrush(clrBackColor_Highlight);
                    NeedsToBeRedrawn = false;
                }
            }
        }

        public SolidBrush sbrBackColor { get { return sbrBackColor_Dull; } }


        override public Color BackColor
        {
            get { return BackColor_Dull; }
            set
            {
                BackColor_Dull
                    = ForeColor_Highlight
                    = value;
                NeedsToBeRedrawn = false;
            }
        }
        #endregion

        #region Draw

        void ScrollBars_SetSizes()
        {
            int intSmallLargeRatio = 10;

            VScrollBar.Min = recSPArea.Top;
            VScrollBar.Max = recSPArea.Height - recVisible.Height;
            VScrollBar.LargeChange = recVisible.Height;
            VScrollBar.SmallChange = VScrollBar.LargeChange / intSmallLargeRatio;
            VScrollBar.Value = recVisible.Top;
            VScrollBar.Visible = ScrollingStyle == enuScrollingStyle.ScrollBars
                                    && recVisible.Height < recSPArea.Height;

            HScrollBar.Min = recSPArea.Left;
            HScrollBar.Max = recSPArea.Width - recVisible.Width;
            HScrollBar.LargeChange = recVisible.Width;
            HScrollBar.SmallChange = HScrollBar.LargeChange / intSmallLargeRatio;
            HScrollBar.Value = recVisible.Left;
            HScrollBar.Visible = ScrollingStyle == enuScrollingStyle.ScrollBars
                                    && recVisible.Width < recSPArea.Width;
        }
        bool Cursor_Toggle()
        {
            bolCursorState = !bolCursorState;
            return bolCursorState;
        }

        Bitmap _BackgroundImage = null;
        override public Image BackgroundImage
        {
            get { return _BackgroundImage; }
            set
            {
                try
                {
                    _BackgroundImage = new Bitmap(value);
                    bolRecVisibleChanged = true;
                    NeedsToBeRedrawn = true;
                    return;
                }
                catch (Exception)
                {
                    
                }
                Thread.Sleep(200);
                BackgroundImage = value;
            }
        }

        public Bitmap MyImage
        {
            get
            {
                if (pic.Image == null || NeedsToBeRedrawn)
                    Draw();
                return (Bitmap)pic.Image;
            }

            set
            {
                pic_Image_Set(ref value);
                NeedsToBeRedrawn = false;
            }
        }

        void pic_Image_Set(ref Bitmap bmpImage)
        {
            if (picArray[0].Image != null)
                picArray[0].Image.Dispose();

            if (picArray[1].Image != null)
                picArray[1].Image.Dispose();

            picArray[0].Image = bmpImage;
            picArray[1].Image = bmpImage;
        }

        void DrawCursor(bool bolCursorState_Local)
        {
            if (cEleFocused != null)
            {
                switch (cEleFocused.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            //txtDrawCursor.cEle.NeedsToBeRedrawn = true;
                            NeedsToBeRedrawn = true;

                            Textbox txtDrawCursor = (Textbox)cEleFocused.obj;
                            txtDrawCursor.CursorState = bolCursorState_Local;
                            txtDrawCursor.CursorNeedsToBeDrawn = true;
                            txtDrawCursor.DrawCursor();
                            //bool bolBuilding = BuildingInProgress;
                            //Building_Start();
                            //{

                            //}
                            //if (!bolBuilding)
                            //    Building_Complete();
                        }
                        break;
                }
            }
        }

        Graphics g = null;
        public void Building_Start()
        {
            bolBuildingcontainer = true;
        }

        public void Building_Complete()
        {
            bolBuildingcontainer = false;
            bolNeedsToBeRedrawn = true;
            try
            {
                Draw();
            }
            catch (Exception)
            {
            }
            
        }
        public void DrawImage(Bitmap bmpMyImage, Rectangle recDest, Rectangle recSource)
        {
            g.DrawImage(bmpMyImage, recDest, recSource, GraphicsUnit.Pixel);
        }


        public Bitmap DrawComplete()
        {
            Rectangle recVisibleCopy = recVisible;
            bool bolCropToFit_Temp = CropToFitPictureBox;
            CropToFitPictureBox = false;
            recVisible = recSPArea;
            bolBuildingcontainer = false;
            bolNeedsToBeRedrawn = true;
            Bitmap bmpRetVal = new Bitmap(MyImage);
            //bmpRetVal.Save(@"c:\debug\bmpRetVal.png");

            CropToFitPictureBox = bolCropToFit_Temp;
            recVisible = recVisibleCopy;

            return bmpRetVal;
        }

        public static Button btnDebug = null;

        void Draw()
        {
            if (!bolNeedsToBeRedrawn || bolBuildingcontainer) return;

            if (recSPArea.Width < 10 || recSPArea.Height < 10
                || pic.Width < 10 || pic.Height < 10) return;

            if (BackgroundImage != null
                &&
                (BackgroundImage.Width < 10 || BackgroundImage.Height < 10)) return;

            DateTime dt_Start = DateTime.Now;

            int intDebugCount = 0;
            string strDebug = "";

            Bitmap bmpTemp = null;

            if (!CropToFitPictureBox)
                bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);
            else if (RecVisible_Changed || pic.Image == null)
                bmpTemp = new Bitmap(pic.Width, pic.Height);
            else
                bmpTemp = new Bitmap(pic.Image);

            Rectangle rectDestination_Local = new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height);
            //bmpTemp.Save(@"c:\debug\bmpTemp.png");
            List<classSweepAndPrune_Element> lstVisible = classSweepAndPrune_Engine.Search(recVisible, ref cMyRef);
            IEnumerable<classSweepAndPrune_Element> query = lstVisible.OrderBy(cEle => cEle.Index);
            lstVisible = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
            //bmpTemp.Save(@"c:\debug\pngTemp.png");
            g = Graphics.FromImage(bmpTemp);
            {
                if (RecVisible_Changed || lstElements_Removed)
                {
                    if (BackgroundImage != null)
                    {
                        g.DrawImage(BackgroundImage,
                                    rectDestination_Local,
                                    recVisible,
                                    GraphicsUnit.Pixel);
                    }
                    else
                        g.FillRectangle(sbrBackColor, rectDestination_Local);
                }

                for (int intEleCounter = 0; intEleCounter < lstVisible.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cEle = lstVisible[intEleCounter];
                    if (cEle.Visible
                             && (cEle.NeedsToBeRedrawn
                                || cEle.ImageRefreshed
                                || RecVisible_Changed
                                || lstElements_Removed)
                        )
                    {
                        //bool bolDebug = false;
                        //if (btnDebug != null)
                        //    System.Diagnostics.Debug.Print("Width:" + btnDebug.Width.ToString() + "  text:" + btnDebug.Text);
                        //if (bolDebug)
                        //{
                        //    btnDebug = (Button)cEle.obj;
                        //}

                        switch (cEle.eSP_ObjectType)
                        {
                            case enuSweepPrune_ObjectType.Button:
                                {
                                    Button btnEle = (Button)cEle.obj;
                                    if (btnEle != null && btnEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, btnEle.MyImage.Width, btnEle.MyImage.Height);
                                        g.DrawImage(btnEle.MyImage, btnEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;
                            case enuSweepPrune_ObjectType.SlideRuler:
                                {
                                    SlideRuler btnEle = (SlideRuler)cEle.obj;
                                    if (btnEle != null && btnEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, btnEle.MyImage.Width, btnEle.MyImage.Height);
                                        g.DrawImage(btnEle.MyImage, btnEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.ButtonControl:
                                {
                                    ButtonControl btcEle = (ButtonControl)cEle.obj;
                                    if (btcEle != null && btcEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, btcEle.MyImage.Width, btcEle.MyImage.Height);
                                        g.DrawImage(btcEle.MyImage, btcEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckBox:
                                {
                                    CheckBox chxEle = (CheckBox)cEle.obj;
                                    if (chxEle != null && chxEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, chxEle.MyImage.Width, chxEle.MyImage.Height);
                                        g.DrawImage(chxEle.MyImage, chxEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckedListBox:
                                {
                                    CheckedListBox cbxEle = (CheckedListBox)cEle.obj;
                                    if (cbxEle != null && cbxEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, cbxEle.MyImage.Width, cbxEle.MyImage.Height);
                                        g.DrawImage(cbxEle.MyImage, cbxEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.ComboBox:
                                {
                                    ComboBox cmbEle = (ComboBox)cEle.obj;
                                    if (cmbEle != null && cmbEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, cmbEle.MyImage.Width, cmbEle.MyImage.Height);
                                        g.DrawImage(cmbEle.MyImage, cmbEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.getPoint:
                                {
                                    getPoint gptEle = (getPoint)cEle.obj;
                                    if (gptEle != null && gptEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, gptEle.MyImage.Width, gptEle.MyImage.Height);
                                        g.DrawImage(gptEle.MyImage, gptEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.getRectangle:
                                {
                                    getRectangle grcEle = (getRectangle)cEle.obj;
                                    if (grcEle != null && grcEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, grcEle.MyImage.Width, grcEle.MyImage.Height);
                                        g.DrawImage(grcEle.MyImage, grcEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.getSize:
                                {
                                    getSize gszEle = (getSize)cEle.obj;
                                    if (gszEle != null && gszEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, gszEle.MyImage.Width, gszEle.MyImage.Height);
                                        g.DrawImage(gszEle.MyImage, gszEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.GroupBox:
                                {
                                    GroupBox grbEle = (GroupBox)cEle.obj;
                                    if (grbEle != null && grbEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, grbEle.MyImage.Width, grbEle.MyImage.Height);
                                        g.DrawImage(grbEle.MyImage, grbEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.Label:
                                {
                                    Label lblEle = (Label)cEle.obj;
                                    if (lblEle != null && lblEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, lblEle.MyImage.Width, lblEle.MyImage.Height);
                                        g.DrawImage(lblEle.MyImage, lblEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.ListBox:
                                {
                                    ListBox lbxEle = (ListBox)cEle.obj;
                                    if (lbxEle != null && lbxEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, lbxEle.MyImage.Width, lbxEle.MyImage.Height);
                                        g.DrawImage(lbxEle.MyImage, lbxEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.numericUpDown:
                                {
                                    numericUpDown nudEle = (numericUpDown)cEle.obj;
                                    if (nudEle != null && nudEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, nudEle.MyImage.Width, nudEle.MyImage.Height);
                                        g.DrawImage(nudEle.MyImage, nudEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.Panel:
                                {
                                    Panel pnlEle = (Panel)cEle.obj;
                                    if (pnlEle != null && pnlEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, pnlEle.MyImage.Width, pnlEle.MyImage.Height);
                                        g.DrawImage(pnlEle.MyImage, pnlEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.PictureBox:
                                {
                                    PictureBox picEle = (PictureBox)cEle.obj;
                                    if (picEle != null && picEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, picEle.MyImage.Width, picEle.MyImage.Height);
                                        g.DrawImage(picEle.MyImage, picEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.ProgressBar:
                                {
                                    ProgressBar prbEle = (ProgressBar)cEle.obj;
                                    if (prbEle != null && prbEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, prbEle.MyImage.Width, prbEle.MyImage.Height);
                                        g.DrawImage(prbEle.MyImage, prbEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.TextBox:
                                {
                                    Textbox txtEle = (Textbox)cEle.obj;
                                    if (txtEle != null && txtEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, txtEle.MyImage.Width, txtEle.MyImage.Height);
                                        g.DrawImage(txtEle.MyImage, txtEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.TextDisplay:
                                {
                                    TextDisplay txtEle = (TextDisplay)cEle.obj;
                                    if (txtEle != null && txtEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, txtEle.MyImage.Width, txtEle.MyImage.Height);
                                        g.DrawImage(txtEle.MyImage, txtEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.TrackBar:
                                {
                                    TrackBar trbEle = (TrackBar)cEle.obj;
                                    if (trbEle != null && trbEle.MyImage != null)
                                    {
                                        Rectangle recSource = new Rectangle(0, 0, trbEle.MyImage.Width, trbEle.MyImage.Height);
                                        g.DrawImage(trbEle.MyImage, trbEle.recDraw, recSource, GraphicsUnit.Pixel);
                                    }
                                }
                                break;

                            case enuSweepPrune_ObjectType.UserDefined:
                                {
                                    if (cEle.eventDraw != null)
                                    {
                                        cEle.eventDraw((object)cEle, new EventArgs());
                                    }
                                    else
                                        g.FillRectangle(new SolidBrush(cEle.BackColor), cEle.recDraw);
                                }
                                break;

                            default:
                                {
                                    System.Windows.Forms.MessageBox.Show("SPObjects.classSweepAndPrune.Draw() : this should not happen");

                                }
                                break;
                        }


                        cEle.ImageRefreshed = false;
                    }
                    else
                    {
                        intDebugCount++;
                        strDebug += "\r\n " + intDebugCount.ToString() + ")" + cEle.Name;
                    }
                }
                RecVisible_Changed = false;
                /*
                if (ScrollingStyle == enuScrollingStyle.ScrollBars)
                {
                    //bmpTemp.Save(@"c:\debug\bmpTemp.png");
                    if (HScrollBar.Visible)
                    {
                        // draw horizontal scroll bar
                        HScrollBar.recDraw = new Rectangle(0,
                                                           bmpTemp.Height - HScrollBar.Thickness,
                                                           bmpTemp.Width,
                                                           HScrollBar.Thickness);
                        HScrollBar.recSliderDraw = new Rectangle((int)((double)(HScrollBar.Value - HScrollBar.Min) / (double)recSPArea.Width * (double)recVisible.Width),
                                                                 HScrollBar.recDraw.Top,
                                                                 (int)((double)HScrollBar.LargeChange / (double)recSPArea.Width * (double)recVisible.Width),
                                                                 HScrollBar.Thickness);
                        g.FillRectangle(Brushes.Gray, HScrollBar.recDraw);
                        g.FillRectangle(Brushes.DarkGray, HScrollBar.recSliderDraw);
                    }

                    if (VScrollBar.Visible)
                    {
                        // draw horizontal scroll bar
                        VScrollBar.recDraw = new Rectangle(bmpTemp.Width - VScrollBar.Thickness,
                                                           0,
                                                           VScrollBar.Thickness,
                                                           bmpTemp.Height);
                        VScrollBar.recSliderDraw = new Rectangle(VScrollBar.recDraw.Left,
                                                                 (int)((double)(VScrollBar.Value - VScrollBar.Min) / (double)recSPArea.Height * (double)recVisible.Height),
                                                                 VScrollBar.Thickness,
                                                                 (int)((double)VScrollBar.LargeChange / (double)recSPArea.Height * (double)recVisible.Height));
                        g.FillRectangle(Brushes.Gray, VScrollBar.recDraw);
                        g.FillRectangle(Brushes.DarkGray, VScrollBar.recSliderDraw);
                    }
                }
                /*/

                if (ScrollingStyle == enuScrollingStyle.ScrollBars)
                {
                    //bmpTemp.Save(@"c:\debug\bmpTemp.png");
                    if (HScrollBar.Visible)
                    {
                        // draw horizontal scroll bar
                        HScrollBar.recDraw = new Rectangle(0,
                                                           bmpTemp.Height - HScrollBar.Thickness,
                                                           bmpTemp.Width,
                                                           HScrollBar.Thickness);
                        HScrollBar.recSliderDraw = new Rectangle((int)((double)(HScrollBar.Value - HScrollBar.Min) / (double)recSPArea.Width * (double)recVisible.Width),
                                                                 HScrollBar.recDraw.Top,
                                                                 (int)((double)HScrollBar.LargeChange / (double)recSPArea.Width * (double)recVisible.Width),
                                                                 HScrollBar.Thickness);
                        g.FillRectangle(Brushes.Gray, HScrollBar.recDraw);
                        g.FillRectangle(Brushes.DarkGray, HScrollBar.recSliderDraw);

                        HScrollBar.recArrow_Left = new Rectangle(HScrollBar.recDraw.Location, HScrollBar.bmpArrow_Left.Size);
                        g.DrawImage(HScrollBar.bmpArrow_Left, HScrollBar.recArrow_Left.Location);

                        HScrollBar.recArrow_Left = new Rectangle(new Point(HScrollBar.recDraw.Right - HScrollBar.bmpArrow_Right.Width, HScrollBar.recDraw.Top), HScrollBar.bmpArrow_Left.Size);
                        g.DrawImage(HScrollBar.bmpArrow_Right, HScrollBar.recArrow_Left.Location);
                    }

                    if (VScrollBar.Visible)
                    {
                        // draw horizontal scroll bar
                        VScrollBar.recDraw = new Rectangle(bmpTemp.Width - VScrollBar.Thickness,
                                                           0,
                                                           VScrollBar.Thickness,
                                                           bmpTemp.Height);
                        VScrollBar.recSliderDraw = new Rectangle(VScrollBar.recDraw.Left,
                                                                 (int)((double)(VScrollBar.Value - VScrollBar.Min) / (double)recSPArea.Height * (double)recVisible.Height),
                                                                 VScrollBar.Thickness,
                                                                 (int)((double)VScrollBar.LargeChange / (double)recSPArea.Height * (double)recVisible.Height));
                        g.FillRectangle(Brushes.Gray, VScrollBar.recDraw);
                        g.FillRectangle(Brushes.DarkGray, VScrollBar.recSliderDraw);

                        VScrollBar.recArrow_Up = new Rectangle(VScrollBar.recDraw.Location, VScrollBar.bmpArrow_Up.Size);
                        g.DrawImage(VScrollBar.bmpArrow_Up, VScrollBar.recArrow_Up.Location);

                        VScrollBar.recArrow_Down = new Rectangle(new Point(VScrollBar.recDraw.Left, VScrollBar.recDraw.Bottom - VScrollBar.bmpArrow_Down.Height), VScrollBar.bmpArrow_Down.Size);
                        g.DrawImage(VScrollBar.bmpArrow_Down, VScrollBar.recArrow_Down.Location);
                    }
                }
                // */
            }
            g.Dispose();
            //bmpTemp.Save(@"c:\debug\bmpTemp.png");
            MyImage = bmpTemp;
            //pic.Refresh();
            RecVisible_Changed
                = lstElements_Removed
                = false;

            DateTime dt_End = DateTime.Now;
            TimeSpan ts = dt_End.Subtract(dt_Start);
        }
        #endregion

        #region events

        private void Pic_BackgroundImageChanged(object sender, EventArgs e)
        {
        }

        private void pic_MouseEnter(object sender, EventArgs e)
        {
            if (MouseEnter != null) MouseEnter(sender, e);
            Cursor = System.Windows.Forms.Cursors.Arrow;
        }

        private void Pic_GotFocus(object sender, EventArgs e)
        {
            txtFocus.Focus();
        }

        void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            recVisible = new Rectangle(recVisible.Left,
                                       VScrollBar.Value,
                                       recVisible.Width,
                                       recVisible.Height);
        }

        void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            recVisible = new Rectangle(HScrollBar.Value,
                                       recVisible.Top,
                                       recVisible.Width,
                                       recVisible.Height);
        }

        private void Pic_SizeChanged(object sender, EventArgs e)
        {
            recVisible = new Rectangle(recVisible.Location, pic.Size);
        }

        private void CMnu_Popup(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu cMnuSender = (System.Windows.Forms.ContextMenu)sender;
            cMnuSender.MenuItems.Clear();
            if (cEleUnderMouse != null)
            {
                if (cEleUnderMouse.ContextMenu != null)
                {

                    cEleUnderMouse.ContextMenu.Show(pic, new Point(recSPArea.Left + MouseLocation.X, recSPArea.Top + MouseLocation.Y));
                    return;
                }
            }

            if (ContextMenu != null)
            {
                ContextMenu.Show(pic, new Point(recSPArea.Left + MouseLocation.X, recSPArea.Top + MouseLocation.Y));
                return;

            }
        }

        private void TmrDrawCursor_Tick(object sender, EventArgs e)
        {
            if (cEleFocused != null)
            {
                DrawCursor(Cursor_Toggle());
            }
        }

        private void TxtFocus_LostFocus(object sender, EventArgs e)
        {
            tmrDrawCursor.Enabled = false;
            DrawCursor(false);
        }

        private void TxtFocus_GotFocus(object sender, EventArgs e)
        {
            tmrDrawCursor.Enabled = true;
        }

        #region Key_events
        bool bolTxtFocus_IgnoreEvents = false;
        private void txtFocus_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bolKeyState[e.KeyValue] = true;
            if (cEleFocused != null)
            {
                switch (cEleFocused.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            SPObjects.Textbox txtEle = (SPObjects.Textbox)cEleFocused.obj;
                            txtEle.SelectionStart = txtFocus.SelectionStart;
                            if (txtEle.KeyDown != null)
                                txtEle.KeyDown((object)txtEle, e);
                        }
                        return;
                }
            }

            if (event_KeyDown != null)
                event_KeyDown((object)this, e);
        }

        private void txtFocus_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bolKeyState[e.KeyValue] = false;
            if (cEleFocused != null)
            {
                switch (cEleFocused.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtEle = (Textbox)cEleFocused.obj;
                            txtEle.SelectionStart = txtFocus.SelectionStart;
                            txtEle.SelectionLength = txtFocus.SelectionLength;

                            if (txtEle.KeyUp != null)
                                txtEle.KeyUp((object)txtEle, e);
                        }
                        return;
                }
            }

            if (event_KeyUp != null)
                event_KeyUp((object)this, e);
        }

        private void txtFocus_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (cEleFocused != null)
            {
                switch (cEleFocused.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtFocused = (Textbox)cEleFocused.obj;
                            if (txtFocused.KeyPress != null)
                                txtFocused.KeyPress((object)txtFocused, new System.Windows.Forms.KeyPressEventArgs(e.KeyChar));
                        }
                        return;
                }
            }

            if (event_KeyPress != null)
            {
                event_KeyPress((object)this, e);
            }
        }

        void txtFocus_TextChanged(object sender, EventArgs e)
        {
            if (bolTxtFocus_IgnoreEvents)
            {
                return;
            }

            if (cEleFocused != null)
            {
                switch (cEleFocused.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtFocused = (Textbox)cEleFocused.obj;
                            txtFocused.Text = txtFocus.Text;
                            txtFocused.MyImage = null;

                            if (txtFocused.TextChanged != null)
                                txtFocused.TextChanged((object)txtFocused, e);

                            txtFocused.cEle.NeedsToBeRedrawn = true;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Mouse_Events
        System.Windows.Forms.MouseEventArgs MouseEventArgs_Shift(System.Windows.Forms.MouseEventArgs e_In, classSweepAndPrune_Element cEle)
        {
            return new System.Windows.Forms.MouseEventArgs(e_In.Button, e_In.Clicks, recVisible.Left + e_In.X - cEle.recSP.Left, recVisible.Top + e_In.Y - cEle.recSP.Top, e_In.Delta);
        }

        Point Point_Shift(classSweepAndPrune_Element cEle)
        {
            return new Point(MouseLocation.X - cEle.recSP.Left, MouseLocation.Y - cEle.recSP.Top);
        }

        public void pic_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseClick != null) MouseClick(sender, e);

            Point ptMouse = new Point(e.X, e.Y);
            if (VScrollBar.Visible
                && classSPMath.PointIsInsideARectangle(ptMouse, VScrollBar.recDraw))
            {
                if (ptMouse.Y < VScrollBar.recSliderDraw.Top)
                {
                    VScrollBar.ScrollUp();
                }
                else if (ptMouse.Y > VScrollBar.recSliderDraw.Bottom)
                {
                    VScrollBar.ScrollDown();
                }
                return;
            }

            if (HScrollBar.Visible
                && classSPMath.PointIsInsideARectangle(ptMouse, HScrollBar.recDraw))
            {
                if (ptMouse.X < HScrollBar.recSliderDraw.Left)
                {
                    HScrollBar.ScrollUp();
                }
                else if (ptMouse.X > HScrollBar.recSliderDraw.Right)
                {
                    HScrollBar.ScrollDown();
                }
                return;
            }


            if (cEleUnderMouse != null)
            {
                switch (cEleUnderMouse.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseClick != null)
                                btnUnderMouse.MouseClick((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler slrUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                            if (slrUnderMouse.MouseClick != null)
                                slrUnderMouse.MouseClick((object)slrUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                            if (btcUnderMouse.MouseClick != null)
                                btcUnderMouse.MouseClick((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;

                            if (chxUnderMouse.MouseClick != null)
                                chxUnderMouse.MouseClick((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            if (chxUnderMouse.Enabled)
                                chxUnderMouse.Checked = !chxUnderMouse.Checked;
                        }
                        break;


                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;

                            if (cmbUnderMouse.MouseClick != null)
                                cmbUnderMouse.MouseClick((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint txtUnderMouse = (getPoint)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle txtUnderMouse = (getRectangle)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize txtUnderMouse = (getSize)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox lblUnderMouse = (GroupBox)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseClick != null)
                                lblUnderMouse.MouseClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseClick != null)
                                lblUnderMouse.MouseClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                            if (picUnderMouse.MouseClick != null)
                                picUnderMouse.MouseClick((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                            lbxUnderMouse.lbx_MouseClick((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;


                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                            cbxUnderMouse.cbx_MouseClick((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown txtUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel lblUnderMouse = (Panel)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseClick != null)
                                lblUnderMouse.MouseClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                            if (prbUnderMouse.MouseClick != null)
                                prbUnderMouse.MouseClick((object)prbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseClick != null)
                                txtUnderMouse.MouseClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                            if (trbUnderMouse.MouseClick != null)
                                trbUnderMouse.MouseClick((object)trbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                            if (spObject.MouseClick != null)
                                spObject.MouseClick((object)spObject, MouseEventArgs_Shift(e, cEleUnderMouse));
                        }
                        break;

                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseDoubleClick() default : this should not happen");

                        }
                        break;

                }

            }
        }

        public void pic_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseDoubleClick != null) MouseDoubleClick(sender, e);
            if (cEleUnderMouse != null)
            {
                switch (cEleUnderMouse.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseDoubleClick != null)
                                btnUnderMouse.MouseDoubleClick((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                 
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler btnUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseDoubleClick != null)
                                btnUnderMouse.MouseDoubleClick((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                            if (btcUnderMouse.MouseDoubleClick != null)
                                btcUnderMouse.MouseDoubleClick((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;
                            if (chxUnderMouse.MouseDoubleClick != null)
                                chxUnderMouse.MouseDoubleClick((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;
                            if (cmbUnderMouse.MouseDoubleClick != null)
                                cmbUnderMouse.MouseDoubleClick((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint txtUnderMouse = (getPoint)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle txtUnderMouse = (getRectangle)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize txtUnderMouse = (getSize)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox lblUnderMouse = (GroupBox)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseDoubleClick != null)
                                lblUnderMouse.MouseDoubleClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseDoubleClick != null)
                                lblUnderMouse.MouseDoubleClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                            if (picUnderMouse.MouseDoubleClick != null)
                                picUnderMouse.MouseDoubleClick((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                            if (lbxUnderMouse.MouseDoubleClick != null)
                                lbxUnderMouse.MouseDoubleClick((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                            if (cbxUnderMouse.MouseDoubleClick != null)
                                cbxUnderMouse.MouseDoubleClick((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown txtUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel lblUnderMouse = (Panel)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseDoubleClick != null)
                                lblUnderMouse.MouseDoubleClick((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                            if (prbUnderMouse.MouseDoubleClick != null)
                                prbUnderMouse.MouseDoubleClick((object)prbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseDoubleClick != null)
                                txtUnderMouse.MouseDoubleClick((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                            if (trbUnderMouse.MouseDoubleClick != null)
                                trbUnderMouse.MouseDoubleClick((object)trbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                            if (spObject.MouseDoubleClick != null)
                                spObject.MouseDoubleClick((object)spObject, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseDoubleClick() default : this should not happen");

                        }
                        break;

                }
            }
        }
        public static Point MouseRelTo(System.Windows.Forms.Control ctrl)
        {
            Point ptRetVal = System.Windows.Forms.Control.MousePosition;

            while (ctrl != null)
            {
                ptRetVal.X -= ctrl.Location.X;
                ptRetVal.Y -= ctrl.Location.Y;
                if (ctrl.Parent == null)
                { // this is probably a form
                    try
                    {
                        System.Windows.Forms.Form frmTemp = (System.Windows.Forms.Form)ctrl;
                        if (frmTemp.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                            ptRetVal.Y -= 25;
                    }
                    catch (Exception)
                    {
                    }
                }

                ctrl = ctrl.Parent;
            }
            return ptRetVal;
        }

        private void TmrScroll_Tick(object sender, EventArgs e)
        {
            tmrScroll.Enabled = false;
            Point ptMouseLocation = MouseRelTo(pic);
            pic_MouseMove((object)pic, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, ptMouseLocation.X, ptMouseLocation.Y, 0));
            intScrollStep += 5;
        }

        int intScrollStep = 15;
        bool bolIgnoreMouseMove = false;
        public void pic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseMove != null) MouseMove(sender, e);
            if (MyImage == null || bolIgnoreMouseMove) return;
            semMouseMove.WaitOne();

            Point ptCursor = new Point(e.X, e.Y);
            Rectangle recTemp = new Rectangle(recVisible.Left, recVisible.Top, recVisible.Width, recVisible.Height);

            bool bolScrollTimer = false;

            switch (ScrollingStyle)
            {
                case enuScrollingStyle.Edges:
                    {
                        int intScrollBorderWidth = 15;
                        if (ptCursor.X < intScrollBorderWidth && recVisible.Left > recSPArea.Left)
                        {
                            recTemp.Location = new Point(recVisible.X - intScrollStep, recVisible.Y);
                            bolScrollTimer = true;
                        }
                        else if (ptCursor.X > pic.Width - intScrollBorderWidth && recVisible.Right < recSPArea.Right)
                        {
                            recTemp.Location = new Point(recVisible.X + intScrollStep, recVisible.Y);
                            bolScrollTimer = true;
                        }
                        if (recTemp.Left < recSPArea.Left) recTemp.X = recSPArea.Left;
                        if (recTemp.Right > recSPArea.Right) recTemp.X = recSPArea.Right;

                        if (ptCursor.Y < intScrollBorderWidth && recVisible.Y > recSPArea.Top)
                        {
                            recTemp.Location = new Point(recVisible.X, recVisible.Y - intScrollStep);
                            bolScrollTimer = true;
                        }
                        else if (ptCursor.Y > recVisible.Height - intScrollBorderWidth && recVisible.Bottom < recSPArea.Bottom)
                        {
                            recTemp.Location = new Point(recVisible.X, recVisible.Y + intScrollStep);
                            bolScrollTimer = true;
                        }
                        if (!bolScrollTimer)
                            intScrollStep = 15;
                        if (recTemp.Top < recSPArea.Top) recTemp.Y = recSPArea.Top;
                        if (recTemp.Bottom > recSPArea.Bottom) recTemp.Y = recSPArea.Bottom;

                        recVisible = recTemp;
                        tmrScroll.Enabled = bolScrollTimer;
                    }
                    break;

                case enuScrollingStyle.ScrollBars:
                    {
                        if (VScrollBar.Visible && VScrollBar.Grabbed)
                        {
                            ScrollBars_SetSizes();
                            double dblY = ptCursor.Y - VScrollBar.ptGrab.Y;
                            VScrollBar.Value = VScrollBar.Min + (int)(dblY / (double)VScrollBar.recDraw.Height * recSPArea.Height);
                            semMouseMove.Release();
                            return;
                        }

                        if (HScrollBar.Visible && HScrollBar.Grabbed)
                        {
                            double dblX = ptCursor.X - HScrollBar.ptGrab.X;
                            int intValue = HScrollBar.Min + (int)(dblX / (double)HScrollBar.recDraw.Width * recSPArea.Width);
                            HScrollBar.Value = intValue;
                            semMouseMove.Release();
                            return;
                        }
                    }
                    break;
            }

            MouseLocation = new Point(ptCursor.X + recVisible.X, ptCursor.Y + recVisible.Y);

            if (cEleUnderMouse != null)
            {
                switch (cEleUnderMouse.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseMove != null)
                                btnUnderMouse.MouseMove((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler btnUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseMove != null)
                                btnUnderMouse.MouseMove((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                            if (btcUnderMouse.MouseMove != null)
                                btcUnderMouse.MouseMove((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;
                            if (chxUnderMouse.MouseMove != null)
                                chxUnderMouse.MouseMove((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;
                            if (cmbUnderMouse.MouseMove != null)
                                cmbUnderMouse.MouseMove((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint txtUnderMouse = (getPoint)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle txtUnderMouse = (getRectangle)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize txtUnderMouse = (getSize)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox lblUnderMouse = (GroupBox)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseMove != null)
                                lblUnderMouse.MouseMove((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseMove != null)
                                lblUnderMouse.MouseMove((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                            if (picUnderMouse.MouseMove != null)
                                picUnderMouse.MouseMove((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                            lbxUnderMouse.lbx_MouseMove((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (lbxUnderMouse.MouseMove != null)
                                lbxUnderMouse.MouseMove((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                            cbxUnderMouse.cbx_MouseMove((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (cbxUnderMouse.MouseMove != null)
                                cbxUnderMouse.MouseMove((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown txtUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                            txtUnderMouse.Highlight(Point_Shift(cEleUnderMouse));
                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel lblUnderMouse = (Panel)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseMove != null)
                                lblUnderMouse.MouseMove((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                            if (prbUnderMouse.MouseMove != null)
                                prbUnderMouse.MouseMove((object)prbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                            System.Windows.Forms.MouseEventArgs mseArgs = MouseEventArgs_Shift(e, cEleUnderMouse);
                            if (trbUnderMouse.MouseMove != null)
                                trbUnderMouse.MouseMove((object)trbUnderMouse, mseArgs);
                            trbUnderMouse.ptMouse = new Point(mseArgs.X, mseArgs.Y);

                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                Point ptText = new Point(MouseLocation.X - cEleUnderMouse.rec.Left, MouseLocation.Y - cEleUnderMouse.rec.Top);
                                int intIndexUnderMouse = txtUnderMouse.getIndexAtPoint(ptText);

                                txtFocus.SelectionLength
                                    = txtUnderMouse.SelectionLength
                                    = (int)Math.Abs(txtUnderMouse.indexMouseDown - intIndexUnderMouse);

                                if (intIndexUnderMouse < txtUnderMouse.indexMouseDown)
                                    txtFocus.SelectionStart
                                        = txtUnderMouse.SelectionStart
                                        = intIndexUnderMouse;
                                txtUnderMouse.Draw();
                            }

                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                //Point ptText = new Point(MouseLocation.X - cEleUnderMouse.rec.Left, MouseLocation.Y - cEleUnderMouse.rec.Top);
                                //int intIndexUnderMouse = txtUnderMouse.getIndexAtPoint(ptText);

                                //txtFocus.SelectionLength
                                //    = txtUnderMouse.SelectionLength
                                //    = (int)Math.Abs(txtUnderMouse.indexMouseDown - intIndexUnderMouse);

                                //if (intIndexUnderMouse < txtUnderMouse.indexMouseDown)
                                //    txtFocus.SelectionStart
                                //        = txtUnderMouse.SelectionStart
                                //        = intIndexUnderMouse;
                                //txtUnderMouse.Draw();
                            }

                            if (txtUnderMouse.MouseMove != null)
                                txtUnderMouse.MouseMove((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                            if (spObject.MouseMove != null)
                                spObject.MouseMove((object)spObject, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseMove.set() default:  this should not happen");

                        }
                        break;

                }
            }
            //Building_Complete();
            semMouseMove.Release();
        }

        public void pic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseDown != null) MouseDown(sender, e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (cEleFocused != null)
                {

                    switch (cEleFocused.eSP_ObjectType)
                    {
                        case enuSweepPrune_ObjectType.Button:
                            {
                                Button btnLostFocus = (Button)cEleFocused.obj;
                                if (btnLostFocus.LostFocus != null)
                                    btnLostFocus.LostFocus((object)btnLostFocus, new EventArgs());

                            }
                            break;
                            
                        case enuSweepPrune_ObjectType.SlideRuler:
                            {
                                SlideRuler btnLostFocus = (SlideRuler)cEleFocused.obj;
                                if (btnLostFocus.LostFocus != null)
                                    btnLostFocus.LostFocus((object)btnLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.ButtonControl:
                            {
                                ButtonControl btcLostFocus = (ButtonControl)cEleFocused.obj;
                                if (btcLostFocus.LostFocus != null)
                                    btcLostFocus.LostFocus((object)btcLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.CheckBox:
                            {
                                CheckBox chxLostFocus = (CheckBox)cEleFocused.obj;
                                if (chxLostFocus.LostFocus != null)
                                    chxLostFocus.LostFocus((object)chxLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.ComboBox:
                            {
                                ComboBox cmbLostFocus = (ComboBox)cEleFocused.obj;
                                if (cmbLostFocus.LostFocus != null)
                                    cmbLostFocus.LostFocus((object)cmbLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.getPoint:
                            {
                                getPoint txtLostFocus = (getPoint)cEleFocused.obj;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.getRectangle:
                            {
                                getRectangle txtLostFocus = (getRectangle)cEleFocused.obj;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.getSize:
                            {
                                getSize txtLostFocus = (getSize)cEleFocused.obj;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.GroupBox:
                            {
                                GroupBox lblLostFocus = (GroupBox)cEleFocused.obj;
                                if (lblLostFocus.LostFocus != null)
                                    lblLostFocus.LostFocus((object)lblLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.Label:
                            {
                                Label lblLostFocus = (Label)cEleFocused.obj;
                                if (lblLostFocus.LostFocus != null)
                                    lblLostFocus.LostFocus((object)lblLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.PictureBox:
                            {
                                PictureBox picLostFocus = (PictureBox)cEleFocused.obj;
                                if (picLostFocus.LostFocus != null)
                                    picLostFocus.LostFocus((object)picLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.ListBox:
                            {
                                ListBox lblLostFocus = (ListBox)cEleFocused.obj;
                                if (lblLostFocus.LostFocus != null)
                                    lblLostFocus.LostFocus((object)lblLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.CheckedListBox:
                            {
                                CheckedListBox cbxLostFocus = (CheckedListBox)cEleFocused.obj;
                                if (cbxLostFocus.LostFocus != null)
                                    cbxLostFocus.LostFocus((object)cbxLostFocus, new EventArgs());
                            }
                            break;

                        case enuSweepPrune_ObjectType.numericUpDown:
                            {
                                numericUpDown txtLostFocus = (numericUpDown)cEleFocused.obj;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.Panel:
                            {
                                Panel lblLostFocus = (Panel)cEleFocused.obj;
                                if (lblLostFocus.LostFocus != null)
                                    lblLostFocus.LostFocus((object)lblLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.ProgressBar:
                            {
                                ProgressBar prbLostFocus = (ProgressBar)cEleFocused.obj;
                                if (prbLostFocus.LostFocus != null)
                                    prbLostFocus.LostFocus((object)prbLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.TrackBar:
                            {
                                TrackBar trbLostFocus = (TrackBar)cEleFocused.obj;
                                if (trbLostFocus.LostFocus != null)
                                    trbLostFocus.LostFocus((object)trbLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.TextBox:
                            {
                                Textbox txtLostFocus = (Textbox)cEleFocused.obj;
                                DrawCursor(false);
                                txtLostFocus.SelectionStart = txtFocus.SelectionStart;
                                txtLostFocus.SelectionLength = txtFocus.SelectionLength;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;
                            

                        case enuSweepPrune_ObjectType.TextDisplay:
                            {
                                TextDisplay txtLostFocus = (TextDisplay)cEleFocused.obj;
                                if (txtLostFocus.LostFocus != null)
                                    txtLostFocus.LostFocus((object)txtLostFocus, new EventArgs());

                            }
                            break;

                        case enuSweepPrune_ObjectType.UserDefined:
                            {
                                SweepPrune_Object spObject = (SweepPrune_Object)cEleFocused.obj;
                                if (spObject.MouseDown!= null)
                                    spObject.MouseDown((object)spObject, MouseEventArgs_Shift(e, cEleFocused));

                            }
                            break;

                        default:
                            {
                                System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseClick() LostFocus default : this should not happen");

                            }
                            break;

                    }
                }

                if (cEleFocused != cEleUnderMouse)
                    cEleFocused = cEleUnderMouse;

                if (cEleFocused != null)
                {
                    switch (cEleFocused.eSP_ObjectType)
                    {
                        case enuSweepPrune_ObjectType.Button:
                            {
                                Button btnUnderMouse = (Button)cEleFocused.obj;
                                if (btnUnderMouse.GotFocus != null)
                                    btnUnderMouse.GotFocus((object)btnUnderMouse, new EventArgs());
                                if (btnUnderMouse.MouseDown != null)
                                    btnUnderMouse.MouseDown((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;
                            
                        case enuSweepPrune_ObjectType.SlideRuler:
                            {
                                SlideRuler btnUnderMouse = (SlideRuler)cEleFocused.obj;
                                if (btnUnderMouse.GotFocus != null)
                                    btnUnderMouse.GotFocus((object)btnUnderMouse, new EventArgs());
                                if (btnUnderMouse.MouseDown != null)
                                    btnUnderMouse.MouseDown((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.ButtonControl:
                            {
                                ButtonControl btcUnderMouse = (ButtonControl)cEleFocused.obj;
                                if (btcUnderMouse.GotFocus != null)
                                    btcUnderMouse.GotFocus((object)btcUnderMouse, new EventArgs());
                                if (btcUnderMouse.MouseDown != null)
                                    btcUnderMouse.MouseDown((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.CheckBox:
                            {
                                CheckBox chxUnderMouse = (CheckBox)cEleFocused.obj;
                                if (chxUnderMouse.GotFocus != null)
                                    chxUnderMouse.GotFocus((object)chxUnderMouse, new EventArgs());
                                if (chxUnderMouse.MouseDown != null)
                                    chxUnderMouse.MouseDown((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;


                        case enuSweepPrune_ObjectType.ComboBox:
                            {
                                ComboBox cmbUnderMouse = (ComboBox)cEleFocused.obj;
                                if (cmbUnderMouse.GotFocus != null)
                                    cmbUnderMouse.GotFocus((object)cmbUnderMouse, new EventArgs());
                                if (cmbUnderMouse.MouseDown != null)
                                    cmbUnderMouse.MouseDown((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.getPoint:
                            {
                                getPoint txtUnderMouse = (getPoint)cEleFocused.obj;
                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());
                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.getRectangle:
                            {
                                getRectangle txtUnderMouse = (getRectangle)cEleFocused.obj;
                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());
                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.getSize:
                            {
                                getSize txtUnderMouse = (getSize)cEleFocused.obj;
                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());
                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.GroupBox:
                            {
                                GroupBox lblUnderMouse = (GroupBox)cEleFocused.obj;
                                if (lblUnderMouse.GotFocus != null)
                                    lblUnderMouse.GotFocus((object)lblUnderMouse, new EventArgs());
                                if (lblUnderMouse.MouseDown != null)
                                    lblUnderMouse.MouseDown((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.Label:
                            {
                                Label lblUnderMouse = (Label)cEleFocused.obj;
                                if (lblUnderMouse.GotFocus != null)
                                    lblUnderMouse.GotFocus((object)lblUnderMouse, new EventArgs());
                                if (lblUnderMouse.MouseDown != null)
                                    lblUnderMouse.MouseDown((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.PictureBox:
                            {
                                PictureBox picUnderMouse = (PictureBox)cEleFocused.obj;
                                if (picUnderMouse.GotFocus != null)
                                    picUnderMouse.GotFocus((object)picUnderMouse, new EventArgs());
                                if (picUnderMouse.MouseDown != null)
                                    picUnderMouse.MouseDown((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.ListBox:
                            {
                                ListBox lbxUnderMouse = (ListBox)cEleFocused.obj;
                                lbxUnderMouse.lbx_MouseDown((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                                if (lbxUnderMouse.GotFocus != null)
                                    lbxUnderMouse.GotFocus((object)lbxUnderMouse, new EventArgs());
                                if (lbxUnderMouse.MouseDown != null)
                                    lbxUnderMouse.MouseDown((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.CheckedListBox:
                            {
                                CheckedListBox cbxUnderMouse = (CheckedListBox)cEleFocused.obj;
                                cbxUnderMouse.cbx_MouseDown((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                                if (cbxUnderMouse.GotFocus != null)
                                    cbxUnderMouse.GotFocus((object)cbxUnderMouse, new EventArgs());
                                if (cbxUnderMouse.MouseDown != null)
                                    cbxUnderMouse.MouseDown((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.numericUpDown:
                            {
                                numericUpDown txtUnderMouse = (numericUpDown)cEleFocused.obj;
                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());
                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.Panel:
                            {
                                Panel lblUnderMouse = (Panel)cEleFocused.obj;
                                if (lblUnderMouse.GotFocus != null)
                                    lblUnderMouse.GotFocus((object)lblUnderMouse, new EventArgs());
                                if (lblUnderMouse.MouseDown != null)
                                    lblUnderMouse.MouseDown((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.ProgressBar:
                            {
                                ProgressBar prbUnderMouse = (ProgressBar)cEleFocused.obj;
                                if (prbUnderMouse.GotFocus != null)
                                    prbUnderMouse.GotFocus((object)prbUnderMouse, new EventArgs());
                                System.Windows.Forms.MouseEventArgs mseArgs = MouseEventArgs_Shift(e, cEleUnderMouse);
                                if (prbUnderMouse.MouseDown != null)
                                    prbUnderMouse.MouseDown((object)prbUnderMouse, mseArgs);
                            }
                            break;

                        case enuSweepPrune_ObjectType.TrackBar:
                            {
                                TrackBar trbUnderMouse = (TrackBar)cEleFocused.obj;
                                if (trbUnderMouse.GotFocus != null)
                                    trbUnderMouse.GotFocus((object)trbUnderMouse, new EventArgs());
                                System.Windows.Forms.MouseEventArgs mseArgs = MouseEventArgs_Shift(e, cEleUnderMouse);
                                if (trbUnderMouse.MouseDown != null)
                                    trbUnderMouse.MouseDown((object)trbUnderMouse, mseArgs);

                                Point ptTRBMouse = new Point(mseArgs.X, mseArgs.Y);
                                if (classSPMath.PointIsInsideARectangle(ptTRBMouse, trbUnderMouse.recSlider))
                                {
                                    trbUnderMouse.GrabSlider = true;
                                }
                                else
                                {
                                    switch (trbUnderMouse.eStyle)
                                    {
                                        case TrackBar.enuTrackBar_Style.Horizontal:
                                            {
                                                if (ptTRBMouse.X < trbUnderMouse.recSlider.Left)
                                                    trbUnderMouse.Step_Left();
                                                else if (ptTRBMouse.X > trbUnderMouse.recSlider.Right)
                                                    trbUnderMouse.Step_Right();
                                            }
                                            break;

                                        case TrackBar.enuTrackBar_Style.Vertical:
                                            {
                                                if (ptTRBMouse.Y < trbUnderMouse.recSlider.Top)
                                                    trbUnderMouse.Step_Left();
                                                else if (ptTRBMouse.Y > trbUnderMouse.recSlider.Bottom)
                                                    trbUnderMouse.Step_Right();
                                            }
                                            break;
                                    }
                                }
                                trbUnderMouse.ptMouse = ptTRBMouse;
                            }
                            break;

                        case enuSweepPrune_ObjectType.TextBox:
                            {

                                Textbox txtUnderMouse = (Textbox)cEleFocused.obj;

                                bolTxtFocus_IgnoreEvents = true;

                                txtFocus.Text = txtUnderMouse.Text;
                                txtFocus.Font = txtUnderMouse.Font;
                                Point ptText = new Point(MouseLocation.X - cEleFocused.rec.Left, MouseLocation.Y - cEleFocused.rec.Top);
                                txtUnderMouse.indexMouseDown = txtUnderMouse.getIndexAtPoint(ptText);

                                txtFocus.SelectionStart
                                    = txtUnderMouse.SelectionStart
                                    = txtUnderMouse.indexMouseDown;
                                txtFocus.SelectionLength
                                    = txtUnderMouse.SelectionLength
                                    = 0;
                                txtFocus.Width = txtUnderMouse.recSPArea.Width + 20;
                                txtFocus.Focus();

                                bolTxtFocus_IgnoreEvents = false;

                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());

                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.TextDisplay:
                            {

                                TextDisplay txtUnderMouse = (TextDisplay)cEleFocused.obj;

                                //bolTxtFocus_IgnoreEvents = true;

                                //txtFocus.Text = txtUnderMouse.Text;
                                //txtFocus.Font = txtUnderMouse.Font;
                                //Point ptText = new Point(MouseLocation.X - cEleFocused.rec.Left, MouseLocation.Y - cEleFocused.rec.Top);
                                //txtUnderMouse.indexMouseDown = txtUnderMouse.getIndexAtPoint(ptText);

                                //txtFocus.SelectionStart
                                //    = txtUnderMouse.SelectionStart
                                //    = txtUnderMouse.indexMouseDown;
                                //txtFocus.SelectionLength
                                //    = txtUnderMouse.SelectionLength
                                //    = 0;
                                //txtFocus.Width = txtUnderMouse.recSPArea.Width + 20;
                                //txtFocus.Focus();

                                bolTxtFocus_IgnoreEvents = false;

                                if (txtUnderMouse.GotFocus != null)
                                    txtUnderMouse.GotFocus((object)txtUnderMouse, new EventArgs());

                                if (txtUnderMouse.MouseDown != null)
                                    txtUnderMouse.MouseDown((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                            }
                            break;

                        case enuSweepPrune_ObjectType.UserDefined:
                            {
                                // do nothing

                            }
                            break;

                        default:
                            {
                                System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseClick() GotFocus default : this should not happen");

                            }
                            break;

                    }
                }

                Point ptMouse = new Point(e.X, e.Y);

                if (VScrollBar.Visible && classSPMath.PointIsInsideARectangle(ptMouse, VScrollBar.recSliderDraw))
                {
                    VScrollBar.Grabbed = true;
                    VScrollBar.ptGrab = classSPMath.SubTwoPoints(ptMouse, VScrollBar.recSliderDraw.Location);
                }

                if (HScrollBar.Visible && classSPMath.PointIsInsideARectangle(ptMouse, HScrollBar.recSliderDraw))
                {
                    HScrollBar.Grabbed = true;
                    HScrollBar.ptGrab = classSPMath.SubTwoPoints(ptMouse, HScrollBar.recSliderDraw.Location);
                }
            }
        }

        public void pic_MouseUP(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseUp != null) MouseUp(sender, e);
            if (cEleUnderMouse != null)
            {
                switch (cEleUnderMouse.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseUp != null)
                                btnUnderMouse.MouseUp((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler btnUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseUp != null)
                                btnUnderMouse.MouseUp((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                            if (btcUnderMouse.MouseUp != null)
                                btcUnderMouse.MouseUp((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;
                            if (chxUnderMouse.MouseUp != null)
                                chxUnderMouse.MouseUp((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;
                            if (cmbUnderMouse.MouseUp != null)
                                cmbUnderMouse.MouseUp((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint gptUnderMouse = (getPoint)cEleUnderMouse.obj;
                            if (gptUnderMouse.MouseUp != null)
                                gptUnderMouse.MouseUp((object)gptUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle grcUnderMouse = (getRectangle)cEleUnderMouse.obj;
                            if (grcUnderMouse.MouseUp != null)
                                grcUnderMouse.MouseUp((object)grcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize gszUnderMouse = (getSize)cEleUnderMouse.obj;
                            if (gszUnderMouse.MouseUp != null)
                                gszUnderMouse.MouseUp((object)gszUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox grbUnderMouse = (GroupBox)cEleUnderMouse.obj;
                            if (grbUnderMouse.MouseUp != null)
                                grbUnderMouse.MouseUp((object)grbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                            if (picUnderMouse.MouseUp != null)
                                picUnderMouse.MouseUp((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseUp != null)
                                lblUnderMouse.MouseUp((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                            lbxUnderMouse.lbx_MouseUp((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (lbxUnderMouse.MouseUp != null)
                                lbxUnderMouse.MouseUp((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                            cbxUnderMouse.cbx_MouseUp((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (cbxUnderMouse.MouseUp != null)
                                cbxUnderMouse.MouseUp((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown nudUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                            if (nudUnderMouse.MouseUp != null)
                                nudUnderMouse.MouseUp((object)nudUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel pnlUnderMouse = (Panel)cEleUnderMouse.obj;
                            if (pnlUnderMouse.MouseUp != null)
                                pnlUnderMouse.MouseUp((object)pnlUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                            if (prbUnderMouse.MouseUp != null)
                                prbUnderMouse.MouseUp((object)prbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;

                            System.Windows.Forms.MouseEventArgs mseArgs = MouseEventArgs_Shift(e, cEleUnderMouse);
                            if (trbUnderMouse.MouseUp != null)
                                trbUnderMouse.MouseUp((object)trbUnderMouse, mseArgs);

                            trbUnderMouse.GrabSlider = false;
                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseUp != null)
                                txtUnderMouse.MouseUp((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        

                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseUp != null)
                                txtUnderMouse.MouseUp((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                            if (spObject.MouseUp != null)
                                spObject.MouseUp((object)spObject, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseUp.set() default:  this should not happen");

                        }
                        break;

                }
            }

            VScrollBar.Grabbed
                = HScrollBar.Grabbed
                = false;
        }

        private void pic_MouseLeave(object sender, EventArgs e)
        {
            if (MouseLeave != null) MouseLeave(sender, e);

            if (bolIgnoreMouseMove) return;
            tmrScroll.Enabled
                = VScrollBar.Grabbed
                = HScrollBar.Grabbed
                = false;
            //Cursor = System.Windows.Forms.Cursors.Arrow;
            return;
            //cEleUnderMouse = null;
            //cEleFocused = null;
        }

        public void pic_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseWheel != null) MouseWheel(sender, e);
            bool bolMouseOverScrollBar = (VScrollBar.Visible && MouseLocation.X > VScrollBar.recDraw.Left && MouseLocation.X < VScrollBar.recDraw.Right)
                                 ||
                                  (HScrollBar.Visible && MouseLocation.Y > HScrollBar.recDraw.Top && MouseLocation.Y < HScrollBar.recDraw.Bottom);

            if (!bolMouseOverScrollBar && cEleUnderMouse != null)
            {
                switch (cEleUnderMouse.eSP_ObjectType)
                {
                    case enuSweepPrune_ObjectType.Button:
                        {
                            Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                            if (btnUnderMouse.MouseWheel != null)
                                btnUnderMouse.MouseWheel((object)btnUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                        }
                        break;
                        
                    case enuSweepPrune_ObjectType.SlideRuler:
                        {
                            SlideRuler slrUnderMouse = (SlideRuler)cEleUnderMouse.obj;                            
                            if (slrUnderMouse.MouseWheel != null)
                                slrUnderMouse.MouseWheel((object)slrUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                        }
                        break;

                    case enuSweepPrune_ObjectType.ButtonControl:
                        {
                            ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                            if (btcUnderMouse.MouseWheel != null)
                                btcUnderMouse.MouseWheel((object)btcUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.CheckBox:
                        {
                            CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;
                            if (chxUnderMouse.MouseWheel != null)
                                chxUnderMouse.MouseWheel((object)chxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ComboBox:
                        {
                            ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;
                            if (cmbUnderMouse.MouseWheel != null)
                                cmbUnderMouse.MouseWheel((object)cmbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getPoint:
                        {
                            getPoint gptUnderMouse = (getPoint)cEleUnderMouse.obj;
                            if (gptUnderMouse.MouseWheel != null)
                                gptUnderMouse.MouseWheel((object)gptUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getRectangle:
                        {
                            getRectangle gszUnderMouse = (getRectangle)cEleUnderMouse.obj;
                            if (gszUnderMouse.MouseWheel != null)
                                gszUnderMouse.MouseWheel((object)gszUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.getSize:
                        {
                            getSize gszUnderMouse = (getSize)cEleUnderMouse.obj;
                            if (gszUnderMouse.MouseWheel != null)
                                gszUnderMouse.MouseWheel((object)gszUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.GroupBox:
                        {
                            GroupBox grbUnderMouse = (GroupBox)cEleUnderMouse.obj;
                            if (grbUnderMouse.MouseWheel != null)
                                grbUnderMouse.MouseWheel((object)grbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Label:
                        {
                            Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                            if (lblUnderMouse.MouseWheel != null)
                                lblUnderMouse.MouseWheel((object)lblUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.PictureBox:
                        {
                            PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                            if (picUnderMouse.MouseWheel != null)
                                picUnderMouse.MouseWheel((object)picUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ListBox:
                        {
                            ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                            lbxUnderMouse.lbx_MouseWheel((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (lbxUnderMouse.MouseWheel != null)
                                lbxUnderMouse.MouseWheel((object)lbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;


                    case enuSweepPrune_ObjectType.CheckedListBox:
                        {
                            CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                            cbxUnderMouse.cbx_MouseWheel((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (cbxUnderMouse.MouseWheel != null)
                                cbxUnderMouse.MouseWheel((object)cbxUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.numericUpDown:
                        {
                            numericUpDown nudUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                            if (e.Delta < 0)
                                nudUnderMouse.Scroll_Down();
                            else if (e.Delta > 0)
                                nudUnderMouse.Scroll_Up();
                            if (nudUnderMouse.MouseWheel != null)
                                nudUnderMouse.MouseWheel((object)nudUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.Panel:
                        {
                            Panel pnlUnderMouse = (Panel)cEleUnderMouse.obj;
                            if (pnlUnderMouse.MouseWheel != null)
                                pnlUnderMouse.MouseWheel((object)pnlUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.ProgressBar:
                        {
                            ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                            if (prbUnderMouse.MouseWheel != null)
                                prbUnderMouse.MouseWheel((object)prbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.TrackBar:
                        {
                            TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                            if (trbUnderMouse.MouseWheel != null)
                                trbUnderMouse.MouseWheel((object)trbUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));
                            if (e.Delta > 0) trbUnderMouse.Slide_Left();
                            else if (e.Delta < 0) trbUnderMouse.Slide_Right();
                        }
                        break;

                    case enuSweepPrune_ObjectType.TextBox:
                        {
                            Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseWheel != null)
                                txtUnderMouse.MouseWheel((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;
                        

                    case enuSweepPrune_ObjectType.TextDisplay:
                        {
                            TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                            if (txtUnderMouse.MouseWheel != null)
                                txtUnderMouse.MouseWheel((object)txtUnderMouse, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    case enuSweepPrune_ObjectType.UserDefined:
                        {
                            SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                            
                            if (spObject.MouseWheel!= null)
                                spObject.MouseWheel((object)spObject, MouseEventArgs_Shift(e, cEleUnderMouse));

                        }
                        break;

                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseWheel.set() default:  this should not happen");

                        }
                        break;

                }
            }

            if (!Scrolling_Disabled && !Scrolling_Override)
            {
     
                if ((cEleUnderMouse != null
                        && cEleUnderMouse.eSP_ObjectType != enuSweepPrune_ObjectType.ListBox
                        && cEleUnderMouse.eSP_ObjectType != enuSweepPrune_ObjectType.CheckedListBox
                        && cEleUnderMouse.eSP_ObjectType != enuSweepPrune_ObjectType.numericUpDown
                        && cEleUnderMouse.eSP_ObjectType != enuSweepPrune_ObjectType.TextDisplay)
                     || cEleUnderMouse == null
                     || bolMouseOverScrollBar)
                {
                    if (VScrollBar.Visible)
                    {
                        if (e.Delta > 0)
                            VScrollBar.ScrollUp();
                        else
                            VScrollBar.ScrollDown();
                    }
                    else if (HScrollBar.Visible)
                    {
                        if (e.Delta > 0)
                            HScrollBar.ScrollUp();
                        else
                            HScrollBar.ScrollDown();
                    }
                }
            }
            Scrolling_Override = false;
        }
        #endregion
        #endregion

        #region Properties      
        public System.Windows.Forms.Control Control
        {
            get
            {
                System.Windows.Forms.Control cParent = pic.Parent;
                while (cParent.Parent != null)
                    cParent = cParent.Parent;
                return cParent;
            }
}


System.Windows.Forms.KeyEventHandler _KeyDown = null;
        public System.Windows.Forms.KeyEventHandler event_KeyDown
        {
            get { return _KeyDown; }
            set { _KeyDown = value; }
        }

        System.Windows.Forms.KeyEventHandler _KeyUp = null;
        public System.Windows.Forms.KeyEventHandler event_KeyUp
        {
            get { return _KeyUp; }
            set { _KeyUp = value; }
        }

        System.Windows.Forms.KeyPressEventHandler _KeyPress = null;
        public System.Windows.Forms.KeyPressEventHandler event_KeyPress
        {
            get { return _KeyPress; }
            set { _KeyPress = value; }
        }

        System.EventHandler _cEleUnderMouseChange = null;
        public System.EventHandler cEleUnderMouseChange
        {
            get { return _cEleUnderMouseChange; }
            set { _cEleUnderMouseChange = value; }
        }

        bool bolCropToFitPictureBox = true;
        public bool CropToFitPictureBox
        {
            get { return bolCropToFitPictureBox; }
            set { bolCropToFitPictureBox = value; }
        }



        EventHandler _event_NeedsToBeRedrawn = null;
        public EventHandler event_NeedsToBeRedrawn
        {
            get { return _event_NeedsToBeRedrawn; }
            set { _event_NeedsToBeRedrawn = value; }
        }

        public bool VScrollBar_Visible
        {
            get { return VScrollBar.Visible; }
        }
        public bool HScrollBar_Visible
        {
            get { return HScrollBar.Visible; }
        }

        public Rectangle VScrollBar_Area
        {
            get { return VScrollBar.recDraw; }
        }

        public Rectangle HScrollBar_Area
        {
            get { return HScrollBar.recDraw; }
        }

        public Rectangle VScrollBarSlider_Area
        {
            get { return VScrollBar.recSliderDraw; }
        }

        public Rectangle HScrollBarSlider_Area
        {
            get { return HScrollBar.recSliderDraw; }
        }



        bool bolDisableScrolling = false;
        public bool Scrolling_Disabled
        {
            get { return bolDisableScrolling; }
            set { bolDisableScrolling = value; }
        }

        bool bolOverrideScrolling = false;
        public bool Scrolling_Override
        {
            get { return bolOverrideScrolling; }
            set { bolOverrideScrolling = value; }
        }

        enuScrollingStyle eScrollingStyle = enuScrollingStyle.ScrollBars;
        public enuScrollingStyle ScrollingStyle
        {
            get { return eScrollingStyle; }
            set { eScrollingStyle = value; }
        }

        public bool VScrollBarGrabbed
        {
            get { return VScrollBar.Grabbed; }
        }

        public bool HScrollBarGrabbed
        {
            get { return HScrollBar.Grabbed; }
        }

        bool bolNeedsToBeRedrawn = false;
        public bool NeedsToBeRedrawn
        {
            get { return bolNeedsToBeRedrawn; }
            set
            {
                if (bolNeedsToBeRedrawn != value)
                {
                    if (bolBuildingcontainer) return;
                    bolNeedsToBeRedrawn = value;
                    if (bolNeedsToBeRedrawn)
                        Draw();
                    if (event_NeedsToBeRedrawn != null)
                        event_NeedsToBeRedrawn((object)this, new EventArgs());
                }
            }
        }

        bool _bolBuildingcontainer = true;
        bool bolBuildingcontainer
        {
            get { return _bolBuildingcontainer; }
            set
            {
                if (_bolBuildingcontainer != value)
                {
                    _bolBuildingcontainer = value;
                }
            }
        }
        public bool BuildingInProgress
        {
            get { return bolBuildingcontainer; }
        }

        bool bolDockFill = false;
        /// <summary>
        /// sets recVisible equal to recSPArea
        /// </summary>
        public bool DockFill
        {
            get { return bolDockFill; }
            set
            {
                if (bolDockFill != value)
                {
                    bolDockFill = value;
                    NeedsToBeRedrawn = true;
                }
            }
        }

        bool bolLstElements_Removed = false;
        public bool lstElements_Removed
        {
            get { return bolLstElements_Removed; }
            set { bolLstElements_Removed = value; }
        }


        bool bolRecVisibleChanged = true;
        public bool RecVisible_Changed
        {
            get { return bolRecVisibleChanged; }
            set { bolRecVisibleChanged = value; }
        }

        Rectangle _recVisible = new Rectangle();
        public Rectangle recVisible
        {
            get
            {
                if (DockFill)
                {
                    return new Rectangle(_recVisible.Left,
                                         _recVisible.Top,
                                         recSPArea.Width < pic.Width
                                                         ? recSPArea.Width
                                                         : pic.Width,
                                         recSPArea.Height < pic.Height
                                                         ? recSPArea.Height
                                                         : pic.Height);
                }

                else
                    return _recVisible;
            }
            set
            {
                if (recVisible.Left != value.Left
                    || recVisible.Top != value.Top
                    || recVisible.Width != value.Width
                    || recVisible.Height != value.Height)
                {
                    _recVisible = value;
                    //if (_recVisible.Width > recSPArea.Width)
                    //    _recVisible.Width = recSPArea.Width;
                    //if (_recVisible.Height > recSPArea.Height)
                    //    _recVisible.Height = recSPArea.Height;

                    //if (_recVisible.Left < recSPArea.Left)
                    //    _recVisible.X = recSPArea.Left;
                    //if (_recVisible.Right > recSPArea.Right)
                    //    _recVisible.X = recSPArea.Right - _recVisible.Width;
                    //if (_recVisible.Top < recSPArea.Top)
                    //    _recVisible.Y = recSPArea.Top;
                    //if (_recVisible.Bottom > recSPArea.Bottom)
                    //    _recVisible.Y = recSPArea.Bottom - _recVisible.Height;

                    if (CropToFitPictureBox)
                    {
                        if (_recVisible.Width > pic.Width)
                            _recVisible.Width = pic.Width;
                        if (_recVisible.Height > pic.Height)
                            _recVisible.Height = pic.Height;
                    }

                    ScrollBars_SetSizes();
                    bolRecVisibleChanged = true;
                    NeedsToBeRedrawn = true;
                    //Draw();
                }
            }
        }

        Point _MouseLocation = new Point();
        Point MouseLocation
        {
            get { return _MouseLocation; }
            set
            {
                //if (Math.Abs(MouseLocation.X - value.X) + Math.Abs(MouseLocation.Y - value.Y) > 2)
                {
                    DateTime dtStart = DateTime.Now;
                    List<classSweepAndPrune_Element> lstSearchResults = Search(value);
                    DateTime dtEnd = DateTime.Now;
                    TimeSpan ts = dtEnd.Subtract(dtStart);

                    if (lstSearchResults != null && lstSearchResults.Count > 0)
                    {
                        IEnumerable<classSweepAndPrune_Element> query = lstSearchResults.OrderByDescending(ele => ele.Index);
                        lstSearchResults = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
                        while (lstSearchResults.Count > 0 && !lstSearchResults[0].Visible)
                            lstSearchResults.RemoveAt(0);
                        if (lstSearchResults.Count > 0)
                            cEleUnderMouse = lstSearchResults[0];
                        else
                            cEleUnderMouse = null;
                    }
                    else
                        cEleUnderMouse = null;
                    _MouseLocation = value;
                }
            }
        }


        //System.Windows.Forms.ContextMenu _ContextMenu = null;
        //public System.Windows.Forms.ContextMenu ContextMenu
        //{
        //    get { return _ContextMenu; }
        //    set { _ContextMenu = value; }
        //}

        classSweepAndPrune_Element _cEleUnderMouse = null;
        public classSweepAndPrune_Element cEleUnderMouse
        {
            get { return _cEleUnderMouse; }
            set
            {
                if (_cEleUnderMouse != value)
                {
           

                    // mouse is under a different element
                    if (cEleUnderMouse != null)
                    {
                        //System.Diagnostics.Debug.Print("cEleUnderMouse Old:" + cEleUnderMouse.ID);

                        // mouse has left previous element
                        switch (cEleUnderMouse.eSP_ObjectType)
                        {
                            case enuSweepPrune_ObjectType.Button:
                                {
                                    Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;
                                
                            case enuSweepPrune_ObjectType.SlideRuler:
                                {
                                    SlideRuler btnUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.ButtonControl:
                                {
                                    ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                                    btcUnderMouse.Highlight = false;
                                    if (btcUnderMouse.MouseLeave != null)
                                        btcUnderMouse.MouseLeave((object)btcUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckBox:
                                {
                                    CheckBox btnUnderMouse = (CheckBox)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.ComboBox:
                                {
                                    ComboBox btnUnderMouse = (ComboBox)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.getPoint:
                                {
                                    getPoint gptUnderMouse = (getPoint)cEleUnderMouse.obj;
                                    if (gptUnderMouse.MouseLeave != null)
                                        gptUnderMouse.MouseLeave((object)gptUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getRectangle:
                                {
                                    getRectangle gszUnderMouse = (getRectangle)cEleUnderMouse.obj;
                                    if (gszUnderMouse.MouseLeave != null)
                                        gszUnderMouse.MouseLeave((object)gszUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.getSize:
                                {
                                    getSize gszUnderMouse = (getSize)cEleUnderMouse.obj;
                                    if (gszUnderMouse.MouseLeave != null)
                                        gszUnderMouse.MouseLeave((object)gszUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.GroupBox:
                                {
                                    GroupBox btnUnderMouse = (GroupBox)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Label:
                                {
                                    Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                                    if (lblUnderMouse.MouseLeave != null)
                                        lblUnderMouse.MouseLeave((object)lblUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.PictureBox:
                                {
                                    PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                                    if (picUnderMouse.MouseLeave != null)
                                        picUnderMouse.MouseLeave((object)picUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                }
                                break;

                            case enuSweepPrune_ObjectType.ListBox:
                                {
                                    ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                                    lbxUnderMouse.lbxMouseLeave((object)lbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                    if (lbxUnderMouse.MouseLeave != null)
                                        lbxUnderMouse.MouseLeave((object)lbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckedListBox:
                                {
                                    CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                                    cbxUnderMouse.cbx_MouseLeave((object)cbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                    if (cbxUnderMouse.MouseLeave != null)
                                        cbxUnderMouse.MouseLeave((object)cbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.numericUpDown:
                                {
                                    numericUpDown nudUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                                    nudUnderMouse.Highlight(new Point(-10, -10));
                                    if (nudUnderMouse.MouseLeave != null)
                                        nudUnderMouse.MouseLeave((object)nudUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Panel:
                                {
                                    Panel btnUnderMouse = (Panel)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ProgressBar:
                                {
                                    ProgressBar btnUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = false;
                                    if (btnUnderMouse.MouseLeave != null)
                                        btnUnderMouse.MouseLeave((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TrackBar:
                                {
                                    TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                                    trbUnderMouse.Highlight = false;
                                    trbUnderMouse.GrabSlider = false;
                                    if (trbUnderMouse.MouseLeave != null)
                                        trbUnderMouse.MouseLeave((object)trbUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TextBox:
                                {
                                    Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                                    txtUnderMouse.Highlight = false;
                                    if (txtUnderMouse.MouseLeave != null)
                                        txtUnderMouse.MouseLeave((object)txtUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;
                                

                            case enuSweepPrune_ObjectType.TextDisplay:
                                {
                                    TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                                    txtUnderMouse.Highlight = false;
                                    if (txtUnderMouse.MouseLeave != null)
                                        txtUnderMouse.MouseLeave((object)txtUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.UserDefined:
                                {
                                    // do nothing

                                }
                                break;

                            default:
                                {
                                    System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseLocation.set() case default : this should not happen");

                                }
                                break;
                        }
                    }

                    _cEleUnderMouse = value;

                    if (cEleUnderMouseChange != null)
                    {
                        cEleUnderMouseChange(_cEleUnderMouse, new EventArgs());
                    }

                    //if (value != null)
                    {
                        bolIgnoreMouseMove = true;
                        Pic_Cycle();
                        bolIgnoreMouseMove = false;
                    }
                    // mouse is under a new element
                    if (cEleUnderMouse != null)
                    {
                        //System.Diagnostics.Debug.Print("cEleUnderMouse Old:" + cEleUnderMouse.ID);
                        // mouse has left previous element
                        switch (cEleUnderMouse.eSP_ObjectType)
                        {
                            case enuSweepPrune_ObjectType.Button:
                                {
                                    Button btnUnderMouse = (Button)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = true;
                                    if (btnUnderMouse.MouseEnter != null)
                                        btnUnderMouse.MouseEnter((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - btnUnderMouse.cEle.rec.Left, MouseLocation.Y - btnUnderMouse.cEle.rec.Top, 0));

                                }
                                break;
                                
                            case enuSweepPrune_ObjectType.SlideRuler:
                                {
                                    SlideRuler btnUnderMouse = (SlideRuler)cEleUnderMouse.obj;
                                    btnUnderMouse.Highlight = true;
                                    if (btnUnderMouse.MouseEnter != null)
                                        btnUnderMouse.MouseEnter((object)btnUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - btnUnderMouse.cEle.rec.Left, MouseLocation.Y - btnUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ButtonControl:
                                {
                                    ButtonControl btcUnderMouse = (ButtonControl)cEleUnderMouse.obj;
                                    btcUnderMouse.Highlight = true;
                                    if (btcUnderMouse.MouseEnter != null)
                                        btcUnderMouse.MouseEnter((object)btcUnderMouse,
                                                                 new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None,
                                                                                                         0,
                                                                                                         MouseLocation.X - btcUnderMouse.cEle.rec.Left,
                                                                                                         MouseLocation.Y - btcUnderMouse.cEle.rec.Top,
                                                                                                         0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckBox:
                                {
                                    CheckBox chxUnderMouse = (CheckBox)cEleUnderMouse.obj;
                                    chxUnderMouse.Highlight = true;
                                    if (chxUnderMouse.MouseEnter != null)
                                        chxUnderMouse.MouseEnter((object)chxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - chxUnderMouse.cEle.rec.Left, MouseLocation.Y - chxUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ComboBox:
                                {
                                    ComboBox cmbUnderMouse = (ComboBox)cEleUnderMouse.obj;
                                    cmbUnderMouse.Highlight = true;
                                    if (cmbUnderMouse.MouseEnter != null)
                                        cmbUnderMouse.MouseEnter((object)cmbUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cmbUnderMouse.cEle.rec.Left, MouseLocation.Y - cmbUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getPoint:
                                {
                                    getPoint gptUnderMouse = (getPoint)cEleUnderMouse.obj;
                                    if (gptUnderMouse.MouseEnter != null)
                                        gptUnderMouse.MouseEnter((object)gptUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gptUnderMouse.cEle.rec.Left, MouseLocation.Y - gptUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getRectangle:
                                {
                                    getRectangle gszUnderMouse = (getRectangle)cEleUnderMouse.obj;
                                    if (gszUnderMouse.MouseEnter != null)
                                        gszUnderMouse.MouseEnter((object)gszUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gszUnderMouse.cEle.rec.Left, MouseLocation.Y - gszUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getSize:
                                {
                                    getSize gszUnderMouse = (getSize)cEleUnderMouse.obj;
                                    if (gszUnderMouse.MouseEnter != null)
                                        gszUnderMouse.MouseEnter((object)gszUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gszUnderMouse.cEle.rec.Left, MouseLocation.Y - gszUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.GroupBox:
                                {
                                    GroupBox grbUnderMouse = (GroupBox)cEleUnderMouse.obj;
                                    grbUnderMouse.Highlight = true;
                                    if (grbUnderMouse.MouseEnter != null)
                                        grbUnderMouse.MouseEnter((object)grbUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - grbUnderMouse.cEle.rec.Left, MouseLocation.Y - grbUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Label:
                                {
                                    Label lblUnderMouse = (Label)cEleUnderMouse.obj;
                                    //lblUnderMouse.Highlight = true;
                                    if (lblUnderMouse.MouseEnter != null)
                                        lblUnderMouse.MouseEnter((object)lblUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lblUnderMouse.cEle.rec.Left, MouseLocation.Y - lblUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.PictureBox:
                                {
                                    PictureBox picUnderMouse = (PictureBox)cEleUnderMouse.obj;
                                    //picUnderMouse.Highlight = true;
                                    if (picUnderMouse.MouseEnter != null)
                                        picUnderMouse.MouseEnter((object)picUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - picUnderMouse.cEle.rec.Left, MouseLocation.Y - picUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ListBox:
                                {
                                    ListBox lbxUnderMouse = (ListBox)cEleUnderMouse.obj;
                                    lbxUnderMouse.lbx_MouseEnter((object)lbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                    if (lbxUnderMouse.MouseEnter != null)
                                        lbxUnderMouse.MouseEnter((object)lbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lbxUnderMouse.cEle.rec.Left, MouseLocation.Y - lbxUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckedListBox:
                                {
                                    CheckedListBox cbxUnderMouse = (CheckedListBox)cEleUnderMouse.obj;
                                    cbxUnderMouse.cbx_MouseEnter((object)cbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));
                                    if (cbxUnderMouse.MouseEnter != null)
                                        cbxUnderMouse.MouseEnter((object)cbxUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cbxUnderMouse.cEle.rec.Left, MouseLocation.Y - cbxUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.numericUpDown:
                                {
                                    numericUpDown nudUnderMouse = (numericUpDown)cEleUnderMouse.obj;
                                    if (nudUnderMouse.MouseEnter != null)
                                        nudUnderMouse.MouseEnter((object)nudUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - nudUnderMouse.cEle.rec.Left, MouseLocation.Y - nudUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Panel:
                                {
                                    Panel pnlUnderMouse = (Panel)cEleUnderMouse.obj;
                                    pnlUnderMouse.Highlight = true;
                                    if (pnlUnderMouse.MouseEnter != null)
                                        pnlUnderMouse.MouseEnter((object)pnlUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - pnlUnderMouse.cEle.rec.Left, MouseLocation.Y - pnlUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ProgressBar:
                                {
                                    ProgressBar prbUnderMouse = (ProgressBar)cEleUnderMouse.obj;
                                    prbUnderMouse.Highlight = true;
                                    if (prbUnderMouse.MouseEnter != null)
                                        prbUnderMouse.MouseEnter((object)prbUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - prbUnderMouse.cEle.rec.Left, MouseLocation.Y - prbUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TrackBar:
                                {
                                    TrackBar trbUnderMouse = (TrackBar)cEleUnderMouse.obj;
                                    trbUnderMouse.Highlight = true;
                                    if (trbUnderMouse.MouseEnter != null)
                                        trbUnderMouse.MouseEnter((object)trbUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - trbUnderMouse.cEle.rec.Left, MouseLocation.Y - trbUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TextBox:
                                {
                                    Textbox txtUnderMouse = (Textbox)cEleUnderMouse.obj;
                                    txtUnderMouse.Highlight = true;
                                    if (txtUnderMouse.MouseEnter != null)
                                        txtUnderMouse.MouseEnter((object)txtUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - txtUnderMouse.cEle.rec.Left, MouseLocation.Y - txtUnderMouse.cEle.rec.Top, 0));

                                }
                                break;
                                

                            case enuSweepPrune_ObjectType.TextDisplay:
                                {
                                    TextDisplay txtUnderMouse = (TextDisplay)cEleUnderMouse.obj;
                                    txtUnderMouse.Highlight = true;
                                    if (txtUnderMouse.MouseEnter != null)
                                        txtUnderMouse.MouseEnter((object)txtUnderMouse, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - txtUnderMouse.cEle.rec.Left, MouseLocation.Y - txtUnderMouse.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.UserDefined:
                                {
                                    SweepPrune_Object spObject = (SweepPrune_Object)cEleUnderMouse.obj;
                                    if (spObject.MouseEnter != null)
                                        spObject.MouseEnter((object)spObject, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - spObject.cEle.rec.Left, MouseLocation.Y - spObject.cEle.rec.Top, 0));
                                }
                                break;

                            default:
                                {
                                    System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseLocation.set() case default : this should not happen");

                                }
                                break;

                        }
                    }
                }
            }
        }

        classSweepAndPrune_Element _cEleFocused = null;
        public classSweepAndPrune_Element cEleFocused
        {
            get { return _cEleFocused; }
            set
            {
                if (_cEleFocused != value)
                {
                    // mouse is under a different element
                    if (cEleFocused != null)
                    {
                        // mouse has left previous element
                        switch (cEleFocused.eSP_ObjectType)
                        {
                            case enuSweepPrune_ObjectType.Button:
                                {
                                    Button btnFocused = (Button)cEleFocused.obj;
                                    btnFocused.Highlight = false;
                                    if (btnFocused.LostFocus != null)
                                        btnFocused.LostFocus((object)btnFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;
                                
                            case enuSweepPrune_ObjectType.SlideRuler:
                                {
                                    SlideRuler btnFocused = (SlideRuler)cEleFocused.obj;
                                    btnFocused.Highlight = false;
                                    if (btnFocused.LostFocus != null)
                                        btnFocused.LostFocus((object)btnFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ButtonControl:
                                {
                                    ButtonControl btcFocused = (ButtonControl)cEleFocused.obj;
                                    btcFocused.Highlight = false;
                                    if (btcFocused.LostFocus != null)
                                        btcFocused.LostFocus((object)btcFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckBox:
                                {
                                    CheckBox chxFocused = (CheckBox)cEleFocused.obj;
                                    if (chxFocused.LostFocus != null)
                                        chxFocused.LostFocus((object)chxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - chxFocused.cEle.rec.Left, MouseLocation.Y - chxFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ComboBox:
                                {
                                    ComboBox cmbFocused = (ComboBox)cEleFocused.obj;
                                    if (cmbFocused.LostFocus != null)
                                        cmbFocused.LostFocus((object)cmbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cmbFocused.cEle.rec.Left, MouseLocation.Y - cmbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getPoint:
                                {
                                    getPoint gptFocused = (getPoint)cEleFocused.obj;
                                    if (gptFocused.LostFocus != null)
                                        gptFocused.LostFocus((object)gptFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getRectangle:
                                {
                                    getRectangle gszFocused = (getRectangle)cEleFocused.obj;
                                    if (gszFocused.LostFocus != null)
                                        gszFocused.LostFocus((object)gszFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getSize:
                                {
                                    getSize gszFocused = (getSize)cEleFocused.obj;
                                    if (gszFocused.LostFocus != null)
                                        gszFocused.LostFocus((object)gszFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.GroupBox:
                                {
                                    GroupBox grbFocused = (GroupBox)cEleFocused.obj;
                                    if (grbFocused.LostFocus != null)
                                        grbFocused.LostFocus((object)grbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - grbFocused.cEle.rec.Left, MouseLocation.Y - grbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Label:
                                {
                                    Label lblFocused = (Label)cEleFocused.obj;
                                    if (lblFocused.LostFocus != null)
                                        lblFocused.LostFocus((object)lblFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lblFocused.cEle.rec.Left, MouseLocation.Y - lblFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.PictureBox:
                                {
                                    PictureBox picFocused = (PictureBox)cEleFocused.obj;
                                    if (picFocused.LostFocus != null)
                                        picFocused.LostFocus((object)picFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - picFocused.cEle.rec.Left, MouseLocation.Y - picFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ListBox:
                                {
                                    ListBox lbxFocused = (ListBox)cEleFocused.obj;
                                    if (lbxFocused.LostFocus != null)
                                        lbxFocused.LostFocus((object)lbxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lbxFocused.cEle.rec.Left, MouseLocation.Y - lbxFocused.cEle.rec.Top, 0));

                                }
                                break;


                            case enuSweepPrune_ObjectType.CheckedListBox:
                                {
                                    CheckedListBox cbxFocused = (CheckedListBox)cEleFocused.obj;
                                    if (cbxFocused.LostFocus != null)
                                        cbxFocused.LostFocus((object)cbxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cbxFocused.cEle.rec.Left, MouseLocation.Y - cbxFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.numericUpDown:
                                {
                                    numericUpDown nudFocused = (numericUpDown)cEleFocused.obj;
                                    if (nudFocused.LostFocus != null)
                                        nudFocused.LostFocus((object)nudFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Panel:
                                {
                                    Panel pnlFocused = (Panel)cEleFocused.obj;
                                    if (pnlFocused.LostFocus != null)
                                        pnlFocused.LostFocus((object)pnlFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - pnlFocused.cEle.rec.Left, MouseLocation.Y - pnlFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ProgressBar:
                                {
                                    ProgressBar prbFocused = (ProgressBar)cEleFocused.obj;
                                    if (prbFocused.LostFocus != null)
                                        prbFocused.LostFocus((object)prbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - prbFocused.cEle.rec.Left, MouseLocation.Y - prbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TrackBar:
                                {
                                    TrackBar trbFocused = (TrackBar)cEleFocused.obj;
                                    if (trbFocused.LostFocus != null)
                                        trbFocused.LostFocus((object)trbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - trbFocused.cEle.rec.Left, MouseLocation.Y - trbFocused.cEle.rec.Top, 0));
                                    trbFocused.GrabSlider = false;
                                }
                                break;

                            case enuSweepPrune_ObjectType.TextBox:
                                {
                                    Textbox txtFocused = (Textbox)cEleFocused.obj;
                                    if (txtFocused.LostFocus != null)
                                        txtFocused.LostFocus((object)txtFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;
                                

                            case enuSweepPrune_ObjectType.TextDisplay:
                                {
                                    TextDisplay txtFocused = (TextDisplay)cEleFocused.obj;
                                    if (txtFocused.LostFocus != null)
                                        txtFocused.LostFocus((object)txtFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.UserDefined:
                                {
                                    // do nothing

                                }
                                break;

                            default:
                                {
                                    System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseLocation.set() case default : this should not happen");

                                }
                                break;

                        }
                    }


                    _cEleFocused = value;

                    // mouse is under a new element
                    if (cEleFocused != null)
                    {
                        // mouse has left previous element
                        switch (cEleFocused.eSP_ObjectType)
                        {
                            case enuSweepPrune_ObjectType.Button:
                                {
                                    Button btnFocused = (Button)cEleFocused.obj;
                                    btnFocused.Highlight = true;
                                    if (btnFocused.GotFocus != null)
                                        btnFocused.GotFocus((object)btnFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - btnFocused.cEle.rec.Left, MouseLocation.Y - btnFocused.cEle.rec.Top, 0));

                                }
                                break;
                                
                            case enuSweepPrune_ObjectType.SlideRuler:
                                {
                                    SlideRuler btnFocused = (SlideRuler)cEleFocused.obj;
                                    btnFocused.Highlight = true;
                                    if (btnFocused.GotFocus != null)
                                        btnFocused.GotFocus((object)btnFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - btnFocused.cEle.rec.Left, MouseLocation.Y - btnFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ButtonControl:
                                {
                                    ButtonControl btcFocused = (ButtonControl)cEleFocused.obj;
                                    btcFocused.Highlight = true;
                                    if (btcFocused.GotFocus != null)
                                        btcFocused.GotFocus((object)btcFocused,
                                                            new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None,
                                                                                                    0,
                                                                                                    MouseLocation.X - btcFocused.cEle.rec.Left,
                                                                                                    MouseLocation.Y - btcFocused.cEle.rec.Top,
                                                                                                    0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.CheckBox:
                                {
                                    CheckBox chxFocused = (CheckBox)cEleFocused.obj;
                                    chxFocused.Highlight = true;
                                    if (chxFocused.GotFocus != null)
                                        chxFocused.GotFocus((object)chxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - chxFocused.cEle.rec.Left, MouseLocation.Y - chxFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ComboBox:
                                {
                                    ComboBox cmbFocused = (ComboBox)cEleFocused.obj;
                                    cmbFocused.Highlight = true;
                                    if (cmbFocused.GotFocus != null)
                                        cmbFocused.GotFocus((object)cmbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cmbFocused.cEle.rec.Left, MouseLocation.Y - cmbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getPoint:
                                {
                                    getPoint gptFocused = (getPoint)cEleFocused.obj;
                                    if (gptFocused.GotFocus != null)
                                        gptFocused.GotFocus((object)gptFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gptFocused.cEle.rec.Left, MouseLocation.Y - gptFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getRectangle:
                                {
                                    getRectangle gszFocused = (getRectangle)cEleFocused.obj;
                                    if (gszFocused.GotFocus != null)
                                        gszFocused.GotFocus((object)gszFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gszFocused.cEle.rec.Left, MouseLocation.Y - gszFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.getSize:
                                {
                                    getSize gszFocused = (getSize)cEleFocused.obj;
                                    if (gszFocused.GotFocus != null)
                                        gszFocused.GotFocus((object)gszFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - gszFocused.cEle.rec.Left, MouseLocation.Y - gszFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.GroupBox:
                                {
                                    GroupBox grbFocused = (GroupBox)cEleFocused.obj;
                                    if (grbFocused.GotFocus != null)
                                        grbFocused.GotFocus((object)grbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - grbFocused.cEle.rec.Left, MouseLocation.Y - grbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Label:
                                {
                                    Label lblFocused = (Label)cEleFocused.obj;
                                    if (lblFocused.GotFocus != null)
                                        lblFocused.GotFocus((object)lblFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lblFocused.cEle.rec.Left, MouseLocation.Y - lblFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.PictureBox:
                                {
                                    PictureBox picFocused = (PictureBox)cEleFocused.obj;
                                    if (picFocused.GotFocus != null)
                                        picFocused.GotFocus((object)picFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - picFocused.cEle.rec.Left, MouseLocation.Y - picFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ListBox:
                                {
                                    ListBox lbxFocused = (ListBox)cEleFocused.obj;
                                    if (lbxFocused.GotFocus != null)
                                        lbxFocused.GotFocus((object)lbxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - lbxFocused.cEle.rec.Left, MouseLocation.Y - lbxFocused.cEle.rec.Top, 0));

                                }
                                break;


                            case enuSweepPrune_ObjectType.CheckedListBox:
                                {
                                    CheckedListBox cbxFocused = (CheckedListBox)cEleFocused.obj;
                                    if (cbxFocused.GotFocus != null)
                                        cbxFocused.GotFocus((object)cbxFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - cbxFocused.cEle.rec.Left, MouseLocation.Y - cbxFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.numericUpDown:
                                {
                                    numericUpDown nudFocused = (numericUpDown)cEleFocused.obj;
                                    if (nudFocused.GotFocus != null)
                                        nudFocused.GotFocus((object)nudFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - nudFocused.cEle.rec.Left, MouseLocation.Y - nudFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.Panel:
                                {
                                    Panel pnlFocused = (Panel)cEleFocused.obj;
                                    if (pnlFocused.GotFocus != null)
                                        pnlFocused.GotFocus((object)pnlFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - pnlFocused.cEle.rec.Left, MouseLocation.Y - pnlFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.ProgressBar:
                                {
                                    ProgressBar prbFocused = (ProgressBar)cEleFocused.obj;
                                    prbFocused.Highlight = true;
                                    if (prbFocused.GotFocus != null)
                                        prbFocused.GotFocus((object)prbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - prbFocused.cEle.rec.Left, MouseLocation.Y - prbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TrackBar:
                                {
                                    TrackBar trbFocused = (TrackBar)cEleFocused.obj;
                                    trbFocused.Highlight = true;
                                    if (trbFocused.GotFocus != null)
                                        trbFocused.GotFocus((object)trbFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - trbFocused.cEle.rec.Left, MouseLocation.Y - trbFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.TextBox:
                                {
                                    Textbox txtFocused = (Textbox)cEleFocused.obj;
                                    txtFocused.Highlight = true;
                                    if (txtFocused.GotFocus != null)
                                        txtFocused.GotFocus((object)txtFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - txtFocused.cEle.rec.Left, MouseLocation.Y - txtFocused.cEle.rec.Top, 0));

                                }
                                break;
                                

                            case enuSweepPrune_ObjectType.TextDisplay:
                                {
                                    TextDisplay txtFocused = (TextDisplay)cEleFocused.obj;
                                    txtFocused.Highlight = true;
                                    if (txtFocused.GotFocus != null)
                                        txtFocused.GotFocus((object)txtFocused, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, MouseLocation.X - txtFocused.cEle.rec.Left, MouseLocation.Y - txtFocused.cEle.rec.Top, 0));

                                }
                                break;

                            case enuSweepPrune_ObjectType.UserDefined:
                                {
                                    // do nothing

                                }
                                break;

                            default:
                                {
                                    System.Windows.Forms.MessageBox.Show("SPObjects.SPContainer.MouseLocation.set() case default : this should not happen");

                                }
                                break;

                        }
                    }
                }
            }
        }


        EventHandler _MouseDown = null;
        new public EventHandler MouseDown
        {
            get { return _MouseDown; }
            set { _MouseDown = value; }
        }

        EventHandler _SelectionChanged = null;
        public EventHandler SelectionChanged
        {
            get { return _SelectionChanged; }
            set { _SelectionChanged = value; }
        }


        System.Windows.Forms.MouseEventHandler _MouseUp = null;
        new public System.Windows.Forms.MouseEventHandler MouseUp
        {
            get { return _MouseUp; }
            set { _MouseUp = value; }
        }


        System.Windows.Forms.MouseEventHandler _MouseClick = null;
        new public System.Windows.Forms.MouseEventHandler MouseClick
        {
            get { return _MouseClick; }
            set { _MouseClick = value; }
        }


        System.Windows.Forms.MouseEventHandler _MouseDoubleClick = null;
        new public System.Windows.Forms.MouseEventHandler MouseDoubleClick
        {
            get { return _MouseDoubleClick; }
            set { _MouseDoubleClick = value; }
        }


        System.Windows.Forms.MouseEventHandler _MouseMove = null;
        new public System.Windows.Forms.MouseEventHandler MouseMove
        {
            get { return _MouseMove; }
            set { _MouseMove = value; }
        }

        
        System.Windows.Forms.MouseEventHandler _MouseWheel = null;
        new public System.Windows.Forms.MouseEventHandler MouseWheel
        {
            get { return _MouseWheel; }
            set { _MouseWheel = value; }
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

        #endregion 
    }

    public class classScrollBar
    {
        public EventHandler NeedsToBeDrawn = null;
        public EventHandler ValueChanged = null;
        static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID
        {
            get { return intID; }
        }

        public SPContainer SPContainer = null;

        public classScrollBar(ref SPContainer SPContainer)
        {
            this.SPContainer = SPContainer;
        }


        Bitmap _bmpArrow_Base = null;
        Bitmap bmpArrow_Base
        {
            get
            {
                if (_bmpArrow_Base == null)
                {
                    _bmpArrow_Base = new Bitmap(Thickness, 10);
                    using (Graphics g = Graphics.FromImage(_bmpArrow_Base))
                    {
                        g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, _bmpArrow_Base.Width, _bmpArrow_Base.Height));

                        Pen pForeColor = new Pen(ForeColor);

                        g.DrawRectangle(pForeColor, new Rectangle(0, 0, _bmpArrow_Base.Width - 1, _bmpArrow_Base.Height - 1));

                        int intGap = 2;
                        Point ptTopCenter = new Point(_bmpArrow_Base.Width / 2, intGap);
                        Point ptBL = new Point(intGap, _bmpArrow_Base.Height - intGap - 1);
                        Point ptBR = new Point(_bmpArrow_Base.Width - intGap - 1, _bmpArrow_Base.Height - intGap - 1);
                        Point[] ptPolygon = { ptTopCenter, ptBR, ptBL, ptTopCenter };
                        g.DrawPolygon(pForeColor, ptPolygon);
                    }
                }
                return _bmpArrow_Base;
            }
        }

        public Bitmap bmpArrow_Up
        {
            get { return bmpArrow_Base; }
        }

        Bitmap _bmpArrow_Down = null;
        public Bitmap bmpArrow_Down
        {
            get
            {
                if (_bmpArrow_Down == null)
                {
                    _bmpArrow_Down = new Bitmap(bmpArrow_Base);
                    _bmpArrow_Down.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                return _bmpArrow_Down;
            }
        }


        Bitmap _bmpArrow_Left = null;
        public Bitmap bmpArrow_Left
        {
            get
            {
                if (_bmpArrow_Left == null)
                {
                    _bmpArrow_Left = new Bitmap(bmpArrow_Base);
                    _bmpArrow_Left.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                return _bmpArrow_Left;
            }
        }

        Bitmap _bmpArrow_Right = null;
        public Bitmap bmpArrow_Right
        {
            get
            {
                if (_bmpArrow_Right == null)
                {
                    _bmpArrow_Right = new Bitmap(bmpArrow_Base);
                    _bmpArrow_Right.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                return _bmpArrow_Right;
            }
        }


        Rectangle _recArrow_Up = new Rectangle(0, 0, 10, 10);
        public Rectangle recArrow_Up
        {
            get { return _recArrow_Up; }
            set { _recArrow_Up = value; }
        }
        Rectangle _recArrow_Down = new Rectangle(0, 0, 10, 10);
        public Rectangle recArrow_Down
        {
            get { return _recArrow_Down; }
            set { _recArrow_Down = value; }
        }
        Rectangle _recArrow_Left = new Rectangle(0, 0, 10, 10);
        public Rectangle recArrow_Left
        {
            get { return _recArrow_Left; }
            set { _recArrow_Left = value; }
        }
        Rectangle _recArrow_Right = new Rectangle(0, 0, 10, 10);
        public Rectangle recArrow_Right
        {
            get { return _recArrow_Right; }
            set { _recArrow_Right = value; }
        }


        public void Arrow_Reset()
        {
            _bmpArrow_Base = null;
            _bmpArrow_Left = null;
            _bmpArrow_Right = null;
            _bmpArrow_Down = null;
        }


        Color BackColor
        {
            get
            {
                if (SPContainer != null)
                    return SPContainer.BackColor;
                else
                    return Color.White;
            }
        }


        Color ForeColor
        {
            get
            {
                if (SPContainer != null)
                    return SPContainer.ForeColor;
                else
                    return Color.Black;
            }
        }

        bool bolVisible = false;
        public bool Visible
        {
            get { return bolVisible; }
            set
            {
                if (bolVisible != value)
                {
                    bolVisible = value;
                    SPContainer.RecVisible_Changed = true;
                }
            }
        }

        public void ScrollUp() { Value = Value - SmallChange; }

        public void ScrollDown() { Value = Value + SmallChange; }

        public void StepUp() { Value = Value - LargeChange; }

        public void StepDown() { Value = Value + LargeChange; }

        int intValue = 0;
        public int Value
        {
            get { return intValue; }
            set
            {
                if (value < Min)
                    value = Min;
                else if (value > Max)
                    value = Max;

                if (intValue != value)
                {
                    intValue = value;

                    if (ValueChanged != null && !SPContainer.Scrolling_Override)
                        ValueChanged((object)this, new EventArgs());
                    if (NeedsToBeDrawn != null)
                        NeedsToBeDrawn((object)this, new EventArgs());
                }
            }
        }

        int intMin = 0;
        public int Min
        {
            get { return intMin; }
            set
            {
                if (intMin != value)
                    intMin = value;

                if (intMax < intMin)
                    Max = intMin + 1;

            }
        }

        int intMax = 100;
        public int Max
        {
            get { return intMax; }
            set
            {
                intMax = value;
                if (intMin > intMax)
                    Min = Max - 1;
            }
        }

        int intLargeChange = 10;
        public int LargeChange
        {
            get { return intLargeChange; }
            set
            {
                if (intLargeChange != value)
                {
                    intLargeChange = value;
                    if (NeedsToBeDrawn != null)
                        NeedsToBeDrawn((object)this, new EventArgs());
                }
            }
        }


        int intSmallChange = 1;
        public int SmallChange
        {
            get { return intSmallChange; }
            set
            {
                if (intSmallChange != value)
                {
                    intSmallChange = value;

                    if (NeedsToBeDrawn != null)
                        NeedsToBeDrawn((object)this, new EventArgs());
                }
            }
        }

        int intThickness = 12;
        public int Thickness
        {
            get { return intThickness; }
            set
            {
                if (intThickness != value)
                {
                    intThickness = value;
                    Arrow_Reset();
                    if (NeedsToBeDrawn != null)
                        NeedsToBeDrawn((object)this, new EventArgs());
                }
            }
        }

        public Point ptGrab = new Point();
        bool bolGrabbed = false;
        public bool Grabbed
        {
            get { return bolGrabbed; }
            set { bolGrabbed = value; }
        }

        Rectangle _recDraw = new Rectangle();
        public Rectangle recDraw
        {
            get { return _recDraw; }
            set { _recDraw = value; }
        }

        Rectangle _recSliderDraw = new Rectangle();
        public Rectangle recSliderDraw
        {
            get { return _recSliderDraw; }
            set
            {
                _recSliderDraw = value;
                if (_recSliderDraw.Height < Thickness)
                    _recSliderDraw.Height = Thickness;
                if (_recSliderDraw.Width < Thickness)
                    _recSliderDraw.Width = Thickness;
            }
        }
    }



    public class SweepPrune_Object
    {
        public SPContainer SPContainer = null;          // pointer to SPContianer that contains this object  - NEVER NULL
        static int intIDCounter = 0;

        int intID = intIDCounter++;
        public int ID
        {
            get { return intID; }
        }
        public static Size szDefault = new Size(100, 30);
        public static int getCharWidth(char c, Font fnt)
        {
            int intNum = (int)Math.Pow(2, 15);
            string strTest = "".PadRight(intNum, c);
            int intWidth = System.Windows.Forms.TextRenderer.MeasureText(strTest, fnt).Width / intNum;

            return intWidth;
        }

        public static int getStringWidth(string str, Font fnt)
        {
            int intDouble = 15;
            int intNum = (int)Math.Pow(2, intDouble);
            string strTest = str;
            for (int intCounter = 0; intCounter < intDouble; intCounter++)
                strTest += strTest;

            int intWidth = System.Windows.Forms.TextRenderer.MeasureText(strTest, fnt).Width / intNum;

            return intWidth;
        }

        public static int getCharHeight(char c, Font fnt)
        {
            string strLine = c.ToString() + "\r\n";
            string strTest = strLine;
            int intDoubleCount = 12;
            int intNum = (int)Math.Pow(2, intDoubleCount);
            for (int intCounter = 0; intCounter < intDoubleCount; intCounter++)
                strTest += strTest;

            int intHeight = System.Windows.Forms.TextRenderer.MeasureText(strTest, fnt).Height / intNum;
            return intHeight;
        }

        public static int getStringHeight(string str, Font fnt)
        {
            string strLine = str + "\r\n";
            string strTest = strLine;
            int intDoubleCount = 12;
            int intNum = (int)Math.Pow(2, intDoubleCount);
            for (int intCounter = 0; intCounter < intDoubleCount; intCounter++)
                strTest += strTest;

            int intHeight = System.Windows.Forms.TextRenderer.MeasureText(strTest, fnt).Height / intNum;
            return intHeight;
        }

        public classSweepAndPrune_Element cEle = null;
        public object Tag = null;
        public object obj = null;
        public SweepPrune_Object(ref SPContainer SPContainer)// : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
        }

        public System.Windows.Forms.ContextMenu ContextMenu
        {
            get { return cEle.ContextMenu; }
            set { cEle.ContextMenu = value; }
        }
        enuSweepPrune_ObjectType _eSP_ObjectType = enuSweepPrune_ObjectType._num;
        public enuSweepPrune_ObjectType eSP_ObjectType
        {
            get { return _eSP_ObjectType; }
            set { _eSP_ObjectType = value; }
        }

        #region ForeColor

        Color clrForeColor_Dull = Color.Black;
        SolidBrush sbrForeColor_Dull = new SolidBrush(Color.Black);
        public Color ForeColor_Dull
        {
            get { return clrForeColor_Dull; }
            set
            {
                if (ForeColor_Dull != value)
                {
                    sbrForeColor_Dull = new SolidBrush(value);
                    clrForeColor_Dull = value;
                    if (Forecolor_Change != null)
                        Forecolor_Change((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;

                }
            }
        }

        Color clrForeColor_Highlight = Color.White;
        SolidBrush sbrForeColor_Highlight = new SolidBrush(Color.White);
        public Color ForeColor_Highlight
        {
            get { return clrForeColor_Highlight; }
            set
            {
                if (ForeColor_Highlight != value)
                {
                    sbrForeColor_Highlight = new SolidBrush(value);
                    clrForeColor_Highlight = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public SolidBrush sbrForeColor
        {
            get { return sbrForeColor_Dull; }
        }

        public virtual Color ForeColor
        {
            get { return ForeColor_Dull; }
            set
            {
                if (ForeColor != value)
                {
                    sbrForeColor_Dull = new SolidBrush(value);
                    if (eSP_ObjectType == enuSweepPrune_ObjectType.UserDefined)
                        cEle.ForeColor = value;

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.ForeColor = value;
                    }
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        #endregion

        #region BackColor

        bool bolBackTransSPContainer = false;
        public bool BackTransSPContainer
        {
            get { return bolBackTransSPContainer; }
            set
            {
                if (bolBackTransSPContainer != value)
                {
                    bolBackTransSPContainer = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public Color BackColor_Dull
        {
            get { return cEle.BackColor; }
            set
            {
                if (cEle.BackColor != value)
                {
                    cEle.BackColor = value;
                    if (Backcolor_Change != null)
                        Backcolor_Change((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public Color BackColor_Highlight
        {
            get { return cEle.BackColor_Highlight; }
            set
            {
                if (BackColor_Highlight != value)
                {
                    cEle.BackColor_Highlight = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public SolidBrush sbrBackColor
        {
            get { return cEle.sbrBackColor_Dull; }
        }

        public virtual Color BackColor
        {
            get { return BackColor_Dull; }
            set
            {
                if (BackColor != value)
                {
                    BackColor_Dull
                        = value;

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.BackColor = value;
                    }

                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        #endregion

        #region bitmap
        Bitmap _bmpDull = null;
        public Bitmap bmpDull
        {
            get { return _bmpDull; }
            set
            {
                if (_bmpDull != null)
                    _bmpDull.Dispose();
                _bmpDull = value;
            }
        }

        Bitmap _bmpHighlight = null;
        public Bitmap bmpHighlight
        {
            get { return _bmpHighlight; }
            set
            {
                if (_bmpHighlight != null)
                    _bmpHighlight.Dispose();
                _bmpHighlight = value;
            }
        }
        #endregion 

        #region Events
        EventHandler _GotFocus = null;
        public EventHandler GotFocus
        {
            get { return _GotFocus; }
            set { _GotFocus = value; }
        }

        EventHandler _LostFocus = null;
        public EventHandler LostFocus
        {
            get { return _LostFocus; }
            set { _LostFocus = value; }
        }

        EventHandler _Forecolor_Change = null;
        public EventHandler Forecolor_Change
        {
            get { return _Forecolor_Change; }
            set { _Forecolor_Change = value; }
        }

        EventHandler _Backcolor_Change = null;
        public EventHandler Backcolor_Change
        {
            get { return _Backcolor_Change; }
            set { _Backcolor_Change = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseEnter = null;
        public System.Windows.Forms.MouseEventHandler MouseEnter
        {
            get { return _MouseEnter; }
            set
            {
                _MouseEnter = value;
            }
        }

        System.Windows.Forms.MouseEventHandler _MouseLeave = null;
        public System.Windows.Forms.MouseEventHandler MouseLeave
        {
            get { return _MouseLeave; }
            set
            {
                _MouseLeave = value;
            }
        }

        System.Windows.Forms.MouseEventHandler _MouseMove = null;
        public System.Windows.Forms.MouseEventHandler MouseMove
        {
            get { return _MouseMove; }
            set { _MouseMove = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseDown = null;
        public System.Windows.Forms.MouseEventHandler MouseDown
        {
            get { return _MouseDown; }
            set { _MouseDown = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseUp = null;
        public System.Windows.Forms.MouseEventHandler MouseUp
        {
            get { return _MouseUp; }
            set { _MouseUp = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseClick = null;
        public System.Windows.Forms.MouseEventHandler MouseClick
        {
            get { return _MouseClick; }
            set { _MouseClick = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseDoubleClick = null;
        public System.Windows.Forms.MouseEventHandler MouseDoubleClick
        {
            get { return _MouseDoubleClick; }
            set { _MouseDoubleClick = value; }
        }

        System.Windows.Forms.MouseEventHandler _MouseWheel = null;
        public System.Windows.Forms.MouseEventHandler MouseWheel
        {
            get { return _MouseWheel; }
            set { _MouseWheel = value; }
        }

        EventHandler _SizeChanged = null;
        public EventHandler Sizechanged
        {
            get { return _SizeChanged; }
            set { _SizeChanged = value; }
        }

        EventHandler _LocationChanged = null;
        public EventHandler LocationChanged
        {
            get { return _LocationChanged; }
            set { _LocationChanged = value; }
        }
        
        EventHandler _MyImage_Changed = null;
        public EventHandler MyImage_Changed
        {
            get { return _MyImage_Changed; }
            set { _MyImage_Changed = value; }
        }
        #endregion

        #region Methods

        public void BringToFront() { cEle.BringToFront(); }
        public void SendToBack() { cEle.SendToBack(); }
        public void Show() { Visible = true; }
        public void Hide() { Visible = false; }

        #endregion

        #region dimensions

        public Rectangle recSPArea
        {
            get
            {
                return cEle.rec;
            }
        }

        public Rectangle recDraw
        {
            get { return cEle.recDraw; }
        }
        
        public Rectangle recVisible
        {
            get
            {
                Rectangle recRetVal = new Rectangle();

                recRetVal.Location = Location;
                recRetVal.Size = Size;
                return recRetVal;
            }
        }


        public Point Location
        {
            get { return cEle.rec.Location; }
            set
            {
                if (cEle.rec.Left != value.X || cEle.rec.Top != value.Y)
                {
                    cEle.rec = new Rectangle(value, cEle.rec.Size);
                    cEle.NeedsToBeRedrawn = true;
                    if (LocationChanged != null)
                        LocationChanged((object)this, new EventArgs());
                    cEle.SPContainer.Reset();
                }
            }
        }

        public int Left
        {
            get
            {
                return cEle.rec.Left;
            }
            set
            {
                Location = new Point(value, cEle.rec.Top);
            }
        }

        public int Right
        {
            get { return cEle.rec.Right; }
        }

        public int Top
        {
            get { return cEle.rec.Top; }
            set
            {
                Location = new Point(cEle.rec.Left, value);
            }
        }

        public int Bottom
        {
            get { return cEle.rec.Bottom; }
        }

        public virtual int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                if (cEle.rec.Width != value)
                {
                    Size = new Size(value, cEle.rec.Height);
                }
            }
        }

        public virtual int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                if (cEle.rec.Height != value)
                {
                    Size = new Size(cEle.rec.Width, value);
                }
            }
        }

        public virtual Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                if (cEle.rec.Width != value.Width || cEle.rec.Height != value.Height)
                {
                    cEle.rec = new Rectangle(cEle.rec.Location, value);
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    _MyImage = null;
                    cEle.SPContainer.Reset();
                }
            }
        }
        #endregion

        #region Properties


        public bool Visible
        {
            get { return cEle.Visible; }
            set { cEle.Visible = value; }
        }

        bool bolHighlight = false;
        public virtual bool Highlight
        {
            get { return bolHighlight; }
            set
            {
                if (bolHighlight != value)
                {
                    bolHighlight = value;
                    /*
                    switch (eSP_ObjectType)
                    {
                        case enuSweepPrune_ObjectType.Button:
                            cEle.NeedsToBeRedrawn = true;
                            break;
                    }
                    /*/
                    cEle.NeedsToBeRedrawn = true;
                    // */
                }
            }
        }

        bool bolBlendIntoSPContainer = false;
        public bool BlendIntoSPContainer
        {
            get { return bolBlendIntoSPContainer; }
            set
            {
                if (bolBlendIntoSPContainer != value)
                {
                    bolBlendIntoSPContainer = value;
                    cEle.NeedsToBeRedrawn = true;
                    classSweepAndPrune_Element cParentTemp = cEle.Parent;
                    while (cParentTemp != null)
                    {
                        cParentTemp.NeedsToBeRedrawn = true;
                        cParentTemp = cParentTemp.Parent;
                    }
                }
            }
        }

        public string strText = "";
        public virtual string Text
        {
            get { return strText; }
            set
            {
                if (value != null)
                {
                    strText = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public Font _Font = new Font("_", 8);
        public virtual Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public Bitmap _MyImage = null;
        public virtual Bitmap MyImage
        {
            get
            {
                if (_MyImage == null || (cEle != null && cEle.NeedsToBeRedrawn))
                {
                    Draw();
                    if (cEle != null)
                        cEle.ImageRefreshed = true;

                    if (MyImage_Changed != null)
                        MyImage_Changed((object)this, new EventArgs());
                }
                return _MyImage;
            }
            set
            {
                _MyImage = value;
                //if (_MyImage != null && eSP_ObjectType == enuSweepPrune_ObjectType.SlideRuler)
                //    _MyImage.Save(@"c:\debug\myimage.png");
                cEle.ImageRefreshed = true;
                cEle.NeedsToBeRedrawn = false;

                
            }
        }

        Bitmap _bmpBackgroundImage = null;
        public Bitmap BackgroundImage
        {
            get { return _bmpBackgroundImage; }
            set
            {
                if (_bmpBackgroundImage != null)
                    _bmpBackgroundImage.Dispose();
                _bmpBackgroundImage = value;
                MyImage = _bmpBackgroundImage;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public virtual void Draw()
        {
            if (SPContainer.BuildingInProgress) return;
            if (!cEle.NeedsToBeRedrawn && MyImage != null) return;

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Bitmap bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (!BlendIntoSPContainer)
                {
                    if (BackgroundImage != null)
                        g.DrawImage(BackgroundImage, new Point(0, 0));
                    else
                        g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));
                }
            }
            MyImage = bmpTemp;
        }
        #endregion
    }



    public class numericUpDown : SweepPrune_Object
    {
        public numericUpDown(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            numericUpDown cMyRef = this;
            SPContainer.Add(ref cMyRef);

            NumPlaces_Set();
            szChar_Set();
        }

        string strHeading = "";
        public string Heading
        {
            get { return strHeading; }
            set
            {
                strHeading = value;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        bool bolAllowLooping = false;
        public bool AllowLooping
        {
            get { return bolAllowLooping; }
            set { bolAllowLooping = value; }
        }


        public override Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                szChar_Set();
            }
        }

        #region Values
        int intValue = 0;
        public int Value
        {
            get { return intValue; }
            set
            {
                if (value < Min)
                    value = Min;
                else if (value > Max)
                    value = Max;
                if (intValue != value)
                {
                    intValue = value;
                    if (_eventValueChanged != null)
                        _eventValueChanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        EventHandler _eventValueChanged = null;
        public EventHandler ValueChanged
        {
            get { return _eventValueChanged; }
            set { _eventValueChanged = value; }
        }
        int intMin = 0;
        public int Min
        {
            get { return intMin; }
            set
            {
                if (value >= Max)
                    Max = value + 1;
                intMin = value;
                if (Value < Min)
                    Value = Min;
                NumPlaces_Set();
            }
        }

        int intMax = 100;
        public int Max
        {
            get { return intMax; }
            set
            {
                if (value <= Min)
                    Min = value - 1;
                intMax = value;
                if (Value > Max)
                    Value = Max;
                NumPlaces_Set();
            }
        }
        #endregion

        public override string Text
        {
            get { return Value.ToString(); }
        }

        public new void Highlight(Point ptMouse)
        {
            HighlightIndex = IndexFromPoint(ptMouse);
        }

        int intHighlightIndex = -1;
        int HighlightIndex
        {
            get { return intHighlightIndex; }
            set
            {
                if (intHighlightIndex != value)
                {
                    if (value <= intNumPlaces && value >= 0)
                    {
                        intHighlightIndex = value;
                    }
                    else
                        intHighlightIndex = -1;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public void Scroll_Down()
        {
            if (intHighlightIndex < 0) return;

            int intDelta = (int)Math.Pow(10, intHighlightIndex);

            int intValue_New = Value - intDelta;
            if (intValue_New < Min)
                Value = AllowLooping
                                ? Max
                                : Min;
            else
                Value = intValue_New;
        }
        public void Scroll_Up()
        {
            if (intHighlightIndex < 0) return;

            int intDelta = (int)Math.Pow(10, intHighlightIndex);

            int intValue_New = Value + intDelta;
            if (intValue_New > Max)
                Value = AllowLooping
                            ? Min
                            : Max;
            else
                Value = intValue_New;
        }

        #region Draw
        int intRightTab = 5;
        int _intNumPlaces = 0;
        public int intNumPlaces
        {
            get
            {

                return _intNumPlaces;
            }
            set
            {
                _intNumPlaces = value;

            }
        }

        void NumPlaces_Set()
        {
            int intAbsMin = Math.Abs(Min);
            int intAbsMax = Math.Abs(Max);
            int intMax = intAbsMin > intAbsMax
                                ? intAbsMin
                                : intAbsMax;
            string strMax = intMax.ToString();
            intNumPlaces = strMax.Length;// (int)Math.Ceiling(Math.Log(intMax) / Math.Log(10));
        }

        Size _szChar = new Size(10, 10);
        public Size szChar
        {
            get { return _szChar; }
            set { _szChar = value; }
        }

        void szChar_Set()
        {
            int intNum = 4096;
            string strTemp = "".PadRight(intNum, '#');

            szChar = new Size(getCharWidth('#', Font), getCharHeight('#', Font));
        }

        int IndexFromPoint(Point pt)
        {
            int intRetVal = (Width - pt.X - intRightTab) / szChar.Width;
            if (intRetVal >= intNumPlaces || intRetVal < 0)
                intRetVal = -1;
            return intRetVal;
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;
            if (recSPArea.Width < 10 || recSPArea.Height < 10) return;
            Bitmap bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);

            int intPlaceOrder = 0;
            bool bolNegative = Value < 0;
            int intValue = Math.Abs(Value);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height), new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                else
                    g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));

                do
                {
                    int intNumeral = intValue % 10;

                    if (Heading.Length > 0)
                    {
                        Size szHeading = System.Windows.Forms.TextRenderer.MeasureText(Heading, Font);
                        Point ptDrawHeading = new Point(0, (Height - szHeading.Height) / 2);
                        g.DrawString(Heading, Font, sbrForeColor, ptDrawHeading);
                    }

                    Point ptDraw = new Point(Width - (1 + intPlaceOrder) * szChar.Width - intRightTab, (bmpTemp.Height - szChar.Height) / 2);
                    g.FillRectangle(intPlaceOrder == intHighlightIndex
                                                   ? sbrForeColor
                                                   : sbrBackColor,
                                    new RectangleF(ptDraw.X, 0, szChar.Width, bmpTemp.Height));
                    string strNumeral = intNumeral.ToString();
                    Size szNumeral = System.Windows.Forms.TextRenderer.MeasureText(strNumeral, Font);
                    g.DrawString(strNumeral,
                                 Font,
                                 intPlaceOrder == intHighlightIndex
                                                ? sbrBackColor
                                                : sbrForeColor,
                                 ptDraw);

                    intValue /= 10;
                    intPlaceOrder++;
                } while (intValue > 0);

                for (int intPlaceCounter = intPlaceOrder; intPlaceCounter <= intNumPlaces; intPlaceCounter++)
                {
                    if (intPlaceCounter == intHighlightIndex)
                    {
                        Point ptDraw = new Point(Width - (1 + intPlaceCounter) * szChar.Width, (bmpTemp.Height - szChar.Height) / 2);
                        g.FillRectangle(sbrForeColor, new RectangleF(ptDraw.X, 0, szChar.Width, bmpTemp.Height));
                    }
                }

                if (bolNegative)
                {
                    Point ptDraw = new Point(Width - (1 + intPlaceOrder) * szChar.Width - 4, (bmpTemp.Height - szChar.Height) / 2);
                    g.DrawString("-", Font, sbrForeColor, ptDraw);
                }
            }

            MyImage = bmpTemp;
        }

        #endregion 

    }

    public class SlideRuler : SweepPrune_Object
    {
        public List<Tab> lstTabs = new List<Tab>();

        const int conTabwidth_Default = 5;
        int _intTabWidth = conTabwidth_Default;
        public int TabWidth
        {
            get { return _intTabWidth; }
            set 
            {
                _intTabWidth = value;
                _intTabWidthByTwo = _intTabWidth / 2;
                MyImage.Dispose();
                MyImage = null;
            }
        }

        int _intTabWidthByTwo = conTabwidth_Default/2;
        public int TabWidthByTwo 
        {  
            get
            {
                return _intTabWidthByTwo;
            }
        }

        System.EventHandler _TabsChanged = null;
        public System.EventHandler TabsChanged
        {
            get { return _TabsChanged; }
            set { _TabsChanged = value; }
        }


        void eventDraw(object sender, EventArgs e)
        {
            Draw();
            if (MyImage != null)
                SPContainer.DrawImage(MyImage, cEle.recDraw, new Rectangle(0, 0, MyImage.Width, MyImage.Height));
        }

        public override void Draw() { _Draw(false); }
        public void _Draw(bool bolForce)
        {
            if (!bolForce)
                if (cEle == null || !cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) 
                    return;

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

            Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                else
                    g.FillRectangle(new SolidBrush(BackColor), rectDestination);
                Pen pForeColor = new Pen(ForeColor, 2);
                Pen pTab_HighLight = new Pen(Color.Black, 2);
                Pen pTab_Dull = new Pen(Color.Yellow, 2);

                int intTabWidthByTwo = TabWidth / 2;
                int intBorderSize = 2;
                for (int intTabCounter = 0; intTabCounter < lstTabs.Count; intTabCounter++)
                {
                    Tab tabTemp = lstTabs[intTabCounter];

                    Pen p = tabTemp.HighLight
                                    ? pTab_HighLight
                                    : pTab_Dull;
                    Rectangle recDraw = new Rectangle(tabTemp.X - intTabWidthByTwo, intBorderSize, TabWidth, Height - 2 * intBorderSize);
                    g.DrawRectangle(p, recDraw);
                }
                if (DrawBorder)
                    g.DrawRectangle(pForeColor, new Rectangle(0, 0, recSPArea.Width - 1, recSPArea.Height - 1));
            }

            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }

        public class Tab
        {
            
            public void Die()
            {
                slideRuler.lstTabs.Remove(this);
            }

            public SlideRuler slideRuler = null;
            public Tab(ref SlideRuler slideRuler)
            {
                this.slideRuler = slideRuler;
                slideRuler.lstTabs.Add(this);
            }

            bool bolHighLight = false;
            public bool HighLight
            {
                get { return bolHighLight; }
                set 
                { 
                    bolHighLight = value; 
                }
            }



            int _intWidth = 0;
            public int Width
            {
                get { return _intWidth; }
                set 
                {
                    _intWidth = value;
                    if (_intWidth < 1)
                        _intWidth = 1;
                    if (slideRuler.TabsChanged != null)
                    {
                        slideRuler.TabsChanged((object)this, new EventArgs());
                    }

                    SpreadError();
                }
            }

            public void SpreadError()
            {
                _intMyIndex = slideRuler.lstTabs.IndexOf(this);
                intX = (Prev != null ? Prev.X : 0) + Width;
                if (slideRuler.MyImage != null)
                    slideRuler.MyImage.Dispose();
                slideRuler.MyImage = null;
                slideRuler._Draw(true);
                slideRuler.cEle.NeedsToBeRedrawn = true;

                if (Next != null)
                    Next.SpreadError();
            }

            int intX = -1;
            public int X
            {
                get
                {
                    //if (intX < 0)
                    //{
                    //    if (Prev != null)
                    //        intX = Prev.X + Width;
                    //    else
                    //        intX = Width;
                    //}
                    return intX;
                }
            }



            static int intCounter = 0;
            string strName = "NewTab_" + (intCounter++).ToString();
            public string Name
            {
                get { return strName; }
                set { strName = value; }
            }

            int _intMyIndex = -1;
            int MyIndex
            {
                get 
                {
                    if (_intMyIndex < 0)
                        _intMyIndex = slideRuler.lstTabs.IndexOf(this);
                    return _intMyIndex; 
                } 
            }

            public Tab Prev
            {
                get 
                {
                    int intMyIndex = MyIndex;
                    if (intMyIndex > 0 && intMyIndex < slideRuler.lstTabs.Count)
                        return slideRuler.lstTabs[intMyIndex - 1];
                    return null; 
                }
            }

            public Tab Next
            {
                get
                {
                    int intMyIndex = MyIndex;
                    if (intMyIndex >= 0 && intMyIndex < slideRuler.lstTabs.Count -1)
                        return slideRuler.lstTabs[intMyIndex + 1];
                    return null;
                }
            }
        }

        static int intSliderCounter = 0;

        int _intSlideRuler_ID = intSliderCounter++;
        public int SlideRuler_ID { get { return _intSlideRuler_ID; } }

        public SlideRuler(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            SlideRuler cMyRef = this;
            SPContainer.Add(ref cMyRef);

            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.SlideRuler;
            cEle.obj = (object)this;
            cEle.Name = "SlideRuler";
            cEle.eventDraw = eventDraw;

            MouseDown = _MouseDown;
            MouseUp = _MouseUp;
            MouseLeave = _MouseLeave;
            MouseMove = _MouseMove;

            BackColor = Color.White;
            ForeColor = Color.Black;

        }

        Point ptLastReset = new Point();

        Point _ptMouseLocation = new Point();
        Point ptMouseLocation
        {
            get { return _ptMouseLocation; }
            set
            {
                _ptMouseLocation = value;

                int intTolerance = 5;
                bool bolShift = Math.Abs(ptLastReset.X - value.X) + Math.Abs(ptLastReset.Y - value.Y) > intTolerance;
                if (bolShift)
                {
                    ptLastReset = _ptMouseLocation;

                    if (tabGrab != null)
                    {
                        tabGrab.Width += (ptMouseLocation.X - tabGrab.X);

                        if (TabsChanged != null)
                            TabsChanged((object)_tabSliderUnderMouse, new EventArgs());
                        
                        _Draw(true);
                        cEle.NeedsToBeRedrawn = true;
                        return;
                    }
                }


                if (tabGrab != null)
                {
                    tabGrab.Width += (ptMouseLocation.X - tabGrab.X);

                    if (TabsChanged != null)
                        TabsChanged((object)_tabSliderUnderMouse, new EventArgs());
                    _Draw(true);
                    cEle.NeedsToBeRedrawn = true;
                    return;
                }

                
                if (lstTabs.Count > 0)
                {
                    int intNumTabs = lstTabs.Count;
                    int intStep = intNumTabs;
                    int intDir = 1;
                    int intTest = 0;
                    int intErrorCounter = 0, intErrorMax = 8;
                    do
                    {
                        intStep /= 2;
                        if (intStep < 1)
                        {
                            intErrorCounter++;
                            intStep = 1;
                        }

                        intTest += Math.Sign(intDir) * intStep;
                        if (intTest < 0)
                        {
                            intTest = 0;
                        }
                        else if (intTest >= lstTabs.Count)
                        {
                            intTest = lstTabs.Count - 1;
                        }
                        intDir = ptMouseLocation.X - lstTabs[intTest].X;
                        if (intDir < 0)
                            tabUnderMouse = lstTabs[intTest];
                        else
                            tabUnderMouse = intTest < lstTabs.Count-1 ? lstTabs[intTest+1] : lstTabs[lstTabs.Count-1];

                        if (Math.Abs(intDir) < TabWidthByTwo)
                        {
                            tabSliderUnderMouse = lstTabs[intTest];
                            return;
                        }
                    } while (intErrorCounter < intErrorMax);

                    tabSliderUnderMouse = null;
                }
            }
        }


        void _MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _MouseRelease(sender, e);
        }

        void _MouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //_MouseRelease(sender, e);
        }

        void _MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ptMouseLocation = new Point(e.X, e.Y);
        }

        void _MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            tabGrab = tabSliderUnderMouse;
        }
        
        void _MouseRelease(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            tabGrab = null;
            tabSliderUnderMouse = null;
        }

        Tab _tabGrab = null;
        Tab tabGrab
        {
            get { return _tabGrab; }
            set
            {
                if (_tabGrab != value)
                {
                    _tabGrab = value;
                }
            }
        }
        
        Tab _tabSliderUnderMouse = null;
        Tab tabSliderUnderMouse
        {
            get { return _tabSliderUnderMouse; }
            set
            {
                if (_tabSliderUnderMouse != value)
                {
                    if (_tabSliderUnderMouse != null)
                        _tabSliderUnderMouse.HighLight = false;

                    _tabSliderUnderMouse = value;

                    if (_tabSliderUnderMouse != null)
                        _tabSliderUnderMouse.HighLight = true;

                    _Draw(true);
                    cEle.NeedsToBeRedrawn = true;
                    
                }
            }
        }
        Tab _tabUnderMouse = null;
        public Tab tabUnderMouse
        {
            get { return _tabUnderMouse; }
            set
            {
                if (_tabUnderMouse != value)
                {
                    _tabUnderMouse = value;                    
                }
            }
        }
        


        //bool bolCanBeToggled = false;
        //public bool CanBeToggled
        //{
        //    get { return bolCanBeToggled; }
        //    set { bolCanBeToggled = value; }
        //}

        //bool bolToggled = false;
        //public bool Toggled
        //{
        //    get { return bolToggled; }
        //    set
        //    {
        //        if (bolToggled != value)
        //        {
        //            bolToggled = value;
        //            cEle.NeedsToBeRedrawn = true;
        //        }
        //    }
        //}


        bool bolDrawBorder = false;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        //public void Toggle_On() { Toggled = true; }
        //public void Toggle_Off() { Toggled = false; }
        //public void Toggle()
        //{
        //    Toggled = !Toggled;
        //}


        bool bolAutoSize = false;
        public bool AutoSize
        {
            get { return bolAutoSize; }
            set
            {
                if (bolAutoSize != value)
                {
                    bolAutoSize = value;
                    Autosize_SetSize();
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, new Size(value, cEle.rec.Height));

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, new Size(cEle.rec.Width, value));

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }

        System.Drawing.ContentAlignment _textAlignment = ContentAlignment.TopLeft;
        public System.Drawing.ContentAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, value);

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }
        void Autosize_SetSize()
        {
            if (AutoSize)
                cEle.rec = new Rectangle(cEle.rec.Location, Autosize_GetSize());
        }

        Size Autosize_GetSize()
        {
            Size szNew = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
            return new Size(szNew.Width + 5, szNew.Height + 2);
        }

        public override string Text
        {
            get { return strText; }
            set
            {
                strText = value;
                Autosize_SetSize();
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public override Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                Autosize_SetSize();

                cEle.NeedsToBeRedrawn = true;
            }
        }


        Point AlignText()
        {
            Size szText = Autosize_GetSize();

            int intX = 0;
            switch (TextAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    intX = 1;
                    break;

                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    intX = (Width - szText.Width) / 2;
                    break;

                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    intX = Width - szText.Width - 1;
                    break;
            }


            int intY = 0;
            switch (TextAlignment)
            {
                case ContentAlignment.MiddleRight:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                    intY = (Height - szText.Height) / 2;
                    break;
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    intY = 1;
                    break;

                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    intY = (Height - szText.Height - 1);
                    break;
            }

            return new Point(intX, intY);
        }

        //public override void Draw()
        //{
        //    if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

        //    if (bolAutoSize)
        //    {
        //        Autosize_SetSize();
        //    }

        //    if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
        //    Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

        //    Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);

        //    using (Graphics g = Graphics.FromImage(bmpTemp))
        //    {
        //        if ((CanBeToggled && Toggled)
        //            || Highlight)
        //        {
        //            if (BlendIntoSPContainer)
        //            { }
        //            else if (bmpHighlight != null && !BlendIntoSPContainer)
        //            {
        //                g.DrawImage(bmpHighlight, rectDestination, new Rectangle(0, 0, bmpHighlight.Width, bmpHighlight.Height), GraphicsUnit.Pixel);
        //            }
        //            else if (BackgroundImage != null)
        //            {
        //                g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
        //            }
        //            else
        //            {
        //                if (!BlendIntoSPContainer)
        //                    g.FillRectangle(new SolidBrush(BackColor_Highlight), rectDestination);
        //            }
        //            g.DrawString(Text, Font, new SolidBrush(ForeColor_Highlight), AlignText());

        //            if (DrawBorder)
        //                g.DrawRectangle(new Pen(BackColor), new Rectangle(0, 0, Width - 1, Height - 1));
        //        }
        //        else
        //        {
        //            if (BlendIntoSPContainer)
        //            { }
        //            else if (bmpDull != null)
        //            {
        //                g.DrawImage(bmpDull, rectDestination, new Rectangle(0, 0, bmpDull.Width, bmpDull.Height), GraphicsUnit.Pixel);
        //            }
        //            else if (BackgroundImage != null)
        //            {
        //                g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
        //            }
        //            else
        //            {
        //                if (!BlendIntoSPContainer)
        //                    g.FillRectangle(new SolidBrush(BackColor_Dull), rectDestination);
        //            }
        //            g.DrawString(Text, Font, new SolidBrush(ForeColor_Dull), AlignText());

        //            if (DrawBorder)
        //                g.DrawRectangle(new Pen(ForeColor), new Rectangle(0, 0, Width - 1, Height - 1));
        //        }
        //    }
        //    MyImage = bmpTemp;
        //}
    }

    public class Button : SweepPrune_Object
    {
        public Button(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            Button cMyRef = this;
            SPContainer.Add(ref cMyRef);

            BackColor = Color.White;
            ForeColor = Color.Black;
        }

        bool bolCanBeToggled = false;
        public bool CanBeToggled
        {
            get { return bolCanBeToggled; }
            set { bolCanBeToggled = value; }
        }

        bool bolToggled = false;
        public bool Toggled
        {
            get { return bolToggled; }
            set
            {
                if (bolToggled != value)
                {
                    bolToggled = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        bool bolDrawBorder = false;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public void Toggle_On() { Toggled = true; }
        public void Toggle_Off() { Toggled = false; }
        public void Toggle() 
        { 
            Toggled = !Toggled;
        }

        System.Drawing.Color _clrBorderColor = System.Drawing.Color.Black;
        public System.Drawing.Color BorderColor
        {
            get { return _clrBorderColor; }
            set { _clrBorderColor = value; }
        }


        bool bolAutoSize = false;
        public bool AutoSize
        {
            get { return bolAutoSize; }
            set
            {
                if (bolAutoSize != value)
                {
                    bolAutoSize = value;
                    Autosize_SetSize();
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, new Size(value, cEle.rec.Height));

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, new Size(cEle.rec.Width, value));

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }

        System.Drawing.ContentAlignment _textAlignment = ContentAlignment.TopLeft;
        public System.Drawing.ContentAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                Size szStart = Size;
                if (AutoSize)
                    Autosize_SetSize();
                else
                    cEle.rec = new Rectangle(cEle.rec.Location, value);

                if (szStart.Width != Size.Width || szStart.Height != Size.Height)
                {
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }
        void Autosize_SetSize()
        {
            if (AutoSize)
                cEle.rec = new Rectangle(cEle.rec.Location, Autosize_GetSize());
        }

        Size Autosize_GetSize()
        {
            Size szNew = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
            return new Size(szNew.Width + 5, szNew.Height + 2);
        }

        public override string Text
        {
            get { return strText; }
            set
            {
                strText = value;
                Autosize_SetSize();
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public override Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                Autosize_SetSize();

                cEle.NeedsToBeRedrawn = true;
            }
        }


        Point AlignText()
        {
            Size szText = Autosize_GetSize();

            int intX = 0;
            switch (TextAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    intX = 1;
                    break;

                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    intX = (Width - szText.Width) / 2;
                    break;

                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    intX = Width - szText.Width - 1;
                    break;
            }


            int intY = 0;
            switch (TextAlignment)
            {
                case ContentAlignment.MiddleRight:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                    intY = (Height - szText.Height) / 2;
                    break;
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    intY = 1;
                    break;

                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    intY = (Height - szText.Height - 1);
                    break;
            }

            return new Point(intX, intY);
        }

        int _intBorderWidth = 1;
        public int BorderWidth
        {
            get { return _intBorderWidth; }
            set
            {
                if (value < 0) value = 1;
                int intMax = (Width < Height ? Width : Height) / 2;
                if (intMax < 1) intMax = 1;
                if (value > intMax)
                    value = intMax;
                if (_intBorderWidth != value)
                {
                    _intBorderWidth = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public override void Draw() { Draw(false); }
        public void Draw(bool bolForceDraw)
        {
            if (!bolForceDraw)
            {
                if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) 
                    return;
            }

            if (bolAutoSize)
            {
                Autosize_SetSize();
            }

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

            Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if ((CanBeToggled && Toggled)
                    || Highlight)
                {
                    if (BlendIntoSPContainer)
                    { }
                    else if (bmpHighlight != null && !BlendIntoSPContainer)
                    {
                        g.DrawImage(bmpHighlight, rectDestination, new Rectangle(0, 0, bmpHighlight.Width, bmpHighlight.Height), GraphicsUnit.Pixel);
                    }
                    else if (BackgroundImage != null)
                    {
                        g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        if (!BlendIntoSPContainer)
                            g.FillRectangle(new SolidBrush(BackColor_Highlight), rectDestination);
                    }
                    g.DrawString(Text, Font, new SolidBrush(ForeColor_Highlight), AlignText());

                    if (DrawBorder)
                    {
                        Pen pBorder = new Pen(BorderColor, BorderWidth);
                        g.DrawRectangle(pBorder, new Rectangle(0, 0, Width - 1, Height - 1));
                    }
                }
                else
                {
                    if (BlendIntoSPContainer)
                    { }
                    else if (bmpDull != null)
                    {
                        g.DrawImage(bmpDull, rectDestination, new Rectangle(0, 0, bmpDull.Width, bmpDull.Height), GraphicsUnit.Pixel);
                    }
                    else if (BackgroundImage != null)
                    {
                        g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        if (!BlendIntoSPContainer)
                            g.FillRectangle(new SolidBrush(BackColor_Dull), rectDestination);
                    }
                    g.DrawString(Text, Font, new SolidBrush(ForeColor_Dull), AlignText());

                    if (DrawBorder)
                    {
                        Pen pBorder = new Pen(BorderColor, BorderWidth);
                        g.DrawRectangle(pBorder, new Rectangle(0, 0, Width - 1, Height - 1));
                    }
                }
            }
            MyImage = bmpTemp;
            //if (bmpTemp != null)
            //    bmpTemp.Save(@"c:\debug\bmpTemp.png");
        }
    }

    public class ProgressBar : Button
    {
        public ProgressBar(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.ProgressBar;
            cEle.obj = (object)this;
            cEle.Name = "ProgressBar";

            BackColor = Color.LightGray;
            ForeColor = Color.LimeGreen;
        }

        int intMin = 0;
        public int Min
        {
            get { return intMin; }
            set
            {
                if (intMin != value)
                {
                    intMin = value;
                    if (intMax <= intMin)
                        Max = Min + 1;
                    if (Value < Min)
                        Value = Min;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        int intMax = 0;
        public int Max
        {
            get { return intMax; }
            set
            {
                if (intMax != value)
                {
                    intMax = value;
                    if (Min >= Max)
                        Min = Max - 1;
                    if (Value > Max)
                        Value = Max;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        int intValue = 0;
        public int Value
        {
            get { return intValue; }
            set
            {
                if (intValue != value)
                {
                    intValue = value;
                    if (intValue < Min)
                        Value = Min;
                    else if (Value > Max)
                        Value = Max;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            double dblRange = (double)(Max - Min);
            double dblValueShifted = (double)(Value - Min);
            double dblPC_Done = dblValueShifted / dblRange;
            int intBarWidth = (int)(Width * dblPC_Done);

            Size szText = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
            Point ptText = new Point(0, (recSPArea.Height - szText.Height) / 2);
            Color clrTransparent = Color.FromArgb(1, 2, 3);

            Bitmap bmpBar = null;

            if (intBarWidth > 0)
            {
                bmpBar = new Bitmap(intBarWidth, recSPArea.Height);


                using (Graphics g = Graphics.FromImage(bmpBar))
                {
                    g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, bmpBar.Width, bmpBar.Height));

                    g.FillRectangle(sbrForeColor, new RectangleF(0, 0, intBarWidth, Height));

                    g.DrawString(Text, Font, Brushes.Black, ptText);
                }
                bmpBar.MakeTransparent(clrTransparent);
            }

            Bitmap bmpOutput = new Bitmap(recSPArea.Width, recSPArea.Height);
            using (Graphics g = Graphics.FromImage(bmpOutput))
            {
                g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpOutput.Width, bmpOutput.Height));

                g.DrawString(Text, Font, sbrForeColor, ptText);
                if (bmpBar != null)
                    g.DrawImage(bmpBar, new PointF(0, 0));

                //                                              draw child elements
                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }
            }

            if (BackTransSPContainer)
                bmpOutput.MakeTransparent(BackColor);
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpOutput;
        }
    }
    public class Textbox : SweepPrune_Object
    {
        public Semaphore semDrawSelected = new Semaphore(1, 1);

        public Textbox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            Textbox cMyRef = this;
            SPContainer.Add(ref cMyRef);

            cFontImages = new classFont(ref cMyRef);
        }

        bool bolDrawCursor = false;
        public bool CursorNeedsToBeDrawn
        {
            get { return bolDrawCursor; }
            set
            {
                if (bolDrawCursor != value)
                {
                    bolDrawCursor = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public class classChar
        {
            public classChar(ref SPObjects.Textbox txt)
            {
                this.txt = txt;
            }

            SPObjects.Textbox txt = null;
            public int intIndex
            {
                get { return txt.lstChar.IndexOf(this); }
            }

            Rectangle _recDraw = new Rectangle(0, 0, 10, 10);
            public Rectangle recDraw
            {
                get { return _recDraw; }
                set { _recDraw = value; }
            }

            char _chrchar = ' ';
            public char chr
            {
                get { return _chrchar; }
                set { _chrchar = value; }
            }

            bool bolSelected = false;
            public bool Selected
            {
                get { return bolSelected; }
                set { bolSelected = value; }
            }

            public System.Drawing.Bitmap bmp
            {
                get
                {
                    return txt.cFontImages.get(chr, Selected ? classFont.enuSelected.Selected : classFont.enuSelected.NotSelected);
                }
            }
        }


        List<classChar> lstChar = new List<classChar>();


        public Point GetPositionFromCharIndex(int index)
        {
            if (index >= 0 && index < lstChar.Count)
                return lstChar[index].recDraw.Location;
            return new Point();
        }

        public int getIndexAtPoint(Point pt)
        {
            classChar cCharAtPoint = getCharAtPoint(pt);

            int intRetVal = cCharAtPoint != null
                                         ? lstChar.IndexOf(cCharAtPoint)
                                         : lstChar.Count;
            if (intRetVal < 0)
                intRetVal = 0;
            return intRetVal;
        }

        public classChar getCharAtPoint(Point pt)
        {
            if (lstChar.Count > 0 && pt.X > lstChar[lstChar.Count - 1].recDraw.Right)
                return null;// lstChar[lstChar.Count -1];

            int intStep = lstChar.Count / 2;
            int intIndex = intStep;
            int intDir = 1;

            while (intIndex >= 0 && intIndex < lstChar.Count)
            {
                classChar cChar = lstChar[intIndex];
                if (cChar.recDraw.Left <= pt.X && cChar.recDraw.Right >= pt.X)
                    return cChar;

                if (cChar.recDraw.Left < pt.X)
                    intDir = 1;
                else
                    intDir = -1;
                intStep /= 2;
                if (intStep < 1)
                    intStep = 1;
                intIndex += intStep * intDir;
            }
            return null;
        }


        public classFont cFontImages = null;

        public override Color ForeColor
        {
            get { return ForeColor_Dull; }
            set
            {
                if (ForeColor != value)
                {
                    ForeColor_Dull = value;// new SolidBrush(value);
                    AppearanceReset();
                    if (eSP_ObjectType == enuSweepPrune_ObjectType.UserDefined)
                        cEle.ForeColor = value;

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.ForeColor = value;
                    }
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        public override Color BackColor
        {
            get { return BackColor_Dull; }
            set
            {
                if (BackColor != value)
                {
                    BackColor_Dull = value;
                    AppearanceReset();

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.BackColor = value;
                    }
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        public override Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                AppearanceReset();
            }
        }

        void AppearanceReset()
        {
            cFontImages.Reset();
            lstChar.Clear();
            MyImage = null;
            cEle.NeedsToBeRedrawn = true;
            Height = System.Windows.Forms.TextRenderer.MeasureText("H", _Font).Height;
            SPContainer.RecVisible_Changed = true;
        }

        bool bolCursorState = false;
        public bool CursorState
        {
            get { return bolCursorState; }
            set { bolCursorState = value; }
        }

        int _intCursorIndex = 0;
        int intCursorIndex
        {
            get { return _intCursorIndex; }
            set
            {
                if (value < 0)
                    _intCursorIndex = 0;
                else if (value >= Text.Length)
                    _intCursorIndex = Text.Length - 1;
                else
                    _intCursorIndex = value;

                if (SelectionLength == 0)
                    SelectionStart = intCursorIndex;
            }
        }

        int intSelectionStart = 0;
        public int SelectionStart
        {
            get { return intSelectionStart; }
            set
            {
                if (intSelectionStart != value)
                {
                    if (value < 0)
                        value = 0;

                    intSelectionStart
                    = SPContainer.txtFocus.SelectionStart
                    = value;

                    if (SelectionLength > 0)
                    {
                        Draw();
                        SPContainer.NeedsToBeRedrawn = true;
                    }

                }
            }
        }

        int intSelectionLength = 0;
        public int SelectionLength
        {
            get { return intSelectionLength; }
            set
            {
                if (intSelectionLength != value)
                {
                    intSelectionLength
                        = SPContainer.txtFocus.SelectionLength
                        = value;

                    Draw();
                    SPContainer.NeedsToBeRedrawn = true;
                }
            }
        }

        public string SelectedText
        {
            get
            {
                if (SelectionLength > 0)
                {
                    if (SelectionStart + SelectionLength < Text.Length)
                        return Text.Substring(SelectionStart, SelectionLength);
                    else if (SelectionStart < Text.Length)
                        return Text.Substring(SelectionStart);
                }
                return "";
            }
        }


        int intIndexMouseDown = 0;
        public int indexMouseDown
        {
            get { return intIndexMouseDown; }
            set { intIndexMouseDown = value; }
        }
        //override string strText = "";
        public override string Text
        {
            get
            {
                return strText;
            }
            set
            {
                if (value != null)
                {
                    if (string.Compare(strText, value) != 0)
                    {
                        strText = value;
                        cEle.NeedsToBeRedrawn = true;
                        lstChar.Clear();
                        if (string.Compare(strText, SPContainer.txtFocus.Text) != 0)
                        {
                            SPContainer.txtFocus.Text = strText;
                            SPContainer.txtFocus.SelectionStart = SelectionStart;
                            SPContainer.txtFocus.SelectionLength = SelectionLength;
                        }
                        MyImage = null;
                    }
                }
            }
        }

        #region Events
        System.Windows.Forms.KeyEventHandler _KeyDown = null;
        public System.Windows.Forms.KeyEventHandler KeyDown
        {
            get { return _KeyDown; }
            set { _KeyDown = value; }
        }

        System.Windows.Forms.KeyEventHandler _KeyUp = null;
        public System.Windows.Forms.KeyEventHandler KeyUp
        {
            get { return _KeyUp; }
            set { _KeyUp = value; }
        }

        System.Windows.Forms.KeyPressEventHandler _KeyPress = null;
        public System.Windows.Forms.KeyPressEventHandler KeyPress
        {
            get { return _KeyPress; }
            set { _KeyPress = value; }
        }

        EventHandler _TextChanged = null;
        public EventHandler TextChanged
        {
            get { return _TextChanged; }
            set { _TextChanged = value; }
        }

        #endregion


        int intDebugCounter = 0;
        public override void Draw()
        {
            intDebugCounter++;
            System.Windows.Forms.TextBox txtTemp = SPContainer.txtFocus;
            txtTemp.Font = Font;

            int intSelStart = txtTemp.SelectionStart;
            int intSelLength = txtTemp.SelectionLength;

            Point ptDraw = new Point(2, 0);
            Size szMin = new Size(10, 10);
            for (int intCharCounter = 0; intCharCounter < Text.Length; intCharCounter++)
            {
                char cTest = Text[intCharCounter];
                if (lstChar.Count <= intCharCounter)
                {
                    SPObjects.Textbox cMyRef = this;
                    classChar cNew = new classChar(ref cMyRef);
                    cNew.chr = cTest;
                    lstChar.Add(cNew);
                }
                classChar cChar = lstChar[intCharCounter];
                cChar.Selected = intCharCounter >= intSelStart
                                    &&
                                 intCharCounter < intSelStart + intSelLength;
                int intX = ptDraw.X;
                Bitmap bmpChar = cChar.bmp;
                if (bmpChar != null)
                {
                    Size sz = cChar.bmp.Size;
                    szMin.Height = sz.Height;

                    ptDraw.X += sz.Width;

                    if (szMin.Width < ptDraw.X)
                        szMin.Width = ptDraw.X;

                    cChar.recDraw = new Rectangle(intX, 0, sz.Width, sz.Height);
                }
            }

            if (szMin.Width < Width)
                szMin.Width = Width;
            if (szMin.Height < Height)
                szMin.Height = Height;
            Bitmap bmp = new Bitmap(szMin.Width, szMin.Height);
            Graphics g = Graphics.FromImage(bmp);
            {
                g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmp.Width, bmp.Height));
                for (int intCharCounter = 0; intCharCounter < lstChar.Count; intCharCounter++)
                {
                    classChar cChar = lstChar[intCharCounter];
                    Bitmap bmpChar = cChar.bmp;
                    if (bmpChar != null)
                        g.DrawImage(bmpChar, cChar.recDraw.Location);
                }

                if (SelectionLength == 0)
                    DrawCursor(ref g);

                g.DrawRectangle(new Pen(ForeColor), new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
            }
            MyImage = bmp;
        }

        public virtual void DrawCursor()
        {
            Bitmap bmpTemp = new Bitmap(MyImage);
            Graphics g = Graphics.FromImage(bmpTemp);
            DrawCursor(ref g);
            MyImage = bmpTemp;
            //cEle.NeedsToBeRedrawn = true;
        }

        Point ptCursor0_OLD, ptCursor1_OLD;

        public virtual void DrawCursor(ref Graphics g)
        {
            System.Windows.Forms.TextBox txtTemp = SPContainer.txtFocus;
            txtTemp.Font = Font;
            if (txtTemp.SelectionLength == 0)
            {
                int intCursorIndex = SelectionStart;
                if (intCursorIndex >= 0 && intCursorIndex < lstChar.Count)
                {
                    classChar cChar = lstChar[intCursorIndex];
                    Bitmap bmpChar = cChar.bmp;
                    if (bmpChar != null)
                    {
                        Point ptCursor0_New = new Point(cChar.recDraw.X, 0);
                        if (intCursorIndex == 0)
                            ptCursor0_New.X++;
                        Point ptCursor1_New = new Point(ptCursor0_New.X, ptCursor0_New.Y + Font.Height);

                        if (ptCursor0_New.X != ptCursor0_OLD.X || ptCursor0_New.Y != ptCursor0_OLD.Y)
                            g.DrawLine(new Pen(BackColor), ptCursor0_OLD, ptCursor1_OLD);

                        g.DrawLine(new Pen(bolCursorState ? BackColor : ForeColor), ptCursor0_New, ptCursor1_New);

                        ptCursor0_OLD = ptCursor0_New;
                        ptCursor1_OLD = ptCursor1_New;
                    }
                }
                else if (intCursorIndex == lstChar.Count && lstChar.Count > 0)
                {
                    classChar cChar = lstChar[lstChar.Count - 1];
                    Bitmap bmpChar = cChar.bmp;
                    if (bmpChar != null)
                    {
                        Point ptCursor0_New = new Point(cChar.recDraw.Right, 0);
                        if (intCursorIndex == 0)
                            ptCursor0_New.X++;
                        Point ptCursor1_New = new Point(ptCursor0_New.X, ptCursor0_New.Y + Font.Height);

                        if (ptCursor0_New.X != ptCursor0_OLD.X || ptCursor0_New.Y != ptCursor0_OLD.Y)
                            g.DrawLine(new Pen(BackColor), ptCursor0_OLD, ptCursor1_OLD);

                        g.DrawLine(new Pen(bolCursorState ? BackColor : ForeColor), ptCursor0_New, ptCursor1_New);

                        ptCursor0_OLD = ptCursor0_New;
                        ptCursor1_OLD = ptCursor1_New;
                    }
                }
            }
        }

        public class classFont
        {
            public SPObjects.Textbox txt = null;
            static public Semaphore semFontFileAccess = new Semaphore(1, 1);
            static public Semaphore semIndexFileAccess = new Semaphore(1, 1);

            public enum enuSelected { NotSelected, Selected, _num };

            Bitmap[,] bmp = new Bitmap[2, 256];

            public classFont(ref SPObjects.Textbox txt)
            {
                this.txt = txt;
            }

            public Font fntStandard
            {
                get { return txt.Font; }
            }

            public void Reset()
            {
                bmp = new Bitmap[2, 256];
            }

            SolidBrush _sBrFore = new SolidBrush(Color.Black);
            SolidBrush sBrFore
            {
                get
                {
                    if (_sBrFore.Color != txt.ForeColor)
                        _sBrFore = new SolidBrush(txt.ForeColor);
                    return _sBrFore;
                }
            }

            SolidBrush _sBrBack = new SolidBrush(Color.Black);
            SolidBrush sBrBack
            {
                get
                {
                    if (_sBrBack.Color != txt.BackColor)
                        _sBrBack = new SolidBrush(txt.BackColor);
                    return _sBrBack;
                }
            }


            #region Font_Create
            static Semaphore semFontCreate = new Semaphore(1, 1);
            static Font fntCreate = new Font("Arial", 7);

            const string SPFontFile = "SPFont.file";
            static string FontFile_Filename
            {
                get
                {
                    return strWorkingDirectory + "\\" + SPFontFile;
                }
            }

            const string SPFontIndex = "SPFont.index";
            static string FontIndex_Filename
            {
                get
                {
                    return strWorkingDirectory + "\\" + SPFontIndex;
                }
            }

            static string _strWorkingDirectory = "";
            static string strWorkingDirectory
            {
                get
                {
                    if (_strWorkingDirectory.Length == 0)
                    {
                        int intDepth = 0;
                        List<string> lstAttempted = new List<string>();
                        List<string> lstUntried = new List<string>();
                        lstUntried.Add(System.IO.Directory.GetCurrentDirectory());
                        while (lstUntried.Count > 0)
                        {
                            string strPath = lstUntried[0];
                            lstAttempted.Add(strPath);
                            lstUntried.RemoveAt(0);

                            List<string> lstFiles = new List<string>();
                            try
                            {
                                lstFiles = System.IO.Directory.GetFiles(strPath, SPFontIndex).ToList<string>();
                            }
                            catch (Exception)
                            {

                            }
                            if (lstFiles.Count > 0)
                            {
                                _strWorkingDirectory = strPath;
                                lstUntried.Clear();
                            }
                            else
                            {
                                List<string> lstSubDir = new List<string>();
                                try
                                {
                                    lstSubDir = System.IO.Directory.GetDirectories(strPath).ToList<string>();
                                }
                                catch (Exception)
                                {
                                }

                                foreach (string s in lstSubDir)
                                {
                                    if (!lstUntried.Contains(s) && !lstAttempted.Contains(s))
                                        lstUntried.Add(s);
                                }

                                if (intDepth < 2)
                                {
                                    int intBackslash = strPath.LastIndexOf('\\');
                                    if (intBackslash > 0)
                                    {
                                        string strParentPath = strPath.Substring(0, intBackslash);

                                        if (!lstUntried.Contains(strParentPath) && !lstAttempted.Contains(strParentPath))
                                        {
                                            lstUntried.Add(strParentPath);
                                            intDepth++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (_strWorkingDirectory.Length == 0)
                        return System.IO.Directory.GetCurrentDirectory();
                    return _strWorkingDirectory;
                }
            }



            static FontStyle[] fntStyleOrder = { FontStyle.Bold, FontStyle.Italic, FontStyle.Strikeout, FontStyle.Underline };

            public static BinaryFormatter formatter = new BinaryFormatter();
            static System.IO.FileStream fsFile = null;
            static System.IO.FileStream fsIndex = null;

            static public void Font_Create(Font fnt)
            {
                if (fsIndex == null)
                {
                    if (System.IO.File.Exists(FontIndex_Filename))
                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Open);
                    else

                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Create);
                }

                if (fsFile == null)
                {
                    if (System.IO.File.Exists(FontFile_Filename))
                        fsFile = new System.IO.FileStream(FontFile_Filename, System.IO.FileMode.Open);
                    else
                        fsFile = new System.IO.FileStream(FontFile_Filename, System.IO.FileMode.Create);
                }

                string strSearchKey = fnt.FontFamily.Name.ToUpper() + fnt.Size.ToString();

                classIndex_Record cIRec = new classIndex_Record();

                fsIndex.Position = 0;
                if (fsIndex.Length == 0)
                {

                    cIRec.Key = strSearchKey;
                    cIRec.Left
                        = cIRec.Right
                        = -1;
                    cIRec.Index = fsFile.Length;

                    Index_Write(ref cIRec, fsIndex.Length);
                }
                else
                {
                    while (fsIndex.Length > fsIndex.Position)
                    {
                        long lngPositionStartRead = fsIndex.Position;

                        cIRec = Index_Read(lngPositionStartRead);

                        int intComparison = string.Compare(cIRec.Key, strSearchKey);
                        if (intComparison < 0)
                        {
                            if (cIRec.Left >= 0)
                                fsIndex.Position = cIRec.Left;
                            else
                            {
                                // insert new key here
                                cIRec.Left = fsIndex.Length;

                                Index_Write(ref cIRec, lngPositionStartRead);

                                cIRec.Key = strSearchKey;
                                cIRec.Left
                                    = cIRec.Right
                                    = -1;
                                cIRec.Index = fsFile.Length;

                                Index_Write(ref cIRec, fsIndex.Length);
                                break;
                            }
                        }
                        else if (intComparison > 0)
                        {
                            if (cIRec.Right >= 0)
                                fsIndex.Position = cIRec.Right;
                            else
                            {
                                // insert new key here
                                cIRec.Right = fsIndex.Length;
                                Index_Write(ref cIRec, lngPositionStartRead);

                                cIRec.Key = strSearchKey;
                                cIRec.Left
                                    = cIRec.Right
                                    = -1;
                                cIRec.Index = fsFile.Length;

                                Index_Write(ref cIRec, fsIndex.Length);
                                break;
                            }
                        }
                        else
                        {
                            // this should never happen
                            //MessageBox.Show("SPObject.Textbox.classFont.Font_Create() - this should never happen");
                            return;
                        }
                    }
                }

                // now append FontFile with this font information
                long lngFontStart = fsFile.Length;
                semFontFileAccess.WaitOne();
                for (byte bytStyleCounter = 0; bytStyleCounter < 16; bytStyleCounter++)
                {
                    FontStyle fnt0 = (bytStyleCounter & (byte)Math.Pow(2, 0)) > 0 ? fntStyleOrder[0] : FontStyle.Regular;
                    FontStyle fnt1 = (bytStyleCounter & (byte)Math.Pow(2, 1)) > 0 ? fntStyleOrder[1] : FontStyle.Regular;
                    FontStyle fnt2 = (bytStyleCounter & (byte)Math.Pow(2, 2)) > 0 ? fntStyleOrder[2] : FontStyle.Regular;
                    FontStyle fnt3 = (bytStyleCounter & (byte)Math.Pow(2, 3)) > 0 ? fntStyleOrder[3] : FontStyle.Regular;

                    FontStyle fntStyle = fnt0 | fnt1 | fnt2 | fnt3;
                    Font fntTemp = new Font(fnt.FontFamily.Name, fnt.Size, fntStyle);
                    fsFile.Position = lngFontStart + FontStyleShift(fntTemp);
                    int intBckRetVal = bytStyleCounter * 256;
                    for (int intCharCounter = 0; intCharCounter < 256; intCharCounter++)
                    {
                        char chr = (char)intCharCounter;
                        Rectangle rec = FontFile_Rectangle(fntTemp, chr);
                        formatter.Serialize(fsFile, (int)rec.Left);
                        formatter.Serialize(fsFile, (int)rec.Top);
                        formatter.Serialize(fsFile, (int)rec.Width);
                        formatter.Serialize(fsFile, (int)rec.Height);
                    }
                }
                semFontFileAccess.Release();

                return;
            }

            static Rectangle FontFile_Rectangle(Font fnt, char cIn)
            {
                Color clrBackground = Color.White;
                SolidBrush sbrBackground = new SolidBrush(clrBackground);

                Color clrForeground = Color.Black;
                SolidBrush sbrForeground = new SolidBrush(clrForeground);

                byte bytIndex = (byte)cIn;

                if ((bytIndex >= 32 && bytIndex < 127)
                        || bytIndex > 160)
                {
                    if (cIn == ' ')
                        cIn = '_';
                    string strTemp = cIn.ToString();
                    Size sz = System.Windows.Forms.TextRenderer.MeasureText(strTemp, fnt);
                    Bitmap bmpSource = new Bitmap((int)(sz.Width * (cIn == '&' ? 2.5 : 1.0)),
                                                  sz.Height + 5);

                    // draw the original character
                    using (Graphics g = Graphics.FromImage(bmpSource))
                    {
                        g.FillRectangle(sbrBackground,
                                        new RectangleF(0, 0, bmpSource.Width, bmpSource.Height));
                        g.DrawString(strTemp,
                                     fnt,
                                     sbrForeground,
                                     new Point(0, 0));
                    }

                    //bmpSource.Save(@"c:\debug\bmpSource.png");
                    // calculate what space of source bmp is painted
                    int intLeft = bmpSource.Width, intRight = 0;
                    if (cIn != ' ')
                    {
                        for (int x = 0; x < bmpSource.Width; x++)
                        {
                            for (int y = 0; y < bmpSource.Height; y++)
                            {
                                Color clrPixel = bmpSource.GetPixel(x, y);
                                if (!(clrPixel.R == sbrBackground.Color.R
                                        && clrPixel.G == sbrBackground.Color.G
                                        && clrPixel.B == sbrBackground.Color.B))
                                {
                                    if (x < intLeft) intLeft = x;
                                    if (x > intRight) intRight = x;
                                }
                            }
                        }
                    }

                    // sample onlyl the painted source bmp and draw onto output bitmap
                    int intWidth = intRight - intLeft + 1;
                    if (intWidth < 1)
                        return new Rectangle(0, 0, 10, 10);
                    Rectangle recRetVal = new Rectangle(intLeft, 0, intWidth, bmpSource.Height);
                    return recRetVal;
                }
                else
                    return new Rectangle(0, 0, 10, 10);
            }
            #endregion

            public Bitmap get(char cIn, enuSelected eSelected)
            {
                byte bytIndex = (byte)cIn;

                if (bmp[(int)eSelected, bytIndex] == null)
                {
                    Color clrBackground = txt.BackColor;
                    SolidBrush sbrBackground = new SolidBrush(clrBackground);

                    Color clrForeground = txt.ForeColor;
                    SolidBrush sbrForeground = new SolidBrush(clrForeground);

                    if (bytIndex < 128 || bytIndex > 160)
                    {
                        string strTemp = cIn.ToString();

                        // sample onlyl the painted source bmp and draw onto output bitmap
                        Rectangle recSource = new Rectangle();
                        while (!Read(fntStandard, cIn, eSelected, ref recSource))
                        {
                            Font_Create(fntStandard);
                        }

                        Rectangle recDestination = new Rectangle(0, 0, recSource.Width, recSource.Height);

                        System.Drawing.Size sz = System.Windows.Forms.TextRenderer.MeasureText(strTemp, fntStandard);
                        Bitmap bmpSource = new Bitmap(sz.Width * 3, sz.Height * 2);
                        Color clrRND = Math3.classRND.Get_Color();

                        using (Graphics g = Graphics.FromImage(bmpSource))
                        {
                            g.FillRectangle(eSelected == enuSelected.Selected
                                                       ? sbrForeground
                                                       : sbrBackground,
                                            new RectangleF(0, 0, bmpSource.Width, bmpSource.Height));
                            g.DrawString(strTemp,
                                         fntStandard,
                                         eSelected == enuSelected.Selected
                                                    ? sbrBackground
                                                    : sbrForeground,
                                         new Point(0, 0));
                        }
                        //bmpSource.Save(@"c:\debug\bmpSource.png");
                        System.Drawing.Bitmap bmpTemp = new System.Drawing.Bitmap(recDestination.Width, recDestination.Height);

                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpTemp))
                        {
                            g.DrawImage(bmpSource, recDestination, recSource, GraphicsUnit.Pixel);
                        }

                        //bmpTemp.Save(@"c:\debug\bmp" + bytIndex.ToString("000") + (eSelected == enuSelected.Selected ? "selected" : "") + ".png");
                        bmp[(int)eSelected, bytIndex] = bmpTemp;
                    }
                }

                return bmp[(int)eSelected, bytIndex];
            }

            static long _lngRecordSize = -1;
            static long lngRecordSize
            {
                get
                {
                    if (_lngRecordSize < 0)
                    {
                        string strFilename = System.IO.Directory.GetCurrentDirectory() + "\\Temp.xyz";
                        int intCounter = 0;
                        while (System.IO.File.Exists(strFilename))
                            strFilename = System.IO.Directory.GetCurrentDirectory() + "\\Temp" + (intCounter++).ToString() + ".xyz";
                        System.IO.FileStream fsTemp = new System.IO.FileStream(strFilename, System.IO.FileMode.Create);

                        fsTemp.Position = 0;
                        Rectangle recExample = new Rectangle(11, 0, 254, 111);

                        formatter.Serialize(fsTemp, (int)recExample.Left);
                        formatter.Serialize(fsTemp, (int)recExample.Top);
                        formatter.Serialize(fsTemp, (int)recExample.Width);
                        formatter.Serialize(fsTemp, (int)recExample.Height);

                        _lngRecordSize = fsTemp.Position;

                        fsTemp.Close();
                        System.IO.File.Delete(strFilename);
                    }
                    return _lngRecordSize;
                }
            }

            static bool Read(Font fnt, char c, enuSelected eSelected, ref Rectangle recRead)
            {
                semIndexFileAccess.WaitOne();

                if (fsIndex == null)
                {
                    if (System.IO.File.Exists(FontIndex_Filename))
                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Open);
                    else

                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Create);
                }

                if (fsFile == null)
                {
                    if (System.IO.File.Exists(FontFile_Filename))
                        fsFile = new System.IO.FileStream(FontFile_Filename, System.IO.FileMode.Open);
                    else
                        fsFile = new System.IO.FileStream(FontFile_Filename, System.IO.FileMode.Create);
                }

                string strSearchKey = fnt.FontFamily.Name.ToUpper() + fnt.Size.ToString();
                fsIndex.Position = 0;
                classIndex_Record cIRec = new classIndex_Record();
                if (fsIndex.Length > fsIndex.Position)
                {
                    do
                    {
                        cIRec.Key = (string)formatter.Deserialize(fsIndex);
                        cIRec.Left = (long)formatter.Deserialize(fsIndex);
                        cIRec.Right = (long)formatter.Deserialize(fsIndex);
                        cIRec.Index = (long)formatter.Deserialize(fsIndex);

                        int intComparison = string.Compare(cIRec.Key, strSearchKey);
                        if (intComparison < 0)
                        {
                            if (cIRec.Left >= 0)
                                fsIndex.Position = cIRec.Left;
                            else
                                break;
                        }
                        else if (intComparison > 0)
                        {
                            if (cIRec.Right >= 0)
                                fsIndex.Position = cIRec.Right;
                            else
                                break;
                        }
                        else
                        {
                            // we have the FontFile index value

                            /*
                            fsFile.Position = cIRec.Index + FontStyleShift(fnt) + (int)c * lngRecordSize;
                            int intLeft = (int)formatter.Deserialize(fsFile);
                            int intTop = (int)formatter.Deserialize(fsFile);
                            int intWidth = (int)formatter.Deserialize(fsFile);
                            int intHeight = (int)formatter.Deserialize(fsFile);
                            recRead = new Rectangle(intLeft, intTop, intWidth, intHeight);
                            /*/
                            recRead = FontFile_Read(cIRec.Index + FontStyleShift(fnt) + (int)c * lngRecordSize);

                            //*/
                            semIndexFileAccess.Release();
                            return true;
                        }
                    } while (true);
                }
                semIndexFileAccess.Release();
                return false;
            }

            static Rectangle FontFile_Read(long lngAddr)
            {
                Rectangle recRetVal = new Rectangle();

                semFontFileAccess.WaitOne();
                {
                    fsFile.Position = lngAddr;

                    recRetVal.X = (int)formatter.Deserialize(fsFile);
                    recRetVal.Y = (int)formatter.Deserialize(fsFile);
                    recRetVal.Width = (int)formatter.Deserialize(fsFile);
                    recRetVal.Height = (int)formatter.Deserialize(fsFile);
                }
                semFontFileAccess.Release();

                return recRetVal;
            }


            static long FontStyleShift(Font fnt)
            {
                byte bytStyle = 0;
                for (int intCounter = 0; intCounter < fntStyleOrder.Length; intCounter++)
                {
                    if ((fnt.Style & fntStyleOrder[intCounter]) > 0)
                    {
                        bytStyle += (byte)Math.Pow(2, intCounter);
                    }
                }

                long lngRetVal = (long)(256 * bytStyle * lngRecordSize);
                return lngRetVal;
            }

            public static void Font_BinTree_Rebuild()
            {
                if (fsIndex == null)
                {
                    if (System.IO.File.Exists(FontIndex_Filename))
                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Open);
                    else
                        fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Create);
                }

                // read entire file into a list
                List<classIndex_Record> lstFontLeaves = new List<classIndex_Record>();

                fsIndex.Position = 0;
                while (fsIndex.Position < fsIndex.Length)
                {
                    classIndex_Record cIRec = Index_Read(fsIndex.Position);
                    cIRec.Left
                        = cIRec.Right
                        = -1;
                    lstFontLeaves.Add(cIRec);
                }

                // erase old file & randomly insert each leaf
                fsIndex.Close();
                System.IO.File.Delete(FontIndex_Filename);

                if (System.IO.File.Exists(FontIndex_Filename))
                    fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Open);
                else
                    fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Create);

                while (lstFontLeaves.Count > 0)
                {
                    int intRND = Math3.classRND.Get_Int(0, lstFontLeaves.Count);
                    classIndex_Record cIRec = lstFontLeaves[intRND];
                    lstFontLeaves.Remove(cIRec);

                    if (fsIndex.Length == 0)
                    {
                        Index_Write(ref cIRec, 0);
                    }
                    else
                    {
                        fsIndex.Position = 0;
                        classIndex_Record cIRTemp = null;
                        do
                        {
                            long lngAddr = fsIndex.Position;
                            cIRTemp = Index_Read(fsIndex.Position);
                            int intComparison = string.Compare(cIRTemp.Key, cIRec.Key);
                            if (intComparison < 0)
                            {
                                if (cIRTemp.Left >= 0)
                                    fsIndex.Position = cIRTemp.Left;
                                else
                                {
                                    cIRTemp.Left = fsIndex.Length;
                                    Index_Write(ref cIRTemp, lngAddr);
                                    Index_Write(ref cIRec, cIRTemp.Left);
                                    break;
                                }
                            }
                            else if (intComparison > 0)
                            {
                                if (cIRTemp.Right >= 0)
                                    fsIndex.Position = cIRTemp.Right;
                                else
                                {
                                    cIRTemp.Right = fsIndex.Length;
                                    Index_Write(ref cIRTemp, lngAddr);
                                    Index_Write(ref cIRec, cIRTemp.Right);
                                    break;
                                }
                            }
                            else
                            {
                                // this should not happen
                                System.Windows.Forms.MessageBox.Show("SPObjects.Textbox.classFont.Font_RebuildBinTree() - this hsould not happen");
                                break;
                            }
                        } while (true);
                    }
                }
            }

            static void Index_Write(ref classIndex_Record cIRec, long lngPosition)
            {
                semIndexFileAccess.WaitOne();
                {
                    fsIndex.Position = lngPosition;

                    formatter.Serialize(fsIndex, cIRec.Key);
                    formatter.Serialize(fsIndex, cIRec.Left);
                    formatter.Serialize(fsIndex, cIRec.Right);
                    formatter.Serialize(fsIndex, cIRec.Index);
                }
                semIndexFileAccess.Release();
            }

            static classIndex_Record Index_Read(long lngAddr)
            {
                classIndex_Record cRetVal = new classIndex_Record();
                semIndexFileAccess.WaitOne();
                {
                    if (fsIndex == null)
                    {
                        if (System.IO.File.Exists(FontIndex_Filename))
                            fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Open);
                        else
                            fsIndex = new System.IO.FileStream(FontIndex_Filename, System.IO.FileMode.Create);
                    }

                    fsIndex.Position = lngAddr;
                    cRetVal.Key = (string)formatter.Deserialize(fsIndex);
                    cRetVal.Left = (long)formatter.Deserialize(fsIndex);
                    cRetVal.Right = (long)formatter.Deserialize(fsIndex);
                    cRetVal.Index = (long)formatter.Deserialize(fsIndex);
                }
                semIndexFileAccess.Release();
                return cRetVal;
            }
        }



        public class classIndex_Record
        {
            public string Key = "";
            public long Left = -1;
            public long Right = -1;
            public long Index = -1;
        }


    }
    public class CheckBox : Button
    {
        public CheckBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.CheckBox;
            cEle.obj = (object)this;
            cEle.Name = "CheckBox";
            System.Windows.Forms.CheckBox chk = new System.Windows.Forms.CheckBox();

            BackColor = Color.LightGray;
            ForeColor = Color.Black;
            //chk.CheckedChanged 
        }

        EventHandler eventCheckedChanged = null;
        public EventHandler CheckedChanged
        {
            get { return eventCheckedChanged; }
            set { eventCheckedChanged = value; }
        }

        bool bolChecked = false;
        public bool Checked
        {
            get { return bolChecked; }
            set
            {
                if (bolChecked != value)
                {
                    bolChecked = value;
                    cEle.NeedsToBeRedrawn = true;
                    if (eventCheckedChanged != null)
                        eventCheckedChanged((object)this, new EventArgs());
                }
            }
        }

        bool bolEnabled = true;
        public bool Enabled
        {
            get { return bolEnabled; }
            set
            {
                if (bolEnabled != value)
                {
                    bolEnabled = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;
            Bitmap bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, new Point(0, 0));
                else
                    g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));

                //                                          draw text
                Size szText = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
                g.DrawString(Text, Font, sbrForeColor, new Point(1, (Height - szText.Height) / 2));

                //                                          draw checked box
                int intBoxWidth = 10;

                Pen pCheck = new Pen(ForeColor, 2);
                Rectangle recBox = new Rectangle(Width - intBoxWidth - 1, (Height - intBoxWidth) / 2, intBoxWidth, intBoxWidth);
                g.DrawRectangle(pCheck, recBox);

                if (Checked)
                {
                    g.DrawLine(pCheck, new Point(recBox.Left + 1, recBox.Top + 1), new Point(recBox.Right - 1, recBox.Bottom - 1));
                    g.DrawLine(pCheck, new Point(recBox.Right - 1, recBox.Top + 1), new Point(recBox.Left + 1, recBox.Bottom - 1));
                }


                //                                          draw child-elements
                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }
            }
            if (BackTransSPContainer)
                bmpTemp.MakeTransparent(BackColor);
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }
    }


    public class CheckedListBox : SweepPrune_Object
    {
        int intIndex_TopVisible = 0;
        public class ItemsHandler
        {
            public class Item
            {
                public string heading = "";
                public bool Checked = false;
                public Item() { }
                public Item(string Heading)
                {
                    this.heading = Heading;
                    Checked = false;
                }
                public Item(string Heading, bool Checked)
                {
                    this.heading = Heading;
                    this.Checked = Checked;
                }
            }
            List<Item> lst = new List<Item>();

            CheckedListBox cCheckedListBox = null;
            public ItemsHandler(ref CheckedListBox CheckedListBox)
            {
                cCheckedListBox = CheckedListBox;
            }

            void Refresh()
            {
                cCheckedListBox.cEle.NeedsToBeRedrawn = true;
            }

            public Item Get(int index)
            {
                if (index >= 0 && index < lst.Count)
                    return lst[index];
                else return null;
            }

            public void Add(string strHeading, bool bolChecked)
            {
                Item itemNew = new Item(strHeading, bolChecked);
                lst.Add(itemNew);
                Refresh();
            }
            public void Add(string strHeading)
            {
                Add(strHeading, false);
            }

            public void Remove(string strRemove)
            {
                Item itemRemove = Search(strRemove);
                Remove(ref itemRemove);
            }

            public void Remove(ref Item itemRemove)
            {
                if (itemRemove != null)
                {
                    if (lst.Contains(itemRemove))
                    {
                        lst.Remove(itemRemove);
                        Refresh();
                    }
                }
            }

            public Item Search(string strSearch)
            {
                for (int intItemCounter = 0; intItemCounter < lst.Count; intItemCounter++)
                {
                    Item item = lst[intItemCounter];
                    if (string.Compare(item.heading, strSearch) == 0)
                        return item;
                }
                return null;
            }

            public void RemoveAt(int index)
            {
                if (index >= 0 && index < lst.Count)
                {
                    lst.RemoveAt(index);
                    Refresh();
                }
            }
            public void SetChecked(int index, bool CheckedState)
            {
                if (index >= 0 && index < lst.Count)
                {
                    lst[index].Checked = CheckedState;
                    Refresh();
                }
            }
            public void ToggleChecked(int index)
            {
                if (index >= 0 && index < lst.Count)
                {
                    lst[index].Checked = !lst[index].Checked;
                    Refresh();
                }
            }
            public List<Item> GetChecked()
            {
                List<Item> lstRetVal = new List<Item>();
                for (int intCounter = 0; intCounter < lst.Count; intCounter++)
                {
                    if (lst[intCounter].Checked)
                        lstRetVal.Add(lst[intCounter]);
                }
                return lstRetVal;
            }

            public void Insert(int index, string Heading)
            {
                Insert(index, Heading, false);
            }
            public void Insert(int index, string Heading, bool Checked)
            {
                Item itemInsert = new Item(Heading, Checked);
                Insert(index, ref itemInsert);
            }

            public void Insert(int index, ref Item itemInsert)
            {
                if (index >= 0 && index < lst.Count)
                {
                    lst.Insert(index, itemInsert);
                    Refresh();
                }
            }

            public int Count { get { return lst.Count; } }

            public void Clear()
            {
                if (lst.Count > 0)
                {
                    lst.Clear();
                    Refresh();
                }
            }

            public bool Contains(string strItem)
            {
                Item itemSearch = Search(strItem);
                return lst.Contains(itemSearch);
            }

        }

        #region events
        EventHandler _SelectedIndexChanged = null;
        public EventHandler SelectedIndexChanged
        {
            get { return _SelectedIndexChanged; }
            set { _SelectedIndexChanged = value; }
        }

        public void cbx_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.CheckedListBox chtest = new System.Windows.Forms.CheckedListBox();
            if (e.Delta > 0)
                Scroll_Up();
            else if (e.Delta < 0)
                Scroll_Down();
        }

        public void cbx_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Items.ToggleChecked(IndexUnderMouse);
                if (MouseClick != null)
                    MouseClick((object)this, e);
            }
        }

        Point UnderMouse = new Point();
        bool bolGrab = false;
        public void cbx_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ptMouse = new Point(e.X, e.Y);
                if (TotalHeight > Height)
                {
                    if (classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                    {
                        if (ptMouse.Y > recSlider.Top && ptMouse.Y < recSlider.Bottom)
                        {
                            // grab slider
                            UnderMouse = new Point(ptMouse.X - recSlider.Left, ptMouse.Y - recSlider.Top);
                            bolGrab = true;
                            return;
                        }
                    }

                }
            }
        }

        public void cbx_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (bolGrab)
            {
                int intSliderTop = e.Y - UnderMouse.Y;
                if (intSliderTop > Height - SliderHeight)
                    intSliderTop = Height - SliderHeight;
                intIndex_TopVisible = intSliderTop * (TotalHeight - Height) / ((Height - SliderHeight) * ItemHeight);
                if (intIndex_TopVisible < 0)
                    intIndex_TopVisible = 0;
                cEle.NeedsToBeRedrawn = true;
            }
            else
            {
                ptMouse = new Point(e.X, e.Y);
            }
        }

        public void cbx_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bolGrab = false;

                ptMouse = new Point(e.X, e.Y);
                if (TotalHeight > Height)
                {
                    if (classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                    {
                        if (ptMouse.Y < recSlider.Top)
                            Scroll_Up(NumVisible);
                        else if (ptMouse.Y > recSlider.Bottom)
                            Scroll_Down(NumVisible);
                        return;
                    }
                }
                else if (classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                {
                    int intIndexClicked = intIndex_TopVisible + (e.Y / ItemHeight);
                    if (intIndexClicked >= 0 && intIndexClicked < Items.Count)
                        SelectedIndex = intIndexClicked;
                }
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public void cbx_MouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            bolGrab = false;
            intIndexUnderMouse = -1;
        }

        public void cbx_MouseEnter(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }


        #endregion 

        public ItemsHandler Items = null;
        public CheckedListBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            CheckedListBox cMyRef = this;
            SPContainer.Add(ref cMyRef);
            Items = new ItemsHandler(ref cMyRef);
        }


        #region Properties

        Point _ptMouse = new Point();
        Point ptMouse
        {
            get { return _ptMouse; }
            set
            {
                _ptMouse = value;

                if (!SPObjects.classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                {
                    int intTemp = intIndex_TopVisible + ptMouse.Y / ItemHeight;
                    if (intTemp >= 0 && intTemp < Items.Count)
                    {
                        intIndexUnderMouse = intTemp;
                        return;
                    }
                }
                intIndexUnderMouse = -1;
            }
        }


        int intSelectedIndex = -1;
        public int SelectedIndex
        {
            get { return intSelectedIndex; }
            set
            {
                if (intSelectedIndex != value)
                {
                    intSelectedIndex = value;
                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged((object)this, new EventArgs());
                }
            }
        }

        bool bolDrawBorder = false;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        int intItemHeight = -1;
        public int ItemHeight
        {
            get
            {
                if (intItemHeight < 0)
                    intItemHeight = System.Windows.Forms.TextRenderer.MeasureText("H", Font).Height;
                return intItemHeight;
            }
        }


        #endregion

        #region Methods


        #endregion
        int NumVisible
        {
            get { return Height / ItemHeight; }
        }


        #region ScrollBar
        void Scroll_Up() { Scroll_Up(1); }
        void Scroll_Up(int intStep)
        {
            if (intIndex_TopVisible > 0)
            {
                intIndex_TopVisible -= intStep;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        void Scroll_Down() { Scroll_Down(1); }
        void Scroll_Down(int intStep)
        {
            if (intIndex_TopVisible + NumVisible < Items.Count)
            {
                intIndex_TopVisible += intStep;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        bool ScrollBarVisible
        {
            get { return (TotalHeight > Height); }
        }

        int TotalHeight
        {
            get { return (Items.Count * ItemHeight); }
        }

        int SliderHeight
        {
            get { return (int)(Math.Pow(Height, 2) / TotalHeight); }
        }

        int SliderTop
        {
            get { return (int)((Height - SliderHeight) * (intIndex_TopVisible * ItemHeight) / (TotalHeight - Height)); }
        }

        int intScrollBarWidth = 10;

        Rectangle _recSlider = new Rectangle();
        Rectangle recSlider
        {
            get
            {
                Size szSlider = new Size(intScrollBarWidth, SliderHeight);
                Point ptSlider = new Point(Width - intScrollBarWidth, SliderTop);
                _recSlider = new Rectangle(ptSlider, szSlider);
                return _recSlider;
            }
        }

        Rectangle _recScrollBar;
        Rectangle recScrollBar
        {
            get
            {
                _recScrollBar = new Rectangle(Width - intScrollBarWidth, 0, intScrollBarWidth, Height);
                return _recScrollBar;
            }
        }

        bool bolHighlightIndexUnderMouse = true;
        public bool Highlight_IndexUnderMouse
        {
            get { return bolHighlightIndexUnderMouse; }
            set
            {
                if (bolHighlightIndexUnderMouse != value)
                {
                    bolHighlightIndexUnderMouse = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        int _intIndexUnderMouse = -1;
        int intIndexUnderMouse
        {
            get { return _intIndexUnderMouse; }
            set
            {
                if (_intIndexUnderMouse != value)
                {
                    _intIndexUnderMouse = value;
                    if (Highlight_IndexUnderMouse)
                        cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        public int IndexUnderMouse
        {
            get { return intIndexUnderMouse; }
        }
        #endregion

        public override void Draw()
        {
            if (cEle == null || !cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

            Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);

            Graphics g = Graphics.FromImage(bmpTemp);
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                else
                    g.FillRectangle(new SolidBrush(BackColor), rectDestination);

                for (int intItemCounter = intIndex_TopVisible;
                    intItemCounter < intIndex_TopVisible + NumVisible && intItemCounter < Items.Count;
                    intItemCounter++)
                {
                    Point ptDraw = new Point(0, ItemHeight * (intItemCounter - intIndex_TopVisible));
                    bool bolHighlight = (intItemCounter == SelectedIndex
                                            || (intItemCounter == intIndexUnderMouse
                                                    && Highlight_IndexUnderMouse));
                    ItemsHandler.Item item = Items.Get(intItemCounter);
                    DrawItem(ref g, ref item, ptDraw, bolHighlight);

                }
                Pen pForeColor = new Pen(ForeColor);

                if (ScrollBarVisible)
                {
                    // draw scroll bar
                    g.DrawRectangle(pForeColor, recScrollBar);
                    g.FillRectangle(new SolidBrush(Color.Gray), recSlider);
                }

                if (DrawBorder)
                    g.DrawRectangle(pForeColor, new Rectangle(0, 0, recSPArea.Width - 1, recSPArea.Height - 1));
            }
            g.Dispose();

            //bool bolDebug = false;
            //if (bolDebug)
            //    bmpTemp.Save(@"c:\debug\bmpTemp.bmp");
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }


        void DrawItem(ref Graphics g, ref ItemsHandler.Item item, Point ptDraw, bool Highlight)
        {
            SolidBrush sbrBackTemp = bolHighlightIndexUnderMouse
                                                ? sbrBackColor
                                                : sbrForeColor;
            SolidBrush sbrForeTemp = bolHighlightIndexUnderMouse
                                                ? sbrForeColor
                                                : sbrBackColor;
            int intRight = Width
                            - (ScrollBarVisible
                                        ? recScrollBar.Width
                                        : 0);
            g.FillRectangle(sbrBackTemp, new RectangleF(ptDraw, new Size(intRight, ItemHeight)));
            g.DrawString(item.heading, Font, sbrForeColor, ptDraw);

            int intGap = 1;
            int intCheckWidth = ItemHeight - 2 * intGap;
            Rectangle recCheck = new Rectangle(ptDraw.X + intRight - intGap - intCheckWidth,
                                               ptDraw.Y + intGap,
                                               intCheckWidth,
                                               intCheckWidth);
            Pen p = new Pen(sbrForeTemp.Color, 2);
            g.DrawRectangle(p, recCheck);
            if (item.Checked)
            {
                int intCheckGap = 2;
                g.DrawLine(p,
                           new Point(recCheck.Left + intCheckGap,
                                     recCheck.Top + intCheckGap),
                           new Point(recCheck.Right - intCheckGap,
                                     recCheck.Bottom - intCheckGap));
                g.DrawLine(p,
                           new Point(recCheck.Left + intCheckGap,
                                     recCheck.Bottom - intCheckGap),
                           new Point(recCheck.Right - intCheckGap,
                                     recCheck.Top + intCheckGap));
            }
        }
    }


    public class Label : Button
    {
        public Label(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.Label;
            cEle.obj = (object)this;
            cEle.Name = "Label";

        }


        public override Color BackColor
        {
            get
            {
                return cEle.BackColor;
            }
            set
            {
                BackColor_Dull
                    = BackColor_Highlight
                    = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return cEle.ForeColor;
            }
            set
            {
                ForeColor_Dull
                    = ForeColor_Highlight
                    = value;
            }
        }
    }

    public class Panel : SweepPrune_Object
    {
        public Panel(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            Panel cMyRef = this;
            SPContainer.Add(ref cMyRef);
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress)
            {
                cEle.DoNotDrawChildElements();
                return;
            }

            bool bolRedraw = _MyImage == null;

            Bitmap bmpTemp = !bolRedraw
                                ? new Bitmap(_MyImage)
                                : new Bitmap(recSPArea.Width, recSPArea.Height);

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (bolRedraw)
                {
                    if (BackgroundImage != null)
                        g.DrawImage(BackgroundImage, new Point(0, 0));
                    else
                        g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));
                    for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                    {
                        classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                        if (cItem.Visible)
                        {
                            Bitmap bmpMyImage = cItem.MyImage;
                            if (bmpMyImage != null)
                            {
                                g.DrawImage(bmpMyImage, cItem.rec);
                                cItem.NeedsToBeRedrawn = false;
                            }
                        }
                    }
                }
                else
                {
                    //_MyImage.Save(@"c:\debug\pnl.png");
                    for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                    {
                        classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                        if (cItem.Visible
                            && (cItem.NeedsToBeRedrawn
                                || cItem.ImageRefreshed))
                        {
                            Bitmap bmpMyImage = cItem.MyImage;
                            if (bmpMyImage != null)
                            {
                                g.DrawImage(bmpMyImage, cItem.rec);
                                cItem.NeedsToBeRedrawn = false;
                            }
                        }
                    }
                }
                //BackgroundImage.Save(@"c:\debug\bmpBackgroundImage.png");
            }

            if (BackTransSPContainer)
                bmpTemp.MakeTransparent(BackColor);
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }
    }
    public class ItemsHandler
    {
        List<string> lst = new List<string>();

        ListBox cListBox = null;
        public ItemsHandler(ref ListBox ListBox)
        {
            cListBox = ListBox;
        }

        void Refresh()
        {
            cListBox.cEle.NeedsToBeRedrawn = true;
        }

        public int IndexOf(string strItem)
        {
            return lst.IndexOf(strItem);
        }
        public string Get(int index)
        {
            if (index >= 0 && index < lst.Count)
                return lst[index];
            else return "";
        }

        public void Add(string strAdd)
        {
            lst.Add(strAdd);
            Refresh();
        }

        public void Remove(string strRemove)
        {
            if (lst.Contains(strRemove))
            {
                lst.Remove(strRemove);
                Refresh();
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < lst.Count)
            {
                lst.RemoveAt(index);
                Refresh();
            }
        }

        public void Insert(int index, string strValue)
        {
            if (index >= 0 && index < lst.Count)
            {
                lst.Insert(index, strValue);
                Refresh();
            }
        }

        public int Count { get { return lst.Count; } }

        public void Clear()
        {
            if (lst.Count > 0)
            {
                lst.Clear();
                Refresh();
            }
        }

        public bool Contains(string strItem)
        {
            return lst.Contains(strItem);
        }

    }

    public class ListBox : SweepPrune_Object
    {
        int intIndex_TopVisible = 0;

        #region events
        EventHandler _SelectedIndexChanged = null;
        public EventHandler SelectedIndexChanged
        {
            get { return _SelectedIndexChanged; }
            set { _SelectedIndexChanged = value; }
        }


        public void lbx_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                Scroll_Up();
            else if (e.Delta < 0)
                Scroll_Down();
        }


        public void lbx_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)

                if (MouseClick != null && !Multiselecting)
                    MouseClick((object)this, e);
        }

        Point UnderMouse = new Point();
        bool bolGrab = false;
        public void lbx_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ptMouse = new Point(e.X, e.Y);
                if (TotalHeight > Height)
                {
                    if (classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                    {
                        if (ptMouse.Y > recSlider.Top && ptMouse.Y < recSlider.Bottom)
                        {
                            // grab slider
                            UnderMouse = new Point(ptMouse.X - recSlider.Left, ptMouse.Y - recSlider.Top);
                            bolGrab = true;
                        }
                    }
                }
            }
        }

        public void lbx_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (bolGrab)
            {
                int intSliderTop = e.Y - UnderMouse.Y;
                //if (intSliderTop > Height - SliderHeight)
                //    intSliderTop = Height - SliderHeight;
                intIndex_TopVisible = intSliderTop * (TotalHeight - Height) / ((Height - SliderHeight) * ItemHeight);
                if (intIndex_TopVisible < 0)
                    intIndex_TopVisible = 0;
                else if (intIndex_TopVisible + NumVisible > Items.Count)
                    intIndex_TopVisible = Items.Count - NumVisible;

                cEle.NeedsToBeRedrawn = true;
            }
            else
            {
                ptMouse = new Point(e.X, e.Y);
            }
        }

        public void lbx_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bolGrab = false;

                ptMouse = new Point(e.X, e.Y);
                if (TotalHeight > Height)
                {
                    if (classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                    {
                        if (ptMouse.Y < recSlider.Top)
                        {
                            int intStep = NumVisible;
                            if (intIndex_TopVisible - intStep < 0)
                                intStep = intIndex_TopVisible;
                            Scroll_Up(intStep);
                        }
                        else if (ptMouse.Y > recSlider.Bottom)
                        {
                            int intStep = NumVisible;
                            if (intIndex_TopVisible + intStep + NumVisible >= Items.Count)
                                intStep = Items.Count - NumVisible - intIndex_TopVisible;
                            Scroll_Down(intStep);
                        }
                        return;
                    }
                }

                int intIndexClicked = intIndex_TopVisible + (e.Y / ItemHeight);
                if ((!MultiSelect && intIndexClicked >= 0 && intIndexClicked < Items.Count)
                    || (MultiSelect && !Multiselecting))
                    SelectedIndex = intIndexClicked;

                Multiselecting = false;
                _intSelecting_IndexSelectedFirst = -1;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public void lbxMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();
            {
                bolGrab = false;
                intIndexUnderMouse = -1;
                cEle.NeedsToBeRedrawn = true;

            }
            if (!bolBuilding)
                SPContainer.Building_Complete();
        }

        public void lbx_MouseEnter(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }


        #endregion 

        public ItemsHandler Items = null;
        public ListBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            ListBox cMyRef = this;
            SPContainer.Add(ref cMyRef);
            Items = new ItemsHandler(ref cMyRef);
        }

        #region MultiSelection
        bool bolMultiselecting = false;
        bool Multiselecting
        {
            get { return bolMultiselecting; }
            set
            {
                bolMultiselecting = value;
            }
        }


        bool bolMultiSelect = false;
        public bool MultiSelect
        {
            get { return bolMultiSelect; }
            set { bolMultiSelect = value; }
        }

        public int SelectedIndex
        {
            get
            {
                return lstSelectedIndices.Count > 0
                                                ? lstSelectedIndices[0]
                                                : -1;
            }
            set
            {
                lstSelectedIndices.Clear();
                lstSelectedIndices.Add(value);
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                _intSelecting_IndexSelectedFirst = -1;
            }

        }

        public string SelectedItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    return Items.Get(SelectedIndex);
                else
                    return "";
            }
        }

        int _intSelecting_IndexSelectedFirst = -1;
        int intSelecting_IndexSelectedFirst
        {
            get { return _intSelecting_IndexSelectedFirst; }
            set
            {
                _intSelecting_IndexSelectedFirst = value;

                lstSelectedIndices.Clear();
                if (_intSelecting_IndexSelectedFirst >= 0 && _intSelecting_IndexSelectedFirst < Items.Count)
                    lstSelectedIndices.Add(_intSelecting_IndexSelectedFirst);
                cEle.NeedsToBeRedrawn = true;
            }
        }

        List<int> lstSelectedIndices = new List<int>();
        public List<int> SelectedIndices
        {
            get
            {
                return lstSelectedIndices;
            }
            set
            {
                IEnumerable<int> query = value.OrderBy(num => num, new SpecialComparer_int());
                value = query.ToList<int>();

                while (value.Count > 0 && value[0] < 0)
                    value.RemoveAt(0);

                while (value.Count > 0 && value[value.Count - 1] >= Items.Count)
                    value.RemoveAt(value.Count - 1);

                lstSelectedIndices = value;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        void Selecting_SetSelected()
        {
            if (IndexUnderMouse < 0) return;
            if (intSelecting_IndexSelectedFirst < 0)
            {
                intSelecting_IndexSelectedFirst = IndexUnderMouse;
                lstSelectedIndices.Clear();
                lstSelectedIndices.Add(IndexUnderMouse);
                return;
            }

            int intStart = intSelecting_IndexSelectedFirst < intIndexUnderMouse
                                                           ? intSelecting_IndexSelectedFirst
                                                           : intIndexUnderMouse;
            int intEnd = intSelecting_IndexSelectedFirst > intIndexUnderMouse
                                                         ? intSelecting_IndexSelectedFirst
                                                         : intIndexUnderMouse;
            lstSelectedIndices.Clear();
            for (int intCounter = intStart; intCounter <= intEnd; intCounter++)
                lstSelectedIndices.Add(intCounter);
        }

        public void SelectAll()
        {
            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();
            {
                lstSelectedIndices.Clear();
                for (int intItemCounter = 0; intItemCounter < Items.Count; intItemCounter++)
                {
                    lstSelectedIndices.Add(intItemCounter);
                }
                cEle.NeedsToBeRedrawn = true;
            }
            if (!bolBuilding)
                SPContainer.Building_Complete();
        }

        #endregion


        #region Properties

        Point _ptMouse = new Point();
        Point ptMouse
        {
            get { return _ptMouse; }
            set
            {
                _ptMouse = value;

                if (!SPObjects.classSPMath.PointIsInsideARectangle(ptMouse, recScrollBar))
                {
                    int intTemp = intIndex_TopVisible + ptMouse.Y / ItemHeight;
                    if (intTemp >= 0 && intTemp < Items.Count)
                    {
                        intIndexUnderMouse = intTemp;

                        if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left && intSelecting_IndexSelectedFirst < 0)
                        {
                            intSelecting_IndexSelectedFirst = IndexUnderMouse;
                            Multiselecting = false;
                        }

                        return;
                    }
                }

                intIndexUnderMouse = -1;
            }
        }

        bool bolDrawBorder = false;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }


        //bool bolCanMoveSelected = false;
        //public bool CanMoveSelected
        //{
        //    get { return bolCanMoveSelected; }
        //    set { bolCanMoveSelected = value; }
        //}


        int intItemHeight = -1;
        public int ItemHeight
        {
            get
            {
                if (intItemHeight < 0)
                    intItemHeight = System.Windows.Forms.TextRenderer.MeasureText("H", Font).Height;
                return intItemHeight;
            }
        }


        #endregion

        #region Methods

        public void SetSelected(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                SelectedIndex = index;
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged((object)this, new EventArgs());
            }
        }

        public string GetSelected()
        {
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                return Items.Get(SelectedIndex);
            else
                return "";
        }
        public void ClearSelected()
        {
            if (SelectedIndex != -1)
            {
                SelectedIndex = -1;
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged((object)this, new EventArgs());
            }
        }

        #endregion
        int NumVisible
        {
            get { return Height / ItemHeight; }
        }


        #region ScrollBar
        void Scroll_Up() { Scroll_Up(1); }
        void Scroll_Up(int intStep)
        {
            if (intIndex_TopVisible > 0)
            {
                intIndex_TopVisible -= intStep;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        void Scroll_Down() { Scroll_Down(1); }
        void Scroll_Down(int intStep)
        {
            if (intIndex_TopVisible + NumVisible < Items.Count)
            {
                intIndex_TopVisible += intStep;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        int TotalHeight
        {
            get { return (Items.Count * ItemHeight); }
        }

        int SliderHeight
        {
            get { return (int)(Math.Pow(Height, 2) / TotalHeight); }
        }

        int SliderTop
        {
            get { return (int)((Height - SliderHeight) * (intIndex_TopVisible * ItemHeight) / (TotalHeight - Height)); }
        }

        int intScrollBarWidth = 10;

        Rectangle _recSlider = new Rectangle();
        Rectangle recSlider
        {
            get
            {
                Size szSlider = new Size(intScrollBarWidth, SliderHeight);
                Point ptSlider = new Point(Width - intScrollBarWidth, SliderTop);
                _recSlider = new Rectangle(ptSlider, szSlider);
                return _recSlider;
            }
        }

        bool ScrollBarVisible
        {
            get { return (TotalHeight > Height); }
        }

        Rectangle _recScrollBar;
        Rectangle recScrollBar
        {
            get
            {
                _recScrollBar = new Rectangle(Width - intScrollBarWidth, 0, intScrollBarWidth, Height);
                return _recScrollBar;
            }
        }

        bool bolHighlightIndexUnderMouse = true;
        public bool Highlight_IndexUnderMouse
        {
            get { return bolHighlightIndexUnderMouse; }
            set
            {
                if (bolHighlightIndexUnderMouse != value)
                {
                    bolHighlightIndexUnderMouse = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        int _intIndexUnderMouse = -1;
        int intIndexUnderMouse
        {
            get { return _intIndexUnderMouse; }
            set
            {
                if (_intIndexUnderMouse != value)
                {
                    _intIndexUnderMouse = value;
                    if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (MultiSelect)
                        {
                            if (_intIndexUnderMouse != intSelecting_IndexSelectedFirst)
                                Multiselecting = true;

                            Selecting_SetSelected();
                        }
                    }

                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }
        public int IndexUnderMouse
        {
            get { return intIndexUnderMouse; }
        }
        #endregion

        public override void Draw()
        {
            if (cEle == null || !cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

            Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);

            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, rectDestination, new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
                else
                    g.FillRectangle(new SolidBrush(BackColor), rectDestination);

                for (int intItemCounter = intIndex_TopVisible;
                    intItemCounter < intIndex_TopVisible + NumVisible && intItemCounter < Items.Count;
                    intItemCounter++)
                {
                    Point ptDraw = new Point(0, ItemHeight * (intItemCounter - intIndex_TopVisible));
                    if (intItemCounter == SelectedIndex
                        || (intItemCounter == intIndexUnderMouse && Highlight_IndexUnderMouse)
                        || (MultiSelect && lstSelectedIndices.Contains(intItemCounter)))
                    {
                        g.FillRectangle(sbrForeColor, new RectangleF(ptDraw, new Size(Width, ItemHeight)));
                        g.DrawString(Items.Get(intItemCounter), Font, sbrBackColor, ptDraw);
                    }
                    else
                    {

                        g.DrawString(Items.Get(intItemCounter), Font, sbrForeColor, ptDraw);
                    }
                }
                Pen pForeColor = new Pen(ForeColor);

                if (ScrollBarVisible)
                {
                    // draw scroll bar
                    g.DrawRectangle(pForeColor, recScrollBar);
                    g.FillRectangle(new SolidBrush(Color.Gray), recSlider);
                }

                if (DrawBorder)
                    g.DrawRectangle(pForeColor, new Rectangle(0, 0, recSPArea.Width - 1, recSPArea.Height - 1));
            }

            //bool bolDebug = false;
            //if (bolDebug)
            //    bmpTemp.Save(@"c:\debug\bmpTemp.bmp");
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }
    }


    public class TextDisplay : SweepPrune_Object
    {
        public classScrollBar cScrollBar = null;
        List<classLine> lstLines = new List<classLine>();
        List<classLine> lstLinesDrawn = new List<classLine>();

        TextDisplay cMyRef = null;

        public TextDisplay(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            cMyRef = this;

            this.SPContainer = SPContainer;
            SPContainer.Add(ref cMyRef);

            cScrollBar = new classScrollBar(ref SPContainer);
            cScrollBar.ValueChanged = cScrollBar_ValueChanged;
            Sizechanged = Event_SizeChanged;
            MouseWheel = Event_MouseWheel;
            MouseDown = Event_MouseDown;
            MouseUp = Event_MouseUp;
            MouseMove = Event_MouseMove;
        }
        
        public string WordUnderMouse
        {
            get
            {
                int intIndex = lstLinesDrawn.Count;
                int intStep = intIndex;
                int intDir = -1;
                int intNumFail = 0;
                while (true)
                {
                    intStep /= 2;
                    if (intStep < 1)
                    {
                        intStep = 1;
                        intNumFail++;
                        if (intNumFail > 32)
                            return "";
                    }
                    intIndex += (intDir * intStep);
                    if (intIndex < 0)
                        intIndex = 0;
                    else if (intIndex >= lstLinesDrawn.Count)
                        intIndex = lstLinesDrawn.Count - 1;

                    if (lstLinesDrawn[intIndex].LineContainsPoint(ptMouse))
                        break;

                    intDir = (ptMouse.Y > lstLinesDrawn[intIndex].rec.Bottom - lstLinesDrawn[0].rec.Top)
                                        ? 1
                                        : -1;
                }

                string strLine = lstLinesDrawn[intIndex].Text;
                //System.Diagnostics.Debug.Print(strLine);
                int intCharCounter = 0;
                string strWord = "";
                while (intCharCounter < strLine.Length)
                {
                    char chr = strLine[intCharCounter];
                    strWord += chr;
                    if (!char.IsLetterOrDigit(chr))
                    {
                        Size szString = System.Windows.Forms.TextRenderer.MeasureText(strLine.Substring(0, intCharCounter), Font);
                        if (szString.Width > ptMouse.X)
                            return strWord;
                        else
                            strWord = "";
                    }
                    intCharCounter++;
                }


                return strWord;
            }
        }


        Point _ptMouse = new Point();
        Point ptMouse
        {
            get { return _ptMouse; }
            set { _ptMouse = value; }
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            Bitmap bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);
            Graphics g = Graphics.FromImage(bmpTemp);
            {
                Pen pBorder = new Pen(ForeColor);

                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, new Point(0, 0));
                else
                    g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));
                
                if (DrawBorder)
                    g.DrawRectangle(pBorder, new Rectangle(0, 0, bmpTemp.Width-1, bmpTemp.Height-1));

                Point ptDraw = new Point(0, 0);
                int intLineCounter = cScrollBar.Value;
                if (intLineCounter < 0)
                    intLineCounter = 0;
                List<classLine> lstLinesDrawn_New = new List<classLine>();
                while (intLineCounter < lstLines.Count)
                {
                    g.DrawImage(lstLines[intLineCounter].bmp, ptDraw);
                    ptDraw.Y += lstLines[intLineCounter].bmp.Height;

                    lstLinesDrawn.Remove(lstLines[intLineCounter]);
                    lstLinesDrawn_New.Add(lstLines[intLineCounter]);

                    intLineCounter++;
                    if (ptDraw.Y > Height || intLineCounter >= lstLines.Count)
                        break;
                }

                while (lstLinesDrawn.Count >0)
                {
                    lstLinesDrawn[0].FreeBitmap();
                    lstLinesDrawn.RemoveAt(0);
                }

                lstLinesDrawn = lstLinesDrawn_New;

                if (cScrollBar.Visible)
                {
                    cScrollBar.recDraw = new Rectangle(bmpTemp.Width - cScrollBar.Thickness,
                                                       0,
                                                       cScrollBar.Thickness,
                                                       bmpTemp.Height);

                    cScrollBar.recSliderDraw = recSlider;

                    g.FillRectangle(Brushes.Gray, cScrollBar.recDraw);
                    g.FillRectangle(Brushes.DarkGray, cScrollBar.recSliderDraw);
                }

                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }
            }
            g.Dispose();
            if (BackTransSPContainer)
                bmpTemp.MakeTransparent(BackColor);

 
            MyImage = bmpTemp;
        }

        
        public void Reset()
        {
            lstLines.Clear();
            
            Parse_Text();
            
            cScrollBar.recDraw = new Rectangle(Width - cScrollBar.Thickness, 0, cScrollBar.Thickness, Height);
            if (lstLines.Count >0 && lstLines[lstLines.Count-1].rec.Bottom > Height)
            {
                int intLineCounter = 0;
                while (intLineCounter < lstLines.Count && lstLines[intLineCounter].rec.Bottom < Height)
                    intLineCounter++;
                cScrollBar.LargeChange = intLineCounter - 1;
                cScrollBar.Visible = true;
            }
            else
                cScrollBar.Visible = false;

            cScrollBar.Value = 0;
        }


        bool bolDrawBorder = false;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set { bolDrawBorder = value; }
        }

        int LineHeight
        {
            get
            {
                if (lstLines.Count == 0) return 0;
                return lstLines[0].bmp.Height;
            }
        }

        int TotalHeight
        {
            get 
            {
                if (lstLines.Count == 0) return 0;
                return lstLines[lstLines.Count - 1].rec.Bottom;
            }
        }

        int SliderHeight
        {
            get { return (int)(Math.Pow(Height, 2) / TotalHeight); }
        }

        int SliderTop
        {
            get { return (int)((Height - SliderHeight) * (cScrollBar.Value * LineHeight) / (TotalHeight - Height)); }
        }

        int intScrollBarWidth = 10;

        Rectangle _recSlider = new Rectangle();
        Rectangle recSlider
        {
            get
            {
                Size szSlider = new Size(intScrollBarWidth, SliderHeight);
                Point ptSlider = new Point(Width - intScrollBarWidth, SliderTop);
                _recSlider = new Rectangle(ptSlider, szSlider);
                return _recSlider;
            }
        }


        public override string Text
        {
            get { return strText; }
            set
            {
                strText = value.Replace("\t", "   ");
                Reset();
            }
        }

        void Parse_Text()
        {
            if (Text.Length == 0) return;
            lstLines.Clear();
            int intLineCut = 0;
            int intCharCounter = 0;
            int int_Start = 0;

            char[] chrSplit = { '\r', '\n' };
            List<string> lstText = Text.Split(chrSplit).ToList<string>();
            while (lstText.Contains(""))
                lstText.Remove("");

            int intMaxWidth = Width - cScrollBar.Thickness;
            string strLine = "";
            for (int intTextCounter = 0; intTextCounter < lstText.Count; intTextCounter++)
            {
                string strTemp = lstText[intTextCounter];
                intCharCounter = 0;
                int_Start = 0;
                strLine = "";
                while (intCharCounter < strTemp.Length)
                {
                    char chr = strTemp[intCharCounter];

                    strLine += chr;

                    Size szLine = System.Windows.Forms.TextRenderer.MeasureText(strLine, Font);

                    if ( szLine.Width > intMaxWidth)
                    {
                        // reached end of line
                        if (intCharCounter < int_Start + 2)
                            return;

                        if (intLineCut == int_Start) // cut word at last char that fits if no line-cut has been found
                            intLineCut = intCharCounter - 1;
                        int intLength = intLineCut - int_Start;
                        if (int_Start + intLineCut > strTemp.Length)
                            intLength = strTemp.Length - int_Start;

                        strLine = strTemp.Substring(int_Start, intLength);
                        classLine cLineNew = new classLine(ref cMyRef, strLine);
                        cLineNew.rec = new Rectangle(lstLines.Count > 0
                                                                   ? new Point(0, lstLines[lstLines.Count - 1].rec.Bottom)
                                                                   : new Point(0, 0),
                                                     cLineNew.Size);
                        lstLines.Add(cLineNew);

                        int_Start
                            = intCharCounter
                            = intLineCut;

                    
                        strLine = "";
                    }

                    if (!char.IsLetterOrDigit(chr) && !".,!?:;".Contains(chr))
                        intLineCut = intCharCounter;

                    intCharCounter++;
                }

                if (int_Start < strTemp.Length)
                {
                    classLine cLine = new classLine(ref cMyRef, strTemp.Substring(int_Start));

                    cLine.rec = new Rectangle(lstLines.Count > 0
                                                           ? new Point(0, lstLines[lstLines.Count - 1].rec.Bottom)
                                                           : new Point(0, 0),
                                              cLine.Size);
                    lstLines.Add(cLine);
                }
            }

            int intMaxLinesVisible = (Height / (lstLines[0].bmp.Height));

            if (lstLines.Count == 0)
                cScrollBar.Max = 0;
            else
                cScrollBar.Max = lstLines.Count - intMaxLinesVisible;
            cScrollBar.Value = 0;
            cScrollBar.LargeChange = intMaxLinesVisible;
            cEle.NeedsToBeRedrawn = true;

        }

        void cScrollBar_ValueChanged(object sender, EventArgs e) 
        {
            _MyImage = null;
            cEle.NeedsToBeRedrawn = true;
            //Draw(); 
        }

        void Event_SizeChanged(object sender, EventArgs e) { Reset(); }

        public void Event_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta < 0)
                cScrollBar.ScrollDown();
            else
                cScrollBar.ScrollUp();
        }
        public void Event_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            if (ptMouse.X > cScrollBar.recSliderDraw.Left && ptMouse.X < cScrollBar.recSliderDraw.Right)
            {
                if (ptMouse.Y < cScrollBar.recSliderDraw.Top)
                {
                    cScrollBar.StepUp();
                }
                else if (ptMouse.Y > cScrollBar.recSliderDraw.Bottom)
                {
                    cScrollBar.StepDown();
                }
                else
                {
                    cScrollBar.ptGrab = new Point(cScrollBar.recSliderDraw.Left - ptMouse.X,
                                                 cScrollBar.recSliderDraw.Top - ptMouse.Y);
                    cScrollBar.Grabbed = true;
                }
            }
        }

        public void Event_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            cScrollBar.Grabbed = false;
        }

        public void Event_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(sender, e);
        }

        public void Event_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ptMouse = new Point(e.X, e.Y);
            if (cScrollBar.Grabbed)
            {
                cScrollBar.Value = (int)(cScrollBar.Max * (ptMouse.Y - cScrollBar.ptGrab.Y) / (Height - cScrollBar.recSliderDraw.Height));
            }
        }

        public class classLine
        {
            TextDisplay txtDisplay = null;
     
            public classLine(ref TextDisplay txtDisplay, string strText)
            {
                this.txtDisplay = txtDisplay;
                this.strText = strText;
            }

            Rectangle _rec = new Rectangle();
            public Rectangle rec
            {
                get { return _rec; }
                set { _rec = value; }
            }

            public bool LineContainsPoint(Point pt)
            {
                return pt.X > rec.Left && pt.X < rec.Right 
                        && pt.Y > rec.Top - txtDisplay.lstLinesDrawn[0].rec.Top  
                        && pt.Y < rec.Bottom - txtDisplay.lstLinesDrawn[0].rec.Top;
            }

            Size _sz = new Size(0, 0);
            public Size Size
            {
                get
                {
                    if (_sz.Width ==0)
                        _sz = System.Windows.Forms.TextRenderer.MeasureText(Text, txtDisplay.Font);
                    return _sz;
                }
            }

            /// <summary>
            /// refers to its index in the lstLines of parent TextDisplay
            /// </summary>
            public int Index { get { return txtDisplay.lstLines.IndexOf(this); } }

            string strText = "";
            public string Text { get { return strText; } }
           
            
            Bitmap _bmp = null;
            public Bitmap bmp
            {
                get
                {
                    if (_bmp == null)
                    {
                        System.Drawing.Size sz = System.Windows.Forms.TextRenderer.MeasureText(strText, txtDisplay.Font);
                        _bmp = new Bitmap(sz.Width + 10, sz.Height);
                        using (Graphics g = Graphics.FromImage(_bmp))
                        {
                            g.FillRectangle(txtDisplay.sbrBackColor, new RectangleF(0, 0, _bmp.Width, _bmp.Height));
                            g.DrawString(strText, txtDisplay.Font, new SolidBrush(txtDisplay.ForeColor), new Point(0, 0));
                        }
                    }
                    return _bmp;
                }
            }

            public void FreeBitmap()
            {
                _bmp.Dispose();
                _bmp = null;
            }
        }

    }


    public class GroupBox : SweepPrune_Object
    {
        public int intGap = 4;

        public GroupBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            GroupBox cMyRef = this;
            SPContainer.Add(ref cMyRef);
        }

        bool bolFrameIsDrawn = true;
        public virtual bool FrameIsDrawn
        {
            get { return bolFrameIsDrawn; }
            set { bolFrameIsDrawn = value; }
        }

        bool bolTextIsDrawn = true;
        public bool TextIsDrawn
        {
            get { return bolTextIsDrawn; }
            set { bolTextIsDrawn = value; }
        }

        public void DrawFrame(ref Graphics g)
        {
            Pen pBorder = new Pen(ForeColor_Dull, 2);
            Size szText = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
            Rectangle recText = new Rectangle(new Point(2 * intGap, 0), szText);
            Point ptLineEnd = new Point();
            List<Point> lstCorners = new List<Point>();
            lstCorners.Add(new Point(Width - 2 * intGap, 2 * intGap));               // TR   
            lstCorners.Add(new Point(Width - 2 * intGap, Height - 2 * intGap));     // BR
            lstCorners.Add(new Point(2 * intGap, Height - 2 * intGap));             // BL
            lstCorners.Add(new Point(2 * intGap, 2 * intGap));                      // TL

            Point ptLineStart = TextIsDrawn
                                    ? new Point(2 * intGap + recText.Right, intGap)
                                    : new Point(2 * intGap, intGap);

            for (double dblTheta = 0; dblTheta < 2 * Math.PI; dblTheta += (Math.PI / 2))
            {
                Point ptCorner = lstCorners[0];
                lstCorners.RemoveAt(00);
                ptLineEnd = classSPMath.AddTwoPoints(ptCorner, (new classRadialCoordinate(dblTheta - Math.PI / 2, intGap)).toPoint());

                g.DrawLine(pBorder, ptLineStart, ptLineEnd);
                g.DrawArc(pBorder,
                          new RectangleF(ptCorner.X - intGap,
                                         ptCorner.Y - intGap,
                                         2 * intGap,
                                         2 * intGap),
                          (float)((dblTheta - Math.PI / 2) * (180f / Math.PI)),
                          (float)((Math.PI / 2) * (180f / Math.PI)));

                ptLineStart = classSPMath.AddTwoPoints(ptCorner, (new classRadialCoordinate(dblTheta, intGap)).toPoint());
            }
        }

        public void DrawText(ref Graphics g)
        {
            Size szText = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
            Rectangle recText = new Rectangle(new Point(2 * intGap, 0), szText);
            g.DrawString(Text, Font, new SolidBrush(ForeColor_Dull), recText.Location);
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            Bitmap bmpTemp = new Bitmap(recSPArea.Width, recSPArea.Height);
            Graphics g = Graphics.FromImage(bmpTemp);
            {
                if (BackgroundImage != null)
                    g.DrawImage(BackgroundImage, new Point(0, 0));
                else
                    g.FillRectangle(sbrBackColor, new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));

                //Pen pBorder = new Pen(ForeColor_Dull, 2);

                Size szText = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
                Rectangle recText = new Rectangle(new Point(2 * intGap, 0), szText);

                if (TextIsDrawn)
                    DrawText(ref g);


                if (FrameIsDrawn)
                    DrawFrame(ref g);


                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }

            }
            g.Dispose();
            if (BackTransSPContainer)
                bmpTemp.MakeTransparent(BackColor);

            //bool bolDebug = false;
            //if (bolDebug)
            //    bmpTemp.Save(@"c:\debug\bmpTemp.bmp");

            MyImage = bmpTemp;
        }
    }

    public class getPoint : GroupBox
    {
        Label lblX = null;
        Label lblY = null;
        public numericUpDown nudX = null;
        public numericUpDown nudY = null;

        public getPoint(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            cEle.obj = (object)this;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.getPoint;
            cEle.Name = "getPoint";

            Backcolor_Change = GrbBackcolorChanged;
            Forecolor_Change = GrbForecolorChanged;

            lblX = new Label(ref SPContainer);
            lblX.cEle.Name = "lblX";

            lblY = new Label(ref SPContainer);
            lblY.cEle.Name = "lblY";

            lblX.AutoSize
                = lblY.AutoSize
                = true;

            lblX.Text = "X";
            lblY.Text = "Y";

            nudX = new numericUpDown(ref SPContainer);
            nudX.cEle.Name = "nudX";

            nudY = new numericUpDown(ref SPContainer);
            nudY.cEle.Name = "nudY";

            nudX.Value
                = nudY.Value
                = 10;
            nudX.ValueChanged += grbValueChanged;
            nudY.ValueChanged += grbValueChanged;

            cEle.lstEle.Add(ref nudY.cEle);
            cEle.lstEle.Add(ref nudX.cEle);
            cEle.lstEle.Add(ref lblY.cEle);
            cEle.lstEle.Add(ref lblX.cEle);

            placeObjects();
            Height = nudY.Bottom + lblX.Top;

            Sizechanged = GrbSizeChanged;
            LocationChanged = GrbLocationChanged;
        }
        #region values
        public Point Value
        {
            get
            {
                if (nudX != null && nudY != null)
                    return new Point(nudX.Value, nudY.Value);
                else return new Point(0, 0);
            }
            set
            {
                nudX.Value = value.X;
                nudY.Value = value.Y;
                if (cEle.Parent != null)
                    cEle.Parent.NeedsToBeRedrawn = true;
                cEle.NeedsToBeRedrawn = true;
            }
        }

        public Point Min
        {
            get
            {
                if (nudX != null && nudY != null)
                    return new Point(nudX.Min, nudY.Min);
                else return new Point(0, 0);
            }
            set
            {
                nudX.Min = value.X;
                nudY.Min = value.Y;
            }
        }

        public Point Max
        {
            get
            {
                if (nudX != null && nudY != null)
                    return new Point(nudX.Max, nudY.Max);
                else
                    return new Point(0, 0);
            }
            set
            {
                nudX.Max = value.X;
                nudY.Max = value.Y;
            }
        }


        public int X_Min
        {
            get
            {
                if (nudX != null)
                    return nudX.Min;
                else return 0;
            }
            set
            {
                nudX.Min = value;
            }
        }

        public int X_Max
        {
            get
            {
                if (nudX != null)
                    return nudX.Max;
                else
                    return X_Min + 1;
            }
            set
            {
                nudX.Max = value;
            }
        }

        public int Y_Min
        {
            get
            {
                if (nudY != null)
                    return nudY.Min;
                else
                    return 0;
            }
            set
            {
                nudY.Min = value;
            }
        }

        public int Y_Max
        {
            get
            {
                if (nudY != null)
                    return nudY.Max;
                else
                    return Y_Min + 1;
            }
            set
            {
                nudY.Max = value;
            }
        }
        public int X
        {
            get { return (int)nudX.Value; }
            set
            {
                nudX.Value = value;
            }
        }

        public int Y
        {
            get { return (int)nudY.Value; }
            set { nudY.Value = value; }
        }
        #endregion

        #region events
        EventHandler _ValueChanged = null;
        public EventHandler ValueChanged
        {
            get { return _ValueChanged; }
            set { _ValueChanged = value; }
        }

        void grbValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged((object)this, e);
        }

        void GrbSizeChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void GrbLocationChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }


        void GrbForecolorChanged(object sender, EventArgs e)
        {
            lblX.ForeColor
                = lblY.ForeColor
                = nudX.ForeColor
                = nudY.ForeColor
                = ForeColor;
        }

        void GrbBackcolorChanged(object sender, EventArgs e)
        {
            lblX.BackColor
                = lblY.BackColor
                = nudX.BackColor
                = nudY.BackColor
                = BackColor;
        }
        #endregion 

        #region dimensions
        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(value, cEle.rec.Height));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(cEle.rec.Width, value));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, value);
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }
        #endregion

        public int NudWidth_Max()
        {
            int intnudX = nudX.intNumPlaces * nudX.szChar.Width;
            int intnudY = nudY.intNumPlaces * nudY.szChar.Height;
            return intnudX > intnudY
                               ? intnudX
                               : intnudY;
        }
        public void placeObjects()
        {
            lblX.Location = new Point(2 * intGap, 4 * intGap);
            nudX.Location = new Point(lblX.Right, lblX.Top);

            lblY.Location = new Point(lblX.Left, lblX.Bottom);
            nudY.Location = new Point(lblY.Right, lblY.Top);
            nudX.Size
                = nudY.Size
                = new Size(Width - nudX.Left - 2 * intGap,
                           lblX.Height);

            cEle.DrawSequence_Set();
            SPContainer.Reset();
        }

    }



    public class ComboBox : Button
    {
        SPObjects.ListBox lbx = null;
        public ComboBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            cEle.obj = (object)this;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.ComboBox;
            cEle.Name = "ComboBox";

            Backcolor_Change = pnlBackcolorChanged;
            Forecolor_Change = pnlForecolorChanged;

            bmpDull = DropDownImage;
            bmpHighlight = DropDownImage_Highlight;

            MouseClick = btn_MouseClick;
            TextAlignment = ContentAlignment.TopLeft;

            lbx = new ListBox(ref SPContainer);
            lbx.DrawBorder = true;
            lbx.SelectedIndexChanged = lbxSelectedIndexChanged;
            lbx.MouseLeave = lbxMouseLeave;

            Collapse();

            Sizechanged = pnlSizeChanged;
            LocationChanged = pnlLocationChanged;
        }

        #region Properties

        public ItemsHandler Items
        {
            get { return lbx.Items; }
        }

        Bitmap _bmpDropDownImage = null;
        public Bitmap DropDownImage
        {
            get
            {
                if (_bmpDropDownImage == null)
                {
                    _bmpDropDownImage = new Bitmap(Width, Height);
                    using (Graphics g = Graphics.FromImage(_bmpDropDownImage))
                    {
                        g.FillRectangle(new SolidBrush(BackColor), new RectangleF(0, 0, _bmpDropDownImage.Width, _bmpDropDownImage.Height));
                        Point ptCenter = new Point(_bmpDropDownImage.Width - 12, _bmpDropDownImage.Height / 2);
                        int intGap = 6;
                        Pen p = new Pen(ForeColor, 2);
                        g.DrawLine(p, new Point(ptCenter.X - intGap, ptCenter.Y - intGap), ptCenter);
                        g.DrawLine(p, new Point(ptCenter.X + intGap, ptCenter.Y - intGap), ptCenter);
                    }
                }
                return _bmpDropDownImage;
            }
            set { _bmpDropDownImage = value; }
        }

        Bitmap _bmpDropDownImage_Highlight = null;
        public Bitmap DropDownImage_Highlight
        {
            get
            {
                if (_bmpDropDownImage_Highlight == null)
                {
                    _bmpDropDownImage_Highlight = new Bitmap(Width, Height);
                    using (Graphics g = Graphics.FromImage(_bmpDropDownImage_Highlight))
                    {
                        g.FillRectangle(new SolidBrush(ForeColor), new RectangleF(0, 0, _bmpDropDownImage_Highlight.Width, _bmpDropDownImage_Highlight.Height));
                        Point ptCenter = new Point(_bmpDropDownImage_Highlight.Width - 12, _bmpDropDownImage_Highlight.Height / 2);
                        int intGap = 6;
                        Pen p = new Pen(BackColor, 2);
                        g.DrawLine(p, new Point(ptCenter.X - intGap, ptCenter.Y - intGap), ptCenter);
                        g.DrawLine(p, new Point(ptCenter.X + intGap, ptCenter.Y - intGap), ptCenter);
                    }
                }
                return _bmpDropDownImage_Highlight;
            }
            set { _bmpDropDownImage_Highlight = value; }
        }

        public int SelectedIndex
        {
            get { return lbx.SelectedIndex; }
            set { lbx.SelectedIndex = value; }
        }

        public string SelectedItem
        {
            get { return lbx.SelectedItem; }
        }

        bool bolDrawLbxAboveBtn = false;
        public bool DrawLbxAboveBtn
        {
            get { return bolDrawLbxAboveBtn; }
            set { bolDrawLbxAboveBtn = value; }
        }

        bool bolExpanded = false;
        bool Expanded
        {
            get { return bolExpanded; }
            set
            {
                //if (bolExpanded != value)

                if (value)
                    Expand();
                else
                    Collapse();
                bolExpanded = value;
                placeObjects();

            }
        }
        #endregion 

        #region events

        EventHandler event_SelectedIndexChanged = null;
        public EventHandler SelectedIndexChanged
        {
            get { return event_SelectedIndexChanged; }
            set { event_SelectedIndexChanged = value; }
        }
        void btn_MouseClick(object sender, EventArgs e)
        {
            Expanded = !Expanded;
        }
        void lbxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbx.SelectedIndex >= 0 && lbx.SelectedIndex < lbx.Items.Count)
            {
                Text = lbx.Items.Get(lbx.SelectedIndex);
                if (event_SelectedIndexChanged != null)
                    event_SelectedIndexChanged((object)this, e);
                Expanded = false;
            }
        }

        void lbxMouseLeave(object sender, EventArgs e)
        {
            Expanded = false;
        }

        void pnlSizeChanged(object sender, EventArgs e)
        {
            drawBackgroundImages();

            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void pnlLocationChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void pnlForecolorChanged(object sender, EventArgs e)
        {
            lbx.ForeColor
                = ForeColor;
            drawBackgroundImages();
        }

        void pnlBackcolorChanged(object sender, EventArgs e)
        {
            lbx.BackColor
                = BackColor;
            drawBackgroundImages();
        }
        #endregion 

        #region dimensions
        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(value, cEle.rec.Height));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(cEle.rec.Width, value));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, value);
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }
        #endregion

        void drawBackgroundImages()
        {
            _bmpDropDownImage = null;
            _bmpDropDownImage_Highlight = null;
            bmpDull = DropDownImage;
            bmpHighlight = DropDownImage_Highlight;
        }

        void Expand()
        {
            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();
            {
                int intSpaceAbove = recDraw.Top;
                int intSpaceBelow = SPContainer.recVisible.Height - recDraw.Bottom;

                int intLbxHeight = (lbx.Items.Count) * lbx.ItemHeight;
                int intStandardLbxHeight = 200;
                if (intLbxHeight > intStandardLbxHeight)
                    intLbxHeight = intStandardLbxHeight;

                if (intSpaceAbove > intSpaceBelow)
                {
                    // expand LBX above BTN
                    if (intSpaceAbove < intLbxHeight)
                        intLbxHeight = intSpaceAbove;

                    lbx.Location = new Point(recDraw.Left, recDraw.Bottom - lbx.Height);
                    lbx.Height = intLbxHeight;
                    DrawLbxAboveBtn = true;
                }
                else
                {
                    // expand LBX below BTN
                    if (intSpaceBelow < intLbxHeight)
                        intLbxHeight = intSpaceBelow;

                    lbx.Location = new Point(Left, Bottom);
                    lbx.Height = intLbxHeight;
                    DrawLbxAboveBtn = false;
                }
            }
            lbx.BringToFront();
            lbx.Show();

            if (!bolBuilding)
                SPContainer.Building_Complete();
        }

        void Collapse()
        {
            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();
            {
                lbx.Visible = false;
            }

            if (!bolBuilding)
                SPContainer.Building_Complete();

            DrawLbxAboveBtn = false;
        }

        void placeObjects()
        {
            bool bolBuilding = SPContainer.BuildingInProgress;
            SPContainer.Building_Start();

            {
                lbx.Width = Width;
                lbx.Left = Left;
                lbx.Top = DrawLbxAboveBtn
                                    ? recDraw.Top - lbx.Height
                                    : Bottom;
            }
            if (!bolBuilding)
                SPContainer.Building_Complete();

        }
    }

    public class getSize : GroupBox
    {
        Label lblWidth = null;
        Label lblHeight = null;
        numericUpDown nudWidth = null;
        numericUpDown nudHeight = null;

        public getSize(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            cEle.obj = (object)this;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.getSize;
            cEle.Name = "getSize";

            Backcolor_Change = GrbBackcolorChanged;
            Forecolor_Change = GrbForecolorChanged;

            lblWidth = new Label(ref SPContainer);
            lblWidth.cEle.Name = "lblWidth";

            lblHeight = new Label(ref SPContainer);
            lblHeight.cEle.Name = "lblHeight";

            lblWidth.AutoSize
                = lblHeight.AutoSize
                = true;

            lblWidth.Text = "Width";
            lblHeight.Text = "Height";

            nudWidth = new numericUpDown(ref SPContainer);
            nudWidth.Min = 1;
            nudWidth.cEle.Name = "nudWidth";

            nudHeight = new numericUpDown(ref SPContainer);
            nudHeight.Min = 1;
            nudHeight.cEle.Name = "nudHeight";

            nudWidth.Value
                = nudHeight.Value
                = 10;
            nudWidth.ValueChanged += grbValueChanged;
            nudHeight.ValueChanged += grbValueChanged;

            cEle.lstEle.Add(ref nudHeight.cEle);
            cEle.lstEle.Add(ref nudWidth.cEle);
            cEle.lstEle.Add(ref lblHeight.cEle);
            cEle.lstEle.Add(ref lblWidth.cEle);

            placeObjects();
            Height = nudHeight.Bottom + lblWidth.Top;

            Sizechanged = GrbSizeChanged;
            LocationChanged = GrbLocationChanged;
        }

        #region values
        public Size Value
        {
            get
            {
                if (nudHeight != null && nudWidth != null)
                    return new Size(nudWidth.Value, nudHeight.Value);
                else
                    return new Size();
            }
            set
            {
                if (nudHeight != null && nudWidth != null)
                {
                    if (nudWidth.Value != value.Width || nudHeight.Value != value.Height)
                    {
                        nudWidth.Value = value.Width;
                        nudHeight.Value = value.Height;
                        cEle.NeedsToBeRedrawn = true;
                        if (cEle.Parent != null)
                            cEle.Parent.NeedsToBeRedrawn = true;
                    }
                }

            }
        }

        public Size Min
        {
            get
            {
                if (nudHeight != null && nudWidth != null)
                    return new Size(nudWidth.Min, nudHeight.Min);
                else
                    return new Size();
            }
            set
            {
                nudWidth.Min = value.Width;
                nudHeight.Min = value.Height;
            }
        }

        public Size Max
        {
            get
            {
                if (nudHeight != null && nudWidth != null)
                    return new Size(nudWidth.Max, nudHeight.Max);
                else return new Size();
            }
            set
            {
                nudWidth.Max = value.Width;
                nudHeight.Max = value.Height;
            }
        }

        public int Width_Min
        {
            get
            {
                if (nudWidth != null)
                    return nudWidth.Min;
                else
                    return 0;
            }
            set
            {
                nudWidth.Min = value;
            }
        }

        public int Width_Max
        {
            get
            {

                if (nudWidth != null)
                    return nudWidth.Max;
                else
                    return 0;
            }
            set
            {
                nudWidth.Max = value;
            }
        }

        public int Height_Min
        {
            get
            {
                if (nudHeight != null)
                    return nudHeight.Min;
                else
                    return 0;
            }
            set
            {
                nudHeight.Min = value;
            }
        }

        public int Height_Max
        {
            get
            {
                if (nudHeight != null)
                    return nudHeight.Max;
                else return 0;

            }
            set
            {
                nudHeight.Max = value;
            }
        }

        #endregion

        #region events

        EventHandler _ValueChanged = null;
        public EventHandler ValueChanged
        {
            get { return _ValueChanged; }
            set { _ValueChanged = value; }
        }

        void grbValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged((object)this, e);
        }

        void GrbSizeChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void GrbLocationChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void GrbForecolorChanged(object sender, EventArgs e)
        {
            lblWidth.ForeColor
                = lblHeight.ForeColor
                = nudWidth.ForeColor
                = nudHeight.ForeColor
                = ForeColor;
        }

        void GrbBackcolorChanged(object sender, EventArgs e)
        {
            lblWidth.BackColor
                = lblHeight.BackColor
                = nudWidth.BackColor
                = nudHeight.BackColor
                = BackColor;
        }

        #endregion

        #region dimensions
        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(value, cEle.rec.Height));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, new Size(cEle.rec.Width, value));
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }

        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                cEle.rec = new Rectangle(cEle.rec.Location, value);
                placeObjects();
                if (Sizechanged != null)
                    Sizechanged((object)this, new EventArgs());
                cEle.NeedsToBeRedrawn = true;
                cEle.SPContainer.Reset();
            }
        }
        #endregion 

        public int NudWidth_Max()
        {
            int intNudWidth = nudWidth.intNumPlaces * nudWidth.szChar.Width;
            int intNudHeight = nudHeight.intNumPlaces * nudHeight.szChar.Height;
            return intNudWidth > intNudHeight
                               ? intNudWidth
                               : intNudHeight;
        }

        public void placeObjects()
        {
            lblWidth.Location = new Point(2 * intGap, 4 * intGap);
            nudWidth.Location = new Point(lblWidth.Right, lblWidth.Top);

            lblHeight.Location = new Point(lblWidth.Left, lblWidth.Bottom);
            nudHeight.Location = new Point(lblHeight.Right, lblHeight.Top);
            nudWidth.Size = new Size(Width - nudWidth.Left - 2 * intGap,
                                    lblWidth.Height);
            nudHeight.Size = new Size(Width - nudHeight.Left - 2 * intGap,
                                      lblHeight.Height);
            lblWidth.cEle.NeedsToBeRedrawn
                = lblHeight.cEle.NeedsToBeRedrawn
                = nudWidth.cEle.NeedsToBeRedrawn
                = nudHeight.cEle.NeedsToBeRedrawn
                = cEle.NeedsToBeRedrawn
                = true;
            cEle.DrawSequence_Set();
            SPContainer.Reset();
        }
    }

    public class getRectangle : GroupBox
    {
        public getPoint getPoint = null;
        public getSize getSize = null;

        public getRectangle(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            cEle.obj = (object)this;
            eSP_ObjectType
                = cEle.eSP_ObjectType
                = enuSweepPrune_ObjectType.getRectangle;
            cEle.Name = "getRectangle";

            Backcolor_Change = GrbBackcolorChanged;
            Forecolor_Change = GrbForecolorChanged;

            getPoint = new getPoint(ref SPContainer);
            getPoint.cEle.Name = "getPoint";
            getPoint.BackTransSPContainer = true;

            getSize = new getSize(ref SPContainer);
            getSize.cEle.Name = "getSize";
            getSize.BackTransSPContainer = true;

            getPoint.Text = "Location";
            getSize.Text = "Size";

            getPoint.ValueChanged += grbValueChanged;
            getSize.ValueChanged += grbValueChanged;

            cEle.lstEle.Add(ref getSize.cEle);
            cEle.lstEle.Add(ref getPoint.cEle);

            DrawSequence_Set();
            placeObjects();

            Sizechanged = GrbSizeChanged;
            LocationChanged = GrbLocationChanged;
        }

        #region Values
        public Rectangle Value
        {
            get { return new Rectangle(getPoint.Value, getSize.Value); }
            set
            {
                bool bolSPContainer_Building = SPContainer.BuildingInProgress;
                if (!bolSPContainer_Building)
                    SPContainer.Building_Start();
                getPoint.Value = value.Location;
                getSize.Value = value.Size;
                if (cEle.Parent != null)
                    cEle.Parent.NeedsToBeRedrawn = true;
                cEle.NeedsToBeRedrawn = true;
                if (!bolSPContainer_Building)
                    SPContainer.Building_Complete();
            }
        }


        public int X_Min
        {
            get { return getPoint.X_Min; }
            set
            {
                getPoint.X_Min = value;
            }
        }


        public int X_Max
        {
            get { return getPoint.X_Max; }
            set
            {
                getPoint.X_Max = value;
            }
        }
        public int X
        {
            get { return (int)getPoint.Value.X; }
            set
            {
                getPoint.Value = new Point(value, getPoint.Value.Y);
            }
        }


        public int Y_Min
        {
            get { return getPoint.Y_Min; }
            set
            {
                getPoint.Y_Min = value;
            }
        }


        public int Y_Max
        {
            get { return getPoint.Y_Max; }
            set
            {
                getPoint.Y_Max = value;
            }
        }

        public int Y
        {
            get { return (int)getPoint.Location.Y; }
            set { getPoint.Location = new Point(getPoint.X, value); }
        }


        public int Width_Min
        {
            get { return getSize.Width_Min; }
            set
            {
                getSize.Width_Min = value;
            }
        }

        public int Width_Max
        {
            get { return getSize.Width_Max; }
            set
            {
                getSize.Width_Max = value;
            }
        }

        public int Width_Value
        {
            get { return (int)getSize.Value.Width; }
            set
            {
                getSize.Value = new Size(value, getSize.Value.Height);
            }
        }


        public int Height_Min
        {
            get { return getSize.Height_Min; }
            set
            {
                getSize.Height_Min = value;
            }
        }


        public int Height_Max
        {
            get { return getSize.Height_Max; }
            set
            {
                getSize.Height_Max = value;
            }
        }
        public int Height_Value
        {
            get { return (int)getSize.Value.Height; }
            set { getSize.Value = new Size(getSize.Value.Width, value); }
        }

        #endregion

        #region events

        void GrbForecolorChanged(object sender, EventArgs e)
        {
            getPoint.ForeColor
                = getSize.ForeColor
                = ForeColor;
        }

        void GrbBackcolorChanged(object sender, EventArgs e)
        {
            getPoint.BackColor
                = getSize.BackColor
                = BackColor;
        }

        EventHandler _ValueChanged = null;
        public EventHandler ValueChanged
        {
            get { return _ValueChanged; }
            set { _ValueChanged = value; }
        }

        void grbValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged((object)this, e);
        }

        void GrbSizeChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        void GrbLocationChanged(object sender, EventArgs e)
        {
            placeObjects();
            cEle.NeedsToBeRedrawn = true;
        }

        #endregion

        #region dimensions
        public override int Width
        {
            get
            {
                bool bolDebug = false;
                if (bolDebug)
                    return -1;
                return cEle.rec.Width;
            }
            set
            {
                Size = new Size(value, cEle.rec.Height);
            }
        }

        public override int Height
        {
            get { return cEle.rec.Height; }
            set
            {
                Size = new Size(cEle.rec.Width, value);
            }
        }

        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                if (cEle.rec.Width != value.Width || cEle.rec.Height != value.Height)
                {
                    cEle.rec = new Rectangle(cEle.rec.Location, value);
                    placeObjects();
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                }
            }
        }

        #endregion 

        bool bolFrameIsDrawn = true;
        public override bool FrameIsDrawn
        {
            get { return bolFrameIsDrawn; }
            set
            {
                if (bolFrameIsDrawn != value)
                {
                    bolFrameIsDrawn = value;
                    placeObjects();
                }
            }
        }

        int BoarderGap
        {
            get { return FrameIsDrawn ? intGap : 0; }
        }

        public void placeObjects()
        {
            getPoint.Location = new Point(BoarderGap, 2 * intGap + 2);
            int intTotalWidth = (Width - 2 * BoarderGap);
            int intPointMinWidth = getPoint.NudWidth_Max() + System.Windows.Forms.TextRenderer.MeasureText("X", getPoint.Font).Width;
            int intSizeMinWidth = getSize.NudWidth_Max() + System.Windows.Forms.TextRenderer.MeasureText("Width", getSize.Font).Width;
            int intExtra = intTotalWidth - intPointMinWidth - intSizeMinWidth;
            getPoint.Width = intPointMinWidth + intExtra / 2;
            getSize.Width = intSizeMinWidth + intExtra / 2;

            getPoint.Height
                = getSize.Height
                = Height - getPoint.Top - BoarderGap;

            getSize.Location = new Point(getPoint.Right, getPoint.Top);

            getPoint.cEle.NeedsToBeRedrawn
                = getSize.cEle.NeedsToBeRedrawn
                = cEle.NeedsToBeRedrawn
                = true;
            DrawSequence_Set();
            SPContainer.Reset();
        }

        public void DrawSequence_Set()
        {
            cEle.DrawSequence_Set();
            getPoint.cEle.DrawSequence_Set();
            getSize.cEle.DrawSequence_Set();
        }

    }

    public class TrackBar : SweepPrune_Object
    {
        public TrackBar(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            TrackBar cMyRef = this;
            SPContainer.Add(ref cMyRef);

        }

        public override Size Size
        {
            get { return cEle.rec.Size; }
            set
            {
                if (cEle.rec.Width != value.Width || cEle.rec.Height != value.Height)
                {
                    cEle.rec = new Rectangle(cEle.rec.Location, value);
                    if (Sizechanged != null)
                        Sizechanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                    cEle.SPContainer.Reset();
                    _bmpHashBoard = null;
                }
            }
        }

        Bitmap _bmpSliderH = null;
        Bitmap bmpSliderH
        {
            get
            {
                if (_bmpSliderH == null)
                {
                    Color clrTransparent = Color.FromArgb(0, 1, 2, 3);
                    int intWidth = 12;
                    int intHeight = 30;
                    _bmpSliderH = new Bitmap(intWidth, intHeight);
                    using (Graphics g = Graphics.FromImage(_bmpSliderH))
                    {
                        g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, intWidth, intHeight));
                        int intLineThickness = 2;
                        Point[] pts =
                        {
                            new Point(0,1),
                            new Point(intWidth - intLineThickness, 1),
                            new Point(intWidth - intLineThickness, intHeight - intWidth/2),
                            new Point((intWidth -intLineThickness)/2, intHeight - intLineThickness),
                            new Point(0, intHeight - intWidth/2)
                        };
                        g.FillPolygon(new SolidBrush(BackColor), pts);
                        g.DrawPolygon(new Pen(ForeColor), pts);
                    }
                    bmpSliderH.MakeTransparent(clrTransparent);
                }
                return _bmpSliderH;
            }
        }

        Bitmap bmpSlider
        {
            get
            {
                switch (eStyle)
                {
                    case enuTrackBar_Style.Vertical:
                        {
                            Bitmap bmpSliderV = new Bitmap(bmpSliderH);
                            bmpSliderV.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            return bmpSliderV;
                        }

                    default:
                    case enuTrackBar_Style.Horizontal:
                        return new Bitmap(bmpSliderH);
                }
            }
        }

        Bitmap _bmpHashBoard = null;
        Bitmap bmpHashBoard
        {
            get
            {
                if (_bmpHashBoard == null)
                {
                    _bmpHashBoard = new Bitmap(recSPArea.Width, recSPArea.Height);
                    using (Graphics g = Graphics.FromImage(_bmpHashBoard))
                    {
                        if (BackgroundImage != null)
                            g.DrawImage(BackgroundImage, new Point(0, 0));
                        else
                            g.FillRectangle(sbrBackColor, new RectangleF(0, 0, _bmpHashBoard.Width, _bmpHashBoard.Height));

                        int intEdge = 5;
                        int intHash_Small = 5;
                        int intHash_Medium = 10;
                        int intHash_Large = 15;
                        int intHash_Ends = 20;

                        Pen pHash = new Pen(ForeColor);

                        switch (eStyle)
                        {
                            case enuTrackBar_Style.Vertical:
                                {

                                }
                                break;

                            default:
                            case enuTrackBar_Style.Horizontal:
                                {
                                    Point ptBar0 = new Point(intEdge, intEdge);
                                    Point ptBar1 = new Point(Width - intEdge, intEdge);
                                    ptBar_Ends[0] = ptBar0;
                                    ptBar_Ends[1] = ptBar1;

                                    // draw bar 
                                    g.DrawLine(pHash, ptBar0, ptBar1);

                                    float fltBarLength = ptBar1.X - ptBar0.X;

                                    // calculate max hash marks & round down to the nearest 10
                                    float fltNumHash = (((int)fltBarLength / intEdge) / 10) * 10;

                                    // draw left-most
                                    Point ptTop = new Point(ptBar0.X, ptBar0.Y - intHash_Small);
                                    Point ptBottom = new Point(ptBar0.X, ptBar0.Y + intHash_Small + intHash_Ends);
                                    g.DrawLine(pHash, ptTop, ptBottom);

                                    // draw right-most
                                    ptTop.X = ptBar1.X;
                                    ptBottom.X = ptBar1.X;
                                    g.DrawLine(pHash, ptTop, ptBottom);

                                    ptTop.Y = ptBar0.Y + intHash_Small;
                                    for (float fltHashCounter = 1; fltHashCounter < fltNumHash; fltHashCounter++)
                                    {
                                        ptTop.X
                                            = ptBottom.X
                                            = (int)(fltBarLength * (1 + fltHashCounter) / fltNumHash);
                                        ptBottom.Y = ptTop.Y
                                                        + (fltHashCounter % 10 == 0
                                                                                ? intHash_Large
                                                                                : fltHashCounter % 5 == 0
                                                                                                       ? intHash_Medium
                                                                                                       : intHash_Small);
                                        g.DrawLine(pHash, ptTop, ptBottom);
                                    }
                                }
                                break;
                        }
                    }
                }
                return _bmpHashBoard;
            }
        }

        public void Slide_Right() { Value += SmallChange; }
        public void Slide_Left() { Value -= SmallChange; }
        public void Step_Right() { Value += LargeChange; }
        public void Step_Left() { Value -= LargeChange; }

        public void Resized()
        {
            _bmpHashBoard = null;
        }

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;
            Bitmap bmpTemp = new Bitmap(bmpHashBoard);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                // draw slider
                float fltScale = (float)(Max - Min);
                float fltSlider = (float)(Value - Min);
                float fltPercentage = fltSlider / fltScale;

                Point ptSliderCenter = new Point((int)(ptBar_Ends[0].X + ((float)(ptBar_Ends[1].X - ptBar_Ends[0].X) * fltPercentage)),
                                                 (int)(ptBar_Ends[0].Y + ((float)(ptBar_Ends[1].Y - ptBar_Ends[0].Y) * fltPercentage)));

                _recSlider = new Rectangle(new Point(ptSliderCenter.X - bmpSlider.Width / 2, ptSliderCenter.Y - bmpSlider.Height / 2),
                                           bmpSlider.Size);
                g.DrawImage(bmpSlider, recSlider.Location);
                if (DrawBorder)
                    g.DrawRectangle(new Pen(ForeColor), new Rectangle(0, 0, Width - 1, Height - 1));


                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }
            }
            if (BackTransSPContainer)
                bmpTemp.MakeTransparent(BackColor);
            cEle.NeedsToBeRedrawn = false;
            MyImage = bmpTemp;
        }

        #region Properties
        public enum enuTrackBar_Style { Horizontal, Vertical, _numTrackBar_Style };
        enuTrackBar_Style _eStyle = enuTrackBar_Style.Horizontal;
        public enuTrackBar_Style eStyle
        {
            get { return _eStyle; }
            set
            {
                if (_eStyle != value)
                {
                    _eStyle = value;
                    MyImage = null;
                    if (_bmpSliderH != null)
                        _bmpSliderH.Dispose();
                    _bmpSliderH = null;
                    if (_bmpHashBoard != null)
                        _bmpHashBoard.Dispose();
                    _bmpHashBoard = null;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        bool bolDrawBorder = true;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }



        int intSmallChange = 1;
        public int SmallChange
        {
            get { return intSmallChange; }
            set { intSmallChange = value; }
        }

        int intLargeChange = 10;
        public int LargeChange
        {
            get { return intLargeChange; }
            set { intLargeChange = value; }
        }

        bool bolGrabSlider = false;
        public bool GrabSlider
        {
            get { return bolGrabSlider; }
            set
            {
                bolGrabSlider = value;
            }
        }

        Point _ptMouse = new Point();
        public Point ptMouse
        {
            get { return _ptMouse; }
            set
            {
                if (_ptMouse.X != value.X || _ptMouse.Y != value.Y)
                {
                    _ptMouse = value;
                    if (GrabSlider)
                    {
                        float fltPercentage = (eStyle == enuTrackBar_Style.Vertical
                                                       ? ((float)(ptMouse.Y - ptBar_Ends[0].Y) / (float)(ptBar_Ends[1].Y - ptBar_Ends[0].Y))
                                                       : ((float)(ptMouse.X - ptBar_Ends[0].X) / (float)(ptBar_Ends[1].X - ptBar_Ends[0].X)));
                        Value = (int)((Max - Min) * fltPercentage) + Min;
                    }
                }
            }
        }


        Point[] _ptBar_Ends = new Point[2];
        Point[] ptBar_Ends
        {
            get { return _ptBar_Ends; }
            set
            {
                _ptBar_Ends = value;
            }
        }

        Rectangle _recSlider = new Rectangle();
        public Rectangle recSlider
        {
            get { return _recSlider; }
        }

        int intMin = 0;
        public int Min
        {
            get { return intMin; }
            set
            {
                intMin = value;
                if (Max <= Min)
                    Max = Min + 1;
                if (Value < intMin)
                    Value = intMin;
            }
        }

        int intMax = 100;
        public int Max
        {
            get { return intMax; }
            set
            {
                intMax = value;
                if (Min >= Max)
                    Min = Max - 1;
                if (Value > Max)
                    Value = Max;
            }
        }

        int intValue = 0;
        public int Value
        {
            get { return intValue; }
            set
            {
                if (value < intMin)
                    value = intMin;
                else if (value > intMax)
                    value = intMax;
                if (intValue != value)
                {
                    intValue = value;
                    if (ValueChanged != null)
                        ValueChanged((object)this, new EventArgs());
                    cEle.NeedsToBeRedrawn = true;
                }

            }
        }

        #endregion

        #region Events
        EventHandler event_ValueChanged = null;
        public EventHandler ValueChanged
        {
            get { return event_ValueChanged; }
            set { event_ValueChanged = value; }
        }

        #endregion

    }

    public class PictureBox : SweepPrune_Object
    {
        public PictureBox(ref SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            PictureBox cMyRef = this;
            SPContainer.Add(ref cMyRef);
        }

        #region Methods

        public override void Draw()
        {
            if (!cEle.NeedsToBeRedrawn || SPContainer.BuildingInProgress) return;

            if (cEle.rec.Width < 10 || cEle.rec.Height < 1) return;
            Rectangle rectDestination = new Rectangle(0, 0, cEle.rec.Size.Width, cEle.rec.Size.Height);

            Bitmap bmpTemp = new Bitmap(rectDestination.Width, rectDestination.Height);

            //Image.Save(@"c:\debug\picbox.png");
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(sbrBackColor, new RectangleF(0, 0, Width, Height));

                if (Image != null && Image.PixelFormat != System.Drawing.Imaging.PixelFormat.DontCare)
                {
                    switch (SizeMode)
                    {
                        case System.Windows.Forms.PictureBoxSizeMode.StretchImage:
                            {
                                Rectangle recSource = new Rectangle(0, 0, Image.Width, Image.Height);
                                Rectangle recDest = new Rectangle(0, 0, Width, Height);
                                g.DrawImage(Image, recDest, recSource, GraphicsUnit.Pixel);
                            }
                            break;

                        case System.Windows.Forms.PictureBoxSizeMode.AutoSize:
                            {
                                Size = Image.Size;
                                g.DrawImage(Image, new Point(0, 0));
                            }
                            break;

                        case System.Windows.Forms.PictureBoxSizeMode.CenterImage:
                            {
                                Size szSource = new Size(Image.Height > Height
                                                                      ? Height
                                                                      : Image.Height,
                                                         Image.Width > Width
                                                                     ? Width
                                                                     : Image.Width);
                                Rectangle recSource = new Rectangle((Image.Width - szSource.Width) / 2,
                                                                    (Image.Height - szSource.Height) / 2,
                                                                    szSource.Width,
                                                                    szSource.Height);
                                Rectangle recDest = new Rectangle((Width - szSource.Width) / 2,
                                                                  (Height - szSource.Height) / 2,
                                                                  szSource.Width,
                                                                  szSource.Height);
                                g.DrawImage(Image, recDest, recSource, GraphicsUnit.Pixel);
                            }
                            break;

                        case System.Windows.Forms.PictureBoxSizeMode.Zoom:
                            {
                                double dblAR_Pic = (double)Height / (double)Width;
                                double dblAR_Image = (double)Image.Height / (double)Image.Width;
                                Size sz = new Size();
                                if (dblAR_Image > dblAR_Pic)
                                {
                                    // height is limit
                                    sz = new Size((int)(Height / dblAR_Image), Height);
                                }
                                else
                                {
                                    // width is limit
                                    sz = new Size(Width, (int)(Width * dblAR_Image));
                                }
                                Rectangle recDest = new Rectangle((Width - sz.Width) / 2, (Height - sz.Height) / 2, sz.Width, sz.Height);
                                Rectangle recSource = new Rectangle(0, 0, Image.Width, Image.Height);
                                g.DrawImage(Image, recDest, recSource, GraphicsUnit.Pixel);
                            }
                            break;

                        case System.Windows.Forms.PictureBoxSizeMode.Normal:
                            {
                                Size szSource = new Size(Image.Width > Width
                                                                     ? Width
                                                                     : Image.Width,
                                                         Image.Height > Height
                                                                      ? Height
                                                                     : Image.Height
                                                         );
                                Rectangle recSource = new Rectangle(0,
                                                                    0,
                                                                    szSource.Width,
                                                                    szSource.Height);
                                Rectangle recDest = new Rectangle(0,
                                                                  0,
                                                                  szSource.Width,
                                                                  szSource.Height);
                                g.DrawImage(Image, recDest, recSource, GraphicsUnit.Pixel);
                            }
                            break;
                    }
                }

                //                                              draw child elements
                for (int intEleCounter = 0; intEleCounter < cEle.lstEle.Count; intEleCounter++)
                {
                    classSweepAndPrune_Element cItem = cEle.lstEle.lst[intEleCounter];
                    Bitmap bmpMyImage = cItem.MyImage;
                    if (bmpMyImage != null)
                    {
                        g.DrawImage(bmpMyImage, cItem.rec);
                        cItem.NeedsToBeRedrawn = false;
                    }
                }

            }
            //bmpTemp.Save(@"c:\debug\picbox.png");
            MyImage = bmpTemp;
        }

        #endregion

        #region Properties
        System.Windows.Forms.PictureBoxSizeMode _sizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;

        public System.Windows.Forms.PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                if (_sizeMode != value)
                {
                    _sizeMode = value;
                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        bool bolDrawBorder = true;
        public bool DrawBorder
        {
            get { return bolDrawBorder; }
            set
            {
                if (bolDrawBorder != value)
                {
                    bolDrawBorder = value;
                    cEle.NeedsToBeRedrawn = true; ;
                }
            }
        }

        Bitmap _image = null;
        public Bitmap Image
        {
            get
            {
                //_image.Save(@"c:\debug\pic.png");
                return _image;
            }
            set
            {
                if (_image != null)
                    _image.Dispose();
                _image = value;
                if (event_ImageChanged != null)
                    event_ImageChanged((object)this, new EventArgs());
                //MyImage = null;

                //value.Save(@"c:\debug\pic.png");
                cEle.NeedsToBeRedrawn = true;
            }
        }

        EventHandler event_ImageChanged = null;
        public EventHandler ImageChanged
        {
            get { return event_ImageChanged; }
            set { event_ImageChanged = value; }
        }

        #endregion
    }

    public class classRadialCoordinate
    {
        public classRadialCoordinate() { }
        public classRadialCoordinate(PointF pt1, PointF pt2) { init(pt2.X - pt1.X, pt2.Y - pt1.Y); }
        public classRadialCoordinate(PointF ptf) { init(ptf.X, ptf.Y); }
        public classRadialCoordinate(double radians, double magnitude)
        {
            _magnitude = magnitude;
            _radians = radians;
        }

        private void init(double x, double y)
        {
            _magnitude = Math.Sqrt(x * x + y * y);
            _radians = Math.Atan2(y, x);
        }

        public classRadialCoordinate Copy()
        {
            classRadialCoordinate cRetVal = new classRadialCoordinate(_radians, _magnitude);
            return cRetVal;
        }

        double _radians;
        public double Radians
        {
            get { return _radians; }
            set { _radians = value; }
        }
        public double Degrees
        {
            get
            {
                double dblRetVal = (_radians * 360.0 / (Math.PI * 2.0) % 360);
                while (dblRetVal < 0)
                    dblRetVal += 360;
                while (dblRetVal > 360)
                    dblRetVal -= 360;
                return dblRetVal;
            }
            set { _radians = value / 360.0 * (2.0 * Math.PI); }
        }

        double _magnitude;
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }



        public Point toPoint()
        {
            return new Point((int)(_magnitude * Math.Cos(_radians)), (int)(_magnitude * Math.Sin(_radians)));
        }

        public PointF toPointF()
        {
            return new PointF((float)(_magnitude * Math.Cos(_radians)), (float)(_magnitude * Math.Sin(_radians)));
        }


    }

    public class classSPMath
    {
        public static bool PointIsInsideARectangle(PointF ptf, RectangleF recF)
        {
            return (ptf.X >= recF.Left
                          && ptf.X <= recF.Right
                          && ptf.Y >= recF.Top
                          && ptf.Y <= recF.Bottom);
        }
        public static PointF AddTwoPoints(Point pt1, PointF ptf2) { return new PointF(pt1.X + ptf2.X, pt1.Y + ptf2.Y); }

        public static Point AddTwoPoints(Point pt1, Point pt2) { return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y); }

        /// <summary>
        /// returns the vector difference between the two PointFs pt1-pt2
        /// </summary>
        /// <param name="pt1">PointF from which second PointF is subtracted</param>
        /// <param name="pt2">PointF subtracted from the first</param>
        /// <returns></returns>
        public static PointF SubTwoPointFs(PointF pt1, PointF pt2) { return new PointF(pt1.X - pt2.X, pt1.Y - pt2.Y); }

        /// <summary>
        /// returns the vector difference between the two PointFs pt1-pt2
        /// </summary>
        /// <param name="pt1">PointF from which second PointF is subtracted</param>
        /// <param name="pt2">PointF subtracted from the first</param>
        /// <returns></returns>
        public static Point SubTwoPoints(Point pt1, Point pt2) { return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y); }

        public static bool PointIsInsideARectangle(PointF ptf, List<RectangleF> lstRecF)
        {
            for (int intRecCounter = 0; intRecCounter < lstRecF.Count; intRecCounter++)
            {
                if (PointIsInsideARectangle(ptf, lstRecF[intRecCounter]))
                    return true;
            }
            return false;
        }

    }

    // 
    public class panelDefaultName : SPObjects.Panel
    {
        public SPObjects.Label lblDefaultName = null;


        public static SPObjects.getRectangle getRec_Debugger = null;

        public panelDefaultName(ref SPObjects.SPContainer SPContainer) : base(ref SPContainer)
        {

            this.SPContainer = SPContainer;
            Text = "";

            cEle.eSP_ObjectType = enuSweepPrune_ObjectType.UserDefined;
            cEle.Name = "panelDefaultName";
            BackColor = Color.White;
            ForeColor = Color.Black;
            cEle.eventDraw = eventDraw;

            lblDefaultName = new SPObjects.Label(ref SPContainer);
            lblDefaultName.cEle.Name = "lblDefaultName";
            lblDefaultName.AutoSize = true;
            lblDefaultName.Location = new Point(2, 2);
            cEle.lstEle.Add(ref lblDefaultName.cEle);

            DefaultName_Draw();
            placeObjects();
            Sizechanged = sizeChanged;
        }

        void sizeChanged(object sender, EventArgs e)
        {
            placeObjects();
            Bitmap bmpBackground = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmpBackground))
            {
                g.FillRectangle(new SolidBrush(BackColor), new RectangleF(0, 0, Width, Height));
                g.DrawRectangle(new Pen(ForeColor, 2), new Rectangle(0, 0, Width - 1, Height - 1));
            }
            BackgroundImage = bmpBackground;
        }

        public virtual void eventDraw(object sender, EventArgs e)
        {
            Draw();
            if (MyImage != null)
                SPContainer.DrawImage(MyImage, recDraw, new Rectangle(0, 0, MyImage.Width, MyImage.Height));
        }

        void DefaultName_Draw()
        {
            lblDefaultName.Text = "DefaultName";
        }


        public virtual void placeObjects()
        {
            lblDefaultName.Top = 0;

            lblDefaultName.Left = 2;
            lblDefaultName.Top = 2;
        }
    }
    public class ButtonControl : SPObjects.Button
    {
        public SPObjects.GroupBox grb = null;
        public SPObjects.ListBox lbx = null;

        System.Windows.Forms.Timer tmrHideGroupBox = new System.Windows.Forms.Timer();

        public static SPObjects.getRectangle getRec_Debugger = null;

        public ButtonControl(ref SPObjects.SPContainer SPContainer) : base(ref SPContainer)
        {
            this.SPContainer = SPContainer;
            Text = "";
            tmrHideGroupBox.Interval = 100;
            tmrHideGroupBox.Tick += TmrHideGroupBox_Tick;

            cEle.eSP_ObjectType = enuSweepPrune_ObjectType.ButtonControl;
            cEle.Name = "ButtonControl";
            BackColor = Color.White;
            ForeColor = Color.Black;
            cEle.eventDraw = eventDraw;

            grb = new SPObjects.GroupBox(ref SPContainer);
            grb.cEle.Name = "grb";
            grb.Sizechanged = grb_SizeChanged;

            lbx = new SPObjects.ListBox(ref SPContainer);
            lbx.cEle.Name = "lbx";
            lbx.SelectedIndex = -1;
            lbx.MouseUp = lbx_MouseUp;

            grb.Visible = false;
            grb.cEle.lstEle.Add(ref lbx.cEle);

            MouseEnter = _MouseEnter;

            placeObjects();
            Show();
            Sizechanged = sizeChanged;
        }


        public string MenuHeading
        {
            get
            {

                return (grb != null
                            ? grb.Text
                            : "");
            }
            set
            {
                if (grb != null)
                {
                    grb.Text = value;
                }
            }
        }
        public override Color BackColor
        {
            get { return BackColor_Dull; }
            set
            {
                if (BackColor != value)
                {
                    BackColor_Dull = value;
                    if (grb != null)
                        grb.BackColor = value;
                    if (lbx != null)
                        lbx.BackColor = value;

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.BackColor = value;
                    }

                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public override Color ForeColor
        {
            get { return ForeColor_Dull; }
            set
            {
                if (ForeColor != value)
                {
                    ForeColor_Dull = value;
                    if (grb != null)
                        grb.ForeColor = value;
                    if (lbx != null)
                        lbx.ForeColor = value;

                    for (int intChildCounter = 0; intChildCounter < cEle.lstEle.Count; intChildCounter++)
                    {
                        classSweepAndPrune_Element cEleChild = cEle.lstEle.lst[intChildCounter];
                        cEleChild.ForeColor = value;
                    }

                    cEle.NeedsToBeRedrawn = true;
                }
            }
        }

        public void Selections_Set(List<string> lstSelections)
        {
            lbx.Items.Clear();
            for (int intSelectionCounter = 0; intSelectionCounter < lstSelections.Count; intSelectionCounter++)
                lbx.Items.Add(lstSelections[intSelectionCounter]);

            lbx.Height = (lbx.ItemHeight * (1 + lbx.Items.Count));
            grb.Height = lbx.Bottom + 15;
        }

        void _MouseEnter(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (tmrHideGroupBox.Enabled) return;
            SPContainer.Building_Start();
            {
                Hide();
                placeObjects();
                grb.Show();
                grb.BringToFront();
            }
            SPContainer.Building_Complete();
            tmrHideGroupBox.Enabled = true;
        }

        void grb_SizeChanged(object sender, EventArgs e)
        {
            lbx.Location = new Point(5, 15);
            lbx.Size = new Size(grb.Width - 10, grb.Height - 20);
        }

        private void TmrHideGroupBox_Tick(object sender, EventArgs e)
        {
            Point ptMouse = classOnControl.MouseRelTo(SPContainer.pic);
            System.Windows.Forms.Control ctrl = SPContainer.pic;
            while (ctrl.Parent != null)
                ctrl = ctrl.Parent;
            ptMouse.X -= ctrl.Left;
            ptMouse.Y -= ctrl.Top;
            if (!classSPMath.PointIsInsideARectangle(ptMouse, grb.recSPArea))
            {
                bool bolBuilding = SPContainer.BuildingInProgress;
                SPContainer.Building_Start();
                {
                    Show();
                    grb.Hide();
                }
                if (!bolBuilding)
                    SPContainer.Building_Complete();

                tmrHideGroupBox.Enabled = false;
            }
        }

        void lbx_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (SelectionMade != null)
                    SelectionMade((object)this, e);
                bool bolBuilding = SPContainer.BuildingInProgress;
                SPContainer.Building_Start();
                {
                    lbx.SelectedIndex = -1;
                    lbx.SelectedIndices.Clear();
                    lbx.MyImage = null;
                    lbx.cEle.NeedsToBeRedrawn = true;
                }
                if (!bolBuilding)
                    SPContainer.Building_Complete();
            }
        }

        EventHandler _eventSelectionMade = null;
        public EventHandler SelectionMade
        {
            get { return _eventSelectionMade; }
            set { _eventSelectionMade = value; }
        }

        void sizeChanged(object sender, EventArgs e)
        {
            placeObjects();
            Bitmap bmpBackground = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmpBackground))
            {
                g.FillRectangle(new SolidBrush(BackColor), new RectangleF(0, 0, Width, Height));
                g.DrawRectangle(new Pen(ForeColor, 2), new Rectangle(0, 0, Width - 1, Height - 1));
            }
            BackgroundImage = bmpBackground;
        }

        public virtual void eventDraw(object sender, EventArgs e)
        {
            cEle.NeedsToBeRedrawn = cEle.NeedsToBeRedrawn
                                    || MyImage == null;
            Draw();
            if (MyImage != null)
                SPContainer.DrawImage(MyImage, recDraw, new Rectangle(0, 0, MyImage.Width, MyImage.Height));
        }


        public virtual void placeObjects()
        {
            Point ptGrb = cEle.recSP.Location;
            Size szGrb = grb.Size;
            int intStep = 5;

            Rectangle recGrb = new Rectangle(ptGrb, szGrb);
            while (recGrb.Right > SPContainer.recSPArea.Right)
                recGrb.X -= intStep;
            while (recGrb.Left < SPContainer.recSPArea.Left)
                recGrb.X += intStep;
            while (recGrb.Bottom > SPContainer.recSPArea.Bottom)
                recGrb.Y -= intStep;
            while (recGrb.Top < 0)
                recGrb.Y += intStep;

            bool bolBuilding = SPContainer.BuildingInProgress;
            if (grb.Visible)
                SPContainer.Building_Start();
            {
                grb.Location = recGrb.Location;

            }
            if (!bolBuilding && grb.Visible)
                SPContainer.Building_Complete();
        }
    }
    public class classOnControl
    {
        public static Point MouseRelTo(System.Windows.Forms.Control ctrl)
        {
            Point ptRetVal = System.Windows.Forms.Control.MousePosition;

            while (ctrl != null && ctrl.Parent != null)
            {
                ptRetVal.X -= ctrl.Location.X;
                ptRetVal.Y -= ctrl.Location.Y;
                ctrl = ctrl.Parent;
            }

            return ptRetVal;
        }

        public static Point RelScreen(System.Windows.Forms.Control ctrl)
        {
            Point ptRetVal = new Point(0, 0);

            while (ctrl != null && ctrl.Parent != null)
            {
                ptRetVal.X += ctrl.Location.X;
                ptRetVal.Y += ctrl.Location.Y;
                ctrl = ctrl.Parent;
            }

            return ptRetVal;
        }
    }

    /// <summary>
    /// compares two integers
    /// </summary>
    public class SpecialComparer_int : IComparer<int>
    {
        /// <summary>
        /// compare two integers
        /// </summary>
        public int Compare(int int1, int int2)
        {
            if (int1 > int2) return 1;
            else if (int1 < int2) return -1;
            else return 0;
        }
    }




    /// <summary>
    /// class used as a data object in the alphaTree
    /// </summary>
    public class classUILanguage_Element
    {
        string strText = "";
        public string Text
        {
            get { return strText; }
            set { strText = value; }
        }


        /// <summary>
        /// index of tooltip to be returned on the next request
        /// </summary>
        int intTipIndex = 0;

        List<string> lstTips = new List<string>();
        public string Tip
        {
            get
            {
                if (lstTips.Count == 0) return "";
                string strRetVal = lstTips[intTipIndex];
                intTipIndex = (intTipIndex + 1) % lstTips.Count;
                return strRetVal;
            }
            set
            {
                if (!lstTips.Contains(value))
                    lstTips.Add(value);
            }
        }

        public void Clear() { lstTips.Clear(); }
    }

}
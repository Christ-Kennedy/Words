using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

namespace Words
{
    public class classNotesInfo
    {
        public static List<classNotesInfo> lst = new List<classNotesInfo>();


        public static void init()
        {
            lst.Clear();
            new classNotesInfo("Root");
        }
        classNotesInfo _cParent = null;
        public classNotesInfo cParent
        {
            get
            {
                return _cParent;
            }
        }
        classProject cProject
        {
            get { return formWords.cProject; }
        }
        public bool bolBackUpFileNote = false;

        enuNotesButtons _eOrderBy = enuNotesButtons.ChapterNumber;
        public enuNotesButtons eOrderBy
        {
            get { return _eOrderBy; }
            set { _eOrderBy = value; }
        }

        bool _bolBackup_ChangesMade = false;
        public bool bolBackup_ChangesMade
        {
            get { return _bolBackup_ChangesMade; }
            set { _bolBackup_ChangesMade = value; }
        }


        public string xml_Parent_Note_ID = "";
        public string xml_Node_ID = "";


        int[] arrWidths = { 20, 200, 90, 90, 200 };
        public int[] TabWidths { get { return arrWidths; } set { arrWidths = value; } }
        public static classNotesInfo cRoot
        {
            get { return panelNotesDataTree.cRoot;}
        }

        public void Project_SetMainEdit()
        {
            if (bolBackUpFileNote)
            {
                System.Windows.Forms.MessageBox.Show("Can not edit a back up file in main editor", "cannot edit back up file");
                return;
            }

            cProject.cEdit_Main = this;
        }


        public void setBasePath(string strPath_New)
        {
            strPath = strPath_New;

            for (int intChildCounter = 0; intChildCounter < lstChildNotes.Count; intChildCounter++)
            {
                classNotesInfo cChildNote = lstChildNotes[intChildCounter];
                cChildNote.setBasePath(strPath_New);
            }
        }

        public void Project_SetParent()
        {
            panelNotesDataTree.cParent = this;
        }

        public void Path_SetNew()
        {
            if (cParent == null)
            {
                Path = System.IO.Directory.GetCurrentDirectory() + "\\";
                return;
            }

            if (cParent.Path != null && cParent.Path.Length > 0)
            {
                Path = cParent.Path;
                return;
            }

            for (int intFileCounter = 0; intFileCounter < cParent.lstChildNotes.Count; intFileCounter++)
            {
                Words.classNotesInfo cNoteSibling = cParent.lstChildNotes[intFileCounter];
                if (cNoteSibling != this && cNoteSibling.Path != null && cNoteSibling.Path.Length > 0)
                {
                    Path = cNoteSibling.Path;
                    return;
                }
            }
            Path = System.IO.Directory.GetCurrentDirectory() + "\\";
        }

        public void die()
        {
            if (this == cRoot)
            {
                MessageBox.Show("Root of Project cannot be destroyed", "no you ain't!");
                return;
            }
            lst.Remove(this);
            cParent.lstChildNotes.Remove(this);
        }


        public void HeadingColor_Set(string RTF)
        {
            /*
            using (RichTextBox rtxTemp = new RichTextBox())
            {
                rtxTemp.Rtf = RTF;
                if (rtxTemp.Text.Length > 0)
                {
                    rtxTemp.Select(0, 1);
                    Color clrFore_New = rtxTemp.SelectionColor;
                    Color clrBack_New = rtxTemp.SelectionBackColor;
                    if (!ck_RichTextBox.ColorComparison(clrFore_New, clrFore_Heading) || !ck_RichTextBox.ColorComparison(clrBack_New, clrBack_Heading))
                    {
                        clrFore_Heading = clrFore_New;
                        clrBack_Heading =clrBack_New;
                    }
                }
                else
                {
                    clrFore_Heading = Color.Black;
                    clrBack_Heading = Color.White;
                }
            }
            /*/
            List<Color> lstColors = classRTF.ColorTable_Parse(RTF);

            string strTag_FirstParagraph = "\\par\r";
            int intFirstParagraph_Index = RTF.IndexOf(strTag_FirstParagraph);
            if (intFirstParagraph_Index >=0)
            {
                Color clrFore_New = Color.Black;
                Color clrBack_New = Color.White;

                string strFirstParagraph = RTF.Substring(0, intFirstParagraph_Index);
                int intHighlight = classRTF.Tag_GetNext(strFirstParagraph, classRTF.enuRTF_Tags.highlight);
                if (intHighlight >= 0 && intHighlight < lstColors.Count)
                    clrBack_New = lstColors[intHighlight];

                int intForeColor = classRTF.Tag_GetNext(strFirstParagraph, classRTF.enuRTF_Tags.cf);
                if (intForeColor >= 0 && intForeColor < lstColors.Count)
                    clrFore_New = lstColors[intForeColor];

                if (!ck_RichTextBox.ColorComparison(clrFore_New, clrFore_Heading) || !ck_RichTextBox.ColorComparison(clrBack_New, clrBack_Heading))
                {
                    clrFore_Heading = clrFore_New;
                    clrBack_Heading = clrBack_New;
                }

            }



            // */
            groupboxNotes.instance.pnlNotes.Buttons_Rebuild();
        }


        System.Drawing.Color _clrFore_Heading = System.Drawing.Color.Black;
        public System.Drawing.Color clrFore_Heading
        {
            get { return _clrFore_Heading; }
            set { _clrFore_Heading = value; }
        }

        System.Drawing.Color _clrBack_Heading = System.Drawing.Color.White;
        public System.Drawing.Color clrBack_Heading
        {
            get { return _clrBack_Heading; }
            set { _clrBack_Heading = value; }
        }


        public string Note_ID 
        { 
            get 
            { 
                return  (cParent != null ? cParent.Note_ID : "")
                      + (lst.IndexOf(this)).ToString() 
                      + "_" + Title;
            }        
        }

        public List<classNotesInfo> _lstChildNotes = new List<classNotesInfo>();
        public List<classNotesInfo> lstChildNotes
        {
            get { return _lstChildNotes; }
        }

        static bool BFS_Ancestors(ref classNotesInfo cNoteTemp, ref classNotesInfo cNoteSearch)
        {
            if (cNoteTemp == null) return false;
            if (cNoteTemp == cNoteSearch) return true;
            classNotesInfo cNoteParent = cNoteTemp.cParent;
            return BFS_Ancestors(ref cNoteParent, ref cNoteSearch);
        }

        static bool BFS_Descendants(ref classNotesInfo cNoteTemp, ref classNotesInfo cNoteSearch)
        {
            if (cNoteTemp == null) return false;
            if (cNoteTemp == cNoteSearch) return true;
            for (int intNoteCounter = 0; intNoteCounter < cNoteTemp.lstChildNotes.Count; intNoteCounter++)
            {
                classNotesInfo cNoteChild = cNoteTemp.lstChildNotes[intNoteCounter];
                return BFS_Descendants(ref cNoteChild, ref cNoteSearch);
            }
            return false;
        }

        public void ChildNodes_Add(ref classNotesInfo cNoteChild)
        {
            classNotesInfo cMyRef = this;
            if (!BFS_Descendants(ref cMyRef, ref cNoteChild))
            {
                if (!BFS_Ancestors(ref cMyRef, ref cNoteChild))
                {
                    if (cProject.cEdit_Alt != null)
                    {
                        int intCurrentIndex = cProject.cEdit_Alt.cParent.lstChildNotes.IndexOf(cProject.cEdit_Alt);
                        if (intCurrentIndex >= 0 && intCurrentIndex < lstChildNotes.Count)
                        {
                            lstChildNotes.Insert(lstChildNotes.Count > 0
                                                                     ? intCurrentIndex + 1
                                                                     : 0,
                                                 cNoteChild);
                            cNoteChild._cParent = this;
                            return;
                        }
                    }
                    lstChildNotes.Add(cNoteChild);
                    cNoteChild._cParent = this;
                    return;
                }
            }
        }


        public List<panelNotesDataTree.NotesButton> lstButtons = new List<panelNotesDataTree.NotesButton>();

        #region btPOVs
        public static BinTree.classBinTree cBT_POVs = new BinTree.classBinTree();
        public class classBT_POV_Item
        {
            public classBT_POV_Item(string strPOV)
            {
                this.strText = strPOV;
                this.strKey = strPOV.Trim().ToUpper();
            }

            string strKey = "";
            public string Key
            {
                get { return strKey; }
            }
            string strText = "";
            public string Text
            {
                get { return strText; }
            }

        }
        public void btPOV_Insert(string strPOV)
        {
            classBT_POV_Item btItem = new classBT_POV_Item(strPOV);
            System.Object objItem = (System.Object)btItem;
            cBT_POVs.Insert(ref objItem, btItem.Key);
        }


        string strKeyDateTime = "";
        public string keyDateTime
        {
            get
            {
                if (true || strKeyDateTime == null || strKeyDateTime.Length == 0)
                    strKeyDateTime = Date + "-" + Time;
                return strKeyDateTime;
            }
        }

        public string Date { get { return Year.ToString().PadLeft(4, ' ') + "/" + Month.ToString("00") + "/" + Day.ToString("00"); } }

        public string Time { get { return Hour.ToString("00") + ":" + Minute.ToString("00"); } }

        #endregion
        void forceDraw(enuNotesButtons eButton)
        {
            int intButton = (int)eButton;
            if (intButton >= 0 && intButton < lstButtons.Count)
            {
                lstButtons[intButton].cEle.NeedsToBeRedrawn = true;
            }
            switch (eButton)
            {
                case enuNotesButtons.Time:
                case enuNotesButtons.Date:
                    strKeyDateTime = "";
                    break;
            }
        }

        public classNotesInfo(string Title)
        {
            strTitle = Title;
            lst.Add(this);

            int intArraySize = Enum.GetNames(typeof(enuNotesFieldTypes)).Length;
            objData = new object[intArraySize];
            Path = System.IO.Directory.GetCurrentDirectory() + "\\";
            for (int intDataFieldCount = 0; intDataFieldCount < intArraySize; intDataFieldCount++)
            {
                enuNotesFieldTypes eFieldType = (enuNotesFieldTypes)intDataFieldCount;
                switch (eFieldType)
                {
                    case enuNotesFieldTypes.Year:
                    case enuNotesFieldTypes.Month:
                    case enuNotesFieldTypes.Day:
                    case enuNotesFieldTypes.Hour:
                    case enuNotesFieldTypes.Minute:
                    case enuNotesFieldTypes.ChapterNumber:
                        objData[intDataFieldCount] = 0;
                        break;

                    case enuNotesFieldTypes.POV:
                    case enuNotesFieldTypes.Path:
                    case enuNotesFieldTypes.Title:
                        objData[intDataFieldCount] = "";
                        break;

                    case enuNotesFieldTypes.CompletionLevel:
                        objData[intDataFieldCount] = "";
                        break;

                    default:
                        MessageBox.Show("this should not happen");
                        break;
                }
            }
        }

        #region Title

        static BinTree.classBinTree _cBT_Title = new BinTree.classBinTree();
        static public BinTree.classBinTree cBT_Title
        {
            get { return _cBT_Title; }
            set { _cBT_Title = value; }
        }


        string strTitle = "";
        public string Title
        {
            get { return strTitle; }
            set
            {
                if (panelNotesDataTree.cRoot == this) return;  // DO NOT CHANGE ROOT'S title
                if (cBT_Title == null) return;

                string strUpper = value.ToUpper();

                object objFind = cBT_Title.Search(strUpper);
                if (objFind != null)  // reject if an identical title exists
                {
                    classNotesInfo cNoteInfo = (classNotesInfo)objFind;
                    MessageBox.Show("That title already exists : " + cNoteInfo.Heading + "\r\n" + cNoteInfo.Filename);
                    return;
                }
                string strFilename_OLD = Filename;
                
                strTitle = value;

                if (File_Toggle)
                {
                    string strFilename_NEW = Filename;
                    File_Rename(strFilename_OLD, strFilename_NEW);
                }

                forceDraw(enuNotesButtons.Title);
                cBT_Title_Rebuild();
            }
        }

        int intCompletion_Level = 0;
        public int Completion_Level
        {
            get { return intCompletion_Level; }
            set { intCompletion_Level = value; }
        }

        static void cBT_Title_Rebuild()
        {
            cBT_Title.Clear();
            for (int intNoteCounter = 0; intNoteCounter < classNotesInfo.lst.Count; intNoteCounter++)
            {
                classNotesInfo cNote = classNotesInfo.lst[intNoteCounter];
                object objNote = cNote;
                cBT_Title.Insert(ref objNote, cNote.Title);
            }
        }

        #endregion

        bool bolCollapsed = false;
        public bool Collapsed
        {
            get { return bolCollapsed; }
            set { bolCollapsed = value; }
        }

        public bool POV_Highlight
        {
            get
            {
                if (!formWords.instance.grbNotes.pnlNotes.POV_Highlight)
                    return false;

                if (cProject.cEdit_Alt == null) return false;

                if (this == cProject.cEdit_Alt)
                    return true;

                return (string.Compare(POV, cProject.cEdit_Alt.POV) == 0);
            }
        }


        public void POV_HighlightChildren()
        {
            Words.panelNotesDataTree pnlEditor = formWords.instance.grbNotes.pnlNotes;

            bool bolBuiliding = pnlEditor.BuildingInProgress;
            pnlEditor.Building_Start();
            {
                //panelNotesDataTree.NotesButton btnAltEditPOV = (cProject != null && cProject.cEdit_Alt != null && cProject.cEdit_Alt.lstButtons.Count > (int)Words.enuNotesButtons.POV )
                //                                                 ? cProject.cEdit_Alt.lstButtons[(int)Words.enuNotesButtons.POV]
                //                                                 : null;

                for (int intChildCounter = 0; intChildCounter < this.lstChildNotes.Count; intChildCounter++)
                {
                    classNotesInfo cNote_Child = lstChildNotes[intChildCounter];
                    if (!cNote_Child.bolBackUpFileNote)
                    {
                        if (cNote_Child.POV_Highlight)
                        {
                            panelNotesDataTree.NotesButton btnPOV = cNote_Child.lstButtons[(int)Words.enuNotesButtons.POV];
                            btnPOV.eHighLight = enuHighlightStyle.POV_Highlight;
                        }
                        else
                        {
                            panelNotesDataTree.NotesButton btnPOV = cNote_Child.lstButtons[(int)Words.enuNotesButtons.POV];
                            panelNotesDataTree.NotesButton btnTitle = cNote_Child.lstButtons[(int)Words.enuNotesButtons.Title];
                            btnPOV.eHighLight = btnTitle.eHighLight;
                        }
                    }
                }
            }
            if (!bolBuiliding)
                pnlEditor.Building_Complete();
        }


        enuHighlightStyle _eHighlightStyle = enuHighlightStyle.none;
        public enuHighlightStyle eHighLight
        {
            get { return _eHighlightStyle; }

            set
            {
                _eHighlightStyle = value;

                bool bolBuilding = groupboxNotes.instance.pnlNotes.BuildingInProgress;
                groupboxNotes.instance.pnlNotes.Building_Start();
                {
                    for (int intBtnCounter = 0; intBtnCounter < lstButtons.Count; intBtnCounter++)
                    {
                        Words.enuNotesButtons eButton = (Words.enuNotesButtons)intBtnCounter;

                        panelNotesDataTree.NotesButton btnNote = lstButtons[intBtnCounter];

                        btnNote.eHighLight = (eButton == enuNotesButtons.POV && POV_Highlight)
                                                        ? enuHighlightStyle.POV_Highlight
                                                        : _eHighlightStyle; ;
                    }
                }
                if (!bolBuilding)
                    groupboxNotes.instance.pnlNotes.Building_Complete();
            }
        }

        public string Heading 
        { 
            get 
            { 
                return Title 
                        + (Date != null && Date.Length >0
                                ? " " + Date 
                                : "")
                        + (POV != null && POV.Length >0
                            ? "(" + POV + ")"
                            : ""); 
            }
        }

        string strPath = "";
        public string Path
        {
            get { return strPath; }
            set 
            {
                if (string.Compare(strPath, value) != 0)
                {
                    string strFilename_OLD = Filename;
                               
                    strPath = value;

                    string strFilename_NEW = Filename;

                    if (!formWords.bolInit || classProject.Loading) return;
                    if (string.Compare(strFilename_OLD, strFilename_NEW) != 0)
                    { // copy to new filename
                        //if (System.IO.File.Exists(strFilename_NEW))
                        //{
                        //    try
                        //    {
                        //        System.IO.File.Delete(strFilename_NEW);
                        //    }
                        //    catch (Exception errDelete)
                        //    {
                        //        MessageBox.Show("Error:" + errDelete.Message);
                        //    }
                        //}

                        if (System.IO.File.Exists(strFilename_OLD))
                        {
                            try
                            {
                                System.IO.File.Copy(strFilename_OLD, strFilename_NEW);
                            }
                            catch (Exception errCopy)
                            {
                                //MessageBox.Show("error:" + errCopy.Message);
                            }

                            //if (System.IO.File.Exists(strFilename_NEW))
                            //{
                            //    try
                            //    {
                            //        System.IO.File.Delete(strFilename_OLD);
                            //    }
                            //    catch (Exception errDelete)
                            //    {
                            //        MessageBox.Show("Error:" + errDelete.Message);
                            //    }
                            //}
                        }
                    }
                }
            }
        }

        public static string File_Rename(string strFilename_OLD, string strFilename_NEW)
        {
            if (string.Compare(strFilename_OLD, strFilename_NEW) != 0)
            { // copy to new filename
                if (System.IO.File.Exists(strFilename_NEW))
                {
                    try
                    {
                        System.IO.File.Delete(strFilename_NEW);
                    }
                    catch (Exception errDelete)
                    {
                        MessageBox.Show("Error:" + errDelete.Message);
                        return strFilename_OLD;
                    }                    
                }

                if (System.IO.File.Exists(strFilename_OLD))
                {
                    try
                    {
                        System.IO.File.Copy(strFilename_OLD, strFilename_NEW);
                    }
                    catch (Exception errCopy)
                    {
                        MessageBox.Show("error:" + errCopy.Message);
                        return strFilename_OLD;
                    }


                    if (System.IO.File.Exists(strFilename_NEW))
                    {
                        try
                        {
                            System.IO.File.Delete(strFilename_OLD);
                        }
                        catch (Exception errDelete)
                        {
                            MessageBox.Show("Error:" + errDelete.Message);
                            return strFilename_OLD;
                        }
                    }
                }
            }
            return strFilename_NEW;
        }



        bool bolFile_Toggle = false;
        public bool File_Toggle
        {
            get { return bolFile_Toggle; }
            set { bolFile_Toggle = value; }
        }

        public string Filename
        {
            get { return Path + Title + ".rtf"; }
            set
            {
                value = value.ToLower();
                Path = value.Replace(Title.ToLower(), "").Replace(".rtf", "");
            }
        }

        object[] objData = null;
        public object[] Data { get { return objData; } }


        public string POV
        {
            get { return (string)objData[(int)enuNotesFieldTypes.POV]; }
            set
            {
                objData[(int)enuNotesFieldTypes.POV] = (object)value;
                forceDraw(enuNotesButtons.POV);
            }
        }

        int intCaret = 0;
        public int Caret
        {
            get { return intCaret; }
            set { intCaret = value; }
        }

        public int Year
        {
            get { return (int)objData[(int)enuNotesFieldTypes.Year]; }
            set
            {
                objData[(int)enuNotesFieldTypes.Year] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }


        public int Month
        {
            get { return (int)objData[(int)enuNotesFieldTypes.Month]; }
            set
            {
                objData[(int)enuNotesFieldTypes.Month] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }

        public int Day
        {
            get { return (int)objData[(int)enuNotesFieldTypes.Day]; }
            set
            {
                objData[(int)enuNotesFieldTypes.Day] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }

        public int Hour
        {
            get { return (int)objData[(int)enuNotesFieldTypes.Hour]; }
            set
            {
                objData[(int)enuNotesFieldTypes.Hour] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }

        public int Minute
        {
            get { return (int)objData[(int)enuNotesFieldTypes.Minute]; }
            set
            {
                objData[(int)enuNotesFieldTypes.Minute] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }


        public int Chapter
        {
            get { return (int)objData[(int)enuNotesFieldTypes.ChapterNumber]; }
            set
            {
                objData[(int)enuNotesFieldTypes.ChapterNumber] = (object)value;
                forceDraw(enuNotesButtons.Date);
            }
        }

        #region XML
        static int xmlInner_GetInt(XmlNode xNode)
        {
            if (xNode == null) return 0;

            try
            {
                int intValue = Convert.ToInt32(xNode.InnerText);
                return intValue;
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public static classNotesInfo fromXml(ref XmlNode xNode,classProject.enuVersion eVersion)
        {
            //eVersion = classProject.enuVersion.WordsProject_1;
            switch(eVersion)
            {
                case classProject.enuVersion.WordsProject:
                    {
                        XmlNode xTitle = xNode.FirstChild;
                        XmlNode xNote_ID = xTitle.NextSibling;
                        XmlNode xChapter = xNote_ID.NextSibling;
                        XmlNode xPath = xChapter.NextSibling;
                        XmlNode xFilenameValid = xPath.NextSibling;
                        XmlNode xPOV = xFilenameValid.NextSibling;
                        XmlNode xDate = xPOV.NextSibling;
                        XmlNode xCaret = xDate.NextSibling;
                        XmlNode xParent = xCaret.NextSibling;
                        XmlNode xTabList = xParent.NextSibling;

                        classNotesInfo cRetVal = null;
                        if (string.Compare(xTitle.InnerText.ToUpper().Trim(), "ROOT") != 0)
                            cRetVal = new classNotesInfo(xTitle.InnerText);
                        else
                            cRetVal = classNotesInfo.cRoot;
                        
                        cRetVal.Path = xPath.InnerText;
                        cRetVal.File_Toggle = string.Compare(xFilenameValid.InnerText, true.ToString()) == 0;
                        cRetVal.POV = xPOV.InnerText;
                        cRetVal.Caret = xmlInner_GetInt(xCaret);
                        cRetVal.xml_Parent_Note_ID = xParent.InnerText;
                        cRetVal.xml_Node_ID = xNote_ID.InnerText;

                        try
                        {
                            cRetVal.Chapter = Convert.ToInt32(xChapter.InnerText);
                        }
                        catch (Exception)
                        {
                        }

                        XmlNode xYear = xDate.FirstChild;
                        cRetVal.Year = xmlInner_GetInt(xYear);
                        XmlNode xMonth = xYear.NextSibling;
                        cRetVal.Month = xmlInner_GetInt(xMonth);
                        XmlNode xDay = xMonth.NextSibling;
                        cRetVal.Day = xmlInner_GetInt(xDay);
                        XmlNode xHour = xDay.NextSibling;
                        cRetVal.Hour = xmlInner_GetInt(xHour);
                        XmlNode xMinute = xHour.NextSibling;
                        cRetVal.Minute = xmlInner_GetInt(xMinute);

                        if (xTabList != null)
                        {
                            cRetVal.TabWidths = new int[xTabList.ChildNodes.Count];
                            for (int intTabCounter = 0; intTabCounter < xTabList.ChildNodes.Count; intTabCounter++)
                            {
                                XmlNode xTab = xTabList.ChildNodes[intTabCounter];
                                try
                                {
                                    cRetVal.TabWidths[intTabCounter] = Convert.ToInt32(xTab.InnerText);
                                }
                                catch (Exception)
                                {
                                    cRetVal.TabWidths[intTabCounter] = 15;
                                }
                            }

                            XmlNode xOrderBy = xTabList.NextSibling;
                            if (xOrderBy != null)
                            {
                                try
                                {
                                    int intOrderBy = Convert.ToInt32(xOrderBy.InnerText);
                                    cRetVal.eOrderBy = (enuNotesButtons)intOrderBy;
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        // not included in this version
                        cRetVal.Completion_Level = 0;
                        return cRetVal;
                    }

                case classProject.enuVersion.WordsProject_1:
                    {
                        XmlNode xTitle = xNode.FirstChild;
                        XmlNode xCompletionLevel = xTitle.NextSibling;
                        XmlNode xNote_ID = xCompletionLevel.NextSibling;
                        XmlNode xChapter = xNote_ID.NextSibling;
                        XmlNode xPath = xChapter.NextSibling;
                        XmlNode xFilenameValid = xPath.NextSibling;
                        XmlNode xPOV = xFilenameValid.NextSibling;
                        XmlNode xDate = xPOV.NextSibling;
                        XmlNode xCaret = xDate.NextSibling;
                        XmlNode xParent = xCaret.NextSibling;
                        XmlNode xTabList = xParent.NextSibling;

                        classNotesInfo cRetVal = null;
                        if (string.Compare(xTitle.InnerText.ToUpper().Trim(), "ROOT") != 0)
                            cRetVal = new classNotesInfo(xTitle.InnerText);
                        else
                            cRetVal = classNotesInfo.cRoot;

                        cRetVal.Path = xPath.InnerText;
                        cRetVal.File_Toggle = string.Compare(xFilenameValid.InnerText, true.ToString()) == 0;
                        cRetVal.POV = xPOV.InnerText;
                        cRetVal.Caret = xmlInner_GetInt(xCaret);
                        cRetVal.xml_Parent_Note_ID = xParent.InnerText;
                        cRetVal.xml_Node_ID = xNote_ID.InnerText;
                        try
                        {
                            cRetVal.Chapter = Convert.ToInt32(xChapter.InnerText);
                        }
                        catch (Exception)
                        {
                        }

                        XmlNode xYear = xDate.FirstChild;
                        cRetVal.Year = xmlInner_GetInt(xYear);
                        XmlNode xMonth = xYear.NextSibling;
                        cRetVal.Month = xmlInner_GetInt(xMonth);
                        XmlNode xDay = xMonth.NextSibling;
                        cRetVal.Day = xmlInner_GetInt(xDay);
                        XmlNode xHour = xDay.NextSibling;
                        cRetVal.Hour = xmlInner_GetInt(xHour);
                        XmlNode xMinute = xHour.NextSibling;
                        cRetVal.Minute = xmlInner_GetInt(xMinute);

                        if (xTabList != null)
                        {
                            cRetVal.TabWidths = new int[xTabList.ChildNodes.Count];
                            for (int intTabCounter = 0; intTabCounter < xTabList.ChildNodes.Count; intTabCounter++)
                            {
                                XmlNode xTab = xTabList.ChildNodes[intTabCounter];
                                try
                                {
                                    cRetVal.TabWidths[intTabCounter] = Convert.ToInt32(xTab.InnerText);
                                }
                                catch (Exception)
                                {
                                    cRetVal.TabWidths[intTabCounter] = 15;
                                }
                            }

                            XmlNode xOrderBy = xTabList.NextSibling;
                            if (xOrderBy != null)
                            {
                                try
                                {
                                    int intOrderBy = Convert.ToInt32(xOrderBy.InnerText);
                                    cRetVal.eOrderBy = (enuNotesButtons)intOrderBy;
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        return cRetVal;
                    }      

                case classProject.enuVersion.WordsProject_2:
                    {
                        XmlNode xTitle = xNode.FirstChild;
                        XmlNode xNote_ID = xTitle.NextSibling;
                        XmlNode xChapter = xNote_ID.NextSibling;
                        XmlNode xPath = xChapter.NextSibling;
                        XmlNode xFilenameValid = xPath.NextSibling;
                        XmlNode xPOV = xFilenameValid.NextSibling;
                        XmlNode xDate = xPOV.NextSibling;
                        XmlNode xCaret = xDate.NextSibling;
                        XmlNode xTabList = xCaret.NextSibling;
                        XmlNode xOrderBy = xTabList.NextSibling;
                        XmlNode xHeadingColors = xOrderBy.NextSibling;
                        XmlNode xChildNotes = xHeadingColors.NextSibling;

                        classNotesInfo cRetVal = null;
                        if (string.Compare(xTitle.InnerText.ToUpper().Trim(), "ROOT") != 0)
                            cRetVal = new classNotesInfo(xTitle.InnerText);
                        else
                            cRetVal = classNotesInfo.cRoot;

                        cRetVal.Path = xPath.InnerText;
                        cRetVal.File_Toggle = string.Compare(xFilenameValid.InnerText, true.ToString()) == 0;
                        cRetVal.POV = xPOV.InnerText;
                        cRetVal.Caret = xmlInner_GetInt(xCaret);
                        cRetVal.xml_Node_ID = xNote_ID.InnerText;
                        try
                        {
                            cRetVal.Chapter = Convert.ToInt32(xChapter.InnerText);
                        }
                        catch (Exception)
                        {
                        }

                        //      date - time
                        {
                            XmlNode xYear = xDate.FirstChild;
                            cRetVal.Year = xmlInner_GetInt(xYear);
                            XmlNode xMonth = xYear.NextSibling;
                            cRetVal.Month = xmlInner_GetInt(xMonth);
                            XmlNode xDay = xMonth.NextSibling;
                            cRetVal.Day = xmlInner_GetInt(xDay);
                            XmlNode xHour = xDay.NextSibling;
                            cRetVal.Hour = xmlInner_GetInt(xHour);
                            XmlNode xMinute = xHour.NextSibling;
                            cRetVal.Minute = xmlInner_GetInt(xMinute);
                        }
                        //


                        if (xTabList != null)
                        {
                            cRetVal.TabWidths = new int[xTabList.ChildNodes.Count];
                            for (int intTabCounter = 0; intTabCounter < xTabList.ChildNodes.Count; intTabCounter++)
                            {
                                XmlNode xTab = xTabList.ChildNodes[intTabCounter];
                                try
                                {
                                    cRetVal.TabWidths[intTabCounter] = Convert.ToInt32(xTab.InnerText);
                                }
                                catch (Exception)
                                {
                                    cRetVal.TabWidths[intTabCounter] = 15;
                                }
                            }
                        }

                        if (xOrderBy != null)
                        {
                            try
                            {
                                int intOrderBy = Convert.ToInt32(xOrderBy.InnerText);
                                cRetVal.eOrderBy = (enuNotesButtons)intOrderBy;
                            }
                            catch (Exception)
                            {
                            }
                        }

                        // heading colors
                        {
                            XmlNode xHeading_ForeColor = xHeadingColors.FirstChild;
                            cRetVal.clrFore_Heading = classNotesInfo.Color_FromXml(ref xHeading_ForeColor);

                            XmlNode xHeading_BackColor = xHeading_ForeColor.NextSibling;
                            cRetVal.clrBack_Heading = classNotesInfo.Color_FromXml(ref xHeading_BackColor);
                        }

                        // chid nodes
                        {
                            for (int intChildCounter = 0; intChildCounter < xChildNotes.ChildNodes.Count; intChildCounter++)
                            {
                                XmlNode xChild = xChildNotes.ChildNodes[intChildCounter];
                                classNotesInfo cChild = fromXml(ref xChild, eVersion);
                                cRetVal.ChildNodes_Add(ref cChild);
                            }
                        }

                        return cRetVal;
                    }

                default:
                    System.Windows.Forms.MessageBox.Show("this should not happen");
                    break;
            }

            return null;
        }


        public XmlElement xml(ref XmlDocument xDoc)
        {
            XmlElement xRetVal = xDoc.CreateElement("xNote");

            //title
            {
                XmlElement xTitle = xDoc.CreateElement("Title");
                xTitle.InnerText = Title;
                xRetVal.AppendChild(xTitle);
            }

            // Note_ID
            {
                XmlElement xParent_Note_ID = xDoc.CreateElement("Note_ID");
                xParent_Note_ID.InnerText = Note_ID;
                xRetVal.AppendChild(xParent_Note_ID);
            }

            // chapter number
            {
                XmlElement xChapter = xDoc.CreateElement("ChapterNumeral");
                xChapter.InnerText = Chapter.ToString();
                xRetVal.AppendChild(xChapter);
            }

            //path 
            {
                XmlElement xPath = xDoc.CreateElement("Path");
                xPath.InnerText = Path;
                xRetVal.AppendChild(xPath);
            }

            // file_valid
            {
                XmlElement xFileValid = xDoc.CreateElement("FileValid");
                xFileValid.InnerText = File_Toggle.ToString();
                xRetVal.AppendChild(xFileValid);
            }

            // pov
            {
                XmlElement xPOV = xDoc.CreateElement("POV");
                xPOV.InnerText = POV;
                xRetVal.AppendChild(xPOV);
            }


            XmlElement xDate = xDoc.CreateElement("Date-Time");
            {
                // year
                {
                    XmlElement xYear = xDoc.CreateElement("Year");
                    xYear.InnerText = Year.ToString();
                    xDate.AppendChild(xYear);
                }
                // month
                {
                    XmlElement xMonth = xDoc.CreateElement("Month");
                    xMonth.InnerText = Month.ToString();
                    xDate.AppendChild(xMonth);
                }
                // day
                {
                    XmlElement xDay = xDoc.CreateElement("Day");
                    xDay.InnerText = Day.ToString();
                    xDate.AppendChild(xDay);
                }
                // hour
                {
                    XmlElement xHour = xDoc.CreateElement("Hour");
                    xHour.InnerText = Hour.ToString();
                    xDate.AppendChild(xHour);
                }
                // minute
                {
                    XmlElement xMinute = xDoc.CreateElement("Minute");
                    xMinute.InnerText = Minute.ToString();
                    xDate.AppendChild(xMinute);
                }
            }
            xRetVal.AppendChild(xDate);

            // caret
            {
                XmlElement xCaret = xDoc.CreateElement("Caret");
                xCaret.InnerText = Caret.ToString();
                xRetVal.AppendChild(xCaret);
            }

            // Tabs of data-tree
            XmlElement xTabs = xDoc.CreateElement("TabWidths");
            {
                for (int intTabCounter = 0; intTabCounter < TabWidths.Length; intTabCounter++)
                {
                    // Tab node
                    XmlElement xTempTab = xDoc.CreateElement("Tab_" + intTabCounter.ToString());
                    {
                        int intTab = TabWidths[intTabCounter];
                        xTempTab.InnerText = intTab.ToString();
                    }
                    xTabs.AppendChild(xTempTab);
                }
            }
            xRetVal.AppendChild(xTabs);

            // orderby
            XmlElement xOrderBy = xDoc.CreateElement("OrderBy");
            {
                xOrderBy.InnerText = ((int)eOrderBy).ToString();
            }
            xRetVal.AppendChild(xOrderBy);

            XmlElement xHeadingColors = xDoc.CreateElement("Heading_Colors");                // heading colors
            {
                XmlElement xHeading_Forecolor = XML_Color(ref xDoc, clrFore_Heading);
                xHeadingColors.AppendChild(xHeading_Forecolor);

                XmlElement xHeading_Backcolor = XML_Color(ref xDoc, clrBack_Heading);
                xHeadingColors.AppendChild(xHeading_Backcolor);
            }
            xRetVal.AppendChild(xHeadingColors);

            XmlElement xListChildNotes = xDoc.CreateElement("Child_Notes");                // child notes
            {
                for (int intChildNoteCounter = 0; intChildNoteCounter < lstChildNotes.Count; intChildNoteCounter++)
                {
                    classNotesInfo cChildNote = lstChildNotes[intChildNoteCounter];
                    if (!cChildNote.bolBackUpFileNote)
                    {
                        XmlElement xChildNote = cChildNote.xml(ref xDoc);
                        xListChildNotes.AppendChild(xChildNote);
                    }
                }
            }
            xRetVal.AppendChild(xListChildNotes);


            return xRetVal;
        }

        public static XmlElement XML_Color(ref XmlDocument xDoc, Color clr)
        {
            XmlElement xRetVal = xDoc.CreateElement("xNote");

            //title
            {
                XmlElement xR = xDoc.CreateElement("R");
                xR.InnerText = clr.R.ToString();
                xRetVal.AppendChild(xR);
                
                XmlElement xG = xDoc.CreateElement("G");
                xG.InnerText = clr.G.ToString();
                xRetVal.AppendChild(xG);
                
                XmlElement xB = xDoc.CreateElement("B");
                xB.InnerText = clr.B.ToString();
                xRetVal.AppendChild(xB);

            }

            return xRetVal;
        }

        public static Color Color_FromXml(ref XmlNode xColor)
        {
            XmlNode xR = xColor.FirstChild;
            XmlNode xG = xR.NextSibling;
            XmlNode xB = xG.NextSibling;

            try
            {
                int intR = Convert.ToInt32(xR.InnerText);
                int intG = Convert.ToInt32(xG.InnerText);
                int intB = Convert.ToInt32(xB.InnerText);

                Color clrRetVal = Color.FromArgb(intR, intG, intB);
                return clrRetVal;
            }
            catch (Exception)
            {
            }

            return Color.Black;
        }


        #endregion
    }


    public class classProject
    {
        public enum enuVersion { WordsProject, WordsProject_1, WordsProject_2};

        static int intVersion_Max = -1;
        static public int Version_Max
        {
            get
            {
                if (intVersion_Max<0)
                    intVersion_Max = Enum.GetNames(typeof(enuVersion)).Length -1;
                return intVersion_Max;
            }
        }

        static public enuVersion eVersion_Latest
        {
            get { return (enuVersion)Version_Max; }
        }

        static public string ProjectFileExtension 
        { 
            get 
            { 
                int intNumExtension = Enum.GetNames(typeof(enuVersion)).Length;
                enuVersion eVersion = ((enuVersion)(intNumExtension - 1));
                return eVersion.ToString(); 
            }
        }

        static public string ProjectExtensionFilter
        {
            get { return "Word Project Files (*." + enuVersion.WordsProject.ToString() + ")|*." + enuVersion.WordsProject.ToString() + "*"; }
        }


        string strName = "";
        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }

        public void New()
        {
            classProject cProject = new classProject();
            cProject.init();

            classNotesInfo cNewFile = new classNotesInfo("New Project");
            cNewFile.Year = DateTime.Now.Year;
            cNewFile.Day = DateTime.Now.Day;
            cNewFile.Month = DateTime.Now.Month;

            cNewFile.Hour = DateTime.Now.Hour;
            cNewFile.Minute = DateTime.Now.Minute;

            classNotesInfo.cRoot.ChildNodes_Add(ref cNewFile);
            cNewFile.Path = System.IO.Directory.GetCurrentDirectory();
            

            panelNotesDataTree.cParent = classNotesInfo.cRoot;

            cProject.cEdit_Alt
                = cProject.cEdit_Main
                = cNewFile;
        }

        public void init()
        {
            groupboxNotes.instance.Init();
            Name = "New Project";
            classNotesInfo.init();
            classNotesInfo.cBT_Title.Clear();
            int[] intTAbWidths = { 20, 85, 44, 110, 200 };

            classNotesInfo.cRoot.TabWidths = intTAbWidths;

            cEdit_Alt
                = cEdit_Main
                = null;

            pnlEditor.Buttons_Rebuild();
        }

        static bool bolLoading = true;
        static  public bool Loading
        {
            get { return bolLoading; }
            set { bolLoading = value; }
        }

        Font _fnt = new Font("ms Sans-serif", 10);
        public Font fnt
        {
            get { return _fnt; }
            set
            {
                if (_fnt != value)
                {
                    _fnt = value;
                    if (grbNotes != null)
                    {
                        if (grbNotes.rtxNotes != null)
                            grbNotes.rtxNotes.Font = _fnt;
                        if (grbNotes.pnlNotes != null)
                            grbNotes.pnlNotes.Font = _fnt;
                    }
                }
            }
        }


        public void setNotesPath(string strPath_New)
        {
            classNotesInfo.cRoot.setBasePath(strPath_New);
        }

        groupboxNotes grbNotes { get { return formWords.instance.grbNotes; } }


        string FontFileName
        {
            get { return System.IO.Directory.GetCurrentDirectory() + "\\fnt.txt"; }
        }

        public void Font_Load()
        {
            if (System.IO.File.Exists(FontFileName))
            {
                string strFont = System.IO.File.ReadAllText(FontFileName);
                char[] chrSplitSeparator = strFontDetail_Delimiter.ToArray<char>();

                string[] strFontDetails = strFont.Split(chrSplitSeparator);
                try
                {
                    string strTrue = true.ToString();
                    string strFamilyName = strFontDetails[(int)enuFontDetails.FamilyName];
                    float fltHeight = (float)Convert.ToDouble(strFontDetails[(int)enuFontDetails.Height]);
                    bool bolBold = string.Compare(strFontDetails[(int)enuFontDetails.Bold], strTrue) == 0;
                    bool bolItalic = string.Compare(strFontDetails[(int)enuFontDetails.Italic], strTrue) == 0;
                    bool bolStrikeOut = string.Compare(strFontDetails[(int)enuFontDetails.StrikeOut], strTrue) == 0;
                    bool bolUnderline = string.Compare(strFontDetails[(int)enuFontDetails.Underline], strTrue) == 0;

                    FontStyle fntStyle = (bolBold ? FontStyle.Bold : FontStyle.Regular)
                                        | (bolItalic ? FontStyle.Italic : FontStyle.Regular)
                                        | (bolUnderline ? FontStyle.Underline : FontStyle.Regular)
                                        | (bolStrikeOut ? FontStyle.Strikeout : FontStyle.Regular);
                    fnt = new Font(strFamilyName, fltHeight, fntStyle);
                }
                catch (Exception)
                {
                }
            }
        }


        string strFontDetail_Delimiter = "|";

        public string Title_Current
        {
            get
            {
                if (cEdit_Main != null)
                    return cEdit_Main.Title;
                else
                    return "";
            }
        }

        classNotesInfo _cEdit_Main = null;
        public classNotesInfo cEdit_Main 
        {
            get { return _cEdit_Main; }
            set
            {
                bool bolBuilding = (grbNotes.pnlNotes != null ? grbNotes.pnlNotes.BuildingInProgress : false);

                if (grbNotes != null && grbNotes.pnlNotes != null)
                    grbNotes.pnlNotes.Building_Start();
                
                {
                    if (cEdit_Alt != null && cEdit_Alt == value )
                    {
                        if (cEdit_Alt.File_Toggle)
                            grbNotes.Note_Save();
                        cEdit_Alt.eHighLight = enuHighlightStyle.none;
                    }

                    if (cEdit_Main != null)
                    {
                        if (cEdit_Main.File_Toggle)
                            formWords.instance.rtxCK.SaveFile(cEdit_Main.Filename);
                        cEdit_Main.eHighLight = enuHighlightStyle.none;
                    }

                    _cEdit_Main = value;

                    if (cEdit_Main != null)
                    {
                        if (cEdit_Main.File_Toggle)
                            formWords.instance.rtxCK.LoadFile(cEdit_Main.Filename);

                        formWords.instance.rtxCK.Heading = cEdit_Main.Heading;
                        cEdit_Main.eHighLight = enuHighlightStyle.Edit_Main;
                    }
                }

                if (grbNotes != null && grbNotes.pnlNotes != null)
                {
                    if (!bolBuilding)
                        grbNotes.pnlNotes.Building_Complete();
                    grbNotes.rtxNotes_SetEnable();
                }
            }
        }

        classNotesInfo _cEdit_Alt = null;
        public classNotesInfo cEdit_Alt
        {
            get { return _cEdit_Alt; }
            set
            {
                if (formWords.instance.grbNotes.Locked) return;

                if (_cEdit_Alt != value)
                {
                    grbNotes.Note_Save();
                    Loading = true;

                    bool bolBuiliding =pnlEditor.BuildingInProgress;
                    pnlEditor.Building_Start();
                    {
                        if (cEdit_Alt != null)
                        {
                            cEdit_Alt.eHighLight = cEdit_Alt == cEdit_Main
                                                              ? enuHighlightStyle.Edit_Main
                                                              : enuHighlightStyle.none;
                            groupboxNotePilot.instance.Data_setToNote();
                            if (cEdit_Alt == classBackUp.NoteInfo)
                                classBackUp.arrWidths = cEdit_Alt.TabWidths;
                        }

                        _cEdit_Alt = value;

                        if (cEdit_Alt != null)
                        {
                            cEdit_Alt.eHighLight = enuHighlightStyle.Edit_Alt;
                            groupboxNotePilot.instance.Data_setFromNote();
                        }

                        if (this.grbNotes.pnlNotes.POV_Highlight && grbNotes.cParent != null)
                            grbNotes.cParent.POV_HighlightChildren();
                    }
                    if (!bolBuiliding)
                        pnlEditor.Building_Complete();

                    Loading = false;
                    grbNotes.Note_Load();
                }         
            }
        }


        panelNotesDataTree pnlEditor
        {
            get { return panelNotesDataTree.instance; }
        }


        string _strFilePath = "";
        public string FilePath
        {
            get
            {
                if (_strFilePath == null || _strFilePath.Length == 0)
                    _strFilePath = System.IO.Directory.GetCurrentDirectory() + "\\test.rtf";
                return _strFilePath;
            }
            set
            {
                _strFilePath = value;
                string strExtension = System.IO.Path.GetExtension(_strFilePath).ToUpper();
                if (strExtension.Length > 0)
                    strExtension = strExtension.Substring(1);
                
                for (int intVersionCounter = 0; intVersionCounter <= classProject.Version_Max; intVersionCounter++)
                {
                    enuVersion eVersionTest = (enuVersion)intVersionCounter;
                    string strVersionTest = eVersionTest.ToString().ToUpper();
                    if (string.Compare(strVersionTest, strExtension) == 0)
                    {
                        eVersion = eVersionTest;
                        return;
                    }
                }
            }
        }

        void BuildDataTree(string strParent_Editor, string strEdit_Main, string strEdit_Alt)
        {
            if (classNotesInfo.lst.Count == 0) return;

            BinTree.classBinTree cBT = new BinTree.classBinTree();
            for (int intNoteCounter = 0; intNoteCounter < classNotesInfo.lst.Count; intNoteCounter++)
            {
                classNotesInfo cNote = classNotesInfo.lst[intNoteCounter];
                object objNote = (object)cNote;
                cBT.Insert(ref objNote, cNote.xml_Node_ID);
            }

            object objParent = null;
            switch (eVersion)
            {
                case enuVersion.WordsProject:
                case enuVersion.WordsProject_1:
                    {
                        for (int intNoteCounter = 1; intNoteCounter < classNotesInfo.lst.Count; intNoteCounter++)
                        {
                            classNotesInfo cNote = classNotesInfo.lst[intNoteCounter];

                            string strParent = cNote.xml_Parent_Note_ID;
                            objParent = cBT.Search(strParent);

                            if (objParent == null)
                                objParent = (object)panelNotesDataTree.cRoot;
                            classNotesInfo cParent = (classNotesInfo)objParent;
                            if (cParent != null)
                                cParent.ChildNodes_Add(ref cNote);
                        }

                    }
                    break;

                case enuVersion.WordsProject_2:
                    {

                    }
                    break;
            }
  
            objParent = cBT.Search(strParent_Editor);
            if (objParent == null)
                objParent = (object)panelNotesDataTree.cRoot;
            panelNotesDataTree.cParent = (classNotesInfo)objParent;
            
            object objEditMain = cBT.Search(strEdit_Main);
            if (objEditMain == null)
                objEditMain = (object)panelNotesDataTree.cRoot;
            cEdit_Alt  = (classNotesInfo)objEditMain;
            panelNotesDataTree.cProject.cEdit_Main = (classNotesInfo)objEditMain;

            object objEditAlt = cBT.Search(strEdit_Alt);
            if (objEditAlt == null)
                objEditAlt = (object)panelNotesDataTree.cRoot;
            cEdit_Alt = (classNotesInfo)objEditAlt;
        }

        public bool Load() 
        {
            bool bolRetVal = false;
            bolLoading = true;
            {
                bolRetVal = _Load();
            }
            bolLoading = false;

            return bolRetVal;
        }

        enuVersion _eVersion = enuVersion.WordsProject;
        public enuVersion eVersion
        {
            get { return _eVersion; }
            set { _eVersion = value; }
        }

        bool _Load()
        {
            init();

            XmlDocument xDoc = new XmlDocument();
            if (System.IO.File.Exists(FilePath))
            {
                string strName = System.IO.Path.GetFileName(FilePath);
                string strExtension = System.IO.Path.GetExtension(FilePath);
                if (strName.Length > strExtension.Length && strExtension.Length > 0)
                    strName = strName.Substring(0, strName.Length - strExtension.Length);
                Name = strName;

                xDoc.Load(FilePath);

                XmlNode xRoot = xDoc.FirstChild;
                XmlNode xMainEditor = xRoot.FirstChild;
                XmlNode xAltEditor = xMainEditor.NextSibling;
                XmlNode xParent = xAltEditor.NextSibling;

                switch (eVersion)
                {
                    case enuVersion.WordsProject:
                    case enuVersion.WordsProject_1:
                        {
                            XmlNode xHeadingList = xParent.NextSibling;

                            for (int intNoteCounter = 0; intNoteCounter < xHeadingList.ChildNodes.Count; intNoteCounter++)
                            {
                                XmlNode xNote = xHeadingList.ChildNodes[intNoteCounter];
                                classNotesInfo cNote = classNotesInfo.fromXml(ref xNote, eVersion);
                            }
                        }
                        break;

                    case enuVersion.WordsProject_2:
                        {
                            XmlNode xNote_Root = xParent.NextSibling;
                            classNotesInfo.fromXml(ref xNote_Root, eVersion);
                        }
                        break;
                }

                BuildDataTree(xParent.InnerText, xMainEditor.InnerText, xAltEditor.InnerText);
                return true;
            }
            else
            {
                return false;
            }
        }


        public void Save()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement xRoot = xDoc.CreateElement("Root");

            XmlElement xMainEditor = xDoc.CreateElement("Main_Editor");
            xMainEditor.InnerText = cEdit_Main != null 
                                                    ? cEdit_Main.Note_ID
                                                    : "NULL";
            xRoot.AppendChild(xMainEditor);

            XmlElement xAltEditor = xDoc.CreateElement("Alt_Editor");
            xAltEditor.InnerText = cEdit_Alt != null
                                                ? cEdit_Alt.Note_ID
                                                : "NULL";
            xRoot.AppendChild(xAltEditor);

            XmlElement xParent = xDoc.CreateElement("cParent");
            xParent.InnerText = panelNotesDataTree.cParent != null 
                                            ? panelNotesDataTree.cParent.Note_ID
                                            : Words.classNotesInfo.cRoot.Note_ID.ToString();
            xRoot.AppendChild(xParent);
            /*
            XmlElement xHeadingsList = xDoc.CreateElement("HeadingList");
            {
                for (int intNoteCounter = 0; intNoteCounter < classNotesInfo.lst.Count; intNoteCounter++)
                {
                    classNotesInfo cNote = classNotesInfo.lst[intNoteCounter];
                    if (!cNote.bolBackUpFileNote)
                    {
                        XmlElement xEle = cNote.xml(ref xDoc);
                        xHeadingsList.AppendChild(xEle);
                    }
                }
            }
            xRoot.AppendChild(xHeadingsList);
            /*/
            XmlElement xNote_ROot = classNotesInfo.cRoot.xml(ref xDoc);
            xRoot.AppendChild(xNote_ROot);
            // */

            xDoc.AppendChild(xRoot);
            xDoc.Save(FilePath);

            Font_Save();
        }

        void Font_Save()
        {
            string strFont = "";
            for (int intCounter = 0; intCounter < (int)enuFontDetails._num; intCounter++)
            {
                enuFontDetails eFontDetail = (enuFontDetails)intCounter;
                switch (eFontDetail)
                {
                    case enuFontDetails.FamilyName:
                        {
                            strFont += fnt.FontFamily.Name;
                        }
                        break;

                    case enuFontDetails.Height:
                        {
                            strFont += fnt.Size.ToString();
                        }
                        break;

                    case enuFontDetails.Bold:
                        {
                            strFont += fnt.Bold.ToString();
                        }
                        break;

                    case enuFontDetails.Underline:
                        {
                            strFont += fnt.Underline.ToString();
                        }
                        break;

                    case enuFontDetails.Italic:
                        {
                            strFont += fnt.Italic.ToString();
                        }
                        break;

                    case enuFontDetails.StrikeOut:
                        {
                            strFont += fnt.Strikeout.ToString();
                        }
                        break;
                }
                strFont += strFontDetail_Delimiter;
            }
            System.IO.File.WriteAllText(FontFileName, strFont);
        }

    }

}
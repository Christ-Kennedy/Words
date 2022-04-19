using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace panelSP
{
    #region SweepAndPrune_Engine
    public class classSweepAndPrune_Element
    {
        public panelSP panelSP = null;
        classSweepAndPrune_Element cMyRef = null;

        public string Name = "MyName";
        public object obj = null;
        Rectangle _recArea = new Rectangle();
        public Rectangle recArea
        {
            get { return _recArea; }
            set 
            {
                _recArea = value;
                panelSP.lstHScan.Clear();
                panelSP.lstVScan.Clear();
            }
        }

        public Point Location
        {
            get { return _recArea.Location; }
            set
            {
                recArea = new Rectangle(value, recArea.Size);
                panelSP.lstHScan.Clear();
                panelSP.lstVScan.Clear();
            }
        }


        public Size Size
        {
            get { return _recArea.Size; }
            set
            {
                recArea = new Rectangle(recArea.Location, value);
                panelSP.lstHScan.Clear();
                panelSP.lstVScan.Clear();
            }
        }

        public classSweepAndPrune_Element(ref panelSP panelSP)
        {
            cMyRef = this;
            this.panelSP = panelSP;
            panelSP.lstElements.Add(this);
        }

        static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID
        {
            get { return intID; }
        }
        
        bool bolNeedsToBeRedrawn = true;
        public bool NeedsToBeRedrawn
        {
            get { return bolNeedsToBeRedrawn; }
            set { bolNeedsToBeRedrawn = value; }
        }
     
   
        object _tag = null;
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
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
        static public List<classSweepAndPrune_Element> Search(Point pt, ref panelSP cSP_Data)
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

        static public List<classSweepAndPrune_Element> Search(Rectangle rec, ref panelSP cSP_Data) { return Search(rec.Location, new Point(rec.Right, rec.Bottom), ref cSP_Data); }
        static public List<classSweepAndPrune_Element> Search(Point ptTL, Point ptBR, ref panelSP cSP_Data)
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

        static public void Reset(ref panelSP cSP_Data)
        {
            cSP_Data.lstHScan
                = cSP_Data.lstVScan
                = null;
        }

        static List<classSweepAndPrune_Element> Search_H(Point pt, ref panelSP cSP_Data)
        {
            if (cSP_Data.lstHScan == null) Build_Scan_H(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstHScan;
            classSweepAndPrune_ElementFind cBFHFind = getBFElementContains(ref lstTemp, pt.X);
            if (cBFHFind != null) return cBFHFind.lstElements;
            return null;
        }
        static List<classSweepAndPrune_ElementFind> Search_H(Point ptTL, Point ptBR, ref panelSP cSP_Data)
        {
            if (cSP_Data.lstHScan == null || cSP_Data.lstHScan.Count ==0) Build_Scan_H(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstHScan;
            List<classSweepAndPrune_ElementFind> lstBFHFind = getBFElementContains(ref lstTemp, ptTL.X, ptBR.X);
            if (lstBFHFind != null) return lstBFHFind;
            return null;
        }
        static List<classSweepAndPrune_Element> Search_V(Point pt, ref panelSP cSP_Data)
        {
            if (cSP_Data.lstVScan == null) Build_Scan_V(ref cSP_Data);

            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstVScan;
            classSweepAndPrune_ElementFind cBFVFind = getBFElementContains(ref lstTemp, pt.Y);
            if (cBFVFind != null) return cBFVFind.lstElements;
            return null;
        }

        static List<classSweepAndPrune_ElementFind> Search_V(Point ptTL, Point ptBR, ref panelSP cSP_Data)
        {
            if (cSP_Data.lstVScan == null || cSP_Data.lstVScan.Count ==0) Build_Scan_V(ref cSP_Data);
            List<classSweepAndPrune_ElementFind> lstTemp = cSP_Data.lstVScan;
            List<classSweepAndPrune_ElementFind> lstBFVFind = getBFElementContains(ref lstTemp, ptTL.Y, ptBR.Y);
            if (lstBFVFind != null) return lstBFVFind;
            return null;
        }


        static void Build_Scan_H(ref panelSP cSP_Data)
        {
            // initialize list classSweepAndPrune_ElementFind elements
            cSP_Data.lstHScan = new List<classSweepAndPrune_ElementFind>();
            IEnumerable<classSweepAndPrune_Element> query = cSP_Data.lstElements.OrderBy(Element => Element.recArea.Left);
            cSP_Data.lstElements = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
            for (int intCounter = 0; intCounter < cSP_Data.lstElements.Count; intCounter++)
            {
                classSweepAndPrune_ElementFind cBFNew = new classSweepAndPrune_ElementFind();
                classSweepAndPrune_Element cEle = cSP_Data.lstElements[intCounter];
                if (cEle.recArea.Width > 0)
                {
                    cBFNew.intStart = cEle.recArea.Left;
                    cBFNew.intEnd = cEle.recArea.Right;
                    cBFNew.lstElements.Add(cEle);
                    cSP_Data.lstHScan.Add(cBFNew);
                }
            }
            /*
            List<classSweepAndPrune_ElementFind> lstHScan_Ref = cSP_Data.lstHScan;
            BuildScan(ref lstHScan_Ref);
            /*/
            BuildScan(ref cSP_Data.lstHScan);
            // */
        }

        static void Build_Scan_V(ref panelSP cSP_Data)
        {
            // initialize list classSweepAndPrune_ElementFind elements
            {
                cSP_Data.lstVScan = new List<classSweepAndPrune_ElementFind>();

                IEnumerable<classSweepAndPrune_Element> query = cSP_Data.lstElements.OrderBy(Element => Element.recArea.Top);
                cSP_Data.lstElements = (List<classSweepAndPrune_Element>)query.ToList<classSweepAndPrune_Element>();
                for (int intCounter = 0; intCounter < cSP_Data.lstElements.Count; intCounter++)
                {
                    classSweepAndPrune_ElementFind cBFNew = new classSweepAndPrune_ElementFind();
                    classSweepAndPrune_Element cEle = cSP_Data.lstElements[intCounter];
                    if (cEle.recArea.Height > 0)
                    {
                        cBFNew.intStart = cEle.recArea.Top;
                        cBFNew.intEnd = cEle.recArea.Bottom;

                        cBFNew.lstElements.Add(cEle);
                        cSP_Data.lstVScan.Add(cBFNew);
                    }
                }
            }
            /*
            List<classSweepAndPrune_ElementFind> lstVScan_Ref = cSP_Data.lstVScan;
            BuildScan(ref lstVScan_Ref);
            /*/
            BuildScan(ref cSP_Data.lstVScan);
            // */
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

    public class panelSP : Panel
    {
        public List<classSweepAndPrune_ElementFind> lstHScan = new List<classSweepAndPrune_ElementFind>();
        public List<classSweepAndPrune_ElementFind> lstVScan = new List<classSweepAndPrune_ElementFind>();
        public List<classSweepAndPrune_Element> lstElements = new List<classSweepAndPrune_Element>();

        public VScrollBar scrollBar_V = new VScrollBar();
        public HScrollBar scrollBar_H = new HScrollBar();

        bool bolScrollBar_Horizontal_Visible = true;
        public bool ScrollBar_Horizontal_Visible
        {
            get { return bolScrollBar_Horizontal_Visible; }
            set { bolScrollBar_Horizontal_Visible = value; }
        }
        bool bolScrollBar_Vertical_Visible = true;
        public bool ScrollBar_Vertical_Visible
        {
            get { return bolScrollBar_Vertical_Visible; }
            set { bolScrollBar_Vertical_Visible = value; }
        }


        panelSP cMyRef = null;
        public panelSP()
        {
            cMyRef = this;
            Controls.Add(scrollBar_V);
            scrollBar_V.ValueChanged += ScrollBar_V_ValueChanged;
            scrollBar_V.MouseWheel += ScrollBar_V_MouseWheel;

            Controls.Add(scrollBar_H);
            scrollBar_H.ValueChanged += ScrollBar_H_ValueChanged;
            scrollBar_H.MouseWheel += ScrollBar_H_MouseWheel;

            SizeChanged += PanelSP_SizeChanged;
        }

        private void ScrollBar_H_MouseWheel(object sender, MouseEventArgs e)
        {
            int intValue_new = scrollBar_H.Value - (scrollBar_H.LargeChange / 4) * Math.Sign(e.Delta);
            if (intValue_new > scrollBar_H.Maximum)
                intValue_new = scrollBar_H.Maximum;
            if (intValue_new < scrollBar_H.Minimum)
                intValue_new = scrollBar_H.Minimum;
            scrollBar_H.Value = intValue_new;
        }

        private void ScrollBar_V_MouseWheel(object sender, MouseEventArgs e)
        {
            int intValue_new = scrollBar_V.Value - (scrollBar_V.LargeChange / 4 ) * Math.Sign( e.Delta);
            if (intValue_new > scrollBar_V.Maximum)
                intValue_new = scrollBar_V.Maximum;
            if (intValue_new < scrollBar_V.Minimum)
                intValue_new = scrollBar_V.Minimum;
            scrollBar_V.Value = intValue_new;
        }

        bool bolBuilding = true;
        public bool Building
        {
            get { return bolBuilding; }
            set
            {
                if (bolBuilding != value)
                {
                    bolBuilding = value;
                    if (!bolBuilding)
                    {
                        setRecArea_Auto();
                        ptVisible = new Point(0, 0);
                    }
                    PlaceObjects();
                }
            }
        }

        Rectangle _recArea = new Rectangle(0, 0, 10, 10);
        public Rectangle recArea
        {
            get { return _recArea; }
            set { _recArea = value; }
        }

        Rectangle _recVisible = new Rectangle(0, 0, 10, 10);
        public Rectangle recVisible
        {
            get { return _recVisible; }
            set
            {
                _recVisible = value;

                PlaceObjects();
            }
        }


        public Point ptVisible
        {
            get { return recVisible.Location; }
            set { recVisible = new Rectangle(value, recVisible.Size); }
        }

        public Size szVisible
        {
            get { return recVisible.Size; }
            set
            {
                recVisible = new Rectangle(recVisible.Location, value);
                PlaceObjects();
            }
        }

        public classSweepAndPrune_Element Add(ref Panel pnlNew)
        {
            classSweepAndPrune_Element cEle_New = new classSweepAndPrune_Element(ref cMyRef);
            cEle_New.obj = (object)pnlNew;
            cEle_New.Size = pnlNew.Size;
            Controls.Add(pnlNew);

            lstHScan.Clear();
            lstVScan.Clear();
            //if (!Building)
            PlaceObjects();
            
            return cEle_New;
        }

        public void Sub(ref classSweepAndPrune_Element cEle_Sub)
        {
            lstElements.Remove(cEle_Sub);
            Panel pnlSub = (Panel)cEle_Sub.obj;
            Controls.Remove(pnlSub);
            PlaceObjects();
        }


        public int IndexOf(ref classSweepAndPrune_Element cEle)
        {
            return lstElements.IndexOf(cEle);
        }

        public int IndexOf(ref Panel pnlSearch)
        {
            for (int intEleCounter = 0; intEleCounter < lstElements.Count; intEleCounter++)
            {
                classSweepAndPrune_Element cEle = lstElements[intEleCounter];
                Panel pnl = (Panel)cEle.obj;
                if (pnl == pnlSearch)
                {
                    return intEleCounter;
                }
            }
            return -1;
        }

        public void Sub(ref Panel pnlSub)
        {
            for (int intEleCounter = 0; intEleCounter < lstElements.Count; intEleCounter++)
            {
                classSweepAndPrune_Element cEle = lstElements[intEleCounter];
                Panel pnl = (Panel)cEle.obj;
                if (pnl == pnlSub)
                {
                    Sub(ref cEle);
                    return;
                }
            }
        }

        public void Clear()
        {
            while (lstElements.Count >0)
            {
                classSweepAndPrune_Element cEle = lstElements[0];
                lstElements.RemoveAt(0);

                Panel pnl = (Panel)cEle.obj;
                Controls.Remove(pnl);
            }
        }


        public void setRecArea_Auto()
        {
            Point ptTL = new Point(int.MaxValue, int.MaxValue);
            Point ptBR = new Point(int.MinValue, int.MinValue);
            for (int intEleCounter = 0; intEleCounter < lstElements.Count; intEleCounter++)
            {
                classSweepAndPrune_Element cEle = lstElements[intEleCounter];
                if (ptTL.X > cEle.recArea.Left)
                    ptTL.X = cEle.recArea.Left;
                if (ptTL.Y > cEle.recArea.Top)
                    ptTL.Y = cEle.recArea.Top;
                if (ptBR.X < cEle.recArea.Right)
                    ptBR.X = cEle.recArea.Right;
                if (ptBR.Y < cEle.recArea.Bottom)
                    ptBR.Y = cEle.recArea.Bottom;
            }

            recArea = new Rectangle(ptTL, new Size(ptBR.X - ptTL.X + scrollBar_V.Width, ptBR.Y - ptTL.Y + scrollBar_H.Height));
            
            scrollBar_V.Minimum = ptTL.Y;
            scrollBar_V.Maximum = ptBR.Y+  scrollBar_H.Height;
            scrollBar_V.LargeChange = Height >10 ? Height : 10;
            scrollBar_V.Value = scrollBar_V.Minimum;

            scrollBar_H.Minimum = ptTL.X;
            scrollBar_H.Maximum = ptBR.X+scrollBar_V.Width;
            scrollBar_H.LargeChange = Width > 10 ?  Width : 10;
            scrollBar_H.Value = scrollBar_H.Minimum;
        }

        public void PlaceObjects()
        {
            if (Building) return;
            List<classSweepAndPrune_Element> lstVisible = classSweepAndPrune_Engine.Search(_recVisible, ref cMyRef);
            for (int intEleCounter = 0; intEleCounter < lstElements.Count; intEleCounter++)
            {
                classSweepAndPrune_Element cEle = lstElements[intEleCounter];
                Panel pnlEle = (Panel)cEle.obj;
                if (lstVisible.Contains(cEle))
                {
                    
                    pnlEle.Location = new Point(cEle.recArea.Left- recVisible.Left,  cEle.recArea.Top- recVisible.Top);
                    Controls.Add(pnlEle);
                
                    pnlEle.Show();
                    pnlEle.Visible = true;                    
                }
                else
                {
                    pnlEle.Hide();
                }
            }

            scrollBar_V.Location = new Point(Width - scrollBar_V.Width, 0);
            scrollBar_V.Visible = ScrollBar_Vertical_Visible;

            scrollBar_H.Location = new Point(0, Height - scrollBar_H.Height);
            scrollBar_H.Visible = ScrollBar_Horizontal_Visible;

            scrollBar_V.Height = Height - scrollBar_H.Height;
            scrollBar_H.Width = Width - scrollBar_V.Width;
        }

        private void PanelSP_SizeChanged(object sender, EventArgs e)
        {
            _recVisible.Size = Size;
            setRecArea_Auto();
            PlaceObjects();
        }

        private void ScrollBar_H_ValueChanged(object sender, EventArgs e)
        {
            _recVisible.X = scrollBar_H.Value;
            
            //if (_recVisible.X < recArea.Left)
            //    _recVisible.X = recArea.Left;
            //if (_recVisible.Right > recArea.Right)
            //    _recVisible.X = recArea.Right - recVisible.Width;
            
            PlaceObjects();
        }

        private void ScrollBar_V_ValueChanged(object sender, EventArgs e)
        {
            _recVisible.Y = scrollBar_V.Value;

            //if (_recVisible.Y < recArea.Top)
            //    _recVisible.Y = recArea.Top;
            //if (_recVisible.Bottom > recArea.Bottom)
            //    _recVisible.Y = recArea.Bottom - _recVisible.Height;

            PlaceObjects();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
namespace Words
{
    public class pictureboxWordAnalyzer : PictureBox
    {
        System.Windows.Forms.Timer tmrDrawDelay = new System.Windows.Forms.Timer();

        Semaphore semDrawMap = new Semaphore(1, 1);

        Point ptDrawMap_TopChar = new Point();
        Point ptDrawMap_LastChar = new Point();
        Size szDrawMap_LastChar = new Size();
        Size szDrawMap_Word = new Size();
        Rectangle recDrawMap_Source = new Rectangle();
        RichTextBox rtxCopy_Text = new RichTextBox();
        List<Rectangle> lstDrawMap_Divisions = new List<Rectangle>();
        double dblDrawMap_FactorDraw = 1.0;

        public enum enuDrawMap_Stages { Idle, CopyRTX, CalculateSize, CreateDivisions, DrawDivisions, _numDrawMap_Stages };

        System.Windows.Forms.Timer tmrDrawMap = new System.Windows.Forms.Timer();

        public static pictureboxWordAnalyzer instance = null;
        public pictureboxWordAnalyzer()
        {
            instance = this;
            MouseWheel += PictureboxWordAnalyzer_MouseWheel;
            MouseClick += PictureboxWordAnalyzer_MouseClick;
            SizeChanged += PictureboxWordAnalyzer_SizeChanged;

            tmrDrawMap.Interval = 100;
            tmrDrawMap.Tick += TmrDrawMap_Tick;
            tmrDrawMap.Enabled = true;

            szPic = Size;
        }

        RichTextBox rtx { get { return groupboxWordAnalyzer.rtxFocus; } }
        
        #region DrawMap

        enuDrawMap_Stages _eDrawMap_Stage = enuDrawMap_Stages.Idle;
        enuDrawMap_Stages eDrawMap_Stage
        {
            get { return _eDrawMap_Stage; }
            set
            {
                if (_eDrawMap_Stage != value)
                {
                    semDrawMap.WaitOne();
                    _eDrawMap_Stage = value;
                    switch(_eDrawMap_Stage)
                    {
                        case enuDrawMap_Stages.Idle:
                            Draw();
                            break;

                        case enuDrawMap_Stages.CopyRTX:
                            tmrDrawMap.Enabled = true;
                            break;

                        default:
                            break;
                    }
                    semDrawMap.Release();
                }
            }
        }

        private void TmrDrawMap_Tick(object sender, EventArgs e)
        {
            tmrDrawMap.Enabled = false;
            semDrawMap.WaitOne();
            
            if (rtx == null || rtx.Text.Length == 0 || strWord.Length == 0)
            {
                semDrawMap.Release();
                eDrawMap_Stage = enuDrawMap_Stages.Idle;
                tmrDrawMap.Enabled = true;
                return;
            }

            switch (eDrawMap_Stage)
            {
                case enuDrawMap_Stages.Idle:
                    tmrDrawMap.Enabled = false;
                    break;

                case enuDrawMap_Stages.CopyRTX:
                    DrawMap_CopyRTX();
                    semDrawMap.Release();
                    eDrawMap_Stage = enuDrawMap_Stages.CalculateSize;
                    tmrDrawMap.Enabled = true;
                    return;

                case enuDrawMap_Stages.CalculateSize:
                    DrawMap_CalculateSize();
                    semDrawMap.Release();
                    eDrawMap_Stage = enuDrawMap_Stages.CreateDivisions;
                    tmrDrawMap.Enabled = true;
                    return;

                case enuDrawMap_Stages.CreateDivisions:
                    DrawMap_CreateDivisions();
                    semDrawMap.Release();
                    eDrawMap_Stage = enuDrawMap_Stages.DrawDivisions;
                    tmrDrawMap.Enabled = true;
                    return;

                case enuDrawMap_Stages.DrawDivisions:
                    DrawMap_DrawDivisions();
                    Draw();
                    if (lstDrawMap_Divisions.Count >0)
                    {
                        semDrawMap.Release();
                        tmrDrawMap.Enabled = true;
                        return;
                    }
                    else
                    {
                        semDrawMap.Release();
                        eDrawMap_Stage = enuDrawMap_Stages.Idle;
                        tmrDrawMap.Enabled = true;
                        return;
                    }

                default:
                    break;
            }

            semDrawMap.Release();
            tmrDrawMap.Enabled = true;
        }

        void DrawMap_CalculateSize()
        {
            try
            {
                ptDrawMap_TopChar = rtx.GetPositionFromCharIndex(0);
                ptDrawMap_LastChar = rtx.GetPositionFromCharIndex(rtx.Text.Length - 1);
            }
            catch (Exception)
            {
                return;
            }

            string strLastChar = rtx.Text[rtx.Text.Length - 1].ToString();
            szDrawMap_LastChar = TextRenderer.MeasureText(strLastChar, rtx.Font);
            recDrawMap_Source = new Rectangle(0, 0, rtx.Width, ptDrawMap_LastChar.Y - ptDrawMap_TopChar.Y + szDrawMap_LastChar.Height);

            dblDrawMap_FactorDraw = (double)szPic.Height / (double)recDrawMap_Source.Height;
            Size szTextSource = TextRenderer.MeasureText(strWord, rtx.Font);
            szDrawMap_Word = new Size((int)(szTextSource.Width * dblDrawMap_FactorDraw), (int)(szTextSource.Height * dblDrawMap_FactorDraw));

            if (szDrawMap_Word.Width < 3)
                szDrawMap_Word.Width = 3;
            if (szDrawMap_Word.Height < 2)
                szDrawMap_Word.Height = 2;
            Size szBitmp = new Size((int)(recDrawMap_Source.Width * dblDrawMap_FactorDraw), (int)(recDrawMap_Source.Height * dblDrawMap_FactorDraw));

            if (szBitmp.Width < 3) szBitmp.Width = 3;
            if (szBitmp.Height < 3) szBitmp.Height = 3;

            _bmpMapBase = new Bitmap(szBitmp.Width, szBitmp.Height);

            using (Graphics g = Graphics.FromImage(_bmpMapBase))
            {
                g.FillRectangle(Brushes.White, new RectangleF(0, 0, _bmpMapBase.Width, _bmpMapBase.Height));
                _bmpMapBase.MakeTransparent(Color.White);
            }
        }

        void DrawMap_CopyRTX()
        {
            rtxCopy_Text.Size = rtx. Size;
            rtxCopy_Text.Rtf = rtx. Rtf;
            rtxCopy_Text.SelectionStart = rtx. SelectionStart;
            rtxCopy_Text.SelectionLength = rtx. SelectionLength;
            rtxCopy_Text.RightMargin = rtx. RightMargin;
            rtxCopy_Text.Select(rtx.SelectionStart, 0);
            rtxCopy_Text.ScrollToCaret();

            rtxCopy_Text.Text = rtxCopy_Text.Text.ToUpper();
        }

        void DrawMap_CreateDivisions()
        {
            lstDrawMap_Divisions.Clear();

            Rectangle recVisible_Source = recVisible_SOURCE_Calculate();
            if (recVisible_Source.Height < 3)
                return;

            int intHeightDivision = (int)Math.Ceiling((double)recVisible_Source.Height / 4.0);
            Rectangle rec_Up = new Rectangle(recVisible_Source.Location, new Size(recVisible_Source.Width, intHeightDivision));
            Rectangle rec_Down = new Rectangle(recVisible_Source.Location, new Size(recVisible_Source.Width, intHeightDivision));

            lstDrawMap_Divisions.Add(recVisible_Source);

            bool bolUp = true;
            bool bolDown = true;
            do
            {
                if (bolUp)
                {
                    rec_Up.Y -= intHeightDivision;
                    if (rec_Up.Bottom > ptDrawMap_TopChar.Y)
                        lstDrawMap_Divisions.Add(rec_Up);
                    else
                        bolUp = false;
                }

                if (bolDown)
                {
                    rec_Down.Y += rec_Down.Height;
                    if (rec_Down.Top < recDrawMap_Source.Bottom)
                    {
                        if (rec_Down.Bottom > recDrawMap_Source.Bottom)
                        {
                            rec_Down.Location = new Point(rec_Down.Left, recDrawMap_Source.Bottom);
                            bolDown = false;
                        }
                        lstDrawMap_Divisions.Add(rec_Down);
                    }
                    else
                        bolDown = false;
                }

            } while (bolUp || bolDown);
        }

        void DrawMap_DrawDivisions()
        {
            if (lstDrawMap_Divisions.Count == 0) return;

            Rectangle recDivision = lstDrawMap_Divisions[0];
            lstDrawMap_Divisions.RemoveAt(0);
            Point ptDrawMap_TopChar = rtx.GetPositionFromCharIndex(0);
            Point ptTL = new Point(recDrawMap_Source.Left, ptDrawMap_TopChar.Y + recDivision.Top);
            int intCharIndex_Start = rtx.GetCharIndexFromPosition(ptTL);
            int intCharIndex_End = rtx.GetCharIndexFromPosition(new Point(rtx.Width, recDivision.Bottom));
            if (intCharIndex_Start < rtxCopy_Text.Text.Length && intCharIndex_Start >= 0)
            {
                using (Graphics g = Graphics.FromImage(_bmpMapBase))
                {
                    int intIndex = rtxCopy_Text.Text.IndexOf(strWord, intCharIndex_Start);
                    while (intIndex >= 0 && intIndex < intCharIndex_End)
                    {
                        char chrBefore = intIndex > 0
                                                  ? rtxCopy_Text.Text[intIndex - 1]
                                                  : ' ';
                        char chrAfter = intIndex + strWord.Length < rtxCopy_Text.Text.Length - 2
                                                                  ? rtxCopy_Text.Text[intIndex + strWord.Length]
                                                                  : ' ';

                        if (!Char.IsLetter(chrBefore) && !Char.IsLetter(chrAfter))
                        {
                            Point ptStart = rtx.GetPositionFromCharIndex(intIndex);
                            Point ptDraw = new Point((int)(dblDrawMap_FactorDraw * ptStart.X), (int)(dblDrawMap_FactorDraw * (ptStart.Y - ptDrawMap_TopChar.Y)));
                            g.FillRectangle(Brushes.Green, new Rectangle(ptDraw, szDrawMap_Word));
                        }

                        try
                        {
                            intIndex = rtxCopy_Text.Text.IndexOf(strWord, intIndex + 1);
                        }
                        catch (Exception)
                        {
                            intIndex = -1;
                        }
                    }
                }
            }
        }
#endregion


        private void PictureboxWordAnalyzer_MouseClick(object sender, MouseEventArgs e)
        {
            Rectangle recPic = new Rectangle((Width - Image.Width) / 2,
                                             (Height - Image.Height) / 2,
                                             Image.Width,
                                             Image.Height);

            Point ptMouseRelPic = new Point(e.X - recPic.Left, e.Y - recPic.Top);

            if (rtx != null)
            {
                int intErrorCounter = 0;
                bool bolMoveUp = false;
                do
                {
                    Point ptDrawMap_TopChar = rtx.GetPositionFromCharIndex(0);
                    Point ptDrawMap_LastChar = rtx.GetPositionFromCharIndex(rtx.Text.Length - 1);

                    Point ptTL = rtx.GetPositionFromCharIndex(0);

                    int intCharTL = rtx.GetCharIndexFromPosition(new Point(0, 0));
                    int intCharBr = rtx.GetCharIndexFromPosition(new Point(rtx.Width, rtx.Height));
                    Point ptVisible_TL = rtx.GetPositionFromCharIndex(intCharTL);
                    Point ptVisible_BR = rtx.GetPositionFromCharIndex(intCharBr);

                    int intRTX_TextHeight = (rtx.GetPositionFromCharIndex(rtx.Text.Length - 1)).Y - ptTL.Y + 17;

                    Rectangle recText = new Rectangle(0, 0, rtx.Width, intRTX_TextHeight);
                    Rectangle recVisible_Source = new Rectangle(0, ptVisible_TL.Y - ptTL.Y, rtx.Width, ptVisible_BR.Y - ptVisible_TL.Y);

                    double dblSizeFactor = (double)bmpMapBase.Height / (double)recText.Height;

                    Rectangle recDraw = new Rectangle(0,
                                                      (int)(recVisible_Source.Top * dblSizeFactor),
                                                      bmpMapBase.Width,
                                                      (int)(recVisible_Source.Height * dblSizeFactor));
                    int intRecDrawHeight_Tolerance = (int)((float)recDraw.Height * 0.3f);
    
                    int intDelta = 120;

                    if (ptMouseRelPic.Y > recDraw.Top + intRecDrawHeight_Tolerance)
                    {
                        if (intErrorCounter > 0 && !bolMoveUp)
                            return;
                        bolMoveUp = true;
                        PictureboxWordAnalyzer_MouseWheel((object)this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, -intDelta));
                    }
                    else if (ptMouseRelPic.Y < recDraw.Bottom - intRecDrawHeight_Tolerance)
                    {
                        if (intErrorCounter > 0 && bolMoveUp)
                            return;
                        bolMoveUp = false;
                        PictureboxWordAnalyzer_MouseWheel((object)this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, intDelta));
                    }
                    else
                    {
                        return;
                    }
                    intErrorCounter++;

                } while (intErrorCounter < 256);
            }
        }

        private void PictureboxWordAnalyzer_SizeChanged(object sender, EventArgs e)
        {
            szPic = Size;
        }


        static Size _szPic = new Size();

        public static Size szPic
        {
            get { return _szPic; }
            set
            { _szPic = value; }
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/answers/questions/753327/send-mousescroll-from-picturebox-to-richtextbox.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureboxWordAnalyzer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!formWords.bolInit) return;
            bolIgnoreDraw = true;
            if (rtx != null)
            {
                int nNbLines = (int)((float)e.Delta * (float)SystemInformation.MouseWheelScrollLines / 120f);
                if (nNbLines > 0)
                {
                    for (int i = 0; i < nNbLines; i++)
                        formWords.SendMessage(rtx.Handle, WM_VSCROLL, (int)MakeWord(SB_LINEUP, 0), IntPtr.Zero);
                }
                else
                {
                    for (int i = nNbLines; i < 0; i++)
                        formWords.SendMessage(rtx.Handle, WM_VSCROLL, (int)MakeWord(SB_LINEDOWN, 0), IntPtr.Zero);
                }
            }
            bolIgnoreDraw = false;
            Draw();
        }

        public const int WM_VSCROLL = 0x0115;
        public const int SB_LINEUP = 0;
        public const int SB_LINEDOWN = 1;

        public static uint MakeWord(byte low, byte high)
        {
            return ((uint)high << 8) | low;
        }

        string strWord = "";
        public string Word
        {
            get { return strWord; }
            set
            {
                value = StringLibrary.classStringLibrary.clean_nonAlpha_Ends(value.ToUpper());
                if (value.Length > 0)
                {
                    if (formWords.grbOptions.Toggle_WordMap_AutoShutOff)
                    {
                        if (groupboxWordAnalyzer.WordMap)
                        {
                            groupboxWordAnalyzer.WordMap = false;
                            return;
                        }
                    }

                    strWord = value;

                    DrawWordMap();
                }
            }
        }

        public void DrawWordMap()
        {
            eDrawMap_Stage = enuDrawMap_Stages.CopyRTX;
        }


        Rectangle recVisible_SOURCE_Calculate()
        {
            Point ptTL = rtx.GetPositionFromCharIndex(0);

            int intCharTL = rtx.GetCharIndexFromPosition(new Point(0, 0));
            int intCharBr = rtx.GetCharIndexFromPosition(new Point(rtx.Width, rtx.Height));
            Point ptVisible_TL = rtx.GetPositionFromCharIndex(intCharTL);
            Point ptVisible_BR = rtx.GetPositionFromCharIndex(intCharBr);

            int intRTX_TextHeight = (rtx.GetPositionFromCharIndex(rtx.Text.Length - 1)).Y - ptTL.Y + 17;

            //Rectangle recText = new Rectangle(0, 0, rtx.Width, intRTX_TextHeight);
            Rectangle recVisible_Source = new Rectangle(0, ptVisible_TL.Y - ptTL.Y, rtx.Width, ptVisible_BR.Y - ptVisible_TL.Y);
            return recVisible_Source;
        }


        Rectangle recVisible_DRAW_Calculate()
        {
            Point ptTL = rtx.GetPositionFromCharIndex(0);
            
            int intRTX_TextHeight = (rtx.GetPositionFromCharIndex(rtx.Text.Length - 1)).Y - ptTL.Y + 17;
            Rectangle recText = new Rectangle(0, 0, rtx.Width, intRTX_TextHeight);
            Rectangle recVisible_Source = recVisible_SOURCE_Calculate(); 

            double dblSizeFactor = (double)bmpMapBase.Height / (double)recText.Height;

            Rectangle recVisible_DRAW = new Rectangle(0,
                                                      (int)(recVisible_Source.Top * dblSizeFactor),
                                                      bmpMapBase.Width,
                                                      (int)(recVisible_Source.Height * dblSizeFactor));
            return recVisible_DRAW;
        }


        bool bolIgnoreDraw = false;
        public void Draw()
        {
            if (rtx == null || bmpMapBase == null) return;
            if (bolIgnoreDraw) return; 

            // draw visible frame
            Rectangle recVisible_DRAW = recVisible_DRAW_Calculate();
            

            if (recVisible_DRAW.Width < 3)
                recVisible_DRAW.Width = 3;
            if (recVisible_DRAW.Height < 2)
                recVisible_DRAW.Height = 2;

            Bitmap bmpOutput = new Bitmap(bmpMapBase.Width, bmpMapBase.Height);
            using (Graphics g = Graphics.FromImage(bmpOutput))
            {
                g.FillRectangle(eDrawMap_Stage == enuDrawMap_Stages.Idle 
                                               ? Brushes.White
                                               : Brushes.Yellow, 
                                new Rectangle(0, 0, bmpOutput.Width, bmpOutput.Height));
                g.FillRectangle(Brushes.LightGray, recVisible_DRAW);
                g.DrawImage(bmpMapBase, new Point(0, 0));
             
            }
            Image = bmpOutput;
            Refresh();
        }


        Bitmap _bmpMapBase = null;
        Bitmap bmpMapBase
        {
            get { return _bmpMapBase; }
            set { _bmpMapBase = value; }
        }
    }
}

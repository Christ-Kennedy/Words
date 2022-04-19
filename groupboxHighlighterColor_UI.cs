using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Words
{
    public class groupboxHighlighterColor_UI : GroupBox
    {
        public static groupboxHighlighterColor_UI instance = null;
        ck_RichTextBox rtxCK = null;
        static public List<panelColorSelection> lstPnlColor = new List<panelColorSelection>();
        static Ck_Objects.classLabelButton btnGrammarCutter_ClauseDelimiters = new Ck_Objects.classLabelButton();

        public Ck_Objects.classLabelButton btnOk = new Ck_Objects.classLabelButton();
        enum enuRankButtons { Minimum, Maximum, _numRankButtons };
        System.Windows.Forms.Timer tmrMouseDown = new System.Windows.Forms.Timer();

        static GroupBox grbGrammarCutter_ClauseDelimiter = null;
        static TextBox txtGrammarCutter_ClauseDelimiter = null;
        static Ck_Objects.classLabelButton btnGrammarCutter_ClauseDelimiter_OK = null;
        static Ck_Objects.classLabelButton btnGrammarCutter_ClauseDelimiter_Cancel = null;
        static Ck_Objects.classLabelButton btnGrammarCutter_ClauseDelimiter_Reset = null;

        public groupboxHighlighterColor_UI(ref ck_RichTextBox rtxCK)
        {
            instance = this;

            this.rtxCK = rtxCK;
            Width = 350;

            Text = "Highlighter Color Sequence";
            
            Controls.Add(btnOk);
            btnOk.Text = "Ok";
            btnOk.AutoSize = true;
            btnOk.Click += BtnOk_Click;
            btnOk.LocationChanged += BtnOk_LocationChanged;

            Controls.Add(btnGrammarCutter_ClauseDelimiters);
            btnGrammarCutter_ClauseDelimiters.Text = "*";
            btnGrammarCutter_ClauseDelimiters.AutoSize = true;
            btnGrammarCutter_ClauseDelimiters.Click += BtnGrammarCutter_ClauseDelimiters_Click;

            VisibleChanged += GroupboxHighlighterColor_UI_VisibleChanged;
            LocationChanged += GroupboxHighlighterColor_UI_LocationChanged;
            ParentChanged += GroupboxHighlighterColor_UI_ParentChanged;
            Disposed += GroupboxHighlighterColor_UI_Disposed;
        }

        private void BtnOk_LocationChanged(object sender, EventArgs e)
        {
         
        }

        private void GroupboxHighlighterColor_UI_ParentChanged(object sender, EventArgs e)
        {
            
        }

        private void GroupboxHighlighterColor_UI_LocationChanged(object sender, EventArgs e)
        {
            
        }

        private void GroupboxHighlighterColor_UI_Disposed(object sender, EventArgs e)
        {
            
        }

        private void BtnGrammarCutter_ClauseDelimiters_Click(object sender, EventArgs e)
        {
            if (grbGrammarCutter_ClauseDelimiter == null)
            {
                // create objects
                grbGrammarCutter_ClauseDelimiter = new GroupBox();
                grbGrammarCutter_ClauseDelimiter.Text = "Clause Delimiters";
                grbGrammarCutter_ClauseDelimiter.BackColor = Color.Blue;
                grbGrammarCutter_ClauseDelimiter.ForeColor = Color.White;

                txtGrammarCutter_ClauseDelimiter = new TextBox();
                txtGrammarCutter_ClauseDelimiter.Font = new Font("Arial", 12);
                txtGrammarCutter_ClauseDelimiter.KeyDown += TxtGrammarCutter_ClauseDelimiter_KeyDown;
                //txtGrammarCutter_ClauseDelimiter.KeyPress += TxtGrammarCutter_ClauseDelimiter_KeyPress;
                grbGrammarCutter_ClauseDelimiter.Controls.Add(txtGrammarCutter_ClauseDelimiter);

                btnGrammarCutter_ClauseDelimiter_OK = new Ck_Objects.classLabelButton();
                {
                    btnGrammarCutter_ClauseDelimiter_OK.AutoSize = true;
                    btnGrammarCutter_ClauseDelimiter_OK.Text = "Ok";
                    btnGrammarCutter_ClauseDelimiter_OK.Click += BtnGrammarCutter_ClauseDelimiter_OK_Click;
                }
                grbGrammarCutter_ClauseDelimiter.Controls.Add(btnGrammarCutter_ClauseDelimiter_OK);

                btnGrammarCutter_ClauseDelimiter_Cancel = new Ck_Objects.classLabelButton();
                {
                    btnGrammarCutter_ClauseDelimiter_Cancel.AutoSize = true;
                    btnGrammarCutter_ClauseDelimiter_Cancel.Text = "Cancel";
                    btnGrammarCutter_ClauseDelimiter_Cancel.Click += BtnGrammarCutter_ClauseDelimiter_Cancel_Click;
                }
                grbGrammarCutter_ClauseDelimiter.Controls.Add(btnGrammarCutter_ClauseDelimiter_Cancel);
                

                btnGrammarCutter_ClauseDelimiter_Reset = new Ck_Objects.classLabelButton();
                {
                    btnGrammarCutter_ClauseDelimiter_Reset.AutoSize = true;
                    btnGrammarCutter_ClauseDelimiter_Reset.Text = "Reset";
                    btnGrammarCutter_ClauseDelimiter_Reset.Click += BtnGrammarCutter_ClauseDelimiter_Reset_Click;
                }
                grbGrammarCutter_ClauseDelimiter.Controls.Add(btnGrammarCutter_ClauseDelimiter_Reset);

                Controls.Add(grbGrammarCutter_ClauseDelimiter);
            }

            // place objects
            txtGrammarCutter_ClauseDelimiter.Location = new Point(5, 15);

            txtGrammarCutter_ClauseDelimiter.Text = ck_RichTextBox.classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI;
            grbGrammarCutter_ClauseDelimiter.Left = 10;
            grbGrammarCutter_ClauseDelimiter.Width = Width - 2 * grbGrammarCutter_ClauseDelimiter.Left;
            txtGrammarCutter_ClauseDelimiter.Width = grbGrammarCutter_ClauseDelimiter.Width - 2 * txtGrammarCutter_ClauseDelimiter.Left;
            btnGrammarCutter_ClauseDelimiter_OK.Location = new Point(txtGrammarCutter_ClauseDelimiter.Right - btnOk.Width, txtGrammarCutter_ClauseDelimiter.Bottom);
            btnGrammarCutter_ClauseDelimiter_Cancel.Location = new Point(btnGrammarCutter_ClauseDelimiter_OK.Left - btnGrammarCutter_ClauseDelimiter_Cancel.Width, btnGrammarCutter_ClauseDelimiter_OK.Top);
            btnGrammarCutter_ClauseDelimiter_Reset.Location = new Point(btnGrammarCutter_ClauseDelimiter_Cancel.Left - btnGrammarCutter_ClauseDelimiter_Reset.Width, btnGrammarCutter_ClauseDelimiter_OK.Top);

            grbGrammarCutter_ClauseDelimiter.Height = btnGrammarCutter_ClauseDelimiter_OK.Bottom + 5;

            grbGrammarCutter_ClauseDelimiter.Location = new Point((Width - grbGrammarCutter_ClauseDelimiter.Width) / 2,
                                                                  (Height - grbGrammarCutter_ClauseDelimiter.Height) / 2);

            grbGrammarCutter_ClauseDelimiter.Show();
            grbGrammarCutter_ClauseDelimiter.BringToFront();
        }

        private void TxtGrammarCutter_ClauseDelimiter_KeyDown(object sender, KeyEventArgs e)
        {

            switch(e.KeyCode)
            {
                case Keys.End:
                case Keys.Home:
                case Keys.Left:
                case Keys.Right:
                case Keys.Back:
                    break;

                case Keys.Enter:
                    e.SuppressKeyPress = true;
                    BtnGrammarCutter_ClauseDelimiter_OK_Click((object)btnGrammarCutter_ClauseDelimiter_OK, new EventArgs());
                    break;

                case Keys.Escape:
                    e.SuppressKeyPress = true;
                    BtnGrammarCutter_ClauseDelimiter_Reset_Click((object)btnGrammarCutter_ClauseDelimiter_Reset, new EventArgs());
                    break;

                default:
                    {
                        if (e.KeyValue < ' ')
                        {
                            e.SuppressKeyPress = true;
                            //System.Diagnostics.Debug.Print("txtGrammarCutter_ClauseDelimiter_Keydown key suppressed");
                        }
                    }
                    break;
            }
        }

        private void BtnGrammarCutter_ClauseDelimiter_OK_Click(object sender, EventArgs e)
        {
            // code to execute when user clicks button
            ck_RichTextBox.classGrammarCutter.strGrammarCutter_SubclauseSplitter_UI = txtGrammarCutter_ClauseDelimiter.Text;
            grbGrammarCutter_ClauseDelimiter.Hide();
        }
        
        private void BtnGrammarCutter_ClauseDelimiter_Cancel_Click(object sender, EventArgs e)
        {
            // code to execute when user clicks button
            grbGrammarCutter_ClauseDelimiter.Hide();
        }
        
        private void BtnGrammarCutter_ClauseDelimiter_Reset_Click(object sender, EventArgs e)
        {
            // code to execute when user clicks button
            txtGrammarCutter_ClauseDelimiter.Text = ck_RichTextBox.classGrammarCutter.strGrammarCutter_SubclauseSplitter_Default;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            for (int intUICounter = 0; intUICounter < lstPnlColor.Count && intUICounter < ck_RichTextBox.lstHighlighterItems.Count; intUICounter++)
            {
                panelColorSelection pnl = lstPnlColor[intUICounter];
                ck_RichTextBox.lstHighlighterItems[intUICounter] = pnl.cColorItem;
            }

            Hide();
        }

        private void GroupboxHighlighterColor_UI_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible) return;

            placeObjects();

            Location = new Point((rtxCK.Width - Width) / 2,
                                 (rtxCK.Height - Height) / 2);
        }

        public static void pnlColorSelection_MoveUp(ref panelColorSelection pnlMoveUp)
        {
            int intIndex = lstPnlColor.IndexOf(pnlMoveUp);
            if (intIndex >0)
            {
                lstPnlColor.Remove(pnlMoveUp);
                lstPnlColor.Insert(intIndex - 1, pnlMoveUp);
                instance.placeObjects();
            }
        }
        
        public static void pnlColorSelection_MoveDown(ref panelColorSelection pnlMoveDown)
        {
                        int intIndex = lstPnlColor.IndexOf(pnlMoveDown);
            if (intIndex >= 0 && intIndex < lstPnlColor.Count -1)
            {
                lstPnlColor.Remove(pnlMoveDown);
                lstPnlColor.Insert(intIndex+1, pnlMoveDown);
                instance.placeObjects();
            }
        }

        void placeObjects()
        {
            if (rtxCK == null) return;
            if (lstPnlColor.Count != ck_RichTextBox.lstHighlighterItems.Count)
            {
                while (lstPnlColor.Count > 0)
                {
                    panelColorSelection pnl = lstPnlColor[0];
                    lstPnlColor.Remove(pnl);
                    Controls.Remove(pnl);
                }
            }

            if (lstPnlColor.Count == 0)
            {
                for (int intColorCounter = 0; intColorCounter < ck_RichTextBox.lstHighlighterItems.Count; intColorCounter++)
                {
                    ck_RichTextBox.classHighlighterColorItem clrItem = ck_RichTextBox.lstHighlighterItems[intColorCounter];
                    panelColorSelection pnlNew = new panelColorSelection(ref clrItem);
                    Controls.Add(pnlNew);
                    lstPnlColor.Add(pnlNew);
                }
            }

            int intPanelHeight = 25;
            Point ptTL = new Point(5, 20);
            for (int intPanelCounter = 0; intPanelCounter < lstPnlColor.Count; intPanelCounter++)
            {
                panelColorSelection pnlTemp = lstPnlColor[intPanelCounter];
                Controls.Add(pnlTemp);
                pnlTemp.Size = new Size(Width - 10, intPanelHeight);
                pnlTemp.Location = ptTL;
                ptTL.Y = pnlTemp.Bottom;
            }

            btnOk.Location = new Point(Width - 5 - btnOk.Width, ptTL.Y);
            btnGrammarCutter_ClauseDelimiters.Location = new Point(5, btnOk.Top);
            Height = btnOk.Bottom +5;
        }


        public class panelColorSelection : Panel
        {
            public static List<panelColorSelection> lstPanels = new List<panelColorSelection>();

            TextBox txt = new TextBox();
            CheckBox chx = new CheckBox();

            static int intIDCounter = 0;
            int intID = intIDCounter++;
            public int ID { get { return intID; } }


            ck_RichTextBox.classHighlighterColorItem _cColorItem = null;
            public ck_RichTextBox.classHighlighterColorItem cColorItem { get { return _cColorItem; } }
            panelColorSelection pnlMyRef = null;
            public panelColorSelection(ref ck_RichTextBox.classHighlighterColorItem cColorItem)
            {
                lstPanels.Add(this);
                if (cColorItem == null) return;
                pnlMyRef = this;

                _cColorItem = cColorItem;

                Controls.Add(chx);
                chx.Size = new Size(18, 18);
                chx.Checked = cColorItem.valid;
                chx.CheckedChanged += Chx_CheckedChanged;

                Controls.Add(txt);
                txt.Text = cColorItem.Text;
                txt.BackColor = cColorItem.clrBack;
                txt.ForeColor = cColorItem.clrFore;
                txt.TextChanged += Txt_TextChanged;
                txt.KeyDown += Txt_KeyDown;
                txt.MouseWheel += Txt_MouseWheel;
                txt.ContextMenu = new ContextMenu();
                txt.ContextMenu.MenuItems.Add(new MenuItem("Edit Back Color", mnuEditBackColor_Click));
                txt.ContextMenu.MenuItems.Add(new MenuItem("Edit Fore Color", mnuEditForeColor_Click));

                SizeChanged += PanelColorSelection_SizeChanged;
            }

            private void Txt_MouseWheel(object sender, MouseEventArgs e)
            {
                if (e.Delta > 0)
                    MoveUp();
                else if (e.Delta < 0)
                    MoveDown();
            }


            void MoveUp()
            {
                pnlColorSelection_MoveUp(ref pnlMyRef);

            }

            void MoveDown()
            {

                pnlColorSelection_MoveDown(ref pnlMyRef);
            }

            private void Txt_KeyDown(object sender, KeyEventArgs e)
            {
                switch(e.KeyCode)
                {
                    case Keys.Up:
                        MoveUp();
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.Down:
                        MoveDown();
                        e.SuppressKeyPress = true;
                        break;
                }
            }

            void mnuEditBackColor_Click(object sender, EventArgs e)
            {
                ColorDialog cd = new ColorDialog();
                cd.Color = cColorItem.clrBack;
                if (cd.ShowDialog() == DialogResult.OK)
                    cColorItem.clrBack 
                        = txt.BackColor
                        = cd.Color;
            }
            void mnuEditForeColor_Click(object sender, EventArgs e)
            {
                ColorDialog cd = new ColorDialog();
                cd.Color = cColorItem.clrFore;
                if (cd.ShowDialog() == DialogResult.OK)
                    cColorItem.clrFore 
                        = txt.ForeColor 
                        = cd.Color;
            }

            private void Chx_CheckedChanged(object sender, EventArgs e)
            {
                cColorItem.valid = chx.Checked;
            }

            private void Txt_TextChanged(object sender, EventArgs e)
            {
                cColorItem.Text = txt.Text;
            }

            private void PanelColorSelection_SizeChanged(object sender, EventArgs e)
            {
                placeObjects();
            }

            void placeObjects()
            {
                chx.Location = new Point(3, 3);
                txt.Location = new Point(chx.Right, chx.Top);
                txt.Width = Width - txt.Left - chx.Left;
            }
        }
    }
}

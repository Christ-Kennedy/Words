using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Words
{
    public partial class formFindReplace : Form
    {
        int intPosition = 0;
        static string strFind = "", strReplace = "";
        public TextBox txtFind;
        public TextBox txtReplace;
        static public Point loc;
        public formFindReplace()
        {
            InitializeComponent();

            txtFind = new TextBox();
            txtFind.Size = new Size(160, 20);
            Controls.Add(txtFind);
            txtFind.Location = new Point(Width - txtFind.Width - 20, lblFind.Top);
            txtFind.Text = strFind;
            txtFind.TextChanged += new EventHandler(txtFind_TextChanged);

            txtReplace = new TextBox();
            txtReplace.Size = new Size(160, 20);
            Controls.Add(txtReplace);
            txtReplace.Location = new Point(Width - txtReplace.Width - 20, lblReplace.Top);
            txtReplace.Text = strReplace;
            txtReplace.TextChanged += new EventHandler(txtReplace_TextChanged);

            txtFind.TabIndex = 0;
            txtReplace.TabIndex = 1;
            btnFindNext.TabIndex = 2;
            btnReplace.TabIndex = 3;
            btnReplaceAll.TabIndex = 4;
            chkMatchCase.TabIndex = 5;

            btnReplaceAll.Click += btnReplaceAll_Click;

            Move += new EventHandler(formFindReplace_Move);

            Location = loc;

            ShowInTaskbar = false;
            TopMost = true;
        }


        void formFindReplace_Move(object sender, EventArgs e)
        {
            loc = Location;
        }

        void txtReplace_TextChanged(object sender, EventArgs e)
        {
            strReplace = txtReplace.Text;
        }

        void txtFind_TextChanged(object sender, EventArgs e)
        {
            strFind = txtFind.Text;
        }

        public void showFind()
        {
            lblReplace.Visible = txtReplace.Visible = false;
            btnReplace.Visible = false;
            btnReplaceAll.Visible = false;
            Text = "Find";
            Show();
            switch (eTypeBox)
            {
                case enuTypeBox.plain:
                    TextBox txt = (TextBox)objTxtBox;
                    txtFind.Text = txt.SelectedText;
                    break;

                case enuTypeBox.rich:
                    RichTextBox rtb = (RichTextBox)objTxtBox;
                    txtFind.Text = rtb.SelectedText;
                    break;

                case enuTypeBox.speller:
                    //classTextBoxSpellChecker stx = (classTextBoxSpellChecker)objTxtBox;
                    //      txtFind.Text = stx.SelectedText;
                    break;
            }
            txtFind.Focus();
        }

        public void showReplace()
        {
            lblReplace.Visible = txtReplace.Visible = true;
            btnReplace.Visible = true;
            btnReplaceAll.Visible = true;
            Text = "Replace";
            Show();
            txtFind.Focus();
        }

        public void setTextBox(ref object txtBox)
        {
            Type typeBox = txtBox.GetType();

            if (typeBox.FullName == "classTextBoxSpellChecker")
                eTypeBox = enuTypeBox.speller;
            else if (typeBox.FullName == "System.Windows.Forms.RichTextBox")
                eTypeBox = enuTypeBox.rich;
            else if (typeBox.FullName == "System.Windows.Forms.TextBox")
                eTypeBox = enuTypeBox.plain;

            intPosition = 0;

            objTxtBox = txtBox;
        }

        object objTxtBox;

        enum enuTypeBox { rich, plain, speller }
        enuTypeBox eTypeBox = enuTypeBox.rich;

        public void btnFindNext_Click(object sender, EventArgs e)
        {
            string strText, strSearch;
            int intNext;
            switch (eTypeBox)
            {
                case enuTypeBox.speller:
                    /*classTextBoxSpellChecker txtSpeller = (classTextBoxSpellChecker)objTxtBox;
                    strText = txtSpeller.Text;
                    strSearch = txtFind.Text;

                    if (!chkMatchCase.Checked)
                    {
                        strText = strText.ToUpper();
                        strSearch = strSearch.ToUpper();
                    }

                    intNext = strText.IndexOf(strSearch, intPosition + 1);
                    if (intNext < 0)
                    { intNext = strText.IndexOf(strSearch); }

                    if (intNext >= 0)
                    {
                        txtSpeller.SelectionStart = intNext;
                        txtSpeller.SelectionLength = strSearch.Length;
                        txtSpeller.ScrollToCaret();
                        txtSpeller.Focus();
                        intPosition = intNext;
                    }*/
                    break;

                case enuTypeBox.plain:
                    TextBox txtPlain = (TextBox)objTxtBox;
                    strText = txtPlain.Text;
                    strSearch = txtFind.Text;

                    if (!chkMatchCase.Checked)
                    {
                        strText = strText.ToUpper();
                        strSearch = strSearch.ToUpper();
                    }

                    intNext = strText.IndexOf(strSearch, intPosition + 1);
                    if (intNext < 0)
                    { intNext = strText.IndexOf(strSearch); }

                    if (intNext >= 0)
                    {
                        txtPlain.SelectionStart = intNext;
                        txtPlain.SelectionLength = strSearch.Length;
                        txtPlain.ScrollToCaret();
                        txtPlain.Focus();
                        intPosition = intNext;
                    }
                    break;

                case enuTypeBox.rich:
                    {
                        ck_RichTextBox ckRTX = (ck_RichTextBox)objTxtBox;
                        RichTextBox txtRich = (RichTextBox)ckRTX.rtx;
                        strText = txtRich.Text;
                        strSearch = txtFind.Text;

                        if (!chkMatchCase.Checked)
                        {
                            strText = strText.ToUpper();
                            strSearch = strSearch.ToUpper();
                        }

                        intNext = strText.IndexOf(strSearch, intPosition + 1);
                        if (intNext < 0)
                        { intNext = strText.IndexOf(strSearch); }

                        if (intNext >= 0)
                        {
                            txtRich.SelectionStart = intNext;
                            txtRich.SelectionLength = strSearch.Length;
                            txtRich.ScrollToCaret();
                            txtRich.Focus();
                            intPosition = intNext;
                        }
                    }
                    break;

            }
        }

        bool replace()
        {
            if ((txtFind.Text == txtReplace.Text)
                || (txtFind.Text.ToUpper() == txtReplace.Text.ToUpper() && chkMatchCase.Checked))
            {
                MessageBox.Show("no action performed");
                return false;
            }
            string strText = "";
            string strSearch = txtFind.Text;
            string strReplace = txtReplace.Text;

            switch (eTypeBox)
            {
                case enuTypeBox.speller:
                    {
                        /*classTextBoxSpellChecker txtSp = (classTextBoxSpellChecker)objTxtBox;
                    strText = txtSp.Text;
                    if (!chkMatchCase.Checked)
                    {
                        strText = strText.ToUpper();
                        strSearch = strSearch.ToUpper();
                    }

                    if (strText.Substring(intPosition, strSearch.Length) == strSearch)
                    {
                        string strLeft = txtSp.Text.Substring(0, intPosition);
                        string strRight = txtSp.Text.Substring(intPosition + strSearch.Length);

                        txtSp.Text = strLeft + txtReplace.Text + strRight;
                        txtSp.SelectionStart = intPosition;
                        txtSp.SelectionLength = strReplace.Length;
                        btnFindNext_Click((object)btnFindNext, new EventArgs());
                        return true;
                    }
                    */
                    }
                    break;


                case enuTypeBox.plain:
                    {
                        TextBox txtBox = (TextBox)objTxtBox;
                        strText = txtBox.Text;

                        if (!chkMatchCase.Checked)
                        {
                            strText = strText.ToUpper();
                            strSearch = strSearch.ToUpper();
                        }

                        if (strText.Substring(intPosition, strSearch.Length) == strSearch)
                        {
                            string strLeft = txtBox.Text.Substring(0, intPosition);
                            string strRight = txtBox.Text.Substring(intPosition + strSearch.Length);

                            txtBox.Text = strLeft + txtReplace.Text + strRight;
                            txtBox.SelectionStart = intPosition;
                            txtBox.SelectionLength = strReplace.Length;
                            btnFindNext_Click((object)btnFindNext, new EventArgs());
                            return true;
                        }
                    }
                    break;


                case enuTypeBox.rich:
                    {
                        ck_RichTextBox ckRTX = (ck_RichTextBox)objTxtBox;
                        RichTextBox rtfBox = (RichTextBox)ckRTX.rtx;
                        strText = rtfBox.Text;

                        if (!chkMatchCase.Checked)
                        {
                            strText = strText.ToUpper();
                            strSearch = strSearch.ToUpper();
                        }

                        if (strText.Substring(intPosition, strSearch.Length) == strSearch)
                        {
                            string strLeft = rtfBox.Text.Substring(0, intPosition);
                            string strRight = rtfBox.Text.Substring(intPosition + strSearch.Length);

                            rtfBox.Text = strLeft + txtReplace.Text + strRight;
                            rtfBox.SelectionStart = intPosition;
                            rtfBox.SelectionLength = strReplace.Length;
                            btnFindNext_Click((object)btnFindNext, new EventArgs());
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            replace();
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            btnFindNext_Click((object)btnFindNext, new EventArgs());
            int intStart = intPosition;
            bool bolLoop = false;
            do
            {
                bolLoop = replace();
            } while (bolLoop && intStart != intPosition);
        }
    }
}

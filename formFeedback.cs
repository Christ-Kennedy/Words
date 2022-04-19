using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words
{

    public partial class formFeedback : Form
    {
        public static formFeedback instance;
        public List<TextBox> lstTxt = new List<TextBox>();

        public formFeedback()
        {
            instance = this;
            MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            TopMost = false;
            BackColor = Color.White;

            Font = new Font("new courier", 12);
            Size = new Size(900, 500);
            SizeChanged += FormFeedback_SizeChanged;
            VisibleChanged += FormFeedback_VisibleChanged;
        }

        public void Txt_Add()
        {
            TextBox txtNew = new TextBox();
            txtNew.Font = new Font("new courier", 12);
            txtNew.Width = Width;
            txtNew.BorderStyle = BorderStyle.None;
            lstTxt.Add(txtNew);
            Controls.Add(txtNew);
            if (lstTxt.Count == 1)
                txtNew.Location = new Point(1, 35);
            else
                txtNew.Location = new Point(lstTxt[lstTxt.Count - 2].Left, lstTxt[lstTxt.Count - 2].Bottom);
        }

        private void FormFeedback_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void FormFeedback_SizeChanged(object sender, EventArgs e)
        {
            for (int intTxtCounter = 0; intTxtCounter < lstTxt.Count; intTxtCounter++)
                lstTxt[intTxtCounter].Width = Width;
        }

        public string Heading
        {
            set
            {
                Text = (string)value;
            }
            get { return Text; }
        }

        int intFeedbackCounter = 0;
        public void Animate()
        {
            if (!Visible) Show();
            intFeedbackCounter = (intFeedbackCounter + 1) % 16;
            if (intFeedbackCounter == 0)
            {
                if (Text.Length > 64)
                    Text = "";
            }
            else
                Text += ".";
        }

        public void Message(int intDictionary, string strMessage)
        {
            if (intDictionary >= 0 && intDictionary < lstTxt.Count)
            {
                lstTxt[intDictionary].Text = strMessage;
                lstTxt[intDictionary].Refresh();
            }
        }

    }
}

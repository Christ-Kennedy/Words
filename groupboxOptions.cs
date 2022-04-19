using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace Words
{

    public class groupboxOptions : GroupBox
    {
        public enum enuOptions
        {
            Toggle_Dictionary_Form_Mouse_Pop_Up_Definition,
            Toggle_Word_List_Form_Mouse_Pop_Up_Definition,
            Toggle_Auto_Save,
            Toggle_Numeral_Insert,
            Toggle_Two_Word_Swap,
            Toggle_Word_Suggestion,
            Toggle_WordMap_AutoShutOff,
            Toggle_Insert_Search_Retains_Prefix_and_Suffix,
            Integer_Definition_Ports_List,
            Integer_CopyCutter_AutoShutOffDelay,
            Integer_Rhyme_Dictionary_MaxResults,
            Integer_Backup_Time_Period,
            Integer_Backup_Files_Total_Number,
            _numOptions
        };
        public enum enuOption_Types { Toggle, Integer, _numOptionTypes };

        Button btnCopyCutterOptions = new Button();

        buttonOptionItem[] btnOptions_Array = new buttonOptionItem[(int)enuOptions._numOptions];
        public class buttonOptionItem : Panel
        {
            static BinaryFormatter formatter = new BinaryFormatter();
            static List<buttonOptionItem> lstOptions = new List<buttonOptionItem>();

            Label lblValue = new Label();
            Label lblHeading = new Label();


            int _intMaximum = 99;
            public int Maximum
            {
                get { return _intMaximum; }
                set { _intMaximum = value; }
            }

            int _intMinimum = 1;
            public int Minimum
            {
                get { return _intMinimum; }
                set { _intMinimum = value; }
            }

            int _intMouseScrollStepSize = 1;
            public int MouseScrollStepSize
            {
                get { return _intMouseScrollStepSize; }
                set { _intMouseScrollStepSize = value; }
            }


            public buttonOptionItem(enuOptions eOptions)
            {
                lstOptions.Add(this);

                Controls.Add(lblValue);
                lblValue.AutoSize = true;
                lblValue.Click += buttonOptionItem_Click;
                lblValue.MouseEnter += _MouseEnter;
                lblValue.MouseLeave += _MouseLeave;
                lblValue.MouseWheel += _MouseWheel;
                
                Controls.Add(lblHeading);
                lblHeading.AutoSize = true;
                lblHeading.Location = new Point(3, 3);
                lblHeading.Click += buttonOptionItem_Click;
                lblHeading.MouseEnter += _MouseEnter;
                lblHeading.MouseLeave += _MouseLeave;
                lblHeading.MouseWheel += _MouseWheel;

                this._eUIName = eOptions;
           
                ForeColor = clrForeColor;
                BackColor = clrBackColor;

                MouseEnter += _MouseEnter;
                MouseLeave += _MouseLeave;
                MouseWheel += _MouseWheel;
                Click += buttonOptionItem_Click;
            }

            static Color _clrForeColor = Color.Black;
            public static Color clrForeColor
            {
                get { return _clrForeColor; }
                set 
                {
                    _clrForeColor = value;
                    lstOptions_SetColors();
                }
            }

            static Color _clrBackColor = Color.White;
            public static Color clrBackColor
            {
                get { return _clrBackColor; }
                set 
                { 
                    _clrBackColor = value;
                    lstOptions_SetColors();
                }
            }
            static void lstOptions_SetColors()
            {
                for (int intOptionCounter = 0; intOptionCounter < lstOptions.Count; intOptionCounter++)
                {
                    buttonOptionItem btn = lstOptions[intOptionCounter];
                    btn.BackColor = clrBackColor;
                    btn.ForeColor = clrForeColor;
                }
            }

            private void _MouseWheel(object sender, MouseEventArgs e)
            {
                switch (eUI_Type)
                {
                    case enuOption_Types.Integer:
                        {
                            int intValue = ((int)var) + Math.Sign(e.Delta) * MouseScrollStepSize;

                            if (intValue < Minimum)
                                intValue = Minimum;
                            else if (intValue > Maximum)
                                intValue = Maximum;

                            var = (object)intValue;
                        }
                        break;

                    default:
                        break;
                }
                HeadingSet();
            }

            private void _MouseEnter(object sender, EventArgs e)
            {
                lblHeading.ForeColor
                    = lblValue.ForeColor
                    = ForeColor
                    = clrBackColor;

                lblHeading.BackColor
                    = lblValue.BackColor
                    = BackColor
                    = clrForeColor;
            }

            private void _MouseLeave(object sender, EventArgs e)
            {
                lblHeading.ForeColor
                    = lblValue.ForeColor
                    = ForeColor
                    = clrForeColor;

                lblHeading.BackColor
                    = lblValue.BackColor
                    = BackColor
                    = clrBackColor;
            }

            object objvar = "";
            public object var
            {
                get { return objvar; }
                set
                {
                    objvar = value;
                    HeadingSet();
                }
            }
        

            private void buttonOptionItem_Click(object sender, EventArgs e)
            {
                switch(eUI_Type)
                {
                    case enuOption_Types.Integer:
                        {
                        }
                        break;

                    case enuOption_Types.Toggle:
                        {
                            bool bolValue = !((bool)var);
                            var = (object)bolValue;
                        }
                        break;
                }

                HeadingSet();
            }

            public void HeadingSet()
            {
                lblValue.Text = varString;
                lblValue.Location = new Point(Width - lblValue.Width - 5, (Height - lblValue.Height) / 2);
                switch (eUI_Name)
                {
    
                    default:
                        lblHeading.Text = (_eUIName.ToString().Replace("_", " ")).Substring(eUI_Type.ToString().Length);
                        break;
                }
                lblHeading.Location = new Point(3, 3);
            }

            public string varString
            {
                get
                {
                    switch (eUI_Type)
                    {
                        case enuOption_Types.Integer:
                            {
                                int intVar = (int)var;

                                switch (_eUIName)
                                {
                                    case enuOptions.Integer_CopyCutter_AutoShutOffDelay:
                                        ck_RichTextBox.classCopyCutter.enuAutoShutOffDelay eAutoShutOffDelay = (ck_RichTextBox.classCopyCutter.enuAutoShutOffDelay)intVar;
                                        return eAutoShutOffDelay.ToString();

                                    default:
                                        return intVar.ToString();
                                }
                            }
                            break;

                        case enuOption_Types.Toggle:
                            {
                                bool bolValue = (bool)var;
                                return bolValue ? "On" : "Off";
                            }
                            break;
                    }
                    return "";
                }
            }


            enuOptions _eUIName = enuOptions._numOptions;
            public enuOptions eUI_Name { get { return _eUIName; } }

            public enuOption_Types eUI_Type { get { return getTypeFromOption(eUI_Name); } }

            static enuOption_Types getTypeFromOption(enuOptions eOption)
            {
                string strOption = eOption.ToString();
                for (int intOptionCounter = 0; intOptionCounter < (int)enuOption_Types._numOptionTypes; intOptionCounter++)
                {
                    enuOption_Types eType = (enuOption_Types)intOptionCounter;
                    string strType = eType.ToString();
                    if (strOption.Length >= strType.Length)
                        if (string.Compare(strType, strOption.Substring(0, strType.Length)) == 0)
                            return eType;
                }
                return enuOption_Types._numOptionTypes;
            }

            public void Load(ref System.IO.FileStream fs)
            {
                _eUIName = (enuOptions)formatter.Deserialize(fs);

                switch(eUI_Type)
                {
                    case enuOption_Types.Integer:
                        {
                            int  intLoaded = (int)formatter.Deserialize(fs);
                            var = (object)intLoaded;
                        }
                        break;

                    case enuOption_Types.Toggle:
                        {
                            bool bolLoaded = (bool)formatter.Deserialize(fs);
                            var = (object)bolLoaded;
                        }
                        break;
                }
            }

            public void Save(ref System.IO.FileStream fs)
            {
                formatter.Serialize(fs, (enuOptions)eUI_Name);

                switch (eUI_Type)
                {
                    case enuOption_Types.Integer:
                        {
                            formatter.Serialize(fs, (int)var);
                        }
                        break;

                    case enuOption_Types.Toggle:
                        {
                            formatter.Serialize(fs, (bool)var);
                        }
                        break;
                }
            }

        }

        string SaveFileName { get { return System.IO.Directory.GetCurrentDirectory() + "\\Options.fs"; } }

        public void ColorsSet(Color clrFore, Color clrBack)
        {
            buttonOptionItem.clrBackColor = clrBack;
            buttonOptionItem.clrForeColor = clrFore;
            BackColor = clrBack;
            ForeColor = clrFore;
        }

        Button btnOk = new Button();
        public groupboxOptions()
        {
            Text = "Options";
            Font = new Font("Arial", 14);

            Font fnt = new Font("Arial", 10);

            btnOk.AutoSize = true;
            btnOk.Text = "Ok";
            btnOk.AutoSize = true;
            btnOk.Font = fnt;
            btnOk.Click += BtnOk_Click;
            Controls.Add(btnOk);

            btnCopyCutterOptions.AutoSize = true;
            btnCopyCutterOptions.Text = "Copy Cutter";
            btnCopyCutterOptions.Font = fnt;
            btnCopyCutterOptions.Click += BtnCopyCutterOptions_Click;
            Controls.Add(btnCopyCutterOptions);

            Size szMinButton = new Size(540, 18);
            Width = szMinButton.Width + 10;
            
            // create the buttons
            for (int intButtonCounter = 0; intButtonCounter < btnOptions_Array.Length; intButtonCounter++)
            {
                enuOptions eOption = (enuOptions)intButtonCounter;
                buttonOptionItem btnNew = new buttonOptionItem(eOption);
                {
                    btnNew.AutoSize = false;
                    btnNew.Size = szMinButton;
                    btnNew.Font = fnt;
                    btnNew.Location = new Point(5, 25 + szMinButton.Height * intButtonCounter);
                    Controls.Add(btnNew);
                }

                switch(eOption)
                {
                    case enuOptions.Integer_Definition_Ports_List:
                        {
                            btnNew.Maximum = 16;
                        }
                        break;

                    case enuOptions.Integer_Backup_Files_Total_Number:
                        {
                            btnNew.Maximum = 256;
                            btnNew.Minimum  = 1;
                        }
                        break;

                    case enuOptions.Integer_Backup_Time_Period:
                        {
                            btnNew.Maximum = 60;
                            btnNew.Minimum = 1;
                        }
                        break;

                    case enuOptions.Integer_CopyCutter_AutoShutOffDelay:
                        {
                            btnNew .Maximum = Enum.GetNames(typeof(ck_RichTextBox.classCopyCutter.enuAutoShutOffDelay)).Length-1;
                            btnNew.Minimum = 0;
                        }
                        break;

                    case enuOptions.Integer_Rhyme_Dictionary_MaxResults:
                    {
                            btnNew.Maximum = 10000;
                            btnNew.Minimum = 0;
                            btnNew.MouseScrollStepSize = 25;
                        }
                        break;
                }

                btnOptions_Array[intButtonCounter] = btnNew;
            }

            // set their values
            if (System.IO.File.Exists(SaveFileName))
            {
                System.IO.FileStream fs = new System.IO.FileStream(SaveFileName, System.IO.FileMode.Open);
                {
                    for (int intButtonCounter = 0; intButtonCounter < btnOptions_Array.Length; intButtonCounter++)
                    {
                        buttonOptionItem btnTemp = btnOptions_Array[intButtonCounter];
                        btnTemp.Load(ref fs);
                    }
                }
                fs.Close();
            }
            else
            { // create file-stream
                System.IO.FileStream fs = new System.IO.FileStream(SaveFileName, System.IO.FileMode.Create);
                {
                    for (int intButtonCounter = 0; intButtonCounter < btnOptions_Array.Length; intButtonCounter++)
                    {
                        buttonOptionItem btnTemp = btnOptions_Array[intButtonCounter];

                        switch (btnTemp.eUI_Name)
                        {
                            case enuOptions.Integer_Definition_Ports_List:
                                {
                                    btnTemp.var = (object)((int)8);
                                }
                                break;

                            case enuOptions.Integer_CopyCutter_AutoShutOffDelay:
                                {
                                    btnTemp.var = (object)((int)ck_RichTextBox.classCopyCutter.enuAutoShutOffDelay.Minutes_Sixty);
                                }
                                break;
                                

                            case enuOptions.Integer_Rhyme_Dictionary_MaxResults:
                                {
                                    btnTemp.var = (object)256;
                                }
                                break;
                                

                            case enuOptions.Integer_Backup_Files_Total_Number:
                                {
                                    btnTemp.var = (object)20;
                                }
                                break;


                            case enuOptions.Integer_Backup_Time_Period:
                                {
                                    btnTemp.var = (object)5;
                                }
                                break;

                            default:
                                {
                                    btnTemp.var = (object)true;
                                }
                                break;
                        }
                        btnTemp.Save(ref fs);
                    }
                }
                fs.Close();
            }

            btnOk.Location = new Point(btnOptions_Array[btnOptions_Array.Length - 1].Right - btnOk.Width,
                                       btnOptions_Array[btnOptions_Array.Length -1].Bottom);

            btnCopyCutterOptions.Location = new Point(btnOk.Left - btnCopyCutterOptions.Width , btnOk.Top);

            Height = btnOk.Bottom + 5;
            VisibleChanged += GroupboxOptions_VisibleChanged;
        }

        private void BtnCopyCutterOptions_Click(object sender, EventArgs e)
        {
            Controls.Add(ck_RichTextBox.grbCopyCutterOptions);
            ck_RichTextBox.grbCopyCutterOptions.Size = Size;
            ck_RichTextBox.grbCopyCutterOptions.Location = new Point(0, 0);
            ck_RichTextBox.grbCopyCutterOptions.BringToFront();
            ck_RichTextBox.grbCopyCutterOptions.ShowOptions();
        }

        private void GroupboxOptions_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                ColorsSet(Color.Yellow, Color.DarkBlue);
                Text = "Options";
            }
        else
            {
                if (ck_RichTextBox.grbCopyCutterOptions != null)
                    ck_RichTextBox.grbCopyCutterOptions.Hide();
            }
        }

        public void Save()
        {
            if (System.IO.File.Exists(SaveFileName))
                System.IO.File.Delete(SaveFileName);            
            
            System.IO.FileStream fs = new System.IO.FileStream(SaveFileName, System.IO.FileMode.Create);
            {
                for (int intButtonCounter = 0; intButtonCounter < btnOptions_Array.Length; intButtonCounter++)
                {
                    buttonOptionItem btnTemp = btnOptions_Array[intButtonCounter];
                    btnTemp.Save(ref fs);
                }
            }
            fs.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Save();
            Hide(); 
        }


        public bool Toggle_Dictionary_Form_Mouse_Pop_Up_Definition
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Dictionary_Form_Mouse_Pop_Up_Definition].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Dictionary_Form_Mouse_Pop_Up_Definition].var = (bool)value; }
        }


        public bool Toggle_Word_List_Form_Mouse_Pop_Up_Definition
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Word_List_Form_Mouse_Pop_Up_Definition].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Word_List_Form_Mouse_Pop_Up_Definition].var = (bool)value; }
        }


        public bool Toggle_Numeral_Insert
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Numeral_Insert].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Numeral_Insert].var = (bool)value; }
        }
        
        public bool Toggle_Auto_Save
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Auto_Save].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Auto_Save].var = (bool)value; }
        }
        public bool Toggle_Two_Word_Swap
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Two_Word_Swap].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Two_Word_Swap].var = (bool)value; }
        }
        
        public bool Toggle_Insert_Search_Retains_Prefix_and_Suffix
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Insert_Search_Retains_Prefix_and_Suffix].var); }
            set { btnOptions_Array[(int)enuOptions.Toggle_Insert_Search_Retains_Prefix_and_Suffix].var = (bool)value; }
        }
        
        public bool Toggle_Word_Suggestion
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_Word_Suggestion].var); }
            set 
            {
                btnOptions_Array[(int)enuOptions.Toggle_Word_Suggestion].var 
                    = ck_RichTextBox.WordSuggestion_Toggle
                    = (bool)value;             
            }
        }        
        
          
        public bool Toggle_WordMap_AutoShutOff
        {
            get { return (bool)(btnOptions_Array[(int)enuOptions.Toggle_WordMap_AutoShutOff].var); }
            set 
            {
                btnOptions_Array[(int)enuOptions.Toggle_WordMap_AutoShutOff].var 
                    = ck_RichTextBox.WordSuggestion_Toggle
                    = (bool)value;             
            }
        }        
        
        
        public int Integer_Definition_Ports_List
        {
            get { return (int)(btnOptions_Array[(int)enuOptions.Integer_Definition_Ports_List].var); }
            set { btnOptions_Array[(int)enuOptions.Integer_Definition_Ports_List].var = (int)value; }
        }


        public int Integer_CopyCutter_AutoShutOffDelay
        {
            get { return (int)(btnOptions_Array[(int)enuOptions.Integer_CopyCutter_AutoShutOffDelay].var); }
            set { btnOptions_Array[(int)enuOptions.Integer_CopyCutter_AutoShutOffDelay].var = (int)value; }
        }

        public int Integer_Rhyme_Dictionary_MaxResults
        {
            get { return (int)(btnOptions_Array[(int)enuOptions.Integer_Rhyme_Dictionary_MaxResults].var); }
            set { btnOptions_Array[(int)enuOptions.Integer_Rhyme_Dictionary_MaxResults].var = (int)value; }
        }

        public int Integer_Backup_Files_Total_Number
        {
            get { return (int)(btnOptions_Array[(int)enuOptions.Integer_Backup_Files_Total_Number].var); }
            set { btnOptions_Array[(int)enuOptions.Integer_Backup_Files_Total_Number].var = (int)value; }
        }

        public int Integer_Backup_Time_Periods
        {
            get { return (int)(btnOptions_Array[(int)enuOptions.Integer_Backup_Time_Period].var); }
            set 
            { 
                btnOptions_Array[(int)enuOptions.Integer_Backup_Time_Period].var = (int)value;
                formWords.cBackUp.Timer_Delay_Set();
            }
        }

    }
}

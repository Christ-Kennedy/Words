using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Words
{
    public class classRTF_DataHandler 
    {
        static List<classRTF_Data> _lstFileInfo = new List<classRTF_Data>();
        public static List<classRTF_Data> lstFileInfo { get { return _lstFileInfo; } }

        public static void LoadProject()
        {
            
        }

        public class classRTF_Data
        {
            #region FileStream
            static BinaryFormatter formatter = new BinaryFormatter();
            public static classRTF_Data FS_Load(ref FileStream fs)
            {
                classRTF_Data cRetVal = new classRTF_Data();

                cRetVal.Chapter = (int)formatter.Deserialize(fs);
                cRetVal.Title = (string)formatter.Deserialize(fs);
                cRetVal.POV = (string)formatter.Deserialize(fs);
                cRetVal.Year = (int)formatter.Deserialize(fs);
                cRetVal.Month = (int)formatter.Deserialize(fs);
                cRetVal.Day = (int)formatter.Deserialize(fs);                

                return cRetVal;
            }
            public void FS_Save(ref FileStream fs)
            {
                formatter.Serialize(fs, Chapter);
                formatter.Serialize(fs, Title);
                formatter.Serialize(fs, POV);
                formatter.Serialize(fs, Year);
                formatter.Serialize(fs, Month);
                formatter.Serialize(fs, Day);
            }

            #endregion 

            public string Filename
            {
                get
                {
                    string strRetVal = cProject.Name
                                        + Title
                                        + " "
                                        +  (POV.Length > 0
                                                       ? "(" + POV + ")  "
                                                       : "")
                                        + " "
                                        + (Year > 0
                                                ? (Year.ToString() + "/" + Month.ToString()+ "/" + Day.ToString())
                                                : "" );
                    return strRetVal;
                }
            }

            public classProject cProject { get { return formWords.cProject; } }


            string strTitle = "";
            public string Title
            {
                get { return strTitle; }
                set { strTitle = value; }
            }
            string strPOV = "";
            public string POV
            {
                get { return strPOV; }
                set { strPOV = value; }
            }

            int _intYear = -1;
            public int Year
            {
                get { return _intYear; }
                set { _intYear = value; }
            }
            int _intMonth = -1;
            public int Month
            {
                get { return _intMonth; }
                set { _intMonth = value;                 }
            }
            int _intDay = -1;
            public int Day
            {
                get { return _intDay; }
                set { _intDay = value; }
            }
            int _intChapter = 1;
            public int Chapter
            {
                get { return _intChapter; }
                set { _intChapter = value; }
            }

            classTreeData _cTreeData = new classTreeData();
            public classTreeData cTreeData { get { return _cTreeData; } }
            public class classTreeData
            {
                classRTF_Data _classRTF_DataParent = null;
                public classRTF_Data Parent
                {
                    get { return _classRTF_DataParent; }
                    set { _classRTF_DataParent = value; }
                }
                classRTF_Data _classRTF_DataFirstChild = null;
                public classRTF_Data FirstChild
                {
                    get { return _classRTF_DataFirstChild; }
                    set { _classRTF_DataFirstChild = value; }
                }
                classRTF_Data _classRTF_DataNextSibling = null;
                public classRTF_Data NextSibling
                {
                    get { return _classRTF_DataNextSibling; }
                    set { _classRTF_DataNextSibling = value; }
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Words
{
    public class classDictionary
    {
        public int intMyIndex = -1;
        string _strSourceDirectory = "";
        public string strSourceDirectory
        {
            get { return _strSourceDirectory; }
            set
            {
                if (string.Compare(_strSourceDirectory.ToUpper(), value.ToUpper()) != 0)
                {
                    _strSourceDirectory = value;

                    classFileContent cFileContent = new classFileContent(_strSourceDirectory, "AAAAAAAA");
                    SearchTreeBuild_Content = !cFileContent.Definition.Contains("SearchTreeValidContent=False");
                    SearchTreeBuild_Heading = !cFileContent.Definition.Contains("SearchTreeValidHeading=False");
                }
            }
        }

        public enuFileExtensions eFileExtension = enuFileExtensions._num;

        public string strListFilesLeftFileNameAndDirectory;
        public static List<classDictionary> lstDictionaries;

        public bool bolInitfiles, bolHeadingListInitFiles;

        bool bolSearchTreeBuild_Heading = true;
        public bool SearchTreeBuild_Heading
        {
            get { return bolSearchTreeBuild_Heading; }
            set { bolSearchTreeBuild_Heading = value; }
        }
        bool bolSearchTreeBuild_Content = true;
        public bool SearchTreeBuild_Content
        {
            get { return bolSearchTreeBuild_Content; }
            set { bolSearchTreeBuild_Content = value; }
        }



        public class classFileData
        {
            int _int_BinRec_Index_NextAvailable;
            public int int_BinRec_Index_NextAvailable
            {
                get { return _int_BinRec_Index_NextAvailable; }
                set { _int_BinRec_Index_NextAvailable = value; }
            }

            int _int_LL_RecordIndex_NextAvailable;
            public int int_LL_RecordIndex_NextAvailable
            {
                get { return _int_LL_RecordIndex_NextAvailable; }
                set { _int_LL_RecordIndex_NextAvailable = value; }
            }

            public string str_BINFileNameAndDirectory, str_LLFileNameAndDirectory;
            public List<classFileStreamManager> lst_LL_SemFS = new List<classFileStreamManager>();
            public List<classFileStreamManager> lst_Bin_SemFS = new List<classFileStreamManager>();
        }

        public class classHeadingListInfo
        {
            public string Filename = "";
            public Semaphore sem = new Semaphore(1, 1);
            public int NumEntries = -1;

            public classDictionary cDictionary = null;
            public classHeadingListInfo(ref classDictionary cDictionary)
            {
                this.cDictionary = cDictionary;
            }

            bool _bolHeadingListCompleted = false;
            public bool bolHeadingListCompleted
            {
                get { return _bolHeadingListCompleted; }
                set { _bolHeadingListCompleted = value; }
            }
        }

        public classHeadingListInfo cHeadingListInfo = null;

        public classFileData[] cFileData = new classFileData[(int)enuFileDataTypes._num];

        public classInitFileInfo cInitFileInfo = new classInitFileInfo();
        public string Heading;
        public string strComplete;

        bool _bolCompleted = false;
        public bool bolCompleted
        {
            get { return _bolCompleted; }
            set
            {
                _bolCompleted = value;
            }
        }

        public classDictionary()
        {
            for (int intFileDataCounter = 0; intFileDataCounter < (int)enuFileDataTypes._num; intFileDataCounter++)
                cFileData[intFileDataCounter] = new classFileData();

            classDictionary cMyRef = this;
            cHeadingListInfo = new classHeadingListInfo(ref cMyRef);
        }


        public static classDictionary get(string strHeading)
        {
            strHeading = strHeading.ToUpper().Trim();
            for (int intDictionaryCounter = 0; intDictionaryCounter < lstDictionaries.Count; intDictionaryCounter++)
            {
                classDictionary cDictionary = lstDictionaries[intDictionaryCounter];
                string strTest = cDictionary.Heading.Trim().ToUpper();
                if (string.Compare(strHeading, strTest) == 0)
                    return cDictionary;
            }
            return null;
        }


        public void HeadingListSize_Set()
        {
            if (cHeadingListInfo.NumEntries > 0) return;
            cHeadingListInfo.sem.WaitOne();
            {
                FileStream fs = null;
                if (System.IO.File.Exists(cHeadingListInfo.Filename))
                    fs = new FileStream(cHeadingListInfo.Filename, FileMode.Open);
                else
                    fs = new FileStream(cHeadingListInfo.Filename, FileMode.Create);
                cHeadingListInfo.NumEntries = (int)(fs.Length / classHeadingList.BuildHeadingList_Size) + 1;
                fs.Close();
            }
            cHeadingListInfo.sem.Release();
        }
    }
}

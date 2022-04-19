using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ck_Objects;
using Math3;
using StringLibrary;

namespace Words
{
    public enum enuFileExtensions { txt, rtf, _num };
    public enum enuFileDataTypes { heading, content, _num };
    public class classBT_Dictionaries
    {
        static BackgroundWorker bckSearch
        {
            get { return formWords.instance.bckSearch; }
        }

        public static int intStandardFileNameLength = 8;
        const int intStandardBinWordSize = 40;
        const int MaxHeadingSize = 120;

        public static string strDictionaries_SourceDirectory = "c:\\Words_dictionaries\\";
        public static string strListFilesLeftFileName = "List_FilesLeft.fil";
        public static string strHeadingList_Filename = "HeadingList.fil";
        public static string strHeading_Bin_FileName = "Heading_";
        public static string strHeading_LL_FileName = "Heading_";
        public static string strContent_Bin_FileName = "Content_";
        public static string strContent_LL_FileName = "Content_";
        public static string str_complete = "English_Complete.txt";
        public static long lngFileListRecordSize, lng_Bin_RecordSize, lng_LL_RecordSize, lng_HeadingList_RecordSize;

        public static bool bolQuit;
        static classDictionary cBck_BuildBinSearch_CurrentDictionary = null;
        public static BackgroundWorker bck_BuildBinSearch = new BackgroundWorker();

        static Semaphore semSearch = new Semaphore(1, 1);
        public static Semaphore semBuild = new Semaphore(1, 1);

        static System.Random rnd = new System.Random();
        public static classBT_Dictionaries instance;

        public classBT_Dictionaries()
        {
            instance = this;
            new formBinSearchResults_();

            string strSearchPattern = "*dict";
            if (!System.IO.Directory.Exists(strDictionaries_SourceDirectory))
                System.IO.Directory.CreateDirectory(strDictionaries_SourceDirectory);
            string[] strSourceDirectories = System.IO.Directory.GetDirectories(strDictionaries_SourceDirectory, strSearchPattern);
            classDictionary.lstDictionaries = new List<classDictionary>();
            bool bolRebuildDictionary = false;
            setNumRecordsVariables();
            for (int intSubDirCounter = 0; intSubDirCounter < strSourceDirectories.Length; intSubDirCounter++)
            {
                classDictionary cThisDictionary = new classDictionary();
                cThisDictionary.intMyIndex = intSubDirCounter;
                cThisDictionary.strListFilesLeftFileNameAndDirectory = strSourceDirectories[intSubDirCounter] + "\\" + strListFilesLeftFileName;
                cThisDictionary.strSourceDirectory = strSourceDirectories[intSubDirCounter] + "\\";

                // init heading
                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].str_BINFileNameAndDirectory = strSourceDirectories[intSubDirCounter] + "\\" + strHeading_Bin_FileName;
                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].str_LLFileNameAndDirectory = strSourceDirectories[intSubDirCounter] + "\\" + strHeading_LL_FileName;
                cThisDictionary.cHeadingListInfo.Filename = strSourceDirectories[intSubDirCounter] + "\\" + strHeadingList_Filename;

                if (System.IO.File.Exists(cThisDictionary.cHeadingListInfo.Filename))
                    cThisDictionary.cHeadingListInfo.bolHeadingListCompleted = true;
                else
                {
                    string strSearchFileName = strDictionaries_SourceDirectory + cThisDictionary.cHeadingListInfo.Filename.Substring(strDictionaries_SourceDirectory.Length).Replace("\\", "_");
                    if (System.IO.File.Exists(strSearchFileName)) // search WordsDictionary main directory for source file
                    {
                        System.IO.File.Copy(strSearchFileName, cThisDictionary.cHeadingListInfo.Filename);
                        cThisDictionary.cHeadingListInfo.bolHeadingListCompleted = true;
                    }
                }

                // search for dictionary header file
                string strHeaderFilename = cThisDictionary.strSourceDirectory + "Header.rtf";
                if (!System.IO.File.Exists(strHeaderFilename))
                {  // if not found copy from the source code's working directory
                    string strHeader_Copy = System.IO.Directory.GetCurrentDirectory() + "\\" + cThisDictionary.strSourceDirectory.Substring(cThisDictionary.strSourceDirectory.Length - 9, 8) + "_Header.rtf";
                    if (System.IO.File.Exists(strHeader_Copy)) // search WordsDictionary main directory for source file
                        System.IO.File.Copy(strHeader_Copy, strHeaderFilename);
                }

                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].lst_Bin_SemFS = new List<classFileStreamManager>();
                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].lst_LL_SemFS = new List<classFileStreamManager>();

                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].int_BinRec_Index_NextAvailable = Bin_NumRecords_Get(ref cThisDictionary, enuFileDataTypes.heading);
                cThisDictionary.cFileData[(int)enuFileDataTypes.heading].int_LL_RecordIndex_NextAvailable = LL_NumRecords_Get(ref cThisDictionary, enuFileDataTypes.heading);

                // init content
                cThisDictionary.cFileData[(int)enuFileDataTypes.content].str_BINFileNameAndDirectory = strSourceDirectories[intSubDirCounter] + "\\" + strContent_Bin_FileName;
                cThisDictionary.cFileData[(int)enuFileDataTypes.content].str_LLFileNameAndDirectory = strSourceDirectories[intSubDirCounter] + "\\" + strContent_LL_FileName;

                cThisDictionary.cFileData[(int)enuFileDataTypes.content].lst_Bin_SemFS = new List<classFileStreamManager>();
                cThisDictionary.cFileData[(int)enuFileDataTypes.content].lst_LL_SemFS = new List<classFileStreamManager>();

                cThisDictionary.cFileData[(int)enuFileDataTypes.content].int_BinRec_Index_NextAvailable = Bin_NumRecords_Get(ref cThisDictionary, enuFileDataTypes.content);
                cThisDictionary.cFileData[(int)enuFileDataTypes.content].int_LL_RecordIndex_NextAvailable = LL_NumRecords_Get(ref cThisDictionary, enuFileDataTypes.content);

                cThisDictionary.bolCompleted = System.IO.File.Exists(strSourceDirectories[intSubDirCounter] + "\\English_Complete.txt");

                cThisDictionary.cInitFileInfo.cSearch.bolInit = System.IO.File.Exists(cThisDictionary.strListFilesLeftFileNameAndDirectory);
                cThisDictionary.bolInitfiles = !cThisDictionary.cInitFileInfo.cSearch.bolInit;

                cThisDictionary.cInitFileInfo.cHeadingList.bolInit = System.IO.File.Exists(cThisDictionary.cHeadingListInfo.Filename);
                cThisDictionary.bolHeadingListInitFiles = !cThisDictionary.cInitFileInfo.cHeadingList.bolInit;

                string strHeadingFilenameBase = cThisDictionary.strSourceDirectory + "LetterA\\aaaaaaaa.";
                for (int intFileExtensionCounter = 0; intFileExtensionCounter < (int)enuFileExtensions._num; intFileExtensionCounter++)
                {
                    enuFileExtensions eFileExtension = (enuFileExtensions)intFileExtensionCounter;
                    string strFilenameTest = strHeadingFilenameBase + eFileExtension.ToString();
                    if (System.IO.File.Exists(strFilenameTest))
                    {
                        cThisDictionary.eFileExtension = eFileExtension;
                        break;
                    }
                }

                classFileContent cFileContent = new classFileContent(cThisDictionary.strSourceDirectory, "aaaaaaaa");
                cThisDictionary.Heading = cFileContent.Heading;

                //System.Diagnostics.Debug.Print(cThisDictionary.Heading);

                classDictionary.lstDictionaries.Add(cThisDictionary);
                formFeedback.instance.Txt_Add();
                formFeedback.instance.Message(intSubDirCounter, "Pending : " + cThisDictionary.Heading.Trim());

                if (!cThisDictionary.bolCompleted)
                    bolRebuildDictionary = true;
            }

            IEnumerable<classDictionary> query_DictionaryList = classDictionary.lstDictionaries.OrderBy(dictionary => dictionary.Heading);
            classDictionary.lstDictionaries = (List<classDictionary>)query_DictionaryList.ToList<classDictionary>();

            if (bolRebuildDictionary)
                formFeedback.instance.Show();

            lngFileListRecordSize = ListFile_GetRecordSize();
            setNumRecordsVariables();
            bck_BuildBinSearch_Init();
            if (bolRebuildDictionary)
                bck_BuildBinSearch.RunWorkerAsync();
        }




        #region BackgroundWorker_BuildBinSearch

        static void bck_BuildBinSearch_Init()
        {
            bck_BuildBinSearch.DoWork += Bck_BuildBinSearch_DoWork;
            bck_BuildBinSearch.ProgressChanged += Bck_BuildBinSearch_ProgressChanged;
            bck_BuildBinSearch.RunWorkerCompleted += Bck_BuildBinSearch_RunWorkerCompleted;
            bck_BuildBinSearch.WorkerReportsProgress = true;
            bck_BuildBinSearch.WorkerSupportsCancellation = true;
        }

        private static void Bck_BuildBinSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            formFeedback.instance.Visible = false;
            bolQuit = false;
            bck_BuildBinSearch.CancelAsync();
            semBuild.Release();
        }

        private static void Bck_BuildBinSearch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (cBck_BuildBinSearch_CurrentDictionary.bolInitfiles)
                try
                {
                    formFeedback.instance.Message(classDictionary.lstDictionaries.IndexOf(cBck_BuildBinSearch_CurrentDictionary),
                                                  cBck_BuildBinSearch_CurrentDictionary.Heading.Trim()
                                                    + " : dirs ("
                                                        + cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.cSearch.intCurrentDir.ToString()
                                                        + "/"
                                                        + (cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.strDir != null
                                                                                                                       ? cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.strDir.Length.ToString()
                                                                                                                       : "null")
                                                        + ")"
                                                    + "\tfiles ("
                                                        + cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.cSearch.intCurrentFile.ToString()
                                                        + "/"
                                                        + (cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.strFiles != null
                                                                                                                        ? cBck_BuildBinSearch_CurrentDictionary.cInitFileInfo.strFiles.Length.ToString()
                                                                                                                        : "")
                                                        + ")");
                }
                catch (Exception)
                {
                    //MessageBox.Show("error:" + e1.Message);
                }
            else if (cBck_BuildBinSearch_CurrentDictionary.bolCompleted)
            {
                formFeedback.instance.Message(classDictionary.lstDictionaries.IndexOf(cBck_BuildBinSearch_CurrentDictionary), cBck_BuildBinSearch_CurrentDictionary.Heading.Trim() + " : Complete");
            }
            else
            {
                formFeedback.instance.Message(classDictionary.lstDictionaries.IndexOf(cBck_BuildBinSearch_CurrentDictionary), cBck_BuildBinSearch_CurrentDictionary.Heading.Trim() + " : " + intCurrentRecord.ToString());
            }
        }

        private static void Bck_BuildBinSearch_DoWork(object sender, DoWorkEventArgs e)
        {

            semBuild.WaitOne();
            for (int intDictionaryCounter = 0; intDictionaryCounter < classDictionary.lstDictionaries.Count; intDictionaryCounter++)
            {
                cBck_BuildBinSearch_CurrentDictionary = classDictionary.lstDictionaries[intDictionaryCounter];
                bck_BuildBinSearch.ReportProgress(0);
                if (!cBck_BuildBinSearch_CurrentDictionary.bolCompleted)
                {
                    do
                    {
                        if (cBck_BuildBinSearch_CurrentDictionary.bolInitfiles)
                        {
                            initFiles(ref cBck_BuildBinSearch_CurrentDictionary);
                            bck_BuildBinSearch.ReportProgress(0);
                        }
                        else
                            handleNextFile(ref cBck_BuildBinSearch_CurrentDictionary);
                    } while (!cBck_BuildBinSearch_CurrentDictionary.bolCompleted && !bolQuit && !bck_BuildBinSearch.CancellationPending);

                }
                if (bolQuit)
                    return;
                bck_BuildBinSearch.ReportProgress(0);
            }
        }

        private static void Bck_Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            formFeedback.instance.Hide();
        }

        private static void Bck_Search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        public static List<class_LL_Record> lstResult_LL = null;

        static string strPrefix = "";
        public static string Prefix
        {
            get { return strPrefix; }
        }

        static string strSuffix = "";
        public static string Suffix
        {
            get { return strSuffix; }
        }

        static string strWordFound = "";
        public static string WordFound { get { return strWordFound; } }

        static string strSearchString = "";
        public static List<class_LL_Record> BinTree_Search(ref classDictionary cDictionary,
                                                               string _strSearchString,
                                                               enuFileDataTypes eFileDataType)
        {
            strSearchString = _strSearchString;
            semSearch.WaitOne();

            if (strSearchString.Length == 0)
            {
                semSearch.Release();
                return null;
            }
            List<string> lstWords = classStringLibrary.getFirstWords(classStringLibrary.Deaccent(strSearchString.Trim().ToLower()));
            if (lstWords == null || lstWords.Count == 0)
            {
                semSearch.Release();
                return new List<class_LL_Record>();
            }


            List<class_BinTree_Record> lstResults = new List<class_BinTree_Record>();

            for (int intWordCounter = 0; intWordCounter < lstWords.Count; intWordCounter++)
            { 
                for (int intPrefixCounter = 0; intPrefixCounter < ck_RichTextBox.lstPrefixes.Count; intPrefixCounter++)
                {
                    for (int intSuffixCounter = 0; intSuffixCounter < ck_RichTextBox.lstSuffixes.Count; intSuffixCounter++)
                    {
                        string strWord = lstWords[intWordCounter];
                        strPrefix = ck_RichTextBox.lstPrefixes[intPrefixCounter];
                        strSuffix = ck_RichTextBox.lstSuffixes[intSuffixCounter];

                        bool bolSearch =  intPrefixCounter == 0 && intSuffixCounter == 0;

                        if (!bolSearch)
                        {
                            bool bolPrefix = strPrefix.Length == 0
                                                ||
                                             (strWord.Length > strPrefix.Length
                                               && string.Compare(strWord.ToLower().Substring(0, strPrefix.Length), strPrefix.ToLower()) == 0);
                            if (bolPrefix)
                                strWord = strWord.Substring(strPrefix.Length);

                            bool bolSuffix = strSuffix.Length == 0;
                            if (!bolSuffix)
                            {
                                if (strWord.Length > strSuffix.Length)
                                {
                                    string strTemp = strWord.Substring(strWord.Length - strSuffix.Length);
                                    if (string.Compare(strTemp.ToLower(), strSuffix.ToLower()) == 0)
                                    {
                                        bolSuffix = true;
                                        strWord = strWord.Substring(0, strWord.Length - strSuffix.Length);
                                    }
                                }
                            }
                            bolSearch = bolSuffix && bolPrefix;
                        }

                        if (bolSearch)
                        {
                            class_BinTree_Record c_BinRec = BinTree_GetRecord(ref cDictionary, strWord, eFileDataType);

                            if (c_BinRec != null && c_BinRec.LL >= 0)
                            {
                                strWordFound = strWord;
                                lstResults.Add(c_BinRec);
                                goto exitPrefixSuffixTest;
                            }
                        }
                    }
                }
                strPrefix
                    = strSuffix
                    = "";
            exitPrefixSuffixTest:
                ;
            }

            if (lstResults.Count < 1)
            {
                lstResult_LL = new List<class_LL_Record>();
                semSearch.Release();
                return null;
            }
            
            IEnumerable<class_BinTree_Record> query = lstResults.OrderBy(bin => bin.weight);
            lstResults = (List<class_BinTree_Record>)query.ToList<class_BinTree_Record>();

            AlphaTrees.cAlphaTree[] cAT_PruneLists = new AlphaTrees.cAlphaTree[2];
            int intIndex = 0, intAltIndex = 1;
            if (lstWords.Count > 1)
            {
                for (int intResultCounter = 0; intResultCounter < lstResults.Count; intResultCounter++)
                {
                    intIndex = intAltIndex;
                    intAltIndex = (intIndex + 1) % 2;
                    cAT_PruneLists[intIndex] = new AlphaTrees.cAlphaTree();
                    class_BinTree_Record cBin = lstResults[intResultCounter];
                    List<class_LL_Record> lst_LL = LL_TraverseWeightTree(ref cDictionary, cBin.LL, eFileDataType);
                    while (lst_LL.Count > 0)
                    {
                        if (bckSearch.CancellationPending)
                            goto quitSearch;
                        class_LL_Record c_LL = lst_LL[0];

                        lst_LL.Remove(c_LL);
                        do
                        {
                            object objLL = c_LL;

                            if (intResultCounter == 0)
                            {
                                cAT_PruneLists[intIndex].Insert(ref objLL, c_LL.FileName.Trim());
                            }
                            else if (c_LL != null)
                            {
                                if (cAT_PruneLists[intAltIndex] != null)
                                {
                                    class_LL_Record cLL_InTree = (class_LL_Record)cAT_PruneLists[intAltIndex].Search(c_LL.FileName.Trim());

                                    if (cLL_InTree != null)
                                    {
                                        objLL = (cLL_InTree.Weight < c_LL.Weight
                                                                   ? cLL_InTree
                                                                   : c_LL);
                                        cAT_PruneLists[intIndex].Insert(ref objLL, c_LL.FileName.Trim());
                                        List<AlphaTrees.cAlphaTree.classTraversalReport_Record> lstTreeTraversal_Test = cAT_PruneLists[intIndex].TraverseTree_InOrder();
                                    }
                                }
                            }

                            if (c_LL.next >= 0)
                                c_LL = LL_Record_Load(ref cDictionary, c_LL.next, eFileDataType);
                            else
                                c_LL = null;
                        } while (c_LL != null);
                    }
                }


                lstResult_LL = new List<class_LL_Record>();
                List<AlphaTrees.cAlphaTree.classTraversalReport_Record> lstTreeTraversal = cAT_PruneLists[intIndex].TraverseTree_InOrder();
                for (int intResultCounter = 0; intResultCounter < lstTreeTraversal.Count; intResultCounter++)
                {
                    if (bckSearch.CancellationPending)
                        goto quitSearch;
                    AlphaTrees.cAlphaTree.classTraversalReport_Record cItem = lstTreeTraversal[intResultCounter];
                    class_LL_Record cLL = (class_LL_Record)cItem.data;
                    lstResult_LL.Add(cLL);
                }

                IEnumerable<class_LL_Record> queryLL = lstResult_LL.OrderByDescending(ll => ll.Weight);
                lstResult_LL = (List<class_LL_Record>)queryLL.ToList<class_LL_Record>();
            }
            else
            {
                class_BinTree_Record cBin = lstResults[0];
                List<class_LL_Record> lst_LL = LL_TraverseWeightTree(ref cDictionary, cBin.LL, eFileDataType);
                lstResult_LL = new List<class_LL_Record>();

                while (lst_LL.Count > 0)
                {
                    class_LL_Record cLL = lst_LL[0];
                    class_LL_Record cLL_Start = lst_LL[0];
                    lst_LL.Remove(cLL);                    
                    lstResult_LL.Add(cLL);
                    while (cLL.next > 0)
                    {

                        if (bckSearch.CancellationPending)
                            goto quitSearch;
                        cLL = LL_Record_Load(ref cDictionary, cLL.next, eFileDataType);

                        if (cLL == cLL_Start)
                            break;

                        lstResult_LL.Add(cLL);

                    }
                }
            }

            //foreach(class_LL_Record cLL in lstResult_LL)
            //{

            //    //////////////////////////////////////////////////////////////////////////////////
            //    System.Diagnostics.Debug.Print(cLL.FileName);
            //    //////////////////////////////////////////////////////////////////////////////////
            //}




            semSearch.Release();
            return lstResult_LL;

        quitSearch:
            lstResult_LL.Clear();
            semSearch.Release();
            return lstResult_LL;
        }

        static List<class_LL_Record> lstLL_TraversalResult = new List<class_LL_Record>();
        public static List<class_LL_Record> LL_TraverseWeightTree(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            lstLL_TraversalResult = new List<class_LL_Record>();
            LL_TraverseWeightTree_Recursion(ref cDictionary, index, eFileDataType);
            return lstLL_TraversalResult;
        }

        static void LL_TraverseWeightTree_Recursion(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            class_LL_Record cLL_Rec = LL_Record_Load(ref cDictionary, index, eFileDataType);
            if (cLL_Rec.left >= 0)
                LL_TraverseWeightTree_Recursion(ref cDictionary, cLL_Rec.left, eFileDataType);

            lstLL_TraversalResult.Add(cLL_Rec);

            if (cLL_Rec.right >= 0)
                LL_TraverseWeightTree_Recursion(ref cDictionary, cLL_Rec.right, eFileDataType);
        }


        //public static void Rebuild(ref classDictionary cDictionary, bool bolContinuePreviouslyStartedRebuild)
        //{
        //    setNumRecordsVariables();
        //    lngFileListRecordSize = ListFile_GetRecordSize();
        //    cDictionary.bolInitfiles = !bolContinuePreviouslyStartedRebuild;
        //    bolQuit
        //        = cDictionary.cInitFileInfo.cSearch.bolInit
        //        = false;
        //    formFeedback.instance.Show();
        //    bck_BuildBinSearch.RunWorkerAsync();
        //}

        #endregion

        public static void setNumRecordsVariables()
        {
            lng_Bin_RecordSize = BinRec_Size_Get();

            lng_LL_RecordSize = LL_RecordSize_Get();
            lng_HeadingList_RecordSize = classHeadingList.BuildHeadingList_Size;
        }

        #region "initfiles"
        public static void initFiles(ref classDictionary cDictionary)
        {
            if (!cDictionary.cInitFileInfo.cSearch.bolInit)
            {
                cDictionary.cInitFileInfo.cSearch.intNumRec = 0;

                if (System.IO.File.Exists(cDictionary.strListFilesLeftFileNameAndDirectory))
                    System.IO.File.Delete(cDictionary.strListFilesLeftFileNameAndDirectory);

                int intHeadingBinCounter = 0;
                string strHeadingBinFileName = BinRec_FileName_Get(ref cDictionary, intHeadingBinCounter, enuFileDataTypes.heading);

                while (System.IO.File.Exists(strHeadingBinFileName))
                {
                    System.IO.File.Delete(strHeadingBinFileName);
                    strHeadingBinFileName = BinRec_FileName_Get(ref cDictionary, ++intHeadingBinCounter, enuFileDataTypes.heading);
                }

                int intHeadingLLCounter = 0;
                string strHeadingLLFileName = LLRec_FileName_Get(ref cDictionary, intHeadingLLCounter, enuFileDataTypes.heading);
                while (System.IO.File.Exists(strHeadingLLFileName))
                {
                    System.IO.File.Delete(strHeadingLLFileName);
                    strHeadingLLFileName = LLRec_FileName_Get(ref cDictionary, ++intHeadingLLCounter, enuFileDataTypes.heading);
                }
                if (System.IO.File.Exists(cDictionary.strSourceDirectory + str_complete))
                    System.IO.File.Decrypt(cDictionary.strSourceDirectory + str_complete);

                int intContentBinCounter = 0;
                string strContentBinFileName = BinRec_FileName_Get(ref cDictionary, intContentBinCounter, enuFileDataTypes.content);

                while (System.IO.File.Exists(strContentBinFileName))
                {
                    System.IO.File.Delete(strContentBinFileName);
                    strContentBinFileName = BinRec_FileName_Get(ref cDictionary, ++intContentBinCounter, enuFileDataTypes.content);
                }

                int intContentLLCounter = 0;
                string strContentLLFileName = LLRec_FileName_Get(ref cDictionary, intContentLLCounter, enuFileDataTypes.content);
                while (System.IO.File.Exists(strContentLLFileName))
                {
                    System.IO.File.Delete(strContentLLFileName);
                    strContentLLFileName = LLRec_FileName_Get(ref cDictionary, ++intContentLLCounter, enuFileDataTypes.content);
                }
                if (System.IO.File.Exists(cDictionary.strSourceDirectory + str_complete))
                    System.IO.File.Decrypt(cDictionary.strSourceDirectory + str_complete);

                cDictionary.cInitFileInfo.cSearch.bolNewDirectory = true;
                cDictionary.cInitFileInfo.cSearch.intCurrentDir = 0;

                cDictionary.cFileData[(int)enuFileDataTypes.heading].int_BinRec_Index_NextAvailable
                    = cDictionary.cFileData[(int)enuFileDataTypes.heading].int_LL_RecordIndex_NextAvailable
                    = cDictionary.cFileData[(int)enuFileDataTypes.content].int_BinRec_Index_NextAvailable
                    = cDictionary.cFileData[(int)enuFileDataTypes.content].int_LL_RecordIndex_NextAvailable
                    = 0;

                cDictionary.cInitFileInfo.cSearch.bolInit = true;
            }

            List<string> lstDirectories = new List<string>();
            lstDirectories.Add(cDictionary.strSourceDirectory + "altSub");
            lstDirectories.AddRange(System.IO.Directory.GetDirectories(cDictionary.strSourceDirectory, "Letter*").ToList<string>());
            cDictionary.cInitFileInfo.strDir = lstDirectories.ToArray();

            FileStream fs = null;
            string strFileName = cDictionary.strListFilesLeftFileNameAndDirectory;
            if (System.IO.File.Exists(strFileName))
                fs = new FileStream(strFileName, FileMode.Open);
            else
                fs = new FileStream(strFileName, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            char chrDir = 'a';
            List<int> lstRecords = new List<int>();
            //cDictionary.cInitFileInfo.bolNewDirectory = true;
            do
            {
                if (cDictionary.cInitFileInfo.cSearch.bolNewDirectory)
                {
                    cDictionary.cInitFileInfo.cSearch.bolNewDirectory = false;
                    List<string> lstFiles = new List<string>();
                    for (int intExCounter = 0; intExCounter < (int)enuFileExtensions._num; intExCounter++)
                    {
                        string strFileExtention = "*." + ((enuFileExtensions)intExCounter).ToString();
                        lstFiles.AddRange(System.IO.Directory.GetFiles(cDictionary.cInitFileInfo.strDir[cDictionary.cInitFileInfo.cSearch.intCurrentDir], strFileExtention).ToList<string>());
                        chrDir = char.ToLower(cDictionary.cInitFileInfo.strDir[cDictionary.cInitFileInfo.cSearch.intCurrentDir][cDictionary.cInitFileInfo.strDir[cDictionary.cInitFileInfo.cSearch.intCurrentDir].Length - 1]);
                        cDictionary.cInitFileInfo.cSearch.intCurrentFile = 0;

                    }
                    cDictionary.cInitFileInfo.strFiles = lstFiles.ToArray<string>();
                }

                if (cDictionary.cInitFileInfo.strFiles != null && cDictionary.cInitFileInfo.cSearch.intCurrentFile < cDictionary.cInitFileInfo.strFiles.Length)
                {
                    cDictionary.cInitFileInfo.cSearch.intNumRec++;

                    string strRecord = getFileName(cDictionary.cInitFileInfo.strFiles[cDictionary.cInitFileInfo.cSearch.intCurrentFile++]).ToLower();

                    if (!Char.IsLetter(strRecord[0]))
                        lstRecords.Add(cDictionary.cInitFileInfo.cSearch.intNumRec);

                    fs.Position = ListFile_GetPosition(cDictionary.cInitFileInfo.cSearch.intNumRec);
                    formatter.Serialize(fs, strRecord);
                }
                else
                {
                    cDictionary.cInitFileInfo.cSearch.intCurrentDir++;
                    if (cDictionary.cInitFileInfo.cSearch.intCurrentDir < cDictionary.cInitFileInfo.strDir.Length)
                        cDictionary.cInitFileInfo.cSearch.bolNewDirectory = true;
                    else
                    {
                        cDictionary.bolInitfiles = false;
                        string strNumRecords = getFileName(cDictionary.cInitFileInfo.cSearch.intNumRec);

                        fs.Position = ListFile_GetPosition(0);
                        formatter.Serialize(fs, strNumRecords);

                        cDictionary.cInitFileInfo.cSearch.bolInit = false;
                    }
                }
                if (cDictionary.cInitFileInfo.cSearch.intNumRec % 32 == 0)
                    bck_BuildBinSearch.ReportProgress(0);
            } while (cDictionary.bolInitfiles);

            if (fs != null) fs.Close();
        }

        public static string StandardHeading(string strHeading)
        {
            strHeading = strHeading.PadRight(MaxHeadingSize, ' ');
            strHeading = strHeading.Substring(0, MaxHeadingSize);
            return strHeading;
        }

        /// <summary>
        /// formats FileName for bin-tree file insertion
        /// </summary>
        /// <param name="strInputFileName"></param>
        /// <returns></returns>
        public static string getFileName(string strInputFileName)
        {
            strInputFileName = strInputFileName.ToLower().Trim();

            if (strInputFileName.Length > 12)
                strInputFileName = strInputFileName.Substring(strInputFileName.Length - 12, 8);

            strInputFileName = strInputFileName.PadLeft(intStandardFileNameLength, ' ').Substring(0, intStandardFileNameLength);
            return strInputFileName;
        }

        public static enuFileExtensions GetFileExtension(string strFileName)
        {
            int intLastIndex = strFileName.LastIndexOf('.');
            if (intLastIndex >= 0 && intLastIndex < strFileName.Length - 1)
            {
                string strExtension = strFileName.Substring(intLastIndex + 1);

                for (int intCounter = 0; intCounter < (int)enuFileExtensions._num; intCounter++)
                {
                    enuFileExtensions eFileExtension = (enuFileExtensions)intCounter;
                    string strTest = eFileExtension.ToString();
                    if (strTest.Length == strExtension.Length)
                    {
                        if (string.Compare(strTest, strExtension) == 0)
                            return eFileExtension;
                    }
                }
            }
            return enuFileExtensions._num;
        }

        /// <summary>
        /// formats FileName for bin-tree insertion
        /// </summary>
        /// <param name="intFileName"></param>
        /// <returns></returns>
        static string getFileName(int intFileName) { return getFileName(intFileName.ToString().PadLeft(intStandardFileNameLength)); }

        public static long ListFile_GetRecordSize()
        {
            string strTempFileName = System.IO.Directory.GetCurrentDirectory() + "\\tempFileName.flx";
            if (System.IO.File.Exists(strTempFileName))
                System.IO.File.Delete(strTempFileName);
            FileStream fs = null;
            long lngRetVal = 0;
            try
            {
                classDictionary cRefDictionary = classDictionary.lstDictionaries[0];
                fs = new FileStream(strTempFileName, FileMode.Create);
                string strTestFileName = getFileName("01234567");
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, strTestFileName);
                lngRetVal = (long)fs.Position;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }
            if (fs != null) fs.Close();
            return lngRetVal;
        }


        static long ListFile_GetPosition(int index) { return (index) * lngFileListRecordSize + 1; }

        static string ListFile_LoadRecord(ref classDictionary cDictionary, int index)
        {
            FileStream fs = null;
            string strRetVal = "";
            string strFileName = cDictionary.strListFilesLeftFileNameAndDirectory;
            if (System.IO.File.Exists(strFileName))
            {
                try
                {
                    fs = new FileStream(strFileName, FileMode.Open);
                    fs.Position = ListFile_GetPosition(index);

                    BinaryFormatter formatter = new BinaryFormatter();
                    strRetVal = (string)formatter.Deserialize(fs);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("error :" + e.Message);
                }
                if (fs != null) fs.Close();
                return strRetVal;
            }
            else
                return "";
        }

        static void ListFile_SaveRecord(ref classDictionary cDictionary, int index, string strRecord)
        {
            FileStream fs = null;

            string strFileName = cDictionary.strListFilesLeftFileNameAndDirectory;
            try
            {
                if (System.IO.File.Exists(strFileName))
                    fs = new FileStream(strFileName, FileMode.Open);
                else
                    fs = new FileStream(strFileName, FileMode.Create);

                BinaryFormatter formatter = new BinaryFormatter();
                fs.Position = ListFile_GetPosition(index);
                formatter.Serialize(fs, strRecord);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }
            if (fs != null) fs.Close();
        }
        #endregion

        #region "Handle files"

        static void frmFeedback_Disposed(object sender, EventArgs e)
        {
            formFeedback.instance.Hide();
            bck_BuildBinSearch.CancelAsync();
        }
        static int intCurrentRecord = 0;
        static void handleNextFile(ref classDictionary cDictionary)
        {
            try
            {
                intCurrentRecord = Convert.ToInt32(ListFile_LoadRecord(ref cDictionary, 0));
                if (bck_BuildBinSearch.CancellationPending)
                    return;

                if (intCurrentRecord > 0)
                {
                    try
                    {
                        int intRndRecord = (int)(rnd.NextDouble() * (intCurrentRecord - 1)) + 1;

                        string strFileName = ListFile_LoadRecord(ref cDictionary, intRndRecord).ToUpper();
                        string strLastRecord = ListFile_LoadRecord(ref cDictionary, intCurrentRecord);
                        ListFile_SaveRecord(ref cDictionary, intRndRecord, strLastRecord);
                        intCurrentRecord--;
                        if (intCurrentRecord % 8 == 0)
                            bck_BuildBinSearch.ReportProgress(0);
                        ListFile_SaveRecord(ref cDictionary, 0, getFileName(intCurrentRecord));
                        try
                        {
                            if (string.Compare(strFileName, "AAAAAAAA") != 0
                                && !strFileName.Contains("BACKUP")
                                && !strFileName.Contains("SEAR"))
                                AppendFile(ref cDictionary, strFileName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("error: " + e.Message);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    cDictionary.bolCompleted = true;
                    string str_Complete = cDictionary.strSourceDirectory + str_complete;
                    System.IO.File.WriteAllText(str_Complete, DateTime.Now.ToString());
                }
            }
            catch (Exception)
            {
            }
        }


        static bool validWord(string strWord)
        {
            string strValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZĀAĒEĪIŌOŪUāaēeīiōoūuÀÁÂÃÄÅàáâãäåÇçÈÉÊËèéêëḥÌÍÎÏ¡ìíîïïíÐÑñÒÓÔÕÖðòóôõöōøØŠšÙÚÛÜùúûüxÝŸýÿŽžÆŒæœ$&?!:;.,[]()«»-­­_=+/`'₁₂₅₆₀²³ 0123456789";
            strWord = strWord.Trim();
            for (int intChrIndex = 0; intChrIndex < strWord.Length; intChrIndex++)
            {
                char c = strWord[intChrIndex];
                if (!strValidCharacters.Contains(c))
                    return false;
            }
            return true;
        }

        public static void AppendFile(ref classDictionary cDictionary, string strFileName)
        {

            int intNumFail = 0;
        AppendFile_Start:
            classFileContent.enuComponentsRequested eComponentRequested = classFileContent.enuComponentsRequested.HeadingContent;
            if (!cDictionary.SearchTreeBuild_Heading && !cDictionary.SearchTreeBuild_Content)
                eComponentRequested = classFileContent.enuComponentsRequested.None;
            else if (!cDictionary.SearchTreeBuild_Heading)
                eComponentRequested = classFileContent.enuComponentsRequested.Content;
            else if (!cDictionary.SearchTreeBuild_Content)
                eComponentRequested = classFileContent.enuComponentsRequested.Heading;

            classFileContent cFileContent = new classFileContent(cDictionary.strSourceDirectory, strFileName, eComponentRequested);
            if (cFileContent.Heading != null && cFileContent.Definition != null)
            {
                string strHeading = cFileContent.Heading.Trim().Replace("*", "");

                if (!validWord(strHeading))
                {
                    string strFileNameAndDirectory = classFileContent.getFileNameAndDirectory(cDictionary.strSourceDirectory, strFileName);
                    if (intNumFail > 0)
                    {
                        System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
                        soundPlayer.SoundLocation = @"C:\Users\Christ\Documents\C#\Projects - Writing\Words\bin\Debug\te_ale.wav";
                        soundPlayer.Play();
                        using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                        {
                            pProcess.StartInfo.FileName = @"C:\Program Files\Windows NT\Accessories\wordpad.exe";
                            pProcess.StartInfo.Arguments = strFileNameAndDirectory + ".rtf";
                            pProcess.StartInfo.UseShellExecute = false;
                            pProcess.StartInfo.RedirectStandardOutput = true;
                            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                            pProcess.Start();
                            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                            pProcess.WaitForExit();
                        }
                        string strErrorMsg = "error file:" + strFileName;
                        //MessageBox.Show(strErrorMsg);
                        //System.Diagnostics.Debug.Print(strErrorMsg);
                    }

                    else if (System.IO.File.Exists(strFileNameAndDirectory + ".rtf"))
                    {

                        RichTextBox rtx = new RichTextBox();
                        rtx.LoadFile(strFileNameAndDirectory + ".rtf");
                        {
                            int intCounter = 0;
                            rtx.Select(intCounter, 1);
                            while (rtx.SelectionFont.Size == 28)
                                rtx.Select(++intCounter, 1);

                            rtx.Select(intCounter, 0);
                            rtx.SelectedText = "\r\n";
                        }
                        rtx.SaveFile(strFileNameAndDirectory + ".rtf");
                    }
                    else
                    {

                        System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
                        soundPlayer.SoundLocation = @"C:\Users\Christ\Documents\C#\Projects - Writing\Words\bin\Debug\te_ale.wav";
                        soundPlayer.Play();
                        MessageBox.Show("error load file : " + strFileNameAndDirectory);
                    }

                    intNumFail++;
                    goto AppendFile_Start;
                }


                string[] strText = new string[(int)enuFileDataTypes._num];


                strText[(int)enuFileDataTypes.heading] = (cFileContent.Heading.Trim()
                                                            + " "
                                                            + ((cDictionary.SearchTreeBuild_Heading && cFileContent.alt_Heading != null)
                                                                                         ? cFileContent.alt_Heading
                                                                                         : "")).ToLower();
                strText[(int)enuFileDataTypes.content] = ((cDictionary.SearchTreeBuild_Content && cFileContent.Definition != null)
                                                                                    ? cFileContent.Definition
                                                                                    : "").ToLower();
                try
                {
                    string strControls = "\r\n\t";
                    for (int intCon = 0; intCon < strControls.Length; intCon++)
                    {
                        for (int intFileDataTypeCounter = 0; intFileDataTypeCounter < (int)enuFileDataTypes._num; intFileDataTypeCounter++)
                            while (strText[intFileDataTypeCounter].Contains(strControls[intCon]))
                                strText[intFileDataTypeCounter] = strText[intFileDataTypeCounter].Replace(strControls[intCon], ' ');
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    for (int intFileDataTypeCounter = 0; intFileDataTypeCounter < (int)enuFileDataTypes._num; intFileDataTypeCounter++)
                    {
                        List<string> lstWords = classStringLibrary.getFirstWords(classStringLibrary.Deaccent(strText[intFileDataTypeCounter]));
                        if (lstWords != null)
                        {
                            int intIndex = 0;
                            while (intIndex < lstWords.Count && intIndex >= 0)
                            {
                                class_LL_Record cLL = new class_LL_Record(ref cDictionary, (enuFileDataTypes)intFileDataTypeCounter);
                                cLL.FileName = getFileName(strFileName);
                                cLL.strHeading = strHeading;
                                string strWord = lstWords[intIndex];
                                try
                                {
                                    while (lstWords.Contains(strWord))
                                    {
                                        cLL.Weight++;
                                        lstWords.Remove(strWord);
                                    }
                                }
                                catch (Exception)
                                {
                                }

                                BinTree_Insert(ref cDictionary, strWord, ref cLL, (enuFileDataTypes)intFileDataTypeCounter);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion


        #region "Bin tree"
        static long BinRec_Size_Get()
        {
            string strTempFileName = System.IO.Directory.GetCurrentDirectory() + "\\tempFileName.flx";
            if (System.IO.File.Exists(strTempFileName))
                System.IO.File.Delete(strTempFileName);
            FileStream fs = null;
            long lngRetVal = 0;
            try
            {
                fs = new FileStream(strTempFileName, FileMode.Create);
                class_BinTree_Record cBinRec = new class_BinTree_Record();

                BinaryFormatter formatter = new BinaryFormatter();
                fs.Position = 0;

                formatter.Serialize(fs, cBinRec.left);
                formatter.Serialize(fs, cBinRec.right);
                formatter.Serialize(fs, cBinRec.LL);

                formatter.Serialize(fs, getStandardWordSize("Chibougamou"));
                formatter.Serialize(fs, cBinRec.weight);

                formatter.Serialize(fs, (long)0);                       // make record size bigger than necessary

                lngRetVal = (long)fs.Position;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }

            if (fs != null) fs.Close();
            return lngRetVal;
        }

        static string getStandardWordSize(string strWord)
        {
            return strWord.PadRight(40, ' ').Substring(0, 40);
        }

        public static int Bin_NumRecords_Get(ref classDictionary cDictionary, enuFileDataTypes eFileDataType)
        {
            string str_Path = cDictionary.strSourceDirectory;

            string strSearchPattern = eFileDataType.ToString() + "_" + "*.bin";


            List<string> lstBinFiles = System.IO.Directory.GetFiles(str_Path, strSearchPattern).ToList<string>();

            if (lstBinFiles.Count > 0)
            {
                string strLastFile = lstBinFiles[0];
                for (int intCounter = 0; intCounter < lstBinFiles.Count; intCounter++)
                    if (string.Compare(strLastFile, lstBinFiles[intCounter]) < 0)
                        strLastFile = lstBinFiles[intCounter];

                FileStream fs = null;
                int intRetVal = 0;
                try
                {
                    fs = new FileStream(strLastFile, FileMode.Open);
                    intRetVal = (lstBinFiles.Count - 1) * intNumBinRecPerFile + (int)(System.Math.Floor((double)fs.Length / (double)lng_Bin_RecordSize));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("error :" + e.Message);
                }
                if (fs != null) fs.Close();
                return intRetVal;
            }
            else
                return 0;

        }

        static int intNumBinRecPerFile = (int)Math.Pow(2, 15);

        public static string BinRec_FileName_Get(ref classDictionary cDictionary, int intFileNumber, enuFileDataTypes eFileDataType)
        {
            return cDictionary.cFileData[(int)eFileDataType].str_BINFileNameAndDirectory + intFileNumber.ToString("000") + ".bin";
        }
        public static int BinRec_FileNumber_Get(int index) { return (int)Math.Floor((float)index / (float)intNumBinRecPerFile); }
        public static classFileStreamManager BinRec_FileStream_Get(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            int intFileNumber = (int)Math.Floor((float)index / (float)intNumBinRecPerFile);
            int intIndex_Revised = index - intFileNumber * intNumBinRecPerFile;
            classFileStreamManager cFS = null;
            if (cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS.Count > intFileNumber)
                cFS = cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS[intFileNumber];

            if (cFS == null)
            {
                string strFileName = BinRec_FileName_Get(ref cDictionary, intFileNumber, eFileDataType);
                cFS = new classFileStreamManager();
                if (System.IO.File.Exists(strFileName))
                    cFS.fs = new FileStream(strFileName, FileMode.Open);
                else
                    cFS.fs = new FileStream(strFileName, FileMode.Create);
                if (cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS.Count <= intFileNumber)
                    cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS.AddRange(new classFileStreamManager[intFileNumber - cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS.Count + 1]);

                cDictionary.cFileData[(int)eFileDataType].lst_Bin_SemFS[intFileNumber] = cFS;
            }
            cFS.fs.Position = ((long)intIndex_Revised * lng_Bin_RecordSize);

            return cFS;
        }
        public static class_BinTree_Record BinRec_Load(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            class_BinTree_Record cRetVal = new class_BinTree_Record();
            if (cDictionary == null) return cRetVal;

            classFileStreamManager cFS = BinRec_FileStream_Get(ref cDictionary, index, eFileDataType);
            cFS.WaitOne();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                cRetVal.left = (int)formatter.Deserialize(cFS.fs);
                cRetVal.right = (int)formatter.Deserialize(cFS.fs);
                cRetVal.LL = (int)formatter.Deserialize(cFS.fs);
                cRetVal.strWord = (string)formatter.Deserialize(cFS.fs);
                cRetVal.weight = (int)formatter.Deserialize(cFS.fs);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }
            cFS.Release();

            return cRetVal;
        }

        public static void BinRec_Save(ref classDictionary cDictionary, int index, ref class_BinTree_Record cBinRec, enuFileDataTypes eFileDataType)
        {
            classFileStreamManager cFS = BinRec_FileStream_Get(ref cDictionary, index, eFileDataType);

            cFS.WaitOne();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(cFS.fs, cBinRec.left);
                formatter.Serialize(cFS.fs, cBinRec.right);
                formatter.Serialize(cFS.fs, cBinRec.LL);
                formatter.Serialize(cFS.fs, cBinRec.strWord);
                formatter.Serialize(cFS.fs, cBinRec.weight);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("error :" + e.Message);
            }

            cFS.Release();
        }

        public static void BinTree_Insert(ref classDictionary cDictionary, string strWord_input, ref class_LL_Record cLL_New, enuFileDataTypes eFileDataType)
        {
            if (strWord_input == null
                || strWord_input.Trim().Length < 1
                || strWord_input.Length > intStandardBinWordSize)
                return;

            class_BinTree_Record cThisBinRec;
            class_BinTree_Record cNewBinRec;
            int intThisBinRecIndex = 0;
            bool bolLoop = true;

            cNewBinRec = new class_BinTree_Record();
            cNewBinRec.strWord = getStandardWordSize(strWord_input);
            cNewBinRec.LL = cLL_New.intMyIndex;
            cNewBinRec.flag = true;
            cLL_New.next = -1;

            cDictionary.cFileData[(int)eFileDataType].int_LL_RecordIndex_NextAvailable++;

            if (cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable == 0)
            {
                BinRec_Save(ref cDictionary, cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable++, ref cNewBinRec, eFileDataType);
                LL_Record_Save(ref cDictionary, cNewBinRec.LL, ref cLL_New, eFileDataType);

                //        class_LL_Record cLL_Test = LL_Record_Load(ref cDictionary, cLL_New.intMyIndex , eFileDataType);

            }
            else
                while (bolLoop)
                {
                    cThisBinRec = null;
                    try
                    {
                        cThisBinRec = BinRec_Load(ref cDictionary, intThisBinRecIndex, eFileDataType);
                    }
                    catch (Exception err1)
                    {
                        MessageBox.Show("erro:" + err1.Message);

                    }
                    if (cThisBinRec == null)
                    {
                        MessageBox.Show("There was an error saving to ");
                        return;
                    }
                    int intCompareResult = string.Compare(cNewBinRec.strWord, cThisBinRec.strWord);
                    if (intCompareResult > 0)
                    {// go right
                        if (cThisBinRec.right > 0)
                            intThisBinRecIndex = cThisBinRec.right;
                        else
                        { // insert right\
                            try
                            {
                                cThisBinRec.right = cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable;
                                BinRec_Save(ref cDictionary, intThisBinRecIndex, ref cThisBinRec, eFileDataType);
                                BinRec_Save(ref cDictionary, cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable++, ref cNewBinRec, eFileDataType);

                                LL_Record_Save(ref cDictionary, cNewBinRec.LL, ref cLL_New, eFileDataType);
                                bolLoop = false;
                            }
                            catch (Exception err2)
                            {
                                MessageBox.Show("error:" + err2.Message);

                            }
                        }
                    }
                    else if (intCompareResult < 0)
                    {// go left
                        if (cThisBinRec.left > 0)
                            intThisBinRecIndex = cThisBinRec.left;
                        else
                        { // insert left
                            try
                            {
                                cThisBinRec.left = cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable;
                                BinRec_Save(ref cDictionary, intThisBinRecIndex, ref cThisBinRec, eFileDataType);
                                BinRec_Save(ref cDictionary, cDictionary.cFileData[(int)eFileDataType].int_BinRec_Index_NextAvailable++, ref cNewBinRec, eFileDataType);

                                LL_Record_Save(ref cDictionary, cNewBinRec.LL, ref cLL_New, eFileDataType);
                                bolLoop = false;
                            }
                            catch (Exception err3)
                            {
                                MessageBox.Show("erro:" + err3.Message);
                            }
                        }
                    }
                    else
                    { // word is already in the binary tree                    
                        bolLoop = false;
                        bool bolLLTree_Loop = true;

                        class_LL_Record cLL_WeightTree_Item = null;
                        cThisBinRec.weight++;
                        BinRec_Save(ref cDictionary, intThisBinRecIndex, ref cThisBinRec, eFileDataType);

                        int intLL_Index = cThisBinRec.LL;
                        do
                        {
                            try { cLL_WeightTree_Item = LL_Record_Load(ref cDictionary, intLL_Index, eFileDataType); }
                            catch (Exception err4) { MessageBox.Show("error:" + err4.Message); }

                            int intLL_CompareResult = cLL_New.Weight - cLL_WeightTree_Item.Weight;

                            if (intLL_CompareResult > 0)
                            {
                                if (cLL_WeightTree_Item.right > 0)
                                    intLL_Index = cLL_WeightTree_Item.right;
                                else
                                {
                                    cLL_WeightTree_Item.right = cLL_New.intMyIndex;
                                    LL_Record_Save(ref cDictionary, ref cLL_WeightTree_Item, eFileDataType);
                                    LL_Record_Append(ref cDictionary, ref cLL_New, eFileDataType);
                                    bolLLTree_Loop = false;
                                }
                            }
                            else if (intLL_CompareResult < 0)
                            {
                                if (cLL_WeightTree_Item.left > 0)
                                    intLL_Index = cLL_WeightTree_Item.left;
                                else
                                {
                                    cLL_WeightTree_Item.left = cLL_New.intMyIndex;
                                    LL_Record_Save(ref cDictionary, ref cLL_WeightTree_Item, eFileDataType);
                                    LL_Record_Append(ref cDictionary, ref cLL_New, eFileDataType);
                                    bolLLTree_Loop = false;
                                }
                            }
                            else
                            {
                                cLL_New.next = cLL_WeightTree_Item.next;
                                cLL_WeightTree_Item.next = cLL_New.intMyIndex;
                                LL_Record_Save(ref cDictionary, ref cLL_WeightTree_Item, eFileDataType);

                                LL_Record_Append(ref cDictionary, ref cLL_New, eFileDataType);
                                bolLLTree_Loop = false;
                            }

                        } while (bolLLTree_Loop);
                    }
                }
        }

        public static class_BinTree_Record BinTree_GetRecord(ref classDictionary cDictionary, string strWord, enuFileDataTypes eFileDataType)
        {
            bool bolLoop = true;
            class_BinTree_Record cThisBinRec;
            if (cDictionary == null) return null;

            class_BinTree_Record cRetVal = new class_BinTree_Record();
            strWord = getStandardWordSize(strWord);
            int intThisBinRecIndex = 0;
            while (bolLoop)
            {
                cThisBinRec = BinRec_Load(ref cDictionary, intThisBinRecIndex, eFileDataType);

                int intCompareResult = strWord.CompareTo(cThisBinRec.strWord.ToLower());
                if (intCompareResult > 0)
                {// go right
                    if (cThisBinRec.right > 0)
                        intThisBinRecIndex = cThisBinRec.right;
                    else
                    {
                        cRetVal = new class_BinTree_Record();
                        cRetVal.strWord = strWord;
                        cRetVal.LL = -1;
                        cRetVal.flag = false;
                        return cRetVal;
                    }
                }
                else if (intCompareResult < 0)
                {// go left
                    if (cThisBinRec.left > 0)
                        intThisBinRecIndex = cThisBinRec.left;
                    else
                    {
                        cRetVal = new class_BinTree_Record();
                        cRetVal.strWord = strWord;
                        cRetVal.LL = -1;
                        cRetVal.flag = false;
                        return cRetVal;
                    }
                }
                else
                { // word is already in the binary tree                    
                    bolLoop = false;
                    return cThisBinRec;
                }
            }

            return cRetVal;
        }

        #endregion

        #region "LL"
        static long LL_RecordSize_Get()
        {
            string strTempFileName = System.IO.Directory.GetCurrentDirectory() + "\\tempFileName.flx";
            if (System.IO.File.Exists(strTempFileName))
                System.IO.File.Delete(strTempFileName);
            FileStream fs = new FileStream(strTempFileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            class_LL_Record c_LLRecord = new class_LL_Record(enuFileDataTypes.heading);

            formatter.Serialize(fs, c_LLRecord.next);
            formatter.Serialize(fs, c_LLRecord.left);
            formatter.Serialize(fs, c_LLRecord.right);
            formatter.Serialize(fs, getFileName("01234567"));
            formatter.Serialize(fs, (int)c_LLRecord.Weight);
            formatter.Serialize(fs, (int)c_LLRecord.intMyIndex);
            formatter.Serialize(fs, (string)StandardHeading("test heading"));
            formatter.Serialize(fs, (long)0);                                       // make record size bigger than necessary

            long lngRetVal = (long)fs.Position;
            fs.Close();
            return lngRetVal;
        }

        public static int LL_NumRecords_Get(ref classDictionary cDictionary, enuFileDataTypes eFileDataType)
        {
            string strPath = cDictionary.strSourceDirectory;
            string strSearchPattern = eFileDataType.ToString() + "_" + "*.LL";

            List<string> lstLLFiles = System.IO.Directory.GetFiles(strPath, strSearchPattern).ToList<string>();

            if (lstLLFiles.Count > 0)
            {
                string strLastFile = lstLLFiles[0];
                for (int intCounter = 1; intCounter < lstLLFiles.Count; intCounter++)
                    if (string.Compare(strLastFile, lstLLFiles[intCounter]) < 0)
                        strLastFile = lstLLFiles[intCounter];

                FileStream fs = null;
                int intRetVal = 0;
                try
                {
                    fs = new FileStream(strLastFile, FileMode.Open);
                    intRetVal = (lstLLFiles.Count - 1) * intNumLLRecPerFile + (int)(System.Math.Floor((double)fs.Length / (double)lng_LL_RecordSize));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("error :" + e.Message);
                }
                if (fs != null) fs.Close();
                return intRetVal;
            }
            else
                return 0;
        }

        static int intNumLLRecPerFile = (int)Math.Pow(2, 15);

        public static string LLRec_FileName_Get(ref classDictionary cDictioary, int intFileNumber, enuFileDataTypes eFileDataType)
        {
            return cDictioary.strSourceDirectory + eFileDataType.ToString() + "_" + intFileNumber.ToString("000") + ".LL";
        }
        public static int _LLRec_FileNumber_Get(int index) { return (int)Math.Floor((float)index / (float)intNumLLRecPerFile); }
        public static classFileStreamManager LLRec_FileStream_Get(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            int intFileNumber = (int)Math.Floor((float)index / (float)intNumLLRecPerFile);
            int intIndex_Revised = index - intFileNumber * intNumLLRecPerFile;
            classFileStreamManager cFS = null;
            if (cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS.Count > intFileNumber)
                cFS = cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS[intFileNumber];

            if (cFS == null)
            {
                string strFileName = LLRec_FileName_Get(ref cDictionary, intFileNumber, eFileDataType);
                cFS = new classFileStreamManager();
                if (System.IO.File.Exists(strFileName))
                    cFS.fs = new FileStream(strFileName, FileMode.Open);
                else
                    cFS.fs = new FileStream(strFileName, FileMode.Create);
                if (cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS.Count <= intFileNumber)
                    cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS.AddRange(new classFileStreamManager[intFileNumber - cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS.Count + 1]);

                cDictionary.cFileData[(int)eFileDataType].lst_LL_SemFS[intFileNumber] = cFS;
            }
            cFS.fs.Position = ((long)intIndex_Revised * lng_LL_RecordSize);

            return cFS;
        }

        public static int HeadingList_NumRecords_Get(ref classDictionary cDictionary)
        {
            string strPath = cDictionary.strSourceDirectory;
            enuFileDataTypes eFileDataType = enuFileDataTypes.heading;
            string strSearchPattern = eFileDataType.ToString() + "_" + "*.WLBin";

            List<string> lstHeadingList_Files = System.IO.Directory.GetFiles(strPath, strSearchPattern).ToList<string>();

            if (lstHeadingList_Files.Count > 0)
            {
                string strLastFile = lstHeadingList_Files[0];
                for (int intCounter = 1; intCounter < lstHeadingList_Files.Count; intCounter++)
                    if (string.Compare(strLastFile, lstHeadingList_Files[intCounter]) < 0)
                        strLastFile = lstHeadingList_Files[intCounter];

                FileStream fs = null;
                int intRetVal = 0;
                try
                {
                    fs = new FileStream(strLastFile, FileMode.Open);
                    intRetVal = (lstHeadingList_Files.Count - 1) * intNumLLRecPerFile + (int)(System.Math.Floor((double)fs.Length / (double)lng_LL_RecordSize));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("error :" + e.Message);
                }
                if (fs != null) fs.Close();
                return intRetVal;
            }
            else
                return 0;
        }




        public static class_LL_Record LL_Record_Load(ref classDictionary cDictionary, int index, enuFileDataTypes eFileDataType)
        {
            if (index < 0) return null;
            classFileStreamManager cFS = LLRec_FileStream_Get(ref cDictionary, index, eFileDataType);
            class_LL_Record cRetVal = new class_LL_Record(ref cDictionary, eFileDataType);
            cRetVal.intMyIndex = -21;
            cFS.WaitOne();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                cRetVal.next = (int)formatter.Deserialize(cFS.fs);
                cRetVal.left = (int)formatter.Deserialize(cFS.fs);
                cRetVal.right = (int)formatter.Deserialize(cFS.fs);
                cRetVal.FileName = (string)formatter.Deserialize(cFS.fs);
                cRetVal.Weight = (int)formatter.Deserialize(cFS.fs);
                cRetVal.intMyIndex = (int)formatter.Deserialize(cFS.fs);
                cRetVal.strHeading = (string)formatter.Deserialize(cFS.fs);
            }
            catch (Exception)
            {
                {
                    System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
                    soundPlayer.SoundLocation = @"C:\Users\Christ\Documents\C#\Projects - Writing\Words\bin\Debug\te_ale.wav";
                    soundPlayer.Play();
                }
                ;
            }

            cFS.Release();
            return cRetVal;
        }

        public static void LL_Record_Append(ref classDictionary cDictionary, ref class_LL_Record c_LLRecord, enuFileDataTypes eFileDataType)
        {
            //    c_LLRecord.intMyIndex = cDictionary.cFileData[(int)eFileDataType] .int_LL_RecordIndex_NextAvailable++;
            LL_Record_Save(ref cDictionary, ref c_LLRecord, eFileDataType);
        }

        public static void LL_Record_Save(ref classDictionary cDictionary, ref class_LL_Record c_LLRecord, enuFileDataTypes eFileDataType)
        {
            LL_Record_Save(ref cDictionary, c_LLRecord.intMyIndex, ref c_LLRecord, eFileDataType);
        }

        public static void LL_Record_Save(ref classDictionary cDictionary, int index, ref class_LL_Record c_LLRecord, enuFileDataTypes eFileDataType)
        {
            classFileStreamManager cFS = LLRec_FileStream_Get(ref cDictionary, index, eFileDataType);

            BinaryFormatter formatter = new BinaryFormatter();
            cFS.WaitOne();
            try
            {
                formatter.Serialize(cFS.fs, c_LLRecord.next);
                formatter.Serialize(cFS.fs, c_LLRecord.left);
                formatter.Serialize(cFS.fs, c_LLRecord.right);
                formatter.Serialize(cFS.fs, getFileName(c_LLRecord.FileName));
                formatter.Serialize(cFS.fs, (int)c_LLRecord.Weight);
                formatter.Serialize(cFS.fs, (int)c_LLRecord.intMyIndex);
                formatter.Serialize(cFS.fs, (string)StandardHeading(c_LLRecord.strHeading));

            }
            catch (Exception)
            {
                ;
            }
            cFS.Release();
        }
        //////////////////
        #endregion

    }

  

    public class formBinSearchResults_ : formMoveable
    {
        public static formBinSearchResults_ instance;
        public ListBox lbxResults = new ListBox();
        public Point ptMouse = new Point();
        public List<string> lstSearchWords;
        public CheckBox chkOrderByFrequency = new CheckBox();
        classDictionary _cDictionary;
        public classDictionary cDictionary
        {
            get { return _cDictionary; }
            set { _cDictionary = value; }
        }
        public formBinSearchResults_()
        {
            instance = this;
            if (formPreview.instance == null)
                formPreview.instance = new formPreview();

            ControlBox = false;
            ShowInTaskbar = false;

            pnlContainer.Controls.Add(chkOrderByFrequency);
            chkOrderByFrequency.Text = "Ordery by Frenquency";
            chkOrderByFrequency.Name = "FormBinSearchResults_.chkOrderByFrequency";
            chkOrderByFrequency.AutoSize = true;
            chkOrderByFrequency.CheckedChanged += ChkOrderByFrequency_CheckedChanged;
            Name = "Content Search";
            cInitSettings.Load();
            Heading.BackColor = Color.Beige;
            Heading.ForeColor = Color.Black;
            Heading.Font = new Font("sans serif", 10);

            pnlContainer.Controls.Add(lbxResults);
            lbxResults.Font = new Font("ms sans-serif", 14);
            lbxResults.Name = "FormBinSearchResults_.lbxResults";
            lbxResults.MouseMove += lbxResults_MouseMove;
            lbxResults.MouseClick += lbxResults_MouseClick;
            lbxResults.MouseLeave += LbxResults_MouseLeave;
            lbxResults.MouseEnter += LbxResults_MouseEnter;
            lbxResults.KeyDown += LbxResults_KeyDown;


            SizeChanged += formBinSearchResults_SizeChanged;
            Activated += FormBinSearchResults_Activated;
            Disposed += FormBinSearchResults_Disposed;
        }

        private void FormBinSearchResults_Disposed(object sender, EventArgs e)
        {
            new formBinSearchResults_();
        }

        bool bolInit = false;
        private void FormBinSearchResults_Activated(object sender, EventArgs e)
        {
            if (bolInit) return;
            bolInit = true;

        }

        private void ChkOrderByFrequency_CheckedChanged(object sender, EventArgs e)
        {
            orderBy();
        }

        void orderBy()
        {
            if (chkOrderByFrequency.Checked)
                orderBy_Frequency();
            else
                orderBy_Alphabet();
        }

        void orderBy_Frequency()
        {
            IEnumerable<class_LL_Record> query = _lst_Results.OrderByDescending(LL => LL.Weight);
            _lst_Results = (List<class_LL_Record>)query.ToList<class_LL_Record>();

            Results_Show();
        }

        void orderBy_Alphabet()
        {
            IEnumerable<class_LL_Record> query = _lst_Results.OrderBy(LL => LL.strHeading);
            _lst_Results = (List<class_LL_Record>)query.ToList<class_LL_Record>();

            Results_Show();
        }

        private void formBinSearchResults_SizeChanged(object sender, EventArgs e)
        {
            if (!Collapsed)
            {
                chkOrderByFrequency.Top = 0;
                chkOrderByFrequency.Left = 1;
                lbxResults.Top = chkOrderByFrequency.Bottom;
                lbxResults.Height = pnlContainer.Height - lbxResults.Top;
                lbxResults.Width = pnlContainer.Width;
            }
        }

        Cursor crsMagnifyingGlass = null;
        private void LbxResults_MouseEnter(object sender, EventArgs e)
        {
            if (crsMagnifyingGlass == null)
            {
                Bitmap btmMagnifyingGlass = new Bitmap(Properties.Resources.MagnifyingGlass_100pix, new Size(30, 30));//, bmpArray[intButton].Width, bmpArray[intButton].Height);
                btmMagnifyingGlass.MakeTransparent(btmMagnifyingGlass.GetPixel(0, 0));
                IntPtr ptr = btmMagnifyingGlass.GetHicon();
                crsMagnifyingGlass = new Cursor(ptr);
            }
            Cursor = crsMagnifyingGlass;
        }

        private void LbxResults_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Hide();
                    break;

                default:
                    break;
            }
        }

        private void LbxResults_MouseLeave(object sender, EventArgs e)
        {
            formPreview.instance.Hide();
            lbxResults.SelectedIndex = -1;
            Cursor = Cursors.Arrow;
        }

        private void lbxResults_MouseClick(object sender, MouseEventArgs e)
        {
            if (lbxResults.SelectedIndex >= 0
                &&
                lbxResults.SelectedIndex < _lst_Results.Count)
            {
                class_LL_Record cLLRec = _lst_Results[lbxResults.SelectedIndex];
                classFileContent cFileContent = new classFileContent(cDictionary.strSourceDirectory, cLLRec.FileName);
                List<string> lstWordsHighlighted = new List<string>();
            }
        }

        List<class_LL_Record> _lst_Results = new List<class_LL_Record>();
        public List<class_LL_Record> lst_Results
        {
            get { return _lst_Results; }
            set
            {
                _lst_Results = value;
            }
        }

        public void Set(ref classDictionary cDictionary, ref List<class_LL_Record> lstResults, string strSearchWordsList)
        {
            _lst_Results = lstResults;
            for (int intCounter = 0; intCounter < _lst_Results.Count; intCounter++)
            {
                _lst_Results[intCounter].strHeading = classStringLibrary.cleanFront_nonAlpha(new classFileContent(cDictionary.strSourceDirectory, _lst_Results[intCounter].FileName).Heading);
            }
            lstSearchWords = classStringLibrary.getFirstWords(classStringLibrary.Deaccent(strSearchWordsList.ToLower()));
            Heading.Text = " Content Search: " + strSearchWordsList;
            orderBy();
            Results_Show();
            Show();
            BringToFront();
        }

        void Results_Show()
        {
            lbxResults.Items.Clear();
            for (int intCounter = 0; intCounter < _lst_Results.Count; intCounter++)
            {
                lbxResults.Items.Add(
                         (chkOrderByFrequency.Checked
                                    ? "(" + _lst_Results[intCounter].Weight.ToString() + ")"
                                    : "")
                          + _lst_Results[intCounter].strHeading);
            }
        }

        private void lbxResults_MouseMove(object sender, MouseEventArgs e)
        {
            ptMouse.X = e.X;
            ptMouse.Y = e.Y;
            int intHeightItem = lbxResults.Font.Height;
            if (lbxResults.Items.Count > 0)
                intHeightItem = lbxResults.GetItemHeight(0);
            int intSelectedIndex = lbxResults.TopIndex + e.Y / intHeightItem;
            if (intSelectedIndex >= 0 && intSelectedIndex < lbxResults.Items.Count)
            {
                if (lbxResults.SelectedIndex != intSelectedIndex)
                {
                    lbxResults.SelectedIndex = intSelectedIndex;
                    class_LL_Record cLL = _lst_Results[lbxResults.SelectedIndex];
                    classFileContent cFileContent = new classFileContent(cDictionary.strSourceDirectory, cLL.FileName);
                    formPreview.instance.ShowFileContent(ref cFileContent, ref lstSearchWords);
                    lbxResults.Focus();
                }
                formPreview.instance.Left = MousePosition.X + 3;
                formPreview.instance.Top = MousePosition.Y + 3;
            }
            else
                formPreview.instance.Hide();
        }
    }

    public class formPreview : Form
    {
        public static formPreview instance;
        public RichTextBox rtx_Preview = new RichTextBox();
        public formPreview()
        {
            instance = this;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            Controls.Add(rtx_Preview);
            TopMost = true;
            rtx_Preview.Multiline = true;
            rtx_Preview.BackColor = Color.Bisque;
            rtx_Preview.Dock = DockStyle.Fill;
            rtx_Preview.KeyDown += Rtx_Preview_KeyDown;
            rtx_Preview.MouseLeave += rtx_Preview_MouseLeave;
            Size = new Size(300, 300);
        }

        private void Rtx_Preview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    formBinSearchResults_.instance.Hide();
                    break;

                default:
                    break;
            }
        }

        private void rtx_Preview_MouseLeave(object sender, EventArgs e)
        {
            Hide();
        }

        public void ShowFileContent(ref classFileContent cFileContent, ref List<string> strWords)
        {
            if (cFileContent != null)
            {
                RichTextBox rtxTemp = new RichTextBox();
                rtx_Preview.Text = cFileContent.Heading
                                + "\r\n" + cFileContent.alt_Heading
                                + "\r\n" + cFileContent.Definition;
                rtxTemp.Text = classStringLibrary.Deaccent(rtx_Preview.Text.ToLower());

                Font fntReg = new Font("Ms Sans-Serif", 10);
                Font fntUnderline = new Font(fntReg.FontFamily, fntReg.Size, FontStyle.Underline);
                Font fntBold = new Font(fntReg.FontFamily, fntReg.Size, FontStyle.Bold);
                string strMain = classStringLibrary.Deaccent(cFileContent.Definition.ToLower());

                rtx_Preview.Select(0, cFileContent.Heading.Length);
                rtx_Preview.SelectionFont = fntUnderline;

                rtx_Preview.Select(cFileContent.Heading.Length + 1, rtx_Preview.Text.Length - cFileContent.Heading.Length - 1);
                rtx_Preview.SelectionFont = fntReg;

                for (int intWordCounter = 0; intWordCounter < strWords.Count; intWordCounter++)
                {
                    string strWord = strWords[intWordCounter];
                    int intIndex = rtxTemp.Text.IndexOf(strWord);
                    while (intIndex >= 0)
                    {
                        bool bolValid = true;
                        if (intIndex > 0)
                        {
                            char chrTest = rtxTemp.Text[intIndex - 1];
                            if (classStringLibrary.isAlpha(chrTest))
                                bolValid = false;
                        }
                        if (intIndex + strWord.Length < rtxTemp.Text.Length - 1)
                        {
                            char chrTest = rtxTemp.Text[intIndex + strWord.Length];
                            if (classStringLibrary.isAlpha(chrTest))
                                bolValid = false;
                        }

                        if (bolValid)
                        {
                            rtx_Preview.Select(intIndex, strWord.Length);
                            rtx_Preview.SelectionFont = fntBold;
                        }
                        intIndex = rtxTemp.Text.IndexOf(strWords[intWordCounter], intIndex + 1);
                    }
                }

                Show();
                Height = 100000;
                rtx_Preview.HideSelection = true;
                rtx_Preview.SelectionLength = 0;
                rtx_Preview.Refresh();

                Point ptEnd = rtx_Preview.GetPositionFromCharIndex(rtx_Preview.Text.Length - 1);
                Height = ptEnd.Y + 35;

                Point ptLocation = MousePosition;
                while (ptLocation.X + Width > Screen.PrimaryScreen.WorkingArea.Width)
                    ptLocation.X--;
                while (ptLocation.X < 0)
                    ptLocation.X++;
                while (ptLocation.Y + Height > Screen.PrimaryScreen.WorkingArea.Height)
                    ptLocation.Y--;
                while (ptLocation.Y < 0)
                    ptLocation.Y++;

                Location = ptLocation;


            }
        }
    }

    public class class_BinTree_Record
    {
        // sub
        public int left = -1;
        public int right = -1;

        //      public int parent;
        public string strWord;
        public int LL;
        public int weight = 1;

        public bool flag;
    }
    public class class_LL_Record
    {
        public string FileName;
        public int left = -1;
        public int right = -1;
        int _next;
        public int next
        {
            get { return _next; }
            set
            {
                _next = value;
            }
        }
        public int intMyIndex = -1;
        public int Weight;
        string _strHeading;
        public string strHeading
        {
            get { return _strHeading; }
            set { _strHeading = value; }
        }

        public classDictionary cDictionary;

        public static classDictionary cDefaultDictionary = new classDictionary();

        public class_LL_Record(enuFileDataTypes eFileDataType) { init(ref cDefaultDictionary, eFileDataType); }
        public class_LL_Record(ref classDictionary Dictionary, enuFileDataTypes eFileDataType) { init(ref Dictionary, eFileDataType); }

        void init(ref classDictionary Dictionary, enuFileDataTypes eFileDataType)
        {
            cDictionary = Dictionary;
            intMyIndex = cDictionary.cFileData[(int)eFileDataType].int_LL_RecordIndex_NextAvailable;
        }
    }

    public class classFileStreamManager
    {
        Semaphore sem = new Semaphore(1, 1);
        public FileStream fs;
        public void WaitOne() { sem.WaitOne(); }
        public void Release() { sem.Release(); }
    }

    public class classInitFileInfo
    {
        public string[] strDir;
        public string[] strFiles;

        public classFileInfo cSearch = new classFileInfo();
        public classFileInfo cHeadingList = new classFileInfo();
        public class classFileInfo
        {
            public int intCurrentDir;
            public int intCurrentFile;
            public int intNumRec;
            public bool bolInit;
            public bool bolNewDirectory;
        }
    }

    public class classFileContent
    {
        const string conNL = "\r\n";
        public string Heading;
        public string alt_Heading;
        public string Definition;

        public enum enuFields
        {
            FileName,
            heading,
            garbage,
            Definition,
            _num
        }

        public enum enuComponentsRequested { Heading, Content, HeadingContent, None, _numComponentRequested };


        public classFileContent(string strFileNameAndDirectory)
        {
            // remove extension
            enuFileExtensions eFileExtension = classBT_Dictionaries.GetFileExtension(strFileNameAndDirectory);
            switch (eFileExtension)
            {
                case enuFileExtensions.rtf:
                case enuFileExtensions.txt:
                    {
                        strFileNameAndDirectory = strFileNameAndDirectory.Substring(0, strFileNameAndDirectory.Length - ("." + eFileExtension.ToString()).Length);
                    }
                    break;

                default:
                    { }
                    break;
            }
            init(strFileNameAndDirectory);
        }
        public classFileContent(string strSourceDirectory, string FileName)
        {
            if (strSourceDirectory == null
                || strSourceDirectory.Length == 0
                || FileName == null
                || FileName.Length == 0) return;

            string strFileNameAndDirectory = getFileNameAndDirectory(strSourceDirectory, FileName);
            init(strFileNameAndDirectory);
        }


        public classFileContent(string strSourceDirectory, string FileName, enuComponentsRequested eComponentsRequested)
        {
            if (strSourceDirectory == null
                || strSourceDirectory.Length == 0
                || FileName == null
                || FileName.Length == 0) return;

            string strFileNameAndDirectory = getFileNameAndDirectory(strSourceDirectory, FileName);
            init(strFileNameAndDirectory, eComponentsRequested);
        }

        static bool ColorComparison(Color clr1, Color clr2) { return (clr1.R == clr2.R && clr1.G == clr2.G && clr1.B == clr2.B); }


        void init(string strFileNameAndDirectory) { init(strFileNameAndDirectory, enuComponentsRequested.HeadingContent); }
        void init(string strFileNameAndDirectory, enuComponentsRequested eComponentsRequested)
        {
            for (int intExtCounter = 0; intExtCounter < (int)enuFileExtensions._num; intExtCounter++)
            {
                enuFileExtensions eFileExtension = (enuFileExtensions)intExtCounter;

                string strFileNameDirExtension = strFileNameAndDirectory + "." + eFileExtension.ToString();
                if (System.IO.File.Exists(strFileNameDirExtension))
                {
                    switch (eFileExtension)
                    {
                        case enuFileExtensions.txt:
                            {
                                string strFile = System.IO.File.ReadAllText(strFileNameDirExtension, Encoding.UTF7);

                                char chrQuote = '\"';
                                int intCut_Open = strFile.IndexOf(chrQuote);
                                int intCut_Close = strFile.IndexOf(chrQuote, intCut_Open + 1);

                                int intField = 0;
                                while (intCut_Close > intCut_Open && intCut_Open >= 0)
                                {

                                    string strField = strFile.Substring(intCut_Open + 1, intCut_Close - intCut_Open - 1);
                                    intCut_Open = strFile.IndexOf(chrQuote, intCut_Close + 1);
                                    intCut_Close = strFile.IndexOf(chrQuote, intCut_Open + 1);

                                    switch ((enuFields)intField)
                                    {
                                        case enuFields.FileName:
                                            intField++;
                                            break;

                                        case enuFields.heading:
                                            Heading = strField;
                                            intField++;
                                            break;

                                        case enuFields.garbage:
                                            if (string.Compare("end of list", strField) == 0)
                                            {
                                                intField++;
                                            }
                                            break;

                                        case enuFields.Definition:
                                            if (string.Compare("end of file", strField) == 0)
                                                return;
                                            Definition += strField;
                                            break;
                                    }
                                }
                            }
                            break;

                        case enuFileExtensions.rtf:
                            {
                                using (RichTextBox rtx = new RichTextBox())
                                {
                                    rtx.LoadFile(strFileNameDirExtension);
                                    Heading
                                        = Definition
                                        = "";
                                    if (rtx.Text.Length > 0)
                                    {
                                        try
                                        {
                                            if (eComponentsRequested == enuComponentsRequested.HeadingContent || eComponentsRequested == enuComponentsRequested.Heading)
                                            {
                                                // parse the heading
                                                int intHeadingCut = rtx.Text.IndexOf("\n", StringComparison.OrdinalIgnoreCase);
                                                if (strFileNameAndDirectory.Contains("IrisDict") && !strFileNameAndDirectory.ToLower().Contains("aaaaaaaa"))
                                                {
                                                    // Irish dictionary needs its RTF files rebuilt...
                                                    // Heading & word info are on the same line!!!
                                                    Color clrSelect = rtx.SelectionColor;

                                                    int intHeading_Start = 0;
                                                    rtx.Select(intHeading_Start, 1);
                                                    clrSelect = rtx.SelectionColor;
                                                    while (intHeading_Start < rtx.Text.Length && ColorComparison(Color.Black, clrSelect))
                                                    {
                                                        rtx.Select(++intHeading_Start, 1);
                                                        clrSelect = rtx.SelectionColor;
                                                    }


                                                    int intHeading_End = intHeading_Start;

                                                    rtx.Select(intHeading_Start, 1);
                                                    Color clrHeading = rtx.SelectionColor;
                                                    intHeadingCut = rtx.Text.IndexOf("\n", intHeading_Start, StringComparison.OrdinalIgnoreCase);
                                                    do
                                                    {
                                                        rtx.Select(++intHeading_End, 1);
                                                        clrSelect = rtx.SelectionColor;
                                                    } while (intHeading_End < intHeadingCut && ColorComparison(clrHeading, clrSelect));

                                                    intHeading_End -= 1;
                                                    if (intHeading_Start >= 0 && intHeading_End >= intHeading_Start)
                                                    {
                                                        Heading = classStringLibrary.clean_nonAlpha_Ends(rtx.Text.Substring(intHeading_Start, intHeading_End - intHeading_Start + 1));
                                                        goto parseDefinition;
                                                    }
                                                }

                                                if (intHeadingCut > 0)
                                                    Heading = rtx.Text.Substring(0, intHeadingCut);
                                            }

                                        parseDefinition:
                                            if (eComponentsRequested == enuComponentsRequested.HeadingContent || eComponentsRequested == enuComponentsRequested.Content)
                                            {
                                                // parse definition text
                                                string[] strCatchPhrases = { "variant:" };
                                                classSweepAndPrune cPS_CatchPhrases = new classSweepAndPrune();
                                                for (int intCPCounter = 0; intCPCounter < strCatchPhrases.Length; intCPCounter++)
                                                {
                                                    string strCP = strCatchPhrases[intCPCounter];
                                                    int intCP = rtx.Text.IndexOf(strCP, StringComparison.OrdinalIgnoreCase);
                                                    while (intCP >= 0)
                                                    {
                                                        classSweepAndPrune.classElement cPS_Element = new classSweepAndPrune.classElement();
                                                        cPS_Element.rec = new Rectangle(intCP, 0, intCP + strCP.Length, 10);
                                                        cPS_CatchPhrases.Add(ref cPS_Element);
                                                        intCP = rtx.Text.IndexOf(strCP, intCP + 1, StringComparison.OrdinalIgnoreCase);
                                                    }
                                                }

                                                string strParse = "\n.,< >;:'\"[]}{-_)(*&^%$#@!~!`\\|0987654321";
                                                char[] chrParse = strParse.ToArray<char>();
                                                int intCut_Prev = 0;
                                                while (strParse.Contains(rtx.Text[intCut_Prev]))
                                                    intCut_Prev++;

                                                int intCut = rtx.Text.IndexOfAny(chrParse, intCut_Prev);
                                                Definition = "";
                                                bool bolQuit = false;
                                                while (intCut >= intCut_Prev)
                                                {
                                                    Point ptTest = new Point(intCut_Prev + (intCut - intCut_Prev) / 2, 1);
                                                    if (cPS_CatchPhrases.Search(ptTest) == null)
                                                    {
                                                        rtx.Select(ptTest.X, 1);
                                                        if (rtx.SelectionColor == Color.Black)
                                                        {
                                                            string strWord = classStringLibrary.clean_nonAlpha_Ends(rtx.Text.Substring(intCut_Prev, intCut - intCut_Prev));

                                                            if (strWord.Length > 0)
                                                                Definition += "." + strWord;
                                                        }
                                                    }

                                                    if (!bolQuit)
                                                    {
                                                        intCut_Prev = intCut;
                                                        intCut = rtx.Text.IndexOfAny(chrParse, intCut_Prev + 1);
                                                        if (intCut < 0)
                                                        {
                                                            bolQuit = true;
                                                            intCut = rtx.Text.Length;
                                                        }
                                                    }
                                                    else
                                                        intCut = -1;

                                                }
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show("error : " + e.Message);
                                        }
                                    }
                                }
                            }
                            break;
                    }

                }
            }
        }

        //public static string getHeading(string strSourceDirectory, string strFilename)
        //{
        //    string strPathAndFilename = getFileNameAndDirectory(strSourceDirectory, strFilename);
        //    if (System.IO.File.Exists(strPathAndFilename))
        //    {

        //    }
        //}
        public static string getFileNameAndDirectory(string strSourceDirectory, string strFileName)
        {
            if (strFileName == null) return "";
            try
            {
                if (Char.IsLetter(strFileName[0]))
                    return strSourceDirectory + "letter" + strFileName[0] + "\\" + strFileName;
                else
                    return strSourceDirectory + "altsub\\" + strFileName;
            }
            catch (Exception e)
            {
                MessageBox.Show("error :" + e.Message);
            }

            return "";
        }
    }
}
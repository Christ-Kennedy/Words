using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace Words
{
    internal class classRhymeDictionary
    {
        const string TT_Tree_BaseFilename = "CK_RhymingDictionary.tree";
        const string RhymeCompleteFilename = "CK_RhymingDictionary_Complete.txt";

        static BackgroundWorker bck = new BackgroundWorker();

        static classTTLeaf cRoot = null;
        public static string Filename_Tree { get { return System.IO.Directory.GetCurrentDirectory() + "\\" + TT_Tree_BaseFilename; } }
        static string CompleteFilename { get { return System.IO.Directory.GetCurrentDirectory() + "\\" + RhymeCompleteFilename; } }
        static bool RhymeTreeDataFileCoomplete 
        { 
            get 
            {
                return System.IO.File.Exists(CompleteFilename);
            } 
        }

        public static FileStream fs_Tree = null;

        public static long Addr_Append
        {
            get
            {
                return fs_Tree != null
                                ? fs_Tree.Length
                                : 0;
            }
        }

        public static void Init()
        {
            if (RhymeTreeDataFileCoomplete) return;
            if (System.IO.File.Exists(Filename_Tree))
            {
                // previous attempt to build it was interrupted -> delete existing file
                System.IO.File.Delete(Filename_Tree);
            }
            bck.WorkerReportsProgress = true;
            bck.WorkerSupportsCancellation = true;
            bck.DoWork += Bck_DoWork;
            bck.ProgressChanged += Bck_ProgressChanged;
            bck.RunWorkerCompleted += Bck_RunWorkerCompleted;

            bck.RunWorkerAsync();
        }

        #region BackgroundWorker
        static private void Bck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.IO.File.WriteAllText(RhymeCompleteFilename, "Complete");

            formWords.Message(" --------- Rhyme Dictionary Ready -----------");
        }

        static int intFeedbackCounter = 0;
        static string BuildFeedback 
        { 
            get 
            {
                intFeedbackCounter = (intFeedbackCounter += 1) % 16;
                string strMsg = "building Rhyme Dictionary " + "*****************************************".Substring(0,intFeedbackCounter);
                return strMsg;
            } 
        }

        static private void Bck_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            formWords.Message(BuildFeedback);
        }


        static private void Bck_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] strListFiles = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "RhymeDictionary_*.txt");
            char[] chrSplit = { ',' };

            for (int intFileCounter = 0; intFileCounter < strListFiles.Length; intFileCounter++)
            {
                string strText = System.IO.File.ReadAllText(strListFiles[intFileCounter]);
                string[] strListWords = strText.Split(chrSplit, StringSplitOptions.RemoveEmptyEntries);

                for (int intWordCounter = 0; intWordCounter < strListWords.Length; intWordCounter++)
                {
                    WordCurrent = strListWords[intWordCounter];

                    Insert(WordCurrent);
                    if (intWordCounter % 256 == 0)
                        bck.ReportProgress(-1);

                    if (bck.CancellationPending)
                        return;
                }
            }
        }


        static string strWordCurrent = "";
        static public string WordCurrent
        {
            get { return strWordCurrent; }
            set { strWordCurrent = value; }
        }

        #endregion 

        public static void Insert(string strWord)
        {
            if (fs_Tree == null) FS_Init();
            List<int> lstSyllables = classRD_DissectWord.DissectWord(strWord);

            cRoot = classTTLeaf.Read(0);

            if (cRoot == null)
            {
                cRoot = new classTTLeaf();
                cRoot.key = 0;
                cRoot.Addr = 0;

                classTTLeaf.Write(ref cRoot);
            }

            classTTLeaf cLeaf = cRoot;

            int intSyllable = lstSyllables[0];
            lstSyllables.RemoveAt(0);

            do
            {
                int intComparison = Math.Sign((ushort)intSyllable - (ushort)cLeaf.key);
                if (intComparison < 0)
                {
                    if (cLeaf.Left >= 0)
                        cLeaf = classTTLeaf.Read(cLeaf.Left);
                    else
                    {
                        cLeaf.Left = Addr_Append;
                        classTTLeaf.Write(ref cLeaf);

                        classTTLeaf cLeaf_New = new classTTLeaf();
                        cLeaf_New.key = intSyllable;
                        classTTLeaf.Append(ref cLeaf_New);

                        if (lstSyllables.Count > 0)
                        {
                            cLeaf = cLeaf_New;
                        }
                        else
                            break;
                    }
                }
                else if (intComparison > 0)
                {
                    if (cLeaf.Right >= 0)
                        cLeaf = classTTLeaf.Read(cLeaf.Right);
                    else
                    {
                        cLeaf.Right = Addr_Append;
                        classTTLeaf.Write(ref cLeaf);

                        classTTLeaf cLeaf_New = new classTTLeaf();
                        cLeaf_New.key = intSyllable;
                        classTTLeaf.Append(ref cLeaf_New);

                        if (lstSyllables.Count > 0)
                        {
                            cLeaf = cLeaf_New;
                        }
                        else
                            break;
                    }
                }
                else
                {  // found matching syllable

                    // front end LL insertion to this TTLeaf
                    classRD_LLItem cLLNew = new classRD_LLItem(strWord);
                    cLLNew.Next = cLeaf.LL;
                    cLeaf.LL = classRD_LLItem.Append(ref cLLNew);
                    cLeaf.LL_Count++;
                    classTTLeaf.Write(ref cLeaf);

                    if (lstSyllables.Count > 1)
                    {
                        if (cLeaf.Next >= 0)
                        {
                            cLeaf = classTTLeaf.Read(cLeaf.Next);
                        }
                        else
                        {
                            cLeaf.Next = Addr_Append;
                            classTTLeaf.Write(ref cLeaf);

                            classTTLeaf cLeaf_New = new classTTLeaf();
                            cLeaf_New.key = lstSyllables[0];
                            classTTLeaf.Append(ref cLeaf_New);
                            cLeaf = cLeaf_New;
                        }

                        intSyllable = lstSyllables[0];
                        lstSyllables.RemoveAt(0);
                    }
                    else
                        break;
                }
            } while (true);
        }


        /// <summary>
        /// returns list of DataInfo type for word being searched
        /// </summary>
        /// <param name="strWord">word searched</param>
        /// <returns>list of dataInfo showing ways the searched word's spelling can be gotten - may include UNIDENTIFIED DELETED WORDS if ShowDelted</returns>
        public static List<List<string>> Search(string strWord) { return Search(strWord, uint.MaxValue); }
        public static List<List<string>> Search(string strWord, uint untMaxResults)
        {
            if (!classRhymeDictionary.RhymeTreeDataFileCoomplete) return new List<List<string>>();

            List<int> lstSyllables = classRD_DissectWord.DissectWord(strWord);
            if (lstSyllables.Count == 0) return new List<List<string>>();

            List<classTTLeaf> lstTTLeaf = new List<classTTLeaf>();
            if (strWord.Length == 0) return new List<List<string>>();
            cRoot = classTTLeaf.Read(0);
            if (cRoot == null)
            {
                if (fs_Tree == null)
                {
                    FS_Init();
                    try
                    {
                        cRoot = classTTLeaf.Read(0);
                    }
                    catch (Exception)
                    {
                        return new List<List<string>>();
                    }
                }
            }

            if (cRoot == null)
                return new List<List<string>>();

            classTTLeaf cLeaf = cRoot;

            int intSyllable = lstSyllables[0];
            lstSyllables.RemoveAt(0);

            do
            {
                ushort uSyllable = (ushort)intSyllable;
                ushort uLeafKey = (ushort)cLeaf.key;

                int intComparison = Math.Sign((ushort)intSyllable - (ushort)cLeaf.key);

                if (intComparison < 0)
                {
                    if (cLeaf.Left >= 0)
                        cLeaf = classTTLeaf.Read(cLeaf.Left);
                    else
                        goto exit;
                }
                else if (intComparison > 0)
                {
                    if (cLeaf.Right >= 0)
                        cLeaf = classTTLeaf.Read(cLeaf.Right);
                    else
                        goto exit;
                }
                else
                {
                    lstTTLeaf.Add(cLeaf);

                    if (cLeaf.Next <= 0 || lstSyllables.Count == 0)
                        goto exit;

                    cLeaf = classTTLeaf.Read(cLeaf.Next);
                    intSyllable = lstSyllables[0];
                    lstSyllables.RemoveAt(0);
                }
            } while (true);

        exit:

            List<List<string>> lstRetVal = new List<List<string>>();
            List<string> lstPrevious = new List<string>();
            for (int intResultCounter = lstTTLeaf.Count - 1; intResultCounter >= 0; intResultCounter--)
            {
                classRhymeDictionary.classTTLeaf cTTLeaf = lstTTLeaf[intResultCounter];
                if (cTTLeaf.LL_Count <= untMaxResults)
                {
                    List<string> lstTemp = new List<string>();
                    long lngLL = cTTLeaf.LL;
                    while (lngLL >= 0)
                    {
                        classRD_LLItem cLL = classRD_LLItem.Read(lngLL);
                        if (!lstPrevious.Contains(cLL.Word))
                            lstTemp.Add(cLL.Word);
                        if (cLL != null)
                        {
                            lngLL = cLL.Next;
                        }
                        else
                            lngLL = -1;
                    }
                    lstRetVal.Insert(0, lstTemp);
                    lstPrevious.AddRange(lstTemp);
                }
                else
                {
                    // /report dropped info
                    List<string> lstTemp = new List<string>();
                    lstTemp.Add("(" + cTTLeaf.LL_Count.ToString() + ") dropped");
                    lstRetVal.Insert(0, lstTemp);
                }
            }

            return lstRetVal;
        }


        public static void FS_Init()
        {
            if (fs_Tree == null)
            {
                if (System.IO.File.Exists(Filename_Tree))
                    fs_Tree = new FileStream(Filename_Tree, FileMode.Open);
                else
                {
                    fs_Tree = new FileStream(Filename_Tree, FileMode.Create);
                    classTTLeaf cRootLeaf = new classTTLeaf();
                    cRootLeaf.key = 0;
                    cRootLeaf.Addr = 0;
                    classTTLeaf.Write(ref cRootLeaf);
                }

            }
        }


        static void ThisShouldNotHappen()
        {
            MessageBox.Show("this should not happen");
        }

        public class classTTLeaf
        {
            public static string strSearchKey = "";

            public long Left = -1;
            public long Right = -1;
            public long Next = -1;

            public long LL = -1;

            public uint LL_Count = 0;

            int intKey;
            public int key
            {
                get { return intKey; }
                set { intKey = value; }
            }

            public long Addr = -1;

            #region Statics

            static BinaryFormatter formatter = new BinaryFormatter();
            static FileStream fs { get { return fs_Tree; } }



            ///// <summary>
            ///// returns list of DataInfo type for word being Geted
            ///// </summary>
            ///// <param name="strWord">word Geted</param>
            ///// <param name="bolValid">when ShowDeletedWords is true this boolean will be true if the word found has been deleted</param>
            ///// <returns>list of dataInfo showing ways the Geted word's spelling can be gotten</returns>
            //public static List<classTTLeaf> Get(string strWord)
            //{
            //    if (strWord.Length == 0) return null;
            //    cRoot = classTTLeaf.Read(0);
            //    if (cRoot == null)
            //    {
            //        if (fs_Tree == null)
            //        {
            //            FS_Init();
            //            try
            //            {
            //                cRoot = classTTLeaf.Read(0);
            //            }
            //            catch (Exception)
            //            {
            //                return null;
            //            }
            //        }
            //    }

            //    if (cRoot == null)
            //        return null;

            //    List<enuSyllables> lstSyllables = classRD_DissectWord.DissectWord(strWord);

            //    List<classTTLeaf> lstRetVal = new List<classTTLeaf>();

            //    classTTLeaf cLeaf = cRoot;

            //    do
            //    {
            //        enuSyllables eSyllable = lstSyllables[lstSyllables.Count - 1];
            //        lstSyllables.RemoveAt(lstSyllables.Count - 1);

            //        int intComparison = Math.Sign((ushort)eSyllable - (ushort)cLeaf.key);

            //        if (intComparison < 0)
            //        {
            //            if (cLeaf.Left >= 0)
            //                cLeaf = classTTLeaf.Read(cLeaf.Left);
            //            else
            //                return lstRetVal;
            //        }
            //        else if (intComparison > 0)
            //        {
            //            if (cLeaf.Right >= 0)
            //                cLeaf = classTTLeaf.Read(cLeaf.Right);
            //            else
            //                return lstRetVal;
            //        }
            //        else
            //        {
            //            if (cLeaf.Next >= 0)
            //            {
            //                strWord = strWord.Substring(1);
            //                if (strWord.Length == 0)
            //                    return lstRetVal;
            //                cLeaf = classTTLeaf.Read(cLeaf.Next);
            //            }
            //            else
            //            {
            //                return lstRetVal;

            //                //if (strWord.Length > 1)
            //                //    return null;
            //                //else
            //                //    return lstRetVal;
            //            }
            //        }
            //    } while (lstSyllables.Count >0);


            //    return null;
            //}

            public static classTTLeaf Read(long lngAddr)
            {
                if (lngAddr < 0) return null;
                if (fs == null)
                    classRhymeDictionary.FS_Init();

                fs.Position = lngAddr;

                if (fs.Position > fs.Length)
                    return null;
                try
                {

                    classTTLeaf cRetVal = new classTTLeaf();

                    cRetVal.Addr = lngAddr;

                    cRetVal.Left = (long)formatter.Deserialize(fs);
                    cRetVal.Right = (long)formatter.Deserialize(fs);
                    cRetVal.Next = (long)formatter.Deserialize(fs);

                    cRetVal.key = (int)formatter.Deserialize(fs);
                    cRetVal.LL = (long)formatter.Deserialize(fs);
                    cRetVal.LL_Count = (uint)formatter.Deserialize(fs);

                    return cRetVal;
                }
                catch (Exception)
                {
                    return null;
                }

            }

            /// <summary>
            /// writes Leaf to end of filestream and returns its Index
            /// </summary>
            /// <param name="cLeaf">Leaf to be written</param>
            /// <returns>index where it is located</returns>
            public static void Append(ref classTTLeaf cLeaf)
            {
                cLeaf.Addr = fs_Tree.Length;
                Write(ref cLeaf);
            }

            public static void Write(ref classTTLeaf cLeaf)
            {
                fs_Tree.Position = cLeaf.Addr;

                formatter.Serialize(fs_Tree, (long)cLeaf.Left);
                formatter.Serialize(fs_Tree, (long)cLeaf.Right);
                formatter.Serialize(fs_Tree, (long)cLeaf.Next);

                formatter.Serialize(fs_Tree, (int)cLeaf.key);
                formatter.Serialize(fs_Tree, (long)cLeaf.LL);
                formatter.Serialize(fs_Tree, (uint)cLeaf.LL_Count);
            }

            #endregion
        }


    }

    public class classRD_LLItem
    {
        public bool Valid = true;
        public long Next = -1;

        public string Word = "";
        const string TT_Data_BaseFilename = "FGB.data";
        public static string Filename_Data { get { return WorkingDirectory + TT_Data_BaseFilename; } }

        static string _strWorkingDirectory = @"C:\Dlús\";
        static public string WorkingDirectory
        {
            get { return _strWorkingDirectory; }
            set
            {
                if (string.Compare(_strWorkingDirectory.ToUpper(), value.ToUpper()) != 0)
                {
                    if (!System.IO.Directory.Exists(value))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(value);
                            _strWorkingDirectory = value;
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("error creating working directory \"" + value + "\" :" + err.Message);
                        }
                    }
                }
            }
        }


        static FileStream fs
        { get { return classRhymeDictionary.fs_Tree; } }
        static Semaphore semFS = new Semaphore(1, 1);

        public long Addr = -1;

        public classRD_LLItem() { }

        public classRD_LLItem(string strWord)
        {
            this.Word = strWord;
        }

        #region Statics

        static BinaryFormatter formatter = new BinaryFormatter();


        public static long NextAddress { get { return fs.Length; } }


        /// <summary>
        /// writes data to end of filestream and returns its Index
        /// </summary>
        /// <param name="cData">data to be written</param>
        /// <returns>address where it is located</returns>
        public static long Append(ref classRD_LLItem cData)
        {
            long lngRetVal = NextAddress;
            Write(lngRetVal, ref cData);
            return lngRetVal;
        }

        public static long Append(string strWord)
        {
            classRD_LLItem cLLNew = new classRD_LLItem(strWord);
            Write(NextAddress, ref cLLNew);
            return cLLNew.Addr;
        }

        public static void Write(ref classRD_LLItem cData) { Write(cData.Addr, ref cData); }
        public static void Write(long lngAddr, ref classRD_LLItem cData)
        {
            semFS.WaitOne();
            fs.Position
                = cData.Addr
                = lngAddr;
            formatter.Serialize(fs, (long)cData.Next);
            formatter.Serialize(fs, (string)cData.Word);
            semFS.Release();
        }

        public static classRD_LLItem Read(long lngAddr)
        {
            if (lngAddr < 0) return null;

            semFS.WaitOne();

            classRD_LLItem cRetVal = new classRD_LLItem();

            fs.Position
                = cRetVal.Addr
                = lngAddr;

            try
            {
                cRetVal.Next = (long)formatter.Deserialize(fs);
                cRetVal.Word = (string)formatter.Deserialize(fs);
            }
            catch (Exception)
            {
            }

            semFS.Release();
            return cRetVal;
        }

        #endregion
    }

    public class classSounds
    {
        public List<string> lstText = new List<string>();
        static int intIDCounter = 0;
        int intID = intIDCounter++;
        public int ID { get { return intID; } }
        public classSounds() { }
        public classSounds(string strSound)
        {
            lstText.Add(strSound);
        }

        public classSounds(string[] strSounds)
        {
            lstText.AddRange(strSounds.ToArray<string>());
        }
    }

    public class classRD_DissectWord
    {
        static int _intPrefixes_Start = -1;
        static public int Prefixes_Start
        {
            get { return _intPrefixes_Start; }
        }
        static int _intPrefixes_End = -1;
        static public int Prefixes_End
        {
            get { return _intPrefixes_End; }
        }
        static int _intClusters_Start = -1;
        static public int Clusters_Start
        {
            get { return _intClusters_Start; }
        }
        static int _intClusters_End = -1;
        static public int Clusters_End
        {
            get { return _intClusters_End; }
        }
        static int _intSuffixes_Start = -1;
        static public int Suffixes_Start
        {
            get { return _intSuffixes_Start; }
        }
        static int _intSuffixes_End = -1;
        static public int Suffixes_End
        {
            get { return _intSuffixes_End; }
        }

        static List<classSounds> lstSounds = new List<classSounds>();


        static void SoundsInit()
        {

            lstSounds.Add(new classSounds("NULL"));

            _intPrefixes_Start = lstSounds.Count;
            lstSounds.Add(new classSounds("extra"));
            lstSounds.Add(new classSounds("hyper"));
            lstSounds.Add(new classSounds("inter"));
            lstSounds.Add(new classSounds("super"));
            lstSounds.Add(new classSounds("trans"));
            lstSounds.Add(new classSounds("ultra"));
            lstSounds.Add(new classSounds("under"));
            lstSounds.Add(new classSounds("anti"));
            lstSounds.Add(new classSounds("auto"));
            lstSounds.Add(new classSounds("down"));
            lstSounds.Add(new classSounds("mega"));
            lstSounds.Add(new classSounds("over"));
            lstSounds.Add(new classSounds("post"));
            lstSounds.Add(new classSounds("semi"));
            lstSounds.Add(new classSounds("tele"));
            lstSounds.Add(new classSounds("con"));
            lstSounds.Add(new classSounds("dis"));
            lstSounds.Add(new classSounds("mid"));
            lstSounds.Add(new classSounds("mis"));
            lstSounds.Add(new classSounds("non"));
            lstSounds.Add(new classSounds("out"));
            lstSounds.Add(new classSounds("pre"));
            lstSounds.Add(new classSounds("pro"));
            lstSounds.Add(new classSounds("sub"));
            lstSounds.Add(new classSounds("de"));
            lstSounds.Add(new classSounds("il"));
            lstSounds.Add(new classSounds("im"));
            lstSounds.Add(new classSounds("ir"));
            lstSounds.Add(new classSounds("in"));
            lstSounds.Add(new classSounds("re"));
            lstSounds.Add(new classSounds("un"));
            lstSounds.Add(new classSounds("up"));
            _intPrefixes_End = lstSounds.Count - 1;

            //  tripthongs      -       diphthongs      -       Consonantal Clusters        -       letters
            _intClusters_Start = lstSounds.Count;
            classSounds cSound = new classSounds();
            cSound.lstText.Add("ayer");
            cSound.lstText.Add("ower");
            cSound.lstText.Add("oyer");
            cSound.lstText.Add("our");
            cSound.lstText.Add("ure");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("eous");
            cSound.lstText.Add("ious");
            cSound.lstText.Add("ius");
            cSound.lstText.Add("iuz");
            cSound.lstText.Add("iouz");
            cSound.lstText.Add("eouz");
            cSound.lstText.Add("euz");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ove");
            cSound.lstText.Add("ote");
            cSound.lstText.Add("ode");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ouse");
            cSound.lstText.Add("ooze");
            cSound.lstText.Add("oose");
            cSound.lstText.Add("ous");
            cSound.lstText.Add("us");
            cSound.lstText.Add("oz");
            cSound.lstText.Add("uz");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ourt");
            cSound.lstText.Add("ort");
            cSound.lstText.Add("ert");
            cSound.lstText.Add("art");
            cSound.lstText.Add("irt");
            cSound.lstText.Add("urt");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("our");
            cSound.lstText.Add("or");
            cSound.lstText.Add("er");
            cSound.lstText.Add("ar");
            cSound.lstText.Add("ure");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ame");
            cSound.lstText.Add("ain");
            cSound.lstText.Add("aim");
            cSound.lstText.Add("ane");
            cSound.lstText.Add("ein");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ique");
            cSound.lstText.Add("ick");
            cSound.lstText.Add("ic");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("igh");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("re");
            cSound.lstText.Add("eɪ");
            cSound.lstText.Add("ay");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ou");
            cSound.lstText.Add("ew");
            cSound.lstText.Add("oy");
            cSound.lstText.Add("oo");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ow");
            cSound.lstText.Add("eau");
            cSound.lstText.Add("au");
            cSound.lstText.Add("oo");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ea");
            cSound.lstText.Add("ee");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ch");
            cSound.lstText.Add("th");
            cSound.lstText.Add("sh");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ph");
            cSound.lstText.Add("gh");
            cSound.lstText.Add("ff");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("bl");
            cSound.lstText.Add("cl");
            cSound.lstText.Add("fl");
            cSound.lstText.Add("gl");
            cSound.lstText.Add("pl");
            cSound.lstText.Add("sl");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("br");
            cSound.lstText.Add("cr");
            cSound.lstText.Add("dr");
            cSound.lstText.Add("fr");
            cSound.lstText.Add("gr");
            cSound.lstText.Add("pr");
            cSound.lstText.Add("tr");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ng");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("qu");
            cSound.lstText.Add("kw");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("sk");
            cSound.lstText.Add("sm");
            cSound.lstText.Add("sn");
            cSound.lstText.Add("sp");
            cSound.lstText.Add("st");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("tw");
            cSound.lstText.Add("sw");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("and");
            cSound.lstText.Add("end");
            cSound.lstText.Add("und");
            cSound.lstText.Add("unt");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("sc");
            cSound.lstText.Add("ss");
            cSound.lstText.Add("z");
            cSound.lstText.Add("s");
            lstSounds.Add(cSound);

            // letters

            cSound = new classSounds();
            cSound.lstText.Add("b");
            cSound.lstText.Add("d");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("p");
            cSound.lstText.Add("t");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("a");
            cSound.lstText.Add("u");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("m");
            cSound.lstText.Add("n");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("g");
            cSound.lstText.Add("j");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("s");
            cSound.lstText.Add("z");
            cSound.lstText.Add("v");
            cSound.lstText.Add("x");
            lstSounds.Add(cSound);

            lstSounds.Add(new classSounds("c"));
            lstSounds.Add(new classSounds("e"));
            lstSounds.Add(new classSounds("f"));
            lstSounds.Add(new classSounds("h"));
            lstSounds.Add(new classSounds("i"));
            lstSounds.Add(new classSounds("k"));
            lstSounds.Add(new classSounds("l"));
            lstSounds.Add(new classSounds("o"));
            lstSounds.Add(new classSounds("q"));
            lstSounds.Add(new classSounds("r"));
            lstSounds.Add(new classSounds("w"));
            lstSounds.Add(new classSounds("y"));
            _intClusters_End = lstSounds.Count - 1;

            // suffixes
            _intSuffixes_Start = lstSounds.Count;
            lstSounds.Add(new classSounds("istic"));
            lstSounds.Add(new classSounds("wards"));
            lstSounds.Add(new classSounds("able"));
            lstSounds.Add(new classSounds("hood"));
            lstSounds.Add(new classSounds("ful"));
            lstSounds.Add(new classSounds("ier"));
            lstSounds.Add(new classSounds("ify"));
            lstSounds.Add(new classSounds("ing"));
            lstSounds.Add(new classSounds("ity"));
            lstSounds.Add(new classSounds("al"));
            lstSounds.Add(new classSounds("ed"));
            lstSounds.Add(new classSounds("en"));
            lstSounds.Add(new classSounds("ry"));
            lstSounds.Add(new classSounds("ty"));
            lstSounds.Add(new classSounds("dom"));
            lstSounds.Add(new classSounds("ship"));

            cSound = new classSounds();
            cSound.lstText.Add("ence");
            cSound.lstText.Add("ance");
            cSound.lstText.Add("unce");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ible");
            cSound.lstText.Add("able");
            cSound.lstText.Add("ouble");
            cSound.lstText.Add("uble");
            cSound.lstText.Add("oodle");
            cSound.lstText.Add("ootle");
            cSound.lstText.Add("utle");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ord");
            cSound.lstText.Add("ard");
            cSound.lstText.Add("urd");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ee");
            cSound.lstText.Add("ea");
            cSound.lstText.Add("y");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("end");
            cSound.lstText.Add("ent");
            cSound.lstText.Add("ant");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("less");
            cSound.lstText.Add("ness");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("sion");
            cSound.lstText.Add("tion");
            cSound.lstText.Add("xion");
            cSound.lstText.Add("shun");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ian");
            cSound.lstText.Add("ion");
            cSound.lstText.Add("ien");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("wise");
            cSound.lstText.Add("ize");
            cSound.lstText.Add("ise");
            cSound.lstText.Add("es");
            cSound.lstText.Add("is");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("eight");
            cSound.lstText.Add("age");
            cSound.lstText.Add("ate");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ight");
            cSound.lstText.Add("ied");
            cSound.lstText.Add("ide");
            cSound.lstText.Add("ite");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ish");
            cSound.lstText.Add("ist");
            cSound.lstText.Add("est");
            cSound.lstText.Add("ism");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ease");
            cSound.lstText.Add("eeze");
            cSound.lstText.Add("ese");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("ies");
            cSound.lstText.Add("ive");
            lstSounds.Add(cSound);

            cSound = new classSounds();
            cSound.lstText.Add("elle");
            cSound.lstText.Add("alle");
            cSound.lstText.Add("olle");
            cSound.lstText.Add("ulle");
            cSound.lstText.Add("ell");
            cSound.lstText.Add("all");
            cSound.lstText.Add("oll");
            cSound.lstText.Add("ull");
            cSound.lstText.Add("el");
            cSound.lstText.Add("al");
            cSound.lstText.Add("ol");
            cSound.lstText.Add("ul");
            cSound.lstText.Add("le");
            lstSounds.Add(cSound);
            _intSuffixes_End = lstSounds.Count - 1;
        }


        public static string Deaccent(string strText)
        {
            if (strText == null) return "";
            string strReplace = "ĀAÀAÁAÂAÃAÄAÅAāaàaáaâaãaäaåaßBÇCçcĒEÈEÉEÊEËEēeèeéeêeëeĪIÌIÍIÎIÏIīiìiíiîiïiÑNñnŌOÒOÓOÔOÕOÖOōoðoòoóoôoõoöoŪUÙUÚUÛUÜUūuùuúuûuüuÝYýyÿyĀAĒEĪIŌOŪUāaēeīiōoūu";

            while (strText.Contains("Æ"))
                strText = strText.Replace("Æ", "AE");
            while (strText.Contains("æ"))
                strText = strText.Replace("æ", "ae");

            if (strText.Length < strReplace.Length / 2)
            {
                for (int intChar = 0; intChar < strText.Length; intChar++)
                {
                    char chr = strText[intChar];
                    int intIndex = strReplace.IndexOf(chr);
                    if (intIndex >= 0 && intIndex % 2 == 0)
                    {
                        char chrReplace = strReplace[intIndex + 1];
                        while (strText.Contains(chr))
                            strText = strText.Replace(chr, chrReplace);
                    }
                }
            }
            else
            {
                for (int intChar = 0; intChar < strReplace.Length; intChar += 2)
                {
                    char chr = strReplace[intChar];
                    char chrReplace = strReplace[intChar + 1];

                    while (strText.Contains(chr))
                        strText = strText.Replace(chr, chrReplace);
                }
            }
            return strText;
        }


        public static List<int> DissectWord(string strWord)
        {
            if (lstSounds.Count == 0)
                SoundsInit();

            string strDebugCopy = strWord;
            bool bolDebug = false;
            if (bolDebug)
                strWord = strDebugCopy;

            List<int> lstRetVal = new List<int>();
            strWord = Deaccent(strWord).ToLower();

            // replace non-alpha char with enum.NULL
            classSounds cNULL = lstSounds[0];
            string strNULL = EnumReplacement(ref cNULL);

            for (int intLetterCounter = strWord.Length - 1; intLetterCounter >= 0; intLetterCounter--)
            {
                char chrTest = strWord[intLetterCounter];
                if (!char.IsLetter(chrTest))
                {
                    string strLeft = strWord.Substring(0, intLetterCounter);
                    string strRight = intLetterCounter < strWord.Length - 1
                                                       ? strWord.Substring(intLetterCounter + 1)
                                                       : "";
                    strWord = strLeft + strNULL + strRight;
                }
            }

            //              prefixes
            for (int intPrefixCounter = Prefixes_Start; intPrefixCounter <= Prefixes_End; intPrefixCounter++)
            {
                classSounds cPrefix = lstSounds[intPrefixCounter];
                string strEnumReplacement = EnumReplacement(ref cPrefix);

                for (int intTextCounter = 0; intTextCounter < cPrefix.lstText.Count; intTextCounter++)
                {
                    string strPrefix = cPrefix.lstText[intTextCounter];

                    if (strWord.Length > strPrefix.Length)
                    {
                        if (string.Compare(strWord.Substring(0, strPrefix.Length), strPrefix) == 0)
                        {
                            // prefix matches word
                            strWord = strEnumReplacement + strWord.Substring(strPrefix.Length);
                            goto exitPrefix;
                        }
                    }
                }
            }
        exitPrefix:


            //              suffixes
            for (int intSuffixCounter = Suffixes_Start; intSuffixCounter <= Suffixes_End; intSuffixCounter++)
            {
                classSounds cSuffix = lstSounds[intSuffixCounter];
                string strEnumReplacement = EnumReplacement(ref cSuffix);

                for (int intTextCounter = 0; intTextCounter < cSuffix.lstText.Count; intTextCounter++)
                {
                    string strSuffix = cSuffix.lstText[intTextCounter];

                    if (strWord.Length > strSuffix.Length)
                    {
                        string strWordEnd = strWord.Substring(strWord.Length - strSuffix.Length);
                        if (string.Compare(strWordEnd, strSuffix) == 0)
                        {
                            // Suffix matches word
                            strWord = strWord.Substring(0, strWord.Length - strSuffix.Length) + strEnumReplacement;
                            goto exitSuffixes;
                        }
                    }
                }
            }
        exitSuffixes:

            //              consonantal
            for (int intClusterCounter = Clusters_Start; intClusterCounter <= Clusters_End; intClusterCounter++)
            {
                classSounds cSound = lstSounds[intClusterCounter];
                string strEnumReplacement = EnumReplacement(ref cSound);

                for (int intTextCounter = 0; intTextCounter < cSound.lstText.Count; intTextCounter++)
                {
                    string strCluster = cSound.lstText[intTextCounter];
                    if (strWord.Length >= strCluster.Length)
                    {
                        if (strWord.Contains(strCluster))
                        {
                            // Cluster matches word
                            DissectWord_ReplaceEnum(ref strWord, ref cSound);
                        }
                    }
                }
            }

            char[] chrSplit = { ']', '[' };
            string[] strEnumList = strWord.Split(chrSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int intCounter = strEnumList.Length - 1; intCounter >= 0; intCounter--)
            {
                string strEnum = strEnumList[intCounter];
                try
                {
                    int intEnum = Convert.ToInt32(strEnum);
                    int eItem = (int)intEnum;
                    lstRetVal.Add(eItem);
                }
                catch (Exception)
                {
                }
            }

            return lstRetVal;
        }

        static string EnumReplacement(ref classSounds cSound)
        {
            return "[" + cSound.ID.ToString() + "]";
        }

        static void DissectWord_ReplaceEnum(ref string strWord, ref classSounds cSound)
        {
            string strDebugCopy = strWord;

            string strReplacement = EnumReplacement(ref cSound);
            for (int intSoundCounter = 0; intSoundCounter < cSound.lstText.Count; intSoundCounter++)
            {
                string strSyllable = cSound.lstText[intSoundCounter];
                strWord = strWord.Replace(strSyllable, strReplacement);
            }
        }
    }
}
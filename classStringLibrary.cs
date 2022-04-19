using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Math3;

namespace StringLibrary
{
    public class classStringLibrary
    {
        private static string _strNonAlphaCharacters = "";
        private static string strNonAlphaCharacters
        {
            get
            {
                if (_strNonAlphaCharacters.Length == 0)
                {
                    _strNonAlphaCharacters += "\r\n\t";
                    for (int intCounter = 32; intCounter < 256; intCounter++)
                    {
                        char chrNew = (char)intCounter;
                        if (!char.IsLetter(chrNew))
                            _strNonAlphaCharacters += chrNew.ToString();
                    }
                }
                return _strNonAlphaCharacters;
            }
        }
        private static char[] chrNonAlphaCharacters = null;
        public static classStringLibrary instance = null;
        public static string strMacrons = "ĀAĒEĪIŌOŪUāaēeīiōoūu";
        private static string strVowels = "AaEeIiOoUuYyĀAĒEĪIŌOŪUÝYāaēeīiōoūu";
        private static bool bolRejectedCharactersFileHasBeenRead = false;
        public static string strAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzĀAĒEĪIŌOŪUāaēeīiōoūuýy";
        public static string strAlpha_LowerCase = "abcdefghijklmnopqrstuvwxyz";
        public static string strAlpha_LowerCase_Space = "abcdefghijklmnopqrstuvwxyz ";
        public static string strAlpha_UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static string strRejectedCharacters = "";
        private static char[] chrRejectedCharacters = new char[0];


        public static int RTX_RightMargin_Widest(ref RichTextBox rtx)
        {
            string strRTF = rtx.Rtf;
            int intLeast_RI = int.MaxValue;
            string strRightMargin = "\\ri";
            int intIndex = strRTF.IndexOf(strRightMargin);
            while (intIndex > 0)
            {
                int intEnd = strRTF.IndexOf("\\", intIndex + 1);
                if (intEnd > intIndex)
                {
                    string strFind = strRTF.Substring(intIndex, intEnd - intIndex);
                    try
                    {
                        int intFind = Convert.ToInt32(strFind.Substring(strRightMargin.Length));
                        if (intFind < intLeast_RI)
                            intLeast_RI = intFind;
                    }
                    catch (Exception)
                    {
                    }
                }

                intIndex = strRTF.IndexOf(strRightMargin, intIndex + 1);
            }
            int intMax = 999;// some value I don't know
            return intMax - intLeast_RI;
        }

        public static void RTX_AppendNL(ref RichTextBox rtx)
        {
            rtx.Select(rtx.Text.Length, 0);
            rtx.SelectedText = "\r";
            rtx.Select(rtx.Text.Length, 0);
            //rtx.SaveFile(@"c:\debug\rtx.rtf");
        }

        public static void RTX_AppendTab(ref RichTextBox rtx)
        {
            rtx.Select(rtx.Text.Length, 0);
            rtx.SelectedText = "\t";
            rtx.Select(rtx.Text.Length, 0);
            rtx.SaveFile(@"c:\debug\rtx.rtf");
        }

        public static void RTX_SelectWordUnderMouse(ref RichTextBox rtx)
        {
            if (rtx == null) return;
            int intStart = rtx.SelectionStart;
            if (intStart > 0 && char.IsLetter(rtx.Text[intStart - 1]))
                intStart--;

            char chrTest = rtx.Text[intStart];
            while (char.IsLetter(chrTest) && intStart > 0)
                chrTest = rtx.Text[--intStart];

            int intEnd = rtx.SelectionStart;
            chrTest = rtx.Text[intEnd];
            while (char.IsLetter(chrTest) && intEnd < rtx.Text.Length - 2)
                chrTest = rtx.Text[++intEnd];
            //string strTemp = rtx.Text.Substring(intStart, intEnd - intStart);
            rtx.Select(intStart + 1, intEnd - intStart - 1);
        }

        public static void RTX_AppendText(ref RichTextBox rtx, string strText, Font fnt, Color clr, int intIndent)
        {
            if (strText.Length == 0) return;

            rtx.SelectionIndent = intIndent;
            rtx.SelectionRightIndent = 15;

            if (rtx.Text.Length == 0)
            {
                rtx.Font = fnt;
                rtx.ForeColor = clr;

                rtx.Text += strText;
            }
            else
            {
                rtx.Select(rtx.Text.Length, 0);
                int intSelectStart = rtx.SelectionStart;
                rtx.SelectedText = strText;
                rtx.Select(intSelectStart, strText.Length);
                rtx.SelectionFont = fnt;
                rtx.SelectionColor = clr;
            }

            rtx.Select(rtx.Text.Length, 0);
            //rtx.SaveFile(@"c:\debug\rtx.rtf");
        }

        public static string clean_nonAlpha_Content(string strSource)
        {
            string strRetVal = "";

            foreach (char c in strSource)
                if (strAlpha_LowerCase.Contains(c) || strAlpha_UpperCase.Contains(c))
                    strRetVal += c.ToString();


            return strRetVal;
        }

        public static List<string> getLines(string strSource)
        {
            char[] chrSplit = { '\n' };
            return strSource.Split(chrSplit).ToList<string>();
        }

        public static string clean_nonAlpha_Ends(string strSource)
        {
            return cleanBack_nonAlpha(cleanFront_nonAlpha(strSource));
        }

        public static string cleanFront_nonAlpha(string strSource)
        {
            while (strSource.Length > 0
                    &&
                 !char.IsLetter(strSource[0]))
            {
                strSource = strSource.Substring(1);
            }
            return strSource;
        }

        public static string cleanBack_nonAlpha(string strSource)
        {
            while (strSource.Length > 0
                    &&
                 !char.IsLetter(strSource[strSource.Length - 1]))
            {
                strSource = strSource.Substring(0, strSource.Length - 1);
            }
            return strSource;
        }

        public classStringLibrary()
        {
            instance = this;
            if (chrNonAlphaCharacters == null)
            {
                chrNonAlphaCharacters = new char[strNonAlphaCharacters.Length];
                for (int intCounter = 0; intCounter < strNonAlphaCharacters.Length; intCounter++)
                {
                    chrNonAlphaCharacters[intCounter] = strNonAlphaCharacters[intCounter];
                }
            }
        }


        public static List<string> getQuoteFromText(string strText, string strWord)
        {
            string strTextCopy = Deaccent(strText.ToUpper());
            strWord = Deaccent(strWord.ToUpper());
            int intFind = strTextCopy.IndexOf(strWord);
            List<string> lstQuotes = new List<string>();
            while (intFind >= 0)
            {
                bool bolValid = true;
                if (intFind > 0)
                {
                    char chrPre = strTextCopy[intFind - 1];
                    bolValid = !isAlpha(chrPre);
                }
                if (bolValid && intFind < strTextCopy.Length - 1 - strWord.Length)
                {
                    char chrPost = strTextCopy[intFind + strWord.Length + 1];
                    if (isAlpha(chrPost))
                        bolValid = false;
                }

                if (bolValid)
                {
                    int intStart = intFind;
                    char chr = strTextCopy[intStart];
                    while (intFind > 0 && chr != '\n')
                        chr = strTextCopy[--intStart];
                    int intEnd = intFind + strWord.Length;
                    chr = strTextCopy[intEnd];
                    while (intFind < strTextCopy.Length && chr != '\r')
                        chr = strTextCopy[++intEnd];

                    string strQuote = strText.Substring(intStart, intEnd - intStart);
                    lstQuotes.Add(strQuote);
                }

                intFind = strTextCopy.IndexOf(strWord, intFind + 1);
            }

            return lstQuotes;
        }

        public static int StringCompare_NumCharMatch(string str1, string str2)
        {
            int intCharCounter = 1;
            int intMaxChar = str1.Length < str2.Length
                                         ? str1.Length
                                         : str2.Length;
            while (intCharCounter <= intMaxChar)
            {
                string strTest1 = str1.Substring(0, intCharCounter);
                string strTest2 = str2.Substring(0, intCharCounter);
                int intCompare = string.Compare(strTest1, strTest2);
                if (intCompare != 0)
                    return intCharCounter - 1;
                intCharCounter++;
            }
            return intCharCounter - 1;
        }


        public static string RemoveNonAlphaCharacters(string strText)
        {
            //string strFileName_RejectedCharacters = "SpellChecker_RejectedCharacters.txt";
            string strFileNameAndDirectory = "1234567890)(*&^%$#@!`~[]}{\';:\"|?><,./\\|";
            string strRejectedCharacterFromFile = "";
            if (!bolRejectedCharactersFileHasBeenRead)
            {
                if (System.IO.File.Exists(strFileNameAndDirectory))
                    try
                    {
                        strRejectedCharacterFromFile = System.IO.File.ReadAllText(strFileNameAndDirectory);
                    }
                    catch (Exception)
                    {
                    }
                strRejectedCharacters = strRejectedCharacterFromFile.Length > strNonAlphaCharacters.Length
                                                    ? strRejectedCharacterFromFile
                                                    : strNonAlphaCharacters;
                chrRejectedCharacters = new char[strRejectedCharacters.Length];
                for (int intCounter = 0; intCounter < strRejectedCharacters.Length; intCounter++)
                    chrRejectedCharacters[intCounter] = strRejectedCharacters[intCounter];


                bolRejectedCharactersFileHasBeenRead = true;
            }
            // swap all rejected characters for tilda '~' 

            foreach (char chr in strRejectedCharacters)
            {
                strText = strText.Replace(chr, '~');
            }

            while (strText.Contains('~'))
                strText = strText.Replace('~', ' ');
            while (strText.Contains("  "))
                strText = strText.Replace("  ", " ").Trim().ToUpper();


            return strText;
        }

        public static bool isAVowel(char chr) { return strVowels.Contains(chr); }
        public static bool isAlpha(char chr) { return char.IsLetter(chr); }
        public static string RemoveMacrons(string strWord)
        {
            if (strWord == null) return "";
            string strRetVal = strWord;
            for (int intCounter = 0; intCounter < strMacrons.Length; intCounter += 2)
            {
                strRetVal = strRetVal.Replace(strMacrons[intCounter], strMacrons[intCounter + 1]);
            }
            return strRetVal;
        }

        /// <summary>
        /// used in iEnumerable OrderBy queries for alphabetizing lists of strings
        /// </summary>
        public class Compare_Alphabetical : IComparer<string>
        {
            const string conChars = "abcdefghijklmnopqrstuvwxyz01233456789_";
            /// <summary>
            /// compare two strings
            /// </summary>
            public int Compare(string str1, string str2)
            {
                str1 = str1.Trim().ToLower();
                str2 = str2.Trim().ToLower();
                int intLength = str1.Length < str2.Length
                                            ? str1.Length
                                            : str2.Length;
                for (int intCounter = 0; intCounter < intLength; intCounter++)
                {
                    if (conChars.IndexOf(str1[intCounter]) < conChars.IndexOf(str2[intCounter]))
                        return -1;
                    else if (conChars.IndexOf(str1[intCounter]) > conChars.IndexOf(str2[intCounter]))
                        return 1;
                }
                if (str1.Length < str2.Length)
                    return -1;
                else if (str1.Length > str2.Length)
                    return 1;
                else return 0;
            }
        }


        /// <summary>
        /// used in iEnumerable OrderBy queries for ordering lists of by length of strings
        /// </summary>
        public class Compare_StringLength : IComparer<string>
        {
            /// <summary>
            /// compare two strings
            /// </summary>
            public int Compare(string str1, string str2)
            {
                int int1 = str1.Length;
                int int2 = str2.Length;

                if (int1 > int2) return 1;
                else if (int1 < int2) return -1;
                else return 0;
            }
        }

        public static string Alphabetize_Words(string strSource)
        {
            List<string> lstWords = classStringLibrary.getFirstWords(strSource);

            IEnumerable<string> query = lstWords.OrderBy(num => num, new Compare_Alphabetical());
            lstWords = query.ToList<string>();
            string strRetVal = "";
            foreach (string s in lstWords)
                strRetVal += s + " ";
            return strRetVal;
        }
        public static List<string> Alphabetize_Words(List<string> lstWords)
        {
            IEnumerable<string> query = lstWords.OrderBy(num => num, new Compare_Alphabetical());
            lstWords = query.ToList<string>();
            return lstWords;
        }

        public static string Alphabetize_LinesOfText(string strSource)
        {
            int intCut = strSource.IndexOf("\r\n");
            List<string> lstLinesOfText = new List<string>();
            while (intCut > 0 && strSource.Length > 0)
            {
                string strLine = classStringLibrary.clean_nonAlpha_Ends(strSource.Substring(0, intCut));
                lstLinesOfText.Add(strLine);
                strSource = classStringLibrary.clean_nonAlpha_Ends(strSource.Substring(intCut + 2));
                intCut = strSource.IndexOf("\r\n");
            }
            if (strSource.Length > 0)
                lstLinesOfText.Add(strSource);

            IEnumerable<string> query = lstLinesOfText.OrderBy(num => num, new Compare_Alphabetical());
            lstLinesOfText = query.ToList<string>();

            string strOutput = "";
            for (int intLinesCounter = 0; intLinesCounter < lstLinesOfText.Count; intLinesCounter++)
            {
                strOutput += (intLinesCounter > 0
                                              ? "\r\n"
                                              : "")
                             + lstLinesOfText[intLinesCounter];
            }
            return strOutput;

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

        public static string PolishText(string strText)
        {
            if (strText == null || strText.Length == 0) return null;
            if (isAlpha(strText[0]))
            {
                strText = char.ToUpper(strText[0]) + strText.Substring(1);
            }
            int intCut = strText.IndexOf('_');
            while (intCut >= 0)
            {
                if (intCut < strText.Length)
                {
                    char chrAfter = char.ToUpper(strText[intCut + 1]);
                    strText = strText.Substring(0, intCut) + " " + chrAfter + strText.Substring(intCut + 2);
                    intCut = strText.IndexOf('_', intCut + 1);
                }
                else intCut = -1;
            }
            return strText;

        }

        const string conGetFirstWordKeepCharIgnoreCode = "keep no char";
        public static List<string> getFirstWords(string strText)
        {
            List<string> lstRetVal = getFirstWords(strText, -1, conGetFirstWordKeepCharIgnoreCode);
            return lstRetVal;
        }


        public static List<string> getWords(string strText)
        {
            //strText = Deaccent(strText);
            List<string> lstRetVal = new List<string>();

            string strNewWord = "";
            foreach (char c in strText)
            {
                if (char.IsLetter(c))
                    strNewWord += c.ToString();
                else
                {
                    if (strNewWord.Length > 0)
                    {
                        lstRetVal.Add(strNewWord);
                        strNewWord = "";
                    }
                }
            }
            if (strNewWord.Length > 0)
                lstRetVal.Add(strNewWord);

            return lstRetVal;
        }

        public static List<string> getFirstWords(string strText, int intNumberWords) { return getFirstWords(strText, intNumberWords, conGetFirstWordKeepCharIgnoreCode); }
        public static List<string> getFirstWords(string strText, char chrKeep) { return getFirstWords(strText, -1, chrKeep.ToString()); }
        public static List<string> getFirstWords(string strText, int intNumberWords, char chrKeep) { return getFirstWords(strText, intNumberWords, chrKeep.ToString()); }
        private static List<string> getFirstWords(string strText, int intNumberWords, string strKeepChar)

        {
            if (strText == null || strText.Length == 0) return null;

            //strText = " " + strText + " ";
            int intNextIndex = 0;
            int intIndex = 0;
            List<string> lstRetVal = new List<string>();

            string str_Local_NonAlphaCharacters = "";
            if (string.Compare(conGetFirstWordKeepCharIgnoreCode, strKeepChar) == 0)
                str_Local_NonAlphaCharacters = strNonAlphaCharacters;
            else
            {
                char chrRemoveFromRejectedCharList = strKeepChar[0];
                int intRemoveIndex = strNonAlphaCharacters.IndexOf(chrRemoveFromRejectedCharList);
                if (intRemoveIndex >= 0 && intRemoveIndex < strNonAlphaCharacters.Length)
                    str_Local_NonAlphaCharacters = strNonAlphaCharacters.Remove(intRemoveIndex, 1);
            }
            char[] chr_Local_str_Local_NonAlphaCharacters = str_Local_NonAlphaCharacters.ToArray<char>();

            strText = clean_nonAlpha_Ends(strText) + ".";

            intNextIndex = strText.IndexOfAny(chr_Local_str_Local_NonAlphaCharacters, intIndex + 1);
            while (intNextIndex > intIndex
                    && (lstRetVal.Count < intNumberWords || intNumberWords < 0)
                    )
            {
                string strWord = strText.Substring(intIndex, intNextIndex - intIndex).Trim();
                lstRetVal.Add(strWord);
                while (intNextIndex < strText.Length && str_Local_NonAlphaCharacters.Contains(strText[intNextIndex]))
                    intNextIndex++;
                intIndex = intNextIndex;

                intNextIndex = strText.IndexOfAny(chr_Local_str_Local_NonAlphaCharacters, intIndex);
            }
            if (lstRetVal.Count == 0
                && strText.Length > 0)
                lstRetVal.Add(strText);

            return lstRetVal;
        }


        public static string getClickedLine(object sender, MouseEventArgs e)
        {
            try
            {
                TextBox txtBox = (TextBox)sender;
                System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
                return getClickedLine(ref txtBox, pt);
            }
            catch (Exception)
            { }

            try
            {
                RichTextBox rtxBox = (RichTextBox)sender;
                System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
                return getClickedLine(ref rtxBox, pt);
            }
            catch (Exception)
            { }

            return "";
        }

        public static string getClickedLine(ref TextBox txtbox, System.Drawing.Point ptMouse)
        {
            ptMouse.X = 0;
            string strRetVal = "";
            int intSelection = txtbox.GetCharIndexFromPosition(ptMouse);

            int intNextNewLine = txtbox.Text.IndexOf('\n', intSelection);

            if (intNextNewLine > intSelection
                && intSelection > 0
                && intNextNewLine < txtbox.Text.Length)
            {
                strRetVal = txtbox.Text.Substring(intSelection, intNextNewLine - intSelection);
            }

            return strRetVal;
        }

        public static string getClickedLine(ref RichTextBox rtxBox, System.Drawing.Point ptMouse)
        {
            ptMouse.X = 0;
            string strRetVal = "";
            int intSelection = rtxBox.GetCharIndexFromPosition(ptMouse);

            int intNextNewLine = rtxBox.Text.IndexOf('\n', intSelection);

            if (intNextNewLine > intSelection
                && intSelection > 0
                && intNextNewLine < rtxBox.Text.Length)
            {
                strRetVal = rtxBox.Text.Substring(intSelection, intNextNewLine - intSelection);
            }

            return strRetVal;
        }

        public static string getWordUnderMouse(ref TextBox txtbox, System.Drawing.Point pt)
        {
            int intSelected = txtbox.GetCharIndexFromPosition(pt);
            return getWordAtSelection(ref txtbox, intSelected);
        }
        public static string getWordUnderMouse(ref RichTextBox rtxBox, System.Drawing.Point pt)
        {
            if (rtxBox == null || rtxBox.IsDisposed) return "";
            int intSelected = rtxBox.GetCharIndexFromPosition(pt);
            return getWordAtSelection(ref rtxBox, intSelected);
        }

        public static string getClickedWord(object sender, MouseEventArgs e)
        {
            try
            {
                TextBox txtbox = (TextBox)sender;
                return getClickedWord(ref txtbox);
            }
            catch (Exception)
            { }

            try
            {
                RichTextBox rtx = (RichTextBox)sender;
                return getClickedWord(ref rtx);
            }
            catch (Exception)
            { }
            return "";
        }




        public static string getClickedWord(ref TextBox txtbox)
        {
            return getWordAtSelection(ref txtbox, txtbox.SelectionStart);
        }
        public static string getClickedWord(ref RichTextBox rtxBox)
        {
            return getWordAtSelection(ref rtxBox, rtxBox.SelectionStart);
        }
        public static string getWordAtSelection(ref RichTextBox rtxbox) { return getWordAtSelection(ref rtxbox, rtxbox.SelectionStart); }
        public static string getWordAtSelection(ref RichTextBox rtxbox, int intSelection)
        {
            string strRetVal = "";
            string strNonAlphaChar = "-";
            int intStartWord = intSelection - 1;
            while (intStartWord >= 0
                    && (char.IsLetter(rtxbox.Text[intStartWord])
                            ||
                        strNonAlphaChar.Contains(rtxbox.Text[intStartWord])))
                intStartWord--;

            int intEndWord = intSelection + rtxbox.SelectionLength;
            while (intEndWord < rtxbox.Text.Length && classStringLibrary.isAlpha(rtxbox.Text[intEndWord])) intEndWord++;
            while (intEndWord < rtxbox.Text.Length
                    && (char.IsLetter(rtxbox.Text[intEndWord])
                            ||
                        strNonAlphaChar.Contains(rtxbox.Text[intEndWord])))
                intEndWord++;

            if (intEndWord > intStartWord + 1
                && intEndWord <= rtxbox.Text.Length)
            {
                int intSubString_Start = intStartWord + 1;
                int intSubString_Length = intEndWord - intStartWord - 1;
                strRetVal = rtxbox.Text.Substring(intSubString_Start, intSubString_Length);
                //    strRetVal = rtxbox.Text.Substring(intStartWord + 1, intEndWord - intStartWord - 1);
            }

            return strRetVal;
        }


        public static string getWordAtSelection(ref TextBox txtbox) { return getWordAtSelection(ref txtbox, txtbox.SelectionStart); }
        public static string getWordAtSelection(ref TextBox txtbox, int intSelection)
        {

            string strRetVal = "";
            int intStartWord = intSelection - 1;
            while (intStartWord >= 0 && classStringLibrary.isAlpha(txtbox.Text[intStartWord])) intStartWord--;


            int intEndWord = intSelection;
            while (intEndWord < txtbox.Text.Length && classStringLibrary.isAlpha(txtbox.Text[intEndWord])) intEndWord++;

            if (intEndWord > intStartWord + 1
                && intEndWord <= txtbox.Text.Length)
            {
                strRetVal = txtbox.Text.Substring(intStartWord + 1, intEndWord - intStartWord - 1);
            }

            return strRetVal;
        }

        /// <summary>
        /// RAM alpha tree
        /// </summary>
        public class cAlphaTree
        {
            class classNode_AlphaTree
            {
                public classNode_AlphaTree[] branches = new classNode_AlphaTree[26];
                public object data;
            }
            public class classTraversalReport_Record
            {
                public object data;
                public string SearchKey;
            }

            List<classTraversalReport_Record> lstReport = new List<classTraversalReport_Record>();
            classNode_AlphaTree cRoot = new classNode_AlphaTree();
            public cAlphaTree() { }
            /// <summary>
            /// will set searchkey to lowerCase() and ignore all non-alphabetical characters during inserting
            /// </summary>
            /// <param name="data">any data formatted to be an object</param>
            /// <param name="SearchKey">a string of characters used to identify the object being stored in the alphatree.  upper/lower case are set to lower case and all non-alphabetical characters are ignored</param>
            /// <returns></returns>
            public bool Insert(ref object data, string SearchKey)
            {
                SearchKey = SearchKey.ToLower();

                Insert(ref data, SearchKey, ref cRoot);
                return true;
            }
            void Insert(ref object data, string SearchKey, ref classNode_AlphaTree cNode)
            {
                if (SearchKey.Length > 0)
                {
                    char chr;
                    int index;
                    do // ignore none alpha characters
                    {
                        chr = SearchKey[0];
                        index = (int)(chr - 'a');
                        SearchKey = SearchKey.Substring(1);
                    } while ((index >= 26 || index < 0) && SearchKey.Length > 0);
                    if (index >= 0 && index < 26)
                    {
                        if (cNode.branches[index] == null)
                        {
                            classNode_AlphaTree cNode_Branch = new classNode_AlphaTree();
                            cNode.branches[index] = cNode_Branch;
                        }
                        Insert(ref data, SearchKey, ref cNode.branches[index]);
                    }
                }
                else
                {
                    cNode.data = data;
                }
            }
            /// <summary>
            /// returns a data object when string of characters in SearchKey match an insertion SearchKey.  Upper/Lower case are set to LowerCase().  All non-alpha characters are ignored
            /// </summary>
            /// <param name="SearchKey">string of characters used to identify the Data Object previously stored in the Alpha-tree</param>
            /// <returns></returns>
            public object Search(string SearchKey) { return Search(SearchKey.ToLower(), ref cRoot); }
            object Search(string SearchKey, ref classNode_AlphaTree cNode)
            {
                if (SearchKey.Length == 0) return cNode.data;
                char chr;
                int index;
                do
                {
                    chr = SearchKey[0];
                    index = (int)(chr - 'a');
                    SearchKey = SearchKey.Substring(1);
                } while ((index < 0 || index >= 26) && SearchKey.Length > 0);

                if (cNode.branches[index] != null)
                    return Search(SearchKey, ref cNode.branches[index]);
                else
                    return null;
            }

            public List<classTraversalReport_Record> TraverseTree_InOrder()
            {
                lstReport.Clear();

                TraverseTree_InOrder(ref cRoot, "");
                return lstReport;
            }

            void TraverseTree_InOrder(ref classNode_AlphaTree cNode, string SearchKey)
            {
                if (cNode.data != null)
                {
                    classTraversalReport_Record cReport = new classTraversalReport_Record();
                    cReport.data = cNode.data;
                    cReport.SearchKey = SearchKey;
                    lstReport.Add(cReport);
                }

                for (int intCounter = 0; intCounter < cNode.branches.Length; intCounter++)
                {
                    classTraversalReport_Record cChild = new classTraversalReport_Record();
                    if (cNode.branches[intCounter] != null)
                    {
                        string strChildSearchKey = SearchKey + (char)('a' + intCounter);
                        TraverseTree_InOrder(ref cNode.branches[intCounter], strChildSearchKey);
                    }
                }
            }
        }

        public static void HighLightText(ref RichTextBox _rtx, string _strSourceText, List<string> _lstSearchWords)
        {
            List<classHighlightFinds> lstTemp = new List<classHighlightFinds>();
            HighLightText(ref _rtx, _strSourceText, _lstSearchWords, ref lstTemp);
        }

        public static void HighLightText(ref RichTextBox _rtx, string strSourceText, List<string> lstSearchWords, ref List<classHighlightFinds> lstSearchFinds)
        {

            //semHighlight.WaitOne();

            classColorTag cClrTag = null;
            try
            {
                cClrTag = (classColorTag)_rtx.Tag;
            }
            catch (Exception)
            {
                cClrTag = new classColorTag();
                throw;
            }

            string strText = classStringLibrary.Deaccent(strSourceText.ToUpper());
            int intFind = 0;
            lstSearchFinds.Clear();

            if (lstSearchWords.Count > 0)
            {
                for (int intWordCounter = 0; intWordCounter < lstSearchWords.Count; intWordCounter++)
                {
                    string strSearchWord = classStringLibrary.Deaccent(lstSearchWords[intWordCounter].ToUpper());
                    intFind = strText.IndexOf(strSearchWord);

                    while (intFind >= 0)
                    {
                        if (intFind > 0)
                        {
                            string strView = strText.Substring(intFind - 1);
                        }
                        bool bolValid = true;
                        if (intFind > 0)
                        {
                            char chrBefore = strText[intFind - 1];
                            if (classStringLibrary.isAlpha(chrBefore))
                                bolValid = false;
                        }
                        if (bolValid && intFind < strText.Length)
                        {
                            char chrAfter = strText[intFind + strSearchWord.Length];
                            if (classStringLibrary.isAlpha(chrAfter))
                                bolValid = false;
                        }

                        if (bolValid)
                            lstSearchFinds.Add(new classHighlightFinds(intFind, strSourceText.Substring(intFind, strSearchWord.Length)));

                        intFind = strText.IndexOf(strSearchWord, intFind + 1);
                    }
                }

                IEnumerable<classHighlightFinds> query = lstSearchFinds.OrderBy(find => find.intSource_SelectionStart);
                lstSearchFinds = (List<classHighlightFinds>)query.ToList<classHighlightFinds>();

                classHighlightFinds cFind_Last = null;
                for (int intCounter = 0; intCounter < lstSearchFinds.Count; intCounter++)
                {
                    classHighlightFinds cFind = lstSearchFinds[intCounter];
                    if (cFind_Last != null)
                    {
                        int intStart = cFind_Last.intSource_SelectionStart + cFind_Last.strWord.Length;
                        string strTextToAdd = strSourceText.Substring(intStart, cFind.intSource_SelectionStart - intStart);
                        _rtx.Text += strTextToAdd;
                    }
                    else
                    {
                        string strTextToAdd = strSourceText.Substring(0, cFind.intSource_SelectionStart);
                        _rtx.Text = strTextToAdd;
                    }

                    cFind.intTarget_SelectionStart = _rtx.Text.Length;
                    _rtx.Text += cFind.strWord;

                    if (intCounter == lstSearchFinds.Count - 1)
                        _rtx.Text += strSourceText.Substring(cFind.intSource_SelectionStart + cFind.strWord.Length);


                    cFind_Last = cFind;
                }

                do
                { _rtx.BackColor = classRND.Get_Color(); } while (_rtx.BackColor == cClrTag.clrBack);
                do
                { _rtx.ForeColor = classRND.Get_Color(); } while (_rtx.ForeColor == cClrTag.clrFore);
                _rtx.Text = strSourceText;
                _rtx.BackColor = cClrTag.clrBack;
                _rtx.ForeColor = cClrTag.clrFore;
                for (int intWordCounter = 0; intWordCounter < lstSearchFinds.Count; intWordCounter++)
                {
                    classHighlightFinds cFind = lstSearchFinds[intWordCounter];
                    _rtx.SelectionStart = cFind.intTarget_SelectionStart;
                    _rtx.SelectionLength = cFind.strWord.Length;
                    _rtx.SelectionBackColor = cClrTag.clrHighlight_Back;
                    _rtx.SelectionColor = cClrTag.clrHighlight_Fore;
                }

                if (lstSearchFinds.Count > 0)
                {
                    _rtx.SelectionStart = lstSearchFinds[0].intTarget_SelectionStart;
                    _rtx.ScrollToCaret();
                }
            }
        }


        public static string HTML_GetNext(string strHTML, string str_Start, string str_End) { return HTML_GetNext(strHTML, str_Start, str_End, 0); }
        public static string HTML_GetNext(string strHTML, string str_Start, string str_End, int intStart)
        {
            if (intStart < 0) return "";
            int intNum_Open = 0;
            int intCut_Start = strHTML.IndexOf(str_Start, intStart, StringComparison.Ordinal), intCut_Close = 0;
            if (intCut_Start > 0)
                strHTML = strHTML.Substring(intCut_Start);
            else if (intCut_Start < 0)
                return "";

            // strHTML starts with a div
            intNum_Open++;
            int intIndex = 0;
            do
            {
                intCut_Start = strHTML.IndexOf(str_Start, intIndex + 1, StringComparison.Ordinal);
                intCut_Close = strHTML.IndexOf(str_End, intIndex + 1, StringComparison.Ordinal);
                if (intCut_Start >= 0 && intCut_Close >= 0)
                {
                    if (intCut_Start < intCut_Close)
                    {
                        intNum_Open++;
                        intIndex = intCut_Start;
                    }
                    else
                    {
                        intNum_Open--;
                        intIndex = intCut_Close;
                    }
                }
                else if (intCut_Close >= 0)
                {
                    intNum_Open--;
                    intIndex = intCut_Close;

                }
                else
                    intNum_Open = -1;
            } while (intNum_Open > 0);

            return strHTML.Substring(0, intCut_Close + str_End.Length);
        }

        public class classHighlightFinds
        {
            public int intTarget_SelectionStart;
            public int intSource_SelectionStart;
            public string strWord;
            public classHighlightFinds(int selectionStart_Source, string word)
            {
                intSource_SelectionStart = selectionStart_Source;
                strWord = word;
            }
        }

        public class classColorTag
        {
            public Color clrFore = Color.Black;
            public Color clrBack = Color.White;
            public Color clrHighlight_Back = Color.White;
            public Color clrHighlight_Fore = Color.Red;
        }
    }
}
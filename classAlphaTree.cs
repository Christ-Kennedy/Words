using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AlphaTrees
{

    /// <summary>
    /// RAM alpha tree stores/retrieves one data object per alphabetical strings as searchKey
    /// </summary>
    public class cAlphaTree
    {
        /// <summary>
        /// each node has 26 branches to other nodes and one object data element
        /// and is used to search the tree by going down the branch appropriate for the letter of the word being searched
        /// </summary>
        class classNode_AlphaTree
        {
            public classNode_AlphaTree[] branches = new classNode_AlphaTree[26];
            public object data;
        }

        /// <summary>
        /// record returned in a tree search and List element of a tree traversal
        /// </summary>
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
            SearchKey = SearchKey.Trim().ToLower();

            Insert(ref data, SearchKey, ref cRoot);
            return true;
        }

        /// <summary>
        /// replaces data content of node found by searchKey scan with provided data 
        /// or creates a new node entry to hold data if none is found
        /// </summary>
        /// <param name="data">data to be Leaveed into tree</param>
        /// <param name="SearchKey">searchKey used to store and later retrieved the data object</param>
        /// <param name="cNode">recursing function uses this node to determine which node to recurse into</param>
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
        public object Search(string SearchKey) { return Search(SearchKey.Trim().ToLower(), ref cRoot); }

        /// <summary>
        /// searches the tree using the provided searchKey and returns the data object if a result is found
        /// </summary>
        /// <param name="SearchKey">key used to scan the tree</param>
        /// <param name="cNode">recursing funcction uses the provided node to determine which node to Leave in next recursion</param>
        /// <returns>returns the data object bound to the tree element found at the end of the searchKey scan if any is found</returns>
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


            if (index >= 0 && index < 26 && cNode.branches[index] != null)
                return Search(SearchKey, ref cNode.branches[index]);
            else
                return null;
        }

        /// <summary>
        /// calls recursing function that traverses through the entire alphatree while storing data found in a global variable that this function returns to the user
        /// </summary>
        /// <returns>a list of all data found in tree</returns>
        public List<classTraversalReport_Record> TraverseTree_InOrder()
        {
            lstReport.Clear();

            TraverseTree_InOrder(ref cRoot, "");
            return lstReport;
        }

        /// <summary>
        /// recursing function that sequentially calls itself for every sub node of the current node and reports any data found to a global variable
        /// </summary>
        /// <param name="cNode">current node being scanned</param>
        /// <param name="SearchKey">accumulates characters as the scan progresses down each node and uses the state of this string when building a classTraversalReport_Record</param>
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


    /////////////////////////////////////////////////////////////
    /// <summary>
    /// RAM alphanum tree stores/retrieves one data object per alphabetical strings as searchKey
    /// and includes various characters the basic AlphaTree does not
    /// </summary>
    public class classAlphaNumTree
    {
        /// <summary>
        /// constant string that defines the characters which the AlphaNum tree recognizes 
        /// </summary>
        public static string conCharacterList = "0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`";

        /// <summary>
        /// size of node branches array is defined by the size of the conCharacterList string
        /// </summary>
        static int intMaxIndex = ((int)(conCharacterList[conCharacterList.Length - 1]) - (int)'0');

        /// <summary>
        /// each branch in the alphaNum tree has as many branches as there are allowable characters in the conCharacterList string
        /// </summary>
        class classNode_AlphaNumTree
        {
            public classNode_AlphaNumTree[] branches = new classNode_AlphaNumTree[intMaxIndex];
            public object data;
        }

        /// <summary>
        /// data record used for tree searches and tree traversals
        /// </summary>
        public class classTraversalReport_Record
        {
            public object data;
            public string SearchKey;
        }

        List<classTraversalReport_Record> lstReport = new List<classTraversalReport_Record>();
        classNode_AlphaNumTree cRoot = new classNode_AlphaNumTree();
        public classAlphaNumTree() { }
        /// <summary>
        /// will set searchkey to upperCase() only accepting "0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`" characters
        /// </summary>
        /// <param name="data">any data formatted to be an object</param>
        /// <param name="SearchKey">a string of characters used to identify the object being stored in the AlphaNumTree.  upper/lower case are set to upper case</param>
        /// <returns></returns>
        public bool Insert(ref object data, string SearchKey)
        {
            SearchKey = SearchKey.ToUpper();

            Insert(ref data, SearchKey, ref cRoot);
            return true;
        }

        /// <summary>
        /// recursing function scans through the searchKey's appropriate nodes until it reaches the end of the searchKey and then stores the specified data at that entry
        /// </summary>
        /// <param name="data">data object to be stored in</param>
        /// <param name="SearchKey">used to store and later retrieve the provided data object</param>
        /// <param name="cNode">node this recursing function is currently scanning</param>
        void Insert(ref object data, string SearchKey, ref classNode_AlphaNumTree cNode)
        {
            if (SearchKey.Length > 0)
            {
                char chr;
                int index;
                do // ignore none alpha characters
                {
                    chr = SearchKey[0];
                    index = (int)(chr - '0');
                    SearchKey = SearchKey.Substring(1);
                } while ((index >= intMaxIndex || index < 0) && SearchKey.Length > 0);
                if (index >= 0 && index < intMaxIndex)
                {
                    if (cNode.branches[index] == null)
                    {
                        classNode_AlphaNumTree cNode_Branch = new classNode_AlphaNumTree();
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
        /// returns a data object when string of characters in SearchKey match an insertion SearchKey.  Upper/Lower case are set to LowerCase().  
        /// </summary>
        /// <param name="SearchKey">string of characters used to identify the Data Object previously stored in the Alpha-tree</param>
        /// <returns></returns>
        public object Search(string SearchKey) { return Search(SearchKey.ToUpper(), ref cRoot); }

        /// <summary>
        /// recursing function called by the global search function to recurse through tree and return data found at the end of searchKey scan
        /// </summary>
        /// <param name="SearchKey">searchKey used to find the expected data</param>
        /// <param name="cNode">node this recursing function is currently scanning</param>
        /// <returns></returns>
        object Search(string SearchKey, ref classNode_AlphaNumTree cNode)
        {
            if (SearchKey.Length == 0) return cNode.data;
            char chr;
            int index;
            do
            {
                chr = SearchKey[0];
                index = (int)(chr - '0');
                SearchKey = SearchKey.Substring(1);
            } while ((index < 0 || index >= intMaxIndex) && SearchKey.Length > 0);

            if (cNode.branches[index] != null)
                return Search(SearchKey, ref cNode.branches[index]);
            else
                return null;
        }

        /// <summary>
        /// calls recursing function that scans the tree and returns a global variable containing all data found in the tree
        /// </summary>
        /// <returns>returns a complete list of tree's data content</returns>
        public List<classTraversalReport_Record> TraverseTree_InOrder()
        {
            lstReport.Clear();

            TraverseTree_InOrder(ref cRoot, "");
            return lstReport;
        }

        /// <summary>
        /// recursing function that the tree using the provided searchKey and stores any data found into a global variable to be returned to the calling function at the end of the tree Traversal
        /// </summary>
        /// <param name="cNode">node this recursing function is currently scanning</param>
        /// <param name="SearchKey">searchKey used to find the expected data</param>
        void TraverseTree_InOrder(ref classNode_AlphaNumTree cNode, string SearchKey)
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
                    string strChildSearchKey = SearchKey + (char)('0' + intCounter);
                    TraverseTree_InOrder(ref cNode.branches[intCounter], strChildSearchKey);
                }
            }
        }
    }


}

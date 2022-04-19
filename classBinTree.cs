using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinTree
{
    public class classBinTree
    {
        classBinTreeNode cRoot = null;
        
        public classBinTree() { }
        public void Clear() { cRoot = null; }
        public void Insert(ref object data, string key)
        {

            if (cRoot == null)
            {
                classBinTreeNode cNodeNew = new classBinTreeNode(ref data, key);
                cRoot = cNodeNew;
                return;
            }

            classBinTreeNode cNode = cRoot;

            do
            {
                int intComparison = string.Compare(key, cNode.key);
                if (intComparison < 0)
                {
                    if (cNode.Left == null)
                    {
                        cNode.Left = new classBinTreeNode(ref data, key);
                        return;
                    }
                    cNode = cNode.Left;
                }
                else if (intComparison > 0)
                {
                    if (cNode.Right == null)
                    {
                        cNode.Right = new classBinTreeNode(ref data, key);
                        return;
                    }
                    cNode = cNode.Right;
                }
                else
                {
                    cNode.data = data;
                    return;
                }
            }
            while (true);
        }


        public object Search(string key)
        {
            if (cRoot == null) return null;
            classBinTreeNode cNode = cRoot;
            do
            {
                int intComparison = string.Compare(key, cNode.key);
                if (intComparison < 0)
                    cNode = cNode.Left;
                else if (intComparison > 0)
                    cNode = cNode.Right;
                else
                    return cNode.data;            
            } while (cNode != null);
            return null;
        }

        List<object> lstTraversalResults = new List<object>();
        public List<object> Traverse()
        {
            lstTraversalResults.Clear();
            Traverse_Iteration(ref cRoot);
            return lstTraversalResults;
        }

        void Traverse_Iteration(ref classBinTreeNode cNode)
        {
            if (cNode == null) return;

            Traverse_Iteration(ref cNode.Left);
            lstTraversalResults.Add(cNode.data);
            Traverse_Iteration(ref cNode.Right);
        }


    }

    public class classBinTreeNode
    {
       public string key = "";
        public classBinTreeNode Left;
        public classBinTreeNode Right;
        public object data = null;
        public classBinTreeNode(ref object data, string key)
        {
            this.data = data;
            this.key = key;
        }
    }

    public class classTraversalReport_Record
    {
        public object data;
        public string SearchKey;
    }
}

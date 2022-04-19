using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using AlphaTrees;
using BinTree;

namespace  Dlús 
{
    /// <summary>
    /// classUILanguage enables the use of multiple tips for all uniquely named controls in a project
    /// the controls' names are used as searchKeys in an AlphaTree with a data object containing the list of tips for each control
    /// </summary>
    public class classUILanguage
    {
        //classAlphaNumTree cBT_Objects = new classAlphaNumTree();
        classBinTree cBT_Objects = new classBinTree();
        public classUILanguage()
        {
  
        }

        /// <summary>
        /// finds the list of toolTips bound to a given control
        /// </summary>
        /// <param name="strComponentName">name of component for which a list of tooltips is requested</param>
        /// <returns>classUILanguage_Element containing list of tooltips for requested control</returns>
        public string Tip_get(string strComponentName)
        {
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                return cTT_Ele.Tip;
            else return "";
        }

        /// <summary>
        /// searches tree for existing tooltip for specified control, adds the specified tip to existing tooltips list for that controller
        /// or creates a new classUILanguage_Element for the control specified if not found in the alphatree and enters it for later retrieval
        /// </summary>
        /// <param name="strComponentName">name of controller whose tip is to be added to tree</param>
        /// <param name="strTip">tip for the control specified </param>
        public void Tip_set(string strComponentName, string strTip)
        {
            if (strComponentName.Length == 0) MessageBox.Show("Component name is blank");
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                cTT_Ele.Tip = strTip;
            else
            {
                cTT_Ele = new classUILanguage_Element();
                cTT_Ele.strName = strComponentName;
                cTT_Ele.Tip = strTip;
                object objTT_Ele = (object)cTT_Ele;
                cBT_Objects.Insert(ref objTT_Ele, strComponentName);
            }
        }

        /// <summary>
        /// finds the list of toolTexts bound to a given control
        /// </summary>
        /// <param name="strComponentName">name of component for which a list of toolTexts is requested</param>
        /// <returns>classUILanguage_Element containing list of toolTexts for requested control</returns>
        public string Text_get(string strComponentName)
        {
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                return cTT_Ele.Text;
            else return "";
        }

        public bool Text_Override(string strComponentName)
        {
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                return cTT_Ele.TextOverride;
            else 
                return false;
        }

        /// <summary>
        /// finds the list of toolTexts bound to a given control
        /// </summary>
        /// <param name="strComponentName">name of component for which a list of toolTexts is requested</param>
        /// <returns>classUILanguage_Element containing list of toolTexts for requested control</returns>
        public void Text_Set(string strComponentName, string strNewText) { Text_Set(strComponentName, strNewText, false); }
        public void Text_Set(string strComponentName, string strNewText, bool bolOverride)
        {
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
            {
                cTT_Ele.Text = strNewText;
                cTT_Ele.TextOverride = bolOverride;
            }
            else
            {
                cTT_Ele = new classUILanguage_Element();
                cTT_Ele.strName = strComponentName;
                cTT_Ele.Text = strNewText;
                cTT_Ele.TextOverride = bolOverride;
                object objTT_Ele = (object)cTT_Ele;
                cBT_Objects.Insert(ref objTT_Ele, strComponentName);
            }
        }

        /// <summary>
        /// erases all previous tips and creates a new empty tree
        /// </summary>
        public void Tip_Clear_All()
        {
            cBT_Objects = new classBinTree();
        }

        /// <summary>
        /// erases list of tips for the specified control
        /// </summary>
        /// <param name="strComponentName">name of control whose tips are to be reset</param>
        public void Tip_clear(string strComponentName)
        {
            classUILanguage_Element cTT_Ele = (classUILanguage_Element)cBT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                cTT_Ele.Clear();
        }

        /// <summary>
        /// class used as a data object in the alphaTree
        /// </summary>
        public class classUILanguage_Element
        {
            /// <summary>
            /// name used as searchKey to store/Retrieve this data object
            /// </summary>
            public string strName;

            string strText = "";
            public string Text
            {
                get { return strText; }
                set { strText = value; }
            }

            bool bolTextOverride = false;
            public bool TextOverride
            {
                get { return bolTextOverride; }
                set { bolTextOverride = value; }
            }

            /// <summary>
            /// index of tooltip to be returned on the next request
            /// </summary>
            int intTipIndex = 0;

            List<string> lstTips = new List<string>();
            public string Tip
            {
                get
                {
                    if (lstTips.Count == 0) return "";
                    string strRetVal = lstTips[intTipIndex];
                    intTipIndex = (intTipIndex + 1) % lstTips.Count;
                    return strRetVal;
                }
                set
                {
                    if (!lstTips.Contains(value))
                        lstTips.Add(value);
                }
            }

            public void Clear() { lstTips.Clear(); }
        }

    }

}

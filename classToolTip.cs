using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlphaTrees;

namespace  Words 
{
    /// <summary>
    /// classToolTip enables the use of multiple tips for all uniquely named controls in a project
    /// the controls' names are used as searchKeys in an AlphaTree with a data object containing the list of tips for each control
    /// </summary>
    public class classToolTip
    {
        classAlphaNumTree cAT_Objects = new classAlphaNumTree();
        public classToolTip()
        {
  
        }

        /// <summary>
        /// finds the list of toolTips bound to a given control
        /// </summary>
        /// <param name="strComponentName">name of component for which a list of tooltips is requested</param>
        /// <returns>classToolTip_Element containing list of tooltips for requested control</returns>
        public string Tip_get(string strComponentName)
        {
            classToolTip_Element cTT_Ele = (classToolTip_Element)cAT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                return cTT_Ele.Tip;
            else return "";
        }

        /// <summary>
        /// searches tree for existing tooltip for specified control, adds the specified tip to existing tooltips list for that controller
        /// or creates a new classToolTip_Element for the control specified if not found in the alphatree and enters it for later retrieval
        /// </summary>
        /// <param name="strComponentName">name of controller whose tip is to be added to tree</param>
        /// <param name="strTip">tip for the control specified </param>
        public void  Tip_set(string strComponentName, string strTip)
        {
            if (strComponentName.Length == 0) MessageBox.Show("Component name is blank");
            classToolTip_Element cTT_Ele = (classToolTip_Element)cAT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                cTT_Ele.Tip = strTip;
            else
            {
                cTT_Ele = new classToolTip_Element();
                cTT_Ele.strName = strComponentName;
                cTT_Ele.Tip = strTip;
                object objTT_Ele = (object)cTT_Ele;
                cAT_Objects.Insert(ref objTT_Ele, strComponentName);
            }
        }

        /// <summary>
        /// erases all previous tips and creates a new empty tree
        /// </summary>
        public void Tip_Clear_All()
        {
            cAT_Objects = new classAlphaNumTree();
        }

        /// <summary>
        /// erases list of tips for the specified control
        /// </summary>
        /// <param name="strComponentName">name of control whose tips are to be reset</param>
        public void Tip_clear(string strComponentName)
        {
            classToolTip_Element cTT_Ele = (classToolTip_Element)cAT_Objects.Search(strComponentName);

            if (cTT_Ele != null)
                cTT_Ele.Clear();
        }

        /// <summary>
        /// class used as a data object in the alphaTree
        /// </summary>
        public class classToolTip_Element
        {
            /// <summary>
            /// name used as searchKey to store/Retrieve this data object
            /// </summary>
            public string strName;

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

using StringLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Words
{

    class classFileCreateInfo
    {
        public string Path = "";
        public DateTime dt = new DateTime();
        public classFileCreateInfo(string Path, DateTime dt)
        {
            this.Path = Path;
            this.dt = dt;
        }
    }

    public class classBackUp
    {
        const string strBackupFileName_Base = "Backup_";

        static public string strBackupFileName
        {
            get
            {
                return (cProject != null && cProject.Title_Current != null
                                 ? cProject.Name 
                                 : "PROJECT")
                        + "_" + strBackupFileName_Base;
            }
        }

        static string strBackupDirectory
        {
            get
            {
                string strFilename = System.IO.Path.GetFileName(formWords.instance.PathAndFilename);
                if (strFilename.Length > 0)
                {
                    int intLastIndex = formWords.instance.PathAndFilename.LastIndexOf(strFilename);
                    string strRetVal = formWords.instance.PathAndFilename.Substring(0, intLastIndex);
                    return strRetVal;
                }
                return "";
            }
        }


        static bool bolNeedsRefresh = false;
        public static int[] arrWidths = null;

        static classNotesInfo _NoteInfo = null;
        static public classNotesInfo NoteInfo
        {
            get
            {
                if (_NoteInfo == null)
                {
                    _NoteInfo = new classNotesInfo("Back up Files");
                    _NoteInfo.bolBackUpFileNote = true;
                    classNotesInfo.cRoot.ChildNodes_Add(ref _NoteInfo);
                    classNotesInfo.cRoot.lstChildNotes.Remove(_NoteInfo);
                    classNotesInfo.cRoot.lstChildNotes.Add(_NoteInfo);

                    if (arrWidths != null)
                        _NoteInfo.TabWidths = arrWidths;
                    bolNeedsRefresh = true;
                }

                if (bolNeedsRefresh)
                {
                    while (_NoteInfo.lstChildNotes.Count > 0)
                    {
                        classNotesInfo cNoteDelete = _NoteInfo.lstChildNotes[0];
                        cNoteDelete.die();
                    }

                    string strProjectFilename = formWords.instance.PathAndFilename;
                    string strExtension = System.IO.Path.GetExtension(strProjectFilename);
                    string[] strFiles = System.IO.Directory.GetFiles(strBackupDirectory, strBackupFileName + "*.rtf");
                    List<classFileCreateInfo> lstFiles = new List<classFileCreateInfo>();
                    for (int intFileCounter = 0; intFileCounter < strFiles.Length; intFileCounter++)
                    {
                        DateTime dt = System.IO.File.GetLastWriteTime(strFiles[intFileCounter]);
                        classFileCreateInfo cFileInfo = new classFileCreateInfo(strFiles[intFileCounter], dt);
                        lstFiles.Add(cFileInfo);
                    }

                    IEnumerable<classFileCreateInfo> query = lstFiles.OrderByDescending (fileInfo => fileInfo.dt.Ticks);
                    lstFiles = (List<classFileCreateInfo>)query.ToList<classFileCreateInfo>();

                    for (int intFileCounter = 0; intFileCounter < lstFiles.Count && intFileCounter < formWords.grbOptions.Integer_Backup_Files_Total_Number; intFileCounter++)
                    {
                        classFileCreateInfo cFile = lstFiles[intFileCounter];
                        string strFileAndPath = cFile.Path;
                        strExtension = System.IO.Path.GetExtension(strFileAndPath);

                        strFileAndPath = strFileAndPath.Length > strExtension.Length
                                                               ? strFileAndPath.Substring(0, strFileAndPath.Length - strExtension.Length)
                                                               : strFileAndPath;

                        string strTitle = System.IO.Path.GetFileName(strFileAndPath);

                        string strFilename = strFileAndPath.Length > strExtension.Length
                                                                    ? strFileAndPath.Substring(0, strFileAndPath.Length)
                                                                    : strFileAndPath;

                        classNotesInfo cNotesInfo_New = new classNotesInfo(strTitle);
                        cNotesInfo_New.File_Toggle = true;
                        cNotesInfo_New.Filename = strFileAndPath;
                        cNotesInfo_New.Day = cFile.dt.Day;
                        cNotesInfo_New.Month = cFile.dt.Month;
                        cNotesInfo_New.Year = cFile.dt.Year;
                        cNotesInfo_New.Hour = cFile.dt.Hour;
                        cNotesInfo_New.Minute = cFile.dt.Minute;
                        cNotesInfo_New.Chapter = intFileCounter;
                        cNotesInfo_New.bolBackUpFileNote = true;
                        _NoteInfo.ChildNodes_Add(ref cNotesInfo_New);
                    }

                    bolNeedsRefresh = false;
                }

                return _NoteInfo;
            }
        }

        Timer tmrBackup = new Timer();
        public static classProject cProject
        {
            get {return formWords.cProject; }
            set { formWords.cProject = value; }
        }

        public classBackUp()
        {
            tmrBackup.Interval = grbOptions.Integer_Backup_Time_Periods * 60 * 1000; 
            tmrBackup.Tick += new EventHandler(tmrBackup_Tick);
            tmrBackup.Enabled = true;
        }

        public void Timer_Delay_Set()
        {
            tmrBackup.Interval = grbOptions.Integer_Backup_Time_Periods * 60 * 1000;
            tmrBackup.Enabled = false;
            tmrBackup.Enabled = true;
        }

        groupboxOptions grbOptions
        {
            get { return formWords.grbOptions; }
        }


        public void tmrBackup_Tick(object sender, EventArgs e)
        {
            if (cProject.cEdit_Main != panelNotesDataTree.cRoot)
            {
                classNotesInfo cNote_User = formWords.ckRTX == groupboxNotes.instance.rtxNotes
                                                            ? cProject.cEdit_Alt
                                                            : cProject.cEdit_Main;

                if (cNote_User == null || !cNote_User.bolBackup_ChangesMade)
                    return;

                cNote_User.bolBackup_ChangesMade = false;

                bolNeedsRefresh = true;
                if (formWords.grbOptions.Toggle_Auto_Save)
                {
                    formWords.instance.mnuFile_Save_Click(sender, e);
                    formWords.Message("auto save : " + cProject.Title_Current);
                }

                string[] strFiles = System.IO.Directory.GetFiles(strBackupDirectory, strBackupFileName + "*.rtf");
                int intOverwrightNumber = strFiles.Length;
                int intMax = formWords.grbOptions.Integer_Backup_Files_Total_Number;
                if (intOverwrightNumber >= intMax)
                {
                    List<string> lstFiles = StringLibrary.classStringLibrary.Alphabetize_Words(strFiles.ToList<string>());

                    List<classFileCreateInfo> lstFileInfo = new List<classFileCreateInfo>();
                    for (int intFileCounter = 0; intFileCounter < lstFiles.Count; intFileCounter++)
                        lstFileInfo.Add(new classFileCreateInfo(lstFiles[intFileCounter], System.IO.File.GetLastWriteTime(lstFiles[intFileCounter])));

                    IEnumerable<classFileCreateInfo> query = lstFileInfo.OrderBy(fileInfo => fileInfo.dt);
                    lstFileInfo = (List<classFileCreateInfo>)query.ToList<classFileCreateInfo>();

                    intOverwrightNumber = lstFiles.IndexOf(lstFileInfo[0].Path);
                }

                string strBackupOverwriteFilename = strBackupDirectory + strBackupFileName + intOverwrightNumber.ToString("00") + ".rtf";

                using (RichTextBox rtxTemp = new RichTextBox())
                {
                    rtxTemp.Rtf = formWords.ckRTX.rtx.Rtf;

                    rtxTemp.Select(0, 0);
                    DateTime dtNow = DateTime.Now;

                    string strTime_SOURCE = dtNow.TimeOfDay.ToString();
                    char[] chrSplit = { ':', '.' };
                    string[] strTime_SPLIT = strTime_SOURCE.Split(chrSplit);
                    string strTime = strTime_SPLIT[0] + ":" + strTime_SPLIT[1] + ":" + strTime_SPLIT[2];


                    string strBackup = "Backup : ";
                    string strTextInsert = strBackup + cNote_User.Title +"\r\n"+ dtNow.Year.ToString("") + "/" + dtNow.Month.ToString("00") + "/" + dtNow.Day.ToString("00") + "\t" + strTime + "\r\n";
                    rtxTemp.SelectedText = strTextInsert;
                    rtxTemp.Select(0, strTextInsert.Length-2);
                    rtxTemp.SelectionAlignment = HorizontalAlignment.Left;
                    rtxTemp.SelectionIndent = 0;
                    rtxTemp.SelectionFont = new Font("courier", 10);
                    rtxTemp.SelectionBackColor = Color.LightGray;
                    rtxTemp.SelectionColor = Color.Black;

                    rtxTemp.Select(strBackup.Length, cNote_User.Title.Length);
                    rtxTemp.SelectionFont = new Font("Arial", 12, FontStyle.Bold | FontStyle.Underline);

                    rtxTemp.SaveFile(strBackupOverwriteFilename);
                }
                
                formWords.instance.bolTextChangedAndNotSaved = false;

                bolNeedsRefresh = true;
                if (panelNotesDataTree.cParent == NoteInfo)
                {
                    panelNotesDataTree.instance.Buttons_Rebuild();
                    NoteInfo.eOrderBy = enuNotesButtons.ChapterNumber;
                    panelNotesDataTree.instance.Reorder();
                    if (formWords.RTX_Focused != groupboxNotes.instance.rtxNotes.rtx) // user is NOT working in the alt editor
                        if (NoteInfo != null && NoteInfo.lstChildNotes != null && NoteInfo.lstChildNotes.Count > 0)
                        cProject.cEdit_Alt = NoteInfo.lstChildNotes[0];
                }
            }
        }

    }
}

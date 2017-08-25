using System;
using System.Xml;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace ARMSim.GUI
{
	/// <summary>
	/// Represents a most recently used (MRU) menu.
	/// </summary>
	/// <remarks>This class shows the MRU list in a popup menu. To display
	/// the MRU list "inline" use <see labelName="MruMenuInline" />.
	/// <para>The class will optionally load the last set of files from the registry
	/// on construction and store them when instructed by the main program.</para>
	/// <para>Internally, this class uses zero-based numbering for the items.
	/// The displayed numbers, however, will start with one.</para></remarks>
    public class MruStripMenu : ARMSim.Preferences.IViewXMLSettings
    {
        private ClickedHandler clickedHandler;
        protected ToolStripMenuItem recentFileMenuItem;
        protected string registryKeyName;
        protected int numberEntries;
        protected int maxEntries = 4;
        protected int maxShortenPathLength = 96;
        protected Mutex mruStripMutex;

        private const string mMenuElement = "MRUMenuSettings";
        private const string mRecentFilesElement = "RecentFiles";
        private const string mFilesElement = "Files";
        private const string mMultipleFilesElement = "MultiFiles";
        private const string mMultipleFilesLoadedElement = "MultiFilesLoaded";

        //these next 2 entries are for tracking multiple file projects that are open.
        //If the user has opened a multi-file project then this string array will be the
        //names of the files and the flag will be true.
        //It is possible that this array is loaded with filenames but the current project
        //does not have multi-files. We wish to keep this in tack so it can be saved to the
        //save document at application shutdown and loaded at startup. This means the user can maintain
        //a single project definition without having to respecify filenames even if they switch to s single
        //file project. - dale
        protected IList<string> _loadedFiles;
        protected bool mMultipleFilesLoaded;

        public static string TagName { get { return mMenuElement; } }

        #region MruMenuItem

        /// <summary>
        /// The menu item which will contain the MRU entry.
        /// </summary>
        /// <remarks>The menu may display a shortened or otherwise invalid pathname.
        /// This class stores the actual filename, preferably as a fully
        /// resolved labelName, that will be returned in the event handler.</remarks>
        public class MruMenuItem : ToolStripMenuItem
        {
            /// <summary>
            /// Initializes a new instance of the MruMenuItem class.
            /// </summary>
            public MruMenuItem()
            {
                Tag = "";
            }

            /// <summary>
            /// Initializes an MruMenuItem object.
            /// </summary>
            /// <param labelName="filename">The string to actually return in the <paramref labelName="eventHandler">eventHandler</paramref>.</param>
            /// <param labelName="entryname">The string that will be displayed in the menu.</param>
            /// <param labelName="eventHandler">The <see cref="EventHandler">EventHandler</see> that 
            /// handles the <see cref="MenuItem.Click">Click</see> event for this menu item.</param>
            public MruMenuItem(string filename, string entryName, EventHandler eventHandler)
            {
                Tag = filename;
                Text = entryName;
                Click += eventHandler;
            }

            /// <summary>
            /// Gets the filename.
            /// </summary>
            /// <value>Gets the filename.</value>
            public string Filename
            {
                get
                {
                    return (string)Tag;
                }
                set
                {
                    Tag = value;
                }
            }
        }
        #endregion

        protected MruStripMenu() { }

        protected void Init(ToolStripMenuItem recentFileMenuItem, ClickedHandler clickedHandler, int maxEntries)
        {
            if (recentFileMenuItem == null)
                throw new ArgumentNullException("recentFileMenuItem");

            this.recentFileMenuItem = recentFileMenuItem;
            this.recentFileMenuItem.Checked = false;
            this.recentFileMenuItem.Enabled = false;

            this.maxEntries = maxEntries;
            this.clickedHandler = clickedHandler;
        }


        #region Event Handling

        public delegate void ClickedHandler(int number, string filename);

        protected void OnClick(object sender, System.EventArgs e)
        {
            MruMenuItem menuItem = (MruMenuItem)sender;
            clickedHandler(MenuItems.IndexOf(menuItem) - StartIndex, menuItem.Filename);
        }


        #endregion

        #region Properties

        public virtual ToolStripItemCollection MenuItems
        {
            get
            {
                return recentFileMenuItem.DropDownItems;
            }
        }

        public virtual int StartIndex
        {
            get
            {
                return 0;
            }
        }

        public virtual int EndIndex
        {
            get
            {
                return numberEntries;
            }
        }

        //public int NumEntries
        //{
        //    get
        //    {
        //        return numberEntries;
        //    }
        //}

        public int MaxEntries
        {
            get
            {
                return maxEntries;
            }
            set
            {
                if (value > 16)
                {
                    maxEntries = 16;
                }
                else
                {
                    maxEntries = value < 4 ? 4 : value;

                    int index = StartIndex + maxEntries;
                    while (numberEntries > maxEntries)
                    {
                        MenuItems.RemoveAt(index);
                        numberEntries--;
                    }
                }
            }
        }

        public int MaxShortenPathLength
        {
            get
            {
                return maxShortenPathLength;
            }
            set
            {
                maxShortenPathLength = value < 16 ? 16 : value;
            }
        }

        public virtual bool IsInline
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Helper Methods

        protected virtual void Enable()
        {
            recentFileMenuItem.Enabled = true;
        }

        protected virtual void Disable()
        {
            recentFileMenuItem.Enabled = false;
            //recentFileMenuItem.MenuItems.RemoveAt(0);
        }

        protected virtual void SetFirstFile(MruMenuItem menuItem)
        {
        }

        public void SetFirstFile(int number)
        {
            if (number > 0 && numberEntries > 1 && number < numberEntries)
            {
                MruMenuItem menuItem = (MruMenuItem)MenuItems[StartIndex + number];

                MenuItems.RemoveAt(StartIndex + number);
                MenuItems.Insert(StartIndex, menuItem);

                SetFirstFile(menuItem);
                FixupPrefixes(0);
            }
        }

        public static string FixupEntryName(int number, string entryName)
        {
            if (number < 9)
                return "&" + (number + 1) + "  " + entryName;
            else if (number == 9)
                return "1&0" + "  " + entryName;
            else
                return (number + 1) + "  " + entryName;
        }

        protected void FixupPrefixes(int startNumber)
        {
            if (startNumber < 0)
                startNumber = 0;

            if (startNumber < maxEntries)
            {
                for (int i = StartIndex + startNumber; i < EndIndex; i++, startNumber++)
                {
                    MenuItems[i].Text = FixupEntryName(startNumber, MenuItems[i].Text.Substring(startNumber == 9 ? 5 : 4));
                }
            }
        }

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param labelName="pathname">The pathname to shorten.</param>
        /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        static public string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength)
                return pathname;

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
                root += Path.DirectorySeparatorChar;

            string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    else
                    {
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
            }
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
            {
                root += "...\\";

                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string

                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathname;
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Returns the entry number matching the passed filename.
        /// </summary>
        /// <param name="filename">The filename to search for.</param>
        /// <returns>The entry number of the matching filename or -1 if not found.</returns>
        public int FindFilenameNumber(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (filename.Length == 0)
                throw new ArgumentException("filename has length 0");

            if (numberEntries > 0)
            {
                int number = 0;
                for (int i = StartIndex; i < EndIndex; i++, number++)
                {
                    if (string.Compare(((MruMenuItem)MenuItems[i]).Filename, filename, true) == 0)
                    {
                        return number;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the menu index of the passed filename.
        /// </summary>
        /// <param name="filename">The filename to search for.</param>
        /// <returns>The menu index of the matching filename or -1 if not found.</returns>
        public int FindFilenameMenuIndex(string filename)
        {
            int number = FindFilenameNumber(filename);
            return number < 0 ? -1 : StartIndex + number;
        }

        /// <summary>
        /// Returns the menu index for a specified MRU item number.
        /// </summary>
        /// <param name="number">The MRU item number.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>The menu index of the passed MRU number.</returns>
        public int GetMenuIndex(int number)
        {
            if (number < 0 || number >= numberEntries)
                throw new ArgumentOutOfRangeException("number");

            return StartIndex + number;
        }

        public string GetFileAt(int number)
        {
            if (number < 0 || number >= numberEntries)
                throw new ArgumentOutOfRangeException("number");

            return ((MruMenuItem)MenuItems[StartIndex + number]).Filename;
        }

        public string[] GetFiles()
        {
            string[] filenames = new string[numberEntries];

            int index = StartIndex;
            for (int i = 0; i < filenames.GetLength(0); i++, index++)
            {
                filenames[i] = ((MruMenuItem)MenuItems[index]).Filename;
            }

            return filenames;
        }

        //// This is used for testing
        //public string[] GetFilesFullEntryString()
        //{
        //    string[] filenames = new string[numberEntries];

        //    int index = StartIndex;
        //    for (int i = 0; i < filenames.GetLength(0); i++, index++)
        //    {
        //        filenames[i] = MenuItems[index].Text;
        //    }

        //    return filenames;
        //}
        #endregion

        #region Add Methods

        public void SetFiles(string[] filenames)
        {
            RemoveAll();
            for (int i = filenames.GetLength(0) - 1; i >= 0; i--)
            {
                AddFile(filenames[i]);
            }
        }

        public void AddFiles(IList<string> files)
        {
            //if more than one file being loaded, then we have a multi file project
            if (files.Count > 1)
            {
                _loadedFiles = files;

                //indicate that the current loaded files are a multi file project
                mMultipleFilesLoaded = true;
            }//if
            else
            {
                mMultipleFilesLoaded = false;
                //otherwise a single file project
                AddFile(files[0]);
            }//else
        }//AddFiles

        public void AddFile(string filename)
        {
            string pathname = Path.GetFullPath(filename);
            AddFile(pathname, ShortenPathname(pathname, MaxShortenPathLength));
        }

        public void AddFile(string filename, string entryName)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (filename.Length == 0)
                throw new ArgumentException("filename");

            if (numberEntries > 0)
            {
                int index = FindFilenameMenuIndex(filename);
                if (index >= 0)
                {
                    SetFirstFile(index - StartIndex);
                    return;
                }
            }

            if (numberEntries < maxEntries)
            {
                MruMenuItem menuItem = new MruMenuItem(filename, FixupEntryName(0, entryName), new System.EventHandler(OnClick));
                MenuItems.Insert(StartIndex, menuItem);
                SetFirstFile(menuItem);

                if (numberEntries++ == 0)
                {
                    Enable();
                }
                else
                {
                    FixupPrefixes(1);
                }
            }
            else if (numberEntries > 1)
            {
                MruMenuItem menuItem = (MruMenuItem)MenuItems[StartIndex + numberEntries - 1];
                MenuItems.RemoveAt(StartIndex + numberEntries - 1);

                menuItem.Text = FixupEntryName(0, entryName);
                menuItem.Filename = filename;

                MenuItems.Insert(StartIndex, menuItem);

                SetFirstFile(menuItem);
                FixupPrefixes(1);
            }
        }

        #endregion

        #region Remove Methods

        public void RemoveFile(int number)
        {
            if (number >= 0 && number < numberEntries)
            {
                if (--numberEntries == 0)
                {
                    Disable();
                }
                else
                {
                    int startIndex = StartIndex;
                    if (number == 0)
                    {
                        SetFirstFile((MruMenuItem)MenuItems[startIndex + 1]);
                    }

                    MenuItems.RemoveAt(startIndex + number);

                    if (number < numberEntries)
                    {
                        FixupPrefixes(number);
                    }
                }
            }
        }

        public void RemoveFile(string filename)
        {
            if (numberEntries > 0)
            {
                RemoveFile(FindFilenameNumber(filename));
            }
        }

        public void RemoveAll()
        {
            if (numberEntries > 0)
            {
                for (int index = EndIndex - 1; index > StartIndex; index--)
                {
                    MenuItems.RemoveAt(index);
                }
                Disable();
                numberEntries = 0;
            }
        }

        #endregion

        #region Rename Methods

        //public void RenameFile(string oldFilename, string newFilename)
        //{
        //    string newPathname = Path.GetFullPath(newFilename);

        //    RenameFile(Path.GetFullPath(oldFilename), newPathname, ShortenPathname(newPathname, MaxShortenPathLength));
        //}

        //public void RenameFile(string oldFilename, string newFilename, string newEntryname)
        //{
        //    if (newFilename == null)
        //        throw new ArgumentNullException("newFilename");

        //    if (newFilename.Length == 0)
        //        throw new ArgumentException("newFilename");

        //    if (numberEntries > 0)
        //    {
        //        int index = FindFilenameMenuIndex(oldFilename);
        //        if (index >= 0)
        //        {
        //            MruMenuItem menuItem = (MruMenuItem)MenuItems[index];
        //            menuItem.Text = FixupEntryName(0, newEntryname);
        //            menuItem.Filename = newFilename;
        //            return;
        //        }
        //    }

        //    AddFile(newFilename, newEntryname);
        //}

        #endregion

        public void defaultSettings()
        {
            //clear out data structures
            this.RemoveAll();
            _loadedFiles = null;
            mMultipleFilesLoaded = false;
        }

        //return the flag indicating if multi files are currently loaded
        public bool MultipleFilesLoaded
        {
            get { return mMultipleFilesLoaded; }
        }
        //return the most recent multi file project.
        //this may return a null!!!
        public IList<string> RecentMultipleFile
        {
            get
            {
                return _loadedFiles;
            }
        }
        //return the most recent file from the single file project list
        //returns and empty string if no files loaded
        public string MostRecentFile
        {
            get
            {
                if (numberEntries <= 0)
                {
                    return "";
                }
                else
                {
                    return GetFileAt(0);
                }
            }
        }

        public void saveState(XmlWriter xmlOut)
        {
            try
            {
                // Add an extra node into the config to store some useless information
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                for (int ii = EndIndex-1; ii >= StartIndex; ii--)
                {
                    if (ii != EndIndex - 1) str.Append(",");
                    str.Append(((MruMenuItem)MenuItems[ii]).Filename);
                }

                xmlOut.WriteStartElement(mMenuElement);

                xmlOut.WriteStartElement(mRecentFilesElement);
                if (str.Length > 0)
                {
                    xmlOut.WriteAttributeString(mFilesElement, str.ToString());
                }
                xmlOut.WriteEndElement();

                xmlOut.WriteStartElement(mMultipleFilesElement);
                if (_loadedFiles != null && _loadedFiles.Count > 0)
                {
                    str.Length = 0;
                    for (int ii = 0; ii < _loadedFiles.Count; ii++)
                    {
                        if (ii != 0) str.Append(",");
                        str.Append(_loadedFiles[ii]);
                    }
                    xmlOut.WriteAttributeString(mFilesElement, str.ToString());
                }
                xmlOut.WriteAttributeString(mMultipleFilesElement, this.mMultipleFilesLoaded.ToString());
                xmlOut.WriteEndElement();

                xmlOut.WriteEndElement();
            }
            catch (Exception)
            { }

        }//saveState

        private void loadRecentFiles(string str)
        {
            if (str != null)
            {
                //break files into an array
                string[] files = str.Split(new char[] { ',' });
                if (files != null)
                {
                    //add each file to MRU list if it is valid
                    foreach (string f in files)
                    {  // added path length limit -- nigel
                        if (f.Length > 0 && f.Length < 260 && File.Exists(Path.GetFullPath(f)))
                        {
                            AddFile(f);
                        }//if
                    }//foreach
                }//if
            }//if
        }//loadRecentFiles

        private void loadMultiFiles(string str, string flag)
        {
            if (str != null)
            {
                //break files into an array
                string[] files = str.Split(new char[] { ',' });

                if (files != null)
                {
                    //count the valid filenames, check that the files exist
                    int count = 0;
                    foreach (string f in files)
                    {   // added file length limit -- nigel
                        if (f.Length > 0 && f.Length < 260 && File.Exists(Path.GetFullPath(f)))
                        {
                            count++;
                        }//if
                    }//foreach

                    if (count > 0)
                    {
                        //allocate the array
                        _loadedFiles = new string[count];
                        count = 0;

                        //load each filename into array
                        foreach (string f in files)
                        {   // added file length limit -- nigel
                            if (f.Length > 0 && f.Length < 260 && File.Exists(Path.GetFullPath(f)))
                            {
                                _loadedFiles[count] = f;
                                count++;
                            }//if
                        }//foreach
                    }//if
                }//if
            }//if

            if (flag!=null)
            {
                mMultipleFilesLoaded = bool.Parse(flag);
			}//if
        }//loadMultiFiles

        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                //clear out data structures
                this.defaultSettings();

                xmlIn.MoveToContent();
                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement) break;
                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        //if the MRU list , hand off
                        if (xmlIn.Name == mRecentFilesElement)
                        {
                            //attempt to load the multi file project list. May not be in document, so handle null case
                            loadRecentFiles(xmlIn.GetAttribute(mFilesElement));
                        }//if
                        else if (xmlIn.Name == mMultipleFilesElement)
                        {
                            loadMultiFiles(xmlIn.GetAttribute(mFilesElement), xmlIn.GetAttribute(mMultipleFilesLoadedElement));
                        }
                    }//if
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }
            catch (Exception ex)
            {
                //if an exception occurs, simply accept the OS defaults
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                defaultSettings();
            }
        }
    }

	/// <summary>
	/// Represents an inline most recently used (mru) menu.
	/// </summary>
	/// <remarks>
	/// This class shows the MRU list "inline". To display
	/// the MRU list as a popup menu use <see labelName="MruMenu">MruMenu</see>.
	/// </remarks>
	public class MruStripMenuInline : MruStripMenu
	{
		protected ToolStripMenuItem owningMenu;
		protected ToolStripMenuItem firstMenuItem;

		public MruStripMenuInline(ToolStripMenuItem owningMenu, ToolStripMenuItem recentFileMenuItem, ClickedHandler clickedHandler)
		{
            maxShortenPathLength = 48;
            this.owningMenu = owningMenu;
            this.firstMenuItem = recentFileMenuItem;
            Init(recentFileMenuItem, clickedHandler, maxEntries);
		}

		#region Overridden Properties

		public override ToolStripItemCollection MenuItems
		{
			get
			{
				return owningMenu.DropDownItems;
			}
		}

		public override int StartIndex
		{
			get
			{
				return MenuItems.IndexOf(firstMenuItem);
			}
		}

		public override int EndIndex
		{
			get
			{
				return StartIndex + numberEntries;
			}
		}

		public override bool IsInline
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Overridden Methods

		protected override void Enable()
		{
			MenuItems.Remove(recentFileMenuItem);
		}

		protected override void SetFirstFile(MruMenuItem menuItem)
		{
			firstMenuItem = menuItem;
		}

		protected override void Disable()
		{
			int index = MenuItems.IndexOf(firstMenuItem);
			MenuItems.RemoveAt(index);
			MenuItems.Insert(index, recentFileMenuItem);
			firstMenuItem = recentFileMenuItem;
		}
		#endregion

	}
}
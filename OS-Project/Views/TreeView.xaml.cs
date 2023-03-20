using OS_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace OS_Project.Views
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class TreeView : UserControl
    {
        public class TreeViewContext
        {

            public List<Driver> Drivers { get; set; }

            public TreeViewContext()
            {
                Drivers = new List<Driver>();
            }
        }

        public class FAT
        {
            public byte[] BS;
            public byte[] RDET;
            public string driveName;
            public ulong BytesPerSector;          //Bytes per sector
            public ulong SectorsPerCluster;       //Sectors per cluster (Sc)
            public ulong ReversedSector;          //Reversed Sectors (Sb) : Số sector trước bảng FAT
            public ulong NumOfFat;                // so bang FAT
            public ulong SizeOfVolume;            // Size of volume(bytes)
            public ulong FatSize;                 // in sectors (Sf)
            public string Version;
            public ulong StartedCluster;
            public ulong starting_RDET;

            public FAT(ulong startingPartitionPosition, string _driveName)
            {
                driveName = _driveName;

                // init MBR, FAT_table, RDET
                BS = new byte[512];

                RDET = new byte[512];

                using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                {
                    // get BS
                    fs.Seek((long)startingPartitionPosition, SeekOrigin.Begin);
                    fs.Read(BS, 0, BS.Length);

                    // get BS information
                    BytesPerSector = BitConverter.ToUInt16(BS, 0x0B);          //Bytes per sector
                    SectorsPerCluster = (ulong)BS[0x0D];                        //Sectors per cluster (Sc)
                    ReversedSector = BitConverter.ToUInt16(BS, 0x0E);          //Reversed Sectors (Sb) : Số sector trước bảng FATfs.Close();
                    NumOfFat = (ulong)BS[0x10];                                 //Number of FAT tables (Nf)
                    SizeOfVolume = BitConverter.ToUInt32(BS, 0x20);            //Size of volume
                    FatSize = (ulong)BitConverter.ToInt32(BS, 0x24);                 //FAT size in sectors (Sf)
                    Version = Encoding.ASCII.GetString(BS, 0x52, 8);         //Version of FAT
                    StartedCluster = (ulong)BitConverter.ToInt32(BS, 0x2C);

                    starting_RDET = startingPartitionPosition + ReversedSector * 512 + FatSize * 2 * 512; // starting position of RDET

                    // get RDET
                    fs.Seek((long)(starting_RDET), SeekOrigin.Begin);
                    fs.Read(RDET, 0, RDET.Length);


                    fs.Close();
                }
            }
        }

        public class Driver
        {
            public string fullname { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public ulong starting_position { get; set; }
        }

        public class NodeInfo : ObservableObject
        {
            public string fullpath { get; set; }
            public ulong RDET_start { get; set; }
            public ulong sub_dir_start { get; set; }
            public bool isFile { get; set; }
            private bool _isExpanded;
            public bool isExpanded { get; set; }
            public ulong size { get; set; }
            public string date { get; set; }
            public string time { get; set; }
            public string timeModified { get; set; }
            public string isArchive { get; set; }
            public string isDirectory { get; set; }
            public string isHidden { get; set; }
            public string isSystem { get; set; }
            public string isVolLabel { get; set; }
            public string isReadOnly { get; set; }

            public NodeInfo() {}

            public NodeInfo(NodeInfo _a)
            {
                fullpath = _a.fullpath;
                RDET_start = _a.RDET_start;
                sub_dir_start = _a.sub_dir_start;
                isFile = _a.isFile;
                isExpanded = _a.isExpanded;
                size = _a.size;
                date = _a.date;
                time = _a.time;
                timeModified = _a.timeModified;

                isArchive = _a.isArchive;
                isDirectory = _a.isDirectory;
                isHidden = _a.isHidden;
                isSystem = _a.isSystem;
                isVolLabel = _a.isVolLabel;
                isReadOnly = _a.isReadOnly;
                OnPropertyChanged("Tag");
            }
        }

        public static string PATH = @"\\.\PhysicalDrive1";

        public TreeView()
        {
            InitializeComponent();



            TreeViewContext treeViewContext = new TreeViewContext();

            ulong[] startingPartitionsPosition = getPartitionsStartingPosition();

         
            #region with each partition -> get it's name and format
            int index = 0;

            List<Driver> drivers = new List<Driver>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    byte[] buffer = new byte[512];
                    using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                    {
                        fs.Seek((long)startingPartitionsPosition[index], SeekOrigin.Begin);
                        fs.Read(buffer, 0, 512);
                        fs.Close();
                    }

                    string format = Encoding.ASCII.GetString(buffer, 0x03, 4);

                    Driver newDriver = new Driver()
                    {
                        fullname = drive.Name + drive.VolumeLabel,
                        name = drive.Name,
                        type = format != "NTFS" ? "FAT32" : format,
                        starting_position = startingPartitionsPosition[index]
                    };

                    index++;

                    drivers.Add(newDriver);
                }
            }

            foreach (Driver driver in drivers)
            {
                treeViewContext.Drivers.Add(driver);
            }
            DataContext = treeViewContext;

            #endregion

        }

        static byte[] getMBR()
        {
            byte[] MBR_buffer = new byte[512];

            using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            {
                fs.Read(MBR_buffer, 0, MBR_buffer.Length);
                fs.Close();
            }
            return MBR_buffer;
        }

        static ulong[] getPartitionsStartingPosition()
        {
            byte[] MBR_buffer = new byte[512];
            MBR_buffer = getMBR();
            ulong[] result = new ulong[4];

            for (int i = 0; i < 4; i++)
            {
                int offset = 0x01BE + (i * 16);
                result[i] = (ulong)BitConverter.ToInt32(MBR_buffer, offset + 8) * 512; // get starting sector and turn to bytes
            }
            return result;
        }

        private void DriveBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            Driver info = (Driver)clickedButton.Tag;

            if (info.type == "NTFS")
            {

            } else
            {
                FAT fat = new FAT(info.starting_position, info.name);
                FolderView.Items.Clear();

                getFATFileFolderNames(fat.RDET, fat.starting_RDET, fat.driveName, null);

            }

        }

        public static string GetFileFolderName(string path)
        {
            // if we have empty/null string
            if (string.IsNullOrEmpty(path)) return string.Empty;

            var normalizedPath = path.Replace('/', '\\');

            var lastIndex = normalizedPath.LastIndexOf('\\');

            // if we don't find the backslash index then full path itself is the name
            if (lastIndex <= 0) return path;

            // return the string after the last backslash(the name we want to find)
            return path.Substring(lastIndex + 1);
        }



        public void getFATFileFolderNames(byte[] data, ulong starting_RDET, string path, TreeViewItem item)
        {

            int index = 0;

            //Iterate through each entry of the directory
            string name = "";
            while (data[index * 32] != 0) // while this entry is not empty
            {
                if (data[index * 32] != 0xE5) // if file/folder was NOT deleted 
                {
                    if (data[index * 32 + 11] == 0x0F) // if this is long entry
                    {
                        name = Encoding.ASCII.GetString(data, index * 32 + 1, 10).TrimEnd() + Encoding.ASCII.GetString(data, index * 32 + 14, 12).TrimEnd() + Encoding.ASCII.GetString(data, index * 32 + 28, 4).TrimEnd() + name;
                    }
                    else // this a short entry
                    {
                        if (name == "") // if there is no long entry, take the name from short entry -> vi tri 00(11 bytes)
                        {
                            if (data[index * 32 + 11] == 0x20) // if entry is file
                                name = Encoding.ASCII.GetString(data, index * 32, 8).TrimEnd() + "." + Encoding.ASCII.GetString(data, index * 32 + 8, 3).TrimEnd();
                            else name = Encoding.ASCII.GetString(data, index * 32, 8).TrimEnd(); //entry is folder
                        }
                        name = name.Replace("?", ""); 

                        // UI: create a TreeViewItem
                        var sub_item = new TreeViewItem();
                        sub_item.Header = name;

                        NodeInfo info = new NodeInfo() {
                            fullpath = path + '\\' + name,
                            RDET_start = starting_RDET,
                            sub_dir_start = 0,
                            isFile = true,
                            isExpanded = false,
                            size = getSize(data, index * 32),
                            date = getDate(data, index * 32),
                            time = getTimeCreated(data, index * 32),
                            timeModified = getTimeModified(data, index * 32),
                            isArchive = isArchive(data, index * 32),
                            isDirectory = isDirectory(data, index * 32),
                            isHidden = isHidden(data, index * 32),
                            isReadOnly = isReadOnly(data, index * 32),
                            isSystem = isSystem(data, index * 32),
                            isVolLabel = isVolLabel(data, index * 32)
                        };

                        sub_item.Tag = info; // store Info of current TreeViewItem in a tag
                        sub_item.DataContext = info.fullpath;  // use to store the fullpath of file/folder
                        sub_item.MouseDoubleClick += TreeItem_DoubleClicked;

                        if (data[index * 32 + 11] != 0x16 && data[index * 32 + 11] != 0x08 && name != "." && name != ".." ) // ignore system file 
                        {

                            if (data[index * 32 + 11] == 0x10) //folder
                            {

                                ulong startingCluster = (ulong)BitConverter.ToInt16(data, index * 32 + 26);

                                // UI

                                //check if folder contains any file or folder ?

                                info.sub_dir_start = starting_RDET + (startingCluster - 2) * 8 * 512;
                                info.isFile = false;


                                if (isAnyFileFolder(info.sub_dir_start))
                                {
                                    sub_item.Items.Add(null);
                                    sub_item.Expanded += Folder_Expanded;
                                    sub_item.Collapsed += Folder_Collapsed;
                                    sub_item.MouseDoubleClick -= TreeItem_DoubleClicked;
                                }

                                //startingClusters.Enqueue();
                            }

                            if (item == null)
                            {
                                FolderView.Items.Add(sub_item);
                            }
                            else
                            {
                                item.Items.Add(sub_item);
                            }
                        }
                        name = "";

                    }
                }
                index++;
            }
        }

        private void TreeItem_DoubleClicked(object sender, RoutedEventArgs e)
        {

            if (!e.Handled)
            {
                TreeViewItem item = sender as TreeViewItem;
                NodeInfo info = (NodeInfo)item.Tag;

                #region Display detail info

                FName.Text = GetFileFolderName(info.fullpath);
                FSize.Text = info.size.ToString();
                FTime.Text = info.time;
                FDate.Text = info.date;
                FArchive.Text = info.isArchive;
                FHidden.Text = info.isHidden;
                FSystem.Text = info.isHidden;
                FVolLabel.Text = info.isVolLabel;
                FDirectory.Text = info.isDirectory;
                FReadOnly.Text = info.isReadOnly;
                FTimeModified.Text = info.timeModified;

                #endregion

                e.Handled = true;
            }
        }

        private bool isAnyFileFolder(ulong startingPosition)
        {

            byte[] data = new byte[512];
            using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            {
                fs.Seek((long)(startingPosition), SeekOrigin.Begin); //
                fs.Read(data, 0, data.Length);
                fs.Close();
            }
            if (data[64] == 0) return false;
            return true;
        }

        private void Folder_Collapsed(object sender, RoutedEventArgs e)
        {
          
            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                NodeInfo info = (NodeInfo)item.Tag;
                #endregion

                info.isExpanded = false;
                item.Tag = new NodeInfo(info);

                #region Display detail info

                FName.Text = GetFileFolderName(info.fullpath);
                FSize.Text = info.size.ToString();
                FTime.Text = info.time;
                FDate.Text = info.date;
                FArchive.Text = info.isArchive;
                FHidden.Text = info.isHidden;
                FSystem.Text = info.isSystem;
                FVolLabel.Text = info.isVolLabel;
                FDirectory.Text = info.isDirectory;
                FReadOnly.Text = info.isReadOnly;
                FTimeModified.Text = info.timeModified;

                #endregion

                e.Handled = true;
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
        

            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                NodeInfo info = (NodeInfo)item.Tag;

                #endregion

                //change the icon of folder from close to expanded
                info.isExpanded = true;
                item.Tag = new NodeInfo(info);

                #region Display detail info

                FName.Text = GetFileFolderName(info.fullpath);
                FSize.Text = info.size.ToString();
                FTime.Text = info.time;
                FDate.Text = info.date;
                FArchive.Text = info.isArchive;
                FHidden.Text = info.isHidden;
                FSystem.Text = info.isSystem;
                FVolLabel.Text = info.isVolLabel;
                FDirectory.Text = info.isDirectory;
                FReadOnly.Text = info.isReadOnly;
                FTimeModified.Text = info.timeModified;

                #endregion

                e.Handled = true;

                #region check if the list contain only dummy data and if yes clear it
                if (item.Items.Count != 1 || item.Items[0] != null) return;

                item.Items.Clear();
                #endregion


                #region Get Sub_dir/Data of this TreeviewItem
                byte[] sub_dir = new byte[512];

                using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek((long)(info.sub_dir_start), SeekOrigin.Begin); //
                    fs.Read(sub_dir, 0, sub_dir.Length);
                    fs.Close();
                }
                #endregion

                getFATFileFolderNames(sub_dir, info.RDET_start, info.fullpath, item);
                
                
            }


        }

        #region get File/Folder Detail Infomation
        static ulong getSize(byte[] data, int start)
        {
            ulong size = 0;
            size = (ulong)BitConverter.ToInt32(data, start + 0x1C);
            return size;
        }

        static string getDate(byte[] data, int start)
        {
            string date = "";
            int dec = BitConverter.ToInt16(data, start + 0x10);
            int d = dec & 0x1F;
            dec = dec >> 5;
            int m = dec & 0xF;
            dec = dec >> 4;
            int y = dec + 1980;
            date = d.ToString() + "/" + m.ToString() + "/" + y.ToString();
            return date;
        }

        static string getTimeCreated(byte[] data, int start)
        {
            string time = "";
            byte[] getTime = new byte[4];
            int k = 0;
            for (int i = 0; i < 3; i++)
                getTime[i] = data[start + 0x0D + i];
            getTime[3] = 0x00;
            int dec = BitConverter.ToInt32(getTime, 0);
            int ms = dec & 0x7F;
            dec = dec >> 7;
            int s = dec & 0x3F;
            dec = dec >> 6;
            int m = dec & 0x3F;
            dec = dec >> 6;
            int h = dec;
            time = h.ToString() + ":" + m.ToString() + ":" + s.ToString() + ":" + ms.ToString();
            return time;
        }

        static string getTimeModified(byte[] data, int start)
        {
            string time = "";
            int dec = BitConverter.ToInt16(data, start);
            int s = dec & 0x1F;
            s *= 2;
            dec = dec >> 5;
            int m = dec & 0x3F;
            dec = dec >> 6;
            int h = dec;
            TimeSpan a = new TimeSpan(h, m, s);
            time = a.ToString();
            return time;
        }

        #region Attribute
        static string isArchive(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x20;
            int check = dec >> 5;
            return check == 1 ? "True" : "False";
        }
        static string isDirectory(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x10;
            int check = dec >> 4;
            return check == 1 ? "True" : "False";
        }
        static string isVolLabel(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x8;
            int check = dec >> 3;
            return check == 1 ? "True" : "False";
        }
        static string isSystem(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x4;
            int check = dec >> 2;
            return check == 1 ? "True" : "False";
        }
        static string isHidden(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x2;
            int check = dec >> 1;
            return check == 1 ? "True" : "False";
        }
        static string isReadOnly(byte[] data, int start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x1;
            int check = dec;
            return check == 1 ? "True" : "False";
        }
        #endregion

        #endregion
    }
}

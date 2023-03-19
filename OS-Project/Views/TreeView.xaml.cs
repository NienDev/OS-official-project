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

        public class NodeInfo
        {
            public string fullpath;
            public ulong RDET_start;
            public ulong sub_dir_start;
            public bool isFile;
        }

        public static string PATH = @"\\.\PhysicalDrive1";

        public TreeView()
        {
            InitializeComponent();

            

            TreeViewContext treeViewContext = new TreeViewContext();

            ulong[] startingPartitionsPosition = getPartitionsStartingPosition();

            // with each partition -> get it's name and format
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



        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {

            //    int i = 0;

            //    foreach (var drive in Directory.GetLogicalDrives())
            //    {
            //        var item = new TreeViewItem();
            //        item.Header = drive;
            //        item.Tag = drive; // store starting position of partition

            //        if (isFileFolderInDriveDir(drive))
            //        {
            //            item.Items.Add(null);
            //            item.Expanded += Folder_Expanded;
            //        }

            //        FolderView.Items.Add(item);
            //    }
            //}

            //    private void Folder_Expanded(object sender, RoutedEventArgs e)
            //    {
            //        var item = (TreeViewItem)sender;
            //        // if the list contain only dummy data
            //        if (item.Items.Count != 1 || item.Items[0] != null) return;

            //        // clear dummy data
            //        item.Items.Clear();

            //        #region Get Folders
            //        // get starting position of clicked partition
            //        var starting = (ulong)item.Tag;

            //        var directories = new List<string>();

            //        // use try catch here to avoid the error which is can not access to the system column folder
            //        try
            //        {
            //            var dirs = Directory.GetDirectories(fullPath);
            //            if (dirs.Length > 0)
            //            {
            //                directories.AddRange(dirs);
            //            }
            //        }
            //        catch
            //        {

            //        }

            //        directories.ForEach(directoryPath =>
            //        {
            //            var subItem = new TreeViewItem()
            //            {
            //                Header = GetFileFolderName(directoryPath),
            //                Tag = directoryPath
            //            };

            //            if (isFileFolderInDriveDir(directoryPath))
            //            {
            //                subItem.Items.Add(null);

            //                subItem.Expanded += Folder_Expanded;
            //            }


            //            item.Items.Add(subItem);
            //        });
            //        #endregion

            //        #region Get Files

            //        var files = new List<string>();

            //        try
            //        {
            //            var fs = Directory.GetFiles(fullPath);

            //            if (fs.Length > 0) files.AddRange(fs);
            //        }
            //        catch { }
            //        files.ForEach(filePath =>
            //        {
            //            // create item for a file
            //            var subItem = new TreeViewItem()
            //            {
            //                Header = GetFileFolderName(filePath),
            //                Tag = filePath
            //            };
            //            item.Items.Add(subItem);

            //        });

            //        #endregion
            //    }

            //    #region Find folder/file name
            //    /// <summary>
            //    /// Find a folder/file name from full path
            //    /// </summary>
            //    /// <returns></returns>
           
            //    #endregion

            //    #region If Drive/Folder contain any folder/file
            //    /// <summary>
            //    /// Check if drive/folder contain any folder or file 
            //    /// </summary>

            //    public static bool isFileFolderInDriveDir(string path)
            //    {
            //        var directories = new List<string>();
            //        var files = new List<string>();

            //        try
            //        {
            //            var dirs = Directory.GetDirectories(path);
            //            if (dirs.Length > 0)
            //            {
            //                directories.AddRange(dirs);
            //            }
            //        }
            //        catch
            //        {

            //        }

            //        try
            //        {
            //            var fs = Directory.GetFiles(path);
            //            if (fs.Length > 0) files.AddRange(fs);
            //        }
            //        catch { }

            //        if (directories.Count > 0 || files.Count > 0)
            //        {
            //            return true;
            //        }

            //        return false;
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
            while (data[index * 32] != 0)
            {
                if (data[index * 32] != 0xE5) // if file/folder was deleted or the type is not related to system
                {
                    if (data[index * 32 + 11] == 0x0F)
                    {
                        name = Encoding.ASCII.GetString(data, index * 32 + 1, 10).TrimEnd() + Encoding.ASCII.GetString(data, index * 32 + 14, 12).TrimEnd() + Encoding.ASCII.GetString(data, index * 32 + 28, 4).TrimEnd() + name;
                    }
                    else
                    {
                        // entry chinh
                        // neu ko co entry phu, -> lay ten o entry chinh -> vi tri 00(11)

                        if (name == "")
                        {
                            if (data[index * 32 + 11] == 0x20)
                                name = Encoding.ASCII.GetString(data, index * 32, 8).TrimEnd() + "." + Encoding.ASCII.GetString(data, index * 32 + 8, 3).TrimEnd();
                            else name = Encoding.ASCII.GetString(data, index * 32, 8).TrimEnd();
                        }
                        name = name.Replace("?", "");

                        // UI
                        var sub_item = new TreeViewItem();
                        sub_item.Header = name;

                        NodeInfo info = new NodeInfo() {
                            fullpath = path + name,
                            RDET_start = starting_RDET,
                            sub_dir_start = 0,
                            isFile = true,
                        };

                        sub_item.Tag = info;

                        if (data[index * 32 + 11] != 0x16 && name != "." && name != "..") // ignore system file
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

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            // if the list contain only dummy data
            if (item.Items.Count != 1 || item.Items[0] != null) return;

            // clear dummy data
            item.Items.Clear();

            NodeInfo info = (NodeInfo)item.Tag;

            byte[] sub_dir = new byte[512];

            using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            {
                fs.Seek((long)(info.sub_dir_start), SeekOrigin.Begin); //
                fs.Read(sub_dir, 0, sub_dir.Length);
                fs.Close();
            }

            getFATFileFolderNames(sub_dir, info.RDET_start, info.fullpath, item);
        }

    }


}

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static OS_Project.Views.TreeView;

namespace OS_Project
{

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
       

        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFile;
            bool isExpanded;

            if (value is Node)
            {
                Node temp = (Node)value;
                isFile = temp.info.isFile;
                isExpanded = temp.info.isExpanded;
            } else
            {
                NodeInfo info = (NodeInfo)value;
                isFile = info.isFile;
                isExpanded = info.isExpanded;
            }

            string image = "images/open-folder.png";

            if (isFile)
            {
                image = "images/file.png";
            }
            else
            {
                if (!isExpanded)
                {
                    image = "images/folder.png";
                }
            }
            

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

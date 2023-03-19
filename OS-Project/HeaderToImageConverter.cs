﻿using System;
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
            NodeInfo info = (NodeInfo)value;
            
            bool isFile = info.isFile;
            bool isExpanded = info.isExpanded;

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

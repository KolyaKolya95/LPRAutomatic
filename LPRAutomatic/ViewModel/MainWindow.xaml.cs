using DatabaseLibrary.Management;
using DatabaseLibrary.Parsers;
using LPRAutomatic.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LPRAutomatic
{
    public partial class MainWindow : Window
    {
        private VideoLPRWindow _videoLPRWindow;
        private PhotoLPRWindow _photoLPRWindow;
        private UserManager _userManagement;
        private FileParser _fileParser;

        public MainWindow()
        {
            InitializeComponent();
            _fileParser = new FileParser();
            _userManagement = new UserManager();
            MainImage.Source = new BitmapImage(new Uri(@"D:\магістерська\project\LPRAutomatic\LPRAutomatic\Source\mainBackround.png", UriKind.RelativeOrAbsolute));
        }

        private void VideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _videoLPRWindow = new VideoLPRWindow();
            NavigationFrame.Navigate(_videoLPRWindow);
        }


        private void PhotoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (VideoLPRWindow.LocalWebCam != null)
                Dispatcher.Invoke((Action)(() => VideoLPRWindow.LocalWebCam.Stop()));

            _photoLPRWindow = new PhotoLPRWindow();

            NavigationFrame.Navigate(_photoLPRWindow);
        }
        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file";
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (openFileDialog.ShowDialog() == true)
            {
                var usersData = _fileParser.ParseXlsx(openFileDialog.FileName);
                if (usersData != null)
                    foreach (var user in usersData)
                        _userManagement.AddUser(user);
            }
        }
    }
}



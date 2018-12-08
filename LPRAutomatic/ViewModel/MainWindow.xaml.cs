using LPRAutomatic.ViewModel;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LPRAutomatic
{
    public partial class MainWindow : Window
    {
        private VideoLPRWindow _videoLPRWindow;
        private PhotoLPRWindow _photoLPRWindow;

        public MainWindow()
        {
            InitializeComponent();
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
    }
}



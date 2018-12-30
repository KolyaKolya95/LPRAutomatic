using DatabaseLibrary.Management;
using DatabaseLibrary.Model;
using DatabaseLibrary.Parsers;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using LPRAutomatic.Helper;
using LPRAutomatic.LPRCore;
using LPRAutomatic.Model;
using LPRAutomatic.Model.ModelView;
using LPRAutomatic.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace LPRAutomatic.ViewModel
{
    /// <summary>
    /// Interaction logic for PhotoLPRWindow.xaml
    /// </summary>
    public partial class PhotoLPRWindow : Page
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private LicensePlateDetector _licensePlateDetector;
        private FileParser _fileParser;

        private UserManager _userManagement;

        public PhotoLPRWindow()
        {
            InitializeComponent();
            _userManagement = new UserManager();
            _fileParser = new FileParser();
            _licensePlateDetector = new LicensePlateDetector(@"C:\Emgu\emgucv-windesktop_x64-cuda 3.1.0.2504\Emgu.CV.World\tessdata");
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a picture";
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FileNameTextBox.Text = openFileDialog.FileName;
                CarImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                LicensePlateModel licensePlate = GetLicensePlate(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(licensePlate.LicensePlate))
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        licensePlate.LicensePlate = CleaningLicensePlate.Cleaning(licensePlate.LicensePlate);
                        var user = _userManagement.GetUser(licensePlate.LicensePlate);

                        if (user != null)
                        {
                            InfoNumberListBox.Items.Add(UserHelper.ConverToLicensePlateUserModel(user));
                        }
                        else
                        {
                            user = new UserModel
                            {
                                LicensePlate = licensePlate.LicensePlate
                            };

                            InfoNumberListBox.Items.Add(UserHelper.ConverToLicensePlateUserModel(user));
                        }

                        TimaLabel.Content = licensePlate.Timer;
                        GuantityPlateLable.Content = licensePlate.GuantityPlateResult;

                        CarImage.Source = licensePlate.Image;
                        ImageOriginal.Source = licensePlate.ImageLicensePlate;

                        MemoryDictionaryHelper.AddLicensePlate(licensePlate);
                    }));
                }
            }
        }

        private void ShowUserInfo_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void InfoNumberListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LicensePlateUserModel listBoxItem = (LicensePlateUserModel)InfoNumberListBox.SelectedItem;
            if (listBoxItem != null)
            {
                LicensePlateModel licensePlate = MemoryDictionaryHelper.GetLicensePlate(listBoxItem.LicensePlate);
                if (licensePlate != null)
                {
                    CarImage.Source = licensePlate.Image;
                    ImageOriginal.Source = licensePlate.ImageLicensePlate;
                }
            }
        }

        private void EditUerButton_Click(object sender, RoutedEventArgs e)
        {
            LicensePlateUserModel listBoxItem = (LicensePlateUserModel)InfoNumberListBox.SelectedItem;
            var user = new UserModel();

            if (!string.IsNullOrEmpty(listBoxItem.UserName))
                user = _userManagement.GetUser(listBoxItem.LicensePlate);
            else
                user.LicensePlate = listBoxItem.LicensePlate;


            InfoUserWindow infoUserWindow = new InfoUserWindow(LicensePlateUserModel.ConvertToLincensePlateUserModel(user));

            if (infoUserWindow.ShowDialog() == true)
            {

            }
            else
            {
                int index = InfoNumberListBox.SelectedIndex;
                InfoNumberListBox.Items.RemoveAt(index);

                var licensePlate = InfoUserWindow.LicensePlateUserModel;

                InfoNumberListBox.Items.Insert(index, licensePlate);
            }
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #region Private

        private LicensePlateModel GetLicensePlate(string imageRoute)
        {

            Mat mate = new Mat(imageRoute);
            UMat uMat = mate.GetUMat(AccessType.ReadWrite);

            LicensePlateModel processResult = ProcessImage(mate);

            if (processResult.Points != null)
            {
                Bgra bgra = new Bgra(200, 200, 200, 200);
                Image<Bgra, Byte> newImg = new Image<Bgra, byte>(uMat.Bitmap);
                newImg.Draw(processResult.Points, bgra, 10);

                BitmapSource bs = ToBitmapSource(newImg);
                processResult.Image = bs;

                return processResult;
            }

            return new LicensePlateModel();
        }

        private LicensePlateModel ProcessImage(IInputOutputArray image)
        {
            LicensePlateModel licensePlateModel = new LicensePlateModel();

            Stopwatch watch = Stopwatch.StartNew();

            List<IInputOutputArray> licensePlateImagesList = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList = new List<RotatedRect>();
            List<string> words = _licensePlateDetector.DetectLicensePlate(
               image,
               licensePlateImagesList,
               filteredLicensePlateImagesList,
               licenseBoxList);

            watch.Stop();

            licensePlateModel.Timer = watch.Elapsed.TotalMilliseconds.ToString();
            licensePlateModel.GuantityPlateResult = words.Count;
            Point startPoint = new Point(10, 10);
            string numberLicense = string.Empty;
            for (int i = 0; i < words.Count; i++)
            {
                numberLicense = words[i].Replace(" ", string.Empty);

                numberLicense = new string(numberLicense.Where(c => !char.IsPunctuation(c)).ToArray());

                Mat dest = new Mat();
                CvInvoke.VConcat(licensePlateImagesList[i], filteredLicensePlateImagesList[i], dest);
                BitmapSource bso = ToBitmapSource(dest);
                licensePlateModel.ImageLicensePlate = bso;

                PointF[] verticesF = licenseBoxList[i].GetVertices();
                Point[] vertices = Array.ConvertAll(verticesF, Point.Round);
                licensePlateModel.Points = vertices;

                if (numberLicense.Length >= 8)
                {
                    licensePlateModel.LicensePlate = numberLicense;
                    return licensePlateModel;
                }
            }
            return licensePlateModel;
        }

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();
                BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage retval;

            try
            {
                retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        #endregion
    }
}

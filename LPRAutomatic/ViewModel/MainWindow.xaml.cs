using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using LPRAutomatic.Helper;
using LPRAutomatic.LPRCore;
using LPRAutomatic.Model;
using LPRAutomatic.ViewModel;
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
using System.Windows.Navigation;
using Point = System.Drawing.Point;

namespace LPRAutomatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private LicensePlateDetector _licensePlateDetector;

        #region WPFmethod

        public MainWindow()
        {
            InitializeComponent();
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
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = licensePlate.LicensePlate;
                    TimaLabel.Content = licensePlate.Timer;
                    GuantityPlateLable.Content = licensePlate.GuantityPlateResult;
                    InfoNumberListBox.Items.Add(itm);
                    CarImage.Source = licensePlate.Image;
                    ImageOriginal.Source = licensePlate.ImageLicensePlate;

                    MemoryDictionaryHelper.AddLicensePlate(licensePlate);
                }
            }
        }

        private void InfoNumberListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)InfoNumberListBox.SelectedItem;
            if (listBoxItem.Content != null)
            {
                LicensePlateModel licensePlate = MemoryDictionaryHelper.GetLicensePlate(listBoxItem.Content.ToString());
                if (licensePlate != null)
                {
                    CarImage.Source = licensePlate.Image;
                    ImageOriginal.Source = licensePlate.ImageLicensePlate;
                }
            }
        }

        #endregion

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            VideoLPRWindow videoLPRWindow = new VideoLPRWindow();
            NavigationFrame.Content = videoLPRWindow;

        }
    }
}



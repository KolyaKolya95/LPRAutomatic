using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using LPRAutomatic.Bll;
using LPRAutomatic.Helper;
using LPRAutomatic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LPRAutomatic.ViewModel
{
    /// <summary>
    /// Interaction logic for VideoLPRWindow.xaml
    /// </summary>
    public partial class VideoLPRWindow : Page
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static VideoCaptureDevice LocalWebCam;

        private LicensePlateDetectorInVideo _licensePlateDetectorInVideo;

        private Object _lock = new Object();

        public FilterInfoCollection LoaclWebCamsCollection;

        private Rectangle _rectangleOriginal;

        private CascadeClassifier cascadeClassifier;

        public VideoLPRWindow()
        {
            InitializeComponent();
           
            Loaded += MainWindow_Loaded;
            cascadeClassifier = new CascadeClassifier(@"D:\MyPrograms\C#\DotNet\WpfAppCamera\haarcascade_russian_plate_number.xml");
            _licensePlateDetectorInVideo = new LicensePlateDetectorInVideo(@"C:\Emgu\emgucv-windesktop_x64-cuda 3.1.0.2504\Emgu.CV.World\tessdata");
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoaclWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            LocalWebCam = new VideoCaptureDevice(LoaclWebCamsCollection[0].MonikerString);
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
           
            LocalWebCam.Start(); 
        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {

                    VideoImage.Source = bi;
                    Image<Bgr, Byte> imageCV = new Image<Bgr, Byte>(BitmapImage2Bitmap(bi));

                    Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(imageCV, 1.4, 0, new System.Drawing.Size(100, 100), new System.Drawing.Size(800, 800));

                    if (rectangles.Length > 0)
                    {
                        var rectanles = rectangles[0];

                        var diferentRectangels = DifferenceBetweenRectangles(rectanles, _rectangleOriginal);

                        if ((diferentRectangels.X != 0 ||  diferentRectangels.Y !=  0) && 
                            diferentRectangels.X > 40 && diferentRectangels.Y > 40 || (_rectangleOriginal.X == 0 && _rectangleOriginal.Y == 0))
                        {
                            _rectangleOriginal = rectanles;

                            imageCV.Draw(rectangles[0], new Bgr(0, 0, 255), 3);
                            VideoImage.Source = ToBitmapSource(imageCV);
                            imageCV.ROI = rectangles[0];

                            RecognizedPlateImage.Source = ToBitmapSource(imageCV);

                            Task.Run(() =>
                            {
                                lock (_lock)
                                {
                                    var licensePlate = ProcessImage(imageCV);
                                    if (licensePlate != null)
                                    {
                                        MemoryDictionaryHelper.AddLicensePlate(licensePlate);

                                        LicensePlateList.Dispatcher.BeginInvoke((Action)(() => { AddToListBox(); })); 
                                    }
                                    else
                                        _rectangleOriginal = new Rectangle();
                                }
                            });
                        }
                    }
                }));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void AddToListBox()
        {                      
            if (MemoryDictionaryHelper.Count() > 0 && 
                MemoryDictionaryHelper.LastIndex < MemoryDictionaryHelper.AddedLastIndex )
            {
                var itmBox = new ListBoxItem();
                itmBox.Content = MemoryDictionaryHelper.GetLatLicensePlate().LicensePlate;

                if (!LicensePlateList.Items.Contains(itmBox))
                {
                    LicensePlateList.Items.Add(itmBox);
                    MemoryDictionaryHelper.LastIndex = MemoryDictionaryHelper.AddedLastIndex;
                }
            }
            TaskScheduler.FromCurrentSynchronizationContext();
        }

        private LicensePlateModel ProcessImage(IInputOutputArray image)
        {
            List<IInputOutputArray> licensePlateImagesList = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList = new List<RotatedRect>();
            List<string> plates = _licensePlateDetectorInVideo.DetectLicensePlate(
               image,
               licensePlateImagesList,
               filteredLicensePlateImagesList,
               licenseBoxList);

            LicensePlateModel licensePlateModel = new LicensePlateModel();
            foreach (var plate in plates)
            {
                Debug.WriteLine($"Sosurce string: {plate}");
                var cleanPlate = CleaningLicensePlate.Cleaning(plate);
                if (cleanPlate != null)
                {

                    Debug.WriteLine($"Definit string: {plate}");

                    licensePlateModel.LicensePlate = cleanPlate;
                    licensePlateModel.CountFindItemLicensePlate = plates.Count;

                    return licensePlateModel;
                }
            }

            return null;
        }

        Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
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

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static (int X, int Y, int Height, int Width) DifferenceBetweenRectangles(Rectangle defRectangle, Rectangle originalRectangle)
        {
            (int X, int Y, int Height, int Width) result = (0 , 0, 0, 0);

            result.X = defRectangle.X - originalRectangle.X < 0 ? (defRectangle.X - originalRectangle.X) * -1 : defRectangle.X - originalRectangle.X;
            result.Y = defRectangle.Y - originalRectangle.Y < 0 ? (defRectangle.Y - originalRectangle.Y) * -1 : defRectangle.Y - originalRectangle.Y; 
            result.Height = defRectangle.Height - originalRectangle.Height;
            result.Width = defRectangle.Width - originalRectangle.Width;

            return result;
        }
    }
}

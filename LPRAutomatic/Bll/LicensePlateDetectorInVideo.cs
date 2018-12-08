using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace LPRAutomatic.Bll
{
    public class LicensePlateDetectorInVideo : DisposableObject
    {
        private Tesseract _ocr;


        public LicensePlateDetectorInVideo(String dataPath)
        {
            InitOcr(dataPath, "eng", OcrEngineMode.TesseractLstmCombined);
            _ocr.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ-1234567890");
        }

        private void InitOcr(String path, String lang, OcrEngineMode mode)
        {
            try
            {
                if (_ocr != null)
                {
                    _ocr.Dispose();
                    _ocr = null;
                }

                if (String.IsNullOrEmpty(path))
                    path = ".";

                String pathFinal = path.Length == 0 ||
                                   path.Substring(path.Length - 1, 1).Equals(Path.DirectorySeparatorChar.ToString())
                    ? path
                    : String.Format("{0}{1}", path, Path.DirectorySeparatorChar);

                _ocr = new Tesseract(pathFinal, lang, mode);
            }
            catch (WebException e)
            {
                _ocr = null;
                throw new Exception("Unable to download tesseract lang file. Please check internet connection.", e);
            }
            catch (Exception e)
            {
                _ocr = null;
                throw e;
            }
        }

        public List<String> DetectLicensePlate(
           IInputArray img,
           List<IInputOutputArray> licensePlateImagesList,
           List<IInputOutputArray> filteredLicensePlateImagesList,
           List<RotatedRect> detectedLicensePlateRegionList)
        {
            List<String> licenses = new List<String>();
            using (Mat gray = new Mat())
            using (Mat canny = new Mat())
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);
                CvInvoke.Canny(gray, canny, 100, 50, 3, false);
                int[,] hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);

                if (true)
                {
                    FindLicensePlate(contours, hierachy, 0, gray, canny, licenses);
                }
                
            }
            return licenses;
        }

        private void FindLicensePlate(
        VectorOfVectorOfPoint contours, int[,] hierachy, int idx, Mat gray, IInputArray canny, List<String> licenses)
        {
            for (; idx >= 0; idx = hierachy[idx, 0])
            {
                using (VectorOfPoint contour = contours[idx])
                {
                    if (CvInvoke.ContourArea(contour) > 400)
                    {
                        using (UMat plate = gray.GetUMat(AccessType.ReadWrite))
                        {
                            UMat filteredPlate = FilterPlate(plate);

                            StringBuilder strBuilder = new StringBuilder();
                            using (UMat tmp = filteredPlate.Clone())
                            {
                                _ocr.SetImage(tmp);
                                _ocr.Recognize();

                                strBuilder.Append(_ocr.GetUTF8Text());
                            }

                            licenses.Add(strBuilder.ToString());
                        }
                    }
                }
            }
        }

        private static UMat FilterPlate(UMat plate)
        {
            UMat thresh = new UMat();
            CvInvoke.Threshold(plate, thresh, 120, 255, ThresholdType.BinaryInv);

            Size plateSize = plate.Size;
            using (Mat plateMask = new Mat(plateSize.Height, plateSize.Width, DepthType.Cv8U, 1))
            using (Mat plateCanny = new Mat())
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                plateMask.SetTo(new MCvScalar(255.0));
                CvInvoke.Canny(plate, plateCanny, 100, 50);
                CvInvoke.FindContours(plateCanny, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contour);
                        if (rect.Height > (plateSize.Height >> 1))
                        {
                            rect.X -= 1; rect.Y -= 1; rect.Width += 2; rect.Height += 2;
                            Rectangle roi = new Rectangle(Point.Empty, plate.Size);
                            rect.Intersect(roi);
                            CvInvoke.Rectangle(plateMask, rect, new MCvScalar(), -1);
                        }
                    }

                }

                thresh.SetTo(new MCvScalar(), plateMask);
            }

            CvInvoke.Erode(thresh, thresh, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Dilate(thresh, thresh, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            return thresh;
        }

        protected override void DisposeObject()
        {
            _ocr.Dispose();
        }
    }
}

﻿using System.Drawing;
using System.Windows.Media;

namespace LPRAutomatic.Model
{
    public class LicensePlateModel
    {
        public string NameImage { get; set; }

        public string LicensePlate { get; set; }

        public Point[] Points { get; set; }

        public ImageSource Image { get; set; }

        public ImageSource ImageLicensePlate { get; set; }

        public int GuantityPlateResult { get; set; }

        public string Timer { get; set; }

        public int CountFindItemLicensePlate { get; set; }
    }
}

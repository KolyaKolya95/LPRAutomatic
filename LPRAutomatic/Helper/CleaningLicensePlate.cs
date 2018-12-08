using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LPRAutomatic.Helper
{
    public static class CleaningLicensePlate
    {

        public static string Cleaning(string plate)
        {
            string licensePlate = string.Empty;

            plate = plate.Replace("\r\n", string.Empty);
            plate = plate.Replace(" ", "");

            Regex regex = new Regex(@"^(?<city>([A-Z]{1,3})(?<number>([0-9]{1,4}))(?<servicNumber>([A-Z]{1,3})))");

            MatchCollection matches = regex.Matches(plate);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    licensePlate = licensePlate + match.Value;
            }

            if (licensePlate.Length > 0)
                return licensePlate;
            else
                return null;
        }
    }
}

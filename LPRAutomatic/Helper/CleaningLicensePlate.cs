using System.Text.RegularExpressions;

namespace LPRAutomatic.Helper
{
    public static class CleaningLicensePlate
    {
        public static string Cleaning(string plate)
        {
            string licensePlate = string.Empty;

            plate = plate.Replace("\r\n", string.Empty);
            plate = plate.Replace(" ", "");

            Regex regex = new Regex(@"^((?<city>([A-Z]{1,3}))(?<number>([0-9]{1,4}))(?<servicNumber>([A-Z]{1,3})))");


            string city = string.Empty;
            string number = string.Empty;
            string servicNumber = string.Empty;

            Match match = regex.Match(plate);
            if (match.Success)
            {
                city = match.Groups["city"].Value;
                city = city.Length > 2 ? city.Remove(city.Length - 1) : city;

                number = match.Groups["number"].Value;

                servicNumber = match.Groups["servicNumber"].Value;
                servicNumber = servicNumber.Length > 2 ? servicNumber.Remove(servicNumber.Length - 1) : servicNumber;
            }

            licensePlate = city + number + servicNumber;

            //MatchCollection matches = regex.Matches(plate);
            //if (matches.Count > 0)
            //{
            //    foreach (Match match in matches)
            //        licensePlate = licensePlate + match.Value;
            //}

            if (licensePlate.Length > 0)
                return licensePlate;
            else
                return null;
        }
    }
}

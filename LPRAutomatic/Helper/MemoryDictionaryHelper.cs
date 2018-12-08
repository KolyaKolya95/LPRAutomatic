using LPRAutomatic.Model;
using System.Collections.Generic;
using System.Linq;

namespace LPRAutomatic.Helper
{
    public static class MemoryDictionaryHelper
    {
        private static Dictionary<string, LicensePlateModel> _infoLicensePlateModels = new Dictionary<string, LicensePlateModel>();

        public static int LastIndex = 0;

        public static int AddedLastIndex { get { return _infoLicensePlateModels.Count; } }

        public static LicensePlateModel GetLicensePlate(string name)
        {
            if (_infoLicensePlateModels.TryGetValue(name, out var licensePlateModel))
                return licensePlateModel;
            else
                return null;
        }

        public static void AddLicensePlate(LicensePlateModel licensePlate)
        {
            if (!string.IsNullOrEmpty(licensePlate.LicensePlate))
                _infoLicensePlateModels[licensePlate.LicensePlate] = licensePlate;
        }

        public static int Count()
        {
            return _infoLicensePlateModels.Values.Count;
        }

        public static LicensePlateModel GetLatLicensePlate()
        {
            return _infoLicensePlateModels.Values.Last();
        }
    }
}

using LPRAutomatic.Model;
using System.Collections.Generic;

namespace LPRAutomatic.Helper
{
    public static class MemoryDictionaryHelper
    {
        private static IDictionary<string, LicensePlateModel> _infoLicensePlateModels = new Dictionary<string, LicensePlateModel>();

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
    }
}

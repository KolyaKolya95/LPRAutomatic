using LPRAutomatic.Common.Model;

namespace LPRAutomatic.Common.Core
{
    /// <summary>
    /// Interface for implement logic for search license plate.
    /// </summary>
    public interface IProcessingLicensePlate
    {
        /// <summary>
        /// Get license plate from with image.
        /// </summary>
        /// <param name="imageRoute"></param>
        /// <returns></returns>
        LicensePlateModel GetLicensePlate(string imageRoute);
    }
}

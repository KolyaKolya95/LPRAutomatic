using LPRAutomatic.Model;

namespace LPRAutomatic.Interfaces
{
    public interface IProcessingLicensePlate
    {
        LicensePlateModel GetLicensePlate(string imageRoute);
    }
}

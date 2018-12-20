using DatabaseLibrary.Model;
using LPRAutomatic.Model.ModelView;

namespace LPRAutomatic.Helper
{
    public static class UserHelper
    {
        public static LicensePlateUserModel ConverToLicensePlateUserModel(UserModel userModel)
        {
            LicensePlateUserModel lincensePlateUserModel = new LicensePlateUserModel();

            lincensePlateUserModel.LicensePlate = userModel.LicensePlate;
            lincensePlateUserModel.UserName = userModel.Name;
            lincensePlateUserModel.Address = userModel.Address;
            lincensePlateUserModel.Email = userModel.Email;
 
            return lincensePlateUserModel;
        }

        public static UserModel ConverToUserModel(LicensePlateUserModel licensePlateUser)
        {
            UserModel userModel = new UserModel();

            userModel.Name = licensePlateUser.UserName;
            userModel.LicensePlate = licensePlateUser.LicensePlate;
            userModel.Address = licensePlateUser.Address;
            userModel.Email = licensePlateUser.Email;

            return userModel;
        }

    }
}

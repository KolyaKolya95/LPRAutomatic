using DatabaseLibrary.Model;

namespace LPRAutomatic.Model.ModelView
{
    public class LicensePlateUserModel
    {
        public string LicensePlate { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public override string ToString()
        {
            return $"User name: {UserName}, Licesne Plate: {LicensePlate}";
        }

        public static LicensePlateUserModel ConvertToLincensePlateUserModel(UserModel userModel)
        {
            return new LicensePlateUserModel()
            {
                            Email = userModel.Email,
                            LicensePlate = userModel.LicensePlate,
                            UserName = userModel.Name,
                            Address = userModel.Address
                            
            };
        }

        public static UserModel ConvertToUserModel(LicensePlateUserModel licensePlateUserModel)
        {
            return new UserModel()
            {
                Email = licensePlateUserModel.Email,
                LicensePlate = licensePlateUserModel.LicensePlate,
                Name = licensePlateUserModel.UserName,
                Address = licensePlateUserModel.Address
            };
        }
    }
}

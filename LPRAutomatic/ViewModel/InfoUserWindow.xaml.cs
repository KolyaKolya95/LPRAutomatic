using DatabaseLibrary.Management;
using DatabaseLibrary.Model;
using LPRAutomatic.Helper;
using LPRAutomatic.Model.ModelView;
using System.Windows;

namespace LPRAutomatic.ViewModel
{
    /// <summary>
    /// Interaction logic for InfoUserWindow.xaml
    /// </summary>
    public partial class InfoUserWindow : Window
    {
        public static LicensePlateUserModel LicensePlateUserModel;
        private readonly UserManager _userManager;

        public InfoUserWindow(LicensePlateUserModel licensePlateUser)
        {
            InitializeComponent();

            LicensePlateUserModel = licensePlateUser;
            _userManager = new UserManager();

            UserNameTextBox.Text = licensePlateUser.UserName;
            LicensePlateTextBox.Text = licensePlateUser.LicensePlate;
            AddressTextBox1.Text = licensePlateUser.Address;
            EmailTextBox.Text = licensePlateUser.Email;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            LicensePlateUserModel.UserName = UserNameTextBox.Text;
            LicensePlateUserModel.LicensePlate = LicensePlateTextBox.Text;
            LicensePlateUserModel.Address = AddressTextBox1.Text;
            LicensePlateUserModel.Email = EmailTextBox.Text;
            _userManager.SaveOrUpdate(UserHelper.ConverToUserModel(LicensePlateUserModel));
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

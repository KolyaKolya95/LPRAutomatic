using DatabaseLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseLibrary.Management
{
    public class UserManager
    {
        private UserRepository _userRepository;

        public UserManager()
        {
            _userRepository = new UserRepository();
        }

        public List<UserModel> GetUsers()
        {
            var result = new List<UserModel>();

            result = _userRepository.Users.ToList();

            return result;  
        }

        public UserModel GetUser(string licensePlte)
        {
            if (string.IsNullOrEmpty(licensePlte))
                return null;

            Regex regex = new Regex(@"^((?<city>([A-Z]{1,3}))(?<number>([0-9]{1,4}))(?<servicNumber>([A-Z]{1,3})))");

            string city = string.Empty;
            string number = string.Empty;
            string servicNumber = string.Empty;

     
            Match match = regex.Match(licensePlte);
            if (match.Success)
            {
                city = match.Groups["city"].Value;
                city = city.Length > 2 ? city.Remove(city.Length - 1) : city;

                number = match.Groups["number"].Value;
                
                servicNumber = match.Groups["servicNumber"].Value;
                servicNumber = servicNumber.Length > 2 ? servicNumber.Remove(servicNumber.Length - 1) : servicNumber;
            }

            var users = GetUsers();


            foreach (var user in users)
            {
                var b = user.LicensePlate.Contains(city);

                if (user.LicensePlate.IndexOf(city) > -1)
                    if (user.LicensePlate.IndexOf(number, StringComparison.OrdinalIgnoreCase) > -1)
                        if (user.LicensePlate.IndexOf(number, StringComparison.OrdinalIgnoreCase) > -1)
                            return user;
            }
            return null;
                
        }

        public void AddUser(UserModel user)
        {
            if (user != null)
            {
               var userFind = _userRepository.Users.ToList().Find(u => u.LicensePlate == user.LicensePlate);
                if (userFind == null)
                {
                    _userRepository.Users.Add(user);
                    _userRepository.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(UserModel user)
        {
            if (user != null)
            {
                var userFind = _userRepository.Users.ToList().Find(u => u.LicensePlate == user.LicensePlate);
                if (userFind == null)
                {
                    _userRepository.Users.Add(user);
                    _userRepository.SaveChanges();
                }
                else
                {
                    _userRepository.Users.Remove(userFind);
                    userFind = user;
                    _userRepository.Users.Add(userFind);
                    _userRepository.SaveChanges();
                }
            }
        }

        public void ClearAll()
        {
            var users = _userRepository.Users;
            foreach (var user in users)
                _userRepository.Users.Remove(user);

            _userRepository.SaveChanges();
        }
    }
}

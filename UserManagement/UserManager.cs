using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UserManagement
{
    public class UserManager : IUserManager
    {
        public User CreateUser(string name, string email, string address, string phone, string userType, decimal money)
        {
            var newUser = new User
            {
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = money
            };

            return newUser;
        }

        public string ValidateInput(string name, string email, string address, string phone)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(name))
            {
                AddIsRequiredErrorMessage(errors, "name");
            }

            if (string.IsNullOrEmpty(email))
            {
                AddIsRequiredErrorMessage(errors, "email");
            }

            if (string.IsNullOrEmpty(address))
            {
                AddIsRequiredErrorMessage(errors, "address");
            }

            if (string.IsNullOrEmpty(phone))
            {
                AddIsRequiredErrorMessage(errors, "phone");
            }

            return string.Join(" ", errors);
        }

        public void NormalizeEmail(IUser user)
        {
            var emailAsArray = user.Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var username = emailAsArray[0];
            var domain = emailAsArray[1];

            username = username.Replace(".", "");

            var atIndex = username.IndexOf("+", StringComparison.Ordinal);

            if (atIndex >= 0)
            {
                username = username.Remove(atIndex);
            }

            var email = string.Join("@", new string[] { username, domain });

            user.Email = email;
        }

        public async Task GetUsersFromFile(string path, List<IUser> users)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (var reader = new StreamReader(fileStream))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var user = new User
                    {
                        Name = line.Split(',')[0].ToString(),
                        Email = line.Split(',')[1].ToString(),
                        Phone = line.Split(',')[2].ToString(),
                        Address = line.Split(',')[3].ToString(),
                        UserType = line.Split(',')[4].ToString(),
                        Money = decimal.Parse(line.Split(',')[5].ToString()),
                    };

                    users.Add(user);
                }
            }
        }

        public bool IsUserDuplicated(IUser newUser, List<IUser> users)
        {
            var isDuplicated = false;

            foreach (var user in users)
            {
                bool duplicatedEmailOrPhone = user.Email == newUser.Email || user.Phone == newUser.Phone;
                bool duplicatedNameAndAddress = user.Name == newUser.Name && user.Address == newUser.Address;

                if (duplicatedEmailOrPhone || duplicatedNameAndAddress)
                {
                    isDuplicated = true;
                }

                if (duplicatedNameAndAddress)
                {
                    throw new Exception("User is duplicated");
                }
            }

            return isDuplicated;
        }

        private void AddIsRequiredErrorMessage(List<string> errors, string fieldName)
        {
            errors.Add($"The {fieldName} is required");
        }

        public void AddUserGif(IUser user)
        {
            decimal percentage = GetPercentage(user.UserType, user.Money);

            var gif = user.Money * percentage;
            user.Money = user.Money + gif;
        }

        private decimal GetPercentage(string type, decimal amount)
        {
            decimal percentage = 0;

            switch (type)
            {
                case "Normal":
                    if (amount > 100)
                    {
                        percentage = Convert.ToDecimal(0.12);
                    }
                    else if (amount < 100 && amount > 10)
                    {
                        percentage = Convert.ToDecimal(0.8);
                    }

                    break;

                case "SuperUser":
                    if (amount > 100)
                    {
                        percentage = Convert.ToDecimal(0.20);
                    }

                    break;

                case "Premium":
                    if (amount > 100)
                    {
                        percentage = 2;
                    }

                    break;
            }

            return percentage;
        }
    }
}

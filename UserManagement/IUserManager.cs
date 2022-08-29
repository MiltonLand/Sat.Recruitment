using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserManager
    {
        User CreateUser(string name, string email, string address, string phone, string userType, decimal money);
        void AddUserGif(IUser user);
        string ValidateInput(string name, string email, string address, string phone);
        void NormalizeEmail(IUser user);
        public Task GetUsersFromFile(string path, List<IUser> users);
        bool IsUserDuplicated(IUser newUser, List<IUser> users);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UserManagement;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly List<IUser> _users = new List<IUser>();
        private readonly IUserManager _manager;
        public UsersController(IUserManager manager)
        {
            _manager = manager;
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var errors = _manager.ValidateInput(name, email, address, phone);

            if (!string.IsNullOrEmpty(errors))
            {
                return new Result()
                {
                    IsSuccess = false,
                    Errors = errors
                };
            }

            var newUser = _manager.CreateUser(name, email, address, phone, userType, decimal.Parse(money));

            _manager.AddUserGif(newUser);

            _manager.NormalizeEmail(newUser);

            await _manager.GetUsersFromFile(Directory.GetCurrentDirectory() + "/Files/Users.txt", _users);

            try
            {
                if (_manager.IsUserDuplicated(newUser, _users))
                {
                    Debug.WriteLine("The user is duplicated");

                    return new Result()
                    {
                        IsSuccess = false,
                        Errors = "The user is duplicated"
                    };
                }
            }
            catch
            {
                Debug.WriteLine("The user is duplicated");
                return new Result()
                {
                    IsSuccess = false,
                    Errors = "The user is duplicated"
                };
            }

            Debug.WriteLine("User Created");

            return new Result()
            {
                IsSuccess = true,
                Errors = "User Created"
            };
        }
    }
}

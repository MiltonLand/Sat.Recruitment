using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UserManagement;
using Xunit;

namespace Sat.Recruitment.Test
{
    public class UserManagerTests
    {
        private UserManager _manager;

        public UserManagerTests()
        {
            _manager = new UserManager();
        }

        [Theory]
        [InlineData(50, "Normal", 90)]
        [InlineData(40, "Normal", 72)]
        [InlineData(150, "Normal", 168)]
        [InlineData(99, "Normal", 178.2)]
        [InlineData(100, "Normal", 100)]
        [InlineData(101, "Normal", 113.12)]
        [InlineData(5, "Normal", 5)]
        [InlineData(0, "Normal", 0)]
        [InlineData(100, "SuperUser", 100)]
        [InlineData(200, "SuperUser", 240)]
        [InlineData(400, "SuperUser", 480)]
        [InlineData(0, "Premium", 0)]
        [InlineData(50, "Premium", 50)]
        [InlineData(100, "Premium", 100)]
        [InlineData(200, "Premium", 600)]
        [InlineData(130, "Premium", 390)]
        public void AddUserGif_ShouldModifyMoney(decimal money, string userType, decimal expectedMoney)
        {
            var actual = new User()
            {
                Name = "Mike",
                Email = "mike@gmail.com",
                Address = "Fake 123",
                Phone = "211531453",
                UserType = userType,
                Money = money
            };

            _manager.AddUserGif(actual);

            Assert.Equal(expectedMoney, actual.Money);
        }

        [Theory]
        [InlineData("", "", "", "", "The name is required The email is required The address is required The phone is required")]
        [InlineData("", null, "", null, "The name is required The email is required The address is required The phone is required")]
        [InlineData("mike", null, "", "51351354", "The email is required The address is required")]
        [InlineData("", "asd", "asdasd", "asdasdasd", "The name is required")]
        [InlineData("", "", "asdasd", "asdasdasd", "The name is required The email is required")]
        [InlineData("mike", "asd", "asdasd", "asdasdasd", "")]
        public void ValidateInput_ShouldReturnErrorString(string name, string email, string address, string phone, string expectedErrors)
        {
            var expected = expectedErrors;

            var actual = _manager.ValidateInput(name, email, address, phone);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("m.i.k.e@gmail.com", "mike@gmail.com")]
        [InlineData("m.i.+k.e@gmail.com", "mi@gmail.com")]
        [InlineData("m.+.ike@gmail.com", "m@gmail.com")]
        [InlineData("m.i.k.+e@gmail.com", "mik@gmail.com")]
        [InlineData("+m.i.k.+e@gmail.com", "@gmail.com")]
        [InlineData("m.i.k.e+@gmail.com", "mike@gmail.com")]
        public void NormalizeEmail_ShouldReturnErrorString(string email, string expectedEmail)
        {
            var actual = new User()
            {
                Name = "Mike",
                Email = email,
                Address = "Fake 123",
                Phone = "211531453",
                UserType = "Normal",
                Money = 0
            };

            _manager.NormalizeEmail(actual);

            Assert.Equal(expectedEmail, actual.Email);
        }

        [Fact]
        public void IsUserDuplicated_ShouldReturnBoolean()
        {
            //Arrange
            var userList = new List<IUser>();

            userList.Add(new User()
            {
                Name = "Juan",
                Email = "Juan@marmol.com",
                Address = "Peru 2464",
                Phone = "+5491154762312",
                UserType = "Normal",
                Money = 1234
            });

            userList.Add(new User()
            {
                Name = "Franco",
                Email = "Franco.Perez@gmail.com",
                Address = "Alvear y Colombres",
                Phone = "+534645213542",
                UserType = "Premium",
                Money = 112234
            });

            userList.Add(new User()
            {
                Name = "Agustina",
                Email = "Agustina@gmail.com",
                Address = "Garay y Otra Calle",
                Phone = "+534645213542",
                UserType = "SuperUser",
                Money = 112234
            });

            var user1 = new User()
            {
                Name = "Agustina",
                Email = "mike@gmail.com",
                Address = "Garay y Otra Calle",
                Phone = "211531453",
                UserType = "Normal",
                Money = 0
            };

            var user2 = new User()
            {
                Name = "Mike",
                Email = "mike@gmail.com",
                Address = "Garay y Otra Calle",
                Phone = "211531453",
                UserType = "Normal",
                Money = 0
            };
            
            var user3 = new User()
            {
                Name = "Mike",
                Email = "mike@gmail.com",
                Address = "Fake 123",
                Phone = "+534645213542",
                UserType = "Normal",
                Money = 0
            };

            var user4 = new User()
            {
                Name = "Mike",
                Email = "Franco.Perez@gmail.com",
                Address = "Fake 123",
                Phone = "211531453",
                UserType = "Normal",
                Money = 0
            };

            //Act
            bool actual2 = _manager.IsUserDuplicated(user2, userList);
            bool actual3 = _manager.IsUserDuplicated(user3, userList);
            bool actual4 = _manager.IsUserDuplicated(user4, userList);

            //Assert
            Assert.Throws<Exception>(() => _manager.IsUserDuplicated(user1, userList));
            Assert.False(actual2);
            Assert.True(actual3);
            Assert.True(actual4);
        }
    }
}

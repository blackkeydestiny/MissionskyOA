using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using MissionskyOA.Services;
using MissionskyOA.Models;

namespace MissionskyOA.Api.Tests
{
    [TestFixture]
    public class UserControllerTest
    {
        private Mock<IUserService> _userServiceMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
        }

        [Test]
        public void GetLeaders()
        {
            // 1. mock empty leaders
            _userServiceMock.Setup(s => s.GetLeaders()).Returns(new List<UserModel>());

            var leaders = _userServiceMock.Object.GetLeaders();

            Assert.IsNotNull(leaders);
            Assert.AreEqual(leaders.Count, 0);

            // 2. mock return 2 leaders
            _userServiceMock.Setup(s => s.GetLeaders()).Returns(new List<UserModel>
            {
                new UserModel { DeptId = 1, EnglishName = "Leader A"},
                new UserModel { DeptId = 2, EnglishName = "Leader B"}
            });

            leaders = _userServiceMock.Object.GetLeaders();

            Assert.IsNotNull(leaders);
            Assert.AreEqual(2, leaders.Count);
            Assert.IsNotNull(leaders[0]);
            Assert.AreEqual(1, leaders[0].DeptId);
            Assert.AreEqual("Leader A", leaders[0].EnglishName);
        }

    }
}

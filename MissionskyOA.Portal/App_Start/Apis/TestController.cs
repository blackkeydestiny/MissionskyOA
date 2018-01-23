
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Portal.Apis
{
    public class TestController : ApiController
    {
        private IUserService UserService { get; set; }
        public TestController(IUserService userService)
        {
            this.UserService = userService;
        }

        [HttpGet]
        public ApiResponse<UserModel> Detail(int id)
        {
            var response = new ApiResponse<UserModel>()
            {
                Result = new UserModel()
                {
                    EnglishName = "Kevin",
                }
            };

            return response;
        }
    }
}

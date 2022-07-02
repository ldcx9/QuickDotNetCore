using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuickDotNetCore.Src.Config;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Utils;
using QuickDotNetCore.Src.vo;
using System.Reflection;
using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.Services.User;
using System.Collections.Generic;
using QuickDotNetCore.Src.DAO.User;
using System.Security.Claims;

namespace QuickDotNetCore.Src.Controllers
{
    /// <summary>
    /// 用户操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroupSort(999)]
    public class AuthController : ControllerBase
    {

        private readonly IUserService userService;
        
        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public BaseResponse<UserVO> Login(string userName, string pwd)
        {
            return userService.Login(userName,pwd);
        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userDO"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public BaseResponse<UserVO> Register(UserDO userDO) {
            return userService.Register(userDO);
        }


        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("FindByUserName")]
        public BaseResponse<UserVO> FindSerByName(string userName) {
            UserVO userVO = userService.ConvetToUserVO(userService.FindUserByUserName(userName));
            return BaseResponse<UserVO>.Success(userVO);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/GetUserInfo")]
        [Authorize]
        [ApiGroupSort(99)]
        public BaseResponse<UserVO> GetUserInfo()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long id = long.Parse(userId);
            UserVO userVO = userService.FindUserByUserId(id);
            return BaseResponse<UserVO>.Success(userVO);
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/GetAllUser")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "super")]
        [ApiGroupSort(99)]
        public BaseResponse<List<UserDO>> GetAllUser()
        {
            return BaseResponse<List<UserDO>>.Success(userService.Mapper<UserDAO>().All());
        }
    }
}

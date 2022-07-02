using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.Config;
using QuickDotNetCore.Src.DAO;
using QuickDotNetCore.Src.DAO.User;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Utils;
using QuickDotNetCore.Src.vo;
using SqlSugar;

namespace QuickDotNetCore.Src.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserDAO userDAO;
        private readonly ISqlSugarClient dbContext;


        public UserService(ISqlSugarClient dbContext, IUserDAO userDAO)
        {
            this.dbContext = dbContext;
            this.userDAO = userDAO;
        }

        public D Mapper<D>()
        {
            return (D)userDAO;
        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userDO">用户对象(ps:id填不填都可以 下面会被重新计算)</param>
        /// <param name="tryTime">尝试次数(以防万一)</param>
        /// <returns></returns>
        public BaseResponse<UserVO> Register(UserDO userDO,int tryTime = 10) {
            if (string.IsNullOrWhiteSpace(userDO.Password)
                || string.IsNullOrWhiteSpace(userDO.UserName)
                || string.IsNullOrWhiteSpace(userDO.Email))
            {
                return BaseResponse<UserVO>.Fail("参数错误");
            }
           
            if (null != FindUserByUserName(userDO.UserName))
            {
                return BaseResponse<UserVO>.Fail("用户名已存在!");
            }
            //必须先设置ID才能对密码加密  可以理解为  这就是所谓的盐   当然  也可以把其他东西当盐
            userDO.Id = userDAO.GetLastId() + 1;
            try
            {
                userDO.Password = UserPasswordEncryptUtil.Encrypt(userDO);
                userDO = userDAO.Insert(userDO);
            }
            catch (Exception)
            {
                if (tryTime > 0)
                {
                    return Register(userDO, --tryTime);
                }
                else {
                    userDO = null;
                }
            }
            if (userDO == null)
            {
                return BaseResponse<UserVO>.Fail("注册失败,请稍后再试!");
            }
            JwtSecurityToken token = JwtUtils.CreateToken(userDO);
            UserVO userVO = ConvetToUserVO(userDO);
            userVO.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return BaseResponse<UserVO>.Success(userVO);
        }

        public BaseResponse<UserVO> Login(string userName, string pwd)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pwd))
            {
                UserDO user = dbContext.Queryable<UserDO>().Where(u => u.UserName.Equals(userName)).First();
                if (user == null)
                {
                    return BaseResponse<UserVO>.Create(Enums.ResponseCodesEnum.AUTH_ERROR, "用户不存在", null);
                }
                if (!UserPasswordEncryptUtil.Encrypt(pwd + pwd).Equals(user.Password))
                {
                    return BaseResponse<UserVO>.Fail("密码错误!");
                }
                JwtSecurityToken token = JwtUtils.CreateToken(user);
                UserVO userVO = ConvetToUserVO(user);
                userVO.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return BaseResponse<UserVO>.Success(userVO);
            }
            else
            {
                return BaseResponse<UserVO>.Fail("用户名和密码都不能为空!");
            }
        }


        public UserVO FindUserByUserId(long userId)
        {
            return ConvetToUserVO(dbContext.Queryable<UserDO>().Where(u => u.Id == userId).First());
        }


        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserDO FindUserByUserName(string userName)
        {
            return userDAO.FindUserByUserName(userName);
        }

        public UserVO ConvetToUserVO(UserDO userDO)
        {
            if (userDO == null)
            {
                return null;
            }
            return JsonUtil.DeserializeToObject<UserVO>(JsonUtil.SerializeToString(userDO));
        }
    }
}

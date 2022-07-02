using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.DAO;
using QuickDotNetCore.Src.DAO.User;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Utils;
using QuickDotNetCore.Src.vo;

namespace QuickDotNetCore.Src.Services.User
{
    [Service(typeof(UserService))]
    public interface IUserService
    {
        D Mapper<D>();
        UserDO FindUserByUserName(string userName);
        UserVO ConvetToUserVO(UserDO userDO);

        BaseResponse<UserVO> Register(UserDO userDO,int tryTime = 10);

        BaseResponse<UserVO> Login(string userName, string pwd);

        UserVO FindUserByUserId(long userId);
    }
}

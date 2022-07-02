using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.Models;

namespace QuickDotNetCore.Src.DAO.User
{
    [DAO(typeof(UserDAO))]
    public interface IUserDAO:IDAO<UserDO>
    {
        UserDO FindUserByUserName(string userName);

        long GetLastId();
    }
}

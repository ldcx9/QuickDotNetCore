using System.Linq;
using QuickDotNetCore.Src.Models;
using SqlSugar;

namespace QuickDotNetCore.Src.DAO.User
{
    public class UserDAO:BaseDAO<UserDO>,IUserDAO
    {
        private readonly ISqlSugarClient dbContext;
        public UserDAO(ISqlSugarClient dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public UserDO FindUserByUserName(string userName)
        {
            return FindAll(u => u.UserName.Equals(userName)).FirstOrDefault();
        }

        public long GetLastId()
        {
           return dbContext.Queryable<UserDO>().Count();
        }
    }
}

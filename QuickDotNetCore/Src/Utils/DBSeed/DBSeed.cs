using QuickDotNetCore.Src.Config;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Utils.DBSeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace QuickDotNetCore.Src.Utils.BDSeed
{
    public class DBSeed
    {
        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <returns></returns>
        public static void Seed(string connectionString, params Type[] types)
        {
            try
            {
                DbSeedContext.Init(connectionString);
                DbSeedContext myContext = new();
                // 注意！一定要手动先创建要给空的数据库
                // 会覆盖，可以设置为true，来备份数据
                // 如果生成过了，第二次，就不用再执行一遍了,注释掉该方法即可
                myContext.CreateTableByEntity(false, types);
            }
            catch (Exception ex)
            {
                throw new Exception("1、注意要先创建空的数据库\n2、" + ex.Message);
            }
        }
    }


}

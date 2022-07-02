using QuickDotNetCore.Src.Config;
using QuickDotNetCore.Src.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.DAO.TestData
{
    public class TestDataDAO:BaseDAO<TestDataDO>,ITestDataDAO
    {
        public TestDataDAO(ISqlSugarClient dbContext) : base(dbContext)
        {

        }
    }
}

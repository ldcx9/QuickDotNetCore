using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.DAO.TestData
{
    [DAO(typeof(TestDataDAO))]
    public interface ITestDataDAO: IDAO<TestDataDO>
    {
    }
}

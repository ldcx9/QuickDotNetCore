using QuickDotNetCore.Src.Attributrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Services.TestData
{
    [Service(typeof(TestDataService))]
    public interface ITestDataService
    {
        void AddRandomData(int count);
    }
}

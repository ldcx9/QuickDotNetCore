using QuickDotNetCore.Src.DAO.TestData;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Services.TestData
{
    public class TestDataService : ITestDataService
    {
        private readonly ITestDataDAO testDataDAO;
        public TestDataService(ITestDataDAO testDataDAO)
        {
            this.testDataDAO = testDataDAO;
        }
        public void AddRandomData(int count) {
            long startId = testDataDAO.Count == 0 ? 1 : testDataDAO.All().Max(d => d.Id) + 1;
            List<TestDataDO> testDataDOs = new List<TestDataDO>();
            for (int i = 0; i < count; i++) {
                testDataDOs.Add(new TestDataDO() {
                    Id = startId + i,
                    Des = new Random().Next(10000000,99999999).ToString()
                });
            }
            testDataDAO.InsertRange(testDataDOs);
        }
    }
}

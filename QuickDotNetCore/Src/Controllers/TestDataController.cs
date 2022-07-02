using Microsoft.AspNetCore.Mvc;
using QuickDotNetCore.Src.DAO.TestData;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.Services.TestData;
using QuickDotNetCore.Src.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




namespace QuickDotNetCore.Src.Controllers
{

    /// <summary>
    /// 数据库测试
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroupSort(999)]
    public class TestDataController
    {
        /**
         * 
         * 注:本类为测试类
         * 
         */
        private readonly ITestDataService testDataService;
        private readonly ITestDataDAO testDataDAO;
        public TestDataController(ITestDataService testDataService, ITestDataDAO testDataDAO)
        {
            this.testDataService = testDataService;
            this.testDataDAO = testDataDAO;
        }


        /// <summary>
        /// 通过某个字段查询
        /// </summary>
        /// <param name="des"></param>
        /// <returns></returns>
        [HttpGet("FindByDes")]
        public BaseResponse<TestDataDO> FindByDes(string des) {
            TestDataDO testDataDO = testDataDAO.FindAll(u => u.Des.Equals(des)).FirstOrDefault();

            if (testDataDO == null)
            {
                return BaseResponse<TestDataDO>.Fail("查找失败");
            }
            return BaseResponse<TestDataDO>.Success(testDataDO);
        }

        /// <summary>
        /// 通过Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FindById")]
        public BaseResponse<TestDataDO> FindById(long id)
        {
            TestDataDO testDataDO = testDataDAO.FindAll(u => u.Id.Equals(id)).FirstOrDefault();

            if (testDataDO == null)
            {
                return BaseResponse<TestDataDO>.Fail("查找失败");
            }
            return BaseResponse<TestDataDO>.Success(testDataDO);
        }


        /// <summary>
        /// 批量添加测试
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("AddRandomData")]
        public BaseResponse<string> AddRandomData(int count) {
            testDataService.AddRandomData(count);
            return BaseResponse<string>.Success("创建完毕");
        }


        /// <summary>
        /// 分页查询测试
        /// </summary>
        /// <param name="pageInde"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public BaseResponse<List<TestDataDO>> GetPage(int pageInde,int pageSize)
        {
            return BaseResponse<List<TestDataDO>>.Success(testDataDAO.LimitPage(pageInde, pageSize));
        }



        /// <summary>
        /// 模糊查询测试
        /// </summary>
        /// <param name="desLike"></param>
        /// <returns></returns>
        [HttpGet("SearchLike")]
        public BaseResponse<List<TestDataDO>> SearchLike(string desLike)
        {
            List<TestDataDO> testDataDOs = testDataDAO.FindAll(u => u.Des.Contains(desLike));
            if (testDataDOs.Count > 0)
            {
                return BaseResponse<List<TestDataDO>>.Success(testDataDOs);
            }
            return BaseResponse<List<TestDataDO>>.Fail("未找到相关数据");
        }
    }
}

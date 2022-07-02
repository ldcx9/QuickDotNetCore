using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Task
{
    public class TestTask : ITask<int>
    {
        public object Id => "TestTask1";

        public string Rule => "w!m:1,s:30";// w!m:1,s:30 当分数为1且秒数为30时执行   m:1,s:30 每一分三十秒执行一次 

        public void Run(int data)
        {
            Console.WriteLine(Id + ":" + data);
        }
    }
}

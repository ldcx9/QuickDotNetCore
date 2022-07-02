using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Task
{
    public interface ITask<T>
    {
        object Id { get; }
        string Rule { get; }

        void Run(T data);
    }
}

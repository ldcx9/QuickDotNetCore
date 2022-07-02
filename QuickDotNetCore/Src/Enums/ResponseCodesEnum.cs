using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Enums
{
    /// <summary>
    /// 状态码标记
    /// </summary>
    public enum ResponseCodesEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 2000,
        /// <summary>
        /// 失败
        /// </summary>
        FAIL = 4000,
        /// <summary>
        /// 权限错误(未登录,越权等)
        /// </summary>
        AUTH_ERROR = 4001,
        /// <summary>
        /// 参数错误
        /// </summary>
        PARAM_ERROR = 4003,
        /// <summary>
        /// 未知错误
        /// </summary>
        UNKNOWN_ERROR = 5001
    }
}

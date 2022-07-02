using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Enums;

namespace QuickDotNetCore.Src.vo
{
    [Serializable]
                
    public class BaseResponse<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code
        {
            get;
        }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message {
            get;
        }
        /// <summary>
        /// 响应体
        /// </summary>
        public T Data {
            get;
        }
        private BaseResponse(ResponseCodesEnum code, string message, T data) :base(){
            this.Code = (int)code;
            this.Message = message;
            this.Data = data;
        }

        public static BaseResponse<T> Success(T data) {
            BaseResponse<T> baseRespon = new(ResponseCodesEnum.SUCCESS, "Success", data);
            return baseRespon;
        }

        public static BaseResponse<T> Success(T data,string message)
        {
            BaseResponse<T> baseRespon = new(ResponseCodesEnum.SUCCESS, message, data);
            return baseRespon;
        }

 
        public static BaseResponse<T> Fail(string message)
        {
            BaseResponse<T> baseRespon = new(ResponseCodesEnum.FAIL, message,default);
            return baseRespon;
        }


        public static BaseResponse<T> Create(ResponseCodesEnum code,string message,T data)
        {
            BaseResponse<T> baseRespon = new(code, message, data);
            return baseRespon;
        }
    }

}

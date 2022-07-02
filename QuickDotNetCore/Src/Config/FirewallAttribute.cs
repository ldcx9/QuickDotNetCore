using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickDotNetCore.Src.Enums;
using QuickDotNetCore.Src.vo;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace TingBao_API.Src.Extensions
{
    public class FirewallAttribute : ActionFilterAttribute, IFilterMetadata
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            System.Console.WriteLine("进入拦截器");
            //获取真实ip地址，拦截代理请求
            var request = context.HttpContext.Request;
            var count = request.Headers["Authorization"].Count;
            if (count > 0)
            {
                var jwt_token = request.Headers["Authorization"][count - 1].ToString();
                string userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                long id = long.Parse(userId);
                Console.WriteLine(jwt_token);

            }
            var ip = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var trueip = request.Headers["CF-Connecting-IP"].ToString();
            if (!string.IsNullOrEmpty(trueip) && ip != trueip)
            {
                context.Result = new JsonResult(BaseResponse<string>.Create(ResponseCodesEnum.UNKNOWN_ERROR, "errer", "客户端请求不合法"));
            }
            //关键词拦截
            var path = HttpUtility.UrlDecode(request.Path + request.QueryString, Encoding.UTF8);
            if (Regex.Match(path ?? "", "彩票|办证|AV女优|av|+q|+Q|+VX|+WX|卖片|淫").Length > 0) 
            {
                context.Result = new JsonResult(BaseResponse<string>.Create(ResponseCodesEnum.UNKNOWN_ERROR, "errer", "参数不合法！"));

            }
            System.Console.WriteLine("访问ip："+ip);
        }

    }
    //public class AccessDenyException : Exception
    //{
    //    public AccessDenyException(string msg) : base(msg)
    //    {
    //        Console.WriteLine(msg);
    //    }
    //}
}

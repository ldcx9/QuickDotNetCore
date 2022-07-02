using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using QuickDotNetCore.Src.vo;

namespace WeChat.Api.Extensions
{
	/// <summary>
	/// 全局异常拦截器
	/// </summary>
	public class GlobalExceptionFilter : IExceptionFilter, IFilterMetadata
	{
		private IWebHostEnvironment _env;

		private ILogger<GlobalExceptionFilter> _logger;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="_env"></param>
		/// <param name="_logger"></param>
		public GlobalExceptionFilter(IWebHostEnvironment _env, ILogger<GlobalExceptionFilter> _logger)
		{
			this._env = _env;
			this._logger = _logger;
		}

		/// <summary>
		/// 接口实现
		/// </summary>
		/// <param name="context"></param>
		public void OnException(ExceptionContext context)
		{
			_logger.LogError(context.Exception.Message + "\r\n" + context.Exception.StackTrace + "\r\n");
			context.Result = new JsonResult(BaseResponse<string>.Create(QuickDotNetCore.Src.Enums.ResponseCodesEnum.UNKNOWN_ERROR,"errer", "系统异常：" + context.Exception.Message));
			context.ExceptionHandled = true;
		}
	}
}

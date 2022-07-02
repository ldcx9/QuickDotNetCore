using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuickDotNetCore.Src.vo
{
	/// <summary>
	/// 接口组排序标记
	/// </summary>
	public class ApiGroupSortFilter : IDocumentFilter
	{
		/// <summary>
		/// 接口实现
		/// </summary>
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			Dictionary<OpenApiTag, int> tags = new Dictionary<OpenApiTag, int>();
			Assembly assembly = Assembly.GetExecutingAssembly();
			string fullname = assembly.GetName().Name + ".Controllers.{0}Controller";
			foreach (OpenApiTag item in swaggerDoc.Tags)
			{
				tags.Add(item, assembly.GetType(string.Format(fullname, item.Name)).GetCustomAttribute<ApiGroupSortAttribute>()?.Sort ?? 0);
              
			}
			swaggerDoc.Tags = tags.OrderByDescending((KeyValuePair<OpenApiTag, int> p) => p.Value).ToDictionary((KeyValuePair<OpenApiTag, int> k) => k.Key, (KeyValuePair<OpenApiTag, int> v) => v.Value).Keys.ToList();
		}
	}
}

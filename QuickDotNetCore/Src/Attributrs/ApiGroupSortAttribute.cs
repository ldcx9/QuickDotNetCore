using System;

namespace QuickDotNetCore.Src.vo
{
	/// <summary>
	/// 接口组排序标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class ApiGroupSortAttribute : Attribute
	{
		/// <summary>
		/// 序号
		/// 越大越靠前
		/// </summary>
		public int Sort { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="sort"></param>
		public ApiGroupSortAttribute(int sort)
		{
			Sort = sort;
		}
	}
}

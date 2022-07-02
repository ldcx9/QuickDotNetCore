using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Utils
{
    public class JsonUtil
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeToString(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string str) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}

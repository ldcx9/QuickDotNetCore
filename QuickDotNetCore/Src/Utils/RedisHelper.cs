
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace QuickDotNetCore.Src.utils
{
   public class RedisHelper
    {
        private readonly static string REDIS_CONN = "127.0.0.1:6379";
        private readonly static string REDIS_IP = "127.0.0.1";
        private readonly static int REDIS_PORT = 6379;


        private static ConnectionMultiplexer redis = null;
        private  static IDatabase database = null;
        private static IServer server = null;
        private static int mydb = 0;
        public static void Register(int db)
        {
            mydb = db;
            if (redis == null)
            {
                redis = ConnectionMultiplexer.Connect(REDIS_CONN);
                database = redis.GetDatabase(mydb);
                server = redis.GetServer(REDIS_IP, REDIS_PORT);
                redis.ErrorMessage += Redis_ErrorMessage;
            }
        }


        /// <summary>
        /// 异常记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Redis_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            //LogHelper.WriteLog("Redis", new Exception(e.Message));
        }

        /// <summary>
        /// 通过key获取value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StringGet(string key)
        {
            if (database == null)
            {
                Register(0);
            }
            return database.StringGet(key);
        }

        /// <summary>
        /// 新增key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool StringSet(string key, string val, TimeSpan? exp = default(TimeSpan?))
        {
            if (database == null)
            {
                Register(0);
            }
            return database.StringSet(key, val, exp);
        }

        /// <summary>
        /// 新增key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool ObjectSet<T>(string key, T val, TimeSpan? exp = default(TimeSpan?))
        {
            if (database == null)
            {
                Register(0);
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(val);
            return database.StringSet(key, json, exp);
        }
        /// <summary>
        /// 新增key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static T? ObjectGet<T>(string key, TimeSpan? exp = default(TimeSpan?))
        {
            if (database == null)
            {
                Register(0);
            }
            string jsonStr = database.StringGet(key);
            if (jsonStr == null)
            {
                return default(T);
            }
            T? json = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
            return json;
        }
        
        
        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<RedisKey> LikeKeys(string pattern = "*")
        {
            if (database == null)
            {
                Register(0);
            }
            return server.Keys(database: mydb, pattern: pattern);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLikeKeyValue(string pattern = "*")
        {
            if (database == null)
            {
                Register(0);
            }
            IEnumerable<RedisKey> list = LikeKeys(pattern);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    dic.Add(item, StringGet(item));
                }
            }
            return dic;
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            if (database == null)
            {
                Register(0);
            }
            return database.KeyDelete(key);
        }
        /// <summary>
        /// 添加或者更新KeyValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StringAddOrUpdate(string key,string value)
        {
            if (database == null)
            {
                Register(0);
            }
            if (string.IsNullOrWhiteSpace(StringGet(key)))
            {
                if(StringSet(key, value))
                {
                    return true;
                }
            }
            else if (KeyDelete(key)&& StringSet(key, value))
            {
                return true;
            }
            return false;
        }


    }
}

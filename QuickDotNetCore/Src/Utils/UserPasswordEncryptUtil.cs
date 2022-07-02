using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Models;

namespace QuickDotNetCore.Src.Utils
{
    public class UserPasswordEncryptUtil
    {
        
        public static string Encrypt(UserDO userDO) {
            return EncryptHelper.MD5(userDO.Password+userDO.Password, System.Text.Encoding.UTF8);
        }

        public static string Encrypt(string info)
        {
            return EncryptHelper.MD5(info, System.Text.Encoding.UTF8);
        }
    }
}

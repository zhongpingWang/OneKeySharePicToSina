using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phone.Helper
{
    /// <summary>
    /// IsolatedStorage 辅助类
    /// </summary>
    public class IsolatedStorageHelper
    {
        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="user"></param>
        public static void SetIsolatedStorage<T>(string key, T value)
        {
            var postSettings = IsolatedStorageSettings.ApplicationSettings;
            postSettings[key] = value;
        }

        /// <summary>
        /// 本地读取用户数据
        /// </summary>
        /// <returns></returns>
        public static void GetIsolatedStorage<T>(string key, out T resultType)
        {
            var localSettings = IsolatedStorageSettings.ApplicationSettings;
            var result = localSettings.Where(a => a.Key == key);
            if (result.Any())
            {
                resultType = (T)localSettings[key];
            }
            else
            {
                resultType = default(T);
            } 
        }
    }
}
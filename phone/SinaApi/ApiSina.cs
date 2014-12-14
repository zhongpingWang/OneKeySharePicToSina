using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace phone.SinaApi
{
    /// <summary>
    /// sina api
    /// </summary>
    public class ApiSina
    {
        private static string _appKey = null;
        /// <summary>
        /// appkey
        /// </summary>
        public static string AppKey
        {
            get
            {
                if (ApiSina._appKey == null)
                {
                    Uri uri = new Uri("config.xml", UriKind.Relative);
                    var configDoc = XDocument.Load(uri.ToString());
                    var configAppkey = configDoc.Element("config").Element("SinaApi").Element("appKey").Value;

                    if (!string.IsNullOrEmpty(configAppkey.ToString()))
                    {
                        ApiSina._appKey = configAppkey.ToString();
                    }
                }
                return ApiSina._appKey;


            }
            set { ApiSina._appKey = value; }
        }


        private static string _appSecret = null;
        /// <summary>
        /// appSecert
        /// </summary>
        public static string AppSecret
        {
            get
            {
                if (ApiSina._appSecret == null)
                {
                    Uri uri = new Uri("config.xml", UriKind.Relative);
                    var configDoc = XDocument.Load(uri.ToString());
                    var configAppSecret = configDoc.Element("config").Element("SinaApi").Element("appSecret").Value;

                    if (!string.IsNullOrEmpty(configAppSecret.ToString()))
                    {
                        ApiSina._appSecret = configAppSecret.ToString();
                    }
                }
                return ApiSina._appSecret;
            }
            set { ApiSina._appSecret = value; }
        }


        private static string _redirectUri = null;
        /// <summary>
        /// redirectUri
        /// </summary>
        public static string RedirectUri
        {
            get
            {
                if (ApiSina._redirectUri == null)
                {
                    Uri uri = new Uri("config.xml", UriKind.Relative);
                    var configDoc = XDocument.Load(uri.ToString());
                    var redirectUri = configDoc.Element("config").Element("SinaApi").Element("redirectUri").Value;

                    if (!string.IsNullOrEmpty(redirectUri.ToString()))
                    {
                        ApiSina._redirectUri = redirectUri.ToString();
                    }
                }
                return ApiSina._redirectUri;
            }
            set { ApiSina._redirectUri = value; }
        } 


    }
}

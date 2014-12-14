// ===============================================================================
// ClientOAuth.cs
// 功能:包含客户端方式鉴权功能，OAuth1.0的XAuth方式、OAuth2.0的password方式
// 作者:linan4@预备影帝
// ===============================================================================
// Copyright (c) Sina. 
// All rights reserved.
// ===============================================================================
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SinaBase;
using Hammock;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using Hammock.Silverlight.Compat;
using System.Text;
using System.Runtime.Serialization.Json;


namespace WeiboSdk
{
    /// <summary>
    /// 用于XAuth登录的静态类
    /// Author:linan4
    /// </summary>
    static public class ClientOAuth
    {
        static private OAuth2LoginBack m_LoginCallback = null;

        ///// <summary>
        ///// OAuth1.0的XAuth登录
        ///// </summary>
        ///// <param name="userName">用户名</param>
        ///// <param name="passWord">密码</param>
        ///// <param name="callBack">回调函数</param>
        //static public void XAuthLogin(string userName, string passWord, OAuth1LoginBack callBack = null)
        //{
        //    SdkAuthError error = new SdkAuthError();
        //    SdkAuthRes response = new SdkAuthRes();

        //    var credentials = new OAuthCredentials
        //    {
        //        Type = OAuthType.ClientAuthentication,
        //        SignatureMethod = OAuthSignatureMethod.HmacSha1,
        //        ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
        //        ConsumerKey = SdkData.AppKey,
        //        ConsumerSecret = SdkData.AppSecret,
        //        ClientUsername = userName,
        //        ClientPassword = passWord
        //    };

        //    var client = new RestClient
        //    {
        //        Authority = ConstDefine.publicApiUrl,
        //        Credentials = credentials,
        //        HasElevatedPermissions = true,

        //    };

        //    var request = new RestRequest
        //    {
        //        Path = "/oauth/access_token",
        //        Method = WebMethod.Post
        //    };

        //    client.BeginRequest(request, (e1, e2, e3) =>
        //    {
        //        if (null != e2.UnKnowException)
        //        {
        //            error.errCode = SdkErrCode.NET_UNUSUAL;
        //            error.errMessage = "无网络";
        //            if (null != callBack)
        //                callBack(false, error, response);
        //            return;
        //        }

        //        if (null != e2.InnerException)
        //        {
        //            error.errCode = SdkErrCode.NET_UNUSUAL;
        //            if (!string.IsNullOrEmpty(e2.Content))
        //            {
        //                error.errCode = SdkErrCode.SERVER_ERR;
        //                //request=%2Foauth%2Faccess_token&error_code=403&error=40309%3AError%3A+
        //                //password+error%21&error_CN=%E9%94%99%E8%AF%AF%3A%E5%AF%86%E7%A0%81%E4%B8%8D%E6%AD%A3%E7%A1%AE
        //                string content = HttpUtility.UrlDecode(e2.Content);
        //                content = BaseTool.GetQueryParameter(content, "error");
        //                int pos = content.IndexOf(":");
        //                if (-1 != pos)
        //                    content = content.Substring(0, pos);
        //                //错误码
        //                error.specificCode = content;
        //            }
        //            if (null != callBack)
        //                callBack(false, error, response);
        //            return;
        //        }
        //        //获取accessToken 和 accessTokenSecret
        //        string token = BaseTool.GetQueryParameter(e2.Content, "oauth_token");
        //        string tokenAccess = BaseTool.GetQueryParameter(e2.Content, "oauth_token_secret");
        //        string userId = BaseTool.GetQueryParameter(e2.Content, "user_id");
        //        //成功/失败
        //        if (0 == token.Length || 0 == tokenAccess.Length)
        //        {
        //            string _content = HttpUtility.UrlDecode(e2.Content);
        //            string errCode = BaseTool.GetQueryParameter(_content, "error");
        //            int pos = errCode.IndexOf(":");
        //            if (pos != -1)
        //                errCode = errCode.Remove(pos);
        //            string chMessage = "";
        //            //if (errCode == "40309")
        //            //    chMessage = "您输入的登录或密码错误，请重新输入.";
        //            //else
        //            chMessage = BaseTool.GetQueryParameter(_content, "error_CN");

        //            error.errCode = SdkErrCode.SERVER_ERR;
        //            error.specificCode = errCode;
        //            error.errMessage = chMessage;
                    
        //            if(null != callBack)
        //                callBack(false, error,response);
        //        }

        //        else
        //        {
        //            if (null != callBack)
        //            {
        //                response.userId = userId;
        //                response.acessToken = token;
        //                response.acessTokenSecret = tokenAccess;
        //                callBack(true, error, response);
        //            }
                        
        //       }
        //    });
        //}

        /// <summary>
        /// OAuth2.0客户端方式(需要授权)获取AccessToken
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="callback">回调函数</param>
        static public void GetAccessToken(string name, string passWord, OAuth2LoginBack callback)
        {
            m_LoginCallback = callback;
            RestClient client = new RestClient();
            client.Authority = ConstDefine.ServerUrl2_0;
            client.HasElevatedPermissions = true;

            RestRequest request = new RestRequest();
            request.Path = "/oauth2/access_token";
            request.Method = WebMethod.Post;

            request.DecompressionMethods = DecompressionMethods.GZip;
            request.Encoding = Encoding.UTF8;

            request.AddParameter("client_id", SdkData.AppKey);
            request.AddParameter("client_secret", SdkData.AppSecret);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", name);
            request.AddParameter("password", passWord);

            client.BeginRequest(request, (e1, e2, e3) => AuthCallback(e1, e2, e3));
        }

        /// <summary>
        /// 用OAuth1.0 refleshCode刷新AccessToken
        /// </summary>
        /// <param name="refleshCode"></param>
        static public void RefleshAccessToken(string refleshCode, OAuth2LoginBack callBack)
        {
            m_LoginCallback = callBack;
            RestClient client = new RestClient();
            client.Authority = ConstDefine.ServerUrl2_0;
            client.HasElevatedPermissions = true;

            RestRequest request = new RestRequest();
            request.Path = "/oauth2/access_token";
            request.Method = WebMethod.Post;
            request.DecompressionMethods = DecompressionMethods.GZip;
            request.Encoding = Encoding.UTF8;

            request.AddParameter("client_id", SdkData.AppKey);
            request.AddParameter("client_secret", SdkData.AppSecret);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refleshCode);

            client.BeginRequest(request, (e1, e2, e3) => AuthCallback(e1, e2, e3));
        }

        static private void AuthCallback(RestRequest request, RestResponse response, object userState)
        {
            SdkAuthError err = new SdkAuthError();
            if (null != response.UnKnowException)
            {
                err.errCode = SdkErrCode.NET_UNUSUAL;
                if (null != m_LoginCallback)
                    m_LoginCallback(false, err, null);
            }
            else if (response.StatusCode != HttpStatusCode.OK || null != response.InnerException)
            {
                if (null == response.ContentStream || response.ContentStream.Length == 0)
                {
                    err.errCode = SdkErrCode.NET_UNUSUAL;
                    if (null != m_LoginCallback)
                        m_LoginCallback(false, err, null);
                    return;
                }
                DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OAuthErrRes));
                OAuthErrRes errRes = ser.ReadObject(response.ContentStream) as OAuthErrRes;
                err.errCode = SdkErrCode.SERVER_ERR;
                err.specificCode = errRes.ErrorCode;
                err.errMessage = errRes.errDes;
                if (null != m_LoginCallback)
                    m_LoginCallback(false, err, null);
            }
            else
            {
                err.errCode = SdkErrCode.SUCCESS;
                DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SdkAuth2Res));
                SdkAuth2Res oauthRes = ser.ReadObject(response.ContentStream) as SdkAuth2Res;
                if (null != m_LoginCallback)
                    m_LoginCallback(true, err, oauthRes);
            }
        }
    }
}

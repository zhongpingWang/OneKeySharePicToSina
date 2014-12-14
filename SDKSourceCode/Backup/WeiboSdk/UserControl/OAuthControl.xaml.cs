// ===============================================================================
// OAuthControl.xaml.cs
// 功能:OAuth的网页授权，根据SdkData的AuthOption变量控制是OAuth1.0/2.0
// 作者:linan4@预备影帝
// ===============================================================================
// Copyright (c) Sina. 
// All rights reserved.
// ===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SinaBase;
using System.Windows.Navigation;
using Hammock;
using Hammock.Authentication;
using System.ComponentModel;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using System.Text;
using System.Runtime.Serialization.Json;
using Hammock.Silverlight.Compat;

namespace WeiboSdk
{
    /// <summary>
    /// OAUTH 登录控件
    /// Author:linan4
    /// </summary>
    public partial class OAuthControl : UserControl
    {
        public OAuthControl()
        {
            InitializeComponent();
            if (DesignerProperties.IsInDesignTool)
                return;
            isCompleted = false;
            if (SdkData.AuthOption == EumAuth.OAUTH2_0)
            {
                string accredit = string.Format("{0}/oauth2/authorize?client_id={1}&response_type=code&redirect_uri={2}&display=mobile"
                    , ConstDefine.ServerUrl2_0, SdkData.AppKey, SdkData.RedirectUri);
                Dispatcher.BeginInvoke(() =>
                {
                    OAuthBrowser.Navigate(new Uri(accredit));
                });
            }
            else
                GetOAuthToken();
        }

        #region 成员变量
        private string oAuthToken = "";
        private string oAuthTokenSecret = "";
        private bool isCompleted = false;
        private SdkAuthError m_Error = new SdkAuthError { errCode = SdkErrCode.SUCCESS };
        private SdkAuthRes resInfo = new SdkAuthRes();

        //登录的回调
        public OAuth1LoginBack OAuthVerifyCompleted { get; set; }
        public OAuth2LoginBack OAuth2VerifyCompleted { get; set; }
        public EventHandler OBrowserCancelled { get; set; }
        public EventHandler OBrowserNavigated { get; set; }
        public EventHandler OBrowserNavigating { get; set; }
        #endregion

        private void BrowserNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //if (null != OAuthBrowserNavigated)
            //    OAuthBrowserNavigated.Invoke(sender, e);
        }

        private void BrowserNavigating(object sender, NavigatingEventArgs e)
        {
            if (isCompleted)
            {
                //e.Cancel = true;
                return;
            }

            if (null != OBrowserNavigating)
                OBrowserNavigating.Invoke(sender, e);

            if (e.Uri.AbsoluteUri.Contains("javascript:void(0)") || e.Uri.AbsoluteUri.Contains("closeWindow()") || e.Uri.AbsoluteUri.Contains("error=access_denied"))
            {
                if (null != OBrowserCancelled)
                    OBrowserCancelled.Invoke(sender, e);
                isCompleted = true;
                return;
            }

            if (SdkData.AuthOption == EumAuth.OAUTH1_0)
            {
                string url = SdkData.RedirectUri.ToLower();
                if (!e.Uri.AbsoluteUri.Contains(url))
                    return;
            }
            else
            {
                if (!e.Uri.AbsoluteUri.Contains("code=") && !e.Uri.AbsoluteUri.Contains("code ="))
                    return;
            }

            ///e.Cancel = true;

            isCompleted = true;
            var arguments = e.Uri.AbsoluteUri.Split('?');
            if (0 == arguments.Length)
            {
                m_Error.errCode = SdkErrCode.SERVER_ERR;
                if (SdkData.AuthOption == EumAuth.OAUTH1_0)
                {
                    if (null != OAuthVerifyCompleted)
                        OAuthVerifyCompleted(false, m_Error, null);
                }
                else
                {
                    if (null != OAuth2VerifyCompleted)
                        OAuth2VerifyCompleted(false, m_Error, null);
                }
                return;
            }

            if (SdkData.AuthOption == EumAuth.OAUTH1_0)
                GetAccessToken(arguments[1]);
            else
                GetOAuth2AccessToken(arguments[1]);
        }

        private void GetOAuthToken()
        {
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = SdkData.AppKey,
                ConsumerSecret = SdkData.AppSecret,
                Version = "1.0",
                CallbackUrl = SdkData.RedirectUri
            };

            var client = new RestClient
            {
                Authority = ConstDefine.OAuth1_0Url,
                Credentials = credentials,
                HasElevatedPermissions = true
            };

            var request = new RestRequest
            {
                Path = "/request_token"
            };
            client.BeginRequest(request, new RestCallback(RequestTokenCallBack));
        }

        private void RequestTokenCallBack(RestRequest request, RestResponse response, object userstate)
        {
            oAuthToken = BaseTool.GetQueryParameter(response.Content, "oauth_token");
            oAuthTokenSecret = BaseTool.GetQueryParameter(response.Content, "oauth_token_secret");
            var authorizeUrl = ConstDefine.OAuth1_0Url + "/authorize" + "?oauth_token=" + oAuthToken;

            authorizeUrl += "&display=mobile&oauth_callback=" + System.Net.HttpUtility.UrlEncode(SdkData.RedirectUri);
            if (String.IsNullOrEmpty(oAuthToken) || String.IsNullOrEmpty(oAuthTokenSecret))
            {

                if (null != OAuthVerifyCompleted)
                {
                    m_Error.errCode = SdkErrCode.SERVER_ERR;
                    OAuthVerifyCompleted(false, m_Error, resInfo);
                }
                return;
            }

            Dispatcher.BeginInvoke(() =>
            {
                OAuthBrowser.Navigate(new Uri(authorizeUrl));
            });
        }

        private void GetAccessToken(string uri)
        {
            var requestToken = BaseTool.GetQueryParameter(uri, "oauth_token");
            if (requestToken != oAuthToken)
            {
                if (null != OAuthVerifyCompleted)
                {
                    m_Error.errCode = SdkErrCode.SERVER_ERR;
                    OAuthVerifyCompleted(false, m_Error, resInfo);
                    return;
                }
            }

            var requestVerifier = BaseTool.GetQueryParameter(uri, "oauth_verifier");

            var credentials = new OAuthCredentials
            {
                Type = OAuthType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = SdkData.AppKey,
                ConsumerSecret = SdkData.AppSecret,
                Token = oAuthToken,
                TokenSecret = oAuthTokenSecret,
                Verifier = requestVerifier
            };

            var client = new RestClient
            {
                Authority = ConstDefine.OAuth1_0Url,
                Credentials = credentials,
                HasElevatedPermissions = true
            };

            var request = new RestRequest
            {
                Path = "/access_token"
            };

            client.BeginRequest(request, new RestCallback(RequestAccessTokenCallBack));
        }

        private void RequestAccessTokenCallBack(RestRequest request, RestResponse response, object userstate)
        {
            string accessToken = BaseTool.GetQueryParameter(response.Content, "oauth_token");
            string accessTokenSecret = BaseTool.GetQueryParameter(response.Content, "oauth_token_secret");
            string userId = BaseTool.GetQueryParameter(response.Content, "user_id");
            //string screenName = GetQueryParameter(response.Content, "screen_name");

            if (String.IsNullOrEmpty(accessToken) || String.IsNullOrEmpty(accessTokenSecret))
            {
                //TODO:
                //通知
                if (null != OAuthVerifyCompleted)
                {
                    m_Error.errCode = SdkErrCode.SERVER_ERR;
                    OAuthVerifyCompleted(false, m_Error, resInfo);
                }
                return;
            }

            //isCompleted = true;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                m_Error.errCode = SdkErrCode.SUCCESS;
                //通知
                if (null != OAuthVerifyCompleted)
                {
                    resInfo.userId = userId;
                    resInfo.acessToken = accessToken;
                    resInfo.acessTokenSecret = accessTokenSecret;
                    OAuthVerifyCompleted(true, m_Error, resInfo);
                }
            });

        }

        private void GetOAuth2AccessToken(string uri)
        {

            string requestVerifier = BaseTool.GetQueryParameter(uri, "code");
            if (string.IsNullOrEmpty(requestVerifier))
            {
                m_Error.errCode = SdkErrCode.NET_UNUSUAL;
                if (null != OAuthVerifyCompleted)
                    OAuthVerifyCompleted(false, m_Error, null);
                return;
            }

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
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", SdkData.RedirectUri);
            request.AddParameter("code", requestVerifier);


            client.BeginRequest(request, (e1, e2, e3) =>
            {
                if (null != e2.UnKnowException || null != e2.InnerException)
                {
                    m_Error.errCode = SdkErrCode.NET_UNUSUAL;
                    if (null != OAuthVerifyCompleted)
                        OAuthVerifyCompleted(false, m_Error, null);
                }
                else if (e2.StatusCode != HttpStatusCode.OK)
                {
                    if (null == e2.ContentStream || e2.ContentStream.Length == 0)
                    {
                        m_Error.errCode = SdkErrCode.NET_UNUSUAL;
                        if (null != OAuthVerifyCompleted)
                            OAuthVerifyCompleted(false, m_Error, null);
                        return;
                    }
                    DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OAuthErrRes));
                    OAuthErrRes errRes = ser.ReadObject(e2.ContentStream) as OAuthErrRes;

                    m_Error.errCode = SdkErrCode.SERVER_ERR;
                    m_Error.specificCode = errRes.ErrorCode;
                    m_Error.errMessage = errRes.errDes;
                    if (null != OAuthVerifyCompleted)
                        OAuthVerifyCompleted(true, m_Error, null);
                }
                else
                {
                    m_Error.errCode = SdkErrCode.SUCCESS;
                    //isCompleted = true;
                    //解析
                    DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SdkAuth2Res));
                    var oauthRes = ser.ReadObject(e2.ContentStream) as SdkAuth2Res;

                    if (null != OAuth2VerifyCompleted)
                        OAuth2VerifyCompleted(true, m_Error, oauthRes);
                }
            });


        }
        private void BrowserLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                OAuthBrowser.InvokeScript("eval",
                @"
                window.ScanTelTag=function(elem) {
                if (elem.getAttribute('target') != null && elem.getAttribute('target').indexOf('_parent') == 0) {
                    elem.setAttribute('target', '_self');
                    }
                }
            
                window.Initialize=function() {
                var elems = document.getElementsByTagName('a');
                for (var i = 0; i < elems.length; i++)
                ScanTelTag(elems[i]);
                }");
                OAuthBrowser.InvokeScript("Initialize");
            }
            catch (Exception)
            {
            }

            if (null != OBrowserNavigated)
                OBrowserNavigated.Invoke(sender, e);
        }

    }
}
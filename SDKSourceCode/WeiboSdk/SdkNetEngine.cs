// ===============================================================================
// SdkNetEngine.cs
// 功能:完成一搬的微博数据请求
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
using System.IO;
using Hammock.Silverlight.Compat;
using System.Text;
using Hammock.Authentication.OAuth;
using Hammock.Authentication.Basic;
using Hammock.Authentication;
using Hammock.Web;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

namespace WeiboSdk
{
    /// <summary>
    /// SDK中的数据请求类
    /// Author:linan4
    /// </summary>
    public class SdkNetEngine
    {
        #region 成员变量
        private RestClient m_Client = new RestClient();
        public delegate void SdkCallBack(SdkRequestType type, SdkResponse response);
        public delegate void RequestBack(SdkResponse response);
        private SdkCallBack m_RequestCallBack = null;
        #endregion

        //public void StopRequest()
        //{
        //    if (null != m_Client)
        //        m_Client.StopRequest();
        //}

        /// <summary>
        /// 普通数据请求
        /// </summary>
        /// <param name="type"请求数据类型></param>
        /// <param name="data">参数包(SdkCmdBase的子类)</param>
        /// <param name="callBack">请求的回调函数</param>
        /// <param name="dataType">要求返回数据类型</param>
        public void RequestCmd (SdkRequestType type, SdkCmdBase data, SdkCallBack callBack)
        {
            m_RequestCallBack = callBack;

            RestRequest request = new RestRequest();

            bool retValue = ConfigParams(request, type, data);

            if (false == retValue)
                return;
            SendRequest(request,data, (e1) =>
            {
                if (null != m_RequestCallBack)
                    m_RequestCallBack(type, e1);

            });

        }

        /// <summary>
        /// 发送普通网络请求，传入组好参数包的RestRequest
        /// </summary>
        /// <param name="request">要发送的组好参的request</param>
        /// <param name="data">如需鉴权传入OAuth1.0的accessToken、accessTokenSecret或者OAuth2.0的accessToken</param>
        /// <param name="callBack">回调</param>
        /// <param name="dataType">需要xml/json</param>
        public void SendRequest(RestRequest request, SdkCmdBase data,RequestBack callBack)
        {
            Action<string> errAction = (e1) =>
            {
                if (null != callBack)
                {
                    SdkErrCode sdkErr = SdkErrCode.XPARAM_ERR;
                    callBack(new SdkResponse
                    {
                        //requestID = null != data ? data.requestId : "",
                        errCode = sdkErr,
                        specificCode = "",
                        content = e1,
                        stream = null
                    });
                }
            };

            if (null == request)
            {
                errAction("request should`t be null.");
                return;
            }

            //string strOauth = "";
            //if (SdkData.AuthOption == EumAuth.OAUTH2_0)
            //    strOauth = "https" + ConstDefine.publicApiUrl;
            //else
            //    strOauth = "http" + ConstDefine.publicApiUrl;
            m_Client.Authority = ConstDefine.publicApiUrl;
            m_Client.HasElevatedPermissions = true;

            //添加鉴权
            request.DecompressionMethods = DecompressionMethods.GZip;
            request.Encoding = Encoding.UTF8;
            //设置 User-Agent
            request.UserAgent = SdkData.UserAgent;

            IWebCredentials credentials = null;

            if (null != data && !string.IsNullOrEmpty(data.acessToken))
            {
                //OAuth 认证
                //if (SdkData.AuthOption == EumAuth.OAUTH2_0)
                //{
                    request.AddHeader("Authorization", string.Format("OAuth2 {0}", data.acessToken));
                //}
                //else
                //{
                //    credentials = new OAuthCredentials
                //    {
                //        Type = OAuthType.ProtectedResource,
                //        SignatureMethod = OAuthSignatureMethod.HmacSha1,
                //        ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                //        ConsumerKey = SdkData.AppKey,
                //        ConsumerSecret = SdkData.AppSecret,
                //        Token = data.acessToken,
                //        TokenSecret = data.acessTokenSecret,
                //        Version = "1.0",
                //    };
                //}
            }
            else
            {
                request.AddParameter("source", SdkData.AppKey);
            }

            request.Credentials = credentials;

            m_Client.BeginRequest(request, (e1, e2, e3) => AsyncCallback(e1, e2, callBack));

        }


        private bool ConfigParams(RestRequest request, SdkRequestType type, SdkCmdBase data)
        {
            Action<string> errAction = (e1) =>
            {
                if (null != m_RequestCallBack)
                {
                    SdkErrCode sdkErr = SdkErrCode.XPARAM_ERR;
                    m_RequestCallBack(type, new SdkResponse
                    {
                        //requestID = null != data ? data.requestId : "",
                        errCode = sdkErr,
                        specificCode = "",
                        content = e1,
                        stream = null
                    });
                }
            };

            if (null == request)
            {
                errAction("发生内部错误");
                return false;
            }
            switch (type)
            {
                case SdkRequestType.UPLOAD_MESSAGE:
                    {
                        cmdUploadMessage message = null;
                        if (data is cmdUploadMessage)
                            message = data as cmdUploadMessage;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Method = WebMethod.Post;
                        request.Path = "/statuses/update.json";

                        if (message.status.Length > 0)
                            request.AddParameter("status", message.status);
                        if (message.ReplyId.Length > 0)
                            request.AddParameter("in_reply_to_status_id", message.ReplyId);
                        if (message.lat.Length > 0)
                            request.AddParameter("lat", message.lat);
                        if (message._long.Length > 0)
                            request.AddParameter("long", message._long);
                        if (message.annotations.Length > 0)
                            request.AddParameter("annotations", message.annotations);
                    }
                    break;
                case SdkRequestType.UPLOAD_MESSAGE_PIC:
                    {
                        cmdUploadPic message = null;
                        if (data is cmdUploadPic)
                            message = data as cmdUploadPic;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Method = WebMethod.Post;
                        request.Path = "/statuses/upload.json";

                        request.AddField("status", message.messageText);

                        request.AddParameter("visible", message.visible.ToString());
                        if (message.lat.Length > 0)
                            request.AddField("lat", message.lat);
                        if (message._long.Length > 0)
                            request.AddField("long", message._long);

                        if (0 == message.picPath.Length)
                        {
                            errAction("_picPath is null.");
                            return false;
                        }

                        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                        if (!file.FileExists(message.picPath))
                        {
                            file.Dispose();
                            errAction("_picPath is not exist.");
                            return false;
                        }
                        file.Dispose();
                        string picType = System.IO.Path.GetExtension(message.picPath);
                        string picName = System.IO.Path.GetFileName(message.picPath);
                        if ("png" == picType)
                        {
                            request.AddFile("pic", picName, message.picPath, "image/png");
                        }
                        else
                        {
                            request.AddFile("pic", picName, message.picPath, "image/jpeg");
                        }
                    }
                    break;

                case SdkRequestType.FRIENDS_TIMELINE:
                    {
                        cmdNormalMessages message = null;
                        if (data is cmdNormalMessages)
                            message = data as cmdNormalMessages;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/statuses/friends_timeline.json";
                        request.Method = WebMethod.Get;
                        if (message.sinceID.Length > 0)
                        {
                            request.AddParameter("since_id", message.sinceID);
                        }
                        if (message.maxID.Length > 0)
                        {
                            request.AddParameter("max_id", message.maxID);
                        }
                        if (message.count.Length > 0)
                        {
                            request.AddParameter("count", message.count);
                        }
                        if (message.page.Length > 0)
                            request.AddParameter("page", message.page);
                        if (message.baseApp.Length > 0)
                            request.AddParameter("base_app", message.baseApp);
                        if (message.feature.Length > 0)
                            request.AddParameter("feature", message.feature);
                    }
                    break;
                case SdkRequestType.USER_TIMELINE:
                    {
                        cdmUserTimeline message = null;
                        if (data is cdmUserTimeline)
                            message = data as cdmUserTimeline;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/statuses/user_timeline.json";
                        request.Method = WebMethod.Get;

                        if (message.userId.Length > 0)
                            request.AddParameter("user_id", message.userId);
                        if (message.screenName.Length > 0)
                            request.AddParameter("screen_name", message.screenName);
                        if (message.sinceID.Length > 0)
                        {
                            request.AddParameter("since_id", message.sinceID);
                        }
                        if (message.maxID.Length > 0)
                        {
                            request.AddParameter("max_id", message.maxID);
                        }
                        if (message.count.Length > 0)
                        {
                            request.AddParameter("count", message.count);
                        }

                        if (message.page.Length > 0)
                            request.AddParameter("page", message.page);
                        if (message.baseApp.Length > 0)
                            request.AddParameter("base_app", message.baseApp);
                        if (message.feature.Length > 0)
                            request.AddParameter("feature", message.feature);

                    }
                    break;
                case SdkRequestType.FRIENDSHIP_CREATE:
                    {
                        cmdFriendShip message = null;
                        if (data is cmdFriendShip)
                            message = data as cmdFriendShip;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/friendships/create.json";
                        request.Method = WebMethod.Post;

                        if (message._userId.Length > 0)
                            request.AddParameter("user_id", message._userId);
                        if (message._screenName.Length > 0)
                            request.AddParameter("screen_name", message._screenName);

                    }
                    break;
                case SdkRequestType.FRIENDSHIP_DESDROY:
                    {
                        cmdFriendShip message = null;
                        if (data is cmdFriendShip)
                            message = data as cmdFriendShip;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/friendships/destroy.json";
                        request.Method = WebMethod.Post;

                        if (message._userId.Length > 0)
                            request.AddParameter("user_id", message._userId);
                        if (message._screenName.Length > 0)
                            request.AddParameter("screen_name", message._screenName);

                        ////加入这个参数就是移除粉丝
                        //if (type == SdkRequestType.DESTORY_FANS)
                        //    request.AddParameter("is_follower", "1");

                    }
                    break;
                case SdkRequestType.FRIENDSHIP_SHOW:
                    {
                        cmdFriendShip message = null;
                        if (data is cmdFriendShip)
                            message = data as cmdFriendShip;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/friendships/show.json";
                        request.Method = WebMethod.Get;

                        if (message._sourceId.Length > 0)
                            request.AddParameter("source_id", message._sourceId);
                        if (message._sourceScreenName.Length > 0)
                            request.AddParameter("source_screen_name", message._sourceScreenName);

                        if (message._userId.Length > 0)
                            request.AddParameter("target_id", message._userId);
                        if (message._screenName.Length > 0)
                            request.AddParameter("target_screen_name", message._screenName);
                    }
                    break;

                case SdkRequestType.AT_USERS:
                    {
                        cmdAtUsers atUsers = null;
                        if (data is cmdAtUsers)
                            atUsers = data as cmdAtUsers;
                        else
                        {
                            errAction("param data type error.");
                            return false;
                        }
                        request.Path = "/search/suggestions/at_users.json";
                        request.Method = WebMethod.Get;
                        if (atUsers._keyword.Length > 0)
                            request.AddParameter("q", atUsers._keyword);
                        if (atUsers._count.Length > 0)
                            request.AddParameter("count", atUsers._count);
                        if (atUsers._range.Length > 0)
                            request.AddParameter("range", atUsers._range);
                        if (atUsers._type.Length > 0)
                            request.AddParameter("type", atUsers._type);

                    }
                    break;
                default:
                    {
                        errAction("此请求尚不支持.");
                        return false;
                    }
            }
            request.AddParameter("curtime", DateTime.Now.ToString());
            return true;
        }

        private void AsyncCallback(RestRequest request, RestResponse response, RequestBack callBack)
        {
            SdkResponse sdkRes = new SdkResponse();
            try
            {
                if (true == response.TimedOut)
                {
                    sdkRes.errCode = SdkErrCode.TIMEOUT;
                    sdkRes.content = "连接超时";

                }
                //未知异常(自定义)
                else if (null != response.UnKnowException)
                {
                    sdkRes.errCode = SdkErrCode.NET_UNUSUAL;
                    sdkRes.content = "网络异常";

                    if (response.UnKnowException is WebException)
                    {
                        WebException ex = response.UnKnowException as WebException;
                        if (WebExceptionStatus.RequestCanceled == ex.Status)
                        {

                            sdkRes.errCode = SdkErrCode.USER_CANCEL;
                            sdkRes.content = "Web Request is cancled.";

                        }
                    }

                }
                //网络异常(WebException)
                else if (null != response.InnerException || HttpStatusCode.OK != response.StatusCode)
                {
                    bool isUserCanceled = false;
                    if (response.InnerException is WebException)
                    {
                        WebException ex = response.InnerException as WebException;
                        if (WebExceptionStatus.RequestCanceled == ex.Status)
                        {
                            sdkRes.errCode = SdkErrCode.USER_CANCEL;
                            sdkRes.content = "Web Request is cancled.";
                            isUserCanceled = true;

                        }
                    }

                    if (!isUserCanceled)
                    {
                        try
                        {
                            ErrorRes resObject = null;
                            //if (state.dataType == DataType.XML)
                            if (request.Path.Contains(".xml") || request.Path.Contains(".XML"))
                            {
                                XElement xmlSina = XElement.Parse(response.Content);
                                if (null != xmlSina.Element("error_code"))
                                {
                                    //得到服务器标准错误信息
                                    XmlSerializer serializer = new XmlSerializer(typeof(ErrorRes));
                                    resObject = serializer.Deserialize(response.ContentStream) as ErrorRes;
                                }
                            }
                            else
                            {
                                DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ErrorRes));
                                resObject = ser.ReadObject(response.ContentStream) as ErrorRes;

                            }

                            if (null != resObject && resObject is ErrorRes)
                            {
                                sdkRes.errCode = SdkErrCode.SERVER_ERR;
                                sdkRes.specificCode = resObject.ErrCode;
                                sdkRes.content = resObject.ErrInfo;
                            }
                            else
                                throw new Exception();
                        }
                        catch//如果没有error_code这个节点...
                        {
                            //不是xml
                            //网络异常时统一错误：NET_UNUSUAL
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                sdkRes.errCode = SdkErrCode.NET_UNUSUAL;
                                sdkRes.content = "网络状况异常";
                            }
                            else
                            {
                                sdkRes.errCode = SdkErrCode.SERVER_ERR;
                                sdkRes.specificCode = response.StatusCode.ToString();
                                sdkRes.content = response.Content;
                            }

                        }
                    }

                }
                else
                {
                    sdkRes.content = response.Content;
                    sdkRes.stream = response.ContentStream;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message);
                //日志
                sdkRes.errCode = SdkErrCode.SERVER_ERR;
                sdkRes.content = "服务器返回信息异常";

            }

            if (null != callBack)
                callBack(sdkRes);
        }

    }
}

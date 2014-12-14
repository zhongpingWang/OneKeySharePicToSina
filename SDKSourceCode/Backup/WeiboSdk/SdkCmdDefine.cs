// ===============================================================================
// SdkCmdDefine.cs
// 功能:结构定义
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
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace WeiboSdk
{

    //自定义的ErrCode
    public enum SdkErrCode
    {
        //参数错误
        XPARAM_ERR = -1,
        //成功
        SUCCESS = 0,
        //网络不可用
        NET_UNUSUAL,
        //服务器返回异常
        SERVER_ERR,
        //访问超时
        TIMEOUT,
        //用户请求被取消
        USER_CANCEL

    }

    //public enum DataType
    //{
    //    XML = 0,
    //    JSON
    //}

    public class SdkResponse
    {
        public SdkErrCode errCode;
        public string specificCode;

        //public string requestID = "";
        public string content = "";
        public Stream stream = null;
    }

    public class SdkAuthError
    {
        public SdkErrCode errCode;
        public string specificCode = "";
        public string errMessage = "";
    }

    public class SdkAuthRes
    {
        public string userId = "";
        public string acessToken = "";
        public string acessTokenSecret = "";

        //refleshToken
    }

    [DataContract]
    public class SdkAuth2Res
    {
        [DataMember(Name = "access_token")]
        public string accesssToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string refleshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public string expriesIn { get; set; }
    }

    [DataContract]
    public class OAuthErrRes
    {
        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "error_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "error_description")]
        public string errDes { get; set; }
    }

    public delegate void OAuth1LoginBack(bool isSucess, SdkAuthError err, SdkAuthRes response);
    public delegate void OAuth2LoginBack(bool isSucess, SdkAuthError err, SdkAuth2Res response);

    /// <summary>
    /// 失败时返回的对象(外部接口)
    /// </summary>
    [XmlRoot("hash")]
    [DataContract]
    public class ErrorRes
    {
        [XmlElement("request")]
        [DataMember(Name = "request")]
        public string Request { get; set; }

        [XmlElement("error_code")]
        [DataMember(Name = "error_code")]
        public string ErrCode { get; set; }

        [XmlElement("error")]
        [DataMember(Name = "error")]
        public string ErrInfo { get; set; }

        //public string InnErrcode
        //{
        //    get
        //    {
        //        string err = "";
        //        if (!string.IsNullOrEmpty(ErrInfo))
        //        {
        //            int pos = ErrInfo.IndexOf(":");
        //            if (-1 != pos)
        //            {
        //                err = ErrInfo.Substring(0, pos);
        //            }
        //        }
        //        return err;
        //    }
        //}
    }

    public enum SdkRequestType
    {
        NULL_TYPE = -1,
        FRIENDS_TIMELINE = 0,        //获取下行数据集(timeline)接口(cmdNormalMessages)
        UPLOAD_MESSAGE,             //发送微博(cmdUploadMessage)
        UPLOAD_MESSAGE_PIC,         //发送带图片微博(cmdUploadPic)
        FRIENDSHIP_CREATE,          //关注某用户(cmdFriendShip)
        FRIENDSHIP_DESDROY,         //取消关注(cmdFriendShip)
        FRIENDSHIP_SHOW,            //获取两个用户关系的详细情况(cmdFriendShip)
        AT_USERS,                   //@用户时的联想建议 (cmdAtUsers)
        USER_TIMELINE,              //获取用户发布的微博消息列表(cdmUserTimeline)
    }

    //鉴权方式
    public enum EumAuth
    {
        OAUTH1_0 = 0,
        OAUTH2_0
    }


    public class SdkCmdBase
    {
        public string acessToken = "";
        //public string acessTokenSecret = "";

        //public string requestId = "";
    }

    /// <summary>
    /// FRIENDS_TIMELINE
    /// </summary>
    public class cmdNormalMessages : SdkCmdBase
    {
        public string id = "";
        public string sinceID = "";
        public string maxID = "";
        public string count = "";
        public string page = "";
        public string feature = "";
        public string baseApp = "";
    }

    /// <summary>
    /// USER_TIMELINE
    /// </summary>
    public class cdmUserTimeline : SdkCmdBase
    {
        public string userId = "";
        public string screenName = "";
        public string sinceID = "";
        public string maxID = "";
        public string count = "";
        public string page = "";
        public string baseApp = "";
        public string feature = "";
    }

    /// <summary>
    /// UPLOAD_MESSAGE
    /// </summary>
    public class cmdUploadMessage : SdkCmdBase
    {
        public string status = "";
        public string ReplyId = "";
        public string lat = "";
        public string _long = "";
        public string annotations = "";

    }

    /// <summary>
    /// UPLOAD_MESSAGE_PIC
    /// </summary>
    public class cmdUploadPic : SdkCmdBase
    {
        public string messageText = "";
        public string lat = "";
        public string _long = "";
        public string picPath = "";

    }

    public class cmdFriendShip : SdkCmdBase
    {
        //源用户(如果不填，则默认取当前登录用户)
        public string _sourceId = "";
        public string _sourceScreenName = "";
        //目标用户
        public string _userId = "";
        public string _screenName = "";

        //public bool isRemoveFans = false;
    }

    public class cmdAtUsers : SdkCmdBase
    {
        /// <summary>
        /// 搜索的关键字。必须进行URL_encoding。UTF-8编码
        /// </summary>
        public string _keyword = "";

        /// <summary>
        /// 每页返回结果数。默认10
        /// </summary>
        public string _count = "";

        /// <summary>
        /// 1代表粉丝，0代表关注人
        /// </summary>
        public string _type = "";

        /// <summary>
        /// 0代表只查关注人，1代表只搜索当前用户对关注人的备注，2表示都查. 默认为2.
        /// </summary>
        public string _range = "";
    }

}

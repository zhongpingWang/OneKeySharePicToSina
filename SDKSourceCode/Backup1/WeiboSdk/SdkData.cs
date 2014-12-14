// SdkData.cs
// 功能:全局数据类
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

namespace WeiboSdk
{
     public class SdkData
    {
         static public string RedirectUri = "";

         static public string UserAgent { get; set; }
         static public  string AppKey { get; set; }
         static public  string AppSecret { get; set; }

         //默认OAuth2.0
         static readonly internal EumAuth AuthOption = EumAuth.OAUTH2_0;
    }
}

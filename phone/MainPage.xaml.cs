using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using phone.SinaApi;
using WeiboSdk;
using WeiboSdk.PageViews;
using phone.Model;
using phone.Helper;
using Hammock;
using Hammock.Web;
using System.Runtime.Serialization.Json;

namespace phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            SdkData.AppKey = SinaApi.ApiSina.AppKey;
            SdkData.AppSecret = SinaApi.ApiSina.AppSecret;

            // 您app设置的重定向页,必须一致
            SdkData.RedirectUri = ApiSina.RedirectUri;
           
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserInfo user=new UserInfo();
            IsolatedStorageHelper.GetIsolatedStorage<UserInfo>("loveUser",out user);
            if ( user!=null && !string.IsNullOrEmpty(user.AccessToken))
            {
                 NavigationService.Navigate(new Uri("/PageViews/MainPage.xaml",
                            UriKind.Relative));
            }
        }


        //授权
        private void recall_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SdkData.AppKey) || string.IsNullOrEmpty(SdkData.AppSecret)
               || string.IsNullOrEmpty(SdkData.RedirectUri))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("请在中MainPage.xmal.cs的构造函数中设置自己的appkey、appkeysecret、RedirectUri.");
                });
                return;
            }

            //授权
            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
            AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);
            //其它通知事件...

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                    , UriKind.Relative));
            });


              

           
        }

        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="isSucess"></param>
        /// <param name="errCode"></param>
        /// <param name="response"></param>
        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {

            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App.AccessToken = response.accesssToken;
                    App.RefleshToken = response.refleshToken; 
                }
                //获取用户ID
                GetUserId();
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{ 
                //  //  NavigationService.Navigate(new Uri("/PageViews/MainPage.xaml",
                //  //      UriKind.Relative));
                //});
            }
            else if (errCode.errCode == SdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == SdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }
           


        }
       
        /// <summary>
        /// 获取用户ID
        /// </summary>
        private void GetUserId() 
        {
            
            SdkCmdBase data = new SdkCmdBase
            { 
                acessToken = App.AccessToken,
            };

            SdkNetEngine net = new SdkNetEngine();
            RestRequest request = new RestRequest();
            //设置request
            request.Method = WebMethod.Get;
            request.Path = "/account/get_uid.json";
            request.AddParameter("access_token", App.AccessToken);

            //发送请求
            net.SendRequest(request, data, (e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserInfo));
                    UserInfo user = ser.ReadObject(e1.stream) as UserInfo;
                    //获取用户详情
                    GetUserInfo(user.uid);
                    //Debug.WriteLine(e1.content);
                }
                else if (e1.errCode == SdkErrCode.SERVER_ERR)
                {
                    MessageBox.Show("服务器返回错误，错误码:" + e1.specificCode);
                    //Debug.WriteLine("服务器返回错误，错误码:" + e1.specificCode);
                }
                else if (e1.errCode == SdkErrCode.NET_UNUSUAL)
                {
                    MessageBox.Show("当前无网络"); 
                }
            });
        }
        
        /// <summary>
        /// 获取用户详情
        /// </summary>
        private void GetUserInfo(string uid) {
            SdkCmdBase data = new SdkCmdBase
            {
                acessToken = App.AccessToken,
            };

            SdkNetEngine net = new SdkNetEngine();
            RestRequest request = new RestRequest();
            //设置request
            request.Method = WebMethod.Get;
            request.Path = "/users/show.json";
            request.AddParameter("access_token", App.AccessToken);
            request.AddParameter("uid", uid);

            //发送请求
            net.SendRequest(request, data, (e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserInfo));
                    UserInfo user = ser.ReadObject(e1.stream) as UserInfo;
                    user.AccessToken = App.AccessToken;
                    user.AccessTokenSecret = App.AccessTokenSecret;
                    user.RefleshToken = App.RefleshToken;
                   // List<UserInfo> userList = new List<UserInfo>(); 
                    IsolatedStorageHelper.SetIsolatedStorage<UserInfo>("loveUser", user);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/PageViews/MainPage.xaml",
                            UriKind.Relative));
                    });
                    //Debug.WriteLine(e1.content);
                }
                else if (e1.errCode == SdkErrCode.SERVER_ERR)
                {
                    MessageBox.Show("服务器返回错误，错误码:" + e1.specificCode);
                    //Debug.WriteLine("服务器返回错误，错误码:" + e1.specificCode);
                }
                else if (e1.errCode == SdkErrCode.NET_UNUSUAL)
                {
                    MessageBox.Show("当前无网络"); 
                    //Debug.WriteLine("当前无网络");
                }
            });
        
        }



        /// <summary>
        /// 取消授权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }

    }
}
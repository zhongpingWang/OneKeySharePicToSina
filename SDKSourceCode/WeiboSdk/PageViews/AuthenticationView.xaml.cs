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
using Microsoft.Phone.Shell;
using System.Windows.Data;

namespace WeiboSdk.PageViews
{
    public partial class AuthenticationView : PhoneApplicationPage
    {
        #region ProgressIndicatorIsVisibleProperty
        public static readonly DependencyProperty ProgressIndicatorIsVisibleProperty =
            DependencyProperty.Register("ProgressIndicatorIsVisible",
            typeof(bool),
            typeof(AuthenticationView),
            new PropertyMetadata(false));

        public bool ProgressIndicatorIsVisible
        {
            get { return (bool)GetValue(ProgressIndicatorIsVisibleProperty); }
            set { SetValue(ProgressIndicatorIsVisibleProperty, value); }
        }
        #endregion

        public AuthenticationView()
        {
            InitializeComponent();
        }

        #region 成员变量
        static public OAuth1LoginBack OAuthVerifyCompleted { get; set; }

        static public OAuth2LoginBack OAuth2VerifyCompleted { get; set; }

        static public EventHandler OBrowserCancelled { get; set; }

        static public EventHandler OBrowserNavigated { get; set; }

        static public EventHandler OBrowserNavigating { get; set; }
        #endregion

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = "数据传输中";
            

            Binding bindingData;
            bindingData = new Binding("ProgressIndicatorIsVisible");
            bindingData.Source = this;
            BindingOperations.SetBinding(SystemTray.ProgressIndicator, ProgressIndicator.IsVisibleProperty, bindingData);
            BindingOperations.SetBinding(SystemTray.ProgressIndicator, ProgressIndicator.IsIndeterminateProperty, bindingData);

            //事件通知
            oauthControl.OBrowserNavigating = new EventHandler(oauthBrowserNavigating);
            oauthControl.OBrowserNavigated = new EventHandler(oauthBrowserNavigated);
            oauthControl.OBrowserCancelled = new EventHandler(oauthBrowserCancelled);
            oauthControl.OAuthVerifyCompleted = OAuthCallBack;
            oauthControl.OAuth2VerifyCompleted = OAuth2CallBack;
        }

        void OAuthCallBack(bool isSucess, SdkAuthError err, SdkAuthRes response)
        {
            if (null != OAuthVerifyCompleted)
                OAuthVerifyCompleted(isSucess, err, response);
        }
        void OAuth2CallBack(bool isSucess, SdkAuthError err, SdkAuth2Res response)
        {
            if (null != OAuth2VerifyCompleted)
                OAuth2VerifyCompleted(isSucess, err, response);
        }
        void oauthBrowserCancelled(object sender, EventArgs e)
        {
            ProgressIndicatorIsVisible = false;
            //new Game().Exit();
            if (null != OBrowserCancelled)
                OBrowserCancelled.Invoke(sender,e);
        }

        void oauthBrowserNavigating(object sender, EventArgs e)
        {
            ProgressIndicatorIsVisible = true;
            if (null != OBrowserNavigating)
                OBrowserNavigating.Invoke(sender, e);
        }

        void oauthBrowserNavigated(object sender, EventArgs e)
        {
            ProgressIndicatorIsVisible = false;
            if (null != OBrowserNavigated)
                OBrowserNavigated.Invoke(sender, e);
        }
    }
}
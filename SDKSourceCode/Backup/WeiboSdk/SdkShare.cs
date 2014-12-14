using System;
using System.Windows;
using Microsoft.Phone.Controls;


namespace WeiboSdk
{
    public class SdkShare : SdkSendBase
    {
        public override void Show()
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/WeiboSdk;component/PageViews/SharePageView.xaml", UriKind.Relative));
            SharePageView.sdkSendBase = this;
        }

    }
}

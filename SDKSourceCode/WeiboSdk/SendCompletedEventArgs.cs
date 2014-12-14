using System;

namespace WeiboSdk
{
    public class SendCompletedEventArgs : EventArgs
    {
        public bool IsSendSuccess { get;internal set; }
        public SdkErrCode ErrorCode { get; internal set; }
        public string Response { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phone.Model
{
    /// <summary>
    /// 用户类
    /// </summary>
     public class UserInfo
    {
        private string _userName;
         /// <summary>
         /// 用户名
         /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }


        private string _passWord;
         //密码
        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }

        private string _accessToken;
         /// <summary>
        /// AccessToken
         /// </summary>
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        private string _accessTokenSecret;
         /// <summary>
        /// AccessTokenSecret
         /// </summary>
        public string AccessTokenSecret
        {
            get { return _accessTokenSecret; }
            set { _accessTokenSecret = value; }
        }

        private string _refleshToken;
         /// <summary>
        /// RefleshToken
         /// </summary>
        public string RefleshToken
        {
            get { return _refleshToken; }
            set { _refleshToken = value; }
        }

        private string _uid;
         /// <summary>
         /// 用户ID
         /// </summary>
        public string uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

         /// <summary>
         /// 上次同步时间
         /// </summary>
        public DateTime prevTime { get; set; }


        public string id { get; set; }
        public string idstr { get; set; }
        public string screen_name { get; set; }
        public string name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string profile_image_url { get; set; }
        public string profile_url { get; set; }
        public string domain { get; set; }
        public string weihao { get; set; }
        public string gender { get; set; }
        public string followers_count { get; set; }
        public string friends_count { get; set; }
        public string statuses_count { get; set; }
        public string favourites_count { get; set; }
        public string created_at { get; set; }
        public bool following { get; set; }
        public bool allow_all_act_msg { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public string IsVerified
        {
            get
            {
                return verified ? "Visible" : "Collapsed";
            }
        }
        public string verified_type { get; set; }
        public string remark { get; set; }
        public bool allow_all_comment { get; set; }
        public string avatar_large { get; set; }
        public string verified_reason { get; set; }
        public bool follow_me { get; set; }
        public string online_status { get; set; }
        public string bi_followers_count { get; set; }
        public string lang { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using phone.Model;
using phone.Helper;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using WeiboSdk;
using Hammock;
using Hammock.Web;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Shapes;
using Hammock.Silverlight.Compat;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Coding4Fun.Phone.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace phone.PageViews
{
    public partial class MainPage : PhoneApplicationPage
    {

        UserInfo userinfo = new UserInfo();
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 页面启动时
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IsolatedStorageHelper.GetIsolatedStorage<UserInfo>("loveUser", out userinfo);
            userName.Text = userinfo.name;
            userDesc.Text = userinfo.description;
            Dispatcher.BeginInvoke(() =>
            {
                uerImg.Source = new BitmapImage(new Uri(userinfo.profile_image_url));
                loading.Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// 用户设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uerImg_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// 发表微博
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionBtn_Click(object sender, RoutedEventArgs e)
        {
            MediaLibrary media = new MediaLibrary();
            // PictureCollection pics = 
            //IEnumerable<PictureCollection> pics =
            //    from s in media.Pictures
            //    where s.Album.Name.Contains("Camera Roll") group s.Name by s.Name;


            PictureCollection pics = media.Pictures;
            DateTime prev = userinfo.prevTime;
            Helper.IsolatedStorageHelper.GetIsolatedStorage<UserInfo>("loveUser", out userinfo);
            Picture pic; 
            List<string> pathList = new List<string>();
            List<Picture> cameraRollPic = new List<Picture>();
            for (int i = 0; i < pics.Count; i++)
            {
                pic = pics[i];

                if (pic.Album.Name.Contains("Camera Roll"))
                {
                    if (pic.Date > prev)
                    {
                        cameraRollPic.Add(pic);
                    } 
                     //pathList.Add(Microsoft.Xna.Framework.Media.PhoneExtensions.MediaLibraryExtensions.GetPath(pic));
                     //if (pathList.Count==10)
                     //{ 
                     //}
                }
               
            } 
 
           
            if (cameraRollPic != null && cameraRollPic.Count > 0)
            {
                foreach (Picture singlePic in cameraRollPic)
                {
                    //Task t1 = new Task(UpdateWeiboTask, singlePic);
                    //t1.Start();
                    //Thread.Sleep(10000);
                    //userinfo.prevTime = singlePic.Date;

                    saveToIsolateStorage(singlePic);
                    UpdateWeibo(@"SdkImage\\Camera Roll");
                    Thread.Sleep(3000);
                    userinfo.prevTime = singlePic.Date;
                  
                }
            }

            //saveToIsolateStorage(pics[0]);
            //UpdateWeibo(@"SdkImage\\Camera Roll"); 
            
            IsolatedStorageHelper.SetIsolatedStorage<UserInfo>("loveUser", userinfo);

            //saveToIsolateStorage(pics[0]);
            //UpdateWeibo(@"SdkImage\\Camera Roll");


            //BitmapImage bit = new BitmapImage();
            //bit.SetSource(pics[0].GetImage());
            //nicai.Source = bit;
            //saveToIsolateStorage(pics[0]);
            //UpdateWeibo(@"SdkImage\\Camera Roll");
            ////还有数据
            //if (pathList.Count > 0)
            //{
            //    for (int i = 0; i < pics.Count; i++)
            //    {

            //    }
            //    UpdateWeibo(@"SdkImage\\Camera Roll");
            //}
             
        }

       
        private void UpdateWeiboTask(object single)
        {
            Picture singlePic = single as Picture;
            saveToIsolateStorage(singlePic);
            UpdateWeibo(@"SdkImage\\Camera Roll");
            userinfo.prevTime = singlePic.Date;
        }


        private void UpdateWeibo(string picPath)
        {
            SdkCmdBase data = new SdkCmdBase
            {
                acessToken = App.AccessToken,
            };
            WeiboSdk.cmdUploadMessage c = new cmdUploadMessage();
            WeiboSdk.cmdUploadPic pic = new cmdUploadPic();


            SdkNetEngine net = new SdkNetEngine();
            RestRequest request = new RestRequest();

            //设置request
            request.Method = WebMethod.Post;

            //添加鉴权
            // request.DecompressionMethods = DecompressionMethods.GZip;
            // request.Encoding = Encoding.UTF8;
            // request.Path = "/statuses/upload.json"; 

            // var oAuthHeader = "OAuth2 access_token";
            // request.AddParameter("24", oAuthHeader);
            //string boundary = "---------------------------7db2b61c40302"; // TODO: Random it
            //string contentType = string.Format("multipart/form-data; boundary={0}", boundary);
            //request.AddParameter("Content-Type", contentType);
            //request.AddHeader("Authorization", string.Format("OAuth2 {0}", App.AccessToken));
            //request.AddParameter("access_token", App.AccessToken);
            //request.AddParameter("status", HttpUtility.HtmlEncode(DateTime.Now.ToString()));
            //request.AddParameter("visible", "1");
            // request.AddParameter("pic", "1");
            //string picType = System.IO.Path.GetExtension(picPath);
            //string picName = System.IO.Path.GetFileName(picPath);
            //if (".png" == picType)
            //{
            //    request.AddFile("pic", picName, picPath, "image/png");
            //}
            //else
            //{
            //    request.AddFile("pic", picName, picPath, "image/jpeg");
            //}



            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (!file.FileExists(picPath))
            {
                return;
            }
            file.Dispose();
            string picType = System.IO.Path.GetExtension(picPath);
            string picName = System.IO.Path.GetFileName(picPath);
            if ("png" == picType)
            {
                request.AddFile("pic", picName, picPath, "image/png");
            }
            else
            {
                request.AddFile("pic", picName, picPath, "image/jpeg");
            }



            cmdUploadPic cmdBase = new cmdUploadPic
            {
                messageText = DateTime.Now.ToString(),
                picPath = picPath,
                visible = 1,
                acessToken = App.AccessToken,

                //acessTokenSecret = sdkSendBase.AccessTokenSecret
            };


            //发送请求
            net.RequestCmd(SdkRequestType.UPLOAD_MESSAGE_PIC, cmdBase, (e2, e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        Coding4FunForMsg("上传成功！", "", 1000);
                    });

                    //Debug.WriteLine(e1.content);
                }
                else if (e1.errCode == SdkErrCode.SERVER_ERR)
                {
                    
                    if (e1.specificCode == "21301")
                    {

                        Dispatcher.BeginInvoke(() =>
                        {
                            userinfo.AccessToken = "";
                            IsolatedStorageHelper.SetIsolatedStorage<UserInfo>("loveUser", userinfo);
                            NavigationService.Navigate(new Uri("/MainPage.xaml",
                           UriKind.Relative));
                        });

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("服务器返回错误，错误码:" + e1.specificCode);
                        });

                    }
                    //Debug.WriteLine("服务器返回错误，错误码:" + e1.specificCode);
                }
                else if (e1.errCode == SdkErrCode.NET_UNUSUAL)
                {
                    IsolatedStorageHelper.SetIsolatedStorage<UserInfo>("loveUser", userinfo);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("当前无网络");
                    });

                    //Debug.WriteLine("当前无网络");
                }
            });

        }

        string _imageFileName;

        private void PhotoFix(PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var dirs = e.OriginalFileName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                _imageFileName = (dirs[5] != null) ? dirs[5] : "TempJPEG";

                BitmapImage bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                bmp.SetSource(e.ChosenPhoto);
                bmp.ImageOpened += new EventHandler<RoutedEventArgs>(bmp_ImageOpened);
            }
            
        }

        void bmp_ImageOpened(object sender, RoutedEventArgs e)
        {
            BitmapImage bmp = sender as BitmapImage;
            bmp.ImageOpened -= new EventHandler<RoutedEventArgs>(bmp_ImageOpened);
           
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string dirPath = "SdkImage";
                _imageFileName = System.IO.Path.Combine(dirPath, _imageFileName);

                if (!myStore.DirectoryExists(dirPath))
                    myStore.CreateDirectory(dirPath);
                if (myStore.FileExists(_imageFileName))
                    myStore.DeleteFile(_imageFileName);

                using (var myFileStream = myStore.CreateFile(_imageFileName))
                {
                    WriteableBitmap wb = new WriteableBitmap(bmp);
                    Extensions.SaveJpeg(wb, myFileStream, wb.PixelWidth, wb.PixelHeight, 0, 90);
                }
            }
             
        }


        private void saveToIsolateStorage(Picture pic)
        { 
            BitmapImage bmp= new BitmapImage();  
            bmp.SetSource(pic.GetImage()); 
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string dirPath = "SdkImage";
                string imageFileName = "Camera Roll";
                imageFileName = System.IO.Path.Combine(dirPath, imageFileName);

                if (!myStore.DirectoryExists(dirPath))
                    myStore.CreateDirectory(dirPath);
                if (myStore.FileExists(imageFileName))
                    myStore.DeleteFile(imageFileName);

                using (var myFileStream = myStore.CreateFile(imageFileName))
                { 
                    WriteableBitmap wb = new WriteableBitmap(bmp);
                    Extensions.SaveJpeg(wb, myFileStream, wb.PixelWidth, wb.PixelHeight, 0, 90); 
                }
            }

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PhotoChooserTask photoPicker = new PhotoChooserTask();
            //photoPicker.ShowCamera = false;
            //photoPicker.Completed += new EventHandler<PhotoResult>(photoPicker_Completed);
            //photoPicker.Show();

            //新建一个 SdkSend 实例
            SdkShare sdkSend = new SdkShare();
            //设置OAuth2.0的access_token
            sdkSend.AccessToken = App.AccessToken;
            //sdkSend.AccessTokenSecret = App.AccessTokenSecret;
            //定义发送微博完毕的回调函数
            sdkSend.Completed = SendCompleted;

            //调用Show方法展现页面的跳转
            sdkSend.Show();

        }

        void SendCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.IsSendSuccess)
                MessageBox.Show("发送成功");
            else
                MessageBox.Show(e.Response, e.ErrorCode.ToString(), MessageBoxButton.OK);
        }

        void photoPicker_Completed(object sender, PhotoResult e)
        {
            (sender as PhotoChooserTask).Completed -= new EventHandler<PhotoResult>(photoPicker_Completed);
            PhotoFix(e);
        }



        DateTime backTime = DateTime.Now; 
        //在按一次推出
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if ((DateTime.Now - backTime).TotalSeconds >= 2)
            { 
                e.Cancel = true;

                Coding4FunForMsg("在按一次退出", "", 1000); 
                backTime = DateTime.Now;
            }
            else
            {
                Application.Current.Terminate();
            } 
            base.OnBackKeyPress(e);
        }



        /// <summary>  
        /// 信息提示  
        /// </summary>  
        /// <param name="content">提示的信息内容</param>  
        /// <param name="title">提示的标题</param>  
        /// <param name="timeout">提示消息的显示过期时间。单位毫秒</param>  
        public void Coding4FunForMsg(string content, string title, int timeout)
        {
            SolidColorBrush White = new SolidColorBrush(Colors.White);
            SolidColorBrush Red = new SolidColorBrush(Colors.Red);
            ToastPrompt toast = new ToastPrompt
            {
                Background = Red,
                IsTimerEnabled = true,
                IsAppBarVisible = true,
                MillisecondsUntilHidden = timeout,
                Foreground = White,
            };
            // toast.Title = title;
            toast.Message = content;
            toast.VerticalAlignment = VerticalAlignment.Top;
            //toast.Margin = new Thickness(0, 0, 0, 0);
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

    }
}
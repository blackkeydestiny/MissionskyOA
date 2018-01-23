using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.jpush.api;
using cn.jpush.api.push.mode;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.push.notification;

namespace MissionskyOA.Services
{
    public static class JPushExtensions
    {
        public static String TITLE = "Test from C# v3 sdk";
        public static String ALERT = "MissionSky OA Platform Message - alert";
        public static String MSG_CONTENT = "Test from MissionSky OA Platform - msgContent";
        public static String REGISTRATION_ID = "0900e8d85ef";
        public static String TAG = "tag_api";
        public static String app_key = "34dd60276a6704831bb1e3a4";
        public static String master_secret = "49a34b06a0100ee51dcc24ad";

        //static void Main(string[] args)
        //{
        //    Console.WriteLine("*****开始发送******");
        //    JPushClient client = new JPushClient(app_key, master_secret);
        //    PushPayload payload = PushObject_All_All_Alert();
        //    try
        //    {
        //        var result = client.SendPush(payload);
        //        //由于统计数据并非非是即时的,所以等待一小段时间再执行下面的获取结果方法
        //        System.Threading.Thread.Sleep(10000);
        //        /*如需查询上次推送结果执行下面的代码*/
        //        var apiResult = client.getReceivedApi(result.msg_id.ToString());
        //        var apiResultv3 = client.getReceivedApi_v3(result.msg_id.ToString());
        //        /*如需查询某个messageid的推送结果执行下面的代码*/
        //        var queryResultWithV2 = client.getReceivedApi("1739302794");
        //        var querResultWithV3 = client.getReceivedApi_v3("1739302794");

        //    }
        //    catch (APIRequestException e)
        //    {
        //        Console.WriteLine("Error response from JPush server. Should review and fix it. ");
        //        Console.WriteLine("HTTP Status: " + e.Status);
        //        Console.WriteLine("Error Code: " + e.ErrorCode);
        //        Console.WriteLine("Error Message: " + e.ErrorCode);
        //    }
        //    catch (APIConnectionException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    Console.WriteLine("*****结束发送******");
        //}

        public delegate PushPayload AsyncDelegate(string alias, string text);
        public static void CallbackMethod(IAsyncResult ar)
        {
            // Retrieve the delegate.      
            AsyncDelegate dlgt = (AsyncDelegate)ar.AsyncState;
            // Call EndInvoke to retrieve the results.              
            PushPayload ret = dlgt.EndInvoke(ar);
        }

        public static PushPayload PushObject_All_All_Alert()
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.all();
            pushPayload.notification = new Notification().setAlert(ALERT);
            return pushPayload;
        }

        public static PushPayload PushObject_Android_And_IOS_AliasAlert(string alias, string text)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.s_alias(alias);
            pushPayload.notification = new Notification().setAlert(text);
            return pushPayload;

        }

        public static void SendPush(PushPayload payload, JPushClient client)
        {
            try
            {
                var result = client.SendPush(payload);
                //由于统计数据并非非是即时的,所以等待一小段时间再执行下面的获取结果方法
                //System.Threading.Thread.Sleep(10000);
                /*如需查询上次推送结果执行下面的代码*/
                //var apiResultv3 = client.getReceivedApi_v3(result.msg_id.ToString());
            }
            catch (APIRequestException e)
            {
                Console.WriteLine("Error response from JPush server. Should review and fix it. ");
                Console.WriteLine("HTTP Status: " + e.Status);
                Console.WriteLine("Error Code: " + e.ErrorCode);
                Console.WriteLine("Error Message: " + e.ErrorCode);
            }
            catch (APIConnectionException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 按别名发送
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="text"></param>
        /// <param name="parameters"></param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        public static PushPayload PushObject_android_and_ios(string[] alias, string title, string text, string parameters, bool isProduction)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.android_ios();
            pushPayload.options.apns_production = isProduction;
            var audience = Audience.s_alias(alias);
            pushPayload.audience = audience;
            var notification = new Notification().setAlert(text);

            notification.IosNotification = new IosNotification();
            notification.AndroidNotification = new AndroidNotification();
            notification.AndroidNotification.setTitle(title);
            notification.IosNotification.setCategory(title);
            notification.IosNotification.incrBadge(1);
            notification.AndroidNotification.AddExtra("params", parameters);
            notification.IosNotification.AddExtra("params", parameters);
            pushPayload.notification = notification.Check();

            return pushPayload;
        }

        /// <summary>
        /// 发送广播给老师或学生,或者All
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="extraId"></param>
        /// <param name="extraMemberId"></param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        public static PushPayload PushBroadcast(string[] tags, string title, string text, string parameters, bool isProduction)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.android_ios();
            pushPayload.options.apns_production = isProduction;
            if (tags == null || tags.Length == 0)
            {
                pushPayload.audience = Audience.all();
            }
            else
            {
                pushPayload.audience = Audience.s_tag(tags);
            }

            var notification = new Notification().setAlert(text);

            notification.AndroidNotification = new AndroidNotification();
            notification.AndroidNotification.setTitle(title);
            notification.AndroidNotification.AddExtra("params", parameters);

            notification.IosNotification = new IosNotification();
            notification.IosNotification.setCategory(title);
            notification.IosNotification.incrBadge(1);
            notification.IosNotification.AddExtra("params", parameters);

            pushPayload.notification = notification.Check();

            return pushPayload;
        }

        public static PushPayload PushObject_ios_tagAnd_alertWithExtrasAndMessage()
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.android_ios();
            pushPayload.audience = Audience.s_tag_and("tag1", "tag_all");
            var notification = new Notification();
            notification.IosNotification = new IosNotification().setAlert(ALERT).setBadge(5).setSound("happy").AddExtra("from", "JPush");

            pushPayload.notification = notification;
            pushPayload.message = Message.content(MSG_CONTENT);
            return pushPayload;

        }
        public static PushPayload PushObject_ios_audienceMore_messageWithExtras()
        {

            var pushPayload = new PushPayload();
            pushPayload.platform = Platform.android_ios();
            pushPayload.audience = Audience.s_tag("tag1", "tag2");
            pushPayload.message = Message.content(MSG_CONTENT).AddExtras("from", "JPush");
            return pushPayload;

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codeplex.Data;
using MrHuo.OAuthLoginLibs.Core;
using MrHuo.OAuthLoginLibs.Interfaces;
using RestSharp;

namespace MrHuo.OAuthLogin.QQApis
{
    /// <summary>
    /// 获取QQ用户信息上下文示例代码
    /// </summary>
    public class QQContext : OAuthContextBase<OAuthToken, QQUserInfo>
    {
        public QQContext(OAuthLoginConfig config, string accessTokenCallbackString)
            : base(config, accessTokenCallbackString)
        {
            //qq的callbackstring和别的平台不一样，所以自己根据开发文档写
            if (accessTokenCallbackString.Contains("callback"))
            {
                throw new Exception("QQContext.QQContext(config,accessTokenCallbackString) Error >> " + accessTokenCallbackString);
            }
        }

        /// <summary>
        /// 获取OAuthToken
        /// </summary>
        /// <returns></returns>
        public override OAuthToken GetOAuthToken()
        {
            InnerHelper.ResponseError(base._accessCallbackString);

            var token = base._accessCallbackString.QueryStringToObject();
            var url = "oauth2.0/me?access_token=" + token.access_token;

            RestRequest request = new RestRequest(url, Method.GET);
            var response = base.Request(request);

            var callback = InnerHelper.ClearQQCallback(response);

            return new OAuthToken()
            {
                OpenId = callback.openid,
                AccessToken = token.access_token,
                ExpiresAt = int.Parse(token.expires_in)
            };
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public override QQUserInfo GetUserInfo()
        {
            var authToken = this.GetOAuthToken();

            var url = string.Format("user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}", authToken.AccessToken, base._config.AppKey, authToken.OpenId);
            RestRequest request = new RestRequest(url, Method.GET);
            var response = base.Request(request);

            //QQ的用户信息中含有回车符和替换付，所以全部替换掉
            response = response.Replace("\r", "").Replace("\n", "");
            var obj = DynamicJson.Parse(response);

            if (obj.ret > 0)
            {
                throw new Exception("QQContext.GetUserInfo Error >> " + obj);
            }

            var user = new QQUserInfo()
            {
                NickName = obj.nickname,
                Avatar = obj.figureurl_qq_1,
                Gender = obj.nickname
            };

            return user;
        }

        class InnerHelper
        {
            public static void ResponseError(string response)
            {
                if (response.Contains("error"))
                {
                    throw new Exception("QQContext发生错误：" + response);
                }
            }
            public static dynamic ClearQQCallback(string content)
            {
                Regex regCallback = new Regex("callback\\((.+?)\\);");
                if (regCallback.IsMatch(content))
                {
                    var ret = regCallback.Match(content).Groups[1].Value.Trim();
                    var obj = ret.StringToObject();
                    return obj;
                }
                throw new Exception("发生异常：" + content);
            }
        }
    }
}

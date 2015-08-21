using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrHuo.OAuthLogin.QQApis
{
    /// <summary>
    /// 需要获取的QQ用户信息
    /// </summary>
    public class QQUserInfo
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
    }
}
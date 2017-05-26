using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClassificationDataFromSeeds
{
    class EntityPatternGenerator
    {
        public static void FadeMain()
        {
            string str = GetCrawlerUrl("https://zhidao.baidu.com/search?lm=0&rn=10&pn=0&fr=search&ie=gbk&word=天下无贼是什么时候的电影");
            Console.WriteLine(str);
        }
        public static string GetCrawlerUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            string encryptUrl = WebProxyTools.AESEncryption.EncryptUrl(url);
            return string.Format("http://www.bing.com/dict/proxy/proxy?k={0}", HttpUtility.UrlEncode(encryptUrl));
        }
    }

}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Com.Netease.Is.Antispam.Demo
{
    class CrawlerSolutionSubmitApiDemoV3
    {
        public static void crawlerSolutionSubmit()
        {     
            /** 产品密钥ID，产品标识 */
            String secretId = "c13768414e6cb63882faddec567e5229";
            /** 产品私有密钥，服务端生成签名信息使用，请严格保管，避免泄露 */
            String secretKey = "9df4774905c05eda5dea30f6f9cb3521";
            /** 易盾反垃圾网站检测解决方案提交接口地址  */
            String apiUrl = "http://as.dun.163.com/v3/crawler/submit";
            Dictionary<String, String> parameters = new Dictionary<String, String>();

            long curr = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            String time = curr.ToString();

            // 1.设置公共参数
            parameters.Add("secretId", secretId);
            parameters.Add("version", "v3.0");
            parameters.Add("timestamp", time);
            parameters.Add("nonce", new Random().Next().ToString());

            // 2.设置私有参数
            parameters.Add("dataId", "ebfcad1c-dba1-490c-b4de-e784c2691768");
            parameters.Add("callback", "34eb8bfdf03f209fcfc02");
            parameters.Add("url", "http://xxx.xxx.com/xxxx");
            // 多个检测项时用英文逗号分隔
            parameters.Add("checkFlags", "1,2");
            // 3.生成签名信息
            String signature = Utils.genSignature(secretKey, parameters);
            parameters.Add("signature", signature);

            // 4.发送HTTP请求
            HttpClient client = Utils.makeHttpClient();
            String result = Utils.doPost(client, apiUrl, parameters, 1000);
            if(result != null)
            {
                JObject ret = JObject.Parse(result);
                int code = ret.GetValue("code").ToObject<Int32>();
                String msg = ret.GetValue("msg").ToObject<String>();
                if (code == 200)
                {
                    JObject resultObject = (JObject)ret["result"];
                    String taskId = resultObject["taskId"].ToObject<String>();
                    String dataId = resultObject["dataId"].ToObject<String>();
                    Console.WriteLine(String.Format("SUCCESS: taskId={0}, dataId={1}", taskId, dataId));
                }
                else
                {
                    Console.WriteLine(String.Format("ERROR: code={0}, msg={1}", code, msg));
                }
            }
            else
            {
                Console.WriteLine("Request failed!");
            }

        }      
    }
}

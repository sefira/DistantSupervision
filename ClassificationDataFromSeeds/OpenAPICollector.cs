using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassificationDataFromSeeds
{
    // generate OpenAPI URL and HTML
    // when we have question pattern in QuestionAnswerPatternGenerator, we can generate the samiliar question by OpenAPI query
    // input: 1 movie_artist_pattern.txt 2 movie_director_pattern.txt 3 movie_publishdate_pattern.txt 4 movie_genres_pattern.txt 5 movie_country_pattern.txt 6 celebrity_act_pattern.txt 7 celebrity_direct_pattern.txt
    // output: 1 movie_artist_openapi.txt 2 movie_director_openapi.txt 3 movie_publishdate_openapi.txt 4 movie_genres_openapi.txt 5 movie_country_openapi.txt 6 celebrity_act_openapi.txt 7 celebrity_direct_openapi.txt
    class OpenAPICollector
    {
        public static void FakeMain()
        {
            OpenapiGoCollect(QuestionList.Movie_Artists);
            //OpenapiGoCollect(QuestionList.Movie_Directors);
            //OpenapiGoCollect(QuestionList.Movie_PublishDate);
            //OpenapiGoCollect(QuestionList.Movie_Genres);
            //OpenapiGoCollect(QuestionList.Movie_Country);
            //OpenapiGoCollect(QuestionList.Movie_Description);
            //OpenapiGoCollect(QuestionList.Celebrity_Act);
            //OpenapiGoCollect(QuestionList.Celebrity_Direct);
        }

        private static List<QuestionAnswerOpenapi> ReadFile(string input_file)
        {
            List<QuestionAnswerOpenapi> ret = new List<QuestionAnswerOpenapi>();
            QuestionAnswer temp;
            int count = 0;
            try
            {
                using (StreamReader sr = new StreamReader(input_file))
                {
                    string line = "";
                    while (!sr.EndOfStream)
                    {
                        count++;
                        line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            continue;
                        }
                        else
                        {
                            temp = JsonConvert.DeserializeObject<QuestionAnswer>(line);
                            ret.Add(new QuestionAnswerOpenapi(temp));
                        }
                    }
                }
            }
            catch { Console.WriteLine($"There is no {input_file}"); Console.WriteLine($"{count} lines have been read"); }
            return ret;
        }

        private static void WriteFile(string output_file, List<QuestionAnswerOpenapi> question_answer_list)
        {
            JsonSerializer serializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(output_file);
            string json = "";
            foreach (var item in question_answer_list)
            {
                json = JsonConvert.SerializeObject(item);
                sw.WriteLine(json);
            }
            sw.Flush();
            sw.Close();
        }

        public static readonly List<string> ProxyList = new List<string>()
        {
            MicrosoftInternalProxies.BrzProxy,
            MicrosoftInternalProxies.EuroProxy,
            MicrosoftInternalProxies.Itgproxy,
            MicrosoftInternalProxies.JpnProxy,
            MicrosoftInternalProxies.Korproxy,
            MicrosoftInternalProxies.NcpProxy,
            MicrosoftInternalProxies.Sinproxy,
            MicrosoftInternalProxies.SvcProxy
        };
        public static readonly List<string> UserAgentList = new List<string>()
        {
            CommonUserAgents.Chrome50,
            CommonUserAgents.Chrome49,
            CommonUserAgents.Chrome41,
            CommonUserAgents.Chrome40,
            CommonUserAgents.Firefox45,
            CommonUserAgents.Firefox41,
            CommonUserAgents.Firefox36,
            CommonUserAgents.IE11,
            CommonUserAgents.IE10,
            CommonUserAgents.Opera36,
        };


        public static OpenAPIInfo GetOpenapiResponse(string query)
        {
            Thread.Sleep(Config.SleepTime);
            OpenAPIInfo temp = new OpenAPIInfo();
            string query_url = @"https://www.bing.com/api/v6/search?q=" + query + "%20site:zhidao.baidu.com&appid=371E7B2AF0F9B84EC491D731DF90A55719C7D209&mkt=zh-cn&responsefilter=webpages&count=50";
            //Console.WriteLine(query_url);
            temp.OriQuery = query_url;
            //Console.WriteLine(temp.OriQuery);
            string json = GetJsonResponse(temp.OriQuery);
            //temp.ResponseJson = json;
            dynamic response = JsonConvert.DeserializeObject(json);
            dynamic web = response.webPages;
            if (web != null)
            {
                temp.webPageswebSearchUrl = (string)web.webSearchUrl;
                Newtonsoft.Json.Linq.JArray value = web.value;
                temp.webPagesvalue = value.ToObject<List<OpenApiValue>>();
            }
            return temp;
        }

        public static string GetJsonResponse(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet), true);
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            else
            {
                Console.WriteLine($"Cannot fetch the HTML of {url}");
            }
            return data;
        }

        private static void OpenapiGoCollect(QuestionList question_type)
        {
            string input_file = Config.path;
            string output_file = Config.path;

            switch (question_type)
            {
                case QuestionList.Movie_Artists:
                    input_file += Config.movie_artist_pattern;
                    output_file += Config.movie_artist_openapi;
                    break;
                case QuestionList.Movie_Directors:
                    input_file += Config.movie_director_pattern;
                    output_file += Config.movie_director_openapi;
                    break;
                case QuestionList.Movie_PublishDate:
                    input_file += Config.movie_publishdate_pattern;
                    output_file += Config.movie_publishdate_openapi;
                    break;
                case QuestionList.Movie_Genres:
                    input_file += Config.movie_genres_pattern;
                    output_file += Config.movie_genres_openapi;
                    break;
                case QuestionList.Movie_Country:
                    input_file += Config.movie_country_pattern;
                    output_file += Config.movie_country_openapi;
                    break;
                case QuestionList.Movie_Description:
                    input_file += Config.movie_description_pattern;
                    output_file += Config.movie_description_openapi;
                    break;
                case QuestionList.Celebrity_Act:
                    input_file += Config.celebrity_act_pattern;
                    output_file += Config.celebrity_act_openapi;
                    break;
                case QuestionList.Celebrity_Direct:
                    input_file += Config.celebrity_direct_pattern;
                    output_file += Config.celebrity_direct_openapi;
                    break;
            }

            List<QuestionAnswerOpenapi> openapi_container = ReadFile(input_file);
            List<QuestionAnswerOpenapi> openapi_res_container = new List<QuestionAnswerOpenapi>();
            int total = openapi_container.Count;
            for(int count = 0; count < total; count++)
            {
                if (count % 3000 == 0 || count == (total - 1))
                {
                    if (count != 0)
                    {
                        WriteFile(output_file.Replace(".json", count + ".json"), openapi_res_container);
                    }
                    openapi_res_container = new List<QuestionAnswerOpenapi>();
                }
                QuestionAnswerOpenapi qa_item = openapi_container[count];
                Console.WriteLine($"Totle {total} questions, downloading {count} th: {qa_item.QuestionPatternFilled}");
                qa_item.Response = GetOpenapiResponse(qa_item.QuestionPatternFilled);
                openapi_res_container.Add(qa_item);
            }
            Console.WriteLine($"Output Response into file {output_file}");
        }
    }
}

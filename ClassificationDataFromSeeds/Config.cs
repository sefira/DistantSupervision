using ChinaOpalSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationDataFromSeeds
{
    enum QuestionList {
        Movie_Artists,
        Movie_Directors,
        Movie_PublishDate,
        //Movie_Rating,
        Movie_Genres,
        Movie_Country,
        Movie_Description,
        Celebrity_Act,
        Celebrity_Direct };

    class Config
    {
        public static string path = @"D:\ZhidaoCarwler\ClassificationDataFromSeeds\Resources\";
        // entity seeds
        public static string movie_name = @"movie.csv";
        public static string artist_name = @"artist.csv";
        public static string director_name = @"director.csv";
        // entity seeds generate AnswerEntity
        public static string movie_wanswer = @"movie_wanswer.json";
        public static string artist_wanswer = @"artist_wanswer.json";
        public static string director_wanswer = @"director_wanswer.json";
        public static string question_pattern = @"QA_pattern_seeds.txt";
        // AnswerEntity generate QuestionAnswerEntityPair
        public static string movie_artist = @"movie_artist.json";
        public static string movie_director = @"movie_director.json";
        public static string movie_publishdate = @"movie_publishdate.json";
        public static string movie_genres = @"movie_genres.json";
        public static string movie_country = @"movie_country.json";
        public static string movie_description = @"movie_description.json";
        public static string celebrity_act = @"celebrity_act.json";
        public static string celebrity_direct = @"celebrity_direct.json";
        // QuestionAnswerEntityPair generate QuestionAnswerPattern in which QuestionAnswerEntityPair has been filled into the pattern seeds
        public static string movie_artist_pattern = @"movie_artist_pattern.json";
        public static string movie_director_pattern = @"movie_director_pattern.json";
        public static string movie_publishdate_pattern = @"movie_publishdate_pattern.json";
        public static string movie_genres_pattern = @"movie_genres_pattern.json";
        public static string movie_country_pattern = @"movie_country_pattern.json";
        public static string movie_description_pattern = @"movie_description_pattern.json";
        public static string celebrity_act_pattern = @"celebrity_act_pattern.json";
        public static string celebrity_direct_pattern = @"celebrity_direct_pattern.json";

        // web crawler
        public static string movie_artist_crawler = @"movie_artist_crawler.txt";
        public static string movie_director_crawler = @"movie_director_crawler.txt";
        public static string movie_publishdate_crawler = @"movie_publishdate_crawler.txt";
        public static string movie_genres_crawler = @"movie_genres_crawler.txt";
        public static string movie_country_crawler = @"movie_country_crawler.txt";
        public static string movie_description_crawler = @"movie_description_crawler.txt";
        public static string celebrity_act_crawler = @"celebrity_act_crawler.txt";
        public static string celebrity_direct_crawler = @"celebrity_direct_crawler.txt";

        // openAPI
        public static string movie_artist_openapi = @"movie_artist_openapi.json";
        public static string movie_director_openapi = @"movie_director_openapi.json";
        public static string movie_publishdate_openapi = @"movie_publishdate_openapi.json";
        public static string movie_genres_openapi = @"movie_genres_openapi.json";
        public static string movie_country_openapi = @"movie_country_openapi.json";
        public static string movie_description_openapi = @"movie_description_openapi.json";
        public static string celebrity_act_openapi = @"celebrity_act_openapi.json";
        public static string celebrity_direct_openapi = @"celebrity_direct_openapi.json";

        public static Dictionary<QuestionList, string> question2string = new Dictionary<QuestionList, string>()
        {
            { QuestionList.Movie_Artists,"Artists:Name"},
            { QuestionList.Movie_Directors, "Directors:Name"},
            { QuestionList.Movie_PublishDate, "PublishDate"},
            //{ QuestionList.Movie_Rating, "Rating"},
            { QuestionList.Movie_Genres, "Genres"},
            { QuestionList.Movie_Country, "Country"},
            { QuestionList.Movie_Description, "Description"},
            { QuestionList.Celebrity_Act, "Act:Name"},
            { QuestionList.Celebrity_Direct, "Direct:Name"},
        };

        public static Dictionary<string, QuestionList> string2question = new Dictionary<string, QuestionList>()
        {
            { "Artists:Name", QuestionList.Movie_Artists},
            //{ "Artists:Name(Performance:1&Gender:male", QuestionList.Movie_Artists},
            //{ "Artists:Name(Performance:1&Gender:female", QuestionList.Movie_Artists},
            { "Directors:Name", QuestionList.Movie_Directors},
            { "PublishDate", QuestionList.Movie_PublishDate},
            //{ QuestionList.Movie_Rating, "Rating"},
            { "Genres", QuestionList.Movie_Genres},
            { "Country", QuestionList.Movie_Country},
            { "Description", QuestionList.Movie_Description},
            { "Act:Name", QuestionList.Celebrity_Act},
            { "Direct:Name", QuestionList.Celebrity_Direct},
        };

        public static int movie_cadidate_number = 1000;

        public static int SleepTime = 25;
    }


    class MovieEntity : IEquatable<MovieEntity>, IComparable<MovieEntity>
    {
        public MovieEntity()
        {
        }

        public MovieEntity(string[] temp)
        {
            Name = temp[0];
            NumberOfReviewer = uint.Parse(temp[1]);
        }

        public MovieEntity(SnappsEntity item)
        {
            Name = item.Name;
            //Artists = string.Join("|", item.Entment.Artists.Distinct().ToArray());
            //Directors = string.Join("|", item.Entment.Directors.Distinct().ToArray());
            //Performance = string.Join("|", item.Entment.Performance.Distinct().ToArray());
            //Country = string.Join("|", item.Geographies.Distinct().ToArray());
            //Genres = string.Join("|", item.Entment.Genres.Distinct().ToArray());
            Artists = item.Entment.Artists.Distinct().ToArray().ToList();
            Directors = item.Entment.Directors.Distinct().ToArray().ToList();
            Performance = item.Entment.Performance;
            Country = item.Geographies.Distinct().ToArray().ToList();
            Genres = item.Entment.Genres.Distinct().ToArray().ToList();
            PublishDate = item.PublishDate;
            Description = "";
            NumberOfReviewer = item.Rank;
        }

        public string Name { get; set; }
        public uint NumberOfReviewer { get; set; }

        public List<string> Artists { get; set; }
        public List<string> Directors { get; set; }
        public Dictionary<string, string> Performance { get; set; }
        public uint PublishDate { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Country { get; set; }
        public string Description { get; set; }

        public bool Equals(MovieEntity other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = ((Name == null) ? 0 : Name.GetHashCode());
            //Calculate the hash code for the MovieEntity. 
            return hashProductName;
        }

        public int CompareTo(MovieEntity other)
        {
            if (other == null)
                return 0;
            else
                return other.NumberOfReviewer.CompareTo(this.NumberOfReviewer);
        }
    }

    class CelebrityEntity
    {
        public CelebrityEntity()
        {
        }

        public CelebrityEntity(string[] temp)
        {
            Name = temp[0];
            Count = temp[1];
            Act = new List<string>();
            Direct = new List<string>();
        }

        public string Name { get; set; }
        public string Count { get; set; }

        public List<string> Act { get; set; }
        public List<string> Direct { get; set; }
    }

    class QuestionAnswer
    {
        public QuestionAnswer()
        {
        }

        public QuestionAnswer(QuestionList question_type, string question_entity, List<string> answer_entity)
        {
            QuestionType = question_type;
            QuestionEntity = question_entity;
            AnswerEntity = answer_entity;
        }

        public QuestionAnswer(QuestionAnswer copy)
        {
            QuestionType = copy.QuestionType;
            QuestionPattern = copy.QuestionPattern;
            QuestionEntity = copy.QuestionEntity;
            QuestionPatternFilled = copy.QuestionPatternFilled;
            AnswerEntity = copy.AnswerEntity;
        }

        public QuestionList QuestionType { set; get; }
        public string QuestionPattern { set; get; }
        public string QuestionEntity { set; get; }
        public string QuestionPatternFilled { set; get; }
        public List<string> AnswerEntity { set; get; }
    }

    public sealed class MicrosoftInternalProxies
    {
        public const string JpnProxy = "jpnproxy.fareast.corp.microsoft.com";
        public const string Sinproxy = "sinproxy.fareast.corp.microsoft.com";
        public const string Itgproxy = "itgproxy.redmond.corp.microsoft.com";
        public const string EuroProxy = "europroxy.europe.corp.microsoft.com";
        public const string Korproxy = "korproxy.fareast.corp.microsoft.com";
        public const string SvcProxy = "SVCPROXY.NORTHAMERICA.CORP.MICROSOFT.COM";
        public const string NcpProxy = "NCPROXY.NORTHAMERICA.CORP.MICROSOFT.COM";
        public const string BrzProxy = "BRZPROXY.SOUTHAMERICA.CORP.MICROSOFT.COM";
    }

    public sealed class CommonUserAgents
    {
        public const string IE11 = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; Touch; rv:11.0) like Gecko";
        public const string IE10 = "Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0";
        public const string Opera36 = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 OPR/36.0.2130.65";
        public const string Chrome50 = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
        public const string Chrome49 = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36";
        public const string Chrome41 = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
        public const string Chrome40 = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.93 Safari/537.36";
        public const string Firefox45 = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
        public const string Firefox41 = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
        public const string Firefox36 = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";
    }

    class OpenAPIInfo
    {
        public string OriQuery { get; set; }
        public string ResponseJson { get; set; }
        public string webPageswebSearchUrl { get; set; }
        public List<OpenApiValue> webPagesvalue { get; set; }
    }
    class OpenApiValue
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public string dateLastCrawled { get; set; }
    }

    class QuestionAnswerOpenapi : QuestionAnswer
    {
        public QuestionAnswerOpenapi()
        {
        }
        public QuestionAnswerOpenapi(QuestionAnswer copy)
        {
            QuestionType = copy.QuestionType;
            QuestionPattern = copy.QuestionPattern;
            QuestionEntity = copy.QuestionEntity;
            QuestionPatternFilled = copy.QuestionPatternFilled;
            AnswerEntity = copy.AnswerEntity;
        }

        // records 50 openapi results in zhidao.baidu.com given a query.
        public OpenAPIInfo Response { get ; set; }
    }
}

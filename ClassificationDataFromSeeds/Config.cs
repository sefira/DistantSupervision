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
        public static string movie_wanswer = @"movie_wanswer.txt";
        public static string artist_wanswer = @"artist_wanswer.txt";
        public static string director_wanswer = @"director_wanswer.txt";
        // AnswerEntity generate QuestionAnswerEntityPair
        public static string movie_artist = @"movie_artist.txt";
        public static string movie_director = @"movie_director.txt";
        public static string movie_publishdate = @"movie_publishdate.txt";
        public static string movie_genres = @"movie_genres.txt";
        public static string movie_country = @"movie_country.txt";
        public static string movie_description = @"movie_description.txt";
        public static string celebrity_act = @"celebrity_act.txt";
        public static string celebrity_direct = @"celebrity_direct.txt";
        // QuestionAnswerEntityPair generate QuestionAnswerPattern in which QuestionAnswerEntityPair has been filled into the pattern seeds
        public static string movie_artist_pattern = @"movie_artist_pattern.txt";
        public static string movie_director_pattern = @"movie_director_pattern.txt";
        public static string movie_publishdate_pattern = @"movie_publishdate_pattern.txt";
        public static string movie_genres_pattern = @"movie_genres_pattern.txt";
        public static string movie_country_pattern = @"movie_country_pattern.txt";
        public static string movie_description_pattern = @"movie_description_pattern.txt";
        public static string celebrity_act_pattern = @"celebrity_act_pattern.txt";
        public static string celebrity_direct_pattern = @"celebrity_direct_pattern.txt";

        public static string question_pattern = @"QA_pattern_seeds.txt";

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
            { "Artists:Name(Performance:1&Gender:male", QuestionList.Movie_Artists},
            { "Artists:Name(Performance:1&Gender:female", QuestionList.Movie_Artists},
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

    class QuestionAnswerWebSpider : QuestionAnswer
    {
        List<string> encoded_url = new List<string>();
    }
}

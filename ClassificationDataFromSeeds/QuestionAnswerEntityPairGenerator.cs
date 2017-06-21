using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ClassificationDataFromSeeds
{
    // generate QuestionAnswerEntityPair by AnswerEntity
    // QuestionAnswerEntityPair has split the question entity and answer entity, while AnswerEntity fusion them.
    // input: movie_wanswer.txt artist_wanswer.txt director_wanswer.txt
    // output: 1 movie_artist.txt 2 movie_director.txt 3 movie_publishdate.txt 4 movie_genres.txt 5 movie_country.txt 6 celebrity_act.txt 7 celebrity_direct.txt

    class QuestionAnswerEntityPairGenerator
    {
        public static void FakeMain()
        {
            MovieArtistGenerator();
            MovieDirectorGenerator();
            MoviePublishdateGenerator();
            MovieGenresGenerator();
            MovieCountryGenerator();
            MovieDescriptionGenerator();
            CelebrityActGenerator();
            CelebrityDirectGenerator();
        }


        private static List<MovieEntity> movie_wanswer_list = new List<MovieEntity>();
        private static List<CelebrityEntity> artist_wanswer_list = new List<CelebrityEntity>();
        private static List<CelebrityEntity> director_wanswer_list = new List<CelebrityEntity>();

        static QuestionAnswerEntityPairGenerator()
        {
            string movie_wanswer_file = Config.path + Config.movie_wanswer;
            string artist_wanswer_file = Config.path + Config.artist_wanswer;
            string director_wanswer_file = Config.path + Config.director_wanswer;
            
            try
            {
                using (StreamReader sr = new StreamReader(movie_wanswer_file))
                {
                    int i = 0;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (i == 0)
                        {
                            Console.WriteLine(line);
                            i++;
                        }
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            continue;
                        }
                        else
                        {
                            movie_wanswer_list.Add(JsonConvert.DeserializeObject<MovieEntity>(line));
                        }
                    }
                }
            }
            catch { Console.WriteLine($"There is no {movie_wanswer_file}"); }

            try
            {
                using (StreamReader sr = new StreamReader(artist_wanswer_file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            continue;
                        }
                        else
                        {
                            artist_wanswer_list.Add(JsonConvert.DeserializeObject<CelebrityEntity>(line));
                        }
                    }
                }
            }
            catch { Console.WriteLine($"There is no {artist_wanswer_file}"); }

            try
            {
                using (StreamReader sr = new StreamReader(director_wanswer_file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            continue;
                        }
                        else
                        {
                            director_wanswer_list.Add(JsonConvert.DeserializeObject<CelebrityEntity>(line));
                        }
                    }
                }
            }
            catch { Console.WriteLine($"There is no {director_wanswer_file}"); }

            JsonSerializer serializer = new JsonSerializer();
            StringWriter strw = new StringWriter();
            serializer.Serialize(new JsonTextWriter(strw), movie_wanswer_list[0]);
            Console.WriteLine(strw.GetStringBuilder().ToString());

            string str = JsonConvert.SerializeObject(movie_wanswer_list[0]);
            Console.WriteLine(str);
        }

        private static void WriteFile<T>(string output_file, List<T> question_answer_entity_list)
        {
            //JsonSerializer serializer = new JsonSerializer();
            //StreamWriter sw = new StreamWriter(output_file);
            //string json = "";
            //foreach (var item in question_answer_entity_list)
            //{
            //    json = JsonConvert.SerializeObject(item);
            //    sw.WriteLine(json);
            //}
            //sw.Flush();
            //sw.Close();
        }
        private static List<string> SplitMovieName(string name)
        {
            List<string> ret = new List<string>();
            ret.Add(name);
            char[] separators = new char[] { '·', '：', '《', '》', '，', '(', ')', '。' };
            string[] temp = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            ret.Add(string.Join("", temp));
            foreach (string item in temp)
            {
                if(item.Length >= 3)
                {
                    ret.Add(item);
                }
            }
            separators = new char[] { '·', '：', '《', '》', '，', '(', ')', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            temp = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            ret.Add(string.Join("", temp));
            foreach (string item in temp)
            {
                if (item.Length >= 3)
                {
                    ret.Add(item);
                }
            }
            ret = ret.Distinct().ToList();
            return ret;
        }

        private static List<string> SplitCelebrityName(List<string> names)
        {
            char[] separators = new char[] { '·', '：', '《', '》', '，', '(', ')', '-', '（', '）', '、', ' ' };
            List<string> ret = new List<string>();

            foreach(string name in names)
            {
                ret.Add(name);
                string[] temp = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                ret.Add(string.Join("", temp));
                foreach (string item in temp)
                {
                    if (item.Length >= 2)
                    {
                        ret.Add(item);
                    }
                }
            }
            ret = ret.Distinct().ToList();
            return ret;
        }

        public static List<QuestionAnswer> MovieArtistGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> artists = SplitCelebrityName(movie.Artists);
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach(string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_Artists, question_entity: movie_name, answer_entity: artists));
                }
            }
            string output_file = Config.path + Config.movie_artist;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> MovieDirectorGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> directors = SplitCelebrityName(movie.Directors);
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach (string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_Directors, question_entity: movie_name, answer_entity: directors));
                }
            }
            string output_file = Config.path + Config.movie_director;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> MoviePublishdateGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach (string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_PublishDate, question_entity: movie_name,
                        answer_entity: new List<string>(new string[] { movie.PublishDate.ToString().Substring(0, 4) })));
                }
            }
            string output_file = Config.path + Config.movie_publishdate;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> MovieCountryGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach (string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_Country, question_entity: movie_name, answer_entity: movie.Country));
                }
            }
            string output_file = Config.path + Config.movie_country;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> MovieGenresGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach (string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_Genres, question_entity: movie_name, answer_entity: movie.Genres));
                }
            }
            string output_file = Config.path + Config.movie_genres;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> MovieDescriptionGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();

            foreach (var movie in movie_wanswer_list)
            {
                List<string> movie_alias = SplitMovieName(movie.Name);
                foreach (string movie_name in movie_alias)
                {
                    question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Movie_Description, question_entity: movie_name, answer_entity: new List<string>()));
                }
            }
            string output_file = Config.path + Config.movie_description;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> CelebrityActGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();
            
            foreach (var artist in artist_wanswer_list)
            {
                List<string> movies_alias = new List<string>();
                foreach (string item in artist.Act)
                {
                    movies_alias.AddRange(SplitMovieName(item));
                }
                question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Celebrity_Act, question_entity: artist.Name, answer_entity: movies_alias));
            }
            string output_file = Config.path + Config.celebrity_act;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }

        public static List<QuestionAnswer> CelebrityDirectGenerator()
        {
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();
            
            foreach (var director in director_wanswer_list)
            {
                List<string> movies_alias = new List<string>();
                foreach (string item in director.Direct)
                {
                    movies_alias.AddRange(SplitMovieName(item));
                }
                question_answer_entity_list.Add(new QuestionAnswer(question_type: QuestionList.Celebrity_Direct, question_entity: director.Name, answer_entity: movies_alias));
            }
            string output_file = Config.path + Config.celebrity_direct;
            WriteFile<QuestionAnswer>(output_file, question_answer_entity_list);
            return question_answer_entity_list;
        }
    }
}

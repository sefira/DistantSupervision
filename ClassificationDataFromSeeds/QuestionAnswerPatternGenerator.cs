using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationDataFromSeeds
{
    // generate QuestionAnswerPatternGenerator by QuestionAnswerEntityPairGenerator
    // QuestionAnswerPatternGenerator has filled the question pattern with question entity in QuestionAnswerEntityPairGenerator
    // input: 1 movie_artist.txt 2 movie_director.txt 3 movie_publishdate.txt 4 movie_genres.txt 5 movie_country.txt 6 celebrity_act.txt 7 celebrity_direct.txt
    // output: 1 movie_artist_pattern.txt 2 movie_director_pattern.txt 3 movie_publishdate_pattern.txt 4 movie_genres_pattern.txt 5 movie_country_pattern.txt 6 celebrity_act_pattern.txt 7 celebrity_direct_pattern.txt

    class QuestionAnswerPatternGenerator
    {
        public static void FakeMain()
        {
            foreach (QuestionList item in Enum.GetValues(typeof(QuestionList)))
            {
                QuestionAnswerGenerator(item);
            }
        }

        private static Dictionary<QuestionList, List<string>> question_pattern_list = new Dictionary<QuestionList, List<string>>();

        static QuestionAnswerPatternGenerator()
        {
            foreach (QuestionList item in Enum.GetValues(typeof(QuestionList)))
            {
                question_pattern_list.Add(item, new List<string>());
            }

            string pattern_file = Config.path + Config.question_pattern;
            int count = 0;
            try
            {
                using (StreamReader sr = new StreamReader(pattern_file))
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
                            string[] line_format = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (Config.string2question.ContainsKey(line_format[1]))
                            {
                                QuestionList question_type = Config.string2question[line_format[1]];
                                question_pattern_list[question_type].Add(line_format[3]);
                            }
                        }
                    }
                }
            }
            catch { Console.WriteLine($"There is no {pattern_file}"); Console.WriteLine($"{count} patterns has beed read"); }
        }

        private static void WriteFile<T>(string output_file, List<T> question_answer_list)
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

        private static List<QuestionAnswer> QuestionAnswerGenerator(QuestionList question_type)
        {
            string entity_pattern = "";
            List<QuestionAnswer> question_answer_entity_list = new List<QuestionAnswer>();
            string output_file = Config.path;

            switch (question_type)
            {
                case QuestionList.Movie_Artists:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MovieArtistGenerator();
                    output_file += Config.movie_artist_pattern;
                    break;
                case QuestionList.Movie_Directors:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MovieDirectorGenerator();
                    output_file += Config.movie_director_pattern;
                    break;
                case QuestionList.Movie_PublishDate:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MoviePublishdateGenerator();
                    output_file += Config.movie_publishdate_pattern;
                    break;
                case QuestionList.Movie_Genres:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MovieGenresGenerator();
                    output_file += Config.movie_genres_pattern;
                    break;
                case QuestionList.Movie_Country:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MovieCountryGenerator();
                    output_file += Config.movie_country_pattern;
                    break;
                case QuestionList.Movie_Description:
                    entity_pattern = "<nmovie>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.MovieDescriptionGenerator();
                    output_file += Config.movie_description_pattern;
                    break;
                case QuestionList.Celebrity_Act:
                    entity_pattern = "<nrcelebrity>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.CelebrityActGenerator();
                    output_file += Config.celebrity_act_pattern;
                    break;
                case QuestionList.Celebrity_Direct:
                    entity_pattern = "<nrcelebrity>";
                    question_answer_entity_list = QuestionAnswerEntityPairGenerator.CelebrityDirectGenerator();
                    output_file += Config.celebrity_direct_pattern;
                    break;
            }

            List<QuestionAnswer> question_answer_list = new List<QuestionAnswer>();
            foreach (QuestionAnswer qa_entity in question_answer_entity_list)
            {
                foreach (string pattern in question_pattern_list[question_type])
                {
                    QuestionAnswer temp = new QuestionAnswer(qa_entity);
                    temp.QuestionPattern = pattern;
                    temp.QuestionPatternFilled = pattern.Replace(entity_pattern, temp.QuestionEntity);
                    question_answer_list.Add(temp);
                }
            }
            WriteFile<QuestionAnswer>(output_file, question_answer_list);
            Console.WriteLine($"Output patterned Question Answer into file {output_file}");
            return question_answer_list;
        }
    }
}

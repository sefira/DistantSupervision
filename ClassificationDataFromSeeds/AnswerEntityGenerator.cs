using ChinaOpalSearch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationDataFromSeeds
{
    // Generate answer for those entities by querying oSearch
    // input: movie.csv artist.csv director.csv 
    // output: movie_wanswer.txt artist_wanswer.txt director_wanswer.txt
    class AnswerEntityGenerator
    {
        public static void FakeMain()
        {
            //GenerateAnswerForMovie();
            //GetAnswerForCelebrity("Artists");
            GetAnswerForCelebrity("Directors");
        }
        
        //private static void GetAnswerForMovie()
        //{
        //    List<MovieEntity> movie_without_answer = new List<MovieEntity>();
        //    List<MovieEntity> movie_with_answer = new List<MovieEntity>();
        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(Config.path + Config.movie_name))
        //        {
        //            while (!sr.EndOfStream)
        //            {
        //                string line = sr.ReadLine();
        //                if (string.IsNullOrWhiteSpace(line))
        //                {
        //                    Console.WriteLine(line);
        //                    continue;
        //                }
        //                string[] temp = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //                movie_without_answer.Add(new MovieEntity(temp));
        //            }
        //        }
        //    }
        //    catch { Console.WriteLine($"There is no {Config.path + Config.movie_name}"); }

        //    // query and parse and fill each MoiveWAnswer
        //    foreach (MovieEntity movie in movie_without_answer)
        //    {
        //        string osearch_query = (string.Format(@"#:""{0}Name """, movie.Name));
        //        var results = oSearchClient.Query(osearch_query);
        //        foreach (var item in results)
        //        {
        //            movie_with_answer.Add(new MovieEntity(item));
        //        }
        //    }
        //    movie_with_answer.Distinct().ToList();
        //    movie_with_answer.Sort();
        //}

        private static void GenerateAnswerForMovie()
        {
            List<MovieEntity> movie_with_answer = new List<MovieEntity>();
            string output_file = Config.path + Config.movie_wanswer;

            string osearch_query = (string.Format(@""));
            var results = oSearchClient.Query(osearch_query);
            foreach (var item in results)
            {
                movie_with_answer.Add(new MovieEntity(item));
                //Console.WriteLine(movie_with_answer.Last().Name + movie_with_answer.Last().NumberOfReviewer);
            }
            Console.WriteLine(movie_with_answer.Count);
            movie_with_answer = movie_with_answer.GetRange(0, Config.movie_cadidate_number);
            Console.WriteLine(movie_with_answer.Count);
            movie_with_answer = movie_with_answer.Distinct().ToList();
            Console.WriteLine(movie_with_answer.Count);
            movie_with_answer.Sort();

            JsonSerializer serializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(output_file);
            foreach (var item in movie_with_answer)
            {
                StringWriter strw = new StringWriter();
                serializer.Serialize(new JsonTextWriter(strw), item);

                sw.Write(strw.GetStringBuilder().ToString());
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();
        }

        private static void GetAnswerForCelebrity(string artist_direct)
        {
            List<CelebrityEntity> celebrity_with_answer = new List<CelebrityEntity>();
            string input_file = "";
            string output_file = "";
            switch (artist_direct)
            {
                case "Artists":
                    input_file = Config.path + Config.artist_name;
                    output_file = Config.path + Config.artist_wanswer;
                    break;
                case "Directors":
                    input_file = Config.path + Config.director_name;
                    output_file = Config.path + Config.director_wanswer;
                    break;
            }
            try
            {
                using (StreamReader sr = new StreamReader(input_file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            continue;
                        }
                        string[] temp = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        celebrity_with_answer.Add(new CelebrityEntity(temp));
                    }
                }
            }
            catch { Console.WriteLine($"There is no {input_file}"); }

            // query and parse and fill each CelebrityEntity

            switch (artist_direct)
            {
                case "Artists":
                    {
                        foreach (CelebrityEntity celebrity in celebrity_with_answer)
                        {
                            string osearch_query = (string.Format(@"#:""{0}Artists """, celebrity.Name));
                            var results = oSearchClient.Query(osearch_query);

                            List<MovieEntity> movie_with_answer = new List<MovieEntity>();
                            foreach (var item in results)
                            {
                                movie_with_answer.Add(new MovieEntity(item));
                            }
                            movie_with_answer = movie_with_answer.Distinct().ToList();
                            movie_with_answer.Sort();

                            foreach (var item in movie_with_answer)
                            {
                                celebrity.Act.Add(item.Name);
                            }
                        }
                        break;
                    }
                case "Directors":
                    {
                        foreach (CelebrityEntity celebrity in celebrity_with_answer)
                        {
                            string osearch_query = (string.Format(@"#:""{0}Directors """, celebrity.Name));
                            var results = oSearchClient.Query(osearch_query);

                            List<MovieEntity> movie_with_answer = new List<MovieEntity>();
                            foreach (var item in results)
                            {
                                movie_with_answer.Add(new MovieEntity(item));
                            }
                            movie_with_answer = movie_with_answer.Distinct().ToList();
                            movie_with_answer.Sort();

                            foreach (var item in movie_with_answer)
                            {
                                celebrity.Direct.Add(item.Name);
                            }
                        }
                        break;
                    }
            }

            JsonSerializer serializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(output_file);
            foreach (var item in celebrity_with_answer)
            {
                StringWriter strw = new StringWriter();
                serializer.Serialize(new JsonTextWriter(strw), item);

                sw.Write(strw.GetStringBuilder().ToString());
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();
        }

    }
}

using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;
using Newtonsoft.Json;

public class Json2TsvExtractor : Extractor
{
    public override Schema Produces(string[] requestedColumns, string[] args)
    {

        //QuestionType: int,
        //QuestionPattern:string,
        //QuestionEntity:string,
        //QuestionPatternFilled:string,
        //AnswerEntity:string,
        //webPagesvalueQuestion:string,
        //webPagesvalueSnippet:string,
        //webPagesvalueURL:string
        return new Schema(requestedColumns);
    }
    public override IEnumerable<Row> Extract(StreamReader reader, Row outputRow, string[] args)
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var temp = JsonConvert.DeserializeObject<QuestionAnswerOpenapi>(line);
            if (temp.Response != null && temp.Response.webPagesvalue != null && temp.Response.webPagesvalue.Count != 0)
            {
                var webPagesvalue = temp.Response.webPagesvalue;
                int QuestionType = (int)temp.QuestionType;
                string QuestionPattern = temp.QuestionPattern;
                string QuestionEntity = temp.QuestionEntity;
                string QuestionPatternFilled = temp.QuestionPatternFilled;
                string AnswerEntity = string.Join("|", temp.AnswerEntity);
                foreach (var item in webPagesvalue)
                {
                    string[] question = item.name.Split(new char[] { '-', '_' });
                    string webPagesvalueQuestion = question[0].Trim();
                    string webPagesvalueSnippet = item.snippet;
                    string webPagesvalueURL = item.url;
                    outputRow[0].Set(QuestionType);
                    outputRow[1].Set(QuestionPattern);
                    outputRow[2].Set(QuestionEntity);
                    outputRow[3].Set(QuestionPatternFilled);
                    outputRow[4].Set(AnswerEntity);
                    outputRow[5].Set(webPagesvalueQuestion);
                    outputRow[6].Set(webPagesvalueSnippet);
                    outputRow[7].Set(webPagesvalueURL);
                    yield return outputRow;
                }
            }
        }
    }
}

enum QuestionList
{
    Movie_Artists,
    Movie_Directors,
    Movie_PublishDate,
    //Movie_Rating,
    Movie_Genres,
    Movie_Country,
    Movie_Description,
    Celebrity_Act,
    Celebrity_Direct
};

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
    public OpenAPIInfo Response { get; set; }
}
using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;
using System.Linq;

public class DistSupervisedProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        //QuestionType: int,
        //QuestionPattern:string,
        //QuestionEntity:string,
        //QuestionPatternFilled:string,
        //AnswerEntity:string,
        //webPagesvalueQuestion:string,
        //webPagesvalueSnippet:string,
        //webPagesvalueURL:string,
        //TrainData:string,
        //TrainLabel:int
        var output_schema = input_schema.Clone();
        var newcol1 = new ColumnInfo("TrainData", typeof(string));
        var newcol2 = new ColumnInfo("TrainLabel", typeof(int));
        output_schema.Add(newcol1);
        output_schema.Add(newcol2);
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        foreach (Row input_row in input_rowset.Rows)
        {
            input_row.CopyTo(output_row);

            string webPagesvalueQuestion = input_row["webPagesvalueQuestion"].String;
            string QuestionEntity = input_row["QuestionEntity"].String;
            string webPagesvalueSnippet = input_row["webPagesvalueSnippet"].String;
            List<string> AnswerEntity = new List<string>(input_row["AnswerEntity"].String.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

            //set train data(e.g. training query)
            string tag = input_row["QuestionPattern"].String.Contains("nmovie") ? "<nmovie>" : "<nrcelebrity>";
            if (string.IsNullOrWhiteSpace(webPagesvalueQuestion))
            {
                string Question = "";
                output_row["TrainData"].Set(Question);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(QuestionEntity))
                {
                    string Question = webPagesvalueQuestion;
                    output_row["TrainData"].Set(Question);
                }
                else
                {
                    string Question = webPagesvalueQuestion.Replace(QuestionEntity, tag);
                    output_row["TrainData"].Set(Question);
                }
            }
            // set label
            if (webPagesvalueQuestion.Contains(QuestionEntity) && AnswerEntity.Any(webPagesvalueSnippet.Contains))
            {
                output_row["TrainLabel"].Set(input_row["QuestionType"].Integer);
            }
            else
            {
                output_row["TrainLabel"].Set(-1);
            }
            yield return output_row;
        }
    }
}
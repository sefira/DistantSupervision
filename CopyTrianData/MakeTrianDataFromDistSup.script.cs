using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;

public class NegativeDataRemoveDuplic : Reducer
{
    public override Schema Produces(string[] columns, string[] args, Schema input)
    {
        //@"my/MovieDomain/DistSupervised/movie_artist_distsup.tsv",              //0
        //@"my/MovieDomain/DistSupervised/movie_director_distsup.tsv",            //1
        //@"my/MovieDomain/DistSupervised/movie_publishdate_distsup.tsv",         //2
        //@"my/MovieDomain/DistSupervised/movie_genres_distsup.tsv",              //3
        //@"my/MovieDomain/DistSupervised/movie_country_distsup.tsv",             //4
        //@"my/MovieDomain/DistSupervised/celebrity_act_distsup.tsv",             //6
        //@"my/MovieDomain/DistSupervised/celebrity_direct_distsup.tsv"           //7
        var output_schema = input.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Reduce(RowSet input, Row output, string[] args)
    {
        List<int> positive_record = new List<int>();
        bool exist_positive = false;
        Row neg_sample = null;
        foreach (Row row in input.Rows)
        {
            int label = row["TrainLabel"].Integer;
            if (label == -1)
            {
                // negative samples are saved and yield later
                neg_sample = row;
                continue;
            }
            else
            {
                // positive samples are yield immediately
                row.CopyTo(output);
                exist_positive = true;
                positive_record.Add(label);
                yield return output;
            }
        }
        // there is no positive samples, negative sample is not duplicate and can be yielded
        if(!exist_positive)
        {
            neg_sample.CopyTo(output);
            yield return output;
        }
    }
}


public class BothDataRemoveDuplic : Reducer
{
    public override Schema Produces(string[] columns, string[] args, Schema input)
    {
        var output_schema = input.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Reduce(RowSet input, Row output, string[] args)
    {
        List<int> positive_record = new List<int>();
        bool exist_positive = false;
        Row neg_sample = null;
        foreach (Row row in input.Rows)
        {
            int label = row["TrainLabel"].Integer;
            if (label == -1)
            {
                // negative samples are saved and yield later
                neg_sample = row;
                continue;
            }
            else
            {
                if (positive_record.Contains(label))
                {
                    // positive samples are duplicate
                    continue;
                }
                else
                {
                    // positive samples are yield immediately
                    row.CopyTo(output);
                    exist_positive = true;
                    positive_record.Add(label);
                    yield return output;
                }
            }
        }
        // there is no positive samples, negative sample is not duplicate and can be yielded
        if (!exist_positive)
        {
            neg_sample.CopyTo(output);
            yield return output;
        }
    }
}

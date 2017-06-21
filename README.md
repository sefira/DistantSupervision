本次使用Distant Supervision是为了为电影问题分类源码准备训练数据

电影问题分类需要将一个用户对话，分类为：
0.不是问题
1.提问电影主演
2.提问电影导演
3.提问电影类型
4.提问电影上映日期
5.提问电影产地
6.提问演员演过的电影
7.提问导演导过的电影

## Bootstrap：
一开始先利用pattern和数据库中的entity收集训练数据：
1.  在基于规则（正则表达式）的过程中我已经编写了足够多的pattern，记为<pattern>
	<pattern>形如：
	Movie	Directors:Name	1	<nmovie>的导演有谁
	Movie	Directors:Name	1	<nmovie>的导演叫什么
	Movie	Directors:Name	1	<nmovie>导演叫什么名字
	Movie	Directors:Name	1	<nmovie>是谁导的
	<pattern>一共有137条

2. 那么自动获得大量数据的第一步就是利用数据库中的实体（记为<entity>）比如刘德华填充到pattern中，填充后的pattern记为 <filled-pattern>
	从数据库中选择最受欢迎的电影3000部，演员196个导演34个，将这些<entity>填充到<pattern>中，那么对于每类问题就得到了几百到几万不等的<filled-pattern>。
	
3. 将这些<filled-pattern>送到百度知道就可以利用百度知道的ranker查询到和这些语言相似的语句，既获得<ZhidaoQuestion>。于是这些查询到的语句就可以作为CNN的训练数据。
	在实践过程中因为百度知道的ranker比较难爬，所以采用Bing OpenAPI。

## Distant Supervision：
bootstrap收集到的数据只是利用了Bing OpenAPI的ranker，没有任何监督信息，所以可以使用Distant Supervision过滤Bootstrap收集到的信息，使得数据既有ranker的优化，又有监督信息的优化，这样收集到的数据噪声就会更少
1. 首先优化Bootstrap中第2步的<entity>为<entity，answer>
	比如对于<pattern>:
	Movie	Artists:Name	1	<nmovie>的演员有谁
	Bootstrap中只提取<刘德华>，那么在Distant Supervision中就需要提取<天下无贼，刘德华>。因为oSearch中有这样的数据，所以我们可以提取到这种<entity，answer> pair 
2. 将<entity，answer>填充到<pattern>中得到<filled-pattern，answer>
3. 利用<filled-pattern，answer>中的filled-pattern到Bing OpenAPI中查询，获得<ZhidaoQuestion, ZhidaoAnswer>，融合这两个二元组为一个四元组<filled-pattern，answer，ZhidaoQuestion, ZhidaoAnswer>
	
4. Distant Supervision：
	ZhidaoQuestion标记为正样本当且仅当ZhidaoAnswer.Contains(Answer),否则标记为负样本
	我在Cosmos上实现了一个Scope版本的Distant Supervision Processor：class DistSupervisedProcessor
```
{
    // set label
    if (webPagesvalueQuestion.Contains(QuestionEntity) && AnswerEntity.Any(webPagesvalueSnippet.Contains))
    {
        // label this Question as positive sample while it meets Distant Supervision Conditions
        output_row["TrainLabel"].Set(input_row["QuestionType"].Integer);
    }
    else
    {
        //label this Question as negative sample while it doesn't meet Distant Supervision Conditions
        output_row["TrainLabel"].Set(-1);
    }
    yield return output_row;
}
```

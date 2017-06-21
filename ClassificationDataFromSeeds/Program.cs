using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProxyTools;

namespace ClassificationDataFromSeeds
{
    class Program
    {
        static void Main(string[] args)
        {
            //AnswerEntityGenerator.FakeMain();
            //QuestionAnswerEntityPairGenerator.FakeMain();
            //QuestionAnswerPatternGenerator.FakeMain();

            OpenAPICollector.FakeMain();
        }
    }
}

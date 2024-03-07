using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleForm2PDF.Core
{
    public class MarkdownWriter
    {
        public static string Convert(QuestionElement element)
        {
            string question = element.Question;
            string answer = element.Answer;
            string result = string.Empty;
            switch (element.Type)
            {
                case eMarkdownType.Question_Header3_Block:
                    result = ("### <p style=\"font-family:함초롬바탕;\">" + question + "</p>" + NEWLINE + " > <p style=\"color:rgb(0, 0, 0); font-family:함초롬바탕; line-height:1.6;\">" + answer+ "</p>");
                    break;
                case eMarkdownType.Question_Header4_Block:
                    result = "#### <p style=\"font-family:함초롬바탕;\">" + question + "</p>" + NEWLINE + "> <p style=\"color:rgb(0, 0, 0); font-family:함초롬돋움;  line-height:1.6;\">" + answer + "</p>";
                    break;
                case eMarkdownType.Information_Header3:
                    result = "### <p style=\"font-family:함초롬바탕;\">" + question + " : " + answer + "</p>";
                    break;
                case eMarkdownType.Information_Header4:
                    result = "#### <p style=\"font-family:함초롬바탕;\">" + question + " : " + answer + "</p>";
                    break;
                case eMarkdownType.TimeStamp:
                    result = "##### <p style=\"font-family:함초롬바탕;\"> 제출 시간 : " + answer + "</p>";
                    break;
            }
            return result + NEWLINE;
        }

        public const string NEWLINE = "NEWLINESPLITIER";
        public const string MDNextLine = "<br>";
    }
}

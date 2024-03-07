using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleForm2PDF.Core
{
    public enum eMarkdownType
    {
        Question_Header3_Block = 0,
        Question_Header4_Block = 1,
        
        Information_Header3,
        Information_Header4,


        TimeStamp
    }

    public class QuestionElement
    {
        public eMarkdownType Type = eMarkdownType.Question_Header3_Block;
        public string Question;
        public string Answer;

        public QuestionElement(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }
    }
}

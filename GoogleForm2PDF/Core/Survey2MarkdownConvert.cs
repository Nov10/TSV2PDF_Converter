using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Markdown2Pdf;
using System.Diagnostics;
using Markdown2Pdf.Options;
using Markdig;

namespace GoogleForm2PDF.Core
{
    public class Survey2MarkdownConvert
    {
        async static Task<string> Convert(string path, string title)
        {
            var options = new Markdown2PdfOptions
            {
                HeaderHtml = File.ReadAllText(@"./header.html"),
                FooterHtml = File.ReadAllText(@"./footer.html"),
                DocumentTitle = title,

                MarginOptions = new MarginOptions
                {
                    Top = "80px",
                    Bottom = "50px",
                    Left = "70px",
                    Right = "70px"
                },
                KeepHtml = true,
                Scale = 0.8M
            };

            var converter = new Markdown2PdfConverter(options);
            var resultPath = await converter.Convert(path);
            Process.Start(new ProcessStartInfo { FileName = resultPath, UseShellExecute = true });
            //Console.WriteLine(resultPath);
            return resultPath;
        }

        public static void ConvertTSV2PDF(string tsvFilePath, string saveFolderPath, eMarkdownType[] types)
        {


            StreamReader sr = new StreamReader(tsvFilePath);

            // 스트림의 끝까지 읽기
            List<string[]> file = new List<string[]>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split('\t');

                file.Add(data);
            }


            string[] questions = file[0];
            SurveyAnswer[] answers = new SurveyAnswer[file.Count - 1];
            for (int line = 1; line < file.Count; line++)
            {
                answers[line - 1] = new SurveyAnswer();
                answers[line - 1].Elements = new List<QuestionElement>();
                for (int i = 0; i < questions.Length; i++)
                {
                    //i==0은 무조건 타임스탬프

                    string answer = file[line][i];
                    var q = new QuestionElement(questions[i], answer);

                    //if (i == 0)
                    //    q.Type = eMarkdownType.TimeStamp;
                    //else if (i == 1 || i == 2 || i == 3)
                    //    q.Type = eMarkdownType.Information_Header3;
                    //else
                    //    q.Type = eMarkdownType.Question_Header3_Block;
                    q.Type = types[i];

                    answers[line - 1].Elements.Add(q);
                }
            }


            for (int k = 0; k < answers.Length; k++)
            {
                string f = string.Empty;
                var s = answers[k];
                for (int i = 0; i < s.Elements.Count; i++)
                {
                    f += Core.MarkdownWriter.Convert(s.Elements[i]);
                    //f += Core.MarkdownWriter.MDNextLine;
                    f += Core.MarkdownWriter.NEWLINE;
                    f += Core.MarkdownWriter.NEWLINE;
                }
                //Console.WriteLine(f);
                //string textfile = @"C:\Users\문종욱\Desktop\a\files\" + s.Elements[1].Answer + ".md";
                string textfile = saveFolderPath + "\\" + s.Elements[1].Answer + ".md";
                Console.WriteLine(textfile);
                // 파일이 존재하지 않으면
                //if (!File.Exists(textfile))
                {
                    var stream = new FileStream(textfile, FileMode.OpenOrCreate);
                    // Create a file to write to.
                    using (StreamWriter sw = new StreamWriter(stream, Encoding.Unicode))
                    {
                        string[] strings = f.Split(new string[] { Core.MarkdownWriter.NEWLINE }, StringSplitOptions.None);
                        for (int i = 0; i < strings.Length; i++)
                        {
                            sw.WriteLine(strings[i]);
                            if (strings[i] == string.Empty)
                            {
                                sw.WriteLine("  ");
                            }
                        }
                    }
                }

                //var v = Task.Run(() => Convert(textfile, s.Elements[1].Answer));
                var task = Convert(textfile, s.Elements[1].Answer + " 방송부 지원서");
                //task.Wait();
                //Console.WriteLine(v);
            }
        }
    }
}

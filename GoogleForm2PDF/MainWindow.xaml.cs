using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GoogleForm2PDF.Core;
using Markdown2Pdf;
using System.Diagnostics;
using Markdown2Pdf.Options;
using Markdig;
using System.IO;

namespace GoogleForm2PDF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string TSVFilePath;
        string PDFFileSavePath;
        string[] Questions;
        SurveyAnswer[] Answers;
        private void Convert2PDFButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Survey2MarkdownConvert.ConvertTSV2PDF(TSVFilePath, PDFFileSavePath, TypesForQuestions);
        }
        private void FindSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            //dialog.InitialDirectory = textbox.Text; // Use current value for initial dir
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                PDFFileSavePath = path;
                PDFSavePathTextBlock.Text = PDFFileSavePath;
            }
            // Configure open file dialog box
            //var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.FileName = "Document"; // Default file name
            //dialog.DefaultExt = ""; // Default file extension
            //dialog.Filter = ""; // Filter files by extension

            //// Show open file dialog box
            //bool? result = dialog.ShowDialog();

            //// Process open file dialog box results
            //if (result == true)
            //{//<ComboBox HorizontalAlignment="Left" Margin="522,50,0,0" VerticalAlignment="Top" Width="160"/>
            //    // Open document
            //    //string filename = dialog.FileName;
            //    PDFFileSavePath = dialog.FileName;
            //    PDFSavePathTextBlock.Text = PDFFileSavePath;
            //}
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".tsv"; // Default file extension
            dialog.Filter = "TSV (.tsv)|*.tsv"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {//<ComboBox HorizontalAlignment="Left" Margin="522,50,0,0" VerticalAlignment="Top" Width="160"/>
                // Open document
                //string filename = dialog.FileName;
                TSVFilePath = dialog.FileName;
                TSVFilePathTextBlock.Text = TSVFilePath;
                var v = ReadTSVFile(out string[] questions);
                Answers = v;
                Questions = questions;
                ShowTSVList();
            }
        }
        eMarkdownType[] TypesForQuestions;
        eMarkdownType Convert2(string name)
        {
            if (name == "블록 - 작은 글자")
                return eMarkdownType.Question_Header3_Block;
            if (name == "블록 - 큰 글자")
                return eMarkdownType.Question_Header4_Block;
            if (name == "한줄 - 큰 글자")
                return eMarkdownType.Information_Header3;
            if (name == "한줄 - 작은 글자")
                return eMarkdownType.Information_Header4;
            if (name == "시간(타임스탬프)")
                return eMarkdownType.TimeStamp;

            return eMarkdownType.Information_Header4;
        }
        void ShowTSVList()
        {
            var data = ReadTSVFile(out string[] questions);
            TypesForQuestions = new eMarkdownType[questions.Length];
            for (int i = 0; i < questions.Length; i++)
            {
                ComboBox combobox = new ComboBox();
                combobox.Items.Clear();
                foreach (string item in new List<string>() { 
                    "블록 - 작은 글자", "블록 - 큰 글자",
                    "한줄 - 큰 글자", "한줄 - 작은 글자",
                    "시간(타임스탬프)" })
                {
                    combobox.Items.Add(item);
                }
                combobox.HorizontalAlignment = HorizontalAlignment.Left;
                combobox.VerticalAlignment = VerticalAlignment.Top;
                combobox.Margin = new Thickness(522, 70 + 30*i, 0, 0);
                combobox.Width = 160;
                int k = i;
                combobox.SelectedItem = "블록 - 큰 글자";
                combobox.SelectionChanged += (sender, args) =>
                {
                    TypesForQuestions[k] = Convert2(((string)combobox.SelectedItem));
                };
                this.QuestionListGrid.Children.Add(combobox);


                Rectangle rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.White);
                rect.HorizontalAlignment = HorizontalAlignment.Left;
                rect.Margin = new Thickness(10, 70 + 30 * i, 0, 0);
                rect.Stroke = new SolidColorBrush(Colors.Black);
                rect.VerticalAlignment = VerticalAlignment.Top;
                rect.Width = 507;
                rect.Height = 21;
                QuestionListGrid.Children.Add(rect);

                TextBlock textblock = new TextBlock();
                //<TextBox HorizontalAlignment="Left" Margin="50,120,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="648"/>
                textblock.HorizontalAlignment = HorizontalAlignment.Left;
                textblock.Margin = new Thickness(17, 73 + i * 30, 0, 0);
                textblock.Text = questions[i];
                textblock.TextWrapping = TextWrapping.Wrap;
                textblock.VerticalAlignment = VerticalAlignment.Top;
                textblock.Width = 494;
                QuestionListGrid.Children.Add(textblock);
            }
        }

        SurveyAnswer[] ReadTSVFile(out string[] out_questions)
        {
            out_questions = null;
            //StreamReader sr = new StreamReader(TSVFilePath);
            StreamReader sr = new StreamReader(TSVFilePath);

            // 스트림의 끝까지 읽기
            List<string[]> file = new List<string[]>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split('\t');

                file.Add(data);
            }
            //while (!sr.EndOfStream)
            //{

            //}


            string[] questions = file[0];
            out_questions = questions;
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

                    if (i == 0)
                        q.Type = eMarkdownType.TimeStamp;
                    else if (i == 1 || i == 2 || i == 3)
                        q.Type = eMarkdownType.Information_Header3;
                    else
                        q.Type = eMarkdownType.Question_Header3_Block;

                    answers[line - 1].Elements.Add(q);
                }
            }

            return answers;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

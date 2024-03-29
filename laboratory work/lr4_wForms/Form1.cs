﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using lr5_ClassLib;

namespace lr4_wForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> WordList = new List<string>();
        string text;
        private void loadupFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileOverview = new OpenFileDialog();
            fileOverview.Filter = "текстовые файлы|*.txt";

            if (fileOverview.ShowDialog() == DialogResult.OK)
            {
                Stopwatch time = new Stopwatch();
                time.Start();

                // читаем текст из выбранного файла в строку
                text = File.ReadAllText(fileOverview.FileName);

                // разделители слов в тексте
                char[] separators = new char[] { ' ', '.', ',', '!', '?', '/', '\t', '\n' };

                string[] wordArray = text.Split(separators);

                foreach (string strTemp in wordArray)
                {
                    string str = strTemp.Trim();
                    // добавляем слово в список WordList, если его там нет
                    if (!WordList.Contains(str)) WordList.Add(str);
                }

                time.Stop();
                this.textBoxFileReadTime.Text = time.Elapsed.ToString();
                this.textBoxFileReadCount.Text = WordList.Count.ToString();
            }
            else
            {
                MessageBox.Show("Файл не выбран");
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchingWord = this.searchWord.Text.Trim(); //получаем текст

            if(WordList.Count > 0 && !string.IsNullOrWhiteSpace(searchingWord))
            {
                if(this.checkBoxLevDist.Checked == true) // Если стоит галочка (Задание ЛР5)
                {
                    string originStr = this.searchWord.Text.Trim();
                    int maxDist = int.Parse(this.textBox4.Text.Trim());
                    int digit = 1000; // условно 1000 (не может же быть слова из 1000 букв :) )
                    int i = 0, j = 0;
                    Stopwatch time = new Stopwatch();
                    time.Start();

                    foreach (string str in WordList)
                    {
                        int digitTemp = LevDistance.Distance(originStr, str);
                        if (digitTemp < digit)
                        {
                            digit = digitTemp;
                            i = j; //запонимаем индекс слова в списке, имеющего на данный момент наименьшее расстоятние Л.
                        }
                        j++;
                    }

                    time.Stop();
                    this.textBoxExactTime.Text = time.Elapsed.ToString();

                    this.listBoxResult.BeginUpdate();

                    this.listBoxResult.Items.Clear();


                    if (digit == -1)
                        this.listBoxResult.Items.Add("Пустые строки... Введите слово (текст)");
                    else if (maxDist != 0) // Если пользователь ввел интересующее его расстояние Левенштейна
                    {
                        if (digit <= maxDist)
                        {
                            this.listBoxResult.Items.Add("Найденое слово: " + WordList[i]);
                            this.listBoxResult.Items.Add("Слова можно считать совпадающими");
                            this.listBoxResult.Items.Add("Расстояние Левенштейна: " + digit + " <= " + maxDist);
                        }
                        if (digit > maxDist)
                        {
                            this.listBoxResult.Items.Add("Слово '" + originStr + "' в тексте не найдено");
                            this.listBoxResult.Items.Add("Не одно слово из текста не совпало с искомым");
                            this.listBoxResult.Items.Add("Расстояние Левенштейна: " + digit + " > " + maxDist);
                        }
                    }
                    else // если растояние Левенштейна пользователем не указано ( находим слово с наименьшим расстоянием Левенштейна )
                    {
                        this.listBoxResult.Items.Add("Найденое слово: " + WordList[i]);
                        this.listBoxResult.Items.Add("Расстояние Левенштейна: " + digit);
                    }

                    this.listBoxResult.EndUpdate();

                }
                else // если галочка не стоит (Задание ЛР4)
                {
                    // для поиска в верхнем регистре
                    string wordUpper = searchingWord.ToUpper();

                    // временные результаты поиска
                    List<string> tempList = new List<string>();

                    Stopwatch time = new Stopwatch();
                    time.Start();
                    foreach (string str in WordList)
                    {
                        if (str.ToUpper().Contains(wordUpper))
                        {
                            tempList.Add(str);
                        }
                    }

                    time.Stop();
                    this.textBoxExactTime.Text = time.Elapsed.ToString();

                    this.listBoxResult.BeginUpdate();

                    // отчистка listBox
                    this.listBoxResult.Items.Clear();

                    // вывод найденного слова 
                    foreach (string str in tempList)
                    {
                        string massege = "Найденное слово: ";
                        this.listBoxResult.Items.Add(massege + str);
                    }
                    this.listBoxResult.EndUpdate();

                    if (tempList.Count == 0)
                    {
                        string massege = ":/ искомого слова в тексте нет :/";
                        this.listBoxResult.Items.Add(massege);
                    }
                    
                }
                
            }
            else
            {
                MessageBox.Show("Необходимо выбрать файл и ввести слово для поиска");
            }


        }

        private void textButton_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                MessageBox.Show(
                  text,
                  "Текст из файла"
                );
            }
            else
            {
                
                MessageBox.Show(
                  "Вы не выбрали файл",
                  "Текст из файла"
                );
            }
        }

        public static List<ResultsOfParallelSearch> ArrayThreadTask(object paramObj)
        {
            ParallelSearchThreadParam param = (ParallelSearchThreadParam)paramObj;

            //Слово для поиска в верхнем регистре
            string wordUpper = param.wordPattern.Trim().ToUpper();

            //Результаты поиска в одном потоке
            List<ResultsOfParallelSearch> Result = new List<ResultsOfParallelSearch>();

            //Перебор всех слов во временном списке данного потока 
            foreach (string str in param.tempList)
            {
                //Вычисление расстояния Дамерау-Левенштейна
                int dist = DistanceEditing.Distance(str.ToUpper(), wordUpper);

                //Если расстояние меньше порогового, то слово добавляется в результат
                if (dist <= param.maxDist)
                {
                    ResultsOfParallelSearch temp = new ResultsOfParallelSearch()
                    {
                        word = str,
                        dist = dist,
                        ThreadNum = param.ThreadNum
                    };

                    Result.Add(temp);
                }
            }
            return Result;
        }

        private void button1_Click(object sender, EventArgs e) //Параллельный поиск
        {
            string searchingWord = this.searchWord.Text.Trim(); //получаем текст

            if (WordList.Count > 0 && !string.IsNullOrWhiteSpace(searchingWord))
            {
                int maxDist;

                if (!int.TryParse(this.MaxDistField.Text.Trim(), out maxDist))
                {
                    MessageBox.Show("Необходимо указать максимальное расстояние");
                    return;
                }

                if (maxDist < 1 || maxDist > 5)
                {
                    MessageBox.Show("Максимальное расстояние должно быть в диапазоне от 1 до 5");
                    return;
                }

                int ThreadCount;
                if (!int.TryParse(this.TreadsQuant.Text.Trim(), out ThreadCount))
                {
                    MessageBox.Show("Необходимо указать количество потоков");
                    return;
                }

                Stopwatch time = new Stopwatch();
                time.Start();


                //Список результатов  
                List<ResultsOfParallelSearch> Result = new List<ResultsOfParallelSearch>();

                //Деление списка на фрагменты для параллельного запуска в потоках
                List<MiniMax> arrayDivList = SabsArrays.DivideSubArrays(0, WordList.Count, ThreadCount);
                int count = arrayDivList.Count;

                //Количество потоков соответствует количеству фрагментов массива
                Task<List<ResultsOfParallelSearch>>[] tasks = new Task<List<ResultsOfParallelSearch>>[count];

                //Запуск потоков
                for (int i = 0; i < count; i++)
                {
                    //Создание временного списка, чтобы потоки не работали параллельно с одной коллекцией
                    List<string> tempTaskList = WordList.GetRange(arrayDivList[i].Min, arrayDivList[i].Max - arrayDivList[i].Min);

                    tasks[i] = new Task<List<ResultsOfParallelSearch>>(
                        //Метод, который будет выполняться в потоке
                        ArrayThreadTask,
                        //Параметры потока 
                        new ParallelSearchThreadParam()
                        {
                            tempList = tempTaskList,
                            maxDist = maxDist,
                            ThreadNum = i,
                            wordPattern = searchingWord
                        });

                    //Запуск потока
                    tasks[i].Start();
                }

                Task.WaitAll(tasks);

                time.Stop();

                //Объединение результатов
                for (int i = 0; i < count; i++)
                {
                    Result.AddRange(tasks[i].Result);
                }
                //Конец поиска

                time.Stop();

                //Вывод результатов

                this.TimeOfParallelSearch.Text = time.Elapsed.ToString();

                this.FoundTreads.Text = count.ToString();

                this.listBoxResult.BeginUpdate();

                this.listBoxResult.Items.Clear();

                //Вывод результатов поиска 
                foreach (var x in Result)
                {
                    string temp = "Найденое слово: " + x.word;
                    this.listBoxResult.Items.Add(temp);
                    this.listBoxResult.Items.Add("Расстояние Левенштейна: " + x.dist.ToString());
                    this.listBoxResult.Items.Add("Поток(-ов): " + x.ThreadNum.ToString());
                }

                this.listBoxResult.EndUpdate();
            }
            else
            {
                MessageBox.Show("Необходимо выбрать файл и ввести слово для поиска");
            }
        }

        private void button2_Click(object sender, EventArgs e) //Печать отчета
        {
            //Имя файла отчета
            string TempReportFileName = "Report_" + DateTime.Now.ToString("dd_MM_yyyy_hhmmss");

            if (this.checkBoxHtml.Checked == true) //Отчет в формате html
            {
                //Диалог сохранения файла отчета
                SaveFileDialog fd = new SaveFileDialog();
                fd.FileName = TempReportFileName;
                fd.DefaultExt = ".html";
                fd.Filter = "HTML Reports|*.html";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    string ReportFileName = fd.FileName;

                    //Формирование отчета
                    StringBuilder b = new StringBuilder();
                    b.AppendLine("<html>");

                    b.AppendLine("<head>");
                    b.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>");
                    b.AppendLine("<title>" + "Отчет: " + ReportFileName + "</title>");
                    b.AppendLine("</head>");

                    b.AppendLine("<body>");

                    b.AppendLine("<h1>" + "Отчет: " + ReportFileName + "</h1>");
                    b.AppendLine("<table border='1'>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Время чтения из файла</td>");
                    b.AppendLine("<td>" + this.textBoxFileReadTime.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Количество уникальных слов в файле</td>");
                    b.AppendLine("<td>" + this.textBoxFileReadCount.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Слово для поиска</td>");
                    b.AppendLine("<td>" + this.searchWord.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Максимальное расстояние для нечеткого поиска</td>");
                    b.AppendLine("<td>" + this.MaxDistField.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Время четкого поиска</td>");
                    b.AppendLine("<td>" + this.textBoxExactTime.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr>");
                    b.AppendLine("<td>Время нечеткого поиска</td>");
                    b.AppendLine("<td>" + this.TimeOfParallelSearch.Text + "</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("<tr valign='top'>");
                    b.AppendLine("<td>Результаты поиска</td>");
                    b.AppendLine("<td>");
                    b.AppendLine("<ul>");

                    foreach (var x in this.listBoxResult.Items)
                    {
                        b.AppendLine("<li>" + x.ToString() + "</li>");
                    }

                    b.AppendLine("</ul>");
                    b.AppendLine("</td>");
                    b.AppendLine("</tr>");

                    b.AppendLine("</table>");

                    b.AppendLine("</body>");
                    b.AppendLine("</html>");

                    //Сохранение файла
                    File.AppendAllText(ReportFileName, b.ToString());

                    MessageBox.Show("Отчет сформирован. Файл: " + ReportFileName);
                }

            }
            if (this.checkBoxTxt.Checked == true) // Отчет в формате txt
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.FileName = TempReportFileName;
                fd.DefaultExt = ".txt";
                fd.Filter = "TXT Reports|*.txt";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    string ReportFileName = fd.FileName;

                    //Формирование отчета
                    StringBuilder b = new StringBuilder();

                    b.AppendLine("Отчет: " + ReportFileName + " ;");

                    b.AppendLine("\nВремя чтения из файла: " + this.textBoxFileReadTime.Text + " ;");

                    b.AppendLine("Количество уникальных слов в файле: " + this.textBoxFileReadCount.Text + " ;");

                    b.AppendLine("Слово для поиска: " + this.searchWord.Text + " ;");

                    b.AppendLine("Максимальное расстояние для нечеткого поиска: " + this.MaxDistField.Text + " ;");

                    b.AppendLine("Время четкого поиска: " + this.textBoxExactTime.Text + " ;");

                    b.AppendLine("Время нечеткого поиска: " + this.TimeOfParallelSearch.Text + " ;");

                    b.AppendLine("Результаты поиска: ");

                    int i = 0;
                    foreach (var x in this.listBoxResult.Items)
                    {
                        b.AppendLine((i + 1) + ") " + x.ToString());
                        i++;
                    }

                    //Сохранение файла
                    File.AppendAllText(ReportFileName, b.ToString());

                    MessageBox.Show("Отчет сформирован. Файл: " + ReportFileName);
                }
            }
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laba2_1;

namespace lr3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title ="Лабораторная работа 3";
            ColorfulPrint("Студент:\tСавельев Алексей\nГруппа:\t\tИУ5-34Б", "Cyan");
            ColorfulPrint("Работа со списками и коллекциями C#", "Yellow");

            // Массив для размеров фигур
            // строки-фигуры: Прямогольник, Квадрат, Круг; 
            // столбцы-размеры: Длина, Ширина, или Длина стороны, или Радиус;
            double[,] dimensions = new double[3, 2]; 
            string str; // временная сточная переменная для консольного ввода
            int stop; // флажок выхода
            do
            {
                // блок замолнения массива размеров фигур
                Console.WriteLine("Введите длину и ширину Прямоугольника (через Enter:)");
                str = Console.ReadLine();
                double.TryParse(str, out dimensions[0, 0]);
                str = Console.ReadLine();
                double.TryParse(str, out dimensions[0, 1]);
                Console.WriteLine("Введите длину стороны Квадрата:");
                str = Console.ReadLine();
                double.TryParse(str, out dimensions[1, 0]);
                Console.WriteLine("Введите длину радиуса Круга:");
                str = Console.ReadLine();
                double.TryParse(str, out dimensions[2, 0]);

                // инициализация обьектов классов
                Rectangle rect = new Rectangle(dimensions[0, 0], dimensions[0, 1]);
                Squad squad = new Squad(dimensions[1, 0]);
                Circle circ = new Circle(dimensions[2, 0]);

                // ArrayList
                ArrayList arr = new ArrayList();
                arr.Add(rect);
                arr.Add(squad);
                arr.Add(circ);

                ColorfulPrint("\nИсходный ArrayList:", "DarkGreen");
                foreach (var x in arr) Console.WriteLine(x);
                arr.Sort(); // сортировка
                ColorfulPrint("\n\nОтсоритрованные ArrayList:", "DarkGreen");
                foreach (var x in arr) Console.WriteLine(x);

                // Список (List)
                List<GeomFigure> list = new List<GeomFigure>();
                list.Add(rect);
                list.Add(squad);
                list.Add(circ);

                ColorfulPrint("\nИсходный List:", "DarkGreen");
                foreach (var x in list) Console.WriteLine(x);
                list.Sort(); // сортировка
                ColorfulPrint("\nОтсортированный List:", "DarkGreen");
                foreach (var x in list) Console.WriteLine(x);

                // Трехмерная матрица
                // печать матрицы на экран была реализована "послойно"
                // т.е. сколько матрица имеет "в глубину", столько и будет
                // напечатано двумерных матрицы (слоёв)
                ColorfulPrint("\nТрехмерная матрица:", "DarkGreen");
                Matrix<GeomFigure> matrix = new Matrix<GeomFigure>(3, 3, 3, new FigureMatrixCheckEmpty());
                matrix[0, 0, 0] = rect;
                matrix[1, 1, 1] = squad;
                matrix[2, 2, 2] = circ;
                Console.WriteLine(matrix.ToString());

                // SimpleList
                SimpleList<GeomFigure> sList = new SimpleList<GeomFigure>();
                sList.Add(rect);
                sList.Add(squad);
                sList.Add(circ);
                ColorfulPrint("\nИсходный SimpleList:", "DarkGreen");
                foreach (var x in sList) Console.WriteLine(x);
                sList.Sort(); // сортировка
                ColorfulPrint("\nОтсортированный SimpleList:", "DarkGreen");
                foreach (var x in sList) Console.WriteLine(x);

                // SimpleStack
                ColorfulPrint("\nSimpleStack:", "DarkGreen");
                SimpleStack<GeomFigure> sStack = new SimpleStack<GeomFigure>();
                sStack.Push(rect);
                sStack.Push(squad);
                sStack.Push(circ);

                GeomFigure figure;
                while(sStack.count > 0)
                {
                    figure = sStack.Pop();
                    Console.WriteLine(figure);
                }

                // выйти или продолжить
                ColorfulPrint("\nЗавершить?", "Magenta");
                Console.WriteLine("0) Нет\n1) Да");
                int.TryParse(Console.ReadLine(), out stop);

            } while (stop != 1);

            ColorfulPrint("\nКонец...", "Red");
            Console.ReadKey();

        }

        static void ColorfulPrint(string outtext, string color)
        {
            switch(color)
            {
                case "Black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case "Blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "Cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "DarkBlue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "DarkCyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "DarkGray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "DarkGreen":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "DarkMagenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "DarkRed":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "DarkYellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "Gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "Green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "Magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "Red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "White":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.WriteLine(outtext);
            Console.ResetColor();
        }
    }
}

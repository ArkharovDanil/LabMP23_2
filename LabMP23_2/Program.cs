using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
namespace LabMP23_2
{
    class MyThread
    {
        public Thread Thrd;
        int[] a;
        public int length;
        public MyThread(string name, int[] array)
        {
            a = array;
            Thrd = new Thread(this.Run);
            Thrd.Name = name;
            Thrd.Start();
            length= a.Length;   
        }
        void Run()
        {
            Console.WriteLine(Thrd.Name + "начат.");
            a = Program.BubbleSort(a);
            Console.WriteLine(Thrd.Name + "завершён.");
            // Program.Show(a);
        }
        public int[] ReturnSortArray()
        {
            return a;
        }
    }
    class Program
    {

        static void Main()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int countOfElements = 100000;
            int countOfThreads = 2;
            int[] mas;
            int difference;
            int min;
            int max;
            int L;
            // mas = GenerateArray(countOfElements);//генерируем нехитрый массив
            mas = CunningGenerateArray(countOfElements);//генерируем хитрый массив
            max = Program.FindMax(mas);
            min = Program.FindMin(mas);
            difference = max-min;
            L = difference / countOfThreads;
            // Show(mas);
           
            List<MyThread> listOfThread = new List<MyThread>();


            for (int i = 0; i < countOfThreads; i++)
            {
                int countOfAdoptedElements;
                if (i != countOfThreads-1)
                {
                    countOfAdoptedElements = Program.CountAdoptedElem(mas,min+L*i,min+L*(i+1));
                }
                else 
                { 
                    countOfAdoptedElements = Program.CountAdoptedElemLast(mas, min + L * i, max);
                }

                int[] tempArray = new int[countOfAdoptedElements];
                if (i != countOfThreads-1)
                {
                    tempArray = Program.ReturnAdoptedArray(mas, min + L * i, min + L * (i + 1), countOfAdoptedElements);        
                }
                else
                {
                    tempArray = Program.ReturnAdoptedArrayForLast(mas, min + L * i, max, countOfAdoptedElements);
                }
                    // for (int j=0; j < countOfElements / countOfThreads; j++)   

                    string name = "Thread" + i.ToString();
                MyThread temp = new MyThread(name, tempArray);
                listOfThread.Add(temp);
            }

            foreach (MyThread temp in listOfThread)
            {
                temp.Thrd.Join();
            }
            Console.WriteLine("Потоки завершены.");

            List<int[]> arrays = new List<int[]>();
            foreach (MyThread temp in listOfThread)
            {
                int[] tempArray = new int[temp.length];
                tempArray = temp.ReturnSortArray();
                arrays.Add(tempArray);
            }
            int[] answer = new int[countOfElements];
            answer = Program.MergeArrays(countOfThreads, arrays, countOfElements);
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            Console.WriteLine(time);
            // Show(answer);
        }
        public static void Show(int[] array)//вывод массива
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(array[i]);
            }

        }


        public static int[] MergeArrays(int count, List<int[]> list, int COE)//слияние массивов
        {
            int[] answer = new int[COE];
            int t = 0;
            for (int i = 0; i < count; i++)
            {
              for (int j=0;j<list[i].Length;j++)
                {
                    answer[t] = list[i][j];
                    t++;
                }
            }

            return answer;
        }


        public static int CountAdoptedElem(int[] mas,int min,int max)//считаем количество элементов подходящих в отрезок для всех потоков,кроме последнего
        {
            int count=0;
            for (int i = 0; i < mas.Length; i++)
            {
                if ((mas[i]>=min)&&(mas[i]<max))
                {
                    count++;
                }
                
            }
            return count;
        }
        
        public static int CountAdoptedElemLast(int[] mas, int min, int max)//считаем количество элементов подходящих в отрезок для последнего потока
        {
            int count = 0;
            for (int i = 0; i < mas.Length; i++)
            {
                if ((mas[i] >= min) && (mas[i] <= max))
                {
                    count++;
                }

            }
            return count;
        }
        public static int[] ReturnAdoptedArray(int[] mas, int min, int max,int count)//создание массива для набора  потока,кроме последнего
        {
            int[] answer=new int[count];
            int j = 0;
            for (int i = 0; i < mas.Length; i++)
            {
                if ((mas[i] >= min) && (mas[i] < max))
                {
                   answer[j]=mas[i] ;
                    j++;
                }

            }
            return answer;
        }
        public static int[] ReturnAdoptedArrayForLast(int[] mas, int min, int max, int count)//создание массива для набора последнего потока
        {
            int[] answer = new int[count];
            int j = 0;
            for (int i = 0; i < mas.Length; i++)
            {
                if ((mas[i] >= min) && (mas[i] <= max))
                {
                    answer[j] = mas[i];
                    j++;
                }

            }
            return answer;
        }
        static int FindMax(int[] mas)//поиск максимального элемента массива
        {
            int maxElem = int.MinValue;
           
            for (int j = 0; j < mas.Length; j++)
            {
                if (maxElem < mas[j])
                {
                    maxElem = mas[j];
                   
                }
            }
            return maxElem;
        }
        static int FindMin(int[] mas)//поиск минимального элемента массива
        {
            int minElem = int.MaxValue;
            for (int j = 0; j < mas.Length; j++)
            {
                if (minElem > mas[j])
                {
                    minElem = mas[j];
                    
                }
            }
            return minElem;
        }
        static int[] GenerateArray(int k)//создание массива заданной размерности
        {
            int[] array = new int[k];
            Random rand = new Random();
            for (int i = 0; i < array.Length; i++)
                array[i] = rand.Next(10); // [0 - 2^31)
            return array;
        }
        static int[] CunningGenerateArray(int k)//создание хитрого массива заданной размерности
        {
            int[] array = new int[k];
            Random rand = new Random();
            
            for (int i = 0; i < array.Length; i++)
            {
                int t = rand.Next(100);
                if (t > 10)
                {
                    array[i] = 1;
                }
                else
                {
                    array[i] = 1000;
                }
                
            }

                
            return array;
        }
        public static int[] BubbleSort(int[] array)//сортировка массива пузырьком
        {
            int temp;
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] > array[j])
                    {
                        temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
            return array;
        }
    }
}


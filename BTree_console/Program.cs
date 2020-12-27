using BTree_lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BTree_console
{
    class Program
    {
        public static Stopwatch sw = new Stopwatch();
        static void Main(string[] args)
        {
            var tree = new BTree<int,int>(100);
            var path = @".\measures\";
            var rnd = new Random();

            var list = new List<int>();
            foreach (var line in File.ReadAllLines(@".\students.txt").Skip(1).Select(x => x.Split(';')))
            {
                var st = new Student(int.Parse(line[0]), line[1], line[2], line[3], line[4], int.Parse(line[5]));
                list.Add(1);
            }
            DoAndMeasure(path, tree, list, tree.Insert);
        }

        static void DoAndMeasure<V>(string path, BTree<int,V> tree, List<V> data, Action<int, int> act)
        {
            path = path + act.Method.Name + "11.csv";
            for (int j = 0; j < 10; j++)
            {
                int prev = 1;
                int prevprev = 1;
                for (int i = 1; i < data.Count; i = i+10000)
                {
                    //tree = new BTree<int, V>(tree.Degree);
                    Measure(path, tree, data.Take(i).ToList(), act);
                    prevprev = prev;
                    prev = i;
                }
            }
        }

        private static void Measure<V>(string path, BTree<int, V> tree, List<V> data, Action<int, int> act)
        {
            sw.Start();
            for (int i = 0; i < data.Count; i++)
            {
                act(tree.Count,tree.Degree*tree.Height);
            }
            sw.Stop();
            File.AppendAllText(path, $"{tree.Count};{sw.ElapsedMilliseconds};\n");
        }
    }
}

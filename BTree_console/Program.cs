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
            var tree = new BTree<int,int>(4);
            //var path = @".\measures\";
            int count = 100;
            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                tree.Insert(rnd.Next(100), 1);
            }
            var list = new List<int>();
            Num(tree.Root);
        }

        static void Num(Node<int,int> node)
        {
            if (node.Children.Count() != 0)
                foreach (var c in node.Children)
                    Num(c);
            foreach (var e in node.Entries)            
                Console.WriteLine(e.Key);            
        }

        static void DoAndMeasure(string path, BTree<int,int> tree, List<int> data, Action<int, int> act)
        {
            path = path + act.Method.Name + ".csv";
            int prev = 1;
            int prevprev = 1;
            for (int i = 1; i < data.Count; i=prev+prevprev)
            {
                Measure(path, tree, data.Take(i).ToList(), act);
                prevprev = prev;
                prev = i;
            }
        }

        private static void Measure(string path, BTree<int, int> tree, List<int> data, Action<int, int> act)
        {
            sw.Start();
            for (int i = 0; i < data.Count; i++)
            {
                act(data[i], 1);
            }
            sw.Stop();
            File.AppendAllText(path, $"{data.Count};{sw.ElapsedMilliseconds}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
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
using BTree_lib;

namespace Z17
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    class Program
    {
        static void Main2(string[] args)
        {
            var tree = new BTree<int, NodeTable>(3);
            InitDB(tree);
            tree.Delete(3);
        }

        static void InitDB(BTree<int, NodeTable> tree)
        {
            foreach (var line in File.ReadAllLines(@".\students.txt").Skip(1).Select(x => x.Split(';')))
                tree.Insert(int.Parse(line[0]), new NodeTable(int.Parse(line[0]), line[1], line[2], line[3], line[4], int.Parse(line[5])));

        }
    }

    public class NodeTable
    {
        //[Идентификатор студента] [Фамилия] [Имя] [Отчество] [Название факультета] [Номер курса]
        public NodeTable(int id, string ln, string fn, string sn, string fac, int course)
        {
            this.id = id;
            LastName = ln;
            FirstName = fn;
            SecondName = sn;
            Faculty = fac;
            CourseNumber = course;
        }
        public int id { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Faculty { get; private set; }
        public int CourseNumber { get; private set; }
    }

}

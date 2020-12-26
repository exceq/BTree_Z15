using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        BTree<int, Student> tree;
        ObservableCollection<Student> students;
        public MainWindow()
        {
            InitializeComponent();
            tree = new BTree<int, Student>(100);
            //Program.InitDB(tree);
            students = new ObservableCollection<Student>();
            InitBD(students, tree);
            studentsGrid.ItemsSource = students;//students;
            var box = FindName("cb") as ComboBox;

            box.SelectionChanged += ActionSelected;
        }

        private void ActionSelected(object sender, RoutedEventArgs e)
        {
            var bt = FindName("bt") as Button;
            bt.IsEnabled = IsCorrect();
            var dockPanels = new DockPanel[] { idPanel, firstName, secondName, lastName, faculty, courseNumber };
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == 0)
                foreach (var el in dockPanels)
                    el.Visibility = Visibility.Visible;
            else
                foreach (var el in dockPanels.Skip(1))
                    el.Visibility = Visibility.Hidden;
        }

        private void Text_Changed(object sender, RoutedEventArgs e)
        {
            var bt = FindName("bt") as Button;
            bt.IsEnabled = IsCorrect();
        }

        private bool IsCorrect()
        {
            var cb = FindName("cb") as ComboBox;
            if (cb.SelectedIndex != 0)
            {
                return int.TryParse(idPanelText.Text, out int res);
            }
            else
            {
                if (!int.TryParse(idPanelText.Text, out int res)
                    || !int.TryParse(courseNumberText.Text, out int res1))
                {
                    return false;
                }
                if (string.IsNullOrWhiteSpace(firstNameText.Text)
                    || string.IsNullOrWhiteSpace(lastNameText.Text)
                    || string.IsNullOrWhiteSpace(facultyText.Text))
                    return false;
                return true;
            }
        }

        private void OffFilter(object sender, RoutedEventArgs e)
        {
            studentsGrid.ItemsSource = null;
            studentsGrid.ItemsSource = students;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            var box = FindName("cb") as ComboBox;
            if (box.SelectedIndex == 0)
            {
                Student st = new Student(int.Parse(idPanelText.Text),
                    lastNameText.Text,
                    firstNameText.Text,
                    secondNameText.Text,
                    facultyText.Text,
                    int.Parse(courseNumberText.Text));
                tree.Insert(st.id, st);
                students.Add(st);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(idPanelText.Text))
                    MessageBox.Show("Введите корректное значение в поле.");
                else
                {
                    if (int.TryParse(idPanelText.Text, out int id))
                    {
                        if (!tree.Contains(id))
                        {
                            MessageBox.Show("Такого значения не существует.", "Ошибка");
                            return;
                        }
                        if (box.SelectedIndex == 1)
                        {
                            students.Remove(tree.Search(id).Value);
                            tree.Delete(id);
                        }
                        else
                        {
                            tree.Search(id);
                            studentsGrid.ItemsSource = students.Where(st => st.id == id);
                        }
                    }
                    else
                        MessageBox.Show("Введите коректное значение в поле.", "Ошибка");
                    
                }
            }
            idPanelText.Text = "";
            firstNameText.Text = "";
            lastNameText.Text = "";
            secondNameText.Text = "";
            facultyText.Text = "";
            courseNumberText.Text = "";
        }

        void InitBD(ObservableCollection<Student> students, BTree<int, Student> tree)
        {
            var lines = File.ReadAllLines(@".\students.txt");
            var st = lines.Skip(1).Select(x => x.Split(';')).ToArray();
            for (int i = 0; i < st.Count(); i++)
                students.Add(new Student(int.Parse(st[i][0]), st[i][1], st[i][2], st[i][3], st[i][4], int.Parse(st[i][5])));
            Program.InitDB(tree);
        }

    }
    class Program
    {
        static void Main2(string[] args)
        {
            var tree = new BTree<int, Student>(100);
            InitDB(tree);
            tree.Delete(3);
        }

        public static void InitDB(BTree<int, Student> tree)
        {
            foreach (var line in File.ReadAllLines(@".\students.txt").Skip(1).Select(x => x.Split(';')))
                tree.Insert(int.Parse(line[0]), new Student(int.Parse(line[0]), line[1], line[2], line[3], line[4], int.Parse(line[5])));

        }
    }

    public class Student
    {
        //[Идентификатор студента] [Фамилия] [Имя] [Отчество] [Название факультета] [Номер курса]
        public Student(int id, string ln, string fn, string sn, string fac, int course)
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

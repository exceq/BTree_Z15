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

namespace Z15
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BTree_INCC<int, Student> tree;
        public MainWindow()
        {
            InitializeComponent();

            tree = new BTree_INCC<int, Student>(100);            

            InitTree(tree);

            studentsGrid.ItemsSource = tree;
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

        private void OffFilter_Click(object sender, RoutedEventArgs e)
        {
            studentsGrid.ItemsSource = null;
            studentsGrid.ItemsSource = tree;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            var box = FindName("cb") as ComboBox;
            if (box.SelectedIndex == 0)
            {
                var st = new Student(int.Parse(idPanelText.Text),
                    lastNameText.Text,
                    firstNameText.Text,
                    secondNameText.Text,
                    facultyText.Text,
                    int.Parse(courseNumberText.Text));
                tree.Insert(st.id, st);
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
                            tree.Delete(id);
                        }
                        else
                        {
                            studentsGrid.ItemsSource = tree.Where(st => st.id == id);
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

        void InitBD(ObservableCollection<Student> students, BTree_INCC<int, Student> tree)
        {
            var lines = File.ReadAllLines(@".\students.txt");
            var st = lines.Skip(1).Select(x => x.Split(';')).ToArray();
            for (int i = 0; i < st.Count(); i++)
                students.Add(new Student(int.Parse(st[i][0]), st[i][1], st[i][2], st[i][3], st[i][4], int.Parse(st[i][5])));
            InitTree(tree);
        }

        void InitTree(BTree_INCC<int, Student> tree)
        {
            foreach (var line in File.ReadAllLines(@".\students.txt").Skip(1).Select(x => x.Split(';')))
            {
                var st = new Student(int.Parse(line[0]), line[1], line[2], line[3], line[4], int.Parse(line[5]));
                tree.Insert(st.id, st);
            }
        }

    }
    

}

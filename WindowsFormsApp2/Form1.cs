using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public interface IPersonView
        {
            string Name { get; set; }
            int Age { get; set; }

            event EventHandler AddPerson;
            event EventHandler SearchPerson;
        }
        public class PersonPresenter
        {
            private readonly IPersonView _view;
            private readonly List<Person> _people;

            public PersonPresenter(IPersonView view)
            {
                _view = view;
                _people = new List<Person>();

                _view.AddPerson += AddPerson;
                _view.SearchPerson += SearchPerson;
            }

            private void AddPerson(object sender, EventArgs e)
            {
                var person = new Person
                {
                    Name = _view.Name,
                    Age = _view.Age
                };

                _people.Add(person);
            }
            private void SearchPerson(object sender, EventArgs e)
            {
                string searchName = _view.Name;
                var result = _people.FirstOrDefault(person => person.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));

                listView1.Items.Clear();
                listView1.Items.Add(result);

            }
            private void SavePeopleToFile(List<Person> people, string filePath)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (Person person in people)
                        {
                            writer.WriteLine($"{person.Name},{person.Age}");
                        }
                    }

                    MessageBox.Show("Данные успешно сохранены в файл.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}");
                }
            }

            private void Save_Click(object sender, EventArgs e)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    SavePeopleToFile(_people, filePath);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.Clear(); 

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2)
                        {
                            string name = parts[0].Trim();
                            int age;
                            if (int.TryParse(parts[1].Trim(), out age))
                            {
                                ListViewItem item = new ListViewItem(name);
                                item.SubItems.Add(age.ToString());
                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при чтении файла: {ex.Message}");
            }
        }
    }
}

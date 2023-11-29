using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace laba5INT
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> books;
        private Dictionary<string, List<string>> chapters;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBooks();
            PopulateBookList();

            bookListBox.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X)
            {
                if (bookListBox.SelectedItem != null)
                {
                    string selectedBook = bookListBox.SelectedItem.ToString();
                    DeleteBook(selectedBook);
                }
            }          

        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            ChooseFolder();
        }

        private void DeleteBook(string book)
        {
            if (books.ContainsKey(book))
            {
                books.Remove(book);
                chapters.Remove(book);

                chapterListBox.Items.Clear();
                chapterTextBlock.Text = string.Empty;
                RefreshBooksList(); // Вызов метода обновления списка книг
            }
        }

        private void RefreshBooksList()
        {
            bookListBox.Items.Clear(); // Очищаем список книг в интерфейсе
            foreach (var book in books.Keys)
            {
                bookListBox.Items.Add(book); // Добавляем книги из словаря books в список книг в интерфейсе
            }
        }
        private void ChooseFolder()
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = Path.GetFileName(folderBrowserDialog.SelectedPath);
                if (!books.ContainsKey(folderName))
                {
                    books[folderName] = folderBrowserDialog.SelectedPath; // Добавляем выбранную папку в словарь books
                    chapters[folderName] = new List<string>(); // Создаем пустой список глав для новой книги
                    PopulateBookList();
                }
                else
                {
                    MessageBox.Show("Эта книга уже существует в списке.");
                }

                PopulateChapterList(folderName);
                if (chapters[folderName].Count > 0)
                {
                    LoadChapter(folderName, chapters[folderName][0]);
                }
            }
        }

        private void InitializeBooks()
        {
            books = new Dictionary<string, string>
            {
                {"Айболит", "C:\\Users\\user\\Desktop\\Новая папка\\Айболит"},
                {"Федорино горе", "C:\\Users\\user\\Desktop\\Новая папка\\Федорино горе"},
                {"Телефон", "C:\\Users\\user\\Desktop\\Новая папка\\телефон"}
            };

            chapters = new Dictionary<string, List<string>>();

            foreach (var book in books.Keys)
            {
                string bookPath = books[book];
                var chapterFiles = Directory.GetFiles(bookPath, "*.txt");

                chapters[book] = new List<string>();

                foreach (var chapterFile in chapterFiles)
                {
                    // Получаем только имя файла без расширения
                    string chapterName = Path.GetFileNameWithoutExtension(chapterFile);
                    chapters[book].Add(chapterName);
                }
            }
        }

        private void PopulateBookList()
        {
            bookListBox.Items.Clear();
            foreach (var book in books.Keys)
            {
                bookListBox.Items.Add(book);
            }
        }

        private void PopulateChapterList(string bookFolder)
        {
            string[] chapterFiles = Directory.GetFiles(books[bookFolder], "*.txt");
            chapters[bookFolder].Clear(); // Очистим список глав, чтобы избежать дублирования
            foreach (var chapterFile in chapterFiles)
            {
                string chapterName = Path.GetFileNameWithoutExtension(chapterFile);
                chapters[bookFolder].Add(chapterName);
            }

            chapterListBox.Items.Clear(); // Очищаем список глав в интерфейсе
            foreach (var chapter in chapters[bookFolder])
            {
                chapterListBox.Items.Add(chapter); // Добавляем главы из словаря chapters в список глав в интерфейсе
            }
        }

        private void LoadChapter(string book, string chapter)
        {
            try
            {
                string bookPath = books[book];
                string chapterPath = Path.Combine(bookPath, $"{chapter}.txt");
                string[] lines = File.ReadAllLines(chapterPath);
                chapterTextBlock.Text = string.Join("\n", lines);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка загрузки главы: {ex.Message}");
            }
        }

        private void bookListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookListBox.SelectedItem != null)
            {
                string selectedBook = bookListBox.SelectedItem.ToString();
                PopulateChapterList(selectedBook);
            }
        }

        private void chapterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chapterListBox.SelectedItem != null)
            {
                string selectedBook = bookListBox.SelectedItem.ToString();
                string selectedChapter = chapterListBox.SelectedItem.ToString();
                LoadChapter(selectedBook, selectedChapter);
            }
        }
    }
}

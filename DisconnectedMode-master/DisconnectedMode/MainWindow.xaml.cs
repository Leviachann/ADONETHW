using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace DisconnectedMode
{
    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
        private DataTable authorsTable;

        public MainWindow()
        {
            InitializeComponent();
            authorsTable = new DataTable();
            authorsTable = CreateAuthorsTable();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            string authorName = txtAuthorName.Text;
            if (!string.IsNullOrEmpty(authorName))
            {
                try
                {
                    DataRow newRow = authorsTable.NewRow();
                    newRow["AuthorName"] = authorName;
                    authorsTable.Rows.Add(newRow);
                    SaveChanges();
                    MessageBox.Show("Author inserted successfully!");
                    txtAuthorName.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter an author name.");
            }
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT AuthorID, AuthorName FROM Authors", connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.Fill(authorsTable);
                }
                lstAuthors.ItemsSource = authorsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedAuthor = lstAuthors.SelectedItem as DataRowView;
            if (selectedAuthor != null)
            {
                int authorID = Convert.ToInt32(selectedAuthor["AuthorID"]);
                string authorName = selectedAuthor["AuthorName"].ToString();

                // Show a confirmation message box
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the author with ID: " + authorID + " and Name: " + authorName + "?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // User confirmed deletion, proceed with deleting the author
                    try
                    {
                        DataRow[] rowsToDelete = authorsTable.Select("AuthorID = " + authorID);
                        if (rowsToDelete.Length > 0)
                        {
                            rowsToDelete[0].Delete();
                            SaveChanges();
                            MessageBox.Show("Author deleted successfully!");
                            authorsTable.AcceptChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an author to delete.");
            }
        }


        private void SaveChanges()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Authors", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(authorsTable);
            }
        }

        private DataTable CreateAuthorsTable()
        {
            DataTable authorsTable = new DataTable("Authors");
            authorsTable.Columns.Add("AuthorID", typeof(int));
            authorsTable.Columns["AuthorID"].AutoIncrement = true;
            authorsTable.Columns["AuthorID"].AutoIncrementSeed = 1;
            authorsTable.Columns.Add("AuthorName", typeof(string));
            authorsTable.PrimaryKey = new DataColumn[] { authorsTable.Columns["AuthorID"] };
            return authorsTable;
        }
    }
}

using Contatos.Modelo;
using System;
using System.Collections.Generic;
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

//namespace do sqlite
using System.Data.SQLite;
using System.IO;

using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Path = System.IO.Path;
using System.Data;

namespace Contatos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection? sqlite = null;
        private string baseDados;
        private string strConection;
        private string dirPath;
        public MainWindow()
        {
            InitializeComponent();
            
            baseDados = Path.Combine(Directory.GetCurrentDirectory(), @".\db\dbsqlite.db");
            strConection = @"Data Source = " + baseDados + "; Version = 3";
            dirPath = baseDados.Replace(@"\dbsqlite.db", "");
            if (sqlite == null)
            {
                VerifyFiles(dirPath, baseDados);
                OpenConnection();
                LoadList();
            }
            
        }

        private void Conectar_Click(object sender, RoutedEventArgs e)
        {
            OpenConnection();
        }

        private void OpenConnection()
        {
           
            VerifyFiles(dirPath, baseDados);
            SQLiteConnection connection = new SQLiteConnection(strConection);
            try
            {
                connection.Open();
                if (sqlite == null)
                    sqlite = connection;
                Logador.Text = "Conexão estabelecida.";
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }
        }

        private void VerifyFiles(string directoryPath, string databaseFilepath)
        {
            if (!Directory.Exists(databaseFilepath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(databaseFilepath))
            {
                SQLiteConnection.CreateFile(databaseFilepath);
            }
        }

        private void Finalizar_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");
                sqlite.Close();
                sqlite = null;
                Logador.Text = "Conexão finalizada com sucesso."; 
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }
        }

        private void Criar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");
                SQLiteCommand cmd = new();
                cmd.Connection = sqlite;
                cmd.CommandText = "CREATE TABLE contatos (id INTEGER PRIMARY KEY AUTOINCREMENT, nome VARCHAR(50), fone VARCHAR(15))";
                cmd.ExecuteNonQuery();

                Logador.Text = "Tabela criada.";
                cmd.Dispose();
            }catch(Exception ex)
            {
                Logador.Text = ex.Message;
            }

        }  

        private void Inserir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");

                if (InputNome.Text == "" || InputNumero.Text == "")
                    throw new Exception("Preencha os dois campos.");
                Contato contato = new()
                {
                    Id = new Random(DateTime.Now.Millisecond).Next(0, 1000).ToString(),
                    Name = InputNome.Text,
                    Phone = InputNumero.Text
                };
                SQLiteCommand cmd = new();
                cmd.Connection = sqlite;
                //campos <id, nome, fone>
                cmd.CommandText = "INSERT INTO contatos (nome, fone) VALUES(@nome, @fone)";
                //cmd.Parameters.AddWithValue("@id", contato.Id);
                cmd.Parameters.AddWithValue("@nome", contato.Name);
                cmd.Parameters.AddWithValue("@fone", contato.Phone);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                Logador.Text = "Contato inserido.";
                ClearFields();
                LoadList();
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }

        }

        private void Procurar_Click(object sender, RoutedEventArgs e)
        {
            LoadList();
        }

        private void LoadList()
        {
            try
            {
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");

                //campos <id, nome, fone>
                string query = "SELECT * FROM contatos";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, sqlite);
                DataTable dados = new DataTable();
                adapter.Fill(dados);

                ListaContatos.ItemsSource = null;
                List<Contato> listContatos2 = new();

                foreach (DataRow dr in dados.Rows)
                {
                    listContatos2.Add(new Contato() { Id = dr["id"].ToString(), Name = dr["nome"].ToString(), Phone = dr["fone"].ToString() });
                }

                ListaContatos.ItemsSource = listContatos2;
                Logador.Text = "Contatos carregados com sucesso.";
                adapter.Dispose();
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Contato? contatoToDelete = ListaContatos.SelectedItem as Contato;
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");
                if (contatoToDelete == null)
                    throw new Exception("Selecione uma linha.");


                SQLiteCommand cmd = new();
                cmd.Connection = sqlite;
                //campos <id, nome, fone>
                cmd.CommandText = "DELETE FROM contatos WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", contatoToDelete.Id);
               
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                ClearFields();
                LoadList();
                Logador.Text = "Contato deletado.";
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }

        } 
        
        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Contato? contatoToUpdate = ListaContatos.SelectedItem as Contato;
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");
                if (InputNome.Text == "" || InputNumero.Text == "")
                    throw new Exception("Preencha os dois campos.");
                if (contatoToUpdate == null)
                    throw new Exception("Selecione uma linha.");

                SQLiteCommand cmd = new();
                cmd.Connection = sqlite;
                //campos <id, nome, fone>
                cmd.CommandText = "UPDATE contatos SET nome = @nome, fone = @fone WHERE id LIKE @id";
                cmd.Parameters.AddWithValue("@id", contatoToUpdate.Id);
                cmd.Parameters.AddWithValue("@nome", InputNome.Text);
                cmd.Parameters.AddWithValue("@fone", InputNumero.Text);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                Logador.Text = "Contato editado.";
                LoadList();
                ClearFields();
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }
        }

        private void ListaContatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Contato? contatoToEdit = ListaContatos.SelectedItem as Contato;
                if (sqlite == null)
                    throw new Exception("Abra a conexão com a base de dados.");
                if (contatoToEdit == null)
                    throw new Exception("Selecione uma linha.");

                InputNome.Text = contatoToEdit.Name;
                InputNumero.Text = contatoToEdit.Phone;
            }
            catch (Exception ex)
            {
                Logador.Text = ex.Message;
            }
        }

        private void ClearFields()
        {
            InputNome.Text = "";
            InputNumero.Text = "";
        }

        private void ListaContatos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ListaContatos.UnselectAllCells();
                ClearFields();
            }
        }
    }
}

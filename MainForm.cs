using System;
using System.Windows.Forms;
using System.Data.OleDb;
using SystemAuthorisation.Errors;

namespace SystemAuthorization
{
    public partial class MainForm : Form
    {
        private string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=users.mdb";
        public MainForm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text;
            try
            {
                AddUser(login, password);
                ShowInfo("Пользователь добавлен.");
            }
            catch (ArgumentNullException ex)
            {
                ShowError(ex.ParamName);
            }
            catch (UserException ex)
            {
                ShowError(ex.ParamName);
            }
            catch (Exception)
            {
                ShowError("Неизвестная ошибка.");
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text;            
            try
            {
                if (CheckUser(login, password))
                {
                    ShowInfo("Вы успешно авторизованы.");
                }
                else
                {
                    ShowError("Неправильный логин или пароль.");
                }
            }
            catch (ArgumentNullException ex)
            {
                ShowError(ex.ParamName);
            }
            catch (UserException ex)
            {
                ShowError(ex.ParamName);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool CheckUser(string login, string password)
        {
            using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                if (string.IsNullOrEmpty(login))
                {
                    throw new ArgumentNullException("Логин не может быть пустым.", nameof(login));
                }
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("Пароль не может быть пустым.", nameof(password));
                }

                using (OleDbCommand command = new OleDbCommand())
                {
                    string commandText = $"SELECT Логин, Пароль " +
                                         $"FROM logins " +
                                         $"WHERE Логин = \'{login}\'";
                    command.Connection = oleDbConnection;
                    command.CommandText = commandText;
                    OleDbDataReader reader = command.ExecuteReader();
                    string realLogin = null;
                    string realPassword = null;
                    if(reader.Read())
                    {
                        realLogin = reader.GetString(0);
                        realPassword = reader.GetString(1);
                        if (login == realLogin && password == realPassword)
                        {
                            return true;
                        }
                    }                    
                }
                return false;
            }
        }

        private void AddUser(string login, string password)
        {
            using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                if (string.IsNullOrEmpty(login))
                {
                    throw new ArgumentNullException("Логин не может быть пустым.", nameof(login));                    
                }
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("Пароль не может быть пустым.", nameof(password));
                }

                bool isUsed = false;
                using (OleDbCommand command = new OleDbCommand())
                {
                    string commandText = $"SELECT COUNT(*) " +
                                         $"FROM logins " +
                                         $"WHERE Логин = \'{login}\'";
                    command.Connection = oleDbConnection;
                    command.CommandText = commandText;
                    int count = (int)command.ExecuteScalar();
                    isUsed = (count != 0) ? true : false;
                }
                if (isUsed)
                {
                    throw new UserException("Такой логин уже занят.", nameof(password));
                }



                int id;                
                using (OleDbCommand command = new OleDbCommand())
                {
                    string commandText = "SELECT COUNT(*) " +
                                         "FROM logins";
                    command.Connection = oleDbConnection;
                    command.CommandText = commandText;
                    id = (int)command.ExecuteScalar() + 1;
                }

                using (OleDbCommand command = new OleDbCommand())
                {
                    string commandText = $"INSERT INTO logins " +
                                         $"VALUES " +
                                         $"({id}, \"{login}\", \"{password}\")";
                    command.Connection = oleDbConnection;
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

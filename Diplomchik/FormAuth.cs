using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Diplomchik
{
    public partial class FormAuth : Form
    {
        public FormAuth()
        {
            InitializeComponent();
            LoadUsersIntoDataGridView();
            guna2TextBox2.UseSystemPasswordChar = statusPasswordChar;
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string login = System.Text.RegularExpressions.Regex.Replace(guna2TextBox1.Text, @"\s +", " ").Trim();
            string password = System.Text.RegularExpressions.Regex.Replace(guna2TextBox2.Text, @"\s +", " ").Trim();
            ClassStorage.login = login;
            ClassStorage.password = password;

            var dbService = new DatabaseService(ClassStorage.supabaseUrl, ClassStorage.supabaseKey);
            await dbService.AuthenticateUserAsync(login, password);
            if (ClassStorage.authorizated)
            {
                this.Hide();
                FormMenu form = new FormMenu();
                Closed += (s, args) => this.Close();
                form.Show();
            }
            else
            {

            }
        }
        private bool statusPasswordChar = true;
        private void button4_Click(object sender, EventArgs e)
        {
            statusPasswordChar = !statusPasswordChar;
            guna2TextBox2.UseSystemPasswordChar = statusPasswordChar;
            if (statusPasswordChar == true)
            {

                button4.BackgroundImage = Properties.Resources.icons8_eye_100;
            }
            if (statusPasswordChar == false)
            {

                button4.BackgroundImage = Properties.Resources.icons8_closed_eye_100;
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormReg form = new FormReg();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private async void LoadUsersIntoDataGridView()
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);

            // Инициализация клиента Supabase
            await supabase.InitializeAsync();

            // Получаем все записи из таблицы users
            var result = await supabase
                .From<users>()
                .Select("*")
                .Get();

            if (result.Models.Any())
            {
                // Преобразуем список пользователей в DataTable для привязки к DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("User id", typeof(int));
                dataTable.Columns.Add("Created at", typeof(string));
                dataTable.Columns.Add("Login", typeof(string));
                dataTable.Columns.Add("Password", typeof(string));
                dataTable.Columns.Add("Role", typeof(string));

                foreach (var user in result.Models)
                {
                    dataTable.Rows.Add(user.user_id, user.created_at, user.login, user.password, user.role);
                }

                // Привязываем DataTable к DataGridView
                guna2DataGridView1.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Нет пользователей для отображения.");
            }
        }

        private void guna2ButtonChangePassword_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChangePassword form = new ChangePassword();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        
    }
}

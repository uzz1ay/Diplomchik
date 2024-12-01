using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Supabase.Postgrest.Constants;

namespace Diplomchik
{
    public partial class FormReg : Form
    {
        public FormReg()
        {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }
        private async void LoadUserData()
        {
            string login = System.Text.RegularExpressions.Regex.Replace(guna2TextBox1.Text, @"\s +", " ").Trim();
            string password = System.Text.RegularExpressions.Regex.Replace(guna2TextBox2.Text, @"\s +", " ").Trim();
            ClassStorage.login = login;
            ClassStorage.password = password;
            if (login.ToLower().Contains("admin"))
            {
                ClassStorage.role = "admin";
            }
            else
            {
                ClassStorage.role = "user";
            }
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            if (password.Length <= 6 || !Regex.IsMatch(password, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
            {
                MessageBox.Show("Рекомендуется использовать пароль длинной более 6 символов,сдержащий буквы и числа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (password.Length >= 6 && Regex.IsMatch(password, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
            {
                var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);

                // Инициализация клиента Supabase
                await supabase.InitializeAsync();

                // Получаем все записи из таблицы users
                // Проверка существования пользователя с данным логином
                var existingUserResult = await supabase
                    .From<users>()
                    .Select("login")
                    .Filter("login", Operator.Equals, ClassStorage.login) // Используем Filter вместо Eq
                    .Get();

                if (existingUserResult.Models.Any())
                {
                    // Если пользователь с таким логином уже существует
                    MessageBox.Show("Пользователь с таким логином уже существует. Пожалуйста, выберите другой логин.");
                }
                else
                {
                    // Если пользователь не найден, добавляем нового пользователя
                    var newUser = new users
                    {
                        login = ClassStorage.login,
                        password = ClassStorage.password,
                        created_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Заполняем текущей датой и временем в нужном формате
                        role = ClassStorage.role
                    };

                    // Добавление нового пользователя в таблицу
                    var insertResult = await supabase
                        .From<users>()
                        .Insert(newUser);

                    if (insertResult.Models.Any())
                    {
                        var user = insertResult.Models.First(); // Получаем добавленного пользователя
                        MessageBox.Show($"Новый пользователь успешно добавле    н. Login: {user.login}, Password: {user.password}");
                        this.Hide();
                        FormAuth form = new FormAuth();
                        Closed += (s, args) => this.Close();
                        form.Show();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении нового пользователя.");
                    }
            
                } 
            }
            


        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormAuth form = new FormAuth();
            Closed += (s, args) => this.Close();
            form.Show();
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using static Supabase.Postgrest.Constants;

namespace Diplomchik
{
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            if (ClassStorage.login == "")
            {
                ClassStorage.login = guna2TextBox4.Text;
            }
            else
            {
                if (guna2TextBox1.Text == ClassStorage.password)
                {
                    if (System.Text.RegularExpressions.Regex.Replace(guna2TextBox3.Text, @"\s +", " ").Trim() == System.Text.RegularExpressions.Regex.Replace(guna2TextBox2.Text, @"\s +", " ").Trim())
                    {
                        var options = new Supabase.SupabaseOptions
                        {
                            AutoConnectRealtime = true
                        };
                        // Получаем новый пароль из текстового поля
                        string newPassword = guna2TextBox3.Text;

                        // Инициализация клиента Supabase
                        var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);
                        await supabase.InitializeAsync();

                        // Получаем пользователя по логину
                        var userResult = await supabase
                            .From<users>()
                            .Select("*")
                            .Filter("login", Operator.Equals, ClassStorage.login)
                            .Get();

                        if (userResult.Models.Any())
                        {
                            // Если пользователь найден, обновляем его пароль
                            var user = userResult.Models.First();
                            user.password = newPassword;

                            // Обновляем пользователя в базе данных
                            var updateResult = await supabase
                                .From<users>()
                                .Filter("login", Operator.Equals, ClassStorage.login)
                                .Update(user);

                            if (updateResult.Models.Any())
                            {
                                MessageBox.Show("Пароль успешно изменен.");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при изменении пароля.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь с таким логином не найден.");
                        }

                        this.Hide();
                        FormAuth form = new FormAuth();
                        Closed += (s, args) => this.Close();
                        form.Show();
                    }
                    else
                    {
                        MessageBox.Show("Новый пароль не совпадает.");
                    }


                }
                else
                {
                    MessageBox.Show("Введенный пароль не соответствует действительному.");
                }
            }
            
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (ClassStorage.authorizated == true)
            {
                this.Hide();
                FormMenu form = new FormMenu();
                Closed += (s, args) => this.Close();
                form.Show();
            }
            else
            {
                this.Hide();
                FormAuth form = new FormAuth();
                Closed += (s, args) => this.Close();
                form.Show();
            }

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

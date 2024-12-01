using Supabase.Gotrue;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Supabase.Postgrest.Constants;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using Diplomchik;


public class DatabaseService
{
    private readonly Supabase.Client _supabase;

    public DatabaseService(string supabaseUrl, string supabaseKey)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };
        _supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
    }

    

    public async Task<User> AuthenticateUserAsync(string login, string password)
    {
        await _supabase.InitializeAsync();
        var result = await _supabase
            .From<users>()
            .Select("*")
            .Filter("login", Operator.Equals, login)
            .Filter("password", Operator.Equals, password)
            .Get();

        if (result.Models.Any())
        {
            var user = result.Models.First();
            MessageBox.Show($"Авторизовано логин: {user.login} пароль: {user.password}");
            ClassStorage.authorizated = true;
            ClassStorage.role = user.role;
        }
        else
        {
            MessageBox.Show("Пользователь не найден или неверный пароль.");
        }

        return null; // Если пользователь не найден
    }



    

    // Другие методы для работы с БД могут быть добавлены здесь

}


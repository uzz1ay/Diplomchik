using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
[Table("users")]
class users : BaseModel
{
    [PrimaryKey("user_id")]
    public int user_id { get; set; }

    [Column("created_at")]
    public string created_at { get; set; }

    [Column("login")]
    public string login { get; set; }

    [Column("password")]
    public string password { get; set; }

    [Column("role")]
    public string role { get; set; }

}
[Table("report")]
class Prediction : BaseModel
{
    [PrimaryKey("id_report")]
    public int IdReport { get; set; }

    [Column("NGDU")]
    public string NGDU { get; set; }

    [Column("Ceh")]
    public string Ceh { get; set; }

    [Column("Skvazh")]
    public string Skvazh { get; set; }

    [Column("Predicted")]
    public float Predicted { get; set; }

    [Column("Actual")]
    public float Actual { get; set; }

    [Column("Error")]
    public float Error { get; set; }
}
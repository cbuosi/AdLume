using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AdLumeDash.Models.Api;

[Table("tMidia")]
public class Midia
{
    [Key] public int cMidia { get; set; } // PK auto increment
    public string? DescMidia { get; set; }
    public string? NomeMidia { get; set; }
    public string? HashMidia { get; set; }
    public string? UrlMidia { get; set; }
    public int cAtivo { get; set; } // PK auto increment


    internal void GeraRegistro(string strArq)
    {
        //



        this.cMidia = 0;
        this.NomeMidia = Path.GetFileName(strArq);
        this.HashMidia = GetFileMd5(strArq);
        this.UrlMidia = "{0}://{1}:{2}/media/" + this.NomeMidia;
        this.cAtivo = 1; //sim

        string DescMidia = this.NomeMidia.Replace("_", " ").Replace(".mp4", "").ToUpper();

        this.DescMidia = DescMidia;
        //
    }

    internal string GetFileMd5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);

        var hashBytes = md5.ComputeHash(stream);

        // converter para string hex
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }

    internal void Inserir()
    {
        var connectionString = "Server=192.168.18.187;Database=dbAdLume;User Id=sa;Password=123@qweasd;TrustServerCertificate=True;";
        using var conn = new SqlConnection(connectionString);
        var id = conn.Insert(this);

    }
}


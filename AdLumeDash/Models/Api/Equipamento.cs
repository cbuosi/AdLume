using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;

namespace AdLumeDash.Models.Api
{

    [Table("tEquipamento")]
    public class Equipamento
    {

        private readonly SqlConnection _conn;


        [Key] public int cEquipamento { get; set; }
        public string? GuidEquipamento { get; set; }
        public string? NomeEquipamento { get; set; }
        public string? DescEquipamento { get; set; }
        public DateTime DtUltAtu { get; set; }

        public Equipamento(SqlConnection conn)
        {
            _conn = conn;
        }

        internal Equipamento Obter(Guid deviceId)
        {
            throw new NotImplementedException();
        }
    }
}

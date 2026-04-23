using AdLumeDash.Interface;
using AdLumeDash.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdLumeDash.Repository;

public class EquipamentoRepository : IEquipamentoRepository
{
    private readonly string _connectionString;

    public EquipamentoRepository(IConfiguration config)
    {
        //_connectionString = config.GetConnectionString("Default");
        _connectionString = config.GetConnectionString("Default") ?? throw new Exception("ConnectionString não encontrada");

    }

    public async Task<IEnumerable<EquipamentoPlaylistDto>?> GetByGuid(string guid)
    {

        SqlConnection conn;
        IEnumerable<EquipamentoPlaylistDto>? result;

        try
        {
            conn = new SqlConnection(_connectionString);

            result = await conn.QueryAsync<EquipamentoPlaylistDto>("pEquipamentoPlaylistDto",
                                                                    new { @GuidEquipamento = guid },
                                                                    commandType: CommandType.StoredProcedure);

            return result;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

    }
}
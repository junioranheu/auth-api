using MySqlConnector;
using System.Data.SqlClient;

namespace Auth.Infrastructure.Factory
{
    public interface IConnectionFactory
    {
        MySqlConnection ObterMySqlConnection();
        SqlConnection ObterSqlServerConnection();
        string ObterStringConnection();
    }
}
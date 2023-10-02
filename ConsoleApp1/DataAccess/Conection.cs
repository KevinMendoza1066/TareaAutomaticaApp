using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace TareaAutomaticaGasFinder.DataAccess
{
    public  class Conection
    {
        private string _connectionString;
        private IDbConnection connection;

        public Conection(string connectionString)
        {
            this._connectionString = connectionString;
            connection = new SqlConnection(connectionString);
        }
        public void OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al abrir la conexión: " + ex.Message);
            }
        }
        public void CloseConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cerrar  la conexión: " + ex.Message);
            }
        }
        public IEnumerable<T> Select<T>(string query, object? _params = null)
        {
            try
            {
                OpenConnection();
                return connection.Query<T>(query, _params);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la consulta: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Update(string query, object? _params = null)
        {
            try
            {
                OpenConnection();
                connection.Execute(query, _params, null, null, CommandType.StoredProcedure) ;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la consulta: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

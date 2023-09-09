using System.Data;
using System.Data.SqlClient;
using ConsoleApp1.Models;
using Dapper;

namespace ConsoleApp1.DataAccess
{
    public class Operations
    {
        private string _connectionString = "TuCadenaDeConexion";
        private Conection connectionBd;

        public  Operations()
        {
            connectionBd = new Conection(_connectionString);

        }
        public void UpdateGasStations(GasStationModel GasStation)
        {
         
                try
                {
                    connectionBd.OpenConnection();

                    // Define la consulta SQL de actualización
                    string query  = "ActualizarGasolinera";

        
                    connectionBd.Update(query, GasStation);
                     
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connectionBd.CloseConnection();
                }
            

        }
    }
}

using System.Data;
using System.Data.SqlClient;
using TareaAutomaticaGasFinder.Models;
using Dapper;

namespace TareaAutomaticaGasFinder.DataAccess
{
    public class Operations
    {
        private string _connectionString = "workstation id=ProyectoEtps34.mssql.somee.com;packet size=4096;user id=S15t3M4dm1n_SQLLogin_1;pwd=ls2f72efvq;data source=ProyectoEtps34.mssql.somee.com;persist security info=False;initial catalog=ProyectoEtps34";
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
        public void UpdatePrices(PricesModel Prices)
        {

            try
            {
                connectionBd.OpenConnection();

                // Define la consulta SQL de actualización
                string query = "ActualizarPrecioActual";


                connectionBd.Update(query, Prices);

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

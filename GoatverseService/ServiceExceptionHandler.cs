using System;
using System.Data.SqlClient;
using System.ServiceModel;

namespace GoatverseService {
    public static class ServiceExceptionHandler {
        public static void HandleServiceException(Exception ex) {
            if (ex is ArgumentNullException) {
                Console.WriteLine("Uno de los argumentos proporcionados es nulo o inválido.");
            } else if (ex is InvalidOperationException) {
                Console.WriteLine("La operación solicitada no es válida en el estado actual.");
            } else if (ex is TimeoutException) {
                Console.WriteLine("La operación tomó demasiado tiempo y se agotó el tiempo de espera.");
            } else if (ex is SqlException) {
                Console.WriteLine("Ocurrió un error al acceder a la base de datos.");
            } else {
                Console.WriteLine("Ocurrió un error inesperado en el servicio.");
            }
        }
    }
}
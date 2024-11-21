using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseServer {
    internal static class Program {
        static void Main(string[] args) {
            using (ServiceHost host = new ServiceHost(typeof(GoatverseService.ServiceImplementation))) {
                host.Open();
                Console.WriteLine("Servidor iniciado");
                Console.ReadLine();
            }
        }
    }
}

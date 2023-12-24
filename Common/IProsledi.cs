using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IProsledi
    {
        [OperationContract]
        void Prosledi(string EntitetzaIzmenu, string korisnik);
        [OperationContract]
        Entitet LBToWorker(Entitet e, int Mod_Del);
        [OperationContract]
        Entitet WorkerToLB(Entitet e);
        [OperationContract]
        void LBtoServer(string Podaci);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IEntitet
    {
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        byte[] Read(byte[] id, string Korisnik, byte[] sign);
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Supervise(string id, string Korisnik, byte[] sign);
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Modify(string id, string Korisnik, byte[] sign);
        [OperationContract]
        void TestConnection(string Korisnik);
        [OperationContract]
        byte[] DataFromServerToCLient();
        [OperationContract]
        void DataFromServerToCLientDecripted();

    }
}

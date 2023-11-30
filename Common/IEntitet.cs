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
        void Read(byte[] sdf);
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Supervise();
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Modify();
        [OperationContract]
        void TestConnection();
    }
}

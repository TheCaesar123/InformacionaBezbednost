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
        void Read();
        [OperationContract]
        void Supervise();
        [OperationContract]
        void Modify();
    }
}

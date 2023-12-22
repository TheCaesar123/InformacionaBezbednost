﻿using System;
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
        void Prosledi(string id, string korisnik);
        [OperationContract]
        Entitet LBToWorker(Entitet e);
        [OperationContract]
        Entitet WorkerToLB(Entitet e);
    }
}

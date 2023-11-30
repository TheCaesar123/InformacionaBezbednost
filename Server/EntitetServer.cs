using Client;
using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class EntitetServer : IEntitet
    {
        public void Modify()
        {
         
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, CitanjeUlogovanogClienta()).Contains("OU=modify"))
                Console.WriteLine("MODIFY");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{CitanjeUlogovanogClienta().ToUpper()}] tried to call Modify method (time: { time.TimeOfDay}). " +
                    "For this method user needs to be member of group Modify.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
        
        public void Read()
        {
           
            
          
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, CitanjeUlogovanogClienta()).Contains("OU=read"))
                Console.WriteLine("READ");
            else
            {
                string name = Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{CitanjeUlogovanogClienta().ToUpper()}] tried to call Read method (time: {time.TimeOfDay}). " +
                    "For this method user needs to be member of group Read.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

        public void Supervise()
        {
            //Console.WriteLine("SUPERVISE");
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, CitanjeUlogovanogClienta()).Contains("OU=supervise"))
                Console.WriteLine("SUPERVISE");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{CitanjeUlogovanogClienta().ToUpper()}] tried to call Supervise method (time: {time.TimeOfDay}). " +
                    "For this method user needs to be member of group Supervise.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
        public string CitanjeUlogovanogClienta()
        {
            string client = File.ReadAllText("C:/Bezbednost/Client/bin/Debug/cert.txt"); // uneti odgovarajucu putanju
            Console.WriteLine(client);
            return client;
        }
    }
}

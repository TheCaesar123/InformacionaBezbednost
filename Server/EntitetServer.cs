using Common;
using System;
using System.Collections.Generic;
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
            //Console.WriteLine("MODIFY"); 
            if (Thread.CurrentPrincipal.IsInRole("Modify"))
                Console.WriteLine("MODIFY");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("Access is denied. User {0} tried to call Modify method (time: {1}). " +
                    "For this method user needs to be member of group Modify.", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

        public void Read()
        {
            Console.WriteLine(Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name));
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name)).Contains("read"))
                Console.WriteLine("READ");
            else
            {
                string name = Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
                DateTime time = DateTime.Now;
                string message = String.Format("Access is denied. User {0} tried to call Read method (time: {1}). " +
                    "For this method user needs to be member of group Read.", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

        public void Supervise()
        {
            //Console.WriteLine("SUPERVISE");
            if (Thread.CurrentPrincipal.IsInRole("Supervise"))
                Console.WriteLine("SUPERVISE");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("Access is denied. User {0} tried to call Supervise method (time: {1}). " +
                    "For this method user needs to be member of group Supervise.", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
    }
}

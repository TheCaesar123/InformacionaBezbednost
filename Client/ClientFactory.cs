using Common;
using Manager;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace Client
{
    public class ClientFactory : ChannelFactory<IEntitet>, IEntitet, IDisposable
    {
        IEntitet factory;

        public ClientFactory(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

        public void Read()
        {
            try
            {
                factory.Read();
                Console.WriteLine("Read...");

            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Read: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }

        }

        public void Supervise()
        {

            try
            {
                factory.Supervise();
                Console.WriteLine("Supervise...");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Supervise: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }
        }

        public void Modify()
        {
            try
            {
                factory.Modify();
                Console.WriteLine("Modify...");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Modify: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }
        }
    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IEntitet> servis = new ChannelFactory<IEntitet>("Servis");

            IEntitet kanal = servis.CreateChannel();

            Console.WriteLine("KLIJENT POVEZAN NA SERVER");

            kanal.Read();
            kanal.Modify();
            kanal.Supervise();
            Console.ReadKey();
        }
    }
}

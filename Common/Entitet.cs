using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Entitet
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SID { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
        public Entitet()
        {
            
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += Id.ToString() + "|" + Name + "|" + SID + "|" + Time.ToString() + ";";
            return retVal;
        }
    }
}

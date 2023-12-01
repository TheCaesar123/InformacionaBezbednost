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
        int id;
        

        public Entitet(int id)
        {
            this.id = id;
        }
        [DataMember]
        public int Id { get => id; set => id = value; }
        public override string ToString()
        {
            string retVal;
            retVal = "" + id;
            return retVal;
        }
    }
}

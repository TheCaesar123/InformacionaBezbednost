using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class worker
    {
        public int Id { get; set; } = 1;
        public int CostFactor { get; set; }

        public worker(int costFactor)
        {
           
            Id++;
            CostFactor = costFactor;
        }
    }
}

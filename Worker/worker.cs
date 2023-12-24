using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class worker
    {
        public int Id { get; set; }
        public static int IdWorkera = 0;
        public int CostFactor { get; set; }

        public worker(int costFactor)
        {
            if (IdWorkera == 4)
            {
                IdWorkera = 0;
            }
            IdWorkera++;
            Id = IdWorkera;
            CostFactor = costFactor;
        }
    }
}

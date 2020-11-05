using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class TempRoom : IRoom
    {
        public int capacity { get; set; }
        public string classification { get; set; }

        public TempRoom()
        {
         
        }
    }
}

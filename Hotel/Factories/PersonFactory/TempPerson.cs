using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    /// <summary>
    /// class to store temperary persons 
    /// so you can assign the correct person class later.
    /// </summary>
    public class TempPerson:IPerson
    {

        public string roomPref { get; set; }
        public bool busy { get; set; }
        

        public TempPerson()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Customer:IPerson
    {
        
        public string roomPref { get; set; }//where roompref is stored
       
        /// <summary>
        /// copy the tempPerson and create a customer
        /// </summary>
        /// <param name="tempPerson">The tempPerson which becomes a Customer</param>
        public Customer(TempPerson tempPerson)
        {
            this.id = tempPerson.id;
            this.roomPref = tempPerson.roomPref;
            this.position = new Point(1, 0);
            modelPerson = Image.FromFile(@"..\..\Assets\Customer.png");//the model of the Customer
            modelPerson.RotateFlip(RotateFlipType.Rotate180FlipX);//flip the model so it is displayed correctly
        }

    }
      
            

        
       
}

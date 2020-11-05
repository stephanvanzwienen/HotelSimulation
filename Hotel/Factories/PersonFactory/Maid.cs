using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Maid: IPerson
    {
        public bool busy { get; set; } //store if the maid is busy with something
        /// <summary>
        /// copy the tempPerson and create a maid
        /// </summary>
        /// <param name="tempPerson">the tempperson which becomes a maid</param>
        public Maid(TempPerson tempPerson)
        {
            this.id = tempPerson.id;
            this.busy = tempPerson.busy;
            this.position = new Point(1, 0);
            modelPerson = Image.FromFile(@"..\..\Assets\StephansDreamMaid.png");//the model
            modelPerson.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate so its displayed correctly
        }
    }
}

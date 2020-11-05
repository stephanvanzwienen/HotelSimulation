using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{

    public class Elevator : IRoom
    {
        public List<IPerson> people = new List<IPerson>(); //list of people currently in the elvator
        public Point prevPosition = new Point(0, 0); //previous postion of the elevator
        public bool running = false; //keep track if elevator is moving 
        public int elevatorTimer = 0; 
        public int elevatorPrevTimer = 0;
        /// <summary>
        /// create a elevator
        /// </summary>
        public Elevator()
        {
            areaType = "Elevator";
            model = Image.FromFile(@"..\..\Assets\Elevator.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
        }



    }
}

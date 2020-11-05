using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    class ElevatorShaft : IRoom
    {
        /// <summary>
        /// create a elevatorshaft
        /// </summary>
        public ElevatorShaft()
        {
            areaType = "ElevatorShaft";
            model = Image.FromFile(@"..\..\Assets\ElevatorShaft.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
        }
    }
}

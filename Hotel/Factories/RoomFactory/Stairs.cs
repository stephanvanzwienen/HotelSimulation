using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    class Stairs : IRoom
    {
        /// <summary>
        /// create stairs
        /// </summary>
        public Stairs()
        {
            areaType = "Stairs";
         
            model = Image.FromFile(@"..\..\Assets\Stairs.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Cinema : IRoom
    {
        /// <summary>
        /// copy temproom to create a cinema
        /// </summary>
        /// <param name="temp">the temproom which becomes a cinema</param>
        public Cinema(TempRoom temp)
        {
            areaType = temp.areaType;
            position = temp.position;
            dimension = temp.dimension;
            model = Image.FromFile(@"..\..\Assets\Cinema.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
            id = temp.id;
            
        }
       
    }
}

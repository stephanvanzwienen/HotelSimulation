using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Restaurant : IRoom
    {
        public int capacity { get; set; }
        /// <summary>
        /// copy temproom to create restaurant
        /// </summary>
        /// <param name="temp">temprroom which becomes a restaurant</param>
        public Restaurant(TempRoom temp)
        {
            areaType = temp.areaType;
            position = temp.position;
            dimension = temp.dimension;
            capacity = temp.capacity;
            model = Image.FromFile(@"..\..\Assets\Restaurant.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
            id = temp.id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Room : IRoom
    {
        public string classification { get; set; } //amount of stars a room has
        
        public bool taken { get; set; } = false;
        /// <summary>
        /// copy temproom to create a room
        /// </summary>
        /// <param name="temp">temproom which becomes a room</param>
        public Room(TempRoom temp)
        {
            areaType = temp.areaType;
            position = temp.position;
            dimension = temp.dimension;
            classification = temp.classification;
            model = Image.FromFile(@"..\..\Assets\Room.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
            id = temp.id;

        }
        
            
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    class Lobby : IRoom
    {
        /// <summary>
        /// create a lobby
        /// </summary>
        public Lobby()
        {
            areaType = "Lobby";
            position = new Point(1, 0); 
            
            model = Image.FromFile(@"..\..\Assets\Lobby.png");
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed




        }
    }
}

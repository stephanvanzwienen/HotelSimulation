using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{

    public abstract class IRoom
    {
        public string areaType { get; set; }
        public Point position { get; set; } 
        public Point dimension { get; set; }
        public Image model { get; set; }
        public int id { get; set; }
        public int distance { get; set; } //distance from an other room, its used for dijsktra and pathfinding
        public Dictionary<IRoom, int> neighbours { get; set; }
        public IRoom prev { get; set; }  //used for pathfinding
        public Customer customer { get; set; } //customer who has the room
        public bool dirtyRoom { get; set; }
        public bool roomIsGettingCleaned { get; set; }
        public bool danger { get; set; } = false; //used for cleaning emergency
        public IRoom()
        {
            distance = Int32.MaxValue / 2;
            prev = null;
            neighbours = new Dictionary<IRoom, int>();

        }
        /// <summary>
        /// Draw the room on the bitmap
        /// </summary>
        /// <param name="layout">bitmap to draw on</param>
        /// <returns>bitmap with the room drawn on it</returns>
        public Bitmap Draw(Bitmap layout, int size)
        {

            using (Graphics g = Graphics.FromImage(layout))
            {
                
                g.DrawImage(model, position.X * size, position.Y * size, dimension.X * size, dimension.Y * size);
            }
            return layout;
        }





    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Hotel
{
    class Draw
    {
        public int drawSizeRoom { get; } = 100;
       
        public Draw()
        {
            
            
        }

        /// <summary>
        /// Draw the layout of the hotel
        /// </summary>
        /// <param name="coordinates">layout of the hotel</param>
        /// <returns>bitmap with the layout</returns>
        public Bitmap DrawLayout (IRoom[,]coordinates)
        {
            //set width and height for bitmap
            int width = coordinates.GetLength(0) * drawSizeRoom;
            int height = coordinates.GetLength(1) * drawSizeRoom;

            Bitmap layout = new Bitmap(width, height);
            //foreach room in the layout, draw the room.
            foreach (IRoom room in coordinates)
            {
                if (room != null)
                {
                    room.Draw(layout,drawSizeRoom);
                }

            }

            //flip the bitmap so the hotel is displayed correctly
            layout.RotateFlip(RotateFlipType.Rotate180FlipX);
           
            return layout;
        }
        /// <summary>
        /// Draw the personLayout 
        /// </summary>
        /// <param name="personLayout">bitmap to draw on</param>
        /// <param name="people">persons to draw</param>
        /// <returns>personLaoyout with the drawn persons</returns>
        public Bitmap DrawPersonLayout(Bitmap personLayout, Dictionary<IPerson, IRoom> people)
        {
            //Foreach person in people
            //Draw it on the bitmap
            foreach (IPerson everyPerson in people.Keys)
            {
                personLayout = everyPerson.DrawPerson((Bitmap)personLayout, drawSizeRoom); 
            }
           
            //flip bitmap so it displays correctly
            personLayout.RotateFlip(RotateFlipType.Rotate180FlipX);

            return (Bitmap)personLayout;
        }
       
    }
}

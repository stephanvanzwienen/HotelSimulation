using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public abstract class IPerson
    {
        public Image modelPerson { get; set; } 
        public Point position { get; set; } //the current postion of the person within the 2d coordinates array.
        public Point dimension { get; set; }// the dimension of the person, its used for displaying the person
        public string id { get; set; } 
        private int personWidth = 60; //width of the person in pixels
        private int personHeight = 75;//height of the person in pixels
        public List<Point> route; //current route person is walking
        private int routeCounter = 0; //counter which keeps track where person is on its current route
        public LinkedList<List<IRoom>> eventQueue = new LinkedList<List<IRoom>>();// a queue containing routes that the person must walk in the future. 
        public int timeBusyEvent { get; set; } //keeps track of much time  a person spent so far in a event
        public bool eventStarted { get; set; } = false;
        public IRoom room { get; set; } //room of the person in the hotel
        public IRoom currentRoom { get; set; }//room where the person is atm
        public bool evac { get; set; } = false; //keeps track if person is evactuating.
        public bool checkedIn { get; set; } = false; 
        public IPerson()
        {
            route = new List<Point>();

        }

        /// <summary>
        /// Makes the animation possible by moving the person to next coordinate.
        /// </summary>
        public void WalkTo()
        {
            if (routeCounter <= route.Count - 1)
            {
            
                this.position = this.route[routeCounter]; //set the current postition to the current position of the route the person is walking
                routeCounter++;
                
            }

        }
        /// <summary>
        /// Saves the path the person is walking atm.
        /// </summary>
        /// <param name="path">The shortest path.</param>
        public void SavePath(List<IRoom> path)
        {
           
            route.Clear();//new path so clear the current one
            routeCounter = 0; //reset the counter
            int i = 0; //an other counter
            ///while the last position of the route not equals last path postion,
            /// keep adding new coordinates to the route
            while (route.LastOrDefault() != path.LastOrDefault().position)
            {
                IRoom current = path[i];//current room where we are in the path 
                IRoom next = null;//create variable to store next room in path later.
                try
                {

                    if (path[i + 1] != null)
                    {
                        next = path[i + 1];
                    }
                }
                catch
                {
                    next = path[0];
                }

                int counter = 0;
                if (next != null)
                {

                    if (current.position.Y == next.position.Y) //next postion is on the samefloor
                    {
                        if (next.position.X > current.position.X) //go right 
                        {
                            counter = 1;
                            while (route.LastOrDefault().X != next.position.X)
                            {

                                this.route.Add(new Point((current.position.X + counter), current.position.Y));
                                counter++;
                            }
                            counter = 0;
                        }
                        else if (next.position.X < current.position.X) //go left
                        {
                            counter = 1;
                            if (i == 0 && current.position.X == 1)
                            {

                                Point x = path.FirstOrDefault().position;
                                Point z = current.position;
                                Point y = next.position;
                                this.route.Add(new Point((current.position.X - counter), current.position.Y));

                            }
                            while (route.LastOrDefault().X != next.position.X)
                            {
                                this.route.Add(new Point((current.position.X - counter), current.position.Y));
                                counter++;
                            }
                        }
                        
                    }
                    else if (current.position.Y != next.position.Y) // go up or down if postion isnt on the same floor
                    {
                        this.route.Add(new Point(next.position.X, next.position.Y));
                    }
                    i++;
                }

            }


        }
        /// <summary>
        /// Draws the person inside of the room he is in.
        /// </summary>
        /// <param name="person">The sprite of the person that is about to be drawn.</param>
        /// <param name="sizeRoom">Need this int to make the character the size of the room so if you change it they change with the it.</param>
        /// <returns>The sprite of the right person in the right room.</returns>
        public Bitmap DrawPerson(Bitmap person, int sizeRoom)
        {

            if (this.id.Contains("Maid"))//draw maid
            {

                using (Graphics g = Graphics.FromImage(person))
                {

                    g.DrawImage(modelPerson, position.X * sizeRoom, position.Y * sizeRoom, personWidth, personHeight);
                }
            }
            else if (this.id.Contains("Gast"))//draw Customer
            {
                using (Graphics g = Graphics.FromImage(person))
                {
                    g.DrawImage(modelPerson, position.X * sizeRoom, position.Y * sizeRoom, personWidth, personHeight);
                }
            }


            return person;
        }
    }
}

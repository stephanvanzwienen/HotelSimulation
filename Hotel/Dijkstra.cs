using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{


    public class Dijkstra
    {
        List<IRoom> open;
        IRoom start;
        IPerson person;
        public bool evac = false; //bool to keep track if a evacuation is going on
        bool useElevator = false; // keep track if elevator is used
        public IRoom[,] coordinates { get; set; }
        public Dijkstra()
        {
            open = new List<IRoom>(); //list of nodes that keeps track of the nodes yet to be visited
        }

        /// <summary>
        /// Look for every room what the neighbours are 
        /// and store them
        /// </summary>
        /// <param name="coordinates">All the room coordinates.</param>
        public void CreateGraph(IRoom[,] coordinates)
        {
            this.coordinates = coordinates;

            foreach (IRoom room in coordinates)
            {

                if (room != null)
                {
                    room.neighbours.Clear();
                    int left = room.position.X - 1;
                    int right = room.position.X + 1;
                    int top = room.position.Y + 1;
                    int bottom = room.position.Y - 1;
                    while (left >= 0 && coordinates[left, room.position.Y] == null)
                    {
                        left--;
                    }
                    while (right < coordinates.GetLength(0) && coordinates[right, room.position.Y] == null)
                    {
                        right++;
                    }

                    if (room.areaType == "Elevator" || room.areaType == "Stairs" || room.areaType == "ElevatorShaft")
                    {
                        while (top < coordinates.GetLength(1) && coordinates[room.position.X, top] == null)
                        {
                            top++;
                        }
                        while (bottom >= 0 && coordinates[room.position.X, bottom] == null)
                        {
                            bottom++;
                        }
                        if (top < coordinates.GetLength(1) && coordinates[room.position.X, top] != null)
                        {
                            IRoom Top = coordinates[room.position.X, top];
                            room.neighbours.Add(Top, Top.dimension.Y);
                        }
                        if (bottom >= 0 && coordinates[room.position.X, bottom] != null)
                        {
                            IRoom Bottom = coordinates[room.position.X, bottom];
                            room.neighbours.Add(Bottom, Bottom.dimension.Y);
                        }

                    }
                    if (left >= 0 && coordinates[left, room.position.Y] != null)
                    {
                        IRoom Left = coordinates[left, room.position.Y];
                        room.neighbours.Add(Left, Left.dimension.X);
                    }
                    if (right < coordinates.GetLength(0) && coordinates[right, room.position.Y] != null)
                    {
                        IRoom Right = coordinates[right, room.position.Y];
                        room.neighbours.Add(Right, room.dimension.X);
                    }
                }
            }

        }

        /// <summary>
        /// Returns path to walk from a certain start point to his destination.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">Destination.</param>
        /// <returns>Path from start to end.</returns>
        public List<IRoom> Run(IRoom start, IRoom end, IPerson person)
        {
            this.person = person;
            this.start = start;
            open.Clear();
            ResetDistances(coordinates);
            start.distance = 0;
            IRoom current = start;
            while (!Visit(current, end))
            {
                current = open.Aggregate((l, r) => l.distance < r.distance ? l : r);

            }
            useElevator = false;
            return CreatePath(current);
        }

        /// <summary>
        /// Returns a path where there is not decided yet which destination is closer.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="areaType">Areatype used to look what destination it needs to head too.</param>
        /// <param name="debug">Used to check what person is gonna move.</param>
        /// <returns>Path from start to closest destination.</returns>
        public List<IRoom> RunUnkown(IRoom start, string areaType)
        {
            
            this.start = start;
            open.Clear();
            ResetDistances(coordinates);
            start.distance = 0;
            IRoom current = start;
            while (!VisitUnkown(current, areaType))
            {
                current = open.Aggregate((l, r) => l.distance < r.distance ? l : r);

            }
            return CreatePath(current);
        }
        /// <summary>
        /// the stored path is reversed so reverse it 
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<IRoom> CreatePath(IRoom end)
        {
            IRoom current = end;
            List<IRoom> reversedPath = new List<IRoom>
            {
                current //put the end node in the list
            };

            //while node has a previous node, continue
            while (current.prev != null)
            {
                current = current.prev; // go to previous node
                reversedPath.Add(current);//add the node to list
            }
            reversedPath.Reverse();//flip the list
            return reversedPath;
        }
        /// <summary>
        /// Shows what rooms person visit in their path to their destination.
        /// </summary>
        /// <param name="current">Room person is in atm.</param>
        /// <param name="end">Destination of the person.</param>
        /// <returns>True if the person visited this room.</returns>
        private bool Visit(IRoom current, IRoom end)
        {
            Console.WriteLine("Visit node " + current.areaType);

            //Check if it is end point
            if (current == end)
            {
                return true;
            }
            //Don't visit
            if (open.Contains(current))
            {
                open.Remove(current);
            }

            foreach (KeyValuePair<IRoom, int> x in current.neighbours)
            {

                if (x.Key.areaType == "Elevator" && useElevator == false && evac == false)
                {

                    Elevator elevator = (Elevator)x.Key;
                    if (elevator.position.Y == start.position.Y)
                    {
                        useElevator = true;
                        int NewDistance = current.distance + x.Value;
                        if (NewDistance < x.Key.distance)
                        {
                            x.Key.distance = NewDistance;//set new distance
                            x.Key.prev = current; //save Path
                            open.Add(x.Key); //Yet to visit;
                        }
                    }
                }
                else if (x.Key.areaType == "ElevatorShaft" && useElevator == true && evac == false)
                {
                    int NewDistance = current.distance + x.Value;
                    if (NewDistance < x.Key.distance)
                    {
                        x.Key.distance = NewDistance;//set new distance
                        x.Key.prev = current; //save Path
                        open.Add(x.Key); //Yet to visit;
                    }
                }
                else if (x.Key.areaType != "Elevator" && x.Key.areaType != "ElevatorShaft")
                {
                    int NewDistance = current.distance + x.Value;
                    if (NewDistance < x.Key.distance)
                    {
                        x.Key.distance = NewDistance;//set new distance
                        x.Key.prev = current; //save Path
                        open.Add(x.Key); //Yet to visit;
                    }
                }


            }
            return false;
        }
        /// <summary>
        /// Shows the path the person walks if the destination is not set yet.
        /// </summary>
        /// <param name="current">Current room person is in right now.</param>
        /// <param name="areaType">The areatype the person needs to go to.</param>
        /// <returns>Returns true if current room has been walked over.</returns>
        private bool VisitUnkown(IRoom current, string areaType)
        {
            Console.WriteLine("Visit node " + current.areaType);

            //Check if it is end point
            if (current.areaType == areaType)
            {
                return true;
            }
            //Don't visit
            if (open.Contains(current))
            {
                open.Remove(current);
            }

            foreach (KeyValuePair<IRoom, int> x in current.neighbours)
            {

                if (x.Key.areaType == "Elevator" && useElevator == false && evac == false)
                {

                    Elevator elevator = (Elevator)x.Key;
                    if (elevator.position.Y == start.position.Y)
                    {
                        useElevator = true;
                        int NewDistance = current.distance + x.Value;
                        if (NewDistance < x.Key.distance)
                        {
                            x.Key.distance = NewDistance;//set new distance
                            x.Key.prev = current; //save Path
                            open.Add(x.Key); //Yet to visit;
                        }
                    }
                }
                else if (x.Key.areaType == "ElevatorShaft" && useElevator == true && evac == false)
                {
                    int NewDistance = current.distance + x.Value;
                    if (NewDistance < x.Key.distance)
                    {
                        x.Key.distance = NewDistance;//set new distance
                        x.Key.prev = current; //save Path
                        open.Add(x.Key); //Yet to visit;
                    }
                }
                else if (x.Key.areaType != "Elevator" && x.Key.areaType != "ElevatorShaft")
                {
                    int NewDistance = current.distance + x.Value;
                    if (NewDistance < x.Key.distance)
                    {
                        x.Key.distance = NewDistance;//set new distance
                        x.Key.prev = current; //save Path
                        open.Add(x.Key); //Yet to visit;
                    }
                }


            }
            return false;
        }
        /// <summary>
        /// When you want to search a new path we have to reset all the distances.
        /// </summary>
        /// <param name="coordinates"></param>
        public void ResetDistances(IRoom[,] coordinates)
        {
            foreach (IRoom room in coordinates)
            {
                if (room != null)
                {
                    room.distance = Int32.MaxValue / 2;
                    room.prev = null;
                }
            }
            open.Clear();
        }
    }
}

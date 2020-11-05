using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System;

namespace Hotel
{
    public class RoomFactory : AbstractFactory
    {
        public List<TempRoom> tempRooms = new List<TempRoom>();
        public List<IRoom> rooms = new List<IRoom>();
        public Elevator elevator;
        public IRoom[,] coordinates { get; set; }

        public RoomFactory()
        {

        }

        /// <summary>
        /// read out the json layout file and save it in a temporary list with temporary rooms
        /// </summary>
        private void DeserializeLayout()
        {
            string json = File.ReadAllText(@"..\..\Hotel3.layout");
            tempRooms = JsonConvert.DeserializeObject<List<TempRoom>>(json);
        }

        /// <summary>
        /// Generate layout from layout file.
        /// </summary>
        public void GenerateEntity()
        {
            DeserializeLayout();

            //foreach temproom create a definite room
            //add definite room to the room list
            foreach (TempRoom temp in tempRooms)
            {

                if (temp.areaType != null)
                {
                    rooms.Add(GenerateRoom<IRoom>(temp.areaType, temp));
                }
            }
            //tempRooms.Clear();//empty temproom list cause we dont need it anymore.

            CreateOverview();

        }

        /// <summary>
        /// Generate a room
        /// </summary>
        /// <typeparam name="T">type of room</typeparam>
        /// <param name="room">areaType of the room</param>
        /// <param name="temp">temperary room for constructor</param>
        /// <returns>instance of the given type room</returns>
        public IRoom GenerateRoom<T>(string room, TempRoom temp) where T : IRoom
        {
            //get the type of the room
            Type type = Type.GetType(room);

            //if type is found return a instance of that type
            if (type != null)
            {
                return (IRoom)Activator.CreateInstance(type, temp);
            }
            //else if type is not found.
            //search the namespace/current assembly for all types that are available
            //make a list out of all the types and look if one of those types contain the name of the areatype
            //if so set that type and create an instance of it.
            //if not, return the temp room.
            else
            {
                string nspace = "Hotel";

                //query for all the types
                var q = from x in Assembly.GetExecutingAssembly().GetTypes()
                        where x.IsClass && x.Namespace == nspace
                        select x;

                List<string> types = new List<string>();
                //put the query in the list
                foreach (Type t in q)
                {
                    types.Add(t.ToString());
                }
                //search the list and if found return instance. 
                foreach (string t in types)
                {
                    if (t.Contains(room))
                    {
                        type = Type.GetType(t);
                        return (IRoom)Activator.CreateInstance(type, temp);
                    }
                }

            }
            return temp;
        }

        /// <summary>
        /// Overview of the layout
        /// </summary>
        /// <returns>2D array with rooms</returns>
        private IRoom[,] CreateOverview()
        {
            Point maxArray = new Point();
            maxArray.X = 0;
            maxArray.Y = 0;
            Point lobbyDimension = new Point();
            Point stairsDimension = new Point();
            Point elevatorDimension = new Point();

            //set the dimensions of the 2dArray
            foreach (IRoom room in rooms)
            {
                if (room.position.X > maxArray.X)
                {
                    maxArray.X = room.position.X;
                }
                if (room.position.Y > maxArray.Y)
                {
                    maxArray.Y = room.position.Y;
                }
            }

            //create 2dArray
            coordinates = new IRoom[maxArray.X + 2, maxArray.Y + 1];

            //add the rooms to the array
            foreach (IRoom room in rooms)
            {
                coordinates[room.position.X, room.position.Y] = room;
            }
            for (int i = 0; i < maxArray.X; i++)
            {
                if (coordinates[i, 0] == null)
                    lobbyDimension.X++;
                else
                    break;
            }



            //create the lobby and add to array
            Lobby lobby = new Lobby();
            lobbyDimension.Y = 1;
            lobby.dimension = new Point(lobbyDimension.X, 1);
            coordinates[1, 0] = lobby;

            for (int i = 0; i < maxArray.Y; i++)
            {
                if (coordinates[0, i] == null)
                {
                    elevatorDimension.Y++;
                    stairsDimension.Y++;
                }
                else
                    break;
            }

            elevator = new Elevator();
            elevator.dimension = new Point(1, 1);
            elevator.position = new Point(0, 0);
            elevator.prevPosition = new Point(0, 0);
            coordinates[0, 0] = elevator;
            //create elevator + shaft and add to array
            for (int i = 1; i < coordinates.GetLength(1); i++)
            {
                ElevatorShaft elevatorShaft = new ElevatorShaft();
                elevatorShaft.dimension = new Point(1, 1);
                elevatorShaft.position = new Point(0, i);
                coordinates[0, i] = elevatorShaft;
            }

            //create stairs and add to array
            for (int i = 0; i < coordinates.GetLength(1); i++)
            {
                Stairs stairs = new Stairs();
                stairs.dimension = new Point(1, 1);
                stairs.position = new Point(coordinates.GetLength(0) - 1, i);
                coordinates[coordinates.GetLength(0) - 1, i] = stairs;
            }

            return coordinates;
        }


    }
}

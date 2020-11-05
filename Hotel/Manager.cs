using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HotelEvents;

namespace Hotel
{
    class Manager : HotelEventListener
    {
        //Variables needed in the entire manager.
        public int amountOfMaids { get; set; }
        public Dictionary<IPerson, IRoom> people = new Dictionary<IPerson, IRoom>();
        public int timerCount { get; set; }
        List<string> paths = new List<string>();
        IRoom[,] coordinates { get; set; }
        public bool evacuation { get; set; } = false;
        public int evacCounter { get; set; }
        public bool hotelEmpty = false; //bool that tells if there are people in the hotel.

        //Connections needed in the entire manager.
        PersonFactory personFactory = new PersonFactory();
        MainForm main { get; set; }
        public Dijkstra dijkstra = new Dijkstra();




        public Manager(IRoom[,] coordinates, int amountOfMaids, MainForm main)
        {
            HotelEventManager.HTE_Factor = 1.5f;
            this.main = main;
            this.amountOfMaids = amountOfMaids;
            this.coordinates = coordinates;
            GenerateMaids();

            HotelEventManager.Register(this);
            dijkstra.CreateGraph(coordinates);

            TempPerson temp = new TempPerson();
            Customer customer = new Customer(temp);

        }
        /// <summary>
        /// Generates the amount of maids you want which you have decided before the simulation started.
        /// </summary>
        private void GenerateMaids()
        {
            TempPerson temp = new TempPerson();
            for (int i = 0; i < amountOfMaids; i++)
            {

                temp.id = "Maid " + (1 + i).ToString();
                people.Add((Maid)personFactory.GetPerson("Maid", temp), null);
                people.Last().Key.currentRoom = coordinates[1, 0];//lobby
                people.Last().Key.checkedIn = true;
            }

        }

        /// <summary>
        /// Start the build up for the clean up event right after the check out event.
        /// </summary>
        /// <param name="room">What room needs to be cleaned.</param>
        /// <param name="hotelEventType">Wanna know if it is a normal clean job or an emergency.</param>
        private void StartClean(IRoom room, HotelEventType hotelEventType)
        {

            IRoom currentRoom = room;
            bool maidFound = false;
            foreach (IPerson work in people.Keys)
            {
                if (work.id.Contains("Maid"))
                {


                    List<Maid> maids = new List<Maid>(); //List of all the maids.
                    foreach (IPerson person in people.Keys)
                    {
                        if (person.id.Contains("Maid"))//add all the maids
                        {
                            maids.Add((Maid)person);
                        }
                    }

                    foreach (Maid worker in maids)
                    {
                        Maid currentMaid = (Maid)worker; //What maid we are on.
                        if (worker.busy == false && currentRoom.roomIsGettingCleaned == false)
                        {

                            //Gives the maid the room we are on.
                            worker.busy = true;
                            worker.room = currentRoom;
                            currentRoom.roomIsGettingCleaned = true;

                            //Find path
                            IRoom end = currentRoom;
                            try
                            {
                                if (hotelEventType == HotelEventType.CLEANING_EMERGENCY)
                                {
                                    IRoom start = coordinates[currentMaid.eventQueue.LastOrDefault().Last().position.X, currentMaid.eventQueue.LastOrDefault().Last().position.Y];
                                    end.danger = true;
                                    FindPath(start, end, currentMaid);
                                }
                                else
                                {

                                    IRoom start = coordinates[currentMaid.eventQueue.LastOrDefault().Last().position.X, currentMaid.eventQueue.LastOrDefault().Last().position.Y];
                                    FindPath(start, end, currentMaid); //Add path to room which needs to be cleaned.
                                }
                            }
                            catch
                            {
                                if (hotelEventType == HotelEventType.CLEANING_EMERGENCY)
                                {
                                    IRoom start = currentMaid.currentRoom;
                                    end.danger = true;
                                    FindPath(start, end, currentMaid);
                                }
                                else
                                {

                                    IRoom start = currentMaid.currentRoom;
                                    FindPath(start, end, currentMaid); //Add path to room which needs to be cleaned.
                                }
                            }
                            maidFound = true;

                        }
                    }
                    if (maidFound == false)
                    {

                        Dictionary<Maid, List<IRoom>> paths = new Dictionary<Maid, List<IRoom>>();
                        for (int i = 0; i < maids.Count; i++)
                        {
                            try
                            {
                                List<IRoom> path = dijkstra.Run(maids[i].currentRoom, currentRoom, maids[i]);
                                paths.Add(maids[i], path);
                            }
                            catch
                            {
                                int posX = maids[i].eventQueue.Last().Last().position.X;
                                int posY = maids[i].eventQueue.Last().Last().position.Y;
                                List<IRoom> path = dijkstra.Run(coordinates[posX, posY], currentRoom, maids[i]);
                                paths.Add(maids[i], path); //Saves all the paths to the final destination.
                            }

                        }
                        List<IRoom> shortestPath = paths.Values.FirstOrDefault();

                        //Gets last element from the list aka the shortest path
                        foreach (var path in paths)
                        {
                            if (path.Value.LastOrDefault().distance < shortestPath.LastOrDefault().distance)
                            {
                                shortestPath = path.Value;
                            }
                            //checks wich maid this short path belongs too.
                            var query2 = from element in paths
                                         where element.Value == shortestPath
                                         select element.Key.id;

                            foreach (string whichMaid in query2)
                            {
                                if (path.Key.id == whichMaid)
                                {
                                    Maid currentMaid = (Maid)path.Key;
                                    currentMaid.busy = true;
                                    currentRoom.roomIsGettingCleaned = true;
                                    IRoom end = path.Value.Last();
                                    try
                                    {
                                        IRoom start = coordinates[currentMaid.eventQueue.LastOrDefault().Last().position.X, currentMaid.eventQueue.LastOrDefault().Last().position.Y];
                                        FindPath(start, end, path.Key);
                                    }
                                    catch
                                    {
                                        IRoom start = currentMaid.currentRoom;

                                        FindPath(start, end, path.Key);
                                    }

                                }
                            }
                        }

                    }




                }

            }

        }
        /// <summary>
        /// Sets all the values back to false after the room is cleaned so that someone else could use it.
        /// </summary>
        /// <param name="currentMaid">Maid who was cleaning this room.</param>
        /// <param name="currentRoom">Room who just got cleaned.</param>
        public void FinishCleaning(Maid currentMaid, IRoom currentRoom)
        {
            currentMaid.busy = false;
            currentRoom.dirtyRoom = false;
            Room room = (Room)currentRoom;
            room.taken = false;
            //currentMaid.eventStarted = false;
            if (currentRoom.danger == true)
            {
                currentRoom.danger = false;
            }
            if (currentMaid.eventQueue.Count() == 0 && (currentMaid.position.X != 1 && currentMaid.position.Y != 0))
            {
                IRoom start = currentMaid.currentRoom;
                IRoom end = coordinates[1, 0];
                FindPath(start, end, currentMaid);
            }



        }




        /// <summary>
        /// Notify functions to launch all the events.
        /// </summary>
        /// <param name="evt">The event that is launched.</param>
        public void Notify(HotelEvent evt)
        {

            if (evt.EventType == HotelEventType.CHECK_IN && evt.Data != null) //Handles the check in event.
            {

                TempPerson tempPerson = new TempPerson();//create a temp person
                tempPerson.id = evt.Data.Keys.FirstOrDefault(); //store id
                tempPerson.roomPref = evt.Data.Values.FirstOrDefault();//store roompref

                Room room = getRoom(tempPerson.roomPref);//get the pref room 
                people.Add((Customer)personFactory.GetPerson("Customer", tempPerson), room);//create the customer
                Customer current = (Customer)people.Keys.Last();
                current.currentRoom = coordinates[1, 0];//set the currentroom the customer is in to lobby
                ///if a room is found
                ///start a pathfinding 
                if (room != null)
                {
                    current.checkedIn = true;
                    current.room = room;
                    IRoom start = current.currentRoom;
                    IRoom end = current.room;
                    FindPath(start, end, current);

                }



            }

            else if (evt.EventType == HotelEventType.CHECK_OUT || evt.EventType == HotelEventType.CLEANING_EMERGENCY) //Handles the Check out and cleaning emergency events.
            {
                if (evt.EventType == HotelEventType.CLEANING_EMERGENCY)
                {
                    //look which room needs to be cleaned
                    foreach (var element in coordinates)
                    {
                        string roomData = evt.Data.Values.FirstOrDefault().ToString();
                        string HTE_Factor_number = evt.Data.Values.LastOrDefault();
                        float HTE_Factor = float.Parse(HTE_Factor_number);
                        if (element != null)
                        {
                            ///if room is found
                            ///start the cleaning
                            if (element.id.ToString() == roomData)
                            {
                                StartClean(element, evt.EventType);
                                HotelEventManager.HTE_Factor = HTE_Factor;
                            }
                        }
                    }
                }
                else//else its a checkout event
                {

                    foreach (var element in people)
                    {
                        if (element.Key.id.Contains("Gast"))
                        {

                            string peopleData = evt.Data.Keys.FirstOrDefault().ToString() + evt.Data.Values.FirstOrDefault().ToString();

                            if (peopleData == element.Key.id)
                            {
                                element.Value.dirtyRoom = true;
                                StartClean(element.Value, evt.EventType);

                                Customer current = null;
                                foreach (KeyValuePair<IPerson, IRoom> person in people)
                                {
                                    if (person.Key == element.Key)
                                    {
                                        current = (Customer)person.Key;
                                    }
                                }
                                IRoom end = coordinates[1, 0];//lobby
                                IRoom start = current.currentRoom;
                                FindPath(start, end, current);//path to lobby
                                current.checkedIn = false;

                                break;
                            }
                        }

                    }
                }




            }
            else if (evt.EventType == HotelEventType.GOTO_CINEMA) //Handles the Go to Cinema event.
            {
                string data = evt.Data.Keys.FirstOrDefault().ToString() + evt.Data.Values.FirstOrDefault().ToString();
                foreach (IPerson person in people.Keys)
                {
                    if (person.id == data)
                    {
                        Customer current = (Customer)person;
                        if (current.checkedIn == true)
                        {
                            string endAreaType = "Cinema";
                            //send to closest cinema
                            try
                            {
                                IRoom start = coordinates[current.eventQueue.LastOrDefault().Last().position.X, current.eventQueue.LastOrDefault().Last().position.Y];
                                FindPathUnkown(start, endAreaType, current);
                            }
                            catch
                            {
                                IRoom start = current.currentRoom;
                                FindPathUnkown(start, endAreaType, current);
                            }
                        }
                    }
                }
            }
            else if (evt.EventType == HotelEventType.NEED_FOOD) //Handles the need food event.
            {
                string data = evt.Data.Keys.FirstOrDefault().ToString() + evt.Data.Values.FirstOrDefault().ToString();
                foreach (IPerson person in people.Keys)
                {
                    if (person.id == data)
                    {
                        Customer current = (Customer)person;
                        if (current.checkedIn == true)
                        {
                            string endAreaType = "Restaurant";
                            //send to closest restaurant
                            try
                            {
                                IRoom start = coordinates[current.eventQueue.LastOrDefault().Last().position.X, current.eventQueue.LastOrDefault().Last().position.Y];
                                FindPathUnkown(start, endAreaType, current);
                            }
                            catch
                            {

                                IRoom start = person.currentRoom;
                                FindPathUnkown(start, endAreaType, current);
                            }

                        }
                    }
                }
            }
            else if (evt.EventType == HotelEventType.GOTO_FITNESS)
            {
                string data = evt.Data.Keys.FirstOrDefault().ToString() + evt.Data.Values.FirstOrDefault().ToString();
                foreach (IPerson person in people.Keys)
                {
                    if (person.id == data)
                    {
                        Customer current = (Customer)person;
                        if (current.checkedIn == true)
                        {
                            string endAreaType = "Fitness";
                            //send to closest fitness
                            try
                            {
                                IRoom start = coordinates[current.eventQueue.LastOrDefault().Last().position.X, current.eventQueue.LastOrDefault().Last().position.Y];
                                FindPathUnkown(start, endAreaType, current);
                            }
                            catch
                            {

                                IRoom start = person.currentRoom;
                                FindPathUnkown(start, endAreaType, current);
                            }

                        }
                    }
                }
            }
            else if (evt.EventType == HotelEventType.EVACUATE)
            {
                IRoom end = coordinates[1, 0];
                evacuation = true;
                foreach (IPerson element in people.Keys)
                {
                    
                    element.eventStarted = false;//set to false so perons who are in a event can start a new event
                    IRoom start = element.currentRoom;
                    //find path
                    if (start.position == element.position)
                    {
                        FindPathEvacuation(start, end, element, true);
                    }
                    else
                    {
                        IRoom nextRoom = element.currentRoom.neighbours.Keys.First();
                        FindPathEvacuation(nextRoom, end, element, true);
                        element.position = nextRoom.position;
                    }

                    if (element.position == new Point(1, 0))
                    {
                        evacCounter++;
                    }
                }
            }


        }
        /// <summary>
        /// Returns path from start point to end destination.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">Destination.</param>
        /// <param name="current">Person who will be walking this path.</param>
        public void FindPath(IRoom start, IRoom end, IPerson current)
        {
            List<IRoom> path = dijkstra.Run(start, end, current);
            List<IRoom> last = current.eventQueue.LastOrDefault();
            if (end.danger == true)
            {
                current.eventQueue.AddFirst(path);
            }
            else
            {
                current.eventQueue.AddLast(path);

            }
            if (current.eventQueue.Count > 1 && last.Last() == current.eventQueue.Last().Last())
            {
                current.eventQueue.RemoveLast();
            }

        }
        /// <summary>
        /// Finds path where it is not decided what will be the final destination.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="areaType">Checks which room is closest of this areatype.</param>
        /// <param name="current">The person who walks this path.</param>
        public void FindPathUnkown(IRoom start, string areaType, IPerson current)
        {
            List<IRoom> path = dijkstra.RunUnkown(start, areaType);
            List<IRoom> last = current.eventQueue.LastOrDefault();
            current.eventQueue.AddLast(path);

            if (current.eventQueue.Count > 1 && last.Last() == current.eventQueue.Last().Last())
            {
                current.eventQueue.RemoveLast();
            }

        }

        public void FindPathEvacuation(IRoom start, IRoom end, IPerson current, bool evac)
        {
            dijkstra.evac = evac;
            List<IRoom> path = dijkstra.Run(start, end, current);
            //List<IRoom> last = current.eventQueue.Last();
            //current.eventQueue.Clear();
            current.eventQueue.AddFirst(path);

            //if (current.eventQueue.Count >= 1)
            //{
            //    current.eventQueue.RemoveLast();
            //}
            dijkstra.evac = false;
        }
        /// <summary>
        /// Gets the room the customers desires.
        /// </summary>
        /// <param name="wish">The room the customer wishes for.</param>
        /// <returns>The room you desire.</returns>
        private Room getRoom(string wish)
        {
            List<Room> rooms = new List<Room>();
            string roomWish = Regex.Match(wish, @"\d+").Value;


            foreach (IRoom room in coordinates)
            {
                if (room != null && room.areaType == "Room")
                {
                    rooms.Add((Room)room);
                }
            }

            foreach (Room room in rooms)
            {
                //get the actual pref room
                if (room.taken == false && room.classification.Contains(roomWish))
                {
                    room.taken = true;
                    return room;
                }
            }

            foreach (Room room in rooms)
            {
                //get a room that isnt taken
                if (room.taken == false)
                {
                    room.taken = true;
                    return room;
                }
            }
            return null;
        }
    }
}

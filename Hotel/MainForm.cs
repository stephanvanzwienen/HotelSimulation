using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using HotelEvents;
using System.Diagnostics;
using System.Reflection;

namespace Hotel
{
    public partial class MainForm : Form
    {
        //program setup
        private Timer timer { get; } = new Timer();
        private Timer timerForMaids { get; } = new Timer();
        private Settings settings { get; set; }
        private Draw draw = new Draw();
        private Bitmap background; //Bitmap where we draw the hotel layout on
        private Bitmap personLayout; //Bitmap where we draw the persons
        private Manager manager;
        private RoomFactory roomFactory = new RoomFactory();
        public Dictionary<IPerson, IRoom> peopleToDraw = new Dictionary<IPerson, IRoom>();
        Elevator elevator;
        private int timerTickCount { get; set; }

        //Form atributes
        private int buttonCounter { get; set; }
        public Button pauseButton = new Button();
        public Button speedButton = new Button();
        public Button stopButton = new Button();


        public MainForm(Settings settings)
        {

            this.settings = settings;//store the selected settings
            timer.Interval = 1000; //set the tick frequency 
            timerForMaids.Interval = 1000;
            roomFactory.GenerateEntity();
            manager = new Manager(roomFactory.coordinates, settings.amountOfMaids, this);

            //creating background and inintializing personLayout 
            background = draw.DrawLayout(roomFactory.coordinates);
            personLayout = new Bitmap(background.Width, background.Height);


            InitializeComponent();
            hotelMap.BackgroundImage = background; //set the background


            //Setup buttons
            int distanceButtons = 100;
            pauseButton.Location = new Point(background.Width + distanceButtons, hotelMap.Location.Y + distanceButtons);
            pauseButton.Size = new Size(250, 100);
            pauseButton.Text = "Pauze";
            pauseButton.Click += new EventHandler(PauseButton_Click);
            speedButton.Location = new Point(background.Width + distanceButtons, pauseButton.Height + distanceButtons);
            speedButton.Size = pauseButton.Size;
            speedButton.Text = "Versnellen";
            speedButton.Click += new EventHandler(SpeedButton_Click);
            stopButton.Location = new Point(background.Width + distanceButtons, speedButton.Location.Y + distanceButtons);
            stopButton.Size = pauseButton.Size;
            stopButton.Text = "Stop";
            stopButton.Click += new EventHandler(StopButton_Click);
            Controls.Add(pauseButton);
            Controls.Add(speedButton);
            Controls.Add(stopButton);

            //Start the hotel events 
            HotelEventManager.Start();
            timer.Start();
            timerForMaids.Start();

            //call the update function after each timer tick.
            timer.Tick += new EventHandler(UpdateImage);


        }

        /// <summary>
        /// update the displayed image in the picturebox of the form so there can be animations displayed.
        /// </summary>
        /// <param name="sender">interval</param>
        /// <param name="e"></param>
        private void UpdateImage(object sender, EventArgs e)
        {
            timerForMaids.Tick += (UpdateEvents); //call updateevents
            elevator = roomFactory.elevator; //get the elevator and store it for later use
            personLayout.Dispose();//empty the Bitmap
            personLayout = new Bitmap(background.Width, background.Height); //create new one

            //if there are changes in the simulation
            //update personlayout 
            if (peopleToDraw != manager.people)
            {
                peopleToDraw.Clear();//empty the list 
                //foreach person in manager.people add it to peopleToDraw
                foreach (KeyValuePair<IPerson, IRoom> person in manager.people)
                {
                    IPerson last = manager.people.Keys.Last();
                    peopleToDraw.Add(person.Key, person.Value);

                    //if person has a path to walk. go walk
                    if (person.Key.route != null && person.Key.route.Count > 0)
                    {
                        bool wait = false;

                        if (roomFactory.elevator.position == person.Key.position) //check if person is in a elevator
                        {
                            wait = true;
                            if (roomFactory.elevator.running == true)//if elevator is running person can go up
                            {
                                person.Key.WalkTo();

                                if (person.Key.Equals(last))//if last person moved update the elvator
                                {
                                    elevator.prevPosition = elevator.position;
                                    elevator.position = new Point(0, elevator.people.Last().position.Y);//set the elevator to same pos as person in elevator.
                                    roomFactory.coordinates[elevator.position.X, elevator.position.Y] = elevator;
                                    if (elevator.prevPosition != elevator.position)
                                    {
                                        ElevatorShaft shaft = new ElevatorShaft();
                                        shaft.dimension = new Point(1, 1);
                                        roomFactory.coordinates[elevator.prevPosition.X, elevator.prevPosition.Y] = shaft;
                                        shaft.position = elevator.prevPosition;
                                    }
                                }
                                IRoom check = roomFactory.coordinates[person.Key.position.X, person.Key.position.Y];
                                if (check != null && roomFactory.coordinates[check.position.X, check.position.Y] == check)
                                {
                                    person.Key.currentRoom = check;
                                }
                                manager.dijkstra.CreateGraph(roomFactory.coordinates);
                            }
                        }
                        else if (wait == false)//if wait is false. The person is not in a elevator so it can continue moving. 
                        {
                            person.Key.WalkTo();
                            IRoom check = roomFactory.coordinates[person.Key.position.X, person.Key.position.Y];
                            if (check != null && roomFactory.coordinates[check.position.X, check.position.Y] == check)
                            {
                                person.Key.currentRoom = check;
                            }

                        }
                        wait = false;
                    }

                    Customer current = null;

                    //if person is customer and is checking out remove him
                    if (person.Key.id.Contains("Gast"))
                    {
                        current = (Customer)person.Key;
                        ///if a person is in the lobby and is checked out or a evac is going on
                        ///make them dissapear
                        if (current.checkedIn == false && current.currentRoom == roomFactory.coordinates[1, 0] || current.currentRoom.position == new Point(1, 0) && manager.evacuation == true)
                        {
                            peopleToDraw.Remove(current);
                        }
                    }

                }

            }
            //draw the new personLayout and background
            background.Dispose();
            background = draw.DrawLayout(roomFactory.coordinates);
            personLayout = draw.DrawPersonLayout(personLayout, peopleToDraw);

            //dislay new persenLayout and background
            hotelMap.BackgroundImage = background;
            hotelMap.Image = personLayout;
            

        }
        /// <summary>
        /// Updates the queue for all the persons in the hotel to see what they can do next
        /// </summary>
        /// <param name="sender">/Interval</param>
        /// <param name="e"></param>
        public void UpdateEvents(object sender, EventArgs e)
        {
            IPerson last = manager.people.Keys.LastOrDefault();
            foreach (IPerson person in peopleToDraw.Keys)
            {

                if (person.id.Contains("Maid")) //checks if the person is from the maid class
                {
                    Maid currentMaid = (Maid)person;

                    if (currentMaid.eventQueue.Count > 0 && currentMaid.eventQueue.FirstOrDefault().LastOrDefault().position == currentMaid.position) //checks if the maid is in the room she needs to clean.
                    {
                        currentMaid.eventStarted = true;
                        if (timerTickCount - currentMaid.timeBusyEvent >= 250 && manager.evacuation == false) //after around 4.8 seconds the maid is done with cleaning room she is in.
                        {
                            IRoom start = currentMaid.eventQueue.FirstOrDefault().Last();
                            if (currentMaid.eventQueue.Count > 0) //removes the room the maid was cleaning out of her toclean list.
                            {
                                currentMaid.eventQueue.RemoveFirst();
                                currentMaid.route.Clear();
                            }

                            currentMaid.eventStarted = false;
                            manager.FinishCleaning(currentMaid, currentMaid.room);//finish cleaning the room

                        }

                    }
                }

                else if (person.id.Contains("Gast")) //checks if the person is from the customer class
                {
                    Customer current = (Customer)person;

                    if ((current.eventQueue.Count > 0 && current.eventQueue.FirstOrDefault().LastOrDefault().position == current.position))//selects shortest path to the final destination of its event.
                    {

                        current.eventStarted = true;


                        if (timerTickCount - current.timeBusyEvent >= 250 && manager.evacuation == false) //after around 4.8 seconds goes to next event in the queue (LinkedList).
                        {

                            if (manager.evacuation == false)
                            {

                                IRoom start = current.currentRoom;

                                if (current.eventQueue.Count > 0) //Removes first element in the list.
                                {
                                    current.eventQueue.RemoveFirst();
                                    current.route.Clear();
                                }
                                //find new path
                                if (current.eventQueue.Count == 0 && current.position != current.room.position)
                                {
                                    IRoom end = current.room;
                                    manager.FindPath(start, end, current);
                                }
                                else if (current.eventQueue.Count > 0)
                                {
                                    IRoom end = current.eventQueue.First().Last();
                                    manager.FindPath(start, end, current);
                                }
                            }

                            current.eventStarted = false;

                        }
                    }

                    if (person.position.X == 0 && elevator.people.Contains(person) == false)//if person is in elevator and the elevator doesnt have the person in his list, add the person to the list
                    {
                        elevator.people.Add(person);
                    }
                    else if (elevator.people.Contains(person) == true && person.position.X != 0) // if person is in the list but not in elevator, remove the person from the list.
                    {
                        elevator.people.Remove(person);
                    }

                    if (elevator.elevatorTimer - elevator.elevatorPrevTimer >= 25)//wait ~5 sec until elevator start moving
                    {
                        elevator.running = true; //if waiting is over start the elevator
                        elevator.elevatorPrevTimer = elevator.elevatorTimer;
                    }
                    else if (roomFactory.elevator.people.Count > 0) //if elevator contains people start counting
                    {
                        elevator.elevatorTimer = timerTickCount;
                    }
                    else if (roomFactory.elevator.people.Count <= 0 && elevator.running == true)
                    {
                        elevator.running = false;
                    }

                }
                ///check if a evac is going on, if so set person.evac to treu
                ///if there is no evac start new the new path
                if (person.eventQueue.Count > 0 && person.route.Count == 0 && person.eventQueue.FirstOrDefault().Count > 1 || manager.evacuation == true && person.evac == false)
                {
                    if (manager.evacuation == true)
                    {
                        person.evac = true;
                    }

                    person.SavePath(person.eventQueue.First());
                }

            }
            if (manager.evacuation == true) // if evac is started
            {

                if (manager.hotelEmpty == false)
                {
                    //query to check if all persons are in the lobby
                    var amount = from person in manager.people.Keys
                                 where person.position != new Point(1, 0)
                                 select person;
                    List<IPerson> check = amount.ToList<IPerson>();

                    //if all are in lobby start the event
                    if (check.Count == 0)
                    {
                        manager.hotelEmpty = true;
                        foreach (IPerson person in manager.people.Keys)
                        {
                            person.eventStarted = true;
                            person.timeBusyEvent = timerTickCount;
                        }
                    }
                }

                ///foreach person check if the evac is over
                ///if yes, continue where they left off.
                foreach (IPerson person in manager.people.Keys)
                {
                    if (manager.evacuation == true && manager.hotelEmpty == true && person.checkedIn == true)
                    {
                        //check if evac is over
                        if (timerTickCount - person.timeBusyEvent >= 250)//check if waitingt time is over, if so return to rooms
                        {
                            //check if the last person in the loop, if yes stop the evac
                            if (last == person)
                            {
                                manager.evacuation = false;
                            }
                            
                            person.evac = false;
                            //remove the evac event from eventqueue
                            if (person.eventQueue.Count > 0)
                            {
                                person.eventQueue.RemoveFirst();
                            }

                            //find path to event that the person was doing before the evac
                            if (person.eventQueue.Count > 0 && person.eventQueue.FirstOrDefault().Count > 2)
                            {
                                int x = person.eventQueue.First().Last().position.X;
                                int y = person.eventQueue.First().Last().position.Y;
                                IRoom end = roomFactory.coordinates[x, y];
                                person.eventQueue.RemoveFirst();
                                if (end.position != new Point(1, 0))
                                {

                                    manager.FindPathEvacuation(person.currentRoom, end, person, false);
                                    person.SavePath(person.eventQueue.First());
                                }

                            }
                            ///if the person had no events.
                            ///customer returns to room.
                            ///maid stays in lobby
                            else if (person.room.position != new Point(1, 0))
                            {
                                manager.FindPathEvacuation(person.currentRoom, person.room, person, false);

                                person.SavePath(person.eventQueue.First());

                            }
                        }
                    }


                }
            }
            foreach (IPerson person in manager.people.Keys)// if a event hasnt started, set timertickcount equal to person time.
            {

                if (person.eventStarted == false)
                {
                    person.timeBusyEvent = timerTickCount;
                }
            }

            timerTickCount++;
        }





        private void hotelMap_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Exit the app 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Pause the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (buttonCounter % 2 == 0 || buttonCounter == 0)
            {
                HotelEventManager.Pauze();
                timer.Stop();
                pauseButton.Text = "Hervat";
                buttonCounter++;
            }
            else
            {
                HotelEventManager.Start();
                timer.Start();
                pauseButton.Text = "Pauze";
                buttonCounter++;
            }
        }

        /// <summary>
        /// Speed up or slow down the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeedButton_Click(object sender, EventArgs e)
        {
            if (buttonCounter % 2 == 0 || buttonCounter == 0)//speed up
            {
                HotelEventManager.HTE_Factor = HotelEventManager.HTE_Factor * 2f;
                timer.Interval = timer.Interval / 2;
                speedButton.Text = "Vertragen";
                buttonCounter++;
            }
            else//slow down
            {
                HotelEventManager.HTE_Factor = HotelEventManager.HTE_Factor / 2f;
                timer.Interval = timer.Interval * 2;
                speedButton.Text = "Versnellen";
                buttonCounter++;
            }
        }

        /// <summary>
        /// Stop the simulation and go back to setting menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, EventArgs e)
        {
            HotelEventManager.Stop();
            this.Hide();
            settings.Show();
        }

    }
}

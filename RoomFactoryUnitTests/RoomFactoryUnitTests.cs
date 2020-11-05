using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hotel;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace RoomFactoryUnitTests
{

    [TestClass]
    public class RoomFactoryUnitTests
    {

        public RoomFactory roomFactory = new RoomFactory();
        [TestMethod]
        public void LayoutReadOutOfJsonFileAndPutInAListOfRooms()
        {
            //arrange
            RoomFactory roomFactory = new RoomFactory();
            roomFactory.GenerateEntity();
            //act
            List<IRoom> output = roomFactory.rooms;
            //assert
            Assert.AreEqual(output, roomFactory.rooms);
        }
        [TestMethod]
        public void AreThereAsMuchRoomsInTheListAsThereReallyAre()
        {
            //arrange
            roomFactory.GenerateEntity();
            //act
            List<IRoom> output = roomFactory.rooms;
            //assert
            Assert.AreEqual(output.Count, roomFactory.rooms.Count);
        }

        [TestMethod]
        public void DoesTheRoomGetTheCorrectRooom()
        {
            //arrange
            RoomFactory roomFactory = new RoomFactory();
            roomFactory.GenerateEntity();

            //act
            TempRoom test = roomFactory.tempRooms[0];
            var room = roomFactory.GenerateRoom<IRoom>(test.areaType, test);

            //assert
            Assert.AreEqual(room.areaType, test.areaType);
        }

        [TestMethod]
        public void CheckIfCreateOverviewTheRoomInTheCorrectSpotSets()
        {
            //arrange
            RoomFactory roomFactory = new RoomFactory();
            roomFactory.GenerateEntity();


            //act

            IRoom room = roomFactory.coordinates[1, 1];
            Point position = room.position;
            Point actualPosition = new Point(1, 1);

            //var room = roomFactory.GenerateRoom<IRoom>(test.areaType, test);

            //assert
            Assert.AreEqual(actualPosition, position);
        }
    }
    [TestClass]
    public class DijkstraUnitTest
    {
        [TestMethod]
        public void DoesDijkstraSendToPrefRoom()
        {
            //arrange
            TempPerson person = new TempPerson();
            Dijkstra dijkstra = new Dijkstra();
            RoomFactory roomFactory = new RoomFactory();
            roomFactory.GenerateEntity();
            dijkstra.CreateGraph(roomFactory.coordinates);
            IRoom start = roomFactory.coordinates[0, 1];
            IRoom end = roomFactory.coordinates[1, 1];

            //act
            List<IRoom> path = dijkstra.Run(start, end, person);

            //assert
            Assert.AreEqual(end, path.Last());
        }

        [TestMethod]
        public void DoesDijkstraSendToRestaurant()
        {
            //arrange
            Dijkstra dijkstra = new Dijkstra();
            RoomFactory roomFactory = new RoomFactory();
            roomFactory.GenerateEntity();
            dijkstra.CreateGraph(roomFactory.coordinates);
            IRoom start = roomFactory.coordinates[0, 1];


            //act
            List<IRoom> path = dijkstra.RunUnkown(start, "Restaurant");

            //assert
            Assert.AreEqual("Restaurant", path.Last().areaType);
        }

    }
    [TestClass]
    public class PersonFactoryTest
    {
        [TestMethod]
        public void DoesFactoryReturnMaidsAndCostumers()
        {
            //arrange
            PersonFactory personFactory = new PersonFactory();
            TempPerson tempMaid = new TempPerson();
            TempPerson tempCustomer = new TempPerson();
            string resultMaid;
            string resultCustomer;

            //act
            IPerson maid = personFactory.GetPerson("Maid", tempMaid);
            IPerson customer = personFactory.GetPerson("Customer", tempCustomer);
            Type typeMaid = maid.GetType();
            Type typeCostumer = customer.GetType();
            resultMaid = typeMaid.Name.ToString();
            resultCustomer = typeCostumer.Name.ToString();

            //assert
            Assert.AreEqual("Maid", resultMaid);
            Assert.AreEqual("Customer", resultCustomer);
        }



    }
}

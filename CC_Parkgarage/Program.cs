using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC_Parkgarage
{
    public class Program
    {
        private static Garage garage;
        private static LinkedList<int> carTracking;

        public static void Main(string[] args)
        {
            while (true)
            {
                ReadInput();

                /* n parking lots
                 * every morning empty
                 * today m cars are arriving
                 * every car gets ticket with number (id)
                 * aren't given out in sorted order
                 * positive number if parking lot is used
                 * negative number if parking lot is free
                 * entries are chronologically -> SortedList
                 * cars have to enter and leave once
                 *

                */
                var maxValues = GetMaximumSimultaneousCarsInGarage();
                Console.WriteLine($"{maxValues.MaxInLot} {maxValues.MaxWaiting}");
            }
        }

        //  task: get maximum number of cars that have been at the same time in the garage
        private static (int MaxInLot, int MaxWaiting) GetMaximumSimultaneousCarsInGarage()
        {
            var node = carTracking.First;
            var maxInLot = 0;
            var waitingQueue = new Queue<int>();
            var maxWaiting = 0;

            while (node != null)
            {
                // car is arriving
                if (node.Value > 0)
                {
                    // check if parking lot is full
                    // if yes -> wait
                    if (garage.IsFull)
                    {
                        waitingQueue.Enqueue(node.Value);
                    }
                    // if not -> reduce waiting list and add to lot
                    else
                    {
                        garage.ParkCar(node.Value);
                    }
                }
                // car is departing
                else
                {
                    garage.UnparkCar(node.Value);

                    // check if someone is waiting
                    if (waitingQueue.Count > 0)
                    {
                        garage.ParkCar(waitingQueue.Dequeue());
                    }
                }

                // check for maximum values
                maxInLot = maxInLot < garage.Lots.Count ? garage.Lots.Count : maxInLot;
                maxWaiting = maxWaiting < waitingQueue.Count ? waitingQueue.Count : maxWaiting;

                node = node.Next;
            }

            return (maxInLot, maxWaiting);
        }

        private static void ReadInput()
        {
            Console.WriteLine("Using input file: C:\\temp\\input.txt\nPress Return for run.");
            Console.ReadLine();

            var lines = File.ReadLines("C:\\temp\\input.txt").ToArray();
            var garageSetup = lines[0].Split(' ');
            var trackingList = lines[1].Split(' ');

            garage = new Garage
            {
                MaxNumberOfLots = int.Parse(garageSetup[0]),
                TodaysNumberOfCars = int.Parse(garageSetup[1]),
                Lots = new Dictionary<int, int>()
            };

            carTracking = new LinkedList<int>();
            foreach (var car in trackingList)
            {
                carTracking.AddLast(int.Parse(car));
            }
        }

        public class Garage
        {
            // max = 200
            public int MaxNumberOfLots { get; set; }

            public int TodaysNumberOfCars { get; set; }

            public Dictionary<int, int> Lots { get; set; }

            public bool IsFull { get { return MaxNumberOfLots == Lots.Count; } }

            public void ParkCar(int i)
            {
                Lots.Add(i, i);
            }

            public void UnparkCar(int i)
            {
                i = i * -1;
                Lots.Remove(i);
            }
        }
    }
}
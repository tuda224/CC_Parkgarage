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
        private static Dictionary<int, int> carWeights;

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
                 * every car parks on lot with lowest number
                 * every parking lot has specific price
                 * on departing car pays no matter how long it was standing

                */
                var maxValues = GetMaximumSimultaneousCarsInGarage();
                //Console.WriteLine($"{maxValues.MaxInLot} {maxValues.MaxWaiting}");
                Console.WriteLine(maxValues.Income);
            }
        }

        //  task: get maximum number of cars that have been at the same time in the garage
        private static (int MaxInLot, int MaxWaiting, int Income) GetMaximumSimultaneousCarsInGarage()
        {
            var node = carTracking.First;
            var maxInLot = 0;
            var waitingQueue = new Queue<Car>();
            var maxWaiting = 0;
            var income = 0;

            while (node != null)
            {
                // car is arriving
                if (node.Value > 0)
                {
                    // check if parking lot is full
                    // if yes -> wait
                    if (garage.IsFull)
                    {
                        waitingQueue.Enqueue(new Car
                        {
                            Number = node.Value,
                            Weight = carWeights[node.Value]
                        });
                    }
                    // if not -> reduce waiting list and add to lot
                    else
                    {
                        garage.ParkCar(new Car
                        {
                            Number = node.Value,
                            Weight = carWeights[node.Value]
                        });
                    }
                }
                // car is departing
                else
                {
                    var lotInformation = garage.UnparkCar(node.Value);
                    income += lotInformation.Price * (int)Math.Ceiling((double)lotInformation.Car.Weight / 100);

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

            return (maxInLot, maxWaiting, income);
        }

        private static void ReadInput()
        {
            Console.WriteLine("Using input file: C:\\temp\\input.txt\nPress Return for run.");
            Console.ReadLine();

            var lines = File.ReadLines("C:\\temp\\input.txt").ToArray();
            var garageSetup = lines[0].Split(' ');
            var prices = lines[1].Split(' ');
            var carweight = lines[2].Split(' ');
            var trackingList = lines[3].Split(' ');

            garage = new Garage
            {
                MaxNumberOfLots = int.Parse(garageSetup[0]),
                TodaysNumberOfCars = int.Parse(garageSetup[1]),
                Lots = new Dictionary<int, LotInformation>()
            };

            // init lots
            for (int i = 1; i <= garage.MaxNumberOfLots; i++)
            {
                garage.Lots.Add(i, new LotInformation
                {
                    Price = int.Parse(prices[i - 1])
                });
            }

            carTracking = new LinkedList<int>();
            foreach (var car in trackingList)
            {
                carTracking.AddLast(int.Parse(car));
            }

            carWeights = new Dictionary<int, int>();
            for (int i = 1; i <= carweight.Length; i++)
            {
                carWeights.Add(i, int.Parse(carweight[i - 1]));
            }
        }

        public class Car
        {
            public int Number { get; set; }
            public int Weight { get; set; }
        }

        public class Garage
        {
            // max = 200
            public int MaxNumberOfLots { get; set; }

            public int TodaysNumberOfCars { get; set; }

            public Dictionary<int, LotInformation> Lots { get; set; }

            public bool IsFull { get { return MaxNumberOfLots == Lots.Values.Where(l => l.Car != null).Count(); } }

            public LotInformation ParkCar(Car car)
            {
                var lotInformation = Lots.Values.Where(l => l.Car == null).First();
                lotInformation.Car = car;
                return lotInformation;
            }

            public (Car Car, int Price) UnparkCar(int i)
            {
                i = i * -1;
                var lotInformation = Lots.Values.Where(l => l.Car != null && l.Car.Number == i).First();
                var car = lotInformation.Car;
                lotInformation.Car = null;
                return (car, lotInformation.Price);
            }
        }

        public class LotInformation
        {
            public Car Car { get; set; }
            public int Price { get; set; } = 0;
        }
    }
}
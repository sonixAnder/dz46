using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CarRacingGame
{
    public abstract class Car
    {
        public string Name { get; set; }
        public int Speed { get; protected set; }
        public int Position { get; protected set; } = 0;
        public event Action<Car> OnFinish; // Событие для уведомления о финише

        protected Random random = new Random();

        public Car(string name)
        {
            Name = name;
        }


        public void Move()
        {
            Speed = random.Next(10, 21); // Случайная скорость от 10 до 20
            Position += Speed;
            if (Position >= 100)
            {
                Position = 100; // Зафиксировать позицию на финише
                OnFinish?.Invoke(this); // Вызов события
            }
        }
    }

    public class SportCar : Car
    {
        public SportCar(string name) : base(name) { }
    }

    public class PassengerCar : Car
    {
        public PassengerCar(string name) : base(name) { }
    }

    public class Truck : Car
    {
        public Truck(string name) : base(name) { }
    }

    public class Bus : Car
    {
        public Bus(string name) : base(name) { }
    }

    public class Race
    {
        private List<Car> cars = new List<Car>();
        private bool raceFinished = false;

        public void AddCar(Car car)
        {
            cars.Add(car);
            car.OnFinish += HandleFinish;
        }

        public void StartRace()
        {
            Console.WriteLine("Гонка началась!");

            while (!raceFinished)
            {
                foreach (var car in cars)
                {
                    if (!raceFinished)
                    {
                        car.Move();
                        Console.WriteLine($"{car.Name} проехал {car.Position} км.");
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void HandleFinish(Car car)
        {
            raceFinished = true;
            Console.WriteLine($"Победитель: {car.Name}!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sportCar = new SportCar("Спортивный автомобиль");
            var passengerCar = new PassengerCar("Легковой автомобиль");
            var truck = new Truck("Грузовик");
            var bus = new Bus("Автобус");

            var race = new Race();

            race.AddCar(sportCar);
            race.AddCar(passengerCar);
            race.AddCar(truck);
            race.AddCar(bus);

            race.StartRace();
        }
    }
}

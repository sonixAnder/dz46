using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame
{
    public class Karta
    {
        public string Masty { get; }
        public string Type { get; }
        public int Value { get; }

        public Karta(string masty, string type, int value)
        {
            Masty = masty;
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type} {Masty}";
        }
    }

    public class Player
    {
        public string Name { get; }
        public Queue<Karta> Cards { get; }

        public Player(string name)
        {
            Name = name;
            Cards = new Queue<Karta>();
        }

        public void AddCards(IEnumerable<Karta> cards)
        {
            foreach (var card in cards)
            {
                Cards.Enqueue(card);
            }
        }

        public Karta PlayCard()
        {
            return Cards.Count > 0 ? Cards.Dequeue() : null;
        }

        public bool HasCards => Cards.Count > 0;

        public override string ToString()
        {
            return $"{Name} ({Cards.Count} карты)";
        }
    }

    public class Game
    {
        private readonly List<Player> players;
        private readonly List<Karta> deck;
        private readonly int maxRounds;

        public Game(List<string> playerNames, int maxRounds)
        {
            if (playerNames.Count < 2)
                throw new ArgumentException("В игре должно быть минимум два игрока.");

            players = playerNames.Select(name => new Player(name)).ToList();
            deck = GenerateDeck();
            this.maxRounds = maxRounds;

        }

        private List<Karta> GenerateDeck()
        {
            var mastys = new[] { "♥️", "♦️", "♣️", "♠️" };
            var types = new[] { "6", "7", "8", "9", "10", "Валет", "Дама", "Король", "Туз" };
            var values = new[] { 6, 7, 8, 9, 10, 11, 12, 13, 14 };

            return mastys.SelectMany(m => types.Select((t, i) => new Karta(m, t, values[i]))).ToList();
        }

        private void ShuffleDeck()
        {
            var random = new Random();

            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }


        private void DealCards()
        {
            int playerCount = players.Count;
            for (int i = 0; i < deck.Count; i++)
            {
                players[i % playerCount].AddCards(new[] { deck[i] });
            }
        }

        public void Play()
        {
            ShuffleDeck();
            DealCards();

            int round = 0;

            while (players.Count(p => p.HasCards) > 1 && round < maxRounds)
            {
                round++;
                Console.WriteLine($"\n--- Раунд {round} ---");
                var table = new List<(Player, Karta)>();

                foreach (var player in players.Where(p => p.HasCards))
                {
                    var card = player.PlayCard();
                    table.Add((player, card));
                    Console.WriteLine($"{player.Name} сыграл карту {card}");
                }

                var winner = DetermineRoundWinner(table);
                Console.WriteLine($"Победитель раунда: {winner.Name}");
                winner.AddCards(table.Select(t => t.Item2));

                players.RemoveAll(p => !p.HasCards);
            }

            if (players.Count == 1)
            {
                Console.WriteLine($"\nПобедитель игры: {players.First().Name}");
            }
            else
            {
                Console.WriteLine("\nИгра завершилась по количеству раундов.");
            }

            Console.WriteLine($"\nПобедитель игры: {players.First().Name}");
        }

        private Player DetermineRoundWinner(List<(Player, Karta)> table)
        {
            var maxCard = table.OrderByDescending(t => t.Item2.Value).First().Item2;
            return table.First(t => t.Item2.Value == maxCard.Value).Item1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите имена игроков через запятую:");
            var playerNames = Console.ReadLine().Split(',').Select(name => name.Trim()).ToList();

            Console.WriteLine("Введите максимальное количество раундов:");
            int maxRounds = int.Parse(Console.ReadLine());

            var game = new Game(playerNames, maxRounds);
            game.Play();
        }
    }
}

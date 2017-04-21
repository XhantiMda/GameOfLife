namespace Conway
{
    using System;
    using System.Text;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            var game = new GameOfLife();
            game.StartGame(20, 100);
        }
    }

    public class GameOfLife
    {
        private bool[,] _generation;
        private Random _random = new Random();

        public void StartGame(int numberRows, int numberCols)
        {
            InitializeGrid(numberRows, numberCols);
            InsertRandomLives();
            Console.Write(Print());

            for (;;)
            {
                GenerateNewGenerations();
                Thread.Sleep(1000);
                Console.Clear();
                Console.Write(Print());
            }
        }

        private void GenerateNewGenerations()
        {
            var tempGen = new bool[_generation.GetLength(0), _generation.GetLength(1)];

            Iterator((row, col) =>
            {
                if (_generation[row, col])
                {
                    if (CountLiveNeighbours(row, col) < 2)
                    {
                        tempGen[row, col] = !_generation[row, col];
                    }

                    if (CountLiveNeighbours(row, col) == 2 || CountLiveNeighbours(row,col) == 3)
                    {
                        tempGen[row, col] = _generation[row, col];
                    }

                    if (CountLiveNeighbours(row, col) > 3)
                    {
                        tempGen[row, col] = !_generation[row, col];
                    }
                }
                else if (CountLiveNeighbours(row, col) == 3)
                {
                    tempGen[row, col] = !_generation[row, col];
                }
            });
            _generation = (bool[,])tempGen.Clone();
        }

        private string Print()
        {
            var sBuilder = new StringBuilder();

            Iterator((row, col) =>
            {
                sBuilder.Append(_generation[row, col] ? "#" : " ");
            }, () =>
             {
                 sBuilder.AppendLine();
             });

            return sBuilder.ToString();
        }

        private int CountLiveNeighbours(int x, int y)
        {
            var count = 0;

            count += x + 1 < _generation.GetLength(0) && _generation[x + 1, y] ? 1 : 0;
            count += y + 1 < _generation.GetLength(1) && _generation[x, y + 1] ? 1 : 0;
            count += x - 1 >= _generation.GetLowerBound(0) && _generation[x - 1, y] ? 1 : 0;
            count += y - 1 >= _generation.GetLowerBound(1) && _generation[x, y - 1] ? 1 : 0;
            count += x + 1 < _generation.GetLength(0) && y - 1 >= _generation.GetLowerBound(1) && _generation[x + 1, y - 1] ? 1 : 0;
            count += x - 1 >= _generation.GetLowerBound(0) && y - 1 >= _generation.GetLowerBound(1) && _generation[x - 1, y - 1] ? 1 : 0;
            count += x + 1 < _generation.GetLength(0) && y + 1 < _generation.GetLength(1) && _generation[x + 1, y + 1] ? 1 : 0;
            count += x - 1 >= _generation.GetLowerBound(0) && y + 1 < _generation.GetLowerBound(1) && _generation[x - 1, y + 1] ? 1 : 0;

            return count;
        }

        private void InsertRandomLives() => Iterator((row, col) => { _generation[row, col] = _random.Next(10) == 1; });

        private void InitializeGrid(int numberRows, int numberOfCols)
        {
            if (numberRows <= 0 || numberOfCols <= 0)
            {
                numberRows = 20;
                numberOfCols = 80;
            }

            _generation = new bool[numberRows, numberOfCols];
        }

        private void Iterator(Action<int, int> action, Action action2 = null)
        {
            for (int row = 0; row < _generation.GetLength(0); row++)
            {
                for (int col = 0; col < _generation.GetLength(1); col++)
                {
                    action.Invoke(row, col);
                }
                action2?.Invoke();
            }
        }
    }
}
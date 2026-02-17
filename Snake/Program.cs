namespace Snake
{
    public class Game{
        private readonly PlayArea _playArea;
        public PlayArea Area => _playArea;
        public Pixel Snake { get; }
        public Pixel Berry { get; }
        public int Score { get; set; }
        public bool GameOver { get; set; }
        private readonly DateTime _startTime;
        private Direction _direction;
        public Direction CurrentDirection
        {
            get => _direction;
            set => _direction = value;
        }

        public Game() {
            _playArea = new PlayArea(64, 48, ConsoleColor.Cyan);
            Snake = new Pixel(_playArea.Size.X/2, _playArea.Size.Y/2, ConsoleColor.Red);
            Berry = new Pixel(0, 0, ConsoleColor.Magenta);
            Score = 0;
            GameOver = false;
            _startTime = DateTime.Now;
            CurrentDirection = Direction.Right;
        }

        public DateTime StartTime => _startTime;

        public class PlayArea(int width, int height, ConsoleColor color, char symbol = '■')
        {
            public Vector2Int Size { get; set; } = new(width, height);
            public ConsoleColor Color { get; set; } = color;
            public char Symbol { get; set; } = symbol;
        }

        public class Pixel(int x, int y, ConsoleColor color, char symbol = '■')
        {
            public Vector2Int Position { get; set; } = new(x, y);
            public ConsoleColor Color { get; set; } = color;
            public char Symbol { get; set; } = symbol;
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public struct Vector2Int(int x, int y)
        {
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
        }
    }

    class Program
    {
        private static readonly Game Game = new Game();
        private static DateTime _lastFrameTime = DateTime.Now;

        /*
         * Refreshes the play area
         */
        static void DrawPlayArea()
        {
            Console.ForegroundColor = Game.Area.Color;
            int x = Game.Area.Size.X;
            int y = Game.Area.Size.Y;
            char areaBlock = Game.Area.Symbol;

            for (int i = 0; i < x; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < x; i++)
            {
                Console.SetCursorPosition(i, y);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < y; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < y; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write(areaBlock);
            }
        }
        /*
         * Refreshes the snake
         */
        static void DrawSnake()
        {
            Console.SetCursorPosition(Game.Snake.Position.X, Game.Snake.Position.Y);
            Console.ForegroundColor = Game.Snake.Color;
            Console.Write(Game.Snake.Symbol);
        }

        /*
         * Refreshes the berry
         */
        static void DrawBerry()
        {
            Console.SetCursorPosition(Game.Berry.Position.X, Game.Berry.Position.Y);
            Console.ForegroundColor = Game.Berry.Color;
            Console.Write(Game.Berry.Symbol);
        }

        private static void IsGameOver()
        {
            if (Game.Snake.Position.X >= Game.Area.Size.X || Game.Snake.Position.X <= 0 || Game.Snake.Position.Y >= Game.Area.Size.Y || Game.Snake.Position.Y <= 0)
            {
                Game.GameOver = true;
            }
        }


        static bool IsItNewFrameYet(int milliseconds = 500)
        {
            DateTime frameTime = DateTime.Now;
            if (frameTime.Subtract(_lastFrameTime).TotalMilliseconds >= milliseconds)
            {
                _lastFrameTime = frameTime;
                return true;
            }
            return false;
        }

        /*
         * Spawns a new berry on the play area
         */
        static void SpawnBerry(int x, int y)
        {
            Random randomNumber = new Random();

            var tempPosition = Game.Berry.Position;
            tempPosition.X = randomNumber.Next(1, x);
            tempPosition.Y = randomNumber.Next(1, y);
            Game.Berry.Position = tempPosition;
        }

        static void Main()
        {

            List<int> xBorder = new List<int>();
            List<int> yBorder = new List<int>();

            while (true)
            {
                Console.Clear();
                IsGameOver();

                DrawPlayArea();


                Console.ForegroundColor = ConsoleColor.Green;
                if (Game.Berry.Position.X == Game.Snake.Position.X && Game.Berry.Position.Y == Game.Snake.Position.Y)
                {
                    Game.Score++;
                    SpawnBerry(Game.Area.Size.X, Game.Area.Size.Y);
                }
                DrawBerry();
                for (int i = 0; i < xBorder.Count(); i++)
                {
                    Console.SetCursorPosition(xBorder[i], yBorder[i]);
                    Console.Write("■");
                    if (xBorder[i] == Game.Snake.Position.X && yBorder[i] == Game.Snake.Position.Y)
                    {
                        Game.GameOver = true;
                    }
                }
                if (Game.GameOver)
                {
                    Console.SetCursorPosition(Game.Area.Size.X / 3, (Game.Area.Size.Y / 2));
                    Console.WriteLine("Game over, Score: " + Game.Score);
                    Console.SetCursorPosition(Game.Area.Size.X / 3, (Game.Area.Size.Y / 2) + 1);
                    Console.SetCursorPosition(0, Game.Area.Size.Y + 1);
                    Console.WriteLine("Press any key to exit...");

                    Console.ReadKey();
                    break;
                }
                DrawSnake();


                Console.ForegroundColor = ConsoleColor.Cyan;
                while (!IsItNewFrameYet(120))
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo pressed = Console.ReadKey(true);
                        switch(pressed.Key){
                            case ConsoleKey.UpArrow:
                                Game.CurrentDirection = Game.Direction.Up;
                                break;
                            case ConsoleKey.DownArrow:
                                Game.CurrentDirection = Game.Direction.Down;
                                break;
                            case ConsoleKey.LeftArrow:
                                Game.CurrentDirection = Game.Direction.Left;
                                break;
                            case ConsoleKey.RightArrow:
                                Game.CurrentDirection = Game.Direction.Right;
                                break;
                        }
                    }

                    Thread.Sleep(1);
                }
                xBorder.Add(Game.Snake.Position.X);
                yBorder.Add(Game.Snake.Position.Y);
                var snakePos = Game.Snake.Position;
                switch (Game.CurrentDirection)
                {
                    case Game.Direction.Up:
                        snakePos.Y--;
                        break;
                    case Game.Direction.Down:
                        snakePos.Y++;
                        break;
                    case Game.Direction.Left:
                        snakePos.X--;
                        break;
                    case Game.Direction.Right:
                        snakePos.X++;
                        break;
                }
                Game.Snake.Position = snakePos;
                if (xBorder.Count() > Game.Score)
                {
                    xBorder.RemoveAt(0);
                    yBorder.RemoveAt(0);
                }
            }
        }


    }
}

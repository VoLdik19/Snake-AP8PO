namespace Snake
{
    public class Game{
        public PlayArea playArea;
        public Pixel snake { get; set; }
        public Pixel berry { get; set; }
        public int score;
        public bool gameOver;
        private DateTime startTime;
        public Direction direction;

        public Game() {
            this.playArea = new PlayArea(64, 48, ConsoleColor.Cyan);
            this.snake = new Pixel(playArea.size.X/2, playArea.size.Y/2, ConsoleColor.Red);
            this.berry = new Pixel(0, 0, ConsoleColor.Magenta);
            this.score = 0;
            this.gameOver = false;
            this.startTime = DateTime.Now;
            this.direction = Direction.Right;
        }

        public DateTime GetStartTime() { return startTime; }

        public class PlayArea
        {
            public Vector2Int size { get; set; }
            public ConsoleColor color { get; set; }
            public char symbol { get; set; }

            public PlayArea(int width, int height, ConsoleColor color, char symbol = '■')
            {
                this.size = new Vector2Int(width, height);
                this.color = color;
                this.symbol = symbol;
            }
        }

        public class Pixel
        {
            public Vector2Int position { get; set; }
            public ConsoleColor color { get; set; }
            public char symbol { get; set; }

            public Pixel(int x, int y, ConsoleColor color, char symbol = '■')
            {
                this.position = new Vector2Int(x, y);
                this.color = color;
                this.symbol = symbol;
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public struct Vector2Int {
            public int X { get; set; }
            public int Y { get; set; }

            public Vector2Int(int x, int y) {
                X = x;
                Y = y;
            }
        }
    }

    class Program
    {
        static Game game = new Game();
        static DateTime lastFrameTime = DateTime.Now;

        /*
         * Refreshes the play area
         */
        static void DrawPlayArea()
        {
            Console.ForegroundColor = game.playArea.color;
            int x = game.playArea.size.X;
            int y = game.playArea.size.Y;
            char areaBlock = game.playArea.symbol;

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
            Console.SetCursorPosition(game.snake.position.X, game.snake.position.Y);
            Console.ForegroundColor = game.snake.color;
            Console.Write(game.snake.symbol);
        }

        /*
         * Refreshes the berry
         */
        static void DrawBerry()
        {
            Console.SetCursorPosition(game.berry.position.X, game.berry.position.Y);
            Console.ForegroundColor = game.berry.color;
            Console.Write(game.berry.symbol);
        }

        static void IsGameOver()
        {
            if (game.snake.position.X >= game.playArea.size.X || game.snake.position.X <= 0 || game.snake.position.Y >= game.playArea.size.Y || game.snake.position.Y <= 0)
            {
                game.gameOver = true;
            }
        }


        static bool IsItNewFrameYet(int milliseconds = 500)
        {
            DateTime frameTime = DateTime.Now;
            if (frameTime.Subtract(lastFrameTime).TotalMilliseconds >= milliseconds)
            {
                lastFrameTime = frameTime;
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

            var tempPosition = game.berry.position;
            tempPosition.X = randomNumber.Next(1, x);
            tempPosition.Y = randomNumber.Next(1, y);
            game.berry.position = tempPosition;
        }

        static void Main(string[] args)
        {

            List<int> xBorder = new List<int>();
            List<int> yBorder = new List<int>();

            while (true)
            {
                Console.Clear();
                IsGameOver();

                DrawPlayArea();


                Console.ForegroundColor = ConsoleColor.Green;
                if (game.berry.position.X == game.snake.position.X && game.berry.position.Y == game.snake.position.Y)
                {
                    game.score++;
                    SpawnBerry(game.playArea.size.X, game.playArea.size.Y);
                }
                DrawBerry();
                for (int i = 0; i < xBorder.Count(); i++)
                {
                    Console.SetCursorPosition(xBorder[i], yBorder[i]);
                    Console.Write("■");
                    if (xBorder[i] == game.snake.position.X && yBorder[i] == game.snake.position.Y)
                    {
                        game.gameOver = true;
                    }
                }
                if (game.gameOver)
                {
                    Console.SetCursorPosition(game.playArea.size.X / 3, (game.playArea.size.Y / 2));
                    Console.WriteLine("Game over, Score: " + game.score);
                    Console.SetCursorPosition(game.playArea.size.X / 3, (game.playArea.size.Y / 2) + 1);
                    Console.SetCursorPosition(0, game.playArea.size.Y + 1);
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
                                game.direction = Game.Direction.Up;
                                break;
                            case ConsoleKey.DownArrow:
                                game.direction = Game.Direction.Down;
                                break;
                            case ConsoleKey.LeftArrow:
                                game.direction = Game.Direction.Left;
                                break;
                            case ConsoleKey.RightArrow:
                                game.direction = Game.Direction.Right;
                                break;
                        }
                    }

                    Thread.Sleep(1);
                }
                xBorder.Add(game.snake.position.X);
                yBorder.Add(game.snake.position.Y);
                var snakePos = game.snake.position;
                switch (game.direction)
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
                game.snake.position = snakePos;
                if (xBorder.Count() > game.score)
                {
                    xBorder.RemoveAt(0);
                    yBorder.RemoveAt(0);
                }
            }
        }


    }
}

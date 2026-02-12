using System.Numerics;

namespace Snake
{
    public class Game{
        public int score;
        public bool gameOver;
        public Pixel snake;
        public Pixel berry;
        public int score;
        public bool gameOver;
        public Vector2 playArea;
        DateTime startTime;

        public Game() {
            score = 0;
            gameOver = false;
            playArea = new Vector2(64, 48);
            startTime = DateTime.Now;
        }

        public DateTime GetStartTime() { return startTime; }

        public class Pixel
        {
            public Vector2 position { get; set; }
            public ConsoleColor color { get; set; }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }

    class Program
    {

        static Pixel berry = new Pixel();


        /*
         * Refreshes the play area
         */
        static void DrawPlayArea(int x, int y, ConsoleColor color, char areaBlock)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < x; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < x; i++)
            {
                Console.SetCursorPosition(i, y - 1);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < y; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(areaBlock);
            }
            for (int i = 0; i < y; i++)
            {
                Console.SetCursorPosition(x - 1, i);
                Console.Write(areaBlock);
            }
        }
        /*
         * Refreshes the snake
         */
        static void DrawSnake()
        {

        }

        /*
         * Refreshes the berry
         */
        static void DrawBerry()
        {
            Console.SetCursorPosition(berry.x, berry.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("B");
        }

        /*
         * Spawns a new berry on the play area
         */
        static void SpawnBerry(int x, int y)
        {
            Random randomNumber = new Random();

            berry.x = randomNumber.Next(0, x);
            berry.y = randomNumber.Next(0, y);


            berry.x = randomNumber.Next(1, x);
            berry.y = randomNumber.Next(1, y);
        }

        static void Main(string[] args)
        {

            Console.WindowHeight = 48;
            Console.WindowWidth = 48;
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int score = 5;
            int gameOver = 0;
            Pixel snakeHead = new Pixel();
            snakeHead.x = screenWidth / 2;
            snakeHead.y = screenHeight / 2;
            snakeHead.color = ConsoleColor.Red;
            string currentStep = "RIGHT";
            List<int> xBorder = new List<int>();
            List<int> yBorder = new List<int>();
            
            DateTime startTime = DateTime.Now;
            DateTime frameTime = DateTime.Now;
            string buttonPressed = "no";
            while (true)
            {
                Console.Clear();
                if (snakeHead.x == screenWidth - 1 || snakeHead.x == 0 || snakeHead.y == screenHeight - 1 || snakeHead.y == 0)
                {
                    gameOver = 1;
                }

                DrawPlayArea(screenWidth, screenHeight, ConsoleColor.Green, '■');


                Console.ForegroundColor = ConsoleColor.Green;
                if (berry.x == snakeHead.x && berry.y == snakeHead.y)
                {
                    score++;
                    SpawnBerry(screenWidth, screenHeight);
                }
                DrawBerry();
                for (int i = 0; i < xBorder.Count(); i++)
                {
                    Console.SetCursorPosition(xBorder[i], yBorder[i]);
                    Console.Write("■");
                    if (xBorder[i] == snakeHead.x && yBorder[i] == snakeHead.y)
                    {
                        gameOver = 1;
                    }
                }
                if (gameOver == 1)
                {
                    break;
                }
                Console.SetCursorPosition(snakeHead.x, snakeHead.y);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("H"); // hlava
 
                Console.ForegroundColor = ConsoleColor.Cyan;
                startTime = DateTime.Now;
                buttonPressed = "no";
                while (true)
                {
                    frameTime = DateTime.Now;
                    if (frameTime.Subtract(startTime).TotalMilliseconds > 500) { break; }
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo pressed = Console.ReadKey(true);
                        if (pressed.Key.Equals(ConsoleKey.UpArrow) && currentStep != "DOWN" && buttonPressed == "no")
                        {
                            currentStep = "UP";
                            buttonPressed = "yes";
                        }
                        if (pressed.Key.Equals(ConsoleKey.DownArrow) && currentStep != "UP" && buttonPressed == "no")
                        {
                            currentStep = "DOWN";
                            buttonPressed = "yes";
                        }
                        if (pressed.Key.Equals(ConsoleKey.LeftArrow) && currentStep != "RIGHT" && buttonPressed == "no")
                        {
                            currentStep = "LEFT";
                            buttonPressed = "yes";
                        }
                        if (pressed.Key.Equals(ConsoleKey.RightArrow) && currentStep != "LEFT" && buttonPressed == "no")
                        {
                            currentStep = "RIGHT";
                            buttonPressed = "yes";
                        }
                    }
                }
                xBorder.Add(snakeHead.x);
                yBorder.Add(snakeHead.y);
                switch (currentStep)
                {
                    case "UP":
                        snakeHead.y--;
                        break;
                    case "DOWN":
                        snakeHead.y++;
                        break;
                    case "LEFT":
                        snakeHead.x--;
                        break;
                    case "RIGHT":
                        snakeHead.x++;
                        break;
                }
                if (xBorder.Count() > score)
                {
                    xBorder.RemoveAt(0);
                    yBorder.RemoveAt(0);
                }
            }
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
        }

        class Pixel
        {
            public int x { get; set; }
            public int y { get; set; }
            public ConsoleColor color; }
        }
    }

//¦

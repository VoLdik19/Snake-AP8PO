namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            Random randomNumber = new Random();
            int score = 5;
            int gameOver = 0;
            Pixel snakeHead = new Pixel();
            snakeHead.x = screenWidth / 2;
            snakeHead.y = screenHeight / 2;
            snakeHead.color = ConsoleColor.Red;
            string currentStep = "RIGHT";
            List<int> xBorder = new List<int>();
            List<int> yBorder = new List<int>();
            Pixel berry = new Pixel();
            berry.x = randomNumber.Next(0, screenWidth);
            berry.y = randomNumber.Next(0, screenHeight);
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
                for (int i = 0; i < screenWidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                }
                for (int i = 0; i < screenWidth; i++)
                {
                    Console.SetCursorPosition(i, screenHeight - 1);
                    Console.Write("■");
                }
                for (int i = 0; i < screenHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                }
                for (int i = 0; i < screenHeight; i++)
                {
                    Console.SetCursorPosition(screenWidth - 1, i);
                    Console.Write("■");
                }
                Console.ForegroundColor = ConsoleColor.Green;
                if (berry.x == snakeHead.x && berry.y == snakeHead.y)
                {
                    score++;
                    berry.x = randomNumber.Next(1, screenWidth - 2);
                    berry.y = randomNumber.Next(1, screenHeight - 2);
                }
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
                Console.ForegroundColor = snakeHead.color;
                Console.Write("■");
                Console.SetCursorPosition(berry.x, berry.y);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");
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

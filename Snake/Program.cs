namespace Snake
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var settings = new GameSettings(width: 32, height: 16, frameDuration: TimeSpan.FromMilliseconds(500), initialBodyLength: 5);
            var game = new SnakeGame(settings);
            game.Run();
        }
    }

    internal class GameSettings
    {
        public GameSettings(int width, int height, TimeSpan frameDuration, int initialBodyLength)
        {
            Width = width;
            Height = height;
            FrameDuration = frameDuration;
            InitialBodyLength = initialBodyLength;
        }

        public int Width { get; }
        public int Height { get; }
        public TimeSpan FrameDuration { get; }
        public int InitialBodyLength { get; }
    }

    internal class SnakeGame
    {
        private readonly GameBoard board;
        private readonly Snake snake;
        private readonly Berry berry;
        private readonly Renderer renderer;
        private readonly InputReader inputReader;
        private int score;

        public SnakeGame(GameSettings settings)
        {
            Console.WindowHeight = settings.Height;
            Console.WindowWidth = settings.Width;

            board = new GameBoard(settings.Width, settings.Height);
            snake = new Snake(board.Center, Direction.Right, settings.InitialBodyLength);
            berry = new Berry(board, snake);
            renderer = new Renderer();
            inputReader = new InputReader(settings.FrameDuration);
            score = settings.InitialBodyLength;
        }

        public void Run()
        {
            while (true)
            {
                bool hasCollision = board.IsWall(snake.HeadPosition) || snake.IsSelfCollision();

                renderer.Draw(board, snake, berry);

                if (hasCollision)
                {
                    break;
                }

                if (snake.IsOnPosition(berry.Position))
                {
                    score++;
                    snake.MaxBodyLength = score;
                    berry.Respawn(board, snake);
                }

                var nextDirection = inputReader.ReadDirection(snake.CurrentDirection);
                snake.ChangeDirection(nextDirection);
                snake.Move();
            }

            renderer.DrawGameOver(score, board);
        }
    }

    internal class GameBoard
    {
        public GameBoard(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public Position Center => new Position(Width / 2, Height / 2);

        public bool IsWall(Position position)
        {
            return position.X == 0 || position.X == Width - 1 || position.Y == 0 || position.Y == Height - 1;
        }
    }

    internal class Renderer
    {
        public void Draw(GameBoard board, Snake snake, Berry berry)
        {
            Console.Clear();
            DrawBorder(board);
            DrawSnake(snake);
            DrawBerry(berry);
        }

        public void DrawGameOver(int score, GameBoard board)
        {
            Console.SetCursorPosition(board.Width / 5, board.Height / 2);
            Console.WriteLine($"Game over, Score: {score - 5}");
            Console.SetCursorPosition(board.Width / 5, board.Height / 2 + 1);
        }

        private static void DrawBorder(GameBoard board)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int x = 0; x < board.Width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("■");
                Console.SetCursorPosition(x, board.Height - 1);
                Console.Write("■");
            }

            for (int y = 0; y < board.Height; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("■");
                Console.SetCursorPosition(board.Width - 1, y);
                Console.Write("■");
            }
        }

        private static void DrawSnake(Snake snake)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var segment in snake.BodySegments)
            {
                Console.SetCursorPosition(segment.X, segment.Y);
                Console.Write("■");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(snake.HeadPosition.X, snake.HeadPosition.Y);
            Console.Write("■");
        }

        private static void DrawBerry(Berry berry)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(berry.Position.X, berry.Position.Y);
            Console.Write("■");
        }
    }

    internal class Snake
    {
        private readonly Queue<Position> bodySegments = new Queue<Position>();

        public Snake(Position start, Direction direction, int maxBodyLength)
        {
            HeadPosition = start;
            CurrentDirection = direction;
            MaxBodyLength = maxBodyLength;
        }

        public Position HeadPosition { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public int MaxBodyLength { get; set; }
        public IEnumerable<Position> BodySegments => bodySegments;

        public void ChangeDirection(Direction newDirection)
        {
            if (IsOppositeDirection(newDirection))
            {
                return;
            }

            CurrentDirection = newDirection;
        }

        public void Move()
        {
            bodySegments.Enqueue(HeadPosition);
            HeadPosition = HeadPosition.Offset(CurrentDirection);

            while (bodySegments.Count > MaxBodyLength)
            {
                bodySegments.Dequeue();
            }
        }

        public bool IsSelfCollision()
        {
            return bodySegments.Any(segment => segment.Equals(HeadPosition));
        }

        public bool IsOnPosition(Position position)
        {
            return HeadPosition.Equals(position);
        }

        public bool Occupies(Position position)
        {
            return HeadPosition.Equals(position) || bodySegments.Any(segment => segment.Equals(position));
        }

        private bool IsOppositeDirection(Direction newDirection)
        {
            return (CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
                   (CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
                   (CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
                   (CurrentDirection == Direction.Right && newDirection == Direction.Left);
        }
    }

    internal class Berry
    {
        private readonly Random random = new Random();

        public Berry(GameBoard board, Snake snake)
        {
            Position = CreateNewPosition(board, snake);
        }

        public Position Position { get; private set; }

        public void Respawn(GameBoard board, Snake snake)
        {
            Position = CreateNewPosition(board, snake);
        }

        private Position CreateNewPosition(GameBoard board, Snake snake)
        {
            Position candidate;
            do
            {
                candidate = new Position(random.Next(1, board.Width - 1), random.Next(1, board.Height - 1));
            } while (snake.Occupies(candidate));

            return candidate;
        }
    }

    internal class InputReader
    {
        private readonly TimeSpan frameDuration;

        public InputReader(TimeSpan frameDuration)
        {
            this.frameDuration = frameDuration;
        }

        public Direction ReadDirection(Direction currentDirection)
        {
            var frameStart = DateTime.Now;
            Direction chosenDirection = currentDirection;

            while (DateTime.Now - frameStart <= frameDuration)
            {
                if (!Console.KeyAvailable)
                {
                    continue;
                }

                var keyInfo = Console.ReadKey(true);
                if (TryMapKeyToDirection(keyInfo.Key, out var requestedDirection) && requestedDirection != currentDirection)
                {
                    if (!IsOpposite(currentDirection, requestedDirection))
                    {
                        chosenDirection = requestedDirection;
                    }

                    break;
                }
            }

            return chosenDirection;
        }

        private static bool TryMapKeyToDirection(ConsoleKey key, out Direction direction)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    direction = Direction.Up;
                    return true;
                case ConsoleKey.DownArrow:
                    direction = Direction.Down;
                    return true;
                case ConsoleKey.LeftArrow:
                    direction = Direction.Left;
                    return true;
                case ConsoleKey.RightArrow:
                    direction = Direction.Right;
                    return true;
                default:
                    direction = Direction.Right;
                    return false;
            }
        }

        private static bool IsOpposite(Direction currentDirection, Direction requestedDirection)
        {
            return (currentDirection == Direction.Up && requestedDirection == Direction.Down) ||
                   (currentDirection == Direction.Down && requestedDirection == Direction.Up) ||
                   (currentDirection == Direction.Left && requestedDirection == Direction.Right) ||
                   (currentDirection == Direction.Right && requestedDirection == Direction.Left);
        }
    }

    internal readonly struct Position : IEquatable<Position>
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public Position Offset(Direction direction)
        {
            return direction switch
            {
                Direction.Up => new Position(X, Y - 1),
                Direction.Down => new Position(X, Y + 1),
                Direction.Left => new Position(X - 1, Y),
                Direction.Right => new Position(X + 1, Y),
                _ => this
            };
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}

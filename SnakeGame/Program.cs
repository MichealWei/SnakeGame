using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        public struct Point
        {
            public int x;
            public int y;
        }

        public struct gameState
        {
            public int _height;
            public int _width;
            public int _currentRound;
            public int _score;
            public bool _gameOver;
            public List<Point> snake;
            public Point _food;

        }

        enum eDirecton {LEFT = 0, RIGHT, DOWN, UP, STOP };
        const string headElement = @"> < ^ v";
        static eDirecton dir;
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.SetWindowSize(150, 80);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            gameState lastState =  Setup();
            PrintGameView(lastState);
            while (!lastState._gameOver)
            {
                Input().Map(direct => new Tuple<gameState, eDirecton>(lastState, direct)).Bind(updateGameState)
                    .Match(
                        Left: err => { Console.WriteLine(err.Message + "\nPlease retry\n"); },
                        Right: newState =>
                        {
                            lastState = newState;
                            PrintGameView(lastState);

                        }
                    );
            }

            //Input().Match(Left: _ => { }, Right: direct => dir = direct);
            //var tokenSource = new CancellationTokenSource();
            //var token = tokenSource.Token;
            //Task task = Task.Run(() => {
            //    while (true)
            //    {
            //        Input().Match(Left: _ => { }, Right: direct => dir = direct);
            //    }
            //}, token);
            //while (!lastState._gameOver)
            //{
            //    AutoInput().Map(direct => new Tuple<gameState, eDirecton>(lastState, direct)).Bind(updateGameState)
            //        .Match(
            //            Left: err => { Console.WriteLine(err.Message + "\nPlease retry\n"); },
            //            Right: newState => {
            //                lastState = newState;
            //                PrintGameView(lastState);

            //            }
            //        );
            //    Thread.Sleep(300);
            //}
            //tokenSource.Cancel();


            Console.WriteLine("Game Over, Thanks! Press enter key to exit!");
            Console.Read();

        }

        private static gameState Setup()
        {
            gameState initState = new gameState();
            bool validInput = false;
            dir = eDirecton.LEFT;
            
           
            string cmd = System.String.Empty;
            Console.Write("Welcome, player! Please input <width_number, height_number> pair to pick the board size:");
            while(!validInput)
            {
                cmd = getBoardSize();
                Right(cmd).Bind(ParseCmd).Match(
                    Left: err => { Console.WriteLine(err.Message + "\nPlease retry\n"); },
                    Right: size => { 
                        initState._height = size.Item2;
                        initState._width = size.Item1;
                        validInput = true;
                    }
                    );
            }

            Point head = new Point { x = initState._width / 2, y = initState._height / 2 };
            initState.snake = new List<Point>();
            initState.snake.Add(head);
            initState._currentRound = 0;
            initState._score = 0;
            int foodX = random.Next() % initState._width;
            int foodY = random.Next() % initState._height;
            initState._food = new Point { x = foodX, y = foodY };
             
            return initState;
        }

        private static Either<Error, eDirecton>  Input()
        {
            eDirecton dir = eDirecton.DOWN;
            var key = Console.ReadKey().Key.ToString();
            switch (key)
            {
                case "A":
                    dir = eDirecton.LEFT;
                    break;
                case "D":
                    dir = eDirecton.RIGHT;
                    break;
                case "W":
                    dir = eDirecton.UP;
                    break;
                case "S":
                    dir = eDirecton.DOWN;
                    break;
                default:
                    return Error("Bad format--please input direction using A,D,W,S keys");
            }
            return dir;
        }
        //private static Either<Error, eDirecton> AutoInput()
        //{
        //    return dir;
        //}



        private static string GetHeadSymbol()
        {
            string[] tempHeadStr = headElement.Split(' ');
            return tempHeadStr[(int)dir];
        }

        private static void PrintGameView(gameState currentstate)
        {
            int height = currentstate._height, width = currentstate._width;
            int theOtherEnd = width - 1;
            int headX = currentstate.snake[0].x, headY = currentstate.snake[0].y;
            int snakeLength = currentstate.snake.Count();
            int[] tailX = new int[height], tailY = new int[height];
            int foodX = currentstate._food.x, foodY = currentstate._food.y;
            Console.Clear();
            Console.WriteLine("==============Game View:================================");
            for (int i = 0; i < width + 2; ++i)
            {
                Console.Write("#");
            }

            Console.WriteLine();
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (j == 0) Console.Write("#");
                    if (i == headY && j == headX)
                        Console.Write(GetHeadSymbol());
                    else if (i == foodY && j == foodX)
                        Console.Write("$");
                    else
                    {
                        bool printed = false;
                        for (int k = 1; k < snakeLength; k++)
                        {
                            if (currentstate.snake[k].x == j && currentstate.snake[k].y == i)
                            {
                                Console.Write("o");
                                printed = true;
                            }
                        }
                        if (!printed)
                            Console.Write(" ");
                    }
                    if (j == theOtherEnd) Console.Write("#");
                    Console.CursorVisible = false;

                }
                Console.WriteLine();
            }

            for (int i = 0; i < width + 2; ++i)
            {
                Console.Write("#");
            }
            Console.WriteLine("\n==============Game View:================================");


            Console.WriteLine("\n Score: {0}", currentstate._score);
            Console.WriteLine("\n Snake length: {0}", currentstate.snake.Count());
            Console.WriteLine("\n Current round: {0}", currentstate._currentRound);
            Console.WriteLine("use key A for left, key D for right, key W for up, key s for down");

        }

        private static string getBoardSize()
        {
            return Console.ReadLine().Trim().ToLower();
        }

        private static Either<Error, Tuple<int, int>> ParseCmd(string cmd)
        {
            if (cmd.IndexOf(',') < 0) return Error("Bad format--not an ints pair, no comma");

            var size = cmd.Split(',');
            if (size.Length != 2) return Error("Bad format--please input <width_number, height_number> pair");

            int width, height;
            if (!int.TryParse(size[0], out width)) return Error("Bad format--width number");
            if (!int.TryParse(size[1], out height)) return Error("Bad format--height number");

            if (width < 10 || width > 100) return Error("Bad input--width number must be within range[10~100]");

            if (height < 10 || width > 100) return Error("Bad input--height number must be within range[10~100]");


            return new Tuple<int, int>(width, height);
        }

        private static Either<Error, gameState> updateGameState(Tuple<gameState, eDirecton> lastStateAndDir)
        {
            gameState lastState = lastStateAndDir.Item1;
            Point snakeHead = lastState.snake[0];
            int headX = snakeHead.x, headY = snakeHead.y;
            int width = lastState._width, height = lastState._height;
            var new_dir = lastStateAndDir.Item2;
            switch (new_dir)
            {
                case eDirecton.LEFT:
                    headX--;
                    break;
                case eDirecton.RIGHT:
                    headX++;
                    break;
                case eDirecton.UP:
                    headY--;
                    break;
                case eDirecton.DOWN:
                    headY++;
                    break;
                default:
                    break;

            }
            if (headX == width || headX == 0 || headY == height || headY == 0)
            {
                lastState._gameOver = true;
                return lastState;
            }

            var tailee = lastState.snake;
            if (tailee.Count() > 1)              // prevent move head to tail direction
            {
                if (tailee[1].x == headX && tailee[1].y == headY)
                    return lastState;
            }
            for (var i = 1; i < tailee.Count(); i++)
            {
                if(headX == tailee[i].x && headY == tailee[i].y && new_dir != dir)
                {
                    lastState._gameOver = true;
                    return lastState;
                }
            }
             
            if (headX == lastState._food.x && headY == lastState._food.y)
            {
                lastState._score++;
                lastState._food.x = random.Next() % width;
                lastState._food.y = random.Next() % height;
                Point newPoint = new Point { x = headX, y = headY };
                List<Point> newList = new List<Point>();
                newList.Add(newPoint);
                foreach (var point in tailee)
                    newList.Add(point);
                lastState.snake = newList;
            }
            else
            {
                lastState.snake.Insert(0, new Point { x = headX, y = headY });
                lastState.snake.RemoveAt(lastState.snake.Count - 1);
            }
            lastState._currentRound++;
            dir = new_dir;
            return lastState;
        }

    }
}

namespace flappyBird_Console
{
    internal class Program
    {
        static bool jumpMode = true;
        static bool easyMode = true;
        static void Main(string[] args)
        {
            while (true)
            {
                EnterSettings();
                Console.Clear();

                Game game = new Game(jumpMode, easyMode);
                int score = game.Start();

                Console.WriteLine($"score: {score}");
                Console.WriteLine("Would  you like to play again? (y/n)");
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.N)
                {
                    return;
                }
                Console.Clear();

            }
        }
        static void EnterSettings()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("welcome!");
                Console.Write($"press M to switch modes. currently: ");
                Console.WriteLine(jumpMode ? "jump mode" : "swing mode");

                Console.Write($"press D to switch difficulties. currently: ");
                Console.WriteLine(easyMode ? "easy" : "hard");

                Console.WriteLine("press any other key to play");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.M)
                {
                    jumpMode = !jumpMode;
                }
                else if (key == ConsoleKey.D)
                {
                    easyMode = !easyMode;
                }
                else break;

            }


        }
    }
    class Game
    {
        public double y = Console.WindowHeight/2; // player location
        public bool jumpMode;
        public bool easy;
        public Game(bool jumpMode, bool easy)
        {
            this.jumpMode = jumpMode;
            this.easy = easy;
        }
        public int Start()
        {
            Player pyr = new Player(this);


            void Print()
            {
                Console.SetCursorPosition(Player.X, (int)y);
                Console.Write("*");
            }
            void Delete()
            {
                Console.SetCursorPosition(Player.X, (int)y);
                Console.Write(" ");
            }
            Print();
            Console.ReadKey(true);

            List<Pipe> pipes = new List<Pipe>();
            int pipeCounter = 20;
            int pipeNumber = 1;

            pyr.StartJump();
            while (true)
            {
                Thread.Sleep(25);
                Delete();

                bool lost = false;
                bool pressed = false;
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    pressed = true;
                }
                y = pyr.GetNewHeight(pressed);


                if (y >= (Console.WindowHeight - 1) || y < 0)
                {
                    lost = true;
                }
                Console.SetCursorPosition(0, 0);
                //Console.Write(Math.Round(v, 3).ToString() + " , " + Math.Round(y, 3).ToString());

                Print();

                if (pipeCounter % 1 == 0)
                {
                    foreach (Pipe p in pipes)
                    {
                        if (p.x <= 1)
                        {
                            p.Delete();
                            continue;
                        }
                        p.Move();
                    }

                }
                pipes.RemoveAll(pipe => pipe.x <= 0);

                if (pipeCounter == 60)
                {
                    pipes.Add(new Pipe((int)(Console.WindowWidth / 1.2 ), this, pipeNumber));
                    pipeCounter = 0;
                    pipeNumber++;
                }
                // check collision

                foreach (Pipe p in pipes)
                {
                    if (p.GetCollision())
                    {
                        lost = true;
                        break;
                    }
                }
                if (lost)
                {
                    Console.Clear();
                    Console.WriteLine("You lost!!!");
                    break;
                }
                pipeCounter++;
            }
            return pipeNumber;
        }

    }
    class Player // only physics, no printing.
    {
        public const int X = 15;
        const double Gravity = 0.18;
        const double JumpHeight = -1.8;
        bool jumpMode;
        public bool easy;
        public double y = Console.WindowHeight / 2;
        double v = 0;
        double a;
        Game game;
        public Player(Game game)
        {
            this.game = game;
            jumpMode = game.jumpMode;
            easy = game.easy;
            a = jumpMode ? -Gravity : -Gravity * 0.8;
        }
        public double GetNewHeight(bool pressed)
        {
            ProcessPhysics(jumpMode, pressed);
            return y;
        }
        void ProcessPhysics(bool jumpmode, bool pressed)
        {
            if (pressed)
            {
                if (jumpmode)
                {
                    v = -JumpHeight;

                }
                else
                {
                    a = -a;
                    v = 0.7 * v;
                }
            }

            v += a;
            y -= v;
        }
        public void StartJump()
        {
            ProcessPhysics(true, true); // a little jump as the start of the game to make it more readable

        }


    }
    class Pipe
    {
        static Random r = new Random();
        public int x;
        int gapTop;
        int gapBottom;

        int gapSize;
        string icon;

        Game game;

        public Pipe(int x, Game game, int numForIcon)
        {
            this.game = game;
            gapSize = this.game.easy ? 14 : 8;
            this.x = x;
            (int minRange, int maxRange) = GetWindowBorders();
            int gapY = r.Next(minRange, maxRange + 1);
            gapTop = gapY + gapSize / 2;
            gapBottom = gapY - gapSize / 2;

            icon = $"{numForIcon}{numForIcon}{numForIcon}";
            Print();

        }
        (int, int) GetWindowBorders()
        {
            int consoleMax = Console.WindowHeight;
            int consoleMin = 0;

            int min = consoleMin + (int)Math.Ceiling(gapSize / 2.0);
            int max = consoleMax - (int)Math.Ceiling(gapSize / 2.0);
            return (min,  max);
        }
        void Print()
        {
            for (int i = 0; i < Console.WindowHeight;  i++)
            {
                Console.SetCursorPosition(x, i);
                if (i > gapBottom && i < gapTop) // on the gap, which is already empty
                {
                    continue;
                }
                else
                {
                    Console.Write(icon);
                }

            }
        }
        public void Delete()
        {
            string deleteString = string.Empty;
            for (int j = 0; j < (icon.Length); j++)
            {
                deleteString += " ";
            }
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.SetCursorPosition(x, i);
                if (i > gapBottom && i < gapTop) // on the gap, which is already empty
                {
                    continue;
                }
                else
                {
                    Console.Write(deleteString);

                }

            }

        }
        public void Move()
        {
            x--;
            string updateString = icon;
            updateString += " ";

            string deleteString = string.Empty;
            for (int j = 0; j < (icon.Length); j++)
            {
                deleteString += " ";
            }
            deleteString += " ";

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(x, i);
                if (!(i > gapBottom && i < gapTop)) // is not on the gap
                {
                    Console.Write(updateString);
                }

            }
        }
        public bool GetCollision()
        {
            if ((x <= Player.X && x >= (Player.X - icon.Length + 1)) && (game.y <= (gapBottom + 1) || game.y >= (gapTop - 1)))
            {
                return true;
            }
            return false;
        }
    }
}

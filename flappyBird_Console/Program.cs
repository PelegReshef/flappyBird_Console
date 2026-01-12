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

                Game game = new Game(jumpMode);
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
                EnterSettings();
            }
            else if (key == ConsoleKey.D)
            {
                easyMode = !easyMode;
                EnterSettings();
            }
            
            
        }
    }
    class Game
    {
        public const int X = 15;
        public double y = 15;
        const double Gravity = 0.18;
        const double JumpHeight = -1.8;
        bool jumpMode;
        public Game(bool jumpmode)
        {
            this.jumpMode = jumpmode;
        }
        public int Start()
        {


            double v = 0;
            double a = jumpMode ? -Gravity : -Gravity*0.8;

            void Print()
            {
                Console.SetCursorPosition(X, (int)y);
                Console.Write("*");
            }
            void Delete()
            {
                Console.SetCursorPosition(X, (int)y);
                Console.Write(" ");
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
            Print();
            Console.ReadKey(true);
            Thread.Sleep(1000);
            List<Pipe> pipes = new List<Pipe>();
            int pipeCounter = 20;
            int pipeNumber = 1;
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
                ProcessPhysics(jumpMode, pressed);

                if (y >= Console.WindowHeight || y < 0)
                {
                    lost = true;
                }
                Console.SetCursorPosition(0, 0);
                Console.Write(Math.Round(v, 3).ToString() + " , " + Math.Round(y, 3).ToString());

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
                    pipes.Add(new Pipe(Console.WindowWidth / 2 - 1, this, pipeNumber));
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
    class Pipe
    {
        static Random r = new Random();
        public int x;
        int gapTop;
        int gapBottom;

        const int GapSize = 10;
        string icon;

        Game game;

        public Pipe(int x, Game game, int numForIcon)
        {
            this.x = x;
            (int minRange, int maxRange) = GetWindowBorders();
            int gapY = r.Next(minRange, maxRange + 1);
            gapTop = gapY + GapSize / 2;
            gapBottom = gapY - GapSize / 2;

            this.game = game;
            icon = $"{numForIcon}{numForIcon}{numForIcon}";

        }
        (int, int) GetWindowBorders()
        {
            int consoleMax = Console.WindowHeight;
            int consoleMin = 0;

            int min = consoleMin + (int)Math.Ceiling(GapSize / 2.0);
            int max = consoleMax - (int)Math.Ceiling(GapSize / 2.0);
            return (min,  max);
        }
        void Print()
        {
            for (int i = 0; i < Console.WindowHeight;  i++)
            {
                Console.SetCursorPosition(x, i);
                if (i > gapBottom && i < gapTop)
                {
                    Console.Write("   ");
                }
                else
                {
                    Console.Write(icon);
                }

            }
        }
        public void Delete()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(x, i);
                string deleteString = string.Empty;
                for (int j = 0; j < (icon.Length); j++)
                {
                    deleteString += " ";
                }
                Console.Write(deleteString);
            }

        }
        public void Move()
        {
            Delete();
            x--;
            Print();
        }
        public bool GetCollision()
        {
            if ((x <= Game.X && x >= (Game.X - icon.Length + 1)) && (game.y <= (gapBottom + 1) || game.y >= (gapTop - 1)))
            {
                return true;
            }
            return false;
        }
    }
}

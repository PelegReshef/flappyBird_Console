namespace flappyBird_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Game game = new Game();
                game.Start();

                Console.WriteLine("Would  yhou like to play again? (y/n)");
                var key = Console.ReadKey().Key;

                if (key != ConsoleKey.Y)
                {
                    return;
                }
                Console.Clear();

            }
        }
    }
    class Game
    {
        public const int X = 15;
        public double y = 15;
        const double Gravity = 0.18;
        const double JumpHeight = -1.8;
        public void Start()
        {


            double v = 0;
            const double a = -Gravity;

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
            Print();
            Console.ReadKey(true);
            Thread.Sleep(1000);
            List<Pipe> pipes = new List<Pipe>();
            int pipeCounter = 20;
            while (true)
            {
                Thread.Sleep(25);
                Delete();

                bool lost = false;
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    v = -JumpHeight;
                }

                v += a;
                y -= v;

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
                    pipes.Add(new Pipe(Console.WindowWidth / 2 - 1, this));
                    pipeCounter = 0;
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
        }

    }
    class Pipe
    {
        Random r = new Random();
        public int x;
        int y;
        int gapTop;
        int gapBottom;

        const int GapSize = 14;
        const string Icon = "###";

        Game game;

        public Pipe(int x, Game game)
        {
            this.x = x;
            (int minRange, int maxRange) = GetWindowBorders();
            int gapY = r.Next(minRange, maxRange + 1);
            gapTop = gapY + GapSize / 2;
            gapBottom = gapY - GapSize / 2;

            this.game = game;

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
                    Console.Write(Icon);
                }

            }
        }
        public void Delete()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("   ");
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
            if (x == Game.X && (game.y <= (gapBottom + 1) || game.y >= (gapTop - 1)))
            {
                return true;
            }
            return false;
        }
    }
}

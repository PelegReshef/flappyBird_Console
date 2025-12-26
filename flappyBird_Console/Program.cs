namespace flappyBird_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const double Gravity = 0.18;
            const double JumpHeight = -1.8;


            const int X = 15;
            double y = 15;
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
                Thread.Sleep(30);
                Delete();

                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    v = -JumpHeight;
                }

                v += a;
                y -= v;

                if (y  < 0)
                {
                    y = 0;
                }
                if (y >= Console.WindowHeight)
                {
                    y = Console.WindowHeight - 1;
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
                    pipes.Add(new Pipe(Console.WindowWidth /2 - 1));
                    pipeCounter = 0;
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

        public Pipe(int x)
        {
            this.x = x;
            (int minRange, int maxRange) = GetWindowBorders();
            int gapY = r.Next(minRange, maxRange + 1);
            gapTop = gapY + GapSize / 2;
            gapBottom = gapY - GapSize / 2;


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
    }
}

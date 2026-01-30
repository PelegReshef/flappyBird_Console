using System.Drawing;
using System.Text;
using System.Diagnostics;

namespace flappyBird_Console
{
    internal class Program
    {
        public static Random r = new Random();

        static bool jumpMode = true;
        static bool easyMode = true;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.ASCII;
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
    static class Buffer
    {
        static char[,] buffer = new char[Console.WindowWidth, Console.WindowHeight];
        static HashSet<int> changes = new HashSet<int>();
        static Point cursor = new Point(0, 0);
        public static void SetCursorPosition(int x, int y)
        {
            cursor = new Point(x, y);
        }
        public static void Write(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                buffer[cursor.X + i, cursor.Y] = line[i];
            }
            cursor.X += line.Length;
            changes.Add(cursor.Y);
        }
        public static void Clear()
        {
            for (int y = 0; y < Console.WindowHeight; y++)
            {
                for (int x = 0; x < Console.WindowWidth; x++)
                {
                    buffer[x, y] = ' ';
                }
            }
            changes.Clear();

        }
        public static void Print()
        {
            char[] line = new char[Console.WindowWidth];
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y< Console.WindowHeight; y++)
            {
                if (!changes.Contains(y))
                {
                    continue;
                }
                for (int x = 0; x < Console.WindowWidth; x++)
                {
                    line[x] = buffer[x, y];
                }
                Console.SetCursorPosition(0, y);

                Console.Write(line);
            }
            changes.Clear();
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
            Buffer.Clear();
            var stopwatch = Stopwatch.StartNew();
            double lastTime = stopwatch.Elapsed.TotalSeconds;


            void Print()
            {
                Buffer.SetCursorPosition(Player.X, (int)y);
                Buffer.Write("*");
            }
            void Delete()
            {
                Buffer.SetCursorPosition(Player.X, (int)y);
                Buffer.Write(" ");
            }

            Print();
            Console.ReadKey(true);

            List<Pipe> pipes = new List<Pipe>();
            List<GPortal> gPortals = new List<GPortal>();
            int gPortalCounter = 0; // used to spawn grvity portals 
            int pipeCounter = 150; // used to spawn pipes
            int pipeAmount = 1; // used for score

            pyr.StartJump();
            while (true)
            {
                double now = stopwatch.Elapsed.TotalSeconds;
                double dt = now - lastTime;
                dt = Math.Min(dt, 0.05);
                lastTime = now;


                bool lost = false;
                bool pressed = false;
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    pressed = true;
                }
                Delete();

                y = pyr.GetNewHeight(pressed, dt);
                


                if (y >= (Console.WindowHeight) || y < 0)
                {
                    lost = true;
                    goto loss;
                }
                //Console.SetCursorPosition(0, 0);
                //Console.Write(Math.Round(v, 3).ToString() + " , " + Math.Round(y, 3).ToString());

                Print();
                if (pipeCounter % 3 == 0)
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
                    foreach (GPortal gp in gPortals)
                    {
                        if (gp.x <= 1)
                        {
                            gp.Delete();
                            continue;
                        }
                        gp.Move();
                    }

                }



                pipes.RemoveAll(pipe => pipe.x <= 0);
                gPortals.RemoveAll(gp => gp.x <= 0);

                if (pipeCounter == 180)
                {
                    pipes.Add(new Pipe((int)(Console.WindowWidth / 1.2 ), this, pipeAmount));
                    pipeCounter = 0;
                    pipeAmount++;
                }
                if (((Program.r.Next(0, 1001) > 998) && (gPortalCounter > 20))|| gPortalCounter == 500)
                {
                    gPortals.Add(new GPortal((int)(Console.WindowWidth / 1.2 )));
                    gPortalCounter = 0;
                }

                // check collision with pipes
                foreach (Pipe p in pipes)
                {
                    if (p.GetCollision())
                    {
                        lost = true;
                        break;
                    }
                }

                //check collison with gravity portals
                foreach(GPortal gp in gPortals)
                {
                    if (gp.GetCollision())
                    {
                        pyr.ReverseGravity();
                        break;
                    }
                }
                loss:
                if (lost)
                {
                    Console.Clear();
                    Console.WriteLine("You lost!!!");
                    break;
                }
                Buffer.Print();
                Thread.Sleep(1);

                pipeCounter++;
                gPortalCounter++;


            }
            return pipeAmount;
        }

    }
    class Player // only physics, no printing.
    {
        public const int X = 15;
        const double Gravity = 0.67;
        const double JumpHeight = -20;
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
            a = jumpMode ? -Gravity : -Gravity * 0.7;
        }
        public double GetNewHeight(bool pressed, double dt)
        {
            ProcessPhysics(jumpMode, pressed, dt);
            return y;
        }
        void ProcessPhysics(bool jumpmode, bool pressed, double dt)
        {
            if (pressed)
            {
                if (jumpmode)
                {
                    double gh = JumpHeight * (a / Math.Abs(a)); // calculation to make the jump height work for both normal and flipped gravity
                    v = gh;

                }
                else
                {
                    a = -a;
                    v = 0.7 * v;
                }
            }

            v += a;
            y -= v * dt;
        }
        public void StartJump()
        {
            ProcessPhysics(true, true, 0.025); // a little jump as the start of the game to make it more readable

        }
        public void ReverseGravity()
        {
            // flip acceleration
            a *= -1;
        }


    }
    class GPortal //gravity portal
    {
        public int x;
        string icon = "-";
        bool used = false;

        public GPortal(int x)
        {
            this.x = x;
        }
        public void Print()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Buffer.SetCursorPosition(x, i);
                Buffer.Write(icon);

            }
        }
        public void Move()
        {
            x--;
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Buffer.SetCursorPosition(x, i);
                Buffer.Write(icon + ' ');

            }
        }
        public void Delete()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Buffer.SetCursorPosition(x, i);
                Buffer.Write(" ");

            }
        }
        public bool GetCollision()
        {
            if ((x <= Player.X && x >= (Player.X - icon.Length + 1)) && !used)
            {
                used = true;
                return true;
            }
            return false;
        }

    }
    class Pipe
    {
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
            int gapY = Program.r.Next(minRange, maxRange + 1);
            gapTop = gapY + gapSize / 2;
            gapBottom = gapY - gapSize / 2;

            icon = $"###";
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
                Buffer.SetCursorPosition(x, i);
                if (i > gapBottom && i < gapTop) // on the gap, which is already empty
                {
                    continue;
                }
                else
                {
                    Buffer.Write(icon);
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
                Buffer.SetCursorPosition(x, i);
                Buffer.SetCursorPosition(x, i);
                if (i > gapBottom && i < gapTop) // on the gap, which is already empty
                {
                    continue;
                }
                else
                {
                    Buffer.Write(deleteString);

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
                Buffer.SetCursorPosition(x, i);
                if (!(i > gapBottom && i < gapTop)) // is not on the gap
                {
                    Buffer.Write(updateString);
                }

            }
        }
        public bool GetCollision()
        {
            if ((x <= Player.X && x >= (Player.X - icon.Length + 1)) && (game.y <= (gapBottom + 1) || game.y >= (gapTop)))
            {
                return true;
            }
            return false;
        }
    }
}

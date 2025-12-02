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
            while (true)
            {
                Thread.Sleep(20);
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
            }
        }
    }
}

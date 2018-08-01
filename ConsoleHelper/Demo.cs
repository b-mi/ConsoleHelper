using System;
using System.Drawing;
using ch = ConsoleHelper.Console;

namespace ConsoleHelper
{
    public class Demo
    {
        public static void Run()
        {
            var dx = ch.GetUniqueRandoms(3, 77, 345, 788, 432);
            dx = ch.GetUniqueRandoms(3, 77, 345, 788, 432);
            dx = ch.GetUniqueRandoms(3, 77, 345, 788, 432);
            dx = ch.GetUniqueRandoms(3, 77, 345, 788, 432);

            ConsoleHelper.Console.FullScreen();

            startTest("DrawRectangle",
                (fl) =>
                {
                    ch.ClearScreen();
                    for (int i = 0; i < 10; i++)
                    {
                        int left, width, top, height;
                        ch.GetRandomLeftWidth(out left, out width);
                        ch.GetRandomTopHeight(out top, out height);
                        ch.SetRandomFgBg();
                        ConsoleHelper.Console.DrawRectangle(left, top, width, height, fl);
                    }
                });

            //kruhy
            startTest("DrawCircle",
                (fl) =>
                {
                    int left, width, top, height;
                    ConsoleHelper.Console.GetRandomLeftWidth(out left, out width);
                    ConsoleHelper.Console.GetRandomTopHeight(out top, out height);
                    ConsoleHelper.Console.DrawCircle(
                        left + width / 2,
                        top + height / 2,
                        height / 2,
                        0, 360, fl);
                });

            //ciary
            startTest("DrawLine",
                (fl) =>
                {

                    var x1 = ConsoleHelper.Console.GetRandomWidth(5);
                    var y1 = ConsoleHelper.Console.GetRandomHeight(2);
                    var x2 = ConsoleHelper.Console.GetRandomWidth(5);
                    var y2 = ConsoleHelper.Console.GetRandomHeight(2);

                    ConsoleHelper.Console.DrawLine(x1, y1, x2, y2);
                });

            //elipsy
            startTest("DrawEllipse",
                (fl) =>
                {
                    int left, width, top, height;
                    ConsoleHelper.Console.GetRandomLeftWidth(out left, out width);
                    ConsoleHelper.Console.GetRandomTopHeight(out top, out height);

                    ConsoleHelper.Console.DrawEllipse(
                        left + width / 2,
                        top + height / 2,
                        width / 2,
                        height / 2,
                        0, 360, fl);
                });


            // trianglatka
            startTest("DrawTriangle",
                (fl) =>
                {

                    Point p1, p2, p3;
                    p1 = new Point(ConsoleHelper.Console.GetRandomWidth(5), ConsoleHelper.Console.GetRandomHeight(2));
                    p2 = new Point(ConsoleHelper.Console.GetRandomWidth(5), ConsoleHelper.Console.GetRandomHeight(2));
                    p3 = new Point(ConsoleHelper.Console.GetRandomWidth(5), ConsoleHelper.Console.GetRandomHeight(2));
                    ConsoleHelper.Console.DrawTriangle(p1, p2, p3, fl);
                });

            bounce();

        }

        private static void bounce()
        {
            resetHeads("MoveRectangle");
            var head1 = new Head();
            //var head2 = new Head();
            bool stop = false;
            bool wait = true;

            while (true)
            {
                while (!System.Console.KeyAvailable)
                {
                    head1.move(wait);
                    //head2.move(wait);
                }
                var kch = System.Console.ReadKey().KeyChar;
                switch (kch)
                {
                    case 'c':
                        resetHeads("MoveRectangle");
                        head1 = new Head();
                        //head2 = new Head();
                        break;
                    case 'w':
                        wait = !wait;
                        break;

                    default:
                        stop = true;
                        break;
                }
                if (stop)
                {
                    break;
                }
            }




        }

        private static void startTest(string msg, Action<bool> act)
        {
            bool fill = false;
            bool wait = true;
            ch.ResetColor();
            while (true)
            {
                reset(msg);
                while (!System.Console.KeyAvailable)
                {
                    ConsoleHelper.Console.SetRandomForeground();
                    act(fill);
                    if (wait)
                        ConsoleHelper.Console.Wait(300);

                }
                var ky = System.Console.ReadKey().KeyChar;
                var stop = false;
                switch (ky)
                {
                    case 'f':
                        fill = !fill;
                        break;
                    case 'w':
                        wait = !wait;
                        break;
                    case 'c':
                        reset(msg);
                        break;
                    default:
                        stop = true;
                        break;
                }
                if (stop)
                {
                    break;
                }
            }


        }

        private static void reset(string msg)
        {
            System.Console.ResetColor();
            System.Console.Clear();
            System.Console.WriteLine($"{msg}, f - toggle fill, c - clear, w - toggle wait, other - continue");
        }

        private static void resetHeads(string msg)
        {
            System.Console.ResetColor();
            System.Console.Clear();
            System.Console.WriteLine($"{msg}, c - new, w - toggle wait, other - continue");
        }
    }

    public class Head
    {
        int moveByX = 0;
        int moveByY = 0;
        Rectangle rect;
        int border = 0;
        int maxMove = 5;

        public Head()
        {
            int l = 0, t = 0, r = 0;
            r = ch.Rand.Next(10, 15);
            while (l <= border + r + 1) l = ch.Rand.Next(5, 100);
            while (t <= border + r + 1) t = ch.Rand.Next(5, 20);
            rect = new Rectangle(l - r, t - ch.ZoomIntNumber(r, 0.5), r * 2 + 1, r + 1);
            ch.SetRandomForeground();
            ch.DrawCircle(l, t, r, 0, 360, true);

            ch.SetRandomForeground();
            ch.DrawCircle(l - 4, t - 2, 2, 0, 180, true);

            ch.SetRandomForeground();
            ch.DrawCircle(l + 4, t - 2, 2, 0, 180, false);

            ch.SetRandomForeground();
            ch.DrawCircle(l, t + 4, 4, 180, 180, false);

            while (moveByX == 0) moveByX = ch.Rand.Next(-maxMove, maxMove);
            while (moveByY == 0) moveByY = ch.Rand.Next(-maxMove, maxMove);
            ch.ResetColor();
        }

        internal void move(bool wait)
        {
            rect = ConsoleHelper.Console.MoveRectangle(rect, moveByX, moveByY);
            var beep = false;
            if (rect.Left <= border - moveByX)
            {
                moveByX = ch.Rand.Next(1, maxMove);
                beep = true;
            }
            else if (rect.Left + rect.Width >= System.Console.WindowWidth - border - moveByX)
            {
                moveByX = ch.Rand.Next(-maxMove, -1);
                beep = true;
            }

            if (rect.Top <= border - moveByY)
            {
                moveByY = ch.Rand.Next(1, maxMove);
                beep = true;
            }
            else if (rect.Top + rect.Height >= System.Console.WindowHeight - border - moveByY)
            {
                moveByY = ch.Rand.Next(-maxMove, -1);
                beep = true;
            }
            if (beep)
                ch.Beep();
            if (wait)
                ch.Wait(50);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using sys = System.Console;

namespace ConsoleHelper
{
    public static class Console
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {

            public short X;
            public short Y;
            public COORD(short x, short y)
            {
                this.X = x;
                this.Y = y;
            }

        }
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetStdHandle(int handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleDisplayMode(
            IntPtr ConsoleOutput
            , uint Flags
            , out COORD NewScreenBufferDimensions
            );




        /// <summary>
        /// Prednastaveny znak pre malovanie
        /// </summary>
        public static char DrawingChar = '■';
        public static int Height { get; private set; }
        public static int Width { get; private set; }

        public static ConsoleColor ForegroundColor { get; set; }
        public static ConsoleColor BackgroundColor { get; set; }



        /// <summary>
        /// Objekt pre nahodne ciska
        /// </summary>
        public static Random Rand = new Random();
        //static ConsoleBuffer buffer;



        /// <summary>
        /// Maximizuje okno, hiduje kurzor, a nastavi kodovu stranku na 866, kde su nejake graficke znaky. googluj codepage 866
        /// </summary>
        public static void FullScreen()
        {
            IntPtr hConsole = GetStdHandle(-11);   // get console handle
            COORD xy = new COORD(100, 100);
            SetConsoleDisplayMode(hConsole, 1, out xy); // set the console to fullscreen
            System.Console.OutputEncoding = Encoding.GetEncoding(866);
            System.Console.CursorVisible = false;
            Wait(100);
            System.Console.BufferHeight = System.Console.WindowHeight;
            System.Console.BufferWidth = System.Console.WindowWidth;
            Width = System.Console.WindowWidth;
            Height = System.Console.WindowHeight;
        }

        /// <summary>
        /// Namaľuje kruh
        /// </summary>
        /// <param name="centerX">X súradnica stredu kružnice</param>
        /// <param name="centerY">Y súradnica stredu kružnice</param>
        /// <param name="radius">Polomer</param>
        /// <param name="angleFrom">Od uhla (0-360)</param>
        /// <param name="angleTo">Po uhol (0-360)</param>
        /// <param name="fill">Ci vyplnit alebo nie</param>
        public static void DrawCircle(double centerX, double centerY, int radius, int angleFrom, int angleCount, bool fill = false)
        {
            if (radius < 1)
            {
                return;
            }
            Dictionary<int, FillInfo> dct = null;
            if (fill)
                dct = new Dictionary<int, FillInfo>();
            for (int angle = angleFrom; angle <= angleFrom + angleCount; angle += 2)
            {
                var x = (int)Math.Round((centerX + radius * Math.Cos(angle * (Math.PI / 180))));
                var y = (int)Math.Round((centerY + radius * Math.Sin(angle * (Math.PI / 180)) / 2));
                if (fill)
                    fillRegisterPoint(dct, x, y);
                if (x < 0 || x >= Width - 1)
                {
                    continue;
                }
                if (y < 0 || y >= Height - 1)
                {
                    continue;
                }
                Write(DrawingChar, x, y, ForegroundColor, BackgroundColor);
            }
            if (fill)
                fillLines(dct);
        }

        /// <summary>
        /// Nahodne cislo v rozsah do sirky obrazovky
        /// </summary>
        /// <returns></returns>
        public static int GetRandomWidth(int margin)
        {
            return Rand.Next(margin, Width - margin);
        }

        /// <summary>
        /// Nahodne cislo v rozsahu do vysky obrazovky
        /// </summary>
        /// <returns></returns>
        public static int GetRandomHeight(int margin)
        {
            return Rand.Next(margin, Height - margin);
        }

        public static int[] GetUniqueRandoms(int count, params int[] list)
        {
            var dct = new Dictionary<int, int>();
            while (dct.Count < count)
            {
                var r = Rand.Next(list.Length);
                var v = list[r];
                if (!dct.ContainsKey(v))
                {
                    dct.Add(v, v);
                }
            }
            return dct.Select(r => r.Key).ToArray();
        }

        public static ConsoleColor GetRandomColor(params ConsoleColor[] allowedColors)
        {
            var rx = Rand.Next(allowedColors.Length);
            return allowedColors[rx];
        }

        public static ConsoleColor GetRandomColor()
        {
            return (ConsoleColor)Rand.Next(16);
        }

        static ConsoleColor[] vividColors =
        {
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta,
            ConsoleColor.Red,
            ConsoleColor.White,
            ConsoleColor.Yellow,
        };

        static ConsoleColor[] darkColors =
{
            ConsoleColor.DarkBlue,
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkGray,
            ConsoleColor.DarkGreen,
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkYellow,
        };



        public static ConsoleColor GetRandomVividColor()
        {
            return vividColors[Rand.Next(vividColors.Length)];
        }

        public static ConsoleColor GetRandomDarkColor()
        {
            return vividColors[Rand.Next(darkColors.Length)];
        }



        /// <summary>
        /// Vrati nahodnu left a width poziciu tak, aby nepresahovala rozmer obrazovky
        /// </summary>
        /// <param name="left"></param>
        /// <param name="width"></param>
        public static void GetRandomLeftWidth(out int left, out int width)
        {
            left = Rand.Next(Width - 10);
            var max = Width - left - 5;
            width = Rand.Next(max) + 3;
        }

        public static void ResetColor()
        {
            sys.ResetColor();
        }

        /// <summary>
        /// Vrati nahodnu top a height suradnicu tak, aby nepresahovala vysku obrazovky
        /// </summary>
        /// <param name="top"></param>
        /// <param name="height"></param>
        public static void GetRandomTopHeight(out int top, out int height)
        {
            top = Rand.Next(Height - 6) + 1;
            var max = Height - top - 5;
            height = Rand.Next(max) + 3;
        }

        /// <summary>
        /// Nastavi nahodnu farbu popredia
        /// </summary>
        public static void SetRandomForeground()
        {
            ForegroundColor = (ConsoleColor)Rand.Next(15) + 1;
        }

        /// <summary>
        /// Nastavi nahodnu farbu pozadia
        /// </summary>
        public static void SetRandomBackground()
        {
            BackgroundColor = (ConsoleColor)Rand.Next(15) + 1;
        }

        /// <summary>
        /// vrati nahopdny true alebo false
        /// </summary>
        /// <returns></returns>
        public static bool GetRandomBool()
        {
            return Rand.Next(2) == 0;
        }

        /// <summary>
        /// Vrati nahodnu hodnotu 1 alebo -1
        /// </summary>
        /// <returns></returns>
        public static int GetRandomSign()
        {
            if (Rand.Next(2) == 0)
                return -1;
            return 1;
        }

        /// <summary>
        /// Nastavi nahodnu farbu popredia a pozadia ale tak, aby nebolo rovnake
        /// </summary>
        public static void SetRandomFgBg()
        {
            var bg = (ConsoleColor)Rand.Next(15) + 1;
            var fg = bg;
            while (fg == bg) fg = (ConsoleColor)Rand.Next(15) + 1;
            BackgroundColor = bg;
            BackgroundColor = fg;
        }

        /// <summary>
        /// Namaluje odlznik
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fill"></param>
        public static void DrawRectangle(int left, int top, int width, int height, bool fill = false)
        {
            Dictionary<int, FillInfo> dct = null;
            if (fill)
                dct = new Dictionary<int, FillInfo>();

            var chrsFull = new string(DrawingChar, width);
            for (int y = 0; y < height; y++)
            {
                if (fill)
                    dct.Add(top + y, new FillInfo { MinX = left + 1, MaxX = left + width - 2 });
                if (y == 0 || y == height - 1 || fill)
                    Write(chrsFull, left, top + y);
                else
                {
                    Write(DrawingChar, left, top + y);
                    Write(DrawingChar, left + width - 1, top + y);
                }
            }
            if (fill)
                fillLines(dct);
        }

        public static void Write(string text, int left, int top)
        {
            Write(text, left, top, ForegroundColor, BackgroundColor);
        }

        public static void Write(string text, int left, int top, ConsoleColor fg, ConsoleColor bg)
        {
            System.Console.SetCursorPosition(left, top);
            if (sys.ForegroundColor != fg)
                sys.ForegroundColor = fg;
            if (sys.BackgroundColor != bg)
                sys.BackgroundColor = bg;
            sys.Write(text);
        }



        public static void Write(char chr, int left, int top)
        {
            Write(chr, left, top, ForegroundColor, BackgroundColor);
        }

        static int lastLeft, lastTop;
        public static void Write(char chr, int left, int top, ConsoleColor fg, ConsoleColor bg)
        {
            if (left < 0 || top < 0 || left >= Width || top >= Height)
            {
                return;
            }
            if (top != lastTop || lastLeft != left - 1)
            {
                System.Console.SetCursorPosition(left, top);
            }
            if (sys.ForegroundColor != fg)
                sys.ForegroundColor = fg;
            if (sys.BackgroundColor != bg)
                sys.BackgroundColor = bg;
            sys.Write(chr);
            lastLeft = left;
            lastTop = top;
        }

        /// <summary>
        /// Namaluje trojuholnik
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="fill"></param>
        public static void DrawTriangle(Point p1, Point p2, Point p3, bool fill = false)
        {
            if (fill)
            {
                var dct = new Dictionary<int, FillInfo>();
                var startPoint = p1;
                var endPoint = p2;
                ConsoleHelper.Console.DrawLine2(dct, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                startPoint = endPoint;
                endPoint = p3;
                ConsoleHelper.Console.DrawLine2(dct, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                startPoint = endPoint;
                endPoint = p1;
                ConsoleHelper.Console.DrawLine2(dct, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

                fillLines(dct);
            }
            else
            {
                var startPoint = p1;
                var endPoint = p2;
                ConsoleHelper.Console.DrawLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                startPoint = endPoint;
                endPoint = p3;
                ConsoleHelper.Console.DrawLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                startPoint = endPoint;
                endPoint = p1;
                ConsoleHelper.Console.DrawLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
            }
        }

        private static void fillLines(Dictionary<int, FillInfo> dct)
        {
            if (dct.Count == 0)
            {
                return;
            }

            var miny = dct.Min(i => i.Key);
            var maxy = dct.Max(i => i.Key);
            var maxScrY = Height - 1;
            var maxScrX = Width;

            foreach (var item in dct)
            {
                var y = item.Key;
                if (y < 0)
                    continue;
                if (y > maxScrY)
                    continue;

                int minx = item.Value.MinX;
                if (minx < 0) minx = 0;
                int maxx = item.Value.MaxX;
                if (maxx > maxScrX)
                    maxx = maxScrX;
                //if (y == miny || y == maxy) continue;
                var diff = maxx - minx + 1;
                if (diff > 0)
                {
                    var fl = new string(ConsoleHelper.Console.DrawingChar, diff);
                    Write(fl, minx, y);
                }
            }

        }

        public static void ClearScreen()
        {
            sys.ResetColor();
            sys.Clear();
        }


        /// Namaľuje elipsu
        /// </summary>
        /// <param name="centerX">X súradnica stredu kružnice</param>
        /// <param name="centerY">Y súradnica stredu kružnice</param>
        /// <param name="radiusX">Polomer X</param>
        /// <param name="radiusY">Polomer Y</param>
        /// <param name="angleFrom">Od uhla (0-360)</param>
        /// <param name="angleTo">Po uhol (0-360)</param>
        /// <param name="chr">Znak</param>
        public static void DrawEllipse(double centerX, double centerY, int radiusX, int radiusY, int angleFrom, int angleCount, bool fill = false)
        {
            if (radiusX < 1 || radiusY < 1)
            {
                return;
            }
            int radX = radiusX;
            int radY = radiusY;
            Dictionary<int, FillInfo> dct = null;
            if (fill)
                dct = new Dictionary<int, FillInfo>();
            //NEMENIT!!!
            for (int angle = angleFrom; angle <= angleFrom + angleCount; angle += 2)
            {
                var x = (int)Math.Round((centerX + radX * Math.Cos(angle * (Math.PI / 180))));
                if (x < 0 || x >= Width - 1)
                {
                    continue;
                }
                var y = (int)Math.Round((centerY + radY * Math.Sin(angle * (Math.PI / 180)) / 2));
                if (y < 0 || y >= Height - 1)
                {
                    continue;
                }
                if (fill)
                    fillRegisterPoint(dct, x, y);
                Write(DrawingChar, x, y);
            }
            if (fill)
            {
                fillLines(dct);
            }

        }

        /// <summary>
        /// Namaluje úsečku
        /// </summary>
        /// <param name="x1">x súradnica počiatočného bodu</param>
        /// <param name="y1">y súradnica počiatočného bodu</param>
        /// <param name="x2">x súradnica koncového bodu</param>
        /// <param name="y2">y súradnica koncového bodu</param>
        /// <param name="chr">znak</param>
        public static void DrawLine(int x, int y, int x2, int y2)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                if (x < 0 || y < 0 || x >= Width - 1 || y >= Height - 1)
                {

                }
                else
                {
                    Write(DrawingChar, x, y);
                }
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        internal static void DrawLine2(Dictionary<int, FillInfo> dctReg, int x, int y, int x2, int y2)
        {

            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                if (x < 0 || y < 0 || x >= Width - 1 || y >= Height - 1)
                {

                }
                else
                {
                    fillRegisterPoint(dctReg, x, y);
                    Write(DrawingChar, x, y);
                }
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        /// <summary>
        /// Posunie cast obsahu obrazovky na inu poziciu
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="moveByX"></param>
        /// <param name="moveByY"></param>
        /// <returns></returns>
        public static Rectangle MoveRectangle(Rectangle rect, int moveByX, int moveByY)
        {
            Rectangle newRect = rect;
            newRect.Offset(moveByX, moveByY);
            System.Console.MoveBufferArea(rect.Left, rect.Top, rect.Width, rect.Height, newRect.Left, newRect.Top);
            return newRect;
        }

        private static void fillRegisterPoint(Dictionary<int, FillInfo> dctReg, int x, int y)
        {
            FillInfo fi;
            if (dctReg.ContainsKey(y))
            {
                fi = dctReg[y];
                if (x < fi.MinX) fi.MinX = x;
                else if (x > fi.MaxX) fi.MaxX = x;
            }
            else
            {
                fi = new FillInfo() { MinX = x, MaxX = x };
                dctReg.Add(y, fi);
            }
        }

        /// <summary>
        /// Pocka zadany pocet milisekund
        /// </summary>
        /// <param name="milliseconds"></param>
        public static void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }





        /// <summary>
        /// Pipne. frekvencia 5000, trvanie 5 miliseknud
        /// </summary>
        public static void Beep()
        {
            System.Console.Beep(5000, 5);
        }

        public static int ZoomIntNumber(int number, double zoomBy)
        {
            return (int)Math.Round((double)number * zoomBy, 0, MidpointRounding.AwayFromZero);
        }

    }

    internal class FillInfo
    {
        public int MinX { get; set; } = int.MaxValue;
        public int MaxX { get; set; } = int.MinValue;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ch = ConsoleHelper.Console;

namespace ConsoleHelper
{
    public class Animator
    {
        public List<AnimationObject> Objects { get; private set; } = new List<AnimationObject>();

        public void Add(AnimationObject ao)
        {
            Objects.Add(ao);
        }

        public void Start()
        {
            var toDelete = new List<AnimationObject>();
            while (Objects.Any() && !System.Console.KeyAvailable)
            {
                foreach (var item in Objects)
                {
                    var now = DateTime.Now.Ticks;
                    var needToAnimate = now >= item.nextEvent;
                    if (needToAnimate)
                    {
                        item.nextEvent += item.AnimateEveryTicks;
                        item.OnAnimation();
                        if (item.killRequest)
                        {
                            toDelete.Add(item);
                        }
                    }
                }
                if (toDelete.Count > 0)
                {
                    toDelete.ForEach(r => Objects.Remove(r));
                }
            }
        }
    }

    public class AnimationObject
    {
        internal long nextEvent;
        public int AnimateEveryTicks { get; private set; } = 1000000;
        internal bool killRequest;

        public void SetAnimationInterval(int intervalInMiliseconds)
        {
            AnimateEveryTicks = intervalInMiliseconds * 10000; // ticks to miliseconds
            init();
        }

        public AnimationObject(int animateEveryMiliseconds)
        {
            this.AnimateEveryTicks = animateEveryMiliseconds * 10000; // ticks to miliseconds
            init();
        }

        public AnimationObject()
        {
            init();
        }

        private void init()
        {
            nextEvent = DateTime.Now.Ticks;
        }


        public void Kill()
        {
            this.killRequest = true;
        }

        public virtual void OnAnimation()
        {

        }
    }


    public class AnimSimple : AnimationObject
    {
        int x, y;
        bool red = false;
        public AnimSimple(int animateeveryms) : base(animateeveryms)
        {
            x = ch.GetRandomWidth(5);
            y = ch.GetRandomHeight(2);
        }

        public override void OnAnimation()
        {
            if (red)
                ch.ForegroundColor = ConsoleColor.Red;
            else
                ch.ForegroundColor = ConsoleColor.Blue;
            red = !red;

            ch.Write(ch.DrawingChar, x, y);

        }
    }

    public class AnimSample : AnimationObject
    {
        char txt;
        int maxBump;
        int x, y, bumpsCount, dx, dy;
        int leftMin, leftMax;
        int topMin, topMax;
        ConsoleColor color;
        public AnimSample(char txt, int interval, int max, ConsoleColor color) : base(interval)
        {
            this.txt = txt;
            this.color = color;
            this.maxBump = max;
            leftMin = topMin = 0;
            leftMax = ch.Width - 0;
            topMax = ch.Height - 1;

            x = ch.Rand.Next(leftMin, leftMax - leftMin);
            y = ch.Rand.Next(topMin, topMax - topMin);
            dx = ch.GetRandomSign(); // 1 alebo -1
            dy = ch.GetRandomSign(); // 1 alebo -1
        }

        public override void OnAnimation()
        {
            var oldx = x;
            var oldy = y;

            x += dx;
            if (x <= leftMin || x >= leftMax)
            {
                dx = -dx; // zmena smeru pohybu
                x += 2 * dx; // a pohneme sa dalej od okraja
                bumpsCount++; // pripocitame odraz
            }

            y += dy;
            if (y <= topMin || y >= topMax)
            {
                dy = -dy; // zmena smeru pohybu
                y += 2 * dy; // a pohneme sa dalej od okraja
                bumpsCount++; // pripocitame odraz
            }

            ch.Write(txt, x, y, color, ConsoleColor.Black);
            ch.Write(' ', oldx, oldy); // zmazeme znak skor namalovany

            if (bumpsCount > maxBump)
            {
                this.Kill(); // bolo dosiahnuty pocet bumpov - koniec
            }
        }
    }

}

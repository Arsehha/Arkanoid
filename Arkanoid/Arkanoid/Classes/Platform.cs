using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс платформы
    /// </summary>
    public class Platform
    {
        public Rectangle Bounds { get; set; }
        public int Speed { get; set; } = 8;

        public Platform(int x, int y, int width, int height)
        {
            Bounds = new Rectangle(x, y, width, height);
        }

        public void MoveLeft()
        {
            Bounds = new Rectangle(
                Math.Max(0, Bounds.X - Speed),
                Bounds.Y,
                Bounds.Width,
                Bounds.Height
            );
        }

        public void MoveRight(int containerWidth)
        {
            Bounds = new Rectangle(
                Math.Min(containerWidth - Bounds.Width, Bounds.X + Speed),
                Bounds.Y,
                Bounds.Width,
                Bounds.Height
            );
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, Bounds);
        }
    }
}

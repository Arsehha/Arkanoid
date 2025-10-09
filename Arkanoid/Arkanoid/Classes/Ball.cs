using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для кирпичей
    /// </summary>
    public class Ball
    {
        private PointF position; // используем поле вместо auto-property (опционально, но чище)

        public PointF Position
        {
            get => position;
            set => position = value;
        }

        public float Radius { get; set; } = 10f;
        public float Dx { get; set; } = 5f;
        public float Dy { get; set; } = -5f;

        public Ball(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Move()
        {
            // Создаём новый PointF с обновлёнными координатами
            Position = new PointF(Position.X + Dx, Position.Y + Dy);
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Red, GetBounds());
        }

        public void ReverseX() => Dx = -Dx;
        public void ReverseY() => Dy = -Dy;
    }
}

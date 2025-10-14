
namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для кирпичей
    /// </summary>
    public class Ball
    {
        private PointF position;

        public PointF Position
        {
            get => position;
            set => position = value;
        }

        public float Radius { get; set; } = 10f;
        public float Dx { get; set; } = 5f;
        public float Dy { get; set; } = -5f;

        /// <summary>
        /// Конструктор мяча
        /// </summary>
        public Ball(float x, float y)
        {
            Position = new PointF(x, y);
        }

        /// <summary>
        /// Движение мяча
        /// </summary>
        public void Move()
        {
            // Создаём новый PointF с обновлёнными координатами
            Position = new PointF(Position.X + Dx, Position.Y + Dy);
        }

        /// <summary>
        /// Расчёт отскока
        /// </summary>
        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }

        /// <summary>
        /// Отрисовка мяча
        /// </summary>
        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Red, GetBounds());
        }

        public void ReverseX() => Dx = -Dx;
        public void ReverseY() => Dy = -Dy;
    }
}

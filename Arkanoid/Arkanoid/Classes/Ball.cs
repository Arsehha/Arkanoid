
namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для шара
    /// </summary>
    public class Ball
    {
        public PointF Position { get; set; }
        public float Radius { get; set; }
        public float Dx { get; set; }
        public float Dy { get; set; }

        /// <summary>
        /// Конструктор шара
        /// </summary>
        public Ball(float x, float y, float radius = 10f, float dx = 5f, float dy = -5f)
        {
            Position = new PointF(x, y);
            Radius = radius;
            Dx = dx;
            Dy = dy;
        }

        public RectangleF Bounds =>
            new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);

        /// <summary>
        /// Перемещние шара
        /// </summary>
        public void Step()
        {
            Position = new PointF(Position.X + Dx, Position.Y + Dy);
        }

        public void ReverseX() => Dx = -Dx;
        public void ReverseY() => Dy = -Dy;
    }
}

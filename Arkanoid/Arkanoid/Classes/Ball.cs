
namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для шара
    /// </summary>
    public class Ball
    {
        /// <summary>
        /// Текущая позиция шара в игровом пространстве.
        /// </summary>
        public PointF Position { get; set; }

        /// <summary>
        /// Радиус шара.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Горизонтальная скорость.
        /// </summary>
        public float Dx { get; set; }

        /// <summary>
        /// Вертикальная скорость.
        /// </summary>
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

        /// <summary>
        /// Прямоугольные границы шара, используемые для проверки столкновений.
        /// </summary>
        public RectangleF Bounds =>
            new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);

        /// <summary>
        /// Перемещние шара
        /// </summary>
        public void Step()
        {
            Position = new PointF(Position.X + Dx, Position.Y + Dy);
        }

        /// <summary>
        /// Разворачивает горизонтальную скорость.
        /// </summary>
        public void ReverseX() => Dx = -Dx;

        /// <summary>
        /// Разворачивает вертикальную скорость.
        /// </summary>
        public void ReverseY() => Dy = -Dy;
    }
}

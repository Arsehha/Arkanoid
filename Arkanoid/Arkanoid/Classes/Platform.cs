
namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс платформы
    /// </summary>
    public class Platform
    {
        /// <summary>
        /// Текущие границы платформы.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Скорость горизонтального перемещения.
        /// </summary>
        public int Speed { get; set; } = 8;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Platform(int x, int y, int width, int height)
        {
            Bounds = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Движение влево
        /// </summary>
        public void MoveLeft()
        {
            Bounds = new Rectangle(Math.Max(0, Bounds.X - Speed), Bounds.Y, Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// Движение вправо
        /// </summary>
        public void MoveRight(int containerWidth)
        {
            Bounds = new Rectangle(Math.Min(containerWidth - Bounds.Width, Bounds.X + Speed),
                                   Bounds.Y, Bounds.Width, Bounds.Height);
        }
    }
}


namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для кирпичей
    /// </summary>
    public class Brick
    {
        /// <summary>
        /// Прямоугольные границы кирпича.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Флаг разрушения кирпича.
        /// </summary>
        public bool IsDestroyed { get; private set; } = false;

        /// <summary>
        /// Цвет кирпича.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Текущая прочность кирпича.
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Конструктор кирпича
        /// </summary>
        public Brick(int x, int y, int width, int height, Color color, int health = 1)
        {
            Bounds = new Rectangle(x, y, width, height);
            Color = color;
            Health = health;
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(int damage = 1)
        {
            if (IsDestroyed)
            {
                return;
            }
            Health -= damage;
            if (Health <= 0)
            {
                IsDestroyed = true;
            }
        }
    }
}

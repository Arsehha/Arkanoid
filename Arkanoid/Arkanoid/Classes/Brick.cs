
namespace Arkanoid.Classes
{
    /// <summary>
    /// Класс для кирпичей
    /// </summary>
    public class Brick
    {
        public Rectangle Bounds { get; set; }
        public bool IsDestroyed { get; set; } = false;
        public Color Color { get; set; }
        public int Health { get; private set; } = 1; // 1 HP

        public Brick(int x, int y, int width, int height, Color color)
        {
            Bounds = new Rectangle(x, y, width, height);
            Color = color;
        }

        /// <summary>
        /// Полечение урона
        /// </summary>
        public void TakeDamage(int damage = 1)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsDestroyed = true;
            }
        }

        /// <summary>
        /// Отрисовка кирпича
        /// </summary>
        public void Draw(Graphics g)
        {
            if (!IsDestroyed)
            {
                g.FillRectangle(new SolidBrush(Color), Bounds);
            }
        }
    }
}

using Arkanoid.Classes;

namespace Arkanoid
{
    /// <summary>
    /// Игровая логика
    /// </summary>
    public class GameController
    {
        // Параметры конфигурации
        /// <summary>Ширина платформы.</summary>
        public int PlatformWidth { get; } = 100;

        /// <summary>Высота платформы.</summary>
        public int PlatformHeight { get; } = 15;

        /// <summary>Отступ платформы от нижнего края окна.</summary>
        public int PlatformYOffset { get; } = 50;

        /// <summary>Радиус мяча.</summary>
        public float BallRadius { get; } = 10f;

        /// <summary>Начальная скорость мяча по X.</summary>
        public float InitialBallSpeedX { get; } = 5f;

        /// <summary>Начальная скорость мяча по Y.</summary>
        public float InitialBallSpeedY { get; } = -5f;

        /// <summary>Ширина кирпича.</summary>
        public int BrickWidth { get; } = 60;

        /// <summary>Высота кирпича.</summary>
        public int BrickHeight { get; } = 20;

        /// <summary>Интервал между кирпичами.</summary>
        public int BrickSpacing { get; } = 5;

        /// <summary>Начальное смещение кирпичей по Y.</summary>
        public int BrickStartY { get; } = 30;

        /// <summary>Начальное смещение кирпичей по X.</summary>
        public int BrickStartX { get; } = 10;

        /// <summary>Количество рядов кирпичей.</summary>
        public int BrickRows { get; } = 5;

        /// <summary>Платформа игрока.</summary>
        public Platform Platform { get; private set; }

        /// <summary>Игровой мяч.</summary>
        public Ball Ball { get; private set; }

        /// <summary>Коллекция кирпичей уровня.</summary>
        public List<Brick> Bricks { get; private set; } = new();

        /// <summary>Флаг запуска мяча.</summary>
        public bool BallLaunched { get; private set; } = false;

        /// <summary>Событие окончания игры (проигрыш).</summary>
        public event Action? GameOver;

        /// <summary>Событие победы (все кирпичи уничтожены).</summary>
        public event Action? Victory;

        /// <summary>
        /// Инициализация/рестарт уровня
        /// </summary>
        public void Initialize(Size clientSize)
        {
            // Платформа по центру снизу
            int platformX = clientSize.Width / 2 - PlatformWidth / 2;
            int platformY = clientSize.Height - PlatformYOffset;
            Platform = new Platform(platformX, platformY, PlatformWidth, PlatformHeight);

            // Мяч — на платформе
            float ballStartX = Platform.Bounds.X + PlatformWidth / 2f;
            float ballStartY = Platform.Bounds.Y - BallRadius;
            Ball = new Ball(ballStartX, ballStartY, BallRadius, InitialBallSpeedX, InitialBallSpeedY);

            // Кирпичи — заполняем строки и столбцы, учитывая ширину окна
            Bricks.Clear();
            int cols = Math.Max(1, (clientSize.Width - 2 * BrickStartX + BrickSpacing) / (BrickWidth + BrickSpacing));
            for (int row = 0; row < BrickRows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = col * (BrickWidth + BrickSpacing) + BrickStartX;
                    int y = row * (BrickHeight + BrickSpacing) + BrickStartY;
                    Bricks.Add(new Brick(x, y, BrickWidth, BrickHeight, GetBrickColor(row)));
                }
            }

            BallLaunched = false;
        }

        private Color GetBrickColor(int row)
        {
            Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Cyan };
            return colors[row % colors.Length];
        }

        /// <summary>
        /// Апдейт состояния игры
        /// </summary>
        public void Update(Size clientSize)
        {
            if (Platform == null || Ball == null)
            {
                return;
            }

            if (!BallLaunched)
            {
                // Удержание мяча на платформе
                Ball.Position = new PointF(Platform.Bounds.X + Platform.Bounds.Width / 2f,
                                           Platform.Bounds.Y - Ball.Radius);
                return;
            }

            // Перемещение мяча
            Ball.Step();

            // Отскок от боков
            if (Ball.Position.X - Ball.Radius <= 0)
            {
                Ball.Position = new PointF(Ball.Radius, Ball.Position.Y);
                Ball.ReverseX();
            }
            else if (Ball.Position.X + Ball.Radius >= clientSize.Width)
            {
                Ball.Position = new PointF(clientSize.Width - Ball.Radius, Ball.Position.Y);
                Ball.ReverseX();
            }

            // Отскок от верха
            if (Ball.Position.Y - Ball.Radius <= 0)
            {
                Ball.Position = new PointF(Ball.Position.X, Ball.Radius);
                Ball.ReverseY();
            }

            // Проигрыш 
            if (Ball.Position.Y - Ball.Radius > clientSize.Height)
            {
                BallLaunched = false;
                GameOver?.Invoke();
                return;
            }

            // Столкновение с платформой
            if (Ball.Bounds.IntersectsWith(Platform.Bounds))
            {
                Ball.Position = new PointF(Ball.Position.X, Platform.Bounds.Y - Ball.Radius - 1f);
                Ball.ReverseY();

                float hitPos = (Ball.Position.X - Platform.Bounds.Left) / (float)Platform.Bounds.Width;
                Ball.Dx = (hitPos - 0.5f) * 10f;
            }

            // Столкновение с кирпичами
            foreach (var brick in Bricks)
            {
                if (brick.IsDestroyed)
                {
                    continue;
                }

                if (Ball.Bounds.IntersectsWith(brick.Bounds))
                {
                    brick.TakeDamage(1);

                    RectangleF b = brick.Bounds;
                    RectangleF ballBounds = Ball.Bounds;

                    float overlapLeft = ballBounds.Right - b.Left;
                    float overlapRight = b.Right - ballBounds.Left;
                    float overlapTop = ballBounds.Bottom - b.Top;
                    float overlapBottom = b.Bottom - ballBounds.Top;

                    float minOverlap = Math.Min(Math.Min(Math.Abs(overlapLeft), Math.Abs(overlapRight)),
                                                Math.Min(Math.Abs(overlapTop), Math.Abs(overlapBottom)));

                    if (minOverlap == Math.Abs(overlapLeft) || minOverlap == Math.Abs(overlapRight))
                    {
                        Ball.ReverseX();
                    }
                    else
                    {
                        Ball.ReverseY();
                    }

                    break;
                }
            }

            // Проверка победы
            if (Bricks.All(b => b.IsDestroyed))
            {
                BallLaunched = false;
                Victory?.Invoke();
            }
        }

        /// <summary>
        /// Движение влево
        /// </summary>
        public void MovePlatformLeft()
        {
            if (Platform != null)
            {
                Platform.MoveLeft();
            }
        }

        /// <summary>
        /// Движение вправо
        /// </summary>
        public void MovePlatformRight(int containerWidth)
        {
            if (Platform != null)
            {
                Platform.MoveRight(containerWidth);
            }
        }

        /// <summary>
        /// Запуск шара
        /// </summary>
        public void LaunchBall()
        {
            BallLaunched = true;
        }
    }
}

using Arkanoid.Classes;

namespace Arkanoid.Forms
{
    public partial class MainForm : Form
    {
        private const int PlatformWidth = 100;
        private const int PlatformHeight = 15;
        private const int PlatformYOffset = 50;

        private const float BallRadius = 10f;
        private const float InitialBallSpeedX = 5f;
        private const float InitialBallSpeedY = -5f;

        private const int BrickWidth = 60;
        private const int BrickHeight = 20;
        private const int BrickSpacing = 5;
        private const int BrickStartY = 30;
        private const int BrickStartX = 10;
        private const int BrickRows = 5;

        private const int TimerIntervalMs = 16;

        private Platform platform;
        private Ball ball;
        private List<Brick> bricks = new List<Brick>();
        private System.Windows.Forms.Timer gameTimer;
        private bool ballLaunched = false;

        /// <summary>
        /// Основной конструктор
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            InitializeGame();
        }

        /// <summary>
        /// Запуск игры
        /// </summary>
        private void InitializeGame()
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();

            // Платформа
            int platformX = Width / 2 - PlatformWidth / 2;
            int platformY = Height - PlatformYOffset;
            platform = new Platform(platformX, platformY, PlatformWidth, PlatformHeight);

            // Шар
            float ballStartX = platform.Bounds.X + PlatformWidth / 2f;
            float ballStartY = platform.Bounds.Y - BallRadius;
            ball = new Ball(ballStartX, ballStartY)
            {
                Radius = BallRadius,
                Dx = InitialBallSpeedX,
                Dy = InitialBallSpeedY
            };

            // Кирпичи
            bricks.Clear();
            int cols = (Width - 2 * BrickStartX + BrickSpacing) / (BrickWidth + BrickSpacing);

            for (int row = 0; row < BrickRows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = col * (BrickWidth + BrickSpacing) + BrickStartX;
                    int y = row * (BrickHeight + BrickSpacing) + BrickStartY;
                    bricks.Add(new Brick(x, y, BrickWidth, BrickHeight, GetBrickColor(row)));
                }
            }

            // Таймер
            gameTimer = new System.Windows.Forms.Timer { Interval = TimerIntervalMs };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        /// <summary>
        /// Возвращает цвет кирпича
        /// </summary>
        private Color GetBrickColor(int row)
        {
            Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Cyan };
            return colors[row % colors.Length];
        }

        /// <summary>
        /// Игровой процесс
        /// </summary>
        private void GameLoop(object sender, EventArgs e)
        {
            if (ballLaunched)
            {
                ball.Move();

                // Отскок от левой и правой стен
                if (ball.Position.X <= ball.Radius || ball.Position.X >= Width - ball.Radius)
                {
                    ball.ReverseX();
                }

                // Отскок от верхней стены
                if (ball.Position.Y <= ball.Radius)
                {
                    ball.ReverseY();
                }

                // Проверка на поражение
                if (ball.Position.Y > Height)
                {
                    gameTimer.Stop();
                    MessageBox.Show("Не повезло!");
                    Application.Restart();
                    return;
                }

                // Столкновение с платформой
                if (ball.GetBounds().IntersectsWith(platform.Bounds))
                {
                    ball.ReverseY();
                    float hitPos = (ball.Position.X - platform.Bounds.Left) / platform.Bounds.Width;
                    ball.Dx = (hitPos - 0.5f) * 10f;
                }

                // Столкновение с кирпичами
                foreach (var brick in bricks)
                {
                    if (!brick.IsDestroyed && ball.GetBounds().IntersectsWith(brick.Bounds))
                    {
                        brick.TakeDamage(1);
                        ball.ReverseY();
                        break;
                    }
                }

                // Проверка победы
                if (bricks.All(b => b.IsDestroyed))
                {
                    gameTimer.Stop();
                    MessageBox.Show("Мои поздравления!");
                    Application.Restart();
                }
            }
            else
            {
                ball.Position = new PointF(
                    platform.Bounds.X + PlatformWidth / 2f,
                    platform.Bounds.Y - ball.Radius
                );
            }

            Invalidate();
        }

        /// <summary>
        /// Вывод
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Dictionary<Color, SolidBrush> brushCache = new Dictionary<Color, SolidBrush>();

            try
            {
                platform.Draw(e.Graphics);
                ball.Draw(e.Graphics);

                foreach (var brick in bricks)
                {
                    if (brick.IsDestroyed)
                    {
                        continue;
                    }

                    if (!brushCache.TryGetValue(brick.Color, out SolidBrush brush))
                    {
                        brush = new SolidBrush(brick.Color);
                        brushCache[brick.Color] = brush;
                    }

                    e.Graphics.FillRectangle(brush, brick.Bounds);
                    e.Graphics.DrawRectangle(Pens.Black, brick.Bounds);
                }
            }
            finally
            {
                // Освобождаем кисти
                foreach (var brush in brushCache.Values)
                {
                    brush.Dispose();
                }
            }
        }


        /// <summary>
        /// Управление
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Left)
            {
                platform.MoveLeft();
            }
            else if (e.KeyCode == Keys.Right)
            {
                platform.MoveRight(Width);
            }
            else if (e.KeyCode == Keys.Space && !ballLaunched)
            {
                ballLaunched = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Arkanoid.Classes;

namespace Arkanoid.Forms
{
    public partial class MainForm : Form
    {
        private const int PLATFORM_WIDTH = 100;
        private const int PLATFORM_HEIGHT = 15;
        private const int PLATFORM_Y_OFFSET = 50;

        private const float BALL_RADIUS = 10f;
        private const float INITIAL_BALL_SPEED_X = 5f;
        private const float INITIAL_BALL_SPEED_Y = -5f;

        private const int BRICK_WIDTH = 60;
        private const int BRICK_HEIGHT = 20;
        private const int BRICK_SPACING = 5;
        private const int BRICK_START_Y = 30;
        private const int BRICK_START_X = 10;
        private const int BRICK_ROWS = 5;

        private const int TIMER_INTERVAL_MS = 16;

        private Platform platform;
        private Ball ball;
        private List<Brick> bricks = new List<Brick>();
        private System.Windows.Forms.Timer gameTimer;
        private bool ballLaunched = false;

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
            int platformX = Width / 2 - PLATFORM_WIDTH / 2;
            int platformY = Height - PLATFORM_Y_OFFSET;
            platform = new Platform(platformX, platformY, PLATFORM_WIDTH, PLATFORM_HEIGHT);

            // Шар
            float ballStartX = platform.Bounds.X + PLATFORM_WIDTH / 2f;
            float ballStartY = platform.Bounds.Y - BALL_RADIUS;
            ball = new Ball(ballStartX, ballStartY)
            {
                Radius = BALL_RADIUS,
                Dx = INITIAL_BALL_SPEED_X,
                Dy = INITIAL_BALL_SPEED_Y
            };

            // Кирпичи
            bricks.Clear();
            int cols = (Width - 2 * BRICK_START_X + BRICK_SPACING) / (BRICK_WIDTH + BRICK_SPACING);

            for (int row = 0; row < BRICK_ROWS; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = col * (BRICK_WIDTH + BRICK_SPACING) + BRICK_START_X;
                    int y = row * (BRICK_HEIGHT + BRICK_SPACING) + BRICK_START_Y;
                    bricks.Add(new Brick(x, y, BRICK_WIDTH, BRICK_HEIGHT, GetBrickColor(row)));
                }
            }

            // Таймер
            gameTimer = new System.Windows.Forms.Timer { Interval = TIMER_INTERVAL_MS };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        /// <summary>
        /// цвет кирпича
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
                    platform.Bounds.X + PLATFORM_WIDTH / 2f,
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
            platform.Draw(e.Graphics);
            ball.Draw(e.Graphics);
            foreach (var brick in bricks)
            {
                brick.Draw(e.Graphics);
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

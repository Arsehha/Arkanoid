using Timer = System.Windows.Forms.Timer;

namespace Arkanoid.Forms
{
    /// <summary>
    /// Форма для отрисоки игры
    /// </summary>
    public partial class MainForm : Form
    {
        private GameController controller;
        private Timer timer;
        private Dictionary<Color, SolidBrush> brushCache = new();

        /// <summary>
        /// Создаёт главное окно игры, инициализирует контроллер и запускает игровой цикл.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            controller = new GameController();
            controller.Initialize(ClientSize);

            timer = new Timer();
            timer.Interval = 16;
            timer.Tick += GameTick;
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            try
            {
                controller.Update(ClientSize);
            }
            catch (Exception ex)
            {
                timer.Stop();

                if (ex.Message == "WIN")
                {
                    MessageBox.Show("Победа!");
                }
                else
                {
                    MessageBox.Show("Поражение!");
                }

                Application.Restart();
            }

            Invalidate();
        }

        /// <summary>
        /// Метод отрисовки
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Платформа
            g.FillRectangle(Brushes.Blue, controller.Platform.Bounds);

            // Мяч
            g.FillEllipse(Brushes.Red, controller.Ball.Bounds);

            // Кирпичи
            foreach (var brick in controller.Bricks)
            {
                if (!brick.IsDestroyed)
                {
                    if (!brushCache.TryGetValue(brick.Color, out var brush))
                    {
                        brush = new SolidBrush(brick.Color);
                        brushCache[brick.Color] = brush;
                    }

                    g.FillRectangle(brush, brick.Bounds);
                    g.DrawRectangle(Pens.Black, brick.Bounds);
                }
            }
        }

        /// <summary>
        /// Обрабатывает нажатие клавиш управления: движение платформы и запуск мяча.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                controller.MovePlatformLeft();
            }

            if (e.KeyCode == Keys.Right)
            {
                controller.MovePlatformRight(ClientSize.Width);
            }

            if (e.KeyCode == Keys.Space)
            {
                controller.LaunchBall();
            }
        }
    }
}

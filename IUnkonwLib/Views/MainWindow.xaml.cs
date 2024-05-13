using IUnkonwLib.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;

namespace IUnkonwLib.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int SnakeSquareSize = 20;
        const int SnakeStartLength = 3;
        const int SnakeStartSpeed = 400;
        const int SnakeSpeedThreshold = 100;

        private int snakeLength;
        private int currentScore = 0;
        private UIElement snakeFood = null;
        private Random rnd = new Random();
        private SolidColorBrush snakeBodyBrush = Brushes.Orange;
        private SolidColorBrush snakeHeadBrush = Brushes.IndianRed;
        private SolidColorBrush foodBrush = Brushes.Red;
        private List<SnakePart> snakeParts = new List<SnakePart>();
        private DispatcherTimer gameTickTimer = new DispatcherTimer();

        public enum SnakeDirection { Left, Right, Up, Down };
        private SnakeDirection snakeDirection = SnakeDirection.Up;

        public MainWindow()
        {
            InitializeComponent();
            gameTickTimer.Tick += GameTickTimer_Tick;
        }       

        private void DrawSnake()
        {
            foreach (SnakePart snakePart in snakeParts)
            {
                if (snakePart.UiElement == null)
                {
                    snakePart.UiElement = new Rectangle()
                    {
                        Width = SnakeSquareSize,
                        Height = SnakeSquareSize,
                        Fill = (snakePart.IsHead ? snakeHeadBrush : snakeBodyBrush),
                        RadiusX = 3, RadiusY = 3
                    };
                    GameArea.Children.Add(snakePart.UiElement);
                    Canvas.SetTop(snakePart.UiElement, snakePart.Position.Y);
                    Canvas.SetLeft(snakePart.UiElement, snakePart.Position.X);
                }
            }
        }

        private void StartNewGame()
        {
            snakeLength = SnakeStartLength;
            snakeDirection = SnakeDirection.Right;
            snakeParts.Add(new SnakePart() { Position = new Point(SnakeSquareSize * 5, SnakeSquareSize * 5) });
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);

            DrawSnake();

            gameTickTimer.IsEnabled = true;

            DrawFood();
        }
        
        private void MoveSnake()
        {
            while (snakeParts.Count >= snakeLength)
            {
                GameArea.Children.Remove(snakeParts[0].UiElement);
                snakeParts.RemoveAt(0);
            }

            foreach (var part in snakeParts)
            {
                (part.UiElement as Rectangle).Fill = snakeBodyBrush;
                part.IsHead = false;
            }
            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];

            double nextX = snakeHead.Position.X;
            double nextY = snakeHead.Position.Y;

            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    nextX -= SnakeSquareSize;
                    break;
                case SnakeDirection.Right:
                    nextX += SnakeSquareSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= SnakeSquareSize;
                    break;
                case SnakeDirection.Down:
                    nextY += SnakeSquareSize;
                    break;
            }

            snakeParts.Add(new SnakePart()
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });
            DrawSnake();
            DoCollisionCheck();
        }

        private void DoCollisionCheck()
        {
            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];
            if (snakeHead.Position.X == Canvas.GetLeft(snakeFood) && (snakeHead.Position.Y == Canvas.GetTop(snakeFood)))
            {
                EatSnakeFood();
                return;
            }

            if (snakeHead.Position.Y < 0 || snakeHead.Position.Y > GameArea.ActualHeight
                || snakeHead.Position.X < 0 || snakeHead.Position.X > GameArea.ActualWidth)
            {
                EndGame();
                return;
            }

            foreach (var part in snakeParts.Take(snakeParts.Count - 1))
            {
                if (snakeHead.Position.X == part.Position.X && snakeHead.Position.Y == part.Position.Y)
                {
                    EndGame();
                    return;
                }
            }

        }

        private void EndGame()
        {
            gameTickTimer.IsEnabled = false;
            GameArea.Children.Remove(snakeFood);
            scoreTbx.Text = "";
            currentScore = 0;
            DialogWindow dialogWindow = new DialogWindow();
            var res = dialogWindow.ShowPauseWindow($"Вы проиграли. Ваш счет: {currentScore}. Начать сначала?");
            if (res == Status.Сontinue)
            {
                gameTickTimer.IsEnabled = true;
            }
            else
            {
                Close();
            }
        }

        private void EatSnakeFood()
        {
            snakeLength++;
            currentScore++;
            int timerInterval = Math.Max(SnakeSpeedThreshold, (int)gameTickTimer.Interval.TotalMilliseconds - (currentScore * 2));
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            GameArea.Children.Remove(snakeFood);
            DrawFood();
            //UpdateGameStatus();
            scoreTbx.Text = currentScore.ToString();
        }

        private Point GetNextFoodPosition()
        {
            int maxX = (int)(GameArea.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameArea.ActualHeight / SnakeSquareSize);
            int foodX = rnd.Next(0, maxX) * SnakeSquareSize;
            int foodY = rnd.Next(0, maxY) * SnakeSquareSize;

            foreach (SnakePart snakePart in snakeParts)
            {
                if ((snakePart.Position.X == foodX) && (snakePart.Position.Y == foodY))
                    return GetNextFoodPosition();
            }

            return new Point(foodX, foodY);
        }

        private void DrawFood()
        {
            Point foodPosition = GetNextFoodPosition();
            snakeFood = new Ellipse()
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = foodBrush,
            };
            GameArea.Children.Add(snakeFood);
            Canvas.SetTop(snakeFood, foodPosition.Y);
            Canvas.SetLeft(snakeFood, foodPosition.X);                
        }

        #region Events        

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SnakeDirection originalSnakeDirection = snakeDirection;
            switch (e.Key)
            {
                case Key.W:
                    if (snakeDirection != SnakeDirection.Down)
                        snakeDirection = SnakeDirection.Up;
                    break;
                case Key.S:
                    if (snakeDirection != SnakeDirection.Up)
                        snakeDirection = SnakeDirection.Down;
                    break;
                case Key.A:
                    if (snakeDirection != SnakeDirection.Right)
                        snakeDirection = SnakeDirection.Left;
                    break;
                case Key.D:
                    if (snakeDirection != SnakeDirection.Left)
                        snakeDirection = SnakeDirection.Right;
                    break;
                case Key.Space:
                    StartNewGame();
                    break;
            }
            if (snakeDirection != originalSnakeDirection)
                MoveSnake();
        }

        private void StartPressed(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
        
        private void PausePressed(object sender, RoutedEventArgs e)
        {
            gameTickTimer.Stop();
            DialogWindow dialogWindow = new DialogWindow();
            var res = dialogWindow.ShowPauseWindow("Пауза");
            if (res == Status.Сontinue)
            {
                gameTickTimer.IsEnabled = true;
            }
            else
            {
                Close();
            }
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            StartNewGame();
        }

        #endregion Events
    }
}


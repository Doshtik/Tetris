using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameState gameState = new GameState();
        public string Language { get; private set; }

        private string _scoreText;
        
        //Поля, хранящие изображения клеток и фигур
        private readonly Image[,] imageControls;
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        //Поля для определения скорости падения фигур
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 100;
        private int delayDecrease = 25;
        public int DifficultModificator { get; set; }

        //Конструктор
        public MainWindow()
        {
            LeaderBoard.Name = "Player";
            InitializeComponent();
            Language = "eng";
            this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
            _scoreText = ScoreText.Text;
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }       
            }
            return imageControls;
        }

        //Методы отрисовки
        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }
        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }
        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];

        }
        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }
        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position pos in block.TilePositions())
            {
                imageControls[pos.Row + dropDistance, pos.Column].Opacity = 0.25;
                imageControls[pos.Row + dropDistance, pos.Column].Source = tileImages[block.Id];
            }
        }
        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);  
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            
            ScoreText.Text = $"{_scoreText}{gameState.Score}";
        }

        //Асинхронный метод, сдвигающий блоки вниз
        private async Task GameLoop()
        {
            //Первичная отрисовка поля
            Draw(gameState);

            //Для меню паузы
            while (!gameState.StartGame)
            {
                await Task.Delay(500);
            }
            StartGameMenu.Visibility = Visibility.Hidden;

            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.ClearedRows * delayDecrease));
                await Task.Delay(delay);
                while (gameState.PauseGame)
                {
                    PauseMenu.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                }
                PauseMenu.Visibility = Visibility.Hidden;
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            //Высчитывание результатов и внесение в таблицу
            User player = new User(LeaderBoard.Name, gameState.Score);
            try
            {
                //Список рекордсменов (Если нет - сработает обработка исключений)
                LeaderBoard.UpdateLeaderBoardList();
                //Позиция игрока в списке (Если игрока в списке нет - сработает отработка исключений)
                LeaderBoard.GetCurrentUser(out User playerInList, out int index);
                if (player.Score > playerInList.Score)
                {
                    LeaderBoard.RewriteLineInList(index, player);
                }
            }
            catch (Exception ex)
            {
                LeaderBoard.AddLineInList(player);
            }
            LeaderBoard.UpdateLeaderBoardList();

            //Для меню конца игры
            FinalScoreText.Text = $"{_scoreText}{gameState.Score}";
            GameOverMenu.Visibility = Visibility.Visible;
        }

        //Метод считывающий нажатия клавиц
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            if (gameState.PauseGame)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.C:
                    gameState.RotateBlcokCCW();
                    break;
                case Key.X:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                case Key.Escape:
                    gameState.PauseGame = true;
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        //Кнопки
        private void bttnStartGame_Click(object sender, RoutedEventArgs e)
        {
            gameState.StartGame = true;
        }
        private void bttnChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            //Немного неправильно каждый раз создавать и удалять массив язывков
            List<string> languages = new List<string> { "rus", "eng"};
            
            if (Language == languages[languages.Count - 1])
            {
                Language = languages.First();
            }
            else
            {
                Language = languages[languages.LastIndexOf(Language) + 1];
            }
            
            switch (Language)
            {
                case "rus":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
                    break;
                case "eng":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
                    break;
            }

            //Получает значение сеттера стиля ScoreText
            Style style = this.FindResource("ScoreText") as Style;
            Setter textSetter = style.Setters.OfType<Setter>().First();

            _scoreText = textSetter.Value.ToString();

            ScoreText.Text = $"{_scoreText}{gameState.Score}";
            FinalScoreText.Text = $"{_scoreText}{gameState.Score}";
        }
        private void bttnHowToPlay_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("index.html");
        }
        private void bttnContinue_Click(object sender, RoutedEventArgs e)
        {
            gameState.PauseGame = false;
        }
        private void bttnSettings_Click(object sender, RoutedEventArgs e)
        {

        }
        private void bttnOpenLB_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                LeaderBoard.UpdateLeaderBoardList();

                LeaderBoardTopFive.Items.Clear();
                for (int i = 0; i < 5; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Foreground = Brushes.Black;
                    textBlock.Margin = new Thickness(10, 5, 5, 5);
                    textBlock.Text = $"{i + 1}) {LeaderBoard.PlayerList[i].Field}";
                    item.Content = textBlock;
                    LeaderBoardTopFive.Items.Add(item);
                }

                LeaderBoard.GetCurrentUser(out User playerInList, out int index);
                LeaderBoardCurrentPosition.Text = $"{index + 1}) {playerInList.Field}";

                LeaderBoardMenu.Visibility = Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }
        private void bttnCloseLB_Click(object sender, RoutedEventArgs e)
        {
            LeaderBoardMenu.Visibility = Visibility.Hidden;
        }
        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void bttnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            gameState.StartGame = true;
            await GameLoop();
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tetris.Frames;
using Tetris.Blocks;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.IO;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Поля и свойства
        public static readonly List<string> Languages = new List<string> { "rus", "eng" };
        public static string DictLanguage { get; set; }
        private GameState gameState = new GameState();
        private string _scoreText; //нужно исключительно в случае, когда пользователь меняет язык в меню паузы

        private CancellationTokenSource _cts;
        private Task _gameLoopTask;

        #region Поля, хранящие изображения клеток и фигур
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
        #endregion

        #region Поля для определения скорости падения фигур
        private readonly int _maxDelay = 1000;
        private int _minDelay = 100;
        private int _delayDecrease = 25;
        #endregion

        private bool _isKeyDownPressed = false;
        public static bool IsGamePaused { get; set; }
        public static bool IsGameStarted { get; set; } = false;
        #endregion

        #region Конструктор
        public MainWindow()
        {
            if (!File.Exists("config.txt"))
                SettingsMenu.CreateConfigFile("config.txt");

            InitializeComponent();

            MainWindow.DictLanguage = "rus";
            this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
            _scoreText = ScoreText.Text;
            imageControls = InitGameCanvas(gameState.GameGrid);
            MainFrame.Content = new StartGameMenu();
        }
        #endregion

        #region Метод инициализации Canvas
        private Image[,] InitGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
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
        #endregion

        #region Методы отрисовки
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
        #endregion

        #region Асинхронный метод, сдвигающий блоки вниз (И по факту основная игра)
        private async Task GameLoop(CancellationToken token)
        {
            try
            {
                Draw(gameState);
                
                //В главном меню
                while (!IsGameStarted)
                {
                    await Task.Delay(500);
                }
                SetLanguage();
                
                GameField.Visibility = Visibility.Visible;

                int difficulty = SettingsMenu.DifficultyModificator;
                
                //В игре
                while (!gameState.GameOver && !token.IsCancellationRequested)
                {
                    //Подсчет времени падения
                    int delay = Math.Max(_minDelay, _maxDelay - (gameState.ClearedRows * _delayDecrease * difficulty));
                    await Task.Delay(delay);

                    //В меню паузы
                    while (IsGamePaused && !token.IsCancellationRequested)
                    {
                        PauseMenu.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        difficulty = SettingsMenu.DifficultyModificator;
                    }
                    PauseMenu.Visibility = Visibility.Hidden;

                    //Чтобы при нажатии в тайминг блок дважды вниз не спускался
                    if (_isKeyDownPressed)
                    {
                        _isKeyDownPressed = false;
                    }
                    else
                    {
                        gameState.MoveBlockDown();
                    }

                    Draw(gameState);
                }
            }
            catch (OperationCanceledException) { } //Необходимо при перезапуске игры с меню паузы
            
            WindowRegistration window = new WindowRegistration();
            window.Owner = this;
            window.Show();

            //Для меню конца игры
            FinalScoreText.Text = $"{_scoreText}{gameState.Score}";
            GameOverMenu.Visibility = Visibility.Visible;
        }
        #endregion

        #region Методы GameLoop
        public void StartGameLoop()
        {
            if (_gameLoopTask == null || _gameLoopTask.IsCompleted)
            {
                _cts = new CancellationTokenSource(); // Создаем новый источник токена
                _gameLoopTask = GameLoop(_cts.Token); // Запускаем асинхронную задачу
            }
        }
        public async Task StopGameLoop()
        {
            if (_cts != null)
            {
                _cts.Cancel(); // Отменяем выполнение
                try
                {
                    await _gameLoopTask; // Дожидаемся завершения текущего цикла игры
                }
                catch (OperationCanceledException) { }
            }
        }
        #endregion

        #region Метод считывающий нажатия клавиц
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver || IsGamePaused)
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
                    _isKeyDownPressed = true;
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
                    IsGamePaused = true;
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }
        #endregion

        #region Метод запускающий отрисровку
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            StartGameLoop();
        }
        #endregion

        #region Метод смены языка
        public void SetLanguage()
        {
            switch (MainWindow.DictLanguage)
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
        #endregion

        #region Кнопки
        private void bttnChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (DictLanguage == MainWindow.Languages[MainWindow.Languages.Count - 1])
            {
                DictLanguage = MainWindow.Languages.First();
            }
            else
            {
                DictLanguage = MainWindow.Languages[MainWindow.Languages.LastIndexOf(DictLanguage) + 1];
            }

            SetLanguage();
        }
        private void bttnContinue_Click(object sender, RoutedEventArgs e)
        {
            IsGamePaused = false;
        }
        private void bttnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SettingsMenu();
        }
        private void bttnOpenLB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Высчитывание результатов и внесение в таблицу
                User player = new User(SettingsMenu.Name, gameState.Score);
                try
                {
                    //Список рекордсменов (Если нет - сработает обработка исключений)
                    LeaderBoard.UpdateLeaderBoardList();
                    //Позиция игрока в списке (Если игрока в списке нет - сработает отработка исключений)
                    LeaderBoard.GetCurrentUser(out User playerInList1, out int index1);
                    if (player.Score > playerInList1.Score)
                    {
                        LeaderBoard.RewriteLineInList(index1, player);
                    }
                }
                catch
                {
                    LeaderBoard.AddLineInList(player);
                }
                LeaderBoard.UpdateLeaderBoardList();

                LeaderBoardMenu leaderBoardMenu = new LeaderBoardMenu();

                leaderBoardMenu.LeaderBoardTopFive.Items.Clear();

                if (LeaderBoard.PlayerList.Count < 5) 
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        User user = new User("player" + i, 2500 * i);
                        LeaderBoard.AddLineInList(user);
                    }
                    LeaderBoard.UpdateLeaderBoardList();
                }

                for (int i = 0; i < 5; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Foreground = Brushes.Black;
                    textBlock.Margin = new Thickness(10, 5, 5, 5);
                    textBlock.Text = $"{i + 1}) {LeaderBoard.PlayerList[i].Field}";
                    item.Content = textBlock;
                    leaderBoardMenu.LeaderBoardTopFive.Items.Add(item);
                }

                LeaderBoard.GetCurrentUser(out User playerInList2, out int index2);
                leaderBoardMenu.LeaderBoardCurrentPosition.Text = $"{index2 + 1}) {playerInList2.Field}";

                MainFrame.Content = leaderBoardMenu;
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }
        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private async void bttnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            await StopGameLoop();
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            PauseMenu.Visibility = Visibility.Hidden;
            IsGamePaused = false;
            StartGameLoop();
        }
        private void bttnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            IsGameStarted = false;
            bttnPlayAgain_Click(sender, e);
            MainFrame.Content = new StartGameMenu();
        }
        #endregion
    }
}
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlockQueue blockQueue;
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;

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

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 30;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
            blockQueue = new BlockQueue();

            backgroundMusic = new MediaElement
            {
                Source = new Uri("Assets\\Music.mp3", UriKind.RelativeOrAbsolute),
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            backgroundMusic.MediaOpened += BackgroundMusic_MediaOpened;
            backgroundMusic.Volume = 0.3;

            // Load line clear sound effect (manual)
            lineClearSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            lineClearSound.Source = new Uri("Assets\\clearsfx.mp3", UriKind.RelativeOrAbsolute);
            lineClearSound.MediaOpened += LineClearSound_MediaOpened;

            RotateSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            RotateSound.Source = new Uri("Assets\\rotate.mp3", UriKind.RelativeOrAbsolute);
            RotateSound.MediaOpened += RotationSound_MediaOpened;

            PlaceSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            PlaceSound.Source = new Uri("Assets\\place.mp3", UriKind.RelativeOrAbsolute);
            PlaceSound.MediaOpened += PlaceSound_MediaOpened;

            gameOverSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            gameOverSound.Source = new Uri("Assets\\gameoversfx.mp3", UriKind.RelativeOrAbsolute);
            gameOverSound.MediaOpened += GameOverSound_MediaOpened;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            timer.Tick += Timer_Tick;

            // Start the timer when the game starts
            StartTimer();



        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime += TimeSpan.FromSeconds(1);
            ElapsedTimeText.Text = $"Time: {elapsedTime:mm\\:ss}";
        }

        private void StartTimer()
        {
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
        }

        private void GameOverSound_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Do nothing in this case, you can leave it empty
        }

        private void PlaceSound_MediaOpened(object sender, RoutedEventArgs e)
        { 
        }


        private void RotationSound_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Do nothing in this case, you can leave it empty
        }

        private void BackgroundMusic_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Start playing background music when it's loaded
            backgroundMusic.Play();
        }

        private void LineClearSound_MediaOpened(object sender, RoutedEventArgs e)
        {
            
        }

        private void PlayLineClearSound()
        {
            if (lineClearSound != null)
            {
                lineClearSound.Position = TimeSpan.Zero;
                lineClearSound.Play();
            }
        }

        private void BackgroundMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Rewind and play the background music when it reaches the end
            backgroundMusic.Position = TimeSpan.Zero;
            backgroundMusic.Play();
        }




        private Image[,] SetupGameCanvas(GameGrid grid)
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

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }    
        }






        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private bool nextButtonClicked = false;

        private int currentImageIndex = 0;
        private string[] blockImagePaths = new string[]
        {
            "Assets/Block-I.png",
            "Assets/Block-J.png",
            "Assets/Block-L.png",
            "Assets/Block-O.png",
            "Assets/Block-S.png",
            "Assets/Block-T.png",
            "Assets/Block-Z.png"
        };






        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the shuffling image source
            ShufflingImage.Source = new BitmapImage(new Uri(blockImagePaths[currentImageIndex], UriKind.RelativeOrAbsolute));

            // Increment index for the next shuffle
            currentImageIndex = (currentImageIndex + 1) % blockImagePaths.Length;
        }

        private bool isShuffleMode = false;







        private async Task GameLoop()
        {
            Draw(gameState);
            gameState.PlaceInitialBlock();

            while (!gameState.GameOver)
            {
                await Task.Delay(100);

                if (nextButtonClicked)
                {
                    // Display the shuffle screen
                    ShowShuffleScreen();
                }
                else
                {
                    gameState.MoveBlockDown();
                    Draw(gameState);
                }

                StopTimer();
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
            gameOverSound.Position = TimeSpan.Zero;
            gameOverSound.Play();
        }

        private void ShowShuffleScreen()
        {
            // Hide Tetris grid and show shuffle screen
            GameCanvas.Visibility = Visibility.Hidden;
            ShuffleScreen.Visibility = Visibility.Visible;

            // Reset the index for shuffling
            currentImageIndex = 0;

            // Start shuffling timer
            StartShufflingTimer();
        }

        private async void StartShufflingTimer()
        {
            while (!nextButtonClicked && currentImageIndex < blockImagePaths.Length)
            {
                // Update shuffling image
                ShufflingImage.Source = new BitmapImage(new Uri(blockImagePaths[currentImageIndex], UriKind.RelativeOrAbsolute));

                // Increment index for the next shuffle
                currentImageIndex++;

                await Task.Delay(200);  // Adjust the delay if needed
            }

            // Reset next button click
            nextButtonClicked = false;

            // Hide shuffle screen and show Tetris grid
            ShuffleScreen.Visibility = Visibility.Hidden;
            GameCanvas.Visibility = Visibility.Visible;
        }


        private async Task EnterShuffleMode()
        {
            // Disable Tetris grid and show shuffle screen
            GameCanvas.Visibility = Visibility.Hidden;
            ShuffleScreen.Visibility = Visibility.Visible;

            // Implement the shuffle logic here
            await ShuffleBlocks();

            // Hide shuffle screen and show Tetris grid
            GameCanvas.Visibility = Visibility.Visible;
            ShuffleScreen.Visibility = Visibility.Hidden;
        }

        private async Task ShuffleBlocks()
        {
            // Implement your shuffle logic here
            // This could involve animating the shuffling image and randomly selecting the next block
            // Once the shuffle is complete, update the gameState with the selected block

            // For demonstration purposes, let's simulate a delay
            await Task.Delay(3000);

            // For demonstration purposes, let's select a random block after shuffling
            gameState.BlockQueue.ShuffleBlocks();
            gameState.BlockQueue.RollDice();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            if (e.Key == Key.Space && !ShuffleButton.IsFocused)
            {
                gameState.DropBlock();
                blockDroppedAfterNext = true; // Set the flag when dropping with space key
            }
            else if (!nextButtonClicked && autoFall) // Check if the block should fall automatically
            {
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
                    case Key.Z:
                        gameState.RotateBlockCCW();
                        break;
                    case Key.LeftShift:
                        gameState.HoldBlock();
                        break;
                    default:
                        return;
                }
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e )
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            backgroundMusic.Close();
            lineClearSound.Close();
        }


   
    }
}
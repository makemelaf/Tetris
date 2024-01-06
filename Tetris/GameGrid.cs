using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Tetris
{
    public class GameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        public int this [int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
           
        }

        public bool IsInside(int r, int c)
        {
            return r>= 0 && r < Rows && c>= 0 && c < Columns;
        }

        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }

        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }

            return true;
        }


        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r, c] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c=0; c<Columns;c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;

                    PlayLineClearSound();
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }
            return cleared;
        }
        public MediaElement lineClearSound;
        public void PlayLineClearSound()
        {
            lineClearSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            lineClearSound.Source = new Uri("Assets\\clearsfx.mp3", UriKind.RelativeOrAbsolute);
            lineClearSound.MediaOpened += LineClearSound_MediaOpened;


            // Assuming lineClearSound is a MediaElement configured for line clear sound
            lineClearSound.Position = TimeSpan.Zero;
            lineClearSound.Play();
            lineClearSound.Volume = 3;


        }

        private void LineClearSound_MediaOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Tetris
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public MediaElement RotateSound;

        public void RotateCW()
        {
            RotateSound = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            RotateSound.Source = new Uri("Assets\\rotate.mp3", UriKind.RelativeOrAbsolute);
            RotateSound.MediaOpened += RotationSound_MediaOpened;
            rotationState = (rotationState + 1) % Tiles.Length;
            RotateSound.Position = TimeSpan.Zero;
            RotateSound.Play();
        }

        private void RotationSound_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Do nothing in this case, you can leave it empty
        }
        public void RotateCCW()
        {
            rotationState = (rotationState - 1 + Tiles.Length) % Tiles.Length;
            RotateSound.Position = TimeSpan.Zero;
           RotateSound.Play();
        }

        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }
    }
}

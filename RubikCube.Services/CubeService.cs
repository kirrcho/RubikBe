using RubikCube.Common;
using RubikCube.Core.Models;
using RubikCube.Services.Interfaces;

namespace RubikCube.Services
{
    public class CubeService : ICubeService
    {
        private KeyValuePair<FaceDirection, FaceDirection>[] RotationCycleOne =
        [
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Front, FaceDirection.Right),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Right, FaceDirection.Bottom),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Bottom, FaceDirection.Left),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Left, FaceDirection.Front)
        ];
        private KeyValuePair<FaceDirection, FaceDirection>[] RotationCycleTwo =
        [
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Right, FaceDirection.Up),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Up, FaceDirection.Left),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Left, FaceDirection.Down),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Down, FaceDirection.Right)
        ];
        private KeyValuePair<FaceDirection, FaceDirection>[] RotationCycleThree =
        [
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Front, FaceDirection.Up),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Up, FaceDirection.Bottom),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Bottom, FaceDirection.Down),
            new KeyValuePair<FaceDirection, FaceDirection>(FaceDirection.Down, FaceDirection.Front)
        ];

        public CubeService() { }

        /// <summary>
        /// Generates a cube based on the provided length.
        /// </summary>
        /// <param name="length">The length of the matrix.</param>
        /// <returns>Generated cube.</returns>
        public Result<IEnumerable<Square>> Generate(int length)
        {
            if (length < 3 || length > 10)
            {
                return Result<IEnumerable<Square>>.BadResult("Invalid cube size. The cube must be between 3 and 10 rows.");
            }

            var squares = new List<Square>
            {
                new Square(FaceDirection.Front),
                new Square(FaceDirection.Right),
                new Square(FaceDirection.Up),
                new Square(FaceDirection.Bottom),
                new Square(FaceDirection.Left),
                new Square(FaceDirection.Down),
            };

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.Green, (row * length * 4) + col, -1, (col * length * 4) + length - row - 1);

                    squares[0].Cells.Add(cell);
                }
            }

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.Red, (row * length * 4) + length + col, (col * length * 4) + length - row - 1, -1);

                    squares[1].Cells.Add(cell);
                }
            }

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.White, -1, ((length - row - 1) * length * 4) + length * 2 - col - 1, (col * length * 4) + length * 2 - row - 1);

                    squares[2].Cells.Add(cell);
                }
            }

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.Blue, (length * row * 4) + length * 2 + col, -1, ((length - col - 1) * length * 4) + length * 2 + row);

                    squares[3].Cells.Add(cell);
                }
            }

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.Orange, (length * row * 4) + length * 3 + col, ((length - col - 1) * length * 4) + length * 2 + row, -1);

                    squares[4].Cells.Add(cell);
                }
            }

            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    var cell = new Cell(row, col, Color.Yellow, -1, (length * row * 4) + length * 3 + col, (col * length * 4) + length * 4 - row - 1);

                    squares[5].Cells.Add(cell);
                }
            }

            return Result<IEnumerable<Square>>.OkResult(squares);
        }

        /// <summary>
        /// Sets up the rotation based on the direction of the swipe.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Generated cube.</returns>
        public Result<IEnumerable<Square>> Rotate(RotateCubeRequest request)
        {
            var isValid = IsValidCube(request.CubeData, request.Length);

            if (!isValid)
            {
                return Result<IEnumerable<Square>>.BadResult("Invalid cube.");
            }

            if (RotationCycleOne.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.startSwipeDirection, request.endSwipeDirection)))
            {
                var startingIncrement = request.Cell.FirstRotationNumber - (request.Cell.FirstRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleOne, startingIncrement, RotationType.FirstRotation, request.Length, false);
            }
            else if (RotationCycleOne.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.endSwipeDirection, request.startSwipeDirection)))
            {
                var startingIncrement = request.Cell.FirstRotationNumber - (request.Cell.FirstRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleOne, startingIncrement, RotationType.FirstRotation, request.Length, true);
            }
            else if (RotationCycleTwo.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.startSwipeDirection, request.endSwipeDirection)))
            {
                var startingIncrement = request.Cell.SecondRotationNumber - (request.Cell.SecondRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleTwo, startingIncrement, RotationType.SecondRotation, request.Length, false);
            }
            else if (RotationCycleTwo.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.endSwipeDirection, request.startSwipeDirection)))
            {
                var startingIncrement = request.Cell.SecondRotationNumber - (request.Cell.SecondRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleTwo, startingIncrement, RotationType.SecondRotation, request.Length, true);
            }
            else if (RotationCycleThree.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.startSwipeDirection, request.endSwipeDirection)))
            {
                var startingIncrement = request.Cell.ThirdRotationNumber - (request.Cell.ThirdRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleThree, startingIncrement, RotationType.ThirdRotation, request.Length, false);
            }
            else if (RotationCycleThree.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.endSwipeDirection, request.startSwipeDirection)))
            {
                var startingIncrement = request.Cell.ThirdRotationNumber - (request.Cell.ThirdRotationNumber % (request.Length * 4));

                RotateCubeColors(request.CubeData, RotationCycleThree, startingIncrement, RotationType.ThirdRotation, request.Length, true);
            }

            return Result<IEnumerable<Square>>.OkResult(request.CubeData);
        }

        /// <summary>
        /// Swaps the colors between the cells in the provided direction.
        /// </summary>
        /// <param name="cubeData">The cube.</param>
        /// <param name="rotationCycle">The cycle that the row/col should be rotated through.</param>
        /// <param name="indexIncrement">Increment that shows which row/col should the rotation 
        /// be inferred to based on the rotation indexes.</param>
        /// <param name="rotationType">The rotation swipe direction.</param>
        /// <param name="length">Length of cube.</param>
        /// <param name="isInverted">Checks if the direction is forward or backward.</param>
        /// <exception cref="ArgumentException"></exception>
        private void RotateCubeColors(
            List<Square> cubeData,
            KeyValuePair<FaceDirection, FaceDirection>[] rotationCycle,
            int indexIncrement,
            RotationType rotationType,
            int length,
            bool isInverted)
        {
            for (int i = 0; i < rotationCycle.Length - 1; i++)
            {
                var rotationNumber = isInverted ? i : rotationCycle.Length - 2 - i;

                var startSwapTargetDirection = rotationCycle[rotationNumber].Key;
                var endSwapTargetDirection = rotationCycle[rotationNumber].Value;

                var startSquare = cubeData.First(cd => cd.Direction == startSwapTargetDirection);
                var endSquare = cubeData.First(cd => cd.Direction == endSwapTargetDirection);

                for (int k = 0; k < length; k++)
                {
                    Cell startCell, endCell;

                    var startRotationIndex = k + indexIncrement + (rotationNumber * length);
                    var endRotationIndex = k + indexIncrement + (rotationNumber * length) + length;

                    switch (rotationType)
                    {
                        case RotationType.FirstRotation:
                            startCell = startSquare.Cells.First(c => c.FirstRotationNumber == startRotationIndex);
                            endCell = endSquare.Cells.First(c => c.FirstRotationNumber == endRotationIndex);
                            break;
                        case RotationType.SecondRotation:
                            startCell = startSquare.Cells.First(c => c.SecondRotationNumber == startRotationIndex);
                            endCell = endSquare.Cells.First(c => c.SecondRotationNumber == endRotationIndex);
                            break;
                        case RotationType.ThirdRotation:
                            startCell = startSquare.Cells.First(c => c.ThirdRotationNumber == startRotationIndex);
                            endCell = endSquare.Cells.First(c => c.ThirdRotationNumber == endRotationIndex);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    var previousColor = startCell.Color;
                    startCell.Color = endCell.Color;
                    endCell.Color = previousColor;
                }
            }
        }

        /// <summary>
        /// Checks whether the provided cube has correct row/col count.
        /// </summary>
        /// <param name="cubeData">The cube.</param>
        /// <param name="length">The size of the cube.</param>
        private bool IsValidCube(List<Square> cubeData, int length)
        {
            foreach (var square in cubeData)
            {
                if (square.Cells.Count != length * length)
                {
                    return false;
                }

                var cellMatrix = new Cell[length, length];

                foreach (var cell in square.Cells)
                {
                    if (cell.Row < 0 || cell.Row >= length || cell.Column < 0 || cell.Column >= length)
                    {
                        return false;
                    }

                    if (cellMatrix[cell.Row, cell.Column] != null)
                    {
                        return false;
                    }

                    cellMatrix[cell.Row, cell.Column] = cell;
                }

                for (int row = 0; row < length; row++)
                {
                    for (int col = 0; col < length; col++)
                    {
                        if (cellMatrix[row, col] == null)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
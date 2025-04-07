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

            var maxLengthForExtraRotation = request.Length * 4;
            var minLengthForExtraRotation = request.Length * 4 * (request.Length - 1);

            FaceDirection? extraRotation = null;
            bool isExtraRotationInverted = false;

            var rotationCycles = new[]
            {
                new { Cycle = RotationCycleOne, RotationType = RotationType.FirstRotation, GetRotationNumber = (Func<Cell, int>)(cell => cell.FirstRotationNumber) },
                new { Cycle = RotationCycleTwo, RotationType = RotationType.SecondRotation, GetRotationNumber = (Func<Cell, int>)(cell => cell.SecondRotationNumber) },
                new { Cycle = RotationCycleThree, RotationType = RotationType.ThirdRotation, GetRotationNumber = (Func<Cell, int>)(cell => cell.ThirdRotationNumber) }
            };

            foreach (var rotationCycle in rotationCycles)
            {
                var cycle = rotationCycle.Cycle;

                if (cycle.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.startSwipeDirection, request.endSwipeDirection)) ||
                    cycle.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.endSwipeDirection, request.startSwipeDirection)))
                {
                    var rotationNumber = rotationCycle.GetRotationNumber(request.Cell);
                    var rotationType = rotationCycle.RotationType;

                    var startingIncrement = rotationNumber - (rotationNumber % (request.Length * 4));
                    bool isInverted = cycle.Contains(new KeyValuePair<FaceDirection, FaceDirection>(request.endSwipeDirection, request.startSwipeDirection));

                    if (rotationNumber < maxLengthForExtraRotation)
                    {
                        extraRotation = rotationType switch
                        {
                            RotationType.FirstRotation => FaceDirection.Up,
                            RotationType.SecondRotation => FaceDirection.Front,
                            RotationType.ThirdRotation => FaceDirection.Left,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        isExtraRotationInverted = isInverted;
                    }
                    else if (rotationNumber >= minLengthForExtraRotation)
                    {
                        extraRotation = rotationType switch
                        {
                            RotationType.FirstRotation => FaceDirection.Down,
                            RotationType.SecondRotation => FaceDirection.Bottom,
                            RotationType.ThirdRotation => FaceDirection.Right,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        isExtraRotationInverted = !isInverted;
                    }

                    RotateCubeColors(request.CubeData, rotationCycle.Cycle, startingIncrement, rotationCycle.RotationType, request.Length, extraRotation, isExtraRotationInverted, isInverted);
                    break;
                }
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
        /// <param name="extraRotation">The square that should be rotated when movement is on the edge of cube</param>
        /// <param name="isExtraRotationInverted">Whether the movement is 90 degrees clockwise or counterClockwise</param>
        private void RotateCubeColors(
            List<Square> cubeData,
            KeyValuePair<FaceDirection, FaceDirection>[] rotationCycle,
            int indexIncrement,
            RotationType rotationType,
            int length,
            FaceDirection? extraRotation,
            bool isExtraRotationInverted,
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

            if (extraRotation != null)
            {
                RotateEdgeSquare(cubeData, length, extraRotation, isExtraRotationInverted);
            }
        }

        /// <summary>
        /// Rotates the square that is completely surrounded by the row/col movement.
        /// </summary>
        /// <param name="cubeData">The cube.</param>
        /// <param name="length">Length of cube.</param>
        /// <param name="extraRotation">The square that should be rotated when movement is on the edge of cube</param>
        /// <param name="isExtraRotationInverted">Whether the movement is 90 degrees clockwise or counterClockwise</param>
        private void RotateEdgeSquare(List<Square> cubeData, int length, FaceDirection? extraRotation, bool isExtraRotationInverted)
        {
            var squareToRotate = cubeData.First(cd => cd.Direction == extraRotation);
            var cellsToRotateCopy = new List<Cell>();

            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    var rowToSwap = isExtraRotationInverted ? length - j - 1 : j;
                    var colToSwap = isExtraRotationInverted ? i : length - i - 1;

                    var startCell = squareToRotate.Cells.First(cell => cell.Row == i && cell.Column == j);
                    var cellToSwap = squareToRotate.Cells.First(cell => cell.Row == rowToSwap && cell.Column == colToSwap);

                    var newCellCopy = new Cell(
                        startCell.Row,
                        startCell.Column,
                        cellToSwap.Color,
                        startCell.FirstRotationNumber,
                        startCell.SecondRotationNumber,
                        startCell.ThirdRotationNumber);

                    cellsToRotateCopy.Add(newCellCopy);
                }
            }

            squareToRotate.Cells.Clear();
            squareToRotate.Cells.AddRange(cellsToRotateCopy);
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
namespace RubikCube.Core.Models
{
    public class Cell
    {
        public Cell(
            int row,
            int column,
            Color color,
            int firstRotationNumber,
            int secondRotationNumber,
            int thirdRotationNumber)
        {
            Row = row;
            Column = column;
            Color = color;
            FirstRotationNumber = firstRotationNumber;
            SecondRotationNumber = secondRotationNumber;
            ThirdRotationNumber = thirdRotationNumber;
        }

        /// <summary>
        /// The row of the cell inside the square.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// the column of the cell inside the square.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The color of the cell.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The index of the rotation that starts from front to up. Generated number that
        /// provides information about which cell color to change.
        /// </summary>
        public int FirstRotationNumber { get; set; }

        /// <summary>
        /// The index of the rotation that starts from right to up. Generated number that
        /// provides information about which cell color to change.
        /// </summary>
        public int SecondRotationNumber { get; set; }

        /// <summary>
        /// The index of the rotation that starts from front to right. Generated number that
        /// provides information about which cell color to change.
        /// </summary>
        public int ThirdRotationNumber { get; set; }
    }
}
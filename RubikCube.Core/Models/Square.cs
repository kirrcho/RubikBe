namespace RubikCube.Core.Models
{
    public class Square
    {
        public Square(FaceDirection direction)
        {
            Direction = direction;
            Cells = new List<Cell>();
        }

        /// <summary>
        /// The direction the square points to.
        /// </summary>
        public FaceDirection Direction { get; set; }

        public List<Cell> Cells { get; set; }
    }
}
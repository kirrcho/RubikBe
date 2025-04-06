namespace RubikCube.Core.Models
{
    public class RotateCubeRequest
    {
        /// <summary>
        /// The list of squares that represent the cube.
        /// </summary>
        public List<Square> CubeData { get; set; }

        /// <summary>
        /// The size of the rubik's cube.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The cell that represents the line to be rotated.
        /// </summary>
        public Cell Cell { get; set; }

        /// <summary>
        /// The starting direction of the cell.
        /// </summary>
        public FaceDirection startSwipeDirection { get; set; }

        /// <summary>
        /// The square which points to the direction of the rotation.
        /// </summary>
        public FaceDirection endSwipeDirection { get; set; }
    }
}
using RubikCube.Common;
using RubikCube.Core.Models;

namespace RubikCube.Services.Interfaces
{
    public interface ICubeService
    {
        Result<IEnumerable<Square>> Generate(int length);

        Result<IEnumerable<Square>> Rotate(RotateCubeRequest request);
    }
}
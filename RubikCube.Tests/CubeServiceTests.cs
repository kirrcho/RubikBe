using RubikCube.Services;
using RubikCube.Services.Interfaces;

namespace RubikCube.Tests
{
    [TestClass]
    public sealed class CubeServiceTests
    {
        private readonly ICubeService _cubeService;

        public CubeServiceTests()
        {
            _cubeService = new CubeService();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(20)]
        public void Generate_InvalidCube_ReturnsError(int invalidLength)
        {
            var result = _cubeService.Generate(invalidLength);

            Assert.IsFalse(result.IsSuccessful);
        }

        [TestMethod]
        public void Generate_ValidCube()
        {
            var validLength = 5;

            var result = _cubeService.Generate(validLength);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Value);

            var cubeSides = 6;
            Assert.AreEqual(result.Value.Count(), cubeSides);

            Assert.IsNotNull(result.Value.First().Cells);
            Assert.AreEqual(result.Value.First().Cells.Count, validLength * validLength);
        }
    }
}

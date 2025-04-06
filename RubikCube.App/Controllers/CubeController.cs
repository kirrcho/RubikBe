using Microsoft.AspNetCore.Mvc;
using RubikCube.Core.Models;
using RubikCube.Services.Interfaces;

namespace RubikCube.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CubeController : ControllerBase
    {
        private readonly ICubeService _cubeMovementService;

        public CubeController(ICubeService cubeMovementService)
        {
            _cubeMovementService = cubeMovementService;
        }

        /// <summary>
        /// Generates a cube with the specified number of rows.
        /// </summary>
        /// <param name="length">Size of rubik's matrix.</param>
        /// <returns>Generated rubik cube.</returns>
        [ProducesResponseType(typeof(IEnumerable<Square>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet("generate")]
        public IActionResult GetDefaultCube(int length)
        {
            var result = _cubeMovementService.Generate(length);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Rotates the cube based on the provided directions.
        /// </summary>
        /// <param name="request">RotateCubeRequest.</param>
        /// <returns>Rubik cube after rotation.</returns>
        [ProducesResponseType(typeof(IEnumerable<Square>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("rotate")]
        public IActionResult RotateCube([FromBody] RotateCubeRequest request)
        {
            var result = _cubeMovementService.Rotate(request);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
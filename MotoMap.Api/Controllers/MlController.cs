using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.ML;

namespace MotoMap.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ml")]
public class MlController : ControllerBase
{
    private readonly MotoStatePredictor _predictor;

    public MlController(MotoStatePredictor predictor) => _predictor = predictor;

    [HttpPost("predict")]
    public IActionResult Predict([FromBody] ModelInput input)
    {
        var res = _predictor.Predict(input);
        return Ok(new { predicted = res.PredictedLabel, score = res.Score });
    }
}

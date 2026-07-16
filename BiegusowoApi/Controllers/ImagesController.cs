using BiegusowoApi.Domain.Image;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    public async Task<ActionResult<PresignedUploadResponse>> UploadPresigned(
        [FromBody] PresignedUploadRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("confirm/{tableName}/{id}")]
    public async Task<IActionResult> ConfirmUpload([FromRoute] string tableName, [FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}

using BiegusowoApi.Domain.Image;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    [HttpPost("presigned")]
    public async Task<ActionResult<PresignedUploadResponse>> UploadPresigned(
        [FromBody] List<PresignedUploadRequest> request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("confirm/{tableName}/{id}")]
    public async Task<IActionResult> ConfirmUpload(
       [FromRoute] string tableName,
       [FromRoute] string id,
       [FromBody] List<string> keys)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{tableName}/{imageId}")]
    public async Task<IActionResult> DeleteImage(
        [FromRoute] string tableName, [FromRoute] string imageId)
    {
        throw new NotImplementedException();
    }
}

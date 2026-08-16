using FluentValidation;

namespace BiegusowoApi.Features.Blobs.Dtos;

public record PresignedUploadRequest(
    List<PresignedUploadFile> Files
    );

public record PresignedUploadFile(
    string FileName,
    string ContentType,
    int FileSizeBytes,
    int FileWidth,
    int FileHeight
    );

public class PresignedUploadFileValidator : AbstractValidator<PresignedUploadFile>
{
    public PresignedUploadFileValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.ContentType)
            .Equal("image/webp")
            .MaximumLength(100);
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(25 * 1024 * 1024); // 25 MB
        RuleFor(x => x.FileWidth)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000); // Arbitrary max width
        RuleFor(x => x.FileHeight)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000); // Arbitrary max height
    }
}
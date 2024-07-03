namespace ChatServiceV2.Dtos
{
    public sealed record RegisterDto(
    string Name,
    IFormFile File);
}

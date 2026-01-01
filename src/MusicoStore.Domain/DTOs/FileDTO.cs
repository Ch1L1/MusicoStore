namespace MusicoStore.Domain.DTOs;

public class FileDTO
{
    public Stream Content { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}

namespace Chat.Domain.Dto;

public class Music
{
    public string Artist { get; set; } = null!;

    public string Album { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Duration { get; set; } = 0!;
}
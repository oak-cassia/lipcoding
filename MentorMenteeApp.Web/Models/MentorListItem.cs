namespace MentorMenteeApp.Web.Models;

public class MentorListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
}

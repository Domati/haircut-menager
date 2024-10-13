namespace HaircutManager.Models
{
    public class OldPassword
    {
        public Guid id { get; set; }
        public string PasswordHash { get; set; }
        public DateTime ChangedAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

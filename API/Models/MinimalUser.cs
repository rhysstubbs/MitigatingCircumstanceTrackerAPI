namespace MCT.RESTAPI.Models
{
    public class MinimalUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
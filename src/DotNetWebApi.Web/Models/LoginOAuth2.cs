using System.ComponentModel.DataAnnotations;

namespace DotNetWebApi.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginOAuth2
    {
        [Required]
        public string Grant_Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Refresh_token { get; set; }
        public string Scope { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

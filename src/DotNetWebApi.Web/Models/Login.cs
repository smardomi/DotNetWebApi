using System.ComponentModel.DataAnnotations;

namespace DotNetWebApi.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Login
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

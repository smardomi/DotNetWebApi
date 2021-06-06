using System.ComponentModel.DataAnnotations;

namespace DotNetWebApi.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangePasswordDto
    {

        /// <summary>
        /// 
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string OldPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}

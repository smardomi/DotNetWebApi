﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.IocConfig.Api;

namespace DotNetWebApi.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserDto : BaseDto<UserDto, User>, IValidatableObject
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
            if (Password.Equals("123456"))
                yield return new ValidationResult("رمز عبور نمیتواند 123456 باشد", new[] { nameof(Password) });
        }
    }
}

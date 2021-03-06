﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Escc.Umbraco.UserAccessWebService.Models
{
    public class PasswordResetModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string UniqueResetId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Verify { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string NewPasswordConfim { get; set; }

        public int UserId { get; set; }
    }
}
﻿using System.ComponentModel.DataAnnotations;

namespace Library.Frontend.Models
{
    public class RegisterViewModel
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [Compare("Password"), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

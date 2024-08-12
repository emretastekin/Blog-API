using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public long IdNumber { get; set; }

        public string Name { get; set; } = "";

        public string? Address { get; set; }

        public bool Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime RegisterDate { get; set; }

        [NotMapped]
        public string? Password { get; set; }

        [NotMapped]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = true; // Varsayılan olarak aktif


    }
}


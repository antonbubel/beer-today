﻿namespace BeerToday.Core.Contracts.Users.Notifications
{
    using MediatR;

    using System.ComponentModel.DataAnnotations;

    public class SignUpNotification : INotification
    {
        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string PasswordConfirmation { get; set; }
    }
}


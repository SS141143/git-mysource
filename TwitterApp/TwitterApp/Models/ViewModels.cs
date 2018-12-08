using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwitterApp.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string User_Id { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
    public class SignUpViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string User_Id { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Joined { get; set; }
        public bool Active { get; set; }
        public string Following_Id { get; set; }
    }
    public class TweetViewModel
    {
        public int Tweet_Id { get; set; }
        public string User_Id { get; set; }
        public string UserWithAlias { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "What are you doing ?")]
        public string Message { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string Timestamp { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IO_3_2.Models
{
    public class AddContactViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Key]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public EventType EventType { get; set; }
        public HttpPostedFileBase Avatar { get; set; }
    }
}
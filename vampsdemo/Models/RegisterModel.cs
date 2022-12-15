using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vampsdemo.Models
{
    public class RegisterModel
    {
        [Key, Column(Order = 1)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]

        public string firstname { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]

        public string lastname { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]

        public string phonenumber { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Entered email format is not valid.")]

        public string email { get; set; }
        [Required]
        public Nullable<System.DateTime> birthdate { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [NotMapped]
        [Compare("password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


    }
}
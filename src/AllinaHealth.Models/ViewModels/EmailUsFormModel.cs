using System.ComponentModel.DataAnnotations;

namespace AllinaHealth.Models.ViewModels
{
    public class EmailUsFormModel
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(80)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(80)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(80)]
        public string Question { get; set; }

        public bool IsSuccess { get; set; }
    }
}
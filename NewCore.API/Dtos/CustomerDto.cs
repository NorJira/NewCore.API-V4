using System;
using System.ComponentModel.DataAnnotations;
using NewCore.Data.Models;

namespace NewCore.API.Dtos
{
    public record CustomerDto
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Location { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
    }

    public record CusIdDto
    {
        [Required]
        public int Id { get; set; }
    }
}

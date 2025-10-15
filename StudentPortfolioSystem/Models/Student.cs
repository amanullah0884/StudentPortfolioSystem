using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortfolioSystem.Models
{
    public class Student
    {
        
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Student name is required")]
            [StringLength(50)]
            public string StudentName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [StringLength(50)]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Major is required")]
            public string Major { get; set; } = string.Empty;

            [Required(ErrorMessage = "Graduation year is required")]
            [Range(2000, 2100, ErrorMessage = "Graduation year must be between 2000 and 2100")]
            public int GraduationYear { get; set; }

            [StringLength(500)]
            public string? Bio { get; set; }

            [Display(Name = "Profile Image")]
            public string? ImagePath { get; set; }

            [ForeignKey("Department")]
            public int DepartmentId { get; set; }

            public Department? Department { get; set; }
        }
    }


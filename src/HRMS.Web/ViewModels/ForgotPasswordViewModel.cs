using System.ComponentModel.DataAnnotations;

namespace HRMS.Web.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

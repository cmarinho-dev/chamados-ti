using System.ComponentModel.DataAnnotations;

namespace ChamadosTI.Models;

public class LoginAdminViewModel
{
    [Display(Name = "Usuario")]
    [Required(ErrorMessage = "Informe o usuario.")]
    public string Usuario { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;
}

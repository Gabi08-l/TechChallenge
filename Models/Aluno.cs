using System.ComponentModel.DataAnnotations;

namespace TechChallenge;

public class Aluno
{
    [Required(ErrorMessage ="o campo id é obrigatorio")]
    public int Id { get; set; } // Primary Key

    [StringLength(100, ErrorMessage ="deve ter esses caracteres ai")]
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public DateTime DataNascimento { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}

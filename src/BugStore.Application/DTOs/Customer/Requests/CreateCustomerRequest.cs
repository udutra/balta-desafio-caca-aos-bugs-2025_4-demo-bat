using System.ComponentModel.DataAnnotations;

namespace BugStore.Application.DTOs.Customer.Requests;

public class CreateCustomerRequest(string name, string email, string phone, DateTime birthDate){
    [Required(ErrorMessage = "Nome é obrigatório.")]
    public string Name { get; set; } = name;

    [Required, EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = email;

    [Required(ErrorMessage = "Telefone é obrigatório.")]
    public string Phone { get; set; } = phone;
    [Required, DataType(DataType.Date)]
    public DateTime BirthDate { get; set; } = birthDate;
}
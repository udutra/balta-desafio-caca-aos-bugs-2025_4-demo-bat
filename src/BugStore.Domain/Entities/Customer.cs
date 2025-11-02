namespace BugStore.Domain.Entities;

public class Customer{
    public Guid Id { get;  set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime BirthDate { get; set; }

    public Customer(string name, string email, string phone, DateTime birthDate){
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail não pode ser vazio.", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("O telefone não pode ser vazio.", nameof(phone));

        Id = Guid.CreateVersion7();
        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        BirthDate = birthDate;
    }
}
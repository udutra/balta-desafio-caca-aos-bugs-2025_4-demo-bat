using BugStore.Domain.Entities;
using BugStore.Domain.Exceptions;

namespace BugStore.Domain.Tests;

public class CustomerTests{

    [Fact]
    public void Construtor_Dado_Os_Dados_Deve_Iniciar_Um_Novo_Cliente(){
        // Arrange
        var nome = "Guilherme Dutra";
        var email = "teste@teste.com";
        var phone = "51999999999";
        var birth = new DateTime(1991, 3, 14);

        // Act
        var c = new Customer(nome, email, phone, birth);

        // Assert
        Assert.Equal(nome, c.Name);
        Assert.Equal(email, c.Email);
        Assert.Equal(phone, c.Phone);
        Assert.Equal(birth, c.BirthDate);
        Assert.NotEqual(Guid.Empty, c.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Nome_For_Nulo_Ou_Espacos(string? nome){
        // Arrange
        var email = "teste@teste.com";
        var phone = "51999999999";
        var birth = new DateTime(1991, 3, 14);

        // Act
        var ex = Assert.Throws<DomainException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("O nome não pode ser vazio.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Email_For_Nulo_Ou_Espacos(string? email){
        // Arrange
        var nome = "Guilherme Dutra";
        var phone = "51999999999";
        var birth = new DateTime(1991, 3, 14);

        // Act
        var ex = Assert.Throws<DomainException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("O e-mail não pode ser vazio.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Telefone_For_Nulo_Ou_Espacos(string? phone){
        // Arrange
        var nome = "Guilherme Dutra";
        var email = "teste@teste.com";
        var birth = new DateTime(1991, 3, 14);

        // Act
        var ex = Assert.Throws<DomainException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("O telefone não pode ser vazio.", ex.Message);
    }

    [Fact]
    public void Id_Deve_Ser_Um_GuidV7(){
        // Arrange & Act
        var c = new Customer("Guilherme Dutra", "teste@teste.com", "51999999999",
            new DateTime(1991, 3, 14));

        // Assert: Guid version 7 has variant bits set and version == 7
        var bytes = c.Id.ToByteArray();
        var version = (bytes[7] >> 4) & 0x0F;
        Assert.Equal(7, version);
    }
}

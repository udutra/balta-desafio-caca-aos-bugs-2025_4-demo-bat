using BugStore.Domain.Entities;

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
        var ex = Assert.Throws<ArgumentException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("name", ex.ParamName);
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
        var ex = Assert.Throws<ArgumentException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("email", ex.ParamName);
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
        var ex = Assert.Throws<ArgumentException>(() => new Customer(nome, email, phone, birth));

        // Assert
        Assert.Equal("phone", ex.ParamName);
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

    [Fact]
    public void Update_Deve_Atualizar_Somente_Campos_Que_Nao_EstejamVazios()
    {
        // Arrange
        var c = new Customer("Guilherme Dutra", "teste@teste.com", "51999999999",
            new DateTime(1991, 3, 14));

        // Act
        c.Update("Guilherme Galarça Dutra", "teste@teste.com.br", "51999999991",
            new DateTime(1990, 3, 1));

        // Assert
        Assert.Equal("Guilherme Galarça Dutra", c.Name);
        Assert.Equal("teste@teste.com.br", c.Email);
        Assert.Equal("51999999991", c.Phone);
        Assert.Equal(new DateTime(1990, 3, 1), c.BirthDate);
    }

    [Fact]
    public void Update_Deve_Ignorar_Valores_Nulos_Ou_Espaços_Em_Branco()
    {
        // Arrange
        var c = new Customer("Guilherme Dutra", "teste@teste.com", "51999999999",
            new DateTime(1991, 3, 14));

        // Act
        c.Update(name: null, email: "   ", phone: null, birthDate: null);

        // Assert
        Assert.Equal("Guilherme Dutra", c.Name);
        Assert.Equal("teste@teste.com", c.Email);
        Assert.Equal("51999999999", c.Phone);
        Assert.Equal(new DateTime(1991, 3, 14), c.BirthDate);
    }

    [Fact]
    public void Update_Somente_Data_De_Nascimento_Deve_Ser_Atualizada()
    {
        // Arrange
        var c = new Customer("Guilherme Dutra", "teste@teste.com", "51999999999",
            new DateTime(1991, 3, 14));

        var newBirth = new DateTime(1990, 3, 1);

        // Act
        c.Update(name: null, email: null, phone: null, newBirth);

        // Assert
        Assert.Equal("Guilherme Dutra", c.Name);
        Assert.Equal("teste@teste.com", c.Email);
        Assert.Equal("51999999999", c.Phone);
        Assert.Equal(newBirth, c.BirthDate);
    }

    [Fact]
    public void Update_SemAlteracoes_Deve_Manter_Todos_Os_Valores()
    {
        // Arrange
        var dataInicial = new DateTime(1991, 3, 14);
        var c = new Customer("Guilherme Dutra", "teste@teste.com", "51999999999", dataInicial);

        // Act
        c.Update(null, null, null, null);

        // Assert
        Assert.Equal("Guilherme Dutra", c.Name);
        Assert.Equal("teste@teste.com", c.Email);
        Assert.Equal("51999999999", c.Phone);
        Assert.Equal(dataInicial, c.BirthDate);
    }
}

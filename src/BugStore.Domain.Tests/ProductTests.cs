using BugStore.Domain.Entities;
using BugStore.Domain.Exceptions;

namespace BugStore.Domain.Tests;

public class ProductTests{

    [Fact]
    public void Construtor_Dado_Os_Dados_Deve_Iniciar_Um_Novo_Produto(){
        // Arrange
        var title = "Camiseta";
        var description = "Camiseta 100% algodão";
        var slug = "camiseta-100-algodao";
        var price = 59.90m;

        // Act
        var p = new Product(title, description, slug, price);

        // Assert
        Assert.Equal(title, p.Title);
        Assert.Equal(description, p.Description);
        Assert.Equal(slug, p.Slug);
        Assert.Equal(price, p.Price);
        Assert.NotEqual(Guid.Empty, p.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Titulo_For_Nulo_Ou_Espacos(string? title){
        // Arrange
        var description = "Desc";
        var slug = "slug";
        var price = 10m;

        var ex = Assert.Throws<DomainException>(() => new Product(title, description, slug, price));

        Assert.Equal("O Título não pode ser vazio.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Descricao_For_Nula_Ou_Espacos(string? description){
        // Arrange
        var title = "Titulo";
        var slug = "slug";
        var price = 10m;

        var ex = Assert.Throws<DomainException>(() => new Product(title, description, slug, price));

        Assert.Equal("A descrição não pode ser vazio.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Slug_For_Nulo_Ou_Espacos(string? slug){
        // Arrange
        var title = "Titulo";
        var description = "Desc";
        var price = 10m;

        var ex = Assert.Throws<DomainException>(() => new Product(title, description, slug, price));

        Assert.Equal("O slug não pode ser vazio.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Construtor_Deve_Lancar_Excecao_Quando_Preco_For_Menor_Ou_Igual_A_Zero(decimal price){
        // Arrange
        var title = "Titulo";
        var description = "Desc";
        var slug = "slug";

        var ex = Assert.Throws<DomainException>(() => new Product(title, description, slug, price));
        Assert.Equal("O preço deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Id_Deve_Ser_Um_GuidV7(){
        // Arrange
        var p = new Product("Titulo", "Desc", "slug", 10m);

        // Assert
        var bytes = p.Id.ToByteArray();
        var version = (bytes[7] >> 4) & 0x0F;
        Assert.Equal(7, version);
    }
}
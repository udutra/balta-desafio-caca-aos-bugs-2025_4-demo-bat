using BugStore.Domain.Entities;

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

        var ex = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));

        Assert.Equal("title", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Descricao_For_Nula_Ou_Espacos(string? description){
        // Arrange
        var title = "Titulo";
        var slug = "slug";
        var price = 10m;

        var ex = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));

        Assert.Equal("description", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Construtor_Deve_Lancar_Excecao_Quando_Slug_For_Nulo_Ou_Espacos(string? slug){
        // Arrange
        var title = "Titulo";
        var description = "Desc";
        var price = 10m;

        var ex = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));

        Assert.Equal("slug", ex.ParamName);
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

        var ex = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));
        Assert.Equal("price", ex.ParamName);
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

    [Fact]
    public void Update_Deve_Atualizar_Todos_Os_Campos_Quando_Validos(){
        // Arrange
        var p = new Product("Titulo", "Desc", "slug", 10m);

        // Act
        p.Update("Novo Titulo", "Nova Desc", "novo-slug", 20m);

        // Assert
        Assert.Equal("Novo Titulo", p.Title);
        Assert.Equal("Nova Desc", p.Description);
        Assert.Equal("novo-slug", p.Slug);
        Assert.Equal(20m, p.Price);
    }

    [Fact]
    public void Update_Deve_Ignorar_Titulo_Descricao_Slug_Quando_Nulos_Ou_Espacos(){
        // Arrange
        var p = new Product("Titulo", "Desc", "slug", 10m);

        // Act
        p.Update(null, "   ", null, null);

        // Assert
        Assert.Equal("Titulo", p.Title);
        Assert.Equal("Desc", p.Description);
        Assert.Equal("slug", p.Slug);
        Assert.Equal(10m, p.Price);
    }

    [Fact]
    public void Update_Deve_Atualizar_Apenas_Preco(){
        // Arrange
        var p = new Product("Titulo", "Desc", "slug", 10m);

        // Act
        p.Update(null, null, null, 99.99m);

        // Assert
        Assert.Equal("Titulo", p.Title);
        Assert.Equal("Desc", p.Description);
        Assert.Equal("slug", p.Slug);
        Assert.Equal(99.99m, p.Price);
    }

    [Fact]
    public void Update_Nao_Deve_Alterar_Preco_Quando_Nulo(){
        // Arrange
        var p = new Product("Titulo", "Desc", "slug", 10m);

        // Act
        p.Update("Novo", "Nova", "novo", null);

        // Assert
        Assert.Equal("Novo", p.Title);
        Assert.Equal("Nova", p.Description);
        Assert.Equal("novo", p.Slug);
        Assert.Equal(10m, p.Price);
    }
}
using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class OrderLineTests
{
    [Fact]
    public void Construtor_Dado_Os_Dados_Deve_Iniciar_Uma_Nova_OrderLine()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 19.99m);
        var quantidade = 3;

        // Act
        var ol = new OrderLine(orderId, quantidade, produto);

        // Assert
        Assert.NotEqual(Guid.Empty, ol.Id);
        Assert.Equal(orderId, ol.OrderId);
        Assert.Equal(quantidade, ol.Quantity);
        Assert.Equal(quantidade * produto.Price, ol.Total);
        Assert.Equal(produto.Id, ol.ProductId);
        Assert.Same(produto, ol.Product);
        Assert.Null(ol.Order);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Construtor_Deve_Lancar_Excecao_Quando_Quantidade_For_Menor_Ou_Igual_A_Zero(int quantidade)
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 10m);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => new OrderLine(orderId, quantidade, produto));

        // Assert
        Assert.Equal("quantity", ex.ParamName);
    }

    [Fact]
    public void Construtor_Deve_Lancar_Excecao_Quando_Produto_For_Nulo()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantidade = 1;

        // Act
        var ex = Assert.Throws<ArgumentException>(() => new OrderLine(orderId, quantidade, null));

        // Assert
        Assert.Equal("product", ex.ParamName);
    }

    [Fact]
    public void Id_Deve_Ser_Um_GuidV7()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 10m);

        // Act
        var ol = new OrderLine(orderId, 1, produto);

        // Assert
        var bytes = ol.Id.ToByteArray();
        var versao = (bytes[7] >> 4) & 0x0F;
        Assert.Equal(7, versao);
    }

    [Fact]
    public void Total_Deve_Ser_Quantidade_Vezes_Preco_Do_Produto()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 19.99m);
        var quantidade = 3;

        // Act
        var ol = new OrderLine(orderId, quantidade, produto);

        // Assert
        Assert.Equal(59.97m, ol.Total);
    }

    [Fact]
    public void ProductId_Deve_Corresponder_Ao_Id_Do_Produto()
    {
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 10m);

        var ol = new OrderLine(orderId, 2, produto);

        Assert.Equal(produto.Id, ol.ProductId);
    }

    [Fact]
    public void OrderId_Deve_Ser_Definido_Conforme_Parametro()
    {
        var orderId = Guid.CreateVersion7();
        var produto = new Product("Produto", "Desc", "produto", 10m);

        var ol = new OrderLine(orderId, 1, produto);

        Assert.Equal(orderId, ol.OrderId);
    }
}


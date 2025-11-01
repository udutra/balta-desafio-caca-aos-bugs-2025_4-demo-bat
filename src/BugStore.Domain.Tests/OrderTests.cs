using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class OrderTests{

    [Fact]
    public void Construtor_Dado_Cliente_E_Linhas_Deve_Criar_Pedido(){
        // Arrange
        var cliente = new Customer("Cliente Teste", "cliente@teste.com", "11999999999", new DateTime(1990, 1, 1));
        var produto = new Product("Produto", "Desc", "produto", 10m);
        var linhas = new List<OrderLine>{
            new(Guid.Empty, 2, produto)
        };

        // Act
        var antes = DateTime.UtcNow;
        var pedido = new Order(cliente, linhas);
        var depois = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, pedido.Id);
        Assert.Equal(cliente.Id, pedido.CustomerId);
        Assert.Equal(20m, linhas.First().Total);
        Assert.Same(cliente, pedido.Customer);
        Assert.Same(linhas, pedido.Lines);
        Assert.InRange(pedido.CreatedAt, antes.AddSeconds(-1), depois.AddSeconds(1));
        Assert.Null(pedido.UpdatedAt);
    }

    [Fact]
    public void Construtor_Dado_Cliente_Nulo_Nao_Deve_Criar_Pedido(){
        // Arrange
        Customer? cliente = null;
        var produto = new Product("Produto", "Desc", "produto", 10m);
        var linhas = new List<OrderLine>{
            new(Guid.Empty, 2, produto)
        };

        // Act
        var ex = Assert.Throws<ArgumentException>(() => new Order(cliente, linhas));

        // Assert
        Assert.Equal("customer", ex.ParamName);
    }

    [Fact]
    public void Id_Deve_Ser_Um_GuidV7(){
        // Arrange
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var pedido = new Order(cliente, []);

        // Act
        var bytes = pedido.Id.ToByteArray();
        var versao = (bytes[7] >> 4) & 0x0F;

        // Assert
        Assert.Equal(7, versao);
    }

    [Fact]
    public void CreatedAt_Deve_Ser_Definido_Como_Momento_Atual(){
        // Arrange
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));

        // Act
        var t0 = DateTime.UtcNow;
        var pedido = new Order(cliente, []);
        var t1 = DateTime.UtcNow;

        //Assert
        Assert.InRange(pedido.CreatedAt, t0.AddSeconds(-1), t1.AddSeconds(1));
    }


    [Fact]
    public void UpdatedAt_Deve_Ser_Nulo_Ao_Criar(){
        // Arrange & Act
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var pedido = new Order(cliente, []);

        // Assert
        Assert.Null(pedido.UpdatedAt);
    }

    [Fact]
    public void Linhas_Devem_Ser_Preservadas(){

        // Arrange
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P1", "D1", "p1", 5m);
        var l1 = new OrderLine(Guid.CreateVersion7(), 3, produto);
        var l2 = new OrderLine(Guid.CreateVersion7(), 1, produto);
        var linhas = new List<OrderLine>{ l1, l2 };

        // Act
        var pedido = new Order(cliente, linhas);

        // Assert
        Assert.Equal(2, pedido.Lines.Count);
        Assert.Same(l1, pedido.Lines[0]);
        Assert.Same(l2, pedido.Lines[1]);
    }

    [Fact]
    public void CustomerId_Deve_Corresponder_Ao_Id_Do_Cliente(){
        // Arrange & Act
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var pedido = new Order(cliente, []);

        // Assert
        Assert.Equal(cliente.Id, pedido.CustomerId);
    }
}
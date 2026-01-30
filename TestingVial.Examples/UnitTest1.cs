using TestingVial.NET;

namespace TestingVial.Examples
{

    public record Cart(int Id);

    public class Product
    {
        public string Name { get; init; }
        public decimal Price { get; init; }
    }
    public class CartService
    {
        private Dictionary<Cart, List<Product>> carts = new();

        public void AddToCart(Cart cart, Product product, int quantity)
        {
            if (carts.TryGetValue(cart, out var products))
            {
                for (int i = 0; i < quantity; i++)
                {
                    products.Add(product);
                }
            }
            else
            {
                var productList = new List<Product>();
                for (int i = 0; i < quantity; i++)
                {
                    productList.Add(product);
                }
                carts[cart] = productList;
            }
        }

        public decimal GetCartTotal(Cart cart)
        {
            if (carts.TryGetValue(cart, out var products))
            {
                return products.Sum(p => p.Price);
            }
            return 0;
        }

    }

    [UnitTestVial<CartVial>(Description = "Carts can have multiple products added")]
    public class CartServiceTests
    {
        private CartService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new CartService();
        }

        [Test]
        public void AddToCart_WithNewCart_AddsProductSuccessfully()
        {
            // Arrange
            var cart = new Cart(1);
            var product = new Product { Name = "Laptop", Price = 999.99m };

            // Act
            _sut.AddToCart(cart, product, 1);

            // Assert
            var total = _sut.GetCartTotal(cart);
            Assert.That(total, Is.EqualTo(999.99m));
        }

        [Test]
        [UnitTestVial<CartVial>(Description = "Carts can have multiple products of the same type")]
        [UnitTestVial("Product", Description = "Products with the same ID are considered to be distinct.")]
        public void AddToCart_WithMultipleQuantity_AddsAllProducts()
        {
            // Arrange
            var cart = new Cart(1);
            var product = new Product { Name = "Mouse", Price = 25.50m };
            var quantity = 3;

            // Act
            _sut.AddToCart(cart, product, quantity);

            // Assert
            var total = _sut.GetCartTotal(cart);
            Assert.That(total, Is.EqualTo(76.50m));
        }

        [Test]
        public void AddToCart_WithExistingCart_AppendsProducts()
        {
            // Arrange
            var cart = new Cart(1);
            var product1 = new Product { Name = "Keyboard", Price = 50.00m };
            var product2 = new Product { Name = "Monitor", Price = 300.00m };

            // Act
            _sut.AddToCart(cart, product1, 2);
            _sut.AddToCart(cart, product2, 1);

            // Assert
            var total = _sut.GetCartTotal(cart);
            Assert.That(total, Is.EqualTo(400.00m));
        }

    }
}

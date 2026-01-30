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
}

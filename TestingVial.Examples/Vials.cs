using TestingVial.NET;

namespace TestingVial.Examples
{
    internal class CartVial : IVial
    {
        public string Name => "Cart";
    }

    internal class ProductVial : IVial
    {
        public string Name => "Product";
    }
}

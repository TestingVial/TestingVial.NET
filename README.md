# TestingVial.NET

[![NuGet](https://img.shields.io/nuget/v/TestingVial.NET.svg)](https://www.nuget.org/packages/TestingVial.NET/)

A C# implementation of the Testing Vial attribute pattern for better test organization and traceability.

## What is a Testing Vial?

The **Testing Vial** is a conceptual approach to organizing and categorizing tests by linking them to specific features or components of your system. 

Just like a laboratory vial contains a sample for analysis, a Testing Vial groups related tests together, making it easier to understand test coverage and maintain your test suite.

This concept was introduced by **Davide Bellone** in his article [**Introducing the Testing Vial: a (better?) alternative to Testing Diamond and Testing Pyramid**](https://www.code4it.dev/blog/testing-vial/).

By using Testing Vials, you can:
- **Identify** which features are being tested
- **Organize** tests by business functionality rather than just technical structure
- **Track** test coverage across different features
- **Document** the purpose and scope of your tests

## Installation

Install the package via NuGet:

```bash
dotnet add package TestingVial.NET
```

Or via the Package Manager Console:

```bash
Install-Package TestingVial.NET
```

## Usage

### Defining Vials

First, create your vials by implementing the `IVial` interface:

```cs
using TestingVial.NET;

internal class CartVial : IVial 
{ 
    public string Name => "Cart"; 
}

internal class ProductVial : IVial 
{ 
    public string Name => "Product"; 
}
```

### Applying Vials to Tests

You can apply vials at both the class and method level using either the generic or non-generic syntax:

```cs
using TestingVial.NET;

[UnitTestVial<CartVial>(Description = "Carts can have multiple products added")] public class CartServiceTests 
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
    [UnitTestVial("Product", Description = "Products with the same ID are considered distinct")]
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
}
```

You can apply vials at the class level to indicate that all tests within the class pertain to a specific feature. Additionally, you can apply vials at the method level for more granular control. 

Also, you can apply more than one vial to a single test if it covers multiple features.


### Available Attributes

TestingVial.NET provides built-in attributes for common test types:

#### Unit Tests

```cs
// Generic syntax with type safety 
[UnitTestVial<CartVial>(Description = "Tests cart functionality")]

// Non-generic syntax with string name 
[UnitTestVial("Cart", Description = "Tests cart functionality")]
```

#### Integration Tests

```cs
// Generic syntax with type safety 
[IntegrationTestVial<PaymentVial>(Description = "Tests payment processing")]

// Non-generic syntax with string name 
[IntegrationTestVial("Payment", Description = "Tests payment processing")]
```

### Multiple Vials

You can apply multiple vials to a single test to indicate it covers multiple features:

```cs
[Test] 
[UnitTestVial<CartVial>(Description = "Carts can have multiple products of the same type")] 
[UnitTestVial<ProductVial>(Description = "Products can be added in quantities")] 
public void AddToCart_WithMultipleQuantity_AddsAllProducts() 
{ 
    // Test implementation 
}
```

### Creating Custom Test Type Vials

You can create your own test type vials by inheriting from `VialAttribute` or `TestingVialAttribute<T>`:

```cs
public class PerformanceTestVial<T> 
    : TestingVialAttribute<T> where T : IVial, new() 
    { 
        public PerformanceTestVial() : base("Performance") { } 
    }

public class PerformanceTestVial 
    : VialAttribute 
    { 
        public PerformanceTestVial(string name) : base(name, "Performance") { } 
    }
```


## Features

- **Type-safe** vial definitions with generic attributes
- **Flexible** string-based vials for quick prototyping
- **Multiple vials** per test for cross-cutting concerns
- **Inheritance support** - vials are inherited by derived classes
- **Extensible** - create your own test type vials
- **Description support** for documenting vial purposes
- Compatible with **.NET 9** and **.NET 10**
- Uses **C# 13.0** features

## Benefits

- **Improved Test Organization**: Group tests by feature rather than just by class structure
- **Better Traceability**: Quickly identify which tests cover specific features
- **Enhanced Documentation**: Vials serve as living documentation of your test coverage
- **Easier Maintenance**: Find and update all tests related to a specific feature
- **Test Analysis**: Analyze test distribution across features and components

## Learn More

Read Davide Bellone's original article to understand the philosophy behind Testing Vials:
[**Introducing the Testing Vial: a (better?) alternative to Testing Diamond and Testing Pyramid**](https://www.code4it.dev/blog/testing-vial/)

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is open source. Please check the repository for license details.

## Repository

[https://github.com/TestingVial/TestingVial.NET](https://github.com/TestingVial/TestingVial.NET)
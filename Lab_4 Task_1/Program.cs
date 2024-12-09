using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Rating { get; set; }

    public Product(string name, decimal price, string description, string category, double rating)
    {
        Name = name;
        Price = price;
        Description = description;
        Category = category;
        Rating = rating;
    }
}

public class User
{
    public string Login { get; set; }
    private string Password { get; set; }
    public List<Order> OrderHistory { get; private set; } = new List<Order>();

    public User(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public bool Authenticate(string password)
    {
        return Password == password;
    }
}

public class Order
{
    public List<(Product Product, int Quantity)> Items { get; private set; } = new List<(Product, int)>();
    public decimal TotalPrice { get; private set; }
    public string Status { get; set; } = "Pending";

    public void AddProduct(Product product, int quantity)
    {
        Items.Add((product, quantity));
        TotalPrice += product.Price * quantity;
    }
}

public interface ISearchable
{
    List<Product> SearchByPrice(decimal minPrice, decimal maxPrice);
    List<Product> SearchByCategory(string category);
    List<Product> SearchByRating(double minRating);
}

public class Shop : ISearchable
{
    private List<Product> Products { get; set; } = new List<Product>();
    private List<User> Users { get; set; } = new List<User>();

    public void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public void RegisterUser(User user)
    {
        Users.Add(user);
    }

    public User AuthenticateUser(string login, string password)
    {
        var user = Users.FirstOrDefault(u => u.Login == login);
        if (user != null && user.Authenticate(password))
        {
            return user;
        }
        return null;
    }

    public List<Product> SearchByPrice(decimal minPrice, decimal maxPrice)
    {
        return Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
    }

    public List<Product> SearchByCategory(string category)
    {
        return Products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Product> SearchByRating(double minRating)
    {
        return Products.Where(p => p.Rating >= minRating).ToList();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Shop shop = new Shop();

        shop.AddProduct(new Product("Laptop", 1200m, "High performance laptop", "Electronics", 4.5));
        shop.AddProduct(new Product("Headphones", 100m, "Noise-cancelling headphones", "Accessories", 4.2));
        shop.AddProduct(new Product("Smartphone", 800m, "Latest model smartphone", "Electronics", 4.8));

        shop.RegisterUser(new User("user1", "password123"));

        User user = shop.AuthenticateUser("user1", "password123");
        if (user != null)
        {
            Console.WriteLine("Login successful!");

            var electronics = shop.SearchByCategory("Electronics");
            Console.WriteLine("Electronics:");
            foreach (var product in electronics)
            {
                Console.WriteLine($"{product.Name} - {product.Price:C} - {product.Rating} stars");
            }

            Order order = new Order();
            order.AddProduct(electronics.First(), 1);
            user.OrderHistory.Add(order);
            Console.WriteLine($"Order placed: {order.TotalPrice:C}");
        }
        else
        {
            Console.WriteLine("Login failed.");
        }
    }
}

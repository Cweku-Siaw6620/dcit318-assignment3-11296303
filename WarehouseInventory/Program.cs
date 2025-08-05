using System;
using System.Collections.Generic;
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (_items.TryGetValue(id, out var item))
            return item;
        throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }
}

public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new();
    private InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        // Electronics
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Lenovo", 24));
        _electronics.AddItem(new ElectronicItem(2, "TV", 5, "Samsung", 36));

        // Groceries
        _groceries.AddItem(new GroceryItem(1, "Milk", 20, DateTime.Now.AddDays(10)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 15, DateTime.Now.AddDays(5)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased for {item.Name}. New quantity: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Expose inventory for testing errors in Main
    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

class Program
{
    static void Main(string[] args)
    {
        var manager = new WareHouseManager();

        manager.SeedData();

        Console.WriteLine("Grocery Inventory:");
        manager.PrintAllItems(manager.Groceries);

        Console.WriteLine("\nElectronic Inventory:");
        manager.PrintAllItems(manager.Electronics);

        Console.WriteLine("\n--- Exception Handling Tests ---");

        // Duplicate Item
        try
        {
            manager.Electronics.AddItem(new ElectronicItem(1, "Speaker", 5, "Sony", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"[Duplicate Error] {ex.Message}");
        }

        // Remove non-existent item
        manager.RemoveItemById(manager.Groceries, 999);

        // Invalid quantity update
        try
        {
            manager.Electronics.UpdateQuantity(2, -10);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"[Invalid Quantity] {ex.Message}");
        }
    }
}

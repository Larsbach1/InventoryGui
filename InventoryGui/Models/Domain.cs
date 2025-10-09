using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryGui.Models;

// --- Basis-varer ---
public class Item
{
    public string Name { get; set; }
    public double PricePerUnit { get; set; } // kr. pr. enhed (stk eller kg)
    public Item(string name, double pricePerUnit)
    {
        Name = name; PricePerUnit = pricePerUnit;
    }
}

// BulkItem: sælges i fx kg, meter
public class BulkItem : Item
{
    public string MeasurementUnit { get; set; } // typisk "kg"
    public BulkItem(string name, double pricePerUnit, string unit = "kg")
        : base(name, pricePerUnit) => MeasurementUnit = unit;
}

// UnitItem: sælges i stk., med vægt pr. stk (kravet siger "Weight")
public class UnitItem : Item
{
    public double Weight { get; set; } // fx i gram
    public UnitItem(string name, double pricePerUnit, double weight)
        : base(name, pricePerUnit) => Weight = weight;
}

// --- Inventory (lager) ---
public class StockEntry
{
    public Item Item { get; set; }
    public double Amount { get; set; } // stk. for UnitItem, kg for BulkItem
    public StockEntry(Item item, double amount) { Item = item; Amount = Math.Max(0, amount); }
}

public class Inventory
{
    // "stock: stores for each item its amount in the inventory"
    public List<StockEntry> Stock { get; } = new();

    public void Add(Item item, double amount)
    {
        var s = Stock.FirstOrDefault(x => ReferenceEquals(x.Item, item));
        if (s == null) Stock.Add(new StockEntry(item, amount));
        else s.Amount += amount;
    }

    public bool Remove(Item item, double amount)
    {
        var s = Stock.FirstOrDefault(x => ReferenceEquals(x.Item, item));
        if (s == null || amount < 0) return false;
        s.Amount = Math.Max(0, s.Amount - amount); // enkelt: ikke under 0
        return true;
    }

    // "LowStockItems(): gets a collection of low stock items, e.g., less than five"
    public IEnumerable<StockEntry> LowStockItems(double threshold = 5) =>
        Stock.Where(s => s.Amount < threshold);
}

// --- Ordrer ---
public class OrderLine
{
    public Item Item { get; set; }
    public double Quantity { get; set; } // stk. eller kg
    public OrderLine(Item item, double qty) { Item = item; Quantity = qty; }

    public double LinePrice =>
        Item.PricePerUnit * Quantity; // simpelt: kg læses bare som enhed
}

public class Order
{
    public DateTime Time { get; set; } = DateTime.Now; // "Time: exact time of an order"
    public List<OrderLine> OrderLines { get; } = new(); // "OrderLines"

    // Beregning af totalpris
    public double TotalPrice() => OrderLines.Sum(l => l.LinePrice);

    // Tekstversioner til GUI-visning
    public string TimeText => Time.ToString("dd-MM-yyyy HH:mm:ss");

    public string LinesText =>
        string.Join(", ", OrderLines.Select(l =>
            $"{l.Item.Name} x {l.Quantity}"));

    public string TotalPriceText => $"{TotalPrice():N2} kr.";
}

// --- Customer ---
public class Customer
{
    public string Name { get; set; }
    public List<Order> Orders { get; } = new(); // "Orders: list of orders"
    public Customer(string name) => Name = name;

    public void CreateOrder(Order o) => Orders.Add(o); // "CreateOrder(Order)"
}

// --- OrderBook ---
public class OrderBook
{
    public List<Order> queuedOrders { get; } = new();    // "queuedOrders"
    public List<Order> processedOrders { get; } = new(); // "processedOrders"

    public void QueueOrder(Order o) => queuedOrders.Add(o); // "QueueOrder(Order)"

    // "ProcessNextOrder(): removes earliest queued order and places it into processed orders"
    public Order? ProcessNextOrder(Inventory inv)
    {
        if (queuedOrders.Count == 0) return null;
        var o = queuedOrders[0];
        queuedOrders.RemoveAt(0);

        // Efter en ordre er processeret, opdateres lageret (krav):
        foreach (var line in o.OrderLines)
            inv.Remove(line.Item, line.Quantity);

        processedOrders.Add(o);
        return o;
    }

    public double TotalRevenue() => processedOrders.Sum(x => x.TotalPrice()); // "TotalRevenue()"
}

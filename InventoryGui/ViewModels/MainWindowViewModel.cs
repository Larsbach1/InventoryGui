using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InventoryGui.Models;

namespace InventoryGui.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Order> Queued { get; } = new();
    public ObservableCollection<Order> Processed { get; } = new();

    public string TotalRevenueText => $"{_book.TotalRevenue():N2} kr.";

    public ICommand ProcessNextCommand { get; }

    private readonly OrderBook _book = new();
    private readonly Inventory _inv = new();

    public MainWindowViewModel()
    {
        // 1) Varer (både UnitItem og BulkItem)
        var kaktus = new UnitItem("Kaktus", 49.95, weight: 120);
        var bonsai = new UnitItem("Bonsai-træ", 249.00, weight: 850);
        var jord   = new BulkItem("Pottemuld", 15.00, unit: "kg");

        // 2) Lager (stk vs kg)
        _inv.Add(kaktus, 25);
        _inv.Add(bonsai, 10);
        _inv.Add(jord,   100); // kg

        // 3) Predefineret kø af ordrer
        var kunde = new Customer("Asha Sharma");
        var o1 = new Order();
        o1.OrderLines.Add(new OrderLine(kaktus, 3));
        o1.OrderLines.Add(new OrderLine(bonsai, 1));
        o1.OrderLines.Add(new OrderLine(jord,   5));
        kunde.CreateOrder(o1);
        _book.QueueOrder(o1);

        var o2 = new Order();
        o2.OrderLines.Add(new OrderLine(kaktus, 2));
        o2.OrderLines.Add(new OrderLine(jord,   12));
        kunde.CreateOrder(o2);
        _book.QueueOrder(o2);

        foreach (var q in _book.queuedOrders) Queued.Add(q);

        ProcessNextCommand = new RelayCommand(ProcessNext);
    }

    private void ProcessNext()
    {
        var processed = _book.ProcessNextOrder(_inv);
        if (processed != null)
        {
            Queued.Remove(processed);
            Processed.Add(processed);
            OnPropertyChanged(nameof(TotalRevenueText));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
}

public class RelayCommand : ICommand
{
    private readonly Action _run;
    public RelayCommand(Action run) => _run = run;
    public bool CanExecute(object? p) => true;
    public void Execute(object? p) => _run();
    public event EventHandler? CanExecuteChanged;
}

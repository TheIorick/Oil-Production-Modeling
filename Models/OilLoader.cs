using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    // Реализация интерфейса погрузчика
    public class OilLoader : ILoader
    {
        // Событие, когда погрузчик завершил загрузку
        public event EventHandler<double>? LoadingCompleted;
        
        // Событие, когда погрузчик отправил нефть
        public event EventHandler<double>? ShippingCompleted;

        public string Name { get; }
        public double MaxCapacity { get; }
        public double CurrentCapacity { get; private set; }
        
        // Конструктор с параметрами
        public OilLoader(string name, double maxCapacity)
        {
            Name = name;
            MaxCapacity = maxCapacity;
            CurrentCapacity = 0;
        }

        // Асинхронная загрузка нефти
        public async Task LoadOil(double amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));
                
            if (CurrentCapacity + amount > MaxCapacity)
                amount = MaxCapacity - CurrentCapacity;
                
            // Имитация времени загрузки
            await Task.Delay(TimeSpan.FromSeconds(amount / 10)); 
            
            CurrentCapacity += amount;
            
            // Вызываем событие завершения загрузки
            LoadingCompleted?.Invoke(this, amount);
        }

        // Асинхронная отправка нефти
        public async Task ShipOil()
        {
            if (CurrentCapacity <= 0)
                return;
                
            double amountShipped = CurrentCapacity;
                
            // Имитация времени отправки
            await Task.Delay(TimeSpan.FromSeconds(amountShipped / 20));
            
            CurrentCapacity = 0;
            
            // Вызываем событие завершения отправки
            ShippingCompleted?.Invoke(this, amountShipped);
        }
    }
}
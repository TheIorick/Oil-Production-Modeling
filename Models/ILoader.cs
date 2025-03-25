using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    // Интерфейс погрузчика
    public interface ILoader
    {
        // Название погрузчика
        string Name { get; }
        
        // Максимальная вместимость (в баррелях)
        double MaxCapacity { get; }
        
        // Текущая вместимость (в баррелях)
        double CurrentCapacity { get; }
        
        // Метод для загрузки нефти
        Task LoadOil(double amount);
        
        // Метод для отправки нефти
        Task ShipOil();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    public class OilRig
    {
        private readonly Random _random = new Random();
        private readonly double _fireChance; // Вероятность возгорания (от 0 до 1)
        private CancellationTokenSource? _productionCts;
        private double _oilProduced; // в баррелях
        
        // Событие возгорания
        public event EventHandler? FireStarted;
        
        // Событие изменения количества нефти
        public event EventHandler<double>? OilAmountChanged;
        
        // Событие готовности к отправке нефти
        public event EventHandler? OilReadyForShipment;
        
        // Событие изменения состояния
        public event EventHandler<OilRigStatus>? StatusChanged;
        
        public string Name { get; }
        public double ProductionRate { get; private set; } // баррелей в секунду
        public double StorageCapacity { get; } // максимальная вместимость хранилища в баррелях
        
        // Текущее состояние вышки
        private OilRigStatus _status;
        public OilRigStatus Status 
        { 
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(this, _status);
                    
                    // Если вышка стала рабочей, запускаем добычу
                    if (_status == OilRigStatus.Operational && !IsProducing)
                    {
                        StartProduction();
                    }
                    // Если вышка повреждена или неактивна, останавливаем добычу
                    else if (_status != OilRigStatus.Operational && IsProducing)
                    {
                        StopProduction();
                    }
                }
            }
        }
        
        // Текущее количество нефти в хранилище
        public double OilAmount 
        { 
            get => _oilProduced;
            private set
            {
                if (_oilProduced != value)
                {
                    _oilProduced = value;
                    OilAmountChanged?.Invoke(this, _oilProduced);
                    
                    // Если хранилище заполнено, уведомляем о готовности к отправке
                    if (_oilProduced >= StorageCapacity * 0.9)
                    {
                        OilReadyForShipment?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        public bool IsOnFire { get; private set; }
        public bool IsProducing { get; private set; }
        
        // Координаты для визуализации
        public double X { get; set; }
        public double Y { get; set; }
        
        public OilRig(string name, double productionRate, double storageCapacity, double fireChance, double x = 0, double y = 0)
        {
            if (fireChance < 0 || fireChance > 1)
                throw new ArgumentOutOfRangeException(nameof(fireChance), "Fire chance must be between 0 and 1");
                
            Name = name;
            ProductionRate = productionRate;
            StorageCapacity = storageCapacity;
            _fireChance = fireChance;
            Status = OilRigStatus.Inactive;
            IsOnFire = false;
            IsProducing = false;
            OilAmount = 0;
            X = x;
            Y = y;
        }
        
        // Запуск процесса добычи нефти
        public void StartProduction()
        {
            if (IsProducing)
                return;
                
            if (Status != OilRigStatus.Operational)
            {
                Status = OilRigStatus.Operational;
                return; // StartProduction будет вызван из сеттера Status
            }
            
            IsProducing = true;
            _productionCts = new CancellationTokenSource();
            
            Task.Run(async () =>
            {
                while (!_productionCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Имитация добычи нефти
                        await Task.Delay(1000, _productionCts.Token);
                        
                        // Проверка возгорания с заданной вероятностью
                        if (_random.NextDouble() < _fireChance)
                        {
                            StartFire();
                            break;
                        }
                        
                        // Добыча нефти, если хранилище не заполнено
                        if (OilAmount < StorageCapacity)
                        {
                            double newAmount = Math.Min(OilAmount + ProductionRate, StorageCapacity);
                            OilAmount = newAmount;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                
                IsProducing = false;
            }, _productionCts.Token);
        }
        
        // Остановка процесса добычи нефти
        public void StopProduction()
        {
            if (!IsProducing)
                return;
                
            _productionCts?.Cancel();
            _productionCts = null;
            IsProducing = false;
        }
        
        // Воспламенение вышки
        private void StartFire()
        {
            if (IsOnFire)
                return;
                
            IsOnFire = true;
            Status = OilRigStatus.Damaged;
            FireStarted?.Invoke(this, EventArgs.Empty);
        }
        
        // Тушение пожара
        public void ExtinguishFire()
        {
            IsOnFire = false;
        }
        
        // Отгрузка нефти погрузчику
        public async Task ShipOilTo(ILoader loader)
        {
            if (OilAmount <= 0)
                return;
                
            double amountToShip = Math.Min(OilAmount, loader.MaxCapacity - loader.CurrentCapacity);
            
            if (amountToShip <= 0)
                return;
                
            OilAmount -= amountToShip;
            await loader.LoadOil(amountToShip);
        }
        
        // Изменение скорости добычи (например, при улучшении оборудования)
        public void ChangeProductionRate(double newRate)
        {
            if (newRate <= 0)
                throw new ArgumentException("Production rate must be positive", nameof(newRate));
                
            ProductionRate = newRate;
        }
    }
}
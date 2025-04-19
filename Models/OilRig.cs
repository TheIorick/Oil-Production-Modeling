// --- Файл: OilRig.cs ---
using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Task3_10.Models
{
    public class OilRig : INotifyPropertyChanged
    {
        // Events
        public event EventHandler<FireEventArgs> FireOccurred;
        public event EventHandler<OilExtractedEventArgs> OilExtracted;
        public event PropertyChangedEventHandler PropertyChanged;

        // ... (свойства остаются прежними) ...
        private string _name;
        private double _oilExtractionRate; // barrels per minute
        private double _fireProbability; // probability of fire per minute
        private bool _isOperational;
        private double _oilStorage; // current oil storage in barrels
        private double _maxOilStorage; // maximum storage capacity
        private bool _isOnFire;

        private Random _random;
        private Task _extractionTask;
        private bool _isExtracting;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        // ... (остальные свойства) ...

        public double OilExtractionRate { /* ... */ }
        public double FireProbability { /* ... */ }
        public bool IsOperational
        {
            get => _isOperational;
            set { _isOperational = value; OnPropertyChanged(); } // Убедитесь, что OnPropertyChanged вызывается
        }
        public double OilStorage
        {
            get => _oilStorage;
            set { _oilStorage = value; OnPropertyChanged(); } // Убедитесь, что OnPropertyChanged вызывается
        }
        public double MaxOilStorage { /* ... */ }
        public bool IsOnFire
        {
            get => _isOnFire;
            set { _isOnFire = value; OnPropertyChanged(); } // Убедитесь, что OnPropertyChanged вызывается
        }


        public OilRig()
        {
            _random = new Random();
            IsOperational = true;
            IsOnFire = false;
        }

        public void StartExtraction()
        {
            if (_isExtracting || !IsOperational || IsOnFire)
                return;

            _isExtracting = true;
            _extractionTask = Task.Run(async () =>
            {
                while (_isExtracting && IsOperational && !IsOnFire)
                {
                    // Добыча нефти
                    if (OilStorage < MaxOilStorage)
                    {
                        double extractedAmount = OilExtractionRate / 60.0; // В секунду
                        OilStorage = Math.Min(OilStorage + extractedAmount, MaxOilStorage);
                        OnOilExtracted(new OilExtractedEventArgs { Amount = extractedAmount });
                    }
                    else
                    {
                        // Хранилище полное, можно приостановить добычу или просто ждать
                    }

                    // Проверка на пожар
                    CheckForFire();

                    await Task.Delay(1000); // Обновление каждую секунду
                }

                 // Если цикл завершился, сбрасываем флаг
                 _isExtracting = false;
                 // Если остановились из-за пожара или неработоспособности,
                 // IsOperational или IsOnFire уже установлены
            });
        }

        public void StopExtraction()
        {
            _isExtracting = false; // Сигнал для завершения задачи Task.Run
            // _extractionTask?.Wait(); // Можно дождаться завершения, но не обязательно
        }

        // Добавляем параметр forceFire
        public void CheckForFire(bool forceFire = false)
        {
            // Если уже горит, ничего не делаем
            if (IsOnFire) return;

            double chance = FireProbability / 60.0; // Шанс в секунду

            if (forceFire || _random.NextDouble() < chance)
            {
                IsOnFire = true;        // Устанавливаем флаг пожара
                IsOperational = false;  // Вышка не работает во время пожара
                StopExtraction();       // Останавливаем добычу
                OnFireOccurred(new FireEventArgs { Severity = _random.Next(1, 6) });
            }
        }

        public double RequestOilShipment(double amount)
        {
            if (IsOnFire || !IsOperational || amount <= 0 || OilStorage <= 0)
                return 0;

            double shippedAmount = Math.Min(amount, OilStorage);
            OilStorage -= shippedAmount;
            return shippedAmount;
        }

        protected virtual void OnFireOccurred(FireEventArgs e)
        {
            FireOccurred?.Invoke(this, e);
        }

        protected virtual void OnOilExtracted(OilExtractedEventArgs e)
        {
            OilExtracted?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    // ... (EventArgs остаются прежними) ...
    public class FireEventArgs : EventArgs { /* ... */ }
    public class OilExtractedEventArgs : EventArgs { /* ... */ }
}
using System;
using Task3_10.Models;
using System.ComponentModel;
using System.Diagnostics; // Добавляем для трассировки

namespace Task3_10.ViewModels
{
    public class OilRigViewModel : ViewModelBase
    {
        private OilRig _model;
        private string _status;
        private bool _isOnFire;
        private double _oilStorage;
        private double _x;
        private double _y;
        
        public OilRig Model => _model;
        
        public string Name => _model.Name;
        
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        public bool IsOnFire
        {
            get => _isOnFire;
            set => SetProperty(ref _isOnFire, value);
        }
        
        public double OilStorage
        {
            get => _oilStorage;
            set => SetProperty(ref _oilStorage, value);
        }
        
        public double X
        {
            get => _x;
            set
            {
                if (SetProperty(ref _x, value))
                {
                    // Выводим значение в консоль для отладки
                    Debug.WriteLine($"Rig {Name} X changed to {value}");
                }
            }
        }
        
        public double Y
        {
            get => _y;
            set
            {
                if (SetProperty(ref _y, value))
                {
                    // Выводим значение в консоль для отладки
                    Debug.WriteLine($"Rig {Name} Y changed to {value}");
                }
            }
        }
        
        public OilRigViewModel(OilRig model)
        {
            _model = model;
            _status = model.IsOperational ? "Operational" : "Not Operational";
            _isOnFire = model.IsOnFire;
            _oilStorage = model.OilStorage;
            
            // Задаём случайную позицию для нефтяной вышки
            var random = new Random();
            _x = random.Next(100, 700);
            _y = random.Next(100, 300);
            
            // Выводим данные для отладки
            Debug.WriteLine($"Created OilRig: {Name}, X={X}, Y={Y}");
            
            // Подписываемся на события модели
            _model.OilExtracted += OnOilExtracted;
            _model.PropertyChanged += OnModelPropertyChanged;
        }
        
        private void OnOilExtracted(object sender, OilExtractedEventArgs e)
        {
            OilStorage = _model.OilStorage;
        }
        
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Обновляем свойства ViewModel при изменении свойств модели
            if (e.PropertyName == nameof(OilRig.IsOperational) || e.PropertyName == nameof(OilRig.IsOnFire))
            {
                Status = _model.IsOperational ? (_model.IsOnFire ? "On Fire!" : "Operational") : "Not Operational";
                Debug.WriteLine($"Rig {Name} status changed to {Status}");
            }
            
            if (e.PropertyName == nameof(OilRig.IsOnFire))
            {
                IsOnFire = _model.IsOnFire;
                Debug.WriteLine($"Rig {Name} fire status changed to {IsOnFire}");
            }
            
            if (e.PropertyName == nameof(OilRig.OilStorage))
            {
                OilStorage = _model.OilStorage;
            }
        }
        
        public void StartExtraction()
        {
            _model.StartExtraction();
            Debug.WriteLine($"Rig {Name} started extraction");
        }
        
        public void StopExtraction()
        {
            _model.StopExtraction();
            Debug.WriteLine($"Rig {Name} stopped extraction");
        }
    }
}
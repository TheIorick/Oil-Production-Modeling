using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    public class TruckLoader : INotifyPropertyChanged, IOilLoader
    {
        public event EventHandler<LoadingCompletedEventArgs> LoadingCompleted;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string _name;
        private double _capacity;
        private double _currentLoad;
        private bool _isBusy;
        private double _x;
        private double _y;
        private double _speed; // скорость перемещения
        
        public string Name 
        { 
            get => _name; 
            set { _name = value; OnPropertyChanged(); } 
        }
        
        public double Capacity 
        { 
            get => _capacity; 
            set { _capacity = value; OnPropertyChanged(); } 
        }
        
        public double CurrentLoad 
        { 
            get => _currentLoad; 
            set { _currentLoad = value; OnPropertyChanged(); } 
        }
        
        public bool IsBusy 
        { 
            get => _isBusy; 
            set { _isBusy = value; OnPropertyChanged(); } 
        }
        
        public double X 
        { 
            get => _x; 
            set { _x = value; OnPropertyChanged(); } 
        }
        
        public double Y 
        { 
            get => _y; 
            set { _y = value; OnPropertyChanged(); } 
        }
        
        public double Speed
        {
            get => _speed;
            set { _speed = value; OnPropertyChanged(); }
        }
        
        public TruckLoader()
        {
            IsBusy = false;
            CurrentLoad = 0;
        }
        
        public async Task LoadOil(OilRig rig, double amount)
        {
            if (IsBusy || amount <= 0 || CurrentLoad >= Capacity)
                return;
                
            IsBusy = true;
            
            // Simulate loading time based on amount
            int loadingTimeSeconds = (int)(3 + amount / 50);
            
            // Request oil from the rig
            double actualLoadedAmount = rig.RequestOilShipment(Math.Min(amount, Capacity - CurrentLoad));
            
            if (actualLoadedAmount > 0)
            {
                // Simulate loading time
                await Task.Delay(loadingTimeSeconds * 1000);
                
                CurrentLoad += actualLoadedAmount;
                
                // Notify loading completed
                LoadingCompleted?.Invoke(this, new LoadingCompletedEventArgs { 
                    Amount = actualLoadedAmount, 
                    Rig = rig 
                });
            }
            
            IsBusy = false;
        }
        
        public async Task TransportOil()
        {
            if (IsBusy || CurrentLoad <= 0)
                return;
                
            IsBusy = true;
            
            // Simulate transport time based on current load and speed
            int transportTimeSeconds = (int)(5 + CurrentLoad / (Speed / 10));
            
            // Simulate transport
            await Task.Delay(transportTimeSeconds * 1000);
            
            // Reset current load after delivery
            CurrentLoad = 0;
            
            IsBusy = false;
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Task3_10.Models
{
    
    public class TruckLoader : IOilLoader, INotifyPropertyChanged
    {
        // Implement IOilLoader interface
        public event EventHandler<LoadingCompletedEventArgs> LoadingCompleted;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string _name;
        private double _capacity;
        private double _currentLoad;
        private bool _isBusy;
        private double _x;
        private double _y;
        private double _speed;
        
        // Properties with notification
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
            if (IsBusy || CurrentLoad >= Capacity)
                return;
                
            IsBusy = true;
            
            // Request oil shipment from rig
            double loadedAmount = rig.RequestOilShipment(Math.Min(amount, Capacity - CurrentLoad));
            
            // Simulate loading time (1 second per 10 barrels)
            int loadingTimeSeconds = (int)(loadedAmount / 10);
            await Task.Delay(loadingTimeSeconds * 1000);
            
            // Update current load
            CurrentLoad += loadedAmount;
            
            // Notify loading completed
            OnLoadingCompleted(new LoadingCompletedEventArgs { Amount = loadedAmount, Rig = rig });
            
            IsBusy = false;
        }
        
        public async Task TransportOil()
        {
            if (IsBusy || CurrentLoad <= 0)
                return;
                
            IsBusy = true;
            
            // Simulate transport time (distance based or fixed)
            await Task.Delay(5000);
            
            // Reset load after delivery
            CurrentLoad = 0;
            
            IsBusy = false;
        }
        
        protected virtual void OnLoadingCompleted(LoadingCompletedEventArgs e)
        {
            LoadingCompleted?.Invoke(this, e);
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
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
        
        // Properties with notification
        public string Name 
        { 
            get => _name; 
            set { _name = value; OnPropertyChanged(); } 
        }
        
        public double OilExtractionRate 
        { 
            get => _oilExtractionRate; 
            set { _oilExtractionRate = value; OnPropertyChanged(); } 
        }
        
        public double FireProbability 
        { 
            get => _fireProbability; 
            set { _fireProbability = value; OnPropertyChanged(); } 
        }
        
        public bool IsOperational 
        { 
            get => _isOperational; 
            set { _isOperational = value; OnPropertyChanged(); } 
        }
        
        public double OilStorage 
        { 
            get => _oilStorage; 
            set { _oilStorage = value; OnPropertyChanged(); } 
        }
        
        public double MaxOilStorage 
        { 
            get => _maxOilStorage; 
            set { _maxOilStorage = value; OnPropertyChanged(); } 
        }
        
        public bool IsOnFire 
        { 
            get => _isOnFire; 
            set { _isOnFire = value; OnPropertyChanged(); } 
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
                    // Extract oil
                    if (OilStorage < MaxOilStorage)
                    {
                        double extractedAmount = OilExtractionRate / 60; // per second
                        OilStorage = Math.Min(OilStorage + extractedAmount, MaxOilStorage);
                        
                        // Notify about extracted oil
                        OnOilExtracted(new OilExtractedEventArgs { Amount = extractedAmount });
                        
                        // Check for fire
                        CheckForFire();
                    }
                    
                    await Task.Delay(1000); // Update every second
                }
            });
        }
        
        public void StopExtraction()
        {
            _isExtracting = false;
        }
        
        private void CheckForFire()
        {
            // Calculate probability per second
            double secondProbability = FireProbability / 60;
            
            if (_random.NextDouble() < secondProbability)
            {
                IsOnFire = true;
                IsOperational = false;
                OnFireOccurred(new FireEventArgs { Severity = _random.Next(1, 6) });
            }
        }
        
        public double RequestOilShipment(double amount)
        {
            if (amount <= 0 || OilStorage <= 0)
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
    
    public class FireEventArgs : EventArgs
    {
        public int Severity { get; set; } // 1-5 scale
    }
    
    public class OilExtractedEventArgs : EventArgs
    {
        public double Amount { get; set; }
    }
}
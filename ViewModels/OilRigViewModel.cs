using System;
using Task3_10.Models;
using System.ComponentModel;

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
            set => SetProperty(ref _x, value);
        }
        
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
        
        public OilRigViewModel(OilRig model)
        {
            _model = model;
            _status = model.IsOperational ? "Operational" : "Not Operational";
            _isOnFire = model.IsOnFire;
            _oilStorage = model.OilStorage;
            
            // Random position for the rig
            var random = new Random();
            _x = random.Next(100, 700);
            _y = random.Next(100, 300);
            
            // Subscribe to model events
            _model.OilExtracted += OnOilExtracted;
            _model.PropertyChanged += OnModelPropertyChanged;
        }
        
        private void OnOilExtracted(object sender, OilExtractedEventArgs e)
        {
            OilStorage = _model.OilStorage;
        }
        
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update view model properties when model properties change
            if (e.PropertyName == nameof(OilRig.IsOperational) || e.PropertyName == nameof(OilRig.IsOnFire))
            {
                Status = _model.IsOperational ? (_model.IsOnFire ? "On Fire!" : "Operational") : "Not Operational";
            }
            
            if (e.PropertyName == nameof(OilRig.IsOnFire))
            {
                IsOnFire = _model.IsOnFire;
            }
            
            if (e.PropertyName == nameof(OilRig.OilStorage))
            {
                OilStorage = _model.OilStorage;
            }
        }
        
        public void StartExtraction()
        {
            _model.StartExtraction();
        }
        
        public void StopExtraction()
        {
            _model.StopExtraction();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3_10.Models;
using System.ComponentModel;
using Avalonia.Threading;

namespace Task3_10.ViewModels
{
    public class LoaderViewModel : ViewModelBase
    {
        private IOilLoader _model;
        private string _status;
        private double _x;
        private double _y;
        private double _targetX;
        private double _targetY;
        private Task _movementTask;
        private bool _isMoving;
        
        public IOilLoader Model => _model;
        
        public string Name => _model.Name;
        
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        public double CurrentLoad => _model.CurrentLoad;
        
        public double Capacity => _model.Capacity;
        
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
        
        public LoaderViewModel(IOilLoader model)
        {
            _model = model;
            _status = model.IsBusy ? "Busy" : "Available";
            
            // Random position for the loader off-screen
            _x = -100;
            _y = 300;
            _model.X = _x;
            _model.Y = _y;
            
            // Логирование создания объекта
            Log($"LoaderViewModel created: {Name}, X={X}, Y={Y}");
            
            try
            {
                // Subscribe to model events
                ((INotifyPropertyChanged)_model).PropertyChanged += OnModelPropertyChanged;
                _model.LoadingCompleted += OnLoadingCompleted;
                Log("Successfully subscribed to model events");
            }
            catch (Exception ex)
            {
                Log($"Error subscribing to events: {ex.Message}");
            }
        }
        
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IOilLoader.IsBusy))
            {
                Status = _model.IsBusy ? "Busy" : "Available";
                Log($"{Name} status changed: {Status}");
            }
            
            if (e.PropertyName == nameof(IOilLoader.CurrentLoad))
            {
                OnPropertyChanged(nameof(CurrentLoad));
                Log($"{Name} current load: {CurrentLoad:F1}/{Capacity}");
            }
            
            if (e.PropertyName == nameof(IOilLoader.X))
            {
                X = _model.X;
                // Логируем только значительные изменения позиции
                if (Math.Abs(_x - _model.X) > 10)
                    Log($"{Name} X position changed: {X}");
            }
            
            if (e.PropertyName == nameof(IOilLoader.Y))
            {
                Y = _model.Y;
                // Логируем только значительные изменения позиции
                if (Math.Abs(_y - _model.Y) > 10)
                    Log($"{Name} Y position changed: {Y}");
            }
        }
        
        private void OnLoadingCompleted(object sender, LoadingCompletedEventArgs e)
        {
            Status = "Loading Completed";
            OnPropertyChanged(nameof(CurrentLoad));
            Log($"{Name} completed loading {e.Amount:F1} from {e.Rig.Name}");
        }
        
        public async Task MoveTo(double targetX, double targetY, double speed = 80)
        {
            if (_isMoving)
                return;
                
            _isMoving = true;
            _targetX = targetX;
            _targetY = targetY;
            
            Log($"{Name} moving to X={targetX}, Y={targetY}");
            
            _movementTask = Task.Run(async () =>
            {
                while (_isMoving && (Math.Abs(X - _targetX) > 5 || Math.Abs(Y - _targetY) > 5))
                {
                    // Calculate direction vector
                    double dx = _targetX - X;
                    double dy = _targetY - Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    
                    // Normalize and scale by speed
                    if (distance > 0)
                    {
                        dx = dx / distance * Math.Min(speed / 10, distance);
                        dy = dy / distance * Math.Min(speed / 10, distance);
                    }
                    
                    // Update position
                    _model.X += dx;
                    _model.Y += dy;
                    X = _model.X;
                    Y = _model.Y;
                    
                    await Task.Delay(50);
                }
                
                _isMoving = false;
                Log($"{Name} arrived at X={X}, Y={Y}");
            });
            
            await _movementTask;
        }
        
        public async Task LoadFromRig(OilRigViewModel rigViewModel)
        {
            if (_model.IsBusy || rigViewModel.Model.OilStorage <= 0)
                return;
                
            Status = "Moving to Rig";
            
            // Move to the rig
            await MoveTo(rigViewModel.X, rigViewModel.Y);
            
            Status = "Loading Oil";
            
            // Start loading
            await _model.LoadOil(rigViewModel.Model, _model.Capacity - _model.CurrentLoad);
            
            // If loaded, move off-screen to "deliver"
            if (_model.CurrentLoad > 0)
            {
                Status = "Delivering Oil";
                await MoveTo(-100, 300);
                await _model.TransportOil();
                Status = "Available";
            }
        }

        private void Log(string message)
        {
            Dispatcher.UIThread.Post(() => 
                MainWindowViewModel.GlobalLogAction?.Invoke($"[Loader] {message}"));
        }
    }
}
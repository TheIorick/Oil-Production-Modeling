using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Task3_10.Models;
using System.ComponentModel;

namespace Task3_10.ViewModels
{
    public class MechanicViewModel : ViewModelBase
    {
        private Mechanic _model;
        private string _status;
        private double _x;
        private double _y;
        private double _targetX;
        private double _targetY;
        private Task _movementTask;
        private bool _isMoving;
        
        public Mechanic Model => _model;
        
        public string Name => _model.Name;
        
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
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
        
        public MechanicViewModel(Mechanic model)
        {
            _model = model;
            _status = model.IsBusy ? "Busy" : "Available";
            
            // Random position for the mechanic
            var random = new Random();
            _x = random.Next(100, 700);
            _y = random.Next(400, 500);
            _model.X = _x;
            _model.Y = _y;
            
            // Subscribe to model events
            _model.PropertyChanged += OnModelPropertyChanged;
            _model.RepairCompleted += OnRepairCompleted;
        }
        
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mechanic.IsBusy))
            {
                Status = _model.IsBusy ? "Busy" : "Available";
            }
            
            if (e.PropertyName == nameof(Mechanic.X))
            {
                X = _model.X;
            }
            
            if (e.PropertyName == nameof(Mechanic.Y))
            {
                Y = _model.Y;
            }
        }
        
        private void OnRepairCompleted(object sender, RepairCompletedEventArgs e)
        {
            Status = "Repair Completed";
        }
        
        public async Task MoveTo(double targetX, double targetY, double speed = 50)
        {
            if (_isMoving)
                return;
                
            _isMoving = true;
            _targetX = targetX;
            _targetY = targetY;
            
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
            });
            
            await _movementTask;
        }
        
        public async Task RepairRig(OilRigViewModel rigViewModel)
        {
            if (_model.IsBusy || !rigViewModel.IsOnFire)
                return;
                
            Status = "Moving to Rig";
            
            // Move to the rig
            await MoveTo(rigViewModel.X, rigViewModel.Y);
            
            Status = "Repairing";
            
            // Start repair
            await _model.RepairRig(rigViewModel.Model, 3); // Assuming fire severity is 3
        }
    }
}
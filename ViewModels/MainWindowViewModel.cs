using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reflection;
using Task3_10.Models;
using Avalonia.Threading;
using System.Collections.ObjectModel;

namespace Task3_10.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<OilRigViewModel> _rigs;
        private ObservableCollection<MechanicViewModel> _mechanics;
        private ObservableCollection<LoaderViewModel> _loaders;
        private string _logText;
        private DispatcherTimer _simulationTimer;
        private Random _random;
        
        public ObservableCollection<OilRigViewModel> Rigs
        {
            get => _rigs;
            set => SetProperty(ref _rigs, value);
        }
        
        public ObservableCollection<MechanicViewModel> Mechanics
        {
            get => _mechanics;
            set => SetProperty(ref _mechanics, value);
        }
        
        public ObservableCollection<LoaderViewModel> Loaders
        {
            get => _loaders;
            set => SetProperty(ref _loaders, value);
        }
        
        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }
        
        // Commands
        public ICommand AddRigCommand { get; }
        public ICommand AddMechanicCommand { get; }
        public ICommand AddLoaderCommand { get; }
        
        public MainWindowViewModel()
        {
            _rigs = new ObservableCollection<OilRigViewModel>();
            _mechanics = new ObservableCollection<MechanicViewModel>();
            _loaders = new ObservableCollection<LoaderViewModel>();
            _logText = "Simulation started.\n";
            _random = new Random();
            
            // Initialize commands
            AddRigCommand = new RelayCommand(AddRig);
            AddMechanicCommand = new RelayCommand(AddMechanic);
            AddLoaderCommand = new RelayCommand(AddLoader);
            
            // Set up a timer for simulation events
            _simulationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            
            _simulationTimer.Tick += SimulationTimerTick;
            _simulationTimer.Start();
        }
        
        private async void SimulationTimerTick(object sender, EventArgs e)
        {
            // Randomly decide what to do in the simulation
            int action = _random.Next(0, 3);
            
            switch (action)
            {
                case 0:
                    // Try to send a loader to a rig with oil
                    await TrySendLoaderToRig();
                    break;
                    
                case 1:
                    // Try to have a mechanic repair a rig on fire
                    await TrySendMechanicToRig();
                    break;
                    
                case 2:
                    // Nothing happens this tick
                    break;
            }
        }
        
        private async Task TrySendLoaderToRig()
        {
            if (_rigs.Count == 0 || _loaders.Count == 0)
                return;
                
            // Find a rig with oil and an available loader
            var rig = _rigs.FirstOrDefault(r => r.Model.OilStorage > 0);
            var loader = _loaders.FirstOrDefault(l => !l.Model.IsBusy);
            
            if (rig != null && loader != null)
            {
                AddLog($"Sending {loader.Name} to load oil from {rig.Name}");
                await loader.LoadFromRig(rig);
            }
        }
        
        private async Task TrySendMechanicToRig()
        {
            if (_rigs.Count == 0 || _mechanics.Count == 0)
                return;
                
            // Find a rig on fire and an available mechanic
            var rig = _rigs.FirstOrDefault(r => r.IsOnFire);
            var mechanic = _mechanics.FirstOrDefault(m => !m.Model.IsBusy);
            
            if (rig != null && mechanic != null)
            {
                AddLog($"Sending {mechanic.Name} to repair {rig.Name} that is on fire");
                await mechanic.RepairRig(rig);
            }
        }
        
        private void AddRig()
        {
            // Create a new oil rig with random parameters
            var rig = new OilRig
            {
                Name = $"Rig-{_rigs.Count + 1}",
                OilExtractionRate = _random.Next(10, 30),
                FireProbability = _random.Next(1, 10) / 100.0, // 1-10%
                MaxOilStorage = _random.Next(500, 1000)
            };
            
            var rigViewModel = new OilRigViewModel(rig);
            
            // Subscribe to fire events
            rig.FireOccurred += (s, e) => 
            {
                Dispatcher.UIThread.InvokeAsync(() => 
                {
                    AddLog($"Fire occurred at {rig.Name} with severity {e.Severity}!");
                });
            };
            
            Rigs.Add(rigViewModel);
            rigViewModel.StartExtraction();
            AddLog($"Added new oil rig: {rig.Name}");
        }
        
        private void AddMechanic()
        {
            // Create a new mechanic with random parameters
            var mechanic = new Mechanic
            {
                Name = $"Mechanic-{_mechanics.Count + 1}",
                SkillLevel = _random.Next(1, 11) // 1-10
            };
            
            var mechanicViewModel = new MechanicViewModel(mechanic);
            
            // Subscribe to repair events
            mechanic.RepairCompleted += (s, e) => 
            {
                Dispatcher.UIThread.InvokeAsync(() => 
                {
                    AddLog($"{mechanic.Name} completed repairs on {e.Rig.Name} in {e.RepairTimeSeconds} seconds");
                });
            };
            
            Mechanics.Add(mechanicViewModel);
            AddLog($"Added new mechanic: {mechanic.Name} with skill level {mechanic.SkillLevel}");
        }
        
        private void AddLoader()
        {
            // Create a new loader with random parameters
            var loader = new TruckLoader
            {
                Name = $"Loader-{_loaders.Count + 1}",
                Capacity = _random.Next(100, 300),
                Speed = _random.Next(40, 80)
            };
            
            var loaderViewModel = new LoaderViewModel(loader);
            
            // Subscribe to loading events
            loader.LoadingCompleted += (s, e) => 
            {
                Dispatcher.UIThread.InvokeAsync(() => 
                {
                    AddLog($"{loader.Name} loaded {e.Amount:F1} barrels from {e.Rig.Name}");
                });
            };
            
            Loaders.Add(loaderViewModel);
            AddLog($"Added new loader: {loader.Name} with capacity {loader.Capacity}");
        }
        
        private void AddLog(string message)
        {
            LogText = $"[{DateTime.Now:HH:mm:ss}] {message}\n" + LogText;
            
            // Trim log if it gets too long
            if (LogText.Length > 5000)
            {
                LogText = LogText.Substring(0, 5000);
            }
        }
    }
    
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        public event EventHandler CanExecuteChanged;
        
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        
        public void Execute(object parameter) => _execute();
        
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Task3_10.Models;

namespace Task3_10.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly OilProductionSimulator _simulator = new();

        public ObservableCollection<OilRig> OilRigs => _simulator.OilRigs;
        public ObservableCollection<Mechanic> Mechanics => _simulator.Mechanics;
        public ObservableCollection<ILoader> Loaders => _simulator.Loaders;
        public ObservableCollection<string> LogMessages { get; } = new();

        // Пример дополнительных свойств для возможного расширения.
        [ObservableProperty]
        private OilRig? selectedOilRig;

        [ObservableProperty]
        private string newRigName = "Нефтяная вышка";

        [ObservableProperty]
        private double newRigProductionRate = 5.0;

        [ObservableProperty]
        private double newRigStorageCapacity = 100.0;

        [ObservableProperty]
        private double newRigFireChance = 0.05;

        [ObservableProperty]
        private string newMechanicName = "Механик";

        [ObservableProperty]
        private int newMechanicSkillLevel = 5;

        [ObservableProperty]
        private string newLoaderName = "Погрузчик";

        [ObservableProperty]
        private double newLoaderCapacity = 50.0;

        [ObservableProperty]
        private string simulationStatistics = "";

        public MainWindowViewModel()
        {
            _simulator.SimulationStarted += (s, e) => IsSimulationRunning = true;
            _simulator.SimulationStopped += (s, e) => IsSimulationRunning = false;
            _simulator.LogEvent += (s, message) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                    while (LogMessages.Count > 100)
                        LogMessages.RemoveAt(0);
                });
            };

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    UpdateStatistics();
                }
            });
        }

        [ObservableProperty]
        private bool isSimulationRunning;

        [RelayCommand]
        private void StartSimulation() => _simulator.Start();

        [RelayCommand]
        private void StopSimulation() => _simulator.Stop();

        [RelayCommand]
        private void AddOilRig()
        {
            // Генерируем случайное положение.
            Random random = new();
            double x = random.Next(50, 700);
            double y = random.Next(50, 300);
            var rig = new OilRig(NewRigName, NewRigProductionRate, NewRigStorageCapacity, NewRigFireChance, x, y);
            _simulator.AddOilRig(rig);
            NewRigName = $"Нефтяная вышка {OilRigs.Count + 1}";
        }

        [RelayCommand]
        private void AddMechanic()
        {
            var mech = new Mechanic(NewMechanicName, NewMechanicSkillLevel);
            _simulator.AddMechanic(mech);
            NewMechanicName = $"Механик {Mechanics.Count + 1}";
        }

        [RelayCommand]
        private void AddLoader()
        {
            var loader = new OilLoader(NewLoaderName, NewLoaderCapacity);
            _simulator.AddLoader(loader);
            NewLoaderName = $"Погрузчик {Loaders.Count + 1}";
        }

        [RelayCommand]
        private void RemoveOilRig(OilRig rig)
        {
            if (rig != null)
                _simulator.RemoveOilRig(rig);
        }

        [RelayCommand]
        private void UpdateStatistics()
        {
            SimulationStatistics = _simulator.GetSimulationStatistics();
        }

        [RelayCommand]
        private void ClearLogs() => LogMessages.Clear();
    }
}
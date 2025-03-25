using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Task3_10.Models;

namespace Task3_10.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly OilProductionSimulator _simulator = new();

        // Наблюдаемые коллекции для привязки данных
        public ObservableCollection<OilRig> OilRigs => _simulator.OilRigs;
        public ObservableCollection<Mechanic> Mechanics => _simulator.Mechanics;
        public ObservableCollection<ILoader> Loaders => _simulator.Loaders;
        public ObservableCollection<string> LogMessages { get; } = new();

        [ObservableProperty]
        private bool _isSimulationRunning;

        [ObservableProperty]
        private string _newRigName = "Нефтяная вышка";

        [ObservableProperty]
        private double _newRigProductionRate = 5.0;

        [ObservableProperty]
        private double _newRigStorageCapacity = 100.0;

        [ObservableProperty]
        private double _newRigFireChance = 0.05;

        [ObservableProperty]
        private string _newMechanicName = "Механик";

        [ObservableProperty]
        private int _newMechanicSkillLevel = 5;

        [ObservableProperty]
        private string _newLoaderName = "Погрузчик";

        [ObservableProperty]
        private double _newLoaderCapacity = 50.0;

        [ObservableProperty]
        private string _simulationStatistics = "";

        public MainWindowViewModel()
        {
            // Подписываемся на события симулятора
            _simulator.SimulationStarted += (s, e) => IsSimulationRunning = true;
            _simulator.SimulationStopped += (s, e) => IsSimulationRunning = false;
            _simulator.LogEvent += (s, message) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                    
                    // Ограничиваем количество сообщений для производительности
                    while (LogMessages.Count > 100)
                        LogMessages.RemoveAt(0);
                });
            };

            // Обновляем статистику каждые 5 секунд
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    UpdateStatistics();
                }
            });
        }

        // Команда запуска симуляции
        [RelayCommand]
        private void StartSimulation()
        {
            _simulator.Start();
        }

        // Команда остановки симуляции
        [RelayCommand]
        private void StopSimulation()
        {
            _simulator.Stop();
        }

        // Команда добавления нефтяной вышки
        [RelayCommand]
        private void AddOilRig()
        {
            // Генерируем случайные координаты для расположения на канвасе
            Random random = new Random();
            double x = random.Next(50, 700);
            double y = random.Next(50, 300);

            var rig = new OilRig(
                NewRigName,
                NewRigProductionRate,
                NewRigStorageCapacity,
                NewRigFireChance,
                x,
                y
            );

            _simulator.AddOilRig(rig);
            
            // Сбрасываем имя, чтобы следующая вышка имела уникальное имя
            NewRigName = $"Нефтяная вышка {OilRigs.Count + 1}";
        }

        // Команда добавления механика
        [RelayCommand]
        private void AddMechanic()
        {
            var mechanic = new Mechanic(
                NewMechanicName,
                NewMechanicSkillLevel
            );

            _simulator.AddMechanic(mechanic);
            
            // Сбрасываем имя для следующего механика
            NewMechanicName = $"Механик {Mechanics.Count + 1}";
        }

        // Команда добавления погрузчика
        [RelayCommand]
        private void AddLoader()
        {
            var loader = new OilLoader(
                NewLoaderName,
                NewLoaderCapacity
            );

            _simulator.AddLoader(loader);
            
            // Сбрасываем имя для следующего погрузчика
            NewLoaderName = $"Погрузчик {Loaders.Count + 1}";
        }

        // Команда удаления нефтяной вышки
        [RelayCommand]
        private void RemoveOilRig(OilRig rig)
        {
            if (rig != null)
            {
                _simulator.RemoveOilRig(rig);
            }
        }

        // Команда обновления статистики
        [RelayCommand]
        private void UpdateStatistics()
        {
            SimulationStatistics = _simulator.GetSimulationStatistics();
        }

        // Команда очистки логов
        [RelayCommand]
        private void ClearLogs()
        {
            LogMessages.Clear();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Task3_10.Models
{
    // Класс для управления симуляцией
    public class OilProductionSimulator
    {
        private readonly Random _random = new Random();
        private CancellationTokenSource? _simulationCts;

        // Коллекции объектов симуляции
        public ObservableCollection<OilRig> OilRigs { get; } = new();
        public ObservableCollection<Mechanic> Mechanics { get; } = new();
        public ObservableCollection<ILoader> Loaders { get; } = new();

        // События изменения состояния симуляции
        public event EventHandler? SimulationStarted;
        public event EventHandler? SimulationStopped;
        
        // Событие для логирования действий
        public event EventHandler<string>? LogEvent;

        public bool IsRunning { get; private set; }

        // Запуск симуляции
        public void Start()
        {
            if (IsRunning)
                return;

            _simulationCts = new CancellationTokenSource();
            IsRunning = true;
            
            SimulationStarted?.Invoke(this, EventArgs.Empty);
            LogEvent?.Invoke(this, "Симуляция запущена");

            // Запускаем все нефтяные вышки
            foreach (var rig in OilRigs)
            {
                rig.Status = OilRigStatus.Operational;
                SubscribeToRigEvents(rig);
            }

            // Запускаем фоновый процесс для обработки событий и управления симуляцией
            Task.Run(RunSimulationLoop, _simulationCts.Token);
        }

        // Остановка симуляции
        public void Stop()
        {
            if (!IsRunning)
                return;

            _simulationCts?.Cancel();
            _simulationCts = null;
            IsRunning = false;

            // Останавливаем все нефтяные вышки
            foreach (var rig in OilRigs)
            {
                rig.StopProduction();
                UnsubscribeFromRigEvents(rig);
            }

            SimulationStopped?.Invoke(this, EventArgs.Empty);
            LogEvent?.Invoke(this, "Симуляция остановлена");
        }

        // Основной цикл симуляции
        private async Task RunSimulationLoop()
        {
            while (!_simulationCts!.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(5000, _simulationCts.Token); // Проверка каждые 5 секунд
                    
                    // Проверяем наличие вышек, нуждающихся в ремонте
                    await HandleRigsNeedingRepair();
                    
                    // Отправляем погрузчики к вышкам с заполненным хранилищем
                    await DispatchLoadersToRigs();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogEvent?.Invoke(this, $"Ошибка в симуляции: {ex.Message}");
                }
            }
        }

        // Обработка вышек, нуждающихся в ремонте
        private async Task HandleRigsNeedingRepair()
        {
            foreach (var rig in OilRigs)
            {
                if ((rig.IsOnFire || rig.Status == OilRigStatus.Damaged) && !rig.IsProducing)
                {
                    // Поиск свободного механика
                    Mechanic? availableMechanic = Mechanics.FirstOrDefault(m => !m.IsBusy);
                    
                    if (availableMechanic != null)
                    {
                        LogEvent?.Invoke(this, $"Механик {availableMechanic.Name} направляется на ремонт вышки {rig.Name}");
                        
                        // Запускаем ремонт асинхронно
                        _ = availableMechanic.RepairRig(rig, _simulationCts!.Token);
                    }
                }
            }
            
            await Task.CompletedTask;
        }

        // Отправка погрузчиков к вышкам с заполненным хранилищем
        private async Task DispatchLoadersToRigs()
        {
            foreach (var rig in OilRigs)
            {
                if (rig.Status == OilRigStatus.Operational && rig.OilAmount > rig.StorageCapacity * 0.9)
                {
                    // Поиск доступного погрузчика
                    ILoader? availableLoader = Loaders.FirstOrDefault(l => l.CurrentCapacity < l.MaxCapacity * 0.9);
                    
                    if (availableLoader != null)
                    {
                        LogEvent?.Invoke(this, $"Погрузчик {availableLoader.Name} отправляется к вышке {rig.Name}");
                        
                        // Отгружаем нефть асинхронно
                        await rig.ShipOilTo(availableLoader);
                        
                        LogEvent?.Invoke(this, $"Погрузчик {availableLoader.Name} загружен, отправляет нефть");
                        
                        // Отправляем нефть
                        await availableLoader.ShipOil();
                    }
                }
            }
        }

        // Добавление новой нефтяной вышки
        public void AddOilRig(OilRig rig)
        {
            OilRigs.Add(rig);
            
            if (IsRunning)
            {
                SubscribeToRigEvents(rig);
                rig.Status = OilRigStatus.Operational;
            }
            
            LogEvent?.Invoke(this, $"Добавлена новая нефтяная вышка: {rig.Name}");
        }

        // Добавление нового механика
        public void AddMechanic(Mechanic mechanic)
        {
            Mechanics.Add(mechanic);
            
            // Подписываемся на события механика для логирования
            mechanic.RepairStarted += (s, e) => LogEvent?.Invoke(this, $"Механик {mechanic.Name} начал ремонт");
            mechanic.RepairCompleted += (s, e) => LogEvent?.Invoke(this, $"Механик {mechanic.Name} завершил ремонт");
            
            LogEvent?.Invoke(this, $"Добавлен новый механик: {mechanic.Name} (уровень навыка: {mechanic.SkillLevel})");
        }

        // Добавление нового погрузчика
        public void AddLoader(ILoader loader)
        {
            Loaders.Add(loader);
            
            // Если это OilLoader, подписываемся на его события
            if (loader is OilLoader oilLoader)
            {
                oilLoader.LoadingCompleted += (s, amount) => LogEvent?.Invoke(this, $"Погрузчик {loader.Name} загрузил {amount} баррелей нефти");
                oilLoader.ShippingCompleted += (s, amount) => LogEvent?.Invoke(this, $"Погрузчик {loader.Name} отправил {amount} баррелей нефти");
            }
            
            LogEvent?.Invoke(this, $"Добавлен новый погрузчик: {loader.Name} (вместимость: {loader.MaxCapacity} баррелей)");
        }

        // Удаление нефтяной вышки
        public void RemoveOilRig(OilRig rig)
        {
            UnsubscribeFromRigEvents(rig);
            rig.StopProduction();
            OilRigs.Remove(rig);
            LogEvent?.Invoke(this, $"Удалена нефтяная вышка: {rig.Name}");
        }

        // Подписка на события нефтяной вышки
        private void SubscribeToRigEvents(OilRig rig)
        {
            rig.FireStarted += (s, e) => LogEvent?.Invoke(this, $"ТРЕВОГА! Возгорание на вышке {rig.Name}!");
            rig.StatusChanged += (s, status) => LogEvent?.Invoke(this, $"Статус вышки {rig.Name} изменился на {status}");
            rig.OilReadyForShipment += (s, e) => LogEvent?.Invoke(this, $"Вышка {rig.Name} готова к отгрузке нефти");
        }

        // Отписка от событий нефтяной вышки
        private void UnsubscribeFromRigEvents(OilRig rig)
        {
            // Используем рефлексию для отписки от всех событий
            Type type = typeof(OilRig);
            EventInfo[] events = type.GetEvents();
            
            foreach (var eventInfo in events)
            {
                var field = type.GetField(eventInfo.Name, 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
                
                if (field != null)
                {
                    field.SetValue(rig, null);
                }
            }
        }

        // Пример использования рефлексии: получение статистики по объектам симуляции
        public string GetSimulationStatistics()
        {
            string stats = "Статистика симуляции:\n";
            
            // Получаем статистику по нефтяным вышкам
            stats += $"Нефтяные вышки ({OilRigs.Count}):\n";
            foreach (var rig in OilRigs)
            {
                // Используем рефлексию для получения всех свойств
                PropertyInfo[] properties = typeof(OilRig).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                stats += $"  - {rig.Name}:\n";
                foreach (var prop in properties)
                {
                    // Пропускаем некоторые свойства для краткости
                    if (prop.Name != "Name" && prop.PropertyType != typeof(ObservableCollection<>))
                    {
                        try
                        {
                            var value = prop.GetValue(rig);
                            stats += $"    {prop.Name}: {value}\n";
                        }
                        catch
                        {
                            // Пропускаем свойства, которые вызывают исключения
                        }
                    }
                }
            }
            
            // Аналогично для других объектов...
            
            return stats;
        }
    }
}
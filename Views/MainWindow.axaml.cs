using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Task3_10.ViewModels;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Task3_10.Views
{
    public partial class MainWindow : Window
    {
        private Canvas _simulationCanvas;
        private Dictionary<object, Control> _entityControls = new Dictionary<object, Control>();
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Инициализация и работа с Canvas должны выполняться после загрузки окна
            this.Loaded += MainWindow_Loaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            _simulationCanvas = this.FindControl<Canvas>("SimulationCanvas");
            
            // Настраиваем кнопку отладки
            var debugButton = this.FindControl<Button>("DebugButton");
            if (debugButton != null)
            {
                debugButton.Click += Debug_Click;
            }
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel == null || _simulationCanvas == null)
            {
                Console.WriteLine("ViewModel or Canvas is null in Loaded event");
                return;
            }
            
            Console.WriteLine($"MainWindow loaded. Canvas found: {_simulationCanvas != null}");
            
            // Подписываемся на изменения коллекций
            viewModel.Rigs.CollectionChanged += Rigs_CollectionChanged;
            viewModel.Mechanics.CollectionChanged += Mechanics_CollectionChanged;
            viewModel.Loaders.CollectionChanged += Loaders_CollectionChanged;
            
            // Добавляем существующие элементы
            foreach (var rig in viewModel.Rigs)
            {
                AddRigToCanvas(rig);
                rig.PropertyChanged += Entity_PropertyChanged;
            }
            
            foreach (var mechanic in viewModel.Mechanics)
            {
                AddMechanicToCanvas(mechanic);
                mechanic.PropertyChanged += Entity_PropertyChanged;
            }
            
            foreach (var loader in viewModel.Loaders)
            {
                AddLoaderToCanvas(loader);
                loader.PropertyChanged += Entity_PropertyChanged;
            }
            
            Console.WriteLine($"Existing entities added: Rigs={viewModel.Rigs.Count}, Mechanics={viewModel.Mechanics.Count}, Loaders={viewModel.Loaders.Count}");
        }
        
        #region Collection Changed Handlers
        
        private void Rigs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OilRigViewModel rig in e.NewItems)
                {
                    AddRigToCanvas(rig);
                    rig.PropertyChanged += Entity_PropertyChanged;
                }
            }
            
            if (e.OldItems != null)
            {
                foreach (OilRigViewModel rig in e.OldItems)
                {
                    RemoveEntityFromCanvas(rig);
                    rig.PropertyChanged -= Entity_PropertyChanged;
                }
            }
        }
        
        private void Mechanics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MechanicViewModel mechanic in e.NewItems)
                {
                    AddMechanicToCanvas(mechanic);
                    mechanic.PropertyChanged += Entity_PropertyChanged;
                }
            }
            
            if (e.OldItems != null)
            {
                foreach (MechanicViewModel mechanic in e.OldItems)
                {
                    RemoveEntityFromCanvas(mechanic);
                    mechanic.PropertyChanged -= Entity_PropertyChanged;
                }
            }
        }
        
        private void Loaders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LoaderViewModel loader in e.NewItems)
                {
                    AddLoaderToCanvas(loader);
                    loader.PropertyChanged += Entity_PropertyChanged;
                }
            }
            
            if (e.OldItems != null)
            {
                foreach (LoaderViewModel loader in e.OldItems)
                {
                    RemoveEntityFromCanvas(loader);
                    loader.PropertyChanged -= Entity_PropertyChanged;
                }
            }
        }
        
        #endregion
        
        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Обновляем положение и внешний вид элементов при изменении свойств моделей
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                UpdateEntityPosition(sender);
            }
            else if (e.PropertyName == "IsOnFire" || e.PropertyName == "Status" || 
                    e.PropertyName == "OilStorage" || e.PropertyName == "CurrentLoad")
            {
                // Для этих свойств создаем новый визуальный элемент
                if (sender is OilRigViewModel rig)
                {
                    RemoveEntityFromCanvas(rig);
                    AddRigToCanvas(rig);
                }
                else if (sender is MechanicViewModel mechanic)
                {
                    RemoveEntityFromCanvas(mechanic);
                    AddMechanicToCanvas(mechanic);
                }
                else if (sender is LoaderViewModel loader)
                {
                    RemoveEntityFromCanvas(loader);
                    AddLoaderToCanvas(loader);
                }
            }
        }
        
        #region Entity Creation
        
        private void AddRigToCanvas(OilRigViewModel rig)
        {
            if (_simulationCanvas == null) return;
            
            try
            {
                // Создаем контейнер для всех элементов нефтяной вышки
                var container = new Grid();
                
                // База вышки
                var baseRect = new Rectangle
                {
                    Width = 80,
                    Height = 120,
                    Fill = rig.IsOnFire ? 
                        new SolidColorBrush(Colors.Red) : 
                        new SolidColorBrush(Colors.Blue),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2
                };
                container.Children.Add(baseRect);
                
                // Вышка (верхняя часть)
                var towerRect = new Rectangle
                {
                    Width = 10,
                    Height = 50,
                    Fill = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(35, -50, 0, 0)
                };
                container.Children.Add(towerRect);
                
                // Название вышки
                var nameText = new TextBlock
                {
                    Text = rig.Name,
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                container.Children.Add(nameText);
                
                // Статус вышки
                var statusText = new TextBlock
                {
                    Text = rig.Status,
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                container.Children.Add(statusText);
                
                // Индикатор запаса нефти
                var progressBar = new ProgressBar
                {
                    Value = rig.OilStorage,
                    Maximum = 1000,
                    Height = 10,
                    Width = 70,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 5),
                    Foreground = new SolidColorBrush(Colors.OrangeRed)
                };
                container.Children.Add(progressBar);
                
                // Устанавливаем позицию на канвасе
                Canvas.SetLeft(container, rig.X);
                Canvas.SetTop(container, rig.Y);
                
                // Добавляем элемент на канвас
                _simulationCanvas.Children.Add(container);
                _entityControls[rig] = container;
                
                Console.WriteLine($"Added rig {rig.Name} to canvas at X={rig.X}, Y={rig.Y}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding rig to canvas: {ex.Message}");
            }
        }
        
        private void AddMechanicToCanvas(MechanicViewModel mechanic)
        {
            if (_simulationCanvas == null) return;
            
            try
            {
                // Создаем контейнер для всех элементов механика
                var container = new Grid();
                
                // Механик (круг)
                var ellipse = new Ellipse
                {
                    Width = 30,
                    Height = 30,
                    Fill = new SolidColorBrush(Colors.RoyalBlue),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                container.Children.Add(ellipse);
                
                // Буква "M" в круге
                var letterText = new TextBlock
                {
                    Text = "M",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                container.Children.Add(letterText);
                
                // Имя механика
                var nameText = new TextBlock
                {
                    Text = mechanic.Name,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    Margin = new Thickness(0, 30, 0, 0)
                };
                container.Children.Add(nameText);
                
                // Статус механика
                var statusText = new TextBlock
                {
                    Text = mechanic.Status,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 10,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    Margin = new Thickness(0, 45, 0, 0)
                };
                container.Children.Add(statusText);
                
                // Устанавливаем позицию на канвасе
                Canvas.SetLeft(container, mechanic.X);
                Canvas.SetTop(container, mechanic.Y);
                
                // Добавляем элемент на канвас
                _simulationCanvas.Children.Add(container);
                _entityControls[mechanic] = container;
                
                Console.WriteLine($"Added mechanic {mechanic.Name} to canvas at X={mechanic.X}, Y={mechanic.Y}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding mechanic to canvas: {ex.Message}");
            }
        }
        
        private void AddLoaderToCanvas(LoaderViewModel loader)
        {
            if (_simulationCanvas == null) return;
            
            try
            {
                // Создаем контейнер для всех элементов загрузчика
                var container = new Grid();
                
                // Грузовик (прямоугольник)
                var rect = new Rectangle
                {
                    Width = 60,
                    Height = 40,
                    Fill = new SolidColorBrush(Colors.ForestGreen),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                container.Children.Add(rect);
                
                // Колеса
                var leftWheel = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(8, 28, 0, 0),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
                };
                container.Children.Add(leftWheel);
                
                var rightWheel = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(0, 28, 8, 0),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
                };
                container.Children.Add(rightWheel);
                
                // Буква "T" в грузовике
                var letterText = new TextBlock
                {
                    Text = "T",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                container.Children.Add(letterText);
                
                // Имя загрузчика
                var nameText = new TextBlock
                {
                    Text = loader.Name,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    Margin = new Thickness(0, 40, 0, 0)
                };
                container.Children.Add(nameText);
                
                // Статус загрузчика
                var statusText = new TextBlock
                {
                    Text = loader.Status,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 10,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    Margin = new Thickness(0, 55, 0, 0)
                };
                container.Children.Add(statusText);
                
                // Индикатор загрузки
                var progressBar = new ProgressBar
                {
                    Value = loader.CurrentLoad,
                    Maximum = loader.Capacity,
                    Width = 50,
                    Height = 5,
                    Margin = new Thickness(0, 0, 0, 5),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Foreground = new SolidColorBrush(Colors.OrangeRed)
                };
                container.Children.Add(progressBar);
                
                // Устанавливаем позицию на канвасе
                Canvas.SetLeft(container, loader.X);
                Canvas.SetTop(container, loader.Y);
                
                // Добавляем элемент на канвас
                _simulationCanvas.Children.Add(container);
                _entityControls[loader] = container;
                
                Console.WriteLine($"Added loader {loader.Name} to canvas at X={loader.X}, Y={loader.Y}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding loader to canvas: {ex.Message}");
            }
        }
        
        #endregion
        
        private void RemoveEntityFromCanvas(object entity)
        {
            if (_simulationCanvas == null || !_entityControls.ContainsKey(entity)) return;
            
            try
            {
                var control = _entityControls[entity];
                _simulationCanvas.Children.Remove(control);
                _entityControls.Remove(entity);
                
                Console.WriteLine($"Removed entity from canvas: {entity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing entity from canvas: {ex.Message}");
            }
        }
        
        private void UpdateEntityPosition(object entity)
        {
            if (!_entityControls.ContainsKey(entity)) return;
            
            var control = _entityControls[entity];
            
            try
            {
                if (entity is OilRigViewModel rig)
                {
                    Canvas.SetLeft(control, rig.X);
                    Canvas.SetTop(control, rig.Y);
                    
                    Console.WriteLine($"Updated rig position: {rig.Name} to X={rig.X}, Y={rig.Y}");
                }
                else if (entity is MechanicViewModel mechanic)
                {
                    Canvas.SetLeft(control, mechanic.X);
                    Canvas.SetTop(control, mechanic.Y);
                    
                    Console.WriteLine($"Updated mechanic position: {mechanic.Name} to X={mechanic.X}, Y={mechanic.Y}");
                }
                else if (entity is LoaderViewModel loader)
                {
                    Canvas.SetLeft(control, loader.X);
                    Canvas.SetTop(control, loader.Y);
                    
                    Console.WriteLine($"Updated loader position: {loader.Name} to X={loader.X}, Y={loader.Y}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating entity position: {ex.Message}");
            }
        }
        
        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel == null)
            {
                Console.WriteLine("ViewModel is null!");
                return;
            }
            
            Console.WriteLine($"Debug Info:");
            Console.WriteLine($"Number of rigs: {viewModel.Rigs?.Count ?? 0}");
            Console.WriteLine($"Number of mechanics: {viewModel.Mechanics?.Count ?? 0}");
            Console.WriteLine($"Number of loaders: {viewModel.Loaders?.Count ?? 0}");
            
            Console.WriteLine($"Canvas found: {_simulationCanvas != null}");
            Console.WriteLine($"Canvas children count: {_simulationCanvas?.Children.Count ?? 0}");
            Console.WriteLine($"Entity controls dictionary count: {_entityControls.Count}");
            
            // Добавляем тестовый элемент
            var testRect = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new SolidColorBrush(Colors.Purple),
                Stroke = new SolidColorBrush(Colors.Gold),
                StrokeThickness = 3
            };
            
            Canvas.SetLeft(testRect, 400);
            Canvas.SetTop(testRect, 200);
            
            _simulationCanvas?.Children.Add(testRect);
            Console.WriteLine("Added test rectangle at X=400, Y=200");
            
            // Добавляем информацию в лог приложения
            viewModel.LogText = $"[DEBUG] Canvas children count: {_simulationCanvas?.Children.Count ?? 0}\n" + viewModel.LogText;
        }
    }
}
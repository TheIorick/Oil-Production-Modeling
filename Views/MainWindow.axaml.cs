using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Task3_10.ViewModels;

namespace Task3_10.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Запускаем таймер для анимации
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(50); // 20 FPS
                    await Dispatcher.UIThread.InvokeAsync(UpdateAnimations);
                }
            });
        }
        
        // Метод для обновления анимаций
        private void UpdateAnimations()
        {
            // Здесь можно добавить дополнительные анимации элементов
            // Например, движение погрузчиков, анимацию пламени и т.д.
        }
    }
}
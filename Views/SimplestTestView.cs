using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace Task3_10.Views
{
    public partial class SimplestTestView : Window
    {
        private Canvas _canvas;
        private Random _random = new Random();
        
        public SimplestTestView()
        {
            InitializeComponent();
            
            // Получаем ссылку на Canvas
            _canvas = this.FindControl<Canvas>("TestCanvas");
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void AddShapes_Click(object sender, RoutedEventArgs e)
        {
            if (_canvas == null) return;
            
            // Добавляем синий прямоугольник (нефтяная вышка)
            var rig = new Rectangle
            {
                Width = 80,
                Height = 120,
                Fill = new SolidColorBrush(Colors.Blue),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2
            };
            Canvas.SetLeft(rig, 400);
            Canvas.SetTop(rig, 200);
            _canvas.Children.Add(rig);
            
            // Добавляем текст внутри прямоугольника
            var text = new TextBlock
            {
                Text = "Rig-1",
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeight.Bold
            };
            Canvas.SetLeft(text, 420);
            Canvas.SetTop(text, 250);
            _canvas.Children.Add(text);
            
            // Добавляем синий круг (механик)
            var mechanic = new Ellipse
            {
                Width = 30, 
                Height = 30,
                Fill = new SolidColorBrush(Colors.RoyalBlue)
            };
            Canvas.SetLeft(mechanic, 300);
            Canvas.SetTop(mechanic, 350);
            _canvas.Children.Add(mechanic);
            
            // Добавляем зеленый прямоугольник (загрузчик)
            var loader = new Rectangle
            {
                Width = 60,
                Height = 40,
                Fill = new SolidColorBrush(Colors.ForestGreen),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(loader, 500);
            Canvas.SetTop(loader, 350);
            _canvas.Children.Add(loader);
            
            Console.WriteLine($"Added shapes to canvas. Total children: {_canvas.Children.Count}");
        }
        
        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (_canvas == null) return;
            
            // Удаляем все элементы кроме исходных трех, которые были добавлены в XAML
            while (_canvas.Children.Count > 3)
            {
                _canvas.Children.RemoveAt(_canvas.Children.Count - 1);
            }
            
            Console.WriteLine($"Canvas cleared. Remaining children: {_canvas.Children.Count}");
        }
    }
}
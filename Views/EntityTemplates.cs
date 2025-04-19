using Avalonia.Controls;
using Avalonia.Media;
using Task3_10.ViewModels;
using Task3_10.Models;
using System;
using Avalonia.Controls.Shapes;

namespace Task3_10.Views
{
    public static class EntityTemplates
    {
        // Создает визуальный элемент для нефтяной вышки
        public static Grid CreateRigTemplate(OilRigViewModel viewModel)
        {
            var grid = new Grid();
            
            // База буровой
            var baseRect = new Rectangle
            {
                Width = 80,
                Height = 120,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            
            // Связываем цвет с состоянием пожара
            if (viewModel.IsOnFire)
                baseRect.Fill = new SolidColorBrush(Colors.Red);
            else
                baseRect.Fill = new SolidColorBrush(Colors.Blue);
            
            // Вышка буровой
            var towerRect = new Rectangle
            {
                Width = 10,
                Height = 50,
                Fill = new SolidColorBrush(Colors.Gray),
                Margin = new Avalonia.Thickness(35, -50, 0, 0)
            };
            
            // Название вышки
            var nameTextBlock = new TextBlock
            {
                Text = viewModel.Name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = Brushes.White
            };
            
            // Статус вышки
            var statusTextBlock = new TextBlock
            {
                Text = viewModel.Status,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Margin = new Avalonia.Thickness(0, 0, 0, 20),
                Foreground = Brushes.White
            };
            
            // Индикатор запаса нефти
            var progressBar = new ProgressBar
            {
                Value = viewModel.OilStorage,
                Maximum = 1000,
                Height = 10,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Margin = new Avalonia.Thickness(5),
                Foreground = new SolidColorBrush(Colors.OrangeRed)
            };
            
            // Добавляем элементы в Grid
            grid.Children.Add(baseRect);
            grid.Children.Add(towerRect);
            grid.Children.Add(nameTextBlock);
            grid.Children.Add(statusTextBlock);
            grid.Children.Add(progressBar);
            
            return grid;
        }
        
        // Создает визуальный элемент для механика
        public static Grid CreateMechanicTemplate(MechanicViewModel viewModel)
        {
            var grid = new Grid();
            
            // Механик (круг)
            var ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Fill = new SolidColorBrush(Colors.RoyalBlue),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            
            // Буква "M" в круге
            var textBlock = new TextBlock
            {
                Text = "M",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = Brushes.White
            };
            
            // Имя механика
            var nameTextBlock = new TextBlock
            {
                Text = viewModel.Name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                Margin = new Avalonia.Thickness(0, 30, 0, 0),
                Foreground = Brushes.Black
            };
            
            // Статус механика
            var statusTextBlock = new TextBlock
            {
                Text = viewModel.Status,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                Margin = new Avalonia.Thickness(0, 45, 0, 0),
                Foreground = Brushes.Gray,
                FontSize = 10
            };
            
            // Добавляем элементы в Grid
            grid.Children.Add(ellipse);
            grid.Children.Add(textBlock);
            grid.Children.Add(nameTextBlock);
            grid.Children.Add(statusTextBlock);
            
            return grid;
        }
        
        // Создает визуальный элемент для загрузчика
        public static Grid CreateLoaderTemplate(LoaderViewModel viewModel)
        {
            var grid = new Grid();
            
            // Грузовик (прямоугольник)
            var rect = new Rectangle
            {
                Width = 60,
                Height = 40,
                Fill = new SolidColorBrush(Colors.ForestGreen),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            
            // Колеса
            var leftWheel = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Black,
                Margin = new Avalonia.Thickness(8, 28, 0, 0),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
            };
            
            var rightWheel = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Black,
                Margin = new Avalonia.Thickness(0, 28, 8, 0),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
            };
            
            // Буква "T" на грузовике
            var textBlock = new TextBlock
            {
                Text = "T",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = Brushes.White
            };
            
            // Имя загрузчика
            var nameTextBlock = new TextBlock
            {
                Text = viewModel.Name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                Margin = new Avalonia.Thickness(0, 40, 0, 0),
                Foreground = Brushes.Black
            };
            
            // Статус загрузчика
            var statusTextBlock = new TextBlock
            {
                Text = viewModel.Status,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                Margin = new Avalonia.Thickness(0, 55, 0, 0),
                Foreground = Brushes.Gray,
                FontSize = 10
            };
            
            // Индикатор загрузки
            var progressBar = new ProgressBar
            {
                Value = viewModel.CurrentLoad,
                Maximum = viewModel.Capacity,
                Width = 50,
                Height = 5,
                Margin = new Avalonia.Thickness(0, 0, 0, 5),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Foreground = new SolidColorBrush(Colors.OrangeRed)
            };
            
            // Добавляем элементы в Grid
            grid.Children.Add(rect);
            grid.Children.Add(leftWheel);
            grid.Children.Add(rightWheel);
            grid.Children.Add(textBlock);
            grid.Children.Add(nameTextBlock);
            grid.Children.Add(statusTextBlock);
            grid.Children.Add(progressBar);
            
            return grid;
        }
    }
}
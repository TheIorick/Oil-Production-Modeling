using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Task3_10.ViewModels
{
    /// <summary>
    /// Реализация интерфейса ICommand для связывания команд в MVVM архитектуре
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Событие, вызываемое при изменении возможности выполнения команды
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Создает новый экземпляр команды RelayCommand
        /// </summary>
        /// <param name="execute">Метод, выполняемый при вызове команды</param>
        /// <param name="canExecute">Метод, определяющий, может ли команда быть выполнена</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "Делегат Execute не может быть null");
            _canExecute = canExecute;
        }

        /// <summary>
        /// Создает новый экземпляр команды RelayCommand без параметра
        /// </summary>
        /// <param name="execute">Метод без параметров, выполняемый при вызове команды</param>
        /// <param name="canExecute">Метод без параметров, определяющий, может ли команда быть выполнена</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(
                execute == null ? null : _ => execute(),
                canExecute == null ? null : _ => canExecute())
        {
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена в текущем состоянии
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        /// <returns>True, если команда может быть выполнена; иначе False</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполняет логику команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Вызывает событие CanExecuteChanged, уведомляющее систему о необходимости
        /// повторной проверки возможности выполнения команды
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
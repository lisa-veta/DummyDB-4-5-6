using System;
using System.Windows.Input;

namespace HardLab5
{
    /// <summary>
    ///  DataContext="{Binding DataTable}" ItemsSource="{Binding DefaultView}"
    /// Selected="TableSelected"
    /// Unselected="TableUnselected"
    /// 
    /// 
    /// RowBackground="LightYellow" AlternatingRowBackground="LightBlue"
    /// </summary>
    /// 
    class DelegateCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        // Событие, необходимое для ICommand
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Два конструктора
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object param)
        {
            return this.canExecute == null || this.canExecute(param);
        }
        public void Execute(object param)
        {
            this.execute(param);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Screentroll
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> OnExec;
        private readonly Predicate<object> OnCanExec;

        public RelayCommand(Action<object> onExec, Predicate<object> onCanExec)
        {
            if (onExec == null) throw new ArgumentNullException(nameof(onExec));
            this.OnExec = onExec;
            this.OnCanExec = onCanExec;
        }
        public RelayCommand(Action<object> onExec) : this(onExec, null) { }

        public bool CanExecute(object parameter)
        {
            return OnCanExec == null ? true : OnCanExec(parameter);
        }

        public void Execute(object parameter)
        {
            OnExec?.Invoke(parameter);
        }

        public void InvokeCanExecuteChanged(object sender)
        {
            this.CanExecuteChanged?.Invoke(sender, new EventArgs());

        }
    }
}

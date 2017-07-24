using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Screentroll.ViewModels
{
    public class MainWindowViewModel : AbstractViewModel
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();
        [DllImport("user32.dll", SetLastError = true)]
        static extern short GetAsyncKeyState(System.Int32 key);

        private readonly string[] Messages = new[]{
            "Smile!"
        };



        private string _displayText = "";
        private bool _showCake = false;
        private BitmapImage _image;
        private DispatcherTimer timer;
        private int steps = 0;

        public bool ShowCake
        {
            get => _showCake;
            set
            {
                this._showCake = value;
                InvokePropertyChanged();
            }
        }
        
        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                InvokePropertyChanged();
            }
        }

        public BitmapImage Image
        {
            get => _image;
            set
            {
                _image = value;
                InvokePropertyChanged();
            }
        }
        public bool IsCanceled { get; private set; }

        private readonly Action _unregisterTriggers;
        private readonly Action _takePicture;
        public MainWindowViewModel(Action unregisterTriggers, Action takePicture)
        {
            if (unregisterTriggers == null) throw new ArgumentNullException(nameof(unregisterTriggers));
            if (takePicture == null) throw new ArgumentNullException(nameof(takePicture));
            _unregisterTriggers = unregisterTriggers;
            _takePicture = takePicture;
        }
        public void TestSpecialKeyShortcuts()
        {
            if (
                ((GetAsyncKeyState(17) & (1 << 15)) != 0 && (GetAsyncKeyState(18) & (1 << 15)) != 0 && (GetAsyncKeyState(46) & (1 << 15)) != 0)
                )
            {
                DisplayText = "Someone wanted to bypass everything!!!";
                this.OnCancel();
                LockWorkStation();
            }

        }
        public void OnTroll()
        {
            this.DisplayText = Messages[this.steps++];
            timer = new DispatcherTimer(DispatcherPriority.DataBind);
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0,0,500);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

                this.timer.Stop();
                //Take picture
                _takePicture();
                Task.Delay(1500).ContinueWith(a =>
                {
                    if (!this.IsCanceled) { 
                        if (LockWorkStation())
                        {
                            DisplayText = $"Your PC was locked due an unwanted intruder at {DateTime.Now.ToShortTimeString()}";
                        }
                        _unregisterTriggers.Invoke();
                    }
                });
        }

        public void OnCancel()
        {

            this.timer?.Stop();
            this.IsCanceled = true;
            this._unregisterTriggers.Invoke();
            this.timer = null;
            DisplayText = $"Good bye!";
            Task.Delay(2500).ContinueWith(_ =>
                App.Current.Dispatcher.Invoke(
                    () => App.Current.Shutdown()
                )
            );
        }
    }
}

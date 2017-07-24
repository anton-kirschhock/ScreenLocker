using Emgu.CV;
using Emgu.CV.Structure;
using Screentroll.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Screentroll
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private MainWindowViewModel mainVm;
        private DispatcherTimer keyChecker;
        private List<Window> AllWindows = new List<Window>();
        private bool Triggered = false;
        private bool Finished = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            mainVm = new MainWindowViewModel(Unregister, TakeScreenShot);
            foreach (var screen in Screen.AllScreens)
            {
                var ratio = Math.Max(screen.WorkingArea.Width / screen.Bounds.Width,
                        screen.WorkingArea.Height / screen.Bounds.Height);
                var window = new MainWindow();

                if (screen.Primary)
                {
                    Current.MainWindow = window;
                }
                window.DataContext = mainVm;
                

                window.MouseMove += Window_MouseMove; ;
                window.KeyDown += Window_KeyDown;
                window.MouseLeftButtonDown += Window_MouseMove;
                window.MouseRightButtonDown += Window_MouseMove;
                window.Closing += Window_Closing;
                window.Left = screen.WorkingArea.Left / ratio;
                window.Top = screen.WorkingArea.Top / ratio;
                window.Width = screen.WorkingArea.Width / ratio;
                window.Height = screen.WorkingArea.Height / ratio;
                window.Show();
                window.WindowState = WindowState.Maximized;
                AllWindows.Add(window);
            }
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            base.OnStartup(e);
            this.keyChecker = new DispatcherTimer(DispatcherPriority.DataBind);
            this.keyChecker.Interval = new TimeSpan(0, 0, 0, 0, 500);
            this.keyChecker.Start();
            this.keyChecker.Tick += KeyChecker_Tick;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.Finished) e.Cancel = true;
        }

        private void KeyChecker_Tick(object sender, EventArgs e)
        {
            this.mainVm.TestSpecialKeyShortcuts();
        }

        

        public void TakeScreenShot()
        {
            using(var frame = new Capture().QueryFrame())
            {
                if(frame != null)
                {
                    using(var stream = new MemoryStream())
                    {
                        frame.Bitmap.Save(stream, ImageFormat.Bmp);
                        frame.Bitmap.Save($"{DateTime.Now.Ticks}.png", ImageFormat.Png);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(stream.ToArray());
                        bitmap.EndInit();
                        
                        this.mainVm.Image = bitmap;
                        this.mainVm.ShowCake = true;
                    }
                }
                else 
                    this.mainVm.ShowCake = false;
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs me)
        {
            if (Triggered) {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
                    Screen.PrimaryScreen.Bounds.Width / 2,
                    Screen.PrimaryScreen.Bounds.Height / 2
                    );
            }
            else { 
                Triggered = true;
                me.Handled = true;
                this.mainVm.OnTroll();
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
                Screen.PrimaryScreen.Bounds.Width / 2,
                Screen.PrimaryScreen.Bounds.Height / 2
                );
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs ke)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt) && ke.Key == Key.F9)
            {
                this.mainVm.OnCancel();
            }
            else if (!Triggered)
            {
                Triggered = true;
                this.mainVm.OnTroll();
            }
        }

        public void Unregister()
        {
            Finished = true;
            foreach (var window in AllWindows)
            {
                window.KeyDown -= Window_KeyDown;
                window.MouseMove -= Window_MouseMove;
                window.MouseLeftButtonDown -= Window_MouseMove;
                window.MouseRightButtonDown -= Window_MouseMove;
            }
        }
        
    }
}

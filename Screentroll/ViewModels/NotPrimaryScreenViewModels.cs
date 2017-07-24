using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screentroll.ViewModels
{
    public class NotPrimaryScreenViewModels: AbstractViewModel
    {

        public bool ShowCake
        {
            get => false;
            set { }
        }

        public string DisplayText
        {
            get => string.Empty;
            set { }
        }
    }
}

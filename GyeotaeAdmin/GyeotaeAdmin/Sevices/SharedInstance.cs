using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public static class SharedInstance
    {
        public static SharedDataService SharedData { get; } = new SharedDataService();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Configs
{
    public class GlobalConfig : Config
    {
        public bool CanMediStopBleeding { get; set; } = false;

        public override string FileName => "Global";
    }
}

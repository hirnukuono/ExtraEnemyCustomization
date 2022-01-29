using Agents;
using EECustom.Customizations.Detections;
using EECustom.Networking;
using EECustom.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Models
{
    public class ScannerSync : StateReplicator<ScannerStatusPacket>
    {
        public override bool ClearOnLevelCleanup => true;
    }

    public struct ScannerStatusPacket
    {
        public AgentMode mode;
    }
}

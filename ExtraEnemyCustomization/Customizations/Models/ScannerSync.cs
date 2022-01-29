using Agents;
using EECustom.Customizations.Detections;
using EECustom.Networking;
using EECustom.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Models
{
    public class ScannerSync : SyncedEvent<ScannerStatusPacket>
    {
        public override void Receive(ScannerStatusPacket packet)
        {
            var status = EnemyProperty<EnemyScannerStatus>.Get(packet.enemyID);
            if (status != null)
                status.Mode = packet.mode;
        }
    }

    public struct ScannerStatusPacket
    {
        public ushort enemyID;
        public AgentMode mode;
    }
}

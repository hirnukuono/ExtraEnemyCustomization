using Agents;
using EECustom.Networking;

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
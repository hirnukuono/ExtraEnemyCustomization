using GTFO.API;

namespace EECustom.Networking
{
    public abstract class SyncedEvent<T> where T : struct
    {
        public delegate void ReceiveHandler(T packet);

        public bool IsSetup { get => _isSetup; }
        public string EventName { get; private set; } = string.Empty;

        private bool _isSetup = false;

        public void Setup(ushort replicatorID = 0)
        {
            if (_isSetup)
                return;

            EventName = $"EEC_Networking_{typeof(T).Name}_{replicatorID}";
            NetworkAPI.RegisterEvent<T>(EventName, ReceiveClient_Callback);
            _isSetup = true;
        }

        public void Send(T packetData)
        {
            NetworkAPI.InvokeEvent(EventName, packetData);
            ReceiveLocal_Callback(packetData);
        }

        private void ReceiveLocal_Callback(T packet)
        {
            ReceiveLocal(packet);
            OnReceiveLocal?.Invoke(packet);
            Receive(packet);
            OnReceive?.Invoke(packet);
        }

        private void ReceiveClient_Callback(ulong sender, T packet)
        {
            Receive(packet);
            OnReceive?.Invoke(packet);
        }

        public virtual void ReceiveLocal(T packet)
        {

        }

        public virtual void Receive(T packet)
        {

        }

        public event ReceiveHandler OnReceive;
        public event ReceiveHandler OnReceiveLocal;
    }
}

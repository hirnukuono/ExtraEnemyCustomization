using GameData;
using System;

namespace EECustom.Utils.JsonElements
{
    //MINOR: NOTE! Calling ctor for LocalizedText before game has loaded will leads to crash.
    public sealed class EventWrapper : IDisposable
    {
        private string _json;
        private WardenObjectiveEventData _cached = null;

        public EventWrapper(string json)
        {
            _json = json;
        }

        public void Cache()
        {
            _cached = JSON.Deserialize<WardenObjectiveEventData>(_json);
            _json = string.Empty;
        }

        public WardenObjectiveEventData ToEvent()
        {
            if (_cached == null)
            {
                Cache();
            }

            return _cached;
        }

        public void Dispose()
        {
            _json = null;
            _cached = null;
        }
    }
}
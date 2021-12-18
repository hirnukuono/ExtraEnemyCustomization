using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Utils.JsonElements
{
    public struct VoiceEvent
    {
        public VoiceType VoiceType;
        public uint VoiceID;
    }

    public enum VoiceType
    {
        UsingID,
        Walk,
    }
}

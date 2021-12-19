using EECustom.Attributes;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib;

namespace EECustom.Networking
{
    [InjectToIl2Cpp]
    public class CustomSNetPacket<T> : SNet_Packet where T : struct
    {
        public static SNet_Packet<T> Create(Action<T> receiveAction, Action<T> validateAction = null)
        {
            if (!SNet_Packet<T>.s_hasMarshaller)
            {
                //MAJOR: FIX THIS
                //SNet_Packet<T>.s_hasMarshaller = SNet_Marshal.TryGetMarshaler<T>(out SNet_Packet<T>.s_marshaller);
                if (!SNet_Packet<T>.s_hasMarshaller)
                {
                    return null;
                }
            }
            return new SNet_Packet<T>
            {
                ReceiveAction = receiveAction,
                ValidateAction = validateAction,
                m_hasValidateAction = (validateAction != null)
            };
        }

        public override void Setup(IReplicator replicator, byte index)
        {
            base.Setup(replicator, index);
        }

        public override void ReceiveBytes(Il2CppStructArray<byte> bytes)
        {
            base.ReceiveBytes(bytes);
        }
    }
}

using GameData;

namespace EEC
{
    public static class EnemyDataBlockExtension
    {
        public static bool TryGetBehaviourBlock(this EnemyDataBlock data, out EnemyBehaviorDataBlock behaviour)
        {
            if (data == null)
            {
                behaviour = null;
                return false;
            }

            behaviour = EnemyBehaviorDataBlock.GetBlock(data.BehaviorDataId);
            return behaviour != null;
        }

        public static bool TryGetBalancingBlock(this EnemyDataBlock data, out EnemyBalancingDataBlock balancing)
        {
            if (data == null)
            {
                balancing = null;
                return false;
            }

            balancing = EnemyBalancingDataBlock.GetBlock(data.BalancingDataId);
            return balancing != null;
        }
    }
}

using EECustom.Utils.JsonElements;
using Enemies;
using GameData;

namespace EECustom.Customizations.Abilities
{
    public sealed class BirthingCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public uint EnemyGroupToSpawn { get; set; }
        public ValueBase ChildrenCost { get; set; } = ValueBase.Unchanged;
        public ValueBase ChildrenPerBirth { get; set; } = ValueBase.Unchanged;
        public ValueBase ChildrenPerBirthMin { get; set; } = ValueBase.Unchanged;
        public ValueBase ChildrenMax { get; set; } = ValueBase.Unchanged;
        public ValueBase MinDelayUntilNextBirth { get; set; } = ValueBase.Unchanged;
        public ValueBase MaxDelayUntilNextBirth { get; set; } = ValueBase.Unchanged;

        public override string GetProcessName()
        {
            return "Birthing";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var eabBirth = agent.GetComponentInChildren<EAB_Birthing>(true);
            if (eabBirth != null)
            {
                eabBirth.m_groupDataBlock = GameDataBlockBase<EnemyGroupDataBlock>.GetBlock(EnemyGroupToSpawn);
                eabBirth.m_childrenCost = ChildrenCost.GetAbsValue(eabBirth.m_childrenCost);
                eabBirth.m_childrenPerBirth = ChildrenPerBirth.GetAbsValue(eabBirth.m_childrenPerBirth);
                eabBirth.m_childrenAllowedSpawn = ChildrenPerBirth.GetAbsValue(eabBirth.m_childrenAllowedSpawn);
                eabBirth.m_childrenPerBirthMin = ChildrenPerBirthMin.GetAbsValue(eabBirth.m_childrenPerBirthMin);
                eabBirth.m_childrenMax = ChildrenMax.GetAbsValue(eabBirth.m_childrenMax);
                eabBirth.m_minDelayUntilNextBirth = MinDelayUntilNextBirth.GetAbsValue(eabBirth.m_minDelayUntilNextBirth);
                eabBirth.m_maxDelayUntilNextBirth = MaxDelayUntilNextBirth.GetAbsValue(eabBirth.m_maxDelayUntilNextBirth);
            }
        }
    }
}
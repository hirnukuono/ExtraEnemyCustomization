using Agents;
using Enemies;
using Player;

namespace EECustom
{
    public static class AgentExtension
    {
        public static bool TryCastToEnemyAgent(this Agent agent, out EnemyAgent enemyAgent)
        {
            return TryCast(agent, AgentType.Enemy, out enemyAgent);
        }

        public static bool TryCastToPlayerAgent(this Agent agent, out PlayerAgent playerAgent)
        {
            return TryCast(agent, AgentType.Player, out playerAgent);
        }

        private static bool TryCast<T>(Agent agent, AgentType type, out T result) where T : Agent
        {
            if (agent == null)
                goto ReturnNULL;

            if (agent.WasCollected)
                goto ReturnNULL;

            if (agent.Type != type)
                goto ReturnNULL;

            var tempResult = agent.TryCast<T>();
            if (tempResult == null)
                goto ReturnNULL;

            result = tempResult;
            return true;

        ReturnNULL:
            result = null;
            return false;
        }
    }
}
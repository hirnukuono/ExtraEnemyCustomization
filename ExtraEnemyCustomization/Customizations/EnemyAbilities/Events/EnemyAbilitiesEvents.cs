namespace EECustom.Customizations.EnemyAbilities.Events
{
    public delegate void GenericAbilitiesHandler(Enemies.EnemyAbilities enemyAbilities);
    public delegate void AbilitiesDamageHanalder(Enemies.EnemyAbilities enemyAbilities, float damage);

    public static class EnemyAbilitiesEvents
    {
        public static event AbilitiesDamageHanalder TakeDamage;
        public static event GenericAbilitiesHandler Dead;
        public static event GenericAbilitiesHandler Hitreact;

        internal static void OnTakeDamage(Enemies.EnemyAbilities enemyAbilities, float damage)
        {
            TakeDamage?.Invoke(enemyAbilities, damage);
        }

        internal static void OnDead(Enemies.EnemyAbilities enemyAbilities)
        {
            Dead?.Invoke(enemyAbilities);
        }

        internal static void OnHitreact(Enemies.EnemyAbilities enemyAbilities)
        {
            Hitreact?.Invoke(enemyAbilities);
        }
    }
}

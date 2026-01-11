using EElemental.Core;

namespace EElemental.Enemies
{
    /// <summary>
    /// Düşman state machine'i - Generic state machine kullanır
    /// </summary>
    public class EnemyStateMachine : StateMachine
    {
        public EnemyBase Enemy { get; private set; }
        public EnemyStats Stats { get; private set; }
        public EnemyAI AI { get; private set; }
        
        public EnemyStateMachine(EnemyBase enemy, EnemyStats stats, EnemyAI ai)
        {
            Enemy = enemy;
            Stats = stats;
            AI = ai;
        }
    }
}

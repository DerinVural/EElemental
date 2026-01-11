using UnityEngine;
using EElemental.Core;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Tüm düşman state'lerinin temel sınıfı
    /// </summary>
    public abstract class EnemyStateBase : IState
    {
        protected EnemyStateMachine stateMachine;
        protected EnemyBase enemy;
        protected EnemyStats stats;
        protected EnemyAI ai;
        
        protected float stateTimer;
        
        public EnemyStateBase(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.enemy = stateMachine.Enemy;
            this.stats = stateMachine.Stats;
            this.ai = stateMachine.AI;
        }
        
        public virtual void Enter()
        {
            stateTimer = 0f;
        }
        
        public virtual void Exit()
        {
        }
        
        public virtual void Update()
        {
            stateTimer += Time.deltaTime;
            CheckTransitions();
        }
        
        public virtual void FixedUpdate()
        {
        }
        
        /// <summary>
        /// State geçişlerini kontrol et - Alt sınıflar override edecek
        /// </summary>
        protected abstract void CheckTransitions();
        
        /// <summary>
        /// Animasyon trigger'ı çağır
        /// </summary>
        protected void SetAnimatorTrigger(string triggerName)
        {
            // enemy.Animator?.SetTrigger(triggerName);
        }
        
        /// <summary>
        /// Animasyon bool değeri set et
        /// </summary>
        protected void SetAnimatorBool(string paramName, bool value)
        {
            // enemy.Animator?.SetBool(paramName, value);
        }
    }
}

using femjami.Managers;
using femjami.runtime;

namespace Project.BehaviourTree.Runtime
{
    public class Attack : ActionNode
    {
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_blackboard._npcData == null) return State.Failure;

            GameEvents.current.LoseGame();
            
            return State.Success;
        }
    }
}

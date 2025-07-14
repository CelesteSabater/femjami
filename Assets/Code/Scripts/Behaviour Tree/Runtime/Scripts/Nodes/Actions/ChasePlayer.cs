using femjami.runtime;

namespace Project.BehaviourTree.Runtime
{
    public class ChasePlayer : ActionNode
    {
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_blackboard._npcData == null) return State.Failure;

            _blackboard._lastPlayerPosition = _blackboard._npcData.ChasePlayer();
            if (_blackboard._npcData.IsNPCInLocation(_blackboard._lastPlayerPosition))
                return State.Success;
            else
                return State.Running;
        }
    }
}

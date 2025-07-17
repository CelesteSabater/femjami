using femjami.runtime;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class InvestigateRandomPosition : ActionNode
    {
        private Vector3 _randomPoint;
        protected override void OnStart()
        {
            _randomPoint = _blackboard._npcData.GetInvestigatePosition();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_blackboard._npcData == null) return State.Failure;
            
            if (!_blackboard._npcData.GoToPosition(_randomPoint))
            {
                _blackboard._npcData.SetHighAlert(false);
                return State.Failure;
            }

            if (_blackboard._npcData.IsNPCInLocation(_randomPoint))
                    return State.Success;
                else
                    return State.Running;
        }
    }
}

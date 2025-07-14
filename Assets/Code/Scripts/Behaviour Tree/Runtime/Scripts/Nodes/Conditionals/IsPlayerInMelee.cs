using femjami.runtime;

namespace Project.BehaviourTree.Runtime
{
    public class IsPlayerInMelee : ConditionalNode
    {
        public bool test;
        protected override bool Question()
        {
            if (_blackboard._npcData == null) return false;

            return _blackboard._npcData.IsPlayerInMelee();
        }
    }
}
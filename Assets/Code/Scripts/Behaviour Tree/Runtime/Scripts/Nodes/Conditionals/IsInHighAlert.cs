namespace Project.BehaviourTree.Runtime
{
    public class IsInHighAlert : ConditionalNode
    {
        protected override bool Question()
        {
            if (_blackboard._npcData == null) return false;
            return _blackboard._npcData.GetHighAlert();
        } 
    }
}
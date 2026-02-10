namespace Vena.BehaviourTree
{
    public sealed class BehaviorTree<T> where T : IBlackboard
    {
        private readonly T _blackboard;

        private readonly Node _root;
        
        public T Blackboard => _blackboard;
        
        public BehaviorTree(Node root, T blackboard)
        {
            _root = root;
            _blackboard = blackboard;
        }
        
        public void Tick(float time, float deltaTime)
        {
            _root.Run(_blackboard, time, deltaTime);
        }
        
        public void Reset()
        {
            _root.Reset(_blackboard);
        }
    }
}
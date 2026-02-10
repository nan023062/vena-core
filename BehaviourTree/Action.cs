namespace Vena.BehaviourTree
{
    public abstract class Action : Node
    {
        protected sealed override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            //tick action executing
            return OnTick(blackboard, time, deltaTime) ? Status.Success : Status.Running;
        }

        protected abstract bool OnTick(IBlackboard blackboard, float time, float deltaTime);
    }
}
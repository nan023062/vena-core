// ***********************************************************************************
// * Author : LiNan
// * File : Condition.cs
// * Date : 2023-06-16-16:47
// ************************************************************************************

namespace Vena.BehaviourTree
{
    public abstract class Condition : Node
    {
        protected sealed override void OnEnter(IBlackboard blackboard, float time)
        {
            //do nothing
        }

        protected sealed override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            //check condition
            return IsSatisfy(blackboard) ? Status.Success : Status.Failure;
        }

        protected sealed override void OnExit(IBlackboard blackboard, float time)
        {
            //do nothing
        }

        protected abstract bool IsSatisfy(IBlackboard blackboard);
    }
}
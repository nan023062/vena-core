using System;

namespace Vena.BehaviourTree
{
    /// <summary>
    /// 修饰节点：只能对一个节点起作用
    /// </summary>
    public abstract class Decorate : Node
    {
        protected Node _node { private set; get; }
        
        protected override void OnEnter(IBlackboard blackboard, float time)
        {
            if (Nodes.Count == 0)
            {
               throw new Exception("");
            }
            
            _node = Nodes[0];
        }
        
        protected override void OnExit(IBlackboard blackboard, float time)
        {
            _node = null;
        }
    }

    /// <summary>
    /// 对子节点执行结果取反
    /// </summary>
    public sealed class DecorateRevert : Decorate
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            var status = _node.Run(blackboard, time, deltaTime);
            if (status == Status.Running) return Status.Running;
            return status == Status.Failure ? Status.Success : Status.Failure;
        }
    }
    
    /// <summary>
    /// 執行N次子节点
    /// </summary>
    public sealed class DecorateRepeater : Decorate
    {
        private int _repeatTimes = 5;

        private int _currentTimes = 0;

        protected override void OnEnter(IBlackboard blackboard, float time)
        {
            base.OnEnter(blackboard, time);
            _currentTimes = 0;
        }

        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            var status = _node.Run(blackboard, time, deltaTime);
            if (status != Status.Running)
            {
                _currentTimes++;
                if (_currentTimes >= _repeatTimes)
                {
                    return Status.Success;
                }
            }
            
            return Status.Running;
        }
    }
    
    
    /// <summary>
    /// 执行到此节点时返回失败
    /// </summary>
    public sealed class DecorateReturnFailure : Decorate
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            _node.Run(blackboard, time, deltaTime);
            return Status.Failure;
        }
    }
    
    /// <summary>
    /// 执行到此节点时返回成功
    /// </summary>
    public sealed class DecorateReturnSuccess : Decorate
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            _node.Run(blackboard, time, deltaTime);
            return Status.Success;
        }
    }
    
    /// <summary>
    /// 直到失败，一直执行子节点
    /// </summary>
    public sealed class DecorateUntilFailure : Decorate
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            if (_node.Run(blackboard, time, deltaTime) == Status.Failure)
            {
                return Status.Success;
            }
            
            return Status.Running;
        }
    }
    
    /// <summary>
    /// 直到成功，一直执行子节点
    /// </summary>
    public sealed class DecorateUntilSuccess : Decorate
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            if (_node.Run(blackboard, time, deltaTime) == Status.Success)
            {
                return Status.Success;
            }
            
            return Status.Running;
        }
    }
}
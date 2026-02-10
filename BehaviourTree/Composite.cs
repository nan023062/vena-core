using System;
using Random = UnityEngine.Random;

namespace Vena.BehaviourTree
{
    /// <summary>
    /// 组合节点：构建复合的AI结构
    /// </summary>
    public abstract class Composite : Node
    {
        protected override void OnEnter(IBlackboard blackboard, float time)
        {
        }
        
        protected override void OnExit(IBlackboard blackboard, float time)
        {
        }
    }
    
    #region Parallel

    /// <summary>
    /// 并行组合节点 - 直到所有节点完成
    /// </summary>
    public sealed class NodeParallel : Composite
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            bool hasRunning = false;
            
            int length = Nodes.Count;
            
            for (int i = 0; i < length; i++)
            {
                var node = Nodes[i];

                var status = node.Run(blackboard, time, deltaTime);
                
                if (status == Status.Failure) 
                    return Status.Failure;
                
                if (status == Status.Running) 
                    hasRunning = true;
            }
            
            return hasRunning ? Status.Running : Status.Success;
        }
    }
    
    /// <summary>
    /// 并行组合节点 - 全部失败才返回失败
    /// </summary>
    public sealed class NodeParallelAllFail : Composite
    {
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            bool hasRunning = false;
            bool allFailed = true;
            int length = Nodes.Count;
            
            for (int i = 0; i < length; i++)
            {
                var node = Nodes[i];

                var status = node.Run(blackboard, time, deltaTime);
                
                if (status != Status.Failure) 
                    allFailed = false;
                
                if (status == Status.Running) 
                    hasRunning = true;
            }
            
            if (allFailed) return Status.Failure;
            
            return hasRunning ? Status.Running : Status.Success;
        }
    }
    
    #endregion
    
    
    #region Sequence

    /// <summary>
    /// 顺序执行
    /// </summary>
    public sealed class NodeSequence : Composite
    {
        private int _index = 0;

        private Node _node = null;

        protected override void OnEnter(IBlackboard blackboard, float time)
        {
            base.OnEnter(blackboard, time);
            _index = -1;
            _node = _TryRunNextNode();
        }

        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            if (null != _node)
            {
                var status = _node.Run(blackboard, time, deltaTime);

                if (status == Status.Failure)
                {
                    return Status.Failure;
                }
                
                if (status == Status.Running)
                {
                    return Status.Running;
                }

                if (status == Status.Success)
                {
                    _node = _TryRunNextNode();
                    if (null != _node)
                    {
                        return Status.Running;
                    }
                }
            }
            
            return Status.Success;
        }

        private Node _TryRunNextNode()
        {
            _index++;
            Node node = null;
            
            int length = Nodes.Count;
            if (_index >= 0 && _index < length)
            {
                node = Nodes[_index];
            }
            
            return node;
        }
    }
    
    #endregion
    
    
    #region Selector

    /// <summary>
    /// 选择执行节点
    /// </summary>
    public sealed class NodeSelector : Composite
    {
        private Node _node;
        
        protected override void OnEnter(IBlackboard blackboard, float time)
        {
            base.OnEnter(blackboard, time);
            _node = null;
        }
        
        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            if (null == _node)
            {
                int length = Nodes.Count;
                for (int i = 0; i < length; i++)
                {
                    var node = Nodes[i];
                    var status = node.Run(blackboard, time, deltaTime);
                    if (status == Status.Success)
                    {
                        return Status.Success;
                    }

                    if (status == Status.Running)
                    {
                        _node = node;
                        return Status.Running;
                    }
                }
                
                return Status.Success;
            }
            
            return _node.Run(blackboard, time, deltaTime);
        }
    }

    #endregion
    
    
    #region Random
    
    /// <summary>
    /// 随机执行节点
    /// </summary>
    public sealed class NodeRandom : Composite
    {
        private Node _node;

        private int _index;

        private int _runTimes = Int32.MaxValue;

        protected override void OnEnter(IBlackboard blackboard, float time)
        {
            base.OnEnter(blackboard, time);

            _node = null;
            int length = Nodes.Count;
            if (length > 0)
            {
                if (_runTimes >= length)
                {
                    _runTimes = 0;
                    _index = Random.Range(0, length);
                }
                _node = Nodes[(_index + _runTimes) % length];
                _runTimes++;
            }
        }

        protected override Status OnRun(IBlackboard blackboard, float time, float deltaTime)
        {
            if (_node == null)
            {
                return Status.Success;
            }
            
            return _node.Run(blackboard, time, deltaTime);
        }
    }
    
    #endregion
}
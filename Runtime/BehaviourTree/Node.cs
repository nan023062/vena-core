using System.Collections.Generic;

namespace Vena.BehaviourTree
{
    public enum Status
    {
        Running, Success, Failure,
    }
    
    /// <summary>
    /// AI树节点基类
    /// </summary>
    public abstract class Node
    {
        private Status _status = Status.Success;
        
        private Node _parent = null;
        
        public Node Parent => _parent;
        
        private readonly List<Node> _nodes = new List<Node>();
        
        protected List<Node> Nodes => _nodes;
        
        public Status Run(IBlackboard blackboard, float time, float deltaTime)
        {
            if (_status == Status.Success)
            {
                OnEnter(blackboard, time);
                _status = Status.Running;
            }
            
            var status = OnRun(blackboard, time, deltaTime);
            
            if (status != Status.Running)
            {
                OnExit(blackboard, time);
                
                _status = Status.Success;
                return status;
            }
            
            return Status.Running;
        }
        
        protected abstract void OnEnter(IBlackboard blackboard, float time);

        protected abstract Status OnRun(IBlackboard blackboard, float time, float deltaTime);
        
        protected abstract void OnExit(IBlackboard blackboard, float time);
        
        public void Reset(IBlackboard blackboard )
        {
            if (_status == Status.Running)
            {
                foreach (var node in Nodes)
                {
                    node.Reset(blackboard);
                }
                
                OnExit(blackboard, 0f);
            }
            
            _status = Status.Success;
        }

        public void Append(Node node) 
        {
            node._parent = this;
            _nodes.Add(node);
        }


        public void Remove(Node node)
        {
            node._parent = null;
            _nodes.Remove(node);
        }

        
        public virtual string Encode()
        {
            return string.Empty;
        }

        
        public virtual void Decode(string str)
        {
            
        }
    }
}
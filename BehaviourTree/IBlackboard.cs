namespace Vena.BehaviourTree
{
    /// <summary>
    /// AI树的数据
    /// </summary>
    public interface IBlackboard
    {
        public Value GetValue(int varId);
        
        public void SetValue(int varId, in Value value);
        
        public int ConvertVarNameToId(string varName);
    }
}
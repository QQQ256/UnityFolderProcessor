namespace FolderProcessor
{
    public interface INodeInfo
    {
        bool IsEmpty { get; }
        void Clear();
    }
}
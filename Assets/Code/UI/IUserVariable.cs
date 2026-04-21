public interface IUserVariable
{
    string[] names { get; }
    bool TrySetVariable(object value, string name);
}
public interface IUserVariable
{
    string[] names { get; }
    public bool TrySetVariable(float value, string name);
}
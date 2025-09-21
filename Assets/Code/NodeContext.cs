using System.Collections.Generic;

public class NodeContext
{
    private Dictionary<Port, object> _values = new();

    public T GetValue<T>(Port port)
    {
        if (_values.TryGetValue(port, out var val))
        {
            return (T)val;
        }
        return default;
    }

    public void SetValue(Port port, object value)
    {
        _values[port] = value;
    }
}
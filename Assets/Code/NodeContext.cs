using System.Collections.Generic;

public class NodeContext
{
    private Dictionary<Port, object> _values = new();
    public bool TryGetValue<T>(out T val)
    {
        foreach (var kvp in _values)
        {
            if (TryGetValue<T>(kvp.Key, out var value))
            {
                val = value;
                return true;
            }
        }
        val = default(T);
        return false;
    }
    public bool TryGetValue<T>(Port port, out T value)
    {
        if (_values.TryGetValue(port, out var val))
        {
            value = (T)val;
            return true;
        }
        value = default;
        return false;
    }
    public T GetValue<T>(Port port)
    {
        if (_values.TryGetValue(port, out var val))
        {
            return (T)val;
        }
        return default;
    }
    public bool CheckRegister(Port port)
    {
        return _values.ContainsKey(port);
    }
    public void SetValue(Port port, object value)
    {
        _values[port] = value;
    }
}
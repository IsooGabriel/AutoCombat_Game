using System.Collections.Generic;
using TMPro;
public enum IfSettings
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
}
public class IfNodeUI : NodeUI, IUserVariable
{
    public const string settingKey = "if setting";
    public const string valueAPortKey = "valueA";
    public const string valueBPortKey = "valueB";
    public string[] names => new string[] { settingKey, valueAPortKey, valueBPortKey };
    public TMP_Dropdown setting;
    private Dictionary<IfSettings, string> settings = new Dictionary<IfSettings, string>()
    {
        {  IfSettings.Equals, "=" },
        { IfSettings.NotEquals, "Not =" },
        { IfSettings.GreaterThan, ">" },
        { IfSettings.LessThan, "<" },
    };
    private bool isStarted = false;

    void Start()
    {
        if (isStarted)
        {
            return;
        }
        setting.ClearOptions();
        List<string> options = new List<string>(settings.Values);
        setting.AddOptions(options);
        setting.value = 0;
        isStarted = true;
    }

    public bool TrySetVariable(object value, string name)
    {
        Start();
        if (name != settingKey)
        {
            return false;
        }

        int iValue = 0;
        if (value is int i)
        {
            iValue = i;
        }
        else if (value is float f)
        {
            iValue = (int)f;
        }

        SetSetting((IfSettings)iValue);
        setting.value = iValue;
        setting.SetValueWithoutNotify(iValue);
        setting.RefreshShownValue();
        return true;
    }
    public void SetSetting(IfSettings value)
    {
        SetInputValue((float)value, settingKey);
    }
    public void SetSetting()
    {
        SetSetting((IfSettings)setting.value);
    }
    public void SetValueA(string value)
    {
        SetInputValue(float.Parse(value), valueAPortKey);
    }
    public void SetValueB(string value)
    {
        SetInputValue(float.Parse(value), valueBPortKey);
    }

    private void SetInputValue(object value, string key)
    {
        foreach (var inputvalue in node.inputValues)
        {
            if (inputvalue.toPortName != key)
            {
                continue;
            }
            inputvalue.value = value;
            return;
        }
        node.inputValues.Add(
            new InputValue<object>(
                key,
                value,
                isUserset: true
            )
        );
    }
}
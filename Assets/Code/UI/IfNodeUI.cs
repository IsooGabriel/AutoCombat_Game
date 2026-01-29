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

    public bool TrySetVariable(float value, string name)
    {
        Start();
        if (name != settingKey)
        {
            return false;
        }

        SetSetting((IfSettings)(int)value);
        setting.value = (int)2;
        setting.SetValueWithoutNotify((int)2);
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

    private void SetInputValue(float value, string key)
    {
        foreach (var inputvalue in node.inputValues)
        {
            if (inputvalue.toPortName != key)
            {
                continue;
            }
            inputvalue.value = (object)(float)value;
            return;
        }
        node.inputValues.Add(
            new InputValue<object>(
                key,
                (float)value,
                isUserset: true
            )
        );
    }
}
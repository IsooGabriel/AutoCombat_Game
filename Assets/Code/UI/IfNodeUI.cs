using System.Collections.Generic;
using TMPro;

public enum IfSettings
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
}
public class IfNodeUI : NodeUI
{
    public const string settingKey = "if setting";
    public const string valueAPortKey = "valueA";
    public const string valueBPortKey = "valueB";
    public TMP_Dropdown setting;
    private Dictionary<IfSettings, string> settings = new Dictionary<IfSettings, string>()
    {
        {  IfSettings.Equals, "=" },
        { IfSettings.NotEquals, "Not =" },
        { IfSettings.GreaterThan, ">" },
        { IfSettings.LessThan, "<" },
};

    void Start()
    {
        setting.ClearOptions();
        List<string> options = new List<string>(settings.Values);
        setting.AddOptions(options);
        setting.value = 0;
    }

    public void SetSetting()
    {
        SetInputValue(setting.value, settingKey);
    }
    public void SetValueA(string value)
    {
        SetInputValue(float.Parse(value), valueAPortKey);
    }
    public void SetValueB(string value)
    {
        SetInputValue(float.Parse(value), valueAPortKey);
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
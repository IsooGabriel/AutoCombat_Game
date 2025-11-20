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
    private string settingKey = "if setting";
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
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>(settingKey, (object)(IfSettings)setting.value, isUserset:true)
        };
    }
}
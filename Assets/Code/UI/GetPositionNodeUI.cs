using System.Collections.Generic;
using TMPro;
class GetPositionNodeUI : NodeUI, IUserVariable
{
    public TMP_Dropdown setting;
    public string[] names => new string[] { GetPositionNode.positionTypeDataName };
    private Dictionary<GetPositionSettings, string> settings = new Dictionary<GetPositionSettings, string>()
    {
        {  GetPositionSettings.CharacterPosition, "my position" },
        { GetPositionSettings.EnemyPosition, "enemy position" }
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
        SetSetting();
        isStarted = true;
    }

    public bool TrySetVariable(float value, string name)
    {
        Start();
        if (name != GetPositionNode.positionTypeDataName)
        {
            return false;
        }
        SetSetting((GetPositionSettings)(int)value);
        setting.value = (int)value;
        setting.RefreshShownValue();
        return true;
    }

    public void SetSetting(GetPositionSettings value)
    {
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>
            (
                GetPositionNode.positionTypeDataName,
                (object)(float)value,
                isUserset:true
            )
        };
    }
    public void SetSetting()
    {
        SetSetting((GetPositionSettings)setting.value);
    }
}

public enum GetPositionSettings
{
    CharacterPosition,
    EnemyPosition,
}

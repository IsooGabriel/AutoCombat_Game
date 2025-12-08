using System.Collections.Generic;
using TMPro;
class GetPositionNodeUI : NodeUI
{
    public TMP_Dropdown setting;
    private Dictionary<GetPositionSettings, string> settings = new Dictionary<GetPositionSettings, string>()
    {
        {  GetPositionSettings.CharacterPosition, "my position" },
        { GetPositionSettings.EnemyPosition, "enemy position" }
    };

    void Start()
    {
        setting.ClearOptions();
        List<string> options = new List<string>(settings.Values);
        setting.AddOptions(options);
        setting.value = 0;
        SetSetting();
    }

    public void SetSetting()
    {
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>
            (
                GetPositionNode.positionTypeDataName,
                (object)(float)setting.value,
                isUserset:true
            )
        };
    }
}

public enum GetPositionSettings
{
    CharacterPosition,
    EnemyPosition,
}

using System.Collections.Generic;
using TMPro;

public class GetStatusNodeUI : NodeUI, IUserVariable
{
    public TMP_Dropdown setting;
    public string[] names => new string[] { GetStatusNode.statusPortName };
    private Dictionary<StatusType, string> settings = new Dictionary<StatusType, string>()
    {
        { StatusType.MAX_HP, "max HP" },
        { StatusType.CURRENT_HP, "current HP" },
        { StatusType.ATTACK, "attack" },
        { StatusType.ATTACK_CT,"attack CT" },
        { StatusType.CRITICAL_CHANCE, "critical chance" },
        { StatusType.CRITICAL_DAMAGE, "critical damage" }
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
        if (name != GetStatusNode.statusPortName)
        {
            return false;
        }
        SetSetting((StatusType)(int)value);
        setting.value = (int)value;
        setting.RefreshShownValue();
        return true;
    }

    public void SetSetting(StatusType value)
    {
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>
            (
                GetStatusNode.statusPortName,
                (object)(float)value,
                isUserset:true
            )
        };
    }
    public void SetSetting()
    {
        SetSetting((StatusType)setting.value);
    }
}
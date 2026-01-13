using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class SetBoolNodeUI : NodeUI, IUserVariable
{

    public Toggle toggle = null;
    public string[] names => new string[] { SetBoolNode.inputBool };


    public bool TrySetVariable(float value, string name)
    {
        if (name != names[0])
        {
            return false;
        }
        SetData(value > 0 ? true : false);
        return true;
    }

    public void SetData()
    {
        if (toggle == null)
        {
            return;
        }
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>(names[0], (object)toggle.isOn, isUserset:true)
        };
    }
    public void SetData(bool value)
    {
        toggle.isOn = value;
        SetData();
    }
    public override void Awake()
    {
        base.Awake();
        node = new SetValueNode();
        toggle.onValueChanged.AddListener(delegate { SetData(); });
    }
}


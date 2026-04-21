using UnityEngine.UI;
using System.Collections.Generic;
public class SetBoolNodeUI : NodeUI, IUserVariable
{

    public Toggle toggle = null;
    public string[] names => new string[] { SetBoolNode.inputBool };


    public bool TrySetVariable(object value, string name)
    {
        if (name != names[0])
        {
            return false;
        }
        bool bValue = false;
        if (value is bool b)
        {
            bValue = b;
        }
        else if (value is float f)
        {
            bValue = f > 0;
        }
        else if (value is int i)
        {
            bValue = i > 0;
        }

        SetData(bValue);
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
            new InputValue<object>(names[0], toggle.isOn ? 1f : 0f, isUserset:true)
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
        node = new SetBoolNode();
        toggle?.onValueChanged.AddListener(delegate { SetData(toggle.isOn); });
    }
}


using System.Collections.Generic;
using TMPro;
public class DoNTimeNodeUI : NodeUI, IUserVariable
{
    public TMP_InputField field = null;
    public string[] names => new string[] { DoNTimeNode.limitPortName };


    public bool TrySetVariable(object value, string name)
    {
        if (name != DoNTimeNode.limitPortName)
        {
            return false;
        }
        float fValue = value is float ? (float)value : 0f;
        if (value is int iValue)
        {
            fValue = (float)iValue;
        }
        
        SetData(fValue);
        field.text = fValue.ToString();
        return true;
    }

    public void SetData()
    {
        if (field == null)
        {
            return;
        }
        float val = float.Parse(string.IsNullOrEmpty(field.text) ? "0" : field.text);
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>(DoNTimeNode.limitPortName, val, isUserset:true)
        };
    }
    public void SetData(float value)
    {
        field.text = value.ToString();
        SetData();
    }
    public override void Awake()
    {
        base.Awake();
        node = new DoNTimeNode();
        field.onEndEdit.AddListener(delegate { SetData(); });
    }
}

using System.Collections.Generic;
using TMPro;
public class SetValueNodeUI : NodeUI, IUserVariable
{
    public TMP_InputField field = null;
    public const string settingKey = "output value";
    public string[] names => new string[] { settingKey };


    public bool TrySetVariable(object value, string name)
    {
        if (name != settingKey)
        {
            return false;
        }
        float fValue = 0.0f;
        if(value is float f)
        {
            fValue = f;
        }
        SetData(fValue);
        field.text = value.ToString();
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
            new InputValue<object>(settingKey, val, isUserset:true)
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
        node = new SetValueNode();
        field.onEndEdit.AddListener(delegate { SetData(); });
    }
}

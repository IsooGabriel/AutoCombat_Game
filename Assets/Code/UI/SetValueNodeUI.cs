using System.Collections.Generic;
using TMPro;
public class SetValueNodeUI : NodeUI, IUserVariable
{
    public TMP_InputField field = null;
    public const string settingKey = "output value";
    public string[] names => new string[] { settingKey };


    public bool TrySetVariable(float value, string name)
    {
        if (name != settingKey)
        {
            return false;
        }
        SetData(value);
        field.text = value.ToString();
        return true;
    }

    public void SetData()
    {
        if (field == null)
        {
            return;
        }
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>(settingKey, (object)float.Parse(string.IsNullOrEmpty(field.text) ? "0" : field.text), isUserset:true)
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

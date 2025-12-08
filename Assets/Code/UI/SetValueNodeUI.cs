using System.Collections.Generic;
using TMPro;
public class SetValueNodeUI : NodeUI
{
    public TMP_InputField field = null;
    public const string settingKey = "output value";

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

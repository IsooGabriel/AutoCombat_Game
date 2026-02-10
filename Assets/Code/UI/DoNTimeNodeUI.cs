using System.Collections.Generic;
using TMPro;
public class DoNTimeNodeUI : NodeUI, IUserVariable
{
    public TMP_InputField field = null;
    public string[] names => new string[] { DoNTimeNode.limitPortName };


    public bool TrySetVariable(float value, string name)
    {
        if (name != DoNTimeNode.limitPortName)
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
            new InputValue<object>(DoNTimeNode.limitPortName, (object)float.Parse(string.IsNullOrEmpty(field.text) ? "0" : field.text), isUserset:true)
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

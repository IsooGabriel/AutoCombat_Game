using System.Collections.Generic;
using TMPro;
public class SetDataNodeUI : NodeUI
{
    public TMP_InputField field = null;

    public void SetData()
    {
        if (field == null)
        {
            return;
        }
        node.inputValues = new List<InputValue<object>>()
        {
            new InputValue<object>("data", (object)float.Parse(string.IsNullOrEmpty(field.text) ? "0" : field.text))
        };
    }
    public override void Awake()
    {
        base.Awake();
        node = new SetValueNode();
        field.onEndEdit.AddListener(delegate { SetData(); });
    }
}

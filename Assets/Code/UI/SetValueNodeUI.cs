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
        node.inputValues = new System.Collections.Generic.List<InputValue<object>>() { new InputValue<object>("data", (object)float.Parse(field.text)) };
    }
    public override void Awake()
    {
        base.Awake();
        node = new SetValueNode();
        field.onEndEdit.AddListener(delegate { SetData(); });
    }
}

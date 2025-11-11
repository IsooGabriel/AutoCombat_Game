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
        node.inputData = new System.Collections.Generic.List<InputValue>() { new InputValue("data", float.Parse(field.text)) };
    }
    public override void Awake()
    {
        base.Awake();
        node = new SetValueNode();
        field.onEndEdit.AddListener(delegate { SetData(); });
    }
}

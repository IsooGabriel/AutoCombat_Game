using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AddNodeButtonFuctory.OutlineColor;

public class AddNodeButtonFuctory : MonoBehaviour
{
    [SerializeField]
    private NodeType nodeType;
    [SerializeField]
    private Button button;
    [SerializeField]
    private bool changeText = false;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Image iconObject;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private Image outline;
    [SerializeField]
    private ImageAnimation_Gabu imageAnimation;
    [SerializeField]
    private OutlineColor outlineColor;
    [SerializeField]
    private GraphEditorManager manager;

    public enum OutlineColor
    {
        None = 0,
        ACTION, GETSET, MATH
    }

    private Dictionary<OutlineColor, Color> outlineColores = new Dictionary<OutlineColor, Color>()
    {
        {   None,   new Color(225, 225, 225)},
        {   ACTION, new Color(229,  23,  57)},
        {   GETSET, new Color(92,  229,  183)},
        {   MATH,   new Color(23 , 92 , 229)},
    };

    public void Awake()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => manager.AddNode(nodeType));

        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (changeText && text != null)
        {
            text.text = $"{nodeType.ToString()}";
        }

        if (iconObject != null && icon != null)
        {
            iconObject.sprite = icon;
        }


        Color color = outlineColores[outlineColor];
        color = new Color(color.r / 360, color.g / 360, color.b / 360);
        if (outline != null)
        {
            outline.color = color;
        }
        if (imageAnimation != null)
        {
            imageAnimation.ChangeColors(UISystem_Gabu.AnimatorStatu.Highlighted, color);
            imageAnimation.ChangeColors(UISystem_Gabu.AnimatorStatu.Selected, color);
            imageAnimation.ChangeColors(UISystem_Gabu.AnimatorStatu.Pressed, color);
        }
    }
}
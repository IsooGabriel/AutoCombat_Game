using UnityEngine;
using UnityEngine.UI;

public class ImageAnimationManager : MonoBehaviour
{
    [SerializeField, Header("UIシステム")]
    public ImageAnimation_Gabu[] uiSystems = null;
    [SerializeField, Header("画像")]
    public Image[] images = null;
    [SerializeField, Header("pixsels per unit multiplie.　133pixelの円使ってるなら2くらいでいいと思う")]
    public float pixelsPerUnitMultiplier = 2;

    private void Start()
    {
        if (uiSystems == null)
        {
            Debug.LogWarning(gameObject.name+"uiSystemsが空です:" + uiSystems.Length);
            enabled = false;
        }
        foreach (var uiSystem in uiSystems)
        {
        }
        foreach (var image in images)
        {
            image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
        }
    }
}

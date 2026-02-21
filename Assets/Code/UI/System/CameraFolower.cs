using UnityEngine;
using TMPro;
public class CameraFolower : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private TMP_InputField xInput;
    [SerializeField]
    private TMP_InputField yInput;

    void Update()
    {
        xInput.text = _camera.transform.position.x.ToString("0");
        yInput.text = _camera.transform.position.y.ToString("0");
    }
}

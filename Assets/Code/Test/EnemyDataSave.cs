using UnityEngine;

public class EnemyDataSave : MonoBehaviour
{
    [SerializeField]
    GraphEditorManager graphEditorManager;

    public void OnClick()
    {
        graphEditorManager.SaveGraph("EnemyData.json");
    }
}

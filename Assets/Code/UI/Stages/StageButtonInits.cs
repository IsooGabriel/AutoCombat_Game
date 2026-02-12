using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonInits : MonoBehaviour
{
    public StageDatabase stageDatabase;
    public SceneLoader sceneLoader;
    public GameObject stageButtonPrefab;
    public Button button;
    public TextMeshProUGUI buttonText;

    void Start()
    {
        for (int i = 0; i < stageDatabase.stages.Count; i++)
        {
            button.onClick.RemoveAllListeners();
            int j = i;
            button.onClick.AddListener(() => sceneLoader.SelectStage(stageDatabase.stages[j].sceneName));
            buttonText.text = stageDatabase.stages[i].sceneName;
            GameObject buttonObj = Instantiate(stageButtonPrefab, stageButtonPrefab.transform.parent);
        }
        stageButtonPrefab.SetActive(false);
    }
}

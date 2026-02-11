using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageButtonInits : MonoBehaviour
{
    public StageDatabase stageDatabase;
    public SceneLoader sceneLoader;
    public GameObject stageButtonPrefab;
    public Button button;
    public TextMeshProUGUI buttonText;

    void Start()
    {
        for(int i = 0; i < stageDatabase.stages.Count; i++)
        {
            buttonText.text = stageDatabase.stages[i].sceneName;
            button.onClick.AddListener(() => sceneLoader.SelectStage(stageDatabase.stages[i].sceneName));
            GameObject buttonObj = Instantiate(stageButtonPrefab, stageButtonPrefab.transform.parent);
        }
        stageButtonPrefab.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    public void OnClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OnQuit()
    {
        Application.Quit();
    }
    public void ChangeTimescale(int scale)
    {
        Time.timeScale = scale;
    }
    private void OnGraphEditor(CallbackContext context)
    {
        SceneManager.LoadScene("GraphEditor");
    }
    private void OnGame(CallbackContext context)
    {
        LoadGameScene();
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene(StageSelector.sceneName);
    }

    public void SetStage(string seneName)
    {
        StageSelector.sceneName = seneName;
    }

    public void SelectStage(string sceneName)
    {
        Debug.Log("====button=====");
        SetStage(sceneName);
        OnClick(sceneName);
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        playerInput.actions["GraphEditor"].performed += OnGraphEditor;
        playerInput.actions["Game"].performed += OnGame;
    }
    private void OnDisable()
    {
        if(playerInput == null)
        {
            return;
        }
        playerInput.actions["GraphEditor"].performed -= OnGraphEditor;
        playerInput.actions["Game"].performed -= OnGame;
    }

}

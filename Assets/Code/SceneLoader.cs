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
        SceneManager.LoadScene(StageSerector.sceneName);
    }

    public void SelectStage(string sceneName)
    {
        StageSerector.sceneName = sceneName;
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
        playerInput.actions["GraphEditor"].performed -= OnGraphEditor;
        playerInput.actions["Game"].performed -= OnGame;
    }

}

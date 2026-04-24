using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    [SerializeField] private LevelsListSO levelsListSO;
    [SerializeField] private Transform levelItemContainer;
    [SerializeField] private GameObject levelItemPrefab;
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;
    private GameLevel selectedLevel;
    private int containerColumnCount = 4;
    private InputActions inputActions;
    private List<LevelItemUI> levelItemUIList;

    private void Awake()
    {
        inputActions = new InputActions();
        levelItemUIList = new List<LevelItemUI>();
    }

    private void Start()
    {
        foreach (GameLevel gameLevel in levelsListSO.GameLevelsList)
        {
            GameObject levelItemObj = Instantiate(levelItemPrefab, levelItemContainer);
            LevelItemUI levelItemUI = levelItemObj.GetComponent<LevelItemUI>();
            levelItemUI.SetLevelData(gameLevel);
            levelItemUIList.Add(levelItemUI);
        }

        levelItemUIList[0].Select();

        startButton.onClick.AddListener(() =>
        {
            if (selectedLevel != null)
            {
                GameManager.GameLevelIndex = selectedLevel.LevelIndex;
                SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
            }
        });

        backButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void OnEnable()
    {
        LevelItemUI.OnLevelSelected += LevelItemUI_OnLevelSelected;

        inputActions.UINavigation.Enable();
        inputActions.UINavigation.MoveRight.performed += ctx => MoveSelection(1);
        inputActions.UINavigation.MoveLeft.performed += ctx => MoveSelection(-1);
        inputActions.UINavigation.MoveDown.performed += ctx => MoveSelection(containerColumnCount);
        inputActions.UINavigation.MoveUp.performed += ctx => MoveSelection(-containerColumnCount);
    }

    private void OnDisable()
    {
        LevelItemUI.OnLevelSelected -= LevelItemUI_OnLevelSelected;
        inputActions.UINavigation.MoveRight.performed -= ctx => MoveSelection(1);
        inputActions.UINavigation.MoveLeft.performed -= ctx => MoveSelection(-1);
        inputActions.UINavigation.MoveDown.performed -= ctx => MoveSelection(containerColumnCount);
        inputActions.UINavigation.MoveUp.performed -= ctx => MoveSelection(-containerColumnCount);
        inputActions.UINavigation.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void MoveSelection(int direction)
    {
        if (selectedLevel == null) return;

        int currentIndex = levelItemUIList.FindIndex(item => item.IsSelected());

        int newIndex = currentIndex + direction;
        if (newIndex < 0 || newIndex >= levelItemUIList.Count) return;
        levelItemUIList[newIndex].Select();
    }

    private void LevelItemUI_OnLevelSelected(object sender, LevelItemUI.LevelSelectedEventArgs e)
    {
        LevelItemUI selectedLevelItem = sender as LevelItemUI;
        selectedLevel = e.selectedLevel;
        
        // Deselect all other level items
        foreach (LevelItemUI levelItemUI in levelItemUIList)
        {
            if (levelItemUI != selectedLevelItem && levelItemUI.IsSelected())
            {
                levelItemUI.Deselect();
            }
        }
    }
}

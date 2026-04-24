using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelItemUI : MonoBehaviour, IPointerClickHandler
{
    public static event EventHandler<LevelSelectedEventArgs> OnLevelSelected;
    public class LevelSelectedEventArgs : EventArgs
    {
        public GameLevel selectedLevel;
    }

    [SerializeField] private Image thumbnailImage;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private GameObject selectionIndicator;
    private GameLevel gameLevel;
    private bool isSelected;

    private void Awake()
    {
        isSelected = false;
        selectionIndicator.SetActive(isSelected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void SetLevelData(GameLevel gameLevel)
    {
        this.gameLevel = gameLevel;
        thumbnailImage.sprite = gameLevel.Thumbnail;
        levelNameText.text = $"Level {gameLevel.LevelIndex}";
    }

    public void Select()
    {
        isSelected = true;
        selectionIndicator.SetActive(isSelected);
        OnLevelSelected?.Invoke(this, new LevelSelectedEventArgs
        {
            selectedLevel = gameLevel
        });
    }

    public void Deselect()
    {
        isSelected = false;
        selectionIndicator.SetActive(isSelected);
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}

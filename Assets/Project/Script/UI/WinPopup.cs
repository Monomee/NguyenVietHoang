using UnityEngine;

public class WinPopup : MonoBehaviour
{
    [SerializeField] GameObject undoButton;
    void OnEnable()
    {
        GameEvents.OnWin += Show;
    }
    void OnDisable()
    {
        GameEvents.OnWin -= Show;
    }
    void Show()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        undoButton.SetActive(false);
    }
}

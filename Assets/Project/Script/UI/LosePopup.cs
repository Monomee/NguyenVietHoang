using System.Security.Cryptography;
using UnityEngine;

public class LosePopup : MonoBehaviour
{
    [SerializeField] GameObject undoButton;
    void OnEnable()
    {
        GameEvents.OnLose += Show;
    }
    void OnDisable()
    {
        GameEvents.OnLose -= Show;
    }
    void Show()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        undoButton.SetActive(false);
    }
    public void ContinueAfterLose()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        undoButton.SetActive(true);
        GameManager.Instance.Undo();
    }
}

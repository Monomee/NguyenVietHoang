using System.Collections.Generic;
using UnityEngine;

public class TubeViewFactory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private TubeView tubeViewPrefab;

    [Header("Settings")]
    [SerializeField] private float spacing = 1.5f;

    List<TubeView> tubeViews;

    void Start()
    {      
        tubeViews = CreateTubeView(GameManager.Instance.levelData);
    }

    public List<TubeView> CreateTubeView(LevelData levelData)
    {
        List<TubeView> tubeViews = new();

        int count = levelData.Tubes.Count;

        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            var tubeView = Instantiate(tubeViewPrefab);

            tubeView.transform.position = new Vector3(
                startX + i * spacing,
                0f,
                0f
            );

            tubeView.Bind(levelData.Tubes[i]);
            tubeViews.Add(tubeView);

        }
        return tubeViews;
    }
}

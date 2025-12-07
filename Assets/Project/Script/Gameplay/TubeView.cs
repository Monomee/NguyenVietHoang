using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TubeView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform slotsRoot;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject outline, cork;

    [Header("Settings")]
    [SerializeField] float liquidHeight = 0.8f;
    public TubeData Data;
    List<SpriteRenderer> slots = new();

    [Header("Animation")]
    [SerializeField] float tiltAngle = 30f;
    [SerializeField] float tiltDuration = 0.25f;
    [SerializeField] float pourDuration = 0.3f;
    Sequence currentSeq;
    private void OnEnable()
    {
        GameEvents.OnPour += OnPour;
        GameEvents.OnTubeCompleted += OnTubeCompleted;
        GameEvents.OnUndo += OnUndo;
    }

    private void OnDisable()
    {
        GameEvents.OnPour -= OnPour;
        GameEvents.OnTubeCompleted -= OnTubeCompleted;
        GameEvents.OnUndo -= OnUndo;
    }

    //===========Envent Handlers===============
    void OnPour(TubeData from, TubeData to, int amount)
    {
        if (Data == from || Data == to)
        {
            Refresh();
        }
    }
    void OnTubeCompleted(TubeData tube)
    {
        if (tube != Data) return;

        SetCorked(true);
        // có thể play FX, sound nhỏ
    }
    void OnUndo(TubeData from, TubeData to)
    {
        if (Data == from || Data == to)
        {
            Refresh();
            SetCorked(false); 
        }
    }
    private void OnMouseDown()
    {
        GameManager.Instance.OnTubeClicked(this);
        //Debug.Log($"Tube clicked: {Data}"); 
    }

    //===========Helpers===============
    public void SetHighlight(bool value)
    {
        outline.SetActive(value);
        if (!value)
        {
            this.transform.localScale = Vector3.one * 1f;
            return;
        }
        else
        {
            this.transform.localScale = Vector3.one * 1.1f;
        }       
    }
    public void SetCorked(bool value)
    {        
        Vector3 originalPos = cork.transform.position;      
        cork.transform.position += Vector3.up * 1f;
        cork.SetActive(value);
        cork.transform.DOMove(originalPos, 0.5f).SetEase(Ease.InBounce);
    }


    //=============BUILD================
    // CALL ONCE 
    public void Bind(TubeData tubeData)
    {
        Data = tubeData;
        BuildSlots(Data.Depth);
        Refresh();
    }
    // Build slot
    void BuildSlots(int depth)
    {
        // clear last slots
        foreach (var s in slots)
            Destroy(s.gameObject);

        slots.Clear();

        float innerPadding = 0.05f;
        float usableHeight = liquidHeight - innerPadding * 2;
        float step = usableHeight / depth;
        float startY = -liquidHeight / 2 + innerPadding + step / 2;

        for (int i = 0; i < depth; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsRoot);

            var sr = slotObj.GetComponent<SpriteRenderer>();

            slotObj.transform.localPosition = new Vector3(
                0f,
                startY + step * i,
                0f
            );

            float spriteHeight = sr.bounds.size.y;
            float scaleY = step / spriteHeight;

            slotObj.transform.localScale = new Vector3(1f, scaleY, 1f);

            slots.Add(sr);
        }
    }

    public void Refresh()
    {
        // clear display
        foreach (var sr in slots)
        {
            sr.sprite = null;
            sr.enabled = false;
        }

        if (Data.Colors.Count == 0) return;

        // Stack: top -> bottom -> reverse for display
        var colors = Data.Colors.Reverse().ToList();
        int count = Mathf.Min(colors.Count, slots.Count);
        for (int i = 0; i < count; i++)
        {
            slots[i].sprite = ColorSpriteDB.Get(colors[i]);
            slots[i].color = ColorConfig.GetColor(colors[i]);
            slots[i].enabled = true;
        }
    }

    // ====== PUBLIC API ======
    public void PlayPourTo(TubeView target, System.Action onComplete = null)
    {
        if (currentSeq != null && currentSeq.IsActive())
            currentSeq.Kill();

        var fromSlot = GetTopFilledSlot();
        var targetSlotIndex = target.GetNextEmptySlotIndex();

        if (fromSlot == null || targetSlotIndex < 0)
            return;

        SpriteRenderer targetSlot = target.slots[targetSlotIndex];

        currentSeq = DOTween.Sequence();

        currentSeq.Append(Tilt(-tiltAngle));
        currentSeq.Append(MoveWater(fromSlot, targetSlot.transform.position));
        currentSeq.Append(Tilt(0));

        currentSeq.OnComplete(() =>
        {
            AttachToTarget(fromSlot, target.slotsRoot, targetSlot.transform.localPosition);
            onComplete?.Invoke();
        });
    }

    // ====== PRIVATE HELPERS ======

    Tween Tilt(float angle)
    {
        return transform.DORotate(
            new Vector3(0, 0, angle),
            tiltDuration
        ).SetEase(Ease.OutQuad);
    }

    Tween MoveWater(SpriteRenderer water, Vector3 targetWorldPos)
    {
        water.transform.SetParent(null);
        return water.transform
            .DOMove(targetWorldPos, pourDuration)
            .SetEase(Ease.InOutSine);
    }

    void AttachToTarget(SpriteRenderer water, Transform targetRoot, Vector3 localPos)
    {
        water.transform.SetParent(targetRoot);
        water.transform.localPosition = localPos;
    }

    SpriteRenderer GetTopFilledSlot()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].enabled)
                return slots[i];
        }
        return null;
    }

    int GetNextEmptySlotIndex()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].enabled)
                return i;
        }
        return -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialColorAnimation : MonoBehaviour
{
    public Color Color;
    public float Duration;
    public Ease Ease;
    public UnityEngine.Events.UnityEvent OnComplete;

    private void OnEnable()
    {
        var mat = GetComponent<Renderer>().material;
        var color = mat.GetColor("_BaseColor");
        DOTween.To(() => color, (x) => { mat.SetColor("_BaseColor", x); }, Color, Duration).SetEase(Ease).OnComplete(()=> { OnComplete?.Invoke(); });
    }
}

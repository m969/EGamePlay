using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameUtils;

public class PointSelectManager : MonoBehaviour
{
    public static PointSelectManager Instance { get; set; }
    private Action<Vector3> OnSelectPointCallback { get; set; }
    public GameObject HeroObj;
    public GameObject RangeCircleObj;
    public GameObject SkillPointObj;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        RangeCircleObj.transform.position = new Vector3(HeroObj.transform.position.x, 0.1f, HeroObj.transform.position.z);
        if (RaycastHelper.CastMapPoint(out var hitPoint))
        {
            SkillPointObj.transform.position = new Vector3(hitPoint.x, 0.1f, hitPoint.z);
            if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
            {
                Hide();
                OnSelectPointCallback?.Invoke(hitPoint);
            }
        }
    }

    public void Show(Action<Vector3> onSelectPointCallback)
    {
        Cursor.visible = false;
        gameObject.SetActive(true);
        OnSelectPointCallback = onSelectPointCallback;
    }

    public void Hide()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }
}

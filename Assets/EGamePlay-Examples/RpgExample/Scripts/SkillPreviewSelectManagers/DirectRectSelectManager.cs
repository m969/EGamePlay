using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using System;
using GameUtils;

public class DirectRectSelectManager : MonoBehaviour
{
    public static DirectRectSelectManager Instance { get; set; }
    private Action<float> OnSelectedCallback { get; set; }
    public GameObject HeroObj;
    public GameObject DirectObj;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        DirectObj.transform.position = new Vector3(HeroObj.transform.position.x, 0.1f, HeroObj.transform.position.z);
        if (RaycastHelper.CastMapPoint(out var hitPoint))
        {
            DirectObj.transform.forward = new Vector3(hitPoint.x, 0.1f, hitPoint.z) - DirectObj.transform.position;
            if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
            {
                Hide();
                OnSelectedCallback?.Invoke(DirectObj.transform.localEulerAngles.y);
            }
        }
    }

    public void Show(Action<float> onSelectedCallback)
    {
        //Cursor.visible = false;
        gameObject.SetActive(true);
        OnSelectedCallback = onSelectedCallback;
    }

    public void Hide()
    {
        //Cursor.visible = true;
        gameObject.SetActive(false);
    }
}

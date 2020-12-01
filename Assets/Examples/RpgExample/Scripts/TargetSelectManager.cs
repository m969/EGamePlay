using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using System;

public class TargetSelectManager : MonoBehaviour
{
    public static TargetSelectManager Instance { get; set; }
    private Action<GameObject> OnSelectTargetCallback { get; set; }
    public Image CursorImage { get; set; }
    public Color CursorColor { get; set; }
    public GameObject HeroObj;
    public GameObject RangeCircleObj;


    private void Awake()
    {
        Instance = this;
        CursorImage = GetComponent<Image>();
        CursorColor = CursorImage.color;
        Hide();
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
        if (RaycastHelper.CastEnemyObj(out var enemyObj))
        {
            //if (Vector3.Distance(enemyObj.transform.position, HeroObj.transform.position) < 10f)
            {
                if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
                {
                    Hide();
                    OnSelectTargetCallback?.Invoke(enemyObj);
                }
                CursorImage.color = Color.red;
            }
        }
        else
        {
            CursorImage.color = CursorColor;
        }
    }

    public void Show(Action<GameObject> onSelectTargetCallback)
    {
        Cursor.visible = false;
        gameObject.SetActive(true);
        RangeCircleObj.SetActive(true);
        OnSelectTargetCallback = onSelectTargetCallback;
    }

    public void Hide()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
        RangeCircleObj.SetActive(false);
    }
}

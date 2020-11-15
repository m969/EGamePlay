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


    private void Awake()
    {
        Instance = this;
        CursorImage = GetComponent<Image>();
        CursorColor = CursorImage.color;
        Hide();
    }

    private void Update()
    {
        if (RaycastHelper.CastEnemyObj(out var enemyObj))
        {
            if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
            {
                OnSelectTargetCallback?.Invoke(enemyObj);
            }
            CursorImage.color = Color.red;
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
        OnSelectTargetCallback = onSelectTargetCallback;
    }

    public void Hide()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public float cursorOffset = 0.2f;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos - new Vector2(0, cursorOffset);
    }
}

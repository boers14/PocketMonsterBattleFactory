using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetUIStats
{
    public static void SetUIPosition(Canvas canvas, GameObject uiObject, float xSize, float ySize, float xPos, float yPos, float yPlacement, 
        float xPlacement, int index = 0, bool moveVertically = false, bool moveSideWard = false, bool iterateThroughtChilds = false, 
        bool setComparativeYSize = false, float extraComperativeYSize = 1)
    {
        uiObject.transform.SetParent(canvas.transform);

        Vector2 originalSize = uiObject.GetComponent<RectTransform>().sizeDelta;

        Vector2 size = Vector2.zero;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / xSize;

        if (setComparativeYSize)
        {
            size.y = size.x * extraComperativeYSize;
        }
        else
        {
            size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / ySize;
        }

        uiObject.GetComponent<RectTransform>().sizeDelta = size;

        if (iterateThroughtChilds)
        {
            LoopTroughtChilds(uiObject.transform, size, originalSize);
        }

        Vector3 pos = Vector3.zero;

        if (yPos != 0)
        {
            if (moveVertically)
            {
                pos.y = (canvas.GetComponent<RectTransform>().sizeDelta.y / yPos * yPlacement) - (size.y / 2 * yPlacement) - size.y * index;
            }
            else
            {
                pos.y = (canvas.GetComponent<RectTransform>().sizeDelta.y / yPos * yPlacement) - (size.y / 2 * yPlacement);
            }
        }
        else
        {
            if (moveVertically)
            {
                pos.y -= size.y * index;
            }
        }

        if (xPos != 0)
        {
            if (moveSideWard)
            {
                pos.x = (-canvas.GetComponent<RectTransform>().sizeDelta.x / xPos * xPlacement) + (size.x / 2 * xPlacement) + size.x * index;
            }
            else
            {
                pos.x = (-canvas.GetComponent<RectTransform>().sizeDelta.x / xPos * xPlacement) + (size.x / 2 * xPlacement);
            }
        }

        uiObject.GetComponent<RectTransform>().localPosition = pos;
    }

    public static void LoopTroughtChilds(Transform uiObject, Vector2 size, Vector2 originalSize)
    {
        for (int i = 0; i < uiObject.childCount; i++)
        {
            SetComparitiveSize(uiObject.GetChild(i), size, originalSize);
            for (int j = 0; j < uiObject.GetChild(i).childCount; j++)
            {
                LoopTroughtChilds(uiObject.GetChild(i), size, originalSize);
            }
        }
    }

    private static void SetComparitiveSize(Transform uiObject, Vector2 size, Vector2 originalSize)
    {
        Vector2 decimalGrowth = Vector2.zero;
        decimalGrowth.x = size.x / originalSize.x;
        decimalGrowth.y = size.y / originalSize.y;
        RectTransform uiTransform = uiObject.GetComponent<RectTransform>();

        Vector2 newSize = Vector2.zero;
        newSize.x = uiTransform.sizeDelta.x * decimalGrowth.x;
        newSize.y = uiTransform.sizeDelta.y * decimalGrowth.y;

        uiTransform.sizeDelta = newSize;

        Vector3 newPos = Vector3.zero;
        newPos.x = uiTransform.localPosition.x * decimalGrowth.x;
        newPos.y = uiTransform.localPosition.y * decimalGrowth.y;
        uiTransform.localPosition = newPos;
    }
}

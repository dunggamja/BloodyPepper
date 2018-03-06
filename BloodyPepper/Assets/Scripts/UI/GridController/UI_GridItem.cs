using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GridItemData
{
}

public class UI_GridItem : MonoBehaviour
{
    [HideInInspector]
    public int index = -1;

    public int Width = 0;
    public int Height = 0;

    public virtual void Initialize()
    {
        index = -1;
    }

    public virtual void UpdateUI(UI_GridItemData itemData)
    {

    }
}

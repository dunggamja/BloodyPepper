using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GridController : MonoBehaviour {

    [SerializeField]
    private ScrollRect scrollRect = null;
    [SerializeField]
    private UI_GridItem gridItem = null;

    [HideInInspector]
    public List<UI_GridItemData> listGridItemData = new List<UI_GridItemData>();
    [HideInInspector]
    public List<UI_GridItem> listGridItem = new List<UI_GridItem>();

    //스크롤시 변경해야할 아이템을 계산하기 위한 변수들
    private int savedStartY = -1;
    private int savedStartX = -1;
    private int savedRowCount = 0;
    private int savedColumnCount = 0;
    private int savedDisplayRowCount = 0;
    private int savedDisplayColumnCount = 0;

    // Use this for initialization
    void Start () {
        //스크롤이 변경될 때 실행될 함수 
        scrollRect.onValueChanged.AddListener(onChangeScroll);
    }

    private void onChangeScroll(Vector2 arg0)
    {
        RefreshGridItem();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ClearGridItem()
    {
        for (int i = 0; i < listGridItem.Count; ++i)
            Destroy(listGridItem[i]);

        listGridItem.Clear();
        listGridItemData.Clear();
    }

    public void CreateGridItem()
    {
        CalculateRowNColumns(out savedRowCount, out savedColumnCount, out savedDisplayRowCount, out savedDisplayColumnCount);


        float sizeX = scrollRect.content.sizeDelta.x;
        float sizeY = scrollRect.content.sizeDelta.y;

        //가로 스크롤이 가능할 경우. 가로 사이즈 재 계산.
        if (scrollRect.horizontal && sizeX < gridItem.Width * savedColumnCount)
            sizeX = gridItem.Width * savedColumnCount;

        //세로 스크롤이 가능할 경우 세로 사이즈 재 계산.
        if (scrollRect.vertical && sizeY < gridItem.Height * savedRowCount)
            sizeY = gridItem.Height * savedRowCount;


        scrollRect.content.sizeDelta = new Vector2(sizeX, sizeY);
        //scrollRect.content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        //메모리 할당.
        listGridItem.Capacity = savedDisplayRowCount * savedDisplayColumnCount;

        //한번에 표시가능한 숫자만큼 오브젝트들을 생성해놓는다..
        for (int y = 0; y < savedDisplayRowCount; ++y)
        {
            for (int x = 0; x < savedDisplayColumnCount; ++x)
            {
                int idx = y * savedDisplayColumnCount + x;

                //원본 오브젝트를 통해 게임오브젝트 생성.
                GameObject newObj = Instantiate(gridItem.gameObject, scrollRect.content.transform) as GameObject;
                UI_GridItem newObjGridItem = newObj.GetComponent<UI_GridItem>();

                //인덱스는 일단 -1 로 초기화 처리.
                if (null != newObjGridItem)
                {
                    newObjGridItem.Initialize();
                }

                
                newObj.name = string.Format("{0}_{1}", newObj.name, idx);
                listGridItem.Add(newObjGridItem);
            }
        }

        RefreshGridItem(true);

        //원본 오브젝트는 비활성화 처리.
        gridItem.gameObject.SetActive(false);
    }

    public void RefreshGridItem(bool aRefreshAllItem = false)
    {
        //현재 스크롤뷰의 포지션을 통해서 표시해야할 아이템의 범위를 결정!!
        int startX = (int)(scrollRect.content.anchoredPosition.x / gridItem.Width);
        int startY = (int)(scrollRect.content.anchoredPosition.y / gridItem.Height);


        if (startX < 0)
        {
            startX = 0;
        }
        else if (savedColumnCount <= startX + savedDisplayColumnCount)
        {
            startX = savedColumnCount - savedDisplayColumnCount;
        }

        if (startY < 0)
        {
            startY = 0;
        }
        else if (savedRowCount <= startY + savedDisplayRowCount)
        {
            startY = savedRowCount - savedDisplayRowCount;
        }


        int endX = startX + savedDisplayColumnCount;
        int endY = startY + savedDisplayRowCount;

        float startPosX = 0f;
        float startPosY = 0f;


        // 그리드 아이템의 시작지점 X.
        if (scrollRect.horizontal)
            startPosX = (gridItem.Width - scrollRect.content.sizeDelta.x) * 0.5f;

        // 그리드 아이템의 시작지점 Y
        if (scrollRect.vertical)
            startPosY = (scrollRect.content.sizeDelta.y - gridItem.Height) * 0.5f;

        
        List<UI_GridItem> listRefreshItem = new List<UI_GridItem>();
        if (aRefreshAllItem)
        {
            List<int> listShowIndexs = new List<int>();

            for (int y = startY; (y < endY) && (y < savedRowCount); ++y)
            {
                if (y < 0) continue;

                for (int x = startX; (x < endX) && (x < savedColumnCount); ++x)
                {
                    if (x < 0) continue;

                    int index = y * savedColumnCount + x;
                    listShowIndexs.Add(index);
                }
            }


            //모든 아이템을 갱신!!
            for (int i = 0; i < listGridItem.Count; ++i)
            {
                if (i < listShowIndexs.Count)
                {
                    listGridItem[i].index = listShowIndexs[i];
                }
                else
                {
                    listGridItem[i].index = -1;
                }
            }

            listRefreshItem.AddRange(listGridItem);
        }
        else
        {

            List<int> listShowIndexs = new List<int>();

            for (int y = startY; (y < endY) && (y < savedRowCount); ++y)
            {
                if (y < 0) continue;

                for (int x = startX; (x < endX) && (x < savedColumnCount); ++x)
                {
                    if (x < 0) continue;

                    if (  0 <= savedStartX && savedStartX <= x && x < savedStartX + savedDisplayColumnCount
                       && 0 <= savedStartY && savedStartY <= y && y < savedStartY + savedDisplayRowCount)
                            continue;

                    int index = y * savedColumnCount + x;
                    listShowIndexs.Add(index);
                }
            }

            //현재 아이템들을 순회하면서 표시되어야 아이템들 체크.
            //표시되어야 하는 인덱스인지 체크!!
            Func<int, int, bool> funcIsValidIndex = (int x, int y) => 
            {
                if (x < startX || y < startY || endX <= x || endY <= y)
                    return false;

                return true;
            };


            //표시되어야 하는 아이템을 1개씩 가져온다.
            //약간 나이브하게 구조가 잡혀있는 것 같지만....
            //그리드 아이템의 갯수가 부족할 일은 계산상 없을테니....
            int showIdx = 0;
            for (int i = 0; i < listGridItem.Count; ++i)
            {
                int x = listGridItem[i].index % savedColumnCount;
                int y = listGridItem[i].index / savedColumnCount;

                if (false == funcIsValidIndex(x, y))
                {
                    if (showIdx < listShowIndexs.Count)
                    {
                        listGridItem[i].index = listShowIndexs[showIdx++];
                    }
                    else
                    {
                        listGridItem[i].index = -1;
                    }

                    listRefreshItem.Add(listGridItem[i]);
                }
            }
        }


        for (int i = 0; i < listRefreshItem.Count; ++i)
        {
            if (null == listRefreshItem[i])
                continue;

            int index = listRefreshItem[i].index;
            if (index  < 0 || listGridItemData.Count <= index)
            {
                listRefreshItem[i].gameObject.SetActive(false);
                continue;
            }

            RectTransform rectComp = listRefreshItem[i].GetComponent<RectTransform>();
            if (rectComp)
            {
                int x = index % savedColumnCount;
                int y = index / savedColumnCount;
                rectComp.anchoredPosition = new Vector2(startPosX + x * gridItem.Width, startPosY + -y * gridItem.Height);
            }

            listRefreshItem[i].UpdateUI(listGridItemData[index]);
        }


        //다음연산때 보정을 위해서 저장합니다. 
        savedStartX = startX;
        savedStartY = startY;
    }

    private void CalculateRowNColumns(out int row, out int column, out int showRow, out int showColumn)
    {
        float scrollHeight = scrollRect.GetComponent<RectTransform>().sizeDelta.y;
        float scrollWidth = scrollRect.GetComponent<RectTransform>().sizeDelta.x;

        //기본값 1
        row = column = showRow = showColumn = 1;

        //스크롤뷰에 보여질수 있는 오브젝트 갯수 연산

        //세로 스크롤이 가능할 경우 
        if (scrollRect.vertical)
        {
            // 스크롤시에는 1개만큼 더 보일수 있으므로 + 1 연산
            showRow = (int)scrollHeight / gridItem.Height + 1;
        }


        //가로 스크롤이 가능할 경우.
        if (scrollRect.horizontal)
        {
            // 스크롤시에는 1개만큼 더 보일수 있으므로 + 1 연산
            showColumn = (int)scrollWidth / gridItem.Width + 1;
        }

        //실제 Row, Column의 갯수 체크
        if (scrollRect.horizontal)
        {
            int divCount = 1;

            //실제 보여지기용으로 +1 연산했던걸 다시 빼줌.
            if (1 < showRow)
                divCount = showRow - 1;

            column = listGridItemData.Count / divCount;
            if (0 != listGridItemData.Count % divCount)
                column++;
        }


        if (scrollRect.vertical)
        {
            //위에서 column 계산했던 값으로 row 연산
            row = listGridItemData.Count / column;
            if (0 != listGridItemData.Count % column)
                row++;
        }
        
    }

}

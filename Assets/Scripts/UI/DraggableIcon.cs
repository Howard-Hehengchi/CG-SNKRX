using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int totalCount;

    private float topPadding;
    private float spacing;
    private float height;
    private int sibling;

    private float yMax, yMin;

    private TrainInfoContainer container;
    private LayoutElement layoutElement;

    private float deltaY;

    private void Start()
    {
        VerticalLayoutGroup layout = transform.parent.GetComponent<VerticalLayoutGroup>();
        topPadding = layout.padding.top;
        spacing = transform.parent.GetComponent<VerticalLayoutGroup>().spacing;
        height = GetComponent<RectTransform>().sizeDelta.y;

        container = transform.parent.GetComponent<TrainInfoContainer>();
        layoutElement = GetComponent<LayoutElement>();

        float canvasHeight = transform.root.GetComponent<RectTransform>().sizeDelta.y;
        RectTransform trainDisplayRectTF = transform.parent.parent.parent.GetComponent<RectTransform>();
        float anchoredY = trainDisplayRectTF.anchoredPosition.y + trainDisplayRectTF.sizeDelta.y / 2f;
        yMax = canvasHeight + anchoredY - height / 2f - topPadding;
        yMin = yMax - (spacing + height) * (totalCount - 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        sibling = transform.GetSiblingIndex();
        container.SetStartIndex(sibling);
        transform.SetAsLastSibling();
        layoutElement.ignoreLayout = true;
        deltaY = transform.position.y - eventData.position.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float yPos = Mathf.Clamp(eventData.position.y + deltaY, yMin, yMax);        
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

        if(yPos >= GetPos(sibling - 1))
        {
            sibling--;
            if(sibling < 0)
            {
                sibling++;
            }
        }
        else if (yPos <= GetPos(sibling + 1))
        {
            sibling++;
            if(sibling >= totalCount)
            {
                sibling = totalCount - 1;
            }
        }
        //sibling = (int)(-(transform.localPosition.y - topPadding) / (height + spacing));
        container.SetEmpty(sibling);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetSiblingIndex(sibling);
        container.DeactiveEmpty();
        container.SetEndIndex(sibling);
        layoutElement.ignoreLayout = false;
    }

    private float GetPos(int index)
    {
        if(index < 0)
        {
            return yMax - height * 0.2f;
        }
        else if(index >= totalCount)
        {
            return yMin + height * 0.2f;
        }

        return yMax - (height + spacing) * index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            int index = transform.GetSiblingIndex();
            GameManager.RemoveUnit(index);

            container.RefreshTrainInfo();
        }
    }
}

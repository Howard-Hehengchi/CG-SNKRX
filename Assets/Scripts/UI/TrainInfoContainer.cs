using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainInfoContainer : MonoBehaviour
{
    [SerializeField]
    private DraggableIcon unitIcon;
    [SerializeField]
    private GameObject emptyUnitIcon;

    private List<DraggableIcon> icons;
    private GameObject emptyUnit;

    private int startIndex;
    private int endIndex;

    private void Start()
    {
        icons = new List<DraggableIcon>();

        int unitCount = GameManager.train.Count;
        for(int i = 0; i < unitCount; i++)
        {
            DraggableIcon icon = Instantiate(unitIcon, transform);
            icon.GetComponent<Image>().color = Unit.GetColor(GameManager.train[i]);
            icon.totalCount = unitCount;
            icons.Add(icon);
        }
        emptyUnit = Instantiate(emptyUnitIcon, transform);
        emptyUnit.SetActive(false);
    }

    public void RefreshTrainInfo()
    {
        int count = icons.Count;
        for(int i = count - 1; i >= 0; i--)
        {
            Destroy(icons[i].gameObject);
            icons.RemoveAt(i);
        }
        Destroy(emptyUnit);
        
        int unitCount = GameManager.train.Count;
        for(int i = 0; i < unitCount; i++)
        {
            DraggableIcon icon = Instantiate(unitIcon, transform);
            icon.GetComponent<Image>().color = Unit.GetColor(GameManager.train[i]);
            icon.totalCount = unitCount;
            icons.Add(icon);
        }
        emptyUnit = Instantiate(emptyUnitIcon, transform);
        emptyUnit.SetActive(false);
    }

    public void SetEmpty(int index)
    {
        emptyUnit.transform.SetSiblingIndex(index);
        emptyUnit.SetActive(true);
    }

    public void DeactiveEmpty()
    {
        emptyUnit.transform.SetAsLastSibling();
        emptyUnit.SetActive(false);
    }

    public void SetStartIndex(int index)
    {
        startIndex = index;
    }

    public void SetEndIndex(int index)
    {
        endIndex = index;
        GameManager.MoveUnit(startIndex, endIndex);
    }
}

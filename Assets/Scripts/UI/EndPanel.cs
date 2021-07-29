using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject successText;
    [SerializeField]
    private GameObject failText;

    //private void Start()
    //{
    //    Debug.Log("FUck?");
    //    successText.SetActive(false);
    //    failText.SetActive(false);
    //}

    private void OnEnable()
    {
        successText.SetActive(false);
        failText.SetActive(false);
    }

    public void ShowEndText(bool succeeded)
    {
        if (succeeded)
        {
            successText.SetActive(true);
        }
        else
        {
            failText.SetActive(true);
        }
    }
}

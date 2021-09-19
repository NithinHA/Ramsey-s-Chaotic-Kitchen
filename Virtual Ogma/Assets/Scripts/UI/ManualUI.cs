using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualUI : MonoBehaviour
{
    public GameObject manual_body;
    public GameObject main_panel;

    public static ManualUI instance;
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    public void showManual()
    {
        if (!manual_body.activeSelf)
        {
            manual_body.SetActive(true);
            main_panel.SetActive(true);
        }
        else
        {
            for (int i = 0; i < manual_body.transform.childCount; i++)
            {
                if (manual_body.transform.GetChild(i).gameObject.activeSelf)
                {
                    manual_body.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
            manual_body.SetActive(false);
        }
    }

    public void openManualPage(GameObject page)
    {
        for (int i = 0; i < manual_body.transform.childCount; i++)
        {
            if (manual_body.transform.GetChild(i).gameObject.activeSelf)
            {
                manual_body.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
        page.SetActive(true);
    }
}

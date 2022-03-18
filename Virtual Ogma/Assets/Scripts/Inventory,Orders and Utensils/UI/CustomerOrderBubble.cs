using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderBubble : MonoBehaviour
{
    [SerializeField] private Image objectIcon;
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0, 50, 0);

    private Customer customer;
    private Transform target;
    private Camera mainCam;
    RectTransform rect;

    private void Start()
    {
        mainCam = Camera.main;
        rect = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        customer.onFoodOrdered -= DisplayOrder;   
        customer.onFoodServed -= OrderServed;
    }

    private void Update()
    {
        if(target != null)
        {
            rect.position = mainCam.WorldToScreenPoint(target.position) + bubbleOffset;
        }
    }

    public void Init(Customer customer, Transform target)
    {
        customer.onFoodOrdered += DisplayOrder;
        customer.onFoodServed += OrderServed;
        this.customer = customer;
        this.target = target;
        gameObject.SetActive(false);
    }

    public void DisplayOrder(Item item)
    {
        objectIcon.sprite = item.icon;
        gameObject.SetActive(true);
    }

    public void OrderServed()
    {
        objectIcon.sprite = null;
        gameObject.SetActive(false);
    }
}

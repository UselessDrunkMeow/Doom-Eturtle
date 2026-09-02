using System;
using UnityEngine;

public class ShopButtonScript : MonoBehaviour
{
    private Boolean IsShowing;
    public GameObject shop;

    public void ShopPressed()
    {
        IsShowing = !IsShowing;
        shop.SetActive(IsShowing);
    }
}

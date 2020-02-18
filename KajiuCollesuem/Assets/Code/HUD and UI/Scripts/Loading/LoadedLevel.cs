﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadedLevel : MonoBehaviour
{
    public RectTransform screenTransition;
    
    void Start()
    {
        StartCoroutine(screenMove());
    }

    IEnumerator screenMove()
    {
        yield return new WaitForSeconds(1);
        screenTransition.DOAnchorPos(new Vector2(0, 561), 0.4f);
    }
}

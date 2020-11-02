using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewspaperUI : MonoBehaviour
{
    [SerializeField] SimpleNewspaperThrower newspaperThrower;
    [SerializeField] TMP_Text text;

    private void FixedUpdate()
    {
        text.text = newspaperThrower.newspapersLeft.ToString();
    }
}

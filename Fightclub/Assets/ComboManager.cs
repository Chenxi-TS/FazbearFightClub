using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text comboText;
    int playerBeingCombo = 1;
    int comboHits = 0;

    Coroutine comboCorotine;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(comboHits > 0)
            comboText.text = comboHits.ToString() + " HIT";
        if (playerBeingCombo == 2)
            comboText.color = Color.cyan;
        else
            comboText.color = Color.magenta;
    }

    public void AHitConnected(int playerHit)
    {
        if (playerBeingCombo != playerHit)
        {
            playerBeingCombo= playerHit;
            comboHits = 1;
        }
        else
            comboHits++;
        if(comboCorotine != null)
            StopCoroutine(comboCorotine);
        comboCorotine = StartCoroutine(ComboTimer());
    }

    public void PlayerRecovered(int playerRecovered)
    {
        if (playerRecovered != playerBeingCombo)
            return;
        Debug.Log("COMBO RECOVER");
        StopCoroutine(comboCorotine);
        comboHits = 0;
        comboText.text = "";
    }

    IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("COMBO TIMEOUT");
        comboHits = 0;
        comboText.text = "";
    }
}

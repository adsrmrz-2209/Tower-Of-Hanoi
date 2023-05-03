using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    [SerializeField] private GameObject[] discs = new GameObject[8];

    public int discIndex = 3;

    [SerializeField] private GameObject poofParticle;

    public int conditionsMet;
    [SerializeField] private GameObject winMsg;

    private void Update()
    {
        AreYaWinningSon();

        discIndex = Mathf.Clamp(discIndex, 3, 7);
    }

    void AreYaWinningSon()
    {
        conditionsMet = Mathf.Clamp(conditionsMet, 0 , discIndex);
        UIScript ui = FindObjectOfType<UIScript>();

        //if conditions of disc set on the 3rd stick is equal to the Changeable/dynamic ring count feature , then player won
        bool status = conditionsMet == ui.discNumDisplay ? true : false; 
        
        winMsg.SetActive(status);
    }

    public void ChangeDiscValue(int discNum, bool setActive)
    {
        conditionsMet = 0;
        GameObject.Find("PoofSound").GetComponent<AudioSource>().Play(); //for dramatic sound
        for (int i = 0; i < discs.Length; i++)
        {
            //reset all the position of each disc
            discs[i].GetComponent<DiscScript>().transform.position = new Vector3(2.569998f, discs[i].GetComponent<DiscScript>().discYpos, 0f);
            discs[i].GetComponent<DiscScript>().rb.isKinematic = false;
        }
        Instantiate(poofParticle, discs[discIndex].transform.position, Quaternion.identity); //for dramatic effects
        discs[discIndex].SetActive(setActive);
        discIndex += discNum;
    }

    public void ResetDisc()
    {
        conditionsMet = 0;
        discIndex = 3;
        GameObject.Find("PoofSound").GetComponent<AudioSource>().Play(); //for dramatic sound

        for (int i = 0; i < discs.Length; i++)
        {
            if (discs[i].activeSelf) Instantiate(poofParticle, discs[i].transform.position, Quaternion.identity);
            //reset all the position of each disc and disable all disc
            discs[i].GetComponent<DiscScript>().transform.position = new Vector3(2.569998f, discs[i].GetComponent<DiscScript>().discYpos, 0f);
            discs[i].GetComponent<DiscScript>().rb.isKinematic = false;
            discs[i].SetActive(false);
        }

        //activate only the first 3 disc
        for (int j = 0; j < 3; j++)
        {
            discs[j].SetActive(true);
            Instantiate(poofParticle, discs[j].transform.position, Quaternion.identity); //for dramatic effects 
        }
    }
}

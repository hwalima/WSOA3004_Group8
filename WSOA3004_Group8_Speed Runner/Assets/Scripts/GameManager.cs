﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject respawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respawnTime;

    public int collectedSticks=0;
    public Text sticksCollectedText;

    private float respawnTimeStart;

    private bool respawn;

    private CinemachineVirtualCamera CVC;

    public PlayableDirector playableDirector;

   public GameObject popUpText;


    public TextMeshProUGUI unlockedAbilityTextHolder;
    public string[] UnlockedAbilityWhatToSay;

    public string[] AbilityTutorial;
    public TMP_Text tutorialTextComponent;
    public bool isCutscene;

    public TextMeshProUGUI lToContinue;

    private void Start()
    {
       
        CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        unlockedAbilityTextHolder.gameObject.SetActive(false);
       
        
    }

    private void Update()
    {
        CheckRespawn();
        if (isCutscene)
        {
            tutorialTextComponent.ForceMeshUpdate();
            var textInfo = tutorialTextComponent.textInfo;
            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                {
                    continue;
                }
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x + 0.01f) * 10f, 0);

                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                tutorialTextComponent.UpdateGeometry(meshInfo.mesh, i);
            }


        }
        if(isCutscene&& Input.GetKeyDown(KeyCode.L))
        {
            isCutscene = false;
            tutorialTextComponent.gameObject.SetActive(false);
            lToContinue.gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            var playerTemp = Instantiate(player, respawnPoint.transform.position, respawnPoint.transform.rotation);
            CVC.m_Follow = playerTemp.transform;
            respawn = false;
        }
    }

    public void CollectedStick(int sticksCollected)
    {
        collectedSticks = collectedSticks + sticksCollected;
        sticksCollectedText.text = collectedSticks.ToString();
        //playableDirector.Play();
        StartCoroutine(NewAbilityText());

        isCutscene = true;


    }

    IEnumerator PopUpText()
    {

        popUpText.SetActive(true);
        yield return new WaitForSeconds(2f);
        popUpText.SetActive(false);


    }
    public void SwitchOnPopUpText()
    {
        StartCoroutine(PopUpText());
    }

    IEnumerator NewAbilityText()
    {
        unlockedAbilityTextHolder.gameObject.SetActive(true);
        unlockedAbilityTextHolder.gameObject.transform.position = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Yay i can Text Pos").transform.position);
        unlockedAbilityTextHolder.text = UnlockedAbilityWhatToSay[collectedSticks - 1];
        tutorialTextComponent.gameObject.SetActive(true);
        tutorialTextComponent.gameObject.transform.position= Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("tutorialTextPos").transform.position);
        tutorialTextComponent.text = AbilityTutorial[collectedSticks - 1];
        lToContinue.gameObject.SetActive(true);
        lToContinue.transform.position = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Player").transform.position)+new Vector3(120,-120f);
        yield return new WaitForSeconds(1f);
        unlockedAbilityTextHolder.gameObject.SetActive(false);
    }

    public void LastCheckPoint()
    {
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        currentPlayer.transform.position = respawnPoint.transform.position;
    }
}

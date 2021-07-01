﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClassroomDialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    private Queue<string> sentences;
    public bool[] isCalledOn;
    public bool[] isWandMoving;
    public bool[] isWriting;
    public int ConvoCount;
    public Animator player;
        
    public float WaitTimeSec;
    public ClassroomDialogue dialogue;

    public bool toBeContinued;
    public bool isChoice;
    public bool isFinished;

    public GameObject thisConversation, nextConversation;
    public GameObject choicesMenu;

    public string RoomToGoTo;

    Animator datePlay;

    public int EndOfLessonLearning;

    public bool isTransfigurationDemonstration;
    public GameObject TransfiguredDemonstration;

    private void Start()
    {
        sentences = new Queue<string>();

        StartCoroutine(InitialWaiting());

        datePlay = GameObject.Find("CanvasForDate").GetComponent<Animator>();
    }

    IEnumerator InitialWaiting()
    {
        yield return new WaitForSeconds(WaitTimeSec);
        StartDialogue(dialogue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(ClassroomDialogue dialogue)
    {
        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isCalledOn.Length == 0 && isWandMoving.Length == 0 && isWriting.Length == 0)
        {
            player.SetBool("calledOn", false);
            player.SetBool("wandMovement", false);
            player.SetBool("isWriting", false);
        }

        else if (ConvoCount > isCalledOn.Length && ConvoCount > isWandMoving.Length && ConvoCount > isWriting.Length)
        {
            player.SetBool("calledOn", false);
            player.SetBool("wandMovement", false);
            player.SetBool("isWriting", false);
        }
        else
        {
            if (isCalledOn[ConvoCount])
            {
                player.SetBool("calledOn", true);
            }
            else if (isWandMoving[ConvoCount] )
            {
                player.SetBool("wandMovement", true);
            }
            else if (isWriting[ConvoCount] )
            {
                player.SetBool("isWriting", true);
            }
            else
            {
                player.SetBool("calledOn", false);
                player.SetBool("wandMovement", false);
                player.SetBool("isWriting", false);
            }
        }
        ConvoCount++;


        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);

        if (toBeContinued)
        {
            nextConversation.SetActive(true);
            thisConversation.SetActive(false);

            if (isTransfigurationDemonstration)
            {
                TransfiguredDemonstration.SetActive(true);
            }
        }
        if (isChoice)
        {
            choicesMenu.SetActive(true);
            thisConversation.SetActive(false);
        }
        if (isFinished)
        {
            Debug.Log("endOfConversation");
            EndOfLessonLearning += GameManager.Intelligence;
            Debug.Log("Play Quick Animation to show learning");
            //Since there is only one gamemanager, we can reference the instance of the CanvasForStats to turn on or off
            GameManager.instance.CanvasForStats.SetActive(true);
            GameManager.IncreaseStatLevel();
            StartCoroutine(StatsWaiting());
           // GameManager.ProgressDay();
        }
    }
    IEnumerator StatsWaiting()
    {
        yield return new WaitForSeconds(3f);
        GameManager.instance.CanvasForStats.SetActive(false);
        datePlay.SetBool("ToPlay", true);
        StartCoroutine(Waiting());
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(2.1f);
        datePlay.SetBool("ToPlay", false);
        StartCoroutine(LoadRoomWait());
    }

    IEnumerator LoadRoomWait()
    {
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene(RoomToGoTo);
    }
}

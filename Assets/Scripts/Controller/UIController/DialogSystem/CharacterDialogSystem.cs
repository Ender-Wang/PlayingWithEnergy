using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterDialogSystem : MonoBehaviour
{
    // [Header("UI")]
    // public TextMeshProUGUI dialogText;
    // public Image characterImage;

    [Header("Dialog")]
    public GameObject dialogPrefab;

    [SerializeField] List<List<string>> textList = new List<List<string>>();
    [SerializeField] List<string> textFile;
    [SerializeField] List<string> optionalTextList = new List<string>();
    public GameObject dialog { get; set; }
    public int state = 0;

    bool inOptional = false;

    // void Awake()
    // {
    //     GetTextFromFile(textFile);
    // }

    /// <summary>
    /// API: Initialize the dialog system
    /// </summary>
    /// <param name="task_id"></param>
    /// <param name="npc_name"></param>
    public void Initialize(TaskType type, int task_id, string npc_name, int rounds, List<string> targetsName = null)
    {
        // Create dialog files randomly or load existing files
        gameObject.AddComponent<DialogCreator>().CreateDialog(type, task_id, npc_name, rounds, targetsName);
        textList = gameObject.GetComponent<DialogCreator>().dialog;
        optionalTextList = gameObject.GetComponent<DialogCreator>().optionalDialog;
        textFile = textList[0];
        gameObject.GetComponent<DialogCreator>().SaveFiles();
    }



    /// <summary>
    /// Start the dialog
    /// </summary>
    public void DialogStart()
    {
        this.dialog = Instantiate(Resources.Load<GameObject>("Dialog Resources/Dialog"), GameManager.Instance.canvas.transform);
        if (!inOptional)
        {// when state = -1, it means the task is not finished
            if (state == 0)
            { // if the first state, combine greeting and demand
                List<string> chat = textList[0];
                List<string> demand = textList[1];
                for (int i = 0; i < demand.Count; i++)
                {
                    chat.Add(demand[i]);
                }
                dialog.GetComponent<Dialog>().SetDialogUI(chat, gameObject);
                state++;
            }
            else
                dialog.GetComponent<Dialog>().SetDialogUI(textList[state], gameObject);
        }
        else
            dialog.GetComponent<Dialog>().SetDialogUI(optionalTextList, gameObject);
    }

    /// <summary>
    /// API: push to the next state
    /// </summary>
    public void NextState()
    {
        inOptional = false;
        if (state < textList.Count - 1)
            state++;
    }

    /// <summary>
    /// API: push to the optianl state (e.g. task is not finished)
    /// </summary>
    public void SetOptianlState()
    {
        if (optionalTextList.Count != 0) inOptional = true;
    }

    /// <summary>
    /// API: push to the previous state
    /// </summary>
    public void PreviousState()
    {
        if (state > 0)
            state--;
    }

    /// <summary>
    /// API: if the dialogs are all finished
    /// </summary>
    /// <returns></returns>
    public bool isDialogsFinished()
    {
        return state >= textList.Count - 1;
    }

}

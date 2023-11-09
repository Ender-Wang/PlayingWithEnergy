using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class DialogCreator : MonoBehaviour
{
    string directoryPath_npc;

    TextAsset dialogTemplate;
    Root root;
    public List<List<string>> dialog = new List<List<string>>();
    public List<string> optionalDialog = new List<string>();

    // Template classes from Json------------------------------------
    // TODO: IMPROVEMENT: Can be change to chain of responsibility
    [System.Serializable]
    class Task
    {
        public List<string> Demands;
        public List<string> Additional;

    }

    [System.Serializable]
    class Root
    {
        public Templates templates;
    }

    [System.Serializable]
    class Templates
    {
        public List<string> Chat;
        public List<string> End;
        public FindNPC findNPC;
        public FindObjects findObjects;
    }

    [System.Serializable]
    class FindNPC : Task
    {
    }

    [System.Serializable]
    class FindObjects : Task
    {
    }

    Task JsonParser(TaskType type)
    {
        switch (type)
        {
            case TaskType.FindNPC:
                return root.templates.findNPC;
            case TaskType.FindObjects:
                return root.templates.findObjects;
            case TaskType.Other:
                return root.templates.findNPC;

            default:
                return null;
        }
    }
    // ----------------------------------------------------------------

    /// <summary>
    /// API: Create dialog files.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="taskId"></param>
    /// <param name="NPC_Name"></param>
    /// <param name="targetsName"></param>
    public void CreateDialog(TaskType type, int taskId, string NPC_Name, int rounds, List<string> targetsName = null)
    {
        // Create directory
        // NPC_Name = NPC_Name.Replace("[", "").Replace("]", "");
        string directoryPath_task = Application.dataPath + "/Resources/Dialog Resources/" + taskId;
        directoryPath_npc = directoryPath_task + "/" + NPC_Name;
        string path = directoryPath_npc + "/dialog.txt";

        // else if the directory of the task doesn't exist
        if (!Directory.Exists(directoryPath_task))
        {
            Directory.CreateDirectory(directoryPath_task);
            Directory.CreateDirectory(directoryPath_npc);
        }
        else if (!Directory.Exists(directoryPath_npc))
        {
            Directory.CreateDirectory(directoryPath_npc);
        }
        else
        { // if the directory of the task exists
            // Load existing files
            List<TextAsset> textFile = new List<TextAsset>();
            textFile.Add(Resources.Load<TextAsset>("Dialog Resources/" + taskId + "/" + NPC_Name + "/begin"));
            for (int i = 0; i < rounds; i++)
            {
                textFile.Add(Resources.Load<TextAsset>("Dialog Resources/" + taskId + "/" + NPC_Name + "/demands_" + i));
            }
            textFile.Add(Resources.Load<TextAsset>("Dialog Resources/" + taskId + "/" + NPC_Name + "/end"));
            textFile.Add(Resources.Load<TextAsset>("Dialog Resources/" + taskId + "/" + NPC_Name + "/optional"));
            GetTextFromFile(textFile);
            return;
        }

        // Create dialog files
        dialogTemplate = Resources.Load<TextAsset>("Dialog Resources/Dialog Template");
        root = JsonUtility.FromJson<Root>(dialogTemplate.text);

        Task task = JsonParser(type);

        // ----------- Begining of the conversation
        string begin = root.templates.Chat[UnityEngine.Random.Range(0, root.templates.Chat.Count)];
        dialog.Add(GetTextList(begin));
        // -------------

        // -------------- Demands
        if (type != TaskType.Other && task.Demands.Count != 0 && task.Demands[0] != "") // if the task is not other (e.g. subtask NPC) and the task has demands
        {
            if (targetsName != null)
            {
                foreach (string target in targetsName)
                {
                    string demand = task.Demands[UnityEngine.Random.Range(0, task.Demands.Count)].Replace("{0}", target);
                    dialog.Add(GetTextList(demand));
                }
            }
        }
        // --------------

        // -------------- End
        string end = root.templates.End[UnityEngine.Random.Range(0, root.templates.End.Count)];
        dialog.Add(GetTextList(end));
        // --------------

        // -------------- Additional
        string additional = task.Additional[UnityEngine.Random.Range(0, task.Additional.Count)];
        optionalDialog = GetTextList(additional);
        // --------------
    }


    void GetTextFromFile(List<TextAsset> textFile)
    {
        for (int i = 0; i < textFile.Count; i++)
        {
            Debug.Log(textFile[i].name);
            var lineData = textFile[i].text.Split('\n');
            List<string> text = new List<string>();
            if (textFile[i].name == "optional")
            { // if optional dialog
                foreach (var line in lineData)
                {
                    if (line != "")
                        optionalDialog.Add(line);
                }
            }
            else
            {
                foreach (var line in lineData)
                {
                    if (line != "")
                        text.Add(line);
                }
                dialog.Add(text);
            }
        }
    }

    /// <summary>
    /// Separate the text into lines
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    List<string> GetTextList(string text)
    {
        List<string> textList = new List<string>();
        var lineData = text.Split('\n');
        foreach (var line in lineData)
        {
            textList.Add(line);
        }
        return textList;
    }

    /// <summary>
    /// Combine the lines into text
    /// </summary>
    /// <param name="textList"></param>
    /// <returns></returns>
    static string GetText(List<string> textList)
    {
        string text = "";
        foreach (string line in textList)
        {
            text += line + "\n";
        }
        return text;
    }

    /// <summary>
    /// API: Save the dialog files. Attention! This function could be slow, take care when to use it!
    /// </summary>
    public void SaveFiles()
    {
        FileStream fs;
        byte[] bytes;
        // begin
        fs = new FileStream(directoryPath_npc + "/begin.txt", FileMode.Create);
        bytes = new UTF8Encoding().GetBytes(GetText(dialog[0]));
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();

        // demands
        int i = 0;
        foreach (List<string> demand in dialog.GetRange(1, dialog.Count - 2))
        {
            fs = new FileStream(directoryPath_npc + "/demands_" + i + ".txt", FileMode.Create);
            bytes = new UTF8Encoding().GetBytes(GetText(demand));
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            i++;
        }

        // end
        fs = new FileStream(directoryPath_npc + "/end.txt", FileMode.Create);
        bytes = new UTF8Encoding().GetBytes(GetText(dialog[dialog.Count - 1]));
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();


        // optional
        fs = new FileStream(directoryPath_npc + "/optional.txt", FileMode.Create);
        bytes = new UTF8Encoding().GetBytes(GetText(optionalDialog));
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }
}

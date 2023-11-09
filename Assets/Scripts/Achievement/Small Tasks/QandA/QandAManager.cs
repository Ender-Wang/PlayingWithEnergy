using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class QandAManager : MonoBehaviour
{
    public TextAsset QandAFile;
    public List<QandAItem> QandAItems { get; set; }
    public GameObject currentQandAPopup { get; set; }
    public static QandAManager Instance;

    public List<int> credits;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        QandAItems = new List<QandAItem>();
        SetListQandAItem(QandAFile.text);
        credits = new List<int>();
    }

    private void SetListQandAItem(string fileContent)
    {
        Stream stream = GenerateStreamFromString(fileContent);
        using (var reader = new StreamReader(stream))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split('|');

            // Find the indices of the Name, Description, Icon, Reward, RewardAmount, RewardType, Condition, ConditionAmount, ConditionType, IsCompleted, IsClaimed columns
            var idIndex = Array.IndexOf(headers, "ID");
            var questionIndex = Array.IndexOf(headers, "Question");
            var optionAIndex = Array.IndexOf(headers, "OptionA");
            var optionBIndex = Array.IndexOf(headers, "OptionB");
            var solutionIndex = Array.IndexOf(headers, "Solution");
            var pointIndex = Array.IndexOf(headers, "Point");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = MyCsvParser.parse(line);

                var id = int.Parse(valuesArray[idIndex]);
                var question = valuesArray[questionIndex];
                var optionA = valuesArray[optionAIndex];
                var optionB = valuesArray[optionBIndex];
                var solution = valuesArray[solutionIndex];
                var point = int.Parse(valuesArray[pointIndex]);

                QandAItems.Add(new QandAItem(id, question, optionA, optionB, solution, point));
            }
        }
    }

    private Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}

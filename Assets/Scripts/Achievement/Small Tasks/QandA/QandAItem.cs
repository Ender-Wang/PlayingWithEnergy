public class QandAItem
{
    public int id { get; set; } // QandQ pieces belong to a QandA Task have the same id
    public string question { get; set; }
    public string optionA { get; set; }
    public string optionB { get; set; }
    public string solution { get; set; }
    public int point { get; set; }

    public QandAItem()
    {
        id = 0;
        question = "Default";
        optionA = "Default";
        optionB = "Default";
        solution = "Default";
        point = 0;
    }

    public QandAItem(int id, string question, string optionA, string optionB, string solution, int point)
    {
        this.id = id;
        this.question = question;
        this.optionA = optionA;
        this.optionB = optionB;
        this.solution = solution;
        this.point = point;
    }
}
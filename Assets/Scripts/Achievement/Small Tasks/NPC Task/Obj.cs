using UnityEngine;

public class Obj : ObjectWithUI
{
    void OnEnable()
    {   
        hasInteraction = true;
        ChangeOffset(0.8f);
        ChangeScale(0.006f);
    }

    protected override void Interact()
    {
        base.Interact();
        transform.GetComponent<Task>().FinishTask(true);
    }
}
using EGamePlay;
using System;

public class WorkFlow : Entity
{
    public WorkFlowSource FlowSource { get; set; }
    public WorkFlow PreWorkFlow { get; set; }
    public WorkFlow PostWorkFlow { get; set; }



    public virtual void Startup()
    {
        
    }

    public void Finish()
    {
        FlowSource.OnFlowFinish();
    }

    public WorkFlow ToEnter<T>() where T : WorkFlow
    {
        var workflow = FlowSource.Parent.CreateChild<T>();
        workflow.FlowSource = FlowSource;
        PostWorkFlow = workflow;
        workflow.PreWorkFlow = this;
        return workflow;
    }

    public void ToRestart()
    {
        ToEnter<WorkRestartFlow>();
    }

    public void ToEnd()
    {
        ToEnter<WorkEndFlow>();
    }
}

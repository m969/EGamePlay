using EGamePlay;

public class WorkFlowSource : Entity
{
    public WorkFlow CurrentWorkFlow { get; private set; }
    public WorkFlow PostWorkFlow { get; private set; }


    public WorkFlow ToEnter<T>() where T : WorkFlow
    {
        var workflow = PostWorkFlow = Parent.AddChild<T>();
        workflow.FlowSource = this;
        return workflow;
    }

    public void Startup()
    {
        CurrentWorkFlow = PostWorkFlow;
        CurrentWorkFlow.Startup();
    }

    public void OnFlowFinish()
    {
        //Log.Debug(($"{GetType().Name}->OnFlowFinish {CurrentWorkFlow.GetType().Name} {CurrentWorkFlow.PostWorkFlow}"));
        CurrentWorkFlow = CurrentWorkFlow.PostWorkFlow;
        CurrentWorkFlow.Startup();
    }
}

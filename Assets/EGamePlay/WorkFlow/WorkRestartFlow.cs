
public class WorkRestartFlow : WorkFlow
{
    public override void Startup()
    {
        base.Startup();
        GetParent<WorkFlow>().Startup();
    }
}

using EGamePlay;
using EGamePlay.Combat;


public class CombatRunFlow : WorkFlow
{
    public override void Awake()
    {
        CombatContext.Instance.Subscribe<CombatEndEvent>((x) => { Finish(); });
    }

    public override async void Startup()
    {
        base.Startup();
        Log.Debug("CombatRunFlow Startup");

        await ET.TimeHelper.WaitAsync(1000);
        CombatContext.Instance.StartCombat();
    }
}

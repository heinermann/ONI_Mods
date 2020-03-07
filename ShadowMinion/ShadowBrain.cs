namespace Heinermann.ShadowMinion
{
  public class ShadowBrain : Brain
  {
    public string symbolPrefix;

    public Tag species;

    override protected void OnPrefabInit()
    {
      base.OnPrefabInit();
      GetComponent<Navigator>().SetAbilities(new CreaturePathFinderAbilities(GetComponent<Navigator>()));
    }

    override public void UpdateBrain()
    {
      //base.UpdateBrain();

      if (IsRunning())
      {
        UpdateChores();
      }
    }

    private void UpdateChores()
    {
      if (this.HasTag(GameTags.PreventChoreInterruption))
      {
        return;
      }
      Chore.Precondition.Context context = default(Chore.Precondition.Context);
      if (FindBetterChore(ref context))
      {
        if (this.HasTag(GameTags.PerformingWorkRequest))
        {
          Trigger((int)GameHashes.ChoreInterrupt);
        }
        else
        {
          GetComponent<ChoreDriver>().SetChore(context);
        }
      }
    }

    private bool FindBetterChore(ref Chore.Precondition.Context context)
    {
      return GetComponent<ChoreConsumer>().FindNextChore(ref context);
    }
    /*
    protected override void OnCmpDisable()
    {
      base.OnCmpDisable();
    }

    protected override void OnCleanUp()
    {
      Components.Brains.Remove(this);
    }*/
  }
}

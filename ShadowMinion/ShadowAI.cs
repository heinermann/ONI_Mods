using UnityEngine;

namespace Heinermann.ShadowMinion
{
  public class ShadowAi : GameStateMachine<ShadowAi, ShadowAi.Instance>
  {
    public new class Instance : GameInstance
    {
      public Instance(IStateMachineTarget master)
          : base(master)
      {
        ChoreConsumer component = GetComponent<ChoreConsumer>();
        component.AddUrge(Db.Get().Urges.EmoteIdle);
      }

      public void RefreshUserMenu()
      {
        Game.Instance.userMenu.Refresh(base.master.gameObject);
      }
    }

    public State alive;

    public State dead;

    public override void InitializeStates(out BaseState default_state)
    {
      default_state = alive;
      base.serializable = true;
      root.ToggleStateMachine((Instance smi) => new DeathMonitor.Instance(smi.master, new DeathMonitor.Def()));
      alive.TagTransition(GameTags.Dead, dead)
        .ToggleStateMachine((Instance smi) => new ThoughtGraph.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new IdleMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new RationMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new CalorieMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new LightMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new FallMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new ThreatMonitor.Instance(smi.master, new ThreatMonitor.Def()))
        .ToggleStateMachine((Instance smi) => new WoundMonitor.Instance(smi.master))
        .ToggleStateMachine((Instance smi) => new MoveToLocationMonitor.Instance(smi.master))
        ;

      dead.ToggleStateMachine((Instance smi) => new FallWhenDeadMonitor.Instance(smi.master)).ToggleBrain("dead")
        .Enter("DropStorage", delegate (Instance smi)
        {
          smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
        });
    }
  }
}

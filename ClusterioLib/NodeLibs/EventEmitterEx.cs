using Events;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ClusterioLib.NodeLibs
{
  public static class EventEmitterArgs {
    public static readonly EventEmitterEventArgs Empty = new EventEmitterEventArgs(new object[] { });
  }

  public class EventEmitterWaitHandle : EventWaitHandle
  {
    public EventEmitterEventArgs EventArgs { get; set; }

    [SecurityCritical]
    public EventEmitterWaitHandle() : base(false, EventResetMode.ManualReset) { }
  }

  public class EventEmitterEx : EventEmitter
  {
    public void On(string emitter, EventEmitterEventHandler handler)
    {
      AddListener(emitter, handler);
    }
    
    public void Once(string emitter, EventEmitterEventHandler handler)
    {
      AddOneTimeListener(emitter, handler);
    }

    public Task<EventEmitterEventArgs> Once(string emitter)
    {
      var wait = new EventEmitterWaitHandle();

      Once(emitter, (sender, args) => {
        wait.EventArgs = args;
        wait.Set();
      });

      return new Task<EventEmitterEventArgs>(() => {
        wait.WaitOne();
        return wait.EventArgs;
      });
    }
  }
}

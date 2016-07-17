namespace React.iOS

open System
open System.Reactive.Concurrency
open System.Reactive.Disposables;
open CoreFoundation;
open Foundation;

module private Scheduler =
  type private NSRunLoopScheduler () =
    interface IScheduler with
      member this.Schedule ((state: 'TState), (action: Func<IScheduler, 'TState, IDisposable>)) =
        let innerDisp = new SingleAssignmentDisposable()

        if NSThread.Current = NSThread.MainThread then 
          action.Invoke (this, state)
        else
          DispatchQueue.MainQueue.DispatchAsync(
            fun () -> if not innerDisp.IsDisposed then innerDisp.Disposable <- action.Invoke(this, state)
          )
          innerDisp :> IDisposable

      member this.Schedule ((state: 'TState), (dueTime: TimeSpan), (action: Func<IScheduler, 'TState, IDisposable>)) =
        let mutable innerDisp = Disposable.Empty
        let mutable isCancelled = false
                        
        let timer = 
          NSTimer.CreateScheduledTimer(
            dueTime, 
            fun _ -> if not isCancelled then innerDisp <- action.Invoke (this, state);
          )
        
        Disposable.Create(
          fun () ->
            isCancelled <- true
            timer.Invalidate()
            innerDisp.Dispose()
        )

      member this.Schedule (state, dueTime, action) =
        let this = (this :> IScheduler) 

        if dueTime <= this.Now 
        then this.Schedule (state, action)
        else this.Schedule (state, dueTime - this.Now, action)

      member this.Now with get () = DateTimeOffset.Now

  let mainLoopScheduler = new NSRunLoopScheduler() :> IScheduler
            
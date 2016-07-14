namespace React.Android.Widget

open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Views
open Android.Widget
open FSharp.Control.Reactive
open ImmutableCollections
open React
open React.Android
open React.Android.Views
open System
open System.Reactive.Disposables
open System.Reactive.Subjects
open System.Runtime.CompilerServices

type IListViewProps =
  inherit IViewGroupProps

type ListViewProps =
  {
    // View Props
    accessibilityLiveRegion: int
    alpha: float32
    backgroundColor: Color
    backgroundTintMode: PorterDuff.Mode
    clickable: bool
    contentDescription: string
    contextClickable: bool
    elevation: Single
    enabled: bool
    filterTouchesWhenObscured: bool
    focusable: bool
    focusableInTouchMode: bool
    hapticFeedbackEnabled: bool
    horizontalFadingEdgeEnabled: bool
    horizontalScrollBarEnabled: bool
    id: int
    layoutParameters: ViewGroup.LayoutParams
    onClick: Func<unit, unit>
    onCreateContextMenu: Func<IContextMenu, IContextMenuContextMenuInfo, unit>
    onDrag: Func<DragEvent, bool>
    onGenericMotion: Func<MotionEvent, bool>
    onHover: Func<MotionEvent, bool>
    onKey: Func<Keycode, KeyEvent, bool>
    onLongClick: Func<unit, bool>
    onSystemUiVisibilityChange: Func<StatusBarVisibility, unit>
    onTouch: Func<MotionEvent, bool>
    padding: Padding
    pivot: Pivot
    requestFocus: IObservable<unit>
    scrollBarSize: int
    scrollBarStyle: ScrollbarStyles
    selected: bool
    soundEffectsEnabled: bool
    systemUiVisibility: StatusBarVisibility
    textAlignment: TextAlignment
    textDirection: TextDirection
    transitionName: string
    translation: Translation
    verticalFadingEdgeEnabled: bool
    verticalScrollBarEnabled: bool
    verticalScrollbarPosition: ScrollbarPosition
    visibility: ViewStates
  }

  interface IListViewProps with
    // View Props
    member this.AccessibilityLiveRegion = this.accessibilityLiveRegion
    member this.Alpha = this.alpha
    member this.BackgroundColor = this.backgroundColor
    member this.BackgroundTintMode = this.backgroundTintMode
    member this.Clickable = this.clickable
    member this.ContentDescription = this.contentDescription
    member this.ContextClickable = this.contextClickable
    member this.Elevation = this.elevation
    member this.Enabled = this.enabled
    member this.FilterTouchesWhenObscured = this.filterTouchesWhenObscured
    member this.Focusable = this.focusable
    member this.FocusableInTouchMode = this.focusableInTouchMode
    member this.HapticFeedbackEnabled = this.hapticFeedbackEnabled
    member this.HorizontalFadingEdgeEnabled = this.horizontalFadingEdgeEnabled
    member this.HorizontalScrollBarEnabled = this.horizontalScrollBarEnabled
    member this.Id = this.id
    member this.LayoutParameters = this.layoutParameters
    member this.OnClick = this.onClick
    member this.OnCreateContextMenu = this.onCreateContextMenu
    member this.OnDrag = this.onDrag
    member this.OnGenericMotion = this.onGenericMotion
    member this.OnHover = this.onHover
    member this.OnKey = this.onKey
    member this.OnLongClick = this.onLongClick
    member this.OnSystemUiVisibilityChange = this.onSystemUiVisibilityChange
    member this.OnTouch = this.onTouch
    member this.Padding = this.padding
    member this.Pivot = this.pivot
    member this.RequestFocus = this.requestFocus
    member this.ScrollBarSize = this.scrollBarSize
    member this.ScrollBarStyle = this.scrollBarStyle
    member this.Selected = this.selected
    member this.SoundEffectsEnabled = this.soundEffectsEnabled
    member this.SystemUiVisibility = this.systemUiVisibility
    member this.TextAlignment = this.textAlignment
    member this.TextDirection = this.textDirection
    member this.TransitionName = this.transitionName
    member this.Translation = this.translation
    member this.VerticalFadingEdgeEnabled = this.verticalFadingEdgeEnabled
    member this.VerticalScrollBarEnabled = this.verticalScrollBarEnabled
    member this.VerticalScrollbarPosition = this.verticalScrollbarPosition
    member this.Visibility = this.visibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ListViewProps =
  let internal defaultProps = {
    // View Props
    accessibilityLiveRegion = ViewGroupProps.Default.accessibilityLiveRegion
    alpha = ViewGroupProps.Default.alpha
    backgroundColor = ViewGroupProps.Default.backgroundColor
    backgroundTintMode = ViewGroupProps.Default.backgroundTintMode
    clickable = ViewGroupProps.Default.clickable
    contentDescription = ViewGroupProps.Default.contentDescription
    contextClickable = ViewGroupProps.Default.contextClickable
    elevation = ViewGroupProps.Default.elevation
    enabled = ViewGroupProps.Default.enabled
    filterTouchesWhenObscured = ViewGroupProps.Default.filterTouchesWhenObscured
    focusable = ViewGroupProps.Default.focusable
    focusableInTouchMode = ViewGroupProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = ViewGroupProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = ViewGroupProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = ViewGroupProps.Default.horizontalScrollBarEnabled
    id = ViewGroupProps.Default.id
    layoutParameters = ViewGroupProps.Default.layoutParameters
    onClick = ViewGroupProps.Default.onClick
    onCreateContextMenu = ViewGroupProps.Default.onCreateContextMenu
    onDrag = ViewGroupProps.Default.onDrag
    onGenericMotion = ViewGroupProps.Default.onGenericMotion
    onHover = ViewGroupProps.Default.onHover
    onKey = ViewGroupProps.Default.onKey
    onLongClick = ViewGroupProps.Default.onLongClick
    onSystemUiVisibilityChange = ViewGroupProps.Default.onSystemUiVisibilityChange
    onTouch = ViewGroupProps.Default.onTouch
    padding = ViewGroupProps.Default.padding
    pivot = ViewGroupProps.Default.pivot
    requestFocus = ViewGroupProps.Default.requestFocus
    scrollBarSize = ViewGroupProps.Default.scrollBarSize
    scrollBarStyle = ViewGroupProps.Default.scrollBarStyle
    selected = ViewGroupProps.Default.selected
    soundEffectsEnabled = ViewGroupProps.Default.soundEffectsEnabled
    systemUiVisibility =  ViewGroupProps.Default.systemUiVisibility
    textAlignment = ViewGroupProps.Default.textAlignment
    textDirection = ViewGroupProps.Default.textDirection
    transitionName = ViewGroupProps.Default.transitionName
    translation = ViewGroupProps.Default.translation
    verticalFadingEdgeEnabled = ViewGroupProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = ViewGroupProps.Default.verticalScrollBarEnabled
    verticalScrollbarPosition = ViewGroupProps.Default.verticalScrollbarPosition
    visibility = ViewGroupProps.Default.visibility
  }

type ListViewProps with
  static member Default = ListViewProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ListView =
  [<Sealed>]
  type ReactListAdapter (children: IObservable<IImmutableMap<int, ReactDOMNode>>,
                         createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<View>,
                         onError: Exception -> unit) as this =
    inherit BaseAdapter()

    let updateNativeView = ReactView.updateNativeView createNativeView

    let childrenSubject =
      new BehaviorSubject<IImmutableVector<int * ReactNativeDOMNode>>(ImmutableVector.empty ())

    let childrenSubscription =
      childrenSubject
      |> Observable.iter (ignore >> this.NotifyDataSetChanged)
      |> Observable.subscribeWithError ignore onError

    let children =
      children
      |> Observable.flatmap (
          Seq.map (
            fun (key, dom) ->
              ReactDom.observe dom |> Observable.map (fun nativeDomNode -> (key, nativeDomNode))
          )
          >> Observable.combineLatestSeq
          >> Observable.map (
              Seq.filter (fun (key, node) -> node |> Option.isSome)
              >> Seq.map (fun (key, node) -> (key, node |> Option.get))
              >> ImmutableVector.create
            )
        )
      |> Observable.observeOn Scheduler.mainLoopScheduler
      |> Observable.subscribeObserver childrenSubject

    let reactViewCache = new ConditionalWeakTable<View, IReactView<View>>()

    override this.Count = childrenSubject.Value.Count

    override this.GetItem position = null

    override this.GetItemId position =
      childrenSubject.Value.Item position |> (fun (key, _) -> key) |> int64

    override this.GetView (position: int, convertView: View, parent: ViewGroup) =
      let (key, nativeDomNode) = childrenSubject.Value.Item position

      let reactView =
        match reactViewCache.TryGetValue(convertView) with
        | (true, reactView) ->  updateNativeView (Some reactView) (Some nativeDomNode)
        | _ ->  updateNativeView None (Some nativeDomNode)

      match reactView with
      | Some reactView ->
           reactViewCache.Remove reactView.View |> ignore
           reactViewCache.Add (reactView.View, reactView)
           reactView.View
      | _  -> failwith "Something went wrong"

    override this.HasStableIds = true

    interface IListAdapter with
      member this.AreAllItemsEnabled () = true
      member this.IsEnabled index = true

    interface IDisposable with
      member this.Dispose () =
        childrenSubscription.Dispose ()

  let private name = typeof<ListView>.FullName

  let private listViewAdapterCache =
    new ConditionalWeakTable<ListView, IImmutableMap<string, IReactView<View>>>()

  let setProps (onError: Exception -> unit) (view: View) (props: IListViewProps) =
    let view = (view :?> ListView)
    ViewGroup.setProps onError view props

  let private createView (context: Context): AndroidViewCreator =
    let createView
        (onError: Exception -> unit)
        (createView: string (* view name *) -> obj (* initialProps *) -> IReactView<View>) =

      let view = new ListView (context)
      let setChildrenSubject = new Subject<IImmutableMap<int, ReactDOMNode>>()

      let adapter = new ReactListAdapter (setChildrenSubject, createView, onError)
      view.Adapter <- adapter

      let onDispose () =
        setChildrenSubject.OnCompleted()
        ()

      ReactView.createView name (view :> View) (setProps onError view) setChildrenSubject.OnNext onDispose

    createView

  let viewProvider = (name, createView)
  let internal reactComponent
      (props: ListViewProps)
      (children: IImmutableMap<int, ReactElement>) = ReactNativeElement {
    Name = name
    Props = props
    Children = children
  }

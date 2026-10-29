using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

    // {{ AOT assemblies
    public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
    {
        "AYellowpaper.SerializedCollections.dll",
        "CatGaming.Clr.Runtime.dll",
        "DOTween.dll",
        "Newtonsoft.Json.dll",
        "System.Core.dll",
        "System.dll",
        "UniTask.Addressables.dll",
        "UniTask.dll",
        "Unity.Addressables.dll",
        "Unity.ResourceManager.dll",
        "UnityEngine.AndroidJNIModule.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.JSONSerializeModule.dll",
        "mscorlib.dll",
    };
    // }}

    // {{ constraint implement type
    // }} 

    // {{ AOT generic types
    // AYellowpaper.SerializedCollections.SerializedDictionary<object,object>
    // AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>
    // Cysharp.Threading.Tasks.AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.<>c<long>
    // Cysharp.Threading.Tasks.AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.<>c<object>
    // Cysharp.Threading.Tasks.AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<long>
    // Cysharp.Threading.Tasks.AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<object>
    // Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource.<>c<long>
    // Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource.<>c<object>
    // Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<long>
    // Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Core.Popup.PopupCommand.<CreateInternal>d__19,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Core.Popup.PopupCommand.<HideAll>d__20>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Core.Popup.PopupCommand.<Show>d__13>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Core.Popup.PopupCommand.<ShowOverlay>d__14>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<MainModule.<FirstLoad>d__12>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Script.Util.Addressloader.<CheckUpdateAddressables>d__2>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Script.Util.Addressloader.<DebugServerHash>d__4>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Script.Util.Addressloader.<DownloadByLabel>d__10>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<Script.Util.Addressloader.<Load>d__1<object>,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.ImageExtention.<DownloadString>d__1,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Core.Popup.PopupCommand.<CreateInternal>d__19,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Core.Popup.PopupCommand.<HideAll>d__20>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Core.Popup.PopupCommand.<Show>d__13>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Core.Popup.PopupCommand.<ShowOverlay>d__14>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<MainModule.<FirstLoad>d__12>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Script.Util.Addressloader.<CheckUpdateAddressables>d__2>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Script.Util.Addressloader.<DebugServerHash>d__4>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Script.Util.Addressloader.<DownloadByLabel>d__10>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<Script.Util.Addressloader.<Load>d__1<object>,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.ImageExtention.<DownloadString>d__1,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>,object>
    // Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
    // Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<object>
    // Cysharp.Threading.Tasks.ITaskPoolNode<object>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,long>>
    // Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,object>>
    // Cysharp.Threading.Tasks.IUniTaskSource<long>
    // Cysharp.Threading.Tasks.IUniTaskSource<object>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,long>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,object>>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<long>
    // Cysharp.Threading.Tasks.UniTask.Awaiter<object>
    // Cysharp.Threading.Tasks.UniTask.CanceledResultSource<long>
    // Cysharp.Threading.Tasks.UniTask.CanceledResultSource<object>
    // Cysharp.Threading.Tasks.UniTask.ExceptionResultSource<long>
    // Cysharp.Threading.Tasks.UniTask.ExceptionResultSource<object>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,long>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,object>>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<long>
    // Cysharp.Threading.Tasks.UniTask.IsCanceledSource<object>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,long>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,object>>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<long>
    // Cysharp.Threading.Tasks.UniTask.MemoizeSource<object>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,long>>
    // Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,object>>
    // Cysharp.Threading.Tasks.UniTask<long>
    // Cysharp.Threading.Tasks.UniTask<object>
    // Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Cysharp.Threading.Tasks.AsyncUnit>
    // Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<long>
    // Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<object>
    // DG.Tweening.Core.DOGetter<UnityEngine.Color>
    // DG.Tweening.Core.DOGetter<UnityEngine.Quaternion>
    // DG.Tweening.Core.DOGetter<UnityEngine.Vector2>
    // DG.Tweening.Core.DOGetter<UnityEngine.Vector3>
    // DG.Tweening.Core.DOGetter<float>
    // DG.Tweening.Core.DOGetter<int>
    // DG.Tweening.Core.DOGetter<object>
    // DG.Tweening.Core.DOSetter<UnityEngine.Color>
    // DG.Tweening.Core.DOSetter<UnityEngine.Quaternion>
    // DG.Tweening.Core.DOSetter<UnityEngine.Vector2>
    // DG.Tweening.Core.DOSetter<UnityEngine.Vector3>
    // DG.Tweening.Core.DOSetter<float>
    // DG.Tweening.Core.DOSetter<int>
    // DG.Tweening.Core.DOSetter<object>
    // DG.Tweening.Core.TweenerCore<UnityEngine.Quaternion,UnityEngine.Vector3,DG.Tweening.Plugins.Options.QuaternionOptions>
    // DG.Tweening.Core.TweenerCore<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>
    // DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<DG.Tweening.Color2,DG.Tweening.Color2,DG.Tweening.Plugins.Options.ColorOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Color,UnityEngine.Color,DG.Tweening.Plugins.Options.ColorOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Quaternion,UnityEngine.Vector3,DG.Tweening.Plugins.Options.QuaternionOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Rect,UnityEngine.Rect,DG.Tweening.Plugins.Options.RectOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.Options.VectorOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.Vector3ArrayOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector4,UnityEngine.Vector4,DG.Tweening.Plugins.Options.VectorOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<double,double,DG.Tweening.Plugins.Options.NoOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<float,float,DG.Tweening.Plugins.Options.FloatOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<int,int,DG.Tweening.Plugins.Options.NoOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<long,long,DG.Tweening.Plugins.Options.NoOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<object,object,DG.Tweening.Plugins.Options.NoOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<object,object,DG.Tweening.Plugins.Options.StringOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<uint,uint,DG.Tweening.Plugins.Options.UintOptions>
    // DG.Tweening.Plugins.Core.ABSTweenPlugin<ulong,ulong,DG.Tweening.Plugins.Options.NoOptions>
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<long>>
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // DelegateList<float>
    // System.Action<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Action<DG.Tweening.Plugins.Options.PathOptions,object,UnityEngine.Quaternion,object>
    // System.Action<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Action<System.DateTime>
    // System.Action<UnityEngine.Color>
    // System.Action<UnityEngine.GradientAlphaKey>
    // System.Action<UnityEngine.GradientColorKey>
    // System.Action<UnityEngine.Quaternion>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<long>>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Action<UnityEngine.UIVertex>
    // System.Action<UnityEngine.Vector2>
    // System.Action<UnityEngine.Vector3>
    // System.Action<byte>
    // System.Action<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Action<float>
    // System.Action<int>
    // System.Action<object,int>
    // System.Action<object,object,int>
    // System.Action<object,object,object,byte>
    // System.Action<object,object,object>
    // System.Action<object,object>
    // System.Action<object>
    // System.Buffers.ArrayPool<int>
    // System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<int>
    // System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<int>
    // System.Buffers.TlsOverPerCoreLockedStacksArrayPool<int>
    // System.ByReference<UnityEngine.jvalue>
    // System.ByReference<int>
    // System.ByReference<ushort>
    // System.Collections.Generic.ArraySortHelper<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Color>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.GradientColorKey>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Quaternion>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.UIVertex>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
    // System.Collections.Generic.ArraySortHelper<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.ArraySortHelper<float>
    // System.Collections.Generic.ArraySortHelper<int>
    // System.Collections.Generic.ArraySortHelper<object>
    // System.Collections.Generic.Comparer<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,long>>
    // System.Collections.Generic.Comparer<System.ValueTuple<byte,object>>
    // System.Collections.Generic.Comparer<UnityEngine.Color>
    // System.Collections.Generic.Comparer<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.Comparer<UnityEngine.GradientColorKey>
    // System.Collections.Generic.Comparer<UnityEngine.Quaternion>
    // System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.Comparer<UnityEngine.UIVertex>
    // System.Collections.Generic.Comparer<UnityEngine.Vector2>
    // System.Collections.Generic.Comparer<UnityEngine.Vector3>
    // System.Collections.Generic.Comparer<byte>
    // System.Collections.Generic.Comparer<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.Comparer<float>
    // System.Collections.Generic.Comparer<int>
    // System.Collections.Generic.Comparer<long>
    // System.Collections.Generic.Comparer<object>
    // System.Collections.Generic.ComparisonComparer<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,long>>
    // System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,object>>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.Color>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.GradientColorKey>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.Vector2>
    // System.Collections.Generic.ComparisonComparer<UnityEngine.Vector3>
    // System.Collections.Generic.ComparisonComparer<byte>
    // System.Collections.Generic.ComparisonComparer<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.ComparisonComparer<float>
    // System.Collections.Generic.ComparisonComparer<int>
    // System.Collections.Generic.ComparisonComparer<long>
    // System.Collections.Generic.ComparisonComparer<object>
    // System.Collections.Generic.Dictionary.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection<object,object>
    // System.Collections.Generic.Dictionary<int,object>
    // System.Collections.Generic.Dictionary<object,int>
    // System.Collections.Generic.Dictionary<object,object>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,long>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,object>>
    // System.Collections.Generic.EqualityComparer<byte>
    // System.Collections.Generic.EqualityComparer<int>
    // System.Collections.Generic.EqualityComparer<long>
    // System.Collections.Generic.EqualityComparer<object>
    // System.Collections.Generic.HashSet.Enumerator<object>
    // System.Collections.Generic.HashSet<object>
    // System.Collections.Generic.HashSetEqualityComparer<object>
    // System.Collections.Generic.ICollection<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ICollection<UnityEngine.Color>
    // System.Collections.Generic.ICollection<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.ICollection<UnityEngine.GradientColorKey>
    // System.Collections.Generic.ICollection<UnityEngine.Quaternion>
    // System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ICollection<UnityEngine.UIVertex>
    // System.Collections.Generic.ICollection<UnityEngine.Vector2>
    // System.Collections.Generic.ICollection<UnityEngine.Vector3>
    // System.Collections.Generic.ICollection<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.ICollection<float>
    // System.Collections.Generic.ICollection<int>
    // System.Collections.Generic.ICollection<object>
    // System.Collections.Generic.IComparer<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IComparer<UnityEngine.Color>
    // System.Collections.Generic.IComparer<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.IComparer<UnityEngine.GradientColorKey>
    // System.Collections.Generic.IComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.IComparer<UnityEngine.Vector2>
    // System.Collections.Generic.IComparer<UnityEngine.Vector3>
    // System.Collections.Generic.IComparer<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.IComparer<float>
    // System.Collections.Generic.IComparer<int>
    // System.Collections.Generic.IComparer<object>
    // System.Collections.Generic.IDictionary<object,object>
    // System.Collections.Generic.IEnumerable<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<UnityEngine.Color>
    // System.Collections.Generic.IEnumerable<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.IEnumerable<UnityEngine.GradientColorKey>
    // System.Collections.Generic.IEnumerable<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IEnumerable<UnityEngine.UIVertex>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerable<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.IEnumerable<float>
    // System.Collections.Generic.IEnumerable<int>
    // System.Collections.Generic.IEnumerable<object>
    // System.Collections.Generic.IEnumerator<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerator<UnityEngine.Color>
    // System.Collections.Generic.IEnumerator<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.IEnumerator<UnityEngine.GradientColorKey>
    // System.Collections.Generic.IEnumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IEnumerator<UnityEngine.UIVertex>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerator<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.IEnumerator<float>
    // System.Collections.Generic.IEnumerator<int>
    // System.Collections.Generic.IEnumerator<object>
    // System.Collections.Generic.IEqualityComparer<int>
    // System.Collections.Generic.IEqualityComparer<object>
    // System.Collections.Generic.IList<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IList<UnityEngine.Color>
    // System.Collections.Generic.IList<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.IList<UnityEngine.GradientColorKey>
    // System.Collections.Generic.IList<UnityEngine.Quaternion>
    // System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IList<UnityEngine.UIVertex>
    // System.Collections.Generic.IList<UnityEngine.Vector2>
    // System.Collections.Generic.IList<UnityEngine.Vector3>
    // System.Collections.Generic.IList<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.IList<float>
    // System.Collections.Generic.IList<int>
    // System.Collections.Generic.IList<object>
    // System.Collections.Generic.IReadOnlyCollection<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.IReadOnlyList<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.KeyValuePair<int,object>
    // System.Collections.Generic.KeyValuePair<object,int>
    // System.Collections.Generic.KeyValuePair<object,object>
    // System.Collections.Generic.LinkedList.Enumerator<object>
    // System.Collections.Generic.LinkedList<object>
    // System.Collections.Generic.LinkedListNode<object>
    // System.Collections.Generic.List.Enumerator<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Color>
    // System.Collections.Generic.List.Enumerator<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.List.Enumerator<UnityEngine.GradientColorKey>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.List.Enumerator<UnityEngine.UIVertex>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
    // System.Collections.Generic.List.Enumerator<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.List.Enumerator<float>
    // System.Collections.Generic.List.Enumerator<int>
    // System.Collections.Generic.List.Enumerator<object>
    // System.Collections.Generic.List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.List<UnityEngine.Color>
    // System.Collections.Generic.List<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.List<UnityEngine.GradientColorKey>
    // System.Collections.Generic.List<UnityEngine.Quaternion>
    // System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.List<UnityEngine.UIVertex>
    // System.Collections.Generic.List<UnityEngine.Vector2>
    // System.Collections.Generic.List<UnityEngine.Vector3>
    // System.Collections.Generic.List<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.List<float>
    // System.Collections.Generic.List<int>
    // System.Collections.Generic.List<object>
    // System.Collections.Generic.ObjectComparer<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,long>>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,object>>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Color>
    // System.Collections.Generic.ObjectComparer<UnityEngine.GradientAlphaKey>
    // System.Collections.Generic.ObjectComparer<UnityEngine.GradientColorKey>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ObjectComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
    // System.Collections.Generic.ObjectComparer<byte>
    // System.Collections.Generic.ObjectComparer<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.Generic.ObjectComparer<float>
    // System.Collections.Generic.ObjectComparer<int>
    // System.Collections.Generic.ObjectComparer<long>
    // System.Collections.Generic.ObjectComparer<object>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,long>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,object>>
    // System.Collections.Generic.ObjectEqualityComparer<byte>
    // System.Collections.Generic.ObjectEqualityComparer<int>
    // System.Collections.Generic.ObjectEqualityComparer<long>
    // System.Collections.Generic.ObjectEqualityComparer<object>
    // System.Collections.Generic.Queue.Enumerator<object>
    // System.Collections.Generic.Queue<object>
    // System.Collections.Generic.Stack.Enumerator<object>
    // System.Collections.Generic.Stack<object>
    // System.Collections.Generic.ValueListBuilder<int>
    // System.Collections.ObjectModel.ReadOnlyCollection<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Color>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.GradientAlphaKey>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.GradientColorKey>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Quaternion>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.UIVertex>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
    // System.Collections.ObjectModel.ReadOnlyCollection<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Collections.ObjectModel.ReadOnlyCollection<float>
    // System.Collections.ObjectModel.ReadOnlyCollection<int>
    // System.Collections.ObjectModel.ReadOnlyCollection<object>
    // System.Comparison<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Comparison<System.ValueTuple<byte,long>>
    // System.Comparison<System.ValueTuple<byte,object>>
    // System.Comparison<UnityEngine.Color>
    // System.Comparison<UnityEngine.GradientAlphaKey>
    // System.Comparison<UnityEngine.GradientColorKey>
    // System.Comparison<UnityEngine.Quaternion>
    // System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Comparison<UnityEngine.UIVertex>
    // System.Comparison<UnityEngine.Vector2>
    // System.Comparison<UnityEngine.Vector3>
    // System.Comparison<byte>
    // System.Comparison<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Comparison<float>
    // System.Comparison<int>
    // System.Comparison<long>
    // System.Comparison<object>
    // System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
    // System.Func<System.DateTime,byte>
    // System.Func<System.Threading.Tasks.VoidTaskResult>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Func<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Func<System.ValueTuple<byte,long>>
    // System.Func<System.ValueTuple<byte,object>>
    // System.Func<UnityEngine.Color,byte>
    // System.Func<UnityEngine.GradientAlphaKey,UnityEngine.Color>
    // System.Func<UnityEngine.GradientAlphaKey,byte>
    // System.Func<UnityEngine.GradientColorKey,UnityEngine.Color>
    // System.Func<UnityEngine.GradientColorKey,byte>
    // System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // System.Func<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Func<UnityEngine.Vector3,byte>
    // System.Func<byte>
    // System.Func<float,byte>
    // System.Func<int,object,byte>
    // System.Func<int>
    // System.Func<long>
    // System.Func<object,System.Threading.Tasks.VoidTaskResult>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Func<object,System.ValueTuple<byte,long>>
    // System.Func<object,System.ValueTuple<byte,object>>
    // System.Func<object,UnityEngine.Vector3>
    // System.Func<object,byte>
    // System.Func<object,float>
    // System.Func<object,long>
    // System.Func<object,object,object>
    // System.Func<object,object>
    // System.Func<object>
    // System.IProgress<float>
    // System.Linq.Buffer<UnityEngine.Color>
    // System.Linq.Buffer<UnityEngine.Vector3>
    // System.Linq.Buffer<object>
    // System.Linq.Enumerable.<SelectManyIterator>d__17<object,object>
    // System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.Iterator<UnityEngine.Color>
    // System.Linq.Enumerable.Iterator<UnityEngine.GradientAlphaKey>
    // System.Linq.Enumerable.Iterator<UnityEngine.GradientColorKey>
    // System.Linq.Enumerable.Iterator<UnityEngine.Vector3>
    // System.Linq.Enumerable.Iterator<float>
    // System.Linq.Enumerable.Iterator<object>
    // System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereArrayIterator<object>
    // System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereEnumerableIterator<UnityEngine.Color>
    // System.Linq.Enumerable.WhereEnumerableIterator<UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereEnumerableIterator<float>
    // System.Linq.Enumerable.WhereEnumerableIterator<object>
    // System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereListIterator<object>
    // System.Linq.Enumerable.WhereSelectArrayIterator<UnityEngine.GradientAlphaKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectArrayIterator<UnityEngine.GradientColorKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectArrayIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,float>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<UnityEngine.GradientAlphaKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<UnityEngine.GradientColorKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,float>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
    // System.Linq.Enumerable.WhereSelectListIterator<UnityEngine.GradientAlphaKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectListIterator<UnityEngine.GradientColorKey,UnityEngine.Color>
    // System.Linq.Enumerable.WhereSelectListIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectListIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectListIterator<object,float>
    // System.Linq.Enumerable.WhereSelectListIterator<object,object>
    // System.Nullable<System.DateTime>
    // System.Nullable<UnityEngine.Vector3>
    // System.Nullable<int>
    // System.Nullable<ushort>
    // System.Predicate<AYellowpaper.SerializedCollections.SerializedKeyValuePair<object,object>>
    // System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Predicate<UnityEngine.Color>
    // System.Predicate<UnityEngine.GradientAlphaKey>
    // System.Predicate<UnityEngine.GradientColorKey>
    // System.Predicate<UnityEngine.Quaternion>
    // System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Predicate<UnityEngine.UIVertex>
    // System.Predicate<UnityEngine.Vector2>
    // System.Predicate<UnityEngine.Vector3>
    // System.Predicate<catgamingmodule.DailyChallenge.DailyChallengeService.Day>
    // System.Predicate<float>
    // System.Predicate<int>
    // System.Predicate<object>
    // System.ReadOnlySpan<UnityEngine.jvalue>
    // System.ReadOnlySpan<int>
    // System.ReadOnlySpan<ushort>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<long>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<long>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<long>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<long>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<object>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<long>
    // System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<object>
    // System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.TaskAwaiter<long>
    // System.Runtime.CompilerServices.TaskAwaiter<object>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,long>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,object>>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<long>
    // System.Runtime.CompilerServices.ValueTaskAwaiter<object>
    // System.Span<UnityEngine.jvalue>
    // System.Span<int>
    // System.Span<ushort>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<long>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<object>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.Sources.IValueTaskSource<long>
    // System.Threading.Tasks.Sources.IValueTaskSource<object>
    // System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.Task<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.Task<long>
    // System.Threading.Tasks.Task<object>
    // System.Threading.Tasks.TaskCompletionSource<long>
    // System.Threading.Tasks.TaskCompletionSource<object>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<long>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
    // System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.TaskFactory<long>
    // System.Threading.Tasks.TaskFactory<object>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<long>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<object>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<long>
    // System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<object>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,long>>
    // System.Threading.Tasks.ValueTask<System.ValueTuple<byte,object>>
    // System.Threading.Tasks.ValueTask<long>
    // System.Threading.Tasks.ValueTask<object>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
    // System.ValueTuple<byte,System.ValueTuple<byte,long>>
    // System.ValueTuple<byte,System.ValueTuple<byte,object>>
    // System.ValueTuple<byte,long>
    // System.ValueTuple<byte,object>
    // UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass79_0<object>
    // UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene,int>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<long>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<long>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<long>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<long>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<long>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
    // UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
    // UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
    // UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
    // UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
    // }}

    public void RefMethods()
    {
        // object CatGaming.Clr.ClrContent.Load<object>(string)
        // Cysharp.Threading.Tasks.UniTask.Awaiter<long> Cysharp.Threading.Tasks.AddressablesAsyncExtensions.GetAwaiter<long>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<long>)
        // Cysharp.Threading.Tasks.UniTask.Awaiter<object> Cysharp.Threading.Tasks.AddressablesAsyncExtensions.GetAwaiter<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
        // Cysharp.Threading.Tasks.UniTask<long> Cysharp.Threading.Tasks.AddressablesAsyncExtensions.ToUniTask<long>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<long>,System.IProgress<float>,Cysharp.Threading.Tasks.PlayerLoopTiming,System.Threading.CancellationToken,bool,bool)
        // Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.AddressablesAsyncExtensions.ToUniTask<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>,System.IProgress<float>,Cysharp.Threading.Tasks.PlayerLoopTiming,System.Threading.CancellationToken,bool,bool)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Core.Popup.PopupCommand.<HideAll>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Core.Popup.PopupCommand.<HideAll>d__20&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Core.Popup.PopupCommand.<Show>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Core.Popup.PopupCommand.<Show>d__13&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Core.Popup.PopupCommand.<ShowOverlay>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Core.Popup.PopupCommand.<ShowOverlay>d__14&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Script.Util.Addressloader.<CheckUpdateAddressables>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Script.Util.Addressloader.<CheckUpdateAddressables>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Script.Util.Addressloader.<DownloadByLabel>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Script.Util.Addressloader.<DownloadByLabel>d__10&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,Script.Util.Addressloader.<DownloadByLabel>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,Script.Util.Addressloader.<DownloadByLabel>d__10&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Core.Popup.PopupCommand.<Show>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Core.Popup.PopupCommand.<Show>d__13&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Core.Popup.PopupCommand.<ShowOverlay>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Core.Popup.PopupCommand.<ShowOverlay>d__14&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,MainModule.<FirstLoad>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,MainModule.<FirstLoad>d__12&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Script.Util.Addressloader.<CheckUpdateAddressables>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Script.Util.Addressloader.<CheckUpdateAddressables>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter,Script.Util.Addressloader.<DebugServerHash>d__4>(Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter&,Script.Util.Addressloader.<DebugServerHash>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter,catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4>(Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter&,catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter,catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2>(Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter&,catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Core.Popup.PopupCommand.<CreateInternal>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Core.Popup.PopupCommand.<CreateInternal>d__19&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Core.Popup.PopupCommand.<CreateInternal>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Core.Popup.PopupCommand.<CreateInternal>d__19&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Script.Util.Addressloader.<Load>d__1<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Script.Util.Addressloader.<Load>d__1<object>&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,catgamingmodule.Tool.ImageExtention.<DownloadString>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,catgamingmodule.Tool.ImageExtention.<DownloadString>d__1&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter,catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9>(Cysharp.Threading.Tasks.UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter&,catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Core.Popup.PopupCommand.<HideAll>d__20>(Core.Popup.PopupCommand.<HideAll>d__20&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21>(Core.Popup.PopupCommand.<HideAndClearDictionaryAsync>d__21&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Core.Popup.PopupCommand.<Show>d__13>(Core.Popup.PopupCommand.<Show>d__13&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Core.Popup.PopupCommand.<ShowOverlay>d__14>(Core.Popup.PopupCommand.<ShowOverlay>d__14&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<MainModule.<FirstLoad>d__12>(MainModule.<FirstLoad>d__12&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Script.Util.Addressloader.<CheckUpdateAddressables>d__2>(Script.Util.Addressloader.<CheckUpdateAddressables>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Script.Util.Addressloader.<DebugServerHash>d__4>(Script.Util.Addressloader.<DebugServerHash>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Script.Util.Addressloader.<DownloadByLabel>d__10>(Script.Util.Addressloader.<DownloadByLabel>d__10&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40>(catgamingmodule.DailyChallenge.DailyChallengeView.<Show>d__40&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0>(catgamingmodule.Tool.Download.DownloadManager.<DownloadImage>d__0&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4>(catgamingmodule.Tool.Download.DownloadManager.<_downloadImage>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3>(catgamingmodule.Tool.IOSAudioSessionHandler.<ReapplyAfterFocus>d__3&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0>(catgamingmodule.Tool.ImageExtention.<DownloadImage>d__0&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2>(catgamingmodule.Tool.ImageExtention.<_downloadImage>d__2&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.Popup.PopupItem.<Hide>d__5>(catgamingmodule.Tool.Popup.PopupItem.<Hide>d__5&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<catgamingmodule.Tool.Popup.PopupItem.<Show>d__4>(catgamingmodule.Tool.Popup.PopupItem.<Show>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<Core.Popup.PopupCommand.<CreateInternal>d__19>(Core.Popup.PopupCommand.<CreateInternal>d__19&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<Script.Util.Addressloader.<Load>d__1<object>>(Script.Util.Addressloader.<Load>d__1<object>&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4>(catgamingmodule.DailyChallenge.DailyChallengeService.<Show>d__4&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.Social.AppleSignin.<Login>d__6>(catgamingmodule.Social.AppleSignin.<Login>d__6&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3>(catgamingmodule.Tool.Download.DownloadManager.<DownloadText>d__3&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.Tool.ImageExtention.<DownloadString>d__1>(catgamingmodule.Tool.ImageExtention.<DownloadString>d__1&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9>(catgamingmodule.Tool.ImageExtention.<GetTextAsync>d__9&)
        // System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>>(catgamingmodule.Tool.Popup.PopupDatabase.<GetAsync>d__8<object>&)
        // Cysharp.Threading.Tasks.UniTask<long> Cysharp.Threading.Tasks.UniTask.FromCanceled<long>(System.Threading.CancellationToken)
        // Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTask.FromCanceled<object>(System.Threading.CancellationToken)
        // Cysharp.Threading.Tasks.UniTask<long> Cysharp.Threading.Tasks.UniTask.FromException<long>(System.Exception)
        // Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTask.FromException<object>(System.Exception)
        // Cysharp.Threading.Tasks.UniTask<long> Cysharp.Threading.Tasks.UniTask.FromResult<long>(long)
        // Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTask.FromResult<object>(object)
        // DG.Tweening.Core.TweenerCore<UnityEngine.Color,UnityEngine.Color,DG.Tweening.Plugins.Options.ColorOptions> DG.Tweening.Core.Extensions.Blendable<UnityEngine.Color,UnityEngine.Color,DG.Tweening.Plugins.Options.ColorOptions>(DG.Tweening.Core.TweenerCore<UnityEngine.Color,UnityEngine.Color,DG.Tweening.Plugins.Options.ColorOptions>)
        // object DG.Tweening.Core.Extensions.SetSpecialStartupMode<object>(object,DG.Tweening.Core.Enums.SpecialStartupMode)
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions> DG.Tweening.Core.TweenManager.GetTweener<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>()
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions> DG.Tweening.Core.TweenManager.GetTweener<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>()
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions> DG.Tweening.DOTween.ApplyTo<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>(DG.Tweening.Core.DOGetter<UnityEngine.Vector2>,DG.Tweening.Core.DOSetter<UnityEngine.Vector2>,UnityEngine.Vector2,float,DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>)
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions> DG.Tweening.DOTween.ApplyTo<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>(DG.Tweening.Core.DOGetter<UnityEngine.Vector3>,DG.Tweening.Core.DOSetter<UnityEngine.Vector3>,object,float,DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>)
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions> DG.Tweening.DOTween.To<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>(DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>,DG.Tweening.Core.DOGetter<UnityEngine.Vector2>,DG.Tweening.Core.DOSetter<UnityEngine.Vector2>,UnityEngine.Vector2,float)
        // DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions> DG.Tweening.DOTween.To<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>(DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>,DG.Tweening.Core.DOGetter<UnityEngine.Vector3>,DG.Tweening.Core.DOSetter<UnityEngine.Vector3>,object,float)
        // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions> DG.Tweening.Plugins.Core.PluginsManager.GetDefaultPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>()
        // DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions> DG.Tweening.Plugins.Core.PluginsManager.GetDefaultPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>()
        // object DG.Tweening.TweenExtensions.Pause<object>(object)
        // object DG.Tweening.TweenExtensions.Play<object>(object)
        // object DG.Tweening.TweenSettingsExtensions.OnKill<object>(object,DG.Tweening.TweenCallback)
        // object DG.Tweening.TweenSettingsExtensions.OnStart<object>(object,DG.Tweening.TweenCallback)
        // object DG.Tweening.TweenSettingsExtensions.OnUpdate<object>(object,DG.Tweening.TweenCallback)
        // object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
        // object DG.Tweening.TweenSettingsExtensions.SetLoops<object>(object,int,DG.Tweening.LoopType)
        // object DG.Tweening.TweenSettingsExtensions.SetRelative<object>(object)
        // object DG.Tweening.TweenSettingsExtensions.SetTarget<object>(object,object)
        // object DG.Tweening.TweenSettingsExtensions.SetUpdate<object>(object,DG.Tweening.UpdateType)
        // bool DG.Tweening.Tweener.Setup<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>(DG.Tweening.Core.TweenerCore<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>,DG.Tweening.Core.DOGetter<UnityEngine.Vector2>,DG.Tweening.Core.DOSetter<UnityEngine.Vector2>,UnityEngine.Vector2,float,DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector2,UnityEngine.Vector2,DG.Tweening.Plugins.CircleOptions>)
        // bool DG.Tweening.Tweener.Setup<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>(DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>,DG.Tweening.Core.DOGetter<UnityEngine.Vector3>,DG.Tweening.Core.DOSetter<UnityEngine.Vector3>,object,float,DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,object,DG.Tweening.Plugins.Options.PathOptions>)
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
        // object System.Activator.CreateInstance<object>()
        // object[] System.Array.Empty<object>()
        // int System.Array.IndexOf<object>(object[],object)
        // int System.Array.IndexOfImpl<object>(object[],object,int,int)
        // bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,int)
        // object System.Linq.Enumerable.ElementAt<object>(System.Collections.Generic.IEnumerable<object>,int)
        // System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
        // object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
        // object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // object System.Linq.Enumerable.Last<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Color> System.Linq.Enumerable.Select<UnityEngine.GradientAlphaKey,UnityEngine.Color>(System.Collections.Generic.IEnumerable<UnityEngine.GradientAlphaKey>,System.Func<UnityEngine.GradientAlphaKey,UnityEngine.Color>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Color> System.Linq.Enumerable.Select<UnityEngine.GradientColorKey,UnityEngine.Color>(System.Collections.Generic.IEnumerable<UnityEngine.GradientColorKey>,System.Func<UnityEngine.GradientColorKey,UnityEngine.Color>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Select<UnityEngine.Vector3,UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>,System.Func<UnityEngine.Vector3,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Select<object,UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<object>,System.Func<object,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Select<object,float>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectMany<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectManyIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
        // bool System.Linq.Enumerable.SequenceEqual<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEnumerable<int>)
        // bool System.Linq.Enumerable.SequenceEqual<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEqualityComparer<int>)
        // float System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
        // UnityEngine.Color[] System.Linq.Enumerable.ToArray<UnityEngine.Color>(System.Collections.Generic.IEnumerable<UnityEngine.Color>)
        // UnityEngine.Vector3[] System.Linq.Enumerable.ToArray<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>)
        // object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.HashSet<object> System.Linq.Enumerable.ToHashSet<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.HashSet<object> System.Linq.Enumerable.ToHashSet<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEqualityComparer<object>)
        // System.Collections.Generic.List<UnityEngine.Vector3> System.Linq.Enumerable.ToList<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>)
        // System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
        // System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Color> System.Linq.Enumerable.Iterator<UnityEngine.GradientAlphaKey>.Select<UnityEngine.Color>(System.Func<UnityEngine.GradientAlphaKey,UnityEngine.Color>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Color> System.Linq.Enumerable.Iterator<UnityEngine.GradientColorKey>.Select<UnityEngine.Color>(System.Func<UnityEngine.GradientColorKey,UnityEngine.Color>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Iterator<UnityEngine.Vector3>.Select<UnityEngine.Vector3>(System.Func<UnityEngine.Vector3,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Iterator<object>.Select<UnityEngine.Vector3>(System.Func<object,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Iterator<object>.Select<float>(System.Func<object,float>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForCompletion>d__10&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForElapsedLoops>d__13&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForKill>d__12&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForPosition>d__14&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForRewind>d__11&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15>(DG.Tweening.DOTweenModuleUnityVersion.<AsyncWaitForStart>d__15&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,MainModule.<Start>d__25>(Cysharp.Threading.Tasks.UniTask.Awaiter&,MainModule.<Start>d__25&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,UniTaskExtension.<WrapErrors>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,UniTaskExtension.<WrapErrors>d__2&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,catgamingmodule.Tool.Popup.PopupItem.<HideTest>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,catgamingmodule.Tool.Popup.PopupItem.<HideTest>d__6&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,UniTaskExtension.<WrapErrors>d__3<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,UniTaskExtension.<WrapErrors>d__3<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,UniTaskExtension.<WrapErrors>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,UniTaskExtension.<WrapErrors>d__4&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,UniTaskExtension.<WrapErrors>d__5<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,UniTaskExtension.<WrapErrors>d__5<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<MainModule.<Init>d__26>(MainModule.<Init>d__26&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<MainModule.<Start>d__25>(MainModule.<Start>d__25&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<UniTaskExtension.<WrapErrors>d__2>(UniTaskExtension.<WrapErrors>d__2&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<UniTaskExtension.<WrapErrors>d__3<object>>(UniTaskExtension.<WrapErrors>d__3<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<UniTaskExtension.<WrapErrors>d__4>(UniTaskExtension.<WrapErrors>d__4&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<UniTaskExtension.<WrapErrors>d__5<object>>(UniTaskExtension.<WrapErrors>d__5<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<catgamingmodule.Tool.Popup.PopupItem.<HideTest>d__6>(catgamingmodule.Tool.Popup.PopupItem.<HideTest>d__6&)
        // object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
        // System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
        // System.Void UnityEngine.AddressableAssets.Addressables.Release<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetAsync<object>(object)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
        // byte UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
        // object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
        // byte UnityEngine.AndroidJavaObject.Call<byte>(string,object[])
        // object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
        // object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
        // byte UnityEngine.AndroidJavaObject.FromJavaArray<byte>(System.IntPtr)
        // object UnityEngine.AndroidJavaObject.FromJavaArray<object>(System.IntPtr)
        // object UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<object>(System.IntPtr)
        // object UnityEngine.AndroidJavaObject.GetStatic<object>(string)
        // byte UnityEngine.AndroidJavaObject._Call<byte>(System.IntPtr,object[])
        // byte UnityEngine.AndroidJavaObject._Call<byte>(string,object[])
        // object UnityEngine.AndroidJavaObject._Call<object>(System.IntPtr,object[])
        // object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
        // object UnityEngine.AndroidJavaObject._CallStatic<object>(System.IntPtr,object[])
        // object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
        // object UnityEngine.AndroidJavaObject._GetStatic<object>(System.IntPtr)
        // object UnityEngine.AndroidJavaObject._GetStatic<object>(string)
        // object UnityEngine.Component.GetComponent<object>()
        // object[] UnityEngine.Component.GetComponents<object>()
        // bool UnityEngine.Component.TryGetComponent<object>(object&)
        // object UnityEngine.GameObject.AddComponent<object>()
        // object UnityEngine.GameObject.GetComponent<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
        // object[] UnityEngine.GameObject.GetComponents<object>()
        // bool UnityEngine.GameObject.TryGetComponent<object>(object&)
        // object UnityEngine.JsonUtility.FromJson<object>(string)
        // object[] UnityEngine.Object.FindObjectsByType<object>(UnityEngine.FindObjectsInactive,UnityEngine.FindObjectsSortMode)
        // object[] UnityEngine.Object.FindObjectsOfType<object>()
        // object UnityEngine.Object.Instantiate<object>(object)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Convert<object>()
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
        // object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.ProvideResource<object>(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
        // object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
        // object UnityEngine.Resources.Load<object>(string)
        // object[] UnityEngine.Resources.LoadAll<object>(string)
        // object UnityEngine.ScriptableObject.CreateInstance<object>()
        // byte UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
        // object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
        // string UnityEngine._AndroidJNIHelper.GetSignature<byte>(object[])
        // string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
    }
}

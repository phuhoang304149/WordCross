using CatGaming.Clr;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace WordCross.Gameplay
{
    public sealed class WordCrossGameEntry : IClrEntry
    {
        private const string FirstScene = "Assets/_Scenes/Home.unity";

        public void Initialize(GameObject host)
        {
            Addressables.LoadSceneAsync(FirstScene, LoadSceneMode.Single).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError("[WordCross] Home scene load failed from Addressables: " + handle.OperationException, host);
            };
        }
    }
}

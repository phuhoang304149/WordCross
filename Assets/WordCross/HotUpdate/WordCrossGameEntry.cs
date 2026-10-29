using CatGaming.Clr;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WordCross.Gameplay
{
    public sealed class WordCrossGameEntry : IClrEntry
    {
        private const string FirstScene = "Home";

        public void Initialize(GameObject host)
        {
            SceneManager.LoadScene(FirstScene);
        }
    }
}

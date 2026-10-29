using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace WordCross.Editor
{
    // Gán icon app (Android adaptive/round/legacy + iOS) và cấu hình splash từ Assets/WordCross/Branding vào PlayerSettings.
    // Hàm static chỉ là điểm vào cho Unity (menu / -executeMethod), phần việc nằm ở hàm instance.
    public class BrandingSetup
    {
        private const string Root = "Assets/WordCross/Branding/";
        private static readonly Color Teal = new Color32(0x12, 0xb5, 0xa6, 0xff);

        [MenuItem("WordCross/Branding/Apply Icons And Splash")]
        public static void Apply()
        {
            new BrandingSetup().DoApply();
        }

        private void DoApply()
        {
            var full = Load("icon-1024");
            var foreground = Load("icon-fg-432");
            var background = Load("icon-bg-432");

            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.backgroundColor = Teal;
            PlayerSettings.SplashScreen.logos = Array.Empty<PlayerSettings.SplashScreenLogo>();

            AssignKinds(NamedBuildTarget.Android, BuildTargetGroup.Android, full, foreground, background);
            AssignKinds(NamedBuildTarget.iOS, BuildTargetGroup.iOS, full, foreground, background);
            AssetDatabase.SaveAssets();
        }

        private Texture2D Load(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(Root + name + ".png");
        }

        private void AssignKinds(NamedBuildTarget target, BuildTargetGroup group, Texture2D full, Texture2D foreground, Texture2D background)
        {
            foreach (var kind in PlayerSettings.GetSupportedIconKindsForPlatform(group))
            {
                var icons = PlayerSettings.GetPlatformIcons(target, kind);
                var layered = false;
                foreach (var icon in icons)
                {
                    if (icon.maxLayerCount == 2) layered = true;
                }
                foreach (var icon in icons)
                {
                    if (layered) icon.SetTextures(background, foreground);
                    else if (kind.ToString() != "Round") icon.SetTextures(full);
                }
                PlayerSettings.SetPlatformIcons(target, kind, icons);
                Debug.Log("[BRANDING] " + target.TargetName + " kind=" + kind + " icons=" + icons.Length + " layered=" + layered);
            }
        }
    }
}

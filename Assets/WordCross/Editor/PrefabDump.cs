using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // Xuất trạng thái prefab ra văn bản để so trước/sau khi chuyển Text -> TMP: script thiếu, tham chiếu serialized,
    // UnityEvent persistent call, kích thước preferred của từng text. Tên kiểu Text/TextMeshProUGUI được chuẩn hoá thành TXT.
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class PrefabDump
    {
        // -executeMethod WordCross.Editor.PrefabDump.DumpFromArgs -dumpAssets "a.prefab;b.prefab" -dumpOut file.txt
        public static void DumpFromArgs()
        {
            var args = Environment.GetCommandLineArgs();
            string assets = null;
            string output = null;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-dumpAssets") assets = args[i + 1];
                if (args[i] == "-dumpOut") output = args[i + 1];
            }
            new PrefabDump().Dump(assets.Split(';'), output);
        }

        public void Dump(string[] paths, string output)
        {
            var sb = new StringBuilder();
            foreach (var path in paths)
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                sb.Append("== ").Append(path).Append('\n');
                sb.Append("missingScripts=").Append(CountMissing(root)).Append('\n');
                foreach (var component in root.GetComponentsInChildren<Component>(true))
                {
                    if (component == null) continue;
                    DumpComponent(sb, root, component);
                }
                PrefabUtility.UnloadPrefabContents(root);
            }
            File.WriteAllText(output, sb.ToString());
        }

        private int CountMissing(GameObject root)
        {
            var count = 0;
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                count += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
            return count;
        }

        private void DumpComponent(StringBuilder sb, GameObject root, Component component)
        {
            var type = Normalize(component.GetType().Name);
            if (component is Text || component is TMP_Text)
            {
                var text = (Text)(component is Text ? component : null);
                var tmp = component as TMP_Text;
                sb.Append(Path(root, component)).Append(" [").Append(type).Append("] text=\"")
                    .Append(text != null ? text.text : tmp.text).Append("\" size=")
                    .Append(text != null ? text.fontSize : tmp.fontSize).Append(" preferred=")
                    .Append(text != null ? text.preferredWidth : tmp.preferredWidth).Append('x')
                    .Append(text != null ? text.preferredHeight : tmp.preferredHeight).Append('\n');
            }
            else if (!(component is Transform))
            {
                sb.Append(Path(root, component)).Append(" [").Append(type).Append("]\n");
            }

            var serialized = new SerializedObject(component);
            var property = serialized.GetIterator();
            while (property.NextVisible(true))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var target = property.objectReferenceValue;
                    if (property.propertyPath == "m_Script") continue;
                    var description = target == null ? "null" : target is Component targetComponent
                        ? Path(root, targetComponent) + " [" + Normalize(target.GetType().Name) + "]"
                        : target.GetType().Name + ":" + target.name;
                    if (target != null && property.propertyPath.EndsWith("m_Target"))
                        sb.Append("    ").Append(property.propertyPath).Append(" -> ").Append(description).Append('\n');
                    else if (!(component is Text) && !(component is TMP_Text))
                        sb.Append("    ").Append(property.propertyPath).Append(" -> ").Append(description).Append('\n');
                }
                else if (property.propertyPath.EndsWith("m_MethodName"))
                {
                    sb.Append("    ").Append(property.propertyPath).Append(" = ").Append(property.stringValue).Append('\n');
                }
            }
        }

        private string Normalize(string typeName)
        {
            return typeName == "Text" || typeName == "TextMeshProUGUI" ? "TXT" : typeName;
        }

        private string Path(GameObject root, Component component)
        {
            var names = new List<string>();
            for (var t = component.transform; t != null; t = t.parent) names.Insert(0, t.name);
            return string.Join("/", names);
        }
    }
}

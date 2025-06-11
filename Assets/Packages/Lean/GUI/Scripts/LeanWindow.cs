using Lean.Common;
using UnityEditor;
using UnityEngine;

namespace Lean.Gui
{
	/// <summary>This component works just like LeanToggle, but it registers itself with the LeanWindowCloser.
	/// This allows the window to be automatically closed if you press the LeanWindowCloser.CloseKey.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanWindow")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Window")]
	public class LeanWindow : LeanToggle
	{
		protected override void TurnOnNow()
		{
			base.TurnOnNow();

			if (Closeable)
			{
				LeanWindowCloser.Register(this);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanWindow), true)]
	public class LeanWindow_Editor : LeanInspector<LeanWindow>
	{
		protected override void DrawInspector()
		{
			Draw("Closeable", "Indicates whether this Window should be closeable by the LeanWindowCloser when it is the topmost Window");

			if (Draw("on", "This lets you change the current toggle state of this UI element.") == true)
			{
				Each(t => t.On = serializedObject.FindProperty("on").boolValue, true);
			}

			if (Draw("turnOffSiblings", "If you enable this, then any sibling GameObjects (i.e. same parent GameObject) will automatically be turned off. This allows you to make radio boxes, or force only one panel to show at a time, etc.") == true)
			{
				Each(t => t.TurnOffSiblings = serializedObject.FindProperty("turnOffSiblings").boolValue, true);
			}

			EditorGUILayout.Separator();

			Draw("onTransitions", "This allows you to perform a transition when this toggle turns on. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the LeanGraphicColor (Graphic.color Transition) component can be used to change the color.\n\nNOTE: Any transitions you perform here should be reverted in the <b>Off Transitions</b> setting using a matching transition component.");
			Draw("offTransitions", "This allows you to perform a transition when this toggle turns off. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the LeanGraphicColor (Graphic.color Transition) component can be used to change the color.");

			EditorGUILayout.Separator();

			Draw("onOn");
			Draw("onOff");

		}
	}
}
#endif
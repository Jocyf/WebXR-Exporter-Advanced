using UnityEngine;
using UnityEditor;
using System.Reflection;

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target MonoBehaviour.
/// 
/// </summary>

//  Use: 
// [InspectorButton("OnButtonClicked")]
// public bool clickMe;
//
// private void OnButtonClicked()
// {
// 	Debug.Log("Clicked!");
// }
// 

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
	private MethodInfo _eventMethodInfo = null;

	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
		//Rect buttonRect = new Rect(position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, position.y, inspectorButtonAttribute.ButtonWidth, position.height);
		Rect buttonRect = new Rect(	position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, 
									position.y + (position.height - inspectorButtonAttribute.ButtonHeight) * 0.5f, 
									inspectorButtonAttribute.ButtonWidth, 
									inspectorButtonAttribute.ButtonHeight);
		if (GUI.Button(buttonRect, label.text))
		{
			System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
			string eventName = inspectorButtonAttribute.MethodName;

			if (_eventMethodInfo == null)
				_eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (_eventMethodInfo != null)
				_eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
			else
				Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
		}
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
		return base.GetPropertyHeight(prop, label) * inspectorButtonAttribute.ButtonHeight * 0.075f;
	}
}

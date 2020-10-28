using UnityEngine;
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

[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public static float kDefaultButtonWidth = 80;
	public static float kDefaultButtonHeight = 30;

	public readonly string MethodName;

	private float _buttonWidth = kDefaultButtonWidth;
	public float ButtonWidth
	{
		get { return _buttonWidth; }
		set { _buttonWidth = value; }
	}

	private float _buttonHeight = kDefaultButtonHeight;
	public float ButtonHeight
	{
		get { return _buttonHeight; }
		set { _buttonHeight = value; }
	}

	public InspectorButtonAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}
}


/////////////////////////////////////////////////////////////////////////////////
//
//	Moga_ControllerManager.cs
//	Unity MOGA Plugin for Android/WP8
//	© 2013 Bensussen Deutsch and Associates, Inc. All rights reserved.
//
//	description:	Provides MOGA Controller functionality within Unity.
//					This Script creates the MOGA Controller Manager and maps
//					the Controls through Unitys' InputManager.
//
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
public class Moga_ControllerManager : MonoBehaviour 
{
	[HideInInspector]
	public KeyCode 				p1ButtonA = 			KeyCode.Joystick1Button14,
								p1ButtonB = 			KeyCode.Joystick1Button13,
								p1ButtonX = 			KeyCode.Joystick1Button15,
								p1ButtonY = 			KeyCode.Joystick1Button12,
								p1ButtonL1 = 			KeyCode.Joystick1Button8,
								p1ButtonR1 = 			KeyCode.Joystick1Button9,
								p1ButtonSelect = 		KeyCode.Joystick1Button1,
								p1ButtonStart = 		KeyCode.Joystick1Button0,
								p1ButtonL3 = 			KeyCode.Joystick1Button2,
								p1ButtonR3 = 			KeyCode.Joystick1Button3,
								p1ButtonL2 = 			KeyCode.Joystick1Button10,
								p1ButtonR2 = 			KeyCode.Joystick1Button11,
								p1ButtonDPadUp = 		KeyCode.Joystick1Button6,
								p1ButtonDPadDown = 		KeyCode.Joystick1Button7,
								p1ButtonDPadLeft = 		KeyCode.Joystick1Button4,
								p1ButtonDPadRight = 	KeyCode.Joystick1Button5;
	[HideInInspector]
	public string 				p1AxisHorizontal = 		"Horizontal",
								p1AxisVertical = 		"Vertical",
								p1AxisLookHorizontal = 	"LookHorizontal",
								p1AxisLookVertical = 	"LookVertical",
								p1AxisL2 = 				"L2",
								p1AxisR2 = 				"R2";
	[HideInInspector]
	public static Moga_Controller 		sMogaController;
	private bool 						mFocused;
	// Create the MOGA Controller Manager -------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void Awake ()
	{
		if (sMogaController == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject controller = Moga_Controller.getInstance (activity);
			sMogaController = new Moga_Controller (activity, controller);
			sMogaController.init ();
		}
	}
	// Pause/Resume the MOGA Controller Manager -------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void OnApplicationFocus (bool focus)
	{
		mFocused = focus;
		if (mFocused)
		{
			if (sMogaController != null)
			{
				sMogaController.onResume ();
			}
		}
	}
	// Exit the MOGA Controller Manager ---------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void OnDestroy ()
	{
		if (sMogaController != null)
		{
			sMogaController.onExit ();
		}
	}
	// Returns whether a MOGA Controller is connected ------------------------------------------------
	// -----------------------------------------------------------------------------------------------
	public bool isControllerConnected()
	{
		if (sMogaController.getState(Moga_Controller.STATE_CONNECTION) == Moga_Controller.ACTION_CONNECTED)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	// Returns the number of MOGA Controllers connected ----------------------------------------------
	// -----------------------------------------------------------------------------------------------
	public int numberOfConnectedControllers()
	{
		return sMogaController.getInfo(Moga_Controller.INFO_KNOWN_DEVICE_COUNT);
	}
	// Returns the number of MOGA Controllers connected ----------------------------------------------
	// -----------------------------------------------------------------------------------------------
	public bool isControllerMogaPro()
	{
		if (sMogaController.getState(Moga_Controller.STATE_CURRENT_PRODUCT_VERSION) == Moga_Controller.ACTION_VERSION_MOGAPRO)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
#elif UNITY_WP8
public class Moga_ControllerManager : MonoBehaviour 
{		
	[HideInInspector]
	public KeyCode 				p1ButtonA = 			KeyCode.Joystick1Button14,
								p1ButtonB = 			KeyCode.Joystick1Button13,
								p1ButtonX = 			KeyCode.Joystick1Button15,
								p1ButtonY = 			KeyCode.Joystick1Button12,
								p1ButtonL1 = 			KeyCode.Joystick1Button8,
								p1ButtonR1 = 			KeyCode.Joystick1Button9,
								p1ButtonSelect = 		KeyCode.Joystick1Button1,
								p1ButtonStart = 		KeyCode.Joystick1Button0,
								p1ButtonL3 = 			KeyCode.Joystick1Button2,
								p1ButtonR3 = 			KeyCode.Joystick1Button3,
								p1ButtonL2 = 			KeyCode.Joystick1Button10,
								p1ButtonR2 = 			KeyCode.Joystick1Button11,
								p1ButtonDPadUp = 		KeyCode.Joystick1Button6,
								p1ButtonDPadDown = 		KeyCode.Joystick1Button7,
								p1ButtonDPadLeft = 		KeyCode.Joystick1Button4,
								p1ButtonDPadRight = 	KeyCode.Joystick1Button5;
	[HideInInspector]
	public string 				p1AxisHorizontal 		= "P1 Horizontal",
								p1AxisVertical 			= "P1 Vertical",
								p1AxisLookHorizontal 	= "P1 LookHorizontal",
								p1AxisLookVertical 		= "P1 LookVertical",
								p1AxisL2 				= "P1 L2",
								p1AxisR2 				= "P1 R2";
	[HideInInspector]
	public bool 						wp8poll = true, armlib = true;
	// Create the MOGA Controller Manager -------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void Awake ()
	{
		DontDestroyOnLoad(transform.gameObject);
	}
	// Pause/Resume the MOGA Controller Manager -------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void OnApplicationFocus (bool focus)
	{
	}
	// Exit the MOGA Controller Manager ---------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	void OnDestroy ()
	{
	}
	// Returns whether a MOGA Controller is connected ------------------------------------------------
	// -----------------------------------------------------------------------------------------------
	public bool isControllerConnected()
	{
		if (Moga_Controller.getState(Moga_Controller.STATE_CONNECTION) == Moga_Controller.ACTION_CONNECTED)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	// Returns the number of MOGA Controllers connected ----------------------------------------------
	// -----------------------------------------------------------------------------------------------
	public int numberOfConnectedControllers()
	{
		return Moga_Controller.getInfo(Moga_Controller.INFO_KNOWN_DEVICE_COUNT);
	}
}
#endif
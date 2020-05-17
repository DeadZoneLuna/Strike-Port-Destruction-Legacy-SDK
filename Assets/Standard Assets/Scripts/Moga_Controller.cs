/////////////////////////////////////////////////////////////////////////////////
//
//	Moga_Controller.cs
//	Unity Android/WP8 MOGA Plugin
//	© 2013 Bensussen Deutsch and Associates, Inc. All rights reserved.
//
//	description:	Enables MOGA Controller functionality within Unity.  This 
//					Script is a Bare-bones wrapper class  for iOS/WP8/Android.
//
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_WP8
public static class Moga_Controller
{
	public const int ACTION_DOWN             = 0;
	public const int ACTION_UP               = 1;
	public const int ACTION_FALSE            = 0;
	public const int ACTION_TRUE             = 1;
	public const int ACTION_DISCONNECTED     = 1;
	public const int ACTION_CONNECTED        = 0;
	public const int ACTION_CONNECTING       = 2;
	public const int ACTION_VERSION_MOGA     = 20;
	public const int ACTION_VERSION_MOGAPRO  = 21;
	public const int ACTION_VERSION_MOGATWO  = 22; // HID Compatible
	public const int 
		STATE_CONNECTION                     = 10,
		STATE_POWER_LOW                      = 11,
		STATE_SUPPORTED_PRODUCT_VERSION      = 12,
		STATE_CURRENT_PRODUCT_VERSION        = 13
	;
	public const int
		KEYCODE_BUTTON_A                     = 10,
		KEYCODE_BUTTON_B                     = 11,
		KEYCODE_BUTTON_X                     = 12,
		KEYCODE_BUTTON_Y                     = 13,
		KEYCODE_BUTTON_START                 = 14,
		KEYCODE_BUTTON_SELECT                = 15,
		KEYCODE_BUTTON_L1                    = 16,
		KEYCODE_BUTTON_R1                    = 17,
		KEYCODE_BUTTON_L2                    = 18,                 
		KEYCODE_BUTTON_R2                    = 19,
		KEYCODE_BUTTON_THUMBL                = 20,
		KEYCODE_BUTTON_THUMBR                = 21,
		KEYCODE_DPAD_UP                      = 22,
		KEYCODE_DPAD_DOWN                    = 23,
		KEYCODE_DPAD_LEFT                    = 24,
		KEYCODE_DPAD_RIGHT                   = 25, 
		KEYCODE_SIZE                         = 26
	;
	public const int INFO_KNOWN_DEVICE_COUNT  = 1;
	public const int INFO_ACTIVE_DEVICE_COUNT = 2;
	public const int 
		AXIS_X                               = 90,
		AXIS_Y                               = 91,
		AXIS_Z                               = 92,
		AXIS_RZ                              = 93,
		AXIS_LTRIGGER                        = 94,
		AXIS_RTRIGGER                        = 95,
		AXIS_SIZE                            = 96
	;
	private static Dictionary<int, int> buttonState = new Dictionary<int, int>(); 
	private static Dictionary<int, int> stateState =  new Dictionary<int, int>();
	private static Dictionary<int, float> axisState = new Dictionary<int, float>();
	// POLLING
	// -------------------------------------------------------------------------
	public static void ReceiveButtonState(Dictionary<int, int> _buttonState) 
	{
		buttonState = _buttonState;
	}
	// -------------------------------------------------------------------------
	public static void ReceiveAxisState(Dictionary<int, float> _axisState) 
	{
		axisState = _axisState;
	}
	// -------------------------------------------------------------------------
	public static void ReceiveStateState(Dictionary<int, int> _stateState)
	{
		stateState = _stateState;
	}
	// LISTENING
	// -------------------------------------------------------------------------
	public static void ReceiveSingleButtonState(int keyCode, int action)
	{
		buttonState[keyCode] = action;
	}
	// -------------------------------------------------------------------------
	public static void ReceiveSingleAxisState(int axis, float axisValue) 
	{
		axisState[axis] = axisValue;
	}
	// -------------------------------------------------------------------------
	public static void ReceiveSingleStateState(int state, int stateValue)
	{
		stateState[state] = stateValue;
	}
	// -------------------------------------------------------------------------
	public static float getAxisValue(int axis)
	{
		if(axisState.ContainsKey(axis)) 
		{
			return axisState[axis];
		}
		return 0.0f;
	}
	// -------------------------------------------------------------------------
	public static int getKeyCode(int keyCode)
	{
		if(buttonState.ContainsKey(keyCode)) 
		{
			return buttonState[keyCode];
		}
		return ACTION_UP;
	}
	// -------------------------------------------------------------------------
	public static int getInfo(int info)
	{
		return ACTION_FALSE;
	}
	// -------------------------------------------------------------------------
	public static int getState(int state)
	{
		if(stateState.ContainsKey(state))
		{
			return stateState[state];
		}
		return ACTION_FALSE;
	}
}
#elif UNITY_ANDROID
public class Moga_Controller
{	
	// Actions
	public const int 	ACTION_DOWN 					= 0,
						ACTION_UP 						= 1,
						ACTION_FALSE 					= 0,
						ACTION_TRUE 					= 1,
						ACTION_DISCONNECTED 			= 0,
						ACTION_CONNECTED 				= 1,
						ACTION_CONNECTING				= 2,
						ACTION_VERSION_MOGA				= 0,
						ACTION_VERSION_MOGAPRO		 	= 1;
	// Modes
	public const int	MODE_MULTI_CONTROLLER 			= 1,
  						MODE_HID_TO_MOGA 				= 2;
	// KeyCodes
	public const int 	KEYCODE_DPAD_UP 				= 19,
						KEYCODE_DPAD_DOWN 				= 20,
						KEYCODE_DPAD_LEFT 				= 21,
						KEYCODE_DPAD_RIGHT 				= 22,
						KEYCODE_BUTTON_A 				= 96,
						KEYCODE_BUTTON_B 				= 97,
						KEYCODE_BUTTON_X 				= 99,
						KEYCODE_BUTTON_Y 				= 100,
						KEYCODE_BUTTON_L1 				= 102,
						KEYCODE_BUTTON_R1 				= 103,
						KEYCODE_BUTTON_L2 				= 104,
						KEYCODE_BUTTON_R2 				= 105,
						KEYCODE_BUTTON_THUMBL 			= 106,
						KEYCODE_BUTTON_THUMBR 			= 107,
						KEYCODE_BUTTON_START 			= 108,
						KEYCODE_BUTTON_SELECT 			= 109;
	// Info
	public const int 	INFO_UNKNOWN 					= 0,
						INFO_KNOWN_DEVICE_COUNT 		= 1,
						INFO_ACTIVE_DEVICE_COUNT 		= 2,
						INFO_BLUETOOTH_ENABLED 			= 3;
	// Axis
	public const int 	AXIS_X 							= 0,
						AXIS_Y 							= 1,
						AXIS_Z 							= 11,
						AXIS_RZ 						= 14,
						AXIS_HAT_X 						= 15,
  						AXIS_HAT_Y 						= 16,
						AXIS_LTRIGGER 					= 17,
						AXIS_RTRIGGER 					= 18;
	// State
	public const int 	STATE_UNKNOWN 					= 0,
  						STATE_CONNECTION 				= 1,
  						STATE_POWER_LOW 				= 2,
						STATE_SUPPORTED_PRODUCT_VERSION = 3,
						STATE_CURRENT_PRODUCT_VERSION 	= 4;
	private readonly AndroidJavaObject mController;
	// ------------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public Moga_Controller(AndroidJavaObject activity, AndroidJavaObject controller)
	{
		mController = controller;
	}
	// Gets a new instance of a game controller library object. ---------------------------------------
	// ------------------------------------------------------------------------------------------------
	public static AndroidJavaObject getInstance(AndroidJavaObject activity)
	{
		using (AndroidJavaClass jc = new AndroidJavaClass("com.bda.controller.Controller"))
		{
			return jc.CallStatic<AndroidJavaObject>("getInstance", activity);
		}
	}
	// Initializes the game controller library --------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public bool init()
	{
		return mController.Call<bool>("init");
	}
	// MODES START
	// Allows one or multiple feature modes to be enabled within the controller -----------------------
	// ------------------------------------------------------------------------------------------------
	public void enableModes(int mode)
	{
		mController.Call("enableMode", mode);
	}
	// Checks whether a feature mode is enabled within the controller ---------------------------------
	// ------------------------------------------------------------------------------------------------
	public bool isModeEnabled()
	{
		return mController.Call<bool>("isModeEnabled");
	}
	// Allows one or multiple feature modes to be disabled --------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public void disableModes(int mode)
	{
		mController.Call("disableModes", mode);
	}
	// MODES END
	// CONTROLLER CONNECTION START
	// Allows new connections to be made to unconnected game controllers ------------------------------
	// ------------------------------------------------------------------------------------------------
	public void allowNewConnections()
	{
		mController.Call("allowNewConnections");
	}
	// Returns value indicating whether new connections are allowed------------------------------------
	// ------------------------------------------------------------------------------------------------
	public bool isAllowingNewConnections()
	{
		return mController.Call<bool>("isAllowingNewConnections");
	}
	// Prevents new connections to be made to unconnected game controllers ----------------------------
	// ------------------------------------------------------------------------------------------------
	public void disallowNewConnections()
	{
		mController.Call("disallowNewConnections");
	}
	// CONTROLLER CONNECTION END
	// Retrieve the value of the requested Axis -------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public float getAxisValue(int axis)
	{
		return mController.Call<float>("getAxisValue", axis);
	}
	// Retrieve the value of the requested key code ---------------------------------------------------
	// ------------------------------------------------------------------------------------------------	
	public int getKeyCode(int keyCode)
	{
		return mController.Call<int>("getKeyCode", keyCode);
	}
	// Returns any of the INFO_xxx constants.----------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public int getInfo(int info)
	{
		return mController.Call<int>("getInfo", info);
	}
	// Returns any of the STATE_xxx constants. --------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public int getState(int state)
	{
		return mController.Call<int>("getState", state);
	}
	// Returns the Controller Model -------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public int getControllerModel()
	{
		return mController.Call<int>("getState", STATE_CURRENT_PRODUCT_VERSION);
	}
	// Notifies the game controller library that the host activity has been paused --------------------
	// ------------------------------------------------------------------------------------------------
	public void onPause()
	{
		mController.Call("onPause");
	}
	// Notifies the game controller library that the host activity has been resumed -------------------
	// ------------------------------------------------------------------------------------------------
	public void onResume()
	{
		mController.Call("onResume");
	}
	// Un-initializes the game controller library -----------------------------------------------------
	// ------------------------------------------------------------------------------------------------
	public void onExit()
	{
		mController.Call("onExit");
	}
	/*
	public void setListener(int controllerID, AndroidJavaObject listener, AndroidJavaObject handler)
	{
		// mController.Call("setListener", listener, handler);
		IntPtr methodId = AndroidJNI.GetMethodID(mController.GetRawClass(), "setListener", "(Lcom/bda/controller/ControllerListener;Landroid/os/Handler;)V");
		object[] arguments = new object[] { controllerID, listener, handler };
		AndroidJNI.CallVoidMethod(mController.GetRawObject(), methodId, AndroidJNIHelper.CreateJNIArgArray(arguments));
	}
	*/
}
#endif
/////////////////////////////////////////////////////////////////////////////////
//
//  Input.cs
//  Unity MOGA Plugin for Android
//  © 2013 Bensussen Deutsch and Associates, Inc. All rights reserved.
//
//  description:  Enables MOGA Controller functionality within Unity.  This 
//          Script is a wrapper for Unitys InputManager.
//
/////////////////////////////////////////////////////////////////////////////////

//#define GPS_ENABLED
using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR 
public class Input : MonoBehaviour
{
  public static Action<KeyCode> on_button_up_event   = null;
  public static Action<KeyCode> on_button_down_event = null;

  const float ANALOG_DEADZONE = 0.19f;
  
  private static GameObject mogaManager = null;
  private static Moga_ControllerManager mogaControllerManager = null;

  enum ButtonState
  {
    RELEASED,
    PRESSING,   // initial frame of pressed
    PRESSED,
    RELEASING,  // initial frame of released
  };

  public class Axis
  {
    public string name  = null;
    public int    id    = -1;
    public float  value = 0.0f;
  }

  private static Dictionary<String, Axis>     s_axes              = new Dictionary<String, Axis>();
  private static Dictionary<int, ButtonState> s_buttons           = new Dictionary<int, ButtonState>();
  private static Dictionary<String, int>      s_strings_to_button = new Dictionary<String, int>();
  private static Dictionary<KeyCode, int>     s_codes_to_buttons  = new Dictionary<KeyCode, int>();
  private static Dictionary<int, KeyCode>     s_buttons_to_codes  = new Dictionary<int, KeyCode>();
  
  private static bool s_any_key_down = false;
  private static bool s_any_key      = false;
  // ------------------------------------------------------------------------------------------------
#if UNITY_ANDROID
  void Update ()
  {
      
      
    if (mogaManager == null || Moga_ControllerManager.sMogaController == null) {
      return;
    }
    if (!mogaControllerManager || !mogaControllerManager.isControllerConnected())
        return;
      
    foreach (KeyValuePair<String, Axis> entry in s_axes) {
      Axis axis = entry.Value;
      axis.value = Moga_ControllerManager.sMogaController.getAxisValue(axis.id);
    }

    s_any_key_down = false;
    s_any_key = false;
    
    foreach (KeyValuePair<int, ButtonState> button in s_buttons) {
			
      int action = Moga_ControllerManager.sMogaController.getKeyCode(button.Key);

      switch (button.Value) {
        case ButtonState.RELEASED:
          if (action == Moga_Controller.ACTION_DOWN) {
            s_buttons[button.Key] = ButtonState.PRESSING;
          }
          break;

        case ButtonState.PRESSING:
          if (action == Moga_Controller.ACTION_UP) {
            s_buttons[button.Key] = ButtonState.RELEASING;
          } else {
            s_buttons[button.Key] = ButtonState.PRESSED;
          }
          break;
            
        case ButtonState.PRESSED:          
          s_any_key_down = true;
          s_any_key = true;
          
          if (action == Moga_Controller.ACTION_UP) {
            s_buttons[button.Key] = ButtonState.RELEASING;
          }
          break;

        case ButtonState.RELEASING:                    
          if (action == Moga_Controller.ACTION_DOWN) {
            s_buttons[button.Key] = ButtonState.PRESSING;
          } else {
            s_buttons[button.Key] = ButtonState.RELEASED;
          }
          break;
      }

      if (s_buttons[button.Key] == ButtonState.RELEASING && on_button_up_event != null) {
        on_button_up_event(s_buttons_to_codes[button.Key]);
      } else if (s_buttons[button.Key] == ButtonState.PRESSING && on_button_down_event != null) {
        on_button_down_event(s_buttons_to_codes[button.Key]);
      }
    }
  }
#endif

#if UNITY_WP8
  void Update ()
  {
    if (mogaManager == null) {
      return;
    }

    foreach (KeyValuePair<String, Axis> entry in s_axes) {
      Axis axis = entry.Value;
      axis.value = Moga_Controller.getAxisValue(axis.id);
    }

    s_any_key_down = false;
    s_any_key = false;
    
    foreach (KeyValuePair<int, ButtonState> button in s_buttons) {
			
      int action = Moga_Controller.getKeyCode(button.Key);

      switch (button.Value) {
        case ButtonState.RELEASED:
          if (action == Moga_Controller.ACTION_DOWN) {
            s_buttons[button.Key] = ButtonState.PRESSING;
          }
          break;

        case ButtonState.PRESSING:
          if (action == Moga_Controller.ACTION_UP) {
            s_buttons[button.Key] = ButtonState.RELEASING;
          } else {
            s_buttons[button.Key] = ButtonState.PRESSED;
          }
          break;
            
        case ButtonState.PRESSED:          
          s_any_key_down = true;
          s_any_key = true;
          
          if (action == Moga_Controller.ACTION_UP) {
            s_buttons[button.Key] = ButtonState.RELEASING;
          }
          break;

        case ButtonState.RELEASING:                    
          if (action == Moga_Controller.ACTION_DOWN) {
            s_buttons[button.Key] = ButtonState.PRESSING;
          } else {
            s_buttons[button.Key] = ButtonState.RELEASED;
          }
          break;
      }

      if (s_buttons[button.Key] == ButtonState.RELEASING && on_button_up_event != null) {
        on_button_up_event(s_buttons_to_codes[button.Key]);
      } else if (s_buttons[button.Key] == ButtonState.PRESSING && on_button_down_event != null) {
        on_button_down_event(s_buttons_to_codes[button.Key]);
      }
    }
  }
#endif		
	
  // ------------------------------------------------------------------------------------------------
  public static void RegisterMogaController ()
  {   
    mogaManager = GameObject.Find("MogaControllerManager");
    
    if (mogaManager != null) {
      mogaControllerManager = mogaManager.GetComponent<Moga_ControllerManager>();
    }
    
    if (mogaControllerManager == null) {
      Debug.Log("MOGA Controller Manager could not be found.  Access the MOGA Menu to create one!");
    } else {
      MapController();
    }
  }
  
  // Use the defined MOGAControllerManager Mappings -------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  private static void MapController()
  {
    RegisterInputKey(mogaControllerManager.p1ButtonA,             Moga_Controller.KEYCODE_BUTTON_A);
    RegisterInputKey(mogaControllerManager.p1ButtonB,             Moga_Controller.KEYCODE_BUTTON_B);
    RegisterInputKey(mogaControllerManager.p1ButtonX,             Moga_Controller.KEYCODE_BUTTON_X);
    RegisterInputKey(mogaControllerManager.p1ButtonY,             Moga_Controller.KEYCODE_BUTTON_Y);
    RegisterInputKey(mogaControllerManager.p1ButtonL1,            Moga_Controller.KEYCODE_BUTTON_L1);
    RegisterInputKey(mogaControllerManager.p1ButtonR1,            Moga_Controller.KEYCODE_BUTTON_R1);
    RegisterInputKey(mogaControllerManager.p1ButtonSelect,        Moga_Controller.KEYCODE_BUTTON_SELECT);
    RegisterInputKey(mogaControllerManager.p1ButtonStart,         Moga_Controller.KEYCODE_BUTTON_START);
    RegisterInputKey(mogaControllerManager.p1ButtonL3,            Moga_Controller.KEYCODE_BUTTON_THUMBL);
    RegisterInputKey(mogaControllerManager.p1ButtonR3,            Moga_Controller.KEYCODE_BUTTON_THUMBR);
    RegisterInputKey(mogaControllerManager.p1ButtonL2,            Moga_Controller.KEYCODE_BUTTON_L2);
    RegisterInputKey(mogaControllerManager.p1ButtonR2,            Moga_Controller.KEYCODE_BUTTON_R2);
    RegisterInputKey(mogaControllerManager.p1ButtonDPadUp,        Moga_Controller.KEYCODE_DPAD_UP);
    RegisterInputKey(mogaControllerManager.p1ButtonDPadDown,      Moga_Controller.KEYCODE_DPAD_DOWN);
    RegisterInputKey(mogaControllerManager.p1ButtonDPadLeft,      Moga_Controller.KEYCODE_DPAD_LEFT);
    RegisterInputKey(mogaControllerManager.p1ButtonDPadRight,     Moga_Controller.KEYCODE_DPAD_RIGHT);
    RegisterInputAxis(mogaControllerManager.p1AxisHorizontal,     Moga_Controller.AXIS_X);
    RegisterInputAxis(mogaControllerManager.p1AxisVertical,       Moga_Controller.AXIS_Y);
    RegisterInputAxis(mogaControllerManager.p1AxisLookHorizontal, Moga_Controller.AXIS_Z);
    RegisterInputAxis(mogaControllerManager.p1AxisLookVertical,   Moga_Controller.AXIS_RZ);
    RegisterInputAxis(mogaControllerManager.p1AxisL2,             Moga_Controller.AXIS_LTRIGGER);
    RegisterInputAxis(mogaControllerManager.p1AxisR2,             Moga_Controller.AXIS_RTRIGGER);
  }   
  
  // Map the String to an Axis. ---------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  private static void RegisterInputAxis (String name, int id)
  {
    Axis axis;
    if (s_axes.TryGetValue(name, out axis) == false) {
      axis = new Axis();
      s_axes[name] = axis;
    }

    axis.name = name;
    axis.id = id;
    axis.value = 0.0f;
  }
  
  // Map the String to a button. --------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  private static void RegisterInputButton (String name, int button)
  { 
    // Has the String already been added?  If Not...
    if (!s_strings_to_button.ContainsKey(name)) {
      s_strings_to_button.Add(name, button); // Keep Record String was Added
      s_buttons.Add(button, ButtonState.RELEASED); // Create Moga Button State
    }
  }
  
  // Map the KeyCode to a button. -------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  private static void RegisterInputKey (KeyCode code, int buttonID)
  {
    // Has the KeyCode already been added?  If Not...
    if (!s_codes_to_buttons.ContainsKey(code)) {
      s_codes_to_buttons.Add(code, buttonID);         // Keep Record String was Added
      s_buttons_to_codes.Add(buttonID, code);
      s_buttons.Add(buttonID, ButtonState.RELEASED);  // Create Moga Button State
    }
  }

  // ----------------------------------------------
  public static bool IsSupported
  {
    get { return true; }   
  }
  
  // ----------------------------------------------
  public static Vector3 acceleration 
  {
    get { return UnityEngine.Input.acceleration; }
  }

  // ----------------------------------------------
  public static int accelerationEventCount 
  {
    get { return UnityEngine.Input.accelerationEventCount; }
  }

  // ----------------------------------------------
  public static bool anyKey 
  {
    get{ return s_any_key || UnityEngine.Input.anyKey; }
  }

  // ----------------------------------------------
  public static bool anyKeyDown 
  {
    get{ return s_any_key_down || UnityEngine.Input.anyKeyDown; }
  }

  // ----------------------------------------------
  public static Compass compass 
  {
    get { return UnityEngine.Input.compass; }
  }

  // ----------------------------------------------
  public static string compositionString 
  {
    get { return UnityEngine.Input.compositionString; }
  }

  // ----------------------------------------------
  public static Vector2 compositionCursorPos 
  {
    get { return UnityEngine.Input.compositionCursorPos; }
  }

  // ----------------------------------------------
  public static DeviceOrientation deviceOrientation 
  {
    get { return UnityEngine.Input.deviceOrientation; }
  }

  // ----------------------------------------------
  public static Gyroscope gyro 
  {
    get { return UnityEngine.Input.gyro; }
  }

  // ----------------------------------------------
  public static IMECompositionMode imeCompositionMode 
  {
    get { return UnityEngine.Input.imeCompositionMode; }
    set{ UnityEngine.Input.imeCompositionMode = value; }
  }

  // ----------------------------------------------
  public static bool imeIsSelected 
  {
    get { return UnityEngine.Input.imeIsSelected; }
  }

  // ----------------------------------------------
  public static string inputString 
  {
    get { return UnityEngine.Input.inputString; }
  }

  // ----------------------------------------------
  public static Vector3 mousePosition 
  {
    get { return UnityEngine.Input.mousePosition; }
  }

  // ----------------------------------------------
  public static bool multiTouchEnabled 
  {
    get { return UnityEngine.Input.multiTouchEnabled; }
    set{ UnityEngine.Input.multiTouchEnabled = value; }
  }

  // ------------------------------------------------------------------------------------------------
  public static int touchCount 
  {
    get { return UnityEngine.Input.touchCount; }
  }
  
  public static Touch[] touches {
    get { return UnityEngine.Input.touches; }
  }

  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static AccelerationEvent GetAccelerationEvent (int index)
  {
    return UnityEngine.Input.GetAccelerationEvent(index);
  }
  
  // Retrieves a Controllers axis. ------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static float GetAxis (String axisName)
  {
    Axis axis;
    if (s_axes.TryGetValue(axisName, out axis)) {
      if (Math.Abs(axis.value) > ANALOG_DEADZONE) {
        return axis.value;
      }
    }

    return UnityEngine.Input.GetAxis(axisName);
  }
  
  // Retrieves a Controllers axis. ------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static float GetAxisRaw (String axisName)
  {
    Axis axis;
    if (s_axes.TryGetValue(axisName, out axis)) {
      if (Math.Abs(axis.value) > ANALOG_DEADZONE) {
        return axis.value;
      }
    }

    return UnityEngine.Input.GetAxisRaw(axisName);
  }
  
  // Retrieves a Controllers button State -----------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetButton (String buttonName)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(buttonName, out buttonID)) {   
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
          case ButtonState.PRESSED:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetButton(buttonName);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetButtonDown (String buttonName)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(buttonName, out buttonID)) { 
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {   
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetButtonDown(buttonName);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetButtonUp (String buttonName)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(buttonName, out buttonID)) { 
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {   
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.RELEASING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetButtonUp(buttonName);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static String[] GetJoystickNames ()
  {
      if (mogaControllerManager && mogaControllerManager.isControllerConnected())
          return new[] {"Moga"};
    return UnityEngine.Input.GetJoystickNames ();
  }
  
  // Detect Continuous Key Presses with KeyCode -----------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKey (KeyCode key)
  {
    int buttonID;
    ButtonState buttonState;
      
    // Does this KeyCode exist? If so...
    if (s_codes_to_buttons.TryGetValue(key, out buttonID)) {
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
          case ButtonState.PRESSED:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKey(key);
  }
  
  // Detect Continuous Key Presses with String ------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKey (String name)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(name, out buttonID)) { 
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {   
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
          case ButtonState.PRESSED:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKey(name);
  }
  
  // Detect Single Key Press with KeyCode -----------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKeyDown (KeyCode key)
  {
    int buttonID;
    ButtonState buttonState;
      
    // Does this Key exist? If so...
    if (s_codes_to_buttons.TryGetValue(key, out buttonID)) {
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKeyDown(key);
  }

  // Detect Single Key Press with String ------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKeyDown (String name)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(name, out buttonID)) { 
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {     
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.PRESSING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKeyDown(name);
  }
  
  // Detect Key Release with KeyCode ----------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKeyUp (KeyCode key)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this Key exist? If so...
    if (s_codes_to_buttons.TryGetValue(key, out buttonID)) {   
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {   
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.RELEASING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKeyUp(key);
  }
  
  // Detect Key Release with String -----------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetKeyUp (String name)
  {
    int buttonID;
    ButtonState buttonState;
    
    // Does this String exist? If so...
    if (s_strings_to_button.TryGetValue(name, out buttonID)) {
      // Get Corrosponding buttonID from Moga Dictionary ...
      if (s_buttons.TryGetValue(buttonID, out buttonState)) {   
        switch (buttonState) {  // If Button State is Pressed or Pressing...
          case ButtonState.RELEASING:
            return true;
        }
      }
    }
    return UnityEngine.Input.GetKeyUp(name);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetMouseButton (int button)
  {
    return UnityEngine.Input.GetMouseButton(button);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetMouseButtonDown (int button)
  {
    return UnityEngine.Input.GetMouseButtonUp(button);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static bool GetMouseButtonUp (int button)
  {
    return UnityEngine.Input.GetMouseButtonUp(button);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static Touch GetTouch (int index)
  {
    return UnityEngine.Input.GetTouch(index);
  }
  
  // ------------------------------------------------------------------------------------------------
  // ------------------------------------------------------------------------------------------------
  public static void ResetInputAxes ()
  {
    foreach (KeyValuePair<String, Axis> entry in s_axes) {
      Axis axis = entry.Value;
      axis.value = 0.0f;
    }

    foreach (KeyValuePair<int, ButtonState> button in s_buttons) {    
      s_buttons[button.Key] = ButtonState.RELEASED;
    }

    UnityEngine.Input.ResetInputAxes();
  }
	
#if GPS_ENABLED
  public static LocationService location
  {
    get { return UnityEngine.Input.location; }
  }
#elif !GPS_ENABLED
  public static LocationService location
  {
    get { Debug.LogError("Define GPS_ENABLED to use this property"); return null; }
  }
#endif
#if UNITY_ANDROID || UNITY_WP8
	
#else

  // ------------------------------------------------------------------------------------------------
  public static void RegisterMogaController ()
  {   
  }

  // ------------------------------------------------------------------------------------------------
  public static float GetAxis (String axisName)
  {
    return 0;
  }

#endif

}
#endif

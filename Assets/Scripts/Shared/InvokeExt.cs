using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public static class InvokeExt
{
	///Calls an action on the next frame
	public static void InvokeNextFrame(this MonoBehaviour mb, UnityAction action)
	{
		mb.InvokeAfter(action, null);
	}

	///Calls an action at the end of the frame
	public static void InvokeEndOfFrame(this MonoBehaviour mb, UnityAction action)
	{
		mb.InvokeAfter(action, new WaitForEndOfFrame());
	}

	///Calls an action at the next FixedUpdate
	public static void InvokeFixedUpdate(this MonoBehaviour mb, UnityAction action)
	{
		mb.InvokeAfter(action, new WaitForFixedUpdate());
	}

	///Calls an action after a seconds within the given range of (fromSeconds, toSeconds)
	public static void InvokeAfterRandom(this MonoBehaviour mb, UnityAction action, float fromSeconds, float toSeconds)
	{
		mb.InvokeAfter(action, UnityEngine.Random.Range(fromSeconds, toSeconds));
	}

	//Calls an action after a given amount of seconds
	public static void InvokeAfter(this MonoBehaviour mb, UnityAction action, float seconds)
	{
		mb.InvokeAfter(action, new WaitForSeconds(seconds));
	}

	//Calls an action after a yield instruction has completed
	public static void InvokeAfter(this MonoBehaviour mb, UnityAction action, YieldInstruction yieldInstruction)
	{
		mb.StartCoroutine(WaitBefore(yieldInstruction, action));
	}

	//Coroutine that waits for the yield instruction to complete, then calls an action
	public static IEnumerator WaitBefore(YieldInstruction ins, UnityAction action)
	{
		yield return ins;
		action.Invoke();
	}

	//Repeats an action every frame starting on the next frame
	public static void InvokeFrameRepeat(this MonoBehaviour mb, UnityAction action, uint numTimes)
	{
		mb.StartCoroutine(RepeatCount(null, action, numTimes));
	}

	//Repeats an action forever with a number of seconds between each call
	public static void InvokeRepeatForeverSeconds(this MonoBehaviour mb, UnityAction action, float secondsBetween)
	{
		mb.StartCoroutine(RepeatForever(new WaitForSeconds(secondsBetween), action));
	}

	public static IEnumerator RepeatCount(YieldInstruction ins, UnityAction action, uint numTimes)
	{
		for (uint x = 0; x < numTimes; x++)
		{
			yield return ins;
			action.Invoke();
		}
	}

	public static void InvokeFrameRepeatForever(this MonoBehaviour mb, UnityAction action)
	{
		mb.StartCoroutine(RepeatForever(null, action));
	}

	public static IEnumerator RepeatForever(YieldInstruction ins, UnityAction action)
	{
		while (true)
		{
			yield return ins;
			action.Invoke();
		}
	}

	public static void InvokeWhenTrue<T>(this MonoBehaviour mb, T source, System.Func<T, bool> boolSelector, UnityAction action)
	{
		mb.StartCoroutine(WaitForValue(null, source, boolSelector, true, null, action));
	}

	public static void InvokeWhenFalse<T>(this MonoBehaviour mb, T source, System.Func<T, bool> boolSelector, UnityAction action)
	{
		mb.StartCoroutine(WaitForValue(null, source, boolSelector, false, null, action));
	}

	public static IEnumerator WaitForValue<T, K>(YieldInstruction waitTicker, T source, System.Func<T, K> selector, K desiredVal, UnityAction tick, UnityAction finished) where K : System.IComparable
	{
		while (selector(source).CompareTo(desiredVal) != 0)
		{
			yield return waitTicker;
			if (tick != null)
				tick.Invoke();
		}
		if (finished != null)
			finished.Invoke();
	}
}
//
//  This file is responsible for making Adobe Analytical calls and should be attached to a persistent game object
//  Android only calls, for iOS See ADBMobileHelper.m

//  Adobe Digital Marketing Suite -- iOS Application Measurement Library
//  Copyright 1996-2015. Adobe, Inc. All Rights Reserved


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using com.adobe.mobile;

public class ADBMobileController : MonoBehaviour 
{
	public string appName;
	private bool setContextComplete = false;
	private const string ADOBE_SDK_VERSION = "4.5.0";


	void Awake()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		ADBMobile.SetContext ();
		var cdata = new Dictionary<string, object> ();
		cdata.Add ("appname", appName);
		cdata.Add("sdkversion", ADOBE_SDK_VERSION + ":"+ Application.version);
		ADBMobile.CollectLifecycleData (cdata);
		StartCoroutine(DelayPauseCalls());
		#endif

	}

	private IEnumerator DelayPauseCalls()
	{
		yield return new WaitForSeconds(2.0f);
		setContextComplete = true;
	}

	void OnApplicationPause(bool isPaused)
	{
		if(!setContextComplete)
			return;
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (isPaused) {			
			ADBMobile.PauseCollectingLifecycleData ();
		} 
		else {
			var cdata = new Dictionary<string, object> ();
			cdata.Add ("appname", appName);
			cdata.Add("sdkversion", ADOBE_SDK_VERSION + ":"+ Application.version);
			ADBMobile.CollectLifecycleData (cdata);
		}
		#endif
	}
	
}

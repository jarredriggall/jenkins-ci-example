//
//  ADBMobile.cs
//  Adobe Digital Marketing Suite
//  Unity Plug-in v: 4.4.2
//
//  Copyright 1996-2015. Adobe, Inc. All Rights Reserved
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System;

namespace com.adobe.mobile
{
	
	public class ADBMobile {
		public enum ADBPrivacyStatus {
			MOBILE_PRIVACY_STATUS_OPT_IN = 1,
			MOBILE_PRIVACY_STATUS_OPT_OUT = 2,
			MOBILE_PRIVACY_STATUS_UNKNOWN = 3
		};
		
		public enum ADBBeaconProximity {
			PROXIMITY_UNKNOWN = 0,
			PROXIMITY_IMMEDIATE = 1,
			PROXIMITY_NEAR = 2,
			PROXIMITY_FAR = 3
		};
		
		#if UNITY_IPHONE
		/* ===================================================================
		 * extern declarations for iOS Methods
		 * =================================================================== */
		[DllImport ("__Internal")]
		private static extern System.IntPtr adb_GetVersion();
		
		[DllImport ("__Internal")]
		private static extern int adb_GetPrivacyStatus ();
		
		[DllImport ("__Internal")]
		private static extern void adb_SetPrivacyStatus (int status);
		
		[DllImport ("__Internal")]
		private static extern double adb_GetLifetimeValue ();
		
		[DllImport ("__Internal")]
		private static extern System.IntPtr adb_GetUserIdentifier();
		
		[DllImport ("__Internal")]
		private static extern void adb_SetUserIdentifier (string userId);
		
		[DllImport ("__Internal")]
		private static extern bool adb_GetDebugLogging ();
		
		[DllImport ("__Internal")]
		private static extern void adb_SetDebugLogging (bool enabled);
		
		[DllImport ("__Internal")]
		private static extern void adb_KeepLifecycleSessionAlive ();
		
		[DllImport ("__Internal")]
		private static extern void adb_CollectLifecycleData ();
		
		[DllImport ("__Internal")]
		private static extern void adb_EnableLocalNotifications ();
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackState(string state, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackAction(string action, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackActionFromBackground(string action, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackLocation(float latValue, float lonValue, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackBeacon(int major, int minor, string uuid, int proximity, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackingClearCurrentBeacon();
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackLifetimeValueIncrease(double amount, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackTimedActionStart(string action, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackTimedActionUpdate(string action, string cdataString);
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackTimedActionEnd(string action);
		
		[DllImport ("__Internal")]
		private static extern bool adb_TrackingTimedActionExists(string action);
		
		[DllImport ("__Internal")]
		private static extern System.IntPtr adb_GetTrackingIdentifier();
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackingSendQueuedHits();
		
		[DllImport ("__Internal")]
		private static extern void adb_TrackingClearQueue();
		
		[DllImport ("__Internal")]
		private static extern int adb_TrackingGetQueueSize();
		
		[DllImport ("__Internal")]
		private static extern System.IntPtr adb_GetMarketingCloudID();
		
		[DllImport ("__Internal")]
		private static extern void adb_VisitorSyncIdentifiers(string identifiersJson);
		#endif
		
		#if UNITY_ANDROID
		/* ===================================================================
		 * Static Helper objects for our JNI access
		 * =================================================================== */
		static AndroidJavaClass analytics = new AndroidJavaClass("com.adobe.mobile.Analytics");
		static AndroidJavaClass config = new AndroidJavaClass("com.adobe.mobile.Config");
		static AndroidJavaClass visitor = new AndroidJavaClass("com.adobe.mobile.Visitor");
		#endif
		
		/* ===================================================================
		 * Configuration Methods
		 * =================================================================== */
		
		public static void CollectLifecycleData()
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				adb_CollectLifecycleData();
				#elif UNITY_ANDROID	
				AndroidJavaObject activity = null;
				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activity = actClass.GetStatic<AndroidJavaObject>("currentActivity");
					config.CallStatic ("collectLifecycleData", activity);
				}
				#endif
			}
		}
		
		public static void CollectLifecycleData(Dictionary<string, object> cdata)
		{
			#if UNITY_ANDROID
			if (!IsEditor ()) 
			{
				AndroidJavaObject activity = null;
				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activity = actClass.GetStatic<AndroidJavaObject>("currentActivity");
					using (var hashmap = GetHashMapFromDictionary(cdata))
					{
						config.CallStatic ("collectLifecycleData", activity, hashmap);
					}
				}
			}
			#endif
		}
		
		public static bool GetDebugLogging()
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				return adb_GetDebugLogging();
				#elif UNITY_ANDROID
				// we have to get AndroidJavaObject because the native method returns a Boolean object rather than a boolean primitive
				using (AndroidJavaObject obj = config.CallStatic<AndroidJavaObject> ("getDebugLogging"))
				{
					// then we have to call (java) public boolean Boolean.booleanValue(); to get the primitive value to return
					return obj.Call<bool>("booleanValue");
				}
				#endif
			}
			
			return false;
		}
		
		public static double GetLifetimeValue()
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				return adb_GetLifetimeValue();
				#elif UNITY_ANDROID
				using (var ltv = config.CallStatic<AndroidJavaObject> ("getLifetimeValue"))
				{
					return ltv.Call<double>("doubleValue");
				}
				#endif
			}
			
			return 0;
		}
		
		public static ADBPrivacyStatus GetPrivacyStatus()
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				return ADBPrivacyStatusFromInt(adb_GetPrivacyStatus());
				#elif UNITY_ANDROID
				using (AndroidJavaObject obj = config.CallStatic<AndroidJavaObject>("getPrivacyStatus"))
				{
					int status = obj.Call<int>("getValue");
					return ADBPrivacyStatusFromInt(status + 1);	// because the enum in iOS is 1-based and Android is 0-based
				}
				#endif	
			}
			
			return ADBPrivacyStatus.MOBILE_PRIVACY_STATUS_UNKNOWN;
		}
		
		public static string GetUserIdentifier()
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				return Marshal.PtrToStringAnsi(adb_GetUserIdentifier());
				#elif UNITY_ANDROID
				// have to get the object in case the uid is null
				// config.CallStatic<string>("getUserIdentifier") will cause a crash if the native method returns null
				try{
					using (var uid = config.CallStatic<AndroidJavaObject>("getUserIdentifier"))
					{
						return uid != null ? uid.Call<string>("toString") : null;
					}
				}catch(Exception ){
					return null;
				}
				#endif
			}
			
			return "";
		}
		
		public static string GetVersion() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				return Marshal.PtrToStringAnsi( adb_GetVersion());		
				#elif UNITY_ANDROID
				return config.CallStatic<string> ("getVersion");
				#endif	
			}
			
			return "";
		}
		
		public static void KeepLifecycleSessionAlive()
		{
			#if UNITY_IPHONE
			if (!IsEditor())		
			{
				adb_KeepLifecycleSessionAlive();
			}
			#endif
		}
		
		public static void OverrideConfigPath(string fileName)
		{
			#if UNITY_ANDROID	
			if (!IsEditor ()) 
			{
				// Activity.getResources().getAssets().open(fileName);
				AndroidJavaObject activity = null;
				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activity = actClass.GetStatic<AndroidJavaObject>("currentActivity");

					// android.content.res.Resources
					using (var resources = activity.Call<AndroidJavaObject>("getResources"))
					{
						// android.content.res.AssetManager
						using (var assets = resources.Call<AndroidJavaObject>("getAssets"))
						{
							// java.io.InputStream
							using (var stream = assets.Call<AndroidJavaObject>("open", fileName))
							{
								config.CallStatic("overrideConfigStream", stream);
							}
						}
					}
				}
			}
			#endif
		}
		
		public static void PauseCollectingLifecycleData()
		{
			#if UNITY_ANDROID
			if (!IsEditor ()) 
			{
				config.CallStatic("pauseCollectingLifecycleData");
			}
			#endif
		}
		
		public static void SetContext()
		{
			#if UNITY_ANDROID
			if (!IsEditor())
			{
				AndroidJavaObject activity = null;
				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activity = actClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				
				config.CallStatic("setContext", activity);
			}
			#endif
		}
		
		public static void SetDebugLogging(bool enabled) 
		{
			if (!IsEditor())
			{
				#if UNITY_IPHONE		
				adb_SetDebugLogging(enabled);		
				#elif UNITY_ANDROID	
				using (var obj = new AndroidJavaObject("java.lang.Boolean", enabled))
				{
					config.CallStatic("setDebugLogging", obj);
				}
				#endif
			}
		}
		
		public static void SetPrivacyStatus(ADBPrivacyStatus status)
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				adb_SetPrivacyStatus((int)status);
				#elif UNITY_ANDROID
				using (var privacyClass = new AndroidJavaClass("com.adobe.mobile.MobilePrivacyStatus"))
				{
					var privacyObject = privacyClass.GetStatic<AndroidJavaObject>(status.ToString());
					config.CallStatic("setPrivacyStatus", privacyObject);
				}
				#endif	
			}
		}
		
		public static void SetUserIdentifier(string userId)
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE		
				adb_SetUserIdentifier(userId);
				#elif UNITY_ANDROID
				config.CallStatic("setUserIdentifier", userId);
				#endif
			}
		}
		
		public static void EnableLocalNotifications()
		{
			#if UNITY_IPHONE
			if (!IsEditor())
			{
				adb_EnableLocalNotifications();
			}
			#endif
		}
		
		/* ===================================================================
		 * Analytics Methods
		 * =================================================================== */
		public static void TrackState(string state, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackState(state, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					analytics.CallStatic("trackState", state, hashmap);
				}
				#endif		
			}
		}
		
		public static void TrackAction(string action, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackAction(action, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					analytics.CallStatic("trackAction", action, hashmap);
				}
				#endif
			}
		}
		
		public static void TrackActionFromBackground(string action, Dictionary<string, object> cdata) 
		{
			#if UNITY_IPHONE
			if (!IsEditor ()) 
			{
				adb_TrackActionFromBackground(action, JsonStringFromDictionary(cdata));
			}
			#endif
		}
		
		public static void TrackLocation(float latValue, float lonValue, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackLocation(latValue, lonValue, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					using (var location = new AndroidJavaObject("android.location.Location", "dummyProvider"))
					{
						location.Call("setLatitude", (double)latValue);
						location.Call("setLongitude", (double)lonValue);
						analytics.CallStatic("trackLocation", location, hashmap);
					}
				}
				#endif
			}
		}
		
		public static void TrackBeacon(int major, int minor, string uuid, ADBBeaconProximity proximity, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackBeacon(major, minor, uuid, (int)proximity, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					using (var proxClass = new AndroidJavaClass("com.adobe.mobile.Analytics$BEACON_PROXIMITY"))
					{
						var proxValue = proxClass.GetStatic<AndroidJavaObject>(proximity.ToString());
						var stringMajor = new AndroidJavaObject("java.lang.String", major.ToString());
						var stringMinor = new AndroidJavaObject("java.lang.String", minor.ToString());
						analytics.CallStatic("trackBeacon", uuid, stringMajor, stringMinor, proxValue, hashmap);
					}
				}
				#endif
			}
		}
		
		public static void TrackingClearCurrentBeacon() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackingClearCurrentBeacon();
				#elif UNITY_ANDROID
				analytics.CallStatic("clearBeacon");
				#endif
			}
		}
		
		public static void TrackLifetimeValueIncrease(double amount, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackLifetimeValueIncrease(amount, JsonStringFromDictionary(cdata));	
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					using (var ltvAmount = new AndroidJavaObject("java.math.BigDecimal", amount))
					{
						analytics.CallStatic("trackLifetimeValueIncrease", ltvAmount, hashmap);
					}
				}
				#endif
			}
		}
		
		public static void TrackTimedActionStart(string action, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackTimedActionStart(action, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					analytics.CallStatic("trackTimedActionStart", action, hashmap);
				}
				#endif
			}
		}
		
		public static void TrackTimedActionUpdate(string action, Dictionary<string, object> cdata) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackTimedActionUpdate(action, JsonStringFromDictionary(cdata));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(cdata))
				{
					analytics.CallStatic("trackTimedActionUpdate", action, hashmap);
				}
				#endif
			}
		}
		
		public static void TrackTimedActionEnd(string action)
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackTimedActionEnd(action);
				#elif UNITY_ANDROID
				analytics.CallStatic("trackTimedActionEnd", action, null);
				#endif
			}
		}
		
		public static bool TrackingTimedActionExists(string action) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				return adb_TrackingTimedActionExists(action);
				#elif UNITY_ANDROID
				using (AndroidJavaObject actionBool = analytics.CallStatic<AndroidJavaObject> ("trackingTimedActionExists", action))
				{
					return actionBool.Call<bool>("booleanValue");
				}
				#endif
			}
			
			return false;
		}
		
		public static string GetTrackingIdentifier() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				return Marshal.PtrToStringAnsi(adb_GetTrackingIdentifier());
				#elif UNITY_ANDROID
				try{
					using (var tid = analytics.CallStatic<AndroidJavaObject>("getTrackingIdentifier"))
					{
						return tid != null ? tid.Call<string>("toString") : null;
					}
				}catch(Exception){
					return null;
				}
				#endif
			}
			
			return "";
		}
		
		public static void TrackingSendQueuedHits() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackingSendQueuedHits();
				#elif UNITY_ANDROID
				analytics.CallStatic("sendQueuedHits");
				#endif
			}
		}
		
		public static void TrackingClearQueue() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_TrackingClearQueue();
				#elif UNITY_ANDROID
				analytics.CallStatic("clearQueue");
				#endif
			}
		}
		
		public static int TrackingGetQueueSize() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				return adb_TrackingGetQueueSize();
				#elif UNITY_ANDROID
				return (int)analytics.CallStatic<long>("getQueueSize");			
				#endif
			}
			
			return 0;
		}
		
		/* ===================================================================
		 * Marketing Cloud ID Methods
		 * =================================================================== */
		public static string GetMarketingCloudID() 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				return Marshal.PtrToStringAnsi(adb_GetMarketingCloudID());
				#elif UNITY_ANDROID
				try{
					using (var mcid = visitor.CallStatic<AndroidJavaObject>("getMarketingCloudId"))
					{
						return mcid != null ? mcid.Call<string>("toString") : null;
					}
				}catch(Exception){
					return null;
				}
				#endif
			}
			
			return "";
		}
		
		public static void VisitorSyncIdentifiers(Dictionary<string, object> identifiers) 
		{
			if (!IsEditor ()) 
			{
				#if UNITY_IPHONE
				adb_VisitorSyncIdentifiers(JsonStringFromDictionary(identifiers));
				#elif UNITY_ANDROID
				using (var hashmap = GetHashMapFromDictionary(identifiers))
				{
					visitor.CallStatic("syncIdentifiers", hashmap);
				}
				#endif
			}
		}
		
		/* ===================================================================
		 * Helper Methods
		 * =================================================================== */
		private static bool IsEditor()
		{
			return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor;
		}
		
		private static ADBPrivacyStatus ADBPrivacyStatusFromInt(int statusInt)
		{
			switch (statusInt) 
			{
			case 1:
				return ADBPrivacyStatus.MOBILE_PRIVACY_STATUS_OPT_IN;				
			case 2:
				return ADBPrivacyStatus.MOBILE_PRIVACY_STATUS_OPT_OUT;				
			case 3:
				return ADBPrivacyStatus.MOBILE_PRIVACY_STATUS_UNKNOWN;				
			default:
				return ADBPrivacyStatus.MOBILE_PRIVACY_STATUS_UNKNOWN;				
			}
		}
		
		private static ADBBeaconProximity ADBBeaconProximityFromInt(int proximity)
		{
			switch (proximity) 
			{
			case 1:
				return ADBBeaconProximity.PROXIMITY_IMMEDIATE;				
			case 2:
				return ADBBeaconProximity.PROXIMITY_NEAR;				
			case 3:
				return ADBBeaconProximity.PROXIMITY_FAR;
			default:
				return ADBBeaconProximity.PROXIMITY_UNKNOWN;				
			}
		}
		
		#if UNITY_IPHONE
		private static string JsonStringFromDictionary(Dictionary<string, object> dict) 
		{
			if (dict == null || dict.Count <= 0) 
			{
				return null;
			}
			
			var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
			string jsonString = "{" + string.Join (",", entries.ToArray()) + "}";
			
			return jsonString;
		}
		#elif UNITY_ANDROID
		private static AndroidJavaObject GetHashMapFromDictionary(Dictionary<string, object> dict)
		{
			// quick out if nothing in the dict param
			if (dict == null || dict.Count <= 0) 
			{
				return null;
			}
			
			AndroidJavaObject hashMap = new AndroidJavaObject ("java.util.HashMap");
			IntPtr putMethod = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			object[] args = new object[2];
			foreach (KeyValuePair<string, object> kvp in dict)
			{
				using (var key = new AndroidJavaObject("java.lang.String", kvp.Key))
				{
					using (var value = new AndroidJavaObject("java.lang.String", kvp.Value))
					{
						args[0] = key;
						args[1] = value;
						AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
					}
				}
			}
			
			return hashMap;
		}
		#endif
	}
}

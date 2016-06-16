//
//  ADBMobileWrapper.h
//  Adobe Digital Marketing Suite -- iOS Application Measurement Library
//  Unity Plug-in
//
//  Copyright 1996-2015. Adobe, Inc. All Rights Reserved
//
#ifndef Unity_iPhone_ADBMobileWrapper_h
#define Unity_iPhone_ADBMobileWrapper_h

extern "C" {
    // configuration
    const char *adb_GetVersion();
    int adb_GetPrivacyStatus();
    void adb_SetPrivacyStatus(int status);
    double adb_GetLifetimeValue();
    const char *adb_GetUserIdentifier();
    void adb_SetUserIdentifier(const char *userId);
    bool adb_GetDebugLogging();
    void adb_SetDebugLogging(bool enabled);
    void adb_KeepLifecycleSessionAlive();
    void adb_CollectLifecycleData();
    void adb_EnableLocalNotifications();
    
    // analytics
    void adb_TrackState(const char *state, const char *cdataString);
    void adb_TrackAction(const char *action, const char *cdataString);
    void adb_TrackActionFromBackground(const char *action, const char *cdataString);
    void adb_TrackLocation(float latValue, float lonValue, const char *cdataString);
    void adb_TrackBeacon(int major, int minor, const char *uuid, int proximity, const char *cdataString);
    void adb_TrackingClearCurrentBeacon();
    void adb_TrackLifetimeValueIncrease(double amount, const char *cdataString);
    void adb_TrackTimedActionStart(const char *action, const char *cdataString);
    void adb_TrackTimedActionUpdate(const char *action, const char *cdataString);
    void adb_TrackTimedActionEnd(const char *action);
    bool adb_TrackingTimedActionExists(const char *action);
    const char *adb_GetTrackingIdentifier();
    void adb_TrackingSendQueuedHits();
    void adb_TrackingClearQueue();
    int adb_TrackingGetQueueSize();
    
    // marketing cloud id
    const char *adb_GetMarketingCloudID();
    void adb_VisitorSyncIdentifiers(const char *identifiers);
}

#endif
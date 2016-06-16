//
//  ADBMobileWrapper.mm
//  Adobe Digital Marketing Suite -- iOS Application Measurement Library
//  Unity Plug-in
//
//  Copyright 1996-2015. Adobe, Inc. All Rights Reserved
//
#import <CoreLocation/CoreLocation.h>
#import "ADBMobileWrapper.h"
#import "ADBMobile.h"

NSDictionary *_getDictionaryFromJsonString(const char *jsonString);

#pragma mark - config methods
const char *adb_GetVersion() {
    return [[ADBMobile version] cStringUsingEncoding:NSUTF8StringEncoding];
}

int adb_GetPrivacyStatus() {
    return [ADBMobile privacyStatus];
}

void adb_SetPrivacyStatus(int status) {
    [ADBMobile setPrivacyStatus:(ADBMobilePrivacyStatus)status];
}

double adb_GetLifetimeValue() {
    return [[ADBMobile lifetimeValue] doubleValue];
}

const char *adb_GetUserIdentifier() {
    NSString *uid = [ADBMobile userIdentifier];
    return uid ? [uid cStringUsingEncoding:NSUTF8StringEncoding] : "";
}

void adb_SetUserIdentifier(const char *userId) {
    if (userId) {
        [ADBMobile setUserIdentifier:[NSString stringWithCString:userId encoding:NSUTF8StringEncoding]];
    }
}

bool adb_GetDebugLogging() {
    return [ADBMobile debugLogging];
}

void adb_SetDebugLogging(bool enabled) {
    [ADBMobile setDebugLogging:enabled];
}

void adb_KeepLifecycleSessionAlive() {
    [ADBMobile keepLifecycleSessionAlive];
}

void adb_CollectLifecycleData() {
    [ADBMobile collectLifecycleData];
}

void adb_EnableLocalNotifications() {
    UIApplication *app = [UIApplication sharedApplication];
    
    if ([app respondsToSelector:@selector(registerUserNotificationSettings:)])
    {
        UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound categories:nil];
        [app registerUserNotificationSettings:settings];
        [app registerForRemoteNotifications];
    }
}

#pragma mark - analytics methods
void adb_TrackState(const char *state, const char *cdataString) {
    NSString *tempState = state ? [NSString stringWithCString:state encoding:NSUTF8StringEncoding] : nil;
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackState:tempState data:cData];
}

void adb_TrackAction(const char *action, const char *cdataString) {
    NSString *tempAction = action ? [NSString stringWithCString:action encoding:NSUTF8StringEncoding] : nil;
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackAction:tempAction data:cData];
}

void adb_TrackActionFromBackground(const char *action, const char *cdataString) {
    NSString *tempAction = [NSString stringWithCString:action encoding:NSUTF8StringEncoding];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackActionFromBackground:tempAction data:cData];
}

void adb_TrackLocation(float latValue, float lonValue, const char *cdataString) {
    CLLocation *location = [[CLLocation alloc] initWithLatitude:latValue longitude:lonValue];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackLocation:location data:cData];
}

void adb_TrackBeacon(int major, int minor, const char *uuid, int proximity, const char *cdataString) {
    if (!uuid) {
        return;
    }
    
    CLBeacon *beacon = [[CLBeacon alloc] init];
    [beacon setValue:[NSNumber numberWithInt:major] forKey:@"major"];
    [beacon setValue:[NSNumber numberWithInt:minor] forKey:@"minor"];
    [beacon setValue:[NSNumber numberWithInt:proximity] forKey:@"proximity"];
    [beacon setValue:[[NSUUID alloc] initWithUUIDString:[NSString stringWithCString:uuid encoding:NSUTF8StringEncoding]] forKey:@"proximityUUID"];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackBeacon:beacon data:cData];
}

void adb_TrackingClearCurrentBeacon() {
    [ADBMobile trackingClearCurrentBeacon];
}

void adb_TrackLifetimeValueIncrease(double amount, const char *cdataString) {
    NSDecimalNumber *tempAmount = [[NSDecimalNumber alloc] initWithDouble:amount];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackLifetimeValueIncrease:tempAmount data:cData];
}

void adb_TrackTimedActionStart(const char *action, const char *cdataString) {
    if (!action) {
        return;
    }
    
    NSString *tempAction = [NSString stringWithCString:action encoding:NSUTF8StringEncoding];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackTimedActionStart:tempAction data:cData];
}

void adb_TrackTimedActionUpdate(const char *action, const char *cdataString) {
    if (!action) {
        return;
    }
    
    NSString *tempAction = [NSString stringWithCString:action encoding:NSUTF8StringEncoding];
    NSDictionary *cData = cdataString ? _getDictionaryFromJsonString(cdataString) : nil;
    
    [ADBMobile trackTimedActionUpdate:tempAction data:cData];
}

void adb_TrackTimedActionEnd(const char *action) {
    if (!action) {
        return;
    }
    
    NSString *tempAction = [NSString stringWithCString:action encoding:NSUTF8StringEncoding];
    
    [ADBMobile trackTimedActionEnd:tempAction logic:^BOOL(NSTimeInterval inAppDuration, NSTimeInterval totalDuration, NSMutableDictionary *data) {
        return YES;
    }];
}

bool adb_TrackingTimedActionExists(const char *action) {
    return action ? [ADBMobile trackingTimedActionExists:[NSString stringWithCString:action encoding:NSUTF8StringEncoding]] : false;
}

const char *adb_GetTrackingIdentifier() {
    NSString *trackingId = [ADBMobile trackingIdentifier];
    return trackingId ? [trackingId cStringUsingEncoding:NSUTF8StringEncoding] : "";
}

void adb_TrackingSendQueuedHits() {
    [ADBMobile trackingSendQueuedHits];
}

void adb_TrackingClearQueue() {
    [ADBMobile trackingClearQueue];
}

int adb_TrackingGetQueueSize() {
    return [ADBMobile trackingGetQueueSize];
}

#pragma mark - marketing cloud id
const char *adb_GetMarketingCloudID() {
    NSString *mcid = [ADBMobile visitorMarketingCloudID];
    return mcid ? [mcid cStringUsingEncoding:NSUTF8StringEncoding] : "";
}

void adb_VisitorSyncIdentifiers(const char *identifiers) {
    [ADBMobile visitorSyncIdentifiers:_getDictionaryFromJsonString(identifiers)];
}

#pragma mark - helpers
NSDictionary *_getDictionaryFromJsonString(const char *jsonString) {
    if (!jsonString) {
        return nil;
    }
    
    NSError *error = nil;
    NSString *tempString = [NSString stringWithCString:jsonString encoding:NSUTF8StringEncoding];
    NSData *data = [tempString dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data
                                                                options:NSJSONReadingMutableContainers
                                                                  error:&error];
    
    return (dict && !error) ? dict : nil;
}
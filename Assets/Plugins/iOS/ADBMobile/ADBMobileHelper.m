#import <Foundation/Foundation.h>
#import "ADBMobileHelper.h"
#import "ADBmobile.h"
static ADBMobileHelper *_sharedInstance = nil;

@implementation ADBMobileHelper
NSString *appName = @"TestApp123";

+(void)load
{
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(launch:) name:UIApplicationDidFinishLaunchingNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(gotoForeground:) name:UIApplicationWillEnterForegroundNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(gotoBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
}

+ (void)launch:(NSNotification *)notification
{
    _sharedInstance = [[ADBMobileHelper alloc] init];
    [self sendADBMobileData];
}

+ (void)gotoForeground:(NSNotification *)notification
{
    [self sendADBMobileData];
    
}

+ (void)gotoBackground:(NSNotification *)notification
{
    [self sendADBMobileData];
}

+ (void) sendADBMobileData
{
    NSString *sdkversion = [NSString stringWithFormat:@"%@:%@:%@", [ADBMobile version],
                            [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"],
                            [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"]]; NSMutableDictionary *data = [NSMutableDictionary dictionary];
    [data setObject:appName forKey:@"appname"];
    [data setObject:sdkversion forKey:@"sdkversion"];
    [ADBMobile collectLifecycleDataWithAdditionalData:data];
}


@end


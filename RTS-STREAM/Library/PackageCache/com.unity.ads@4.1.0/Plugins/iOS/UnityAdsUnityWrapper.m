#import "UnityAppController.h"
#import "Unity/UnityInterface.h"

#import "UnityAds/UnityAds.h"
#import <UnityAds/UADSBanner.h>
#import "UnityAds/UADSMetaData.h"

#import "UnityAdsUtilities.h"
#import "UnityAdsInitializationListener.h"
#import "UnityAdsLoadListener.h"
#import "UnityAdsShowListener.h"

void UnityAdsInitialize(const char * gameId, bool testMode, void *listenerPtr) {
    UnityAdsInitializationListener *listener = listenerPtr ? (__bridge UnityAdsInitializationListener *)listenerPtr : nil;
    [UnityAds initialize:[NSString stringWithUTF8String:gameId] testMode:testMode initializationDelegate:listener];
}

void UnityAdsLoad(const char * placementId, void *listenerPtr) {
    UnityAdsLoadListener *listener = listenerPtr ? (__bridge UnityAdsLoadListener *)listenerPtr : nil;
    [UnityAds load:[NSString stringWithUTF8String:placementId] loadDelegate:listener];
}

void UnityAdsShow(const char * placementId, void *listenerPtr) {
    UnityAdsShowListener *listener = listenerPtr ? (__bridge UnityAdsShowListener *)listenerPtr : nil;
    [UnityAds show:UnityGetGLViewController() placementId:NSSTRING_OR_EMPTY(placementId) showDelegate:listener];
}

bool UnityAdsGetDebugMode() {
    return [UnityAds getDebugMode];
}

void UnityAdsSetDebugMode(bool debugMode) {
    [UnityAds setDebugMode:debugMode];
}

bool UnityAdsIsSupported() {
    return [UnityAds isSupported];
}

const char * UnityAdsGetVersion() {
    return UnityAdsCopyString([[UnityAds getVersion] UTF8String]);
}

bool UnityAdsIsInitialized() {
    return [UnityAds isInitialized];
}

void UnityAdsSetMetaData(const char * category, const char * data) {
    if(category != NULL && data != NULL) {
        UADSMetaData* metaData = [[UADSMetaData alloc] initWithCategory:[NSString stringWithUTF8String:category]];
        NSDictionary* json = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:data] dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];
        for(id key in json) {
            [metaData set:key value:[json objectForKey:key]];
        }
        [metaData commit];
    }
}

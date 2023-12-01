#import <Foundation/Foundation.h>
#import <AVFoundation/AVAudioSession.h>

extern "C" {
    float unity_services_analytics_get_device_volume() {
        return [AVAudioSession sharedInstance].outputVolume;
    }
}

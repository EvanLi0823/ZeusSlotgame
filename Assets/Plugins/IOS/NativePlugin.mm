//
//  NativePlugin.m
//  Unity-iPhone
//
//  Created by 陈亮 on 2025/2/18.
//

#import "NativePlugin.h"
#import "UnityInterface.h"
#import "SDFGsonUtil.h"

@implementation NativePlugin
id<NativePluginProtocol> api = NULL;
+(void) registerAPIforNative:(id<NativePluginProtocol>) aApi
{
    api = aApi;
}

+(void)H5InitResult{
    NSLog(@"H5InitResult...");
    UnitySendMessage([@"PlatformManager" UTF8String], [@"H5InitResult" UTF8String], [@"" UTF8String]);
}

+(void)H5AddCash:(NSString *)json{
    UnitySendMessage([@"PlatformManager" UTF8String], [@"H5AddCash" UTF8String], [json UTF8String]);
}

+(void)ADPlayResult:(NSString *)json{
    UnitySendMessage([@"PlatformManager" UTF8String], [@"ADPlayResult" UTF8String], [json UTF8String]);
}
+(void)portrait{
    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
    [dic setObject:@"1" forKey:@"amount"];
    UnitySendMessage([@"PlatformManager" UTF8String], [@"SetOrientation" UTF8String], [[SDFGsonUtil toJson:dic] UTF8String]);
}
+(void)landscape{
    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
    [dic setObject:@"0" forKey:@"amount"];
    UnitySendMessage([@"PlatformManager" UTF8String], [@"SetOrientation" UTF8String], [[SDFGsonUtil toJson:dic] UTF8String]);
}
@end

char* convertNSStringToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

extern "C" {
    const char* callNative(const char* json) {
        if(api){
            return convertNSStringToCString([api native:[NSString stringWithUTF8String:json]]);
        }
        return convertNSStringToCString(@"");
    }
}

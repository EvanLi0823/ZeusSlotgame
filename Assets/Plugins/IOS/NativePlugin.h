//
//  NativePlugin.h
//  Unity-iPhone
//
//  Created by 陈亮 on 2025/2/18.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol NativePluginProtocol
@required
- (NSString *) native:(NSString*)json;
// other methods
@end

__attribute__ ((visibility("default")))
@interface NativePlugin : NSObject
+(void) registerAPIforNative:(id<NativePluginProtocol>) aApi;

//show h5 enter
+(void)H5InitResult;
//h5 reward
+(void)H5AddCash:(NSString *)json;
//ad result
+(void)ADPlayResult:(NSString *)json;
+(void)portrait;
+(void)landscape;
@end

NS_ASSUME_NONNULL_END

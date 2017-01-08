//
//  Goodies.cpp
//  TestIosLibrary
//
//  Created by Taras Leskiv on 28/07/16.
//  Copyright © 2016 Dead Mosquito Games. All rights reserved.
//

#import "GoodiesAlertHandler.h"
#import "IosGoodiesFunctionDefs.h"
#import "IosGoodiesUtils.h"
#import "GoodiesUiMessageDelegate.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C" {

GoodiesAlertHandler *handler;

void _showConfirmationDialog(const char *title, const char *message,
        const char *buttonTitle,
        ActionVoidCallbackDelegate callback,
        void *onSuccessActionPtr) {
    NSString *titleStr = [IosGoodiesUtils createNSStringFrom:title];
    NSString *messageStr = [IosGoodiesUtils createNSStringFrom:message];
    NSString *buttonTitleStr = [IosGoodiesUtils createNSStringFrom:buttonTitle];

    handler = [GoodiesAlertHandler new];
    handler.callbackButtonClicked = ^(long index) {
        callback(onSuccessActionPtr);
    };

    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:titleStr
                                                    message:messageStr
                                                   delegate:handler
                                          cancelButtonTitle:nil
                                          otherButtonTitles:buttonTitleStr, nil];
    [alert show];
}

void _showQuestionDialog(const char *title, const char *message,
        const char *buttonOkTitle,
        const char *buttonCancelTitle,
        ActionVoidCallbackDelegate callback,
        void *onSuccessActionPtr, void *onCancelActionPtr) {

    NSString *titleStr = [IosGoodiesUtils createNSStringFrom:title];
    NSString *messageStr = [IosGoodiesUtils createNSStringFrom:message];
    NSString *buttonTitleStr = [IosGoodiesUtils createNSStringFrom:buttonOkTitle];
    NSString *buttonCancelStr =
            [IosGoodiesUtils createNSStringFrom:buttonCancelTitle];

    handler = [GoodiesAlertHandler new];
    handler.callbackButtonClicked = ^(long index) {
        if (index == 0) {
            callback(onCancelActionPtr);
        } else {
            callback(onSuccessActionPtr);
        }
    };

    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:titleStr
                                                    message:messageStr
                                                   delegate:handler
                                          cancelButtonTitle:buttonCancelStr
                                          otherButtonTitles:buttonTitleStr, nil];
    [alert show];
}

void _showOptionalDialog(const char *title, const char *message,
        const char *buttonFirst,
        const char *buttonSecond,
        const char *buttonCancel,
        ActionVoidCallbackDelegate callback,
        void *onFirstButtonActionPtr,
        void *onSecondButtonActionPtr,
        void *onCancelActionPtr) {

    NSString *titleStr = [IosGoodiesUtils createNSStringFrom:title];
    NSString *messageStr = [IosGoodiesUtils createNSStringFrom:message];

    NSString *buttonCancelStr = [IosGoodiesUtils createNSStringFrom:buttonCancel];
    NSString *buttonFirstStr = [IosGoodiesUtils createNSStringFrom:buttonFirst];
    NSString *buttonSecondStr = [IosGoodiesUtils createNSStringFrom:buttonSecond];

    handler = [GoodiesAlertHandler new];
    handler.callbackButtonClicked = ^(long index) {
        switch (index) {
            case 0:
                callback(onCancelActionPtr);
                break;

            case 1:
                callback(onFirstButtonActionPtr);
                break;

            default:
                callback(onSecondButtonActionPtr);
                break;
        }
    };

    UIAlertView *alert =
            [[UIAlertView alloc] initWithTitle:titleStr
                                       message:messageStr
                                      delegate:handler
                             cancelButtonTitle:buttonCancelStr
                             otherButtonTitles:buttonFirstStr, buttonSecondStr, nil];
    [alert show];
}

void _showShareMessageWithImage(const char *message, const void *data,
        const unsigned long data_length,
        ActionVoidCallbackDelegate callback,
        void *onSharedActionPtr) {

    NSString *messageStr = [IosGoodiesUtils createNSStringFrom:message];
    NSMutableArray *array = [NSMutableArray new];

    [array addObject:messageStr];

    if (data_length > 0) {
        NSData *imageData = [[NSData alloc] initWithBytes:data length:data_length];
        UIImage *image = [UIImage imageWithData:imageData];
        [array addObject:image];
    }

    UIActivityViewController *controller =
            [[UIActivityViewController alloc] initWithActivityItems:array
                                              applicationActivities:nil];
    UIActivityViewController *weakController = controller;

    [UnityGetGLViewController() presentViewController:controller
                                             animated:true
                                           completion:nil];

    [controller setCompletionHandler:^(NSString *__nullable activityType,
            BOOL completed) {
        callback(onSharedActionPtr);
        weakController.completionHandler = nil;
    }];
}

void _openUrl(const char *link) {
    NSString *linkStr = [IosGoodiesUtils createNSStringFrom:link];
    NSURL *url = [NSURL URLWithString:linkStr];
    [[UIApplication sharedApplication] openURL:url];
}

}

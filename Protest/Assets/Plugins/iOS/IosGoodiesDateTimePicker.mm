#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "DateTimePicker.h"

extern "C" {

DateTimePicker *pickerController;

void _showDatePicker(void *callbackPtr, OnDateSelectedDelegate *onDateSelectedDelegate,
        void *cancelPtr, ActionVoidCallbackDelegate onCancel, int datePickerType) {
    pickerController = nil;
    pickerController = [[DateTimePicker alloc] initWithCallbackPtr:callbackPtr
                                            onDateSelectedDelegate:onDateSelectedDelegate
                                                       onCancelPtr:cancelPtr
                                                  onCancelDelegate:onCancel
                                                    datePickerType:datePickerType];
    [pickerController showPicker];
}
}

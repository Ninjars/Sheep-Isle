#import <AppKit/AppKit.h>
#include <atomic>
#include <float.h>
#include <stdint.h>

namespace
{
    constexpr int32_t ShowAction = 1;
    constexpr int32_t HideAction = 2;
    constexpr int32_t TogglePinAction = 4;
    constexpr int32_t QuitAction = 8;

    std::atomic<int32_t> pendingActions(0);
    NSWindow *playerWindow = nil;
    NSStatusItem *statusItem = nil;
    NSMenuItem *visibilityItem = nil;
    NSMenuItem *pinItem = nil;
    NSObject *menuTarget = nil;
    bool menuVisible = true;
    bool menuPinned = false;

    void OnMainSync(dispatch_block_t block)
    {
        if ([NSThread isMainThread])
        {
            block();
        }
        else
        {
            dispatch_sync(dispatch_get_main_queue(), block);
        }
    }

    NSWindow *FindPlayerWindow()
    {
        if (NSApp.keyWindow != nil)
        {
            return NSApp.keyWindow;
        }
        if (NSApp.mainWindow != nil)
        {
            return NSApp.mainWindow;
        }

        for (NSWindow *window in NSApp.orderedWindows)
        {
            if (window.isVisible && ![window isKindOfClass:NSPanel.class])
            {
                return window;
            }
        }
        for (NSWindow *window in NSApp.windows)
        {
            if (![window isKindOfClass:NSPanel.class])
            {
                return window;
            }
        }
        return nil;
    }

    NSScreen *NearestScreen(NSRect frame)
    {
        NSScreen *bestScreen = nil;
        CGFloat bestDistance = DBL_MAX;
        NSPoint center = NSMakePoint(NSMidX(frame), NSMidY(frame));

        for (NSScreen *screen in NSScreen.screens)
        {
            NSRect visible = screen.visibleFrame;
            CGFloat nearestX = MIN(MAX(center.x, NSMinX(visible)), NSMaxX(visible));
            CGFloat nearestY = MIN(MAX(center.y, NSMinY(visible)), NSMaxY(visible));
            CGFloat deltaX = center.x - nearestX;
            CGFloat deltaY = center.y - nearestY;
            CGFloat distance = deltaX * deltaX + deltaY * deltaY;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestScreen = screen;
            }
        }

        return bestScreen;
    }

    void UpdateMenuState()
    {
        visibilityItem.title = menuVisible ? @"Hide Sheep Isle" : @"Show Sheep Isle";
        pinItem.title = menuPinned ? @"Unpin" : @"Pin";
    }

    void DisposeInternal()
    {
        if (statusItem != nil)
        {
            [[NSStatusBar systemStatusBar] removeStatusItem:statusItem];
        }

        statusItem = nil;
        visibilityItem = nil;
        pinItem = nil;
        menuTarget = nil;
        playerWindow = nil;
        pendingActions.store(0);
    }
}

@interface SheepIsleMacMenuTarget : NSObject
- (void)toggleVisibility:(id)sender;
- (void)togglePin:(id)sender;
- (void)quit:(id)sender;
@end

@implementation SheepIsleMacMenuTarget
- (void)toggleVisibility:(id)sender
{
    pendingActions.fetch_or(menuVisible ? HideAction : ShowAction);
}

- (void)togglePin:(id)sender
{
    pendingActions.fetch_or(TogglePinAction);
}

- (void)quit:(id)sender
{
    pendingActions.fetch_or(QuitAction);
}
@end

extern "C"
{
    __attribute__((visibility("default"))) bool SheepIsleMac_Initialize()
    {
        __block bool initialized = false;
        OnMainSync(^{
            if (statusItem != nil && playerWindow != nil)
            {
                initialized = true;
                return;
            }

            DisposeInternal();
            playerWindow = FindPlayerWindow();
            if (playerWindow == nil)
            {
                return;
            }

            SheepIsleMacMenuTarget *target = [[SheepIsleMacMenuTarget alloc] init];
            NSStatusItem *item = [[NSStatusBar systemStatusBar]
                statusItemWithLength:NSVariableStatusItemLength];
            if (item == nil || item.button == nil)
            {
                DisposeInternal();
                return;
            }

            item.button.title = @"🐑";
            item.button.toolTip = @"Sheep Isle";

            NSMenu *menu = [[NSMenu alloc] initWithTitle:@"Sheep Isle"];
            menu.autoenablesItems = NO;

            NSMenuItem *visibility = [[NSMenuItem alloc]
                initWithTitle:@"Hide Sheep Isle"
                action:@selector(toggleVisibility:)
                keyEquivalent:@""];
            visibility.target = target;
            [menu addItem:visibility];

            NSMenuItem *pin = [[NSMenuItem alloc]
                initWithTitle:@"Pin"
                action:@selector(togglePin:)
                keyEquivalent:@""];
            pin.target = target;
            [menu addItem:pin];
            [menu addItem:NSMenuItem.separatorItem];

            NSMenuItem *quit = [[NSMenuItem alloc]
                initWithTitle:@"Quit Sheep Isle"
                action:@selector(quit:)
                keyEquivalent:@""];
            quit.target = target;
            [menu addItem:quit];

            item.menu = menu;
            menuTarget = target;
            statusItem = item;
            visibilityItem = visibility;
            pinItem = pin;
            menuVisible = playerWindow.isVisible;
            menuPinned = false;
            pendingActions.store(0);
            UpdateMenuState();
            initialized = true;
        });
        return initialized;
    }

    __attribute__((visibility("default"))) void SheepIsleMac_Dispose()
    {
        OnMainSync(^{
            DisposeInternal();
        });
    }

    __attribute__((visibility("default"))) int32_t SheepIsleMac_ConsumeActions()
    {
        return pendingActions.exchange(0);
    }

    __attribute__((visibility("default"))) void SheepIsleMac_SetMenuState(
        bool visible,
        bool pinned)
    {
        OnMainSync(^{
            menuVisible = visible;
            menuPinned = pinned;
            UpdateMenuState();
        });
    }

    __attribute__((visibility("default"))) bool SheepIsleMac_SetWindowVisible(bool visible)
    {
        __block bool applied = false;
        OnMainSync(^{
            if (playerWindow == nil)
            {
                return;
            }

            if (visible)
            {
                [playerWindow makeKeyAndOrderFront:nil];
                [playerWindow orderFrontRegardless];
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
                [NSApp activateIgnoringOtherApps:YES];
#pragma clang diagnostic pop
            }
            else
            {
                [playerWindow orderOut:nil];
            }

            applied = playerWindow.isVisible == visible;
        });
        return applied;
    }

    __attribute__((visibility("default"))) bool SheepIsleMac_IsWindowVisible()
    {
        __block bool visible = false;
        OnMainSync(^{
            visible = playerWindow != nil && playerWindow.isVisible;
        });
        return visible;
    }

    __attribute__((visibility("default"))) bool SheepIsleMac_ClampWindowToVisibleScreen()
    {
        __block bool contained = false;
        OnMainSync(^{
            if (playerWindow == nil)
            {
                return;
            }

            NSRect frame = playerWindow.frame;
            NSScreen *screen = NearestScreen(frame);
            if (screen == nil)
            {
                return;
            }

            NSRect visible = screen.visibleFrame;
            if (frame.size.width > visible.size.width ||
                frame.size.height > visible.size.height)
            {
                return;
            }

            frame.origin.x = MIN(
                MAX(frame.origin.x, NSMinX(visible)),
                NSMaxX(visible) - frame.size.width);
            frame.origin.y = MIN(
                MAX(frame.origin.y, NSMinY(visible)),
                NSMaxY(visible) - frame.size.height);
            [playerWindow setFrameOrigin:frame.origin];
            contained = NSContainsRect(visible, playerWindow.frame);
        });
        return contained;
    }
}

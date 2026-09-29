#import <AppKit/AppKit.h>
#include <dlfcn.h>
#include <stdint.h>
#include <stdio.h>

namespace
{
    int Fail(const char *message)
    {
        fprintf(stderr, "FAIL: %s\n", message);
        return 1;
    }

    template<typename T>
    T LoadSymbol(void *bundle, const char *name)
    {
        dlerror();
        void *symbol = dlsym(bundle, name);
        const char *error = dlerror();
        if (error != nullptr || symbol == nullptr)
        {
            fprintf(stderr, "Missing export %s: %s\n", name,
                error == nullptr ? "unknown error" : error);
            return nullptr;
        }

        return reinterpret_cast<T>(symbol);
    }

    void PumpEvents()
    {
        NSDate *deadline = [NSDate dateWithTimeIntervalSinceNow:0.05];
        while (true)
        {
            NSEvent *event = [NSApp nextEventMatchingMask:NSEventMaskAny
                untilDate:deadline
                inMode:NSDefaultRunLoopMode
                dequeue:YES];
            if (event == nil)
            {
                break;
            }

            [NSApp sendEvent:event];
        }
        [NSApp updateWindows];
    }
}

int main(int argc, const char *argv[])
{
    @autoreleasepool
    {
        if (argc != 2)
        {
            return Fail("expected the bridge bundle path");
        }

        [NSApplication sharedApplication];
        [NSApp setActivationPolicy:NSApplicationActivationPolicyAccessory];
        [NSApp finishLaunching];

        NSWindow *window = [[NSWindow alloc]
            initWithContentRect:NSMakeRect(120.0, 120.0, 320.0, 240.0)
            styleMask:NSWindowStyleMaskTitled
            backing:NSBackingStoreBuffered
            defer:NO];
        window.title = @"Sheep Isle bridge host";
        [window makeKeyAndOrderFront:nil];
        PumpEvents();
        if (!window.isVisible)
        {
            return Fail("host window did not become visible");
        }

        void *bundle = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
        if (bundle == nullptr)
        {
            fprintf(stderr, "Missing bridge bundle: %s\n", dlerror());
            return 20;
        }

        using Initialize = bool (*)();
        using Dispose = void (*)();
        using ConsumeActions = int32_t (*)();
        using SetMenuState = void (*)(bool, bool);
        using SetWindowVisible = bool (*)(bool);
        using IsWindowVisible = bool (*)();
        using ClampWindow = bool (*)();

        Initialize initialize = LoadSymbol<Initialize>(bundle, "SheepIsleMac_Initialize");
        Dispose dispose = LoadSymbol<Dispose>(bundle, "SheepIsleMac_Dispose");
        ConsumeActions consumeActions = LoadSymbol<ConsumeActions>(bundle,
            "SheepIsleMac_ConsumeActions");
        SetMenuState setMenuState = LoadSymbol<SetMenuState>(bundle,
            "SheepIsleMac_SetMenuState");
        SetWindowVisible setWindowVisible = LoadSymbol<SetWindowVisible>(bundle,
            "SheepIsleMac_SetWindowVisible");
        IsWindowVisible isWindowVisible = LoadSymbol<IsWindowVisible>(bundle,
            "SheepIsleMac_IsWindowVisible");
        ClampWindow clampWindow = LoadSymbol<ClampWindow>(bundle,
            "SheepIsleMac_ClampWindowToVisibleScreen");
        if (initialize == nullptr || dispose == nullptr || consumeActions == nullptr ||
            setMenuState == nullptr || setWindowVisible == nullptr ||
            isWindowVisible == nullptr || clampWindow == nullptr)
        {
            return 21;
        }

        if (!initialize())
        {
            return Fail("bridge initialization failed");
        }
        if (consumeActions() != 0)
        {
            dispose();
            return Fail("new bridge had pending actions");
        }

        setMenuState(true, false);
        setMenuState(false, true);

        if (!setWindowVisible(false))
        {
            dispose();
            return Fail("native hide request failed");
        }
        PumpEvents();
        if (window.isVisible || isWindowVisible())
        {
            dispose();
            return Fail("host window remained visible after hide");
        }

        if (!setWindowVisible(true))
        {
            dispose();
            return Fail("native show request failed");
        }
        PumpEvents();
        if (!window.isVisible || !isWindowVisible())
        {
            dispose();
            return Fail("host window did not return after show");
        }

        NSRect offscreen = window.frame;
        offscreen.origin = NSMakePoint(100000.0, -100000.0);
        [window setFrame:offscreen display:YES];
        if (!clampWindow())
        {
            dispose();
            return Fail("off-screen clamp request failed");
        }

        bool contained = false;
        for (NSScreen *screen in NSScreen.screens)
        {
            if (NSContainsRect(screen.visibleFrame, window.frame))
            {
                contained = true;
                break;
            }
        }
        if (!contained)
        {
            dispose();
            return Fail("clamped frame was not fully inside a visible screen");
        }

        dispose();
        dispose();
        if (setWindowVisible(false) || isWindowVisible())
        {
            return Fail("disposed bridge retained its player window");
        }

        [window orderOut:nil];
        puts("PASS: Sheep Isle AppKit bridge contract");
        return 0;
    }
}

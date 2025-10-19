# Canvas Example

This is a simple Android application that demonstrates the usage of AndroidDrawing library.

## Features

The example demonstrates various canvas operations:
- Drawing lines with different colors
- Drawing and filling rectangles
- Drawing and filling ellipses
- Text rendering (single and multiline)
- Anti-aliasing support

## Building

To build the example:

```bash
dotnet build CanvasExample.csproj
```

## Running

To run on an Android device or emulator:

```bash
dotnet build -t:Run -f net9.0-android
```

## Code Overview

The example creates a simple `MainActivity` with a custom `CanvasView` that overrides the `OnDraw` method to demonstrate various drawing operations using the AndroidDrawing `Graphics` class.

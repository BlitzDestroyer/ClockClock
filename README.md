# ClockClock

A simple Avalonia UI application that renders a digital clock using a grid of miniature analog clocks.  
Each digit (HH:MM:SS) is composed of multiple clock faces whose hands form line segments to visually represent numbers.

## Overview

- Built with **Avalonia UI**
- Displays the current time as `HH:MM:SS`
- Each digit is a **6×4 grid** of small clocks
- Clock hands rotate to form 7-segment digit shapes

## How It Works

- Each digit is represented by 24 clock segments
- Segments map to box-drawing characters (`┌ ┐ └ ┘ ─ │`)
- Each character corresponds to a pair of angles for the hour and minute hands
- Digits are updated by mapping the current time to predefined angle layouts

## Key Components

- `MainWindow`
    - Initializes digit grids and clock elements
    - Starts a timer to refresh the display
- `ClockDigit` (custom control)
    - Renders an individual analog clock
    - Configurable size, colors, border, and hand thickness
- Angle maps
    - `DigitToAngle`: digit → layout
    - `Angles`: layout symbol → hand angles

## Customization

Several visual properties can be adjusted in `MainWindow`:

- Clock size
- Border size
- Hand thickness
- Background, border, and hand colors
- Update interval

## Requirements

- .NET
- Avalonia UI

## Running

1. Open the solution in your IDE
2. Restore dependencies
3. Run the application

A window will open displaying a live, animated clock.

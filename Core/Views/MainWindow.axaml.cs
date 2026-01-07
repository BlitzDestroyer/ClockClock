using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Core.Controls;

namespace Core.Views;

public partial class MainWindow : Window
{
    private const int NumRowsPerDigit = 6;
    private const int NumColsPerDigit = 4;
    
    private const int DigitPartsCount = 24;
    private const int ClockSize = 50;
    private const double BorderSize = 3.75;
    private const double HandThickness = 1.25;
    private const string BackgroundColor = "#161616";
    private const string BorderColor = "#2e2e2e";
    private const string HandColor = "#e0ff55";

    private static readonly Dictionary<char, (int, int)> Angles = new()
    {
        ['┌'] = (90, 180),
        ['┐'] = (180, 270),
        ['┘'] = (270, 0),
        ['└'] = (90, 0),
        ['─'] = (90, 270),
        ['│'] = (180, 0),
        [' '] = (225, 225),
    };

    private static readonly Dictionary<int, char[]> DigitToAngle = new()
    {
        [0] = [
            '┌', '─', '─', '┐',
            '│', '┌', '┐', '│',
            '│', '│', '│', '│',
            '│', '│', '│', '│',
            '│', '└', '┘', '│',
            '└', '─', '─', '┘',
        ],
        [1] = [
            '┌', '─', '┐', ' ',
            '└', '┐', '│', ' ',
            ' ', '│', '│', ' ',
            ' ', '│', '│', ' ',
            '┌', '┘', '└', '┐',
            '└', '─', '─', '┘',
        ],
        [2] = [
            '┌', '─', '─', '┐',
            '└', '─', '┐', '│',
            '┌', '─', '┘', '│',
            '│', '┌', '─', '┘',
            '│', '└', '─', '┐',
            '└', '─', '─', '┘',
        ],
        [3] = [
            '┌', '─', '─', '┐',
            '└', '─', '┐', '│',
            ' ', '┌', '┘', '│',
            ' ', '└', '┐', '│',
            '┌', '─', '┘', '│',
            '└', '─', '─', '┘',
        ],
        [4] = [
            '┌', '┐', '┌', '┐',
            '│', '│', '│', '│',
            '│', '└', '┘', '│',
            '└', '─', '┐', '│',
            ' ', ' ', '│', '│',
            ' ', ' ', '└', '┘',
        ],
        [5] = [
            '┌', '─', '─', '┐',
            '│', '┌', '─', '┘',
            '│', '└', '─', '┐',
            '└', '─', '┐', '│',
            '┌', '─', '┘', '│',
            '└', '─', '─', '┘',
        ],
        [6] = [
            '┌', '─', '─', '┐',
            '│', '┌', '─', '┘',
            '│', '└', '─', '┐',
            '│', '┌', '┐', '│',
            '│', '└', '┘', '│',
            '└', '─', '─', '┘',
        ],
        [7] = [
            '┌', '─', '─', '┐',
            '└', '─', '┐', '│',
            ' ', ' ', '│', '│',
            ' ', ' ', '│', '│',
            ' ', ' ', '│', '│',
            ' ', ' ', '└', '┘',
        ],
        [8] = [
            '┌', '─', '─', '┐',
            '│', '┌', '┐', '│',
            '│', '└', '┘', '│',
            '│', '┌', '┐', '│',
            '│', '└', '┘', '│',
            '└', '─', '─', '┘',
        ],
        [9] = [
            '┌', '─', '─', '┐',
            '│', '┌', '┐', '│',
            '│', '└', '┘', '│',
            '└', '─', '┐', '│',
            '┌', '─', '┘', '│',
            '└', '─', '─', '┘',
        ],
    };
    
    private (int hour, int minute)[] _digitParts = new (int hour, int minute)[DigitPartsCount];
    private Grid[] _digits;
    
    public MainWindow()
    {
        InitializeComponent();

        _digits = [ HourDigitTens, HourDigitOnes, MinuteDigitTens, MinuteDigitOnes, SecondDigitTens, SecondDigitOnes ];
        const int restingAngle = 225;
        foreach(var digit in _digits)
        {
            for (var i = 0; i < DigitPartsCount; i++)
            {
                var clock = new ClockDigit
                {
                    Size = ClockSize,
                    BorderSize = BorderSize,
                    Background = Brush.Parse(BackgroundColor),
                    BorderColor = Brush.Parse(BorderColor),
                    HandColor = Brush.Parse(HandColor),
                    HandThickness = HandThickness,
                    HourDegree = restingAngle,
                    MinuteDegree = restingAngle,
                };
                
                Grid.SetRow(clock, i / NumColsPerDigit);
                Grid.SetColumn(clock, i % NumColsPerDigit);
                digit.Children.Add(clock);
            }
        }

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500),
        };

        timer.Tick += (_, _) =>
        {
            UpdateClock();
            
            InvalidateVisual();
        };
        
        timer.Start();
    }

    private void UpdateClock()
    {
        var currentTime = DateTime.Now;
        var hourTens = currentTime.Hour / 10;
        var hourOnes = currentTime.Hour % 10;
        var minuteTens = currentTime.Minute / 10;
        var minuteOnes = currentTime.Minute % 10;
        var secondTens = currentTime.Second / 10;
        var secondOnes = currentTime.Second % 10;
        Console.WriteLine($"{hourTens}{hourOnes}:{minuteTens}{minuteOnes}:{secondTens}{secondOnes}");
        UpdateAngle(_digits[0], hourTens);
        UpdateAngle(_digits[1], hourOnes);
        UpdateAngle(_digits[2], minuteTens);
        UpdateAngle(_digits[3], minuteOnes);
        UpdateAngle(_digits[4], secondTens);
        UpdateAngle(_digits[5], secondOnes);
    }

    // private static void UpdateAngle(Grid digit, int value)
    // {
    //     var angles = DigitToAngle[value];
    //     for (var i = 0; i < DigitPartsCount; i++)
    //     {
    //         var clock = (ClockDigit) digit.Children[i];
    //         var (hour, minute) = Angles[angles[i]];
    //         clock.HourDegree = hour;
    //         clock.MinuteDegree = minute;
    //     }
    // }
    
    private static void UpdateAngle(Grid digit, int value, bool alwaysAnimate = false)
    {
        var angles = DigitToAngle[value];
        for (var i = 0; i < DigitPartsCount; i++)
        {
            var clock = (ClockDigit)digit.Children[i];
            var (hour, minute) = Angles[angles[i]];

            if (!alwaysAnimate && !(Math.Abs(clock.HourDegree - hour) > 0.001) &&
                !(Math.Abs(clock.MinuteDegree - minute) > 0.001))
            {
                continue;
            }

            clock.HourDegree = hour;
            clock.MinuteDegree = minute;
        }
    }

}
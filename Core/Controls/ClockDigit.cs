using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Animation.Easings;

namespace Core.Controls;

public class ClockDigit : Control
{
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<ClockDigit, double>(nameof(Size), 100);

    public static readonly StyledProperty<double> BorderSizeProperty =
        AvaloniaProperty.Register<ClockDigit, double>(nameof(BorderSize), 5);

    public static readonly StyledProperty<IBrush> BackgroundProperty =
        AvaloniaProperty.Register<ClockDigit, IBrush>(nameof(Background), Brush.Parse("#161616"));

    public static readonly StyledProperty<IBrush> BorderColorProperty =
        AvaloniaProperty.Register<ClockDigit, IBrush>(nameof(BorderColor), Brush.Parse("#2e2e2e"));

    public static readonly StyledProperty<IBrush> HandColorProperty =
        AvaloniaProperty.Register<ClockDigit, IBrush>(nameof(HandColor), Brush.Parse("#e0ff55"));

    public static readonly StyledProperty<double> HandThicknessProperty =
        AvaloniaProperty.Register<ClockDigit, double>(nameof(HandThickness), 5);

    public static readonly StyledProperty<double> HourDegreeProperty =
        AvaloniaProperty.Register<ClockDigit, double>(nameof(HourDegree), 90);

    public static readonly StyledProperty<double> MinuteDegreeProperty =
        AvaloniaProperty.Register<ClockDigit, double>(nameof(MinuteDegree), 180);

    private double _animatedHour;
    private double _animatedMinute;

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public double BorderSize
    {
        get => GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public IBrush Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public IBrush BorderColor
    {
        get => GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public IBrush HandColor
    {
        get => GetValue(HandColorProperty);
        set => SetValue(HandColorProperty, value);
    }

    public double HandThickness
    {
        get => GetValue(HandThicknessProperty);
        set => SetValue(HandThicknessProperty, value);
    }

    public double HourDegree
    {
        get => GetValue(HourDegreeProperty);
        set => SetValue(HourDegreeProperty, value);
    }

    public double MinuteDegree
    {
        get => GetValue(MinuteDegreeProperty);
        set => SetValue(MinuteDegreeProperty, value);
    }

    public ClockDigit()
    {
        HourDegreeProperty.Changed.AddClassHandler<ClockDigit>((x, e) => x.OnDegreeChanged(true, e));
        MinuteDegreeProperty.Changed.AddClassHandler<ClockDigit>((x, e) => x.OnDegreeChanged(false, e));

        // initialize visual values
        _animatedHour = HourDegree;
        _animatedMinute = MinuteDegree;
    }

    private void OnDegreeChanged(bool isHour, AvaloniaPropertyChangedEventArgs e)
    {
        if (e is not { OldValue: double oldVal, NewValue: double newVal })
        {
            return;
        }

        if (Math.Abs(oldVal - newVal) > 0.001)
        {
            _ = AnimateHand(isHour, oldVal, newVal, 200);
        }
    }

    private async Task AnimateHand(bool isHour, double from, double to, int durationMs)
    {
        var diff = ((to - from + 540) % 360) - 180; // shortest path
        var start = from;
        var end = from + diff;

        var startTime = DateTime.UtcNow;
        var duration = TimeSpan.FromMilliseconds(durationMs);
        var easing = new CubicEaseOut();

        while (true)
        {
            var t = (DateTime.UtcNow - startTime).TotalMilliseconds / duration.TotalMilliseconds;
            if (t >= 1.0)
            {
                break;
            }

            var eased = easing.Ease(t);
            var angle = start + diff * eased;

            if (isHour)
            {
                _animatedHour = angle;
            }
            else
            {
                _animatedMinute = angle;
            }

            InvalidateVisual();
            await Task.Delay(16); // ~60fps
        }

        if (isHour)
        {
            _animatedHour = end % 360;
        }
        else
        {
            _animatedMinute = end % 360;
        }

        InvalidateVisual();
    }

    protected override Size MeasureOverride(Size availableSize) => new(Size, Size);
    protected override Size ArrangeOverride(Size finalSize) => new(Size, Size);

    public sealed override void Render(DrawingContext context)
    {
        var s = Size;
        var b = BorderSize;
        var center = new Point(s / 2, s / 2);
        var innerSize = s - 2 * b;

        // Border + background
        context.FillRectangle(BorderColor, new Rect(0, 0, s, s));
        context.FillRectangle(Background, new Rect(b, b, innerSize, innerSize));

        // Use _animatedHour/_animatedMinute for drawing
        DrawHand(context, center, innerSize / 2, HandThickness, HandColor, _animatedHour);
        DrawHand(context, center, innerSize / 2, HandThickness, HandColor, _animatedMinute);

        base.Render(context);
    }

    private static void DrawHand(DrawingContext context, Point center, double length, double thickness, IBrush color, double angleDegrees)
    {
        var handRect = new Rect(center.X - (thickness / 2), center.Y - length, thickness, length);

        using var translation1 = context.PushTransform(Matrix.CreateTranslation(center.X, center.Y));
        using var rotation = context.PushTransform(Matrix.CreateRotation(Math.PI * angleDegrees / 180.0));
        using var translation2 = context.PushTransform(Matrix.CreateTranslation(-center.X, -center.Y));
        context.FillRectangle(color, handRect);
    }
}

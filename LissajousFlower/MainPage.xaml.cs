﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace LissajousFlower
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private DateTime start = DateTime.Now;

        private void Canvas_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var seconds = (DateTime.Now - start).TotalSeconds;
            var speed1 = this.SpeedSlider1.Value;
            var delta1 = seconds * speed1;
            var speed2 = this.SpeedSlider2.Value;
            var delta2 = seconds * speed2;
            var loops1 = this.LoopsSlider1.Value;
            var loops2 = this.LoopsSlider2.Value * loops1;
            var deg2rad = Math.PI / 180;
            var ippd = 1 / this.PointsPerDegreeSlider.Value;
            var drawLines = this.RadioButtonDrawLines.IsChecked == true;
            var t = (float)(this.LineThicknessSlider.Value * (drawLines ? 1 : .5));

            var w = this.canvas.ActualWidth;
            var h = this.canvas.ActualHeight;
            var w2 = w / 2;
            var h2 = h / 2;
            var r1 = Math.Min(w, h) * 0.4;
            var r2 = r1 * 0.24 * (1 - this.LoopSizeRatioSlider.Value);
            var r3 = r1 * 0.24 * this.LoopSizeRatioSlider.Value;
            float x1 = 0;
            float y1 = 0;

            args.DrawingSession.Clear(Colors.Black);

            for (double a = 0; a <= 360.0001; a += ippd)
            {
                float x = (float)(w2 + r1 * Math.Sin(a * deg2rad) + r2 * Math.Sin((a + delta1) * loops1 * deg2rad) + r3 * Math.Sin((a + delta2) * loops2 * deg2rad));
                float y = (float)(h2 + r1 * Math.Cos(a * deg2rad) + r2 * Math.Cos((a + delta1) * loops1 * deg2rad) + r3 * Math.Cos((a + delta2) * loops2 * deg2rad));

                if (a != 0)
                {
                    var c = a * 255 / 360.0001;
                    double r, g, b;
                    var q = 255 / 60;

                    if (a <= 60)
                    {
                        r = 255;
                        g = a * q;
                        b = 0;
                    }
                    else if (a <= 120)
                    {
                        r = 255 - (a - 60) * q;
                        g = 255;
                        b = 0;
                    }
                    else if (a <= 180)
                    {
                        r = 0;
                        g = 255;
                        b = (a - 120) * q;
                    }
                    else if (a <= 240)
                    {
                        r = 0;
                        g = 255 - (a - 180) * q;
                        b = 255;
                    }
                    else if (a <= 300)
                    {
                        r = (a - 240) * q;
                        g = 0;
                        b = 255;
                    }
                    else
                    {
                        r = 255;
                        g = 0;
                        b = 255 - (a - 300) * q;
                    }

                    if (drawLines)
                        args.DrawingSession.DrawLine(x1, y1, x, y, Color.FromArgb(255, (byte)r, (byte)g, (byte)b), t);
                    else
                        args.DrawingSession.FillEllipse(x, y, t, t, Color.FromArgb(255, (byte)r, (byte)g, (byte)b));

                }

                x1 = x;
                y1 = y;
            }

            if (CheckBoxDrawMoire.IsChecked == true)
            {
                DrawMoiredLines(args, delta1, w2, h2, ref x1, ref y1);
            }

            canvas.Invalidate();
            //args.DrawingSession.DrawText("Hello, world!", 100, 100, Colors.Yellow);
            //args.DrawingSession.
        }

        private static void DrawMoiredLines(CanvasDrawEventArgs args, double delta1, double w2, double h2, ref float x1, ref float y1)
        {
            int numOfLines = (int)(w2 / 2);
            var sin = Math.Sin(delta1 / 100);
            var cos = Math.Cos(delta1 / 100);
            var t = (float)(.5 * w2 / numOfLines);

            for (int i = 0; i < numOfLines; i += 1)
            {
                x1 = (float)(w2 / 2 + ((double)i / numOfLines) * w2);
                y1 = (float)(h2 / 2);
                var x2 = x1;
                var y2 = (float)(h2 * 3 / 2);

                args.DrawingSession.DrawLine(x1, y1, x2, y2, Colors.White, t);

                x1 -= (float)w2;
                y1 -= (float)h2;
                x2 -= (float)w2;
                y2 -= (float)h2;

                float xx1 = (float)(cos * x1 - sin * y1);
                float yy1 = (float)(sin * x1 + cos * y1);
                float xx2 = (float)(cos * x2 - sin * y2);
                float yy2 = (float)(sin * x2 + cos * y2);
                xx1 += (float)w2;
                yy1 += (float)h2;
                xx2 += (float)w2;
                yy2 += (float)h2;
                args.DrawingSession.DrawLine(xx1, yy1, xx2, yy2, Colors.White, t);
            }
        }

        private void Allow_unclosed_loops_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.CheckBoxAllowUnclosedLoops.IsChecked == true)
            {
                this.LoopsSlider1.StepFrequency = .1;
                this.LoopsSlider2.StepFrequency = .1;
            }
            else
            {
                this.LoopsSlider1.StepFrequency = 1;
                this.LoopsSlider2.StepFrequency = 1;
            }
        }
    }
}

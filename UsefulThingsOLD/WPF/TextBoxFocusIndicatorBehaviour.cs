using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Changes TextBox BorderBrush when item is in focus.
    /// </summary>
    public static class TextBoxFocusIndicatorBehaviour
    {
        public static readonly DependencyProperty IsFocusIndicationEnabled =
            DependencyProperty.RegisterAttached(nameof(IsFocusIndicationEnabled), typeof(bool), typeof(TextBoxFocusIndicatorBehaviour),
                new UIPropertyMetadata(true, OnIsFocusIndicationChanged));


        public static readonly DependencyProperty IndicationColour =
            DependencyProperty.RegisterAttached(nameof(IndicationColour), typeof(Brush), typeof(TextBoxFocusIndicatorBehaviour),
                new UIPropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty IndicationThickness =
            DependencyProperty.RegisterAttached(nameof(IndicationThickness), typeof(Thickness), typeof(TextBoxFocusIndicatorBehaviour),
                new UIPropertyMetadata(3));

        public static readonly DependencyProperty OriginalBorderBrush =
            DependencyProperty.RegisterAttached(nameof(OriginalBorderBrush), typeof(Brush), typeof(TextBoxFocusIndicatorBehaviour));

        public static readonly DependencyProperty OriginalIndicationThickness =
            DependencyProperty.RegisterAttached(nameof(OriginalIndicationThickness), typeof(Thickness), typeof(TextBoxFocusIndicatorBehaviour));


        public static Thickness GetOrigIndicationThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(OriginalIndicationThickness);
        }

        public static void SetOrigIndicationThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(OriginalIndicationThickness, value);
        }

        public static Brush GetOrigIndicationColour(DependencyObject obj)
        {
            return (Brush)obj.GetValue(OriginalBorderBrush);
        }

        public static void SetOrigIndicationColour(DependencyObject obj, Brush value)
        {
            obj.SetValue(OriginalBorderBrush, value);
        }




        public static Thickness GetIndicationThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(IndicationThickness);
        }

        public static void SetIndicationThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(IndicationThickness, value);
        }

        public static Brush GetIndicationColour(DependencyObject obj)
        {
            return (Brush)obj.GetValue(IndicationColour);
        }

        public static void SetIndicationColour(DependencyObject obj, Brush value)
        {
            obj.SetValue(IndicationColour, value);
        }

        public static bool GetIsFocusEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusIndicationEnabled);
        }

        public static void SetIsFocusEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusIndicationEnabled, value);

            if(obj is TextBox box)
            {
                if (value)
                {
                    box.GotFocus += OnInputTextGotFocus;
                    box.LostFocus += OnInputTextBoxLostFocus;
                }
                else
                {
                    box.GotFocus -= OnInputTextGotFocus;
                    box.LostFocus -= OnInputTextBoxLostFocus;
                }
            }
        }

        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is TextBox box)
            {
                box.BorderBrush = GetOrigIndicationColour(box);
                box.BorderThickness = GetOrigIndicationThickness(box);
            }
        }

        private static void OnInputTextGotFocus(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is TextBox box)
            {
                // Save original details
                SetOrigIndicationColour(box, box.BorderBrush);
                SetOrigIndicationThickness(box, box.BorderThickness);

                // Set indications
                box.BorderThickness = GetIndicationThickness(box);
                box.BorderBrush = GetIndicationColour(box);
                //box.Effect = 
            }
        }

        public static void OnIsFocusIndicationChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(sender is TextBox box)
                SetIsFocusEnabled(box, (bool)e.NewValue);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UsefulThings.WPF
{
    // Significantly from https://blogs.msdn.microsoft.com/dgartner/2009/11/11/wpf-attached-behaviour-example-watermark-text/

    /// <summary>
    /// Enables watermark text on textboxes.
    /// </summary>
    public static class TextBoxWatermarkBehaviour
    {
        /// <summary>
        /// Indicates whether the watermark is shown on a textbox when empty and focus lost.
        /// </summary>
        public static readonly DependencyProperty IsWaterMarkEnabled =
            DependencyProperty.RegisterAttached(nameof(IsWaterMarkEnabled), typeof(bool), typeof(TextBoxWatermarkBehaviour),
                new UIPropertyMetadata(true, OnIsWatermarkEnabled));

        /// <summary>
        /// Specifies the watermark text shown on an empty textbox when not in focus.
        /// </summary>
        public static readonly DependencyProperty WaterMarkText =
            DependencyProperty.RegisterAttached(nameof(WaterMarkText), typeof(string), typeof(TextBoxWatermarkBehaviour),
                new UIPropertyMetadata("Search...", OnWatermarkTextChanged));

        /// <summary>
        /// Indicates colour for the watermark. Initially Brushes.LightGrey
        /// </summary>
        public static readonly DependencyProperty WaterMarkColour =
            DependencyProperty.RegisterAttached(nameof(WaterMarkColour), typeof(Brush), typeof(TextBoxWatermarkBehaviour),
                new UIPropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Indicates foreground. Initially probably black.
        /// </summary>
        public static readonly DependencyProperty OriginalForeground =
            DependencyProperty.RegisterAttached(nameof(OriginalForeground), typeof(Brush), typeof(TextBoxWatermarkBehaviour));


        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WaterMarkText);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WaterMarkText, value);
        }

        public static bool GetIsWatermarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWaterMarkEnabled);
        }

        public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWaterMarkEnabled, value);
        }

        public static Brush GetWaterMarkColour(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WaterMarkColour);
        }

        public static void SetWaterMarkColour(DependencyObject obj, Brush colour)
        {
            obj.SetValue(WaterMarkColour, colour);
        }

        public static Brush GetOriginalForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(OriginalForeground);
        }

        public static void SetOriginalForeground(DependencyObject obj, Brush colour)
        {
            obj.SetValue(OriginalForeground, colour);
        }

        private static void OnWatermarkTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox box)
                box.Text = (string)e.NewValue;
        }

        private static void OnIsWatermarkEnabled(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                bool isEnabled = (bool)e.NewValue;
                if (isEnabled)
                {
                    box.GotFocus += OnInputTextBoxGotFocus;
                    box.LostFocus += OnInputTextBoxLostFocus;
                }
                else
                {
                    box.GotFocus -= OnInputTextBoxGotFocus;
                    box.LostFocus -= OnInputTextBoxLostFocus;
                }
            }
        }

        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox box && string.IsNullOrEmpty(box.Text))
            {
                box.Text = GetWatermarkText(box);

                // Save original foreground if required.
                if (box.Foreground != GetWaterMarkColour(box))
                    SetOriginalForeground(box, box.Foreground);

                box.Foreground = GetWaterMarkColour(box);
            }
        }

        private static void OnInputTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox box && box.Text == GetWatermarkText(box))
            {
                box.Text = string.Empty;

                var originalForeground = GetOriginalForeground(box);
                if (originalForeground != null)
                    box.Foreground = originalForeground;
            }
        }
    }
}

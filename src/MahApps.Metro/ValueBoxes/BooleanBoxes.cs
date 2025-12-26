// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.ValueBoxes
{
    /// <summary>
    /// Helps boxing Boolean values.
    /// </summary>
    public static class BooleanBoxes
    {
        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "true" value.
        /// </summary>
        public static readonly object TrueBox = true;

        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "false" value.
        /// </summary>
        public static readonly object FalseBox = false;

        /// <summary>
        /// Returns a boxed representation for the specified Boolean value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns>A boxed <see cref="bool"/> value.</returns>
        public static object Box(bool value) => value ? TrueBox : FalseBox;

        /// <summary>
        /// Returns a boxed value for the specified nullable <paramref name="value"/>.
        /// </summary>
        /// <returns>A boxed nullable <see cref="bool"/> value.</returns>
        public static object? Box(bool? value)
        {
            if (value.HasValue)
            {
                return value.Value
                    ? TrueBox
                    : FalseBox;
            }

            return null;
        } 
    }
    public static class UsedBoxes
    {

        public static object VerticalBox = Orientation.Vertical;

        public static object HorizontalBox = Orientation.Horizontal;

        public static object VisibleBox = Visibility.Visible;

        public static object CollapsedBox = Visibility.Collapsed;

        public static object HiddenBox = Visibility.Hidden;

        public static object Double01Box = .1;

        public static object Double0Box = .0;

        public static object Double1Box = 1.0;

        public static object Double10Box = 10.0;

        public static object Double20Box = 20.0;

        public static object Double100Box = 100.0;

        public static object Double200Box = 200.0;

        public static object Double300Box = 300.0;

        public static object DoubleNeg1Box = -1.0;

        public static object Int0Box = 0;

        public static object Int1Box = 1;

        public static object Int2Box = 2;

        public static object Int5Box = 5;

        public static object Int99Box = 99;
    }
}
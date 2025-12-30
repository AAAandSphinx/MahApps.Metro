// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    public static class Extensions
    {
        public static T Invoke<T>([NotNull] this DispatcherObject dispatcherObject, [NotNull] Func<T> func)
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                return func();
            }
            else
            {
                return dispatcherObject.Dispatcher.Invoke(func);
            }
        }

        public static void Invoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action invokeAction)
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                invokeAction();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(invokeAction);
            }
        }

        /// <summary> 
        ///   Executes the specified action asynchronously with the DispatcherPriority.Background on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object where the action runs.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        /// <param name="priority">The dispatcher priority.</param> 
        public static void BeginInvoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, invokeAction);
        }

        public static void BeginInvoke<T>([NotNull] this T dispatcherObject, [NotNull] Action<T> invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
            where T : DispatcherObject
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, new Action(() => invokeAction(dispatcherObject)));
        }

        /// <summary> 
        ///   Executes the specified action if the element is loaded or at the loaded event if it's not loaded.
        /// </summary>
        /// <param name="element">The element where the action should be run.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        public static void ExecuteWhenLoaded([NotNull] this FrameworkElement element, [NotNull] Action invokeAction)
        {
            if (element.IsLoaded)
            {
                element.Invoke(invokeAction);
            }
            else
            {
                void ElementLoaded(object o, RoutedEventArgs a)
                {
                    element.Loaded -= ElementLoaded;
                    element.Invoke(invokeAction);
                }

                element.Loaded += ElementLoaded;
            }
        }

        #region EnumerableExtension
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            var enumerable = source as IList<TSource> ?? source.ToList();
            foreach (var item in enumerable)
            {
                predicate.Invoke(item);
            }

            return enumerable;
        }

        public static IEnumerable<TSource> DoWithIndex<TSource>(this IEnumerable<TSource> source, Action<TSource, int> predicate)
        {
            var enumerable = source as IList<TSource> ?? source.ToList();
            for (var i = 0; i < enumerable.Count; i++)
            {
                predicate.Invoke(enumerable[i], i);
            }

            return enumerable;
        }
        #endregion


        #region UIElementExtension
        /// <summary>
        ///     显示元素
        /// </summary>
        /// <param name="element"></param>
        public static void Show(this UIElement element) => element.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);

        /// <summary>
        ///     显示元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="show"></param>
        public static void Show(this UIElement element, bool show) => element.SetCurrentValue(UIElement.VisibilityProperty, show ? Visibility.Visible : Visibility.Collapsed);

        /// <summary>
        ///     不现实元素，但保留空间
        /// </summary>
        /// <param name="element"></param>
        public static void Hide(this UIElement element) => element.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);

        /// <summary>
        ///     不显示元素，且不保留空间
        /// </summary>
        /// <param name="element"></param>
        public static void Collapse(this UIElement element) => element.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        #endregion






    }
}
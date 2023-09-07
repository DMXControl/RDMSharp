using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace RDMSharp
{
    internal static class EventTools
    {
        [DebuggerHidden]
        public static int InvokeFailSafe(this EventHandler @event, object sender, EventArgs args)
        {
            return InvokeFailSaveGeneric(@event, a => a(sender, args));
        }

        /// <summary>
        /// Calles the Invoke in a safe form, where even if one of the Subscribed EventHandlers
        /// fails with an exception, the other subscribed methods still get called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="args"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int InvokeFailSafe(this Action @event)
        {
            return InvokeFailSaveGeneric(@event, a => a());
        }

        /// <summary>
        /// Calles the Invoke in a safe form, where even if one of the Subscribed EventHandlers
        /// fails with an exception, the other subscribed methods still get called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="args"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int InvokeFailSafe<T>(this Action<T> @event, T args)
        {
            return InvokeFailSaveGeneric(@event, a => a(args));
        }

        /// <summary>
        /// Calles the Invoke in a safe form, where even if one of the Subscribed EventHandlers
        /// fails with an exception, the other subscribed methods still get called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int InvokeFailSafe<T>(this EventHandler<T> @event, object sender, T args)
        {
            return InvokeFailSaveGeneric(@event, a => a(sender, args));
        }

        /// <summary>
        /// Calles the Invoke in a safe form, where even if one of the Subscribed EventHandlers
        /// fails with an exception, the other subscribed methods still get called.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int InvokeFailSafe(this PropertyChangedEventHandler @event, object sender, PropertyChangedEventArgs args)
        {
            return InvokeFailSaveGeneric(@event, a => a(sender, args));
        }

        /// <summary>
        /// Calles the Invoke in a safe form, where even if one of the Subscribed EventHandlers
        /// fails with an exception, the other subscribed methods still get called.
        /// </summary>
        /// <param name="delegate"></param>
        /// <param name="elog"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static IReadOnlyList<object> InvokeFailSafe(this Delegate @delegate, params object[] values)
        {
            return InvokeFailSaveGeneric(@delegate, a => a.DynamicInvoke(values));
        }

        [DebuggerHidden]
        public static int InvokeFailSaveGeneric<TDelegate>(TDelegate @delegate, Action<TDelegate> invoker) where TDelegate : Delegate
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            if (@delegate == null) return 0;

            var target = @delegate.GetInvocationList();
            int ret = 0;
            for (int i = 0; i < target.Length; i++)
            {
                TDelegate del = (TDelegate)target[i];
                if (del == null) continue;

                try
                {
                    invoker(del);
                    ret++;
                }
                catch
                {
                }
            }
            return ret;
        }

        [DebuggerHidden]
        public static IReadOnlyList<TReturn> InvokeFailSaveGeneric<TDelegate, TReturn>(TDelegate @delegate, Func<TDelegate, TReturn> invoker) where TDelegate : Delegate
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            if (@delegate == null) return null;

            var target = @delegate.GetInvocationList();
            var ret = new List<TReturn>(target.Length);
            for (int i = 0; i < target.Length; i++)
            {
                TDelegate del = (TDelegate)target[i];
                if (del == null) continue;

                try
                {
                    var x = invoker(del);
                    ret.Add(x);
                }
                catch
                {
                }
            }
            return ret;
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SprintPlanner.FrameworkWPF
{
    public static class Extensions
    {
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static string StripLeft(this string value, int length)
        {
            return value.Substring(length, value.Length - length);
        }

        public static void Raise(this PropertyChangedEventHandler eventHandler, object source, string propertyName)
        {
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(propertyName));
        }

        public static void Raise(this EventHandler eventHandler, object source)
        {
            eventHandler?.Invoke(source, EventArgs.Empty);
        }

        public static void Register(this INotifyPropertyChanged model, string propertyName, Action whenChanged)
        {
            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == propertyName)
                    whenChanged();
            };
        }
    }
}

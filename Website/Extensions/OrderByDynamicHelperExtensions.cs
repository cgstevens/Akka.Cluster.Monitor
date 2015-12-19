using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Website.Extensions
{
    public static class OrderByDynamicHelper
    {
        private static readonly Hashtable Accessors = new Hashtable();

        private static readonly Hashtable CallSites = new Hashtable();

        private static CallSite<Func<CallSite, object, object>> GetCallSiteLocked(string name)
        {
            var callSite = (CallSite<Func<CallSite, object, object>>)CallSites[name];
            if (callSite == null)
            {
                CallSites[name] = callSite = CallSite<Func<CallSite, object, object>>.Create(
                            Binder.GetMember(CSharpBinderFlags.None, name, typeof(OrderByDynamicHelper),
                            new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
            }
            return callSite;
        }

        internal static Func<dynamic, object> GetAccessor(string name)
        {
            var accessor = (Func<dynamic, object>)Accessors[name];
            if (accessor == null)
            {
                lock (Accessors)
                {
                    accessor = (Func<dynamic, object>)Accessors[name];
                    if (accessor == null)
                    {
                        if (name.IndexOf('.') >= 0)
                        {
                            string[] props = name.Split('.');
                            CallSite<Func<CallSite, object, object>>[] arr = Array.ConvertAll(props, GetCallSiteLocked);
                            accessor = target =>
                            {
                                var val = (object)target;
                                return arr.Aggregate(val, (current, cs) => cs.Target(cs, current));
                            };
                        }
                        else
                        {
                            var callSite = GetCallSiteLocked(name);
                            accessor = target => callSite.Target(callSite, (object)target);
                        }
                        Accessors[name] = accessor;
                    }
                }
            }
            return accessor;
        }

        public static IOrderedEnumerable<dynamic> OrderBy(this IEnumerable<dynamic> source, string property)
        {
            return source.OrderBy(GetAccessor(property), Comparer<object>.Default);
        }
        public static IOrderedEnumerable<dynamic> OrderByDescending(this IEnumerable<dynamic> source, string property)
        {
            return source.OrderByDescending(GetAccessor(property), Comparer<object>.Default);
        }
        public static IOrderedEnumerable<dynamic> ThenBy(this IOrderedEnumerable<dynamic> source, string property)
        {
            return source.ThenBy(GetAccessor(property), Comparer<object>.Default);
        }
        public static IOrderedEnumerable<dynamic> ThenByDescending(this IOrderedEnumerable<dynamic> source, string property)
        {
            return source.ThenByDescending(GetAccessor(property), Comparer<object>.Default);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace AOMapper.Extensions
{    
    public static class ObjectExtensions
    {
        /// <summary>
        /// <para>The same as to do 'object as T' but in more clear manner</para>
        /// <para>Doing in this way allows us to continue 'dotting' instead of doing something like (obj as T).property</para>
        /// </summary>        
        [DebuggerStepThrough]
        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// Allows to manipulate the object in the chain manner.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T Apply<T>(this T source, Action<T> action) where T : class
        {
            action(source);
            return source;
        }

        /// <summary>
        /// Determines whether the specified collection has more them one element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsMultiple<T>(this IEnumerable<T> collection)
        {
            bool result;
            using (var enumerator = collection.GetEnumerator())
            {
                result = enumerator.MoveNext() && enumerator.MoveNext();
            }
            return result;
        }

        [DebuggerStepThrough]
        public static string MemberName<T, TR>(this T o, Expression<Func<T, TR>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null) throw new MissingMemberException();

            return body.Member.Name;
        }

        [DebuggerStepThrough]
        public static void For(this int number, Action<int> action)
        {
            for (int i = 0; i < number; i++)
            {
                action(i);
            }
        }

        [DebuggerStepThrough]
        public static object Create(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        [DebuggerStepThrough]
        public static object Create(this Type type, params object[] @params)
        {
            return Activator.CreateInstance(type, @params);
        }

        [DebuggerStepThrough]
        public static T Create<T>(this Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        [DebuggerStepThrough]
        public static T Create<T>(this Type type, params object[] @params)
        {
            return (T)Activator.CreateInstance(type, @params);
        }
    }
}

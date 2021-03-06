﻿using System;
using System.Reflection;

namespace AOMapper.Interfaces
{
    public interface IAccessObject
    {
        object Get(object obj);
        void Set(object obj, object value);

        Func<T, TR> GetGetter<T, TR>();
        Action<T, TR> GetSetter<T, TR>();
        Delegate GetterDelegate { get; }
        Delegate SetterDelegate { get; }

        TR GetGeneric<T, TR>(T obj) where TR : class;
        void SetGeneric<T, TR>(T obj, TR value);
        bool CanSet { get; }
        bool CanGet { get; }
        bool CanCreate { get; }

        PropertyInfo PropertyInfo { get; }
    }
}
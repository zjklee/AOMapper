﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AOMapper.Data.Keys;
using AOMapper.Extensions;
using AOMapper.Interfaces;
using AOMapper.Resolvers;

namespace AOMapper.Data
{
    public class MappingRoute : 
        IEnumerable<MappingRoute>
    {
        internal readonly Dictionary<StringKey, MappingRoute> _routes
            = new Dictionary<StringKey, MappingRoute>();

        internal DataProxy<object> _dataProxy;
        internal DataProxy<object> _routeDataProxy;
        protected PropertyInfo _propertyInfo;

        internal Delegate _getDelegate;
        internal Delegate _getConverteDelegate;
        internal Delegate _setDelegate;        

        public Type Root { get; protected set; }
        public Type Type { get; protected set; }
        public StringKey Key { get; protected set; }
        public MappingRoute Parent { get; protected set; }

        public Delegate GetDelegate { get; internal set; }
        public Delegate GetConverteDelegate { get; internal set; }
        public Delegate SetDelegate { get; internal set; }

        public Delegate ChildGetDelegate;

        public Resolver Resolver { get; protected set; }

        public IMap Map { get; protected set; }

        public MappingRoute DestinationRoute { get; set; }

        public MappingRoute SourceRoute { get; set; }

        protected bool Initialized { get; set; }

        public Type ObjectType { get; protected set; }

        public string Route { get; set; }

        public MappingRoute(IMap map, Type type = null)
        {
            Map = map;
            Key = string.Empty;
            if (type != null)
            {
                Root = type;
                Type = type;
                _dataProxy = DataProxy.Create(type);
            }            
        }

        #region Methods

        public MappingRoute GetRoot()
        {
            var root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }

        public MappingRoute GetRoute(string key)
        {
            char separator = '/';
            Map.ConfigMap(config => separator = config.Separator);
            var elements = key.Split(separator);

            MappingRoute result = this;
            foreach (var element in elements)
            {
                if (!result._routes.TryGetValue(element, out result))
                    return null;
            }

            if (ReferenceEquals(result, this))
                return null;

            return result;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<MappingRoute> GetEnumerator()
        {
            return _routes.Select(o => o.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Statics

        public static MappingRoute Create<T, TObject>(MappingRoute route, string key, Resolver resolver = null, IEnumerable<KeyValuePair<string, string>> registredPaths = null)
        {
            return MappingRoute<T, TObject>.Create(route, key, resolver, registredPaths);
        }

        public static MappingRoute Create(Type type, Type objectType, MappingRoute route, string key, Resolver resolver = null, IEnumerable<KeyValuePair<string, string>> registredPaths = null)
        {
            return (MappingRoute) typeof (MappingRoute)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(o => o.IsGenericMethod)
                .MakeGeneric(type, objectType)
                .Invoke(null, new object[] { route, key, resolver, registredPaths });
        }

        public static MappingRoute Parse(string path, MappingRoute root, Resolver resolver = null, MappingRoute mappingRoute = null, IEnumerable<KeyValuePair<string, string>> registredPaths = null, bool initObjects = false)
        {
            char separetor = '/';
            root.Map.ConfigMap(config => separetor = config.Separator);

            var elements = path.Split(separetor);

            //string previous = null;
            foreach (var element in elements)
            {
                if(!root._routes.ContainsKey(element))
                {
                    var proxy = root._routeDataProxy ?? root._dataProxy;
                    if (resolver == null && initObjects && !element.Equals(elements.Last()))
                    {
                        //if (mappingRoute != null)
                        //{
                        //    resolver = Resolver.Create(root.Type, mappingRoute.Parent.Type, root.Map, typeof(ActivationResolver<,>));                            
                        //}
                        //else
                        //{                        
                        resolver = Resolver.Create(root.Type, proxy.GetMemberType(element), root.Map, typeof(ActivationResolver<,>)); 
                        //}
                    }
                    root = Create(root.Root, proxy.GetMemberType(element), root, element, resolver, registredPaths);                    
                }
                else
                {
                    root = root._routes[element];
                }
            }

            return root;
        }

        #endregion

    }

    public class MappingRoute<T, TObject> : MappingRoute
    {
        private object _value;        

        public static MappingRoute<T, TObject> Create(MappingRoute parent, string key, Resolver resolver = null, IEnumerable<KeyValuePair<string, string>> registredPaths = null)
        {
            //var type = resolver == null ? 
            //    parent != null ? parent.Type : typeof(T) : 
            //    resolver.DestinationType;

            var type = parent != null ? parent.Type : typeof (T);

            var route = new MappingRoute<T, TObject>(parent.Map, parent.Root)
            {
                Key = key,
                Parent = parent,
                DestinationRoute = parent.DestinationRoute
            };            

            route._dataProxy = DataProxy.Create(type);
            route._propertyInfo = route._dataProxy.GetPropertyInfo(key);
            route.Type = route._propertyInfo.PropertyType;
            route._routeDataProxy = DataProxy.Create(route.Type);
            route.Resolver = resolver;

            route.Parent._routes.Add(key, route);

            CreateGetterDelegate(route, registredPaths);
            CreateSetterDelegate(route);

            char separetor = '.';
            route.Map.ConfigMap(config => separetor = config.Separator);

            if (route.Parent != null && !string.IsNullOrEmpty(route.Parent.Route))
                route.Route = string.Format("{0}{1}{2}", route.Parent.Route, separetor, route.Key.Value);
            else route.Route = route.Key.Value;

            return route;
        }

        //private void Build()
        //{
        //    foreach (var route in _routes)
        //    {
        //        ((MappingRoute<T>) route.Value).Build();
                
        //        (Func<T, TSource>)route.Value.GetDelegate

        //        route.Value.
        //    }
        //}

        private static void CreateGetterDelegate(MappingRoute<T, TObject> route, IEnumerable<KeyValuePair<string, string>> registredPaths = null)
        {
            var method = typeof(MappingRoute<T, TObject>)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(o => o.Name.Equals("CreateGetterDelegate"))
                .Single(o => o.IsGenericMethod);

            var isParentNull = route.Parent == null;
            var returnType = isParentNull ? route.GetDelegate.Method.ReturnType : route.Parent.GetDelegate.Method.ReturnType;
            var parentType = !isParentNull ? route.Parent.Type : route.Type;
            var destinationType = route.Resolver != null ? route.Resolver.DestinationType : route.Type;
            method.MakeGeneric(parentType, route.Type, destinationType, parentType).Invoke(null, new object[] { route, registredPaths });
        }

        private static void CreateGetterDelegate<TSource, TDestination, TResolver, TParent>(MappingRoute<T, TObject> route, 
            IEnumerable<KeyValuePair<string, string>> registredPaths = null)
            //where TSource : TObject
        {            
            if (route.Resolver != null)
            {
                var valueGetter = DataProxy<object>.GetValueGetter<TSource, TDestination>(route._propertyInfo,
                    route.Parent.Type);

                route._getDelegate = valueGetter;
                //route._getConverteDelegate = valueGetter.Convert<TSource, TDestination, TS, object>();

                if (route.Resolver is ActivationResolver<TSource, TDestination>)
                {
                    var @delegate = ((Func<T, TSource>)route.Parent.GetDelegate).Compose(source =>
                    {
                        var result = DataProxy.Create(source)[route.Key];
                        route.Resolver.Resolve(source, ref result);
                        return (TDestination)result;
                    });
                    route.GetDelegate = @delegate;//.Compose(valueGetter);
                    route.GetConverteDelegate = @delegate.Convert<T, TDestination, T, object>();
                }
                else
                {
                    var @delegate = ((Func<T, TSource>)route.Parent.GetDelegate).Compose(valueGetter).Compose(source =>
                    {
                        object result = null;
                        route.Resolver.Resolve(source, ref result);
                        return (TResolver) result;
                    });
                    route.GetDelegate = @delegate;
                    route.GetConverteDelegate = @delegate.Convert<T, TResolver, T, object>();
                }
            }
            else
            {
                var valueGetter = DataProxy<object>.GetValueGetter<TSource, TDestination>(route._propertyInfo,
                    route.Parent.Type);
                route._getDelegate = valueGetter;
                var @delegate = ((Func<T, TSource>) route.Parent.GetDelegate)
                    .Compose(valueGetter);
                route.GetDelegate = @delegate;
                route.GetConverteDelegate = @delegate.Convert<T, TDestination, T, object>();
            }                                       
        }

        private static void CreateSetterDelegate(MappingRoute<T, TObject> route)
        {
            var method = typeof(MappingRoute<T, TObject>)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(o => o.Name.Equals("CreateSetterDelegate"))
                .Single(o => o.IsGenericMethod);

            var returnType = route.Parent == null ? route.GetDelegate.Method.ReturnType : route.Parent.GetDelegate.Method.ReturnType;
            method.MakeGeneric(returnType, route._dataProxy.Type, route.Type).Invoke(null, new object[] { route });
        }

        private static void CreateSetterDelegate<TSource, TResolver, TDestination>(MappingRoute<T, TObject> route)
        {
            if (route.Resolver != null)
            {
                var valueSetter = DataProxy<object>.GetValueSetter<TSource, TDestination>(route._propertyInfo, route.Parent.Type);
                route._setDelegate = valueSetter;

                route.SetDelegate = ((Func<T, TSource>)route.Parent.GetDelegate)
                    //.Compose(source => {
                    //    object result = null;
                    //    route.Resolver.Resolve(source, ref result);
                    //    return (TResolver)result;
                    //})
                    .Compose(valueSetter);
            }
            else
            {
                var valueSetter = DataProxy<object>.GetValueSetter<TSource, TDestination>(route._propertyInfo, route.Parent.Type);
                route._setDelegate = valueSetter;
                route.SetDelegate = ((Func<T, TSource>)route.Parent.GetDelegate)
                    .Compose(valueSetter);
            }
        }

        public MappingRoute(IMap map, Type root = null) 
            : base(map, root)
        {
            GetDelegate = (Func<T, T>)((T x) => x);
            ObjectType = typeof (TObject);
        }
    }
}
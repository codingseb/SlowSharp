﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slowsharp
{
    internal static class HybExt
    {
        public static object Unwrap(this HybInstance _this)
        {
            if (_this.isCompiledType)
                return _this.innerObject;
            return _this;
        }
        public static Type[] Unwrap(this HybType[] _this)
        {
            var objs = new Type[_this.Length];

            for (int i = 0; i < _this.Length; i++)
                objs[i] = _this[i].compiledType;

            return objs;
        }
        public static object[] Unwrap(this HybInstance[] _this)
        {
            var objs = new object[_this.Length];

            for (int i = 0; i < _this.Length; i++)
                objs[i] = _this[i].innerObject;

            return objs;
        }
        public static HybInstance[] Wrap(this object[] _this)
        {
            var objs = new HybInstance[_this.Length];

            for (int i = 0; i < _this.Length; i++)
                objs[i] = HybInstance.Object(_this[i]);

            return objs;
        }
        public static HybInstance Wrap(this object _this)
        {
            if (_this is HybInstance hyb) return hyb;
            return HybInstance.Object(_this);
        }

        public static bool IsAssignableFrom(this Type _this, HybType type)
        {
            return IsAssignableFromEx(_this, type, out _);
        }
        public static bool IsAssignableFromEx(
            this Type _this, HybType type, out Type[] genericBound)
        {
            genericBound = null;

            if (type.isCompiledType == false)
                return false;

            var cType = type.compiledType;
            if (_this.IsGenericType)
            {
                genericBound = new Type[] { cType };
                var gs = _this.GetGenericArguments();

                if (cType.IsGenericType &&
                    gs.Length == cType.GetGenericArguments().Length)
                {
                    _this = _this.GetGenericTypeDefinition()
                        .MakeGenericType(cType.GetGenericArguments());

                    genericBound = cType.GetGenericArguments();
                }
                else if (gs.Length == 1)
                {
                    _this = _this.GetGenericTypeDefinition();

                    if (_this == typeof(IEnumerable<>))
                    {
                        if (cType.IsArray)
                            genericBound = new Type[] { cType.GetElementType() };
                    }

                    _this = _this.MakeGenericType(genericBound);
                }
            }

            return _this.IsAssignableFrom(type.compiledType);
        }
        public static bool IsSubclassOf(this Type _this, HybType type)
        {
            if (type.isCompiledType == false)
                return false;

            return _this.IsSubclassOf(type.compiledType);
        }
    }
}

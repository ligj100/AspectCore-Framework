﻿using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace AspectCore.Extensions.Reflection
{
    public struct MethodSignature
    {
        private static readonly ConcurrentDictionary<MethodBase, int> signatures = new ConcurrentDictionary<MethodBase, int>();

        private readonly int _signature;

        public int Value => _signature;

        public MethodSignature(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            _signature = signatures.GetOrAdd(method, GetSignatureCode);
        }

        public override bool Equals(object obj)
        {
            if (obj is MethodSignature signature)
            {
                return _signature == signature._signature;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _signature;
        }

        public static bool operator !=(MethodSignature signature, MethodSignature other)
        {
            return signature._signature != other._signature;
        }

        public static bool operator ==(MethodSignature signature, MethodSignature other)
        {
            return signature._signature == other._signature;
        }

        private static int GetSignatureCode(MethodBase method)
        {
            unchecked
            {
                var signatureCode = method.Name.GetHashCode();
                var parameterTypes = method.GetParameterTypes();
                signatureCode = (signatureCode * 397) ^ parameterTypes.Length.GetHashCode();
                foreach (var paramterType in parameterTypes)
                    if (!paramterType.IsGenericParameter)
                        signatureCode = (signatureCode * 397) ^ paramterType.GetHashCode();
                signatureCode = (signatureCode * 397) ^ method.GetGenericArguments().Length.GetHashCode();
                return signatureCode;
            }
        }
    }
}
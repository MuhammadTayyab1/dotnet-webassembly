using System;
using System.Linq;
using System.Reflection;

namespace WebAssembly
{
    /// <summary>
    /// Indicates a method to use for a WebAssembly function import.
    /// </summary>
    public class FunctionImport : RuntimeImport
    {
        /// <summary>
        /// Always <see cref="ExternalKind.Function"/>.
        /// </summary>
        public sealed override ExternalKind Kind => ExternalKind.Function;

        /// <summary>
        /// The (optional) object and method to use for the import.
        /// </summary>
        public Delegate Function { get; private set; }

        internal readonly Type Type;

        private static System.Type GetDelegateTypeForMethod(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (method is System.Reflection.Emit.MethodBuilder)
                throw new ArgumentException("Imported methods cannot be dynamic method builders.", nameof(method));

            var parameters = method.GetParameters();
            System.Type generic;

            if (method.ReturnType == typeof(void))
            {
                switch (parameters.Length)
                {
                    case 00: return typeof(Action);
                    case 01: generic = typeof(Action<>); break;
                    case 02: generic = typeof(Action<,>); break;
                    case 03: generic = typeof(Action<,,>); break;
                    case 04: generic = typeof(Action<,,,>); break;
                    case 05: generic = typeof(Action<,,,,>); break;
                    case 06: generic = typeof(Action<,,,,,>); break;
                    case 07: generic = typeof(Action<,,,,,,>); break;
                    case 08: generic = typeof(Action<,,,,,,,>); break;
                    case 09: generic = typeof(Action<,,,,,,,,>); break;
                    case 10: generic = typeof(Action<,,,,,,,,,>); break;
                    case 11: generic = typeof(Action<,,,,,,,,,,>); break;
                    case 12: generic = typeof(Action<,,,,,,,,,,,>); break;
                    case 13: generic = typeof(Action<,,,,,,,,,,,,>); break;
                    case 14: generic = typeof(Action<,,,,,,,,,,,,,>); break;
                    case 15: generic = typeof(Action<,,,,,,,,,,,,,,>); break;
                    case 16: generic = typeof(Action<,,,,,,,,,,,,,,,>); break;
                    default: throw new NotSupportedException();
                }

                return generic.MakeGenericType(parameters.Select(p => p.ParameterType).ToArray());
            }
            else
            {
                switch (parameters.Length)
                {
                    case 00: generic = typeof(Func<>); break;
                    case 01: generic = typeof(Func<,>); break;
                    case 02: generic = typeof(Func<,,>); break;
                    case 03: generic = typeof(Func<,,,>); break;
                    case 04: generic = typeof(Func<,,,,>); break;
                    case 05: generic = typeof(Func<,,,,,>); break;
                    case 06: generic = typeof(Func<,,,,,,>); break;
                    case 07: generic = typeof(Func<,,,,,,,>); break;
                    case 08: generic = typeof(Func<,,,,,,,,>); break;
                    case 09: generic = typeof(Func<,,,,,,,,,>); break;
                    case 10: generic = typeof(Func<,,,,,,,,,,>); break;
                    case 11: generic = typeof(Func<,,,,,,,,,,,>); break;
                    case 12: generic = typeof(Func<,,,,,,,,,,,,>); break;
                    case 13: generic = typeof(Func<,,,,,,,,,,,,,>); break;
                    case 14: generic = typeof(Func<,,,,,,,,,,,,,,>); break;
                    case 15: generic = typeof(Func<,,,,,,,,,,,,,,,>); break;
                    case 16: generic = typeof(Func<,,,,,,,,,,,,,,,,>); break;
                    default: throw new NotSupportedException();
                }

                return generic.MakeGenericType(new[] { method.ReturnType }.Concat(parameters.Select(p => p.ParameterType)).ToArray());
            }
        }

        /// <summary>
        /// Creates a new <see cref="FunctionImport"/> instance with the provided <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="moduleName">The first portion of the two part name.</param>
        /// <param name="exportName">The second portion of the two-part name.</param>
        /// <param name="method">The method to use for the import.</param>
        /// <exception cref="ArgumentNullException">No parameters can be null.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> must be public, static, and cannot be a <see cref="System.Reflection.Emit.MethodBuilder"/>.</exception>
        public FunctionImport(string moduleName, string exportName, MethodInfo method)
            : this(moduleName, exportName, Delegate.CreateDelegate(GetDelegateTypeForMethod(method), method))
        {
        }

        /// <summary>
        /// Creates a new <see cref="FunctionImport"/> instance with the provided <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="moduleName">The first portion of the two part name.</param>
        /// <param name="exportName">The second portion of the two-part name.</param>
        /// <param name="function">The (optional) object and method to use for the import.</param>
        /// <exception cref="ArgumentNullException">No parameters can be null.</exception>
        /// <exception cref="ArgumentException"><paramref name="function"/> must be public, static, and cannot be a <see cref="System.Reflection.Emit.MethodBuilder"/>.</exception>
        public FunctionImport(string moduleName, string exportName, Delegate function)
            : base(moduleName, exportName)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            var method = function.Method;
            if (method.IsStatic == false || method.IsPublic == false)
                throw new ArgumentException("Imported methods must be public and static.", nameof(function));

            this.Type = new Type();
            if (method.ReturnType != typeof(void))
            {
                if (!method.ReturnType.TryConvertToValueType(out var type))
                    throw new ArgumentException($"Return type {method.ReturnType} is not compatible with WebAssembly.", nameof(function));

                this.Type.Returns = new[] { type };
            }

            foreach (var parameter in method.GetParameters())
            {
                if (!parameter.ParameterType.TryConvertToValueType(out var type))
                    throw new ArgumentException($"Parameter type {parameter} is not compatible with WebAssembly.", nameof(function));

                this.Type.Parameters.Add(type);
            }

            this.Function = function;
        }
    }
}
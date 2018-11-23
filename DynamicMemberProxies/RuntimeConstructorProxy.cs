using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMemberProxies
{
    /// <summary>
    /// Runtime constructor proxy class which calls given <see cref="ConstructorInfo"/> during runtime by using Dynamic methods
    /// </summary>
    public class RuntimeConstructorProxy
    {
        #region Types

        /// <summary>
        /// A delegate which represents the signature of generated dynamic method
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns></returns>
        public delegate object DynamicMethodDelegate(object[] args);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ConstructorInfo"/> which this proxy class will call
        /// </summary>
        public ConstructorInfo Constructor { get; private set; }

        /// <summary>
        /// Gets the generated <see cref="DynamicMethodDelegate"/> instance
        /// </summary>
        public DynamicMethodDelegate Proxy { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided values
        /// </summary>
        /// <param name="constructor">The constructor info</param>
        /// <param name="proxy">The dynamic method</param>
        private RuntimeConstructorProxy(ConstructorInfo constructor, DynamicMethodDelegate proxy)
        {
            Constructor = constructor;
            Proxy = proxy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invokes the constructor with no parameter
        /// </summary>
        /// <returns></returns>
        public object Invoke()
        {
            return Invoke(null);
        }

        /// <summary>
        /// Invokes dynamic method and returns created type as casted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Invoke<T>()
            where T:class
        {
            return Invoke() as T;
        }

        /// <summary>
        /// Invokes the constructor with given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Invoke(params object[] parameters)
        {
            return Proxy.Invoke(parameters);
        }

        /// <summary>
        /// Invokes dynamic method with given parameters and returns the created type as casted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Invoke<T>(params object[] parameters)
            where T : class
        {
            return Invoke(parameters) as T;
        }

        /// <summary>
        /// Creates a new runtime constructor proxy for the given type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        public static RuntimeConstructorProxy Create<T>()
        {
            return Create(typeof(T));
        }

        /// <summary>
        /// Creates a new runtime constructor proxy for the given type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        /// /// <exception cref="ArgumentNullException">Throws argument null exception when <c>type</c> is null</exception>
        /// <exception cref="ArgumentException">Throws argument exception when there is no constructor</exception>
        public static RuntimeConstructorProxy Create(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            var constructors = type.GetConstructors();
            if (constructors == null || constructors.Length == 0)
            {
                throw new ArgumentException("Type has no constructor");
            }
            return Create(constructors[0]);
        }

        /// <summary>
        /// Creates a new <see cref="RuntimeConstructorProxy"/> for the given <see cref="ConstructorInfo"/>
        /// </summary>
        /// <param name="constructor">The constructor info</param>
        /// <returns></returns>
        /// /// <exception cref="ArgumentNullException">Throws argument null exception when <c>constructor</c> is null</exception>
        public static RuntimeConstructorProxy Create(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            var proxy = CreateProxyMethod(constructor);
            return new RuntimeConstructorProxy(constructor, proxy);
        }

        /// <summary>
        /// Explicitly converts given <see cref="ConstructorInfo"/> to the <see cref="RuntimeConstructorProxy"/>
        /// </summary>
        /// <param name="constructorInfo">The constructor info</param>
        public static explicit operator RuntimeConstructorProxy(ConstructorInfo constructorInfo)
        {
            return Create(constructorInfo);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Creates the dynamic proxy method which calls given <see cref="ConstructorInfo"/>
        /// </summary>
        /// <param name="methodInfo"></param>
        private static DynamicMethodDelegate CreateProxyMethod(ConstructorInfo constructor)
        {
            var method = new DynamicMethod(constructor.DeclaringType.Name + "_ctor_Proxy",
                typeof(object),
                new Type[] { typeof(object[]) },
                constructor.DeclaringType);

            var wrapper = new ILGeneratorWrapper(method.GetILGenerator());

            var parameters = constructor.GetParameters();
            var parameterTypes = new Type[parameters.Length + 1];
            parameterTypes[0] = constructor.DeclaringType;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i + 1] = parameters[i].ParameterType;
            }

            wrapper.DeclareLocals(parameterTypes);

            for (int i = 1; i < parameterTypes.Length; i++)
            {
                wrapper.EmitLoadArg(0);
                wrapper.EmitLoadCons(i - 1);
                wrapper.Emit(OpCodes.Ldelem_Ref);
                wrapper.EmitCast(parameterTypes[i]);
                wrapper.EmitStoreLocal(i);
            }

            for (int i = 1; i < parameterTypes.Length; i++)
            {
                wrapper.EmitLoadLoc(i);
            }

            wrapper.Emit_NewObj(constructor);

            wrapper.EmitStoreLocal(0);
            wrapper.EmitLoadLoc(0);
            wrapper.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(DynamicMethodDelegate)) as DynamicMethodDelegate;
        }

        #endregion
    }
}

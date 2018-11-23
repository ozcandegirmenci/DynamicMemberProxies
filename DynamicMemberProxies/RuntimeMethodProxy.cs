using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMemberProxies
{
    /// <summary>
    /// Runtime method info proxy class which calls given <see cref="MethodInfo"/> during runtime by using Dynamic methods
    /// </summary>
    public class RuntimeMethodProxy
    {
        #region Types

        /// <summary>
        /// A delegate which represents the signature of generated method invocation dynamic method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate object DynamicMethodDelegate(object instance, object[] args);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> which this proxy class will work with
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the generated <see cref="DynamicMethodDelegate"/> instance
        /// </summary>
        public DynamicMethodDelegate Proxy { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided arguments
        /// </summary>
        /// <param name="methodInfo">The method info</param>
        /// <param name="proxy">The proxy instance</param>
        private RuntimeMethodProxy(MethodInfo methodInfo, DynamicMethodDelegate proxy)
        {
            Method = methodInfo;
            Proxy = proxy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invokes the method (static with no parameter)
        /// </summary>
        /// <returns></returns>
        public object Invoke()
        {
            return Invoke(null, new object[0]);
        }

        /// <summary>
        /// Invokes the method (which is static)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T InvokeStatic<T>()
        {
            return (T)InvokeStatic();
        }

        /// <summary>
        /// Invokes the method (which is static) with the given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeStatic(params object[] parameters)
        {
            return InvokeStatic(null, parameters);
        }

        /// <summary>
        /// Invokes the method (which is static) with the given parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public T InvokeStatic<T>(params object[] parameters)
        {
            return (T)InvokeStatic(parameters);
        }

        /// <summary>
        /// Runs the method by using given parameters
        /// </summary>
        /// <param name="instance">Represents the instance which this method will be run on it,
        /// if this is a static method than this parameter must be <c>null</c></param>
        /// <param name="parameters">The arguments of the method</param>
        /// <returns></returns>
        public object Invoke(object instance, params object[] parameters)
        {
            return Proxy.Invoke(instance, parameters);
        }

        /// <summary>
        /// Invokes the method with the given instance and method parameter arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance which method will be run on</param>
        /// <param name="parameters">The parameters for the method</param>
        /// <returns></returns>
        public T Invoke<T>(object instance, params object[] parameters)
        {
            return (T)Invoke(instance, parameters);
        }

        /// <summary>
        /// Creates a new <see cref="RuntimeMethodProxy"/> instance for the given <see cref="MethodInfo"/>
        /// </summary>
        /// <param name="methodInfo">The method info</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws argument null exception when <c>methodInfo</c> is null</exception>
        public static RuntimeMethodProxy Create(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            var proxy = CreateMethodProxy(methodInfo);
            return new RuntimeMethodProxy(methodInfo, proxy);
        }

        /// <summary>
        /// Explicitly converts given <see cref="MethodInfo"/> to a new instance of <see cref="RuntimeMethodProxy"/>
        /// </summary>
        /// <param name="methodInfo">The method info</param>
        public static explicit operator RuntimeMethodProxy(MethodInfo methodInfo)
        {
            return Create(methodInfo);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Initializes dynamic method
        /// </summary>
        /// <param name="methodInfo"></param>
        private static DynamicMethodDelegate CreateMethodProxy(MethodInfo methodInfo)
        {
            var method = new DynamicMethod(methodInfo.Name + "_Proxy",
                typeof(object),
                new Type[] { typeof(object), typeof(object[]) },
                methodInfo.DeclaringType);

            var wrapper = new ILGeneratorWrapper(method.GetILGenerator());

            var parameters = methodInfo.GetParameters();
            var parameterTypes = new Type[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }

            wrapper.DeclareLocals(parameterTypes);

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                wrapper.EmitLoadArg(1);
                wrapper.EmitLoadCons(i);
                wrapper.Emit(OpCodes.Ldelem_Ref);
                wrapper.EmitCast(parameterTypes[i]);
                wrapper.EmitStoreLocal(i);
            }

            if (!methodInfo.IsStatic)
            {
                wrapper.Emit(OpCodes.Ldarg_0);
            }

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                wrapper.EmitLoadLoc(i);
            }

            wrapper.EmitCall(methodInfo);

            if (methodInfo.ReturnType == typeof(void))
            {
                wrapper.Emit(OpCodes.Ldnull);
            }
            else if (methodInfo.ReturnType.IsValueType)
            {
                wrapper.EmitBox(methodInfo.ReturnType);
            }
            wrapper.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(DynamicMethodDelegate)) as DynamicMethodDelegate;
        }

        #endregion
    }
}

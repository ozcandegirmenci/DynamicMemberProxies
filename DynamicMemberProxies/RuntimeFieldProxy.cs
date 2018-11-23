using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMemberProxies
{
    /// <summary>
    /// Runtime field info proxy class which calls given <see cref="FieldInfo"/> during runtime by using Dynamic methods
    /// </summary>
    public class RuntimeFieldProxy
    {
        #region Types

        /// <summary>
        /// A delegate which represents the signature of generated get field value dynamic method
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public delegate object GetFieldDelegate(object instance);

        /// <summary>
        /// A delegate which represents the signature of generated set field value dynamic method
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="value">The value</param>
        public delegate void SetFieldDelegate(object instance, object value);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="FieldInfo"/> which this proxy class will call
        /// </summary>
        public FieldInfo Field { get; }

        /// <summary>
        /// Gets the generated get value <see cref="GetFieldDelegate"/> proxy instance
        /// </summary>
        public GetFieldDelegate GetProxy { get; }

        /// <summary>
        /// Gets the generated set value <see cref="SetFieldDelegate"/> proxy instance
        /// </summary>
        public SetFieldDelegate SetProxy { get; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided field info
        /// </summary>
        /// <param name="fieldInfo">The field info</param>
        private RuntimeFieldProxy(FieldInfo fieldInfo, GetFieldDelegate getProxy, SetFieldDelegate setProxy)
        {
            Field = fieldInfo;
            GetProxy = getProxy;
            SetProxy = setProxy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets value of the field with in the given instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetValue(object instance)
        {
            return GetProxy.Invoke(instance);
        }

        /// <summary>
        /// Returns the value of the field for the given instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T GetValue<T>(object instance)
        {
            return (T)GetValue(instance);
        }

        /// <summary>
        /// Sets value of a field with in the given instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <remarks>For static field use <c>null</c> as the instance parameter</remarks>
        public void SetValue(object instance, object value)
        {
            SetProxy.Invoke(instance, value);
        }

        /// <summary>
        /// Creates a new <see cref="RuntimeFieldProxy"/> instance for the given <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="fieldInfo">The field info</param>
        /// <returns></returns>
        public static RuntimeFieldProxy Create(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            var getProxy = CreateGetMethodProxy(fieldInfo);
            var setProxy = CreateSetMethodProxy(fieldInfo);
            return new RuntimeFieldProxy(fieldInfo, getProxy, setProxy);
        }

        /// <summary>
        /// Explicitly converts given <see cref="FieldInfo"/> to <see cref="RuntimeFieldProxy"/>
        /// </summary>
        /// <param name="fieldInfo">The field info</param>
        public static explicit operator RuntimeFieldProxy(FieldInfo fieldInfo)
        {
            return Create(fieldInfo);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Creates get method proxy for the given <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="fieldInfo">The field info</param>
        private static GetFieldDelegate CreateGetMethodProxy(FieldInfo fieldInfo)
        {
            var method = new DynamicMethod(fieldInfo.Name + "_Get_Proxy",
                typeof(object),
                new[] { typeof(object) },
                fieldInfo.DeclaringType);
            var wrapper = new ILGeneratorWrapper(method.GetILGenerator());
            
            wrapper.DeclareLocals(new Type[] { fieldInfo.FieldType });

            wrapper.EmitLoadArg(0);
            wrapper.EmitLoadField(fieldInfo);
            wrapper.EmitStoreLocal(0);
            wrapper.EmitLoadLoc(0);
            if (fieldInfo.FieldType.IsValueType)
            {
                wrapper.EmitBox(fieldInfo.FieldType);
            }
            wrapper.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(GetFieldDelegate)) as GetFieldDelegate;
        }

        /// <summary>
        /// Creates set method proxy for the given <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="info">The field info</param>
        private static SetFieldDelegate CreateSetMethodProxy(FieldInfo info)
        {
            var method = new DynamicMethod(info.Name + "_Set_Proxy",
                typeof(void),
                new[] { typeof(object), typeof(object) },
                info.DeclaringType);
            var wrapper = new ILGeneratorWrapper(method.GetILGenerator());

            wrapper.DeclareLocals(new Type[] { info.FieldType });

            wrapper.EmitLoadArg(0);
            wrapper.EmitLoadArg(1);
            if (info.FieldType.IsValueType)
            {
                wrapper.EmitUnbox_Any(info.FieldType);
            }
            wrapper.EmitStoreField(info);
            wrapper.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(SetFieldDelegate)) as SetFieldDelegate;
        }

        #endregion
    }
}

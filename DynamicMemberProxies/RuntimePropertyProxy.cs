using System;
using System.Reflection;

namespace DynamicMemberProxies
{
    /// <summary>
    /// Runtime property info proxy class which calls given <see cref="PropertyInfo"/> during runtime by using Dynamic methods
    /// </summary>
    public class RuntimePropertyProxy
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> which this proxy class will work with
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the <see cref="RuntimeMethodProxy"/> for the Property get method
        /// </summary>
        public RuntimeMethodProxy GetMethod { get; private set; }

        /// <summary>
        /// Gets the <see cref="RuntimeMethodProxy"/> for the Property set method
        /// </summary>
        public RuntimeMethodProxy SetMethod { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided arguments
        /// </summary>
        /// <param name="propertyInfo">The property info</param>
        /// <param name="getMethod">The get method proxy</param>
        /// <param name="setMethod">The set method proxy</param>
        private RuntimePropertyProxy(PropertyInfo propertyInfo, RuntimeMethodProxy getMethod, RuntimeMethodProxy setMethod)
        {
            Property = propertyInfo;
            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns value of the property by using <see cref="GetMethod"/> proxy
        /// </summary>
        /// <param name="instance">Represents the instance which this method will be run on it,
        /// if this is a static method than this parameter must be <c>null</c></param>
        /// <param name="indexes">The index arguments of the property</param>
        /// <returns></returns>
        public object GetValue(object instance, params object[] indexes)
        {
            return GetMethod.Invoke(instance, indexes);
        }

        /// <summary>
        /// Returns value of the property by using <see cref="GetMethod"/> proxy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">Represents the instance which this method will be run on it,
        /// if this is a static method than this parameter must be <c>null</c></param>
        /// <param name="indexes">The index parameters for the property</param>
        /// <returns></returns>
        public T GetValue<T>(object instance, params object[] indexes)
        {
            return (T)GetValue(instance, indexes);
        }

        /// <summary>
        /// Sets value of the property by using <see cref="SetMethod"/> proxy
        /// </summary>
        /// <param name="instance">Represents the instance which this method will be run on it,
        /// if this is a static method than this parameter must be <c>null</c></param>
        /// <param name="value">The value of the property</param>
        /// <returns></returns>
        public void SetValue(object instance, object value)
        {
            SetMethod.Invoke(instance, value);
        }

        /// <summary>
        /// Sets value of the property by using <see cref="SetMethod"/> proxy
        /// </summary>
        /// <param name="instance">Represents the instance which this method will be run on it,
        /// if this is a static method than this parameter must be <c>null</c></param>
        /// <param name="value">The value of the property</param>
        /// <param name="indexes">The indexes of the property</param>
        /// <returns></returns>
        public void SetValue(object instance, object value, object[] indexes)
        {
            if (indexes != null)
            {
                var args = new object[indexes.Length + 1];
                Array.Copy(indexes, args, indexes.Length);
                args[args.Length - 1] = value;
                SetMethod.Invoke(instance, args);
            }
            else
            {
                SetMethod.Invoke(instance, value);
            }
        }

        /// <summary>
        /// Creates a new <see cref="RuntimePropertyProxy"/> instance for the given <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="propertyInfo">The property info</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws argument null exception when <c>propertyInfo</c> is null</exception>
        public static RuntimePropertyProxy Create(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            RuntimeMethodProxy getMethod = null;
            if (propertyInfo.CanRead)
            {
                getMethod = RuntimeMethodProxy.Create(propertyInfo.GetMethod);
            }
            RuntimeMethodProxy setMethod = null;
            if (propertyInfo.CanWrite)
            {
                setMethod = RuntimeMethodProxy.Create(propertyInfo.SetMethod);
            }
            return new RuntimePropertyProxy(propertyInfo, getMethod, setMethod);
        }

        /// <summary>
        /// Explicitly converts given <see cref="PropertyInfo"/> to a new instance of <see cref="RuntimePropertyProxy"/>
        /// </summary>
        /// <param name="propertyInfo">The property info</param>
        public static explicit operator RuntimePropertyProxy(PropertyInfo propertyInfo)
        {
            return Create(propertyInfo);
        }

        #endregion
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMemberProxies.Tests
{
    /// <summary>
    /// Contains unit tests related with the <see cref="RuntimeConstructorProxy"/>
    /// </summary>
    [TestClass]
    public class RuntimeConstructorProxyTests
    {
        #region Types

        /// <summary>
        /// Test class
        /// </summary>
        public class Test
        {
            public int ValueTypeProp { get; set; }

            public string ReferenceTypeProp { get; set; }

            public Test()
            {

            }

            public Test(int valueTypeProp)
            {
                ValueTypeProp = valueTypeProp;
            }

            public Test(string referenceTypeProp)
            {
                ReferenceTypeProp = referenceTypeProp;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests creating an instance of a type by using parameterless constructor
        /// </summary>
        [TestMethod]
        public void CreateInstanceWithParameterlessConstructor()
        {
            var type = typeof(Test);

            var constructorInfo = type.GetConstructor(new Type[0]);
            var proxy = (RuntimeConstructorProxy)constructorInfo;
            var result = proxy.Invoke<Test>();

            Assert.AreNotEqual(null, result);
            Assert.AreEqual(0, result.ValueTypeProp);
        }

        /// <summary>
        /// Tests creating an instance of a type by using a constructor which has parameters
        /// </summary>
        [TestMethod]
        public void CreateInstanceWithValueTypeParameteredConstructor()
        {
            var type = typeof(Test);

            var constructorInfo = type.GetConstructor(new Type[] { typeof(int) });
            var proxy = (RuntimeConstructorProxy)constructorInfo;
            var result = proxy.Invoke<Test>(2);
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(2, result.ValueTypeProp);
        }

        /// <summary>
        /// Tests creating an instance of a type by using a constructor which has parameters
        /// </summary>
        [TestMethod]
        public void CreateInstanceWithReferenceTypeParameteredConstructor()
        {
            var type = typeof(Test);

            var constructorInfo = type.GetConstructor(new Type[] { typeof(string) });
            var proxy = (RuntimeConstructorProxy)constructorInfo;
            var result = proxy.Invoke<Test>("Ozcan Degirmenci");
            Assert.AreNotEqual(null, result);
            Assert.AreEqual("Ozcan Degirmenci", result.ReferenceTypeProp);
        }

        #endregion
    }
}

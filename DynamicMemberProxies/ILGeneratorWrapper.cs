using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMemberProxies
{
    /// <summary>
    /// Provides some helper methods for generating dynamic IL codes
    /// </summary>
    internal class ILGeneratorWrapper
    {
        #region Properties

        /// <summary>
        /// Gets the IL Generator instance
        /// </summary>
        public ILGenerator Generator { get; }

        /// <summary>
        /// Gets the locals
        /// </summary>
        public LocalBuilder[] Locals { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided <see cref="ILGenerator"/>
        /// </summary>
        /// <param name="generator">The IL Generator instance</param>
        public ILGeneratorWrapper(ILGenerator generator)
        {
            Generator = generator;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Declares local variables for the given <see cref="Type"/>s
        /// </summary>
        /// <param name="localTypes">Type of the local variables</param>
        public void DeclareLocals(Type[] localTypes)
        {
            Locals = new LocalBuilder[localTypes.Length];
            for (int i = 0; i < localTypes.Length; i++)
            {
                Locals[i] = Generator.DeclareLocal(localTypes[i]);
            }
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Newobj"/> opcode with the provided <see cref="ConstructorInfo"/> operand
        /// </summary>
        /// <param name="constructor">The constructor which will be invoke for creating new object</param>
        public void Emit_NewObj(ConstructorInfo constructor)
        {
            Generator.Emit(OpCodes.Newobj, constructor);
        }

        /// <summary>
        /// Emits given <see cref="OpCode"/>
        /// </summary>
        /// <param name="opCode">The opcode</param>
        public void Emit(OpCode opCode)
        {
            Generator.Emit(opCode);
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Stloc"/> for the given local variable operand
        /// </summary>
        /// <param name="localIndex">The index of the local variable</param>
        public void EmitStoreLocal(int value)
        {
            switch (value)
            {
                case 0:
                    Generator.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    Generator.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    Generator.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    Generator.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (IsSByte(value))
                    {
                        Generator.Emit(OpCodes.Stloc_S, (sbyte)value);
                    }
                    else
                    {
                        Generator.Emit(OpCodes.Stloc, value);
                    }
                    break;
            }
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Ldfld"/> for the given <see cref="FieldInfo"/> operand
        /// </summary>
        /// <param name="field">The field info</param>
        public void EmitLoadField(FieldInfo field)
        {
            Generator.Emit(OpCodes.Ldfld, field);
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Stfld"/> for the given <see cref="FieldInfo"/> operand
        /// </summary>
        /// <param name="field">The field info</param>
        public void EmitStoreField(FieldInfo field)
        {
            Generator.Emit(OpCodes.Stfld, field);
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Box"/> with the given <see cref="Type"/> operand
        /// </summary>
        /// <param name="type">The boxing type</param>
        public void EmitBox(Type type)
        {
            Generator.Emit(OpCodes.Box, type);
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Unbox_Any"/> with the given <see cref="Type"/> operand
        /// </summary>
        /// <param name="type">The unboxing type</param>
        public void EmitUnbox_Any(Type type)
        {
            Generator.Emit(OpCodes.Unbox_Any, type);
        }

        /// <summary>
        /// Emits <see cref="OpCodes.Call"/> or <see cref="OpCodes.Callvirt"/> with the provided <see cref="MethodInfo"/> operand
        /// </summary>
        /// <param name="info">The method info</param>
        /// <remarks>If method is a <c>static</c> method than <see cref="OpCodes.Call"/> will be emit otherwise <see cref="OpCodes.Callvirt"/></remarks>
        public void EmitCall(MethodInfo info)
        {
            if (info.IsStatic)
            {
                Generator.EmitCall(OpCodes.Call, info, null);
            }
            else
            {
                Generator.EmitCall(OpCodes.Callvirt, info, null);
            }
        }

        /// <summary>
        /// Emits LdLoc[a_S] with the given local index operand
        /// </summary>
        /// <param name="localIndex">The local index</param>
        public void EmitLoadLoc(int localIndex)
        {
            if (Locals[localIndex].LocalType.IsByRef)
            {
                Generator.Emit(OpCodes.Ldloca_S, Locals[localIndex]);
            }
            else
            {
                Generator.Emit(OpCodes.Ldloc, Locals[localIndex]);
            }
        }

        /// <summary>
        /// Emits a casting operation (Unbox_Any or Castclass) with the provided <see cref="Type"/> operand
        /// </summary>
        /// <param name="type">The type</param>
        public void EmitCast(Type type)
        {
            if (type.IsValueType)
            {
                Generator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                Generator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Emits Ldarg (Load Argument) op code with the provided argument index operand
        /// </summary>
        /// <param name="argumentIndex">The argument index</param>
        public void EmitLoadArg(int argumentIndex)
        {
            switch (argumentIndex)
            {
                case 0:
                    Generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    Generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    Generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    Generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (IsSByte(argumentIndex))
                    {
                        Generator.Emit(OpCodes.Ldarg_S, (sbyte)argumentIndex);
                    }
                    else
                    {
                        Generator.Emit(OpCodes.Ldarg, argumentIndex);
                    }
                    break;
            }
        }

        /// <summary>
        /// Emits Ldloc (Load Local) opcode with the provided local index operand
        /// </summary>
        /// <param name="localIndex">The local index</param>
        public void EmitLoadLocal(int localIndex)
        {
            switch (localIndex)
            {
                case 0:
                    Generator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    Generator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    Generator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    Generator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (IsSByte(localIndex))
                    {
                        Generator.Emit(OpCodes.Ldloc_S, (sbyte)localIndex);
                    }
                    else
                    {
                        Generator.Emit(OpCodes.Ldloc, localIndex);
                    }
                    break;
            }
        }

        /// <summary>
        /// Emits Ldc (Load Constant) with the provided operand
        /// </summary>
        /// <param name="value">The constant value</param>
        public void EmitLoadCons(int value)
        {
            switch (value)
            {
                case -1:
                    Generator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    Generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    Generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    Generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    Generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    Generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    Generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    Generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    Generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    Generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (IsSByte(value))
                    {
                        Generator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
                    }
                    else
                    {
                        Generator.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Returns that is given value is a signed byte or not?
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        private static bool IsSByte(int value)
        {
            return (value > -129 && value < 128);
        }

        #endregion
    }
}

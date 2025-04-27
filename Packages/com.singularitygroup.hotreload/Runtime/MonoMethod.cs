#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Runtime.InteropServices;


namespace SingularityGroup.HotReload.Interop {
    //see _MonoMethod struct in class-internals.h
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Size = 8 + sizeof(long) * 3 + 4)]
    internal unsafe struct MonoMethod64 {
        [FieldOffset(0)]
        public MethodAttributes flags;
        [FieldOffset(2)]
        public MethodImplAttributes iflags;
        [FieldOffset(4)]
        public uint token;
        [FieldOffset(8)]
        public void* klass;
        [FieldOffset(8 + sizeof(long))]
        public void* signature;
        [FieldOffset(8 + sizeof(long) * 2)]
        public char* name;
        /* this is used by the inlining algorithm */
        [FieldOffset(8 + sizeof(long) * 3)]
        public MonoMethodFlags monoMethodFlags;
        [FieldOffset(8 + sizeof(long) * 3 + 2)]
        public short slot;
    }
    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Size = 8 + sizeof(int) * 3 + 4)]
    internal unsafe struct MonoMethod32 {
        [FieldOffset(0)]
        public MethodAttributes flags;
        [FieldOffset(2)]
        public MethodImplAttributes iflags;
        [FieldOffset(4)]
        public uint token;
        [FieldOffset(8)]
        public void* klass;
        [FieldOffset(8 + sizeof(int))]
        public void* signature;
        [FieldOffset(8 + sizeof(int) * 2)]
        public char* name;
        /* this is used by the inlining algorithm */
        [FieldOffset(8 + sizeof(int) * 3)]
        public MonoMethodFlags monoMethodFlags;
        [FieldOffset(8 + sizeof(int) * 3 + 2)]
        public short slot;
    }
    
    //Corresponds to the bitflags of the _MonoMethod struct
    [Flags]
    internal enum MonoMethodFlags : ushort {
        inline_info = 1 << 0,           //:1
        inline_failure = 1 << 1,        //:1
        wrapper_type =  1 << 2,         //:5
        string_ctor =  1 << 7,          //:1
        save_lmf = 1 << 8,              //:1
        dynamic = 1 << 9,               //:1       /* created & destroyed during runtime */
        sre_method = 1 << 10,           //:1       /* created at runtime using Reflection.Emit */
        is_generic = 1 << 11,           //:1       /* whenever this is a generic method definition */
        is_inflated = 1 << 12,          //:1       /* whether we're a MonoMethodInflated */
        skip_visibility = 1 << 13,      //:1       /* whenever to skip JIT visibility checks */
        verification_success = 1 << 14, //:1       /* whether this method has been verified successfully.*/ 
    }


    [Flags]
    internal enum MethodImplAttributes : ushort {
        /// <summary><para>Specifies that the method implementation is in Microsoft intermediate language (MSIL).</para></summary>
        IL = 0,

        /// <summary><para>Specifies that the method is implemented in managed code. </para></summary>
        Managed = 0,

        /// <summary><para>Specifies that the method implementation is native.</para></summary>
        Native = 1,

        /// <summary><para>Specifies that the method implementation is in Optimized Intermediate Language (OPTIL).</para></summary>
        OPTIL = 2,

        /// <summary><para>Specifies flags about code type.</para></summary>
        CodeTypeMask = 3,

        /// <summary><para>Specifies that the method implementation is provided by the runtime.</para></summary>
        Runtime = 3,

        /// <summary><para>Specifies whether the method is implemented in managed or unmanaged code.</para></summary>
        ManagedMask = 4,

        /// <summary><para>Specifies that the method is implemented in unmanaged code.</para></summary>
        Unmanaged = 4,

        /// <summary><para>Specifies that the method cannot be inlined.</para></summary>
        NoInlining = 8,

        /// <summary><para>Specifies that the method is not defined.</para></summary>
        ForwardRef = 16, // 0x00000010

        /// <summary><para>Specifies that the method is single-threaded through the body. Static methods (Shared in Visual Basic) lock on the type, whereas instance methods lock on the instance. You can also use the C# <format type="text/html"><a href="656DA1A4-707E-4EF6-9C6E-6D13B646AF42">lock statement</a></format> or the Visual Basic <format type="text/html"><a href="14501703-298f-4d43-b139-c4b6366af176">SyncLock statement</a></format> for this purpose. </para></summary>
        Synchronized = 32, // 0x00000020

        /// <summary><para>Specifies that the method is not optimized by the just-in-time (JIT) compiler or by native code generation (see <format type="text/html"><a href="44bf97aa-a9a4-4eba-9a0d-cfaa6fc53a66">Ngen.exe</a></format>) when debugging possible code generation problems.</para></summary>
        NoOptimization = 64, // 0x00000040

        /// <summary><para>Specifies that the method signature is exported exactly as declared.</para></summary>
        PreserveSig = 128, // 0x00000080

        /// <summary><para>Specifies that the method should be inlined wherever possible.</para></summary>
        AggressiveInlining = 256, // 0x00000100

        /// <summary><para>Specifies an internal call.</para></summary>
        InternalCall = 4096, // 0x00001000

        /// <summary><para>Specifies a range check value.</para></summary>
        MaxMethodImplVal = 65535, // 0x0000FFFF
    }



    /// <summary><para>Specifies flags for method attributes. These flags are defined in the corhdr.h file.</para></summary>
    [Flags]
    internal enum MethodAttributes : ushort {
        /// <summary><para>Retrieves accessibility information.</para></summary>
        MemberAccessMask = 7,

        /// <summary><para>Indicates that the member cannot be referenced.</para></summary>
        PrivateScope = 0,

        /// <summary><para>Indicates that the method is accessible only to the current class.</para></summary>
        Private = 1,

        /// <summary><para>Indicates that the method is accessible to members of this type and its derived types that are in this assembly only.</para></summary>
        FamANDAssem = 2,

        /// <summary><para>Indicates that the method is accessible to any class of this assembly.</para></summary>
        Assembly = FamANDAssem | Private, // 0x00000003

        /// <summary><para>Indicates that the method is accessible only to members of this class and its derived classes.</para></summary>
        Family = 4,

        /// <summary><para>Indicates that the method is accessible to derived classes anywhere, as well as to any class in the assembly.</para></summary>
        FamORAssem = Family | Private, // 0x00000005

        /// <summary><para>Indicates that the method is accessible to any object for which this object is in scope.</para></summary>
        Public = Family | FamANDAssem, // 0x00000006

        /// <summary><para>Indicates that the method is defined on the type; otherwise, it is defined per instance.</para></summary>
        Static = 16, // 0x00000010

        /// <summary><para>Indicates that the method cannot be overridden.</para></summary>
        Final = 32, // 0x00000020

        /// <summary><para>Indicates that the method is virtual.</para></summary>
        Virtual = 64, // 0x00000040

        /// <summary><para>Indicates that the method hides by name and signature; otherwise, by name only.</para></summary>
        HideBySig = 128, // 0x00000080

        /// <summary><para>Indicates that the method can only be overridden when it is also accessible.</para></summary>
        CheckAccessOnOverride = 512, // 0x00000200

        /// <summary><para>Retrieves vtable attributes.</para></summary>
        VtableLayoutMask = 256, // 0x00000100

        /// <summary><para>Indicates that the method will reuse an existing slot in the vtable. This is the default behavior.</para></summary>
        ReuseSlot = 0,

        /// <summary><para>Indicates that the method always gets a new slot in the vtable.</para></summary>
        NewSlot = VtableLayoutMask, // 0x00000100

        /// <summary><para>Indicates that the class does not provide an implementation of this method.</para></summary>
        Abstract = 1024, // 0x00000400

        /// <summary><para>Indicates that the method is special. The name describes how this method is special.</para></summary>
        SpecialName = 2048, // 0x00000800

        /// <summary><para>Indicates that the method implementation is forwarded through PInvoke (Platform Invocation Services).</para></summary>
        PinvokeImpl = 8192, // 0x00002000

        /// <summary><para>Indicates that the managed method is exported by thunk to unmanaged code.</para></summary>
        UnmanagedExport = 8,

        /// <summary><para>Indicates that the common language runtime checks the name encoding.</para></summary>
        RTSpecialName = 4096, // 0x00001000

        /// <summary><para>Indicates a reserved flag for runtime use only.</para></summary>
        ReservedMask = 53248, // 0x0000D000

        /// <summary><para>Indicates that the method has security associated with it. Reserved flag for runtime use only.</para></summary>
        HasSecurity = 16384, // 0x00004000

        /// <summary><para>Indicates that the method calls another method containing security code. Reserved flag for runtime use only.</para></summary>
        RequireSecObject = 32768, // 0x00008000
    }
}
#endif

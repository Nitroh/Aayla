using System.Linq;
using System.Runtime.InteropServices;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoExecutable : PortableExecutable
    {

        private const string MonoModuleName = @"mono.dll";
        private const string MonoGetRootDomain = @"mono_get_root_domain";
        private const string MonoAssemblyName = @"Assembly-CSharp";
        private const int MonoGetRootDomainFuncSize = 4;

        public MonoExecutable(string processName) : base(processName, MonoModuleName)
        {
        }

        public long GetTest()
        {
            var domainSize = Marshal.SizeOf(typeof(MonoDomain));
            var assemblySize = Marshal.SizeOf(typeof(MonoAssembly));
            var assemblyNameSize = Marshal.SizeOf(typeof(MonoAssemblyName));
            var imageSize = Marshal.SizeOf(typeof(MonoImage));
            var classSize = Marshal.SizeOf(typeof(MonoClass));
            var typeSize = Marshal.SizeOf(typeof(MonoType));

            //domainSize --> 112
            //assemblyNameSize --> 56
            //assemblySize --> 84
            //imageSize --> 672

            var currSize = 0x8;

            return 0;
            //TODO: this is test code
            //var rootDomain = GetMonoDomain();
            //var assembly = GetMonoAssembly(rootDomain, MonoAssemblyName);
            //return assembly.image;
        }

        private MonoAssembly GetMonoAssembly(MonoDomain domain, string name)
        {
            var currentAssemblyPointer = domain.domain_assemblies;
            while (currentAssemblyPointer != 0)
            {
                var currentAssemblyOffset = WindowsHelper.ReadUInt(Process.Handle, currentAssemblyPointer);
                var currentAssembly = WindowsHelper.ReadStruct<MonoAssembly>(Process.Handle, currentAssemblyOffset);
                var currentAssemblyName = WindowsHelper.ReadString(Process.Handle, currentAssembly.aname.name);
                if(currentAssemblyName == name)
                    return currentAssembly;
                currentAssemblyPointer = WindowsHelper.ReadUInt(Process.Handle, currentAssemblyPointer + sizeof(uint));
            }
            return new MonoAssembly();
        }

        private MonoDomain GetMonoDomain()
        {
            var rootDomainFunctionPointer = FunctionPointers.FirstOrDefault(x => x.Item1 == MonoGetRootDomain)?.Item2 ?? 0;
            if (rootDomainFunctionPointer == 0) return new MonoDomain();

            byte[] rawFunctionCode;
            var valid = WindowsHelper.ReadMemory(Process.Handle, BaseAddress + rootDomainFunctionPointer, MonoGetRootDomainFuncSize + 2, out rawFunctionCode);
            if(!valid) return new MonoDomain();

            var functionCode = WindowsHelper.ReadFunctionCode(rawFunctionCode);
            var rootDomainPointer = WindowsHelper.ReadUInt(functionCode);
            if(rootDomainPointer == 0) return new MonoDomain();

            var rootDomainOffset = WindowsHelper.ReadUInt(Process.Handle, rootDomainPointer);
            return WindowsHelper.ReadStruct<MonoDomain>(Process.Handle, rootDomainOffset);
        }
    }
}
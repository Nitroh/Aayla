using System.Collections.Generic;
using System.Linq;
using Nitroh.Mono.Internals;
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

        public IEnumerable<MonoClassEx> GetClasses()
        {
            var rootDomain = GetMonoDomain();
            var assembly = GetMonoAssembly(rootDomain, MonoAssemblyName);
            var classes = GetMonoClasses(assembly);
            return classes;
        }

        private IEnumerable<MonoClassEx> GetMonoClasses(MonoAssembly assembly)
        {
            var result = new List<MonoClassEx>();
            if (assembly.image == 0) return result;
            var image = ReadStruct<MonoImage>(assembly.image);
            for (var index = 0; index < image.class_cache.size; index++)
            {
                var monoClassPointer = ReadUInt(image.class_cache.table + 4*index);
                while (monoClassPointer != 0)
                {
                    var monoClass = ReadStruct<MonoClass>(monoClassPointer);
                    result.Add(new MonoClassEx(monoClass, Process.Handle));
                    monoClassPointer = ReadUInt(monoClass.next_class_cache);
                }

            }
            return result;
        }

        private MonoAssembly GetMonoAssembly(MonoDomain domain, string name)
        {
            if(domain.domain_assemblies == 0) return new MonoAssembly();
            var currentAssemblyPointer = domain.domain_assemblies;
            while (currentAssemblyPointer != 0)
            {
                var currentAssemblyOffset = ReadUInt(currentAssemblyPointer);
                var currentAssembly = ReadStruct<MonoAssembly>(currentAssemblyOffset);
                var currentAssemblyName = WindowsHelper.ReadString(Process.Handle, currentAssembly.aname.name);
                if(currentAssemblyName == name)
                    return currentAssembly;
                currentAssemblyPointer = ReadUInt(currentAssemblyPointer + sizeof(uint));
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

            var functionCode = WindowsHelper.ParseFunctionCode(rawFunctionCode);
            var rootDomainPointer = WindowsHelper.ParseUInt(functionCode);
            if(rootDomainPointer == 0) return new MonoDomain();

            var rootDomainOffset = ReadUInt(rootDomainPointer);
            return ReadStruct<MonoDomain>(rootDomainOffset);
        }

        private T ReadStruct<T>(long offset) where T : struct
        {
            return WindowsHelper.ReadStruct<T>(Process.Handle, offset);
        }

        private uint ReadUInt(long offset)
        {
            return WindowsHelper.ReadUInt(Process.Handle, offset);
        }
    }
}
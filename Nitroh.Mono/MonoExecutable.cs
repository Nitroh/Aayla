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

        public long GetTest()
        {
            //TODO: this is test code
            var rootDomain = GetMonoDomain();
            var assembly = GetMonoAssembly(rootDomain, MonoAssemblyName);
            var classes = GetMonoClasses(assembly);
            var names = classes.Select(c => c.Name).ToList();
            return 1;
        }

        private IEnumerable<MonoClassEx> GetMonoClasses(MonoAssembly assembly)
        {
            var result = new List<MonoClassEx>();
            var image = ReadStruct<MonoImage>(assembly.image);
            for (var index = 0; index < image.class_cache.size; index++)
            {
                var monoClassPointer = ReadUInt(image.class_cache.table + 4*index);
                var monoClass = ReadStruct<MonoClass>(monoClassPointer);
                result.Add(new MonoClassEx(monoClass, Process.Handle));
            }
            return result;
        }

        private MonoAssembly GetMonoAssembly(MonoDomain domain, string name)
        {
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
            var rootDomainPointer = WindowsHelper.ReadUInt(functionCode);
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
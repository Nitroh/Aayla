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
        
        public IEnumerable<MonoClassEx> MonoClasses { get; private set; }

        public MonoExecutable(string processName) : base(processName, MonoModuleName)
        {
            MonoClasses = GetClasses();
        }

        protected override void Refresh()
        {
            base.Refresh();
            MonoClasses = GetClasses();
        }

        private IEnumerable<MonoClassEx> GetClasses()
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
            var image = ReadStruct<MonoImage>(assembly.image, false);
            for (var index = 0; index < image.class_cache.size; index++)
            {
                var monoClassPointer = ReadUInt(image.class_cache.table + 4*index, false);
                while (monoClassPointer != 0)
                {
                    var monoClass = ReadStruct<MonoClass>(monoClassPointer, false);
                    result.Add(new MonoClassEx(monoClass, this));
                    monoClassPointer = ReadUInt(monoClass.next_class_cache, false);
                }
            }
            return result;
        }

        private MonoAssembly GetMonoAssembly(MonoDomain domain, string name)
        {
            if (domain.domain_assemblies == 0)
            {
                return new MonoAssembly();
            }
            var currentAssemblyPointer = domain.domain_assemblies;
            while (currentAssemblyPointer != 0)
            {
                var currentAssemblyOffset = ReadUInt(currentAssemblyPointer, false);
                var currentAssembly = ReadStruct<MonoAssembly>(currentAssemblyOffset, false);
                var currentAssemblyName = ReadString(currentAssembly.aname.name, false);
                if(currentAssemblyName == name)
                    return currentAssembly;
                currentAssemblyPointer = ReadUInt(currentAssemblyPointer + sizeof(uint), false);
            }
            return new MonoAssembly();
        }

        private MonoDomain GetMonoDomain()
        {
            var rootDomainFunctionPointer = FunctionPointers.FirstOrDefault(x => x.Item1 == MonoGetRootDomain)?.Item2 ?? 0;
            if (rootDomainFunctionPointer == 0) return new MonoDomain();

            byte[] rawFunctionCode;
            var valid = GetMemory(BaseAddress + rootDomainFunctionPointer, MonoGetRootDomainFuncSize + 2, out rawFunctionCode, false);
            if(!valid) return new MonoDomain();

            var functionCode = ParseFunctionCode(rawFunctionCode);
            var rootDomainPointer = ParseUInt(functionCode);
            if(rootDomainPointer == 0) return new MonoDomain();

            var rootDomainOffset = ReadUInt(rootDomainPointer, false);
            return ReadStruct<MonoDomain>(rootDomainOffset, false);
        }
    }
}
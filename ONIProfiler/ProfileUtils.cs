using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Heinermann.ONIProfiler
{
  public class ProfileUtils
  {
    public static Assembly GetAssemblyByName(string name)
    {
      return AppDomain.CurrentDomain.GetAssemblies().
             SingleOrDefault(assembly => assembly.GetName().Name == name);
    }

    public static IEnumerable<MethodBase> GetTargetMethods()
    {
      Assembly assembly = GetAssemblyByName("Assembly-CSharp");
      if (assembly == null)
      {
        Debug.LogError("Failed to find assembly");
      }

      return assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsInterface )
        .SelectMany(type => AccessTools.GetDeclaredMethods(type))
        .Where(method => {
          bool isDllImport = false;
          foreach (object attr in method.GetCustomAttributes(false))
          {
            if (attr is DllImportAttribute) isDllImport = true;
          }
          return !method.ContainsGenericParameters &&
          !method.IsAbstract &&
          !method.IsVirtual &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.InternalCall) &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.Native) &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.Unmanaged) &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.InternalCall) &&
          !method.Attributes.HasFlag(MethodAttributes.PinvokeImpl) &&
          !method.Attributes.HasFlag(MethodAttributes.Abstract) &&
          !method.Attributes.HasFlag(MethodAttributes.UnmanagedExport) &&
          !method.Attributes.HasFlag(MethodAttributes.Virtual) &&
          !isDllImport;
        })
        .Cast<MethodBase>();
    }
    public static long ticksToNanoTime(long ticks)
    {
      return 10000L * ticks / TimeSpan.TicksPerMillisecond * 100L;
    }

  }
}

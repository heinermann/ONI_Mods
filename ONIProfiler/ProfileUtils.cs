using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

      char[] invalidChars = new[] { '<', '>' };

      return assembly.GetTypes()
        .Where(type => type.IsClass )
        .SelectMany(type => AccessTools.GetDeclaredMethods(type))
        .Where(method => {
          bool isDllImport = false;
          foreach (object attr in method.GetCustomAttributes(false))
          {
            if (attr is DllImportAttribute) isDllImport = true;
          }
          return !method.ContainsGenericParameters &&
          !method.IsAbstract &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.InternalCall) &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.Native) &&
          !method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.Unmanaged) &&
          !method.Attributes.HasFlag(MethodAttributes.PinvokeImpl) &&
          !method.Attributes.HasFlag(MethodAttributes.Abstract) &&
          !isDllImport; /*&&
          !method.Name.Any(invalidChars.Contains) &&
          !method.DeclaringType.FullName.Any(invalidChars.Contains) &&
          !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_");*/
        })
        .Cast<MethodBase>();
    }
    public static long ticksToNanoTime(long ticks)
    {
      return 10000L * ticks / TimeSpan.TicksPerMillisecond * 100L;
    }

  }
}

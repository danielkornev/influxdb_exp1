using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZU.Framework
{
    public static partial class AssemblyExtensions
    {
        /// <summary>
        /// See http://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesWithInterface(this Assembly asm, Type type)
        {
            var it = type;
            return asm.GetLoadableTypes().Where(it.IsAssignableFrom).ToList();
        }

        // source: http://www.java2s.com/Code/CSharp/Reflection/Detectswhetheranassemblyisimplementingaspecificinterfaceornot.htm


        /// <summary>
        /// Detects whether an assembly is implementing a specific interface or not.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="interfaceName">The String containing the name of the interface to get. For generic interfaces, this is the mangled name.</param>
        /// <returns>true if one of the assembly classes implements the specified interface; otherwise, false.</returns>
        public static bool IsInterfaceImplemented(this Assembly assembly, string interfaceName)
        {
            // ensure interface name is not null
            if (!string.IsNullOrEmpty(interfaceName) && interfaceName.Length > 0)
            {
                // Next we'll loop through all the Types found in the assembly
                foreach (Type pluginType in assembly.GetTypes())
                {
                    if (!pluginType.IsPublic) continue;
                    if (pluginType.IsAbstract) continue;
                    try
                    {
                        // Gets a type object of the interface we need the assembly to match
                        var typeInterface = pluginType.GetInterface(interfaceName, true);

                        // Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
                            return true;
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        var sb = new StringBuilder();
                        foreach (var exSub in ex.LoaderExceptions)
                        {
                            sb.AppendLine(exSub.Message);
                            var exFileNotFound = exSub as FileNotFoundException;
                            if (!string.IsNullOrEmpty(exFileNotFound?.FusionLog))
                            {
                                sb.AppendLine("Fusion Log:");
                                sb.AppendLine(exFileNotFound.FusionLog);
                            }
                            sb.AppendLine();
                        }
                        var errorMessage = sb.ToString();
                        //Display or log the error based on your application.
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = ex.Message;
                    }
                }
            }
            return false;
        }
    } // class
} // namespace

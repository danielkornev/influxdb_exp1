using System;
using System.Reflection;

namespace ZU.Framework
{
    public static class AssemblyLoader
    {

        public static Assembly TryToIdentifyAndLoadAssembly(string file, Type type)
        {
            try
            {
                var asm = Assembly.LoadFrom(file);

                var typeName = type.FullName;

                if (!asm.IsInterfaceImplemented(typeName))
                {
                    throw new Exception("Given assembly doesn't have any implementations of the given interface");
                }

                return asm;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception was thown while trying to load the given assembly:\n" + ex.Message);
            }
        }
    }
}
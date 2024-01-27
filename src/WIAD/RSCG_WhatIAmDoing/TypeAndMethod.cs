using System.Diagnostics;

namespace RSCG_WhatIAmDoing;
[DebuggerDisplay("{MethodInvocation}")]
public class TypeAndMethod
{
    public static TypeAndMethod InvalidEmpty = new TypeAndMethod("", "", null, "");
    public TypeAndMethod(string typeOfClass, string methodInvocation, ITypeSymbol? typeReturn, string nameOfVariable)
    {
        InstanceIsNotNull = true;
        TypeOfClass = typeOfClass;
        MethodInvocation = methodInvocation;
        TypeReturn = typeReturn?.ToString()??"";
        
        NameOfVariable = nameOfVariable;

    }
    public TypeAndMethod(string staticMethod, string typeReturn)
    {
        InstanceIsNotNull = false;
        var methods = staticMethod.Split('.');
        TypeOfClass = methods[0] + ".";
        for (int i = 1; i < methods.Length - 1; i++)
        {
            TypeOfClass += methods[i];
            TypeOfClass += ".";
        }
        TypeOfClass = TypeOfClass.TrimEnd('.');
        MethodInvocation = staticMethod;
        TypeReturn = typeReturn;

        NameOfVariable = "";
    }
    public bool InstanceIsNotNull { get; }
    public string TypeOfClass { get; }
    public string MethodInvocation { get; }
    public string TypeReturn { get; }
    public string NameOfVariable { get; set; }

    public Argument[] ValueArguments { get; set; } = Array.Empty<Argument>();
    public Argument[] StringArguments { get; set; } = Array.Empty<Argument>();

    private static Argument[] ValueArgs(TypeAndMethod source)
    {
        return source.Arguments.Where(it => it.IsValueType).ToArray();
    }
    const string typeofString = "String";
    private static Argument[] StringArgs(TypeAndMethod source)
    {
        return source.Arguments
            .Where(it => !it.IsValueType)
            .Where(it =>
                (it.TypeArgument == typeofString)
                ||
                (it.TypeArgument == typeofString + "?")
                )
            .ToArray();
    }



    public bool IsStatic
    {
        get
        {
            return !InstanceIsNotNull;
        }
    }
    public bool HasTaskReturnType
    {
        get
        {
            return TypeReturn.Contains("System.Threading.Tasks.Task");
        }
    }
    public bool IsValid()
    {

        return TypeOfClass.Length > 0 && MethodInvocation.Length > 0;

    }
    public string MethodName
    {
        get
        {
            if (this.InstanceIsNotNull)
            {
                return MethodInvocation;
            }
            else
            {
                return MethodInvocation.Split('.').Last().Replace("()", "");
            }
        }
    }
    public bool IsVoid()
    {
        return TypeReturn == "void";
    }
    public string ReturnString
    {
        get
        {
            if (IsVoid())
            {
                return "";
            }
            else
            {
                return $"return";
            }
        }
    }
    public string CallMethod
    {
        get
        {
            var args = string.Join(",", Arguments.Select(a => a.Name));
            if (this.InstanceIsNotNull)
            {
                return $"{DisplayNameOfVariable}.{MethodInvocation}({args})";
            }
            else
            {
                return $"{MethodInvocation}({args})";
            }
        }
    }
    public string ArgumentsForCallMethod
    {
        get
        {
            string args = string.Join(",", Arguments.Select(a => a.TypeAndName));

            if (this.InstanceIsNotNull && args.Length > 0)
            {
                //first argument is this
                args = "," + args;
            }
            return args;
        }
    }
    public string DisplayNameOfVariable
    {
        get
        {
            if (this.NameOfVariable.Length==0)
            {
                return "selfThis";
            }
            else
            {
                return NameOfVariable;
            }
        }
    }
    public string ThisArgument
    {
        get
        {
            if (this.InstanceIsNotNull)
            {
                
                return $"this {TypeOfClass} {DisplayNameOfVariable}";
            }
            else
            {
                return "";
            }
        }
    }
    static int nrSignature=0;
    public string MethodSignature
    {
        get
        {
            var nameOfVariable = NameOfVariable.Replace(".", "_");
            return $"Intercept_{nameOfVariable}_{MethodName}_{nrSignature++}";
        }
    }
    public void SetArguments(Argument[] arguments)
    {
        Arguments = arguments;
        ValueArguments = ValueArgs(this);
        StringArguments = StringArgs(this);
        //var x = "asd";
    }
    public Argument[] Arguments { get; private set; } = [];
    public int NrArguments
    {
        get
        {
            return Arguments.Length;
        }
    }
}
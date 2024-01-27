﻿using Microsoft.CodeAnalysis.Operations;

namespace RSCG_WhatIAmDoing;
[Generator]
public class GeneratorWIAD : IIncrementalGenerator
{
    private static bool IsAppliedOnClass(
    SyntaxNode syntaxNode,
    CancellationToken cancellationToken)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclaration;

    }
    private static DataFromInterceptStatic FindAttributeDataStatic(
    GeneratorAttributeSyntaxContext context,
    CancellationToken cancellationToken)
    {
        var type = (INamedTypeSymbol)context.TargetSymbol;
        var dataInfo = new DataFromInterceptStatic(type);
        return dataInfo;
    }

    private static DataFromInterceptClass FindAttributeDataClass(
    GeneratorAttributeSyntaxContext context,
    CancellationToken cancellationToken)
    {
        var type = (INamedTypeSymbol)context.TargetSymbol;
        var dataInfo = new DataFromInterceptClass(type);
        return dataInfo;
    }
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //find attributes to intercept
        var staticToIntercept = context.SyntaxProvider.ForAttributeWithMetadataName(
            "RSCG_WhatIAmDoing.InterceptStaticAttribute",
            IsAppliedOnClass,
            FindAttributeDataStatic
            )
             .Collect()
            .SelectMany((data, _) => data.Distinct())
            .Collect()
        ;

        var instancesToIntercept = context.SyntaxProvider.ForAttributeWithMetadataName(
            "RSCG_WhatIAmDoing.InterceptInstanceClassAttribute",
            IsAppliedOnClass,
            FindAttributeDataClass
            )
             .Collect()
            .SelectMany((data, _) => data.Distinct())
            .Collect()
        ;

        var classesToIntercept = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (context, token) =>
                {
                    var operation = context.SemanticModel.GetOperation(context.Node, token);
                    return operation;
                })
            .Where(static m => m is not null)!
            ;
        var compilationAndDataStatic
         = context.CompilationProvider.Combine(classesToIntercept.Collect())
           .Combine(staticToIntercept);
        //.Combine(context.AdditionalTextsProvider.Collect());

        context.RegisterSourceOutput(compilationAndDataStatic,
           (spc, data) =>
           ExecuteGenStatic(spc, data!));

        var compilationAndDataInstance
 = context.CompilationProvider.Combine(classesToIntercept.Collect())
   .Combine(instancesToIntercept);
        //.Combine(context.AdditionalTextsProvider.Collect());

        context.RegisterSourceOutput(compilationAndDataInstance,
           (spc, data) =>
           ExecuteGenInstance(spc, data!));

    }
    private void ExecuteGenInstance(SourceProductionContext spc, ((Compilation Left, System.Collections.Immutable.ImmutableArray<IOperation> Right) Left, System.Collections.Immutable.ImmutableArray<DataFromInterceptClass> Right) value)
    {
        var AlltypesToIntercept = value
    .Right.ToArray()
    .Select(it => it.TypeTo)
    .Distinct()
    .ToArray();

        ;
        var types = AlltypesToIntercept
            .SelectMany(it => it.Split(','))
            .Select(it => it?.Trim())
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .Select(it => it!)
            .Distinct()
            .ToArray();

        if (types.Length == 0)
            return;

        var compilation = value.Left.Left;

        var ops = value
    .Left.Right
    .Select(op =>
    {
        TryGetMapMethodName(op.Syntax, out var methodName);
        var typeReturn = op.Type;
        var invocation = op as IInvocationOperation;
        Argument[] arguments = [];
        IOperation? instance = invocation?.Instance as ILocalReferenceOperation;
        if(instance == null)
        {
            instance = invocation?.Instance as IInstanceReferenceOperation;
        }
        TypeAndMethod typeAndMethod;
        if (instance == null)
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);
        }
        var typeOfClass = instance.Type;
        if (typeOfClass == null)
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);
        }
        var s=typeOfClass.ToString();
        if(!types.Contains(s))
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);
        }
        var nameVar = "";
        if(instance is ILocalReferenceOperation localOp)
            nameVar = localOp.Local.Name;

        typeAndMethod = new TypeAndMethod(typeOfClass?.ToString() ?? "", methodName ?? "", typeReturn, nameVar);

        if (invocation != null && invocation.Arguments.Length > 0)
        {
            arguments = invocation
            .Arguments
            .Where(it => it?.Parameter != null)
            .Select(it => new Argument(it!.Parameter!))
            .ToArray();
        }

        typeAndMethod.SetArguments(arguments);

        return new Tuple<TypeAndMethod, IOperation>(typeAndMethod, op);

    })
    .Where(it => it.Item1.IsValid())
    .GroupBy(x => x.Item1)
    .ToDictionary(x => x.Key, x => x.ToArray())
    ;

        if (ops.Keys.Count == 0)
            return;
        var nrFilesPerMethodAndClass = ops.Keys
            .GroupBy(it => it.TypeOfClass + "_" + it.MethodName)
            .Select(a => new { a.Key, Count = a.Count() })
            .ToDictionary(it => it.Key, it => (number: 0, total: it.Count));

        Dictionary<TypeAndMethod, DataForSerializeFile> dataForSerializeFiles = new();
        foreach (var item in ops.Keys)
        {

            var ser = new DataForSerializeFile(item);
            var val = ops[item];
            dataForSerializeFiles.Add(item, ser);
            ser.item = item;

            var methodName = item.MethodName;
            var typeOfClass = item.TypeOfClass;

            var nameOfVariable = item.NameOfVariable;
            //int extraLength = ser.extraLength;

            foreach (var itemData in ops[item])
            {
                DataForEachIntercept dataForEachIntercept = new();
                ser.dataForEachIntercepts.Add(dataForEachIntercept);
                var op = itemData.Item2;
                var tree = op.Syntax.SyntaxTree;
                var filePath = compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
                var location = tree.GetLocation(op.Syntax.Span);
                var lineSpan = location.GetLineSpan();
                var startLinePosition = lineSpan.StartLinePosition;
                SourceText sourceText = location.SourceTree!.GetText();
                var line = sourceText.Lines[startLinePosition.Line];
                string code = line.ToString();
                dataForEachIntercept.code = code;
                dataForEachIntercept.Path = lineSpan.Path;
                dataForEachIntercept.Line = startLinePosition.Line + 1;
                int startMethod;
                if(item.NameOfVariable.Length>0)
                    startMethod = code.IndexOf(item.NameOfVariable + "." + item.MethodName + "(", startLinePosition.Character)+1;//dot
                else
                    startMethod = code.IndexOf(item.MethodName + "(", startLinePosition.Character);
                //dataForEachIntercept.StartMethod = startLinePosition.Character + 1 + extraLength;
                dataForEachIntercept.StartMethod = startMethod + item.NameOfVariable.Length + 1;//dot
            }


            //Diagnostic d = Diagnostic.Create(
            //    new DiagnosticDescriptor("RSCG_InterceptorTemplate",
            //    "RSCG_WIAD_001",
            //    $"no template for method {name}", "RSCG_InterceptorTemplate",
            //    DiagnosticSeverity.Warning,
            //    true),
            //    Location.None);

            //spc.ReportDiagnostic(d);
            //write default?            
            //spc.AddSource(ser.Value.nameFileToBeWritten + ".cs", ser.Value.DataToBeWriten);
            //continue;


        }
        foreach (var ser1 in dataForSerializeFiles)
        {

            RSCG_WhatIAmDoing.AOPInstance v = new AOPInstance(ser1.Value);
            string fileContent = v.Render();
            spc.AddSource(ser1.Value.nameFileToBeWritten + ".cs", fileContent);
        }


    }

    private void ExecuteGenStatic(SourceProductionContext spc, ((Compilation Left, System.Collections.Immutable.ImmutableArray<IOperation> Right) Left, System.Collections.Immutable.ImmutableArray<DataFromInterceptStatic> Right) value)
    {
        var AlltypesToIntercept = value
    .Right.ToArray()
    .Select(it => it.TypesTo)
    .Distinct()
    .ToArray();

        ;
        var types = AlltypesToIntercept
            .SelectMany(it => it.Split(','))
            .Select(it => it?.Trim())
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .Select(it => it!)
            .Distinct()
            .ToArray();

        if (types.Length == 0)
            return;

        var compilation = value.Left.Left;

        var ops = value
    .Left.Right
    .Select(op =>
    {
        TryGetMapMethodName(op.Syntax, out var methodName);
        var typeReturn = op.Type;
        var invocation = op as IInvocationOperation;
        Argument[] arguments = [];
        var instance = invocation?.Instance as ILocalReferenceOperation;
        TypeAndMethod typeAndMethod;
        if (instance != null)
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);

        }
        var staticMember = invocation?.TargetMethod?.ToDisplayString();
        if (staticMember is null)
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);
        }
        bool hasType = false;
        foreach (var item in types)
        {
            var regex = item;

            var temp = Regex.IsMatch(staticMember, regex);
            if (temp)
            {
                hasType = true;
                break;
            }
        }

        if (!hasType)
        {
            return new Tuple<TypeAndMethod, IOperation>(TypeAndMethod.InvalidEmpty, op);
        }
        var justMethod = staticMember?.IndexOf("(");
        if (justMethod != null && justMethod > 0)
        {
            staticMember = staticMember?.Substring(0, justMethod.Value);
        }
        //if(staticMember != null)
        //{
        //    var typeOfClass = compilation.GetTypeByMetadataName(staticMember);
        //}
        typeAndMethod = new TypeAndMethod(staticMember ?? "", typeReturn?.ToString() ?? "");
        var nameMethod = typeAndMethod.MethodName;
        //string fullCall = op.Syntax.ToFullString();
        //bool replaceEmptyLines = true;
        //while (replaceEmptyLines)
        //{
        //    replaceEmptyLines = false;
        //    if (fullCall.StartsWith("\r"))
        //    {
        //        replaceEmptyLines = true;
        //        fullCall = fullCall.Substring(1);
        //    }
        //    if (fullCall.StartsWith("\n"))
        //    {
        //        replaceEmptyLines = true;
        //        fullCall = fullCall.Substring(1);
        //    }
        //}
        //fullCall = fullCall.Trim();
        //justMethod = fullCall.IndexOf("(");
        //if (justMethod != null && justMethod > 0)
        //{
        //    fullCall = fullCall.Substring(0, justMethod.Value);
        //}
        //var nameVar = fullCall.Substring(0, fullCall.Length - nameMethod.Length - ".".Length);
        //typeAndMethod.NameOfVariable = nameVar;



        if (invocation != null && invocation.Arguments.Length > 0)
        {
            arguments = invocation
            .Arguments
            .Where(it => it?.Parameter != null)
            .Select(it => new Argument(it!.Parameter!))
            .ToArray();
        }

        typeAndMethod.SetArguments(arguments);

        return new Tuple<TypeAndMethod, IOperation>(typeAndMethod, op);

    })
    .Where(it => it.Item1.IsValid())
    .GroupBy(x => x.Item1)
    .ToDictionary(x => x.Key, x => x.ToArray())
    ;

        if (ops.Keys.Count == 0)
            return;
        var nrFilesPerMethodAndClass = ops.Keys
            .GroupBy(it => it.TypeOfClass + "_" + it.MethodName)
            .Select(a => new { a.Key, Count = a.Count() })
            .ToDictionary(it => it.Key, it => (number: 0, total: it.Count));

        Dictionary<TypeAndMethod, DataForSerializeFile> dataForSerializeFiles = new();
        foreach (var item in ops.Keys)
        {

            var ser = new DataForSerializeFile(item);
            var val = ops[item];
            dataForSerializeFiles.Add(item, ser);
            ser.item = item;

            var methodName = item.MethodName;
            var typeOfClass = item.TypeOfClass;

            var nameOfVariable = item.NameOfVariable;
            //int extraLength = ser.extraLength;

            foreach (var itemData in ops[item])
            {
                DataForEachIntercept dataForEachIntercept = new();
                ser.dataForEachIntercepts.Add(dataForEachIntercept);
                var op = itemData.Item2;
                var tree = op.Syntax.SyntaxTree;
                var filePath = compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
                var location = tree.GetLocation(op.Syntax.Span);
                var lineSpan = location.GetLineSpan();
                var startLinePosition = lineSpan.StartLinePosition;
                SourceText sourceText = location.SourceTree!.GetText();
                var line = sourceText.Lines[startLinePosition.Line];
                string code = line.ToString();
                dataForEachIntercept.code = code;
                dataForEachIntercept.Path = lineSpan.Path;
                dataForEachIntercept.Line = startLinePosition.Line + 1;
                var startMethod = code.IndexOf(item.MethodName + "(", startLinePosition.Character);
                //dataForEachIntercept.StartMethod = startLinePosition.Character + 1 + extraLength;
                dataForEachIntercept.StartMethod = startMethod + 1;
            }


            //Diagnostic d = Diagnostic.Create(
            //    new DiagnosticDescriptor("RSCG_InterceptorTemplate",
            //    "RSCG_WIAD_001",
            //    $"no template for method {name}", "RSCG_InterceptorTemplate",
            //    DiagnosticSeverity.Warning,
            //    true),
            //    Location.None);

            //spc.ReportDiagnostic(d);
            //write default?            
            //spc.AddSource(ser.Value.nameFileToBeWritten + ".cs", ser.Value.DataToBeWriten);
            //continue;


        }
        foreach (var ser1 in dataForSerializeFiles)
        {

            RSCG_WhatIAmDoing.AOPStatic v = new AOPStatic(ser1.Value);
            string fileContent = v.Render();
            spc.AddSource(ser1.Value.nameFileToBeWritten + ".cs", fileContent);
        }


    }

    private bool IsSyntaxTargetForGeneration(SyntaxNode s)
    {

        if (!TryGetMapMethodName(s, out var method))
            return false;
        return true;

    }
    public static bool TryGetMapMethodName(SyntaxNode node, out string? methodName)
    {
        if (node is not InvocationExpressionSyntax inv)
        {
            methodName = default;
            return false;
        }
        methodName = default;
        if (inv is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier: { ValueText: var methodValue } } })
        {
            methodName = methodValue;
        }
        if (inv is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: var method } } } })
        {
            methodName = method;
        }
        if (string.IsNullOrWhiteSpace(methodName))
            return false;

        var parent = inv.Parent;
        while (parent is not null && parent is not ClassDeclarationSyntax)
        {
            parent = parent.Parent;
        }
        if (parent == null)
            return true;//in program.cs without declaration of a class
        var cds = parent as ClassDeclarationSyntax;
        if (cds is null)
            return false;
        var attr = cds.AttributeLists;
        if (attr.Count == 0)
            return true;//no intercept static attribute

        var existsStatic =
             attr
             .SelectMany(it => it.Attributes)
             .Select(it => it.Name)
             .Any(it => it.ToFullString().Contains("InterceptStatic"));
        if (existsStatic) //do not intercept the interceptor
            return false;

        return true;
    }

}

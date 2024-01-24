using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace RSCG_WhatIAmDoing;
[Generator]
public class GeneratorWIAD : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        MethodsToIntercept methods = new();

        var data=methods.Methods.Select(static m => m.MethodName!).ToArray();
        var classesToIntercept = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s, data),
                transform: static (context, token) =>
                {
                    var operation = context.SemanticModel.GetOperation(context.Node, token);
                    return operation;
                })
            .Where(static m => m is not null)!
            ;
        var compilationAndData
         = context.CompilationProvider.Combine(classesToIntercept.Collect()).Combine(context.AdditionalTextsProvider.Collect());
        context.RegisterSourceOutput(compilationAndData,
           (spc, data) =>
           ExecuteGen(spc, data!, methods));

    }

    private void ExecuteGen(SourceProductionContext spc, ((Compilation Left, System.Collections.Immutable.ImmutableArray<IOperation> Right) Left, System.Collections.Immutable.ImmutableArray<AdditionalText> Right) value,MethodsToIntercept methods)
    {
        var textes = value
    .Right.ToArray()
    .Select(it => new { it.Path, text = it.GetText()?.ToString() })
    .ToArray();
        ;

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
        if (instance == null)
        {
            var staticMember = invocation?.TargetMethod?.ToDisplayString();
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
            string fullCall = op.Syntax.ToFullString();
            bool replaceEmptyLines = true;
            while (replaceEmptyLines)
            {
                replaceEmptyLines= false;
                if (fullCall.StartsWith("\r"))
                {
                    replaceEmptyLines = true;
                    fullCall = fullCall.Substring(1);
                }
                if (fullCall.StartsWith("\n"))
                {
                    replaceEmptyLines = true;
                    fullCall = fullCall.Substring(1);
                }
            }
            fullCall= fullCall.Trim();
            justMethod = fullCall.IndexOf("(");
            if (justMethod != null && justMethod > 0)
            {
                fullCall = fullCall.Substring(0, justMethod.Value);
            }
            var nameVar = fullCall.Substring(0, fullCall.Length - nameMethod.Length - ".".Length);
            typeAndMethod.NameOfVariable = nameVar;

        }
        else
        {
            var typeOfClass = instance.Type;
            var nameVar = instance.Local.Name;
            typeAndMethod = new TypeAndMethod(typeOfClass?.ToString() ?? "", methodName ?? "", typeReturn?.ToString() ?? "", nameVar);

        }
        //maybe do this before?
        if(!methods.Methods.Any( m => m.ItsATypeAndMethod(typeAndMethod)))
        {
            return new Tuple<TypeAndMethod,IOperation>( TypeAndMethod.InvalidEmpty, op ); 
        }

        
        if (invocation != null && invocation.Arguments.Length > 0)
        {
            arguments = invocation
            .Arguments
            .Where(it => it?.Parameter != null)
            .Select(it => new Argument(it!.Parameter!))
            .ToArray();
        }

        typeAndMethod.SetArguments (arguments);

        return new Tuple<TypeAndMethod, IOperation>(typeAndMethod, op );

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
            dataForSerializeFiles.Add(item, ser);
            ser.item = item;

            var methodName = item.MethodName;
            var typeOfClass = item.TypeOfClass;
            //string nameFile = typeOfClass + "_" + methodName;
            //var nrFiles = nrFilesPerMethodAndClass[nameFile];

            //var nr = nrFiles.number;
            //nr++;
            //nrFilesPerMethodAndClass[nameFile]= (nr, nrFiles.total);
            //nameFile += $"_nr_{nr}_from_{nrFiles.total}";
            //var typeReturn = item.TypeReturn;
            //ser.nameFileToBeWritten = nameFile;
            var nameOfVariable = item.NameOfVariable;
            int extraLength = ser.extraLength;

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
                dataForEachIntercept.StartMethod = startLinePosition.Character + 1 + extraLength;
            }

        }

        foreach (var ser in dataForSerializeFiles)
        {
            var name = ser.Key.MethodInvocation;
            var fileText = textes.FirstOrDefault(it => it.Path.EndsWith(name + ".txt"))?.text;
            if (fileText == null)
            {
                fileText = textes.FirstOrDefault(it => it.Path.EndsWith("GenericInterceptorForAllMethods.txt"))?.text;

            }
            if (fileText != null)
            {
                Template? template=null;
                try
                {
                    //template = Template.Parse(fileText);
                    //string fileContent = template.Render(new { ser = ser.Value, Version = "8.2023.2811.524" }, m => m.Name);
                    RSCG_WhatIAmDoing.AOP v=new AOP(ser.Value);
                    string fileContent = v.Render();
                    spc.AddSource(ser.Value.nameFileToBeWritten + ".cs", fileContent);
                }
                catch( Exception)
                {
                    var m=template?.Messages?.FirstOrDefault();
                    var msg = m?.ToString() ?? "";
                    Diagnostic d1 = Diagnostic.Create(
    new DiagnosticDescriptor("RSCG_InterceptorTemplate",
    "RSCG_WIAD_002",
    $"parsing template error method {name} " +msg, "RSCG_InterceptorTemplate",
    DiagnosticSeverity.Error,
    true),
    Location.None);

                    spc.ReportDiagnostic(d1);

                }
                continue;
            }
            Diagnostic d = Diagnostic.Create(
                new DiagnosticDescriptor("RSCG_InterceptorTemplate",
                "RSCG_WIAD_001",
                $"no template for method {name}", "RSCG_InterceptorTemplate",
                DiagnosticSeverity.Warning,
                true),
                Location.None);

            spc.ReportDiagnostic(d);
            //write default?            
            //spc.AddSource(ser.Value.nameFileToBeWritten + ".cs", ser.Value.DataToBeWriten);
            continue;


        }


    }

    private bool IsSyntaxTargetForGeneration(SyntaxNode s, string[] data)
    {
        if (data.Length == 0) return false;
        if (!TryGetMapMethodName(s, out var method))
            return false;
        if (data.Contains(method))
            return true;
        return false;

    }
    public static bool TryGetMapMethodName(SyntaxNode node, out string? methodName)
    {
        methodName = default;
        if (node is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: var method } } } } m)
        {
            methodName = method;
            return true;
        }
        return false;
    }

}

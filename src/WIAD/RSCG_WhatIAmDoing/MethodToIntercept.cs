
namespace RSCG_WhatIAmDoing;
class MethodToIntercept
{
    
    public string? MethodName { get; set; }
    public  string? ClassName { get; set; }
    public bool IsStatic { get; set; }
    
    public bool IsValid()
    {
        if(string.IsNullOrWhiteSpace(MethodName))
        {
            return false;
        }
        if(string.IsNullOrWhiteSpace(ClassName))
        {
            return false;
        }
        return true;
    }
    public string MethodInvocation
    {
        get
        {
            return ClassName + "." + MethodName;            
        }
    }
    public bool ItsATypeAndMethod(TypeAndMethod typeAndMethod)
    {
        if(!IsValid())        
            return false;
        
        if(!(typeAndMethod.IsStatic == IsStatic))
            return false;
        
        if(IsStatic)
        if (!(typeAndMethod.MethodInvocation == MethodInvocation))
            return false;

        if(!IsStatic)
            throw new NotImplementedException();
        return true;

    }
} 

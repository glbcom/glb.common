using System;

namespace Glb.Common.Exceptions;
public class NoDocumentsModifiedException : Exception
{
    public NoDocumentsModifiedException(string Message) : base(Message){}  
}
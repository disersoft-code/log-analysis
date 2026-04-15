namespace LogAnalysis.Models;
using System;

public class AnalyzeProcessFilesEventArgs<T> : EventArgs
{
    public T Data { get; }
    public AnalyzeMessageType Type { get; }
    public string Message { get; }

    public AnalyzeProcessFilesEventArgs(T data, AnalyzeMessageType type, string message)
    {
        Data = data;
        Type = type;
        Message = message;
    }
}
using System;
using System.IO;
using System.Text;

namespace SFD.Scripting.Resources.Generator;

public sealed class CodeWriter : StreamWriter
{
    public int Padding;

    public CodeWriter(Stream stream) : base(stream) { }

    public CodeWriter(Stream stream, Encoding? encoding) : base(stream, encoding) { }

    public CodeWriter(Stream stream, Encoding? encoding, int bufferSize) : base(stream, encoding, bufferSize) { }

    public CodeWriter(Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, bufferSize, leaveOpen) { }

    public CodeWriter(string path) : base(path) { }

    public CodeWriter(string path, bool append) : base(path, append) { }

    public CodeWriter(string path, bool append, Encoding? encoding) : base(path, append, encoding) { }

    public CodeWriter(string path, bool append, Encoding? encoding, int bufferSize) : base(path, append, encoding, bufferSize) { }

    public CodeWriter(string path, FileStreamOptions options) : base(path, options) { }

    public CodeWriter(string path, Encoding? encoding, FileStreamOptions options) : base(path, encoding, options) { }

    public void IncrementPadding()
    {
        Padding += 4;
    }

    public void DecrementPadding()
    {
        if (Padding >= 4) Padding -= 4;
    }

    private string? PadLines(string? value)
    {
        if (Padding == 0 || string.IsNullOrEmpty(value)) return value;

        string[] lines = value.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
            lines[i] = lines[i].PadLeft(lines[i].Length + Padding);

        return string.Join(Environment.NewLine, lines);
    }

    public override void WriteLine(string? value)
    {
        base.WriteLine(PadLines(value));
    }

    public override void Write(string? value)
    {
        base.WriteLine(PadLines(value));
    }

    public override void WriteLine(char value)
    {
        base.WriteLine(Padding == 0 ? value : value.ToString().PadLeft(1 + Padding));
    }

    public override void Write(char value)
    {
        base.WriteLine(Padding == 0 ? value : value.ToString().PadLeft(1 + Padding));
    }
}

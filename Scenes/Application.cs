using Godot;
using System;

public partial class Application : Control
{
    public override void _Ready()
    {
        var filesManager = GetNode<FilesManager>("HSplitContainer/FilesManager");
        var markdownEditor = GetNode<MarkdownEditor>("HSplitContainer/MarkdownEditor");

        filesManager.FileSelected += markdownEditor.OpenFile;
    }
}

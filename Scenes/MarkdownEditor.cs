using Godot;
using System;
using System.IO;

public partial class MarkdownEditor : Control
{
    private TextEdit _editor;
    private RichTextLabel _preview;
    private string _currentFilePath;

    public override void _Ready()
    {
        _editor = GetNode<TextEdit>("HSplitContainer/Editor");
        _preview = GetNode<RichTextLabel>("HSplitContainer/Preview");

        _editor.TextChanged += OnEditorTextChanged;
    }

    public void OpenFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            _currentFilePath = filePath;
            string content = File.ReadAllText(filePath);
            _editor.Text = content;
            UpdatePreview(content);
        }
    }

    private void OnEditorTextChanged()
    {
        string content = _editor.Text;
        UpdatePreview(content);
        // TODO: Implement auto-save or save indicator
    }

    private void UpdatePreview(string markdownContent)
    {
        // TODO: Implement Markdown to BBCode conversion
        _preview.Text = markdownContent;
    }

    public void SaveFile()
    {
        if (!string.IsNullOrEmpty(_currentFilePath))
        {
            File.WriteAllText(_currentFilePath, _editor.Text);
            // TODO: Update save indicator
        }
    }
}

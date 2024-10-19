using Godot;
using System;
using System.IO;

public partial class MarkdownEditor : Control
{
    private TextEdit _editor;
    private RichTextLabel _preview;
    private string _currentFilePath;
    private bool _hasUnsavedChanges = false;

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
            _hasUnsavedChanges = false;
            UpdateTitle();
        }
    }

    private void OnEditorTextChanged()
    {
        string content = _editor.Text;
        UpdatePreview(content);
        _hasUnsavedChanges = true;
        UpdateTitle();
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
            _hasUnsavedChanges = false;
            UpdateTitle();
        }
    }

    private void UpdateTitle()
    {
        if (!string.IsNullOrEmpty(_currentFilePath))
        {
            string fileName = Path.GetFileName(_currentFilePath);
            string title = fileName + (_hasUnsavedChanges ? "*" : "");
            // TODO: Update the title of the tab or window
            GD.Print($"File: {title}"); // Temporary, replace with actual title update
        }
    }
}

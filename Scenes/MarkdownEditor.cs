using Godot;
using System.IO;
using System.Collections.Generic;
using System.Linq; // 添加这行以使用 LINQ

public partial class MarkdownEditor : Control
{
    private TextEdit _editor;
    private RichTextLabel _preview;
    private TabBar _tabBar;
    private string _currentFilePath;
    private bool _hasUnsavedChanges = false;
    private Dictionary<int, string> _openFiles = new Dictionary<int, string>();

    public override void _Ready()
    {
        _editor = GetNode<TextEdit>("VBoxContainer/HSplitContainer/Editor");
        _preview = GetNode<RichTextLabel>("VBoxContainer/HSplitContainer/Preview");
        _tabBar = GetNode<TabBar>("VBoxContainer/TabBar");

        _editor.TextChanged += OnEditorTextChanged;
        _tabBar.TabClicked += OnTabClicked; // 修改这行
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.S && keyEvent.CtrlPressed)
            {
                SaveFile();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public void OpenFile(string filePath)
    {
        GD.Print($"Opening file: {filePath}");
        if (File.Exists(filePath))
        {
            if (!_openFiles.ContainsValue(filePath))
            {
                _tabBar.AddTab(Path.GetFileName(filePath)); // 移除返回值赋值
                int tabIndex = _tabBar.TabCount - 1; // 获取新添加的标签索引
                _openFiles[tabIndex] = filePath;
                _tabBar.CurrentTab = tabIndex;
            }
            else
            {
                int tabIndex = _openFiles.First(x => x.Value == filePath).Key; // 使用 First 而不是 FirstOrDefault
                _tabBar.CurrentTab = tabIndex;
            }

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
            int currentTab = _tabBar.CurrentTab;
            _tabBar.SetTabTitle(currentTab, title);
        }
    }

    private void OnTabClicked(long tabIndex) // 修改方法签名
    {
        if (_openFiles.TryGetValue((int)tabIndex, out string filePath)) // 转换 long 为 int
        {
            OpenFile(filePath);
        }
    }
}

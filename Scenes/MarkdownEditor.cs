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
        _tabBar.TabChanged += OnTabChanged;
        _tabBar.TabClosePressed += OnTabClosePressed; // 添加这行
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
            else if (keyEvent.Keycode == Key.W && keyEvent.CtrlPressed)
            {
                CloseCurrentTab();
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
                _tabBar.AddTab(Path.GetFileName(filePath));
                int tabIndex = _tabBar.TabCount - 1;
                _openFiles[tabIndex] = filePath;
                _tabBar.CurrentTab = tabIndex;
            }
            else
            {
                int tabIndex = _openFiles.First(x => x.Value == filePath).Key;
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

    private void OnTabChanged(long tabIndex)
    {
        if (_openFiles.TryGetValue((int)tabIndex, out string filePath))
        {
            OpenFile(filePath);
        }
    }

    public void CloseCurrentTab()
    {
        int currentTab = _tabBar.CurrentTab;
        if (currentTab >= 0)
        {
            CloseFile(currentTab);
        }
    }

    private void CloseFile(int tabIndex)
    {
        if (_openFiles.TryGetValue(tabIndex, out string filePath))
        {
            // TODO: 检查是否有未保存的更改，并提示用户保存

            _openFiles.Remove(tabIndex);
            _tabBar.RemoveTab(tabIndex);

            // 更新剩余标签的索引
            var updatedOpenFiles = new Dictionary<int, string>();
            for (int i = 0; i < _tabBar.TabCount; i++)
            {
                if (_openFiles.TryGetValue(i, out string path))
                {
                    updatedOpenFiles[i] = path;
                }
            }
            _openFiles = updatedOpenFiles;

            // 如果关闭的是当前标签，切换到新的当前标签
            if (_currentFilePath == filePath)
            {
                if (_tabBar.TabCount > 0)
                {
                    int newTabIndex = tabIndex >= _tabBar.TabCount ? _tabBar.TabCount - 1 : tabIndex;
                    _tabBar.CurrentTab = newTabIndex;
                    OpenFile(_openFiles[newTabIndex]);
                }
                else
                {
                    // 没有打开的文件了，清空编辑器
                    _currentFilePath = null;
                    _editor.Text = "";
                    _preview.Text = "";
                }
            }
        }
    }

    private void OnTabClosePressed(long tabIndex)
    {
        CloseFile((int)tabIndex);
    }
}

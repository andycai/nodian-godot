using Godot;
using System;

public partial class Application : Control
{
    public override void _Ready()
    {
        var filesManager = GetNode<FilesManager>("HSplitContainer/FilesManager");
        var markdownEditor = GetNode<MarkdownEditor>("HSplitContainer/MarkdownEditor");

        filesManager.FileSelected += markdownEditor.OpenFile;

        // 创建自定义主题
        var theme = new Theme();
        var defaultFont = ThemeDB.FallbackFont;
        var fontData = new SystemFont();
        fontData.FontNames = new string[] { defaultFont.ResourceName };
        theme.DefaultFont = fontData;

        // 设置默认字体大小（增加到24）
        theme.DefaultFontSize = 24;

        // 设置图标大小（增加到30）
        SetScaledIcon(theme, "Folder", "EditorIcons", 30);
        SetScaledIcon(theme, "File", "EditorIcons", 30);

        // 应用主题到根节点
        this.Theme = theme;

        // 调整 Tree 和 TabBar 的最小高度
        AdjustControlSizes();
    }

    private void SetScaledIcon(Theme theme, string iconName, string iconGroup, int size)
    {
        var originalIcon = theme.GetIcon(iconName, iconGroup);
        var image = originalIcon.GetImage();

        var scaledImage = Image.CreateEmpty(size, size, false, image.GetFormat());
        scaledImage.Resize(size, size, Image.Interpolation.Bilinear);
        scaledImage.BlitRect(image, new Rect2I(0, 0, image.GetWidth(), image.GetHeight()), Vector2I.Zero);

        var scaledTexture = ImageTexture.CreateFromImage(scaledImage);
        theme.SetIcon(iconName, iconGroup, scaledTexture);
    }

    private void AdjustControlSizes()
    {
        var filesManager = GetNode<FilesManager>("HSplitContainer/FilesManager");
        var markdownEditor = GetNode<MarkdownEditor>("HSplitContainer/MarkdownEditor");

        // 调整 Tree 的最小高度
        var tree = filesManager.GetNode<Tree>("%Tree");
        tree.CustomMinimumSize = new Vector2(0, 40);

        // 调整 TabBar 的最小高度
        var tabBar = markdownEditor.GetNode<TabBar>("VBoxContainer/TabBar");
        tabBar.CustomMinimumSize = new Vector2(0, 40);
    }
}

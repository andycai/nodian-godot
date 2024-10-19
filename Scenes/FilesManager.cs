using Godot;
using static Godot.GD;
using System.IO;
using System.Linq;

public partial class FilesManager : PanelContainer
{
	private Tree _tree;
	private TreeItem _root;
	private const string ROOT_DIRECTORY = "nodian";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Print("File manager ready");

		_tree = GetNode<VBoxContainer>("VBoxContainer").GetNode<Tree>("Tree");
		_tree.HideRoot = true;
		_tree.ItemSelected += TreeOnItemSelected;
		_root = _tree.CreateItem();

		string rootPath = EnsureRootDirectory();
		PopulateTree(rootPath, _root);
	}

	private string EnsureRootDirectory()
	{
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string rootPath = Path.Combine(documentsPath, ROOT_DIRECTORY);

		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}

		return rootPath;
	}

	private void PopulateTree(string path, TreeItem parent)
	{
		var directories = Directory.GetDirectories(path);
		var markdownFiles = Directory.GetFiles(path, "*.md");

		foreach (var directory in directories.OrderBy(d => d))
		{
			var item = _tree.CreateItem(parent);
			item.SetText(0, Path.GetFileName(directory));
			item.SetIcon(0, GetThemeIcon("Folder", "EditorIcons"));
			PopulateTree(directory, item);
		}

		foreach (var file in markdownFiles.OrderBy(f => f))
		{
			var item = _tree.CreateItem(parent);
			item.SetText(0, Path.GetFileName(file));
			item.SetIcon(0, GetThemeIcon("File", "EditorIcons"));
			item.SetMetadata(0, file);
		}
	}

	private void TreeOnItemSelected()
	{
		TreeItem selectedItem = _tree.GetSelected();
		if (selectedItem != null)
		{
			string itemPath = selectedItem.GetMetadata(0) as string;
			if (!string.IsNullOrEmpty(itemPath) && File.Exists(itemPath))
			{
				// TODO: Implement file opening logic
				Print($"Selected file: {itemPath}");
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// TODO: Implement any necessary frame-by-frame logic
	}
}

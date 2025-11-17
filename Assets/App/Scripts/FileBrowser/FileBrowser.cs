using System;
using System.IO;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class FileBrowser : Ui
{
    [SerializeField] private TMP_Text t_title;
    [SerializeField] private InputFieldSimple input;
    protected override void Reset()
    {
        deactivateOnClose = true;
        
        input = GetComponentInChildren<InputFieldSimple>();
        transform.TryFindObject(nameof(t_title), out t_title);
        
        base.Reset();
    }

    public static FileBrowser Instance { get; private set; }
    private ListContainer _listContainer;
    public override Task Register()
    {
        _listContainer = GetComponentInChildren<ListContainer>();
        Instance = this;
        return base.Register();
    }

    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(() =>
        {
            SelectedFile = null;
            Close();
        });
        
        gameObject.FindObject<ButtonImage>("b_ok").OnClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(_selectedPath))
            {
                MessageUi.Show("Select a file");
                return;
            }
            
            Close();
        });
        
        input.onValueChanged.AddListener((text) =>
        {
            _selectedPath = text;
        });
    }

    protected override void SetUi()
    {
        input.Text = "";
        base.SetUi();
    }

    private string _selectedPath;
    private FileItemPanel SelectedFile { get; set; }
    public void Select(FileItemPanel file)
    {
        SelectedFile = file;
        input.Text = file.FilePath;
        _selectedPath = file.FilePath;
        _listContainer.Select(file);
    }

    public override Task OpenAsync()
    {
        SelectedFile = null;
        _selectedPath = string.Empty;
        return base.OpenAsync();
    }

    public async Task<string> OpenSave(string path, string extension) => await Set(path, extension, false);
    public async Task<string> OpenLoad(string path, string extension) => await Set(path, extension, true);

    private bool _loadMode;
    private string _root;
    private string _extension;
    private async Task<string> Set(string path, string extension, bool loadMode)
    {
        _root = path;
        _extension = extension;
        _loadMode = loadMode;
        t_title.text = _loadMode ? "Load" : "Save";
        
        input.gameObject.SetActive(!loadMode);
        
        var files = Directory.GetFileSystemEntries(path, "*" + extension);
        for (var index = 0; index < files.Length; index++)
        {
            var file = files[index];
            file = Path.GetFileNameWithoutExtension(file);
            files[index] = file;
        }

        _listContainer.Set(files).Enable();
        
        await base.OpenAsync();
        return _selectedPath;
    }

    public override async void Close()
    {
        try
        {
            if (!_loadMode && File.Exists(Path.Combine(_root, $"{_selectedPath}.{_extension}")))
            {
                var response = await ConfirmationUi.ShowMessage("Warning!", "File Already Exists! Do you want to overwrite it?", "Ok",
                    "Cancel");
                destroyCancellationToken.ThrowIfCancellationRequested();
                if (response != ConfirmType.Confirm) return;
            }
        
            base.Close();
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Delete))
        {
            return;
        }
        
        if (string.IsNullOrEmpty(SelectedFile.FilePath))
        {
            File.Delete(_selectedPath);
            SelectedFile = null;
            _selectedPath = string.Empty;
        }
    }
}
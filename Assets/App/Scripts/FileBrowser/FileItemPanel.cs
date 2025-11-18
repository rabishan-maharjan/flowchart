using System.Collections;
using System.IO;
using Arcube.UiManagement;
using UnityEngine;

public class FileItemPanel : PanelItem
{
    private const float DoubleClickSpeed = 0.2f;
    private bool _isClickActive;
    private void Start()
    {
        OnClick.AddListener(() =>
        {
            FileBrowser.Instance.Select(this);
            if (_isClickActive)
            {
                FileBrowser.Instance.Close();
            }
            else StartCoroutine(WaitForDoubleClick());
        });
    }
    
    private IEnumerator WaitForDoubleClick()
    {
        _isClickActive = true;
        yield return new WaitForSeconds(DoubleClickSpeed);
        _isClickActive = false;
    }

    public string FilePath { get; private set; }
    public override void Set<T>(T item)
    {
        FilePath = item.ToString();
        Text = Path.GetFileNameWithoutExtension(FilePath);
        base.Set(item);
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;
using UnityEngine;

public class IOManager : ManagerBase
{
    private string _projectFolder;
    public override Task Register()
    {
        _projectFolder = Path.Combine(Application.persistentDataPath, "Projects");
        return base.Register();
    }

    public void Save(string fileName, Dictionary<string, Function> code)
    {
        var fullPath = Path.Combine(_projectFolder, fileName);
        var data = JsonConvert.SerializeObject(code, Formatting.Indented);
        File.WriteAllText(fullPath, data);
    }
    
    public Dictionary<string, Function> Load(string fileName)
    {
        var fullPath = Path.Combine(_projectFolder, fileName);
        var data = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<Dictionary<string, Function>>(data);
    }
}
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
        if(!Directory.Exists(_projectFolder)) Directory.CreateDirectory(_projectFolder);
        
        return base.Register();
    }

    public void Save(string fileName, Dictionary<string, Function> code)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        var fullPath = Path.Combine(_projectFolder, fileName);
        var data = JsonConvert.SerializeObject(code, Formatting.Indented, settings);
        File.WriteAllText(fullPath, data);
    }
    
    public Dictionary<string, Function> Load(string fileName)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        var fullPath = Path.Combine(_projectFolder, fileName);
        var data = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<Dictionary<string, Function>>(data, settings);
    }
}
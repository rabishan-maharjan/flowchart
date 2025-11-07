using Arcube.Animation;
using UnityEditor;
using UnityEngine;

namespace Common.EditorExtension
{
    public class TweenAnimationScriptGenerator : ScriptableWizard
    {
        [MenuItem("Tools/DOTween Animation Script Generator")]
        private static void CreateWizard()
        {
            if (!HasOpenInstances<TweenAnimationScriptGenerator>())
            {
                DisplayWizard("DOTween Animation Script Generator", typeof(TweenAnimationScriptGenerator), "Done");
            }
        }

        public TweenBuilder builder = new TweenBuilder();
        [TextArea(2, 50)] public string result;
        private void OnWizardCreate()
        {
        }

        private void OnWizardUpdate()
        {
            result = builder.GetJsonString();
        }
    }
}
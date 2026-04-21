using System.IO;

namespace greg.SaveEngine
{
    public static class GregVanillaDetector
    {
        public static bool CheckIfVanillaSave(string filePath)
        {
            if (!File.Exists(filePath)) return true;
            
            // Check if it's our own .db file
            if (filePath.EndsWith(".greg.db"))
            {
                return !GregSaveEngine.Instance.IsGregSave(filePath);
            }
            
            // If it's another file and it's being loaded by Vanilla, it's vanilla
            return true;
        }
    }
}

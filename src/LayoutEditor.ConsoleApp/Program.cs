using LayoutEditor.Common;

namespace LayoutEditor.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        Test();
    }
    private static void Test()
    {
        var sortedIni = IniSorter.SortIniFile(@"C:\Games\ProjectQuarm\UI_Ufirst_pq.proj.ini");
        var profile = CharacterUiProfile.LoadFromFile(@"C:\Games\ProjectQuarm\UI_Ufirst_pq.proj.ini");
        var profileIni = profile.ToIniString();

        if (sortedIni != profileIni)
            throw new InvalidOperationException("sortedIni does not match profileIni.");
        else
            Console.WriteLine($"Ini Files Match!");
    }
}

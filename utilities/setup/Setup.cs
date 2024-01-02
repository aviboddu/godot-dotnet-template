#if TOOLS
using System.Diagnostics;
using System.IO;
using Godot;

namespace Utilities;

[Tool]
public partial class Setup : EditorScript
{
	private const string CSPROJ = "./{0}.csproj";
	private const string SLN = "./{0}.sln";
	private const string DOT_SETTINGS = "./{0}.sln.DotSettings";
	private const string DOT_SETTINGS_USER = "./{0}.sln.DotSettings.user";
	private const string IDEA_FOLDER = "./.idea/.idea.{0}/.idea/";
	private const string NAME_FILE = "./.idea/.idea.{0}/.idea/.name";

	private string OldProjectName;
	private const string NEW_PROJECT_NAME = "NetTemplate";

	// Called when the script is executed (using File -> Run in Script Editor).
	public override void _Run()
	{
		OldProjectName = (string)ProjectSettings.GetSetting("application/config/name");
		GD.Print(OldProjectName);
		GD.Print(NEW_PROJECT_NAME);
		Debug.Assert(!NEW_PROJECT_NAME.Contains(' '), "Setup - New Project Name cannot contain a space");
		Debug.Assert(NEW_PROJECT_NAME != OldProjectName, "Setup - New Project Name cannot be the same as the old project name");

		Move(CSPROJ);
		Move(SLN);
		ReplaceFileContents(SLN);
		Move(DOT_SETTINGS);
		Move(DOT_SETTINGS_USER);
		
		ProjectSettings.SetSetting("application/config/name", NEW_PROJECT_NAME);
		ProjectSettings.SetSetting("dotnet/project/assembly_name", NEW_PROJECT_NAME);
		ProjectSettings.Save();
	}

	private void Move(string fileFormat)
	{
		File.Move(string.Format(fileFormat, OldProjectName), string.Format(fileFormat, NEW_PROJECT_NAME));
	}

	private void ReplaceFileContents(string fileFormat)
	{
		string fileName = string.Format(fileFormat, NEW_PROJECT_NAME);
		string contents = File.ReadAllText(fileName);
		contents = contents.Replace(OldProjectName, NEW_PROJECT_NAME);
		File.WriteAllText(fileName, contents);
	}
}
#endif

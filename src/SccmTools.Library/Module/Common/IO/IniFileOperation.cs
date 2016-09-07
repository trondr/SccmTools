using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IniParser;
using SccmTools.Library.Infrastructure.LifeStyles;
using SccmTools.Library.Module.Services;

namespace SccmTools.Library.Module.Common.IO
{
    
    /// <summary>
    /// Ini file operation
    /// </summary>
    [Singleton]
    public class IniFileOperation : IIniFileOperation
    {        
        public string Read(string path, string section, string key)
        {
            var iniFileParser = new FileIniDataParser();
            var iniData = iniFileParser.ReadFile(path);
            if (!iniData.Sections.ContainsSection(section))
            {
                throw new SccmToolsException("Ini file section not found: " + section);
            }            
            var value = iniData[section][key];
            return value;
        }

        public void Write(string path, string section, string key, string value)
        {
            var iniFileParser = new FileIniDataParser();
            var iniData = iniFileParser.ReadFile(path);
            if (!iniData.Sections.ContainsSection(section))
            {
                iniData.Sections.AddSection(section);
            }
            iniData[section][key] = value;
            iniFileParser.WriteFile(path, iniData, Encoding.ASCII);
        }

        public string[] ReadKeys(string path, string section, string regexKeyNamePattern)
        {
            var iniFileParser = new FileIniDataParser();
            var iniData = iniFileParser.ReadFile(path);
            var keyPatternRegex = new Regex(regexKeyNamePattern);
            var values = from key in iniData[section]
                where keyPatternRegex.IsMatch(key.KeyName)
                select iniData[section][key.KeyName];
            var valuesArray = values.ToArray();
            return valuesArray;
        }
    }
}

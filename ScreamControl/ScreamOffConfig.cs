using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScreamControl
{
    internal class ScreamOffConfig
    {
        static private readonly List<string> _assetFonts = new List<string>();

        static ScreamOffConfig()
        {
            var assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            if (Directory.Exists(assetPath))
            {
                assetPath += Path.DirectorySeparatorChar;
                Uri fontUri = new Uri(assetPath, UriKind.Absolute);
                foreach (System.Windows.Media.FontFamily fontFamily in Fonts.GetFontFamilies(fontUri))
                {
                    var fontName = fontFamily.Source;
                    if (fontName.StartsWith("./#"))
                    {
                        fontName = fontName.Substring(3);
                    }
                    _assetFonts.Add(fontName);
                }
            }
        }

        public enum FontLocation { NotExsit, Assets, System };
        public struct TextOption
        {
            public string Text;
            public System.Windows.Media.Color TextColor;
            public string Font;
            public double FontAdjust;
            public FontLocation Location;
        }

        private const string DefaultFont = "Arial";
        private const double DefaultFontAdjust = 0.0;
        private readonly System.Windows.Media.Color DefaultTextColor = System.Windows.Media.Colors.White;

        public struct ImageOption
        {
            public string File;
            public double Left;
            public double Top;
        }

        public TextOption Title { get; private set; }
        public TextOption Option1 { get; private set; }
        public TextOption Option2 { get; private set; }
        public TextOption Option3 { get; private set; }

        public List<ImageOption> Images { get; private set; } = new List<ImageOption>();

        public void LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found: " + Path.GetFullPath(filename));

            using (StreamReader sr = new StreamReader(filename))
            {
                int lineNum = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    ++lineNum;
                    if (line == null)
                        continue;
                    line = line.Trim();
                    if (line.StartsWith("#") || line.Length == 0)
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length != 2)
                    {
                        throw new Exception($"Improperly formatted config in {filename} on line {lineNum}");
                    }
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (key.Length == 0 || value.Length == 0)
                    {
                        throw new Exception($"Empty configuration value in {filename} on line {lineNum}");
                    }

                    var pattern = @"^[^\(]+\([^,]*,[^\)]*\)$";
                    if (!Regex.IsMatch(key, pattern))
                    {
                        throw new Exception($"Improperly formatted configuration value '{key}' in {filename} on line {lineNum}");
                    }

                    int paramStart = key.IndexOf("(") + 1;
                    int paramEnd = key.IndexOf(")", paramStart);
                    var parameters = key.Substring(paramStart, paramEnd - paramStart).Split(',');
                    for (var i = 0; i < parameters.Length; ++i)
                    {
                        parameters[i] = parameters[i].Trim();
                    }

                    if (key.StartsWith("Title(") || key.StartsWith("Option1(") || key.StartsWith("Option2(") || key.StartsWith("Option3("))
                    {
                        if (parameters.Length != 3)
                        {
                            throw new Exception($"Invalid parameters in {filename} on line {lineNum}");
                        }
                        if (parameters[0].Length == 0)
                        {
                            parameters[0] = DefaultFont;
                        }
                        var fontLocation = DoesFontExist(parameters[0]);
                        if (fontLocation == FontLocation.NotExsit)
                        {
                            throw new Exception($"Font '{parameters[0]}' does not exist in Assets folder or system fonts, from {filename} on line {lineNum}");
                        }

                        var fontAdjust = DefaultFontAdjust;
                        if (parameters[1].Length > 0)
                        {
                            if (!Regex.IsMatch(parameters[1], @"^[+-][0-9]+$"))
                            {
                                throw new Exception($"Invalid font adjust value '{parameters[1]}' in {filename} on line {lineNum}");
                            }

                            if (!double.TryParse(parameters[1], out fontAdjust))
                            {
                                throw new Exception($"Invalid font adjust value '{parameters[1]}' in {filename} on line {lineNum}");
                            }
                        }

                        var textColor = DefaultTextColor;
                        if (parameters[2].Length > 0)
                        {
                            if (!Regex.IsMatch(parameters[2], @"^[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]+$"))
                            {
                                throw new Exception($"Invalid color hex value '{parameters[2]}' in {filename} on line {lineNum}");
                            }

                            try
                            {
                                textColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#" + parameters[2]);
                            }
                            catch
                            {
                                throw new Exception($"Invalid color hex value '{parameters[2]}' in {filename} on line {lineNum}");
                            }
                        }

                        var textOption = new TextOption
                        {
                            Text = value,
                            TextColor = textColor,
                            Font = parameters[0],
                            FontAdjust = fontAdjust,
                            Location = fontLocation
                        };
                        if (key.StartsWith("Title("))
                        {
                            Title = textOption;
                        }
                        else if (key.StartsWith("Option1("))
                        {
                            Option1 = textOption;
                        }
                        else if (key.StartsWith("Option2("))
                        {
                            Option2 = textOption;
                        }
                        else if (key.StartsWith("Option3("))
                        {
                            Option3 = textOption;
                        }
                    }
                    else if (key.StartsWith("Image"))
                    {
                        if (parameters.Length != 2)
                        {
                            throw new Exception($"Invalid parameters in {filename} on line {lineNum}");
                        }
                        if (!double.TryParse(parameters[0], out var left))
                        {
                            throw new Exception($"Invalid image left value '{parameters[0]}' in {filename} on line {lineNum}");
                        }
                        if (!double.TryParse(parameters[1], out var top))
                        {
                            throw new Exception($"Invalid image top value '{parameters[1]}' in {filename} on line {lineNum}");
                        }

                        var filePath = Path.GetFullPath("Assets" + Path.DirectorySeparatorChar + value);
                        if (!File.Exists(filePath))
                        {
                            throw new Exception($"Image file '{value}' does not exist in Assets folder, from {filename} on line {lineNum}");
                        }

                        Images.Add(new ImageOption
                        {
                            File = filePath,
                            Left = left,
                            Top = top
                        });
                    }
                    else
                    {
                        throw new Exception($"Invalid configuration value '{key}' in {filename} on line {lineNum}");
                    }
                }

                if (Title.Text == null)
                {
                    throw new Exception($"Missing Title configuration in {filename}");
                }
                if (Option1.Text == null)
                {
                    throw new Exception($"Missing Option1 configuration in {filename}");
                }
                if (Option2.Text == null)
                {
                    throw new Exception($"Missing Option2 configuration in {filename}");
                }
                if (Option3.Text == null)
                {
                    throw new Exception($"Missing Option3 configuration in {filename}");
                }
            }
        }

        private FontLocation DoesFontExist(string fontName)
        {
            if (_assetFonts.Contains(fontName))
            {
                return FontLocation.Assets;
            }

            // Search the WPF SystemFontFamilies collection for a match
            if (Fonts.SystemFontFamilies.Any(f => f.Source.Equals(fontName, StringComparison.OrdinalIgnoreCase)))
            {
                return FontLocation.System;
            }

            return FontLocation.NotExsit;
        }
    }
}

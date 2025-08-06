using Newtonsoft.Json;
using PresetEditor.Models;
using System.Text.RegularExpressions;


namespace PresetEditor.Services
{
    internal class PresetSettingSegment : IPresetSettingSegment
    {
        private string? ExtractInputsBlock(string text, string keyword)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            
            // 1. 找到父节点起始位置
            var keywordStart = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (keywordStart == -1) return null;

            // 2. 找到父节点第一个 '{'
            var braceStart = text.IndexOf('{', keywordStart);
            if (braceStart == -1) return null;

            // 3. 用计数法找到父节点内容范围
            var braceCount = 1;
            var i = braceStart + 1;
            while (i < text.Length && braceCount > 0)
            {
                switch (text[i])
                {
                    case '{':
                        braceCount++;
                        break;
                    case '}':
                        braceCount--;
                        break;
                }

                i++;
            }
            if (braceCount != 0) return null; // 括号不匹配

            var keywordNodeContent = text.Substring(braceStart + 1, i - braceStart - 2);
            return keywordNodeContent;
        }

        public IEnumerable<GroupInput>? GetGroupInputs(string text, string keyword, string subKeyword = "UserControls = ordered()")
        {
            var keywordContent = ExtractInputsBlock(text, keyword);
            if (string.IsNullOrWhiteSpace(keywordContent)) return null;
            var subKeywordContent = ExtractInputsBlock(keywordContent, subKeyword);
            if (string.IsNullOrWhiteSpace(subKeywordContent)) return null;
            
            var controlPattern = @"(\w+)\s*=\s*{(.*?)}";
            var controls = Regex.Matches(subKeywordContent, controlPattern, RegexOptions.Singleline);

            var groupInputs = new List<GroupInput>();
            foreach (Match control in controls)
            {
                var groupInput = new GroupInput
                {
                    GroupName = control.Groups[1].Value.Trim()
                };

                var controlBody = control.Groups[2].Value;
                var propMatches = Regex.Matches(controlBody, @"(\w+)\s*=\s*(""[^""]*""|[\d\.]+)", RegexOptions.Multiline);
                
                foreach (Match pm in propMatches)
                {
                    groupInput.PropertyList.Add(new InputItem
                    {
                        Key = pm.Groups[1].Value.Trim(),
                        Value = pm.Groups[2].Value.Trim().Trim('"')
                    });
                }
                groupInputs.Add(groupInput);
            }
            return groupInputs;
        }


        public IEnumerable<InstanceInputRaw>? GetOrderedInstanceInputs(string text)
        {
            var inputsBlock = ExtractInputsBlock(text, "Inputs = ordered()");
            if (string.IsNullOrWhiteSpace(inputsBlock)) return null;
            
            var inputMatches = Regex.Matches(inputsBlock, @"(\w+)\s*=\s*InstanceInput\s*\{([\s\S]*?)\},", RegexOptions.Multiline);

            var instanceInputRaws = new List<InstanceInputRaw>();
            foreach (Match im in inputMatches)
            {
                var instanceInputRaw = new InstanceInputRaw
                {
                    InputName = im.Groups[1].Value.Trim()
                };

                var inputBody = im.Groups[2].Value;
                var propMatches = Regex.Matches(inputBody, @"(\w+)\s*=\s*(""[^""]*""|[\d\.]+)", RegexOptions.Multiline);
                foreach (Match pm in propMatches)
                {
                    instanceInputRaw.PropertyList.Add(new InputItem
                    {
                        Key = pm.Groups[1].Value.Trim(),
                        Value = pm.Groups[2].Value.Trim().Trim('"')
                    });
                }
                instanceInputRaws.Add(instanceInputRaw);
            }

            return instanceInputRaws;
        }

        public string OrderedInputs2Text(List<InstanceInput> inputs)
        {
            return null;
            //var sb = new System.Text.StringBuilder();
            //sb.AppendLine("Inputs = ordered() {");
            //foreach (var input in inputs)
            //{
            //    sb.AppendLine($"\t{input.Name ?? "Input"} = InstanceInput {{");
            //    if (!string.IsNullOrEmpty(input.SourceOp))
            //        sb.AppendLine($"\t\tSourceOp = \"{input.SourceOp}\",");
            //    if (!string.IsNullOrEmpty(input.Source))
            //        sb.AppendLine($"\t\tSource = \"{input.Source}\",");
            //    if (!string.IsNullOrEmpty(input.Name))
            //        sb.AppendLine($"\t\tName = \"{input.Name}\",");
            //    if (input.Default.HasValue)
            //        sb.AppendLine($"\t\tDefault = {input.Default.Value},");
            //    if (input.MaxScale.HasValue)
            //        sb.AppendLine($"\t\tMaxScale = {input.MaxScale.Value},");
            //    if (input.Width.HasValue)
            //        sb.AppendLine($"\t\tWidth = {input.Width.Value},");
            //    if (input.ControlGroup.HasValue)
            //        sb.AppendLine($"\t\tControlGroup = {input.ControlGroup.Value},");
            //    sb.AppendLine("\t},");
            //}
            //sb.AppendLine("}");
            //return sb.ToString();
        }
    }
}

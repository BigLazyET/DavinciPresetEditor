using System.Text;
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
                    GroupSourceName = control.Groups[1].Value.Trim()
                };

                var controlBody = control.Groups[2].Value;
                var propMatches = Regex.Matches(controlBody, @"(\w+)\s*=\s*(""[^""]*""|[\d\.]+|\w+)", RegexOptions.Multiline);
                
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


        public IEnumerable<InstanceInput>? GetOrderedInstanceInputs(string text)
        {
            var inputsBlock = ExtractInputsBlock(text, "Inputs = ordered()");
            if (string.IsNullOrWhiteSpace(inputsBlock)) return null;
            
            var inputMatches = Regex.Matches(inputsBlock, @"(\w+)\s*=\s*InstanceInput\s*\{([\s\S]*?)\},?", RegexOptions.Multiline);

            var instanceInputs = new List<InstanceInput>();
            foreach (Match im in inputMatches)
            {
                var instanceInput = new InstanceInput
                {
                    InputName = im.Groups[1].Value.Trim()
                };

                var inputBody = im.Groups[2].Value;
                var propMatches = Regex.Matches(inputBody, @"(\w+)\s*=\s*(""[^""]*""|[\d\.]+|\w+)", RegexOptions.Multiline);
                foreach (Match pm in propMatches)
                {
                    instanceInput.PropertyList.Add(new InputItem
                    {
                        Key = pm.Groups[1].Value.Trim(),
                        Value = pm.Groups[2].Value.Trim().Trim('"')
                    });
                }
                instanceInputs.Add(instanceInput);
            }

            return instanceInputs;
        }

        public string OrderedInputs2Text(IList<InstanceInput> inputs)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                sb.AppendLine($"{input.InputName} = InstanceInput {{");

                // 先拿到属性列表和总数
                var props = input.PropertyList;
                var propCount = props.Count;
                for (var j = 0; j < propCount; j++)
                {
                    var prop = props[j];

                    // 格式化值
                    if (double.TryParse(prop.Value, out var dv))
                        sb.Append($"\t{prop.Key} = {dv},");
                    else if (bool.TryParse(prop.Value, out var bv))
                    {
                        if (bv)
                            sb.Append($"\t{prop.Key} = true,");
                        else
                            sb.Append($"\t{prop.Key} = false,");
                    }
                    else
                        sb.Append($"\t{prop.Key} = \"{prop.Value}\",");

                    sb.AppendLine();    // 默认都加上逗号
                    // // 只有不是最后一个属性才加逗号
                    // if (j < propCount - 1)
                    //     sb.AppendLine(",");
                    // else
                    //     sb.AppendLine();        // 最后一项直接换行，不加逗号
                }

                // 结束 InstanceInput 块，只有不是最后一个 input 才加逗号
                if (i < inputs.Count - 1)
                    sb.AppendLine("},");
                else
                    sb.AppendLine("}");
            }

            return sb.ToString();
        }
        
        public string OrderedGroups2Text(IList<GroupInput> inputs)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UserControls = ordered() {{");

            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                sb.AppendLine($"\t{input.GroupSourceName} = {{");

                // 先拿到属性列表和总数
                var props = input.PropertyList;
                var propCount = props.Count;
                for (var j = 0; j < propCount; j++)
                {
                    var prop = props[j];

                    // 格式化值
                    if (double.TryParse(prop.Value, out var dv))
                        sb.Append($"\t\t{prop.Key} = {dv},");
                    else if (bool.TryParse(prop.Value, out var bv))
                    {
                        if (bv)
                            sb.Append($"\t\t{prop.Key} = true,");
                        else
                            sb.Append($"\t\t{prop.Key} = false,");
                    }
                    else
                        sb.Append($"\t\t{prop.Key} = \"{prop.Value}\",");

                    sb.AppendLine();    // 默认都加上逗号
                    // // 只有不是最后一个属性才加逗号
                    // if (j < propCount - 1)
                    //     sb.AppendLine(",");
                    // else
                    //     sb.AppendLine();        // 最后一项直接换行，不加逗号
                }

                // 结束 InstanceInput 块，只有不是最后一个 input 才加逗号
                if (i < inputs.Count - 1)
                    sb.AppendLine("\t},");
                else
                    sb.AppendLine("\t}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

using Newtonsoft.Json;
using PresetEditor.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;


namespace PresetEditor.Services
{
    internal class PresetSettingSegment : IPresetSettingSegment
    {
        private string ExtractInputsBlock(string text)
        {
            var keyword = "Inputs = ordered() {";
            int start = text.IndexOf(keyword);
            if (start < 0) return null;

            start += keyword.Length;
            int braceCount = 1;
            int pos = start;
            while (pos < text.Length && braceCount > 0)
            {
                if (text[pos] == '{') braceCount++;
                else if (text[pos] == '}') braceCount--;
                pos++;
            }
            // pos指向第一个结束的'}'后
            return text.Substring(start, pos - start - 1).Trim();
        }

        // Segment: Inputs = ordered(){...}
        public string OrderedInputs2Json(string text)
        {
            // 1. 提取 Inputs = ordered() { ... } 内容
            string inputsBlock = ExtractInputsBlock(text);

            // 2. 提取每个 Input
            var inputMatches = Regex.Matches(inputsBlock, @"(\w+)\s*=\s*InstanceInput\s*\{([\s\S]*?)\},", RegexOptions.Multiline);

            var instanceInputs = new List<InstanceInput>();
            foreach (Match im in inputMatches)
            {
                string inputName = im.Groups[1].Value.Trim();
                string inputBody = im.Groups[2].Value;

                var instanceInput = new InstanceInput();
                // 3. 将属性行转换为 JSON
                instanceInput.InputName = inputName;
                var propMatches = Regex.Matches(inputBody, @"(\w+)\s*=\s*(""[^""]*""|[\d\.]+)", RegexOptions.Multiline);
                foreach (Match pm in propMatches)
                {
                    instanceInput.PropertyList.Add(new InstanceInputItem
                    {
                        Key = pm.Groups[1].Value.Trim(),
                        Value = pm.Groups[2].Value.Trim().Trim('"')
                    });
                }
                instanceInputs.Add(instanceInput);
            }

            // 4. 转为 JSON 并输出
            string json = JsonConvert.SerializeObject(instanceInputs, Formatting.Indented);
            return json;
        }

        public IList<InstanceInput>? OrderedInputs2List(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];
            return JsonConvert.DeserializeObject<IList<InstanceInput>>(json);
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

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PresetEditor.Services;

public static class DSLFormatter
{
    public static string Format(string input, int spacesPerIndent = 4)
    {
        var lines = input.Replace("\r\n", "\n").Split('\n');
        var sb = new StringBuilder();

        int indentLevel = 0;
        var stack = new Stack<string>();

        // 匹配行首是 "key = Something {" 的情况
        var blockStartRegex = new Regex(@"^[\[\]\w""\._]+?\s*=\s*[\w\[\]""\._]+\s*\{$");
        // 匹配单一 { 行
        var singleOpenBraceRegex = new Regex(@"\{\s*$");
        // 匹配单一 } 行
        var singleCloseBraceRegex = new Regex(@"^\s*\}");
        // 匹配块结束且后面可能还有同级项 "},"
        var blockEndWithCommaRegex = new Regex(@"\},\s*$");

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
            {
                sb.AppendLine();
                continue;
            }

            // 1. 如果这一行是 "}" 开始的，先减少缩进
            if (singleCloseBraceRegex.IsMatch(line) || line.StartsWith("},") || line.StartsWith("]"))
            {
                indentLevel = Math.Max(indentLevel - 1, 0);
                if (stack.Count > 0) stack.Pop();
            }

            // 2. 输出该行（按照当前缩进级别）
            sb.AppendLine(new string(' ', indentLevel * spacesPerIndent) + line);

            // 3. 分析该行，调整下一行缩进
            if (blockStartRegex.IsMatch(line))
            {
                // 形如 "Input3 = InstanceInput {" 新增层级
                stack.Push(line);
                indentLevel++;
            }
            else if (singleOpenBraceRegex.IsMatch(line) && !blockEndWithCommaRegex.IsMatch(line))
            {
                // 单个 { 也需要缩进（非 "}," 形式）
                indentLevel++;
            }
            else if (blockEndWithCommaRegex.IsMatch(line))
            {
                // "}," 收尾，但可能还有下一个键，恢复到父级缩进
                indentLevel = Math.Max(indentLevel - 1, 0);
            }
        }

        return sb.ToString();
    }
}

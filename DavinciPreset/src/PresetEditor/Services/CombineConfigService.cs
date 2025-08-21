using System.Text;

namespace PresetEditor.Services;

public class CombineConfigService : ICombineConfigService
{
    /// <summary>
    /// 在文本中查找指定关键字后的第一个 { } block，并替换它的内容
    /// </summary>
    /// <param name="original">原始大字符串</param>
    /// <param name="keyword">要查找的关键字，例如 "Inputs = ordered()"</param>
    /// <param name="newContent">要替换的新内容（不含外层括号）</param>
    /// <returns>替换后的完整字符串。如果找不到则返回原字符串。</returns>
    public string? ReplaceBlockByKeyword(string original, string keyword, string newContent)
    {
        var index = StartAndEndIndex(original, keyword);
        if (index == null) return null;
        var contentStartIndex = index.Value.Start;
        var contentEndIndex = index.Value.End;

        // 4. 用 Span 构造新字符串（避免中间Substring分配）
        var source = original.AsSpan();
        var finalLength = contentStartIndex + newContent.Length + (original.Length - contentEndIndex);

        var sb = new StringBuilder(finalLength);
        sb.Append(source.Slice(0, contentStartIndex)); // '{' 之前含 '{'
        sb.AppendLine();
        var indentedContent = GenerateIndentedContent(original, keyword, newContent);
        sb.Append(indentedContent);
        sb.Append(source.Slice(contentEndIndex, original.Length - contentEndIndex)); // 从 '}' 开始到末尾

        return sb.ToString();
    }
    
    private string GenerateIndentedContent(string original, string keyword, string newContent)
    {
        var spaceCounts = GetSpaceCounts(original, keyword);

        var indent = new string(' ', spaceCounts + 4);

        var newLines = newContent.Replace("\r\n", "\n")
            .Split('\n')
            .Select(line => indent + line); // 保留原有缩进

        return string.Join(Environment.NewLine, newLines);
    }

    private int GetSpaceCounts(string text, string keyword)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n');

        return (from line in lines
                where line.TrimStart().StartsWith(keyword)
                let leading = line.Substring(0, line.Length - line.TrimStart().Length)
                select leading.Replace("\t", "    ").Length // tab => 4 spaces
            ).FirstOrDefault();
    }

    private (int Start, int End)? StartAndEndIndex(string original, string keyword)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(keyword))
            return null;

        // 1. 找到关键字位置
        var keywordIndex = original.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        if (keywordIndex == -1)
            return null;

        // 2. 找到 '{'
        var braceStart = original.IndexOf('{', keywordIndex);
        if (braceStart == -1)
            return null;

        // 3. 计数法匹配对应 '}'
        var braceCount = 1;
        var i = braceStart + 1;
        while (i < original.Length && braceCount > 0)
        {
            var ch = original[i];
            switch (ch)
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

        if (braceCount != 0) // 括号不匹配
            return null;

        var contentStartIndex = braceStart + 1; // 内容起点（不包含 '{'）
        var contentEndIndex = i - 1;            // 内容终点（不包含 '}'）

        return (contentStartIndex, contentEndIndex);
    }

    public string? ReplaceGroupBlockByKeyword(string original, string keyword, string newContent, string subKeyword = "UserControls = ordered()")
    {
        var index = StartAndEndIndex(original, keyword);
        if (index == null) return null;
        var nodeContentStart = index.Value.Start;
        var nodeContentEnd = index.Value.End;

        // 4. 检查该范围内是否存在 UserControls
        var nodeContentSpan = original.AsSpan(nodeContentStart, nodeContentEnd - nodeContentStart);
        if (nodeContentSpan.IndexOf(subKeyword, StringComparison.OrdinalIgnoreCase) < 0)
        {
            var source = original.AsSpan();
            var finalLength = nodeContentStart + newContent.Length + (original.Length - nodeContentStart) + 1;

            var sb = new StringBuilder(finalLength);
            sb.Append(source.Slice(0, nodeContentStart).TrimEnd()); // '{' 之前含 '{'
            sb.AppendLine();
            var indentedContent = GenerateIndentedContent(original, keyword, newContent);
            sb.Append(indentedContent.TrimEnd() + ',');
            sb.Append(source.Slice(nodeContentStart, original.Length - nodeContentStart)); // 从 '}' 开始到末尾
            return sb.ToString();
        }
        else
        {
            // keywordNodeContent =》 node = nodetype { .... } 里的内容....
            var keywordNodeContent = original.Substring(nodeContentStart, nodeContentEnd - nodeContentStart);
            var subIndex = StartAndEndIndex(keywordNodeContent, subKeyword);
            if (subIndex == null) return null;
            var subContentStartIndex =
                nodeContentStart + nodeContentSpan.IndexOf(subKeyword, StringComparison.OrdinalIgnoreCase);
            var braceStart = original.IndexOf('{', subContentStartIndex);
            var subContentEndIndex = braceStart + 1 + subIndex.Value.End - subIndex.Value.Start;
            
            var finalLength = subContentStartIndex + newContent.Length + (original.Length - subContentEndIndex) + 1;
            var sb = new StringBuilder(finalLength);
            var source = original.AsSpan();
            sb.Append(source.Slice(0, subContentStartIndex - 1).TrimEnd());
            sb.AppendLine();
            var indentedContent = GenerateIndentedContent(original, keyword, newContent);
            sb.AppendLine(indentedContent.TrimEnd() + ',');
            sb.Append(source.Slice(subContentEndIndex + 1, original.Length - subContentEndIndex - 1));
            return sb.ToString();
        }
    }
}
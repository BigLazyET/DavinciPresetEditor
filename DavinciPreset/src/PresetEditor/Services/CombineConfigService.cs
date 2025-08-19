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
        sb.Append(newContent);
        sb.Append(source.Slice(contentEndIndex, original.Length - contentEndIndex)); // 从 '}' 开始到末尾

        return sb.ToString();
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
            if (ch == '{') braceCount++;
            else if (ch == '}') braceCount--;
            i++;
        }

        if (braceCount != 0) // 括号不匹配
            return null;

        var contentStartIndex = braceStart + 1; // 内容起点（不包含 '{'）
        var contentEndIndex = i - 1;            // 内容终点（不包含 '}'）

        return (contentStartIndex, contentEndIndex);
    }
    
    public string? EnsureUserControls(string original, string keyword)
    {
        var index = StartAndEndIndex(original, keyword);
        if (index == null) return null;
        var nodeContentStart = index.Value.Start;
        var nodeContentEnd = index.Value.End;

        // 4. 检查该范围内是否存在 UserControls
        var nodeContentSpan = original.AsSpan(nodeContentStart, nodeContentEnd - nodeContentStart);
        if (nodeContentSpan.IndexOf("UserControls", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // 已存在，直接返回
            return original;
        }

        // 5. 准备插入的默认内容，注意缩进
        // 计算与最后一行相同的缩进
        var indentCount = GetIndentBeforeBrace(original, nodeContentStart - 1);
        var indent = new string(' ', indentCount + 1);

        var insertContent =
            $"{indent}UserControls = ordered() {{}}\n";

        // 6. 用 Span 拼接新字符串
        var source = original.AsSpan();
        var finalLen = original.Length + insertContent.Length;

        var sb = new StringBuilder(finalLen);
        sb.Append(source.Slice(0, nodeContentEnd)); // 到最后一个 '}' 之前
        sb.Append('\n');
        sb.Append(insertContent);
        sb.Append(source.Slice(nodeContentEnd, original.Length - nodeContentEnd)); // 剩余部分含 '}'

        return sb.ToString();
    }

    /// <summary>
    /// 获取 { 之前的空格数（用于保持缩进风格）
    /// </summary>
    private int GetIndentBeforeBrace(string text, int braceIndex)
    {
        int indentCount = 0;
        for (int i = braceIndex - 1; i >= 0; i--)
        {
            if (text[i] == '\n')
                break;
            if (text[i] == ' ')
                indentCount++;
            else
                indentCount = 0; // 有非空格字符则缩进只算到它之后
        }
        return indentCount;
    }
}
[![Build MAUI App](https://github.com/BigLazyET/DavinciPresetEditor/actions/workflows/build.yml/badge.svg)](https://github.com/BigLazyET/DavinciPresetEditor/actions/workflows/build.yml)

<p align="left"> <a href="https://ko-fi.com/biglazyet"> <img src="https://ko-fi.com/img/githubbutton_sm.svg" alt="Support me on Ko‑fi"> </a> </p>

# DavinciPresetEditor
**实现可视化界面配置达芬奇模板的分组与分页**

本项目基于 Microsoft .NET MAUI，旨在提供同时支持 Windows 与 macOS 的跨平台桌面应用。MAUI 理论上也可面向移动端，但受界面布局与交互限制，移动平台暂不提供支持。

你可以用它完成的工作：

- 对 DaVinci Resolve 模板的公开参数（Published Parameters）进行重新排序
- 为模板添加分页
- 新增“分组源”和分组配置（用于定义分组、标题、说明等）
- 将分组配置应用到公开参数，并可调整分组顺序
- 导出调整后的公开参数与分组源配置

### Release

你可以在 [Releases](https://github.com/BigLazyET/DavinciPresetEditor/releases) 页面下载已编译的安装包，目前仅提供 Windows 版本的分发包。

macOS：由于尚无开发者账号，暂不提供签名的 macOS 分发包。你可以：

- 直接克隆本仓库，自行编译并运行；或
- 作为贡献者协助打包并发布 macOS 版本。
- 下载入口：[Releases](https://github.com/BigLazyET/DavinciPresetEditor/releases)

### 一、项目初衷

在剪辑学习中，模板制作几乎是必经之路。但在 DaVinci Resolve 的模板制作流程中，一些使用层面的不足常让创作者困惑与沮丧。

主要痛点：官方尚未提供用于配置“公开参数（Published Parameters）”分组与分页的可视化界面。勾选公开参数并导出模板后，使用时这些参数会在 Inspector 面板中杂乱堆叠，影响体验与效率。

当前常见的权宜做法（可能也有其他方案）：

用文本编辑器直接打开模板文件；
通过手动编辑实现公开参数的重新排序、分页与分组。
这种方式虽可行，但并不理想：手工文本编辑不确定性高、易出错、反复修改，既耗时又耗力。

### 二、如何使用

接下来我在Mac上演示如何使用
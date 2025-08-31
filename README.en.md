[![Build MAUI App](https://github.com/BigLazyET/DavinciPresetEditor/actions/workflows/build.yml/badge.svg)](https://github.com/BigLazyET/DavinciPresetEditor/actions/workflows/build.yml)

<p align="left"> <a href="https://ko-fi.com/biglazyet"> <img src="https://ko-fi.com/img/githubbutton_sm.svg" alt="Support me on Ko‑fi"> </a> </p>

# DavinciPresetEditor

<p align="left"> <a href="README.md"><img src="https://img.shields.io/badge/%E7%AE%80%E4%BD%93%E4%B8%AD%E6%96%87-说明-fd6a02?style=for-the-badge" alt="简体中文"></a> </p>

**Implement a visual interface for configuring groups and pagination of DaVinci Resolve templates.**

This project is built with Microsoft .NET MAUI to deliver a cross-platform desktop app for Windows and macOS. Although MAUI can theoretically target mobile platforms, mobile support is currently not provided due to layout and interaction constraints.

What you can do with it:

- Reorder a DaVinci Resolve template’s Published Parameters
- Add pagination to the template
- Create group sources and group configurations (define groups, titles, descriptions, etc.)
- Apply a group configuration to the Published Parameters and adjust the group order
- Export the adjusted Published Parameters and the group-source configuration

### Release

You can download prebuilt installers from the [Releases](https://github.com/BigLazyET/DavinciPresetEditor/releases) page. At the moment, only Windows builds are available.

macOS: as we don’t have a developer account yet, a signed macOS distribution is not provided. You can:

- clone the repository and build/run locally; or
- contribute a packaged macOS build.
- Download: [Releases](https://github.com/BigLazyET/DavinciPresetEditor/releases)

### 一、Why this project?

Template creation is almost inevitable when learning editing, but the current DaVinci Resolve template workflow has usability gaps that often frustrate creators.

Key pain point: there is no visual interface to configure grouping and pagination for Published Parameters. After marking parameters as published and exporting the template, they appear as a cluttered list in the Inspector, which hurts usability and efficiency.

Common workaround (there may be other options i don't know, but as i know is):

Open the template file in a text editor;
Manually edit it to reorder, paginate, and group the published parameters.
While this works, it is not ideal: manual text editing is uncertain and error‑prone, leading to repeated fixes and wasted time and effort.
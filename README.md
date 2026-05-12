# PSX Download Helper

PSX Download Helper 是一个基于 **.NET Framework 4.0** 的 Windows 桌面工具，核心能力是通过本地代理监听与 URL 替换能力，协助处理 PSX 相关下载链路。

## 主要功能

- 本地 HTTP 监听服务（可自定义 IP/端口）
- 下载请求日志窗口与状态展示
- 本地文件替换（将目标 URL 映射到本地文件）
- 离线地址记录与匹配
- CDN 主机配置读取与导出
- Wi-Fi AP（Hosted Network）辅助设置
- 多语言资源文件（含简体中文、繁体中文、英文、葡语）

## 项目结构

解决方案：`/home/runner/work/PSX-Download-Helper/PSX-Download-Helper/PSXDownloadHelper/PSSDownloadHelper.sln`

- `PSXDownloadHelper/PSXDownloadHelper`：WinForms 主程序（UI 与交互）
- `PSXDownloadHelper/PSXDH.BLL`：业务逻辑层
- `PSXDownloadHelper/PSXDH.DAL`：数据访问与映射存储
- `PSXDownloadHelper/PSXDH.Model`：配置与数据模型
- `PSXDownloadHelper/PSXDH.HttpsHelp`：HTTP 监听与连接处理
- `PSXDownloadHelper/PSXDH.ProxyHelp`：底层代理连接组件
- `PSXDownloadHelper/HelpWizard`：辅助向导程序

## 构建与运行

### 环境要求

- Windows
- .NET Framework 4.0 Targeting Pack
- Visual Studio（建议与解决方案年代匹配的版本）

### 构建

1. 使用 Visual Studio 打开 `PSSDownloadHelper.sln`
2. 选择 `Debug` 或 `Release`
3. 编译并运行 `PSXDownloadHelperUI` 项目

> 说明：当前仓库为传统 .NET Framework 4.0 项目，在 Linux `dotnet build` 环境下会因缺少 .NET Framework 4.0 参考程序集而无法直接构建。

## 开源协议

本项目采用 **GNU General Public License v3.0 (GPL-3.0)** 许可证发布。

- 许可证全文见仓库根目录 `LICENSE`
- 分发或修改本项目时，请遵守 GPLv3 相关条款

## 隐私说明

隐私政策文件：`/home/runner/work/PSX-Download-Helper/PSX-Download-Helper/privacy-policy.md`

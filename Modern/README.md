# PSX Download Helper — Modern edition

A separate modernization track for the original PSX Download Helper. The
original .NET Framework 4.0 solution remains under `PSXDownloadHelper/` and
is not modified by this edition.

## Current status — 2.3.0

- New Windows Forms application targeting .NET 10.
- Async local TCP proxy listener.
- HTTP absolute-form forwarding.
- HTTPS `CONNECT` tunnelling (TLS remains end-to-end; this app does not decrypt it).
- Request logging and configurable bind address / port.
- Safe default: listen on `127.0.0.1` only.
- Configurable CDN host/path rewrite rules with priority ordering, validation, and JSON import/export.
- Offline URL Manager with exact URL-to-mirror mappings, optional Host/Cookie overrides, priorities and JSON import/export.

The modern edition is an incremental port. Wi-Fi helper functions and broader settings migration remain tracked for subsequent milestones.

## Requirements

- Windows 10/11 x64
- .NET 10 SDK to build
- .NET 10 Desktop Runtime to run a framework-dependent build

## Build

```powershell
dotnet build .\Modern\PSXDownloadHelper.Modern.sln -c Release
dotnet run --project .\Modern\PSXDownloadHelper.Modern\PSXDownloadHelper.Modern.csproj
```

## Proxy setup

1. Start the app and keep the default loopback address for local testing.
2. Configure a client on the same PC to use the shown HTTP proxy address.
3. For a console or another device on your LAN, explicitly select `0.0.0.0`,
   allow the port through Windows Firewall only on a trusted Private network,
   and use the PC's LAN IPv4 address on the client.
4. Stop the proxy when it is not needed.

HTTPS connections use CONNECT tunnelling. The proxy can observe the destination
host and port but does not inspect encrypted HTTPS payloads. Do not install a
custom root certificate or disable TLS validation.

## Local URL-to-file mappings

On first launch, the app creates a starter configuration at:

`%LOCALAPPDATA%\\PSXDownloadHelper\\local-mappings.json`

Edit the JSON file while the proxy is running; mappings are reloaded for each
HTTP request. Each entry maps one exact HTTP URL (including its path and query)
to an existing local file:

```json
[
  {
    "url": "http://example.com/game.pkg",
    "filePath": "C:\\PSXOffline\\game.pkg"
  }
]
```

A matching GET or HEAD request is served locally without contacting the remote
server. GET supports one byte range, including open-ended and suffix ranges,
and returns `206 Partial Content`, `Content-Range`, and `Accept-Ranges`.
Unsatisfiable single ranges return `416`; malformed or multi-range requests
are ignored and receive a full `200` response. `If-Range` currently falls back
to a full response because validators are not yet emitted. HEAD returns headers
only. The mapping uses an explicit local file path; URL path segments
are never appended to that path. The file path must be readable by the current
Windows user.

CDN rules are applied to cleartext HTTP proxy requests after local-file mapping is checked and before the upstream connection is opened. HTTPS `CONNECT` is not rewritten because the encrypted request URL is unavailable without TLS interception. HTTPS
CONNECT remains an opaque TLS tunnel, so HTTPS URLs cannot be mapped or
inspected without TLS interception; this project intentionally does not add
certificate interception.

## Offline URL mappings

Open **Offline URLs** from the main window. The manager supports JSON import/export and importing the original legacy `DataHistory.xml` file. Legacy records marked as offline are converted to source/target mappings; disabled, duplicate, malformed and HTTPS records are skipped because this modern proxy only rewrites cleartext HTTP requests. The rules are stored at:

`%LOCALAPPDATA%\\PSXDownloadHelper\\offline-urls.json`

Each rule maps one exact HTTP URL to another HTTP mirror URL. Optional
`hostHeader` and `cookie` fields reproduce the legacy offline-request
behavior where a mirror expects a specific Host header or Cookie value.
Higher priority rules are checked first; matching is exact for scheme, host,
port, path and query string (path matching is case-sensitive).

Example:

```json
[
  {
    "enabled": true,
    "name": "Example game mirror",
    "sourceUrl": "http://download.example.com/game.pkg?region=eu",
    "targetUrl": "http://mirror.example.net/files/game.pkg?region=eu",
    "hostHeader": "",
    "cookie": "",
    "priority": 100
  }
]
```

The optional Cookie value is sent only when configured and replaces any
existing Cookie header for that request. Treat this configuration as sensitive
if it contains session cookies. Rules are applied to cleartext HTTP requests
after local-file mappings and before CDN rewrites. HTTPS requests use CONNECT
tunnelling and cannot be rewritten without decrypting TLS; this project does
not intercept TLS. Only use mirror URLs and cookies that you trust.

## CI validation

The .NET 10 solution built successfully on GitHub Actions in run [#42668994505](https://github.com/meicamghe/PSX-Download-Helper/actions/runs/42668994505) for the 2.3.0 offline URL manager milestone.

## Architecture roadmap

1. Core proxy lifecycle, cancellation and structured request events.
2. URL-to-local-file mapping with HTTP Range support and safe path handling.
3. CDN host/path rules with deterministic matching, validation and import/export.
4. Offline URL rule migration and settings persistence.
5. Wi-Fi helper replacement with explicit Windows capability checks.
6. Automated unit/integration tests and signed release packaging.

## License

This derivative follows the repository's GPL-3.0 license. See the root
`LICENSE` file and retain upstream attribution.

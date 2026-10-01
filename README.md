# WiFi Analyzer

A fork from the original [repo](https://github.com/Kurulko/WiFi-Analyzer)  
Overuse of claude to migrate to net10.0 + AvaloniaUI + some cleanup  
Tested on Win11

# Contents

- [Installation](#installation)
- [Overview](#overview)
- [Usage](#usage)
<br/>

## Installation

1. **Install the .NET 10 SDK** (Windows only: the app uses the native WLAN API).

2. **Clone the repository:**

   ```sh
   git clone https://github.com/Kurulko/WiFi-Analyzer.git
   cd WiFi-Analyzer
   ```

3. **Build, test and run:**

   ```sh
   dotnet build WiFiAnalyzer.slnx
   dotnet test --solution WiFiAnalyzer.slnx
   dotnet run --project src/WiFiAnalyzer.Desktop
   ```

   You can also open `WiFiAnalyzer.slnx` in Visual Studio 2026 or Rider. No MAUI workload is needed.

Scanned networks are stored in a SQLite database at `%LocalAppData%\WiFiAnalyzer\WiFiAnalyzer.db`. It is created on first start. No configuration is needed.

> **Note:** On Windows 11, scanning WiFi networks requires location access for desktop apps (Settings > Privacy & security > Location > "Let desktop apps access your location"). Without it, the app shows an "Access denied" error.
<br/>

## Overview

The WiFi Network Analyzer is a Windows desktop application built with .NET 10 and [Avalonia UI](https://avaloniaui.net/), designed to provide detailed information and analysis of WiFi networks. It offers insights into various parameters of your WiFi connection and available networks.

## Pages

### 1. Home

![image](https://github.com/Kurulko/WiFi-Analyzer/assets/95112563/d3e29c47-7adf-4934-97a2-2c52bd62eb70)

- **SSID:** Name of the WiFi network
- **Channel:** Channel number of the WiFi network
- **Frequency:** Frequency of the WiFi network (2.4 GHz, 5 GHz, or 6 GHz)
- **Signal Level:** Signal strength of the WiFi network (in percentage or using a scale)
- **BSSID:** MAC address of the access point
- **Distance:** Approximate distance to the access point
- **Secured:** is secured WiFi network
- **Protocol:** WiFi network protocol (for example, 802.11a/b/g/n/ac/ax)
- **Authentication:** Type of network authentication (PSK, EAP)
  
- **Download Speed:** Data download speed from the WiFi network

### 2. Connected

![image](https://github.com/Kurulko/WiFi-Analyzer/assets/95112563/58fae17e-1fdc-4d70-9c81-50a27d358183)

#### General Information
- Reiterates the information displayed on the main page

#### IP Address
- **Private IPv4 Address:** Local IP address assigned to your device for communication within the network
- **Public IPv4 Address:** External IP address assigned by your ISP for internet identification (Note: Not all networks have a public IP address, some use NAT)
- **Subnet Mask:** Defines the portion of the IP address used to identify the network and the host within the network

#### Security
- **Authentication**: Type of network authentication (PSK, EAP)
- **Encryption:** Type of encryption used by the WiFi network to protect data (e.g., WPA2, WPA, Open)

#### Infrastructure
- **Interfaces:** Refers to various network interfaces on your device, typically only showing WiFi interface information on this page
- **Type:** Type of network interfaces (e.g., Ethernet, Token-Ring, FDDI, Wireless80211, DSL)

### 3. Networks

#### Display of All Available Networks

- **Filtering:** Ability to filter data by WiFi network frequency (2.4 GHz, 5 GHz or 6 GHz)

#### 3.1 Table
![image](https://github.com/Kurulko/WiFi-Analyzer/assets/95112563/b276ff02-0942-463c-9c0d-9d7772f346e9)

- **Sorting:** Click any column header to sort (e.g., SSID, Signal Level, Distance). The sort is kept when data refreshes.

#### 3.2 Graphs

Use the **View Graph** / **View Table** button to switch between the table and the graphs.

##### a) dBm
![image](https://github.com/Kurulko/WiFi-Analyzer/assets/95112563/eef1de1b-61e0-4799-8b3f-373506f29d0b)

- **Signal Strength Graph:** Displays the signal strength of each WiFi network in the table
  - **X-Axis:** SSID of the WiFi network
  - **Y-Axis:** Signal strength (in dBm)

##### b) Distance
![image](https://github.com/Kurulko/WiFi-Analyzer/assets/95112563/ae93c750-2536-4787-80a9-ba0826bb49dc)

- **Distance Graph:** Displays the distance to each WiFi network in the table
  - **X-Axis:** SSID of the WiFi network
  - **Y-Axis:** Distance (in meters)
<br/>

## Usage

To get started with the WiFi Network Analyzer, launch the application and navigate through the pages to view detailed information about your current WiFi connection and other available networks. Use the sorting and graphing features to analyze the data and make informed decisions to optimize your wireless connectivity.

## Requirements

- Windows 10 or later, with a WiFi adapter
- .NET 10 SDK

## Project structure

```
src/WiFiAnalyzer.Core/                    Models, services (WLAN, speed test), EF Core context, ViewModels
src/WiFiAnalyzer.Desktop/                 Avalonia UI app (views, converters, notifications)
tests/WiFiAnalyzer.Core.UnitTests/        xUnit v3 unit tests
tests/WiFiAnalyzer.Core.IntegrationTests/ xUnit v3 tests against a real SQLite file
```

## Tech stack

- .NET 10, C# latest
- Avalonia UI 12 (Fluent theme, DataGrid)
- CommunityToolkit.Mvvm
- ScottPlot for the graphs
- Entity Framework Core with SQLite
- ManagedNativeWifi (native WLAN API)
- Download speed test against `speed.cloudflare.com` (4 parallel streams, 15 s, `HttpClient` only)

> Screenshots above come from the original .NET MAUI version. The Avalonia version keeps the same pages and data.

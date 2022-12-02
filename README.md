# Get Windows Build Numbers

Easily get build numbers for Windows 10/11 feature updates through automated means.

## 🤔 Why make this?

So what's one thing that can be obnoxious with updating compliance policies in Microsoft Intune? Not being able to easily update the build numbers for in-service Windows releases easily. I initially wrote [a PowerShell script](https://gist.github.com/Smalls1652/1e00193fb51bbf85ff18b41fe33fad0d) in late 2020/early 2021 that would get the Windows 10 feature updates, build numbers (Based on the quality updates for their respective feature updates), and their servicing support status, but it eventually got complicated to update as a single PowerShell script. I could have converted the entire thing into a pure script-based PowerShell module, but I've got other plans for using this outside of PowerShell.

As far as I'm aware, Microsoft doesn't provide a simple API or utility to get this information. It's all web-based, which makes automation harder when you need to use that data.

## ⚙️ How does it work?

It's very simple, but also very complicated. Data is gathered from these sources:

- [Windows Release Health dashboard](https://learn.microsoft.com/en-us/windows/release-health/)
- Microsoft's lifecycle pages:
  - [Windows 10 Home and Pro (Consumer)](https://learn.microsoft.com/en-us/lifecycle/products/windows-10-home-and-pro)
  - [Windows 10 Enterprise and Education (Enterprise)](https://learn.microsoft.com/en-us/lifecycle/products/windows-10-enterprise-and-education)
  - [Windows 11 Home and Pro (Consumer)](https://learn.microsoft.com/en-us/lifecycle/products/windows-11-home-and-pro)
  - [Windows 11 Enterprise and Education (Enterprise)](https://learn.microsoft.com/en-us/lifecycle/products/windows-11-enterprise-and-education)

Data is scraped from these pages using regular expressions and returned with this data:

- The feature update's release name (Eg. 21H2)
- The end-of-life date for both the consumer and enterprise releases and whether or not it is currently end-of-life.
- A collection of all of the quality updates released for that release.
  - Data returned is:
    - The build number
    - The servicing channels it has been released to.
    - The date it was released on and whether or not it's a _"Patch Tuesday"_ release.
    - The KB article ID and it's URL.

## 🛒 Current availability

Right now there are two components currently available for use:

- A standard class library that drives the core functionality of getting the data.
- A PowerShell module for using through a command-line interface (CLI).

### Nuget package

> **⚠️ Note**
>  
> A Nuget package has not been published yet.

### PowerShell Module

> **⚠️ Note**
>  
> The PowerShell module **requires** PowerShell 7.2 or higher to run.
>  
> There are no plans at the moment to support Windows Powershell 5.1, which is installed on Windows 10/11 by default, due to the reliance on targetting .NET 6 and higher.

You can install the [PowerShell module from the PowerShell Gallery](https://www.powershellgallery.com/packages/SmallsOnline.WindowsBuildNumbers.Pwsh) by running the following command in a PowerShell console:

```powershell
Install-Module -Name "SmallsOnline.WindowsBuildNumbers.Pwsh"
```

Once installed, you can run commands like so:

#### Example 01

Get all release info for Windows 10.

```powershell
PS > Get-WindowsReleaseInfo -WindowsVersion Windows10
```

```text
ReleaseName       : 22H2
ConsumerEoLDate   : 5/14/2024 10:59:59 PM -08:00
ConsumerIsEoL     : False
EnterpriseEoLDate : 5/13/2025 10:59:59 PM -08:00
EnterpriseIsEoL   : False
ReleaseBuilds     : {19045.2311, 19045.2251, 19045.2130}

ReleaseName       : 21H2
ConsumerEoLDate   : 6/13/2023 10:59:59 PM -08:00
ConsumerIsEoL     : False
EnterpriseEoLDate : 6/11/2024 10:59:59 PM -08:00
EnterpriseIsEoL   : False
ReleaseBuilds     : {19044.2311, 19044.2251, 19044.2194, 19044.2193…}

[...]
```

#### Example 02

Get all release info for Windows 11.

```powershell
PS > Get-WindowsReleaseInfo -WindowsVersion Windows11
```

```text
ReleaseName       : 22H2
ConsumerEoLDate   : 10/8/2024 10:59:59 PM -08:00
ConsumerIsEoL     : False
EnterpriseEoLDate : 10/14/2025 10:59:59 PM -08:00
EnterpriseIsEoL   : False
ReleaseBuilds     : {22621.900, 22621.819, 22621.755, 22621.675…}

ReleaseName       : 21H2
ConsumerEoLDate   : 10/10/2023 10:59:59 PM -08:00
ConsumerIsEoL     : False
EnterpriseEoLDate : 10/8/2024 10:59:59 PM -08:00
EnterpriseIsEoL   : False
ReleaseBuilds     : {22000.1281, 22000.1219, 22000.1165, 22000.1100…}
```

#### Example 03

Get the release info for a specific feature update release of Windows 11 and list all quality update builds for that feature update.

```powershell
PS > $win11Release = Get-WindowsReleaseInfo -WindowsVersion Windows11 -ReleaseName "22H2"

PS > $win11Release
```

```text
ReleaseName       : 22H2
ConsumerEoLDate   : 10/8/2024 10:59:59 PM -08:00
ConsumerIsEoL     : False
EnterpriseEoLDate : 10/14/2025 10:59:59 PM -08:00
EnterpriseIsEoL   : False
ReleaseBuilds     : {22621.900, 22621.819, 22621.755, 22621.675…}
```

```powershell
PS > $win11Release.ReleaseBuilds
```

```text
BuildNumber           : 22621.900
ServicingChannels     : {General Availability Channel}
ReleaseDate           : 11/29/2022 6:00:00 PM +00:00
IsPatchTuesdayRelease : False
KbArticleId           : KB5020044
KbArticleUrl          : https://support.microsoft.com/help/5020044

BuildNumber           : 22621.819
ServicingChannels     : {General Availability Channel}
ReleaseDate           : 11/8/2022 6:00:00 PM +00:00
IsPatchTuesdayRelease : True
KbArticleId           : KB5019980
KbArticleUrl          : https://support.microsoft.com/help/5019980

BuildNumber           : 22621.755
ServicingChannels     : {General Availability Channel}
ReleaseDate           : 10/25/2022 6:00:00 PM +00:00
IsPatchTuesdayRelease : False
KbArticleId           : KB5018496
KbArticleUrl          : https://support.microsoft.com/help/5018496

BuildNumber           : 22621.675
ServicingChannels     : {General Availability Channel}
ReleaseDate           : 10/18/2022 6:00:00 PM +00:00
IsPatchTuesdayRelease : False
KbArticleId           : KB5019509
KbArticleUrl          : https://support.microsoft.com/help/5019509

BuildNumber           : 22621.674
ServicingChannels     : {General Availability Channel}
ReleaseDate           : 10/11/2022 6:00:00 PM +00:00
IsPatchTuesdayRelease : True
KbArticleId           : KB5018427
KbArticleUrl          : https://support.microsoft.com/help/5018427

[...]
```

## 🧮 Plans for the future

In the future, I am planning two extra pieces:

- A public web API.
- A public website for viewing the data.

All of which will be open-source for your own use-cases, such as self-hosting or utlizing in your own projects.

## 🏗️ Building from source

### 🧰 Pre-requistites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
  - .NET 6 is the minimum SDK version for the class library, but, in the future, the API and public website will be targeting .NET 7 and future releases as they come out.

## 🤝 License

This project is licensed under the [MIT License](./LICENSE).

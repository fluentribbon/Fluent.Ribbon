# Fluent.Ribbon

<div align="center">

![Banner](./Images/banner.png)

Fluent.Ribbon is a library that implements an Office-like user interface for the Windows Presentation Foundation (WPF). It provides controls such as RibbonTabControl, Backstage, Gallery, QuickAccessToolbar, ScreenTip and so on.

[![Twitter](https://img.shields.io/badge/twitter-%40batzendev-blue.svg?style=flat-square)](https://twitter.com/batzendev)

[![Build status (master)](https://img.shields.io/appveyor/ci/batzen/fluent-ribbon/master.svg?style=flat-square&&label=master)](https://ci.appveyor.com/project/batzen/fluent-ribbon/branch/master)
[![Build status (develop)](https://img.shields.io/appveyor/ci/batzen/fluent-ribbon/develop.svg?style=flat-square&&label=develop)](https://ci.appveyor.com/project/batzen/fluent-ribbon/branch/develop)
[![Tests (develop)](https://img.shields.io/appveyor/tests/batzen/fluent-ribbon/develop.svg?style=flat-square)](https://ci.appveyor.com/project/batzen/fluent-ribbon/branch/develop)

[![Downloads](https://img.shields.io/nuget/dt/Fluent.Ribbon.svg?style=flat-square)](http://www.nuget.org/packages/Fluent.Ribbon/)
[![Nuget](https://img.shields.io/nuget/v/Fluent.Ribbon.svg?style=flat-square)](http://nuget.org/packages/Fluent.Ribbon)
[![Nuget (pre)](https://img.shields.io/nuget/vpre/Fluent.Ribbon.svg?style=flat-square&label=nuget-pre)](http://nuget.org/packages/Fluent.Ribbon)
[![Release](https://img.shields.io/github/release/fluentribbon/fluent.ribbon.svg?style=flat-square)](https://github.com/fluentribbon/Fluent.Ribbon/releases/latest)

[![Issues](https://img.shields.io/github/issues/fluentribbon/fluent.ribbon.svg?style=flat-square)](https://github.com/fluentribbon/Fluent.Ribbon/issues)
[![Issues](https://img.shields.io/github/issues-closed/fluentribbon/fluent.ribbon.svg?style=flat-square)](https://github.com/fluentribbon/Fluent.Ribbon/issues?q=is%3Aissue+is%3Aclosed)

[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/License.txt)

</div>

![Showcase](./Images/Showcase.gif)

- [More Screenshots](../../wiki/Screenshots)

## Documentation

Visit the [documentation](http://fluentribbon.github.io/documentation/).  
Almost all features are shown in the [showcase application](./Fluent.Ribbon.Showcase).  
The showcase application is included with every release, so you can download stable versions from [releases](../../releases) and preview versions from [CI artifacts](https://ci.appveyor.com/project/batzen/fluent-ribbon/branch/develop/artifacts).  
If you think there is something missing in the showcase application feel free to create an issue for that.

To be more familiar with the Ribbon concept see [msdn article](https://docs.microsoft.com/en-us/windows/desktop/uxguide/cmd-ribbons).

## History &amp; roadmap

A history of changes is maintained in the [Changelog](Changelog.md).
The roadmap is done by [milestones](../../milestones).

If you feel lack of some important features feel free to use [issues](https://github.com/fluentribbon/Fluent.Ribbon/issues) to create an issue/feature request.

## Access to preview versions

You can access preview versions through the [AppVeyor nuget feed](https://ci.appveyor.com/nuget/fluent-ribbon).

## What you can do to help us

- We are accepting pull requests, so you are very welcome to create one
- [Fix some bugs](../../issues)
- Help us translating
- Help us updating the documentation

## Development requirements

- .NET SDK 8.0.401 or later
- An IDE that supports the required .NET SDK (for example Rider 2024.1, Visual Studio 2022 (17.10) or later)

## Designer issues

If you have issues using the designer inside of Visual Studio you can try the following things:

- Clear `%LOCALAPPDATA%\Microsoft\VisualStudio\[VERSION]\Designer\ShadowCache` or `%LOCALAPPDATA%\Microsoft\WDExpress\[VERSION]\Designer\ShadowCache` where `[VERSION]` has to be changed according to your version of Visual Studio
- Clear the `.vs` folder in your development folder

If none of that helps feel free to report an issue here.

## Formatting settings that have to be used

- For general formatting you have to use editorconfig (should work out of the box with Visual Studio 2019)
- XAML
  - Position each attribute on a separate line
    - Position first attribute on same line as start tag

## Awesome tools which Fluent.Ribbon can use

[![ReSharper logo](https://resources.jetbrains.com/storage/products/company/brand/logos/ReSharper.svg)](https://jb.gg/OpenSourceSupport)

<div>
  <a href="https://www.appveyor.com/">
    <img alt="appveyor" width="128" heigth="128" src="./Images/Appveyor_logo.svg">
  </a>
</div>

## Licence

[MIT License (MIT)](./License.txt)

---

This project was previously hosted on [CodePlex](https://fluent.codeplex.com/).

# Notes for developers

This codebase is bit old so you'll need some old tech to build it:

- .NET Framework 4.6.2
- .NET Core 2.1.402
- NodeJS 9.11.2
- Yarn 1.22.17
- nvm 1.1.9 (when you need to run multiple versions of NodeJS only)
- Python 2.7.18 (when you are building node-saas from source only)

I am using Visual studio 2022 to build this.

## Building for dev

To make it runnable vrom VS, use these steps:

- Install VS2022 with .NET Framework 4.6.2 SDK and .NET Core SDK 2.1.402.
- Checkout the source code from [https://github.com/enif77/EESSI-AS4.NET](https://github.com/enif77/EESSI-AS4.NET).
- Open the `AS4.sln` in the VS2022.
- Build the solution.
- Run unit tests from the `Eu.EDelivery.AS4.UnitTests` project (this creates sample configs and pmodes in the `output` directory).
- Create directory `EESSI-AS4.NET\output\messages\out`.
- Set the `Services\Eu.EDelivery.AS4.ServiceHandler.ConsoleHost` project as the launch project.

Now you can run the `Eu.EDelivery.AS4.ServiceHandler.ConsoleHost` project to play with it! :-)

### How to compile the frontend

- For running multiple NodeJS versions I am using NVM: [nvm-windows](https://github.com/coreybutler/nvm-windows).
- You need NodeJS >= 4 a <= 9 (I used the version 9.11.2).
- Compilation is controlled by the Yarn 1.x (I used version 1.22.17)
- Go to the `source\Eu.EDelivery.AS4.Fe\ui` directory and run these commands:

```
yarn install
yarn build
yarn copytooutput-dev
```

A `dist` directory is created and its content is copyied to the `output\ui\dist` directory, where it is expected by the frontend (.Fe) project.


## Building the release (staging) version

Run these commands in a whatever directory:

```
git clone https://github.com/enif77/EESSI-AS4.NET.git
CD EESSI-AS4.NET
powershell scripts\release.ps1
```

It creates everything in the `output\staging` directory.

NOTE: If you are using NVM, run these commands in an admin console (with eleveted rights)!

----------

# <span>AS4.NET</span>

## Introduction

<span>AS4.NET</span> is an open-source application that implements the OASIS AS4 specification. It supports both the e-SENS e-Delivery and the EESSI AS4 Messaging profile as an ebMS endpoint.  
Since version v3.0.0, <span>AS4.NET</span> can also act as an intermediary MSH (i-MSH) with message forwarding support and MEP bridging.

The component has been conformance tested against the e-SENS eDelivery specifications.  
Testing against the EESSI AS4 Messaging Profile has also been conducted.

<span>AS4.NET</span> is interoperable with multiple other AS4 gateway providers; <span>AS4.NET</span> has undergone performance and interop-tests against Holodeck B2B, RSSBus, Domibus, Flame Message Server and IBM B2B.

## Installation

<span>AS4.NET</span> can be downloaded from the following locations:

- [AS4.NET v4.0.1](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/4.0.1/eessi_as4.net-4.0.1.zip)
- [AS4.NET v4.0.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/4.0.0/eessi_as4.net-4.0.0.zip)
- [AS4.NET v3.1.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/3.1.0/eessi_as4.net-3.1.0.zip)
- [AS4.NET v3.0.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/3.0.0/eessi_as4.net-3.0.0.zip)
- [AS4.NET v2.0.1](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/2.0.1/eessi_as4.net-2.0.1.zip)
- [AS4.NET v2.0.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/2.0.0/eessi_as4.net-2.0.0.zip)
- [AS4.NET v1.1.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/1.1.0/eessi_as4.net-1.1.0.zip)
- [AS4.NET v1.0.0](https://ec.europa.eu/cefdigital/artifact/content/groups/public/eu/eessi/as4/eessi_as4.net/1.0/eessi_as4.net-1.0.zip)

## Documentation

A configuration- and usermanual can be found [online](https://ec.europa.eu/cefdigital/wiki/display/EDELCOMMUNITY/AS4.NET).

## Features

### v1.0.0

- One-Way/Push message exchange pattern
- XML based configuration
- XML based PMode configuration
- Dynamic PMode override
- Multiple submit, notify and deliver agents
- FILE based receivers and senders
- Signing and encryption using WS-Security
- AS4 Compression
- AS4 Reception Awareness and Retry
- AS4 Duplicate Detection and Elimination

### v1.1.0

- Submit, deliver and notify via HTTP protocol
- Submit and deliver attachments via HTTP protocol
- One-Way/Pull pattern as initiator
- Support for sub channels in One-Way/Pull pattern
- Support for multi-hop AS4 profile
- Support for TLS client certificates
- Performance tuning for large messages, up to 2GB
- Performance tuning for high volume processing

### v2.0.0

- Web interface for configuration
- Web interface for monitoring
- Web interface for user management
- Web interface for testing
- One-Way/Pull pattern as responder
- Support for sub-channels
- Support for message forwarding
- Support for MEP bridging
- Support for PullRequest authorization
- Support for SMP/SML dynamic discovery
- Support for TLS server side
- Continued performance tuning for large messages, up to 3GB
- Continued performance tuning for high volume processing
- Improvements to the internal messaging engine

### v2.0.1

- Configurable payload naming when delivering on filesystem
- Continued performance tuning for large messages and high volume processing

### v3.0.0

- Intermediary MSH functionality with message forwarding including MEP bridging support
- Static Submit support
- Possibility to run the <span>AS4.NET</span> MSH as a Windows Service
- Improved Dynamic Discovery implementation
- Dynamic Forwarding support
- Improved high availability support
- Support for Non-Repudiation of Receipt verification
- Support for automatic Message Cleanup
- Optionally allow that a message is signed with a certificate coming from an unknown CA authority when verifying message signatures
- Web interface for SMP Routing Configuration
- Improvements in the web interface for configuration
- Improvements to the internal messaging engine

### v3.1.0

- Retry functionality for deliver operation
- Retry functionality for notify operation
- Static Receive support
- Improvements in the web interface for configuration
- Improvements in the internal messaging engine

### v4.0.0

- Support for the OASIS BDX dynamic discovery profile
- Support for sending response signal messages via reliable piggybacking in a pull receive scenario
- Allow the AS4.NET Windows Service MSH to be installed via an MSI
- Control the AS4.NET Windows Service MSH via a system tray application
- Improved Receiving PMode matching proces when multiple from-parties / to-parties are specified in the AS4 Message or in the Receiving PMode
- Allow dynamic discovery based on the sender information in the SubmitMessage or in case of a forwarding scenario on the sender information in the AS4 Message
- Support for internal journal logging to track down operations executed on the message (compress/decompress, signing/verify, encrypt/decrypt)
- Support for receiving bundled message units
- Configurable submit payload retrieval path location
- Configurable pull authorization map path location
- Improvements in the web interface for pmode and agents configuration
- Improvements to the internal messaging engine

> This version doesn't support **Sending PModes** anymore as a way to respond to AS4 messages but uses the **Receiving PMode** for this. Please update your **Receiving PModes**, for more information see: [Remove Sending PMode as responding PMode](output/doc/wiki/runtime/configuration/remove-response-pmode.md).

### v4.0.1

- DynamicDiscovery settings that can be defined in the Sending PMode are no longer case-sensitive
- Bugfix: Send retry-functionality (receptionawareness) was not working in v4.0.0 when `IsMultihop` was enabled in the Sending PMode.  This issue is fixed in v4.0.1.
- Bugfix: When AS4.NET v4.0.0 is configured to receive messages via pulling, response signal messages were created but were never sent.  This issue is fixed in v4.0.1.

## Third Party software

The following third party libraries are used by <span>AS4.NET</span> runtime:

- [BouncyCastle](https://github.com/bcgit/bc-csharp) ([MIT License](https://opensource.org/licenses/MIT))
- [FluentValidation](https://github.com/JeremySkinner/FluentValidation) ([Apache 2](http://www.apache.org/licenses/LICENSE-2.0.html))
- [Mimekit](https://github.com/jstedfast/MimeKit) ([MIT License](https://opensource.org/licenses/MIT))
- [Nlog](https://github.com/NLog/NLog) ([BSD License](https://opensource.org/licenses/BSD-3-Clause))
- [Polly](https://github.com/App-vNext/Polly) ([BSD License](https://opensource.org/licenses/BSD-3-Clause))
- [SQLite](https://sqlite.org/) ([Public Domain](https://sqlite.org/copyright.html))
- [Remotion](https://github.com/re-motion/Relinq) ([Apache 2](http://www.apache.org/licenses/LICENSE-2.0.html))

### All versions up to v3.1.0

- [Automapper](https://github.com/AutoMapper/AutoMapper) ([MIT License](https://opensource.org/licenses/MIT))

### All versions starting from v4.0.0

- [Heijden.Dns](https://github.com/ghuntley/Heijden.Dns) ([MIT License](https://opensource.org/licenses/MIT))
- [Wiry.Base32](https://github.com/wiry-net/Wiry.Base32) ([MIT License](https://opensource.org/licenses/MIT))

## License

This software is licensed under the [EUPL License v1.1](https://joinup.ec.europa.eu/community/eupl/og_page/european-union-public-licence-eupl-v11).

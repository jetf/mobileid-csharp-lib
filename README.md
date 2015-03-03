# C# client library for Mobile ID 

This is a C# library that an application provider (*AP*) can use to call the [Mobile ID](https://www.swisscom.ch/mid) services.
It currently consists of 3 visual studio projects:

* Service:      The C# client library
* CliClient:    A console application using the C# client library. It serves as an example for using the library.
* ServiceTest:  Regression tests for C# client library

Limitation: Current version supports only 2 services in C# client library, more will be added soon.

## System Requirement:

* Microsoft .NET Framework 4.5
* You have a Mobile ID Application Provider Account (`AP_ID`)

## Setting up the Runtime Environment:

### Step 1: IP connectivity between Mobile ID client and Mobile ID server

An application provider (*AP*) can access the Mobile ID servers (*MID*) only from specific IP addresses.
The address range is specific to an Application Provider (*AP*) and configured by the MID service during the enrollment process for the AP.

### Step 2. SSL connectivity between Mobile ID client and Mobile ID server

An AP must establish a mutually authenticated SSL/TLS connection with a MID server before calling the MID services.
The C# client library relies on operating system facility (Windows Certificate Store / SChannel) for SSL/TLS connections.

During the enrollment process for the AP, you have created a SSL/TLS client certificate
(either self-signed or signed by an Certificate Authority trusted by your organisation) for your Mobile ID client.
You also need the certificate of the Certificate Authority (*CA*) for the MID servers, which can be downloaded from:

* http://aia.swissdigicert.ch/sdcs-root2.crt (productive environment, SHA1 thumbprint `77474fc630e40f4c47643f84bab8c6954a8a41ec`, SHA256 thumbprint `f09b122c7114f4a09bd4ea4f4a99d558b46e4c25cd81140d29c05613914c3841`)
* http://aia.pre.swissdigicert.ch/sdcs-root2.crt (test environment, SHA1 thumbprint `ce527fa7a7a7b7eb002fb90ec14e963d3e524441`, SHA256 thumbprint `f94a99db313a113895439f3e573343deb5ee81ee20d0fb2ee4eb671da2360878`)

#### Configuration for SSL/TLS:

1. Import your SSL client certificate file (PFX/PKCS#12 format) into your personal certificate store:

  Right-click your SSL Client certificate file, select `Install PFX`, the Certificate Import Wizard will pop up.
  Click `Next` twice, then enter the passphase of the PFX file, click `Next` and click `Finish`.

2. If your SSL client certificate is issued by a Certificate Authority trusted by your organisation, you
  can skip this step. If your SSL client certificate is a self-signed certificate, you need explicitly 
  configure trust for it:
  Run `certmgr.msc`, this opens the Certificate Management Console for Current User;
  right-click `Trusted People`, navigate to `All Task`, then `Import...`, this opens the `Certificate Import Wizard`;
  Clicks `Next`, locates the PFX file in `File to Import`, `Next`, enters passphrase for the private key, clicks `Next` twice and `Finish`.

3. Verify the SSL client certificate has been correctly imported and trusted:

  In Certificate Management Console (`certmgr.msc`), navigate to Personal > Certificates, double-click the certificate imported in step 1, 
  select `Certification Path`, the `Certificate stutus` should displays "This certificate is OK".

4. Configure trust to Root CA of MID servers:
   In `certmgr.msc`, navigate to `Trusted Root Certificate Authority`,
   Right-click `Certificates`, select `All Tasks`, `Import...` , then `Next`,
   select the file *.crt containing the Root CA of MID servers in the correct environment (production/test),
   `Next` twice, `Finish`, confirm `Yes` on the Security Warning "You are about to install a certificate from a certificate authority (CA) claiming to represent: ... Thumbprint (sha1): ..."
   Click `OK`.

5. Verify the SSL/TLS connectivity:
   Use "Internet Explorer" (version 10 & 11 are tested) to connect to the URL
   https://mobileid.swisscom.com/soap/services/MSS_ProfilePort (productive environment) or
   https://soap.pp.mobileid.swisscom.com/soap/services/MSS_ProfilePort (test environment).
   IE should display a "`Confirm Certificate`" dialog for picking up the client certificate and then the text

`````
   MSS_ProfilePort
   Hi there, ...
`````

### Step 3: Configuration of Mobile ID client

The Mobile ID Client library can be configured by a configuration file in XML syntax.
The sample Mobile ID client console application reads the configuration file `MobileIdClient.xml`.
The file need to be adapted for your environment.

// TODO: more documentation of MobileIdClient.xml

The logging / tracing of Mobile ID Client library can be controlled via the dotNet tracing
configuration mechanism. The sample configuration writes logging entries to 3 destinations:
Windows Event Log, Console, and a log file.
See the `app.config` (or `CliClient.exe.config`) for details.


## Overview of the Classes in C# client library

* `MobileID.IAuthentication`:	interface exposed by the library
* `MobileID.WebClientImpl`:	an implementation of the interface. 
                                It uses `System.Net.WebRequest` class to send request, and uses 
                                `System.Xml.XmlDocument` to parse service response (without using WSDL).

The rest are data types (`AuthRequestDto`, `AuthResponseDto`, `ServiceStatus`)
or enums (`ServiceStatusCode`, `ServiceStatusColor`, `Userlanguage`, `EventId`) used in the interface,
and helper classes (`Util`, `WebClientConfig`).

See `Program.cs` in project `CliClient` for example of usage.

Limitation: Many services are currently not yet implemented.

# C# client library for Mobile ID 

Contains a C# library that AP can use to call the [https://github.com/SCS-CBU-CED-IAM/mobileid](Mobile ID) services.
It consists of 3 visual studio projects currently:

* Service:      The C# client library
* CliClient:    A console application using the C# client library. It serves as an example for using the library.
* ServiceTest:  Regression tests for C# client library

Limitation: Current alpha release supports only 2 services in C# client library.

## System Requirement:

* Microsoft .NET Framework 4.5
* You have a Mobile ID Application Provider Account (`AP_ID`)

## Setting up the Runtime Environment:

### IP connectivity between Mobile ID client (i.e. *Application Provider*, or *AP*) and Mobile ID (*MID*) server

Access to MID servers is only possible from specific IP addresses. 
The address range is specific to an AP and defined during the enrollment process of the AP.

### SSL connectivity between Mobile ID client and Mobile ID server

AP must establish a mutually authenticated SSL/TLS connection with a MID server before calling the MID services.
The C# client library relies on operating system facility (Windows Certificate Store / SChannel) for the establishment of SSL/TLS connections.

During the enrollment process of the AP, you have created a SSL/TLS client certificate
(either self-signed or signed by an Certificate Authority trusted by your organisation) for your Mobile ID client.
In this document, we use the file ApSslClientCert.pfx We denote this certificate as TCC.
The Certificate Authority of MID servers can be downloaded from:

* http://aia.swissdigicert.ch/sdcs-root2.crt (productive environment, SHA1 thumbprint `77474fc630e40f4c47643f84bab8c6954a8a41ec`, SHA256 thumbprint `f09b122c7114f4a09bd4ea4f4a99d558b46e4c25cd81140d29c05613914c3841`)
* http://aia.pre.swissdigicert.ch/sdcs-root2.crt (test environment, SHA1 thumbprint `ce527fa7a7a7b7eb002fb90ec14e963d3e524441`, SHA256 thumbprint `f94a99db313a113895439f3e573343deb5ee81ee20d0fb2ee4eb671da2360878`)

#### Configuration Procedure for SSL:

1. Import your SSL client certificate file into your personal certificate store:
  Right-Click your SSL Client certificate file, select `Install PFX`, the Certificate Import Wizard will pop up.
  Click Next twice, then enter the passphase used to protected the private key in PFX, 
  Click Next and Click Finish.

2. If your SSL client certificate is issued by a Certificate Authority trusted by your organisation, you
  can skip this step. If your SSL client certificate is a self-signed certificate, you need explicitly 
  configure trust for it:
  Start `certmgr.msc`, this opens the certificate store of Current User;
  Right-click `Trusted People`, navigate to `All Task`, then `Import...`, this opens the `Certificate Import Wizard`;
  Clicks `Next`, locates the PFX file in `File to Import`, `Next`, enters passphrase for the private key, `Next` twice, Finish.

3. Verify the SSL client certificate has been correctly imported and trusted:
  In `certmgr.msc`, navigate to Personal > Certificates, double-click the certificate imported in step 1, 
  select `Certification Path`, the `Certificate stutus` should displays "This certificate is OK".

4. Configure trust to Root CA of MID servers:
   In `certmgr.msc`, navigate to `Trusted Root Certificate Authority`,
   Right-click `Certificates`, select `All Tasks`, 'Import...' , then `Next`,
   select the file *.crt containing the Root CA of MID servers in the correct environment (production/test),
   `Next` twice, `Finish`, confirm `Yes` on the Security Warning "You are about to install a certificate from a certificate authority (CA) claiming to represent: ... Thumbprint (sha1): ..."
   Click `OK`.

5. Verify the SSL/TLS connectivity:
   Use "Internet Explorer" (version 10 & 11 are tested) to connect to the URL
   https://soap.pp.mobileid.swisscom.com/soap/services/MSS_ProfilePort (test environment) or
   https://mobileid.swisscom.com/soap/services/MSS_ProfilePort (productive environment).
   IE should display a "Confirm Certificate" dialog for picking up the client certificate and then the text

   > MSS_ProfilePort
   > Hi there, ...

### Configuration of Mobile ID client

The Mobile ID Client library can be configured by a configuration file in XML syntax.
The sample Mobile ID client console application reads the configuration file `MobileIdClient.xml`.
The file need to be adapted for your environment.

// TODO: more documentation of MobileIdClient.xml

The logging / tracing of Mobile ID Client library can be controlled via the .net tracing 
configuration mechanism. The sample configuration writes logging entries to 3 destinations:
Windows Event Log, Console, and a log file.

See the `app.config` (or `CliClient.exe.config`) for details.


## Overview of the Classes in C# client library

* MobileID.IAuthentication:	interface exposed by the library
* MobileID.WebClientImpl:	an implementation of the interface. 
                                It uses `System.Net.WebRequest` class to send request, and uses 
                                `System.Xml.XmlDocument` to parse service response (without using WSDL).

The rest are data types (`AuthRequestDto`, `AuthResponseDto`, `ServiceStatus`)
or enums (`ServiceStatusCode`, `ServiceStatusColor`, `Userlanguage`, `EventId`) used in the interface,
and helper classes (`Util`, `WebClientConfig`)

See `Program.cs` in project `CliClient` for example of usage.

Limitation: Many services are currently not yet implemented.

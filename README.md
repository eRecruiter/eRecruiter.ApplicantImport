# eRecruiter.ApplicantImport

Import applicant profiles from a CSV file into eRecruiter. You'll need an up-and-running eRecruiter API endpoint (version >= 1.37) and the respective API key.

## Download

This nifty little tool builds automatically on every commit on GitHub using AppVeyor. You can download the latest build here: https://ci.appveyor.com/project/saxx/erecruiter-applicantimport/build/artifacts

[![Build status](https://ci.appveyor.com/api/projects/status/2rn1eok9hfjnrlcr)](https://ci.appveyor.com/project/saxx/erecruiter-applicantimport)

## Supported applicant attributes
Each applicant attribute corresponds to a column in your CSV file. At the moment, these attributes are supported:

- **Gender:** `Gender`
- **First name:** `FirstName` - must never be empty
- **Last name:** `LastName` - must never be empty
- **Title:** `Title` - based on the mandator settings "title before name" or "title after name" is automatically set correctly.
- **E-Mail:** `Email`
- **Phone:** `Phone`
- **Mobile phone:** `MobilePhone`
- **Street (address):** `Street`
- **ZIP code:** `ZipCode`
- **City (address):** `City`
- **Country (address):** `Country`
- **Nationality (citizenship):** `Nationality`
- **Date of birth:**: `Birthdate`
- **CV:** `Cv` - must be empty or a path to an existing file
- **Photo** (applicant portrait): `Photo` - must be empty or a path to an existing file
- **Document:** `Document` - must be empty or a path to an existing file (to import single file) or directory (to import all files in directory). Also, configuration for `SubType` is required to specify the applicant document type.
- **Job profile:** `JobProfile`
- **Region:** `Region`
- **Earliest possible begin date:** `BeginDate` - use `DateFormat` to specify the date format, default is `yyyy-MM-dd`.
- **Applicant #:** `Id` - this is actually a "magic column". If it is specified (and contains a valid applicant #), existing applicants will be updated instead of newly created. This is very handy if you want to bulk-add information to already existing applicants.
- **Career level:** `Careerlevel`
- **Referrer:** `Referrer`
- **Referrer additional information:** `ReferrerAdditionalInfo` - if present, a column for `Referrer` must be present as well.
- **Knowledge:** `Knowledge` - configuration for `SubType` is required to specify the knowledge.
- **Date of creation:** `CreationDate` - use `DateFormat` to specify the date format, default is `yyyy-MM-dd`.
- **Important information (about applicant:** `ImportantInfo`
- **Classification:** `Classification`
- **Classification reason:** `ClassificationReason` - if present, a column for `Classification` must be present as well.
- **Custom field:** `CustomField` - configuration for `SubType` is required to specify the custom field. Also, configuration for `DateFormat` is supported (if custom field is of type `Date`)
- **Ignore:** `Ignore` - Specifically configure columns that should be ignored at all.

## Known problems and shortcomings

- It's not possible to add a history entry to an applicant at the moment.
- The auto-generated history entries are a little buggy at the moment:
    - Base information (name, gender, e-mail) are not displayed in history (they are set correctly tho).
    - Photos are displayed as [-] instead of [+].

## Usage
`eRecruiter.ApplicantImport.exe --config=your_config_file.json --file=your_csv_file.csv [--continueOnWarnings] [--generateCsvStub]`

You'll need to specify a configuration file (contains API and column configuration) and a CSV file (contains the actual data to import).

### Configuration file
For an example JSON configuration file that contains all supported applicant attributes, see [example-configuration.json](example-configuration.json)

### CSV file
Use UTF8 encoding and TAB-separated CSV. See [example-data.csv](example-data.csv) for an example CSV file.


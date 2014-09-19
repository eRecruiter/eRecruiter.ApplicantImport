# eRecruiter.ApplicantImport

Import applicant profiles from a CSV file into eRecruiter.

## Supported applicant attributes

- First name: `FirstName` - must never be empty
- Last name: `LastName` - must never be empty
- Gender: `Gender`
- E-Mail: `Email`
- Phone: `Phone`
- Mobile phone: `MobilePhone`
- Street (address): `Street`
- ZIP code: `ZipCode`
- City (address): `City`
- CV: `Cv` - must be empty or a path to an existing file
- Photo (applicant portrait): `Photo` - must be empty or a path to an existing file
- Document: `Document` - must be empty or a path to an existing file (to import single file) or directory (to import all files in directory). Also, configuration for `AdditionalType` is required to specify the applicant document type.
- Job profile: `JobProfile`
- Region: `Region`
- Applicant #: `Id` - this is actually a "magic column". If it is specified (and contains a valid applicant #), existing applicants will be updated instead of newly created. This is very handy if you want to bulk-add information to already existing applicants.

## Known problems and shortcomings

- The applicants creation date cannot be set at the moment. It is always the date/time of import.
- History entries are not very nice (or missing completely) at the moment:
    - Base information (name, gender, e-mail) are not displayed in history (they are set correctly tho).
    - Photos are displayed as [-] instead of [+].
- Imported documents are always public, meaning they are visible on the applicant portal.

## Usage
`eRecruiter.ApplicantImport.exe --config=your_config_file.json --file=your_csv_file.csv [--continueOnWarnings]`

## Configuration file format (JSON)

```
{
	Api: {
		Endpoint: "https://your_api_endpoint",
		MandatorId: your_mandator_id,
		Key: "your_api_key"
	},
	
	Columns: [
		{
			Header: "Vorname",
			Type: "FirstName"
		},
		{
			Header: "Nachname",
			Type: "LastName"
		},
		{
			Header: "Lebenslauf",
			Type: "CV"
		},
		{
			Header: "Zeugnisse",
			Type: "Document",
			AdditionalType: "Zeugnis"
		}
	]	
}
```

## CSV file format
Use UTF8 and TAB-seperated CSV.

```
Vorname	Nachname	Lebenslauf	Zeugnisse
Hannes	Sachsenhofer	c:\lebenslauf_sachsenhofer.pdf	c:\zeugnisse\sachsenhofer
Michael	Mittermair		c:\zeugnisse\mittermair
```

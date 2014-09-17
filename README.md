# eRecruiter.ApplicantImport

Import applicant profiles from a CSV file into eRecruiter.

## Supported applicant attributes

- First name: `FirstName` - must never be empty
- Last name: `LastName` - must never be empty
- E-Mail: `Email`
- Phone: `Phone`
- Mobile phone: `MobilePhone`
- Street (address): `Street`
- ZIP code: `ZipCode`
- City (address): `City`
- CV: `Cv` - must be empty or a path to an existing file
- Photo (applicant portrait): `Photo` - must be empty or a path to an existing file

## Known problems and shortcomings

- The applicants creation date cannot be set at the moment. It is always the date/time of import.
- History entries are not very nice (or missing completely) at the moment.

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
		}
	]	
}
```

## CSV file format
Use UTF8 and TAB-seperated CSV.

```
Vorname	Nachname
Hannes	Sachsenhofer
Michael	Mittermair
```

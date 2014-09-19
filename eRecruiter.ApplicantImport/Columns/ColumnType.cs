
namespace eRecruiter.ApplicantImport.Columns
{
    public enum ColumnType
    {
        FirstName,
        LastName,
        Email,
        Phone,
        MobilePhone,
        Street,
        ZipCode,
        City,
        Cv,
        Photo,
        Document,
        /* CustomField, not possible yet, because there's no API method to get available custom fields at the moment */
        Gender
    }
}

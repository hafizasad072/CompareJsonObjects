namespace JsonDiff
{
    public class AuditFields
    {
        public AuditFields(string fieldName, string preChangeValue, string postChangeValue)
        {
            FieldName = fieldName;
            PreChangeValue = preChangeValue;
            PostChangeValue = postChangeValue;
        }
        public string FieldName { get; set; } //Property Name of Entity
        public string PreChangeValue { get; set; } //Optional value before change (eg Status or Program)
        public string PostChangeValue { get; set; } //Optional value after change (eg Status or Program)
    }
}

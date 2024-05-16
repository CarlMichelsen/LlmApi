namespace Api.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BasicAuthAttribute(bool admin) : Attribute
{
    public bool Admin => admin;
}

namespace WIAD_DemoConsole;
public class Person
{
    public int Id { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public int Age { get; set; }
    public Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        
    }
    
    public string FullName()
    {
        return $"{FirstName} {LastName} {Age}";
    }
}
